namespace DGRV3TS
{
	internal class VariableManager
	{
		// Don't worry about this if you're not me
		private bool AltConvention;

		public List<ListBox> ListBoxes;

		public ListBox Menu;

		public string NoVarStr = "No Variables";

		// Definition, Value
		public List<Tuple<string, string>> Variables;

		public VariableManager(bool ic)
		{
			Init(ic);
		}

		public void Init(bool ic)
		{
			AltConvention = new bool();
			AltConvention = ic;

			AddVariables();
			ChangeItemNames();
		}

		// Get the Value from the Definition
		public string SolveVar(string var)
		{
			foreach (Tuple<string, string> tp in Variables)
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

		// Get the Definition from the Value
		public string UnSolve(string str)
		{
			//string ret = str; // "UnknownVar";
			int howmany = 0;
			string ret = str;
			foreach (Tuple<string, string> tp in Variables)
			{
				if (tp.Item2 == str)
				{
					ret = tp.Item1;
					howmany++;
				}
			}

			if (howmany > 1)
			{
				InputManager.Print("More than 1 found! The copied string may not be correct.");
			}

			// for now
			//return "UnknownVar";
			return ret;
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

		public void AddV(string var, string value)
		{
			Variables.Add(new Tuple<string, string>(var, value));
		}

		public void AddVariantVWithPriority(string var, string value)
		{
			Variables.Insert(IndexOfVariableByRaw(var), new Tuple<string, string>(var, value));
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

		public string RawByLine(string str)
		{
			if (str.IndexOf(",") >= 0)
			{
				return str.Substring(0, str.IndexOf(","));
			}

			return str;
		}

		public string ValueByLine(string str, bool alt)
		{
			string ret = "";

			if (AltConvention)
			{
				ret = alt ? str.Substring(str.IndexOf(": ") + 2) : str.Substring(str.IndexOf(", ") + 2);
			}
			else
			{
				if (alt)
				{
					ret = str.Substring(str.IndexOf(", ") + 2);
					ret = ret.Substring(0, ret.IndexOf(" :"));
				}
				else
				{
					ret = str.Substring(str.IndexOf(", ") + 2);
				}
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
			Variables = new List<Tuple<string, string>>();
			Menu = new ListBox();

			ListBoxes = new List<ListBox>();

			// The variables' file is "vars_bak.txt"

			if (!File.Exists("vars_bak.txt"))
			{
				return;
			}

			bool donefirst = false;
			int catnum = -1;
			var lines = File.ReadLines("vars_bak.txt");
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

				string raw = "";
				string value = "";

				raw = RawByLine(line);
				if (line.IndexOf(",") >= 0 && !line.Contains("SIGNAL") && !line.Contains("PLATFORM"))
				{
					bool is_all_ice = IsAllIceCompatible(line);
					value = ValueByLine(line, is_all_ice);
				}
				else
				{
					value = raw;
				}

				AddV(raw, value);

				ListBoxes[catnum].Items.Add(value);
			}
		}

		public void AddMenu(string str)
		{
			Menu.Items.Add(str);
		}

		public string ReplaceCLTs(string s)
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

		public string ReplaceSignals(string s)
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
	}
}