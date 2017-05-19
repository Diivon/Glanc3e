using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Glc
{
	public class Scene
	{
		public Scene()
		{
			ClassName = "Scene" + _count.ToString();
			ObjectName = "Obj" + ClassName;
			_layerList = new List<Layer>();
			++_count;
		}
		public string ClassName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					_className = value;
				else throw new ArgumentException("Name must contain only letters, numbers and cymbol '_'");
			}
			get { return _className; }
		}
		public string ObjectName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					_objectName = value;
				else throw new ArgumentException("Name must contain only letters, numbers and cymbol '_'");
			}
			get { return _objectName; }
		}
		public void AddLayer(Layer go)
		{
			go.SetScene(this);
			_layerList.Add(go);
		}
		public List<Layer> GetLayerList()
		{
			return _layerList;
		}
		public string GetDeclarationFileName()
		{
			return ClassName + ".h";
		}
		internal List<Layer> _layerList;
		private string _className;
		private string _objectName;
		private static int _count;
		static Scene() { _count = 0; }
		internal string GetAllLayersOnStart()
		{
			string result = "";
			foreach (var i in _layerList)
				result += i.ObjectName + ".onStart();\n";
			return result;
		}
		internal string GetAllLayersOnUpdate()
		{
			string result = "";
			foreach (var i in _layerList)
				result += i.ObjectName + ".onUpdate(dt);\n";
			return result;
		}
		internal string GetVariables()
		{
			string result = "";
			foreach (var i in _layerList)
				result += i.ClassName + ' ' + i.ObjectName + ";\n"; //variables
			return result;
		}
		internal string GetConstructors()
		{
			if (_layerList.Count == 0)
				return "";
			var list = new List<string>();
			foreach (var i in _layerList)
				list.Add(i.ObjectName + "(*this)");
			return ":\n" + Glance.GatherStringList(list, ", ");
		}
		internal string GetGetLayers()
		{
			var result = "";
			foreach (var i in _layerList)
			{
				result += "template<>\n" + i.ClassName + " & getLayer(){\nreturn " + i.ObjectName + ";\n}\n";
				result += "template<>\nconst " + i.ClassName + " & getLayer() const{\nreturn " + i.ObjectName + ";\n}\n";
			}
			return result;
		}
		internal string GetRender()
		{
			string result = "template<>\ninline void ::gc::Renderer::renderScene(const " + ClassName + " & s){\n";
			foreach (var i in _layerList)	//renderScene
				result += "this->renderLayer(s.getLayer<" + i.ClassName + ">());\n";
			result += '}';
			return result;
		}
		internal string GetIncludes()
		{
			var result = "";
			foreach (var i in _layerList)
				result += "#include \"" + i.GetDeclarationFileName() + "\"\n";
			return result;
		}
	}
}