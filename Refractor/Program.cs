using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Robotics.API;

namespace Refractor
{
	enum AccessModifier
	{
		Public,
		Protected,
		Private
	}

	class Program
	{
		private Dictionary<string, string> fullTypeNames;
		private Dictionary<string, string> includes;

		static void Main(string[] args)
		{
			Program p = new Program();
			p.Run();
		}

		private void Run()
		{
			
			if (!Directory.Exists("output")) Directory.CreateDirectory("output");
			//BuildNamespace("Robotics", "output");
			BuildNamespace("Robotics.API", "output\\API");
			BuildNamespace("System.Net.Sockets", "output\\Sockets");
			BuildNamespace("Robotics.Utilities", "output\\Utilities");
		}

		private void BuildNamespace(string ns, string outputDir)
		{
			Type[] typelist = GetTypesInNamespace(Assembly.LoadFile(CurrentPath + "\\Robotics.dll"), ns);
			if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
			Console.WriteLine("Building types dictionary for " + ns);
			GenerateDictionaries(typelist);
			for (int i = 0; i < typelist.Length; i++)
			{
				if ((!typelist[i].IsClass && !typelist[i].IsInterface) || typelist[i].IsGenericType)
					continue;
				GenerateHpp(typelist[i], outputDir);
				if (typelist[i].IsInterface)
					continue;
				GenerateCpp(typelist[i], outputDir);
				Console.WriteLine();
			}
		}

		private void GenerateHpp(Type type, string outputDir)
		{
			string exName;
			StringBuilder sb = new StringBuilder();
			Console.WriteLine("Generating " + type.Name + ".hpp for " + type.FullName + " class");

			sb.AppendLine("#pragma once");
			exName = GetHeaderExclusionName(type.Name);
			sb.AppendLine("#ifndef " + exName);
			sb.AppendLine("#define " + exName);
			sb.AppendLine();

			GenerateIncludes(sb, type);
			sb.AppendLine();

			GenerateNamespace(sb, type);

			sb.Append("class ");
			sb.Append(GetFullTypeName(type));
			sb.Append(GetCppParents(type));
			sb.AppendLine("{");
			//sb.AppendLine("public:");
			//sb.AppendLine("\t"+type.Name+"(void);");
			//sb.AppendLine("\t~"+type.Name+"(void);");

			GenerateHppConstructors(sb, type);
			GenerateHppGetProperties(sb, type);
			GenerateHppSetProperties(sb, type);
			GenerateHppMethods(sb, type);

			sb.AppendLine("\r\n};\r\n#endif // " + exName);

			if (File.Exists(outputDir + "\\" + type.Name + ".hpp"))
				File.Delete(outputDir + "\\" + type.Name + ".hpp");
			File.WriteAllText(outputDir + "\\" + type.Name + ".hpp", sb.ToString());
		}

		private string GetCppParents(Type type)
		{
			StringBuilder sb;
			int i;
			List<Type> ifaces = new List<Type>(type.GetInterfaces());

			for (i = 0; i < ifaces.Count; ++i)
			{
				if (ifaces[i].IsGenericType || !ifaces[i].FullName.Contains("Robotics."))
					ifaces.RemoveAt(i--);
			}

			if (((type.BaseType == null) || (type.BaseType == typeof(Object))) && (ifaces.Count < 1))
				return String.Empty;

			sb = new StringBuilder(1024);
			sb.AppendLine(" :");
			sb.Append('\t');
			i = 0;
			if ((type.BaseType != null) && (type.BaseType != typeof(Object)))
			{
				++i;
				sb.Append("public ");
				sb.Append(GetFullTypeName(type.BaseType));
				sb.Append(' ');
			}
			while (i < ifaces.Count)
			{
				if (i > 0) sb.Append(',');
				sb.Append(" public ");
				sb.Append(GetFullTypeName(ifaces[i]));
				++i;
			}

			return sb.ToString();
		}

