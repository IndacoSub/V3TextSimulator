namespace DGRV3TS
{
	internal class ImageManager
	{
		public List<string> BackgroundImagesDR = new List<string>();
		public List<string> BackgroundImagesAI = new List<string>();

		public ImageManager(GameIndex game)
		{
			Init(game);
		}

		public string GetWhiteBackground()
		{
			// Graphics/Backgrounds/White.png
			string cur = FileManager.GetCurrentDirectory();
			string gfx = Path.Combine(cur, "Graphics");
			string backgrounds = Path.Combine(gfx, "Backgrounds");
			string ret = Path.Combine(backgrounds, "White.png");
			//MessageBox.Show(ret);
			return ret;
		}

		public void Init(GameIndex game)
		{
			// Graphics/Backgrounds/GAME/*.png

			BackgroundImagesDR = new List<string>();
			BackgroundImagesAI = new List<string>();

			string cur = FileManager.GetCurrentDirectory();

			string gfx = Path.Combine(cur, "Graphics");
			string backgrounds = Path.Combine(gfx, "Backgrounds");
			string bg_final = "";
			switch (game)
			{
				case GameIndex.V3:
					bg_final = Path.Combine(backgrounds, "Danganronpa");	// TODO: Rename this to DR, breaking compatibility with older versions
					break;
				case GameIndex.AI:
					bg_final = Path.Combine(backgrounds, "AI");
					break;
				default:
					break;
			}

			if (!Directory.Exists(backgrounds))
			{
				InputManager.Print("Directory not found: " + backgrounds + "!\n\nThings *WILL* break!");
				return;
			}

			string[] files =
				Directory.GetFiles(bg_final, "*.png", SearchOption.AllDirectories);

			foreach (string file in files)
			{
				if (!file.Contains("Copy") && !file.Contains("Backup") && !file.Contains("Copia"))
				{
					switch (game)
					{
						case GameIndex.V3:
							BackgroundImagesDR.Add(file);
							break;
						case GameIndex.AI:
							BackgroundImagesAI.Add(file);
							break;
						default:
							break;
					}
				}
			}
		}

		public Image ScaleImage(Image image, int maxWidth, int maxHeight)
		{
			if (image == null)
			{
				return null;
			}

			var ratioX = (double)maxWidth / image.Width;
			var ratioY = (double)maxHeight / image.Height;
			var ratio = Math.Min(ratioX, ratioY);

			var newWidth = (int)(image.Width * ratio);
			var newHeight = (int)(image.Height * ratio);

			// Uncomment if somehow this works
			/*
            if(newWidth == maxWidth && maxHeight == newHeight)
            {
                return image;
            }
            */

			var newImage = new Bitmap(newWidth, newHeight);

			using (var graphics = Graphics.FromImage(newImage))
				graphics.DrawImage(image, 0, 0, newWidth, newHeight);

			return newImage;
		}

		public (bool, Bitmap) V3CharacterImageFromString(string ch, FileManager.LoadedFileType lft, 
			string animation, bool dbg)
		{
			// No character? No image
			if (ch.Length == 0)
			{
				return (false, null);
			}

			string cur = FileManager.GetCurrentDirectory();
			string gfx = Path.Combine(cur, "Graphics");
			string sprites = Path.Combine(gfx, "Sprites");
			string drfolder = Path.Combine(sprites, "DR");
			string character = Path.Combine(drfolder, ch);
			string animfile = animation + ".png";
			string characterdefault = Path.Combine(character, "default.png");
			string characterfile = Path.Combine(character, animfile);
			string hatena = Path.Combine(sprites, "chara_Hatena");
			string hatenafile = Path.Combine(hatena, "image.png");
			string blank = Path.Combine(sprites, "chara_Blank");
			string blankfile = Path.Combine(blank, "image.png");

			Bitmap i = null;

			// For now?, *.vo doesn't support animations
			if (lft == FileManager.LoadedFileType.Txt || lft == FileManager.LoadedFileType.Xlsx || string.IsNullOrWhiteSpace(ch))
			{
				if (string.IsNullOrWhiteSpace(ch))
				{
					InputManager.Print("Character is null or whitespace!");
				}

				i = new Bitmap(hatenafile);
				return (true, i);
			}

			// Is the current animation valid?
			bool animexists = animation.Length > 0;
			// Does the default.png exist?
			bool defaultexists = File.Exists(characterdefault);

			// The file that needs to be red
			string actuallyred = "";

			switch (ch)
			{
				case "chara_Hatena":
					animexists = File.Exists(hatenafile);
					if (animexists)
					{
						actuallyred = hatenafile;
						i = new Bitmap(hatenafile);
					}

					break;
				case "chara_Blank":
					animexists = File.Exists(blankfile);
					if (animexists)
					{
						actuallyred = blankfile;
						i = new Bitmap(blankfile);
					}

					break;
				default:
					animexists &= File.Exists(characterfile);
					if (defaultexists)
					{
						if (animexists)
						{
							actuallyred = characterfile;
							i = new Bitmap(characterfile);
						}
						else
						{
							if (File.Exists(characterdefault))
							{
								actuallyred = characterdefault;
								i = new Bitmap(characterdefault);
							}
						}
					}
					else
					{
						if (File.Exists(hatenafile))
						{
							actuallyred = hatenafile;
							i = new Bitmap(hatenafile);
						}
					}

					break;
			}

			if (dbg)
			{
				InputManager.Print("Actually red: " + actuallyred);
			}

			return (animexists, i);
		}
	}
}