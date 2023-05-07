namespace DGRV3TS
{
	// A dictionary for AutoTranslation based on previous translations

	internal class Variant
	{
		public string File = "";
		public string Line = "";
		public string Message = "";
	}

	internal class Suggestion
	{
		public string OriginalLine = "";
		public List<Variant> Variants = new List<Variant>();
	}

	internal class TLDictionary
	{
		private readonly int MinimumLength = 1; // Arbitrary number
		private List<Suggestion> Suggestions = new List<Suggestion>();

		private Dictionary<string, int> DuplicateCount = new Dictionary<string, int>();

		public Suggestion GetSuggestionFromString(string ogline)
		{
			return Suggestions.FirstOrDefault(s => s.OriginalLine == ogline);
		}

		public void ScanDirectory(string curfile, string dir, FileManager fm)
		{
			Suggestions = new List<Suggestion>();
			DuplicateCount = new Dictionary<string, int>();

			const bool disallow_autotl_suggestions = false;
			string[] files = null;

			// Only file formats that contain both the original and the translation are supported
			switch (fm.Type)
			{
				case FileManager.LoadedFileType.Vo:
					files = Directory.GetFiles(dir, "*.vo", SearchOption.AllDirectories);
					break;
				case FileManager.LoadedFileType.Po:
					files = Directory.GetFiles(dir, "*.po", SearchOption.AllDirectories);
					break;
				case FileManager.LoadedFileType.Xlsx:
					files = Directory.GetFiles(dir, "*.xlsx", SearchOption.AllDirectories);
					break;
			}

			if (files == null || files.Length <= 0)
			{
				return;
			}

			foreach (string file in files)
			{
				if (curfile == file)
				{
					continue;
				}

				// Remove duplicates if present
				Suggestions = CleanupDuplicates(Suggestions);

				if (fm.Type == FileManager.LoadedFileType.Po)
				{
					// Read the current file if it is a .po
					fm.PoInfo = new List<string>();
					fm.PoList = new List<PoInternal>();
					fm.ReadPo(file, false);

					// Read each line individually
					foreach (PoInternal po in fm.PoList)
					{
						// Has the line already been autotranslated?
						bool is_autotl = po.MessageString.Contains("AutoTL");
						int tl_index = po.MessageString.Length;
						if (is_autotl)
						{
							// What?
							tl_index = po.MessageString.IndexOf("<SIGNAL_AutoTL>");
						}

						// Remove signals from the string in memory
						bool contains_signal = po.MessageString.Contains("<SIGNAL=");
						if (contains_signal)
						{
							int index = po.MessageString.IndexOf("<SIGNAL=");
							if (index > 0)
							{
								po.MessageString = po.MessageString.Substring(0, index);
							}
						}

						if (po.MessageString == null || po.MessageString.Length <= 0 ||
							po.OriginalMessage.Length <= MinimumLength)
						{
							continue;
						}

						if (disallow_autotl_suggestions && is_autotl)
						{
							continue;
						}

						// Has this line ever been translated before?
						bool already_contains = Suggestions.Any(s => s.OriginalLine == po.OriginalMessage);
						if (already_contains)
						{
							// If it has been translated before,
							// check every translation possible
							foreach (Suggestion sg in Suggestions)
							{
								if (sg.OriginalLine != po.OriginalMessage)
								{
									continue;
								}

								Variant v = new Variant();
								v.Message = is_autotl ? po.MessageString.Substring(0, tl_index) : po.MessageString;

								v.Line = po.LineNumber;
								v.File = po.OriginFile;
								sg.Variants.Add(v);

								if (DuplicateCount.ContainsKey(v.Message))
								{
									++DuplicateCount[v.Message];
								}
								else
								{
									DuplicateCount.Add(v.Message, 1);
								}
							}
						}
						else
						{
							// If it hasn't been translated before
							// just add it I guess
							Suggestion sg = new Suggestion();
							sg.OriginalLine = po.OriginalMessage;
							sg.Variants = new List<Variant>();
							Variant vv = new Variant();
							vv.Line = po.LineNumber;
							vv.File = po.OriginFile;
							vv.Message = is_autotl ? po.MessageString.Substring(0, tl_index) : po.MessageString;
							sg.Variants.Add(vv);
							Suggestions.Add(sg);

							if (DuplicateCount.ContainsKey(vv.Message))
							{
								++DuplicateCount[vv.Message];
							}
							else
							{
								DuplicateCount.Add(vv.Message, 1);
							}
						}
					}
				}
				else
				{
					if (fm.Type == FileManager.LoadedFileType.Vo)
					{
						// Pretty much same as above but with Vo files

						fm.VoInfo = new List<string>();
						fm.VoList = new List<VoInternal>();
						fm.ReadVo(file, false);
						foreach (VoInternal vo in fm.VoList)
						{
							bool is_autotl = vo.Translations[vo.TranslationNumber].Contains("AutoTL");
							int tl_index = vo.Translations[vo.TranslationNumber].Length;
							if (is_autotl)
							{
								tl_index = vo.Translations[vo.TranslationNumber].IndexOf("<SIGNAL_AutoTL>");
							}
							bool contains_signal = vo.Translations[vo.TranslationNumber].Contains("<SIGNAL");
							if (contains_signal)
							{
								int index = vo.Translations[vo.TranslationNumber].IndexOf("<SIGNAL");
								if (index > 0)
								{
									vo.Translations[vo.TranslationNumber] =
										vo.Translations[vo.TranslationNumber].Substring(0, index);
								}
							}

							if (vo.Translations[vo.TranslationNumber] == null ||
								vo.Translations[vo.TranslationNumber].Length <= 0 ||
								vo.Translations[vo.TranslationNumber].Length <= MinimumLength)
							{
								continue;
							}

							if (disallow_autotl_suggestions && is_autotl)
							{
								continue;
							}

							bool already_contains = Suggestions.Any(s => s.OriginalLine == vo.OriginalMessage);
							if (already_contains)
							{
								foreach (Suggestion sg in Suggestions)
								{
									if (sg.OriginalLine != vo.OriginalMessage)
									{
										continue;
									}

									Variant v = new Variant();
									v.Message = is_autotl ? vo.Translations[vo.TranslationNumber].Substring(0, tl_index) : vo.Translations[vo.TranslationNumber];

									v.Line = vo.LineNumber.ToString();
									v.File = vo.OriginFile;
									sg.Variants.Add(v);

									if (DuplicateCount.ContainsKey(v.Message))
									{
										++DuplicateCount[v.Message];
									}
									else
									{
										DuplicateCount.Add(v.Message, 1);
									}
								}
							}
							else
							{
								Suggestion sg = new Suggestion();
								sg.OriginalLine = vo.OriginalMessage;
								sg.Variants = new List<Variant>();
								Variant vv = new Variant();
								vv.Line = vo.LineNumber.ToString();
								vv.File = vo.OriginFile;
								vv.Message = is_autotl ? vo.Translations[vo.TranslationNumber].Substring(0, tl_index) : vo.Translations[vo.TranslationNumber];
								sg.Variants.Add(vv);
								Suggestions.Add(sg);

								if (DuplicateCount.ContainsKey(vv.Message))
								{
									++DuplicateCount[vv.Message];
								}
								else
								{
									DuplicateCount.Add(vv.Message, 1);
								}
							}
						}
					}
					else
					{
						if (fm.Type == FileManager.LoadedFileType.Xlsx)
						{

							// Pretty much same as above but with Xlsx files
							fm.XLSXList.Clear();
							fm.ReadXLSX(file, false);
							int cont_columns = 0;
							foreach (XLSXRow xlsx in fm.XLSXList)
							{
								bool is_autotl = xlsx.Translations[fm.SelectedLanguage].Contains("AutoTL");
								int tl_index = xlsx.Translations[fm.SelectedLanguage].Length;
								if (is_autotl)
								{
									tl_index = xlsx.Translations[fm.SelectedLanguage].IndexOf("<SIGNAL_AutoTL>");
								}

								bool contains_signal = xlsx.Translations[fm.SelectedLanguage].Contains("<SIGNAL");
								if (contains_signal)
								{
									int index = xlsx.Translations[fm.SelectedLanguage].IndexOf("<SIGNAL");
									if (index > 0)
									{
										xlsx.Translations[fm.SelectedLanguage] =
											xlsx.Translations[fm.SelectedLanguage].Substring(0, index);
									}
								}

								if (xlsx.Translations[fm.SelectedLanguage] == null ||
									xlsx.Translations[fm.SelectedLanguage].Length <= 0 ||
									xlsx.Translations[fm.SelectedLanguage].Length <= MinimumLength)
								{
									continue;
								}

								if (disallow_autotl_suggestions && is_autotl)
								{
									continue;
								}

								bool already_contains = Suggestions.Any(s => s.OriginalLine == xlsx.Original);
								if (already_contains)
								{
									foreach (Suggestion sg in Suggestions)
									{
										if (sg.OriginalLine != xlsx.Original)
										{
											continue;
										}

										Variant v = new Variant();
										v.Message = is_autotl ? xlsx.Translations[fm.SelectedLanguage].Substring(0, tl_index) : xlsx.Translations[fm.SelectedLanguage];

										v.Line = cont_columns.ToString();
										v.File = Path.GetFileNameWithoutExtension(fm.LoadedFileName);
										sg.Variants.Add(v);

										if (DuplicateCount.ContainsKey(v.Message))
										{
											++DuplicateCount[v.Message];
										}
										else
										{
											DuplicateCount.Add(v.Message, 1);
										}
									}
								}
								else
								{
									Suggestion sg = new Suggestion();
									sg.OriginalLine = xlsx.Original;
									sg.Variants = new List<Variant>();
									Variant vv = new Variant();
									vv.Line = cont_columns.ToString();
									vv.File = Path.GetFileNameWithoutExtension(fm.LoadedFileName);
									vv.Message = is_autotl ? xlsx.Translations[fm.SelectedLanguage].Substring(0, tl_index) : xlsx.Translations[fm.SelectedLanguage];
									sg.Variants.Add(vv);
									Suggestions.Add(sg);

									if (DuplicateCount.ContainsKey(vv.Message))
									{
										++DuplicateCount[vv.Message];
									}
									else
									{
										DuplicateCount.Add(vv.Message, 1);
									}
								}

								cont_columns++;
							}
						}
					}
				}
			}

			// Remove duplicates one final time
			Suggestions = CleanupDuplicates(Suggestions);
		}

		public List<Suggestion> CleanupDuplicates(List<Suggestion> suggestions)
		{
			// Should the suggestions be sorted?

			foreach (var sg in suggestions)
			{
				sg.Variants = sg.Variants.Distinct().ToList();
				string last = string.Empty;
				for (int j = 0; j < sg.Variants.Count; j++)
				{
					Variant v = sg.Variants[j];
					string s = v.Message;
					if (s == last)
					{
						sg.Variants.RemoveAt(j);
						j = 0;
						if (sg.Variants.Count > 0)
						{
							last = sg.Variants[0].Message;
						}
						continue;
					}
					last = s;
				}
			}
			return suggestions.Distinct().ToList();
		}

		public int CountVariantsWithMessage(string message)
		{
			if (DuplicateCount.TryGetValue(message, out int ret))
			{
				return ret;
			}
			else
			{
				return 0;
			}
		}

		public void ApplySuggestions(FileManager fm, bool trmode)
		{

			// Actually auto-translate

			int cont_translated = 0;
			int len = 0;
			switch (fm.Type)
			{
				case FileManager.LoadedFileType.Vo:
					// How many lines in total?
					len = fm.VoList.Count;
					// For each line
					foreach (VoInternal vi in fm.VoList)
					{
						int index = 0;
						const bool check = true; // Debug option?
						Suggestion gg = GetSuggestionFromString(vi.OriginalMessage);
						if (gg == null || gg == new Suggestion())
						{
							continue;
						}

						// Do checks
						if (check)
						{
							if (vi.OriginalMessage.Length <= MinimumLength)
							{
								continue;
							}

							if (trmode)
							{
								if (vi.Translations[vi.TranslationNumber] != vi.OriginalMessage)
								{
									continue;
								}
							}
							else
							{
								if (vi.Translations[vi.TranslationNumber].Length > 0)
								{
									continue;
								}
							}

							// Should probably be "Any" for most people?
							if (gg.Variants.All(x => x.Message == gg.OriginalLine))
							{
								continue;
							}

							if (gg.Variants.All(x => x.Message == vi.Translations[vi.TranslationNumber]))
							{
								continue;
							}

							if (gg.Variants.Count > 1 && gg.Variants.All(x => x.Message.Length > 0))
							{
								// Prioritize variables or signals
								if (gg.Variants.Any(x => x.Message.Contains("VAR") || x.Message.Contains("SIGNAL")))
								{
									int j = 0;
									foreach (Variant viv in gg.Variants)
									{
										if (viv.Message.Contains("VAR") || viv.Message.Contains("SIGNAL"))
										{
											index = j;
											break;
										}

										j++;
									}
								}
								else
								{
									List<string> choices = new List<string>();
									foreach (Variant vv in gg.Variants)
									{
										int cnt = CountVariantsWithMessage(vv.Message);
										choices.Add("[" + cnt + "] " + vv.File + " -- Line " + vv.Line + " -- : " + vv.Message);
									}

									index = CreateListbox(choices, gg.OriginalLine, vi.Character, vi.LineNumber.ToString());
								}
							}
						}

						if (gg.Variants[index].Message != vi.Translations[vi.TranslationNumber])
						{
							vi.Translations[vi.TranslationNumber] = gg.Variants[index].Message + "<SIGNAL_AutoTL>";
							cont_translated++;
						}
					}
					break;
				case FileManager.LoadedFileType.Po:

					// Same as above but with .po files

					len = fm.PoList.Count;
					foreach (PoInternal pi in fm.PoList)
					{
						int index = 0;
						const bool check = true; // Debug option?
						Suggestion gg = GetSuggestionFromString(pi.OriginalMessage);
						if (gg == null || gg == new Suggestion())
						{
							continue;
						}

						if (check)
						{
							if (pi.OriginalMessage.Length <= MinimumLength)
							{
								continue;
							}

							if (trmode)
							{
								if (pi.MessageString != pi.OriginalMessage)
								{
									continue;
								}
							}
							else
							{
								if (pi.MessageString.Length > 0)
								{
									continue;
								}
							}

							// Should probably be "Any" for most people
							if (gg.Variants.All(x => x.Message == gg.OriginalLine))
							{
								continue;
							}

							if (gg.Variants.All(x => x.Message == pi.MessageString))
							{
								continue;
							}

							if (gg.Variants.Count > 1 && gg.Variants.All(x => x.Message.Length > 0))
							{
								// Priority to variables and signals
								if (gg.Variants.Any(x => x.Message.Contains("VAR") || x.Message.Contains("SIGNAL")))
								{
									int j = 0;
									foreach (Variant viv in gg.Variants)
									{
										if (viv.Message.Contains("VAR") || viv.Message.Contains("SIGNAL"))
										{
											index = j;
											break;
										}

										j++;
									}
								}
								else
								{
									List<string> choices = new List<string>();
									foreach (Variant vv in gg.Variants)
									{
										int cnt = CountVariantsWithMessage(vv.Message);
										choices.Add("[" + cnt + "] " + vv.File + " -- Line " + vv.Line + " -- : " + vv.Message);
									}

									index = CreateListbox(choices, gg.OriginalLine, pi.Character, pi.LineNumber);
								}
							}
						}

						if (gg.Variants[index].Message != pi.MessageString)
						{
							pi.MessageString = gg.Variants[index].Message + "<SIGNAL_AutoTL>";
							cont_translated++;
						}
					}

					break;

				case FileManager.LoadedFileType.Xlsx:

					// Same as above but with .xlsx files

					len = fm.XLSXList.Count;
					int cont_columns = 0;
					foreach (XLSXRow row in fm.XLSXList)
					{
						int index = 0;
						const bool check = true; // Debug option?
						Suggestion gg = GetSuggestionFromString(row.Original);
						if (gg == null || gg == new Suggestion())
						{
							continue;
						}

						if (check)
						{
							if (row.Original.Length <= MinimumLength)
							{
								continue;
							}

							if (trmode)
							{
								if (row.Translations[fm.SelectedLanguage] != row.Original)
								{
									continue;
								}
							}
							else
							{
								/*
								if (row.Translations[fm.SelectedLanguage].Length > 0)
								{
									continue;
								}
                                */
							}

							// Should probably be "Any" for most people
							if (gg.Variants.All(x => x.Message == gg.OriginalLine))
							{
								continue;
							}

							if (gg.Variants.All(x => x.Message == row.Original))
							{
								continue;
							}

							if (gg.Variants.Count > 1 && gg.Variants.All(x => x.Message.Length > 0))
							{
								// Priority to variables and signals
								if (gg.Variants.Any(x => x.Message.Contains("VAR") || x.Message.Contains("SIGNAL")))
								{
									int j = 0;
									foreach (Variant viv in gg.Variants)
									{
										if (viv.Message.Contains("VAR") || viv.Message.Contains("SIGNAL"))
										{
											index = j;
											break;
										}

										j++;
									}
								}
								else
								{
									List<string> choices = new List<string>();
									foreach (Variant vv in gg.Variants)
									{
										int cnt = CountVariantsWithMessage(vv.Message);
										choices.Add("[" + cnt + "] " + vv.File + " -- Line " + vv.Line + " -- : " + vv.Message);
									}

									index = CreateListbox(choices, gg.OriginalLine, "DefaultCharacter", cont_columns.ToString());
								}
							}
						}

						if (gg.Variants[index].Message != row.Translations[fm.SelectedLanguage])
						{
							row.Translations[fm.SelectedLanguage] = gg.Variants[index].Message + "<SIGNAL_AutoTL>";
							cont_translated++;
						}

						cont_columns++;
					}
					break;
			}

			Suggestions = CleanupDuplicates(Suggestions);

			if (cont_translated == len)
			{
				InputManager.Print("Whole file autotranslated!");
			}
		}

		private int CreateListbox(List<string> choices, string original, string ch, string lineno)
		{
			// The user needs to select which translation they want to use, if there are "conflicts"

			int ret = 0;

			ListboxSelect ls = new ListboxSelect();
			foreach (string choice in choices)
			{
				ls.listBox1.Items.Add(choice);
			}

			if (ls.listBox1.Items.Count > 1)
			{
				ls.Text = "Select translation for: \"" + original + "\" said by " + ch + " at line " + lineno;

				ls.listBox1.Refresh();
				ls.Refresh();
				var res = ls.ShowDialog();
				while (res != DialogResult.OK || ls.SelectedElement == ListBox.NoMatches)
				{
					res = ls.ShowDialog();
				}

				ret = ls.SelectedElement;

				ls.Close();
			}

			ls.listBox1.Dispose();
			ls.panel1.Dispose();
			ls.Dispose();

			return ret;
		}
	}
}