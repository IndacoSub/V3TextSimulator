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
			this.components = new System.ComponentModel.Container();
			this.Textbox = new System.Windows.Forms.RichTextBox();
			this.CB_Game = new System.Windows.Forms.ComboBox();
			this.CheckboxAutoTranslation = new System.Windows.Forms.CheckBox();
			this.CheckboxPlayVoiceTTS = new System.Windows.Forms.CheckBox();
			this.CheckboxMaybeAccurateHeight = new System.Windows.Forms.CheckBox();
			this.LabelUnsupportedWarning = new System.Windows.Forms.Label();
			this.CB_TB = new System.Windows.Forms.ComboBox();
			this.LabelTextboxStyle = new System.Windows.Forms.Label();
			this.ButtonReloadText = new System.Windows.Forms.Button();
			this.ListBoxMenuElements = new System.Windows.Forms.ListBox();
			this.ListBoxMenuIndex = new System.Windows.Forms.ListBox();
			this.ButtonNextLanguage = new System.Windows.Forms.Button();
			this.ButtonBackLanguage = new System.Windows.Forms.Button();
			this.ButtonNextText = new System.Windows.Forms.Button();
			this.ButtonBackText = new System.Windows.Forms.Button();
			this.LabelVoiceline = new System.Windows.Forms.Label();
			this.LabelCurrentAnimation = new System.Windows.Forms.Label();
			this.CheckboxUseAlternateVars = new System.Windows.Forms.CheckBox();
			this.LabelCurrentTranslation = new System.Windows.Forms.Label();
			this.LabelOriginFile = new System.Windows.Forms.Label();
			this.LabelCharacterName = new System.Windows.Forms.Label();
			this.LabelLineNumber = new System.Windows.Forms.Label();
			this.CheckboxTranslationMode = new System.Windows.Forms.CheckBox();
			this.CheckboxPauseAutoplay = new System.Windows.Forms.CheckBox();
			this.LabelOpenedFile = new System.Windows.Forms.Label();
			this.CheckboxStartAutoplay = new System.Windows.Forms.CheckBox();
			this.CheckboxDisplayCharacter = new System.Windows.Forms.CheckBox();
			this.CheckboxReplaceVariables = new System.Windows.Forms.CheckBox();
			this.CheckboxDisplayOriginalText = new System.Windows.Forms.CheckBox();
			this.TextboxCurrentLanguage = new System.Windows.Forms.TextBox();
			this.LabelFontName = new System.Windows.Forms.Label();
			this.LabelFontSize = new System.Windows.Forms.Label();
			this.NumericUpDownFontSize = new System.Windows.Forms.NumericUpDown();
			this.ButtonSaveAs = new System.Windows.Forms.Button();
			this.ButtonReloadVariables = new System.Windows.Forms.Button();
			this.ButtonFastRead = new System.Windows.Forms.Button();
			this.ButtonResetStringIndex = new System.Windows.Forms.Button();
			this.ButtonOpenFile = new System.Windows.Forms.Button();
			this.ButtonCopyImage = new System.Windows.Forms.Button();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ReopenWindowButton = new System.Windows.Forms.Button();
			this.SetupSpritesButton = new System.Windows.Forms.Button();
			this.SetupVoicelinesButton = new System.Windows.Forms.Button();
			this.DumpSelectedVoicelinesButton = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.OpenVerticalViewButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownFontSize)).BeginInit();
			this.SuspendLayout();
			// 
			// Textbox
			// 
			this.Textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.Textbox.Location = new System.Drawing.Point(14, 14);
			this.Textbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Textbox.Name = "Textbox";
			this.Textbox.Size = new System.Drawing.Size(520, 96);
			this.Textbox.TabIndex = 3;
			this.Textbox.Text = "This is an example\\nmessage.";
			this.Textbox.TextChanged += new System.EventHandler(this.Textbox_TextChanged);
			// 
			// CB_Game
			// 
			this.CB_Game.FormattingEnabled = true;
			this.CB_Game.Location = new System.Drawing.Point(16, 123);
			this.CB_Game.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CB_Game.Name = "CB_Game";
			this.CB_Game.Size = new System.Drawing.Size(63, 23);
			this.CB_Game.TabIndex = 67;
			this.CB_Game.SelectedIndexChanged += new System.EventHandler(this.CB_Game_SelectedIndexChanged);
			// 
			// CheckboxAutoTranslation
			// 
			this.CheckboxAutoTranslation.AutoSize = true;
			this.CheckboxAutoTranslation.Location = new System.Drawing.Point(400, 155);
			this.CheckboxAutoTranslation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CheckboxAutoTranslation.Name = "CheckboxAutoTranslation";
			this.CheckboxAutoTranslation.Size = new System.Drawing.Size(109, 19);
			this.CheckboxAutoTranslation.TabIndex = 66;
			this.CheckboxAutoTranslation.Text = "AutoTranslation";
			this.CheckboxAutoTranslation.UseVisualStyleBackColor = true;
			this.CheckboxAutoTranslation.CheckedChanged += new System.EventHandler(this.CheckboxAutoTranslation_CheckedChanged);
			// 
			// CheckboxPlayVoiceTTS
			// 
			this.CheckboxPlayVoiceTTS.AutoSize = true;
			this.CheckboxPlayVoiceTTS.Location = new System.Drawing.Point(400, 120);
			this.CheckboxPlayVoiceTTS.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CheckboxPlayVoiceTTS.Name = "CheckboxPlayVoiceTTS";
			this.CheckboxPlayVoiceTTS.Size = new System.Drawing.Size(108, 19);
			this.CheckboxPlayVoiceTTS.TabIndex = 65;
			this.CheckboxPlayVoiceTTS.Text = "Play Voice (TTS)";
			this.CheckboxPlayVoiceTTS.UseVisualStyleBackColor = true;
			// 
			// CheckboxMaybeAccurateHeight
			// 
			this.CheckboxMaybeAccurateHeight.AutoSize = true;
			this.CheckboxMaybeAccurateHeight.Location = new System.Drawing.Point(400, 138);
			this.CheckboxMaybeAccurateHeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CheckboxMaybeAccurateHeight.Name = "CheckboxMaybeAccurateHeight";
			this.CheckboxMaybeAccurateHeight.Size = new System.Drawing.Size(133, 19);
			this.CheckboxMaybeAccurateHeight.TabIndex = 64;
			this.CheckboxMaybeAccurateHeight.Text = "Size-adjusted height";
			this.CheckboxMaybeAccurateHeight.UseVisualStyleBackColor = true;
			this.CheckboxMaybeAccurateHeight.CheckedChanged += new System.EventHandler(this.CheckboxMaybeAccurateHeight_CheckedChanged);
			// 
			// LabelUnsupportedWarning
			// 
			this.LabelUnsupportedWarning.AutoSize = true;
			this.LabelUnsupportedWarning.ForeColor = System.Drawing.Color.Red;
			this.LabelUnsupportedWarning.Location = new System.Drawing.Point(12, 153);
			this.LabelUnsupportedWarning.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelUnsupportedWarning.Name = "LabelUnsupportedWarning";
			this.LabelUnsupportedWarning.Size = new System.Drawing.Size(286, 15);
			this.LabelUnsupportedWarning.TabIndex = 63;
			this.LabelUnsupportedWarning.Text = "Text contains one or more unsupported characters: {}";
			this.LabelUnsupportedWarning.Visible = false;
			// 
			// CB_TB
			// 
			this.CB_TB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CB_TB.FormattingEnabled = true;
			this.CB_TB.Location = new System.Drawing.Point(133, 123);
			this.CB_TB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CB_TB.Name = "CB_TB";
			this.CB_TB.Size = new System.Drawing.Size(173, 23);
			this.CB_TB.TabIndex = 62;
			this.CB_TB.SelectedIndexChanged += new System.EventHandler(this.CB_TB_SelectedIndexChanged);
			// 
			// LabelTextboxStyle
			// 
			this.LabelTextboxStyle.AutoSize = true;
			this.LabelTextboxStyle.Location = new System.Drawing.Point(88, 127);
			this.LabelTextboxStyle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelTextboxStyle.Name = "LabelTextboxStyle";
			this.LabelTextboxStyle.Size = new System.Drawing.Size(35, 15);
			this.LabelTextboxStyle.TabIndex = 61;
			this.LabelTextboxStyle.Text = "Style:";
			// 
			// ButtonReloadText
			// 
			this.ButtonReloadText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.ButtonReloadText.Location = new System.Drawing.Point(314, 118);
			this.ButtonReloadText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonReloadText.Name = "ButtonReloadText";
			this.ButtonReloadText.Size = new System.Drawing.Size(79, 32);
			this.ButtonReloadText.TabIndex = 60;
			this.ButtonReloadText.Text = "Refresh";
			this.ButtonReloadText.UseVisualStyleBackColor = true;
			this.ButtonReloadText.Click += new System.EventHandler(this.ButtonReloadText_Click);
			// 
			// ListBoxMenuElements
			// 
			this.ListBoxMenuElements.FormattingEnabled = true;
			this.ListBoxMenuElements.ItemHeight = 15;
			this.ListBoxMenuElements.Location = new System.Drawing.Point(709, 14);
			this.ListBoxMenuElements.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ListBoxMenuElements.Name = "ListBoxMenuElements";
			this.ListBoxMenuElements.Size = new System.Drawing.Size(387, 79);
			this.ListBoxMenuElements.TabIndex = 73;
			this.ListBoxMenuElements.SelectedIndexChanged += new System.EventHandler(this.ListBoxMenuElements_SelectedIndexChanged);
			this.ListBoxMenuElements.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListBoxMenuElements_RightClick);
			this.ListBoxMenuElements.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListBoxOnMouseMove);
			// 
			// ListBoxMenuIndex
			// 
			this.ListBoxMenuIndex.FormattingEnabled = true;
			this.ListBoxMenuIndex.ItemHeight = 15;
			this.ListBoxMenuIndex.Location = new System.Drawing.Point(544, 14);
			this.ListBoxMenuIndex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ListBoxMenuIndex.Name = "ListBoxMenuIndex";
			this.ListBoxMenuIndex.Size = new System.Drawing.Size(158, 79);
			this.ListBoxMenuIndex.TabIndex = 72;
			this.ListBoxMenuIndex.SelectedIndexChanged += new System.EventHandler(this.ListBoxMenuIndex_SelectedIndexChanged);
			// 
			// ButtonNextLanguage
			// 
			this.ButtonNextLanguage.Location = new System.Drawing.Point(636, 135);
			this.ButtonNextLanguage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonNextLanguage.Name = "ButtonNextLanguage";
			this.ButtonNextLanguage.Size = new System.Drawing.Size(79, 30);
			this.ButtonNextLanguage.TabIndex = 71;
			this.ButtonNextLanguage.Text = "Lang ++";
			this.ButtonNextLanguage.UseVisualStyleBackColor = true;
			this.ButtonNextLanguage.Click += new System.EventHandler(this.ButtonNextLanguage_Click);
			// 
			// ButtonBackLanguage
			// 
			this.ButtonBackLanguage.Location = new System.Drawing.Point(544, 136);
			this.ButtonBackLanguage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonBackLanguage.Name = "ButtonBackLanguage";
			this.ButtonBackLanguage.Size = new System.Drawing.Size(85, 30);
			this.ButtonBackLanguage.TabIndex = 70;
			this.ButtonBackLanguage.Text = "Lang --";
			this.ButtonBackLanguage.UseVisualStyleBackColor = true;
			this.ButtonBackLanguage.Click += new System.EventHandler(this.ButtonBackLanguage_Click);
			// 
			// ButtonNextText
			// 
			this.ButtonNextText.Location = new System.Drawing.Point(636, 102);
			this.ButtonNextText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonNextText.Name = "ButtonNextText";
			this.ButtonNextText.Size = new System.Drawing.Size(79, 31);
			this.ButtonNextText.TabIndex = 69;
			this.ButtonNextText.Text = "Line ++";
			this.ButtonNextText.UseVisualStyleBackColor = true;
			this.ButtonNextText.Click += new System.EventHandler(this.ButtonNextText_Click);
			// 
			// ButtonBackText
			// 
			this.ButtonBackText.Location = new System.Drawing.Point(544, 100);
			this.ButtonBackText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonBackText.Name = "ButtonBackText";
			this.ButtonBackText.Size = new System.Drawing.Size(85, 31);
			this.ButtonBackText.TabIndex = 68;
			this.ButtonBackText.Text = "Line --";
			this.ButtonBackText.UseVisualStyleBackColor = true;
			this.ButtonBackText.Click += new System.EventHandler(this.ButtonBackText_Click);
			// 
			// LabelVoiceline
			// 
			this.LabelVoiceline.AutoSize = true;
			this.LabelVoiceline.Location = new System.Drawing.Point(913, 121);
			this.LabelVoiceline.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelVoiceline.Name = "LabelVoiceline";
			this.LabelVoiceline.Size = new System.Drawing.Size(57, 15);
			this.LabelVoiceline.TabIndex = 80;
			this.LabelVoiceline.Text = "Voiceline:";
			// 
			// LabelCurrentAnimation
			// 
			this.LabelCurrentAnimation.AutoSize = true;
			this.LabelCurrentAnimation.Location = new System.Drawing.Point(913, 98);
			this.LabelCurrentAnimation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelCurrentAnimation.Name = "LabelCurrentAnimation";
			this.LabelCurrentAnimation.Size = new System.Drawing.Size(109, 15);
			this.LabelCurrentAnimation.TabIndex = 79;
			this.LabelCurrentAnimation.Text = "Current Animation:";
			// 
			// CheckboxUseAlternateVars
			// 
			this.CheckboxUseAlternateVars.AutoSize = true;
			this.CheckboxUseAlternateVars.Location = new System.Drawing.Point(917, 143);
			this.CheckboxUseAlternateVars.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CheckboxUseAlternateVars.Name = "CheckboxUseAlternateVars";
			this.CheckboxUseAlternateVars.Size = new System.Drawing.Size(136, 19);
			this.CheckboxUseAlternateVars.TabIndex = 78;
			this.CheckboxUseAlternateVars.Text = "Enable Alternate Vars";
			this.CheckboxUseAlternateVars.UseVisualStyleBackColor = true;
			this.CheckboxUseAlternateVars.CheckedChanged += new System.EventHandler(this.CheckboxUseAlternateVars_CheckedChanged);
			// 
			// LabelCurrentTranslation
			// 
			this.LabelCurrentTranslation.AutoSize = true;
			this.LabelCurrentTranslation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.LabelCurrentTranslation.Location = new System.Drawing.Point(722, 145);
			this.LabelCurrentTranslation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelCurrentTranslation.Name = "LabelCurrentTranslation";
			this.LabelCurrentTranslation.Size = new System.Drawing.Size(67, 13);
			this.LabelCurrentTranslation.TabIndex = 77;
			this.LabelCurrentTranslation.Text = "Translations:";
			// 
			// LabelOriginFile
			// 
			this.LabelOriginFile.AutoSize = true;
			this.LabelOriginFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.LabelOriginFile.Location = new System.Drawing.Point(722, 130);
			this.LabelOriginFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelOriginFile.Name = "LabelOriginFile";
			this.LabelOriginFile.Size = new System.Drawing.Size(53, 13);
			this.LabelOriginFile.TabIndex = 76;
			this.LabelOriginFile.Text = "Origin file:";
			// 
			// LabelCharacterName
			// 
			this.LabelCharacterName.AutoSize = true;
			this.LabelCharacterName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.LabelCharacterName.Location = new System.Drawing.Point(722, 114);
			this.LabelCharacterName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelCharacterName.Name = "LabelCharacterName";
			this.LabelCharacterName.Size = new System.Drawing.Size(56, 13);
			this.LabelCharacterName.TabIndex = 75;
			this.LabelCharacterName.Text = "Character:";
			// 
			// LabelLineNumber
			// 
			this.LabelLineNumber.AutoSize = true;
			this.LabelLineNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.LabelLineNumber.Location = new System.Drawing.Point(722, 98);
			this.LabelLineNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelLineNumber.Name = "LabelLineNumber";
			this.LabelLineNumber.Size = new System.Drawing.Size(30, 13);
			this.LabelLineNumber.TabIndex = 74;
			this.LabelLineNumber.Text = "Line:";
			// 
			// CheckboxTranslationMode
			// 
			this.CheckboxTranslationMode.AutoSize = true;
			this.CheckboxTranslationMode.Checked = true;
			this.CheckboxTranslationMode.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckboxTranslationMode.Location = new System.Drawing.Point(1104, 25);
			this.CheckboxTranslationMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CheckboxTranslationMode.Name = "CheckboxTranslationMode";
			this.CheckboxTranslationMode.Size = new System.Drawing.Size(117, 19);
			this.CheckboxTranslationMode.TabIndex = 87;
			this.CheckboxTranslationMode.Text = "Translation Mode";
			this.CheckboxTranslationMode.UseVisualStyleBackColor = true;
			this.CheckboxTranslationMode.CheckedChanged += new System.EventHandler(this.CheckboxTranslationMode_CheckedChanged);
			// 
			// CheckboxPauseAutoplay
			// 
			this.CheckboxPauseAutoplay.AutoSize = true;
			this.CheckboxPauseAutoplay.Location = new System.Drawing.Point(1105, 142);
			this.CheckboxPauseAutoplay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CheckboxPauseAutoplay.Name = "CheckboxPauseAutoplay";
			this.CheckboxPauseAutoplay.Size = new System.Drawing.Size(108, 19);
			this.CheckboxPauseAutoplay.TabIndex = 86;
			this.CheckboxPauseAutoplay.Text = "Pause Autoplay";
			this.CheckboxPauseAutoplay.UseVisualStyleBackColor = true;
			this.CheckboxPauseAutoplay.CheckedChanged += new System.EventHandler(this.CheckboxPauseAutoplay_CheckedChanged);
			// 
			// LabelOpenedFile
			// 
			this.LabelOpenedFile.AutoSize = true;
			this.LabelOpenedFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.LabelOpenedFile.Location = new System.Drawing.Point(1101, 7);
			this.LabelOpenedFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelOpenedFile.Name = "LabelOpenedFile";
			this.LabelOpenedFile.Size = new System.Drawing.Size(60, 13);
			this.LabelOpenedFile.TabIndex = 85;
			this.LabelOpenedFile.Text = "Current file:";
			// 
			// CheckboxStartAutoplay
			// 
			this.CheckboxStartAutoplay.AutoSize = true;
			this.CheckboxStartAutoplay.Location = new System.Drawing.Point(1105, 119);
			this.CheckboxStartAutoplay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CheckboxStartAutoplay.Name = "CheckboxStartAutoplay";
			this.CheckboxStartAutoplay.Size = new System.Drawing.Size(148, 19);
			this.CheckboxStartAutoplay.TabIndex = 84;
			this.CheckboxStartAutoplay.Text = "Start/Resume AutoPlay";
			this.CheckboxStartAutoplay.UseVisualStyleBackColor = true;
			this.CheckboxStartAutoplay.CheckedChanged += new System.EventHandler(this.CheckboxStartAutoplay_CheckedChanged);
			// 
			// CheckboxDisplayCharacter
			// 
			this.CheckboxDisplayCharacter.AutoSize = true;
			this.CheckboxDisplayCharacter.Checked = true;
			this.CheckboxDisplayCharacter.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckboxDisplayCharacter.Location = new System.Drawing.Point(1104, 97);
			this.CheckboxDisplayCharacter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CheckboxDisplayCharacter.Name = "CheckboxDisplayCharacter";
			this.CheckboxDisplayCharacter.Size = new System.Drawing.Size(118, 19);
			this.CheckboxDisplayCharacter.TabIndex = 83;
			this.CheckboxDisplayCharacter.Text = "Display Character";
			this.CheckboxDisplayCharacter.UseVisualStyleBackColor = true;
			this.CheckboxDisplayCharacter.CheckedChanged += new System.EventHandler(this.CheckboxDisplayCharacter_CheckedChanged);
			// 
			// CheckboxReplaceVariables
			// 
			this.CheckboxReplaceVariables.AutoSize = true;
			this.CheckboxReplaceVariables.Checked = true;
			this.CheckboxReplaceVariables.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckboxReplaceVariables.Location = new System.Drawing.Point(1104, 75);
			this.CheckboxReplaceVariables.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CheckboxReplaceVariables.Name = "CheckboxReplaceVariables";
			this.CheckboxReplaceVariables.Size = new System.Drawing.Size(116, 19);
			this.CheckboxReplaceVariables.TabIndex = 82;
			this.CheckboxReplaceVariables.Text = "Replace Variables";
			this.CheckboxReplaceVariables.UseVisualStyleBackColor = true;
			this.CheckboxReplaceVariables.CheckedChanged += new System.EventHandler(this.CheckboxReplaceVariables_CheckedChanged);
			// 
			// CheckboxDisplayOriginalText
			// 
			this.CheckboxDisplayOriginalText.AutoSize = true;
			this.CheckboxDisplayOriginalText.Location = new System.Drawing.Point(1104, 52);
			this.CheckboxDisplayOriginalText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.CheckboxDisplayOriginalText.Name = "CheckboxDisplayOriginalText";
			this.CheckboxDisplayOriginalText.Size = new System.Drawing.Size(133, 19);
			this.CheckboxDisplayOriginalText.TabIndex = 81;
			this.CheckboxDisplayOriginalText.Text = "Display Original Text";
			this.CheckboxDisplayOriginalText.UseVisualStyleBackColor = true;
			this.CheckboxDisplayOriginalText.CheckedChanged += new System.EventHandler(this.CheckboxDisplayOriginalText_CheckedChanged);
			// 
			// TextboxCurrentLanguage
			// 
			this.TextboxCurrentLanguage.Location = new System.Drawing.Point(1402, 90);
			this.TextboxCurrentLanguage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.TextboxCurrentLanguage.Name = "TextboxCurrentLanguage";
			this.TextboxCurrentLanguage.Size = new System.Drawing.Size(94, 23);
			this.TextboxCurrentLanguage.TabIndex = 97;
			// 
			// LabelFontName
			// 
			this.LabelFontName.AutoSize = true;
			this.LabelFontName.Location = new System.Drawing.Point(1399, 119);
			this.LabelFontName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelFontName.Name = "LabelFontName";
			this.LabelFontName.Size = new System.Drawing.Size(63, 15);
			this.LabelFontName.TabIndex = 96;
			this.LabelFontName.Text = "FontName";
			// 
			// LabelFontSize
			// 
			this.LabelFontSize.AutoSize = true;
			this.LabelFontSize.Location = new System.Drawing.Point(1399, 141);
			this.LabelFontSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LabelFontSize.Name = "LabelFontSize";
			this.LabelFontSize.Size = new System.Drawing.Size(30, 15);
			this.LabelFontSize.TabIndex = 95;
			this.LabelFontSize.Text = "Size:";
			// 
			// NumericUpDownFontSize
			// 
			this.NumericUpDownFontSize.Location = new System.Drawing.Point(1437, 137);
			this.NumericUpDownFontSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.NumericUpDownFontSize.Name = "NumericUpDownFontSize";
			this.NumericUpDownFontSize.Size = new System.Drawing.Size(59, 23);
			this.NumericUpDownFontSize.TabIndex = 94;
			this.NumericUpDownFontSize.ValueChanged += new System.EventHandler(this.NumericUpDownFontSize_ValueChanged);
			// 
			// ButtonSaveAs
			// 
			this.ButtonSaveAs.Location = new System.Drawing.Point(1402, 8);
			this.ButtonSaveAs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonSaveAs.Name = "ButtonSaveAs";
			this.ButtonSaveAs.Size = new System.Drawing.Size(103, 35);
			this.ButtonSaveAs.TabIndex = 93;
			this.ButtonSaveAs.Text = "Save File As";
			this.ButtonSaveAs.UseVisualStyleBackColor = true;
			this.ButtonSaveAs.Click += new System.EventHandler(this.ButtonSaveAs_Click);
			// 
			// ButtonReloadVariables
			// 
			this.ButtonReloadVariables.Location = new System.Drawing.Point(1278, 120);
			this.ButtonReloadVariables.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonReloadVariables.Name = "ButtonReloadVariables";
			this.ButtonReloadVariables.Size = new System.Drawing.Size(118, 35);
			this.ButtonReloadVariables.TabIndex = 92;
			this.ButtonReloadVariables.Text = "Reload Vars";
			this.ButtonReloadVariables.UseVisualStyleBackColor = true;
			this.ButtonReloadVariables.Click += new System.EventHandler(this.ButtonReloadVariables_Click);
			// 
			// ButtonFastRead
			// 
			this.ButtonFastRead.Location = new System.Drawing.Point(1278, 84);
			this.ButtonFastRead.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonFastRead.Name = "ButtonFastRead";
			this.ButtonFastRead.Size = new System.Drawing.Size(118, 35);
			this.ButtonFastRead.TabIndex = 91;
			this.ButtonFastRead.Text = "Fast-Read";
			this.ButtonFastRead.UseVisualStyleBackColor = true;
			this.ButtonFastRead.Click += new System.EventHandler(this.ButtonFastRead_Click);
			// 
			// ButtonResetStringIndex
			// 
			this.ButtonResetStringIndex.Location = new System.Drawing.Point(1278, 46);
			this.ButtonResetStringIndex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonResetStringIndex.Name = "ButtonResetStringIndex";
			this.ButtonResetStringIndex.Size = new System.Drawing.Size(118, 35);
			this.ButtonResetStringIndex.TabIndex = 90;
			this.ButtonResetStringIndex.Text = "Reset";
			this.ButtonResetStringIndex.UseVisualStyleBackColor = true;
			this.ButtonResetStringIndex.Click += new System.EventHandler(this.ButtonResetStringIndex_Click);
			// 
			// ButtonOpenFile
			// 
			this.ButtonOpenFile.Location = new System.Drawing.Point(1278, 7);
			this.ButtonOpenFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonOpenFile.Name = "ButtonOpenFile";
			this.ButtonOpenFile.Size = new System.Drawing.Size(118, 36);
			this.ButtonOpenFile.TabIndex = 89;
			this.ButtonOpenFile.Text = "Open File/Font";
			this.ButtonOpenFile.UseVisualStyleBackColor = true;
			this.ButtonOpenFile.Click += new System.EventHandler(this.ButtonOpenFile_Click);
			// 
			// ButtonCopyImage
			// 
			this.ButtonCopyImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.ButtonCopyImage.Location = new System.Drawing.Point(1402, 46);
			this.ButtonCopyImage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ButtonCopyImage.Name = "ButtonCopyImage";
			this.ButtonCopyImage.Size = new System.Drawing.Size(103, 35);
			this.ButtonCopyImage.TabIndex = 88;
			this.ButtonCopyImage.Text = "Copy image";
			this.ButtonCopyImage.UseVisualStyleBackColor = true;
			this.ButtonCopyImage.Click += new System.EventHandler(this.ButtonCopyImage_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// ReopenWindowButton
			// 
			this.ReopenWindowButton.Location = new System.Drawing.Point(1512, 8);
			this.ReopenWindowButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.ReopenWindowButton.Name = "ReopenWindowButton";
			this.ReopenWindowButton.Size = new System.Drawing.Size(31, 35);
			this.ReopenWindowButton.TabIndex = 98;
			this.ReopenWindowButton.Text = "[  ]";
			this.ReopenWindowButton.UseVisualStyleBackColor = true;
			this.ReopenWindowButton.Click += new System.EventHandler(this.ReopenWindowButton_Click);
			// 
			// SetupSpritesButton
			// 
			this.SetupSpritesButton.Location = new System.Drawing.Point(1512, 49);
			this.SetupSpritesButton.Name = "SetupSpritesButton";
			this.SetupSpritesButton.Size = new System.Drawing.Size(31, 32);
			this.SetupSpritesButton.TabIndex = 99;
			this.SetupSpritesButton.Text = "[i]";
			this.SetupSpritesButton.UseVisualStyleBackColor = true;
			this.SetupSpritesButton.Click += new System.EventHandler(this.SetupSpritesButton_Click);
			// 
			// SetupVoicelinesButton
			// 
			this.SetupVoicelinesButton.Location = new System.Drawing.Point(1512, 84);
			this.SetupVoicelinesButton.Name = "SetupVoicelinesButton";
			this.SetupVoicelinesButton.Size = new System.Drawing.Size(31, 29);
			this.SetupVoicelinesButton.TabIndex = 100;
			this.SetupVoicelinesButton.Text = "[<]";
			this.SetupVoicelinesButton.UseVisualStyleBackColor = true;
			this.SetupVoicelinesButton.Click += new System.EventHandler(this.SetupVoicelinesButton_Click);
			// 
			// DumpSelectedVoicelinesButton
			// 
			this.DumpSelectedVoicelinesButton.Location = new System.Drawing.Point(1503, 117);
			this.DumpSelectedVoicelinesButton.Name = "DumpSelectedVoicelinesButton";
			this.DumpSelectedVoicelinesButton.Size = new System.Drawing.Size(40, 29);
			this.DumpSelectedVoicelinesButton.TabIndex = 1;
			this.DumpSelectedVoicelinesButton.Text = "[S<]";
			this.DumpSelectedVoicelinesButton.UseVisualStyleBackColor = true;
			this.DumpSelectedVoicelinesButton.Click += new System.EventHandler(this.DumpVoicelineOnlyButton_Click);
			// 
			// OpenVerticalViewButton
			// 
			this.OpenVerticalViewButton.Location = new System.Drawing.Point(1503, 149);
			this.OpenVerticalViewButton.Name = "OpenVerticalViewButton";
			this.OpenVerticalViewButton.Size = new System.Drawing.Size(40, 25);
			this.OpenVerticalViewButton.TabIndex = 101;
			this.OpenVerticalViewButton.Text = "VW";
			this.OpenVerticalViewButton.UseVisualStyleBackColor = true;
			this.OpenVerticalViewButton.Click += new System.EventHandler(this.OpenVerticalViewButton_Click);
			// 
			// Operations
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1546, 179);
			this.Controls.Add(this.OpenVerticalViewButton);
			this.Controls.Add(this.DumpSelectedVoicelinesButton);
			this.Controls.Add(this.SetupVoicelinesButton);
			this.Controls.Add(this.SetupSpritesButton);
			this.Controls.Add(this.ReopenWindowButton);
			this.Controls.Add(this.TextboxCurrentLanguage);
			this.Controls.Add(this.LabelFontName);
			this.Controls.Add(this.LabelFontSize);
			this.Controls.Add(this.NumericUpDownFontSize);
			this.Controls.Add(this.ButtonSaveAs);
			this.Controls.Add(this.ButtonReloadVariables);
			this.Controls.Add(this.ButtonFastRead);
			this.Controls.Add(this.ButtonResetStringIndex);
			this.Controls.Add(this.ButtonOpenFile);
			this.Controls.Add(this.ButtonCopyImage);
			this.Controls.Add(this.CheckboxTranslationMode);
			this.Controls.Add(this.CheckboxPauseAutoplay);
			this.Controls.Add(this.LabelOpenedFile);
			this.Controls.Add(this.CheckboxStartAutoplay);
			this.Controls.Add(this.CheckboxDisplayCharacter);
			this.Controls.Add(this.CheckboxReplaceVariables);
			this.Controls.Add(this.CheckboxDisplayOriginalText);
			this.Controls.Add(this.LabelVoiceline);
			this.Controls.Add(this.LabelCurrentAnimation);
			this.Controls.Add(this.CheckboxUseAlternateVars);
			this.Controls.Add(this.LabelCurrentTranslation);
			this.Controls.Add(this.LabelOriginFile);
			this.Controls.Add(this.LabelCharacterName);
			this.Controls.Add(this.LabelLineNumber);
			this.Controls.Add(this.ListBoxMenuElements);
			this.Controls.Add(this.ListBoxMenuIndex);
			this.Controls.Add(this.ButtonNextLanguage);
			this.Controls.Add(this.ButtonBackLanguage);
			this.Controls.Add(this.ButtonNextText);
			this.Controls.Add(this.ButtonBackText);
			this.Controls.Add(this.CB_Game);
			this.Controls.Add(this.CheckboxAutoTranslation);
			this.Controls.Add(this.CheckboxPlayVoiceTTS);
			this.Controls.Add(this.CheckboxMaybeAccurateHeight);
			this.Controls.Add(this.LabelUnsupportedWarning);
			this.Controls.Add(this.CB_TB);
			this.Controls.Add(this.LabelTextboxStyle);
			this.Controls.Add(this.ButtonReloadText);
			this.Controls.Add(this.Textbox);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.MaximumSize = new System.Drawing.Size(1562, 218);
			this.MinimumSize = new System.Drawing.Size(1562, 218);
			this.Name = "Operations";
			this.Text = "Operations (v2.5)";
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownFontSize)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

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
		private Button DumpSelectedVoicelinesButton;
		private ToolTip toolTip1;
		private Button OpenVerticalViewButton;
	}
}