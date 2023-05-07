using ExcelDataReader;
using OfficeOpenXml;
using System.Text;

namespace DGRV3TS
{
	internal class FileManager
	{
		public enum LoadedFileType
		{
			Vo,
			Po,
			Txt,
			Stx,
			Xlsx,
		}

		public string LastOpenedFile = "";

		public string LoadedFileName;
		public List<string> PoInfo;

		public List<PoInternal> PoList;
		public int SelectedLanguage;

		public int StringIndex;

		public StxInternal StxFile;
		public bool TxtHasBrackets;

		public List<TxtInternal> TxtList;

		public string TxtSavePath;

		public LoadedFileType Type;
		public List<string> VoInfo;

		public List<VoInternal> VoList;

		public List<XLSXRow> XLSXList;

		public GameIndex GameIndex = GameIndex.V3;

		public FileManager()
		{
			StringIndex = new int();
			StringIndex = 0;

			SelectedLanguage = new int();
			SelectedLanguage = 0;

			VoList = new List<VoInternal>();
			VoInfo = new List<string>();

			PoList = new List<PoInternal>();
			PoInfo = new List<string>();

			TxtList = new List<TxtInternal>();
			TxtHasBrackets = new bool();

			XLSXList = new List<XLSXRow>();

			Type = new LoadedFileType();
		}

		public void ReadVo(string file, bool trmode)
		{
			using (StreamReader reader = File.OpenText(file))
			{
				string t_line = "";
				VoInternal vv = new VoInternal();
				bool found_newline = false;
				bool first = false;
				while ((t_line = reader.ReadLine()) != null)
				{
					if (!first)
					{
						// Complicated but makes sense
						found_newline = !found_newline && t_line.Length == 0;
						if (found_newline)
						{
							first = true;
						}
						else
						{
							if (!VoInfo.Contains(t_line))
							{
								VoInfo.Add(t_line);
							}
						}

						continue;
					}

					if (t_line.Length == 0)
					{
						VoList.Add(vv);
						vv = new VoInternal();
						continue;
					}

					vv.Parse(t_line);
				}
			}

			if (!trmode || VoList.Count <= 0)
			{
				return;
			}

			foreach (VoInternal vv in VoList)
			{
				if (vv.TranslationNumber <= 0)
				{
					continue;
				}

				for (int tl = 0; tl < vv.TranslationNumber; tl++)
				{
					if (vv.OriginalMessageLength <= 0)
					{
						continue;
					}

					if (vv.Translations[tl] == VoInternal.GetBlank())
					{
						vv.Translations[tl] = vv.OriginalMessage;
					}
				}
			}
		}

		public void ReadPo(string file, bool trmode)
		{
			using (StreamReader reader = File.OpenText(file))
			{
				string t_line = "";

				PoInternal pp = new PoInternal();

				pp.GameIndex = GameIndex;

				bool found_newline = false;
				bool first = false;

				while (!reader.EndOfStream)
				{
					t_line = reader.ReadLine();

					if (t_line == null)
					{
						// TODO
						break;
					}

					if (!first)
					{
						found_newline = !found_newline && t_line.Length == 0;
						if (found_newline)
						{
							first = true;
						}
						else
						{
							if (!PoInfo.Contains(t_line))
							{
								PoInfo.Add(t_line);
							}
						}

						continue;
					}

					if (t_line.Length == 0)
					{
						PoList.Add(pp);
						pp = new PoInternal();
						pp.GameIndex = GameIndex;
						continue;
					}

					pp.Parse(t_line);
				}

				if (!string.IsNullOrEmpty(t_line) && pp != new PoInternal())
				{
					PoList.Add(pp);
				}
			}

			// If in translation mode, copy the original message to the translation
			// if the translation is empty or considered so
			if (!trmode || PoList.Count <= 0)
			{
				return;
			}

			foreach (PoInternal pp in PoList)
			{
				if (pp.OriginalMessage.Length <= 0)
				{
					continue;
				}

				if (pp.MessageString == PoInternal.GetBlank())
				{
					pp.MessageString = pp.OriginalMessage;
				}
			}
		}

		public void ReadTXT(string file)
		{
			using (StreamReader reader = File.OpenText(file))
			{
				string t_line = "";
				uint cont = 0;
				while ((t_line = reader.ReadLine()) != null)
				{
					if (cont == 0)
					{
						if (t_line == "{")
						{
							TxtHasBrackets = true;
						}
					}

					TxtInternal ti = new TxtInternal
					{
						Text = t_line
					};
					TxtList.Add(ti);
					cont++;
				}
			}
		}

		public bool ReadSTX(string file)
		{
			if (!StxInternal.IsMagicValid(file))
			{
				InputManager.Print("Invalid STX file magic!");
				return false;
			}

			StxFile = new StxInternal(file);
			if (StxFile.Sentences.Length <= 0)
			{
				return false;
			}

			Type = LoadedFileType.Stx;

			// STX files alone don't contain info about animations/voicelines
			var reply = InputManager.Ask("Would you like to open the\ncorresponding WRD as well?");
			if (reply == DialogResult.Yes)
			{
				OpenFileDialog wrddial = new OpenFileDialog();
				wrddial.Title = "Open WRD file";
				wrddial.Filter = "wrd file (*.wrd)|*.wrd|All files (*.*)|*.*";
				wrddial.ShowDialog();
				var wrdfn = wrddial.FileName;
				StxFile.LoadedWRD = new WRDInternal(wrdfn);
			}

			return true;
		}

