namespace DGRV3TS
{
	partial class Operations
	{
		private void CheckboxDisplayOriginalText_CheckedChanged(object sender, EventArgs e)
		{
			if (!LoadedFile)
			{
				CheckboxDisplayOriginalText.Checked = false;
				CheckboxDisplayOriginalText.Update();
				return;
			}

			if (fi.Type == FileManager.LoadedFileType.Txt || fi.Type == FileManager.LoadedFileType.Stx)
			{
				CheckboxDisplayOriginalText.Checked = false;
				CheckboxDisplayOriginalText.Update();
				return;
			}

			if (AutoPlayOn)
			{
				CheckboxDisplayOriginalText.Checked = false;
				CheckboxDisplayOriginalText.Update();
				return;
			}

			CheckUnsaved();
			LoadOriginalLanguage(false);
			UpdateRTB();
			UpdateLineCharacter();
			Reload();
		}

		private void CheckboxReplaceVariables_CheckedChanged(object sender, EventArgs e)
		{
			if (AutoPlayOn || CheckboxStartAutoplay.Checked)
			{
				CheckboxReplaceVariables.Checked = false;
				CheckboxReplaceVariables.Update();
				return;
			}

			if (CheckboxReplaceVariables.Checked)
			{
				UpdateRTB();
				Reload();
			}
			else
			{
				UpdateTextbox();
				Reload();
			}
		}

		private void CheckboxDisplayCharacter_CheckedChanged(object sender, EventArgs e)
		{
			if (!LoadedFile)
			{
				CheckboxDisplayCharacter.Checked = false;
				CheckboxDisplayCharacter.Update();
				return;
			}

			if (fi.Type == FileManager.LoadedFileType.Txt)
			{
				CheckboxDisplayCharacter.Checked = false;
				CheckboxDisplayCharacter.Update();
				return;
			}

			if (AutoPlayOn)
			{
				CheckboxDisplayCharacter.Checked = false;
				CheckboxDisplayCharacter.Update();
				return;
			}

			CheckUnsaved();
			UpdateTextbox();
			DisplayCharacterImage();
		}

		private void CheckboxStartAutoplay_CheckedChanged(object sender, EventArgs e)
		{
			if (!LoadedFile)
			{
				CheckboxStartAutoplay.Checked = false;
				CheckboxStartAutoplay.Update();
				return;
			}

			if (AutoPlayOn)
			{
				CheckboxStartAutoplay.Checked = true;
				CheckboxStartAutoplay.Update();
				return;
			}

			if (CheckboxStartAutoplay.Checked)
			{
				// NOT thread-safe
				CheckForIllegalCrossThreadCalls = false;
				Thread t = new Thread(AutoPlay);
				t.Start();
			}
			else
			{
				if (CheckboxPauseAutoplay.Checked)
				{
					CheckboxPauseAutoplay.Checked = false;
					CheckboxPauseAutoplay.Update();
				}
			}
		}

		private void CheckboxPauseAutoplay_CheckedChanged(object sender, EventArgs e)
		{
			if (!CheckboxStartAutoplay.Checked)
			{
				CheckboxPauseAutoplay.Checked = false;
				CheckboxPauseAutoplay.Update();
				return;
			}

			if (AutoPlayOn)
			{
				AutoPlayOn = !CheckboxPauseAutoplay.Checked;
			}
		}

		private void CheckboxTranslationMode_CheckedChanged(object sender, EventArgs e)
		{
			if (LoadedFile)
			{
				// It needs to be activated before opening a file
				CheckboxTranslationMode.Checked = false;
				CheckboxTranslationMode.Update();
				return;
			}

			if (AutoPlayOn)
			{
				CheckboxTranslationMode.Checked = false;
				CheckboxTranslationMode.Update();
			}
		}

		private void CheckboxUseAlternateVars_CheckedChanged(object sender, EventArgs e)
		{
			if (AutoPlayOn || FastReading)
			{
				CheckboxUseAlternateVars.Checked = false;
				CheckboxUseAlternateVars.Update();
				return;
			}

			Convention = CheckboxUseAlternateVars.Checked;
		}

		private void CheckboxMaybeAccurateHeight_CheckedChanged(object sender, EventArgs e)
		{
			Reload();
		}

		private void CheckboxAutoTranslation_CheckedChanged(object sender, EventArgs e)
		{
		}
	}
}