using System;
using System.Collections.Generic;
using System.Text;
using Robotics.Utilities;

namespace Robotics.API
{
	/// <summary>
	/// Encapsulates shared variable information
	/// </summary>
	[Serializable]
	public class SharedVariableInfo :ISharedVariableInfo
	{
		#region Variables

		/// <summary>
		/// An array containing the list of all module names with write permission on the shared variable.
		/// A null value represents that all modules may write the shared variable
		/// </summary>
		private string[] allowedWriters;

		/// <summary>
		/// The creation time of the shared variable
		/// </summary>
		private DateTime creationTime;

		/// <summary>
		/// Indicaties if the variable is an array
		/// </summary>
		private bool isArray;

		/// <summary>
		/// If the variable is an array gets the length of the array, else returns -1
		/// </summary>
		private int length;

		/// <summary>
		/// The name of the SharedVariable
		/// </summary>
		private string name;

		/// <summary>
		/// Information about the subscriptions to the shared variable
		/// </summary>
		private ISharedVariableSubscriptionInfo[] subscriptions;

		/// <summary>
		/// The global name of the type of the SharedVariable
		/// </summary>
		private string typeName;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SharedVariableInfo.
		/// This constructor is provided for serialization purposes only.
		/// </summary>
		public SharedVariableInfo()
		{
			this.typeName = "var";
			this.name = "var1";
			this.isArray = false;
			this.length = -1;
		}

		/// <summary>
		/// Initializes a new instance of SharedVariableInfo
		/// </summary>
		/// <param name="typeName">The global name of the type of the SharedVariable</param>
		/// <param name="name">The name of the SharedVariable</param>
		public SharedVariableInfo(string typeName, string name)
			: this(typeName, name, false, -1, null) { }

		/// <summary>
		/// Initializes a new instance of SharedVariableInfo
		/// </summary>
		/// <param name="typeName">The global name of the type of the SharedVariable</param>
		/// <param name="name">The name of the SharedVariable</param>
		/// <param name="allowedWriters">An array containing the list of all module names with write 
		/// permission on the shared variable. A null value represents that all modules may write the shared variable</param>
		public SharedVariableInfo(string typeName, string name, string[] allowedWriters)
			: this(typeName, name, false, -1, allowedWriters) { }

		/// <summary>
		/// Initializes a new instance of SharedVariableInfo
		/// </summary>
		/// <param name="typeName">The global name of the type of the SharedVariable</param>
		/// <param name="name">The name of the SharedVariable</param>
		/// <param name="isArray">Indicaties if the variable is an array</param>
		/// <param name="length">If the variable is an array gets the length of the array, else returns -1</param>
		public SharedVariableInfo(string typeName, string name, bool isArray, int length)
			: this(typeName, name, isArray, length, null) { }

