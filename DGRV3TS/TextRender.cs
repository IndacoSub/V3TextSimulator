namespace DGRV3TS
{
	partial class Operations
	{

		private List<string> DivideStringByCLT(string text)
		{
			// Divide a string from CLTs
			List<string> strings = new List<string>();

			const string clt = "<CLT";
			var t = text;
			while (t.IndexOf(clt) != -1)
			{
				var found = t.IndexOf(clt);
				if (found != -1)
				{
					found++;
					var found2 = t.IndexOf(">", found);
					if (found2 == -1)
					{
						break;
					}

					found2++;
					string cltstr = t.Substring(found - 1, found2 - found + 1);
					string cropped = t.Substring(0, found - 1);
					strings.Add(cropped);
					strings.Add(cltstr);
					t = t.Remove(0, found2);
				}
				else
				{
					break;
				}
			}

			strings.Add(t);

			return strings;
		}


		// What does most of this do? I don't know! I don't remember!
		private List<string> UNK1(List<string> strings)
		{
			// I really don't know what I was thinking (1/2)
			List<string> strings2 = new List<string>();

			for (int j = 0; j < strings.Count; j++)
			{
				string s = strings[j];
				int next = -1;
				for (int i = j + 1; i < strings.Count; i++)
				{
					if (strings[i].StartsWith("<"))
					{
						next = i;
						break;
					}
				}

				if (next == -1)
				{
					for (int i = j + 1; i < strings.Count; i++)
					{
						s += strings[i];
					}

					strings2.Add(s);
					break;
				}

				for (int i = j + 1; i < next; i++)
				{
					s += strings[i];
				}

				j += next - j - 1;
				strings2.Add(s);
			}

			return strings2;
		}


		// What does most of this do? I don't know! I don't remember!
		private List<string> UNK2(List<string> strings2)
		{
			// I really don't know what I was thinking (2/2)
			List<string> string3 = new List<string>();

			for (int j = 0; j < strings2.Count; j++)
			{
				while (strings2[j].IndexOf("\n") != -1)
				{
					var found = strings2[j].IndexOf('\n');
					if (found != -1)
					{
						string cropped = strings2[j].Substring(0, found + 1);
						string3.Add(cropped);
						strings2[j] = strings2[j].Remove(0, found + 1);
					}
					else
					{
						break;
					}
				}

				string3.Add(strings2[j]);
			}

			return string3;
		}

		private float GetNewlineHeight()
		{

			float ret = -10.0f;
			switch (CurrentGameIndex)
			{
				case GameIndex.AI:
					switch (CB_TB.GetItemText(CB_TB.Items[CB_TB.SelectedIndex]))
					{
						case "AI_Tutorial":
							ret = 41.0f;
							break;
						case "ToWitter":
							ret = 11.0f;
							break;
						default:
							ret = 41.0f;
							break;
					}
					break;
				default:
					ret = 41.0f;
					break;
			}
			return ret;
		}

		// Should probably be renamed
		private Image DrawText3(string text, Font font, Color backColor)
		{
			float posx = 0;
			float posy = 0;

			bool find_signal_autotl = text.Contains("<SIGNAL_AutoTL>");
			if (find_signal_autotl)
			{
				// Remove it
				int index = text.IndexOf("<SIGNAL_AutoTL>");
				text = text.Substring(0, index);
			}

			List<string> strings = DivideStringByCLT(text);
			List<string> string2 = UNK1(strings);
			List<string> string3 = UNK2(string2);
			var result = new Bitmap(1305, 900); // TODO: Why was this 1305x900?
			using (var g = Graphics.FromImage(result))
			{
				g.Clear(backColor);
				// Load the font initially
				fm.LoadByCSFont(font.FontFamily.Name, fm.FontSize, font.Style);
				foreach (string s in string3)
				{
					string str = s;

					bool contains_clt = fm.ChangeBasedOnCLT(str);

					if (CheckboxReplaceVariables.Checked && contains_clt)
					{
						str = vm.ReplaceCLTs(str);
					}

					if (str.Length == 0)
					{
						continue;
					}

					// If it's a newline, set the X to zero and increase the Y
					// Obviously don't draw it
					if (str == "\n" || str == "\\n")
					{
						posx = 0.0f;
						posy += CheckboxMaybeAccurateHeight.Checked
							? g.MeasureString(str, fm.CurrentFont).Height
							: GetNewlineHeight();
						continue;
					}

					// Mark autotranslations with pink color
					if (find_signal_autotl)
					{
						fm.CurrentColor = Color.HotPink;
					}

					// Actually load the font this time
					fm.LoadByCSFont(font.FontFamily.Name, fm.CurrentFontSize, font.Style);
					SolidBrush sb = new SolidBrush(fm.CurrentColor);

					g.DrawString(str, fm.CurrentFont, sb, posx, posy);

					float addx = g.MeasureString(str, fm.CurrentFont).Width;
					if (contains_clt && CheckboxReplaceVariables.Checked)
					{
						// I don't know

						// WORKAROUND
						//addx -= g.MeasureString(" ", fm.CurrentFont).Width;
					}

					// Unfortunately the width and height of the string needs to be calculated manually
					posx += addx;

					// If it ends with a newline *but is not a newline*
					if (str.EndsWith("\n") || str.EndsWith("\\n"))
					{
						posx = 0.0f;
						posy += CheckboxMaybeAccurateHeight.Checked
							? g.MeasureString(str, fm.CurrentFont).Height
							: GetNewlineHeight();
					}
				}

				g.Save();

				fm.CurrentColor = Color.White;
			}

			return result;
		}
	}
}