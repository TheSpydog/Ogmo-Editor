﻿using System.Drawing;
using OgmoEditor.LevelEditors.Actions.GridActions;

namespace OgmoEditor.LevelData.Layers
{
	public class GridSelection
	{
		public GridLayer Layer;
		public Rectangle Area;
		public bool[,] Under;

		public Rectangle ScaledArea
		{
			get
			{
				return new Rectangle(
					Area.X * Layer.Definition.Grid.Width,
					Area.Y * Layer.Definition.Grid.Height,
					Area.Width * Layer.Definition.Grid.Width,
					Area.Height * Layer.Definition.Grid.Height
				);
			}
		}

		public GridSelection(GridLayer layer, Rectangle area)
		{
			Layer = layer;
			Area = area;

			Under = new bool[Area.Width, Area.Height];
		}

		public void SetUnderFromGrid()
		{
			for (int i = 0; i < Area.Width; i++)
				for (int j = 0; j < Area.Height; j++)
					Under[i, j] = Layer.Grid[i + Area.X, j + Area.Y];
		}

		public bool[,] GetBitsFromGrid()
		{
			bool[,] bits = new bool[Area.Width, Area.Height];
			for (int i = 0; i < Area.Width; i++)
				for (int j = 0; j < Area.Height; j++)
					bits[i, j] = Layer.Grid[i + Area.X, j + Area.Y];
			return bits;
		}

		public GridMoveSelectionAction GetMoveAction(Point move)
		{
			if (Area.X + move.X >= 0 && Area.Y + move.Y >= 0 && Area.X + move.X + Area.Width <= Layer.GridCellsX && Area.Y + move.Y + Area.Height <= Layer.GridCellsY)
				return new GridMoveSelectionAction(Layer, move);
			else
				return null;
		}
	}
}
