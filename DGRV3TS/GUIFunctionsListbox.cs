using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using static DGRV3TS.VariableManager;
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
				string final_s = s;
				final_s = vm.ReplaceVars(final_s);
				ListBoxMenuElements.Items.Add(final_s);
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
			(VariableEntry v, int i) = vm.EntryByValue(ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString());
			Clipboard.SetText(v.Definition);
			InputManager.Print("Copied!");
		}

		private void menuItem1_Click(object sender, EventArgs e)
		{
			string name = ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString();
			(VariableEntry v, int i) = vm.EntryByValue(name);

			if (v.Definition.Length <= 0)
			{
				return;
			}

			if (!v.Definition.Contains("_MN"))
			{
				v.Definition += "_MN";
			}

			if (vm.EntryByDefinition(v.Definition).Value == v.Definition)
			{
				InputManager.Print(v.Definition + " not found, remember to add it later!");
			}

			string comment = vm.EntryByDefinition(v.Definition).Comment;

			vm.AddVariantVWithPriority(v.Definition,
				name.ToLowerInvariant(), comment);

			Clipboard.SetText(v.Definition);
			InputManager.Print("Copied!");
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			string name = ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString();
			(VariableEntry v, int i) = vm.EntryByValue(name);

			if (v.Definition.Length <= 0)
			{
				return;
			}

			if (!v.Definition.Contains("_MS"))
			{
				v.Definition += "_MS";
			}

			if (vm.EntryByDefinition(v.Definition).Value == v.Definition)
			{
				InputManager.Print(v.Definition + " not found, remember to add it later!");
			}

			string comment = vm.EntryByDefinition(v.Definition).Comment;

			vm.AddVariantVWithPriority(v.Definition,
				name.ToUpperInvariant(), comment);

			Clipboard.SetText(v.Definition);
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
			(VariableEntry v, int i) = vm.EntryByValue(str);

			string view = "Index: " + index + "\nString: " + str + "\nVariable: " + v.Definition;
			InputManager.Print(view);
		}

		private void menuItem6_Click(object sender, EventArgs e)
		{
			string value = ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString();
			(VariableEntry v, int i) = vm.EntryByValue(value);
			VariableManager vm2 = new VariableManager(!CheckboxUseAlternateVars.Checked);
			string variant = vm.EntryByDefinition(v.Definition).Value;
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

			if (itemstr != vm.NoVarStr && !itemstr.Contains("MAKE_") && !itemstr.Contains("MY_ARG"))
			{
				if (listbox == ListBoxMenuElements && ListBoxMenuElements.Items.Count > 0)
				{
					(VariableEntry v, int i) = vm.EntryByValue(itemstr);
					if (v == null)
					{
#if DEBUG
						Debug.WriteLine("v == null, itemstr: " + itemstr);
#else
						Console.WriteLine("v == null, itemstr: " + itemstr);
#endif
						return;
					}
					VariableEntry vc = vm.EntryByDefinition(v.Definition);
					if(vc == null)
					{
						return;
					}
					string comment = vc.Comment;
					if (comment.Length > 0)
					{
						style = FontStyle.Underline;
					}
					if (i > 1)
					{
						style |= FontStyle.Italic;
					}
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