		public void ReadXLSX(string file, bool offset)
		{

			using (var stream = File.Open(file, FileMode.Open, FileAccess.Read))
			{
				// Auto-detect format, supports:
				//  - Binary Excel files (2.0-2003 format; *.xls)
				//  - OpenXml Excel files (2007 format; *.xlsx)
				using (var reader = ExcelReaderFactory.CreateReader(stream))
				{
					var content = reader.AsDataSet();

					var table = content.Tables[0];

					for (var i = 1; i < table.Rows.Count; i++)
					{
						XLSXRow row = new XLSXRow();
						var id = table.Rows[i][0].ToString();
						var key = offset ? table.Rows[i][0].ToString() : table.Rows[i][1].ToString();
						//var key = string.Concat(table.Rows[i][0].ToString(), "|", table.Rows[i][1].ToString());
						row.ID = id;
						row.Original = key;
						for (int c = 2; c < table.Columns.Count; c++)
						{
							var value = table.Rows[i][c].ToString();
							row.Translations.Add(value);
						}

						XLSXList.Add(row);
					}
				}
			}
		}

		public string OpenFile(bool trmode, TranslationManager trm, bool autotl)
		{
			OpenFileDialog o = new OpenFileDialog();
			o.Filter = "All files (*.*)|*.*|" +
					   "vo files (*.vo)|*.vo|po file (*po)|*.po|txt files|*.txt|" +
					   "stx files (*.stx)|*.stx|" +
					   "TrueType Font files(*.ttf)|*.ttf|OpenType Font files (*.otf)|*.otf";
			_ = o.ShowDialog();
			var file = o.FileName;
			LastOpenedFile = file;

			bool IsVo = Path.GetExtension(file).ToLowerInvariant() == ".vo";
			bool IsPo = Path.GetExtension(file).ToLowerInvariant() == ".po";
			bool IsTxt = Path.GetExtension(file).ToLowerInvariant() == ".txt";
			bool IsStx = Path.GetExtension(file).ToLowerInvariant() == ".stx";
			bool IsXlsx = Path.GetExtension(file).ToLowerInvariant() == ".xlsx";
			bool IsFont = Path.GetExtension(file).ToLowerInvariant() == ".ttf" || Path.GetExtension(file) == ".otf";
			if (!IsVo && !IsPo && !IsTxt && !IsStx && !IsXlsx)
			{
				if (!IsFont)
				{
					InputManager.Print("Invalid extension!");
				}

				return "";
			}


			string nopath = Path.GetFileName(file);
			string ext = Path.GetExtension(file);
			if (nopath.Contains(ext))
			{
				nopath = nopath.Replace(ext, "");
			}

			// If the file has already been translated (according to the TranslationManager)
			// then don't open it in translation mode?
			bool ok = !trm.TranslatedFiles.Contains(nopath);
			trmode &= ok;

			// Not STX?
			if (!IsStx)
			{
				if (IsVo)
				{
					Type = LoadedFileType.Vo;
					if (autotl)
					{
						var dir = Directory.GetParent(file);
						if (dir == null)
						{
							return "";
						}
						trm.Dictionary.ScanDirectory(file, dir.FullName, this);
					}

					VoInfo.Clear();
					VoList.Clear();

					ReadVo(file, trmode);
					if (autotl)
					{
						trm.Dictionary.ApplySuggestions(this, trmode);
					}
				}
				else
				{
					if (IsPo)
					{
						Type = LoadedFileType.Po;
						if (autotl)
						{
							var dir = Directory.GetParent(file);
							if (dir == null)
							{
								return "";
							}
							trm.Dictionary.ScanDirectory(file, dir.FullName, this);
						}

						PoInfo.Clear();
						PoList.Clear();

						ReadPo(file, trmode);
						if (autotl)
						{
							trm.Dictionary.ApplySuggestions(this, trmode);
						}
					}
					else
					{
						if (IsTxt)
						{
							TxtList.Clear();

							// Assume TXT
							ReadTXT(file);

							Type = LoadedFileType.Txt;
						}
						else
						{
							if (IsXlsx)
							{
								Type = LoadedFileType.Xlsx;

								if (autotl)
								{
									var dir = Directory.GetParent(file);
									if (dir == null)
									{
										return "";
									}
									trm.Dictionary.ScanDirectory(file, dir.FullName, this);
								}

								bool use_offset = false;
								XLSXList.Clear();
								ReadXLSX(file, use_offset);

								if (autotl)
								{
									trm.Dictionary.ApplySuggestions(this, trmode);
								}
							}
							else
							{
								// ???
							}
						}
					}
				}
			}
			else
			{
				// STX
				bool ret = ReadSTX(file);
				if (!ret)
				{
					return "";
				}
			}

			return file;
		}

