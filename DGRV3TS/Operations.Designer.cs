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
			CheckboxAutoTranslation = new CheckBox();
			CheckboxPlayVoiceTTS = new CheckBox();
			CheckboxMaybeAccurateHeight = new CheckBox();
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
			CheckboxUseAlternateVars = new CheckBox();
			LabelCurrentTranslation = new Label();
			LabelOriginFile = new Label();
			LabelCharacterName = new Label();
			LabelLineNumber = new Label();
			CheckboxTranslationMode = new CheckBox();
			CheckboxPauseAutoplay = new CheckBox();
			LabelOpenedFile = new Label();
			CheckboxStartAutoplay = new CheckBox();
			CheckboxDisplayCharacter = new CheckBox();
			CheckboxReplaceVariables = new CheckBox();
			CheckboxDisplayOriginalText = new CheckBox();
			TextboxCurrentLanguage = new TextBox();
			LabelFontName = new Label();
			LabelFontSize = new Label();
			NumericUpDownFontSize = new NumericUpDown();
			ButtonSaveAs = new Button();
			ButtonReloadVariables = new Button();
			ButtonFastRead = new Button();
			ButtonResetStringIndex = new Button();
			ButtonOpenFile = new Button();
			ButtonCopyImage = new Button();
			contextMenuStrip1 = new ContextMenuStrip(components);
			ReopenWindowButton = new Button();
			SetupSpritesButton = new Button();
			SetupVoicelinesButton = new Button();
			((System.ComponentModel.ISupportInitialize)NumericUpDownFontSize).BeginInit();
			SuspendLayout();
			// 
			// Textbox
			// 
			Textbox.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
			Textbox.Location = new Point(14, 14);
			Textbox.Margin = new Padding(4, 3, 4, 3);
			Textbox.Name = "Textbox";
			Textbox.Size = new Size(520, 96);
			Textbox.TabIndex = 3;
			Textbox.Text = "This is an example\\nmessage.";
			Textbox.TextChanged += Textbox_TextChanged;
			// 
			// CB_Game
			// 
			CB_Game.FormattingEnabled = true;
			CB_Game.Location = new Point(16, 123);
			CB_Game.Margin = new Padding(4, 3, 4, 3);
			CB_Game.Name = "CB_Game";
			CB_Game.Size = new Size(63, 23);
			CB_Game.TabIndex = 67;
			CB_Game.SelectedIndexChanged += CB_Game_SelectedIndexChanged;
			// 
			// CheckboxAutoTranslation
			// 
			CheckboxAutoTranslation.AutoSize = true;
			CheckboxAutoTranslation.Location = new Point(400, 155);
			CheckboxAutoTranslation.Margin = new Padding(4, 3, 4, 3);
			CheckboxAutoTranslation.Name = "CheckboxAutoTranslation";
			CheckboxAutoTranslation.Size = new Size(109, 19);
			CheckboxAutoTranslation.TabIndex = 66;
			CheckboxAutoTranslation.Text = "AutoTranslation";
			CheckboxAutoTranslation.UseVisualStyleBackColor = true;
			CheckboxAutoTranslation.CheckedChanged += CheckboxAutoTranslation_CheckedChanged;
			// 
			// CheckboxPlayVoiceTTS
			// 
			CheckboxPlayVoiceTTS.AutoSize = true;
			CheckboxPlayVoiceTTS.Location = new Point(400, 120);
			CheckboxPlayVoiceTTS.Margin = new Padding(4, 3, 4, 3);
			CheckboxPlayVoiceTTS.Name = "CheckboxPlayVoiceTTS";
			CheckboxPlayVoiceTTS.Size = new Size(108, 19);
			CheckboxPlayVoiceTTS.TabIndex = 65;
			CheckboxPlayVoiceTTS.Text = "Play Voice (TTS)";
			CheckboxPlayVoiceTTS.UseVisualStyleBackColor = true;
			// 
			// CheckboxMaybeAccurateHeight
			// 
			CheckboxMaybeAccurateHeight.AutoSize = true;
			CheckboxMaybeAccurateHeight.Location = new Point(400, 138);
			CheckboxMaybeAccurateHeight.Margin = new Padding(4, 3, 4, 3);
			CheckboxMaybeAccurateHeight.Name = "CheckboxMaybeAccurateHeight";
			CheckboxMaybeAccurateHeight.Size = new Size(133, 19);
			CheckboxMaybeAccurateHeight.TabIndex = 64;
			CheckboxMaybeAccurateHeight.Text = "Size-adjusted height";
			CheckboxMaybeAccurateHeight.UseVisualStyleBackColor = true;
			CheckboxMaybeAccurateHeight.CheckedChanged += CheckboxMaybeAccurateHeight_CheckedChanged;
			// 
			// LabelUnsupportedWarning
			// 
			LabelUnsupportedWarning.AutoSize = true;
			LabelUnsupportedWarning.ForeColor = Color.Red;
			LabelUnsupportedWarning.Location = new Point(12, 153);
			LabelUnsupportedWarning.Margin = new Padding(4, 0, 4, 0);
			LabelUnsupportedWarning.Name = "LabelUnsupportedWarning";
			LabelUnsupportedWarning.Size = new Size(286, 15);
			LabelUnsupportedWarning.TabIndex = 63;
			LabelUnsupportedWarning.Text = "Text contains one or more unsupported characters: {}";
			LabelUnsupportedWarning.Visible = false;
			LabelUnsupportedWarning.Click += LabelUnsupportedWarning_Click;
			// 
			// CB_TB
			// 
			CB_TB.DropDownStyle = ComboBoxStyle.DropDownList;
			CB_TB.FormattingEnabled = true;
			CB_TB.Location = new Point(133, 123);
			CB_TB.Margin = new Padding(4, 3, 4, 3);
			CB_TB.Name = "CB_TB";
			CB_TB.Size = new Size(173, 23);
			CB_TB.TabIndex = 62;
			CB_TB.SelectedIndexChanged += CB_TB_SelectedIndexChanged;
			// 
			// LabelTextboxStyle
			// 
			LabelTextboxStyle.AutoSize = true;
			LabelTextboxStyle.Location = new Point(88, 127);
			LabelTextboxStyle.Margin = new Padding(4, 0, 4, 0);
			LabelTextboxStyle.Name = "LabelTextboxStyle";
			LabelTextboxStyle.Size = new Size(35, 15);
			LabelTextboxStyle.TabIndex = 61;
			LabelTextboxStyle.Text = "Style:";
			LabelTextboxStyle.Click += LabelTextboxStyle_Click;
			// 
			// ButtonReloadText
			// 
			ButtonReloadText.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			ButtonReloadText.Location = new Point(314, 118);
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
			ListBoxMenuElements.Location = new Point(709, 14);
			ListBoxMenuElements.Margin = new Padding(4, 3, 4, 3);
			ListBoxMenuElements.Name = "ListBoxMenuElements";
			ListBoxMenuElements.Size = new Size(387, 79);
			ListBoxMenuElements.TabIndex = 73;
			ListBoxMenuElements.SelectedIndexChanged += ListBoxMenuElements_SelectedIndexChanged;
			ListBoxMenuElements.MouseUp += ListBoxMenuElements_RightClick;
			// 
			// ListBoxMenuIndex
			// 
			ListBoxMenuIndex.FormattingEnabled = true;
			ListBoxMenuIndex.ItemHeight = 15;
			ListBoxMenuIndex.Location = new Point(544, 14);
			ListBoxMenuIndex.Margin = new Padding(4, 3, 4, 3);
			ListBoxMenuIndex.Name = "ListBoxMenuIndex";
			ListBoxMenuIndex.Size = new Size(158, 79);
			ListBoxMenuIndex.TabIndex = 72;
			ListBoxMenuIndex.SelectedIndexChanged += ListBoxMenuIndex_SelectedIndexChanged;
			// 
			// ButtonNextLanguage
			// 
			ButtonNextLanguage.Location = new Point(636, 135);
			ButtonNextLanguage.Margin = new Padding(4, 3, 4, 3);
			ButtonNextLanguage.Name = "ButtonNextLanguage";
			ButtonNextLanguage.Size = new Size(79, 30);
			ButtonNextLanguage.TabIndex = 71;
			ButtonNextLanguage.Text = "Lang ++";
			ButtonNextLanguage.UseVisualStyleBackColor = true;
			ButtonNextLanguage.Click += ButtonNextLanguage_Click;
			// 
			// ButtonBackLanguage
			// 
			ButtonBackLanguage.Location = new Point(544, 136);
			ButtonBackLanguage.Margin = new Padding(4, 3, 4, 3);
			ButtonBackLanguage.Name = "ButtonBackLanguage";
			ButtonBackLanguage.Size = new Size(85, 30);
			ButtonBackLanguage.TabIndex = 70;
			ButtonBackLanguage.Text = "Lang --";
			ButtonBackLanguage.UseVisualStyleBackColor = true;
			ButtonBackLanguage.Click += ButtonBackLanguage_Click;
			// 
			// ButtonNextText
			// 
			ButtonNextText.Location = new Point(636, 102);
			ButtonNextText.Margin = new Padding(4, 3, 4, 3);
			ButtonNextText.Name = "ButtonNextText";
			ButtonNextText.Size = new Size(79, 31);
			ButtonNextText.TabIndex = 69;
			ButtonNextText.Text = "Line ++";
			ButtonNextText.UseVisualStyleBackColor = true;
			ButtonNextText.Click += ButtonNextText_Click;
			// 
			// ButtonBackText
			// 
			ButtonBackText.Location = new Point(544, 100);
			ButtonBackText.Margin = new Padding(4, 3, 4, 3);
			ButtonBackText.Name = "ButtonBackText";
			ButtonBackText.Size = new Size(85, 31);
			ButtonBackText.TabIndex = 68;
			ButtonBackText.Text = "Line --";
			ButtonBackText.UseVisualStyleBackColor = true;
			ButtonBackText.Click += ButtonBackText_Click;
			// 
			// LabelVoiceline
			// 
			LabelVoiceline.AutoSize = true;
			LabelVoiceline.Location = new Point(913, 121);
			LabelVoiceline.Margin = new Padding(4, 0, 4, 0);
			LabelVoiceline.Name = "LabelVoiceline";
			LabelVoiceline.Size = new Size(57, 15);
			LabelVoiceline.TabIndex = 80;
			LabelVoiceline.Text = "Voiceline:";
			// 
			// LabelCurrentAnimation
			// 
			LabelCurrentAnimation.AutoSize = true;
			LabelCurrentAnimation.Location = new Point(913, 98);
			LabelCurrentAnimation.Margin = new Padding(4, 0, 4, 0);
			LabelCurrentAnimation.Name = "LabelCurrentAnimation";
			LabelCurrentAnimation.Size = new Size(109, 15);
			LabelCurrentAnimation.TabIndex = 79;
			LabelCurrentAnimation.Text = "Current Animation:";
			LabelCurrentAnimation.Click += LabelCurrentAnimation_Click;
			// 
			// CheckboxUseAlternateVars
			// 
			CheckboxUseAlternateVars.AutoSize = true;
			CheckboxUseAlternateVars.Location = new Point(917, 143);
			CheckboxUseAlternateVars.Margin = new Padding(4, 3, 4, 3);
			CheckboxUseAlternateVars.Name = "CheckboxUseAlternateVars";
			CheckboxUseAlternateVars.Size = new Size(136, 19);
			CheckboxUseAlternateVars.TabIndex = 78;
			CheckboxUseAlternateVars.Text = "Enable Alternate Vars";
			CheckboxUseAlternateVars.UseVisualStyleBackColor = true;
			CheckboxUseAlternateVars.CheckedChanged += CheckboxUseAlternateVars_CheckedChanged;
			// 
			// LabelCurrentTranslation
			// 
			LabelCurrentTranslation.AutoSize = true;
			LabelCurrentTranslation.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			LabelCurrentTranslation.Location = new Point(722, 145);
			LabelCurrentTranslation.Margin = new Padding(4, 0, 4, 0);
			LabelCurrentTranslation.Name = "LabelCurrentTranslation";
			LabelCurrentTranslation.Size = new Size(67, 13);
			LabelCurrentTranslation.TabIndex = 77;
			LabelCurrentTranslation.Text = "Translations:";
			LabelCurrentTranslation.Click += LabelCurrentTranslation_Click;
			// 
			// LabelOriginFile
			// 
			LabelOriginFile.AutoSize = true;
			LabelOriginFile.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			LabelOriginFile.Location = new Point(722, 130);
			LabelOriginFile.Margin = new Padding(4, 0, 4, 0);
			LabelOriginFile.Name = "LabelOriginFile";
			LabelOriginFile.Size = new Size(53, 13);
			LabelOriginFile.TabIndex = 76;
			LabelOriginFile.Text = "Origin file:";
			LabelOriginFile.Click += LabelOriginFile_Click;
			// 
			// LabelCharacterName
			// 
			LabelCharacterName.AutoSize = true;
			LabelCharacterName.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			LabelCharacterName.Location = new Point(722, 114);
			LabelCharacterName.Margin = new Padding(4, 0, 4, 0);
			LabelCharacterName.Name = "LabelCharacterName";
			LabelCharacterName.Size = new Size(56, 13);
			LabelCharacterName.TabIndex = 75;
			LabelCharacterName.Text = "Character:";
			LabelCharacterName.Click += LabelCharacterName_Click;
			// 
			// LabelLineNumber
			// 
			LabelLineNumber.AutoSize = true;
			LabelLineNumber.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			LabelLineNumber.Location = new Point(722, 98);
			LabelLineNumber.Margin = new Padding(4, 0, 4, 0);
			LabelLineNumber.Name = "LabelLineNumber";
			LabelLineNumber.Size = new Size(30, 13);
			LabelLineNumber.TabIndex = 74;
			LabelLineNumber.Text = "Line:";
			LabelLineNumber.Click += LabelLineNumber_Click;
			// 
			// CheckboxTranslationMode
			// 
			CheckboxTranslationMode.AutoSize = true;
			CheckboxTranslationMode.Checked = true;
			CheckboxTranslationMode.CheckState = CheckState.Checked;
			CheckboxTranslationMode.Location = new Point(1104, 25);
			CheckboxTranslationMode.Margin = new Padding(4, 3, 4, 3);
			CheckboxTranslationMode.Name = "CheckboxTranslationMode";
			CheckboxTranslationMode.Size = new Size(117, 19);
			CheckboxTranslationMode.TabIndex = 87;
			CheckboxTranslationMode.Text = "Translation Mode";
			CheckboxTranslationMode.UseVisualStyleBackColor = true;
			CheckboxTranslationMode.CheckedChanged += CheckboxTranslationMode_CheckedChanged;
			// 
			// CheckboxPauseAutoplay
			// 
			CheckboxPauseAutoplay.AutoSize = true;
			CheckboxPauseAutoplay.Location = new Point(1105, 142);
			CheckboxPauseAutoplay.Margin = new Padding(4, 3, 4, 3);
			CheckboxPauseAutoplay.Name = "CheckboxPauseAutoplay";
			CheckboxPauseAutoplay.Size = new Size(108, 19);
			CheckboxPauseAutoplay.TabIndex = 86;
			CheckboxPauseAutoplay.Text = "Pause Autoplay";
			CheckboxPauseAutoplay.UseVisualStyleBackColor = true;
			CheckboxPauseAutoplay.CheckedChanged += CheckboxPauseAutoplay_CheckedChanged;
			// 
			// LabelOpenedFile
			// 
			LabelOpenedFile.AutoSize = true;
			LabelOpenedFile.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			LabelOpenedFile.Location = new Point(1101, 7);
			LabelOpenedFile.Margin = new Padding(4, 0, 4, 0);
			LabelOpenedFile.Name = "LabelOpenedFile";
			LabelOpenedFile.Size = new Size(60, 13);
			LabelOpenedFile.TabIndex = 85;
			LabelOpenedFile.Text = "Current file:";
			// 
			// CheckboxStartAutoplay
			// 
			CheckboxStartAutoplay.AutoSize = true;
			CheckboxStartAutoplay.Location = new Point(1105, 119);
			CheckboxStartAutoplay.Margin = new Padding(4, 3, 4, 3);
			CheckboxStartAutoplay.Name = "CheckboxStartAutoplay";
			CheckboxStartAutoplay.Size = new Size(148, 19);
			CheckboxStartAutoplay.TabIndex = 84;
			CheckboxStartAutoplay.Text = "Start/Resume AutoPlay";
			CheckboxStartAutoplay.UseVisualStyleBackColor = true;
			CheckboxStartAutoplay.CheckedChanged += CheckboxStartAutoplay_CheckedChanged;
			// 
			// CheckboxDisplayCharacter
			// 
			CheckboxDisplayCharacter.AutoSize = true;
			CheckboxDisplayCharacter.Checked = true;
			CheckboxDisplayCharacter.CheckState = CheckState.Checked;
			CheckboxDisplayCharacter.Location = new Point(1104, 97);
			CheckboxDisplayCharacter.Margin = new Padding(4, 3, 4, 3);
			CheckboxDisplayCharacter.Name = "CheckboxDisplayCharacter";
			CheckboxDisplayCharacter.Size = new Size(118, 19);
			CheckboxDisplayCharacter.TabIndex = 83;
			CheckboxDisplayCharacter.Text = "Display Character";
			CheckboxDisplayCharacter.UseVisualStyleBackColor = true;
			CheckboxDisplayCharacter.CheckedChanged += CheckboxDisplayCharacter_CheckedChanged;
			// 
			// CheckboxReplaceVariables
			// 
			CheckboxReplaceVariables.AutoSize = true;
			CheckboxReplaceVariables.Checked = true;
			CheckboxReplaceVariables.CheckState = CheckState.Checked;
			CheckboxReplaceVariables.Location = new Point(1104, 75);
			CheckboxReplaceVariables.Margin = new Padding(4, 3, 4, 3);
			CheckboxReplaceVariables.Name = "CheckboxReplaceVariables";
			CheckboxReplaceVariables.Size = new Size(116, 19);
			CheckboxReplaceVariables.TabIndex = 82;
			CheckboxReplaceVariables.Text = "Replace Variables";
			CheckboxReplaceVariables.UseVisualStyleBackColor = true;
			CheckboxReplaceVariables.CheckedChanged += CheckboxReplaceVariables_CheckedChanged;
			// 
			// CheckboxDisplayOriginalText
			// 
			CheckboxDisplayOriginalText.AutoSize = true;
			CheckboxDisplayOriginalText.Location = new Point(1104, 52);
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
			TextboxCurrentLanguage.Location = new Point(1402, 90);
			TextboxCurrentLanguage.Margin = new Padding(4, 3, 4, 3);
			TextboxCurrentLanguage.Name = "TextboxCurrentLanguage";
			TextboxCurrentLanguage.Size = new Size(94, 23);
			TextboxCurrentLanguage.TabIndex = 97;
			// 
			// LabelFontName
			// 
			LabelFontName.AutoSize = true;
			LabelFontName.Location = new Point(1399, 119);
			LabelFontName.Margin = new Padding(4, 0, 4, 0);
			LabelFontName.Name = "LabelFontName";
			LabelFontName.Size = new Size(63, 15);
			LabelFontName.TabIndex = 96;
			LabelFontName.Text = "FontName";
			LabelFontName.Click += LabelFontName_Click;
			// 
			// LabelFontSize
			// 
			LabelFontSize.AutoSize = true;
			LabelFontSize.Location = new Point(1399, 141);
			LabelFontSize.Margin = new Padding(4, 0, 4, 0);
			LabelFontSize.Name = "LabelFontSize";
			LabelFontSize.Size = new Size(30, 15);
			LabelFontSize.TabIndex = 95;
			LabelFontSize.Text = "Size:";
			LabelFontSize.Click += LabelFontSize_Click;
			// 
			// NumericUpDownFontSize
			// 
			NumericUpDownFontSize.Location = new Point(1441, 137);
			NumericUpDownFontSize.Margin = new Padding(4, 3, 4, 3);
			NumericUpDownFontSize.Name = "NumericUpDownFontSize";
			NumericUpDownFontSize.Size = new Size(64, 23);
			NumericUpDownFontSize.TabIndex = 94;
			NumericUpDownFontSize.ValueChanged += NumericUpDownFontSize_ValueChanged;
			// 
			// ButtonSaveAs
			// 
			ButtonSaveAs.Location = new Point(1402, 8);
			ButtonSaveAs.Margin = new Padding(4, 3, 4, 3);
			ButtonSaveAs.Name = "ButtonSaveAs";
			ButtonSaveAs.Size = new Size(103, 35);
			ButtonSaveAs.TabIndex = 93;
			ButtonSaveAs.Text = "Save File As";
			ButtonSaveAs.UseVisualStyleBackColor = true;
			ButtonSaveAs.Click += ButtonSaveAs_Click;
			// 
			// ButtonReloadVariables
			// 
			ButtonReloadVariables.Location = new Point(1278, 120);
			ButtonReloadVariables.Margin = new Padding(4, 3, 4, 3);
			ButtonReloadVariables.Name = "ButtonReloadVariables";
			ButtonReloadVariables.Size = new Size(118, 35);
			ButtonReloadVariables.TabIndex = 92;
			ButtonReloadVariables.Text = "Reload Vars";
			ButtonReloadVariables.UseVisualStyleBackColor = true;
			ButtonReloadVariables.Click += ButtonReloadVariables_Click;
			// 
			// ButtonFastRead
			// 
			ButtonFastRead.Location = new Point(1278, 84);
			ButtonFastRead.Margin = new Padding(4, 3, 4, 3);
			ButtonFastRead.Name = "ButtonFastRead";
			ButtonFastRead.Size = new Size(118, 35);
			ButtonFastRead.TabIndex = 91;
			ButtonFastRead.Text = "Fast-Read";
			ButtonFastRead.UseVisualStyleBackColor = true;
			ButtonFastRead.Click += ButtonFastRead_Click;
			// 
			// ButtonResetStringIndex
			// 
			ButtonResetStringIndex.Location = new Point(1278, 46);
			ButtonResetStringIndex.Margin = new Padding(4, 3, 4, 3);
			ButtonResetStringIndex.Name = "ButtonResetStringIndex";
			ButtonResetStringIndex.Size = new Size(118, 35);
			ButtonResetStringIndex.TabIndex = 90;
			ButtonResetStringIndex.Text = "Reset";
			ButtonResetStringIndex.UseVisualStyleBackColor = true;
			ButtonResetStringIndex.Click += ButtonResetStringIndex_Click;
			// 
			// ButtonOpenFile
			// 
			ButtonOpenFile.Location = new Point(1278, 7);
			ButtonOpenFile.Margin = new Padding(4, 3, 4, 3);
			ButtonOpenFile.Name = "ButtonOpenFile";
			ButtonOpenFile.Size = new Size(118, 36);
			ButtonOpenFile.TabIndex = 89;
			ButtonOpenFile.Text = "Open File/Font";
			ButtonOpenFile.UseVisualStyleBackColor = true;
			ButtonOpenFile.Click += ButtonOpenFile_Click;
			// 
			// ButtonCopyImage
			// 
			ButtonCopyImage.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
			ButtonCopyImage.Location = new Point(1402, 46);
			ButtonCopyImage.Margin = new Padding(4, 3, 4, 3);
			ButtonCopyImage.Name = "ButtonCopyImage";
			ButtonCopyImage.Size = new Size(103, 35);
			ButtonCopyImage.TabIndex = 88;
			ButtonCopyImage.Text = "Copy image";
			ButtonCopyImage.UseVisualStyleBackColor = true;
			ButtonCopyImage.Click += ButtonCopyImage_Click;
			// 
			// contextMenuStrip1
			// 
			contextMenuStrip1.Name = "contextMenuStrip1";
			contextMenuStrip1.Size = new Size(61, 4);
			// 
			// ReopenWindowButton
			// 
			ReopenWindowButton.Location = new Point(1512, 8);
			ReopenWindowButton.Margin = new Padding(4, 3, 4, 3);
			ReopenWindowButton.Name = "ReopenWindowButton";
			ReopenWindowButton.Size = new Size(31, 35);
			ReopenWindowButton.TabIndex = 98;
			ReopenWindowButton.Text = "[  ]";
			ReopenWindowButton.UseVisualStyleBackColor = true;
			ReopenWindowButton.Click += ReopenWindowButton_Click;
			// 
			// SetupSpritesButton
			// 
			SetupSpritesButton.Location = new Point(1512, 49);
			SetupSpritesButton.Name = "SetupSpritesButton";
			SetupSpritesButton.Size = new Size(31, 32);
			SetupSpritesButton.TabIndex = 99;
			SetupSpritesButton.Text = "[i]";
			SetupSpritesButton.UseVisualStyleBackColor = true;
			SetupSpritesButton.Click += SetupSpritesButton_Click;
			// 
			// SetupVoicelinesButton
			// 
			SetupVoicelinesButton.Location = new Point(1512, 84);
			SetupVoicelinesButton.Name = "SetupVoicelinesButton";
			SetupVoicelinesButton.Size = new Size(31, 29);
			SetupVoicelinesButton.TabIndex = 100;
			SetupVoicelinesButton.Text = "[<]";
			SetupVoicelinesButton.UseVisualStyleBackColor = true;
			SetupVoicelinesButton.Click += SetupVoicelinesButton_Click;
			// 
			// Operations
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1546, 179);
			Controls.Add(SetupVoicelinesButton);
			Controls.Add(SetupSpritesButton);
			Controls.Add(ReopenWindowButton);
			Controls.Add(TextboxCurrentLanguage);
			Controls.Add(LabelFontName);
			Controls.Add(LabelFontSize);
			Controls.Add(NumericUpDownFontSize);
			Controls.Add(ButtonSaveAs);
			Controls.Add(ButtonReloadVariables);
			Controls.Add(ButtonFastRead);
			Controls.Add(ButtonResetStringIndex);
			Controls.Add(ButtonOpenFile);
			Controls.Add(ButtonCopyImage);
			Controls.Add(CheckboxTranslationMode);
			Controls.Add(CheckboxPauseAutoplay);
			Controls.Add(LabelOpenedFile);
			Controls.Add(CheckboxStartAutoplay);
			Controls.Add(CheckboxDisplayCharacter);
			Controls.Add(CheckboxReplaceVariables);
			Controls.Add(CheckboxDisplayOriginalText);
			Controls.Add(LabelVoiceline);
			Controls.Add(LabelCurrentAnimation);
			Controls.Add(CheckboxUseAlternateVars);
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
			Controls.Add(CheckboxAutoTranslation);
			Controls.Add(CheckboxPlayVoiceTTS);
			Controls.Add(CheckboxMaybeAccurateHeight);
			Controls.Add(LabelUnsupportedWarning);
			Controls.Add(CB_TB);
			Controls.Add(LabelTextboxStyle);
			Controls.Add(ButtonReloadText);
			Controls.Add(Textbox);
			Margin = new Padding(4, 3, 4, 3);
			MaximumSize = new Size(1562, 218);
			MinimumSize = new Size(1562, 218);
			Name = "Operations";
			Text = "Operations";
			((System.ComponentModel.ISupportInitialize)NumericUpDownFontSize).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.RichTextBox Textbox;
		private System.Windows.Forms.ComboBox CB_Game;
		private System.Windows.Forms.CheckBox CheckboxAutoTranslation;
		private System.Windows.Forms.CheckBox CheckboxPlayVoiceTTS;
		private System.Windows.Forms.CheckBox CheckboxMaybeAccurateHeight;
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
		private System.Windows.Forms.CheckBox CheckboxUseAlternateVars;
		private System.Windows.Forms.Label LabelCurrentTranslation;
		private System.Windows.Forms.Label LabelOriginFile;
		private System.Windows.Forms.Label LabelCharacterName;
		private System.Windows.Forms.Label LabelLineNumber;
		private System.Windows.Forms.CheckBox CheckboxTranslationMode;
		private System.Windows.Forms.CheckBox CheckboxPauseAutoplay;
		private System.Windows.Forms.Label LabelOpenedFile;
		private System.Windows.Forms.CheckBox CheckboxStartAutoplay;
		private System.Windows.Forms.CheckBox CheckboxDisplayCharacter;
		private System.Windows.Forms.CheckBox CheckboxReplaceVariables;
		private System.Windows.Forms.CheckBox CheckboxDisplayOriginalText;
		private System.Windows.Forms.TextBox TextboxCurrentLanguage;
		private System.Windows.Forms.Label LabelFontName;
		private System.Windows.Forms.Label LabelFontSize;
		private System.Windows.Forms.NumericUpDown NumericUpDownFontSize;
		private System.Windows.Forms.Button ButtonSaveAs;
		private System.Windows.Forms.Button ButtonReloadVariables;
		private System.Windows.Forms.Button ButtonFastRead;
		private System.Windows.Forms.Button ButtonResetStringIndex;
		private System.Windows.Forms.Button ButtonOpenFile;
		private System.Windows.Forms.Button ButtonCopyImage;
		private System.Windows.Forms.Button ReopenWindowButton;
		private Button SetupSpritesButton;
		private Button SetupVoicelinesButton;
	}
}