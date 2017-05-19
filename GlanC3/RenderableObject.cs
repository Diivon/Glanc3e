using System;
using System.Collections.Generic;
using System.Linq;

namespace Glc
{
	public class RenderableObject : PhysicalObject
	{
		/// <summary>Graphical components of this object</summary>
		public Component.GraphicalComponent GraphComponent;
		public RenderableObject(Vec2 p) : base(p)
		{
			GraphComponent = null;
			ClassName = "RenderableObject" + _count++;
			ObjectName = "Obj" + ClassName;
		}
		static RenderableObject() { _count = 0; }

		private static uint _count;
		internal override void GenerateCode()
		{
			if (_implementationfilePath == "" || _implementationfilePath == null)
				throw new Exception("File implementation do not exist for " + ClassName + ", when GenerateCode called");
			if (_declarationfilePath == "" || _declarationfilePath == null)
				throw new Exception("File declaration do not exist for " + ClassName + ", when GenerateCode called");
			if (_scene == null)
				throw new Exception("Object " + ClassName + " haven't Scene, when GenerateCode called");
			if (_layer == null)
				throw new Exception("Object " + ClassName + " haven't Layer, when GenerateCode called");
			Glance.CodeGenerator.writeRenderableObject(_declarationfilePath, _implementationfilePath, this);
		}
		internal override string GetComponentsVariables()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			var dict = new Dictionary<Glance.FieldsAccessType, List<string>>();
			foreach (var i in _components)
				dict.gAddOrMerge(i.GetCppVariables());
			dict.gAddOrMerge(GraphComponent.GetCppVariables());
			dict = dict.gRemoveWhiteSpace();

			result += "\npublic:\n";
			result += Glance.GatherStringList(dict.gGetByKeyOrDefault(Glance.FieldsAccessType.Public), ";\n") + ';';
			result += "\nprivate:\n";
			result += Glance.GatherStringList(dict.gGetByKeyOrDefault(Glance.FieldsAccessType.Private), ";\n") + ';';
			return result;
		}
		internal override string GetComponentsMethodsDeclaration()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			var dict = new Dictionary<Glance.FieldsAccessType, List<string>>();
			foreach (var i in _components)
				dict.gAddOrMerge(i.GetCppMethodsDeclaration());
			dict.gAddOrMerge(GraphComponent.GetCppMethodsDeclaration());
			//distinct array
			result += "public:\n";
			result += Glance.GatherStringList(dict.gGetByKeyOrDefault(Glance.FieldsAccessType.Public).Distinct().ToList(), ";\n");
			result += "private:\n";
			result += Glance.GatherStringList(dict.gGetByKeyOrDefault(Glance.FieldsAccessType.Private).Distinct().ToList(), ";\n");
			//some methods may repeat in different components, it's normal
			return result;
		}
		internal override string GetComponentsMethodsImplementation()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			var functions = new Dictionary<string, string>();
			foreach (var i in _components)
				Glance.MergeDictionary(ref functions, i.GetCppMethodsImplementation());
			Glance.MergeDictionary(ref functions, GraphComponent.GetCppMethodsImplementation());
			foreach (var i in functions)
				if (i.Key != "") 
					result += Glance.GetRetTypeFromSignature(i.Key) + ' ' + ClassName + "::" + Glance.GetSignatureWithoutRetType(i.Key) + '{' + i.Value + '}' + '\n';
			return result;
		}
		internal override string GetComponentsConstructors()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			List<string> result = new List<string>();
			foreach (var i in _components)
				result.AddRange(i.GetCppConstructor());
			result.AddRange(GraphComponent.GetCppConstructor());
			if (result.Count == 0)
				return "";
			return ", " + Glance.GatherStringList(result, ", ");
		}
		internal override string GetComponentsConstructorsBody()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppConstructorBody() == "")
					continue;
				result += '\n' + com.GetCppConstructorBody();
			}
			result += GraphComponent.GetCppConstructorBody();
			if (result != "")
				result += '\n';
			return result;
		}
		internal override string GetComponentsOnUpdate()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppOnUpdate() == "")
					continue;
				result += "\n" + com.GetCppOnUpdate();
			}
			result += GraphComponent.GetCppOnUpdate();
			if (result == "")
				result += '\n';
			return result;
		}
		internal override string GetComponentsOnStart()
		{
			if (GraphComponent == null)
				throw new InvalidOperationException("GraphComponent for Object " + ClassName + "is empty");
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppOnStart() == "")
					continue;
				result += "\n" + com.GetCppOnStart();
			}
			result += GraphComponent.GetCppOnStart();
			if (result == "")
				result += '\n';
			return result;
		}
		internal string GetCurrentSprite()
		{
			if (GraphComponent is Component.GraphicalComponent.Sprite)
				return "return sprite;";
			if (GraphComponent is Component.GraphicalComponent.Animation)
				return "return " + Glance.NameSetting.AnimationName + ".getCurrentSprite();";
			return "static_assert(false, \"in Glc.RenderableObject.GetCurrentSprite insufficient cases for GraphComponent\");";
		}
	}
}