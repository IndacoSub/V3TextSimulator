namespace DGRV3TS
{
    public class StxInternal
	{
		// Magic converts to TXTS, which is STXT inverted...
		public static readonly uint Magic = 0x54585453; // Nyeh, I told you magic was real
		public static readonly uint JPLL = 0x4C4C504A;
		public uint Language; // 0x4
		public uint Unk1; // 0x8
		public uint HeaderSizeH; // 0xC
		public uint Unk2; // 0x10
		public uint DetectedPointers; // 0x14
		public readonly uint[] Num;
		public readonly uint[] Pointers;
		public readonly string[] Sentences;
		public bool IsValidSTX;
		public WRDInternal LoadedWRD = null;

		public StxInternal(string file)
		{
			if (!File.Exists(file))
			{
				return;
			}

			(Sentences, Num, Pointers) = ReadSentencesFromSTX(file);
			IsValidSTX = true;
		}

		public static bool IsMagicValid(string file)
		{
			using (FileStream f = new FileStream(file, FileMode.Open, FileAccess.Read))
			using (BinaryReader rf = new BinaryReader(f))
			{
				if (rf.ReadUInt32() == Magic)
				{
					return true;
				}
			}

			return false;
		}

		private (string[], uint[], uint[]) ReadSentencesFromSTX(string fileSTX)
		{
			using (FileStream fs = new FileStream(fileSTX, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					uint headerSize;
					uint NpointersToRead;

					br.ReadUInt32(); // MagicID
					Language = br.ReadUInt32(); // lang
					Unk1 = br.ReadUInt32(); // unk1
					HeaderSizeH = headerSize = br.ReadUInt32(); // Read header size (in hex)
					Unk2 = br.ReadUInt32(); //unk2
					DetectedPointers = NpointersToRead = br.ReadUInt32();

					uint[] pointers = new uint[NpointersToRead]; // pointers to positions in the file
					uint[] num = new uint[NpointersToRead]; // "number" of each pointer
					string[]
						sentences =
							new string[NpointersToRead]; // the number of pointers corresponds to the number of sentences

					// Skip the header
					fs.Seek(headerSize, SeekOrigin.Begin);

					// All the pointers are close to one another
					// so we can read them one after the other
					for (uint i = 0; i < NpointersToRead; i++)
					{
						// "num[i] = i" cannot be used because
						// of string deduplication issues
						// (which are edge cases)
						num[i] = br.ReadUInt32();
						pointers[i] = br.ReadUInt32();
					}

					for (uint i = 0; i < NpointersToRead; i++)
					{
						// For (NpointersToRead), jump to the position
						// of the pointer, and read the data from there
						fs.Seek(pointers[i], SeekOrigin.Begin);

						ushort Letter = 0;
						string tempSentence = string.Empty;

						// Read the string until an unsupported character is found,
						// or the end of stream is reached
						while (fs.Position != fs.Length && (Letter = br.ReadUInt16()) > 0)
						{
							tempSentence += (char)Letter;
						}

						// If the string is empty, replace it with "[EMPTY_LINE]"
						if (tempSentence == string.Empty)
						{
							sentences[i] = "[EMPTY_LINE]";
						}
						else
						{
							// Replace \r\n with \n first, then delete any remaining \r
							sentences[i] = tempSentence.Replace("\r\n", "\n").Replace("\r", string.Empty)
								.Replace("\n", "\\n");
						}
					}

					return (sentences, num, pointers);
				}
			}
		}

		public string CharacterByLineNumber(int linenum)
		{
			if (LoadedWRD == null)
			{
				return "";
			}

			string chara = "";
			var keys = LoadedWRD.charaNames.Keys;

			foreach (var _ in LoadedWRD.charaNames)
			{
				uint lastkey = 0;
				foreach (var key in keys)
				{
					if (linenum < key)
					{
						break;
					}

					lastkey = key;
				}

				if (!LoadedWRD.charaNames.TryGetValue(lastkey, out string temp))
				{
					return chara;
				}

				chara = temp;
			}

			return chara;
		}

		public string ExpressionByLineNumber(int linenum, string character)
		{
			if (LoadedWRD == null)
			{
				return "";
			}

			if (!LoadedWRD.charaExpressions.ContainsKey(character))
			{
				return "";
			}

			string expr = "";
			if (LoadedWRD.charaExpressions[character] == null)
			{
				return "";
			}

			var charaexpressions = LoadedWRD.charaExpressions[character];
			if (charaexpressions == null)
			{
				InputManager.Print("No animations found for " + character + "!");
				return "";
			}

			if (charaexpressions.InitialAnimation == null ||
				string.IsNullOrWhiteSpace(charaexpressions.InitialAnimation) ||
				charaexpressions.InitialAnimation == "C999_ABCDE")
			{
				if (character != "chara_Blank" && character != "chara_Hatena" && character != "non" && character != "None")
				{
					InputManager.Print("Invalid initial animation for " + character + "!");
				}

				return "";
			}

			if (charaexpressions.Expressions.Count <= 0)
			{
				return charaexpressions.InitialAnimation;
			}

			// Never-seen this character? -> Use default animation
			var first_anim = charaexpressions.Expressions.First();
			uint firstkey = first_anim.Key;
			if (linenum < firstkey)
			{
				return charaexpressions.InitialAnimation;
			}

			var keys = charaexpressions.Expressions.Keys;

			foreach (var _ in charaexpressions.Expressions)
			{
				uint lastkey = 0;
				foreach (var key in keys)
				{
					if (linenum < key)
					{
						break;
					}

					lastkey = key;
				}

				if (!charaexpressions.Expressions.TryGetValue(lastkey, out string temp))
				{
					return expr;
				}

				expr = temp;
			}

			return expr;
		}

		public string VoicelineByLineNumber(int linenum)
		{
			if (LoadedWRD == null)
			{
				return "";
			}

			if (!LoadedWRD.voiceLines.TryGetValue((uint)linenum, out string line))
			{
				return "";
			}

			return line;
		}
	}
}