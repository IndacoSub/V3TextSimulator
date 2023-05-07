namespace DGRV3TS
{
	public partial class Operations : Form
	{
		DialogueWindow dialogue_window;
		public Operations()
		{
			InitializeComponent();
			Init();
			OpenWindow();
		}

		public void OpenWindow()
		{
			string cur = Directory.GetCurrentDirectory();
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
			} else
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