		public string ManageFile(bool trmode, TranslationManager trm, bool autotl)
		{
			string file;
			bool ok = true;

			do
			{
				ok = true;
				file = OpenFile(trmode, trm, autotl);

				if (file.Length == 0)
				{
					string ext = Path.GetExtension(LastOpenedFile).ToLowerInvariant();
					if (ext != ".ttf" && ext != ".otf")
					{
						InputManager.Print("Invalid file!");
					}

					return "";
				}

				if (trmode)
				{
					if (Type == LoadedFileType.Txt)
					{
						// You *DON'T* want to open a TXT in translation mode
						InputManager.Print("Cannot open TXT in translation mode!");
						return "";
					}

					if (Type == LoadedFileType.Stx)
					{
						InputManager.Print("Cannot open STX in translation mode!");
						return "";
					}

					string nopath = Path.GetFileName(file);
					string ext = Path.GetExtension(file);
					if (nopath.Contains(ext))
					{
						nopath = nopath.Replace(ext, "");
					}

					ok = !trm.TranslatedFiles.Contains(nopath);
					if (!ok)
					{
						var ret = InputManager.Ask(file +
												   " has been already translated.\nDo you want to open it anyway?");
						if (ret == DialogResult.Yes)
						{
							ok = true;
						}
					}
				}
			} while (!ok);

			LoadedFileName = file;

			return LoadedFileName;
		}

		public void SaveTxt(string file, bool oglang)
		{
			TxtSavePath = Path.GetDirectoryName(file);
			//Print("VoList.Count: " + VoList.Count.ToString());
			bool needs_brackets = true;

			string first_line = "";
			switch (Type)
			{
				case LoadedFileType.Vo:
					first_line = oglang
						? VoList.ElementAt(0).OriginalMessage
						: VoList.ElementAt(0).Translations.ElementAt(SelectedLanguage);
					break;
				case LoadedFileType.Po:
					first_line = oglang ? PoList.ElementAt(0).OriginalMessage : PoList.ElementAt(0).MessageString;
					break;
				case LoadedFileType.Txt:
					first_line = TxtList.ElementAt(0).Text;
					break;
				case LoadedFileType.Stx:
					first_line = StxFile.Sentences[0];
					break;
				case LoadedFileType.Xlsx:
					first_line = oglang ? XLSXList.ElementAt(SelectedLanguage).Original : XLSXList.ElementAt(SelectedLanguage).Translations[0];
					break;
			}

			if (first_line.StartsWith("{"))
			{
				needs_brackets = false;
			}

			using (StreamWriter writetext = new StreamWriter(file))
			{
				if (needs_brackets)
				{
					writetext.WriteLine("{");
				}

				if (Type == LoadedFileType.Vo)
				{
					foreach (VoInternal vv in VoList)
					{
						string tosave = vv.Translations.ElementAt(SelectedLanguage);
						if (oglang)
						{
							tosave = vv.OriginalMessage;
						}

						// Remove \" and use "
						tosave = tosave.Replace("\\\"", "\"");
						writetext.WriteLine(tosave);
					}
				}
				else
				{
					if (Type == LoadedFileType.Po)
					{
						foreach (PoInternal pp in PoList)
						{
							string tosave = pp.MessageString;
							if (oglang)
							{
								tosave = pp.OriginalMessage;
							}

							tosave = tosave.Replace("\\\"", "\"");

							writetext.WriteLine(tosave);
						}
					}
					else
					{
						if (Type == LoadedFileType.Txt)
						{
							foreach (TxtInternal ti in TxtList)
							{
								string tosave = ti.Text;
								// Remove \" and use "
								tosave = tosave.Replace("\\\"", "\"");
								writetext.WriteLine(tosave);
							}
						}
						else
						{
							if (Type == LoadedFileType.Stx)
							{
								foreach (string stxi in StxFile.Sentences)
								{
									string tosave = stxi;
									// Remove \" and use "
									tosave = tosave.Replace("\\\"", "\"");
									writetext.WriteLine(tosave);
								}
							}
							else
							{
								if (Type == LoadedFileType.Xlsx)
								{
									foreach (XLSXRow xlsx in XLSXList)
									{
										string tosave = xlsx.Translations[SelectedLanguage];
										// Remove \" and use "
										tosave = tosave.Replace("\\\"", "\"");
										writetext.WriteLine(tosave);
									}
								}
							}
						}
					}
				}

				if (needs_brackets)
				{
					writetext.WriteLine("}");
				}
			}

			InputManager.Print("Saved!");
		}

		//https://stackoverflow.com/questions/52031521/find-count-of-substring-in-string-c-sharp
		public int CountStr(string s, string substr, StringComparison strComp = StringComparison.CurrentCulture)
		{
			int count = 0, index = s.IndexOf(substr, strComp);
			while (index != -1)
			{
				count++;
				index = s.IndexOf(substr, index + substr.Length, strComp);
			}

			return count;
		}

