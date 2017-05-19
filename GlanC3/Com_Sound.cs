using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glc.Component
{
	public class Sound : Component
	{
		public string FileName;
		public Sound(string filePath)
		{
			if (filePath == null || filePath.Length == 0 || filePath[1] == ':')//last condition is check absolute path to file
				throw new System.ArgumentException(Glance.BuildSetting.sourceDir + filePath);
			FileName = filePath;
		}
		internal override Dictionary<Glance.FieldsAccessType, List<string>> GetCppVariables()
		{
            var result = new Dictionary<Glance.FieldsAccessType, List<string>>();
			var variables = Glance.templates["Com:Sound:Vars"].Split(';').gForEach(x => x.Trim());
			result.Add(Glance.FieldsAccessType.Public, variables.ToList());
            return result;
		}
		internal override Dictionary<Glance.FieldsAccessType, List<string>> GetCppMethodsDeclaration()
		{
            var result = new Dictionary<Glance.FieldsAccessType, List<string>>();
            var methods = Glance.templates["Com:Sound:Methods"].Split(';').gForEach(x => x.Trim());
            result.Add(Glance.FieldsAccessType.Public, methods.ToList());
            return result;
		}
		internal override Dictionary<string, string> GetCppMethodsImplementation()
		{
			return new Dictionary<string, string>();
		}
		internal override List<string> GetCppConstructor()
		{
			return Glance.templates["Com:Sound:Constructor"].
				Replace("#FileName#", Glance.ToCppString(FileName)).Split(',').gForEach(x => x.Trim()).ToList();
		}
		internal override string GetCppConstructorBody()
		{
			return Glance.templates["Com:Sound:ConstructorBody"];
		}
		internal override string GetCppOnStart()
		{
			return Glance.templates["Com:Sound:OnStart"];
		}
		internal override string GetCppOnUpdate()
		{
			return Glance.templates["Com:Sound:OnUpdate"];
		}
	}//class Sound
}
