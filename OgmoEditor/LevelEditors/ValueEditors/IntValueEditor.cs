﻿using System;
using System.Drawing;
using System.Windows.Forms;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.Definitions.ValueDefinitions;
using OgmoEditor.LevelEditors.Actions.EntityActions;

namespace OgmoEditor.LevelEditors.ValueEditors
{
	public partial class IntValueEditor : ValueEditor
	{
		public IntValueDefinition Definition { get; private set; }

		public IntValueEditor(Value value, int x, int y)
			: base(value, x, y)
		{
			Definition = (IntValueDefinition)value.Definition;
			InitializeComponent();

			nameLabel.Text = Definition.Name;
			valueTextBox.Text = Value.Content;

			valueTextBox.LostFocus += valueTextBox_Leave;

			//Deal with the slider
			if (Definition.ShowSlider)
			{
				valueTrackBar.Minimum = Definition.Min;
				valueTrackBar.Maximum = Definition.Max;
				valueTrackBar.Value = Convert.ToInt32(Value.Content);
				valueTrackBar.TickFrequency = (Definition.Max - Definition.Min) / 10;
			}
			else
			{
				Controls.Remove(valueTrackBar);
				valueTrackBar = null;
				Size = new Size(128, 24);
			}
		}

		private void handleTextBox()
		{
			string temp = Value.Content;
			OgmoParse.ParseIntToString(ref temp, Definition.Min, Definition.Max, valueTextBox);
			if (temp != Value.Content)
			{
				if (valueTrackBar != null)
					valueTrackBar.Value = Convert.ToInt32(temp);
				Ogmo.MainWindow.LevelEditors[Ogmo.CurrentLevelIndex].Perform(
						new EntitySetValueAction(null, Value, temp)
					);
			}
		}

		/*
		 *  Events
		 */
		private void valueTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				handleTextBox();
		}

		private void valueTextBox_Leave(object sender, EventArgs e)
		{
			handleTextBox();
		}

		private void valueTrackBar_Scroll(object sender, EventArgs e)
		{
			if (valueTrackBar.Value.ToString() != Value.Content)
			{
				valueTextBox.Text = valueTrackBar.Value.ToString();
				Ogmo.MainWindow.LevelEditors[Ogmo.CurrentLevelIndex].Perform(
						new EntitySetValueAction(null, Value, valueTrackBar.Value.ToString())
					);
			}
		}
	}
}