		public void SaveStx(List<string> strings, string file)
		{
			using (FileStream NEWOutFile = new FileStream(file, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter OutFileBW = new BinaryWriter(NEWOutFile),
					   TextUnicode = new BinaryWriter(NEWOutFile, Encoding.Unicode))
				{
					if (StxFile == null)
					{
						StxFile = new StxInternal("");
					}

					// Probably copied from LiquidS' DRV3-STX-TOOL

					if (!StxFile.IsValidSTX)
					{
						OutFileBW.Write(StxInternal.Magic);
						OutFileBW.Write(StxInternal.JPLL);
						OutFileBW.Write(0x1U);
						OutFileBW.Write(0x20U);
						OutFileBW.Write(0x8U);
						OutFileBW.Write((uint)strings.Count);
						OutFileBW.Write(0x0U);
						OutFileBW.Write(0x0U);
					}
					else
					{
						OutFileBW.Write(StxInternal.Magic);
						OutFileBW.Write(StxInternal.JPLL);
						OutFileBW.Write(StxFile.Unk1);
						OutFileBW.Write(StxFile.HeaderSizeH);
						OutFileBW.Write(StxFile.Unk2);
						OutFileBW.Write((uint)strings.Count);
						OutFileBW.Write(0x0U);
						OutFileBW.Write(0x0U);
					}

					long[] sentencesOffeset = new long[strings.Count];
					long pointers_addr = NEWOutFile.Position;

					// Fill the pointers zone with zeroes. I'll populate this at the end.
					for (int i = 0; i < strings.Count; i++)
					{
						// More efficient than writing two uint (one for number and one for addr)
						OutFileBW.Write((long)0);
					}

					for (int i = 0; i < strings.Count; i++)
					{
						bool duplicate = false;

						int x = 0;

						// Check if the sentences "i" is a duplicate. This way we can save some space and write the sentence just once instead of multiples times.
						while (x < i)
						{
							if (strings[i] == strings[x])
							{
								duplicate = true;
								sentencesOffeset[i] = sentencesOffeset[x];
								break;
							}

							x++;
						}

						if (duplicate == false)
						{
							sentencesOffeset[i] = NEWOutFile.Position;

							// Write the sentence n# [i] in the repacked file.
							TextUnicode.Write(strings[i].ToCharArray());

							// Write down the null string terminator.
							OutFileBW.Write((ushort)0x00);
						}
					}

					// Now populate the pointers zone.
					NEWOutFile.Seek(pointers_addr, SeekOrigin.Begin);

					for (int i = 0; i < strings.Count; i++)
					{
						OutFileBW.Write((uint)i);
						OutFileBW.Write((uint)sentencesOffeset[i]);
					}
				}
			}
		}

		public void StxToVo(string file)
		{
			List<string> info = new List<string>();
			List<VoInternal> VoLines = new List<VoInternal>();

			info.Add(VoInternal.GetDefaultMsgid());
			info.Add(VoInternal.GetDefaultMsgstr());
			info.Add(VoInternal.GetDefaultProjectID());
			info.Add(VoInternal.GetDefaultReportTo());
			info.Add(VoInternal.GetDefaultPOTCreationDate());
			info.Add(VoInternal.GetDefaultRevisionDate());
			info.Add(VoInternal.GetDefaultLastTranslator());
			info.Add(VoInternal.GetDefaultLanguageTeam());
			info.Add(VoInternal.GetDefaultLanguage());
			info.Add(VoInternal.GetDefaultMIMEVersion());
			info.Add(VoInternal.GetDefaultContentType());
			info.Add(VoInternal.GetDefaultContentEncoding());

			for (int cont = 0; cont < StxFile.Sentences.Length; cont++)
			{
				string str = StxFile.Sentences[cont];

				VoInternal vi = new VoInternal();

				int numlines = CountStr(str, "\\n") * 2 + 1;

				vi.InternalLines = numlines;

				vi.CommentsNumber = 0;

				vi.Comments = new List<string>();

				vi.OriginFile = Path.GetFileNameWithoutExtension(LoadedFileName);

				vi.LineNumber = cont;

				vi.Character = StxFile.CharacterByLineNumber(cont);

				vi.OriginalMessageLength = str.Length;

				vi.OriginalMessage = str;

				vi.TranslationNumber = 1;

				vi.Translations = new List<string>
				{
					""
				};

				VoLines.Add(vi);
			}

			using (StreamWriter sw = new StreamWriter(file))
			{
				foreach (string line in info)
				{
					sw.WriteLine(line);
				}

				sw.WriteLine("");

				foreach (var vi in VoLines)
				{
					sw.WriteLine("Lines: " + vi.InternalLines);
					sw.WriteLine("Number of comments: " + vi.CommentsNumber);
					sw.WriteLine("Comments:");
					foreach (string comment in vi.Comments)
					{
						sw.WriteLine(comment);
					}

					sw.WriteLine("File: " + vi.OriginFile);
					sw.WriteLine("Line: " + vi.LineNumber);
					sw.WriteLine("Character: " + vi.Character);
					sw.WriteLine("Original message length: " + vi.OriginalMessageLength);
					sw.WriteLine("Original message: \"" + vi.OriginalMessage + "\"");
					sw.WriteLine("Number of translations: " + vi.TranslationNumber);
					sw.WriteLine("Translations:");
					foreach (string tl in vi.Translations)
					{
						sw.WriteLine("\"" + tl + "\"");
					}

					sw.WriteLine("");
				}
			}
		}

