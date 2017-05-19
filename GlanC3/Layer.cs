using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Glc
{
	public class Layer
	{
		public Layer()
		{
			_className = "Layer" + _count;
			_objectName = "layerObject" + _count;
			++_count;
			_objects = new List<GameObject>();
			_scripts = new List<Component.Script>();
		}
		public void AddObject(GameObject o)
		{
			_objects.Add(o);
			o._layer = this;
			o._scene = _scene;
		}
		public List<GameObject> GetObjectList()
		{
			return _objects;
		}
		public void AddScript(Component.Script s)
		{
			_scripts.Add(s);
		}
		public void SetScene(Scene s)
		{
			_scene = s;
			foreach (var i in _objects)
				i.SetScene(s);
		}

		public string GetImplementationFileName()
		{
			return _scene.ClassName + '-' + ClassName + ".cpp";
		}
		public string GetDeclarationFileName()
		{
			return _scene.ClassName + '-' + ClassName + ".h";
		}

		/// <summary>Name of this object for client, and name of this object class in cpp code</summary>
		public string ClassName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					_className = value;
				else throw new ArgumentException("Name must contain only letters, numbers and cymbol '_'");
			}
			get{return _className;}
		}
		/// <summary>Name of this object in cpp code</summary>
		internal string ObjectName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					_objectName = value;
			}
			get { return _objectName; }
		}
		/// <summary>Generate .h and .cpp files for this object</summary>
		internal void GenerateCode()
		{
			if (_scene == null)
				throw new Exception("Layer " + ClassName + " haven't Scene, when GenerateCode called");
				Glance.CodeGenerator.writeLayer(
					Glance.BuildSetting.sourceDir + GetDeclarationFileName(),
					Glance.BuildSetting.sourceDir + GetImplementationFileName(), 
					this);
		}
		/// <summary>return all variables of this layer</summary>
		internal string GetVariables()
		{
			string result = "";
			var dict = new Dictionary<Glance.FieldsAccessType, List<string>>();
			foreach (var i in _scripts)
				dict.gAddOrMerge(i.GetCppVariables());
			dict = dict.gRemoveWhiteSpace();

			result += "\npublic:\n";
			result += Glance.GatherStringList(dict.gGetByKeyOrDefault(Glance.FieldsAccessType.Public), ";\n") + ';';
			foreach (var i in _objects)
				result += i.ClassName + ' ' + i.ObjectName + ";\n";//in public area

			result += "\nprivate:\n";
			result += Glance.GatherStringList(dict.gGetByKeyOrDefault(Glance.FieldsAccessType.Private), ";\n") + ';';
			return result;
		}
		/// <summary>return all methods declaration of this layer</summary>
		internal string GetMethodsDeclaration()
		{
			string result = "";
			var dict = new Dictionary<Glance.FieldsAccessType, List<string>>();
			foreach (var i in _scripts)
				dict.gAddOrMerge(i.GetCppMethodsDeclaration());
			//distinct array
			result += "public:\n";
			result += Glance.GatherStringList(dict.gGetByKeyOrDefault(Glance.FieldsAccessType.Public).Distinct().ToList(), ";\n");
			result += "private:\n";
			result += Glance.GatherStringList(dict.gGetByKeyOrDefault(Glance.FieldsAccessType.Private).Distinct().ToList(), ";\n");
			//some methods may repeat in different components, it's normal
			return result;
		}
		/// <summary>return all methods implementation of this layer</summary>
		internal string GetMethodsImplementation()
		{
			string result = "";
			var functions = new Dictionary<string, string>();
			foreach (var i in _scripts)
				Glance.MergeDictionary(ref functions, i.GetCppMethodsImplementation());
			foreach (var i in functions)
				if (i.Key != "")
					result += Glance.GetRetTypeFromSignature(i.Key) + ' ' + ClassName + "::" + Glance.GetSignatureWithoutRetType(i.Key) + '{' + i.Value + '}' + '\n';
			return result;
		}
		/// <summary>return constructor syntax for all holded objects</summary>
		internal string GetConstructor()
		{
			string result = ", ";
			foreach (var i in _objects)
				result += i.ObjectName + "(scene, *this)";
			return result;
		}
		/// <summary>return all components necessary Constructor code</summary>
		internal string GetConstructorBody()
		{
			return "";
		}
		/// <summary>return all components necessary OnUpdate code</summary>
		internal string GetOnUpdate()
		{
			string result = "";
			foreach (var i in _scripts)
			{
				string temp = i.GetCppOnUpdate();
				if (temp != "")
					result += '\n' + i.GetCppOnUpdate();
			}
			result += '\n';
			foreach (var i in _objects)
				result += i.ObjectName + ".onUpdate(dt);\n";
			return result;
		}
		/// <summary>return all components necessary OnStart code</summary>
		internal string GetOnStart()
		{
			string result = "";
			foreach (var i in _scripts)
			{
				string temp = i.GetCppOnStart();
				if (temp != "")
					result += '\n' + i.GetCppOnStart();
			}
			result += '\n';
			foreach (var i in _objects)
				result += i.ObjectName + ".onStart();\n";
			return result;
		}

		internal Scene _scene;
		/// <summary>All GameObjects in this layer</summary>
		internal List<GameObject> _objects;
		/// <summary>All Scripts for this layer</summary>
		internal List<Component.Script> _scripts;
		internal string _className;
		protected string _objectName;
		private static uint _count;
		static Layer() { _count = 0; }
	}
}
