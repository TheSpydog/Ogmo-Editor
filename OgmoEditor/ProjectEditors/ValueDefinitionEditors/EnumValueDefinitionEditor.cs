﻿using System;
using System.Drawing;
using System.Windows.Forms;
using OgmoEditor.Definitions.ValueDefinitions;

namespace OgmoEditor.ProjectEditors.ValueDefinitionEditors
{
	public partial class EnumValueDefinitionEditor : UserControl
	{
		private EnumValueDefinition def;

		public EnumValueDefinitionEditor(EnumValueDefinition def)
		{
			this.def = def;
			InitializeComponent();
			Location = new Point(99, 53);

			elementsTextBox.Text = string.Join("\r\n", def.Elements, 0, def.Elements.Length);
		}

		private void elementsTextBox_Validated(object sender, EventArgs e)
		{
			def.Elements = elementsTextBox.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < def.Elements.Length; i++)
				def.Elements[i] = def.Elements[i].TrimEnd('\n');
		}


	}
}
