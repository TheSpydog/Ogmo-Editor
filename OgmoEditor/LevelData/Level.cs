﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using OgmoEditor.LevelData.Layers;
using OgmoEditor.LevelEditors;
using OgmoEditor.Definitions.ValueDefinitions;

namespace OgmoEditor.LevelData
{
	public class Level
	{
		//Running instance variables
		public Project Project { get; private set; }
		public string SavePath;
		private bool changed;
		public bool Salvaged { get; private set; }

		//Actual parameters to be edited/exported
		public Size Size;
		public List<Layer> Layers { get; private set; }
		public List<Value> Values { get; private set; }
		public Point CameraPosition;

		public Level(Project project, string filename)
		{
			this.Project = project;

			if (File.Exists(filename))
			{
				if (Project.ProjectType == Ogmo.ProjectType.XML)
				{
					//Load the level from XML
					XmlDocument doc = new XmlDocument();
					FileStream stream = new FileStream(filename, FileMode.Open);
					doc.Load(stream);
					stream.Close();

					LoadFromXML(doc);
				}
				else if (Project.ProjectType == Ogmo.ProjectType.JSON)
				{
					// Load the level from JSON
					JObject json = new JObject();

					using (StreamReader stream = new StreamReader(filename))
					using (JsonTextReader reader = new JsonTextReader(stream))
					{
						JsonSerializer serializer = new JsonSerializer();
						JObject levelData = serializer.Deserialize<JObject>(reader);
						LoadFromJSON(levelData);
					}
				}

				SavePath = filename;
			}
			else
			{
				//Initialize size
				Size = Project.LevelDefaultSize;

				//Initialize layers
				Layers = new List<Layer>();
				foreach (var def in Project.LayerDefinitions)
					Layers.Add(def.GetInstance(this));

				//Initialize values
				if (Project.LevelValueDefinitions.Count > 0)
				{
					Values = new List<Value>();
					foreach (var def in Project.LevelValueDefinitions)
						Values.Add(new Value(def));
				}

				SavePath = "";
			}

			changed = false;
		}

		public Level(Project project, XmlDocument xml)
		{
			this.Project = project;

			LoadFromXML(xml);
			SavePath = "";
			changed = false;
		}

		public Level(Level level)
			: this(level.Project, level.GenerateXML())
		{

		}

		public void CloneFrom(Level level)
		{
			LoadFromXML(level.GenerateXML());
		}

		public bool Changed
		{
			get { return changed; }
			set
			{
				changed = value;
				int idx = Ogmo.Levels.FindIndex(l => l == this);
				if (idx >= 0) Ogmo.MainWindow.MasterTabControl.TabPages[idx].Text = Name;
			}
		}

		public string Name
		{
			get
			{
				string s;
				if (string.IsNullOrEmpty(SavePath))
					s = Ogmo.NEW_LEVEL_NAME;
				else
					s = Path.GetFileName(SavePath);
				if (Changed)
					s += "*";
				return s;
			}
		}

		public string SaveName
		{
			get
			{
				string s;
				if (string.IsNullOrEmpty(SavePath))
					s = Ogmo.NEW_LEVEL_NAME;
				else
					s = Path.GetFileName(SavePath);
				return s;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return !HasBeenSaved && !Changed;
			}
		}

		public bool HasBeenSaved
		{
			get { return !string.IsNullOrEmpty(SavePath); }
		}

		public Rectangle Bounds
		{
			get { return new Rectangle(0, 0, Size.Width, Size.Height); }
		}

		public PointF Center
		{
			get { return new PointF(Size.Width / 2, Size.Height / 2); }
		}

		/*
		 *  XML/JSON
		 */
		public XmlDocument GenerateXML()
		{
			XmlDocument doc = new XmlDocument();
			XmlAttribute a;

			XmlElement level = doc.CreateElement("level");
			doc.AppendChild(level);

			//Export the size
			{
				a = doc.CreateAttribute("width");
				a.InnerText = Size.Width.ToString();
				level.Attributes.Append(a);

				a = doc.CreateAttribute("height");
				a.InnerText = Size.Height.ToString();
				level.Attributes.Append(a);
			}

			//Export camera position
			if (Ogmo.Project.ExportCameraPosition)
			{
				XmlElement cam = doc.CreateElement("camera");

				a = doc.CreateAttribute("x");
				a.InnerText = CameraPosition.X.ToString();
				cam.Attributes.Append(a);

				a = doc.CreateAttribute("y");
				a.InnerText = CameraPosition.Y.ToString();
				cam.Attributes.Append(a);

				level.AppendChild(cam);
			}

			//Export the level values
			if (Values != null)
				foreach (var v in Values)
					level.Attributes.Append(v.GetXML(doc));

			//Export the layers
			for (int i = 0; i < Layers.Count; i++)
				level.AppendChild(Layers[i].GetXML(doc));

			return doc;
		}

