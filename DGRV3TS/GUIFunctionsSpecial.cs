namespace DGRV3TS
{
	partial class Operations
	{
		private void Textbox_TextChanged(object sender, EventArgs e)
		{
			CheckInvalidity(Textbox.Text);
		}

		private void NumericUpDownFontSize_ValueChanged(object sender, EventArgs e)
		{
			if (NumericUpDownFontSize.Value <= 0)
			{
				NumericUpDownFontSize.Value = 1;
			}

			fm.FontSize = (float)NumericUpDownFontSize.Value;
			Reload();
		}

		private void RecreateImageBase()
		{
			if (dialogue_window == null)
			{
				return;
			}
			if (dialogue_window.DisplayedImage.Image != null) dialogue_window.DisplayedImage.Image.Dispose();

			switch (CurrentGameIndex)
			{
				case GameIndex.V3:
					{
						if(im.BackgroundImagesDR.Count <= 0)
						{
							return;
						}
						dialogue_window.DisplayedImage.Image = new Bitmap(new Bitmap(im.BackgroundImagesDR[CB_TB.SelectedIndex]),
							new Size(dialogue_window.DisplayedImage.Width, dialogue_window.DisplayedImage.Height));
					}
					break;
				case GameIndex.AI:
					{
						if(im.BackgroundImagesAI.Count <= 0)
						{
							return;
						}
						dialogue_window.DisplayedImage.Image = new Bitmap(new Bitmap(im.BackgroundImagesAI[CB_TB.SelectedIndex]),
							new Size(dialogue_window.DisplayedImage.Width, dialogue_window.DisplayedImage.Height));

						switch (CB_TB.GetItemText(CB_TB.Items[CB_TB.SelectedIndex]))
						{
							case "AIBlank":
								NumericUpDownFontSize.Value = 28;
								fm.FontSize = 28;
								break;
							case "AITutorial":
								NumericUpDownFontSize.Value = 25;
								fm.FontSize = 25;
								break;
							case "ToWitter":
								NumericUpDownFontSize.Value = 10;
								fm.FontSize = 10;
								break;
						}
					}
					break;
			}

			//DisplayedImage.Update();
			Reload(false);
		}

		private void CB_TB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GC.Collect();
			RecreateImageBase();
		}

		private void CB_Game_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Change game

			var last = CurrentGameIndex;
			CurrentGameIndex = (GameIndex)CB_Game.SelectedIndex;
			im = new ImageManager(CurrentGameIndex);
			InitCBTB(CurrentGameIndex);
			if (fm.FontSize == 29 && last == GameIndex.V3 && CurrentGameIndex == GameIndex.AI)
			{
				NumericUpDownFontSize.Value = 28;
				fm.FontSize = 28;
			}
			else if ((fm.FontSize == 28 || fm.FontSize == 10) && last == GameIndex.AI && CurrentGameIndex == GameIndex.V3)
			{
				NumericUpDownFontSize.Value = 29;
				fm.FontSize = 29;
			}

			// Game-specific support for character/expression/voiceline etc.
			switch (CurrentGameIndex)
			{
				case GameIndex.V3:
					LabelCharacterName.Visible = true;
					LabelCurrentAnimation.Visible = true;
					LabelOriginFile.Visible = true;
					LabelVoiceline.Visible = true;
					break;
				case GameIndex.AI:
					LabelCharacterName.Visible = false;
					LabelCurrentAnimation.Visible = false;
					LabelOriginFile.Visible = false;
					LabelVoiceline.Visible = false;
					break;
			}
		}

		private void SetupSpritesButton_Click(object sender, EventArgs e)
		{
			/*
			From a folder containing extracted sprites of every character (except Maki?),
			copy the files from those folders to the Graphics/Sprites/ folder
			based on the ID of the character
			*/

			var character_ids = new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("Shuichi Saihara", "C000_Saiha"),
				new Tuple<string, string>("Kaito Momota", "C001_Momot"),
				new Tuple<string, string>("Ryoma Hoshi", "C002_Hoshi"),
				new Tuple<string, string>("Rantaro Amami", "C003_Amami"),
				new Tuple<string, string>("Gonta Gokuhara", "C004_Gokuh"),
				new Tuple<string, string>("Kokichi Oma", "C005_Oma__"),
				new Tuple<string, string>("Korekiyo Shinguji", "C006_Shing"),
				new Tuple<string, string>("Keebo", "C007_Ki-Bo"),
				// Should be "Tojo" actually
				new Tuple<string, string>("Kirumi Toujo", "C008_Tojo_"),
				new Tuple<string, string>("Himiko Yumeno", "C009_Yumen"),
				new Tuple<string, string>("Maki Harukawa", "C010_Haruk"),
				new Tuple<string, string>("Tenko Chabashira", "C011_Chaba"),
				new Tuple<string, string>("Tsumugi Shirogane", "C012_Shiro"),
				new Tuple<string, string>("Angie Yonaga", "C013_Yonag"),
				new Tuple<string, string>("Miu Iruma", "C014_Iruma"),
				new Tuple<string, string>("Kaede Akamatsu", "C015_Akama"),
				new Tuple<string, string>("Monokuma", "C020_Monok"),
				new Tuple<string, string>("Monotaro", "C021_Monot"),
				new Tuple<string, string>("Monosuke", "C022_Msuke"),
				new Tuple<string, string>("Monophanie", "C023_Mfunn"),
				new Tuple<string, string>("Monodam", "C024_Mdam_"),
				new Tuple<string, string>("Monokid", "C025_Mkid_"),
			};

			// First, let's open the folder

			string copy_from_folder = "";

			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult res = fbd.ShowDialog();
				if (res != DialogResult.OK)
				{
					return;
				}
				copy_from_folder = fbd.SelectedPath;
			}

			// Does the folder exist?

			if (!Directory.Exists(copy_from_folder))
			{
				InputManager.Print("Invalid folder!");
				return;
			}

			// Does it contain any stand*.png files?

			var files = Directory.GetFiles(copy_from_folder, "stand*.png", SearchOption.AllDirectories);
			if (files.Length <= 0)
			{
				InputManager.Print("No sprites found?");
				return;
			}

			// We want to copy to the /Graphics/Sprites/CHARTACTER/ folder

			string copy_to_folder = FileManager.GetCurrentDirectory();
			copy_to_folder = Path.Combine(copy_to_folder, "Graphics");
			copy_to_folder = Path.Combine(copy_to_folder, "Sprites");

			foreach(string file in files)
			{
				string filename = Path.GetFileName(file);
				string actual_copy_to_folder = "";

				if(file.Contains("Tsumugi Cosplay Sprites"))
				{
					continue;
				}

				if (file.Contains("Fixed Sprites"))
				{
					switch (filename)
					{
						case "stand_000_320.png":
						case "stand_000_331.png":
							actual_copy_to_folder = Path.Combine(copy_to_folder, "C000_Saiha");
							break;
						case "stand_025_010.png":
							actual_copy_to_folder = Path.Combine(copy_to_folder, "C025_Mkid_");
							break;
						default:
							InputManager.Print("Unknown fixed sprite: " + file);
							return;
					}
				}
				else
				{
					if (file.Contains("Tsumugi Cosplay"))
					{
						actual_copy_to_folder = Path.Combine(copy_to_folder, "C012_Shiro");
					}
					else
					{
						// Find the name in the array
						var it = character_ids.Find(x => file.Contains(x.Item1));
						if (it == null)
						{
							continue;
						}
						int index = character_ids.IndexOf(it);
						if (index <= -1 || index >= character_ids.Count)
						{
							continue;
						}

						actual_copy_to_folder = Path.Combine(copy_to_folder, it.Item2);
					}
				}

				if(!Directory.Exists(actual_copy_to_folder))
				{
					Directory.CreateDirectory(actual_copy_to_folder);
				}

				// stand*.png -> anim*.png
				int last_underscore_index = filename.LastIndexOf('_');
				if(last_underscore_index == -1)
				{
					continue;
				}
				filename = filename.Substring(last_underscore_index + 1);
				filename = filename.Insert(0, "anim");

				string copy_file = Path.Combine(actual_copy_to_folder, filename);
				if(File.Exists(copy_file))
				{
					File.Delete(copy_file);
				}
				File.Copy(file, copy_file, true);
			}

			InputManager.Print("Done!");
		}

		private void SetupVoicelinesButton_Click(object sender, EventArgs e)
		{
			InputManager.Print("Currently unimplemented!");
		}
	}
}