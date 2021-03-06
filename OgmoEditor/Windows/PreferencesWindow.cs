﻿using System;
using System.Windows.Forms;

namespace OgmoEditor.Windows
{
	public partial class PreferencesWindow : Form
	{
		public PreferencesWindow()
		{
			InitializeComponent();
		}

		private void PreferencesWindow_Shown(object sender, EventArgs e)
		{
			maximizeCheckBox.Checked = Properties.Settings.Default.StartMaximized;
			undoLimitTextBox.Text = Properties.Settings.Default.UndoLimit.ToString();
			levelLimitTextBox.Text = Properties.Settings.Default.LevelLimit.ToString();
			rightRadioButton.Checked = Properties.Settings.Default.LevelResizeFromRight;
			leftRadioButton.Checked = !Properties.Settings.Default.LevelResizeFromRight;
			bottomRadioButton.Checked = Properties.Settings.Default.LevelResizeFromBottom;
			topRadioButton.Checked = !Properties.Settings.Default.LevelResizeFromBottom;

			clearHistoryButton.Enabled = Properties.Settings.Default.RecentProjects.Count > 0;
		}

		private void PreferencesWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			Properties.Settings.Default.StartMaximized = maximizeCheckBox.Checked;
			Properties.Settings.Default.LevelResizeFromRight = rightRadioButton.Checked;
			Properties.Settings.Default.LevelResizeFromBottom = bottomRadioButton.Checked;

			try
			{
				Properties.Settings.Default.UndoLimit = Convert.ToInt32(undoLimitTextBox.Text);
			}
			catch
			{ }

			try
			{
				Properties.Settings.Default.LevelLimit = Convert.ToInt32(levelLimitTextBox.Text);
			}
			catch
			{ }

			Properties.Settings.Default.Save();
			Ogmo.MainWindow.EnableEditing();
		}

		private void clearHistoryButton_Click(object sender, EventArgs e)
		{
			Ogmo.ClearRecentProjects();
			clearHistoryButton.Enabled = false;
		}

		private void doneButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