		private void GenerateNamespace(StringBuilder sb, Type type)
		{
			int i;
			string[] parts = type.Namespace.Split('.');
			for (i = 0; i < parts.Length; ++i)
			{
				for (int j = 0; j < i; ++j)
					sb.Append('\t');
				sb.AppendLine("namespace " + parts[i] + "{");
			}
			for (int j = 0; j < i; ++j)
				sb.Append('\t');
			sb.AppendLine("class " + type.Name + ";");
			--i;
			while (i >= 0)
			{
				for (int j = 0; j < i; ++j)
					sb.Append('\t');
				sb.AppendLine("}");
				--i;
			}
			sb.AppendLine();
		}

		private string GetNamespace(Type type)
		{
			return type.Namespace.Replace(".", "::");
		}

		private string GetFullTypeName(Type type)
		{
			return type.Namespace.Replace(".", "::") + "::" + type.Name;
		}

		private void GenerateIncludes(StringBuilder sb, Type type)
		{
			MethodInfo[] methods;
			ParameterInfo[] parameters;
			List<Type> baseTypes;
			List<string> usedTypes = new List<string>(1000);
			List<string> includeFiles = new List<string>(1000);

			baseTypes = new List<Type>(type.GetInterfaces());
			if (type.BaseType != null) baseTypes.Add(type.BaseType);

			foreach (Type t in baseTypes)
			{
				if (!usedTypes.Contains(t.Name))
					usedTypes.Add(t.Name);
			}

			methods = type.GetMethods();
			foreach (MethodInfo mi in methods)
			{
				if ((mi.ReturnType != type) && !usedTypes.Contains(mi.ReturnType.Name))
					usedTypes.Add(mi.ReturnType.Name);
				parameters = mi.GetParameters();
				foreach (ParameterInfo pi in parameters)
				{
					if ((pi.ParameterType != type) && !usedTypes.Contains(pi.ParameterType.Name))
						usedTypes.Add(pi.ParameterType.Name);
				}
			}

			foreach (string sType in usedTypes)
			{
				if (includes.ContainsKey(sType) && !includeFiles.Contains(includes[sType]))
				includeFiles.Add(includes[sType]);
			}
			includeFiles.Sort();
			foreach (string sInclude in includeFiles)
			{
				sb.Append("#include ");
				sb.Append(sInclude.StartsWith("<") ? String.Empty : "\"");
				sb.Append(sInclude);
				sb.AppendLine(sInclude.StartsWith("<") ? String.Empty : "\"");
			}
		}

		private void GenerateCpp(Type type, string outputDir)
		{
			string fullClassName;
			StringBuilder sb = new StringBuilder();
			Console.WriteLine("Generating " + type.Name + ".cpp for " + type.FullName + " class");

			sb.Append("#include \"");
			sb.Append(type.Name);
			sb.AppendLine(".hpp\"");
			sb.AppendLine();

			fullClassName = GetFullTypeName(type) + "::";
			GenerateCppConstructors(sb, type, fullClassName);
			GenerateCppGetProperties(sb, type, fullClassName);
			GenerateCppSetProperties(sb, type, fullClassName);
			GenerateCppMethods(sb, type, fullClassName);

			if (File.Exists(outputDir + "\\" + type.Name + ".cpp"))
				File.Delete(outputDir + "\\" + type.Name + ".cpp");
			File.WriteAllText(outputDir + "\\" + type.Name + ".cpp", sb.ToString());
		}

		private void GenerateDictionaries(Type[] typelist)
		{
			fullTypeNames = new Dictionary<string, string>(2 * typelist.Length);
			includes = new Dictionary<string, string>(2 * typelist.Length);
			foreach (Type t in typelist)
			{
				fullTypeNames.Add(t.Name, GetFullTypeName(t));
				includes.Add(t.Name, t.Name + ".hpp");
			}

			includes.Add("String", "<string>");
			includes.Add("string", "<string>");
			
			includes.Add("Byte[]", "<vector>");
			includes.Add("Int16[]", "<vector>");
			includes.Add("Int32[]", "<vector>");
			includes.Add("Int64[]", "<vector>");
			
			includes.Add("SByte[]", "<vector>");
			includes.Add("SInt16[]", "<vector>");
			includes.Add("SInt32[]", "<vector>");
			includes.Add("SInt64[]", "<vector>");

			includes.Add("Socket", "<boost/bind.hpp>");
			includes.Add("IPAddress", "<boost/asio/ip/address.hpp>");
			includes.Add("EndPoint", "<boost/asio.hpp>");
			includes.Add("IPEndPoint", "<boost/asio.hpp>");
			includes.Add("Thread", "<boost/thread.hpp>");
			includes.Add("ThreadStart", "<boost/thread.hpp>");
			includes.Add("ParameterizedThreadStart", "<boost/thread.hpp>");
		}

