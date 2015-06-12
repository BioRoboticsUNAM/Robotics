using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Robotics.Utilities;

namespace Robotics.API
{

	/// <summary>
	/// Gets access to a variable stored in the Blackboard
	/// </summary>
	public abstract class SharedVariable : ISharedVariableInfo
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
		/// Information about the subscriptions to the shared variable
		/// </summary>
		private ISharedVariableSubscriptionInfo[] subscriptions;

		#endregion

		#region Properties

		/// <summary>
		/// Gets an array containing the list of all module names with write permission on the shared variable.
		/// A null value represents that all modules may write the shared variable
		/// </summary>
		public string[] AllowedWriters
		{
			get { return this.allowedWriters; }
		}

		/// <summary>
		/// Gets the CommandManager object used to communicate with the Blackboard
		/// </summary>
		public abstract CommandManager CommandManager
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the creation time of the shared variable
		/// </summary>
		public DateTime CreationTime
		{
			get { return this.creationTime; }
		}

		/// <summary>
		/// Gets a value indicating if the variable has been initialized (created or readed from blackboard)
		/// </summary>
		public abstract bool Initialized { get; internal set; }

		/// <summary>
		/// Gets a value indicating if the variable is an array
		/// </summary>
		public abstract bool IsArray
		{
			get;
		}

		/// <summary>
		/// Gets the local time when the value of the shared variable was last updated
		/// </summary>
		public abstract DateTime LastUpdated { get; }

		/// <summary>
		/// Gets the name of the module wich performed the last write operation over the shared variable if known,
		/// otherwise it returns null.
		/// This property returns always null if there is not a subscription to the shared variable.
		/// </summary>
		public abstract string LastWriter { get; protected set; }

		/// <summary>
		/// If the variable is an array gets the length of the array, else returns -1
		/// </summary>
		public abstract int Length { get; }

		/// <summary>
		/// Gets the name of the SharedVariable
		/// </summary>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// Gets the information about the subscriptions to the shared variable
		/// </summary>
		public ISharedVariableSubscriptionInfo[] Subscriptions
		{
			get { return this.subscriptions; }
		}

		/// <summary>
		/// Gets the report type for the current subscription to the shared variable
		/// </summary>
		public abstract SharedVariableReportType SubscriptionReportType { get; protected set; }

		/// <summary>
		/// Gets the subscription type for the current subscription to the shared variable
		/// </summary>
		public abstract SharedVariableSubscriptionType SubscriptionType { get; protected set; }

		/// <summary>
		/// Gets the global name of the type of the SharedVariable
		/// </summary>
		public abstract string TypeName
		{
			get;
		}

