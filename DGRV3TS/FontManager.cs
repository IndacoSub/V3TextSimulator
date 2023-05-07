using System.Diagnostics;
using System.Drawing.Text;

namespace DGRV3TS
{
	internal class FontManager
	{
		public const int FirstFont = 0;
		public Color CurrentColor = Color.White;
		public Font CurrentFont;
		public float CurrentFontSize;
		public PrivateFontCollection CustomFonts;

		// Use Calibri for the displayed text
		public string DefaultFontName = "Calibri";

		// Use Microsoft Sans Serif for the editable text box
		public string DefaultFontNameForText = "Microsoft Sans Serif";
		public List<Font> FontList;

		public string FontName = "";
		public float FontSize;
		public int FontSizeForText = 14;
		private bool LoadedFont;

		public FontManager(string init_font)
		{
			LoadFont(init_font);
		}

		public void LoadByCSFont(string familyname, float fsize, FontStyle style)
		{
			CurrentFont = new Font(familyname, fsize, style);
			CurrentFontSize = fsize;
		}

		public void LoadFont(string init_font)
		{
			CustomFonts = new PrivateFontCollection();
			FontSize = new float();
			FontSize = 0.0f;

			FontList = new List<Font>();

			FontSize = 29.0f;

			CurrentFontSize = FontSize;

			FontName = DefaultFontName;

			// The ideal font for V3 would be FOT-HummingStd-D (I guess?)
			if (File.Exists("font.otf"))
			{
				CustomFonts.AddFontFile("font.otf");
			}
			else
			{
				if (File.Exists("font.ttf"))
				{
					CustomFonts.AddFontFile("font.ttf");
				}
				else
				{
					string file = init_font;
					LoadCustomFont(file);
				}
			}

			if (CustomFonts.Families.Length > 0)
			{
				LoadedFont = true;
			}

			if (LoadedFont)
			{
				FontFamily[] fontFamilies = CustomFonts.Families;

				for (int j = 0, count = fontFamilies.Length; j < count; ++j)
				{
					// Get the font family name.
					string familyName = fontFamilies[j].Name;

					FontName = familyName;

					LoadRegular(fontFamilies[j], familyName);

					LoadBold(fontFamilies[j], familyName);

					LoadItalic(fontFamilies[j], familyName);

					LoadBoldItalic(fontFamilies[j], familyName);

					LoadUnderline(fontFamilies[j], familyName);

					LoadStrikeout(fontFamilies[j], familyName);
				}
			}
		}

		public void LoadCurrentFont()
		{
			CurrentFont = LoadedFont ? FontList[FirstFont] : new Font(DefaultFontName, FontSize);
		}

		public void LoadCustomFont(string filename)
		{
			var file = filename;
			if ((!file.ToLowerInvariant().Contains(".ttf") && !file.ToLowerInvariant().Contains(".otf")) || file.Length == 0)
			{
				Debug.WriteLine("Could not load font!");
				return;
			}

			CustomFonts.AddFontFile(file);
		}

		public void LoadRegular(FontFamily ff, string fn)
		{
			// Is the regular style available?
			if (ff.IsStyleAvailable(FontStyle.Regular))
			{
				Font regFont = new Font(
					fn,
					FontSize,
					FontStyle.Regular,
					GraphicsUnit.Pixel);

				FontList.Add(regFont);
			}
		}

		public void LoadBold(FontFamily ff, string fn)
		{
			// Is the bold style available?
			if (ff.IsStyleAvailable(FontStyle.Bold))
			{
				Font boldFont = new Font(
					fn,
					FontSize,
					FontStyle.Bold,
					GraphicsUnit.Pixel);

				FontList.Add(boldFont);
			}
		}

		public void LoadItalic(FontFamily ff, string fn)
		{
			// Is the italic style available?
			if (ff.IsStyleAvailable(FontStyle.Italic))
			{
				Font italicFont = new Font(
					fn,
					FontSize,
					FontStyle.Italic,
					GraphicsUnit.Pixel);

				FontList.Add(italicFont);
			}
		}

