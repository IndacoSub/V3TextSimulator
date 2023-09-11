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
		public GameIndex GameIndex = GameIndex.V3;

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

			MessageContext = basestr.Substring(0, basestr.Length - 1);

			bool is_V3 = GameIndex == GameIndex.V3;

			if (!is_V3)
			{
				int next_quote = basestr.IndexOf('\"');
				LineNumber = basestr.Substring(0, next_quote);
				Stage++;
				return;
			}

			if (basestr.Length < 3 + 1)
			{
				// ???
				return;
			}

			string lineno = basestr.Substring(0, 4);
			string ofile = "";
			string chara = "";
			string anim = "";
			string voice = "";
			basestr = basestr.Substring(4);

			int count = basestr.Count(f => f == '|');

			if (count > 0)
			{
				basestr = basestr.Substring(3); // ' | '
			}

			count--;

			if (count >= 0)
			{
				switch (count)
				{
					case 0:
						ofile = basestr.Substring(0, basestr.Length - 1);
						break;
					case 1:
						ofile = basestr.Substring(0, basestr.IndexOf("|") - 1);
						basestr = basestr.Substring(ofile.Length + 3);
						chara = basestr.Substring(0, basestr.Length - 1);
						break;
					case 2:
						ofile = basestr.Substring(0, basestr.IndexOf("|") - 1);
						basestr = basestr.Substring(ofile.Length + 3);
						chara = basestr.Substring(0, basestr.IndexOf("|") - 1);
						basestr = basestr.Substring(chara.Length + 3);
						anim = basestr.Substring(0, basestr.Length - 1);
						if (anim.StartsWith("vic"))
						{
							voice = anim;
							anim = "";
						}

						break;
					case 3:
						ofile = basestr.Substring(0, basestr.IndexOf("|") - 1);
						basestr = basestr.Substring(ofile.Length + 3);
						chara = basestr.Substring(0, basestr.IndexOf("|") - 1);
						basestr = basestr.Substring(chara.Length + 3);
						anim = basestr.Substring(0, basestr.IndexOf("|") - 1);
						basestr = basestr.Substring(anim.Length + 3);
						voice = basestr.Substring(0, basestr.Length - 1);
						break;
				}
			}

			LineNumber = lineno;
			OriginFile = ofile;
			Character = chara;
			Expression = anim;
			Voiceline = voice;

			/*
            InputManager.Print(
                "Str: " + str + "\n" + 
                "Found | : " + ogcount + " (" + count + ")\n" + 
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