		public void StxToPo(string file)
		{
			List<string> info = new List<string>();
			List<PoInternal> PoLines = new List<PoInternal>();

			info.Add(PoInternal.GetDefaultMsgid());
			info.Add(PoInternal.GetDefaultMsgstr());
			info.Add(PoInternal.GetDefaultProjectID());
			info.Add(PoInternal.GetDefaultReportTo());
			info.Add(PoInternal.GetDefaultPOTCreationDate());
			info.Add(PoInternal.GetDefaultRevisionDate());
			info.Add(PoInternal.GetDefaultLastTranslator());
			info.Add(PoInternal.GetDefaultLanguageTeam());
			info.Add(PoInternal.GetDefaultLanguage());
			info.Add(PoInternal.GetDefaultMIMEVersion());
			info.Add(PoInternal.GetDefaultContentType());
			info.Add(PoInternal.GetDefaultContentEncoding());

			for (int cont = 0; cont < StxFile.Sentences.Length; cont++)
			{
				string contstr = cont.ToString();
				while (contstr.Length < 4) contstr = contstr.Insert(0, "0");

				PoInternal pi = new PoInternal();

				// msgctx

				pi.MessageContext = "msgctxt \"";

				pi.MessageContext += contstr + " | ";

				pi.MessageContext += Path.GetFileNameWithoutExtension(LoadedFileName);

				if (StxFile.LoadedWRD != null)
				{
					pi.MessageContext += " | ";
					pi.MessageContext += StxFile.CharacterByLineNumber(cont);
				}

				pi.MessageContext += "\"";

				// msgid

				pi.OriginalMessage = "msgid ";

				string pizza = StxFile.Sentences[cont];

				pi.OriginalMessage += ConvertPoNewlines(pizza);

				// msgstr

				pi.MessageString = "msgstr \"\"";

				PoLines.Add(pi);
			}

			using (StreamWriter sw = new StreamWriter(file))
			{
				foreach (string line in info)
				{
					sw.WriteLine(line);
				}

				sw.WriteLine("");

				foreach (PoInternal pi in PoLines)
				{
					sw.WriteLine(pi.MessageContext);
					sw.WriteLine(pi.OriginalMessage);
					sw.WriteLine(pi.MessageString);
					sw.WriteLine("");
				}
			}
		}

		public void SavePo(string file)
		{
			File.Delete(file);
			using (StreamWriter writetext = new StreamWriter(file))
			{
				foreach (string s in PoInfo)
				{
					writetext.WriteLine(s);
				}

				writetext.WriteLine();
				int filecont = 0;
				foreach (PoInternal pp in PoList)
				{
					string msgctxt = "msgctxt \"" + pp.LineNumber + " | " + pp.OriginFile;

					if (!string.IsNullOrWhiteSpace(pp.Character))
					{
						msgctxt += " | " + pp.Character;

						// Voiceline exists?
						if (!string.IsNullOrWhiteSpace(pp.Voiceline))
						{
							// Voiceline exists 

							if (string.IsNullOrWhiteSpace(pp.Expression))
							{
								msgctxt += " | None | " + pp.Voiceline;
							}
							else
							{
								msgctxt += " | " + pp.Expression + " | " + pp.Voiceline;
							}
						}
						else
						{
							// Voiceline does not exist

							if (!string.IsNullOrWhiteSpace(pp.Expression))
							{
								msgctxt += " | " + pp.Expression;
							}
						}
					}

					msgctxt += "\"";

					string msgid = pp.OriginalMessage;
					string msgstr = pp.MessageString;
					writetext.WriteLine(msgctxt);

					if (msgid.Contains("\\n"))
					{
						writetext.WriteLine("msgid \"\"");
						while (msgid.Contains("\\n"))
						{
							int index = msgid.IndexOf("\\n");
							string partial = msgid.Substring(0, index + 2);
							string pstr = "\"" + partial + "\"";
							writetext.WriteLine(pstr);
							msgid = msgid.Substring(index + 2);
						}

						string pstr2 = "\"" + msgid + "\"";
						writetext.WriteLine(pstr2);
					}
					else
					{
						writetext.WriteLine("msgid \"" + msgid + "\"");
					}

					if (msgstr.Contains("\\n"))
					{
						writetext.WriteLine("msgstr \"\"");
						while (msgstr.Contains("\\n"))
						{
							int index = msgstr.IndexOf("\\n");
							string partial = msgstr.Substring(0, index + 2);
							string pstr = "\"" + partial + "\"";
							writetext.WriteLine(pstr);
							msgstr = msgstr.Substring(index + 2);
						}

						string pstr2 = "\"" + msgstr + "\"";
						writetext.WriteLine(pstr2);
					}
					else
					{
						writetext.WriteLine("msgstr \"" + msgstr + "\"");
					}

					if (filecont < PoList.Count - 1)
					{
						writetext.WriteLine();
					}

					filecont++;
				}
			}
		}

