using System.Text.RegularExpressions;

namespace DGRV3TS
{
    public class TranslationManager
	{
		private readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
		private readonly Regex cjkExt1CharRegex = new Regex(@"\p{IsCJKUnifiedIdeographsExtensionA}");
		private readonly Regex cjkGrammar = new Regex(@"\p{IsCJKSymbolsandPunctuation}");
		private readonly Regex HiraganaCharRegex = new Regex(@"\p{IsHiragana}");
		private readonly Regex KatakanaCharRegex = new Regex(@"\p{IsKatakana}");
		public TLDictionary Dictionary = new TLDictionary();

		// Array that contains the files that have been translated
		// The files should be manually inserted
		public List<string> TranslatedFiles;

		public TranslationManager()
		{
			LoadTranslatedFiles();
		}

		public bool IsDebateFile(string file)
		{
			List<string> debate_files = new List<string>()
			{
				// Chapter 1
				"c01_202_001",
				"c01_204_001",
				"c01_206_001",
				"c01_206_002",
				"c01_206_003",
				"c01_208_001",
				"c01_210_001",
				"c01_212_001",
				"c01_214_001",
				"c01_218_001",
				"c01_220_001",
				"c01_222_001",
				"c01_225_001",
				"c01_229_001",
				"c01_231_001",
			};

			return debate_files.Contains(Path.GetFileNameWithoutExtension(file));
		}

		public void LoadTranslatedFiles()
		{
			// List of translated files
			TranslatedFiles = new List<string>
			{
				//"c00_000_001",
				//"c00_000_002",
				//"c00_000_004",
				//"c00_000_007",
				//"c00_000_010",
				//"c00_000_011",
				//"c00_000_015",
				//"c00_000_016",
				//"c00_000_017",
				//"c00_000_018",
				//"c00_000_019",
				//"c00_000_020",
				//"c00_000_025",
				//"c00_000_042",
				//"c00_000_100",
				//"c00_000_250",
				//"c00_000_251",
				//"c00_000_253",
				//"c00_001_018",
				//"c00_002_018",
				//"c00_003_002",
				//"c00_004_007",
				//"c00_005_018",
				//"c00_006_018",
				//"c00_007_001",
				//"c00_007_002",
				//"c00_007_004",
				//"c00_007_010",
				//"c00_007_011",
				//"c00_007_015",
				//"c00_007_016",
				//"c00_007_017",
				//"c00_007_018",
				//"c00_007_019",
				//"c00_007_020",
				//"c00_007_025",
				//"c00_007_042",
				//"c00_008_001",
				//"c00_008_100",
				//"c00_008_250",
				//"c00_008_251",
				//"c00_008_253",
				//"c00_009_007",
				//"c00_999_001",
				//"c00_999_002",
				//"c00_999_003",
			};
		}

		private bool IsCjk(char c)
		{
			return cjkCharRegex.IsMatch(c.ToString());
		}

		private bool IsCjkGrammar(char c)
		{
			return cjkGrammar.IsMatch(c.ToString());
		}

		private bool IsHiragana(char c)
		{
			return HiraganaCharRegex.IsMatch(c.ToString());
		}

		private bool IsKatakana(char c)
		{
			return KatakanaCharRegex.IsMatch(c.ToString());
		}

		private bool IsCjkExt1(char c)
		{
			return cjkExt1CharRegex.IsMatch(c.ToString());
		}

		public bool IsJapanese(string str)
		{
			foreach (char z in str)
			{
				if (IsCjk(z))
				{
					return true;
				}

				if (IsCjkGrammar(z))
				{
					return true;
				}

				if (IsHiragana(z))
				{
					return true;
				}

				if (IsKatakana(z))
				{
					return true;
				}

				if (IsCjkExt1(z))
				{
					return true;
				}
			}

			return false;
		}
	}
}