		private void GenerateHppConstructors(StringBuilder sb, Type type)
		{
			ConstructorInfo[] constructors;
			List<ConstructorInfo> publicConstructors;
			List<ConstructorInfo> protectedConstructors;
			List<ConstructorInfo> privateConstructors;

			constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			publicConstructors = new List<ConstructorInfo>(constructors.Length);
			protectedConstructors = new List<ConstructorInfo>(constructors.Length);
			privateConstructors = new List<ConstructorInfo>(constructors.Length);

			if (constructors.Length < 1)
				return;

			foreach (ConstructorInfo ci in constructors)
			{
				if (ci.IsPrivate)
					privateConstructors.Add(ci);
				else if (ci.IsFamily)
					protectedConstructors.Add(ci);
				else
					publicConstructors.Add(ci);
			}

			sb.AppendLine("/************************************************************************/");
			sb.AppendLine("/* Constructors                                                         */");
			sb.AppendLine("/************************************************************************/");

			if (publicConstructors.Count > 0)
			{
				sb.AppendLine("public:");
				foreach (ConstructorInfo ci in publicConstructors)
					GenerateHppConstructor(sb, type, ci);
			}
			if (protectedConstructors.Count > 0)
			{
				sb.AppendLine("protected:");
				foreach (ConstructorInfo ci in protectedConstructors)
					GenerateHppConstructor(sb, type, ci);
			}
			if (privateConstructors.Count > 0)
			{
				sb.AppendLine("private:");
				foreach (ConstructorInfo ci in privateConstructors)
					GenerateHppConstructor(sb, type, ci);
			}
		}

		private void GenerateCppConstructors(StringBuilder sb, Type type, string fullClassName)
		{
			ConstructorInfo[] constructors;
			List<ConstructorInfo> publicConstructors;
			List<ConstructorInfo> protectedConstructors;
			List<ConstructorInfo> privateConstructors;

			constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			publicConstructors = new List<ConstructorInfo>(constructors.Length);
			protectedConstructors = new List<ConstructorInfo>(constructors.Length);
			privateConstructors = new List<ConstructorInfo>(constructors.Length);

			if (constructors.Length < 1)
				return;

			foreach (ConstructorInfo ci in constructors)
			{
				if (ci.IsPrivate)
					privateConstructors.Add(ci);
				else if (ci.IsFamily)
					protectedConstructors.Add(ci);
				else
					publicConstructors.Add(ci);
			}

			sb.AppendLine("/************************************************************************/");
			sb.AppendLine("/* Constructors                                                         */");
			sb.AppendLine("/************************************************************************/");

			if (publicConstructors.Count > 0)
			{
				foreach (ConstructorInfo ci in publicConstructors)
					GenerateCppConstructor(sb, type, ci, fullClassName);
			}
			if (protectedConstructors.Count > 0)
			{
				foreach (ConstructorInfo ci in protectedConstructors)
					GenerateCppConstructor(sb, type, ci, fullClassName);
			}
			if (privateConstructors.Count > 0)
			{
				foreach (ConstructorInfo ci in privateConstructors)
					GenerateCppConstructor(sb, type, ci, fullClassName);
			}
		}

