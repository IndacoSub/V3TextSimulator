namespace DGRV3TS
{
	internal class VoInternal
	{
		private enum Indexes
		{
			InternalLineIndex = 6,
			CommentsNumberIndex = 19,
			FileIndex = 6,
			LineIndex = 5,
			CharacterIndex = 11,
			OgMessageLengthIndex = 24,
			OgMessageIndex = 18,
			TranslationNumberIndex = 24
		}

		public string Character = "DefaultCharacter";
		public List<string> Comments = new List<string>();
		public int CommentsNumber;
		public int InternalLines;
		public int LineNumber;
		public string OriginalMessage = "DefaultOriginal";
		public int OriginalMessageLength;
		public string OriginFile = "DefaultFile";
		public bool RedC;

		public bool RedCN;
		public int RedComments;
		public bool RedEmpty;
		public bool RedTN;
		public int RedTranslations;
		public int StageP1;
		public int StageP2;
		public int TranslationNumber;
		public List<string> Translations = new List<string>();

		public VoInternal()
		{
			InternalLines = new int();
			InternalLines = 0;

			CommentsNumber = new int();
			CommentsNumber = 0;

			LineNumber = new int();
			LineNumber = 0;

			OriginalMessageLength = new int();
			OriginalMessageLength = 0;

			TranslationNumber = new int();
			TranslationNumber = 0;

			RedCN = new bool();
			RedCN = false;

			RedEmpty = new bool();
			RedEmpty = false;

			RedC = new bool();
			RedC = false;

			RedTN = new bool();
			RedTN = false;

			StageP1 = new int();
			StageP1 = 0;

			StageP2 = new int();
			StageP2 = 0;

			RedComments = new int();
			RedComments = 0;

			RedTranslations = new int();
			RedTranslations = 0;
		}

		public static string GetBlank()
		{
			return "(blank)";
		}

		public static string GetDefaultMsgid()
		{
			return "msgid \"\"";
		}

		public static string GetDefaultMsgstr()
		{
			return "msgstr \"\"";
		}

		public static string GetDefaultProjectID()
		{
			const string project_id_version = "DRV3";
			return "\"Project-Id-Version: " + project_id_version + "\\n\"";
		}

		public static string GetDefaultReportTo()
		{
			const string whom_rep = "your_email";
			return "\"Report-Msgid-Bugs-To: " + whom_rep + "\\n\"";
		}

		public static string GetDefaultPOTCreationDate()
		{
			var now = DateTime.Now;
			string cr_date = now.Date.ToString().Substring(0, 10);
			return "\"POT-Creation-Date: " + cr_date + "\\n\"";
		}

		public static string GetDefaultRevisionDate()
		{
			const string rev_date = "";
			return "\"PO-Revision-Date: " + rev_date + "\\n\"";
		}

		public static string GetDefaultLastTranslator()
		{
			const string whom_tr = "";
			return "\"Last-Translator: " + whom_tr + "\\n\"";
		}

		public static string GetDefaultLanguageTeam()
		{
			const string team = "";
			return "\"Language-Team: " + team + "\\n\"";
		}

		public static string GetDefaultLanguage()
		{
			const string lang = "en-US";
			return "\"Language: " + lang + "\\n\"";
		}

		public static string GetDefaultMIMEVersion()
		{
			const string mime_ver = "1.0";
			return "\"MIME-Version: " + mime_ver + "\\n\"";
		}

		public static string GetDefaultContentType()
		{
			const string cont_type = "text/plain; charset=UTF-8";
			return "\"Content-Type: " + cont_type + "\\n\"";
		}

		public static string GetDefaultContentEncoding()
		{
			const string cont_enc = "8bit";
			return "\"Content-Transfer-Encoding: " + cont_enc + "\\n\"";
		}

		public void ReadInternalLines(string str)
		{
			var intlines = str.IndexOf("Lines:");
			if (intlines != -1)
			{
				InternalLines = int.Parse(str.Substring((int)Indexes.InternalLineIndex));
				//Print("Internal lines: " + InternalLines.ToString());
			}
		}

		public void ReadCommentsNumber(string str)
		{
			var commnum = str.IndexOf("Number of comments:");
			if (commnum != -1)
			{
				CommentsNumber = int.Parse(str.Substring((int)Indexes.CommentsNumberIndex));
				//Print("Comments number: " + CommentsNumber.ToString());
				RedCN = true;
			}
		}

		public void ReadComments(string str)
		{
			RedC = true;
			if (RedComments < CommentsNumber)
			{
				if (str.Length > 0)
				{
					Comments.Add(str);
					RedComments++;

					//Print("Comment: " + str);
				}
			}
		}

		public void ReadFile(string str)
		{
			var file = str.IndexOf("File:");
			if (file != -1)
			{
				OriginFile = str.Substring((int)Indexes.FileIndex);

				//Print("Origin file: " + OriginFile);
			}
		}

		public void ReadLine(string str)
		{
			var line = str.IndexOf("Line:");
			if (line != -1)
			{
				LineNumber = int.Parse(str.Substring((int)Indexes.LineIndex));

				//Print("LineNumber: " + Line.ToString());
			}
		}

		public void ReadCharacter(string str)
		{
			var character = str.IndexOf("Character:");
			if (character != -1)
			{
				Character = str.Substring((int)Indexes.CharacterIndex);

				//Print("Character: " + Character);
			}
		}

		public void ReadOGMessageLength(string str)
		{
			var oglen = str.IndexOf("Original message length:");
			if (oglen != -1)
			{
				OriginalMessageLength = int.Parse(str.Substring((int)Indexes.OgMessageLengthIndex));

				//Print("OG message length: " + OriginalMessageLength.ToString());
			}
		}

		public void ReadOGMessage(string str)
		{
			var og = str.IndexOf("Original message:");
			if (og != -1)
			{
				OriginalMessage = str.Substring((int)Indexes.OgMessageIndex);

				if (OriginalMessage.Length > 2)
				{
					if (OriginalMessage.StartsWith("\""))
					{
						OriginalMessage = OriginalMessage.Substring(1);
					}

					if (OriginalMessage.EndsWith("\""))
					{
						OriginalMessage = OriginalMessage.Substring(0, OriginalMessage.Length - 1);
					}

					OriginalMessage = OriginalMessage.Replace("\"\"", "");
				}
				else
				{
					if (OriginalMessage == "\"\"")
					{
						OriginalMessage = "(blank)";
					}
				}

				//Print("Original message: " + OriginalMessage);
			}
		}

		public void ReadTranslationNumber(string str)
		{
			var translat_num = str.IndexOf("Number of translations:");
			if (translat_num != -1)
			{
				TranslationNumber = int.Parse(str.Substring((int)Indexes.TranslationNumberIndex));
				//Print("Translation number: " + TranslationNumber.ToString());
				RedTN = true;
			}
		}

		public void ReadTranslation(string str)
		{
			//Print("Stock: " + str);

			if (RedTranslations < TranslationNumber)
			{
				if (str.Length > 2)
				{
					if (str.StartsWith("\""))
					{
						str = str.Substring(1);
					}

					if (str.EndsWith("\""))
					{
						str = str.Substring(0, str.Length - 1);
					}

					str = str.Replace("\"\"", "");
				}
				else
				{
					if (str == "\"\"")
					{
						str = "(blank)";
					}
				}

				//Print("Modified: " + str);

				Translations.Add(str);
				RedTranslations++;
			}
		}

		public void ParsePhase1(string str)
		{
			switch (StageP1)
			{
				case 0:
					ReadInternalLines(str);
					break;
				case 1:
					ReadCommentsNumber(str);
					break;
				case 2:
					RedEmpty = true;
					break;
				default:
					ReadComments(str);
					break;
			}

			StageP1++;
		}

		public void ParsePhase2(string str)
		{
			switch (StageP2)
			{
				case 0:
					ReadFile(str);
					break;
				case 1:
					ReadLine(str);
					break;
				case 2:
					ReadCharacter(str);
					break;
				case 3:
					ReadOGMessageLength(str);
					break;
				case 4:
					ReadOGMessage(str);
					break;
				case 5:
					ReadTranslationNumber(str);
					break;
				case 6:
					if (TranslationNumber <= 0)
					{
						TranslationNumber = 1;
						Translations.Add("(blank)");
					}

					break;
				default:
					ReadTranslation(str);
					break;
			}

			StageP2++;
		}

		public void Parse(string str)
		{
			bool should_p2 = false;
			if (RedCN)
			{
				bool red = false;
				red = CommentsNumber > 0 ? RedC : RedEmpty;
				should_p2 = RedComments >= CommentsNumber && red;
			}

			if (should_p2)
			{
				ParsePhase2(str);
			}
			else
			{
				ParsePhase1(str);
			}
		}
	}
}