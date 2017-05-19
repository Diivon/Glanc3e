using System;
using System.Collections.Generic;
using System.IO;

namespace Glc.Component
{
		public abstract class Component
		{
			/// <summary>
            /// { {Public, {"int a", "float b"}}, {Private, {"gc::Vec2 dist", "float sec"}} }
            /// </summary>
			internal abstract Dictionary<Glance.FieldsAccessType, List<string>> GetCppVariables();
            /// <summary>
            /// { {Public, "void a()"}, {Private, "int get(int)"} }
            /// </summary>
			internal abstract Dictionary<Glance.FieldsAccessType, List<string>> GetCppMethodsDeclaration();
			/// <summary>
            /// { {"void a()", "gc::debug.log(distance);"}, {"int get(int)", "return 2 * 229;"} }
            /// </summary>
			internal abstract Dictionary<string, string> GetCppMethodsImplementation();
			/// <summary>
            /// {"a()", "b(a)", "c(b, a)"}
            /// </summary>
			internal abstract List<string> GetCppConstructor();
			/// <summary>
            /// "gc::debug.log("I was born!");"
            /// </summary>
			internal abstract string GetCppConstructorBody();
			/// <summary>
            /// "gc::debug.log("I was updated!");"
            /// </summary>
			internal abstract string GetCppOnUpdate();
            /// <summary>
            /// "gc::debug.log("I was started!");"
            /// </summary>
            internal abstract string GetCppOnStart();
		}
}//ns Glc