		/// <summary>
		/// Gets the type of the SharedVariable
		/// </summary>
		public abstract Type Type
		{
			get;
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when a report is received from the blacboard due to a subscription
		/// </summary>
		public event SharedVariableReportReceivedEventHadler ReportReceived;

		/// <summary>
		/// Occurs when the value of the shared variable is updated
		/// </summary>
		public event SharedVariableUpdatedEventHadler Updated;

		#endregion

		#region Methodos

		/// <summary>
		/// Gets the value of the the SharedVariable stored in cache
		/// </summary>
		/// <returns>The cached value of the SharedVariable represented by this object</returns>
		public abstract object GetCachedValue();

		/// <summary>
		/// Gets the value of the the SharedVariable stored in cache
		/// </summary>
		/// <returns>The cached value of the SharedVariable represented by this object</returns>
		public abstract T GetCachedValue<T>();

		/// <summary>
		/// Connects to the remote variable. If the remote variable does not exist it is created.
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		/// Gets a value indicating if the data provided from the blackboard for an update is valid
		/// </summary>
		/// <param name="variableType">The type of the variable specified by blackboard</param>
		/// <param name="variableName">The name of the variable specified by blackboard</param>
		/// <param name="isArray">Value that indicates if the variable specified by blackboard is an array</param>
		/// <param name="arraySize">The size of the variable specified by blackboard if it is an array</param>
		/// <param name="ex">When this method returns contains null if the variable was updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if provided data is suitable for update the variable, false otherwise</returns>
		protected virtual bool IsValidUpdateData(string variableType, bool isArray, int arraySize, string variableName, out Exception ex)
		{
			ex = null;
			if (String.Compare(this.TypeName, variableType) != 0)
			{
				ex = new ArgumentException("Incompatible types");
				return false;
			}
			if (this.IsArray != isArray)
			{
				ex = new ArgumentException("Incompatible types");
				return false;
			}
			if (String.Compare(this.Name, variableName) != 0)
			{
				ex = new ArgumentException("Incompatible names");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Gets the value stored in the blackboard for the SharedVariable represented by this object
		/// </summary>
		/// <returns>The value stored in the blackboard for the SharedVariable represented by this object</returns>
		//public abstract TSerializable Read<TSerializable>() where TSerializable : ISerializable;
		public abstract T Read<T>();

		/// <summary>
		/// Raises the ReportReceived event
		/// </summary>
		/// <param name="report">The report data received from Blackboard</param>
		protected virtual void OnReportReceived(SharedVariableReport report)
		{
			if (this.ReportReceived != null)
				this.ReportReceived(this, report);
		}

		/// <summary>
		/// Raises the Updated event
		/// </summary>
		protected virtual void OnUpdated()
		{
			if (this.Updated != null)
				this.Updated(this);
		}

		#region Subscription Methods

		/// <summary>
		/// Request the blackbard to notify each time the shared variable is written
		/// </summary>
		/// <remarks>The type of report is set to Notify and the type of subscription is set to WriteOthers</remarks>
		public virtual void Subscribe()
		{
			Subscribe(SharedVariableReportType.Notify, SharedVariableSubscriptionType.WriteOthers);
		}

		/// <summary>
		/// Request the blackbard to notify each time the shared variable is written
		/// </summary>
		/// <param name="reportType">The type of report. This afects the event that will be raised when the variable is written</param>
		/// <remarks>The type of subscription is set to WriteOthers</remarks>
		public virtual void Subscribe(SharedVariableReportType reportType)
		{
			Subscribe(reportType, SharedVariableSubscriptionType.WriteOthers);
		}

		/// <summary>
		/// Request the blackbard to notify each time the shared variable is written
		/// </summary>
		/// <param name="subscriptionType">The type of subscription</param>
		/// <remarks>The type of report is set to Notify and the type of subscription is set to WriteOthers</remarks>
		public virtual void Subscribe(SharedVariableSubscriptionType subscriptionType)
		{
			Subscribe(SharedVariableReportType.Notify, subscriptionType);
		}

		/// <summary>
		/// Request the blackbard to notify each time the shared variable is written
		/// </summary>
		/// <param name="reportType">The type of report. This afects the event that will be raised when the variable is written</param>
		/// <param name="subscriptionType">The type of subscription</param>
		public virtual void Subscribe(SharedVariableReportType reportType, SharedVariableSubscriptionType subscriptionType)
		{
			Subscribe(reportType, subscriptionType, 500, 3);
		}

		/// <summary>
		/// Request the blackbard to notify each time the shared variable is written
		/// </summary>
		/// <param name="reportType">The type of report. This afects the event that will be raised when the variable is written</param>
		/// <param name="subscriptionType">The type of subscription</param>
		/// <param name="attempts">The number of subscription attempts to perform</param>
		public virtual void Subscribe(SharedVariableReportType reportType, SharedVariableSubscriptionType subscriptionType, int attempts)
		{
			Subscribe(reportType, subscriptionType, 500, attempts);
		}

		/// <summary>
		/// Request the blackbard to notify each time the shared variable is written
		/// </summary>
		/// <param name="reportType">The type of report. This afects the event that will be raised when the variable is written</param>
		/// <param name="subscriptionType">The type of subscription</param>
		/// <param name="timeout">The timeout for each subscription attempt</param>
		public virtual void Subscribe(SharedVariableReportType reportType, SharedVariableSubscriptionType subscriptionType, TimeSpan timeout)
		{
			Subscribe(reportType, subscriptionType, timeout.Milliseconds, 3);
		}

		/// <summary>
		/// Request the blackbard to notify each time the shared variable is written
		/// </summary>
		/// <param name="reportType">The type of report. This afects the event that will be raised when the variable is written</param>
		/// <param name="subscriptionType">The type of subscription</param>
		/// <param name="timeout">The timeout for each subscription attempt</param>
		/// <param name="attempts">The number of subscription attempts to perform</param>
		public virtual void Subscribe(SharedVariableReportType reportType, SharedVariableSubscriptionType subscriptionType, TimeSpan timeout, int attempts)
		{
			Subscribe(reportType, subscriptionType, timeout.Milliseconds, attempts);
		}

		/// <summary>
		/// Request the blackbard to notify each time the shared variable is written
		/// </summary>
		/// <param name="reportType">The type of report. This afects the event that will be raised when the variable is written</param>
		/// <param name="subscriptionType">The type of subscription</param>
		/// <param name="timeout">The timeout for each subscription attempt in milliseconds</param>
		/// <param name="attempts">The number of subscription attempts to perform</param>
		public abstract void Subscribe(SharedVariableReportType reportType, SharedVariableSubscriptionType subscriptionType, int timeout, int attempts);

		/// <summary>
		/// Request the blackbard to stop notifying each time the shared variable is written
		/// </summary>
		public void Unsubscribe()
		{
			Unsubscribe(300, 3);
		}

		/// <summary>
		/// Request the blackbard to stop notifying each time the shared variable is written
		/// </summary>
		/// <param name="timeout">The timeout for each unsubscription attempt (of 3) in milliseconds</param>
		public void Unsubscribe(int timeout)
		{
			Unsubscribe(timeout, 3);
		}

		/// <summary>
		/// Request the blackbard to stop notifying each time the shared variable is written
		/// </summary>
		/// <param name="timeout">The timeout for each unsubscription attempt in milliseconds</param>
		/// <param name="attempts">The number of unsubscription attempts to perform</param>
		public abstract void Unsubscribe(int timeout, int attempts);

		#endregion

		/// <summary>
		/// Returns the string representation of the shared variable
		/// </summary>
		/// <returns>The string representation of the shared variable</returns>
		public override string ToString()
		{
			string s = this.TypeName;
			if (this.IsArray) s += "[]";
			s += " " + this.Name;
			return s;
		}

		/// <summary>
		/// Updates the SharedVariable object with data provided from the blackboard
		/// </summary>
		/// <param name="variableType">The type of the variable specified by blackboard</param>
		/// <param name="variableName">The name of the variable specified by blackboard</param>
		/// <param name="isArray">Value that indicates if the variable specified by blackboard is an array</param>
		/// <param name="arraySize">The size of the variable specified by blackboard if it is an array</param>
		/// <param name="data">The serialized data stored in blackboard</param>
		/// <param name="ex">When this method returns contains null if the variable was updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if variable was updated successfully, false otherwise</returns>
		internal bool Update(string variableType, bool isArray, int arraySize, string variableName, string data, out Exception ex)
		{
			if (!IsValidUpdateData(variableType, isArray, arraySize, variableName, out ex))
				return false;

			UpdateValue(data, out ex);
			return ex == null;
		}

		/// <summary>
		/// Performs a read operation and updates the data stored in cache
		/// </summary>
		/// <returns>true if the the data acquisition operation succeeded, false otherwise</returns>
		public virtual bool UpdateBufferedData()
		{
			return UpdateBufferedData(300);
		}

		/// <summary>
		/// Performs a read operation and updates the data stored in cache
		/// </summary>
		/// <param name="timeout">The amout of time to wait for a read confirmation from blackboard</param>
		/// <returns>true if the the data acquisition operation succeeded, false otherwise</returns>
		public abstract bool UpdateBufferedData(int timeout);

		/// <summary>
		/// Updates the SharedVariable object with data provided from the blackboard due to a subscription
		/// </summary>
		/// <param name="svReport">The report which contains the information for update</param>
		/// <param name="ex">When this method returns contains null if the variable was updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if variable was updated successfully, false otherwise</returns>
		internal abstract bool Update(SharedVariableReport svReport, out Exception ex);

		/// <summary>
		/// Updates the SharedVariable object with data provided from the blackboard due to a subscription
		/// </summary>
		/// <param name="variableType">The type of the variable specified by blackboard</param>
		/// <param name="variableName">The name of the variable specified by blackboard</param>
		/// <param name="isArray">Value that indicates if the variable specified by blackboard is an array</param>
		/// <param name="arraySize">The size of the variable specified by blackboard if it is an array</param>
		/// <param name="sData">The serialized data contained in the report</param>
		/// <param name="reportType">The type of report</param>
		/// <param name="subscriptionType">The type of subscription</param>
		/// <param name="writer">The name of the module which performed the write/create operation</param>
		/// <param name="ex">When this method returns contains null if the variable was updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if variable was updated successfully, false otherwise</returns>
		internal bool Update(string variableType, bool isArray, int arraySize, string variableName, string sData, SharedVariableReportType reportType,
			SharedVariableSubscriptionType subscriptionType, string writer, out Exception ex)
		{
			SharedVariableReport report;
			SharedVariableInfo variableInfo;

			variableInfo = new SharedVariableInfo(variableType, variableName, isArray, arraySize);
			report = new SharedVariableReport(variableInfo, sData, reportType, subscriptionType, writer);
			return Update(report, out ex);
		}

		/// <summary>
		/// Queries the Blackboard for updated information (writers and subscriptions) about the Shared Variable
		/// </summary>
		public void UpdateInfo()
		{
			Exception ex;
			if (!UpdateInfo(500, out ex))
				throw ex;
		}

		/// <summary>
		/// Queries the Blackboard for updated information (writers and subscriptions) about the Shared Variable
		/// </summary>
		/// <param name="timeout">The amout of time to wait for a stat confirmation from blackboard</param>
		/// <param name="ex">When this method returns contains null if the variable information was
		/// updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if the information was updated successfully, false otherwise</returns>
		protected internal bool UpdateInfo(int timeout, out Exception ex)
		{
			Command cmdStatVar;
			Response rspStatVar;
			SharedVariableInfo varInfo;

			ex = null;
			if (this.CommandManager == null)
			{
				ex = new Exception("Cannot communicate with blackboard");
				return false;
			}

			cmdStatVar = new Command("stat_var", this.Name);
			if (!this.CommandManager.SendAndWait(cmdStatVar, timeout, out rspStatVar))
			{
				ex = new Exception("No response from blackboard");
				return false;
			}
			if (!rspStatVar.Success)
			{
				ex = new Exception("Unsupported command" + this.Name);
				return false;
			}

			if (!SharedVariableInfo.Deserialize(rspStatVar.Parameters, out varInfo, out ex))
				return false;

			if ((String.Compare(this.Name, varInfo.Name, false) != 0) ||
				(String.Compare(this.TypeName, varInfo.TypeName, false) != 0))
			{
				ex = new Exception("Invalid response. Variable type/name mismatch.");
				return false;
			}

			this.creationTime = varInfo.CreationTime;
			this.allowedWriters = varInfo.AllowedWriters;
			this.subscriptions = varInfo.Subscriptions;
			return true;
		}

		/// <summary>
		/// Deserializes and updates the value of the shared variable
		/// </summary>
		/// <param name="serializedData">The serialized data containing the new value for the shared variable</param>
		/// <param name="ex">When this method returns contains null if the variable was updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if variable was updated successfully, false otherwise</returns>
		protected bool UpdateValue(string serializedData, out Exception ex)
		{
			return UpdateValue(serializedData, null, out ex);
		}

		/// <summary>
		/// Deserializes and updates the value of the shared variable
		/// </summary>
		/// <param name="serializedData">The serialized data containing the new value for the shared variable</param>
		/// <param name="writerModuleName">The name of the module which performs the write operation</param>
		/// <param name="ex">When this method returns contains null if the variable was updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if variable was updated successfully, false otherwise</returns>
		protected abstract bool UpdateValue(string serializedData, string writerModuleName, out Exception ex);

		/// <summary>
		/// Writes the provided value to the blackboard variable asociated to this SharedVariable object
		/// </summary>
		/// <param name="value">The value to write in to the blackboard shared variable</param>
		//public abstract void Write<TSerializable>(TSerializable value) where TSerializable : ISerializable;
		public abstract void Write<T>(T value);
		#endregion

		#region Static Variables

		/// <summary>
		/// Expression used to extract identifiers from variables
		/// </summary>
		protected static readonly string identifierExpression = @"[A-Z_a-z][0-9A-Z_a-z]*";

		/// <summary>
		/// Expression used to extract data from variables
		/// </summary>
		protected static readonly string dataExpression = @"(""(\\.|[^""])*""|\""(\\.|[^""])*\""|(?!\s*\}).)*";
		//protected static readonly string dataExpression = @".*";

		/*

		/// <summary>
		/// Expression used to validate SharedVariable objects. Capture groups: type, array, size, name and data.
		/// </summary>
		protected static readonly string sharedVariableExpression =
			@"(?<type>" + identifierExpression + ")" +
			@"(?<array>\[(?<size>\d*)\])?" +
			@"\s+" +
			@"(?<name>" + identifierExpression + ")" +
			@"\s+" +
			@"(?<data>" + dataExpression + ")";

		/// <summary>
		/// Regular expression used to validate variable types
		/// </summary>
		public static readonly Regex RxVarTypeValidator = new Regex(@"^"+identifierExpression+@"(\[\d*\])?$", RegexOptions.Compiled);

		/// <summary>
		/// Regular expression used to validate variable names
		/// </summary>
		public static readonly Regex RxVarNameValidator = new Regex(identifierExpression, RegexOptions.Compiled);

		/// <summary>
		/// Regular expression used to validate variable data
		/// </summary>
		public static readonly Regex RxVarDataValidator = new Regex(@"(?<data>"+dataExpression+")", RegexOptions.Compiled);

		/// <summary>
		/// Regular expression used to validate SharedVariable objects. Capture groups: type, array, size, name and data.
		/// </summary>
		public static readonly Regex RxSharedVariableValidator = new Regex(
			@"^\s*((" + sharedVariableExpression + @")|(\{\s*" + sharedVariableExpression + @"\s*\}))?\s*$",
			RegexOptions.Compiled);

		*/

		/// <summary>
		/// Regular expression used to validate notifications from blackboard
		/// </summary>
		public static readonly Regex RxSharedVariableNotificationValidator = new Regex(@"(\{\s*)?((?<type>" + identifierExpression + @")(?<array>\[(?<size>\d*)\])?\s+)?(?<name>" + identifierExpression + @")(\s+(?<data>" + dataExpression + @"))?(\s*\})?\s*\%?\s*(?<report>\w+)\s*\%\s*(?<subscription>\w+)\s*\%\s*(?<writer>[A-Z][0-9A-Z\-]*[0-9A-Z])", RegexOptions.Compiled);

		#endregion

		#region Static Properties
		#endregion

		#region Static Methodos

		/// <summary>
		/// Decodes the data contained in a received response. A return value indicates whether the operation succeeded
		/// </summary>
		/// <param name="response">Response which contains the data to parse</param>
		/// <param name="variableType">When this method returns contains the type of the variable coded in the response parameters if the conversion succeeded, or null if the conversion failed.</param>
		/// <param name="isArray">When this method returns is set to true if the conversion succeded and the variable coded in the response parameters is an array, false otherwise.</param>
		/// <param name="arrayLength">When this method returns contains the size of the array if the conversion succeded and the variable coded in the response parameters is an array, -1 otherwise.</param>
		/// <param name="variableName">When this method returns contains the name of the variable coded in the response parameters if the conversion succeeded, or null if the conversion failed.</param>
		/// <param name="variableData">When this method returns contains the content of the variable coded in the response parameters if the conversion succeeded, or the null if the conversion failed.</param>
		/// <returns>true if the the data extraction succeeded, false otherwise</returns>
		public static bool ParseResponse(Response response, out string variableType, out bool isArray, out int arrayLength, out string variableName, out string variableData)
		{
			return Parser.ParseSharedVariable(response.Parameters, out variableType, out isArray, out arrayLength, out variableName, out variableData);
		}

		#endregion
	}
}