﻿using System.Drawing;
using System.Windows.Forms;
using OgmoEditor.LevelData.Layers;

namespace OgmoEditor.LevelEditors.ValueEditors
{
	public partial class ValueEditor : UserControl
	{
		public Value Value { get; private set; }

		private ValueEditor()
		{
			//Never call this!
		}

		public ValueEditor(Value value, int x, int y)
		{
			Value = value;
			Location = new Point(x, y);
		}
	}
}
