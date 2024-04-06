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

		private void ChangedReplaceVariables()
		{
			if (AutoPlayOn || CheckboxStartAutoplay.Checked)
			{
				replaceVariablesToolStripMenuItem.Checked = false;
				replaceVariablesToolStripMenuItem.Invalidate();
				return;
			}

			if (replaceVariablesToolStripMenuItem.Checked)
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

		private void CheckboxReplaceVariables_CheckedChanged(object sender, EventArgs e)
		{
			ChangedReplaceVariables();
		}

		private void ChangedDisplayCharacter()
		{
			if (!LoadedFile)
			{
				displayCharacterToolStripMenuItem.Checked = false;
				displayCharacterToolStripMenuItem.Invalidate();
				return;
			}

			if (fi.Type == FileManager.LoadedFileType.Txt)
			{
				displayCharacterToolStripMenuItem.Checked = false;
				displayCharacterToolStripMenuItem.Invalidate();
				return;
			}

			if (AutoPlayOn)
			{
				displayCharacterToolStripMenuItem.Checked = false;
				displayCharacterToolStripMenuItem.Invalidate();
				return;
			}

			CheckUnsaved();
			UpdateTextbox();
			DisplayCharacterImage();
		}

		private void CheckboxDisplayCharacter_CheckedChanged(object sender, EventArgs e)
		{
			ChangedDisplayCharacter();
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
				CheckboxPauseAutoplay.Enabled = true;
				CheckForIllegalCrossThreadCalls = false;
				Thread t = new Thread(AutoPlay);
				t.Start();
			}
			else
			{
				if (CheckboxPauseAutoplay.Checked)
				{
					CheckboxPauseAutoplay.Enabled = false;
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

		private void ChangedTranslationMode()
		{
			if (LoadedFile)
			{
				// It needs to be activated before opening a file
				translationModeToolStripMenuItem.Checked = false;
				CheckboxTranslationMode.Update();
				return;
			}

			if (AutoPlayOn)
			{
				translationModeToolStripMenuItem.Checked = false;
				CheckboxTranslationMode.Update();
			}
		}

		private void CheckboxTranslationMode_CheckedChanged(object sender, EventArgs e)
		{
			ChangedTranslationMode();
		}

		private void ChangedAlternateVars()
		{
			if (AutoPlayOn || FastReading)
			{
				useAlternateVarsToolStripMenuItem.Checked = false;
				useAlternateVarsToolStripMenuItem.Invalidate();
				return;
			}

			AltVars = useAlternateVarsToolStripMenuItem.Checked;
			DoReloadVariables();
		}

		private void CheckboxUseAlternateVars_CheckedChanged(object sender, EventArgs e)
		{
			ChangedAlternateVars();
		}

		private void ChangedAccurateHeight()
		{
			Reload();
		}

		private void CheckboxMaybeAccurateHeight_CheckedChanged(object sender, EventArgs e)
		{
			ChangedAccurateHeight();
		}
	}
}