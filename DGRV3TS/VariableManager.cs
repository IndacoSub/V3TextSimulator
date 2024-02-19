using System.Diagnostics;
using static OfficeOpenXml.ExcelErrorValue;
using static System.Collections.Specialized.BitVector32;

namespace DGRV3TS
{
	internal class VariableManager
	{
		// Don't worry about this if you're not me
		private bool AltNames;

		public List<ListBox> ListBoxes;

		public ListBox Menu;

		public string NoVarStr = "No Variables";

		// Definition, Value, Comment
		public List<VariableEntry> Variables;

		static int MaxRecursive = 3;

		public class VariableEntry
		{
			public string Definition;
			public string Value;
			public string Comment;
		}

		public VariableManager(bool alt_vars)
		{
			Init(alt_vars);
		}

		public void Init(bool alt_vars)
		{
			AltNames = new bool();
			AltNames = alt_vars;

			AddVariables();
			ChangeItemNames();
		}

		public int GetIndexInListbox(ListBox ll, string str)
		{
			int cont = 0;
			foreach (string s in ll.Items)
			{
				if (s == str)
				{
					return cont;
				}

				cont++;
			}

			return -1;
		}

		public void AddV(string var, string value, string comment)
		{
			VariableEntry ve = new VariableEntry();
			ve.Definition = var;
			ve.Value = value;
			ve.Comment = comment;
			Variables.Add(ve);
		}

		public void AddVariantVWithPriority(string var, string value, string comment)
		{
			VariableEntry ve = new VariableEntry();
			ve.Definition = var;
			ve.Value = value;
			ve.Comment = comment;

			Variables.Insert(IndexOfVariableByRaw(var), ve);
		}

		public int IndexOfVariableByRaw(string raw)
		{
			int cont = 0;
			foreach (var iable in Variables)
			{
				if (iable.Definition == raw)
				{
					return cont;
				}
			}

			return cont;
		}

		public string NameByLine(string str)
		{
			string value = str;
			int comma_index = str.IndexOf(",");

			if (comma_index >= 0)
			{
				value = str.Substring(0, comma_index);
			}

			while (value.StartsWith(" "))
			{
				value = value.Substring(1);
			}

			while (value.EndsWith(" "))
			{
				value = value.Substring(0, value.Length - 1);
			}

			return value;
		}

		public string ValueByLine(string str, bool alt)
		{
			string ret = "";

			int colon_index = str.IndexOf(": ");
			bool has_colon = colon_index >= 0;
			int comma_index = str.IndexOf(", ");
			bool has_comma = comma_index >= 0;

			if (AltNames)
			{
				if (has_colon && colon_index > comma_index)
				{
					ret = str.Substring(colon_index + 2);
				}
				else
				{
					ret = str.Substring(comma_index + 2);
				}
			}
			else
			{
				if (alt)
				{
					ret = str.Substring(comma_index + 2);
					has_colon = ret.Contains(": ");
					if (has_colon && colon_index > comma_index)
					{
						colon_index = ret.IndexOf(": ");
						ret = ret.Substring(0, colon_index);
					}
				}
				else
				{
					ret = str.Substring(comma_index + 2);
				}
			}

			int pipe_index = ret.IndexOf('|');
			bool has_pipe = pipe_index > 0 && !ret.EndsWith('|') && !ret.EndsWith("| ");
			if (has_pipe)
			{
				ret = ret.Substring(0, pipe_index - 1);
			}

			while (ret.StartsWith(" "))
			{
				ret = ret.Substring(1);
			}
			while (ret.EndsWith(" "))
			{
				ret = ret.Substring(0, ret.Length - 1);
			}

			return ret;
		}

		public string CommentByLine(string line)
		{
			int pipe_index = line.IndexOf('|');
			int comma_index = line.IndexOf(',');
			bool has_pipe = pipe_index > 0;
			bool has_comment = has_pipe && pipe_index > comma_index && !line.EndsWith('|') && !line.EndsWith("| ");
			if (!has_comment)
			{
				return "";
			}

			string ret = line.Substring(pipe_index + 2);

			while (ret.StartsWith(" "))
			{
				ret = ret.Substring(1);
			}
			while (ret.EndsWith(" "))
			{
				ret = ret.Substring(0, ret.Length - 1);
			}

			return ret;
		}