		/// <summary>
		/// Initializes a new instance of SharedVariableInfo
		/// </summary>
		/// <param name="typeName">The global name of the type of the SharedVariable</param>
		/// <param name="name">The name of the SharedVariable</param>
		/// <param name="isArray">Indicaties if the variable is an array</param>
		/// <param name="length">If the variable is an array gets the length of the array, else returns -1</param>
		/// <param name="allowedWriters">An array containing the list of all module names with write 
		/// permission on the shared variable. A null value represents that all modules may write the shared variable</param>
		public SharedVariableInfo(string typeName, string name, bool isArray, int length, string[] allowedWriters)
		{
			this.CreationTime = DateTime.MinValue;
			this.typeName = typeName;
			this.name = name;
			this.length = (this.isArray = isArray) ? length : -1;
			this.allowedWriters = allowedWriters;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets an array containing the list of all module names with write permission on the shared variable.
		/// A null value represents that all modules may write the shared variable
		/// </summary>
		public string[] AllowedWriters
		{
			get { return this.allowedWriters; }
			set
			{
				if (value != null)
				{
					for (int i = 0; i < value.Length; ++i)
					{
						if (String.IsNullOrEmpty(value[i]))
							throw new ArgumentException("Invalid module name at position " + i.ToString());
					}
					if (value.Length < 1) value = null;
				}
				this.allowedWriters = value;
			}
		}

		/// <summary>
		/// Gets the creation time of the shared variable
		/// </summary>
		public DateTime CreationTime
		{
			get { return this.creationTime; }
			set { this.creationTime = value; }
		}

		/// <summary>
		/// Gets a value indicating if the variable is an array
		/// </summary>
		public bool IsArray
		{
			get { return this.isArray; }
			set { this.isArray = value; }
		}

		/// <summary>
		/// If the variable is an array gets the length of the array, else returns -1
		/// </summary>
		public int Length
		{
			get { return this.length; }
			set { this.length = value; }
		}

		/// <summary>
		/// Gets the name of the SharedVariable
		/// </summary>
		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		/// <summary>
		/// Gets or sets the information about the subscriptions to the shared variable
		/// </summary>
		public ISharedVariableSubscriptionInfo[] Subscriptions
		{
			get { return this.subscriptions; }
			set
			{
				if (value != null)
				{
					for (int i = 0; i < value.Length; ++i)
					{
						if ((value[i] == null) || ((value[i].VariableInfo != null) && (value[i].VariableInfo != this)))
							throw new ArgumentException("Invalid subscription information at position " + i.ToString());
						//value[i].
					}
					if (value.Length < 1) value = null;
				}

				this.subscriptions = value;
			}
		}

		/// <summary>
		/// Gets the global name of the type of the SharedVariable
		/// </summary>
		public string TypeName
		{
			get { return this.typeName; }
			set { this.typeName = value; }
		}

		#endregion

		#region Methods
		#endregion

		#region Static Methods

		/// <summary>
		/// Deserializes an ISharedVariableInfo object from a string
		/// </summary>
		/// <param name="serialized">String that contains the data to be deserialized</param>
		/// <param name="info">When this method returns contains a ISharedVariableInfo object created from the
		/// information provided in the input string if the serialization succeded,
		/// or false if the serialization failed</param>
		/// <returns>true if the deserialization succeded, false otherwise</returns>
		public static bool Deserialize(string serialized, out SharedVariableInfo info)
		{
			Exception ex;
			return Deserialize(serialized, out info, out ex);
		}

		/// <summary>
		/// Deserializes an ISharedVariableInfo object from a string
		/// </summary>
		/// <param name="serialized">String that contains the data to be deserialized</param>
		/// <param name="info">When this method returns contains a ISharedVariableInfo object created from the
		/// information provided in the input string if the serialization succeded,
		/// or false if the serialization failed</param>
		/// <param name="ex">When this method returns contains null if the variable information was deserialized
		/// successfully, or the exception to be thrown if the deserialization failed</param>
		/// <returns>true if the deserialization succeded, false otherwise</returns>
		public static bool Deserialize(string serialized, out SharedVariableInfo info, out Exception ex)
		{
			int cc = 0;
			string[] writers;
			SubscriptionInfo[] subscriptions;
			DateTime creationTime;

			info = null;
			ex = null;
			if (String.IsNullOrEmpty(serialized))
			{
				ex = new ArgumentNullException("Invalid input string");
				return false;
			}

			// 1. Deserialize variable information
			if (!DeserializeSVInfo(serialized, ref cc, out info, out ex))
			{
				info = null;
				return false;
			}

			if (!DeserializeCreationTime(serialized, ref cc, out creationTime, out ex))
			{
				info = null;
				return false;
			}

			// 2. Deserialize writers
			if (!DeserializeWriters(serialized, ref cc, out writers, out ex))
			{
				info = null;
				return false;
			}

			// 3. Deserialize subscriptions information
			if (!DeserializeSubscriptions(info, serialized, ref cc, out subscriptions, out ex))
			{
				info = null;
				return false;
			}

			// 4. Integrate package
			info.creationTime = creationTime;
			info.subscriptions = subscriptions;
			info.allowedWriters = writers;
			return true;
		}

		private static bool DeserializeSVInfo(string serialized, ref int cc, out SharedVariableInfo info, out Exception ex)
		{
			string variableType;
			string variableName;
			ushort uLength;
			int length = -1;
			bool isArray = false;

			// 1. Initialize output values
			info = null;
			ex = null;

			// 2. Extract variable type
			if (!Parser.XtractIdentifier(serialized, ref cc, out variableType))
			{
				ex = new Exception("Expected identifier (variable type)");
				return false;
			}

			// 3. Get variable array data
			if (Scanner.ReadChar('[', serialized, ref cc))
			{
				length = Scanner.XtractUInt16(serialized, ref cc, out uLength) ? uLength : -1;
				if (!Scanner.ReadChar(']', serialized, ref cc))
				{
					ex = new Exception("Expected ']'");
					return false;
				}
				isArray = true;
			}

			// 4. Get variable name
			Parser.SkipSpaces(serialized, ref cc);
			if (!Parser.XtractIdentifier(serialized, ref cc, out variableName))
			{
				ex = new Exception("Expected identifier (variable name)");
				return false;
			}

			info = new SharedVariableInfo(variableType, variableName, isArray, length);
			return true;
		}

		private static bool DeserializeCreationTime(string serialized, ref int cc, out DateTime creationTime, out Exception ex)
		{
			int bcc;

			creationTime = DateTime.MinValue;
			ex = null;
			Scanner.SkipSpaces(serialized, ref cc);
			if (!Scanner.ReadString("creationTime=", serialized, ref cc))
				return true;

			if (!Scanner.ReadChar('{', serialized, ref cc))
			{
				ex = new Exception("Invalid creationTime segment, expected '{'");
				return false;
			}

			bcc = cc;
			// Read until next '}' character
			while (cc < serialized.Length)
			{
				if (serialized[cc] == '}')
					break;
				++cc;
			}
			if (cc >= serialized.Length)
			{
				ex = new Exception("Invalid creationTime segment, expected '}'");
				return false;
			}

			if ((cc == bcc) || !DateTime.TryParse(serialized.Substring(bcc, cc - bcc), out creationTime))
			{
				ex = new Exception("Invalid creation Time. Incorrect format.");
				return false;
			}
			// Finally skip the closing }
			Scanner.ReadChar('}', serialized, ref cc);
			return true;
		}

		private static bool DeserializeWriters(string serialized, ref int cc, out string[] writers, out Exception ex)
		{
			int bcc;

			writers = null;
			ex = null;
			Scanner.SkipSpaces(serialized, ref cc);
			if (!Scanner.ReadString("writers=", serialized, ref cc))
				return true;

			if (!Scanner.ReadChar('{', serialized, ref cc))
			{
				ex = new Exception("Invalid writers segment, expected '{'");
				return false;
			}

			bcc = cc;
			// Read until next '}' character
			while (cc < serialized.Length)
			{
				if (serialized[cc] == '}')
					break;
				++cc;
			}
			if (cc >= serialized.Length)
			{
				ex = new Exception("Invalid writers segment, expected '}'");
				return false;
			}

			if (cc == bcc)
			{
				ex = new Exception("Invalid writers segment. Incorrect format.");
				return false;
			}
			writers = serialized.Substring(bcc, cc - bcc).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if ((writers.Length == 1) && (writers[0] == "*"))
				writers = null;

			// Finally skip the closing }
			Scanner.ReadChar('}', serialized, ref cc);
			return true;

		}

		private static bool DeserializeSubscriptions(SharedVariableInfo svInfo, string serialized, ref int cc, out SubscriptionInfo[] subscriptions, out Exception ex)
		{
			SubscriptionInfo subscription;
			List<SubscriptionInfo> lstSubscription;

			subscriptions = null;
			ex = null;
			Scanner.SkipSpaces(serialized, ref cc);
			if (!Scanner.ReadString("subscriptions=", serialized, ref cc))
				return true;

			if (!Scanner.ReadChar('{', serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment, expected '{'");
				return false;
			}

			lstSubscription = new List<SubscriptionInfo>();
			while ((cc < serialized.Length) && (serialized[cc] == '{'))
			{
				if (!DeserializeSubscription(svInfo, serialized, ref cc, out subscription, out ex))
					return false;
				lstSubscription.Add(subscription);
			}

			if (!Scanner.ReadChar('}', serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment, expected '}'");
				return false;
			}

			subscriptions = lstSubscription.ToArray();
			return true;
		}

		private static bool DeserializeSubscription(SharedVariableInfo svInfo, string serialized, ref int cc, out SubscriptionInfo subscription, out Exception ex)
		{
			//int bcc;
			string subscriber;
			SharedVariableSubscriptionType sType;
			SharedVariableReportType rType;
			string writer = null;

			subscription = null;
			if (!Scanner.ReadChar('{', serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment, expected '{'");
				return false;
			}

			if (!DeserializeSubscriber(serialized, ref cc, out subscriber, out ex))
				return false;
			if (!DeserializeSubscriptionType(serialized, ref cc, out sType, out ex))
				return false;
			if (!DeserializeReportType(serialized, ref cc, out rType, out ex))
				return false;
			if ((sType == SharedVariableSubscriptionType.WriteModule) && !DeserializeSubscriptionWriter(serialized, ref cc, out writer, out ex))
				return false;

			if (!Scanner.ReadChar('}', serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment, expected '}'");
				return false;
			}

			subscription = new SubscriptionInfo(svInfo);
			subscription.ModuleName = subscriber;
			subscription.SubscriptionType = sType;
			subscription.ReportType = rType;
			subscription.WriterModule = writer;
			return true;
		}

		private static bool DeserializeSubscriber(string serialized, ref int cc, out string subscriber, out Exception ex)
		{
			int bcc;

			ex = null;
			subscriber = null;
			if (!Scanner.ReadString("subscriber=", serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment, expected 'subscriber='");
				return false;
			}
			bcc = cc;
			if (!Scanner.AdvanceToChar(',', serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment, expected ','");
				return false;
			}
			subscriber = serialized.Substring(bcc, cc - bcc);
			return true;
		}

		private static bool DeserializeSubscriptionType(string serialized, ref int cc, out SharedVariableSubscriptionType sType, out Exception ex)
		{
			int bcc;

			ex = null;
			sType = SharedVariableSubscriptionType.Unknown;
			if (!Scanner.ReadString(", sType=", serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment (subscription type), expected ','");
				return false;
			}
			bcc = cc;
			if (!Scanner.AdvanceToChar(',', serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment (subscription type), expected ','");
				return false;
			}

			try
			{
				sType = (SharedVariableSubscriptionType)Enum.Parse(typeof(SharedVariableSubscriptionType), serialized.Substring(bcc, cc - bcc));
			}
			catch
			{
				ex = new Exception("Invalid value for subscription type");
				return false;
			}
			return true;
		}

		private static bool DeserializeReportType(string serialized, ref int cc, out SharedVariableReportType rType, out Exception ex)
		{
			int bcc;

			ex = null;
			rType = SharedVariableReportType.Unknown;
			if (!Scanner.ReadString(", rType=", serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment (subscription report type), expected ','");
				return false;
			}
			bcc = cc;
			if (!Scanner.AdvanceToChar(new char[]{',', '}'}, serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment (subscription report type), expected '}'");
				return false;
			}

			try
			{
				rType = (SharedVariableReportType)Enum.Parse(typeof(SharedVariableReportType), serialized.Substring(bcc, cc - bcc));
			}
			catch
			{
				ex = new Exception("Invalid value for subscription report type");
				return false;
			}
			return true;
		}

		private static bool DeserializeSubscriptionWriter(string serialized, ref int cc, out string subscriber, out Exception ex)
		{
			int bcc;

			ex = null;
			subscriber = null;
			if (!Scanner.ReadString("writer=", serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment, expected 'writer='");
				return false;
			}
			bcc = cc;
			if (!Scanner.AdvanceToChar('}', serialized, ref cc))
			{
				ex = new Exception("Invalid subscriptions segment, expected ','");
				return false;
			}
			subscriber = serialized.Substring(bcc, cc - bcc);
			return true;
		}

		/// <summary>
		/// Serializes an ISharedVariableInfo object to a string
		/// </summary>
		/// <param name="info">The ISharedVariableInfo object to serialize</param>
		/// <param name="serializedData">When this method returns contains the string representation of the
		/// object if the serialization succeded, or false if the serialization failed</param>
		/// <returns>true if the serialization succeded, false otherwise</returns>
		public static bool Serialize(ISharedVariableInfo info, out string serializedData)
		{
			serializedData = null;
			if (info == null)
				return false;
			StringBuilder sb = new StringBuilder(4096);

			// 1. Append variable information
			SerializeSVInfo(sb, info);

			// 1. Append creation time
			SerializeCreationTime(sb, info);

			// 3. Append writers
			SerializeWriters(sb, info);

			// 4. Append subscriptions information
			SerializeSubscriptions(sb, info);

			serializedData = sb.ToString();
			return true;
		}

		private static void SerializeSVInfo(StringBuilder sb, ISharedVariableInfo info)
		{
			// 1. Append variable type
			sb.Append(info.TypeName);
			if (info.IsArray)
			{
				sb.Append('[');
				if (info.Length > 0)
					sb.Append(info.Length);
				sb.Append(']');
			}

			// 2. Append variable name
			sb.Append(' ');
			sb.Append(info.Name);
		}

		private static void SerializeCreationTime(StringBuilder sb, ISharedVariableInfo info)
		{
			sb.Append(" creationTime={");
			sb.Append(info.CreationTime.ToString("yyyy-MM-dd hh:mm:ss"));
			sb.Append('}');
		}

		private static void SerializeWriters(StringBuilder sb, ISharedVariableInfo info)
		{
			if ((info.AllowedWriters == null) || (info.AllowedWriters.Length < 1))
				return;

			sb.Append(" writers={");
			sb.Append(info.AllowedWriters[0]);
			for (int i = 1; i < info.AllowedWriters.Length; ++i)
			{
				sb.Append(',');
				sb.Append(info.AllowedWriters[i]);
			}
			sb.Append('}');
		}

		private static void SerializeSubscriptions(StringBuilder sb, ISharedVariableInfo info)
		{
			if ((info.Subscriptions == null) || (info.Subscriptions.Length < 1))
				return;

			sb.Append(" subscriptions={");
			SerializeSubscription(sb, info.Subscriptions[0]);
			for (int i = 1; i < info.Subscriptions.Length; ++i)
			{
				sb.Append(',');
				SerializeSubscription(sb, info.Subscriptions[i]);
			}
			sb.Append('}');
		}

		private static void SerializeSubscription(StringBuilder sb, ISharedVariableSubscriptionInfo subcription)
		{
			if (subcription == null)
				return;

			sb.Append("{subscriber=");
			sb.Append(subcription.ModuleName);
			sb.Append(", sType=");
			sb.Append(subcription.SubscriptionType);
			sb.Append(", rType=");
			sb.Append(subcription.ReportType);
			if (subcription.SubscriptionType == SharedVariableSubscriptionType.WriteModule)
			{
				sb.Append(", writer=");
				sb.Append(subcription.WriterModule);
			}
			sb.Append('}');

		}

		#endregion
	}
}