		private void GenerateHppMethods(StringBuilder sb, Type type)
		{
			MethodInfo[] methods;
			List<MethodInfo> publicMethods;
			List<MethodInfo> protectedMethods;
			List<MethodInfo> privateMethods;

			methods = type.GetMethods();
			publicMethods = new List<MethodInfo>(methods.Length);
			protectedMethods = new List<MethodInfo>(methods.Length);
			privateMethods = new List<MethodInfo>(methods.Length);

			if (methods.Length < 1)
				return;

			foreach (MethodInfo mi in methods)
			{
				if (mi.IsPrivate)
					privateMethods.Add(mi);
				else if (mi.IsFamily)
					protectedMethods.Add(mi);
				else
					publicMethods.Add(mi);
			}

			sb.AppendLine("/************************************************************************/");
			sb.AppendLine("/* Methods                                                              */");
			sb.AppendLine("/************************************************************************/");

			if (publicMethods.Count > 0)
			{
				sb.AppendLine("public:");
				foreach (MethodInfo mi in publicMethods)
					GenerateHppMethod(sb, mi);
			}
			if (protectedMethods.Count > 0)
			{
				sb.AppendLine("protected:");
				foreach (MethodInfo mi in protectedMethods)
					GenerateHppMethod(sb, mi);
			}
			if (privateMethods.Count > 0)
			{
				sb.AppendLine("private:");
				foreach (MethodInfo mi in privateMethods)
					GenerateHppMethod(sb, mi);
			}
		}

		private void GenerateCppMethods(StringBuilder sb, Type type, string fullClassName)
		{
			MethodInfo[] methods;
			List<MethodInfo> publicMethods;
			List<MethodInfo> protectedMethods;
			List<MethodInfo> privateMethods;

			methods = type.GetMethods();
			publicMethods = new List<MethodInfo>(methods.Length);
			protectedMethods = new List<MethodInfo>(methods.Length);
			privateMethods = new List<MethodInfo>(methods.Length);

			if (methods.Length < 1)
				return;

			foreach (MethodInfo mi in methods)
			{
				if (mi.IsPrivate)
					privateMethods.Add(mi);
				else if (mi.IsFamily)
					protectedMethods.Add(mi);
				else
					publicMethods.Add(mi);
			}

			sb.AppendLine("/************************************************************************/");
			sb.AppendLine("/* Methods                                                              */");
			sb.AppendLine("/************************************************************************/");

			if (publicMethods.Count > 0)
			{
				foreach (MethodInfo mi in publicMethods)
					GenerateCppMethod(sb, mi, fullClassName);
			}
			if (protectedMethods.Count > 0)
			{
				foreach (MethodInfo mi in protectedMethods)
					GenerateCppMethod(sb, mi, fullClassName);
			}
			if (privateMethods.Count > 0)
			{
				foreach (MethodInfo mi in privateMethods)
					GenerateCppMethod(sb, mi, fullClassName);
			}
		}

		private void GenerateHppGetProperties(StringBuilder sb, Type type)
		{
			PropertyInfo[] properties;
			List<PropertyInfo> publicProperties;
			List<PropertyInfo> protectedProperties;
			List<PropertyInfo> privateProperties;

			properties = type.GetProperties();
			publicProperties = new List<PropertyInfo>(properties.Length);
			protectedProperties = new List<PropertyInfo>(properties.Length);
			privateProperties = new List<PropertyInfo>(properties.Length);

			if (properties.Length < 1)
				return;

			foreach (PropertyInfo pi in properties)
			{
				MethodInfo mi;
				if ((mi = pi.GetGetMethod()) == null)
					continue;

				if (mi.IsPrivate)
					privateProperties.Add(pi);
				else if (mi.IsFamily)
					protectedProperties.Add(pi);
				else
					publicProperties.Add(pi);
			}

			sb.AppendLine("/************************************************************************/");
			sb.AppendLine("/* Getters: get/read properties                                         */");
			sb.AppendLine("/************************************************************************/");

			if (publicProperties.Count > 0)
			{
				sb.AppendLine("public:");
				foreach (PropertyInfo pi in publicProperties)
					GenerateHppGetProperty(sb, pi);
			}
			if (protectedProperties.Count > 0)
			{
				sb.AppendLine("protected:");
				foreach (PropertyInfo pi in protectedProperties)
					GenerateHppGetProperty(sb, pi);
			}
			if (privateProperties.Count > 0)
			{
				sb.AppendLine("private:");
				foreach (PropertyInfo pi in privateProperties)
					GenerateHppGetProperty(sb, pi);
			}
		}

