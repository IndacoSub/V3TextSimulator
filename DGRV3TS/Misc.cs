using System.Diagnostics;
using System.Media;
using System.Speech.Synthesis;

namespace DGRV3TS
{
	partial class Operations
	{
		bool DoneSpeaking = false;
		// Actual voice file player
		private SoundPlayer SndPlayer = new SoundPlayer();

		private string ApplyGameSpecificHacks(string replaced)
		{
			if (CB_TB.Items.Count <= 0)
			{
				return replaced;
			}

			switch (CurrentGameIndex)
			{
				case GameIndex.AI:
					switch (CB_TB.GetItemText(CB_TB.Items[CB_TB.SelectedIndex]))
					{
						case "ToWitter":
							{
								//replaced = replaced.Replace("\n", "");
								int last_greater = replaced.LastIndexOf('>');
								if (last_greater > 0)
								{
									replaced = replaced.Substring(last_greater + 1);
								}
							}
							break;
					}
					break;
			}

			return replaced;
		}


		private void Reload(bool play_voice = true)
		{
			// Re-create the image

			//ViewFontInfo(Textbox.Font);

			if (Textbox.Text.Length == 0)
			{
				return;
			}

			string old_translation = LoadedFile ? fi.GetCurrentTranslation() : "";

			// TODO: Potentially bad?
            CheckUnsaved();

            string replaced = Textbox.Text;

			if (CheckboxReplaceVariables.Checked)
			{
				replaced = vm.ReplaceVars(replaced);
			}

			replaced = replaced.Replace("\\n", "\n");
			replaced = replaced.Replace("\\\"", "\"");

			replaced = ApplyGameSpecificHacks(replaced);

			CheckInvalidity(replaced);

			// Don't load an unsupported string

			if (LabelUnsupportedWarning.Visible)
			{
				return;
			}

			fm.LoadCurrentFont();

			if(!File.Exists(im.GetWhiteBackground()))
			{
				return;
			}

			Image HB = new Bitmap(im.GetWhiteBackground());

			switch (CurrentGameIndex)
			{
				case GameIndex.V3:
					HB = new Bitmap(im.BackgroundImagesDR[CB_TB.SelectedIndex]);
					break;
				case GameIndex.AI:
					HB = new Bitmap(im.BackgroundImagesAI[CB_TB.SelectedIndex]);
					break;
			}

			int pos_x, pos_y, sc_x, sc_y = 0;
			int full_x = 1280, full_y = 720;

			switch (CurrentGameIndex)
			{
				case GameIndex.V3:
				default:
					pos_x = 260;
					pos_y = 575;

					sc_x = full_x;
					sc_y = full_y;

					fm.CurrentColor = Color.White;
					break;

				case GameIndex.AI:
					{
						switch (CB_TB.GetItemText(CB_TB.Items[CB_TB.SelectedIndex]))
						{
							case "AITutorial":
								pos_x = 335;
								pos_y = 280;

								sc_x = full_x;
								sc_y = full_y;

								fm.CurrentColor = Color.White;
								break;

							case "ToWitter":
								pos_x = 830;
								pos_y = 175;

								sc_x = full_x;
								sc_y = full_y;

								fm.CurrentColor = Color.Black;
								break;

							default:
								pos_x = 355;
								pos_y = 560;

								sc_x = full_x;
								sc_y = full_y;

								fm.CurrentColor = Color.White;
								break;
						}
					}
					break;
			}

			Color COLOR_BACK = Color.Transparent;

			Image ScaledHB = im.ScaleImage(HB, sc_x, sc_y);
			Image MyText = DrawText3(replaced, fm.CurrentFont, COLOR_BACK);
			using (Graphics g = Graphics.FromImage(ScaledHB))
			{
				g.DrawImage(MyText, new Point(pos_x, pos_y));
			}

			if (dialogue_window != null)
			{
				if (dialogue_window.DisplayedImage.Image != null) dialogue_window.DisplayedImage.Image.Dispose();
				dialogue_window.DisplayedImage.Image = ScaledHB.Clone() as Bitmap; // ???
			}

            if (LoadedFile && fi.Type == FileManager.LoadedFileType.Po && !Textbox.Text.Contains(fi.PoList[fi.StringIndex].OriginalMessage))
            {
                //Textbox.Text += "|" + fi.PoList[fi.StringIndex].OriginalMessage;
            }

			if(old_translation != Textbox.Text && vertical_view != null && vertical_view.Visible && LoadedFile && !FastReading)
			{
                OpenVerticalView();
            }

            ScaledHB.Dispose();
			HB.Dispose();
			MyText.Dispose();

			DisplayCharacterImage();

			if (CheckboxPlayVoiceTTS.Checked && play_voice)
			{
				PlayVoice(!CheckboxStartAutoplay.Checked);
			}

			GC.Collect();
		}