		// Don't worry about this if you're not me
		public bool IsAllIceCompatible(string str)
		{
			return str.IndexOf(" : ") >= 0 && !string.IsNullOrWhiteSpace(str);
		}

		public void AddVariables()
		{
			Menu = new ListBox();
			Variables = new List<VariableEntry>();
			ListBoxes = new List<ListBox>();

			string vars_file = FileManager.GetCurrentDirectory();
			vars_file = Path.Combine(vars_file, "vars_bak.txt");    // Hardcoded

			// The variables' file is "vars_bak.txt"

			if (!File.Exists(vars_file))
			{
				return;
			}

			bool donefirst = false;
			int catnum = -1;
			var lines = File.ReadLines(vars_file);
			foreach (var line in lines)
			{

				if (string.IsNullOrWhiteSpace(line) || line.Length <= 0 || line == "\n")
				{
					continue;
				}

				string ln = line;

				if (ln.StartsWith("//"))
				{
					if (!donefirst)
					{
						donefirst = true;
						continue;
					}

					catnum++;
					string catname = ln.Substring(3); // // + space
					AddMenu(catname);
					ListBoxes.Add(new ListBox());
					ListBoxes[catnum] = new ListBox();
					continue;
				}

				while (ln.EndsWith(' '))
				{
					ln = ln.Substring(0, ln.Length - 1);
				}

				while (ln.StartsWith(' '))
				{
					ln = ln.Substring(1);
				}

				if (ln.Length <= 0)
				{
					continue;
				}

				string name = "";
				string value = "";
				string comment = "";

				int comma_index = ln.IndexOf(',');
				bool has_comma = comma_index >= 0;

				var raw = NameByLine(ln);
				name = raw;

				if (name.Contains("_MN"))
				{
					continue;
				}

				if (name.Contains("_MS"))
				{
					continue;
				}

				if (has_comma && !ln.Contains("SIGNAL") && !ln.Contains("PLATFORM"))
				{
					// Has a value
					bool is_all_ice = IsAllIceCompatible(ln);
					value = ValueByLine(ln, is_all_ice);
				}
				else
				{
					// No value? Value is the name
					value = name;
				}

				int pipe_index = ln.IndexOf('|');
				bool has_pipe = pipe_index >= 0;
				bool has_comment = has_pipe && pipe_index > comma_index && !ln.EndsWith('|') && !ln.EndsWith("| ");

				if (has_comment)
				{
					comment = CommentByLine(ln);
				}

				if (name.Length > 0)
				{
					if (!name.StartsWith("MAKE_"))
					{
						AddV(name, value, comment);
						ListBoxes[catnum].Items.Add(value);
					}
				}
			}

			foreach (var iables in Variables)
			{
				iables.Value = this.ReplaceVars(iables.Definition);
			}
		}

		public void AddMenu(string str)
		{
			Menu.Items.Add(str);
		}

		public static string ReplaceCLTs(string s)
		{
			string replaced = s;

			replaced = replaced.Replace("<CLT=size1>", "");
			replaced = replaced.Replace("<CLT=size1.1>", "");
			replaced = replaced.Replace("<CLT=size1.2>", "");
			replaced = replaced.Replace("<CLT=size1.5>", "");
			replaced = replaced.Replace("<CLT=size2>", "");
			replaced = replaced.Replace("<CLT=size2.2>", "");
			replaced = replaced.Replace("<CLT=size3>", "");
			replaced = replaced.Replace("<CLT=size4>", "");
			replaced = replaced.Replace("<CLT=size6>", "");
			replaced = replaced.Replace("<CLT=cltMIND>", "");
			replaced = replaced.Replace("<CLT=cltSTRONG>", "");
			replaced = replaced.Replace("<CLT=cltNORMAL>", "");
			replaced = replaced.Replace("<CLT=typeNORMAL>", "");
			replaced = replaced.Replace("<CLT=cltSYSTEM>", "");
			replaced = replaced.Replace("<CLT=cltWEAK>", "");
			replaced = replaced.Replace("<CLT=cltAGREE>", "");

			replaced = replaced.Normalize();

			return replaced;
		}

