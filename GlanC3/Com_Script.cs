using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Glc.Component
{
	public class Script : Component
	{
		public string FileName;
		public void Validate()
		{
			string onUpdateCode = GetCppOnUpdate();
			if (onUpdateCode.Contains(Glance.NameSetting.AnimationName + ".update"))
				throw new Exception("Animation updated in script");
			if (onUpdateCode.Contains(Glance.NameSetting.AnimatorName + ".update"))
				throw new Exception("Animator updated in script");
		}
		public static void CreateFile(string path)
		{
			using (var fs = System.IO.File.Create(path))
			{
				string str = Glance.templates["B:Script"];
				byte[] arr = System.Text.Encoding.UTF8.GetBytes(str);
				fs.Write(arr, 0, arr.Length);
			}
		}
		public Script(string filename)
		{
			if (!File.Exists(Glance.BuildSetting.scriptsDir + filename))
				throw new ArgumentException("script file not found " + Glance.BuildSetting.scriptsDir + filename);
			FileName = Glance.BuildSetting.scriptsDir + filename;
			_data = new _Data(this);
		}
		internal override Dictionary<Glance.FieldsAccessType, List<string>> GetCppVariables()
		{
			return _data.Variables;
		}
		internal override List<string> GetCppConstructor()
		{
			return _data.Constructors;
		}
		internal override string GetCppConstructorBody()
		{
			return _data.ConstructorBody;
		}
		internal override string GetCppOnUpdate()
		{
			return _data.OnUpdate;
		}
		internal override string GetCppOnStart()
		{
			return _data.OnStart;
		}
		internal override Dictionary<Glance.FieldsAccessType, List<string>> GetCppMethodsDeclaration()
		{
			return _data.MethodsDeclarations;
		}
		internal override Dictionary<string, string> GetCppMethodsImplementation()
		{
			return _data.MethodsImplementations;
		}
		private _Data _data;
		private class _Data
		{
			enum Region
			{
				Variables,
				Methods,
				Constructor,
				ConstructorBody,
				Undefined
			}
			public _Data(Script s)
			{
				_isInitializated = false;
				_owner = s;
				MethodsImplementations = new Dictionary<string, string>();
				MethodsImplementations.Add(Glance.NameSetting.ScriptOnStartSignature, "");//costili
				MethodsImplementations.Add(Glance.NameSetting.ScriptOnUpdateSignature, "");//KOCTblJlU
				MethodsDeclarations = new Dictionary<Glance.FieldsAccessType, List<string>>();
				_Init();
			}
			public Dictionary<Glance.FieldsAccessType, List<string>> Variables { get;private set; }
			public string OnUpdate { get; private set; }
			public string OnStart { get; private set; }
			public Dictionary<Glance.FieldsAccessType, List<string>> MethodsDeclarations { get; private set; }
			public Dictionary<string, string> MethodsImplementations { get; private set; }
			public List<string> Constructors { get; private set; }
			public string ConstructorBody { get; private set; }

			//private Dictionary<Glance.FieldsAccessType, List<string>> _variables;
			private Script _owner;
			private bool _isInitializated;
			public void _Init()
			{
				if (_isInitializated)
					return;
				//
				var strings = File.ReadAllLines(_owner.FileName);
				bool isInsideMethod = false;
				byte tabs = 0;
				var variables = new Dictionary<Glance.FieldsAccessType, List<string>>();
				List<string> constructors = new List<string>();

				string constructorBody = "";
				string currentMethodSignature = null;
				Region currentRegion = Region.Undefined;
				Glance.FieldsAccessType currentAcessType = Glance.FieldsAccessType.Public;

				foreach (var i in strings)
				{
					var str = i.Trim();
					if (str == "")
						continue;
					if (str.Length > 2)
						if (str.Substring(0, 2) == "//")
							continue;
					if (str.Contains(Glance.NameSetting.ScriptVariablesRegionName))
					{
						currentRegion = Region.Variables;
						currentAcessType = Glance.FieldsAccessType.Public;
						continue;
					}
					if (str.Contains(Glance.NameSetting.ScriptMethodsRegionName))
					{
						currentRegion = Region.Methods;
						currentAcessType = Glance.FieldsAccessType.Public;
						continue;
					}

					if (str.Contains(Glance.NameSetting.ScriptConstructorsRegionName))
					{
						currentRegion = Region.Constructor;
						continue;
					}
					if (str.Contains(Glance.NameSetting.ScriptConstructorBodyRegionName))
					{
						currentRegion = Region.ConstructorBody;
						continue;
					}

					if (currentRegion == Region.Variables)
					{
						if (str.Contains("public:"))
						{
							currentAcessType = Glance.FieldsAccessType.Public;
							continue;
						}
						if (str.Contains("private:"))
						{
							currentAcessType = Glance.FieldsAccessType.Private;
							continue;
						}
						var tmp = new List<string>();
						tmp.Add(str.Replace(";", ""));
						variables.gAddOrMerge(currentAcessType, tmp);
					}
					if (currentRegion == Region.Constructor)
						constructors.AddRange(str.Split(',').gForEach(x => x.Trim()));//add Trimmed array

					if (currentRegion == Region.ConstructorBody)
						constructorBody += str;

					if (currentRegion == Region.Methods)
					{
						if (!isInsideMethod)
						{
							//if we are not inside of method, then we are at signature of this method
							if (Glance.getLastChar(str) != '{')
								throw new Exception("'{' not at the end of line with method signature, signature is: " + str);
							if (str.Contains("public:"))
						{
							currentAcessType = Glance.FieldsAccessType.Public;
							continue;
						}
						if (str.Contains("private:"))
						{
							currentAcessType = Glance.FieldsAccessType.Private;
							continue;
						}
							currentMethodSignature = str.Substring(0, str.Length - 1);//cut '{'
							++tabs;
							isInsideMethod = true;
							if (MethodsImplementations.ContainsKey(currentMethodSignature))
								MethodsImplementations[currentMethodSignature] += "";
							else
								MethodsImplementations.Add(currentMethodSignature, "");
							continue;
						}
						if (isInsideMethod)
						{
							if (str.Contains("{"))
								++tabs;
							if (str.Contains("}"))
								--tabs;
							if (tabs == 0)
							{
								isInsideMethod = false;
								continue;
							}
							if (MethodsImplementations.ContainsKey(currentMethodSignature))
								MethodsImplementations[currentMethodSignature] += str + '\n';
							else
								MethodsImplementations.Add(currentMethodSignature, str + '\n');
						}
					}
				}//foreach
				Constructors = constructors;
				ConstructorBody = constructorBody;
				Variables = variables;
				OnUpdate = MethodsImplementations[Glance.NameSetting.ScriptOnUpdateSignature];
				OnStart = MethodsImplementations[Glance.NameSetting.ScriptOnStartSignature];

				MethodsImplementations.Remove(Glance.NameSetting.ScriptOnStartSignature);
				MethodsImplementations.Remove(Glance.NameSetting.ScriptOnUpdateSignature);
				if (MethodsImplementations.ContainsKey(""))
					MethodsImplementations.Remove("");

				MethodsDeclarations.Add(Glance.FieldsAccessType.Public, MethodsImplementations.Keys.ToList());

				_isInitializated = true;
			}//_Init()
		}//_Data
	}//class Script
}
