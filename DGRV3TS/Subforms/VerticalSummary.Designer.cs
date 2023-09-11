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
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.MaximumSize = new System.Drawing.Size(230, 200);
            this.label1.MinimumSize = new System.Drawing.Size(230, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = "AAAAAAAAAAAAAAAAAAAAAaaaaaaaaa\r\n---------------------------------\r\nBBBBBBBBBBBBBB" +
    "BBBBBBBBBBBBB";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // VerticalSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(230, 200);
            this.MinimumSize = new System.Drawing.Size(230, 40);
            this.Name = "VerticalSummary";
            this.Size = new System.Drawing.Size(228, 45);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
    }
}
