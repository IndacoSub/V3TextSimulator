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
			string temp_str = vm.UnSolve(ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString());
			Clipboard.SetText(temp_str);
			InputManager.Print("Copied!");
		}

		private void menuItem1_Click(object sender, EventArgs e)
		{
			string temp_str = vm.UnSolve(ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString());

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

			vm.AddVariantVWithPriority(temp_str,
				ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString().ToLowerInvariant());

			Clipboard.SetText(temp_str);
			InputManager.Print("Copied!");
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			string temp_str = vm.UnSolve(ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString());

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

			vm.AddVariantVWithPriority(temp_str,
				ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString().ToUpperInvariant());

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
			string var = vm.UnSolve(str);

			string view = "Index: " + index + "\nString: " + str + "\nVariable: " + var;
			InputManager.Print(view);
		}

		private void menuItem6_Click(object sender, EventArgs e)
		{
			string value = ListBoxMenuElements.Items[ListBoxMenuElements.SelectedIndex].ToString();
			string tentative_raw = vm.UnSolve(value);
			VariableManager vm2 = new VariableManager(!CheckboxUseAlternateVars.Checked);
			string variant = vm2.SolveVar(tentative_raw);
			InputManager.Print(variant);
		}

		private void ListBoxMenuElements_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Needs to be empty?
		}
	}
}