		private void GenerateCppGetProperties(StringBuilder sb, Type type, string fullClassName)
		{
			PropertyInfo[] properties;
			List<PropertyInfo> publicProperties;
			List<PropertyInfo> protectedProperties;
			List<PropertyInfo> privateProperties;

			properties = type.GetProperties();
			publicProperties = new List<PropertyInfo>(properties.Length);
			protectedProperties = new List<PropertyInfo>(properties.Length);
			privateProperties = new List<PropertyInfo>(properties.Length);

			if (properties.Length < 1)
				return;

			foreach (PropertyInfo pi in properties)
			{
				MethodInfo mi;
				if ((mi = pi.GetGetMethod()) == null)
					continue;

				if (mi.IsPrivate)
					privateProperties.Add(pi);
				else if (mi.IsFamily)
					protectedProperties.Add(pi);
				else
					publicProperties.Add(pi);
			}

			sb.AppendLine("/************************************************************************/");
			sb.AppendLine("/* Getters: get/read properties                                         */");
			sb.AppendLine("/************************************************************************/");

			if (publicProperties.Count > 0)
			{
				foreach (PropertyInfo pi in publicProperties)
					GenerateCppGetProperty(sb, pi, fullClassName);
			}
			if (protectedProperties.Count > 0)
			{
				foreach (PropertyInfo pi in protectedProperties)
					GenerateCppGetProperty(sb, pi, fullClassName);
			}
			if (privateProperties.Count > 0)
			{
				foreach (PropertyInfo pi in privateProperties)
					GenerateCppGetProperty(sb, pi, fullClassName);
			}
		}

		private void GenerateHppSetProperties(StringBuilder sb, Type type)
		{
			PropertyInfo[] properties;
			List<PropertyInfo> publicProperties;
			List<PropertyInfo> protectedProperties;
			List<PropertyInfo> privateProperties;

			properties = type.GetProperties();
			publicProperties = new List<PropertyInfo>(properties.Length);
			protectedProperties = new List<PropertyInfo>(properties.Length);
			privateProperties = new List<PropertyInfo>(properties.Length);

			if (properties.Length < 1)
				return;

			foreach (PropertyInfo pi in properties)
			{
				MethodInfo mi;
				if (!pi.CanWrite)
					continue;
				if((mi = pi.GetSetMethod()) == null)
					continue;
				if (mi.IsPrivate)
					privateProperties.Add(pi);
				else if (mi.IsFamily)
					protectedProperties.Add(pi);
				else
					publicProperties.Add(pi);
			}

			sb.AppendLine("/************************************************************************/");
			sb.AppendLine("/* Setters: set/write properties                                        */");
			sb.AppendLine("/************************************************************************/");

			if (publicProperties.Count > 0)
			{
				sb.AppendLine("public:");
				foreach (PropertyInfo pi in publicProperties)
					GenerateHppSetProperty(sb, pi);
			}
			if (protectedProperties.Count > 0)
			{
				sb.AppendLine("protected:");
				foreach (PropertyInfo pi in protectedProperties)
					GenerateHppSetProperty(sb, pi);
			}
			if (privateProperties.Count > 0)
			{
				sb.AppendLine("private:");
				foreach (PropertyInfo pi in privateProperties)
					GenerateHppSetProperty(sb, pi);
			}
		}

