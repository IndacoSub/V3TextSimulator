namespace DGRV3TS
{
	public enum GameIndex
	{
		V3,
		AI,

		Count,
	};

	partial class Operations
	{
		GameIndex CurrentGameIndex = GameIndex.V3;

		private bool AutoPlayOn;
		private bool AltVars;
		public bool DEBUG_ON = false;
		private bool FastReading;

		private FileManager fi;
		private FontManager fm = new FontManager("Arial");
		private ImageManager im;

		private bool LoadedFile;
		private SoundManager sm;

		private TranslationManager tm = new TranslationManager();
		private VariableManager vm = new VariableManager(false, GameIndex.V3);

		private void InitWindow()
		{
			// DR is default
			if (im.BackgroundImagesDR.Count <= 0)
			{
				return;
			}
			if (dialogue_window.DisplayedImage.Image != null) dialogue_window.DisplayedImage.Image.Dispose();
			dialogue_window.DisplayedImage.Image = new Bitmap(new Bitmap(im.BackgroundImagesDR[CB_TB.SelectedIndex]),
				new Size(dialogue_window.DisplayedImage.Width, dialogue_window.DisplayedImage.Height));
			dialogue_window.DisplayedImage.Refresh();
		}
		private void Init()
		{
			CreateListBoxMenu();

			listbox_tooltip = new ToolTip
			{
				AutoPopDelay = 0,
				InitialDelay = 0,
				ReshowDelay = 0,
				UseFading = false,
				UseAnimation = false,
				AutomaticDelay = 0,
				ShowAlways = true
			};

			LoadedFile = new bool();
			LoadedFile = false;

			AutoPlayOn = new bool();
			AutoPlayOn = false;

			FastReading = new bool();
			FastReading = false;

			AltVars = new bool();
			AltVars = false;

			CB_Game.Items.Clear();
			CB_Game.Items.Add("V3");
			CB_Game.Items.Add("AI");

			CB_Game.SelectedIndex = 0;

			// Check if the executable name is DGRV3TEST
			// If so, enable some debug functionalities
			// TODO: Such as...?
			if (AppDomain.CurrentDomain.FriendlyName.Contains("DGRV3TEST"))
			{
				DEBUG_ON = true;
			}
#if DEBUG
            DEBUG_ON = true;
#endif

			translationModeToolStripMenuItem.Checked = true;
			replaceVariablesToolStripMenuItem.Checked = true;
			CheckboxDisplayOriginalText.Enabled = false;
			CheckboxPauseAutoplay.Enabled = false;
			CheckboxStartAutoplay.Enabled = false;
			useAlternateVarsToolStripMenuItem.Enabled = false;
			LabelVoiceline.Visible = false;
			LabelCharacterName.Visible = false;
			LabelCurrentAnimation.Visible = false;
			LabelLineNumber.Visible = false;
			LabelOriginFile.Visible = false;
			LabelCurrentTranslation.Visible = false;
			ButtonBackLanguage.Visible = false;
			ButtonBackText.Visible = false;
			ButtonNextLanguage.Visible = false;
			ButtonNextText.Visible = false;
			LabelCurrentAnimation.Text = "";
			LabelVoiceline.Text = "";

			im = new ImageManager(CurrentGameIndex);

			InitCBTB(CurrentGameIndex);

			ReloadListboxes();

			tm = new TranslationManager();

			fm = new FontManager("");

			NumericUpDownFontSize.Value = (int)fm.CurrentFontSize;

			NumericUpDownFontSize.ValueChanged += NumericUpDownFontSize_ValueChanged;

			fi = new FileManager();

			fi.FMGameIndex = CurrentGameIndex;

			CB_TB.SelectedIndexChanged += CB_TB_SelectedIndexChanged;

			TextboxCurrentLanguage.Text = "en-US";

			sm = new SoundManager();

			ButtonReloadText_Click(null, null);

			// Create the ToolTip and associate with the Form container.
			ListBoxToolTip = new ToolTip();

			// Set up the delays for the ToolTip.
			ListBoxToolTip.AutoPopDelay = 5000;
			ListBoxToolTip.InitialDelay = 1000;
			ListBoxToolTip.ReshowDelay = 500;
			// Force the ToolTip text to be displayed whether or not the form is active.
			ListBoxToolTip.ShowAlways = true;

			foreach(var toolstrips in this.Controls)
			{
				var menustrips = (toolstrips) as MenuStrip;
				if(menustrips == null)
				{
					continue;
				}
				foreach (ToolStripMenuItem children in menustrips.Items)
				{
					if (children == null)
					{
						continue;
					}
					bool any_children_with_check = false;
					if (children.HasDropDownItems)
					{
						foreach (ToolStripMenuItem child in children.DropDownItems)
						{
							if (child == null)
							{
								continue;
							}

							if (child.HasDropDownItems)
							{
								foreach (ToolStripMenuItem superchild in child.DropDownItems)
								{
									if (superchild == null)
									{
										continue;
									}
									if (superchild.CheckOnClick)
									{
										superchild.DropDown.Closing += DropDown_Closing;
										any_children_with_check |= true;
									}
								}
							}

							if (child.CheckOnClick || any_children_with_check)
							{
								child.DropDown.Closing += DropDown_Closing;
							}
						}
					}

					if(children.CheckOnClick || any_children_with_check)
					{
						children.DropDown.Closing += DropDown_Closing;
					}
				}
			}

			// Collect garbage from initialization?
			GC.Collect();
		}

		private void DropDown_Closing(object? sender, ToolStripDropDownClosingEventArgs e)
		{
			var tsdd = sender as ToolStripDropDown;
			if(tsdd == null)
			{
				return;
			}
			Point p = tsdd.PointToClient(Control.MousePosition);
			if (tsdd.ClientRectangle.Contains(p))
			{
				e.Cancel = true;
			}
		}

		private void ReloadListboxes()
		{
			if (AutoPlayOn || FastReading)
			{
				return;
			}

			vm = new VariableManager(AltVars, CurrentGameIndex);

			ListBoxMenuIndex.Items.Clear();
			ListBoxMenuElements.Items.Clear();

			foreach (string ms in vm.Menu.Items)
			{
				ListBoxMenuIndex.Items.Add(ms);
			}
			bool has_vars = ListBoxMenuIndex.Items.Count > 0;
			ListBoxMenuIndex.Enabled = has_vars;
			ListBoxMenuElements.Enabled = has_vars;
			ListBoxMenuIndex.BackColor = ListBoxMenuIndex.Enabled ? Color.White : Color.Gainsboro;
			ListBoxMenuElements.BackColor = ListBoxMenuElements.Enabled ? Color.White : Color.Gainsboro;
			ListBoxMenuElements.DrawMode = DrawMode.OwnerDrawFixed;
			ListBoxMenuIndex.DrawMode = DrawMode.OwnerDrawFixed;
			//ListBoxMenuElements.DrawMode = DrawMode.Normal;

			useAlternateVarsToolStripMenuItem.Enabled = has_vars;
		}

		private void InitCBTB(GameIndex index)
		{
			// Add the backgrounds from /Graphics/Backgrounds/ and assign them a name

			List<string> backgrounds = new List<string>();

			switch (index)
			{
				case GameIndex.V3:
					backgrounds = im.BackgroundImagesDR;
					break;
				case GameIndex.AI:
					backgrounds = im.BackgroundImagesAI;
					break;
			}

			CB_TB.Items.Clear();

			if (backgrounds.Count <= 0)
			{
				return;
			}

			foreach (string file in backgrounds)
			{
				string fn = Path.GetFileNameWithoutExtension(file);
				CB_TB.Items.Add(fn);
			}

			CB_TB.SelectedIndex = 0;
		}
	}
}