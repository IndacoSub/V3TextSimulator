namespace DGRV3TS
{
	partial class Operations
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			Textbox = new RichTextBox();
			CB_Game = new ComboBox();
			LabelUnsupportedWarning = new Label();
			CB_TB = new ComboBox();
			LabelTextboxStyle = new Label();
			ButtonReloadText = new Button();
			ListBoxMenuElements = new ListBox();
			ListBoxMenuIndex = new ListBox();
			ButtonNextLanguage = new Button();
			ButtonBackLanguage = new Button();
			ButtonNextText = new Button();
			ButtonBackText = new Button();
			LabelVoiceline = new Label();
			LabelCurrentAnimation = new Label();
			LabelCurrentTranslation = new Label();
			LabelOriginFile = new Label();
			LabelCharacterName = new Label();
			LabelLineNumber = new Label();
			CheckboxPauseAutoplay = new CheckBox();
			CheckboxStartAutoplay = new CheckBox();
			CheckboxDisplayOriginalText = new CheckBox();
			TextboxCurrentLanguage = new TextBox();
			LabelFontSize = new Label();
			NumericUpDownFontSize = new NumericUpDown();
			ListBoxRightClickCMS = new ContextMenuStrip(components);
			ListBoxToolTip = new ToolTip(components);
			MainMenuStrip = new MenuStrip();
			fileToolStripMenuItem = new ToolStripMenuItem();
			openFileToolStripMenuItem = new ToolStripMenuItem();
			saveFileToolStripMenuItem = new ToolStripMenuItem();
			saveScreenshotToolStripMenuItem = new ToolStripMenuItem();
			copyScreenshotToolStripMenuItem = new ToolStripMenuItem();
			editToolStripMenuItem = new ToolStripMenuItem();
			fastReadToolStripMenuItem = new ToolStripMenuItem();
			resetToolStripMenuItem = new ToolStripMenuItem();
			reloadVariablesToolStripMenuItem = new ToolStripMenuItem();
			maximizeWindowToolStripMenuItem = new ToolStripMenuItem();
			settingsToolStripMenuItem = new ToolStripMenuItem();
			translationModeToolStripMenuItem = new ToolStripMenuItem();
			autoTranslationToolStripMenuItem = new ToolStripMenuItem();
			replaceVariablesToolStripMenuItem = new ToolStripMenuItem();
			useAlternateVarsToolStripMenuItem = new ToolStripMenuItem();
			displayCharacterToolStripMenuItem = new ToolStripMenuItem();
			enableTTSVoicelinesToolStripMenuItem = new ToolStripMenuItem();
			experimentalToolStripMenuItem = new ToolStripMenuItem();
			sizeadjustedHeightToolStripMenuItem = new ToolStripMenuItem();
			toolsToolStripMenuItem = new ToolStripMenuItem();
			openGraphicsWinToolStripMenuItem = new ToolStripMenuItem();
			reopenVerticalViewToolStripMenuItem = new ToolStripMenuItem();
			setupSpritesFromFolderToolStripMenuItem = new ToolStripMenuItem();
			setupVoicelinesFromFolderToolStripMenuItem = new ToolStripMenuItem();
			dumpVoicelinesToolStripMenuItem = new ToolStripMenuItem();
			aboutToolStripMenuItem = new ToolStripMenuItem();
			tODOToolStripMenuItem = new ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)NumericUpDownFontSize).BeginInit();
			MainMenuStrip.SuspendLayout();
			SuspendLayout();
			// 
			// Textbox
			// 
			Textbox.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
			Textbox.Location = new Point(12, 32);
			Textbox.Margin = new Padding(4, 3, 4, 3);
			Textbox.Name = "Textbox";
			Textbox.Size = new Size(520, 78);
			Textbox.TabIndex = 3;
			Textbox.Text = "This is an example\\nmessage.";
			Textbox.TextChanged += Textbox_TextChanged;
			// 
			// CB_Game
			// 
			CB_Game.FormattingEnabled = true;
			CB_Game.Location = new Point(13, 119);
			CB_Game.Margin = new Padding(4, 3, 4, 3);
			CB_Game.Name = "CB_Game";
			CB_Game.Size = new Size(72, 23);
			CB_Game.TabIndex = 67;
			CB_Game.Text = "Game";
			CB_Game.SelectedIndexChanged += CB_Game_SelectedIndexChanged;
			// 
			// LabelUnsupportedWarning
			// 
			LabelUnsupportedWarning.AutoSize = true;
			LabelUnsupportedWarning.ForeColor = Color.Red;
			LabelUnsupportedWarning.Location = new Point(10, 153);
			LabelUnsupportedWarning.Margin = new Padding(4, 0, 4, 0);
			LabelUnsupportedWarning.Name = "LabelUnsupportedWarning";
			LabelUnsupportedWarning.Size = new Size(155, 15);
			LabelUnsupportedWarning.TabIndex = 63;
			LabelUnsupportedWarning.Text = "Unsupported character(s): {}";
			LabelUnsupportedWarning.Visible = false;
			// 
			// CB_TB
			// 
			CB_TB.DropDownStyle = ComboBoxStyle.DropDownList;
			CB_TB.FormattingEnabled = true;
			CB_TB.Location = new Point(131, 119);
			CB_TB.Margin = new Padding(4, 3, 4, 3);
			CB_TB.Name = "CB_TB";
			CB_TB.Size = new Size(173, 23);
			CB_TB.TabIndex = 62;
			CB_TB.SelectedIndexChanged += CB_TB_SelectedIndexChanged;
			// 
			// LabelTextboxStyle
			// 
			LabelTextboxStyle.AutoSize = true;
			LabelTextboxStyle.Location = new Point(90, 124);
			LabelTextboxStyle.Margin = new Padding(4, 0, 4, 0);
			LabelTextboxStyle.Name = "LabelTextboxStyle";
			LabelTextboxStyle.Size = new Size(35, 15);
			LabelTextboxStyle.TabIndex = 61;
			LabelTextboxStyle.Text = "Style:";
			// 
			// ButtonReloadText
			// 
			ButtonReloadText.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			ButtonReloadText.Location = new Point(312, 114);
			ButtonReloadText.Margin = new Padding(4, 3, 4, 3);
			ButtonReloadText.Name = "ButtonReloadText";
			ButtonReloadText.Size = new Size(79, 32);
			ButtonReloadText.TabIndex = 60;
			ButtonReloadText.Text = "Refresh";
			ButtonReloadText.UseVisualStyleBackColor = true;
			ButtonReloadText.Click += ButtonReloadText_Click;
			// 
			// ListBoxMenuElements
			// 
			ListBoxMenuElements.FormattingEnabled = true;
			ListBoxMenuElements.ItemHeight = 15;
			ListBoxMenuElements.Location = new Point(766, 32);
			ListBoxMenuElements.Margin = new Padding(4, 3, 4, 3);
			ListBoxMenuElements.Name = "ListBoxMenuElements";
			ListBoxMenuElements.Size = new Size(342, 79);
			ListBoxMenuElements.TabIndex = 73;
			ListBoxMenuElements.DrawItem += DrawListbox;
			ListBoxMenuElements.MeasureItem += ListboxMeasure;
			ListBoxMenuElements.SelectedIndexChanged += ListBoxMenuElements_SelectedIndexChanged;
			ListBoxMenuElements.MouseMove += ListBoxOnMouseMove;
			ListBoxMenuElements.MouseUp += ListBoxMenuElements_RightClick;
			// 
			// ListBoxMenuIndex
			// 
			ListBoxMenuIndex.FormattingEnabled = true;
			ListBoxMenuIndex.ItemHeight = 15;
			ListBoxMenuIndex.Location = new Point(540, 32);
			ListBoxMenuIndex.Margin = new Padding(4, 3, 4, 3);
			ListBoxMenuIndex.Name = "ListBoxMenuIndex";
			ListBoxMenuIndex.Size = new Size(218, 79);
			ListBoxMenuIndex.TabIndex = 72;
			ListBoxMenuIndex.DrawItem += DrawListbox;
			ListBoxMenuIndex.MeasureItem += ListboxMeasure;
			ListBoxMenuIndex.SelectedIndexChanged += ListBoxMenuIndex_SelectedIndexChanged;
			// 
			// ButtonNextLanguage
			// 
			ButtonNextLanguage.Location = new Point(467, 149);
			ButtonNextLanguage.Margin = new Padding(4, 3, 4, 3);
			ButtonNextLanguage.Name = "ButtonNextLanguage";
			ButtonNextLanguage.Size = new Size(65, 23);
			ButtonNextLanguage.TabIndex = 71;
			ButtonNextLanguage.Text = "Lang ++";
			ButtonNextLanguage.UseVisualStyleBackColor = true;
			ButtonNextLanguage.Click += ButtonNextLanguage_Click;
			// 
			// ButtonBackLanguage
			// 
			ButtonBackLanguage.Location = new Point(399, 149);
			ButtonBackLanguage.Margin = new Padding(4, 3, 4, 3);
			ButtonBackLanguage.Name = "ButtonBackLanguage";
			ButtonBackLanguage.Size = new Size(60, 23);
			ButtonBackLanguage.TabIndex = 70;
			ButtonBackLanguage.Text = "Lang --";
			ButtonBackLanguage.UseVisualStyleBackColor = true;
			ButtonBackLanguage.Click += ButtonBackLanguage_Click;
			// 
			// ButtonNextText
			// 
			ButtonNextText.Location = new Point(467, 114);
			ButtonNextText.Margin = new Padding(4, 3, 4, 3);
			ButtonNextText.Name = "ButtonNextText";
			ButtonNextText.Size = new Size(65, 32);
			ButtonNextText.TabIndex = 69;
			ButtonNextText.Text = "Line ++";
			ButtonNextText.UseVisualStyleBackColor = true;
			ButtonNextText.Click += ButtonNextText_Click;
			// 
			// ButtonBackText
			// 
			ButtonBackText.Location = new Point(399, 114);
			ButtonBackText.Margin = new Padding(4, 3, 4, 3);
			ButtonBackText.Name = "ButtonBackText";
			ButtonBackText.Size = new Size(60, 32);
			ButtonBackText.TabIndex = 68;
			ButtonBackText.Text = "Line --";
			ButtonBackText.UseVisualStyleBackColor = true;
			ButtonBackText.Click += ButtonBackText_Click;
			// 
			// LabelVoiceline
			// 
			LabelVoiceline.AutoSize = true;
			LabelVoiceline.Location = new Point(766, 129);
			LabelVoiceline.Margin = new Padding(4, 0, 4, 0);
			LabelVoiceline.Name = "LabelVoiceline";
			LabelVoiceline.Size = new Size(57, 15);
			LabelVoiceline.TabIndex = 80;
			LabelVoiceline.Text = "Voiceline:";
			// 
			// LabelCurrentAnimation
			// 
			LabelCurrentAnimation.AutoSize = true;
			LabelCurrentAnimation.Location = new Point(766, 114);
			LabelCurrentAnimation.Margin = new Padding(4, 0, 4, 0);
			LabelCurrentAnimation.Name = "LabelCurrentAnimation";
			LabelCurrentAnimation.Size = new Size(66, 15);
			LabelCurrentAnimation.TabIndex = 79;
			LabelCurrentAnimation.Text = "Animation:";
			// 
			// LabelCurrentTranslation
			// 
			LabelCurrentTranslation.AutoSize = true;
			LabelCurrentTranslation.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			LabelCurrentTranslation.Location = new Point(538, 159);
			LabelCurrentTranslation.Margin = new Padding(4, 0, 4, 0);
			LabelCurrentTranslation.Name = "LabelCurrentTranslation";
			LabelCurrentTranslation.Size = new Size(67, 13);
			LabelCurrentTranslation.TabIndex = 77;
			LabelCurrentTranslation.Text = "Translations:";
			// 
			// LabelOriginFile
			// 
			LabelOriginFile.AutoSize = true;
			LabelOriginFile.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			LabelOriginFile.Location = new Point(538, 145);
			LabelOriginFile.Margin = new Padding(4, 0, 4, 0);
			LabelOriginFile.Name = "LabelOriginFile";
			LabelOriginFile.Size = new Size(53, 13);
			LabelOriginFile.TabIndex = 76;
			LabelOriginFile.Text = "Origin file:";
			// 
			// LabelCharacterName
			// 
			LabelCharacterName.AutoSize = true;
			LabelCharacterName.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			LabelCharacterName.Location = new Point(538, 130);
			LabelCharacterName.Margin = new Padding(4, 0, 4, 0);
			LabelCharacterName.Name = "LabelCharacterName";
			LabelCharacterName.Size = new Size(56, 13);
			LabelCharacterName.TabIndex = 75;
			LabelCharacterName.Text = "Character:";
			// 
			// LabelLineNumber
			// 
			LabelLineNumber.AutoSize = true;
			LabelLineNumber.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			LabelLineNumber.Location = new Point(538, 116);
			LabelLineNumber.Margin = new Padding(4, 0, 4, 0);
			LabelLineNumber.Name = "LabelLineNumber";
			LabelLineNumber.Size = new Size(30, 13);
			LabelLineNumber.TabIndex = 74;
			LabelLineNumber.Text = "Line:";
			// 
			// CheckboxPauseAutoplay
			// 
			CheckboxPauseAutoplay.AutoSize = true;
			CheckboxPauseAutoplay.Location = new Point(963, 153);
			CheckboxPauseAutoplay.Margin = new Padding(4, 3, 4, 3);
			CheckboxPauseAutoplay.Name = "CheckboxPauseAutoplay";
			CheckboxPauseAutoplay.Size = new Size(108, 19);
			CheckboxPauseAutoplay.TabIndex = 86;
			CheckboxPauseAutoplay.Text = "Pause Autoplay";
			CheckboxPauseAutoplay.UseVisualStyleBackColor = true;
			CheckboxPauseAutoplay.CheckedChanged += CheckboxPauseAutoplay_CheckedChanged;
			// 
			// CheckboxStartAutoplay
			// 
			CheckboxStartAutoplay.AutoSize = true;
			CheckboxStartAutoplay.Location = new Point(963, 135);
			CheckboxStartAutoplay.Margin = new Padding(4, 3, 4, 3);
			CheckboxStartAutoplay.Name = "CheckboxStartAutoplay";
			CheckboxStartAutoplay.Size = new Size(148, 19);
			CheckboxStartAutoplay.TabIndex = 84;
			CheckboxStartAutoplay.Text = "Start/Resume AutoPlay";
			CheckboxStartAutoplay.UseVisualStyleBackColor = true;
			CheckboxStartAutoplay.CheckedChanged += CheckboxStartAutoplay_CheckedChanged;
			// 
			// CheckboxDisplayOriginalText
			// 
			CheckboxDisplayOriginalText.AutoSize = true;
			CheckboxDisplayOriginalText.Location = new Point(963, 116);
			CheckboxDisplayOriginalText.Margin = new Padding(4, 3, 4, 3);
			CheckboxDisplayOriginalText.Name = "CheckboxDisplayOriginalText";
			CheckboxDisplayOriginalText.Size = new Size(133, 19);
			CheckboxDisplayOriginalText.TabIndex = 81;
			CheckboxDisplayOriginalText.Text = "Display Original Text";
			CheckboxDisplayOriginalText.UseVisualStyleBackColor = true;
			CheckboxDisplayOriginalText.CheckedChanged += CheckboxDisplayOriginalText_CheckedChanged;
			// 
			// TextboxCurrentLanguage
			// 
			TextboxCurrentLanguage.Location = new Point(312, 148);
			TextboxCurrentLanguage.Margin = new Padding(4, 3, 4, 3);
			TextboxCurrentLanguage.Name = "TextboxCurrentLanguage";
			TextboxCurrentLanguage.PlaceholderText = "TTSLanguage";
			TextboxCurrentLanguage.Size = new Size(79, 23);
			TextboxCurrentLanguage.TabIndex = 97;
			TextboxCurrentLanguage.TextAlign = HorizontalAlignment.Center;
			// 
			// LabelFontSize
			// 
			LabelFontSize.AutoSize = true;
			LabelFontSize.Location = new Point(766, 153);
			LabelFontSize.Margin = new Padding(4, 0, 4, 0);
			LabelFontSize.Name = "LabelFontSize";
			LabelFontSize.Size = new Size(57, 15);
			LabelFontSize.TabIndex = 95;
			LabelFontSize.Text = "Font Size:";
			// 
			// NumericUpDownFontSize
			// 
			NumericUpDownFontSize.Location = new Point(831, 149);
			NumericUpDownFontSize.Margin = new Padding(4, 3, 4, 3);
			NumericUpDownFontSize.Name = "NumericUpDownFontSize";
			NumericUpDownFontSize.Size = new Size(59, 23);
			NumericUpDownFontSize.TabIndex = 94;
			NumericUpDownFontSize.TextAlign = HorizontalAlignment.Center;
			NumericUpDownFontSize.ValueChanged += NumericUpDownFontSize_ValueChanged;
			// 
			// ListBoxRightClickCMS
			// 
			ListBoxRightClickCMS.Name = "contextMenuStrip1";
			ListBoxRightClickCMS.Size = new Size(61, 4);
			// 
			// MainMenuStrip
			// 
			MainMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, settingsToolStripMenuItem, toolsToolStripMenuItem, aboutToolStripMenuItem });
			MainMenuStrip.Location = new Point(0, 0);
			MainMenuStrip.Name = "MainMenuStrip";
			MainMenuStrip.Size = new Size(1119, 24);
			MainMenuStrip.TabIndex = 102;
			MainMenuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem, saveFileToolStripMenuItem, saveScreenshotToolStripMenuItem, copyScreenshotToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			// 
			// openFileToolStripMenuItem
			// 
			openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
			openFileToolStripMenuItem.Size = new Size(163, 22);
			openFileToolStripMenuItem.Text = "Open File/Font";
			openFileToolStripMenuItem.Click += openFileToolStripMenuItem_Click;
			// 
			// saveFileToolStripMenuItem
			// 
			saveFileToolStripMenuItem.Name = "saveFileToolStripMenuItem";
			saveFileToolStripMenuItem.Size = new Size(163, 22);
			saveFileToolStripMenuItem.Text = "Save File As...";
			saveFileToolStripMenuItem.Click += saveFileToolStripMenuItem_Click;
			// 
			// saveScreenshotToolStripMenuItem
			// 
			saveScreenshotToolStripMenuItem.Name = "saveScreenshotToolStripMenuItem";
			saveScreenshotToolStripMenuItem.Size = new Size(163, 22);
			saveScreenshotToolStripMenuItem.Text = "Save Screenshot";
			saveScreenshotToolStripMenuItem.Click += saveScreenshotToolStripMenuItem_Click;
			// 
			// copyScreenshotToolStripMenuItem
			// 
			copyScreenshotToolStripMenuItem.Name = "copyScreenshotToolStripMenuItem";
			copyScreenshotToolStripMenuItem.Size = new Size(163, 22);
			copyScreenshotToolStripMenuItem.Text = "Copy Screenshot";
			copyScreenshotToolStripMenuItem.Click += copyScreenshotToolStripMenuItem_Click;
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { fastReadToolStripMenuItem, resetToolStripMenuItem, reloadVariablesToolStripMenuItem, maximizeWindowToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new Size(39, 20);
			editToolStripMenuItem.Text = "Edit";
			// 
			// fastReadToolStripMenuItem
			// 
			fastReadToolStripMenuItem.Name = "fastReadToolStripMenuItem";
			fastReadToolStripMenuItem.Size = new Size(193, 22);
			fastReadToolStripMenuItem.Text = "Fast-Read";
			fastReadToolStripMenuItem.Click += fastReadToolStripMenuItem_Click;
			// 
			// resetToolStripMenuItem
			// 
			resetToolStripMenuItem.Name = "resetToolStripMenuItem";
			resetToolStripMenuItem.Size = new Size(193, 22);
			resetToolStripMenuItem.Text = "Reset reading progress";
			resetToolStripMenuItem.Click += resetToolStripMenuItem_Click;
			// 
			// reloadVariablesToolStripMenuItem
			// 
			reloadVariablesToolStripMenuItem.Name = "reloadVariablesToolStripMenuItem";
			reloadVariablesToolStripMenuItem.Size = new Size(193, 22);
			reloadVariablesToolStripMenuItem.Text = "Reload Variables";
			reloadVariablesToolStripMenuItem.Click += reloadVariablesToolStripMenuItem_Click;
			// 
			// maximizeWindowToolStripMenuItem
			// 
			maximizeWindowToolStripMenuItem.CheckOnClick = true;
			maximizeWindowToolStripMenuItem.Name = "maximizeWindowToolStripMenuItem";
			maximizeWindowToolStripMenuItem.Size = new Size(193, 22);
			maximizeWindowToolStripMenuItem.Text = "Maximize Window";
			maximizeWindowToolStripMenuItem.Click += maximizeWindowToolStripMenuItem_Click;
			// 
			// settingsToolStripMenuItem
			// 
			settingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { translationModeToolStripMenuItem, replaceVariablesToolStripMenuItem, displayCharacterToolStripMenuItem, enableTTSVoicelinesToolStripMenuItem, experimentalToolStripMenuItem });
			settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			settingsToolStripMenuItem.Size = new Size(61, 20);
			settingsToolStripMenuItem.Text = "Settings";
			// 
			// translationModeToolStripMenuItem
			// 
			translationModeToolStripMenuItem.CheckOnClick = true;
			translationModeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { autoTranslationToolStripMenuItem });
			translationModeToolStripMenuItem.Name = "translationModeToolStripMenuItem";
			translationModeToolStripMenuItem.Size = new Size(187, 22);
			translationModeToolStripMenuItem.Text = "Translation Mode";
			translationModeToolStripMenuItem.CheckedChanged += TranslationModeToolStripMenuItem_CheckedChanged;
			translationModeToolStripMenuItem.Click += translationModeToolStripMenuItem_Click;
			// 
			// autoTranslationToolStripMenuItem
			// 
			autoTranslationToolStripMenuItem.CheckOnClick = true;
			autoTranslationToolStripMenuItem.Name = "autoTranslationToolStripMenuItem";
			autoTranslationToolStripMenuItem.Size = new Size(157, 22);
			autoTranslationToolStripMenuItem.Text = "AutoTranslation";
			// 
			// replaceVariablesToolStripMenuItem
			// 
			replaceVariablesToolStripMenuItem.Checked = true;
			replaceVariablesToolStripMenuItem.CheckOnClick = true;
			replaceVariablesToolStripMenuItem.CheckState = CheckState.Checked;
			replaceVariablesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { useAlternateVarsToolStripMenuItem });
			replaceVariablesToolStripMenuItem.Name = "replaceVariablesToolStripMenuItem";
			replaceVariablesToolStripMenuItem.Size = new Size(187, 22);
			replaceVariablesToolStripMenuItem.Text = "Replace Variables";
			replaceVariablesToolStripMenuItem.Click += replaceVariablesToolStripMenuItem_Click;
			// 
			// useAlternateVarsToolStripMenuItem
			// 
			useAlternateVarsToolStripMenuItem.CheckOnClick = true;
			useAlternateVarsToolStripMenuItem.Name = "useAlternateVarsToolStripMenuItem";
			useAlternateVarsToolStripMenuItem.Size = new Size(168, 22);
			useAlternateVarsToolStripMenuItem.Text = "Use Alternate Vars";
			useAlternateVarsToolStripMenuItem.Click += useAlternateVarsToolStripMenuItem_Click;
			// 
			// displayCharacterToolStripMenuItem
			// 
			displayCharacterToolStripMenuItem.CheckOnClick = true;
			displayCharacterToolStripMenuItem.Name = "displayCharacterToolStripMenuItem";
			displayCharacterToolStripMenuItem.Size = new Size(187, 22);
			displayCharacterToolStripMenuItem.Text = "Display Character";
			displayCharacterToolStripMenuItem.Click += displayCharacterToolStripMenuItem_Click;
			// 
			// enableTTSVoicelinesToolStripMenuItem
			// 
			enableTTSVoicelinesToolStripMenuItem.CheckOnClick = true;
			enableTTSVoicelinesToolStripMenuItem.Name = "enableTTSVoicelinesToolStripMenuItem";
			enableTTSVoicelinesToolStripMenuItem.Size = new Size(187, 22);
			enableTTSVoicelinesToolStripMenuItem.Text = "Enable TTS/Voicelines";
			// 
			// experimentalToolStripMenuItem
			// 
			experimentalToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { sizeadjustedHeightToolStripMenuItem });
			experimentalToolStripMenuItem.Name = "experimentalToolStripMenuItem";
			experimentalToolStripMenuItem.Size = new Size(187, 22);
			experimentalToolStripMenuItem.Text = "Experimental";
			// 
			// sizeadjustedHeightToolStripMenuItem
			// 
			sizeadjustedHeightToolStripMenuItem.CheckOnClick = true;
			sizeadjustedHeightToolStripMenuItem.Name = "sizeadjustedHeightToolStripMenuItem";
			sizeadjustedHeightToolStripMenuItem.Size = new Size(183, 22);
			sizeadjustedHeightToolStripMenuItem.Text = "Size-adjusted Height";
			// 
			// toolsToolStripMenuItem
			// 
			toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openGraphicsWinToolStripMenuItem, reopenVerticalViewToolStripMenuItem, setupSpritesFromFolderToolStripMenuItem, setupVoicelinesFromFolderToolStripMenuItem, dumpVoicelinesToolStripMenuItem });
			toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			toolsToolStripMenuItem.Size = new Size(46, 20);
			toolsToolStripMenuItem.Text = "Tools";
			// 
			// openGraphicsWinToolStripMenuItem
			// 
			openGraphicsWinToolStripMenuItem.Name = "openGraphicsWinToolStripMenuItem";
			openGraphicsWinToolStripMenuItem.Size = new Size(224, 22);
			openGraphicsWinToolStripMenuItem.Text = "Reopen Graphics Window";
			openGraphicsWinToolStripMenuItem.Click += openGraphicsWinToolStripMenuItem_Click;
			// 
			// reopenVerticalViewToolStripMenuItem
			// 
			reopenVerticalViewToolStripMenuItem.Name = "reopenVerticalViewToolStripMenuItem";
			reopenVerticalViewToolStripMenuItem.Size = new Size(224, 22);
			reopenVerticalViewToolStripMenuItem.Text = "Reopen Vertical View";
			reopenVerticalViewToolStripMenuItem.Click += reopenVerticalViewToolStripMenuItem_Click;
			// 
			// setupSpritesFromFolderToolStripMenuItem
			// 
			setupSpritesFromFolderToolStripMenuItem.Name = "setupSpritesFromFolderToolStripMenuItem";
			setupSpritesFromFolderToolStripMenuItem.Size = new Size(224, 22);
			setupSpritesFromFolderToolStripMenuItem.Text = "Setup Sprites from Folder";
			setupSpritesFromFolderToolStripMenuItem.Click += setupSpritesFromFolderToolStripMenuItem_Click;
			// 
			// setupVoicelinesFromFolderToolStripMenuItem
			// 
			setupVoicelinesFromFolderToolStripMenuItem.Name = "setupVoicelinesFromFolderToolStripMenuItem";
			setupVoicelinesFromFolderToolStripMenuItem.Size = new Size(224, 22);
			setupVoicelinesFromFolderToolStripMenuItem.Text = "Setup Voicelines from Folder";
			setupVoicelinesFromFolderToolStripMenuItem.Click += setupVoicelinesFromFolderToolStripMenuItem_Click;
			// 
			// dumpVoicelinesToolStripMenuItem
			// 
			dumpVoicelinesToolStripMenuItem.Name = "dumpVoicelinesToolStripMenuItem";
			dumpVoicelinesToolStripMenuItem.Size = new Size(224, 22);
			dumpVoicelinesToolStripMenuItem.Text = "Dump Voicelines";
			dumpVoicelinesToolStripMenuItem.Click += dumpVoicelinesToolStripMenuItem_Click;
			// 
			// aboutToolStripMenuItem
			// 
			aboutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tODOToolStripMenuItem });
			aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			aboutToolStripMenuItem.Size = new Size(52, 20);
			aboutToolStripMenuItem.Text = "About";
			// 
			// tODOToolStripMenuItem
			// 
			tODOToolStripMenuItem.Name = "tODOToolStripMenuItem";
			tODOToolStripMenuItem.Size = new Size(180, 22);
			tODOToolStripMenuItem.Text = "TODO";
			// 
			// Operations
			// 
			AutoScaleMode = AutoScaleMode.None;
			ClientSize = new Size(1119, 179);
			Controls.Add(MainMenuStrip);
			Controls.Add(TextboxCurrentLanguage);
			Controls.Add(LabelFontSize);
			Controls.Add(NumericUpDownFontSize);
			Controls.Add(CheckboxPauseAutoplay);
			Controls.Add(CheckboxStartAutoplay);
			Controls.Add(CheckboxDisplayOriginalText);
			Controls.Add(LabelVoiceline);
			Controls.Add(LabelCurrentAnimation);
			Controls.Add(LabelCurrentTranslation);
			Controls.Add(LabelOriginFile);
			Controls.Add(LabelCharacterName);
			Controls.Add(LabelLineNumber);
			Controls.Add(ListBoxMenuElements);
			Controls.Add(ListBoxMenuIndex);
			Controls.Add(ButtonNextLanguage);
			Controls.Add(ButtonBackLanguage);
			Controls.Add(ButtonNextText);
			Controls.Add(ButtonBackText);
			Controls.Add(CB_Game);
			Controls.Add(LabelUnsupportedWarning);
			Controls.Add(CB_TB);
			Controls.Add(LabelTextboxStyle);
			Controls.Add(ButtonReloadText);
			Controls.Add(Textbox);
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MaximumSize = new Size(1135, 218);
			MinimumSize = new Size(1135, 218);
			Name = "Operations";
			Text = "Operations (v2.5)";
			Load += Operations_Load;
			((System.ComponentModel.ISupportInitialize)NumericUpDownFontSize).EndInit();
			MainMenuStrip.ResumeLayout(false);
			MainMenuStrip.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
		#endregion

		private System.Windows.Forms.ContextMenuStrip ListBoxRightClickCMS;
		public System.Windows.Forms.RichTextBox Textbox;
		private System.Windows.Forms.ComboBox CB_Game;
		private System.Windows.Forms.CheckBox CheckboxTranslationMode;
		private System.Windows.Forms.Label LabelUnsupportedWarning;
		private System.Windows.Forms.ComboBox CB_TB;
		private System.Windows.Forms.Label LabelTextboxStyle;
		private System.Windows.Forms.Button ButtonReloadText;
		private System.Windows.Forms.ListBox ListBoxMenuElements;
		private System.Windows.Forms.ListBox ListBoxMenuIndex;
		private System.Windows.Forms.Button ButtonNextLanguage;
		private System.Windows.Forms.Button ButtonBackLanguage;
		private System.Windows.Forms.Button ButtonNextText;
		private System.Windows.Forms.Button ButtonBackText;
		private System.Windows.Forms.Label LabelVoiceline;
		private System.Windows.Forms.Label LabelCurrentAnimation;
		private System.Windows.Forms.Label LabelCurrentTranslation;
		private System.Windows.Forms.Label LabelOriginFile;
		private System.Windows.Forms.Label LabelCharacterName;
		private System.Windows.Forms.Label LabelLineNumber;
		private System.Windows.Forms.CheckBox CheckboxPauseAutoplay;
		private System.Windows.Forms.CheckBox CheckboxStartAutoplay;
		private System.Windows.Forms.CheckBox CheckboxDisplayOriginalText;
		private System.Windows.Forms.TextBox TextboxCurrentLanguage;
		private System.Windows.Forms.Label LabelFontSize;
		private System.Windows.Forms.NumericUpDown NumericUpDownFontSize;
		private ToolTip ListBoxToolTip;
		private MenuStrip MainMenuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem settingsToolStripMenuItem;
		private ToolStripMenuItem aboutToolStripMenuItem;
		private ToolStripMenuItem openFileToolStripMenuItem;
		private ToolStripMenuItem saveFileToolStripMenuItem;
		private ToolStripMenuItem translationModeToolStripMenuItem;
		private ToolStripMenuItem replaceVariablesToolStripMenuItem;
		private ToolStripMenuItem displayCharacterToolStripMenuItem;
		private ToolStripMenuItem autoTranslationToolStripMenuItem;
		private ToolStripMenuItem editToolStripMenuItem;
		private ToolStripMenuItem fastReadToolStripMenuItem;
		private ToolStripMenuItem resetToolStripMenuItem;
		private ToolStripMenuItem useAlternateVarsToolStripMenuItem;
		private ToolStripMenuItem reloadVariablesToolStripMenuItem;
		private ToolStripMenuItem experimentalToolStripMenuItem;
		private ToolStripMenuItem sizeadjustedHeightToolStripMenuItem;
		private ToolStripMenuItem enableTTSVoicelinesToolStripMenuItem;
		private ToolStripMenuItem saveScreenshotToolStripMenuItem;
		private ToolStripMenuItem copyScreenshotToolStripMenuItem;
		private ToolStripMenuItem toolsToolStripMenuItem;
		private ToolStripMenuItem openGraphicsWinToolStripMenuItem;
		private ToolStripMenuItem reopenVerticalViewToolStripMenuItem;
		private ToolStripMenuItem setupSpritesFromFolderToolStripMenuItem;
		private ToolStripMenuItem setupVoicelinesFromFolderToolStripMenuItem;
		private ToolStripMenuItem dumpVoicelinesToolStripMenuItem;
		private ToolStripMenuItem maximizeWindowToolStripMenuItem;
		private ToolStripMenuItem tODOToolStripMenuItem;
	}
}