		private void GenerateCppSetProperties(StringBuilder sb, Type type, string fullClassName)
		{
			PropertyInfo[] properties;
			List<PropertyInfo> publicProperties;
			List<PropertyInfo> protectedProperties;
			List<PropertyInfo> privateProperties;

			properties = type.GetProperties();
			publicProperties = new List<PropertyInfo>(properties.Length);
			protectedProperties = new List<PropertyInfo>(properties.Length);
			privateProperties = new List<PropertyInfo>(properties.Length);

			if (properties.Length < 1)
				return;

			foreach (PropertyInfo pi in properties)
			{
				MethodInfo mi;
				if (!pi.CanWrite)
					continue;
				if ((mi = pi.GetSetMethod()) == null)
					continue;
				if (mi.IsPrivate)
					privateProperties.Add(pi);
				else if (mi.IsFamily)
					protectedProperties.Add(pi);
				else
					publicProperties.Add(pi);
			}

			sb.AppendLine("/************************************************************************/");
			sb.AppendLine("/* Setters: set/write properties                                        */");
			sb.AppendLine("/************************************************************************/");

			if (publicProperties.Count > 0)
			{
				foreach (PropertyInfo pi in publicProperties)
					GenerateCppSetProperty(sb, pi, fullClassName);
			}
			if (protectedProperties.Count > 0)
			{
				foreach (PropertyInfo pi in protectedProperties)
					GenerateCppSetProperty(sb, pi, fullClassName);
			}
			if (privateProperties.Count > 0)
			{
				foreach (PropertyInfo pi in privateProperties)
					GenerateCppSetProperty(sb, pi, fullClassName);
			}
		}

		private void GenerateHppConstructor(StringBuilder sb, Type type, ConstructorInfo constructor)
		{
			sb.Append('\t');
			GenerateSignature(sb, null, type.Name, constructor.GetParameters(), constructor.IsAbstract, constructor.IsVirtual, false);
			if (constructor.IsAbstract)
				sb.Append(" = 0");
			sb.AppendLine(";");
			sb.AppendLine();
		}

		private void GenerateCppConstructor(StringBuilder sb, Type type, ConstructorInfo constructor, string fullClassName)
		{
			GenerateSignature(sb, null, fullClassName + type.Name, constructor.GetParameters(), constructor.IsAbstract, constructor.IsVirtual, false);
			sb.AppendLine();
			sb.AppendLine("{");
			sb.AppendLine("}");
			sb.AppendLine();
		}

		private void GenerateHppGetProperty(StringBuilder sb, PropertyInfo property)
		{
			if (!property.CanRead)
				return;

			string methodName;
			MethodInfo mi = property.GetGetMethod();
			methodName = property.CanWrite ?
				"get" + property.Name :
				property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);
			sb.Append('\t');
			GenerateSignature(sb, property.PropertyType, methodName, null, mi.IsAbstract, mi.IsVirtual, mi.IsStatic);
			if (mi.IsAbstract)
				sb.Append(" = 0");
			sb.AppendLine(";");
			sb.AppendLine();
			/*
			string typeName = ConvertTypeName(property.PropertyType.Name);
			
			sb.Append(padding);
			if (mi.IsAbstract || mi.IsVirtual)
				sb.Append("virtual ");
			sb.Append(typeName);
			if (property.PropertyType.IsArray)
				sb.Append('*');
			else if (!typeName.EndsWith("*") && (property.PropertyType.IsInterface || property.PropertyType.IsClass))
				sb.Append('&');
			sb.Append(' ');
			sb.Append(prefix);
			if (property.CanWrite)
			{
				sb.Append("get");
				sb.Append(property.Name);
			}
			else
			{
				sb.Append(property.Name.Substring(0, 1).ToLower());
				sb.Append(property.Name.Substring(1));
			}
			sb.Append("()");
			if (mi.IsAbstract)
				sb.Append(" = 0");
			sb.AppendLine(";");
			sb.AppendLine();
			*/
		}

