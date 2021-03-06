﻿using System.Drawing;
using OgmoEditor.LevelEditors.LayerEditors;

namespace OgmoEditor.LevelEditors.Resizers
{
	public abstract class Resizer
	{
		public LayerEditor Editor { get; private set; }

		public Resizer(LayerEditor editor)
		{
			Editor = editor;
		}

		public abstract void Resize(Size oldSize, bool fromRight, bool fromBottom);
		public abstract void Undo();
	}
}
