using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using static OfficeOpenXml.ExcelErrorValue;

namespace DGRV3TS
{
	partial class Operations
	{
		private void ListBoxMenuIndex_SelectedIndexChanged(object sender, EventArgs e)
		{
			// When the listbox menu index changes, load the variables in that section

			if (AutoPlayOn)
			{
				return;
			}

			List<string> tempList = new List<string>();

			ListBoxMenuElements.Items.Clear();
			foreach (string s in vm.ListBoxes[ListBoxMenuIndex.SelectedIndex].Items)
			{
				tempList.Add(s);
			}

			tempList.Sort();

			foreach (string s in tempList)
			{
				ListBoxMenuElements.Items.Add(s);
			}

			if (ListBoxMenuElements.Items.Count == 0)
			{
				ListBoxMenuElements.Items.Add(vm.NoVarStr);
			}

			ListBoxMenuElements.SelectedIndex = -1;
			ListBoxMenuElements.Update();
		}

		private void ListBoxMenuElements_RightClick(object sender, MouseEventArgs e)
		{
			// When the listbox is right-clicked, open a menu

			if (AutoPlayOn)
			{
				return;
			}

			int index = ListBoxMenuElements.IndexFromPoint(e.Location);
			ListBoxMenuElements.SelectedIndex = index;

			if (index == ListBox.NoMatches)
			{
				return;
			}

			string st = ListBoxMenuElements.Items[index].ToString();
			if (st.Length <= 0)
			{
				return;
			}

			if (st == vm.NoVarStr)
			{
				return;
			}

			string unsolved = GetVarFromListbox(index);
			if (unsolved.Length <= 0)
			{
				return;
			}

			// Click with right button
			if (e.Button == MouseButtons.Right)
			{
				contextMenuStrip1.Show(ListBoxMenuIndex, e.Location);
			}
			else
			{
				// Click with left button
				Clipboard.SetText(unsolved);
				InputManager.Print("Copied!");
			}
		}

		private void menuItem0_Click(object sender, EventArgs e)
		{
			string temp_str = vm.UnSolve(ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString()).Item1;
			Clipboard.SetText(temp_str);
			InputManager.Print("Copied!");
		}

		private void menuItem1_Click(object sender, EventArgs e)
		{
			string name = ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString();
			string temp_str = vm.UnSolve(name).Item1;

			if (temp_str.Length <= 0)
			{
				return;
			}

			if (!temp_str.Contains("_MN"))
			{
				temp_str += "_MN";
			}

			if (vm.SolveVar(temp_str) == temp_str)
			{
				InputManager.Print(temp_str + " not found, remember to add it later!");
			}

			string comment = vm.CommentFromDefinition(temp_str);

			vm.AddVariantVWithPriority(temp_str,
				name.ToLowerInvariant(), comment);

			Clipboard.SetText(temp_str);
			InputManager.Print("Copied!");
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			string name = ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString();
			string temp_str = vm.UnSolve(name).Item1;

			if (temp_str.Length <= 0)
			{
				return;
			}

			if (!temp_str.Contains("_MS"))
			{
				temp_str += "_MS";
			}

			if (vm.SolveVar(temp_str) == temp_str)
			{
				InputManager.Print(temp_str + " not found, remember to add it later!");
			}

			string comment = vm.CommentFromDefinition(temp_str);

			vm.AddVariantVWithPriority(temp_str,
				name.ToUpperInvariant(), comment);

			Clipboard.SetText(temp_str);
			InputManager.Print("Copied!");
		}

		private void menuItem3_Click(object sender, EventArgs e)
		{
			if (ListBoxMenuIndex.SelectedIndex <= -1)
			{
				return;
			}

			if (ListBoxMenuElements.SelectedIndex <= -1)
			{
				return;
			}

			if (vm.ListBoxes.Count < ListBoxMenuIndex.SelectedIndex)
			{
				return;
			}

			int index = ListBoxMenuElements.SelectedIndex;
			string str = ListBoxMenuElements.Items[index].ToString();
			int find = vm.GetIndexInListbox(vm.ListBoxes[ListBoxMenuIndex.SelectedIndex], str);

			if (find <= -1)
			{
				return;
			}

			ListBoxMenuElements.Items.RemoveAt(ListBoxMenuElements.SelectedIndex);

			vm.ListBoxes[ListBoxMenuIndex.SelectedIndex].Items.RemoveAt(find);

			ListBoxMenuElements.SelectedIndex = -1;

			if (ListBoxMenuElements.Items.Count == 0)
			{
				ListBoxMenuElements.Items.Add(vm.NoVarStr);
			}

			ListBoxMenuElements.Update();
		}

		private void menuItem4_Click(object sender, EventArgs e)
		{
			// This needs to be empty
		}

		private void menuItem5_Click(object sender, EventArgs e)
		{
			int index = ListBoxMenuElements.SelectedIndex;
			string str = ListBoxMenuElements.Items[index].ToString();
			string var = vm.UnSolve(str).Item1;

			string view = "Index: " + index + "\nString: " + str + "\nVariable: " + var;
			InputManager.Print(view);
		}

		private void menuItem6_Click(object sender, EventArgs e)
		{
			string value = ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString();
			string tentative_raw = vm.UnSolve(value).Item1;
			VariableManager vm2 = new VariableManager(!CheckboxUseAlternateVars.Checked);
			string variant = vm2.SolveVar(tentative_raw);
			InputManager.Print(variant);
		}

		private void ListBoxMenuElements_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Needs to be empty?
		}

		private void DrawListbox(object sender, DrawItemEventArgs e)
		{
			var listbox = sender as ListBox;

			if (listbox == null || listbox.Items == null || listbox.Items.Count <= 0 ||
				e == null || e.Index < 0 || e.Index >= listbox.Items.Count || e.Font == null)
			{
				return;
			}

			var style = FontStyle.Regular;
			var item = listbox.Items[e.Index];
			if(item == null)
			{
				return;
			}
			string? itemstr = item.ToString();
			if(itemstr == null || itemstr.Length <= 0)
			{
				return;
			}

			if (listbox == ListBoxMenuElements && ListBoxMenuElements.Items.Count > 0)
			{
				var unsolve = vm.UnSolve(itemstr, false);
				string comment = vm.CommentFromDefinition(unsolve.Item1);
				if (comment.Length > 0)
				{
					style = FontStyle.Underline;
				}
				if (unsolve.Item2)
				{
					style |= FontStyle.Italic;
				}
			}

			// TODO: the text looks different, meanwhile using DrawMode.Normal the text looks fine

			e.DrawBackground();
			if (e.State == (DrawItemState.Selected | DrawItemState.NoAccelerator | DrawItemState.NoFocusRect) ||
				e.State == (DrawItemState.Selected | DrawItemState.Focus | DrawItemState.NoFocusRect | DrawItemState.NoAccelerator))
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), e.Bounds);
			}
			e.Graphics.DrawString(itemstr, new Font(e.Font.Name ?? "Segoe UI", 9, style), new SolidBrush(SystemColors.WindowText), e.Bounds);
			e.DrawFocusRectangle();
			e.Dispose();
		}

		private void ListboxMeasure(object sender, MeasureItemEventArgs e)
		{
			var listbox = sender as ListBox;
			if(listbox == null || listbox.Font == null)
			{
				return;
			}

			e.ItemHeight = listbox.Font.Height;
		}
	}
}