		private void StringIndexMinus()
		{
			// Go back one line
			if (!LoadedFile)
			{
				InputManager.Print("You need to open a file first!");
				return;
			}

			if (fi.StringIndex - 1 >= 0)
			{
				fi.StringIndex--;
			}

			UpdateLineCharacter();
			UpdateTextbox();
		}

		private void StringIndexPlus()
		{
			// Go forward one line
			if (!LoadedFile)
			{
				InputManager.Print("You need to open a file first!");
				return;
			}

			if (fi.Type == FileManager.LoadedFileType.Vo)
			{
				if (fi.StringIndex + 1 < fi.VoList.Count)
				{
					fi.StringIndex++;
				}
			}
			else
			{
				if (fi.Type == FileManager.LoadedFileType.Po)
				{
					if (fi.StringIndex + 1 < fi.PoList.Count)
					{
						fi.StringIndex++;
					}
				}
				else
				{
					if (fi.Type == FileManager.LoadedFileType.Txt)
					{
						if (fi.StringIndex + 1 < fi.TxtList.Count)
						{
							fi.StringIndex++;
						}
					}
					else
					{
						if (fi.Type == FileManager.LoadedFileType.Stx)
						{
							if (fi.StringIndex + 1 < fi.StxFile.Sentences.Length)
							{
								fi.StringIndex++;
							}
						}
						else
						{
							if (fi.Type == FileManager.LoadedFileType.Xlsx)
							{
								if (fi.StringIndex + 1 < fi.XLSXList.Count)
								{
									fi.StringIndex++;
								}
							}
						}
					}
				}
			}

			UpdateLineCharacter();
			UpdateTextbox();
		}

		private void LoadOriginalLanguage(bool checkuns)
		{

			if (!LoadedFile)
			{
				return;
			}

			if (fi.Type == FileManager.LoadedFileType.Txt)
			{
				if (checkuns)
				{
					CheckUnsaved();
				}

				return;
			}

			if (CheckboxDisplayOriginalText.Checked)
			{
				if (!Textbox.ReadOnly && checkuns)
				{
					CheckUnsaved();
				}

				if (!Textbox.Enabled)
				{
					return;
				}

				Textbox.ReadOnly = true;

				if (fi.Type == FileManager.LoadedFileType.Vo)
				{
					if (Textbox.Text != fi.VoList.ElementAt(fi.StringIndex).OriginalMessage)
					{
						Textbox.Text = fi.VoList.ElementAt(fi.StringIndex).OriginalMessage;
					}
				}
				else
				{
					if (fi.Type == FileManager.LoadedFileType.Po)
					{
						if (Textbox.Text != fi.PoList.ElementAt(fi.StringIndex).OriginalMessage)
						{
							Textbox.Text = fi.PoList.ElementAt(fi.StringIndex).OriginalMessage;
						}
					}
					else
					{
						if (fi.Type == FileManager.LoadedFileType.Xlsx)
						{
							if (Textbox.Text != fi.XLSXList.ElementAt(fi.StringIndex).Original)
							{
								Textbox.Text = fi.XLSXList.ElementAt(fi.StringIndex).Original;
							}
						}
					}
				}
			}
			else
			{
				if (!Textbox.Enabled)
				{
					return;
				}

				Textbox.ReadOnly = false;

				if (fi.Type == FileManager.LoadedFileType.Vo)
				{
					Textbox.Text = fi.VoList.ElementAt(fi.StringIndex).Translations.ElementAt(fi.SelectedLanguage);
				}
				else
				{
					if (fi.Type == FileManager.LoadedFileType.Po)
					{
						Textbox.Text = fi.PoList.ElementAt(fi.StringIndex).MessageString;
					}
					else
					{
						if (fi.Type == FileManager.LoadedFileType.Xlsx)
						{
							Textbox.Text = fi.XLSXList.ElementAt(fi.StringIndex).Translations.ElementAt(fi.SelectedLanguage);
						}
					}
				}
			}
		}

