namespace DGRV3TS
{
	public class PoInternal
	{
		public string Character = "DefaultCharacter";
		public string Expression = "DefaultExpression";
		private bool IsDoneReadingOriginalMessage;
		public string LineNumber = "9999";
		public string MessageContext = "DefaultMessageContext";
		public string MessageString = "";
		public string OriginalMessage = "";
		public string OriginFile = "DefaultOriginFile";
		public int Stage;
		public string Voiceline = "DefaultVoiceline";
		public GameIndex PoGameIndex = GameIndex.V3;

		public static string GetBlank()
		{
			return "";
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

		public void ReadMessageContext(string str)
		{
			if (!str.Contains("msgctxt"))
			{
				// ???
				return;
			}

			string basestr = str.Substring(9); // msgctxt "
			basestr = basestr.Substring(0, basestr.Length - 1);

			MessageContext = basestr;

			string lineno = "";
			string ofile = "";
			string chara = "";
			string anim = "";
			string voice = "";

			var split = basestr.Split(" | ");
			switch(split.Length)
			{
				case 0:
				case 1:
					lineno = basestr;
					break;
				case 2:
					lineno = split[0];
					ofile = split[1];
					break;
				case 3:
					lineno = split[0];
					ofile = split[1];
					chara = split[2];
					break;
				case 4:
					lineno = split[0];
					ofile = split[1];
					chara = split[2];
					anim = split[3];
					break;
				case 5:
					lineno = split[0];
					ofile = split[1];
					chara = split[2];
					anim = split[3];
					voice = split[4];
					break;

			}

			if (anim.StartsWith("vic"))
			{
				voice = anim;
				anim = "";
			}

			LineNumber = lineno;
			OriginFile = ofile;
			Character = chara;
			Expression = anim;
			Voiceline = voice;

			/*
            InputManager.Print(
                "Str: " + str + "\n" + 
                "LineNo: " + this.LineNumber + "\n" +
                "OriginFile: " + this.OriginFile + "\n" +
                "Character: " + this.Character + "\n" +
                "Expression: " + this.Expression + "\n" +
                "VoiceLine: " + this.Voiceline
            );
			*/

			Stage++;
		}

		public void ReadOriginalMessage(string str)
		{
			if (str.Contains("msgid"))
			{
				string s = str.Substring(7);
				s = s.Substring(0, s.Length - 1);
				OriginalMessage += s;
			}
			else
			{
				string s = str.Substring(1);
				s = s.Substring(0, s.Length - 1);
				OriginalMessage += s;
			}
		}

		public void ReadMessageString(string str)
		{
			if (str.Contains("msgstr"))
			{
				string s = str.Substring(8);
				s = s.Substring(0, s.Length - 1);
				MessageString += s;
			}
			else
			{
				if (str.Length > 0)
				{
					if (str.Length > 1)
					{
						string s = str.Substring(1);
						s = s.Substring(0, s.Length - 1);
						MessageString += s;
					}
					else
					{
						MessageString += str;
					}
				}
			}
		}

		public void Parse(string str)
		{
			switch (Stage)
			{
				case 0:
					ReadMessageContext(str);
					break;
				case 1:
					{
						if (str.First() == '\"')
						{
							if (!IsDoneReadingOriginalMessage)
							{
								ReadOriginalMessage(str);
							}
							else
							{
								ReadMessageString(str);
							}
						}
						else
						{
							if (str.Contains("msgstr"))
							{
								IsDoneReadingOriginalMessage = true;
								ReadMessageString(str);
							}
							else
							{
								ReadOriginalMessage(str);
							}
						}
					}
					break;
			}
		}
	}
}