		public void LoadBoldItalic(FontFamily ff, string fn)
		{
			// Is the bold italic style available?
			if (ff.IsStyleAvailable(FontStyle.Italic) && ff.IsStyleAvailable(FontStyle.Bold))
			{
				Font italicFont = new Font(
					fn,
					FontSize,
					FontStyle.Italic | FontStyle.Bold,
					GraphicsUnit.Pixel);

				FontList.Add(italicFont);
			}
		}

		public void LoadUnderline(FontFamily ff, string fn)
		{
			// Is the underline style available?
			if (ff.IsStyleAvailable(FontStyle.Underline))
			{
				Font underlineFont = new Font(
					fn,
					FontSize,
					FontStyle.Underline,
					GraphicsUnit.Pixel);

				FontList.Add(underlineFont);
			}
		}

		public void LoadStrikeout(FontFamily ff, string fn)
		{
			// Is the strikeout style available?
			if (ff.IsStyleAvailable(FontStyle.Strikeout))
			{
				Font strikeFont = new Font(
					fn,
					FontSize,
					FontStyle.Strikeout,
					GraphicsUnit.Pixel);

				FontList.Add(strikeFont);
			}
		}

		public Font CreateFont(string name, int size, FontStyle style)
		{
			Font replacementFont = new Font(name, size, style);
			return replacementFont;
		}

		public bool ChangeBasedOnCLT(string str)
		{
			bool ret = false;

			if (str.Contains("<CLT=cltSYSTEM>"))
			{
				ret = true;
				CurrentColor = Color.Green;
			}

			if (str.Contains("<CLT=cltMIND>"))
			{
				ret = true;
				CurrentColor = Color.Aqua;
			}

			if (str.Contains("<CLT=cltSTRONG>"))
			{
				ret = true;
				CurrentColor = Color.Orange;
			}

			if (str.Contains("<CLT=cltNORMAL>") || str.Contains("<CLT=typeNORMAL>"))
			{
				ret = true;
				CurrentColor = Color.White;
				CurrentFontSize = FontSize;
			}

			if (str.Contains("<CLT=cltWEAK>"))
			{
				ret = true;
				CurrentColor = Color.Gold;
			}

			if (str.Contains("<CLT=size1>"))
			{
				ret = true;
				CurrentFontSize = FontSize;
			}

			if (str.Contains("<CLT=size1.1>"))
			{
				ret = true;
				CurrentFontSize = FontSize + 0.5f;
			}

			if (str.Contains("<CLT=size1.2>"))
			{
				ret = true;
				CurrentFontSize = FontSize + 0.7f;
			}

			if (str.Contains("<CLT=size1.5>"))
			{
				ret = true;
				CurrentFontSize = FontSize + 1.0f;
			}

			if (str.Contains("<CLT=size2>"))
			{
				ret = true;
				CurrentFontSize = FontSize + 2.0f;
			}

			if (str.Contains("<CLT=size2.2>"))
			{
				ret = true;
				CurrentFontSize = FontSize + 2.2f;
			}

			if (str.Contains("<CLT=size3>"))
			{
				ret = true;
				CurrentFontSize = FontSize + 3.0f;
			}

			if (str.Contains("<CLT=size4>"))
			{
				ret = true;
				CurrentFontSize = FontSize + 4.0f;
			}

			if (str.Contains("<CLT=size6>"))
			{
				ret = true;
				CurrentFontSize = FontSize + 6.0f;
			}

			return ret;
		}

		/*

        private string GetFontInfo(Font f)
        {
            string name = "Name: " + f.Name + "\n";
            string family = "Family: " + f.FontFamily.ToString() + "\n";
            string size = "Size: " + f.Size.ToString() + "\n";
            string style = "Style: " + f.Style + "\n";
            string charset = "GdiCharSet: " + f.GdiCharSet.ToString() + "\n";
            string vertical = "GdiVerticalFont: " + f.GdiVerticalFont.ToString() + "\n";
            string height = "Height: " + f.Height + "\n";
            string ogname = "OriginalFontName: " + f.OriginalFontName + "\n";
            return name + family + size + style + charset + vertical + height + ogname;
        }

        private void ViewFontInfo(Font f)
        {
            InputManager.Print(GetFontInfo(f));
        }

        */
	}
}