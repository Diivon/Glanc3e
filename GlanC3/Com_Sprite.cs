using System.Collections.Generic;
using System.Linq;

namespace Glc.Component
{
	public abstract partial class GraphicalComponent : Component
	{
		public class Sprite : GraphicalComponent
		{
			string FileName;
			public Sprite(string fn)
			{
				if (fn == null || fn.Length == 0 || fn[1] == ':')//last condition is check absolute path to file
					throw new System.ArgumentException(Glance.BuildSetting.sourceDir + fn);
				FileName = fn;
			}
			internal override Dictionary<Glance.FieldsAccessType, List<string>> GetCppVariables()
			{
                var result = new Dictionary<Glance.FieldsAccessType, List<string>>();
				var variables = Glance.templates["Com:Sprite:Vars"].Split(';').gForEach(x => x.Trim());
				result.Add(Glance.FieldsAccessType.Public, variables.ToList());
                return result;
            }
			internal override Dictionary<Glance.FieldsAccessType, List<string>> GetCppMethodsDeclaration()
			{
                var result = new Dictionary<Glance.FieldsAccessType, List<string>>();
                var methods = Glance.templates["Com:Sprite:Methods"].Split(';').gForEach(x => x.Trim());
                result.Add(Glance.FieldsAccessType.Public, methods.ToList());
                return result;
            }
			internal override Dictionary<string, string> GetCppMethodsImplementation()
			{
				var result = new Dictionary<string, string>();
				foreach (var i in GetCppMethodsDeclaration())//{ {Public, {"void a()"}}, {Private, {"int b(int)"}} }
					foreach (var u in i.Value)  //{ "void a()", "int b(int)" }
						result.Add(u, "");      //{ {"void a()", ""}, {"int b()", ""} }
				return result;
			}
			internal override List<string> GetCppConstructor()
			{
				return Glance.templates["Com:Sprite:Constructor"].Replace("#FileName#", Glance.ToCppString(FileName))
							.Split(',').gForEach(x => x.Trim()).ToList();
			}
			internal override string GetCppConstructorBody()
			{
				return Glance.templates["Com:Sprite:ConstructorBody"].Replace("#FileName#", Glance.ToCppString(FileName));
			}
			internal override string GetCppOnUpdate()
			{
				return Glance.templates["Com:Sprite:OnUpdate"];
			}
			internal override string GetCppOnStart()
			{
				return Glance.templates["Com:Sprite:OnUpdate"];
			}
		}
	}
}
