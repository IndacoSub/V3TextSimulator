namespace DGRV3TS
{
	partial class DialogueWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			DisplayedImage = new PictureBox();
			((System.ComponentModel.ISupportInitialize)DisplayedImage).BeginInit();
			SuspendLayout();
			// 
			// DisplayedImage
			// 
			DisplayedImage.Dock = DockStyle.Fill;
			DisplayedImage.InitialImage = null;
			DisplayedImage.Location = new Point(0, 0);
			DisplayedImage.Margin = new Padding(4, 3, 4, 3);
			DisplayedImage.MaximumSize = new Size(1280, 720);
			DisplayedImage.MinimumSize = new Size(1280, 720);
			DisplayedImage.Name = "DisplayedImage";
			DisplayedImage.Size = new Size(1280, 720);
			DisplayedImage.SizeMode = PictureBoxSizeMode.AutoSize;
			DisplayedImage.TabIndex = 1;
			DisplayedImage.TabStop = false;
			DisplayedImage.Click += DisplayedImage_Click;
			// 
			// DialogueWindow
			// 
			AutoScaleMode = AutoScaleMode.None;
			ClientSize = new Size(1280, 720);
			Controls.Add(DisplayedImage);
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MaximumSize = new Size(1296, 759);
			MinimumSize = new Size(1296, 759);
			Name = "DialogueWindow";
			Text = "Dialogue Viewer";
			((System.ComponentModel.ISupportInitialize)DisplayedImage).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		public System.Windows.Forms.PictureBox DisplayedImage;
	}
}