		public string GenerateJSON()
		{
			StringWriter sw = new StringWriter();
			using (JsonTextWriter jw = new JsonTextWriter(sw))
			{
				// Set up formatting
				jw.Formatting = Newtonsoft.Json.Formatting.Indented;

				// Start the root object
				jw.WriteStartObject();

				// Export the size
				jw.WritePropertyName("width");
				jw.WriteValue(Size.Width);
				jw.WritePropertyName("height");
				jw.WriteValue(Size.Height);

				// Export camera position
				if (Ogmo.Project.ExportCameraPosition)
				{
					jw.WriteStartObject();

					jw.WritePropertyName("x");
					jw.WriteValue(CameraPosition.X);

					jw.WritePropertyName("y");
					jw.WriteValue(CameraPosition.Y);

					jw.WriteEndObject();
				}

				// Export the level values
				if (Values != null)
				{
					foreach (var v in Values)
					{
						jw.WritePropertyName(v.Definition.Name);

						// Make sure to save it as the right type
						Type defType = v.Definition.GetType();
						if (defType == typeof(IntValueDefinition))
						{
							jw.WriteValue(Convert.ToInt32(v.Content));
						}
						else if (defType == typeof(BoolValueDefinition))
						{
							jw.WriteValue(Convert.ToBoolean(v.Content));
						}
						else if (defType == typeof(FloatValueDefinition))
						{
							jw.WriteValue(Convert.ToSingle(v.Content));
						}
						else
						{
							// Treat it as a string
							jw.WriteValue(v.Content);
						}
					}
				}

				// Export the layer array
				jw.WritePropertyName("layers");
				jw.WriteStartArray();
				foreach (Layer layer in Layers)
					layer.WriteJSON(jw);
				jw.WriteEndArray();

				// The End.
				jw.WriteEndObject();
			}

			return sw.ToString();
		}

		private void LoadFromXML(XmlDocument xml)
		{
			bool cleanXML = true;
			XmlElement level = (XmlElement)xml.GetElementsByTagName("level")[0];

			{
				Size size = new Size();

				//Import the size
				if (level.Attributes["width"] != null)
					size.Width = Convert.ToInt32(level.Attributes["width"].InnerText);
				else
					size.Width = Ogmo.Project.LevelDefaultSize.Width;
				if (level.Attributes["height"] != null)
					size.Height = Convert.ToInt32(level.Attributes["height"].InnerText);
				else
					size.Height = Ogmo.Project.LevelDefaultSize.Height;

				//Error check the width
				if (size.Width < Ogmo.Project.LevelMinimumSize.Width)
				{
					size.Width = Ogmo.Project.LevelMinimumSize.Width;
					cleanXML = false;
				}
				else if (size.Width > Ogmo.Project.LevelMaximumSize.Width)
				{
					size.Width = Ogmo.Project.LevelMaximumSize.Width;
					cleanXML = false;
				}

				//Error check the height
				if (size.Height < Ogmo.Project.LevelMinimumSize.Height)
				{
					size.Height = Ogmo.Project.LevelMinimumSize.Height;
					cleanXML = false;
				}
				else if (size.Height > Ogmo.Project.LevelMaximumSize.Height)
				{
					size.Height = Ogmo.Project.LevelMaximumSize.Height;
					cleanXML = false;
				}

				Size = size;
			}

			//Import the camera position
			if (level.GetElementsByTagName("camera").Count > 0)
			{
				XmlElement cam = (XmlElement)level.GetElementsByTagName("camera")[0];
				CameraPosition.X = Convert.ToInt32(cam.Attributes["x"].InnerText);
				CameraPosition.Y = Convert.ToInt32(cam.Attributes["y"].InnerText);
			}

			//Import the level values
			//Initialize values
			if (Project.LevelValueDefinitions.Count > 0)
			{
				Values = new List<Value>();
				foreach (var def in Project.LevelValueDefinitions)
					Values.Add(new Value(def));
				OgmoParse.ImportValues(Values, level);
			}

			//Import layers
			Layers = new List<Layer>();
			for (int i = 0; i < Project.LayerDefinitions.Count; i++)
			{
				Layer layer = Project.LayerDefinitions[i].GetInstance(this);
				Layers.Add(layer);
				if (level[Project.LayerDefinitions[i].Name] != null)
					cleanXML = (layer.SetXML(level[Project.LayerDefinitions[i].Name]) && cleanXML);
			}

			Salvaged = !cleanXML;
		}

