namespace DGRV3TS
{
    partial class VerticalView
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
			panel1 = new Panel();
			SuspendLayout();
			// 
			// panel1
			// 
			panel1.Dock = DockStyle.Fill;
			panel1.Location = new Point(0, 0);
			panel1.Name = "panel1";
			panel1.Size = new Size(250, 720);
			panel1.TabIndex = 0;
			// 
			// VerticalView
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(250, 720);
			Controls.Add(panel1);
			MaximumSize = new Size(266, 759);
			MinimumSize = new Size(266, 759);
			Name = "VerticalView";
			Text = "VerticalView";
			ResumeLayout(false);
		}

		#endregion

		private Panel panel1;
        private List<Subforms.VerticalSummary> Summaries;
    }
}