		public void SaveVo(string file)
		{
			File.Delete(file);
			//Print("VoList.Count: " + VoList.Count.ToString());
			using (StreamWriter writetext = new StreamWriter(file))
			{
				foreach (string s in VoInfo)
				{
					writetext.WriteLine(s);
				}

				writetext.WriteLine();
				foreach (VoInternal vv in VoList)
				{
					writetext.WriteLine("Lines: " + vv.InternalLines);
					writetext.WriteLine("Number of comments: " + vv.CommentsNumber);
					writetext.WriteLine("Comments:");
					writetext.WriteLine("File: " + vv.OriginFile);
					writetext.WriteLine("Line: " + vv.LineNumber);
					writetext.WriteLine("Character: " + vv.Character);
					writetext.WriteLine("Original message length: " + vv.OriginalMessageLength);
					writetext.WriteLine("Original message: " + "\"" + vv.OriginalMessage + "\"");
					writetext.WriteLine("Number of translations: " + vv.TranslationNumber);
					writetext.WriteLine("Translations:");
					foreach (string tl in vv.Translations)
					{
						writetext.WriteLine("\"" + tl + "\"");
					}

					writetext.WriteLine();
				}
			}

			InputManager.Print("Saved!");
		}

		public string RectifyLineNumber(string num)
		{
			// Number padding

			int cipher = num.Length;
			if (cipher >= 4)
			{
				string trimmed = num.Substring(0, 4 - 1);
				return trimmed;
			}

			string temp = "";
			int diff = 4 - cipher;
			for (int j = 0; j < diff; j++)
			{
				temp += "0";
			}

			temp += num;
			return temp;
		}

		public string CalculateContext(int linenum, string ogfile, string character)
		{
			string line_str = RectifyLineNumber(linenum.ToString());
			string msgctxt = "msgctxt \"" + line_str + " | " + ogfile + " | " + character + "\"";
			return msgctxt;
		}

		public List<string> CalculateIDs(string ogmsg)
		{
			string msgid = "msgid \"" + ogmsg + "\"";
			const string tofind = "\\n";
			var find = msgid.IndexOf(tofind);
			List<string> msgid_list = new List<string>();
			if (find != -1)
			{
				do
				{
					find = msgid.IndexOf(tofind);
					if (find != -1)
					{
						string partial = "\"" + msgid.Substring(0, find) + "\"";
						partial = partial.Replace("\"\"", "\"");
						if (partial.StartsWith("\""))
						{
							partial = partial.Substring(1);
						}

						msgid_list.Add(partial);
						msgid = msgid.Substring(find + 1);
					}
				} while (find != -1);
			}
			else
			{
				if (msgid.StartsWith("\""))
				{
					msgid = msgid.Substring(1);
				}

				msgid_list.Add(msgid);
			}

			return msgid_list;
		}

		public List<string> CalculateStr(string tr0)
		{
			List<string> msgstr_list = new List<string>();
			const string tofind = "\\n";
			string msgstr = "msgstr \"" + tr0 + "\"";
			var find = msgstr.IndexOf(tofind);
			if (find != -1)
			{
				do
				{
					find = msgstr.IndexOf(tofind);
					if (find != -1)
					{
						string partial = "\"" + msgstr.Substring(0, find) + "\"";
						partial = partial.Replace("\"\"", "\"");
						msgstr_list.Add(partial);
						msgstr = msgstr.Substring(find + 1);
					}
				} while (find != -1);
			}
			else
			{
				msgstr_list.Add(msgstr);
			}

			return msgstr_list;
		}

		public void XlsxToVo(string file)
		{

			List<string> fileinfo = new List<string>
			{
				VoInternal.GetDefaultMsgid(),
				VoInternal.GetDefaultMsgstr(),
				VoInternal.GetDefaultProjectID(),
				VoInternal.GetDefaultReportTo(),
				VoInternal.GetDefaultPOTCreationDate(),
				VoInternal.GetDefaultRevisionDate(),
				VoInternal.GetDefaultLastTranslator(),
				VoInternal.GetDefaultLanguageTeam(),
				VoInternal.GetDefaultLanguage(),
				VoInternal.GetDefaultMIMEVersion(),
				VoInternal.GetDefaultContentType(),
				VoInternal.GetDefaultContentEncoding()
			};

			using (StreamWriter writetext = new StreamWriter(file))
			{
				foreach (string s in fileinfo)
				{
					writetext.WriteLine(s);
				}

				writetext.WriteLine();
				int cont_column = 0;
				foreach (XLSXRow row in XLSXList)
				{
					writetext.WriteLine("Lines: 3");
					writetext.WriteLine("Number of comments: 0");
					writetext.WriteLine("Comments:");
					writetext.WriteLine("File: " + Path.GetFileNameWithoutExtension(LoadedFileName));
					writetext.WriteLine("Line: " + cont_column);
					writetext.WriteLine("Character: ???");
					writetext.WriteLine("Original message length: " + row.Original.Length);
					writetext.WriteLine("Original message: " + "\"" + row.Original + "\"");
					writetext.WriteLine("Number of translations: " + row.Translations.Count);
					writetext.WriteLine("Translations:");
					foreach (string tl in row.Translations)
					{
						writetext.WriteLine("\"" + tl + "\"");
					}

					writetext.WriteLine();
					cont_column++;
				}
			}
		}

