namespace DGRV3TS.Subforms
{
    partial class VerticalSummary
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Codice generato da Progettazione componenti

		/// <summary> 
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
			label1 = new Label();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Dock = DockStyle.Fill;
			label1.Location = new Point(0, 0);
			label1.MaximumSize = new Size(230, 200);
			label1.MinimumSize = new Size(230, 40);
			label1.Name = "label1";
			label1.Size = new Size(230, 45);
			label1.TabIndex = 0;
			label1.Text = "AAAAAAAAAAAAAAAAAAAAAaaaaaaaaa\r\n---------------------------------\r\nBBBBBBBBBBBBBBBBBBBBBBBBBBB";
			label1.TextAlign = ContentAlignment.MiddleLeft;
			label1.MouseDown += Label1_MouseDown;
			label1.MouseUp += Vertical_MouseUp;
			// 
			// VerticalSummary
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			BorderStyle = BorderStyle.FixedSingle;
			Controls.Add(label1);
			MaximumSize = new Size(230, 200);
			MinimumSize = new Size(230, 40);
			Name = "VerticalSummary";
			Size = new Size(228, 45);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Label label1;
    }
}
