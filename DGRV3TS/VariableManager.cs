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
		public List<Tuple<string, string, string>> Variables;

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

		// Get the Value from the Definition
		public string SolveVar(string var)
		{
			foreach (Tuple<string, string, string> tp in Variables)
			{
				if (tp.Item1 == var)
				{
					return tp.Item2;
				}
			}

			// for now
			// return "UnknownValue";
			return var;
		}


		// Get the Comment from the Definition
		public string CommentFromDefinition(string var)
		{
			foreach (Tuple<string, string, string> tp in Variables)
			{
				if (tp.Item1 == var)
				{
					return tp.Item3;
				}
			}

			// for now
			// return "UnknownValue";
			return "";
		}


		// Get the Definition from the Value
		public Tuple<string, bool> UnSolve(string str, bool verbose = true)
		{
			//string ret = str; // "UnknownVar";
			int howmany = 0;
			string ret = str;
			bool ambiguous = false;

			if (str == null || str.Length <= 0)
			{
				return new Tuple<string, bool>(ret, ambiguous);
			}

			foreach (Tuple<string, string, string> tp in Variables)
			{
				if (tp.Item2 == str)
				{
					ret = tp.Item1;
					howmany++;
				}
			}

			if (howmany > 1)
			{
				ambiguous = true;
				if (verbose)
				{
					InputManager.Print("More than 1 found! The string may be incorrect.");
				}
			}

			// for now
			//return "UnknownVar";
			return new Tuple<string, bool>(ret, ambiguous);
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
			Variables.Add(new Tuple<string, string, string>(var, value, comment));
		}

		public void AddVariantVWithPriority(string var, string value, string comment)
		{
			Variables.Insert(IndexOfVariableByRaw(var), new Tuple<string, string, string>(var, value, comment));
		}

		public int IndexOfVariableByRaw(string raw)
		{
			int cont = 0;
			foreach (var iable in Variables)
			{
				if (iable.Item1 == raw)
				{
					return cont;
				}
			}

			return cont;
		}

		public string NameByLine(string str)
		{
			string value = "";
			int comma_index = str.IndexOf(",");

			if (comma_index >= 0)
			{
				value = str.Substring(0, comma_index);
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
			if(has_pipe)
			{
				ret = ret.Substring(0, pipe_index - 1);
			}

			return ret;
		}

		public string CommentByLine(string line)
		{
			int pipe_index = line.IndexOf('|');
			int comma_index = line.IndexOf(',');
			bool has_pipe = pipe_index > 0;
			bool has_comment = has_pipe && pipe_index > comma_index && !line.EndsWith('|') && !line.EndsWith("| ");
			if(!has_comment)
			{
				return "";
			}

			string ret = line.Substring(pipe_index + 2);
			return ret;
		}

		// Don't worry about this if you're not me
		public bool IsAllIceCompatible(string str)
		{
			return str.IndexOf(" : ") >= 0 && !string.IsNullOrWhiteSpace(str);
		}

		public void AddVariables()
		{
			Variables = new List<Tuple<string, string, string>>();
			Menu = new ListBox();

			ListBoxes = new List<ListBox>();

			string vars_file = FileManager.GetCurrentDirectory();
			vars_file = Path.Combine(vars_file, "vars_bak.txt");	// Hardcoded

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
				if (string.IsNullOrWhiteSpace(line))
				{
					continue;
				}

				if (line.StartsWith("//"))
				{
					if (!donefirst)
					{
						donefirst = true;
						continue;
					}

					catnum++;
					string catname = line.Substring(3); // // + space
					AddMenu(catname);
					ListBoxes.Add(new ListBox());
					ListBoxes[catnum] = new ListBox();
					continue;
				}

				if (line.Contains("_MN"))
				{
					continue;
				}

				if (line.Contains("_MS"))
				{
					continue;
				}

				string name = "";
				string value = "";
				string comment = "";

				int comma_index = line.IndexOf(',');
				bool has_comma = comma_index >= 0;

				var raw = NameByLine(line);
				name = raw;

				if (has_comma && !line.Contains("SIGNAL") && !line.Contains("PLATFORM"))
				{
					// Has a value
					bool is_all_ice = IsAllIceCompatible(line);
					value = ValueByLine(line, is_all_ice);
				}
				else
				{
					// No value? Value is the name
					value = name;
				}

				int pipe_index = line.IndexOf('|');
				bool has_pipe = pipe_index >= 0;
				bool has_comment = has_pipe && pipe_index > comma_index && !line.EndsWith('|') && !line.EndsWith("| ");

				if (has_comment)
				{
					comment = CommentByLine(line);
				}

				AddV(name, value, comment);

				ListBoxes[catnum].Items.Add(value);
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

        public string ReplaceVars(string replaced)
        {
            // Replace variables using the VariableManager

            List<string> contained = new List<string>();

            foreach (Tuple<string, string, string> tp in this.Variables)
            {
                if (replaced.Contains(tp.Item1))
                {
                    contained.Add(tp.Item1);
                }
            }

            foreach (string sc in contained)
            {
				if(sc.Length <= 0)
				{
					continue;
				}

                if (sc.StartsWith("<CLT"))
                {
                    continue;
                }

                replaced = replaced.Replace(sc, this.SolveVar(sc));
            }

            return replaced;
        }
    }
}