		private void LoadFromJSON(JObject json)
		{
			bool cleanJSON = true;

			Size size = new Size();

			//Import the size
			if (json.Property("width") != null)
				size.Width = Convert.ToInt32(json.GetValue("width"));
			else
				size.Width = Ogmo.Project.LevelDefaultSize.Width;

			if (json.Property("height") != null)
				size.Height = Convert.ToInt32(json.GetValue("height"));
			else
				size.Height = Ogmo.Project.LevelDefaultSize.Height;

			//Error check the width
			if (size.Width < Ogmo.Project.LevelMinimumSize.Width)
			{
				size.Width = Ogmo.Project.LevelMinimumSize.Width;
				cleanJSON = false;
			}
			else if (size.Width > Ogmo.Project.LevelMaximumSize.Width)
			{
				size.Width = Ogmo.Project.LevelMaximumSize.Width;
				cleanJSON = false;
			}

			Size = size;

			//Import the camera position
			if (json.Property("camera") != null)
			{
				JObject cam = (JObject)json.GetValue("camera");
				CameraPosition.X = Convert.ToInt32(cam.GetValue("x"));
				CameraPosition.Y = Convert.ToInt32(cam.GetValue("y"));
			}

			//Import the level values
			//Initialize values
			if (Project.LevelValueDefinitions.Count > 0)
			{
				Values = new List<Value>();
				foreach (var def in Project.LevelValueDefinitions)
					Values.Add(new Value(def));
				OgmoParse.ImportValues(Values, json);
			}

			//Import layers
			Layers = new List<Layer>();
			List<JToken> jsonLayers = ((JArray)json.GetValue("layers")).ToList();

			for (int i = 0; i < Project.LayerDefinitions.Count; i++)
			{
				Layer layer = Project.LayerDefinitions[i].GetInstance(this);
				Layers.Add(layer);

				// Find the json object with the name of this layer
				JObject jLayer = (JObject)jsonLayers.Find(o => o.Value<string>("name") == Project.LayerDefinitions[i].Name);

				if (jLayer != null)
					cleanJSON = (layer.SetJSON(jLayer) && cleanJSON);
			}

			Salvaged = !cleanJSON;
		}

		public void EditProperties()
		{
			Ogmo.MainWindow.DisableEditing();
			LevelProperties lp = new LevelProperties(this);
			lp.Show(Ogmo.MainWindow);
		}

		#region Saving

		public bool Save()
		{
			//If it hasn't been saved before, do SaveAs instead
			if (!HasBeenSaved)
				return SaveAs();

			WriteTo(SavePath);
			return true;
		}

		public bool SaveAs()
		{
			SaveFileDialog dialog = new SaveFileDialog();
			if (Project.RecentLevelDirectory == "" || !Directory.Exists(Project.RecentLevelDirectory))
				dialog.InitialDirectory = Project.SavedDirectory;
			else
				dialog.InitialDirectory = Project.RecentLevelDirectory;
			dialog.RestoreDirectory = true;
			dialog.FileName = SaveName;
			dialog.OverwritePrompt = true;
			dialog.Filter = Ogmo.GetLevelFilter();

			//Handle cancel
			if (dialog.ShowDialog() == DialogResult.Cancel)
				return false;

			SavePath = dialog.FileName;
			WriteTo(dialog.FileName);

			// Remember this directory
			string fileDirectory = Path.GetDirectoryName(dialog.FileName);
			if (Project.RecentLevelDirectory != fileDirectory)
			{
				Project.RecentLevelDirectory = fileDirectory;
				Project.Save();
			}

			return true;
		}

		public void WriteTo(string filename)
		{
			if (Project.ProjectType == Ogmo.ProjectType.XML)
			{
				XmlDocument doc = GenerateXML();
				doc.Save(filename);
			}
			else if (Project.ProjectType == Ogmo.ProjectType.JSON)
			{
				string json = GenerateJSON();

				using (StreamWriter stream = new StreamWriter(filename))
				{
					stream.Write(json);
				}
			}

			Changed = false;
		}

		#endregion
	}
}
