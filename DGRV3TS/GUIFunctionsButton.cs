namespace DGRV3TS
{
	partial class Operations
	{
		private void ButtonReloadText_Click(object sender, EventArgs e)
		{
			if (AutoPlayOn)
			{
				return;
			}

			Reload();
		}

		private void ButtonCopyImage_Click(object sender, EventArgs e)
		{
			if (dialogue_window == null)
			{
				return;
			}

			if (InputManager.Prompt(MessageBoxButtons.YesNo, "Copy the current image to the clipboard?") !=
				DialogResult.Yes)
			{
				return;
			}

			Clipboard.SetImage(dialogue_window.DisplayedImage.Image);
			InputManager.Print("Copied!");
		}

		private void OpenFile(string arg_file)
		{
			// Try to open a file

			LoadedFile = false;
			fi = new FileManager();
			fi.GameIndex = CurrentGameIndex;
			// Reset to default text
			Textbox.Text = "This is an example\\nmessage.";
			UpdateLineCharacter();
			UpdateRTB();
			Reload();

			var file = fi.ManageFile(CheckboxTranslationMode.Checked, tm, CheckboxAutoTranslation.Checked, arg_file);

			string ext = Path.GetExtension(fi.LastOpenedFile).ToLowerInvariant();
			bool is_font = ext == ".ttf" || ext == ".otf";

            if (is_font)
			{
				fm = new FontManager(fi.LastOpenedFile);
				LabelFontName.Text = fm.FontName;
				Reload();
			}

			if (string.IsNullOrEmpty(file))
			{
				CheckboxAutoTranslation.Visible = true;
				return;
			}

			CheckboxAutoTranslation.Visible = false;

			switch (fi.Type)
			{
				// Only some filetypes support multiple translations
				case FileManager.LoadedFileType.Vo:
				case FileManager.LoadedFileType.Xlsx:
					LabelCurrentTranslation.Visible = true;
					ButtonBackLanguage.Visible = true;
					ButtonNextLanguage.Visible = true;
					break;
				case FileManager.LoadedFileType.Txt:
				case FileManager.LoadedFileType.Po:
				case FileManager.LoadedFileType.Stx:
					LabelCurrentTranslation.Visible = false;
					ButtonBackLanguage.Visible = false;
					ButtonNextLanguage.Visible = false;
					break;
				default:
					return;
			}

			LoadedFile = true;

			string pathfile = Path.GetFileNameWithoutExtension(file);
			// Limited from space in the GUI
			// WWWWWWWWWWWWWWWWWWWWWWWWWW... is 11 but we'll do 13
			if (pathfile.Length > 13)
			{
				pathfile = pathfile.Substring(0, 13);
				pathfile += "...";
			}

			LabelOpenedFile.Text = "Opened: " + pathfile;
			LabelOpenedFile.Update();

			UpdateTextbox();
			UpdateLineCharacter();
			DisplayCharacterImage();

            if (!is_font)
            {
                DestroyVerticalView();
                OpenVerticalView();
            }
        }

		private void ButtonOpenFile_Click(object sender, EventArgs e)
		{
			OpenFile("");
		}

		private void ButtonBackText_Click(object sender, EventArgs e)
		{
			if (CheckboxStartAutoplay.Checked)
			{
				return;
			}

			CheckUnsaved();
			StringIndexMinus();

            if (vertical_view != null && vertical_view.Visible && LoadedFile && !FastReading)
            {
                OpenVerticalView();
            }
        }

		private void ButtonNextText_Click(object sender, EventArgs e)
		{
			if (CheckboxStartAutoplay.Checked)
			{
				return;
			}

			CheckUnsaved();
			StringIndexPlus();

            if (vertical_view != null && vertical_view.Visible && LoadedFile && !FastReading)
            {
                OpenVerticalView();
            }
        }

		private void ButtonBackLanguage_Click(object sender, EventArgs e)
		{
			if (!LoadedFile)
			{
				InputManager.Print("You need to open a file first!");
				return;
			}

			if (fi.Type != FileManager.LoadedFileType.Vo && fi.Type != FileManager.LoadedFileType.Xlsx)
			{
				return;
			}

			if (CheckboxStartAutoplay.Checked)
			{
				return;
			}

			if (CheckboxDisplayOriginalText.Checked)
			{
				return;
			}

			CheckUnsaved();

			if (fi.SelectedLanguage - 1 >= 0)
			{
				fi.SelectedLanguage--;
			}

			UpdateLineCharacter();
			UpdateTextbox();
        }

		private void ButtonNextLanguage_Click(object sender, EventArgs e)
		{
			if (!LoadedFile)
			{
				InputManager.Print("You need to open a file first!");
				return;
			}

			if (fi.Type != FileManager.LoadedFileType.Vo && fi.Type != FileManager.LoadedFileType.Xlsx)
			{
				return;
			}

			if (CheckboxStartAutoplay.Checked)
			{
				return;
			}

			if (CheckboxDisplayOriginalText.Checked)
			{
				return;
			}

			CheckUnsaved();

			if (fi.Type == FileManager.LoadedFileType.Vo)
			{
				if (fi.SelectedLanguage + 1 < fi.VoList.ElementAt(fi.StringIndex).Translations.Count)
				{
					fi.SelectedLanguage++;
				}
			}
			else
			{
				if (fi.Type == FileManager.LoadedFileType.Xlsx)
				{
					if (fi.SelectedLanguage + 1 < fi.XLSXList.ElementAt(fi.StringIndex).Translations.Count)
					{
						fi.SelectedLanguage++;
					}
				}
			}

			UpdateLineCharacter();
			UpdateTextbox();
        }

		private void ButtonResetStringIndex_Click(object sender, EventArgs e)
		{
			if (!LoadedFile)
			{
				InputManager.Print("You need to open a file first!");
				return;
			}

			if (CheckboxStartAutoplay.Checked)
			{
				return;
			}

			// This is the reset button

			fi.StringIndex = 0;
			UpdateTextbox();
			UpdateLineCharacter();
			DisplayCharacterImage();
        }

		private void ButtonFastRead_Click(object sender, EventArgs e)
		{
			if (!LoadedFile)
			{
				return;
			}

			if (AutoPlayOn)
			{
				return;
			}

			bool old_read = Textbox.ReadOnly;

			if (dialogue_window != null)
			{
				dialogue_window.DisplayedImage.Visible = false;
				dialogue_window.DisplayedImage.Enabled = false;
			}

			Textbox.ReadOnly = true;
			if (!DEBUG_ON)
			{
				Textbox.Enabled = false;
			}

			Textbox.Visible = false;

			CheckboxDisplayCharacter.Enabled = false;

			FastReading = true;

			if (fi.Type == FileManager.LoadedFileType.Vo)
			{
				// Benchmark
				// Stopwatch sw = new Stopwatch();
				// sw.Start();

				while (fi.StringIndex + 1 < fi.VoList.Count)
				{
					StringIndexPlus();
					Reload();
				}

				// sw.Stop();
				// InputManager.Print("Elapsed: " + sw.Elapsed.ToString());
			}
			else
			{
				if (fi.Type == FileManager.LoadedFileType.Po)
				{
					while (fi.StringIndex + 1 < fi.PoList.Count)
					{
						StringIndexPlus();
						Reload();
					}
				}
				else
				{
					if (fi.Type == FileManager.LoadedFileType.Txt)
					{
						while (fi.StringIndex + 1 < fi.TxtList.Count)
						{
							StringIndexPlus();
							Reload();
						}
					}
					else
					{
						if (fi.Type == FileManager.LoadedFileType.Stx)
						{
							while (fi.StringIndex + 1 < fi.StxFile.Sentences.Length)
							{
								StringIndexPlus();
								Reload();
							}
						}
						else
						{
							if (fi.Type == FileManager.LoadedFileType.Xlsx)
							{
								while (fi.StringIndex + 1 < fi.XLSXList.Count)
								{
									StringIndexPlus();
									Reload();
								}
							}
						}
					}
				}
			}

			InputManager.Print("Done!");

			FastReading = false;
			Textbox.Enabled = true;
			Textbox.Visible = true;
			Textbox.ReadOnly = old_read;
			if (dialogue_window != null)
			{
				dialogue_window.DisplayedImage.Enabled = true;
				dialogue_window.DisplayedImage.Visible = true;
			}

			CheckboxDisplayCharacter.Enabled = true;

			UpdateTextbox();

			Reload();
		}

		private void ButtonReloadVariables_Click(object sender, EventArgs e)
		{
			if (AutoPlayOn || FastReading)
			{
				return;
			}

			vm = new VariableManager(Convention);

			ListBoxMenuIndex.Items.Clear();
			ListBoxMenuElements.Items.Clear();

			foreach (string ms in vm.Menu.Items)
			{
				ListBoxMenuIndex.Items.Add(ms);
			}

            bool has_vars = ListBoxMenuIndex.Items.Count > 0;
            ListBoxMenuIndex.Visible = has_vars;
            ListBoxMenuElements.Visible = has_vars;
        }

		private void ButtonSaveAs_Click(object sender, EventArgs e)
		{
			if (!LoadedFile)
			{
				InputManager.Print("You need to open a file first!");
				return;
			}

			CheckUnsaved();

			SaveFileDialog o = new SaveFileDialog();
			var res = o.ShowDialog();
			if (res != DialogResult.OK)
			{
				return;
			}

			var file = o.FileName;

			string ext = Path.GetExtension(file);
			switch (ext)
			{
				case ".vo":
					if (fi.Type == FileManager.LoadedFileType.Txt)
					{
						InputManager.Print("Cannot convert TXT to VO!");
						break;
					}

					if (fi.Type == FileManager.LoadedFileType.Stx)
					{
						fi.StxToVo(file);
					}
					else
					{
						if (fi.Type == FileManager.LoadedFileType.Vo)
						{
							fi.SaveVo(file);
						}
						else
						{
							if (fi.Type == FileManager.LoadedFileType.Xlsx)
							{
								fi.XlsxToVo(file);
							}
						}
					}

					break;
				case ".po":
					if (fi.Type == FileManager.LoadedFileType.Txt)
					{
						InputManager.Print("Cannot convert TXT to PO!");
						break;
					}

					if (fi.Type == FileManager.LoadedFileType.Vo)
					{
						fi.VoToPo(file);
					}

					if (fi.Type == FileManager.LoadedFileType.Stx)
					{
						fi.StxToPo(file);
					}
					else
					{
						if (fi.Type == FileManager.LoadedFileType.Po)
						{
							fi.SavePo(file);
						}
						else
						{
							if (fi.Type == FileManager.LoadedFileType.Xlsx)
							{
								fi.XlsxToPo(file);
							}
						}
					}

					break;
				case ".txt":
					fi.SaveTxt(file, CheckboxDisplayOriginalText.Checked);
					break;
				case ".stx":
					{
						List<string> strings = new List<string>();
						switch (fi.Type)
						{
							case FileManager.LoadedFileType.Vo:
								{
									foreach (VoInternal vo in fi.VoList)
									{
										string str = vo.Translations[fi.SelectedLanguage];
										strings.Add(str);
									}
								}
								break;
							case FileManager.LoadedFileType.Po:
								{
									foreach (PoInternal po in fi.PoList)
									{
										string str = po.MessageString;
										strings.Add(str);
									}
								}
								break;
							case FileManager.LoadedFileType.Txt:
								{
									foreach (TxtInternal txt in fi.TxtList)
									{
										string str = txt.Text;
										if (str != "{" && str != "}")
										{
											strings.Add(str);
										}
									}
								}
								break;
							case FileManager.LoadedFileType.Stx:
								{
									foreach (string sentence in fi.StxFile.Sentences)
									{
										string str = sentence;
										strings.Add(str);
									}
								}
								break;
							case FileManager.LoadedFileType.Xlsx:
								{
									foreach (XLSXRow row in fi.XLSXList)
									{
										string str = row.Translations[fi.SelectedLanguage];
										strings.Add(str);
									}
								}
								break;
						}

						fi.SaveStx(strings, file);
					}
					break;
				case ".xlsx":
					{
						switch (fi.Type)
						{
							case FileManager.LoadedFileType.Txt:
								fi.TxtToXlsx(file);
								break;
							case FileManager.LoadedFileType.Po:
								fi.PoToXlsx(file);
								break;
							case FileManager.LoadedFileType.Vo:
								fi.VoToXlsx(file);
								break;
							case FileManager.LoadedFileType.Stx:
								fi.StxToXlsx(file);
								break;
							case FileManager.LoadedFileType.Xlsx:
								fi.SaveXlsx(file);
								break;
						}
					}
					break;
				default:
					InputManager.Print("Unsupported extension: " + ext);
					break;
			}
		}
	}
}