using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Robotics.API.PrimitiveSharedVariables
{
	/// <summary>
	/// Builds shared variable classes from other classes, structures and IDL files
	/// </summary>
	public class SharedVariableBuilder
	{

		private static readonly Dictionary<string, bool> supportedTypes;

		static SharedVariableBuilder()
		{
			supportedTypes = new Dictionary<string, bool>();
			String[] types = {
							   "System.SByte", "System.Int16", "System.Int32", "System.Int64",
							"System.Byte", "System.UInt16", "System.UInt32", "System.UInt64",
							"System.Single", "System.Double", "System.Boolean", "System.String"
						   };
			foreach (string s in types)
			{
				supportedTypes.Add(s, true);
				supportedTypes.Add(s+"[]", true);
			}
		}

		#region BuildClass (Object)

		/// <summary>
		/// Generates a SharedVariable derived class wrapper for the type of the provided object with serialization and deserialization methods.
		/// </summary>
		/// <param name="o">A non-null object of the type to be used by the SharedVariable wrapper</param>
		/// <param name="filePath">The path of the output cs file with the code of the class</param>
		public static void BuildClass(Object o, string filePath)
		{
			BuildClass(o, filePath);
		}

		/// <summary>
		/// Generates a SharedVariable derived class wrapper for the type of the provided object with serialization and deserialization methods.
		/// </summary>
		/// <param name="o">A non-null object of the type to be used by the SharedVariable wrapper</param>
		/// <param name="className">The name of the class to generate</param>
		/// <param name="filePath">The path of the output cs file with the code of the class</param>
		public static void BuildClass(Object o, string className, string filePath)
		{
			BuildClass(o, className, filePath);
		}

		/// <summary>
		/// Generates a SharedVariable derived class wrapper for the type of the provided object with serialization and deserialization methods.
		/// </summary>
		/// <param name="o">A non-null object of the type to be used by the SharedVariable wrapper</param>
		/// <param name="ns">The namespace which will contain the generated class</param>
		/// <param name="className">The name of the class to generate</param>
		/// <param name="filePath">The path of the output cs file with the code of the class</param>
		public static void BuildClass(Object o, string ns, string className, string filePath)
		{
			if (o == null)
				throw new ArgumentNullException("Parameter o cannot be null");
			BuildClass(o.GetType(), ns, className, filePath);
		}

		#endregion

		#region BuildClass (Type)

		/// <summary>
		/// Generates a SharedVariable derived class wrapper for the specified serialization and deserialization methods.
		/// </summary>
		/// <param name="t">The type to be used by the SharedVariable wrapper</param>
		/// <param name="filePath">The path of the output cs file with the code of the class</param>
		public static void BuildClass(Type t, string filePath){
			if (t == null)
				throw new ArgumentNullException("Parameter t cannot be null");
			Type eType = t;
			while (eType.IsArray)
				eType = eType.GetElementType();
			string className = eType.Name;
			if(t.IsArray)
				className+="Array";
			className+="SharedVariable";
			BuildClass(t, t.Namespace, className, filePath);
		}

		/// <summary>
		/// Generates a SharedVariable derived class wrapper for the specified serialization and deserialization methods.
		/// </summary>
		/// <param name="t">The type to be used by the SharedVariable wrapper</param>
		/// <param name="className">The name of the class to generate</param>
		/// <param name="filePath">The path of the output cs file with the code of the class</param>
		public static void BuildClass(Type t, string className, string filePath) {
			if (t == null)
				throw new ArgumentNullException("Parameter t cannot be null");
			BuildClass(t, t.Namespace, className, filePath);
		}

		/// <summary>
		/// Generates a SharedVariable derived class wrapper for the specified serialization and deserialization methods.
		/// </summary>
		/// <param name="type">The type to be used by the SharedVariable wrapper</param>
		/// <param name="ns">The namespace which will contain the generated class</param>
		/// <param name="className">The name of the class to generate</param>
		/// <param name="filePath">The path of the output cs file with the code of the class</param>
		public static void BuildClass(Type type, string ns, string className, string filePath)
		{
			if (type == null)
				throw new ArgumentNullException("Parameter t cannot be null");

			List<FieldInfo> fields = new List<FieldInfo>();
			List<PropertyInfo> properties = new List<PropertyInfo>();
			if (type.GetNestedTypes(BindingFlags.Public | BindingFlags.Instance).Length > 0)
				throw new Exception("Shared variables don't support nested typing");
			if (type.IsGenericType)
				throw new Exception("Shared variables don't support generic types");
			if (!HasParameterlessConstructor(type))
				throw new Exception("The data type must have a public parameterless constructor");
			GetRWProperties(type, properties);
			GetRWFields(type, fields);
			if((fields.Count < 1) && (properties.Count < 1))
				throw new Exception("The data type must have at least one public field or property");
			//ResolveDependencies(writableMembers, filePath);

			// Save memory for create the class. 100kB shall suffice.
			int tabs = 0;
			StringBuilder sb = new StringBuilder(102400);

			AddUsingRegion(sb);
			AddNamespaceRegion(sb, ns, ref tabs);
			AddClassName(sb, className, type, ref tabs);
			AddConstructorsRegion(sb, className, type, ref tabs);
			AddPropertiesRegion(sb, type, ref tabs);
			AddMethodsRegion(sb, type, fields, properties, ref tabs);
			AddClosingBraces(sb, ref tabs);
			File.WriteAllText(filePath, sb.ToString());
		}

		#endregion

		#region CS file generation methods

		private static void AddUsingRegion(StringBuilder sb)
		{
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using System.Text;");
			sb.AppendLine("using Robotics.Utilities;");
			sb.AppendLine("using Robotics.API;");
			sb.AppendLine("using Robotics.API.PrimitiveSharedVariables;");
			sb.AppendLine();
		}

		private static void AddNamespaceRegion(StringBuilder sb, string ns, ref int tabs)
		{
			sb.Append("namespace ");
			sb.AppendLine(ns);
			sb.AppendLine("{");
			++tabs;
		}

		private static void AddClassName(StringBuilder sb, string className, Type type, ref int tabs)
		{
			AppendTabs(sb, tabs);
			sb.Append("public class ");
			sb.Append(className);
			sb.Append(" : SharedVariable<");
			sb.Append(type);
			sb.AppendLine(">");
			AppendTabs(sb, tabs);
			sb.AppendLine("{");
			++tabs;
		}

		private static void AddConstructorsRegion(StringBuilder sb, string className, Type type, ref int tabs)
		{

			TabbedAppendLine(sb, tabs, "#region Constructors");
			sb.AppendLine();
			AddConstructor(sb, className, type, tabs, false, false, false);
			sb.AppendLine();
			AddConstructor(sb, className, type, tabs, false, false, true);
			sb.AppendLine();
			AddConstructor(sb, className, type, tabs, true, true, false);
			sb.AppendLine();
			AddConstructor(sb, className, type, tabs, true, true, true);
			sb.AppendLine();
			TabbedAppendLine(sb, tabs, "#endregion");
			sb.AppendLine();
		}

		private static void AddPropertiesRegion(StringBuilder sb, Type type, ref int tabs)
		{
			TabbedAppendLine(sb, tabs, "#region Properties");
			sb.AppendLine();
			AddPropertyIsArray(sb, type, tabs);
			sb.AppendLine();
			AddPropertyLength(sb, type, tabs);
			sb.AppendLine();
			AddPropertyTypeName(sb, type, tabs);
			sb.AppendLine();
			TabbedAppendLine(sb, tabs, "#endregion");
			sb.AppendLine();
		}

		private static void AddMethodsRegion(StringBuilder sb, Type type, List<FieldInfo> fields, List<PropertyInfo> properties, ref int tabs)
		{
			AddDeserializationMethodsRegion(sb, type, fields, properties, tabs);
			AddSerializationMethodsRegion(sb, type, fields, properties, tabs);
		}

		private static void AddDeserializationMethodsRegion(StringBuilder sb, Type type, List<FieldInfo> fields, List<PropertyInfo> properties, int tabs)
		{
			Dictionary<string, bool> deserializableTypes = new Dictionary<string, bool>(supportedTypes);

			TabbedAppendLine(sb, tabs, "#region Deserialization Methods");
			sb.AppendLine();

			AddDeserializeMethod(sb, type, fields, properties, tabs);
			sb.AppendLine();
			Type eType = type;
			while (eType.IsArray)
			{
				eType = eType.GetElementType();
				GenerateDeserializationMethodForType(sb, tabs, eType, deserializableTypes);
			}
			foreach (FieldInfo fi in fields)
				GenerateDeserializationMethodForType(sb, tabs, fi.FieldType, deserializableTypes);
			foreach (PropertyInfo pi in properties)
				GenerateDeserializationMethodForType(sb, tabs, pi.PropertyType, deserializableTypes);

			TabbedAppendLine(sb, tabs, "#endregion");
			sb.AppendLine();
		}

		private static void AddSerializationMethodsRegion(StringBuilder sb, Type type, List<FieldInfo> fields, List<PropertyInfo> properties, int tabs)
		{
			Dictionary<string, bool> serializableTypes = new Dictionary<string, bool>(supportedTypes);

			TabbedAppendLine(sb, tabs, "#region Serialization Methods");
			sb.AppendLine();
			
			AddSerializeMethod(sb, type, fields, properties, tabs);
			sb.AppendLine();
			Type eType = type;
			while (eType.IsArray)
			{
				eType = eType.GetElementType();
				GenerateSerializationMethodForType(sb, tabs, eType, serializableTypes);
			}
			foreach (FieldInfo fi in fields)
				GenerateSerializationMethodForType(sb, tabs, fi.FieldType, serializableTypes);
			foreach (PropertyInfo pi in properties)
				GenerateSerializationMethodForType(sb, tabs, pi.PropertyType, serializableTypes);

			TabbedAppendLine(sb, tabs, "#endregion");
			sb.AppendLine();
		}

		private static void AddDeserializeMethod(StringBuilder sb, Type type, List<FieldInfo> fields, List<PropertyInfo> properties, int tabs)
		{
			AddDeserializeMethodDocumentation(sb, tabs, type);
			AddDeserializeMethodSignature(sb, tabs, type);
			TabbedAppendLine(sb, tabs, "{");
			if (type.IsArray)
				AddDeserializeArrayMethodBody(sb, tabs + 1, type, fields, properties);
			else
				AddDeserializeMethodBody(sb, tabs + 1, type, fields, properties);
			TabbedAppendLine(sb, tabs, "}");
		}

		private static void AddSerializeMethod(StringBuilder sb, Type type, List<FieldInfo> fields, List<PropertyInfo> properties, int tabs)
		{
			AddSerializeMethodDocumentation(sb, tabs, type);
			AddSerializeMethodSignature(sb, tabs, type);
			TabbedAppendLine(sb, tabs, "{");
			if (type.IsArray)
				AddSerializeArrayMethodBody(sb, tabs + 1, type, fields, properties);
			else
				AddSerializeMethodBody(sb, tabs + 1, type, fields, properties);
			TabbedAppendLine(sb, tabs, "}");
		}

		private static void AddClosingBraces(StringBuilder sb, ref int tabs)
		{
			do
			{
				AppendTabs(sb, --tabs);
				sb.AppendLine("}");
			} while (tabs > 0);
		}

		#endregion

		#region Constructor generation methods

		private static void AddConstructor(StringBuilder sb, string className, Type type, int tabs, bool cmdManParam, bool initializeParam, bool valueParam)
		{
			AddConstructorDocumentation(sb, className, tabs, cmdManParam, initializeParam, valueParam);
			AddConstructorSignature(sb, className, type, tabs, cmdManParam, initializeParam, valueParam);
			AddConstructorCaller(sb, type, tabs, cmdManParam, initializeParam, valueParam);
		}

		private static void AddConstructorDocumentation(StringBuilder sb, string className, int tabs, bool cmdManParam, bool initializeParam, bool valueParam)
		{
			AddSummary(sb, tabs, "Initializes a new instance of " + className);
			if (cmdManParam)
				AddSummaryParam(sb, tabs, "commandManager", "The CommandManager object used to communicate with the Blackboard");
			AddSummaryParam(sb, tabs, "variableName", "The name of the variable in the Blackboard");
			if (valueParam)
				AddSummaryParam(sb, tabs, "value", "The value to store in the shared variable if it does not exist");
			if (initializeParam)
				AddSummaryParam(sb, tabs, "initialize", "Indicates if the shared variable will be automatically initialized if the commandManager is different from null");
			AddSummaryRemarks(sb, tabs, "If there is no variable with the provided name in the blackboard, a new variable with the associated name is created");
		}

		private static void AddConstructorSignature(StringBuilder sb, string className, Type type, int tabs, bool cmdManParam, bool initializeParam, bool valueParam)
		{
			TabbedAppend(sb, tabs, "public ");
			sb.Append(className);
			sb.Append("(");
			if (cmdManParam) sb.Append("CommandManager commandManager, ");
			sb.Append("string variableName");
			if (valueParam)
			{
				sb.Append(", ");
				sb.Append(type.FullName);
				sb.Append(" value");
			}
			if (initializeParam) sb.Append(", bool initialize");
			sb.AppendLine(")");
		}

		private static void AddConstructorCaller(StringBuilder sb, Type type, int tabs, bool cmdManParam, bool initializeParam, bool valueParam)
		{
			TabbedAppend(sb, tabs + 1, "	: ");
			sb.Append((cmdManParam && initializeParam && valueParam) ? "base" : "this");
			sb.Append("(");
			sb.Append(cmdManParam ? "commandManager, " : "null, ");
			sb.Append("variableName");
			sb.Append(valueParam ? ", value" : ", default(" + type.FullName + ")");
			sb.Append(initializeParam ? ", initialize" : ", false");
			sb.AppendLine(") { }");
		}

		#endregion

		#region Properties generation methods

		private static void AddPropertyIsArray(StringBuilder sb, Type type, int tabs)
		{
			AddSummary(sb, tabs, "Returns " + type.IsArray.ToString());
			TabbedAppendLine(sb, tabs, "public override bool IsArray");
			TabbedAppendLine(sb, tabs, "{");
			TabbedAppend(sb, tabs + 1, "get { return ");
			sb.Append(type.IsArray.ToString().ToLower());
			sb.AppendLine("; }");
			TabbedAppendLine(sb, tabs, "}");
		}

		private static void AddPropertyLength(StringBuilder sb, Type type, int tabs)
		{
			AddSummary(sb, tabs, "Returns " + (type.IsArray? "the length of the array" : "-1"));
			TabbedAppendLine(sb, tabs, "public override int Length");
			TabbedAppendLine(sb, tabs, "{");
			if (type.IsArray)
			TabbedAppendLine(sb, tabs + 1, "get { return this.BufferedData != null ? this.BufferedData.Length : -1; }");
			else
				TabbedAppendLine(sb, tabs + 1, "get { return -1; }");
			TabbedAppendLine(sb, tabs, "}");
		}

		private static void AddPropertyTypeName(StringBuilder sb, Type type, int tabs)
		{
			AddSummary(sb, tabs, "Returns " + type.Name);
			TabbedAppendLine(sb, tabs, "public override string TypeName");
			TabbedAppendLine(sb, tabs, "{");
			TabbedAppendLine(sb, tabs + 1, "get { return \"" + type.Name + "\"; }");
			TabbedAppendLine(sb, tabs, "}");
		}

		#endregion

		#region Data-deserialization-method generation methods

		private static void AddDeserializeMethodDocumentation(StringBuilder sb, int tabs, Type type)
		{
			AddSummary(sb, tabs, "Deserializes an array of doubles from a string");
			AddSummaryParam(sb, tabs, "serializedData", "String containing the serialized object");
			AddSummaryParam(sb, tabs, "value", "When this method returns contains the value stored in serializedData the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized");
			AddSummaryReturns(sb, tabs, "true if serializedData was deserialized successfully; otherwise, false");
		}

		private static void AddDeserializeMethodSignature(StringBuilder sb, int tabs, Type type)
		{
			TabbedAppend(sb, tabs, "protected override bool Deserialize(string serializedData, out ");
			sb.Append(type.FullName);
			sb.AppendLine(" value)");
		}

		private static void AddDeserializeMethodBody(StringBuilder sb, int tabs, Type type, List<FieldInfo> fields, List<PropertyInfo> properties)
		{
			// Generate line: value = default(Type);
			TabbedAppend(sb, tabs, "value = default(");
			sb.Append(type.FullName);
			sb.AppendLine(");");
			// Generate line: Type temp = new Type();
			TabbedAppend(sb, tabs, type.FullName);
			sb.Append(" tmp = new ");
			sb.Append(type.FullName);
			sb.AppendLine("();");
			TabbedAppendLine(sb, tabs, "string memberName;");
			TabbedAppendLine(sb, tabs, "string memberData;");
			TabbedAppendLine(sb, tabs, "int cc = 0;");
			foreach (FieldInfo fi in fields)
				GenerateDeserializationCode(sb, tabs, fi.FieldType, fi.Name);
			foreach (PropertyInfo pi in properties)
				GenerateDeserializationCode(sb, tabs, pi.PropertyType, pi.Name);
			TabbedAppendLine(sb, tabs, "value = tmp;");
			TabbedAppendLine(sb, tabs, "return true;");
		}

		private static void AddDeserializeArrayMethodBody(StringBuilder sb, int tabs, Type type, List<FieldInfo> fields, List<PropertyInfo> properties)
		{
			Type eType = type.GetElementType();
			// Generate line: value = default(Type);
			TabbedAppendLine(sb, tabs, "value = null;");
			// Generate line: Type item;
			TabbedAppendLine(sb, tabs, eType.FullName + " item;");
			// Generate line: List<Type> lvalues = new List<Type>();
			string lst = "List<" + eType.FullName + ">";
			TabbedAppend(sb, tabs, lst);
			sb.Append(" lvalues = new ");
			sb.Append(lst);
			sb.AppendLine("();");
			sb.AppendLine();
			TabbedAppendLine(sb, tabs, "if( String.IsNullOrEmpty(serializedData) || (serializedData.Trim() == \"null\") )");
			TabbedAppendLine(sb, tabs+1, "return true;");
		}

		private static void AddTypeDeserializationMethod(StringBuilder sb, int tabs, Type type, List<FieldInfo> fields, List<PropertyInfo> properties, Dictionary<string, bool> serializableTypes)
		{
			AddTypeDeserializationMethodDocumentation(sb, tabs, type);
			AddTypeDeserializationMethodSignature(sb, tabs, type);
			TabbedAppendLine(sb, tabs, "{");
			if (type.IsArray)
				AddTypeArrayDeserializationMethodBody(sb, tabs + 1, type);
			else
				AddTypeDeserializationMethodBody(sb, tabs + 1, type, fields, properties);
			TabbedAppendLine(sb, tabs, "}");
			sb.AppendLine();
			if (type.IsArray)
				GenerateDeserializationMethodForType(sb, tabs, type.GetElementType(), serializableTypes);
		}

		private static void AddTypeDeserializationMethodDocumentation(StringBuilder sb, int tabs, Type type)
		{
			AddSummary(sb, tabs, "Deserializes the provided object to a string");
			AddSummaryParam(sb, tabs, "serializedData", "String containing the serialized object");
			AddSummaryParam(sb, tabs, "value", "When this method returns contains the value stored in serializedData the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized");
			AddSummaryReturns(sb, tabs, "true if serializedData was deserialized successfully; otherwise, false");
		}

		private static void AddTypeDeserializationMethodSignature(StringBuilder sb, int tabs, Type type)
		{
			TabbedAppend(sb, tabs, "protected bool Deserialize(string serializedData, out ");
			sb.Append(type.FullName);
			sb.AppendLine(" value)");
		}

		private static void AddTypeDeserializationMethodBody(StringBuilder sb, int tabs, Type type, List<FieldInfo> fields, List<PropertyInfo> properties)
		{
			AddDeserializeMethodBody(sb, tabs, type, fields, properties);
		}

		private static void AddTypeArrayDeserializationMethodBody(StringBuilder sb, int tabs, Type type)
		{
			type = type.GetElementType();
			AddTypeArrayDeserializationMethodCore(sb, tabs, type);
			sb.AppendLine();
			TabbedAppendLine(sb, tabs, "return true;");
		}

		private static void AddTypeArrayDeserializationMethodCore(StringBuilder sb, int tabs, Type type)
		{
			TabbedAppendLine(sb, tabs, "int initialLength = sb.Length;");
			TabbedAppendLine(sb, tabs, "if ((values == null) || (values.Length < 1))");
			TabbedAppendLine(sb, tabs + 1, "return true;");
			sb.AppendLine();
			TabbedAppend(sb, tabs, "if (!");
			sb.Append(supportedTypes.ContainsKey(type.FullName) ? "PrimitiveSerializer.Serialize" : "Serialize");
			sb.AppendLine("(values[0], sb))");
			TabbedAppendLine(sb, tabs, "{");
			TabbedAppendLine(sb, tabs + 1, "sb.Length = initialLength;");
			TabbedAppendLine(sb, tabs + 1, "return false;");
			TabbedAppendLine(sb, tabs, "}");
			TabbedAppendLine(sb, tabs, "for (int i = 1; i < values.Length; ++i)");
			TabbedAppendLine(sb, tabs, "{");
			TabbedAppendLine(sb, tabs + 1, "sb.Append(' ');");
			TabbedAppendLine(sb, tabs + 1, "if (!Serialize(values[i], sb))");
			TabbedAppendLine(sb, tabs + 1, "{");
			TabbedAppendLine(sb, tabs + 2, "sb.Length = initialLength;");
			TabbedAppendLine(sb, tabs + 2, "return false;");
			TabbedAppendLine(sb, tabs + 1, "}");
			TabbedAppendLine(sb, tabs, "}");
		}

		private static void GenerateDeserializationCode(StringBuilder sb, int tabs, Type type, string memberName)
		{
			TabbedAppend(sb, tabs, "// Deserialize ");
			sb.Append(type.Name);
			sb.Append(' ');
			sb.AppendLine(memberName);
			TabbedAppendLine(sb, tabs, "if(!PrimitiveSerializer.XtractNamedValueData(serializedData, ref cc, out memberName, out memberData)) return false;");
			TabbedAppend(sb, tabs, "if(memberName != \"");
			sb.Append(memberName);
			sb.AppendLine("\") return false;");
			// Type memberName;
			string codeMemberName = " __" + memberName + "__";
			TabbedAppend(sb, tabs, type.FullName);
			sb.Append(codeMemberName);
			sb.AppendLine(";");
			TabbedAppend(sb, tabs, "if(!");
			sb.Append(supportedTypes.ContainsKey(type.FullName) ? "PrimitiveSerializer.Deserialize" : "Deserialize");
			sb.Append("(memberData, out");
			sb.Append(codeMemberName);
			sb.AppendLine(")) return false;");
			TabbedAppend(sb, tabs, "tmp.");
			sb.Append(memberName);
			sb.Append(" =");
			sb.Append(codeMemberName);
			sb.AppendLine(";");
			sb.AppendLine();
		}

		private static void GenerateDeserializationMethodForType(StringBuilder sb, int tabs, Type type, Dictionary<string, bool> deserializableTypes)
		{
			if (deserializableTypes.ContainsKey(type.FullName))
				return;

			List<FieldInfo> fields = new List<FieldInfo>();
			List<PropertyInfo> properties = new List<PropertyInfo>();
			GetRWFields(type, fields);
			GetRWProperties(type, properties);
			AddTypeDeserializationMethod(sb, tabs, type, fields, properties, deserializableTypes);
			deserializableTypes.Add(type.FullName, true);
			foreach (FieldInfo fi in fields)
				GenerateDeserializationMethodForType(sb, tabs, fi.FieldType, deserializableTypes);
			foreach (PropertyInfo pi in properties)
				GenerateDeserializationMethodForType(sb, tabs, pi.PropertyType, deserializableTypes);
		}

		#endregion

		#region Data-serialization-method generation methods

		private static void AddSerializeMethodDocumentation(StringBuilder sb, int tabs, Type type)
		{
			AddSummary(sb, tabs, "Serializes the provided object to a string");
			AddSummaryParam(sb, tabs, "value", "Object to be serialized");
			AddSummaryParam(sb, tabs, "serializedData", "When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized");
			AddSummaryReturns(sb, tabs, "true if value was serialized successfully; otherwise, false");
		}

		private static void AddSerializeMethodSignature(StringBuilder sb, int tabs, Type type)
		{
			TabbedAppend(sb, tabs, "protected override bool Serialize(");
			sb.Append(type.FullName);
			sb.AppendLine(" value, out string serializedData)");
		}

		private static void AddSerializeMethodBody(StringBuilder sb, int tabs, Type type, List<FieldInfo> fields, List<PropertyInfo> properties)
		{
			TabbedAppendLine(sb, tabs, "serializedData = null;");
			TabbedAppendLine(sb, tabs, "StringBuilder sb = new StringBuilder(8192);");
			foreach (FieldInfo fi in fields)
				GenerateSerializationCode(sb, tabs, fi.FieldType, fi.Name);
			foreach (PropertyInfo pi in properties)
				GenerateSerializationCode(sb, tabs, pi.PropertyType, pi.Name);
			TabbedAppendLine(sb, tabs, "serializedData = sb.ToString();");
			TabbedAppendLine(sb, tabs, "return true;");
		}

		private static void AddSerializeArrayMethodBody(StringBuilder sb, int tabs, Type type, List<FieldInfo> fields, List<PropertyInfo> properties)
		{
			TabbedAppendLine(sb, tabs, "serializedData = null;");
			TabbedAppendLine(sb, tabs, "StringBuilder sb = new StringBuilder(8192);");
			AddTypeArraySerializationMethodCore(sb, tabs, type);
			TabbedAppendLine(sb, tabs, "serializedData = sb.ToString();");
			TabbedAppendLine(sb, tabs, "return true;");
		}

		private static void AddTypeSerializationMethod(StringBuilder sb, int tabs, Type type, List<FieldInfo> fields, List<PropertyInfo> properties, Dictionary<string, bool> serializableTypes)
		{
			AddTypeSerializationMethodDocumentation(sb, tabs, type);
			AddTypeSerializationMethodSignature(sb, tabs, type);
			TabbedAppendLine(sb, tabs, "{");
			if(type.IsArray)
				AddTypeArraySerializationMethodBody(sb, tabs + 1, type);
			else
				AddTypeSerializationMethodBody(sb, tabs + 1, type, fields, properties);
			TabbedAppendLine(sb, tabs, "}");
			sb.AppendLine();
			if (type.IsArray)
				GenerateSerializationMethodForType(sb, tabs, type.GetElementType(), serializableTypes);
		}

		private static void AddTypeSerializationMethodDocumentation(StringBuilder sb, int tabs, Type type)
		{
			AddSummary(sb, tabs, "Serializes the provided object to a string");
			AddSummaryParam(sb, tabs, "value", "Object to be serialized");
			AddSummaryParam(sb, tabs, "sb", "A StringBuilder object where serialized data will be written");
			AddSummaryReturns(sb, tabs, "true if value was serialized successfully; otherwise, false");
		}

		private static void AddTypeSerializationMethodSignature(StringBuilder sb, int tabs, Type type)
		{
			TabbedAppend(sb, tabs, "protected bool Serialize(");
			sb.Append(type.FullName);
			sb.AppendLine(" value, StringBuilder sb)");
		}

		private static void AddTypeSerializationMethodBody(StringBuilder sb, int tabs, Type type, List<FieldInfo> fields, List<PropertyInfo> properties)
		{
			foreach (FieldInfo fi in fields)
				GenerateSerializationCode(sb, tabs, fi.FieldType, fi.Name);
			foreach (PropertyInfo pi in properties)
				GenerateSerializationCode(sb, tabs, pi.PropertyType, pi.Name);
		}

		private static void AddTypeArraySerializationMethodBody(StringBuilder sb, int tabs, Type type)
		{
			type = type.GetElementType();
			AddTypeArraySerializationMethodCore(sb, tabs, type);
			sb.AppendLine();
			TabbedAppendLine(sb, tabs, "return true;");
		}

		private static void AddTypeArraySerializationMethodCore(StringBuilder sb, int tabs, Type type)
		{
			TabbedAppendLine(sb, tabs, "int initialLength = sb.Length;");
			TabbedAppendLine(sb, tabs, "if ((value == null) || (value.Length < 1))");
			TabbedAppendLine(sb, tabs + 1, "return true;");
			sb.AppendLine();
			TabbedAppendLine(sb, tabs, "sb.Append(\"{ \");");
			TabbedAppend(sb, tabs, "if (!");
			sb.Append(supportedTypes.ContainsKey(type.FullName) ? "PrimitiveSerializer.Serialize" : "Serialize");
			sb.AppendLine("(value[0], sb))");
			TabbedAppendLine(sb, tabs, "{");
			TabbedAppendLine(sb, tabs + 1, "sb.Length = initialLength;");
			TabbedAppendLine(sb, tabs + 1, "return false;");
			TabbedAppendLine(sb, tabs, "}");
			TabbedAppendLine(sb, tabs, "for (int i = 1; i < value.Length; ++i)");
			TabbedAppendLine(sb, tabs, "{");
			TabbedAppendLine(sb, tabs + 1, "sb.Append(' ');");
			TabbedAppend(sb, tabs, "if (!");
			sb.Append(supportedTypes.ContainsKey(type.FullName) ? "PrimitiveSerializer.Serialize" : "Serialize");
			sb.AppendLine("(value[i], sb))");
			TabbedAppendLine(sb, tabs + 1, "{");
			TabbedAppendLine(sb, tabs + 2, "sb.Length = initialLength;");
			TabbedAppendLine(sb, tabs + 2, "return false;");
			TabbedAppendLine(sb, tabs + 1, "}");
			TabbedAppendLine(sb, tabs, "}");
		}

		private static void GenerateSerializationCode(StringBuilder sb, int tabs, Type type, string memberName)
		{
			TabbedAppend(sb, tabs, "sb.Append(\"");
			sb.Append(memberName);
			sb.AppendLine("={ \");");
			TabbedAppend(sb, tabs, "if(!");
			sb.Append(supportedTypes.ContainsKey(type.FullName) ? "PrimitiveSerializer.Serialize" : "Serialize");
			sb.Append("(value.");
			sb.Append(memberName);
			sb.AppendLine(", sb)) return false;");
			TabbedAppendLine(sb, tabs, "sb.Append(\" } \");");
			TabbedAppendLine(sb, tabs, "return true;");
		}

		private static void GenerateSerializationMethodForType(StringBuilder sb, int tabs, Type type, Dictionary<string, bool> serializableTypes)
		{
			if (serializableTypes.ContainsKey(type.FullName))
				return;

			List<FieldInfo> fields = new List<FieldInfo>();
			List<PropertyInfo> properties = new List<PropertyInfo>();
			GetRWFields(type, fields);
			GetRWProperties(type, properties);
			AddTypeSerializationMethod(sb, tabs, type, fields, properties, serializableTypes);
			serializableTypes.Add(type.FullName, true);
			foreach (FieldInfo fi in fields)
				GenerateSerializationMethodForType(sb, tabs, fi.FieldType, serializableTypes);
			foreach (PropertyInfo pi in properties)
				GenerateSerializationMethodForType(sb, tabs, pi.PropertyType, serializableTypes);
		}

		#endregion

		#region Summary Generation Methods

		private static void AddSummary(StringBuilder sb, int tabs, string text)
		{
			TabbedAppendLine(sb, tabs, "/// <summary>");
			TabbedAppend(sb, tabs, "/// ");
			sb.AppendLine(text);
			TabbedAppendLine(sb, tabs, "/// </summary>");
		}

		private static void AddSummaryParam(StringBuilder sb, int tabs, string paramName, string comment)
		{
			TabbedAppend(sb, tabs, "/// <param name=\"");
			sb.Append(paramName);
			sb.Append("\">");
			sb.Append(comment);
			sb.AppendLine("</param>");
		}

		private static void AddSummaryRemarks(StringBuilder sb, int tabs, string text)
		{
			TabbedAppend(sb, tabs, "/// <remarks>");
			sb.Append(text);
			sb.AppendLine("</remarks>");
		}

		private static void AddSummaryReturns(StringBuilder sb, int tabs, string text)
		{
			TabbedAppend(sb, tabs, "/// <returns>");
			sb.Append(text);
			sb.AppendLine("</returns>");
		}

		#endregion

		private static void TabbedAppend(StringBuilder sb, int tabs, string value)
		{
			AppendTabs(sb, tabs);
			sb.Append(value);
		}

		private static void TabbedAppendLine(StringBuilder sb, int tabs, string value)
		{
			AppendTabs(sb, tabs);
			sb.AppendLine(value);
		}

		private static void AppendTabs(StringBuilder sb, int tabs)
		{
			for (int i = 0; i < tabs; ++i)
				sb.Append('\t');
		}

		/// <summary>
		/// Fills the writableMembers list with the public fields of the provided type
		/// </summary>
		private static void GetRWFields(Type t, List<MemberInfo> writableMembers)
		{
			while (t.IsArray) t = t.GetElementType();
			FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo fi in fields)
			{
				if (fi.IsInitOnly)
					continue;
				if (fi.FieldType.IsGenericType)
					throw new Exception("Error getting fields from " + t.FullName + ". Shared variables don't support generic types");
				writableMembers.Add(fi);
			}
		}

		/// <summary>
		/// Fills the writableMembers list with the public fields of the provided type
		/// </summary>
		private static void GetRWFields(Type t, List<FieldInfo> fields)
		{
			while (t.IsArray) t = t.GetElementType();
			FieldInfo[] aFields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo fi in aFields)
			{
				if (fi.IsInitOnly)
					continue;
				if (fi.FieldType.IsGenericType)
					throw new Exception("Error getting fields from " + t.FullName + ". Shared variables don't support generic types");
				fields.Add(fi);
			}
		}

		/// <summary>
		/// Fills the writableMembers list with the public properties of the provided type
		/// </summary>
		private static void GetRWProperties(Type t, List<MemberInfo> writableMembers)
		{
			while (t.IsArray) t = t.GetElementType();
			PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo pi in properties)
			{
				if (!pi.CanRead || !pi.CanWrite)
					continue;
				if (pi.PropertyType.IsGenericType)
					throw new Exception("Error getting properties from " + t.FullName + ". Shared variables don't support generic types");
				writableMembers.Add(pi);
			}
		}

		/// <summary>
		/// Fills the writableMembers list with the public properties of the provided type
		/// </summary>
		private static void GetRWProperties(Type t, List<PropertyInfo> properties)
		{
			while (t.IsArray) t = t.GetElementType();
			PropertyInfo[] aProp = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo pi in aProp)
			{
				if (!pi.CanRead || !pi.CanWrite)
					continue;
				if (pi.PropertyType.IsGenericType)
					throw new Exception("Error getting properties from " + t.FullName + ". Shared variables don't support generic types");
				properties.Add(pi);
			}
		}

		/// <summary>
		/// Gets a value indicating if the Type contains a parameterless constructor
		/// </summary>
		private static bool HasParameterlessConstructor(Type t)
		{
			while (t.IsArray) t = t.GetElementType();
			ConstructorInfo[] constructors = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
			foreach (ConstructorInfo ci in constructors)
			{
				if (ci.GetParameters().Length == 0)
					return true;
			}
			return false;
		}
	}
}
