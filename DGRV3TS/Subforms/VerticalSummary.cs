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
        public int ID = 0;

        public VerticalSummary()
        {
            InitializeComponent();
        }

        public void SetID(int id)
        {
            this.ID = id;
        }

        public void SetString(string translation, string original)
        {

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

        private void label1_Click(object sender, EventArgs e)
        {
            Panel panel = (this.Parent as Panel);
            if(panel == null)
            {
                return;
            }
            //MessageBox.Show(panel.Parent.Name);
            VerticalView parentForm = (panel.Parent as VerticalView);
            if(parentForm == null)
            {
                return;
            }
            parentForm.OnClick(ID);

        }
    }
}
