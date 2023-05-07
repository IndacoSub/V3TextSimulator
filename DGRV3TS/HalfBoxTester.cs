using System.Drawing.Imaging;

namespace DGRV3TS
{

	public partial class DialogueWindow : Form
	{
		public DialogueWindow()
		{

			InitializeComponent();
		}

		private void DisplayedImage_Click(object sender, EventArgs e)
		{
			// Does not include characters?
			SaveImage(DisplayedImage.Image, DisplayedImage.Image);
		}

		public void SaveImage(Image baseimage, Image tosave)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
			Image ToSave;
			{
				if (InputManager.Prompt(MessageBoxButtons.YesNo, "Save the current image?") != DialogResult.Yes)
					return;
				using (Graphics grfx = Graphics.FromImage(baseimage))
				{
					if (tosave != null)
					{
						grfx.DrawImage(tosave, 0, 0);
					}
				}

				ToSave = baseimage;
			}
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				ToSave.Save(sfd.FileName, ImageFormat.Png);
				InputManager.Print("Saved!");
			}
		}
	}
}
