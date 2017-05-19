using System;
using System.Collections.Generic;
using System.Linq;

namespace Glc.Component
{
	public abstract partial class GraphicalComponent : Component
	{
		public class Animation : GraphicalComponent
		{
			AnimationType _AnimationType;
			List<SpriteFrame> Frames;

			/// <summary>return processed string for GetCpp... family</summary>
			public Animation(AnimationType t)
			{
				_AnimationType = t;
				Frames = new List<SpriteFrame>();
			}
			public void AddFrame(SpriteFrame sf)
			{
				Frames.Add(sf);
			}
			public void AddFrame(string path, float dur)
			{
				Frames.Add(new SpriteFrame(path, dur));
			}


			internal override Dictionary<Glance.FieldsAccessType, List<string>> GetCppVariables()
			{
                var result = new Dictionary<Glance.FieldsAccessType, List<string>>();
                var adding = _GetProcessed(Glance.templates["Com:Animation:Vars"]).Split(';').gForEach(x => x.Trim());
                result.Add(Glance.FieldsAccessType.Public, adding.ToList());
                return result;
			}
			internal override Dictionary<Glance.FieldsAccessType, List<string>> GetCppMethodsDeclaration()
			{
                var result = new Dictionary<Glance.FieldsAccessType, List<string>>();
				var methods = _GetProcessed(Glance.templates["Com:Animation:Methods"]).Split(';').gForEach(x => x.Trim());
				result.Add(Glance.FieldsAccessType.Public, methods.ToList());
                return result;
            }
			internal override Dictionary<string, string> GetCppMethodsImplementation()
			{
				var result = new Dictionary<string, string>();
				foreach (var i in GetCppMethodsDeclaration())//{ {Public, {"void a()"}}, {Private, {"int b(int)"}} }
					foreach (var u in i.Value)	//{ "void a()", "int b(int)" }
						result.Add(u, "");		//{ {"void a()", ""}, {"int b()", ""} }
				return result;
			}
			internal override List<string> GetCppConstructor()
			{
				return _GetProcessed(Glance.templates["Com:Animation:Constructor"]).Split(',').gForEach(x => x.Trim()).ToList();
			}
			internal override string GetCppConstructorBody()
			{
				string code = "";
				foreach (var i in Frames)
					code += Glance.NameSetting.AnimationName + ".emplaceFrame(" + Glance.ToCppString(i.FilePath) + ", " + i.Duration.ToString("0.00").Replace(',', '.') + "f);\n";
				return _GetProcessed(Glance.templates["Com:Animation:ConstructorBody"].Replace("#SpriteFrames#", code));
			}
			internal override string GetCppOnUpdate()
			{
				return _GetProcessed(Glance.templates["Com:Animation:OnUpdate"]);
			}
			internal override string GetCppOnStart()
			{
				return _GetProcessed(Glance.templates["Com:Animation:OnStart"]);
			}
			private string _GetProcessed(string a)
			{
				return a
						.Replace("#AnimationType#", _AnimationTypeToString(_AnimationType))
						.Replace("#AnimationTypeName#", Glance.NameSetting.AnimationType)
						.Replace("#AnimationName#", Glance.NameSetting.AnimationName)
						.Trim()
						;
			}
		}
	}
}
