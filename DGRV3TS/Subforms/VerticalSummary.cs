using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DGRV3TS.Subforms
{
    public partial class VerticalSummary : UserControl
    {
        ContextMenuStrip cms = new ContextMenuStrip();
        public int ID = 0;
        public string Translation = "";

        public VerticalSummary()
        {
            InitializeComponent();
            cms = new ContextMenuStrip();
            OnRightClick();
        }

		private void Vertical_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				cms.Show(this, e.Location);
			}
		}

		private void OnRightClick()
		{
			// This menu shows up when right-clicking an item from the listbox

			ContextMenuStrip menuStrip = new ContextMenuStrip();

			ToolStripMenuItem menuItem0 = new ToolStripMenuItem("Copy");

			menuItem0.Click += OnCopy;

			menuItem0.Name = "Copy";

			ToolStripMenuItem menuItem4 = new ToolStripMenuItem("I clicked by mistake");

			menuItem4.Click += OnClickByMistake;

			menuItem4.Name = "I clicked by mistake";

			ToolStripMenuItem menuItem5 = new ToolStripMenuItem("Paste");

			menuItem5.Click += OnPaste;

			menuItem5.Name = "I clicked by mistake";

			menuStrip.Items.Add(menuItem0);
			menuStrip.Items.Add(menuItem5);
			menuStrip.Items.Add(menuItem4);

			cms = menuStrip;
		}

		private void OnCopy(object sender, EventArgs e)
		{
			Clipboard.SetText(Translation);
            cms.Close();
		}

		private void OnPaste(object sender, EventArgs e)
		{
			var text = Clipboard.GetText();

			Panel panel = (this.Parent as Panel);
			if (panel == null)
			{
				return;
			}
			//MessageBox.Show(panel.Parent.Name);
			VerticalView parentForm = (panel.Parent as VerticalView);
			if (parentForm == null)
			{
				return;
			}
			Operations op = (parentForm.Owner as Operations);
			if(op == null)
			{
				return;
			}
			op.fi.OverrideLine(this.ID, text);
			if(op.fi.StringIndex == this.ID)
			{
				op.Textbox.Text = text;
			}
			parentForm.OnClick(ID);
		}

		private void OnClickByMistake(object sender, EventArgs e)
		{
			cms.Close();
		}

		public void SetID(int id)
        {
            this.ID = id;
        }

        public void SetString(string translation, string original)
        {
            Translation = translation;
            string str = MakeString(translation, original);
            this.label1.Text = str;
        }

        public string MakeString(string translation, string original)
        {

            return original + '\n' + GetDivider() + '\n' + translation;
        }

        public string GetDivider()
        {
            return "------------------------------------------";
        }

		private void Label1_MouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left)
			{
				return;
			}

			Panel panel = (this.Parent as Panel);
			if (panel == null)
			{
				return;
			}
			//MessageBox.Show(panel.Parent.Name);
			VerticalView parentForm = (panel.Parent as VerticalView);
			if (parentForm == null)
			{
				return;
			}
			parentForm.OnClick(ID);
		}
    }
}