		public string ConvertPoNewlines(string str)
		{
			bool contains_newline = str.Contains("\\n");
			string ret = "";

			if (contains_newline)
			{
				ret += "\"\"";
				ret += "\n";
			}

			int cut_length = 0;
			string slice = str;
			while (slice.Contains("\\n"))
			{
				slice = str.Substring(cut_length);
				int index = slice.IndexOf("\\n");
				if (index < 0)
				{
					break;
				}

				int max = index + "\\n".Length;
				slice = slice.Substring(0, max);
				cut_length += max;
				ret += "\"" + slice + "\"";
				if (slice.Length > 0)
				{
					ret += "\n";
				}
				//InputManager.Print("\"" + slice + "\"");
			}

			slice = str.Substring(cut_length);
			if (slice.Length > 0)
			{
				ret += "\"" + slice + "\"";
			}

			return ret;
		}

		public void XlsxToPo(string file)
		{
			List<PoInternal> PoLines = new List<PoInternal>();

			List<string> fileinfo = new List<string>
			{
				PoInternal.GetDefaultMsgid(),
				PoInternal.GetDefaultMsgstr(),
				PoInternal.GetDefaultProjectID(),
				PoInternal.GetDefaultReportTo(),
				PoInternal.GetDefaultPOTCreationDate(),
				PoInternal.GetDefaultRevisionDate(),
				PoInternal.GetDefaultLastTranslator(),
				PoInternal.GetDefaultLanguageTeam(),
				PoInternal.GetDefaultLanguage(),
				PoInternal.GetDefaultMIMEVersion(),
				PoInternal.GetDefaultContentType(),
				PoInternal.GetDefaultContentEncoding()
			};

			for (int cont = 0; cont < XLSXList.Count; cont++)
			{
				string contstr = cont.ToString();
				while (contstr.Length < 4) contstr = contstr.Insert(0, "0");

				PoInternal pi = new PoInternal();

				// msgctx

				pi.MessageContext = "msgctxt \"";

				/*
				pi.MessageContext += contstr + " | ";

				pi.MessageContext += Path.GetFileNameWithoutExtension(LoadedFileName);
                */

				pi.MessageContext += XLSXList[cont].ID;

				pi.MessageContext += "\"";

				// msgid

				pi.OriginalMessage = "msgid ";

				string pizza = XLSXList[cont].Original;

				pi.OriginalMessage += ConvertPoNewlines(pizza);

				// msgstr

				pi.MessageString = "msgstr ";
				pi.MessageString += ConvertPoNewlines(XLSXList[cont].Translations[SelectedLanguage]);

				PoLines.Add(pi);
			}

			using (StreamWriter sw = new StreamWriter(file))
			{
				foreach (string line in fileinfo)
				{
					sw.WriteLine(line);
				}

				sw.WriteLine("");

				foreach (PoInternal pi in PoLines)
				{
					sw.WriteLine(pi.MessageContext);
					sw.WriteLine(pi.OriginalMessage);
					sw.WriteLine(pi.MessageString);
					sw.WriteLine("");
				}
			}
		}

		public void TxtToXlsx(string file)
		{
			using (var excel = new ExcelPackage())
			{
				var sheet = excel.Workbook.Worksheets.Add("Hoja 1");

				var header = new List<string[]>
				{
					new[] { "TRADUCCIÓN" }
				};

				sheet.Cells["A1"].LoadFromArrays(header);

				var row = 1;
				foreach (var subtitle in TxtList)
				{
					sheet.Cells[row, 1].Value = subtitle.Text;

					row++;
				}

				var excelFile = new FileInfo(file);
				excel.SaveAs(excelFile);
			}
		}

		public void PoToXlsx(string file)
		{
			using (var excel = new ExcelPackage())
			{
				var sheet = excel.Workbook.Worksheets.Add("Hoja 1");

				var header = new List<string[]>
				{
					new[] {"OFFSET", "ORIGINAL", "TRADUCCIÓN"}
				};

				sheet.Cells["A1:C1"].LoadFromArrays(header);

				var row = 2;
				foreach (var subtitle in PoList)
				{
					// TODO?
					sheet.Cells[row, 1].Value = subtitle.MessageContext;
					sheet.Cells[row, 2].Value = subtitle.OriginalMessage;
					sheet.Cells[row, 3].Value = subtitle.MessageString;

					row++;
				}

				var excelFile = new FileInfo(file);
				excel.SaveAs(excelFile);
			}
		}

