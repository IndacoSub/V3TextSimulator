using System.Drawing.Drawing2D;

namespace DGRV3TS
{
	partial class Operations
	{
		public (string, string, string, string) GetCharacterAndOrigin()
		{
			string ch = "";
			string origin = "";
			string expression = "";
			string voice = "";

			switch (fi.Type)
			{
				case FileManager.LoadedFileType.Vo:
					ch = fi.VoList.ElementAt(fi.StringIndex).Character;
					origin = fi.VoList.ElementAt(fi.StringIndex).OriginFile;
					break;

				case FileManager.LoadedFileType.Po:
					ch = fi.PoList.ElementAt(fi.StringIndex).Character;
					origin = fi.PoList.ElementAt(fi.StringIndex).OriginFile;
					expression = fi.PoList.ElementAt(fi.StringIndex).Expression;
					voice = fi.PoList.ElementAt(fi.StringIndex).Voiceline;

					if (voice.Length > 0)
					{
						LabelVoiceline.Text = "Voiceline: " + voice;
					}
					else
					{
						LabelVoiceline.Text = "Voiceline: None";
					}

					LabelCharacterName.Visible = ch != "DefaultCharacter";
					LabelOriginFile.Visible = origin != "DefaultOriginFile";
					LabelCurrentAnimation.Visible = expression != "DefaultExpression";
					LabelCurrentAnimation.Text = "Current Animation: " + expression;
					break;

				case FileManager.LoadedFileType.Stx:
					// The WRD file is needed
					if (fi.StxFile.LoadedWRD != null)
					{
						ch = fi.StxFile.CharacterByLineNumber(fi.StringIndex);
						expression = fi.StxFile.ExpressionByLineNumber(fi.StringIndex, ch);
						LabelCurrentAnimation.Text = "Current Animation: " + expression;
						origin = Path.GetFileName(fi.LoadedFileName);
						voice = fi.StxFile.VoicelineByLineNumber(fi.StringIndex);
						if (voice.Length > 0)
						{
							LabelVoiceline.Text = "Voiceline: " + voice;
						}
						else
						{
							LabelVoiceline.Text = "Voiceline: None";
						}
					}
					break;
			}

			return (ch, origin, expression, voice);
		}

		private bool V3Draw(string ch, string expression)
		{

			// Are you using full-body sprites?
			const bool using_full_sprites = true;
			Point point = new Point(0, 0);
			using (Graphics g = Graphics.FromImage(dialogue_window.DisplayedImage.Image))
			{
				(bool ret_img, Bitmap cc, string str) = im.V3CharacterImageFromString(ch, fi.Type, expression, DEBUG_ON);

                // Set up the ToolTip text for the Button and Checkbox.
                toolTip1.SetToolTip(this.LabelCurrentAnimation, "File red: " + str);

                if (cc == null)
				{
					//InputManager.Print("Character image is null!");
					return false;
				}

				// kak = flashback?
				if (using_full_sprites && !ch.Contains("_kak") && expression.Length > 0)
				{
					switch (ch)
					{
						case "chara_Blank":
						case "non":
						case "None":
						case "":
						case "chara_Hatena":
						// Remove this case (only the case) if you obtained full-body sprites for Maki
						case "C010_Haruk":
							point.Y = dialogue_window.DisplayedImage.Size.Height - cc.Height;
							break;
						case "C020_Monok":
						case "C021_Mtaro":
						case "C022_Msuke":
						case "C023_Mfunn":
						case "C024_Mdam_":
						case "C025_Mkid_":
							cc = (Bitmap)im.ScaleImage(cc, 440, 440);
							point.X = -120;
							point.Y = dialogue_window.DisplayedImage.Size.Height - cc.Height / 2;
							point.Y -= 100;
							break;
						default:
							cc = (Bitmap)im.ScaleImage(cc, 430, 680);
							point.X = -60;
							point.Y = dialogue_window.DisplayedImage.Size.Height - cc.Height / 2;
							point.Y -= 10;
							break;
					}
				}
				else
				{
					if (using_full_sprites)
					{
						if (expression.Length == 0)
						{
							cc = (Bitmap)im.ScaleImage(cc, 280, 280);
						}
					}
					point.Y = dialogue_window.DisplayedImage.Size.Height - cc.Height;
				}

				g.CompositingMode = CompositingMode.SourceOver;
				g.DrawImage(cc, point);
				g.Save();
				cc.Dispose();
				//InputManager.Print("Drew at: " + point.X + ", " + point.Y);

				return ret_img;
			}
		}

		private void DisplayCharacterImage()
		{
			if (!LoadedFile)
			{
				return;
			}

			// Debug needs this to work
			if (FastReading && !DEBUG_ON)
			{
				return;
			}

			if (dialogue_window == null)
			{
				return;
			}

			bool loaded_img = false;

			if (CheckboxDisplayCharacter.Checked)
			{
				(string ch, string origin, string expression, string voice) =
					GetCharacterAndOrigin();

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

				bool ret_snd = SoundManager.SoundFileExists(voice, game);

				if (fi.Type == FileManager.LoadedFileType.Txt ||
					(fi.Type == FileManager.LoadedFileType.Stx && fi.StxFile != null && fi.StxFile.LoadedWRD == null) ||
					fi.Type == FileManager.LoadedFileType.Xlsx ||
					ch.Length == 0
					)
				{
					// Don't load image
					//_ = im.CharacterImageFromString(ch, fi.Type, origin, expression, DEBUG_ON);
				}
				else
				{
					if (dialogue_window.DisplayedImage == null || dialogue_window.DisplayedImage.Image == null)
					{
						return;
					}

					switch (CurrentGameIndex)
					{
						case GameIndex.V3:
							loaded_img = V3Draw(ch, expression);
							break;
						case GameIndex.AI:
						default:
							break;
					}
				}

				// Extensions supporting animations and voicelines
				if (fi.Type == FileManager.LoadedFileType.Stx || fi.Type == FileManager.LoadedFileType.Po)
				{
                    // If the image was successfully loaded
                    if (loaded_img)
					{
						LabelCurrentAnimation.ForeColor = Color.Black;
					}
					else
					{
						LabelCurrentAnimation.ForeColor = Color.Red;

						if (DEBUG_ON && expression != "" && expression != "non" && expression != "None")
						{
							// debug
							InputManager.Print("Unsupported animation for " + ch + ": \"" + expression +
											   "\" (file: " + origin + ", line: " + LabelLineNumber.Text +
											   ")\n\""
											   + Textbox.Text + "\"");
						}
					}

					// If the voiceline was successfully loaded
					if (ret_snd || (voice != null && voice.Length == 0))
					{
						LabelVoiceline.ForeColor = Color.Black;
					}
					else
					{
						LabelVoiceline.ForeColor = Color.Red;
					}
				}
			}

			dialogue_window.DisplayedImage.Refresh();
		}
	}
}