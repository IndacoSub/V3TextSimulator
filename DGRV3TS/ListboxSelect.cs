namespace DGRV3TS
{
	public partial class ListboxSelect : Form
	{
		public int SelectedElement = -1;
		public ListboxSelect()
		{
			InitializeComponent();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedElement = listBox1.SelectedIndex;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}
	}
}