		private void GenerateCppGetProperty(StringBuilder sb, PropertyInfo property, string fullClassName)
		{
			if (!property.CanRead)
				return;

			string methodName = fullClassName;
			MethodInfo mi = property.GetGetMethod();
			if (mi.IsAbstract)
				return;
			methodName+= property.CanWrite ?
				"get" + property.Name :
				property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);
			GenerateSignature(sb, property.PropertyType, methodName, null, mi.IsAbstract, mi.IsVirtual, mi.IsStatic);
			sb.AppendLine();
			sb.AppendLine("{");
			sb.AppendLine("}");
			sb.AppendLine();
		}

		private void GenerateHppSetProperty(StringBuilder sb, PropertyInfo property)
		{
			if (!property.CanWrite)
				return;

			string methodName;
			MethodInfo mi = property.GetSetMethod();
			methodName = property.CanRead ?
				"set" + property.Name :
				property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);
			sb.Append('\t');
			GenerateSignature(sb, property.PropertyType, methodName, null, mi.IsAbstract, mi.IsVirtual, mi.IsStatic);
			if (mi.IsAbstract)
				sb.Append(" = 0");
			sb.AppendLine(";");
			sb.AppendLine();

			/*
			string typeName = ConvertTypeName(property.PropertyType.Name);
			MethodInfo mi = property.GetSetMethod();
			sb.Append(prefix);
			if(mi.IsAbstract || mi.IsVirtual)
				sb.Append("virtual ");
			sb.Append("void ");
			sb.Append(prefix);
			sb.Append("set");
			sb.Append(property.Name);
			sb.Append('(');
			sb.Append(typeName);
			if (property.PropertyType.IsArray)
				sb.Append('*');
			else if (!typeName.EndsWith("*") && (property.PropertyType.IsInterface || property.PropertyType.IsClass))
				sb.Append('&');
			sb.Append(" value)");
			if(mi.IsAbstract)
				sb.Append(" = 0");
			sb.AppendLine(";");
			sb.AppendLine();
			*/
		}

		private void GenerateCppSetProperty(StringBuilder sb, PropertyInfo property, string fullClassName)
		{
			if (!property.CanWrite)
				return;

			string methodName = fullClassName;
			MethodInfo mi = property.GetSetMethod();
			if (mi.IsAbstract)
				return;
			methodName+= property.CanRead ?
				"set" + property.Name :
				property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);
			GenerateSignature(sb, property.PropertyType, methodName, null, mi.IsAbstract, mi.IsVirtual, mi.IsStatic);
			sb.AppendLine();
			sb.AppendLine("{");
			sb.AppendLine("}");
			sb.AppendLine();
		}

		private void GenerateHppMethod(StringBuilder sb, MethodInfo method)
		{
			string methodName;

			if (!method.IsPublic && !method.IsFamily)
				return;
			if (SkipMethod(method))
				return;

			sb.Append('\t');
			methodName = method.Name.Substring(0, 1).ToLower() + method.Name.Substring(1);
			GenerateSignature(sb, method.ReturnType, methodName, method.GetParameters(), method.IsAbstract, method.IsVirtual, method.IsStatic);

			if(method.IsAbstract)
				sb.Append(" = 0");
			sb.AppendLine(";");
			sb.AppendLine();
		}

		private void GenerateCppMethod(StringBuilder sb, MethodInfo method, string fullClassName)
		{
			string methodName;

			if (!method.IsPublic && !method.IsFamily)
				return;
			if (SkipMethod(method) || method.IsAbstract)
				return;

			methodName = fullClassName + method.Name.Substring(0, 1).ToLower() + method.Name.Substring(1);
			GenerateSignature(sb, method.ReturnType, methodName, method.GetParameters(), false, false, false);
			sb.AppendLine();
			sb.AppendLine("{");
			sb.AppendLine("}");
			sb.AppendLine();
		}

		private void GenerateSignature(StringBuilder sb, Type returnType, string methodName, ParameterInfo[] parameters, bool isAbstract, bool isVirtual, bool isStatic)
		{
			int i;

			if (isAbstract || isVirtual)
				sb.Append("virtual ");
			if (isStatic)
				sb.Append("static ");
			if (returnType != null)
			{
				sb.Append(ConvertTypeName(returnType.Name));
				if (returnType.IsArray)
					sb.Append('*');
				else if (returnType.IsClass || returnType.IsInterface)
					sb.Append('&');
				sb.Append(' ');
			}

			sb.Append(methodName);
			sb.Append('(');

			if (parameters != null)
			{
				if (parameters.Length > 0)
					GenerateParameter(sb, parameters[0]);
				for (i = 1; i < parameters.Length; ++i)
				{
					sb.Append(", ");
					GenerateParameter(sb, parameters[i]);
				}
			}

			sb.Append(')');
		}

		private void GenerateParameter(StringBuilder sb, ParameterInfo pi)
		{
			string typeName = ConvertTypeName(pi.ParameterType.Name.Replace("&", String.Empty));

			if (pi.ParameterType.IsArray)
			{
				if(!pi.IsOut)
					sb.Append("const ");
				sb.Append(typeName);
				sb.Append('*');
				if(pi.IsOut)
					sb.Append('*');
			}
			else if (pi.ParameterType.IsClass || pi.ParameterType.IsInterface)
			{
				if (pi.IsIn)
					sb.Append("const ");

				sb.Append(typeName);
				if (!typeName.EndsWith("*"))
					sb.Append((pi.ParameterType.IsByRef || pi.IsOut) ? '*' : '&');
			}
			else
			{
				sb.Append(typeName);
				if (!typeName.EndsWith("*") && (pi.ParameterType.IsByRef || pi.IsOut))
					sb.Append('*');
			}
			sb.Append(' ');
			sb.Append(pi.Name);
		}

		private string ConvertTypeName(string typeName)
		{
			if (typeName.EndsWith("[]"))
				typeName = typeName.Substring(0, typeName.Length - 2);
			switch (typeName.ToLower())
			{
				case "boolean":
					return "bool";

				case "byte":
					return "char";

				case "byte[]":
					return "std::vector<char>";

				case "char":
					return "char";

				case "int16":
					return "short";

				case "int32":
					return "int";

				case "int64":
					return "long";

				case "uint16":
					return "unsigned short";

				case "uint32":
					return "unsigned int";

				case "uint64":
					return "unsigned long";

				case "string":
					return "std::string";

				case "float":
					return "float";

				case "double":
					return "double";

				case "void":
					return "void";

				case "object":
					return "void*";

				case "ipaddress":
					return "boost::asio::ip::address";

				case "ipendpoint":
					return "boost::asio::ip::tcp::endpoint";

				case "datetime":
					return "boost::posix_time::ptime";

				//case "timespan":
				//	return "";

				case "thread":
					return "boost::thread";

				default :
					if(this.fullTypeNames.ContainsKey(typeName))
						return fullTypeNames[typeName];
					return typeName;
			}
		}

		private string GetHeaderExclusionName(string typeName)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append('_');
			//sb.Append(typeName[0].ToString().ToUpper());
			for (int i = 0; i < typeName.Length; ++i)
			{
				int offset = 0;
				if ((typeName[i] >= 'a') && (typeName[i] <= 'z'))
					offset = 'a' - 'A';
				if ((typeName[i] >= 'A') && (typeName[i] <= 'Z'))
				{
					sb.Append('_');
				}
				sb.Append((char)(typeName[i] - offset));
			}
			sb.Append("_H__");
			return sb.ToString();
		}

		private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
		{
			Type[] types = assembly.GetTypes();
			return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
		}

		private bool SkipMethod(MethodInfo method)
		{
			return method.Name.StartsWith("get_") || method.Name.StartsWith("set_") ||
				method.Name.StartsWith("add_") || method.Name.StartsWith("remove_") || (method.Name == "GetType")
				|| (method.Name == "ToString") || (method.Name == "Equals") || (method.Name == "GetHashCode");
		}

		private string CurrentPath
		{
			get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); } 
		}
		
	}

	static class Extensions
	{
		public static void AppendLine(this StringBuilder sb)
		{
			sb.Append("\r\n");
		}

		public static void AppendLine(this StringBuilder sb, string text)
		{
			sb.Append(text);
			sb.Append("\r\n");
		}

		public static void AppendSubLine(this StringBuilder sb, string text)
		{
			sb.Append('\t');
			sb.Append(text);
			sb.Append("\r\n");
		}
	}
}
