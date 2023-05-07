namespace DGRV3TS
{
	partial class Operations
	{
		private string GetVarFromListbox(int index)
		{
			if (index == ListBox.NoMatches)
			{
				return "";
			}

			if (index >= ListBoxMenuElements.Items.Count)
			{
				ListBoxMenuElements.SelectedItem = -1;
				return "";
			}

			// Value
			string st = ListBoxMenuElements.Items[index].ToString();
			if (st.Length <= 0)
			{
				ListBoxMenuElements.SelectedIndex = -1;
				return "";
			}

			// Variable
			var unsolved = vm.UnSolve(st);
			return unsolved;
		}

		private void CreateListBoxMenu()
		{
			// This menu shows up when right-clicking an item from the listbox

			ContextMenuStrip menuStrip = new ContextMenuStrip();

			ToolStripMenuItem menuItem0 = new ToolStripMenuItem("Copy");

			menuItem0.Click += menuItem0_Click;

			menuItem0.Name = "Copy";

			ToolStripMenuItem menuItem1 = new ToolStripMenuItem("Get Lowercase");

			menuItem1.Click += menuItem1_Click;

			menuItem1.Name = "Lowercase";

			ToolStripMenuItem menuItem2 = new ToolStripMenuItem("Get Uppercase");

			menuItem2.Click += menuItem2_Click;

			menuItem2.Name = "Uppercase";

			ToolStripMenuItem menuItem3 = new ToolStripMenuItem("Remove");

			menuItem3.Click += menuItem3_Click;

			menuItem3.Name = "Remove";

			ToolStripMenuItem menuItem4 = new ToolStripMenuItem("I clicked by mistake");

			menuItem4.Click += menuItem4_Click;

			menuItem4.Name = "I clicked by mistake";

			ToolStripMenuItem menuItem5 = new ToolStripMenuItem("What is this?");

			menuItem5.Click += menuItem5_Click;

			menuItem5.Name = "What is this?";

			ToolStripMenuItem menuItem6 = new ToolStripMenuItem("Lookup variant");

			menuItem6.Click += menuItem6_Click;

			menuItem6.Name = "Lookup variant";

			menuStrip.Items.Add(menuItem0);
			menuStrip.Items.Add(menuItem1);
			menuStrip.Items.Add(menuItem2);
			menuStrip.Items.Add(menuItem3);
			menuStrip.Items.Add(menuItem6);
			menuStrip.Items.Add(menuItem5);
			menuStrip.Items.Add(menuItem4);

			contextMenuStrip1 = menuStrip;
		}
	}
}