﻿using OgmoEditor.Definitions.LayerDefinitions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OgmoEditor.LevelEditors.LayerEditors;
using OgmoEditor.LevelEditors;

namespace OgmoEditor.LevelData.Layers
{
	public abstract class Layer
	{
		public LayerDefinition Definition { get; private set; }
		public Level Level { get; private set; }

		public Layer(Level level, LayerDefinition definition)
		{
			Level = level;
			Definition = definition;
		}

		public abstract XmlElement GetXML(XmlDocument doc);
		public abstract bool SetXML(XmlElement xml);

		public abstract void WriteJSON(JsonTextWriter jw);
		public abstract bool SetJSON(JToken json);

		public abstract LayerEditor GetEditor(LevelEditor editor);
	}
}