		private void SaveChanges()
		{
			if (!LoadedFile)
			{
				InputManager.Print("You need to open a file first!");
				return;
			}

			switch (fi.Type)
			{
				case FileManager.LoadedFileType.Vo:
					fi.VoList[fi.StringIndex].Translations[fi.SelectedLanguage] = Textbox.Text;
					break;
				case FileManager.LoadedFileType.Po:
					fi.PoList[fi.StringIndex].MessageString = Textbox.Text;
					break;
				case FileManager.LoadedFileType.Xlsx:
					fi.XLSXList[fi.StringIndex].Translations[fi.SelectedLanguage] = Textbox.Text;
					break;
			}
		}

		private void AutoPlay()
		{
			AutoPlayOn = true;
			int count = 0;

			switch (fi.Type)
			{
				case FileManager.LoadedFileType.Vo:
					count = fi.VoList.Count;
					break;
				case FileManager.LoadedFileType.Po:
					count = fi.PoList.Count;
					break;
				case FileManager.LoadedFileType.Txt:
					count = fi.TxtList.Count;
					break;
				case FileManager.LoadedFileType.Stx:
					count = fi.StxFile.Sentences.Length;
					break;
				case FileManager.LoadedFileType.Xlsx:
					count = fi.XLSXList.Count;
					break;
			}

			while (fi.StringIndex + 1 < count)
			{
				if (!CheckboxStartAutoplay.Checked)
				{
					return;
				}

				if (CheckboxPlayVoiceTTS.Checked)
				{
					Debug.WriteLine("Entering loop -- line " + fi.StringIndex);
					while (!DoneSpeaking)
					{
						// Wait
					}
					Debug.WriteLine("Exiting loop -- line " + fi.StringIndex);
				}
				else
				{
					// TODO: 2500 should be enough
					Thread.Sleep(2500);
				}

				if (!CheckboxStartAutoplay.Checked)
				{
					return;
				}

				if (CheckboxPauseAutoplay.Checked)
				{
					AutoPlayOn = false;
				}

				while (CheckboxPauseAutoplay.Checked)
				{
				}

				if (!CheckboxStartAutoplay.Checked)
				{
					return;
				}

				UpdateLineCharacter();
				UpdateTextbox();
				if (dialogue_window != null)
				{
					dialogue_window.DisplayedImage.Refresh();
				}

				Debug.WriteLine("Line" + fi.StringIndex + " completed.");

				if (fi.StringIndex + 1 < count - 1)
				{
					fi.StringIndex++;
				} else
				{
					break;
				}
			}

			Debug.WriteLine("Exiting Autoplay");

			this.CheckboxPauseAutoplay.Checked = true;
			this.CheckboxStartAutoplay.Checked = false;
			this.CheckboxPauseAutoplay.Checked = false;
			this.Refresh();
		}

		private void CheckUnsaved()
		{
			if (!LoadedFile)
			{
				return;
			}

			if (!Textbox.ReadOnly)
			{
				if (fi.Type == FileManager.LoadedFileType.Vo)
				{
					if (fi.VoList.ElementAt(fi.StringIndex).Translations.ElementAt(fi.SelectedLanguage) != Textbox.Text)
					{
						SaveChanges();
					}
				}
				else
				{
					if (fi.Type == FileManager.LoadedFileType.Po)
					{
						if (fi.PoList.ElementAt(fi.StringIndex).MessageString != Textbox.Text)
						{
							SaveChanges();
						}
					}
					else
					{
						if (fi.Type == FileManager.LoadedFileType.Txt)
						{
							if (fi.TxtList.ElementAt(fi.StringIndex).Text != Textbox.Text)
							{
								SaveChanges();
							}
						}
						else
						{
							if (fi.Type == FileManager.LoadedFileType.Stx)
							{
								if (fi.StxFile.Sentences[fi.StringIndex] != Textbox.Text)
								{
									SaveChanges();
								}
							}
							else
							{
								if (fi.Type == FileManager.LoadedFileType.Xlsx)
								{
									if (fi.XLSXList.ElementAt(fi.StringIndex).Translations.ElementAt(fi.SelectedLanguage) != Textbox.Text)
									{
										SaveChanges();
									}
								}
							}
						}
					}
				}
			}
		}

