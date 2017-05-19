using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace Glc
{
	public abstract class GameObject
	{

		public void SetLayer(Layer l)
		{
			_layer = l;
		}
		public void SetScene(Scene s)
		{
			_scene = s;
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
		public void AddComponent(Component.Component c)
		{
			_components.Add(c);
		}
		/// <summary>Components of this object</summary>
		protected List<Component.Component> _components;
		/// <summary>Path to .h file of this Object</summary>
		public string ImplementationFilePath
		{
			get { return _implementationfilePath; }
			set
			{
				if (File.Exists(value))
					_implementationfilePath = value;
				else throw new ArgumentException("invalid file path");
			}
		}
		public string DeclarationFilePath
		{
			get { return _declarationfilePath; }
			set
			{
				if (File.Exists(value))
					_declarationfilePath = value;
				else throw new ArgumentException("invalid file path");
			}
		}

		internal string ObjectName
		{
			set
			{
				if (Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
					_objectName = value;
				else throw new ArgumentException("Name must contain only letters, numbers and cymbol '_'");
			}
			get { return _objectName; }
		}
		/// <summary>Scene, where this object is</summary>
		internal Scene _scene;
		internal Layer _layer;
		protected string _objectName;
		protected string _className;
		protected string _implementationfilePath;
		protected string _declarationfilePath;

		protected GameObject()
		{
			_implementationfilePath = null;
			_declarationfilePath = null;
			_components = new List<Component.Component>();
		}
		/// <summary>Generate .h and .cpp files for this object</summary>
		internal abstract void GenerateCode();
		/// <summary>return all components necessary Variables</summary>
		internal abstract string GetComponentsVariables();
		/// <summary>return all components necessary Methods</summary>
		internal abstract string GetComponentsMethodsDeclaration();
		/// <summary>return all components necessary Methods declaration</summary>
		internal abstract string GetComponentsMethodsImplementation();
		/// <summary>return all components necessary Constructors</summary>
		internal abstract string GetComponentsConstructors();
		/// <summary>return all components necessary Constructor code</summary>
		internal abstract string GetComponentsConstructorsBody();
		/// <summary>return all components necessary OnUpdate code</summary>
		internal abstract string GetComponentsOnUpdate();
		/// <summary>return all components necessary OnStart code</summary>
		internal abstract string GetComponentsOnStart();
		///
		internal string GetDeclarationFileName()
		{
			return _scene.ClassName + '-' + _layer.ClassName + '-' + ClassName + ".h";
		}
		internal string GetImplementationFileName()
		{
			return _scene.ClassName + '-' + _layer.ClassName + '-' + ClassName + ".cpp";
		}
	}
}
