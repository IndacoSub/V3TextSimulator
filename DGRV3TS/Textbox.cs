namespace DGRV3TS
{
	partial class Operations
	{
		// Blacklist
		private static List<char> UnallowedCharacters = new List<char>()
		{

		};

		private void CheckInvalidity(string str)
		{
			// Does the string contain invalid characters?

			if (str.Length == 0)
			{
				return;
			}

			bool invalids = false;
			List<char> inv = new List<char>();
			foreach (char c in str.Where(c => UnallowedCharacters.Contains(c)))
			{
				if (!invalids)
				{
					invalids = true;
				}

				if (!inv.Contains(c))
				{
					inv.Add(c);
				}
			}

			if (invalids)
			{
				LabelUnsupportedWarning.Text =
					string.Format("Warning: Text contains one or more unsupported characters: {0}",
						string.Join(",", inv));
				LabelUnsupportedWarning.Visible = true;
			}
			else
			{
				LabelUnsupportedWarning.Visible = false;
			}
		}

		private void UpdateRTB()
		{
			// RTB = RichBlockText?

			if (!Textbox.Enabled)
			{
				return;
			}

			if (FastReading && !DEBUG_ON)
			{
				return;
			}

			var bk = Textbox.Text;
			if (!tm.IsJapanese(bk))
			{
				// Reset font
				Textbox.Font = fm.CreateFont(fm.DefaultFontNameForText, fm.FontSizeForText, FontStyle.Regular);
			}

			Textbox.ResetText();
			Textbox.Text = bk;
			Textbox.Refresh();
			Textbox.Update();
			RecreateImageBase();
		}

		private void UpdateLineCharacter()
		{
			// Update info labels

			if (!LoadedFile)
			{
				return;
			}

			switch (fi.Type)
			{
				case FileManager.LoadedFileType.Vo:
					if (fi.VoList.Count < fi.StringIndex)
					{
						return;
					}

					LabelLineNumber.Text = "Line: " + fi.VoList.ElementAt(fi.StringIndex).LineNumber;
					LabelCharacterName.Text = "Character: " + fi.VoList.ElementAt(fi.StringIndex).Character;
					LabelOriginFile.Text = "Origin file: " + fi.VoList.ElementAt(fi.StringIndex).OriginFile;
					// 4 is opened file so it's always the same
					if (CheckboxDisplayOriginalText.Checked)
					{
						LabelCurrentTranslation.Text = "Translation: Original Text";
					}
					else
					{
						LabelCurrentTranslation.Text = "Translation: " + (fi.SelectedLanguage + 1) + "/" +
													   fi.VoList.ElementAt(fi.StringIndex).TranslationNumber;
					}

					break;
				case FileManager.LoadedFileType.Po:
					if (fi.PoList.Count < fi.StringIndex)
					{
						return;
					}

					LabelLineNumber.Text = "Line: " + fi.PoList.ElementAt(fi.StringIndex).LineNumber;
					LabelCharacterName.Text = "Character: " + fi.PoList.ElementAt(fi.StringIndex).Character;
					LabelOriginFile.Text = "Origin file: " + fi.PoList.ElementAt(fi.StringIndex).OriginFile;
					break;
				case FileManager.LoadedFileType.Txt:
				case FileManager.LoadedFileType.Xlsx:
					LabelLineNumber.Text = "Line: " + (fi.StringIndex + 1);
					LabelOriginFile.Text = "Origin file: " + Path.GetFileName(fi.LoadedFileName);
					break;
				case FileManager.LoadedFileType.Stx:
					LabelLineNumber.Text = "Line: " + (fi.StringIndex + 1);
					if (fi.StxFile.LoadedWRD != null)
					{
						LabelCharacterName.Text = "Character: " + fi.StxFile.CharacterByLineNumber(fi.StringIndex);
					}

					LabelOriginFile.Text = "Origin file: " + Path.GetFileName(fi.LoadedFileName);
					break;
			}

			LabelLineNumber.Update();
			LabelCharacterName.Update();
			LabelOriginFile.Update();
			LabelCurrentTranslation.Update();
		}

		private void UpdateTextbox()
		{
			if (!LoadedFile) return;

			// If the translation is valid, load the translation
			// Otherwise, load the original message

			if (fi.Type == FileManager.LoadedFileType.Vo)
			{
				if (fi.VoList.Count >= 1)
				{
					if (fi.VoList.ElementAt(fi.StringIndex) != null)
					{
						if (fi.VoList.ElementAt(fi.StringIndex).Translations.Count >= 1)
						{
							if (fi.VoList.ElementAt(fi.StringIndex).Translations.ElementAt(fi.SelectedLanguage) != null)
							{
								if (fi.VoList.ElementAt(fi.StringIndex).OriginalMessage != null)
								{
									LoadOriginalLanguage(CheckboxDisplayOriginalText.Checked);
									UpdateRTB();
									// Debug needs this off
									if (!FastReading || DEBUG_ON)
									{
										Reload();
									}
								}
							}
						}
					}
				}
			}
			else
			{
				if (fi.Type == FileManager.LoadedFileType.Po)
				{
					if (fi.PoList.Count >= 1)
					{
						if (fi.PoList.ElementAt(fi.StringIndex) != null)
						{
							if (fi.PoList.ElementAt(fi.StringIndex).OriginalMessage != null)
							{
								LoadOriginalLanguage(CheckboxDisplayOriginalText.Checked);
								UpdateRTB();
								// Debug needs this off
								if (!FastReading || DEBUG_ON)
								{
									Reload();
								}
							}
						}
					}
				}
				else
				{
					if (fi.Type == FileManager.LoadedFileType.Txt)
					{
						if (fi.TxtList.Count >= 1)
						{
							if (fi.TxtList.ElementAt(fi.StringIndex) != null)
							{
								if (fi.TxtList.ElementAt(fi.StringIndex).Text != null)
								{
									Textbox.Text = fi.TxtList.ElementAt(fi.StringIndex).Text;
									UpdateRTB();
									if (!FastReading || DEBUG_ON)
									{
										Reload();
									}
								}
							}
						}
					}
					else
					{
						if (fi.Type == FileManager.LoadedFileType.Stx)
						{
							if (fi.StxFile.Sentences.Length >= 1)
							{
								if (fi.StxFile.Sentences[fi.StringIndex] != null)
								{
									Textbox.Text = fi.StxFile.Sentences[fi.StringIndex];
									UpdateRTB();
									if (!FastReading || DEBUG_ON)
									{
										Reload();
									}
								}
							}
						}
						else
						{
							if (fi.Type == FileManager.LoadedFileType.Xlsx)
							{
								if (fi.XLSXList.Count >= 1)
								{
									if (fi.XLSXList.ElementAt(fi.StringIndex) != null)
									{
										if (fi.XLSXList.ElementAt(fi.StringIndex).Translations.ElementAt(fi.SelectedLanguage) != null)
										{
											Textbox.Text = fi.XLSXList.ElementAt(fi.StringIndex).Translations.ElementAt(fi.SelectedLanguage);
											UpdateRTB();
											if (!FastReading || DEBUG_ON)
											{
												Reload();
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}