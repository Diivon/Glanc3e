using System;
using System.Collections.Generic;
using System.Linq;

namespace Glc
{
	public class PhysicalObject : GameObject
	{
		public Vec2 Pos;
		public PhysicalObject(Vec2 pos) : base()
		{
			_scene = null;
			Pos = pos;
			ClassName = "PhysicalObject" + _count++;
			ObjectName = "Obj" + ClassName;
		}
		static PhysicalObject() { _count = 0; }

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
			Glance.CodeGenerator.writePhysicalObject(_declarationfilePath, _implementationfilePath, this);
		}
		internal override string GetComponentsVariables()
		{
			string result = "";
			var dict = new Dictionary<Glance.FieldsAccessType, List<string>>();
			foreach (var i in _components)
				dict.gAddOrMerge(i.GetCppVariables());
			result += "public:\n";
			result += Glance.GatherStringList(dict.gGetByKeyOrDefault(Glance.FieldsAccessType.Public), ";\n");
			result += "private:\n";
			result += Glance.GatherStringList(dict.gGetByKeyOrDefault(Glance.FieldsAccessType.Private), ";\n");
			//do not distinct array, if in components two same variables, it must cause compiling error
			return result;
		}
		internal override string GetComponentsMethodsDeclaration()
		{
			string result = "";
			var dict = new Dictionary<Glance.FieldsAccessType, List<string>>();
			foreach (var i in _components)
				dict.gAddOrMerge(i.GetCppMethodsDeclaration());
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
			string result = "";
			var functions = new Dictionary<string, string>();
			foreach (var i in _components)
				Glance.MergeDictionary(ref functions, i.GetCppMethodsImplementation());
			foreach (var i in functions)
				if(i.Key != "")
					result += Glance.GetRetTypeFromSignature(i.Key) + ' ' + ClassName + "::" + Glance.GetSignatureWithoutRetType(i.Key) + '{' + i.Value + '}' + '\n';
			return result;
		}
		internal override string GetComponentsConstructors()
		{
			var result = new List<string>();
			foreach (var i in _components)
				result.AddRange(i.GetCppConstructor());
			return Glance.GatherStringList(result, ", ");
		}
		internal override string GetComponentsConstructorsBody()
		{
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppConstructorBody() == "")
					continue;
				result += '\n' + com.GetCppConstructorBody();
			}
			if (result != "")
				result += '\n';
			return result;
		}
		internal override string GetComponentsOnUpdate()
		{
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppOnUpdate() == "")
					continue;
				result += "\n" + com.GetCppOnUpdate();
			}
			if (result == "")
				result += '\n';
			return result;
		}
		internal override string GetComponentsOnStart()
		{
			string result = "";
			foreach (var com in _components)
			{
				if (com.GetCppOnStart() == "")
					continue;
				result += "\n" + com.GetCppOnStart();
			}
			if (result == "")
				result += '\n';
			return result;
		}
	}
}