using System;
using System.Collections.Generic;
using System.Text;

using Robotics.API.Parsers;
using Robotics.Utilities;

namespace Robotics.API
{
	/// <summary>
	/// Represents a report received from the Blackboard due to a shared variable subscription
	/// </summary>
	public class SharedVariableReport
	{
		#region Variables

		/// <summary>
		/// The type of the report
		/// </summary>
		private SharedVariableReportType reportType;

		/// <summary>
		/// The serialized data received with the report
		/// </summary>
		private string serializedData;

		/// <summary>
		/// The type of the subscription
		/// </summary>
		private SharedVariableSubscriptionType subscriptionType;

		/// <summary>
		/// The name of the module which performed the write/create operation
		/// </summary>
		private string writer;

		/// <summary>
		/// Stores the information of the related shared variable
		/// </summary>
		private ISharedVariableInfo variableInfo;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SharedVariableReport
		/// </summary>
		/// <param name="variableInfo">The information of the related shared variable</param>
		/// <param name="serializedData">The serialized value of the shared variable</param>
		/// <param name="reportType">The type of the report</param>
		/// <param name="subscriptionType">The type of the subscription</param>
		/// <param name="writer"> The name of the module which performed the write/create operation</param>
		public SharedVariableReport(ISharedVariableInfo variableInfo, string serializedData,
			SharedVariableReportType reportType, SharedVariableSubscriptionType subscriptionType, string writer)
		{
			if (variableInfo == null)
				throw new ArgumentNullException();
			this.serializedData = serializedData;
			this.variableInfo = variableInfo;
			this.reportType = reportType;
			this.subscriptionType = subscriptionType;
			this.writer = writer;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the type of the report
		/// </summary>
		public SharedVariableReportType ReportType
		{
			get { return this.reportType; }
		}

		/// <summary>
		/// Gets the serialized data received with the report
		/// </summary>
		public string SerializedData
		{
			get { return this.serializedData; }
		}

		/// <summary>
		/// Gets the type of the subscription
		/// </summary>
		public SharedVariableSubscriptionType SubscriptionType
		{
			get { return this.subscriptionType; }
		}

		/// <summary>
		/// Gets the information of the related shared variable
		/// </summary>
		public ISharedVariableInfo VariableInfo { get { return this.variableInfo; } }

		/// <summary>
		/// Gets the name of the module which performed the write/create operation
		/// </summary>
		public string Writer
		{
			get { return this.writer; }
		}

		#endregion

		#region Methodos
		#endregion

		#region Static Methodos

		/// <summary>
		/// Generates a SharedVariableReport object from a Response object
		/// </summary>
		/// <param name="response">The Response object to be used to generate the report</param>
		/// <returns>A SharedVariableReport object created from the Response object</returns>
		public static SharedVariableReport CreateFromResponse(Response response)
		{
			Exception ex;
			SharedVariableReport report;
			if (!CreateFromResponse(response, out report, out ex))
				throw ex;
			return report;
		}

		/// <summary>
		/// Generates a SharedVariableReport object from a Response object
		/// </summary>
		/// <param name="response">The Response object to be used to generate the report</param>
		/// <param name="report">When this method returns contains the SharedVariableReport object
		/// extracted from the response if the parse operation was completed successfully, null otherwise</param>
		/// <param name="ex">When this method returns contains null if the parse operation was completed successfully,
		/// or the exception to be thrown if the operation failed</param>
		/// <returns>A SharedVariableReport object created from the Response object</returns>
		internal static bool CreateFromResponse(Response response, out SharedVariableReport report, out Exception ex)
		{
			string parameters;
			string data;
			
			string writer;
			SharedVariableReportType reportType;
			SharedVariableSubscriptionType subscriptionType;
			ISharedVariableInfo varInfo;

			report = null;
			ex = null;

			parameters = response.Parameters;
			// 1. Get writer
			if (!GetWriter(ref parameters, out writer, out ex))
				return false;

			// 2. Get subscription type
			if(!GetSubscriptionType(ref parameters, out subscriptionType, out ex))
				return false;

			// 3. Get report type.
			if (!GetReportType(ref parameters, out reportType, out ex))
				return false;

			// 4. Get variable info
			if (!GetVariableInfo(ref parameters, out varInfo, out data, out ex))
				return false;

			// 5. Create the report
			report = new SharedVariableReport(varInfo, data, reportType, subscriptionType, writer);
			return true;
		}

		private static bool GetVariableInfo(ref string parameters, out ISharedVariableInfo info, out string data, out Exception ex)
		{
			SharedVariableInfo varInfo;
			string type;
			bool isArray;
			int size;
			string name;
			int cc;

			info = null;
			ex = null;
			data = null;
			varInfo = new SharedVariableInfo();

			// 1. Get variable type
			cc = 0;
			if (!GetVariableType(ref parameters, ref cc, out type, out isArray, out size, out ex))
				return false;
			varInfo.TypeName = type;
			varInfo.IsArray = isArray;
			varInfo.Length = size;

			// 2. Get variable name
			if (!GetVariableName(ref parameters, ref cc, out name, out ex))
				return false;
			varInfo.Name = name;

			// 3. Get variable data
			if (!GetVariableData(ref parameters, ref cc, out data, out ex))
				return false;

			info = varInfo;
			return true;			
		}

		private static bool GetWriter(ref string parameters, out string writer, out Exception ex)
		{
			int pos;

			writer = null;
			ex = null;

			pos = parameters.LastIndexOf('%');
			if (pos == -1)
			{
				ex = new Exception("Invalid parameters. No Writer found");
				return false;
			}
			writer = parameters.Substring(pos + 1).Trim();
			parameters = parameters.Substring(0, pos);

			return true;
		}

		private static bool GetSubscriptionType(ref string parameters, out SharedVariableSubscriptionType subscriptionType, out Exception ex)
		{
			int pos;
			string sSubscriptionType;

			subscriptionType = SharedVariableSubscriptionType.WriteAny;
			ex = null;

			pos = parameters.LastIndexOf('%');
			if (pos == -1)
			{
				ex = new Exception("Invalid parameters. No subscription type found.");
				return false;
			}
			sSubscriptionType = parameters.Substring(pos + 1).Trim();
			parameters = parameters.Substring(0, pos);

			try { subscriptionType = (SharedVariableSubscriptionType)Enum.Parse(typeof(SharedVariableSubscriptionType), sSubscriptionType, true); }
			catch
			{
				ex = new Exception("Invalid parameters. Invalid subscription type.");
				return false;
			}
			return true;
		}

		private static bool GetReportType(ref string parameters, out SharedVariableReportType reportType, out Exception ex)
		{
			int pos;
			string sReportType;

			reportType = SharedVariableReportType.Notify;
			ex = null;

			pos = Math.Max(parameters.LastIndexOf('%'), parameters.LastIndexOf('}'));
			if (pos == -1)
			{
				ex = new Exception("Invalid parameters. No report type type found.");
				return false;
			}
			sReportType = parameters.Substring(pos + 1).Trim();
			parameters = parameters.Substring(0, pos);

			try { reportType = (SharedVariableReportType)Enum.Parse(typeof(SharedVariableReportType), sReportType, true); }
			catch
			{
				ex = new Exception("Invalid parameters. Invalid report type.");
				return false;
			}
			return true;
		}

		private static bool GetVariableType(ref string parameters, ref int cc, out string type,
			out bool isArray, out int size, out Exception ex)
		{
			ushort usize;

			type = null;
			size = -1;
			isArray = false;
			ex = null;

			if (parameters[cc] == '{')
				parameters = parameters.Substring(1, parameters.Length - 1);
			parameters = parameters.Trim();
			if (!Parser.XtractIdentifier(parameters, ref cc, out type))
			{
				ex = new Exception("Invalid parameters. No variable type found.");
				return false;
			}
			isArray = false;
			size = -1;
			if (Scanner.ReadChar('[', parameters, ref cc))
			{
				if (Scanner.XtractUInt16(parameters, ref cc, out usize))
					size = usize;
				if (!Scanner.ReadChar(']', parameters, ref cc))
				{
					ex = new Exception("Invalid parameters. Expected ']'.");
					return false;
				}
				isArray = true;
			}
			return true;
		}

		private static bool GetVariableName(ref string parameters, ref int cc, out string name, out Exception ex)
		{
			ex = null;
			name = null;

			Scanner.SkipSpaces(parameters, ref cc);
			if (!Parser.XtractIdentifier(parameters, ref cc, out name))
			{
				ex = new Exception("Invalid parameters. Expected identifier.");
				return false;
			}
			return true;	
		}

		private static bool GetVariableData(ref string parameters, ref int cc, out string data, out Exception ex)
		{
			ex = null;
			Scanner.SkipSpaces(parameters, ref cc);
			data = parameters.Substring(cc);
			return true;
		}

		#endregion
	}
}