		public void PlayVoice(bool async)
		{
			if (!LoadedFile)
			{
				return;
			}

			// Only .vo?, .po and .stx+wrd files support voicelines
			if (fi.Type == FileManager.LoadedFileType.Txt || fi.Type == FileManager.LoadedFileType.Xlsx)
			{
				return;
			}

			(string ch, string origin, string anim, string voice) = GetCharacterAndOrigin();
			if (ch == null || origin == null || anim == null || voice == null)
			{
				return;
			}

			string game = "";
			switch (CurrentGameIndex)
			{
				case GameIndex.V3:
					game = "DR";
					break;
				case GameIndex.AI:
					game = "AI";
					break;
				default:
					game = "Unknown";
					break;
			}

			VoiceGender gender = VoiceGender.NotSet;
			VoiceAge age = VoiceAge.NotSet;
			switch (CurrentGameIndex) {
				case GameIndex.V3:
					gender = sm.V3GenderByCharacter(ch, origin);
					age = sm.V3AgeByCharacter(ch);
					break;
				case GameIndex.AI:
				default:
					break;
			}

			if(gender == VoiceGender.NotSet || age == VoiceAge.NotSet)
			{
				return;
			}

			string rawtext = Textbox.Text;
			string ogtext = "";
			switch (fi.Type)
			{
				case FileManager.LoadedFileType.Vo:
					ogtext = fi.VoList[fi.StringIndex].OriginalMessage;
					break;
				case FileManager.LoadedFileType.Po:
					ogtext = fi.PoList[fi.StringIndex].OriginalMessage;
					break;
				case FileManager.LoadedFileType.Txt:
				case FileManager.LoadedFileType.Stx:
				case FileManager.LoadedFileType.Xlsx:
				default:
					ogtext = rawtext;
					break;
			}

			// Default to en-US
			string language = TextboxCurrentLanguage.Text;
			if (rawtext == ogtext && rawtext.Length >= 5)
			{
				language = "en-US";
			}

			// Treat newlines as spaces
			string text = rawtext.Replace("\\n", " ");
			// Treat \" as "
			text = text.Replace("\\\"", "\"");
			if (CheckboxReplaceVariables.Checked)
			{
				text = vm.ReplaceVars(text);
			}

			// Replace signals and CLTs
			text = VariableManager.ReplaceCLTs(text);
			text = VariableManager.ReplaceSignals(text);
			// Once again treat newlines as spaces, possibly for signals
			text = text.Replace("\\n", " ");

			DoneSpeaking = false;

			if (SoundManager.SoundFileExists(voice, game) && !FastReading)
			{

				// Actual voice file
				SndPlayer.Stop();
				sm.Synthesizer.Pause();
				sm.Synthesizer.SpeakAsyncCancelAll();
				sm.Synthesizer = new SpeechSynthesizer();

				SndPlayer = new SoundPlayer();
				SndPlayer.SoundLocation = SoundManager.GetSoundFileByName(voice, game);
				SndPlayer.Load();
				if (AutoPlayOn)
				{
					SndPlayer.PlaySync();
				} else
				{
					SndPlayer.Play();
				}
				DoneSpeaking = true;
			}
			else
			{
				bool debate_mode = AutoPlayOn && tm.IsDebateFile(fi.LoadedFileName);
				// TTS
				if (!debate_mode)
				{
					SndPlayer.Stop();
					sm.Synthesizer.Pause();
					sm.Synthesizer.SpeakAsyncCancelAll();

					sm.Synthesizer = new SpeechSynthesizer();
					sm.PlayVoice(text.Replace("_MN", ""), language, gender, age, async);
				}
				DoneSpeaking = true;
			}
		}
	}
}