using System.Globalization;
using System.Speech.Synthesis;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace DGRV3TS
{
	internal class SoundManager
	{
		public PromptBuilder Builder;
		public bool DoneSpeaking = true;
		public SpeechSynthesizer Synthesizer;

		public SoundManager()
		{
			Synthesizer = new SpeechSynthesizer();
			Synthesizer.SetOutputToDefaultAudioDevice();
			Builder = new PromptBuilder();

			/*
            // show installed voices
            foreach (var v in Synthesizer.GetInstalledVoices().Select(v => v.VoiceInfo))
            {
                InputManager.Print("Name: " + v.Description + ", Gender: " + v.Gender.ToString() + ", Age: " + v.Age.ToString());
            }
            */
		}

		public static string GetSoundFileByName(string name, string game)
		{
			// Sound/Voices/*.wav?

			string ext = ".wav";
			string cur = FileManager.GetCurrentDirectory();
			string sound = Path.Combine(cur, "Sound");
			string voice = Path.Combine(sound, "Voices");
			string path = Path.Combine(voice, game);
			string ret = Path.Combine(path, name + ext);
			if (!File.Exists(ret))
			{
				string[] files =
					Directory.GetFiles(FileManager.GetCurrentDirectory(), "*.wav", SearchOption.AllDirectories);
				foreach (string file in files)
				{
					if (file.Contains(name))
					{
						//InputManager.Print("Found: " + file);
						name = Path.GetFileNameWithoutExtension(file);
						ret = file; // Path.Combine(voice, name + ext);
						break;
					}
				}
			}

			//Console.WriteLine(ret);
			return ret;
		}

		public static bool SoundFileExists(string name, string game)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}

			string sfile = GetSoundFileByName(name, game);
			return File.Exists(sfile);
		}

		public void PlayVoice(string line, string language, VoiceGender gender, VoiceAge age, bool async)
		{
			DoneSpeaking = false;
			Builder.ClearContent();
			Builder.StartVoice(new CultureInfo(language));
			Builder.AppendText(line);
			Builder.EndVoice();
			Synthesizer.SelectVoiceByHints(gender, age);

			if (async)
			{
				Synthesizer.SpeakAsync(Builder);
			}
			else
			{
				Synthesizer.Speak(Builder);
				if (line == "...")
				{
					Thread.Sleep(1000);
				}
			}

			DoneSpeaking = true;
		}

		public VoiceGender V3GenderByCharacter(string chara, string origin)
		{
			// I tried to stick to the canon

			VoiceGender ret = VoiceGender.Female;

			switch (chara)
			{
				case "C000_Saiha":
				case "C001_Momot":
				case "C002_Hoshi":
				case "C003_Amami":
				case "C004_Gokuh":
				case "C005_Oma__":
				case "C006_Shing":
				case "C007_Ki-Bo":
					ret = VoiceGender.Male;
					break;

				case "C008_Tojo_":
				case "C009_Yumen":
				case "C010_Haruk":
				case "C011_Chaba":
				case "C012_Shiro":
				case "C013_Yonag":
				case "C014_Iruma":
				case "C015_Akama":
					ret = VoiceGender.Female;
					break;

				case "C020_Monok":
				case "C021_Mtaro":
				case "C022_Msuke":
				case "C024_Mdam_":
				case "C025_Mkid_":
				case "C026_Eguis":
				case "C027_Mono5":
				case "C035_Exred":
				case "C036_Exyel":
				case "C038_Exgre":
				case "C039_Exblu":
					ret = VoiceGender.Male;
					break;

				case "C023_Mfunn":
				case "C037_Expin":
					ret = VoiceGender.Female;
					break;

				case "chara_Blank":
				case "non":
				case "None":
				case "":
					// Good enough for now
					if (origin.StartsWith("c00") || origin.StartsWith("c01"))
					{
						ret = VoiceGender.Female;
					}
					else
					{
						ret = VoiceGender.Male;
					}

					break;

				case "chara_Hatena":
					ret = VoiceGender.Male;
					break;

				default:
					InputManager.Print("Unknown gender for: " + chara);
					ret = VoiceGender.Male;
					break;
			}

			return ret;
		}

		public VoiceAge V3AgeByCharacter(string chara)
		{
			// These ages are not necessarily canon

			VoiceAge ret = VoiceAge.Teen;

			switch (chara)
			{
				case "C000_Saiha":
					ret = VoiceAge.Teen;
					break;
				case "C001_Momot":
					ret = VoiceAge.Teen;
					break;
				case "C002_Hoshi":
					ret = VoiceAge.Adult;
					break;
				case "C003_Amami":
					ret = VoiceAge.Teen;
					break;
				case "C004_Gokuh":
					ret = VoiceAge.Child;
					break;
				case "C005_Oma__":
					ret = VoiceAge.Child;
					break;
				case "C006_Shing":
					ret = VoiceAge.Adult;
					break;
				case "C007_Ki-Bo":
					ret = VoiceAge.Teen;
					break;
				case "C008_Tojo_":
					ret = VoiceAge.Adult;
					break;
				case "C009_Yumen":
					ret = VoiceAge.Child;
					break;
				case "C010_Haruk":
					ret = VoiceAge.Teen;
					break;
				case "C011_Chaba":
					ret = VoiceAge.Child;
					break;
				case "C012_Shiro":
					ret = VoiceAge.Teen;
					break;
				case "C013_Yonag":
					ret = VoiceAge.Child;
					break;
				case "C014_Iruma":
					ret = VoiceAge.Teen;
					break;
				case "C015_Akama":
					ret = VoiceAge.Teen;
					break;

				case "C020_Monok":
					ret = VoiceAge.Adult;
					break;
				case "C021_Mtaro":
				case "C022_Msuke":
				case "C023_Mfunn":
				case "C024_Mdam_":
				case "C025_Mkid_":
					ret = VoiceAge.Teen;
					break;

				case "C026_Eguis":
					ret = VoiceAge.Senior;
					break;

				case "C027_Mono5":
					ret = VoiceAge.Teen;
					break;

				case "C035_Exred":
				case "C036_Exyel":
				case "C037_Expin":
				case "C038_Exgre":
				case "C039_Exblu":
					ret = VoiceAge.Senior;
					break;

				case "chara_Blank":
				case "chara_Hatena":
				case "non":
				case "None":
				case "":
					ret = VoiceAge.Teen;
					break;

				default:
					InputManager.Print("Unknown age for: " + chara);
					ret = VoiceAge.Adult;
					break;
			}

			return ret;
		}
	}
}