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
	}
}