		public static string ReplaceSignals(string s)
		{
			string replaced = s;

			if (replaced.Contains("<SIGNAL_NDR>"))
			{
				int index = replaced.IndexOf("<SIGNAL_NDR>");
				replaced = replaced.Substring(0, index);
			}

			if (replaced.Contains("<SIGNAL_ALT>"))
			{
				int index = replaced.IndexOf("<SIGNAL_ALT>");
				replaced = replaced.Substring(0, index);
			}

			replaced = replaced.Replace("<PLATFORM_PC>", "");
			replaced = replaced.Replace("<PLATFORM_CONSOLE>", "");

			replaced = replaced.Replace("<SIGNAL_NOSWAP>", "");
			replaced = replaced.Replace("<SIGNAL_CBL>", "");
			replaced = replaced.Replace("<SIGNAL_OE>", "");

			replaced = replaced.Replace("<SIGNAL_AutoTL>", "");

			replaced = replaced.Normalize();

			return replaced;
		}

		public void ChangeItemNames()
		{
			for (int j = 0; j < Menu.Items.Count; j++)
			{
				Menu.Items[j] += " [" + ListBoxes[j].Items.Count + "]";
			}
		}

		public VariableEntry EntryByDefinition(string definition)
		{
			if (definition == null)
			{
				return null;
			}

			foreach (VariableEntry tp in this.Variables)
			{
				if (tp.Definition == definition)
				{
					return tp;
				}
			}

			return null;
		}

		public (VariableEntry, int) EntryByValue(string value)
		{
			if (value == null)
			{
				return (null, 9999);
			}

			int count = 0;
			VariableEntry first = null;
			foreach (VariableEntry tp in this.Variables)
			{
				string str = tp.Definition;
				var solved = this.ReplaceVars(str);
				if (solved == value)
				{
					if (first == null)
					{
						first = tp;
					}
					count++;
				}
				if (count > 1)
				{
					break;
				}
			}

			return (first, count);
		}

		public string ReplaceVars(string replaced)
		{
			// Replace variables using the VariableManager

			int count = 0;
			bool cond = false;

			do
			{
				List<string> contained = new List<string>();

				foreach (VariableEntry tp in this.Variables)
				{
					if (replaced.Contains(tp.Definition))
					{
						contained.Add(tp.Definition);
					}

					if (!tp.Definition.Contains("MAKE_") || !replaced.Contains("MAKE_"))
					{
						continue;
					}

					int section_start = replaced.IndexOf("MAKE_");
					string section = replaced.Substring(section_start);
					int section_end = replaced.IndexOf(")");
					section = section.Substring(0, section_end);
					int find = replaced.IndexOf("(");
					string value = replaced.Substring(find + 1);
					find = value.IndexOf(")");
					value = value.Substring(0, find);

					replaced = replaced.Replace(section, tp.Value.Replace("MY_ARG", value));
				}

				for (int i = 0; i < contained.Count; i++)
				{
					if (contained[i].Length <= 0)
					{
						continue;
					}

					if (contained[i].StartsWith("<CLT"))
					{
						continue;
					}

					var entry = EntryByDefinition(contained[i]);
					if (entry == null || entry.Value == null || entry.Value.Length == 0)
					{
						continue;
					}

					replaced = replaced.Replace(contained[i], entry.Value);
				}

				count++;
				cond = (replaced.Contains("MAKE_") || replaced.Contains("VAR_")) && count < VariableManager.MaxRecursive;
			} while (cond);

			return replaced.Replace("  ", " ");
		}
	}
}