		public void VoToXlsx(string file)
		{
			using (var excel = new ExcelPackage())
			{
				var sheet = excel.Workbook.Worksheets.Add("Hoja 1");

				var header = new List<string[]>
				{
					// Spanish to maintain compatibility with another program (namely TranslationFramework2)
					new[] {"OFFSET", "ORIGINAL", "TRADUCCIÓN"}
				};

				sheet.Cells["A1:C1"].LoadFromArrays(header);

				var row = 2;
				foreach (var subtitle in VoList)
				{
					string condensed_comments = "";
					int count_comments = 0;
					foreach (var comment in subtitle.Comments)
					{
						condensed_comments += comment;
						if (count_comments < subtitle.CommentsNumber - 1)
						{
							condensed_comments += "; ";
						}
						count_comments++;
					}
					sheet.Cells[row, 1].Value = condensed_comments;
					sheet.Cells[row, 2].Value = subtitle.OriginalMessage;

					for (int tl = 3; tl < subtitle.TranslationNumber + 3; tl++)
					{
						sheet.Cells[row, tl].Value = subtitle.Translations[tl - 3];
					}

					row++;
				}

				var excelFile = new FileInfo(file);
				excel.SaveAs(excelFile);
			}
		}

		public void StxToXlsx(string file)
		{
			using (var excel = new ExcelPackage())
			{
				// Spanish to maintain compatibility with another program (namely TranslationFramework2)
				// This means "Page 1"
				var sheet = excel.Workbook.Worksheets.Add("Hoja 1");

				var header = new List<string[]>
				{
					// Spanish to maintain compatibility with another program (namely TranslationFramework2)
					new[] {"CONTEXT", "TRADUCCIÓN"}
				};

				sheet.Cells["A1:B1"].LoadFromArrays(header);

				var row = 2;
				int line = 0;
				foreach (var subtitle in StxFile.Sentences)
				{
					string condensed_context = "";
					string character = StxFile.CharacterByLineNumber(line);
					string expression = StxFile.ExpressionByLineNumber(line, character);
					string voiceline = StxFile.VoicelineByLineNumber(line);
					condensed_context += line.ToString();
					condensed_context += " -- ";
					condensed_context += (character.Length == 0 ? "Unknown" : character);
					condensed_context += " (";
					condensed_context += (expression.Length == 0 ? "Unknown" : expression);
					condensed_context += ", ";
					condensed_context += (voiceline.Length == 0 ? "No Voiceline" : voiceline);
					condensed_context += ")";
					sheet.Cells[row, 1].Value = condensed_context;
					sheet.Cells[row, 2].Value = subtitle;

					row++;
					line++;
				}

				var excelFile = new FileInfo(file);
				excel.SaveAs(excelFile);
			}
		}

		public void SaveXlsx(string file)
		{
			using (var excel = new ExcelPackage())
			{
				var sheet = excel.Workbook.Worksheets.Add("Hoja 1");

				var header = new List<string[]>
				{
					// Spanish to maintain compatibility with another program (namely TranslationFramework2)
					new[] {"OFFSET", "ORIGINAL", "TRADUCCIÓN"}
				};

				sheet.Cells["A1:C1"].LoadFromArrays(header);

				var row = 2;
				foreach (var subtitle in XLSXList)
				{
					sheet.Cells[row, 1].Value = subtitle.ID;
					sheet.Cells[row, 2].Value = subtitle.Original;
					for (int tl = 3; tl < subtitle.Translations.Count + 3; tl++)
					{
						sheet.Cells[row, tl].Value = subtitle.Translations[tl - 3];
					}

					row++;
				}

				var excelFile = new FileInfo(file);
				excel.SaveAs(excelFile);
			}
		}

		public void VoToPo(string file)
		{
			List<string> fileinfo = new List<string>();

			fileinfo.Add(VoInternal.GetDefaultMsgid());
			fileinfo.Add(VoInternal.GetDefaultMsgstr());
			fileinfo.Add(VoInternal.GetDefaultProjectID());
			fileinfo.Add(VoInternal.GetDefaultReportTo());
			fileinfo.Add(VoInternal.GetDefaultPOTCreationDate());
			fileinfo.Add(VoInternal.GetDefaultRevisionDate());
			fileinfo.Add(VoInternal.GetDefaultLastTranslator());
			fileinfo.Add(VoInternal.GetDefaultLanguageTeam());
			fileinfo.Add(VoInternal.GetDefaultLanguage());
			fileinfo.Add(VoInternal.GetDefaultMIMEVersion());
			fileinfo.Add(VoInternal.GetDefaultContentType());
			fileinfo.Add(VoInternal.GetDefaultContentEncoding());

			using (StreamWriter writetext = new StreamWriter(file))
			{
				foreach (string s in fileinfo)
				{
					writetext.WriteLine(s);
				}

				writetext.WriteLine();

				foreach (VoInternal vv in VoList)
				{
					string context = CalculateContext(vv.LineNumber, vv.OriginFile, vv.Character);
					writetext.WriteLine(context);

					List<string> msgid_list = new List<string>(CalculateIDs(vv.OriginalMessage));

					foreach (string ids in msgid_list)
					{
						writetext.WriteLine(ids);
					}

					List<string> msgstr_list = new List<string>(CalculateStr(vv.Translations[0]));

					foreach (string str in msgstr_list)
					{
						writetext.WriteLine(str);
					}

					writetext.WriteLine();
				}
			}

			InputManager.Print("Done!");
		}
	}
}