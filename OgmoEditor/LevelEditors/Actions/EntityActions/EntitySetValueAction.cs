﻿using OgmoEditor.LevelData.Layers;

namespace OgmoEditor.LevelEditors.Actions.EntityActions
{
	public class EntitySetValueAction : EntityAction
	{
		private Value value;
		private string setTo;
		private string was;

		public EntitySetValueAction(EntityLayer entityLayer, Value value, string setTo)
			: base(entityLayer)
		{
			this.value = value;
			this.setTo = setTo;
		}

		public override void Do()
		{
			base.Do();

			was = value.Content;
			value.Content = setTo;
		}

		public override void Undo()
		{
			base.Undo();

			value.Content = was;
			Ogmo.EntitySelectionWindow.RefreshContents();
		}
	}
}
