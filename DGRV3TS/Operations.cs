using System.Transactions;
// ^ ??? I swear, there aren't any DLCs and/or microtransactions here

namespace DGRV3TS
{
	public partial class Operations : Form
	{
		DialogueWindow dialogue_window;
		VerticalView vertical_view;
		ToolTip listbox_tooltip;
		string[] program_args;
		string auto_open_file = "";
		const string ProgramInfo = "Operations (v2.5)";

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
			DestroyVerticalView();
			OpenVerticalView();

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

		public void DestroyVerticalView()
		{
			if (vertical_view != null)
			{
				vertical_view.Owner = null;
				vertical_view.Controls.Clear();
				vertical_view.Close();
				vertical_view.Dispose();
			}
		}

		public void OpenVerticalView()
		{
			if (!LoadedFile)
			{
				return;
			}

			if (vertical_view == null || vertical_view.Disposing || vertical_view.IsDisposed)
			{
				vertical_view = new VerticalView(new List<string>(), new List<string>(), new List<string>());
				vertical_view.Owner = this;

				List<string> translation = fi.GetAllTranslatedText();
				List<string> original = fi.GetAllOriginalText();
				List<string> speakers = fi.GetAllSpeakers();

				vertical_view.InitSummaries(translation, original, speakers);
				vertical_view.Show();
			}
			else
			{
				UpdateVerticalView();
			}
		}

		public void UpdateVerticalView()
		{

			List<string> translated = fi.GetAllTranslatedText();
			List<string> original = fi.GetAllOriginalText();

			vertical_view.UpdateSummaries(translated, original, -1);
			vertical_view.Show();
		}

		public void OnVerticalViewClick(int ID)
		{
			CheckUnsaved();

			fi.StringIndex = ID;
			UpdateTextbox();
			UpdateLineCharacter();
			DisplayCharacterImage();
			UpdateVerticalView();
		}

		private void ReopenWindowButton_Click(object sender, System.EventArgs e)
		{

			OpenWindow();
		}

		private void DoDumpVoicelines()
		{

			// This *definitely* has more than one use-case

			if (!LoadedFile)
			{
				return;
			}

			if (fi.Type != FileManager.LoadedFileType.Po)
			{
				return;
			}

			List<PoInternal> pos = new List<PoInternal>();

			foreach (PoInternal po in fi.PoList)
			{
				// No voiceline = skip
				if (po.Voiceline.Length <= 0)
				{
					continue;
				}

				// Filter by character?
				// In this case:
				// Dump all Monokuma dialogues which contain a voiceline
				if (!po.Character.Contains("Monok"))
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

		private void DumpVoicelineOnlyButton_Click(object sender, EventArgs e)
		{
			DoDumpVoicelines();
		}

		private void OpenVerticalViewButton_Click(object sender, EventArgs e)
		{

		}

		private void Operations_Load(object sender, EventArgs e)
		{

		}

		private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFile("");
		}

		private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs();
		}

		private void resetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ResetReadingProgress();
		}

		private void fastReadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DoFastRead();
		}

		private void copyScreenshotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.dialogue_window == null)
			{
				return;
			}
			DoCopyImage();
		}

		private void saveScreenshotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.dialogue_window == null)
			{
				return;
			}

			this.dialogue_window.DoSaveImage();
		}

		private void translationModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangedTranslationMode();
		}

		private void replaceVariablesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangedReplaceVariables();
		}

		private void displayCharacterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangedDisplayCharacter();
		}

		private void useAlternateVarsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangedAlternateVars();
		}

		private void reloadVariablesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DoReloadVariables();
		}

		private void openGraphicsWinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenWindow();
		}

		private void reopenVerticalViewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DestroyVerticalView();
			OpenVerticalView();
		}

		private void setupSpritesFromFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DoSetupSprites();
		}

		private void setupVoicelinesFromFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			InputManager.Print("Currently unimplemented!");
		}

		private void dumpVoicelinesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DoDumpVoicelines();
		}

		private void DoMaximizeWindow()
		{
			if (this.Size.Width == 1562)
			{
				this.MinimumSize = new Size(1135, 218);
				this.MaximumSize = this.MinimumSize;
				this.Size = this.MaximumSize;
			}
			else
			{
				this.MaximumSize = new Size(1562, 218);
				this.Size = this.MaximumSize;
				this.MinimumSize = this.Size;
			}
		}

		private void maximizeWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DoMaximizeWindow();
		}

		private void TranslationModeToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			if (translationModeToolStripMenuItem.Checked)
			{
				autoTranslationToolStripMenuItem.Checked = true;
				displayCharacterToolStripMenuItem.Checked = true;
			}
		}
	}
}
