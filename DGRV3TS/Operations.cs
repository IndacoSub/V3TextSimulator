namespace DGRV3TS
{
	public partial class Operations : Form
	{
		DialogueWindow dialogue_window;
		string[] program_args;
		string auto_open_file = "";

		public Operations(string[] args)
		{
			program_args = args;
			if (program_args.Length > 0)
			{
				auto_open_file = program_args[0];
			}

			InitializeComponent();
			Init();
			OpenWindow();

			if (auto_open_file.Length > 0)
			{
				OpenFile(auto_open_file);
				auto_open_file = "";
			}
		}

		public void OpenWindow()
		{
			string cur = FileManager.GetCurrentDirectory();
			string gfx = Path.Combine(cur, "Graphics");
			string backgrounds = Path.Combine(gfx, "Backgrounds");
			if (Directory.Exists(backgrounds))
			{
				if (dialogue_window != null) dialogue_window.Close();
				if (dialogue_window != null) dialogue_window.Dispose();
				// Initialize the image window
				dialogue_window = new DialogueWindow();
				InitWindow();
				UpdateRTB();
				dialogue_window.Show();
			}
			else
			{
				if (dialogue_window != null)
				{
					dialogue_window.Close();
				}
			}
		}

		private void ReopenWindowButton_Click(object sender, System.EventArgs e)
		{

			OpenWindow();
		}

        private void DumpVoicelineOnlyButton_Click(object sender, EventArgs e)
        {

			// This *definitely* has more than one use-case

            if (!LoadedFile)
            {
				return;
            }

			if(fi.Type != FileManager.LoadedFileType.Po)
			{
				return;
			}

            List<PoInternal> pos = new List<PoInternal>();

			foreach(PoInternal po in fi.PoList)
			{
				// No voiceline = skip
				if(po.Voiceline.Length <= 0)
				{
					continue;
				}

				// Filter by character?
				// In this case:
				// Dump all Monokuma dialogues which contain a voiceline
				if(!po.Character.Contains("Monok"))
				{
					continue;
				}

				pos.Add(po);
			}

			if (pos.Count > 0)
			{
				fi.PoList = pos;
				fi.SaveTxt("dump.txt", false);
			}
        }
    }
}
