using System;
using System.Linq;
using System.Text;
using System.IO;

namespace Glc
{
	public static partial class Glance
	{
		internal static partial class CodeGenerator
		{
			///<summary>FileStream for Write/WriteLn</summary>
			internal static FileStream FStream;
			///<summary>write data + '\n' -> fs considering TabCount</summary>
			internal static void WriteLnIn(string fs, string data)
			{
				if (data == null)
					throw new ArgumentNullException();
				char[] charsToTrim = { '\t', ' ' };
				string[] strings = data.Split('\n');	//return lines
				int tabcount = 0;                       //code block level(if they are inside '{', '}')
				string result = "";
				foreach (var str in strings)				//for every line in data
				{
					if (String.IsNullOrWhiteSpace(str))	//if white space
						continue;
					if (str.Contains('}'))              //if block is ended P.S. it make bugs if '{' and '}' at the same line
						if(!str.Contains('{'))
							--tabcount;

					string finalstr = "";				//string, that will be writed in file
					for (byte i = 0; i < tabcount; ++i)	//add tabs 
						finalstr += '\t';               //for better reading
					finalstr += str.Trim();             //without trash
					result += finalstr + '\n';
					if (str.Contains('{'))              //if block is started P.S. it make bugs if '{' and '}' at the same line
						++tabcount;
				}
				File.WriteAllText(fs, result);
			}
			///<summary>write "StandartIncludes" from settings.gcs -> fs</summary>
			internal static string getStdInc()
			{
				return templates["Set:StandartIncludes:Def"] + '\n';
			}
			///<summary> </summary>
			internal static void writeMainCpp(string fs)
			{
				string Sub_include = "#include \"main.h\"";
				WriteLnIn(fs,
							templates["File:Main:Def"].
								Replace("#Include#", Sub_include).
								Replace("#FirstSceneObjName#", scenes[0].ObjectName).
								Replace("#FirstSceneClassName#", scenes[0].ClassName)
						);
			}
			///<summary>include everything, what must know main.cpp</summary>
			internal static void writeMainH(string fs)
			{
				string result = "";
				foreach (var Scn in scenes)
					result += "#include \"" + Scn.GetDeclarationFileName() + "\"\n";
				WriteLnIn(fs, getStdInc() + result);
			}
			///<summary>write header, and implementation of this object</summary>
			internal static void writePhysicalObject(string declaration, string implementation, PhysicalObject PO)
			{
				WriteLnIn(declaration, getStdInc() +  templates["Class:PhysicalObject:Declaration"]
					.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
					.Replace("#Pos#" , PO.Pos.GetCppCtor())
					.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
					.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
					.Replace("#ComponentsMethods#", PO.GetComponentsMethodsDeclaration())
					.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
					.Replace("#OnStart#", PO.GetComponentsOnStart())
					.Replace("#ClassName#", PO.ClassName)
					.Replace("#SceneName#", PO._scene.ClassName)
					.Replace("#LayerName#", PO._layer.ClassName)
							);
				WriteLnIn(implementation, templates["Class:PhysicalObject:Implementation"]
					.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
					.Replace("#Pos#", PO.Pos.GetCppCtor())
					.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
					.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
					.Replace("#ComponentsMethods#", PO.GetComponentsMethodsImplementation())
					.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
					.Replace("#OnStart#", PO.GetComponentsOnStart())
					.Replace("#ClassName#", PO.ClassName)
					.Replace("#SceneName#", PO._scene.ClassName)
					.Replace("#LayerName#", PO._layer.ClassName)
							);
			}
			internal static void writeRenderableObject(string declaration, string implementation, RenderableObject PO)
			{
				WriteLnIn(declaration, getStdInc() + templates["Class:RenderableObject:Declaration"]
					.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
					.Replace("#Pos#", PO.Pos.GetCppCtor())
					.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
					.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
					.Replace("#ComponentsMethods#", PO.GetComponentsMethodsDeclaration())
					.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
					.Replace("#OnStart#", PO.GetComponentsOnStart())
					.Replace("#ClassName#", PO.ClassName)
					.Replace("#SceneName#", PO._scene.ClassName)
					.Replace("#LayerName#", PO._layer.ClassName)
							);
				WriteLnIn(implementation, getStdInc() +  templates["Class:RenderableObject:Implementation"]
					.Replace("#ComponentsVariables#", PO.GetComponentsVariables())
					.Replace("#Pos#", PO.Pos.x.ToString() + ", " + PO.Pos.y.ToString())
					.Replace("#AdditionalConstructorList#", PO.GetComponentsConstructors())
					.Replace("#ConstructorBody#", PO.GetComponentsConstructorsBody())
					.Replace("#ComponentsMethods#", PO.GetComponentsMethodsImplementation())
					.Replace("#OnUpdate#", PO.GetComponentsOnUpdate())
					.Replace("#OnStart#", PO.GetComponentsOnStart())
					.Replace("#ClassName#", PO.ClassName)
					.Replace("#SceneName#", PO._scene.ClassName)
					.Replace("#LayerName#", PO._layer.ClassName)
					.Replace("#GetCurrentSprite#", PO.GetCurrentSprite())
							);
			}
			internal static void writeLayer(string declaration, string implementation, Layer l)
			{
				{
					string getObjects = "public:\n";
					foreach (var i in l._objects)
					{
						getObjects += "template<>\n" + i.ClassName + " & getObject(){\nreturn " + i.ObjectName + ";\n}\n";
						getObjects += "template<>\nconst " + i.ClassName + " & getObject() const{\nreturn " + i.ObjectName + ";\n}\n";
					}

					string objDeclInclude = "";
					foreach (var i in l._objects)
						objDeclInclude += "#include \"" + i.GetDeclarationFileName() + "\"\n";

					string render = "template<>\ninline void ::gc::Renderer::renderLayer(const " + l.ClassName + " & l){\n";
					foreach (var i in l._objects)
						if (i is RenderableObject)
							render += "this->render(l.getObject<" + i.ClassName + ">().getCurrentSprite(), l.getObject<" + i.ClassName + ">().pos);\n";
					render += '}';

					WriteLnIn(declaration, getStdInc() + templates["Class:Layer:Declaration"]
										.Replace("#SceneName#", l._scene.ClassName)
										.Replace("#ClassName#", l.ClassName)
										.Replace("#ObjectVariables#", l.GetVariables())
										.Replace("#ComponentsVariables#", "")
										.Replace("#ComponentsMethodsDeclaration#", l.GetMethodsDeclaration())
										.Replace("#getObjects#", getObjects)
										.Replace("#ObjectsDeclarationInclude#", objDeclInclude)
										.Replace("#RenderLayer#", render)
						);
				}//declaration
				{
					string ctorList = "";
					foreach (var i in l._objects)
						ctorList += ", " + i.ObjectName + "(scene, *this)";
					string foreach_str = "";
					foreach (var i in l._objects)
						foreach_str += "f(this->getObject<" + i.ClassName + ">());\n";

					WriteLnIn(implementation, templates["Class:Layer:Implementation"]
										.Replace("#SceneName#", l._scene.ClassName)
										.Replace("#ClassName#", l.ClassName)
										.Replace("#AdditionalConstructorList#", ctorList)
										.Replace("#ConstructorBody#", l.GetConstructorBody())
										.Replace("#OnStart#", l.GetOnStart())
										.Replace("#OnUpdate#", l.GetOnUpdate())
										.Replace("#ComponentsMethodsImplementation#", l.GetMethodsImplementation())
										.Replace("#Foreach#", foreach_str)
						);
				}//implementation
			}
			internal static void writeScene(string fs, Scene s)
			{
				WriteLnIn(fs, getStdInc() +  templates["Сlass:Scene:FDef"]
									.Replace("#ClassName#", s.ClassName)
									.Replace("#Layers#", s.GetVariables())
									.Replace("#Ctors#", s.GetConstructors())
									.Replace("#start#", s.GetAllLayersOnStart())
									.Replace("#update#", s.GetAllLayersOnUpdate())
									.Replace("#getLayers#", s.GetGetLayers())
									.Replace("#LayersDeclarationInclude#", s.GetIncludes())
									.Replace("#RenderScene#", s.GetRender())
					);
			}
			///<summary>Code generate</summary>
			internal static void GenerateCode()
			{
				foreach (var scene in scenes)
				{
					foreach (var Lr in  scene._layerList)//layers
					{
						foreach (var SO in Lr._objects)
						{
							if (SO.ImplementationFilePath == null || SO.ImplementationFilePath == "")
							{
								string implFileName = BuildSetting.sourceDir + SO.GetImplementationFileName();
								File.Create(implFileName).Close();
								SO.ImplementationFilePath = implFileName;
							}
							if (SO.DeclarationFilePath == null || SO.DeclarationFilePath == "")
							{
								string declFileName = BuildSetting.sourceDir + SO.GetDeclarationFileName();
								File.Create(declFileName).Close();
								SO.DeclarationFilePath = declFileName;
							}
							SO.GenerateCode();
						}
					}
				}
				{//layers	
					foreach (var i in scenes)
						foreach(var o in i._layerList)
						{
							writeLayer(BuildSetting.sourceDir + o.GetDeclarationFileName(), BuildSetting.sourceDir + o.GetImplementationFileName(), o);
						}
				}
				{//scenes
					foreach (var i in scenes)
					{
						writeScene(BuildSetting.sourceDir + i.GetDeclarationFileName(), i);
					}
				}
				{//mainh
					writeMainH(BuildSetting.sourceDir + "main.h");
				}
				{//maincpp
					writeMainCpp(BuildSetting.sourceDir + "main.cpp");
				}

			}
			static CodeGenerator()
			{
				FStream = null;
			}
		}
	}
}