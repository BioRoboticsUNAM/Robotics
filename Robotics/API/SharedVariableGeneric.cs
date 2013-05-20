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
	/// Represents a function that will serialize the provided object to a string.
	/// Serialized strings must have the format: "dataType variableName variableData" or "dataType variableName[arraySize] variableData"
	/// </summary>
	/// <typeparam name="T">Type of the object the function can serialize</typeparam>
	/// <param name="value">Object to be serialized</param>
	/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
	/// <returns>true if value was serialized successfully; otherwise, false</returns>
	public delegate bool SharedVariableStringSerializer<T>(T value, out string serializedData);
	/// <summary>
	/// Represents a function that will deserialize the provided object from a string
	/// Serialized strings have the format: "dataType variableName variableData" or "dataType variableName[arraySize] variableData"
	/// </summary>
	/// <typeparam name="T">Type of the object the function can deserialize</typeparam>
	/// <param name="serializedData">String containing the serialized object</param>
	/// <param name="value">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
	/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
	public delegate bool SharedVariableStringDeserializer<T>(string serializedData, out T value);

	/// <summary>
	/// Gets access to a variable stored in the Blackboard
	/// </summary>
	/// <typeparam name="T">The type of data stored in the shared variable</typeparam>
	public class SharedVariable<T> : SharedVariable
	{
		#region Variables

		/// <summary>
		/// Stores a copy of the last bufferedData
		/// </summary>
		protected T previousBufferedData = default(T);

		/// <summary>
		/// Stores a copy of the last readed, received or writed value of the shared variable
		/// </summary>
		protected T bufferedData;

		/// <summary>
		/// The CommandManager object used to communicate with the Blackboard
		/// </summary>
		private CommandManager commandManager;

		/// <summary>
		/// The default value provided in constructor for further initialization
		/// </summary>
		private readonly T defaultValue;

		/// <summary>
		/// Represents the deserialization function
		/// </summary>
		private SharedVariableStringDeserializer<T> deserializer;

		/// <summary>
		/// Indicates if the variable has been initialized
		/// </summary>
		private bool initialized;

		/// <summary>
		/// The local time when the value of the shared variable was last updated
		/// </summary>
		public DateTime lastUpdate;

		/// <summary>
		/// The name of the module wich performed the last write operation over the shared variable if known,
		/// otherwise it returns null.
		/// This property returns always null if there is not a subscription to the shared variable.
		/// </summary>
		public string lastWriter;

		/// <summary>
		/// Represents the serialization function
		/// </summary>
		private SharedVariableStringSerializer<T> serializer;

		/// <summary>
		/// The report type for the current subscription to the shared variable
		/// </summary>
		private SharedVariableReportType subscriptionReportType;
		
		/// <summary>
		/// The subscription type for the current subscription to the shared variable
		/// </summary>
		private SharedVariableSubscriptionType subscriptionType;

		/// <summary>
		/// The name of the variable in the Blackboard
		/// </summary>
		protected readonly string variableName;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public SharedVariable(CommandManager commandManager, string variableName, bool initialize)
			: this(commandManager, variableName, default(T), initialize) { }

		/// <summary>
		/// Initializes a new instance of SharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public SharedVariable(CommandManager commandManager, string variableName, T value, bool initialize)
		{
			this.initialized = false;
			this.commandManager = commandManager;
			this.subscriptionReportType = SharedVariableReportType.Unknown;
			this.subscriptionType = SharedVariableSubscriptionType.Unknown;
			this.variableName = variableName;
			this.lastUpdate = DateTime.MinValue;
			this.defaultValue = value;
			this.serializer = new SharedVariableStringSerializer<T>(Serialize);
			this.deserializer = new SharedVariableStringDeserializer<T>(Deserialize);
			if ((commandManager != null) && initialize)
				Initialize(value);
		}

		/// <summary>
		/// Initializes a new instance of SharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public SharedVariable(string variableName)
			: this(null, variableName, default(T), false) { }

		/// <summary>
		/// Initializes a new instance of SharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public SharedVariable(string variableName, T value)
			: this(null, variableName, value, false) { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets a buffered copy of the last readed, received or writed value of the shared variable
		/// </summary>
		public T BufferedData
		{
			get { return bufferedData; }
			protected set
			{
				previousBufferedData = bufferedData;
				bufferedData = value;
			}
		}

		/// <summary>
		/// Gets the CommandManager object used to communicate with the Blackboard
		/// </summary>
		public override CommandManager CommandManager
		{
			get { return commandManager; }
			internal set
			{
				if (value == null) throw new ArgumentNullException();
				this.commandManager = value;
				Initialize(this.defaultValue);
			}
		}

		/// <summary>
		/// Gets or sets the deserialization function
		/// </summary>
		private SharedVariableStringDeserializer<T> Deserializer
		{
			get { return deserializer; }
			set
			{
				if (value == null)
					deserializer = new SharedVariableStringDeserializer<T>(this.Deserialize);
				else
					deserializer = value;
			}
		}

		/// <summary>
		/// Gets a value indicating if the variable has been initialized (created or readed from blackboard)
		/// </summary>
		public override bool Initialized
		{
			get { return initialized; }
			internal set
			{
				initialized = value;
			}
		}

		/// <summary>
		/// Gets a value indicating if the variable is an array
		/// </summary>
		public override bool IsArray
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the local time when the value of the shared variable was last updated
		/// </summary>
		public override DateTime LastUpdated
		{
			get { return this.lastUpdate; }
		}
		
		/// <summary>
		/// Gets the name of the module wich performed the last write operation over the shared variable if known,
		/// otherwise it returns null.
		/// This property returns always null if there is not a subscription to the shared variable.
		/// </summary>
		public override string LastWriter
		{
			get { return this.lastWriter; }
			protected set { this.lastWriter = value; }
		}

		/// <summary>
		/// If the variable is an array gets the length of the array, else returns -1
		/// </summary>
		public override int Length { get { return -1; } }

		/// <summary>
		/// Gets the name of the SharedVariable
		/// </summary>
		public override string Name
		{
			get { return variableName; }
		}

		/// <summary>
		/// Gets or sets the serialization function
		/// </summary>
		private SharedVariableStringSerializer<T> Serializer
		{
			get { return serializer; }
			set
			{
				if (value == null)
					serializer = new SharedVariableStringSerializer<T>(this.Serialize);
				else
					serializer = value;
			}
		}

		/// <summary>
		/// Gets the report type for the current subscription to the shared variable
		/// </summary>
		public override SharedVariableReportType SubscriptionReportType
		{
			get { return this.subscriptionReportType; }
			protected set { this.subscriptionReportType = value; }
		}

		/// <summary>
		/// Gets the subscription type for the current subscription to the shared variable
		/// </summary>
		public override SharedVariableSubscriptionType SubscriptionType
		{
			get { return this.subscriptionType; }
			protected set { this.subscriptionType = value; }
		}

		/// <summary>
		/// Gets the type of the SharedVariable
		/// </summary>
		public override Type Type
		{
			get { return typeof(T); }
		}

		/// <summary>
		/// Gets the global name of the type of the SharedVariable
		/// </summary>
		public override string TypeName
		{
			get
			{
				return "var";
			}
		}

		/// <summary>
		/// Gets or sets the value of a shared variable
		/// </summary>
		public T Value
		{
			get
			{
				return Read();
			}
			set
			{
				Write(value);
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the value of the shared variable changes and new value is received
		/// </summary>
		public event SharedVariableSubscriptionReportEventHadler<T> ValueChanged;

		/// <summary>
		/// Occurs when the value of the shared variable changes and a notification is received
		/// </summary>
		public event SharedVariableSubscriptionReportEventHadler<T> WriteNotification;

		#endregion

		#region Methodos

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		protected virtual bool Deserialize(string serializedData, out T value)
		{
			value = default(T);
			if (String.IsNullOrEmpty(serializedData))
				return false;
			if (deserializer(serializedData, out value))
				return true;
			return false;
		}

		/// <summary>
		/// Deserializes the provided data to an object of the type of the variable
		/// </summary>
		/// <param name="data">The data to be deserialized</param>
		/// <returns>The object obtained from deserialization</returns>
		private T Deserialize(string data)
		{
			T value;
			if (!Deserialize(data, out value))
				throw new Exception("Deserialization error.");
			return value;
		}

		/// <summary>
		/// Gets the value of the the SharedVariable stored in cache
		/// </summary>
		/// <returns>The cached value of the SharedVariable represented by this object</returns>
		public override object GetCachedValue()
		{
			return bufferedData;
		}

		/// <summary>
		/// Gets the value of the the SharedVariable stored in cache
		/// </summary>
		/// <returns>The cached value of the SharedVariable represented by this object</returns>
		public override TSerializable GetCachedValue<TSerializable>()
		{
			if (typeof(T) != typeof(TSerializable))
				throw new ArgumentException("Invalid data type");
			return (TSerializable)((object)bufferedData);
		}

		/// <summary>
		/// Request the updated value of the variable from the Blackboard
		/// </summary>
		/// <param name="timeout">The amout of time to wait for a read confirmation from blackboard</param>
		/// <param name="serializedData">When this method returns contains the serialized value of the variable if it was fetched successfully, null otherwise</param>
		/// <param name="ex">When this method returns contains null if the new variable data was fetched successfully, or the exception to be thrown if the fetch failed</param>
		/// <returns>true if the new variable data was fetched successfully, false otherwise</returns>
		private bool GetUpdatedSerializedData(int timeout, out string serializedData, out Exception ex)
		{
			Command cmdReadVar;
			Response rspReadVar;

			serializedData = null;
			ex = null;
			cmdReadVar = new Command("read_var", variableName);
			if (!commandManager.SendAndWait(cmdReadVar, timeout, out rspReadVar))
			{
				ex = new Exception("No response from blackboard");
				return false;
			}
			if (!rspReadVar.Success)
			{
				ex = new Exception("Cannot read variable" + variableName);
				return false;
			}

			if (!XtractData(rspReadVar, out serializedData, out ex))
				return false;
			return true;
		}

		/// <summary>
		/// Connects to the remote variable. If the remote variable does not exist it is created.
		/// </summary>
		public override void Initialize()
		{
			Initialize(default(T));
		}

		/// <summary>
		/// Connects to the remote variable. If the remote variable does not exist it is created.
		/// </summary>
		/// <param name="value">The value to initialize the variable with if it does not exists</param>
		private void Initialize(T value)
		{
			if ((initialized) || (commandManager == null))
				return;

			Command command;
			Response response;
			Exception ex;
			bool success;
			string parameters;
			//int size;
			string data;
			T content;

			command = new Command("read_var", variableName);
			// Three initialization attempts
			success = false;
			response = null;
			for (int i = 0; i < 3; ++i)
			{
				if (success = commandManager.SendAndWait(command, 300, out response))
					break;
			}
			if (!success)
				return;

			// The variable exists and is responding
			if (response.Success)
			{
				initialized = true;
				if (!XtractData(response, out data, out ex))
					throw ex;
				if (!Deserialize(data, out content))
				{
					ex = new Exception("Error deserializing data", new Exception("Deserialization error with string: " + data));
					throw ex;
				}
				UpdateInfo();
				BufferedData = content;
				return;
			}

			// The variable does not exist. Try to create a new one
			//if (typeof(T).IsValueType)
			//{
			//	DecodeData(default(T), out data);
			//	size = data.Length;
			//}
			//else size = 0;
			//size = 0;

			parameters = "{ " + TypeName;
			if (IsArray)
			{
				parameters += "[";
				if (Length != -1)
					parameters += Length.ToString();
				parameters += "]";
			}
			parameters += " " + variableName + " }";
			command = new Command("create_var", parameters);
			if (!commandManager.SendAndWait(command, 300, out response))
				throw new Exception("Can not create variable in blackboard");
			TryWrite(value);
			UpdateInfo();
			initialized = true;
		}

		/// <summary>
		/// Raises the ValueChanged event
		/// </summary>
		/// <param name="report">The SharedVariableSubscriptionReport object which contain the report information</param>
		protected virtual void OnValueChanged(SharedVariableSubscriptionReport<T> report)
		{
			//if ((this.ValueChanged != null) && (report.ReportType != SharedVariableReportType.Notify) && (bufferedData == previousBufferedData))
			if ((this.ValueChanged != null) && (report.ReportType != SharedVariableReportType.Notify))
			{
				try
				{
					ValueChanged(report);
				}
				catch { }
			}
		}

		/// <summary>
		/// Raises the WriteNotification event
		/// </summary>
		/// <param name="report">The SharedVariableSubscriptionReport object which contain the report information</param>
		protected virtual void OnWriteNotification(SharedVariableSubscriptionReport<T> report)
		{
			if (WriteNotification != null)
			{
				try
				{
					WriteNotification(report);
				}
				catch { }
			}
		}

		/// <summary>
		/// Gets the value stored in the blackboard for the SharedVariable represented by this object
		/// </summary>
		/// <returns>The value stored in the blackboard for the SharedVariable represented by this object</returns>
		public T Read()
		{
			Exception ex;
			T value;
			if (!Read(out value, 300, out ex))
				throw ex;
			return value;
		}

		/// <summary>
		/// Gets the value stored in the blackboard for the SharedVariable represented by this object
		/// </summary>
		/// <param name="timeout">The amout of time to wait for a read confirmation from blackboard</param>
		/// <returns>The value stored in the blackboard for the SharedVariable represented by this object</returns>
		public T Read(int timeout)
		{
			Exception ex;
			T value;
			if (!Read(out value, timeout, out ex))
				throw ex;
			return value;
		}

		/// <summary>
		/// Gets the value stored in the blackboard for the SharedVariable represented by this object. A return value indicates whether the acquisition operation succeeded
		/// </summary>
		/// <param name="value">When this method returns, contains the value stored in the blackboard for the SharedVariable represented by this object, if the conversion succeeded, or the default value of the type if the conversion failed.</param>
		/// <param name="timeout">The amout of time to wait for a read confirmation from blackboard</param>
		/// <param name="ex">When this method returns false contains the exception generated</param>
		/// <returns>true if the the data acquisition operation succeeded, false otherwise</returns>
		protected bool Read(out T value, int timeout, out Exception ex)
		{
			ex = null;
			value = default(T);
			if (commandManager == null)
			{
				ex = new Exception("Cannot communicate with blackboard");
				return false;
			}

			string data;
			if (!GetUpdatedSerializedData(timeout, out data, out ex))
				return false;

			return UpdateValue(data, out value, out ex);
		}

		/// <summary>
		/// Gets the value stored in the blackboard for the SharedVariable represented by this object
		/// </summary>
		/// <returns>The value stored in the blackboard for the SharedVariable represented by this object</returns>
		public override TSerializable Read<TSerializable>()
		{
			if (typeof(T) != typeof(TSerializable))
				throw new ArgumentException("Invalid data type");
			return (TSerializable)((object)Read());
		}

		/// <summary>
		/// Request the update of the value of a variable in the Blackboard
		/// </summary>
		/// <param name="timeout">The amout of time to wait for a write confirmation from blackboard</param>
		/// <param name="value">The new value for the variable</param>
		/// <param name="ex">When this method returns contains null if the new variable data was written successfully, or the exception to be thrown if the write failed</param>
		/// <returns>true if the new variable data was written successfully, false otherwise</returns>
		private bool SendUpdatedSerializedData(int timeout, T value, out Exception ex)
		{
			Command cmdWriteVar;
			Response rspWriteVar;
			string parameters;
			string data;

			ex = null;
			if (!Serialize(value, out data))
			{
				ex = new Exception("Serialization error.");
				return false;
			}

			parameters = "{ " + TypeName;
			if (IsArray)
				parameters += "[" + (Length>=0 ? Length.ToString() : String.Empty) + "]";
			parameters += " " + variableName + " " + data + " }";

			cmdWriteVar = new Command("write_var", parameters);
			if (timeout != 0)
			{
				if (!commandManager.SendAndWait(cmdWriteVar, timeout, out rspWriteVar))
				{
					ex = new Exception("No response from blackboard");
					return false;
				}
				if (!rspWriteVar.Success)
				{
					ex = new Exception("Cannot write variable" + variableName);
					return false;
				}
			}
			else if (!commandManager.SendCommand(cmdWriteVar))
			{
				ex = new Exception("Cannot write variable" + variableName);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Sserializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		protected virtual bool Serialize(T value, out string serializedData)
		{
			if (!serializer(value, out serializedData))
				return false;
			return true;
		}

		/// <summary>
		/// Serializes the provided object to an array of bytes
		/// </summary>
		/// <param name="value">The object to be serialized</param>
		/// <returns>The array of bytes obtained from serialization</returns>
		private string Serialize(T value)
		{
			string serializedData;
			if (!Serialize(value, out serializedData))
				throw new Exception("Serialization error.");
			return serializedData;
		}

		/// <summary>
		/// Request the blackbard to notify each time the shared variable is written
		/// </summary>
		/// <param name="reportType">The type of report. This afects the event that will be raised when the variable is written</param>
		/// <param name="subscriptionType">The type of subscription</param>
		/// <param name="timeout">The timeout for each subscription attempt in milliseconds</param>
		/// <param name="attempts">The number of subscription attempts to perform</param>
		public override void Subscribe(SharedVariableReportType reportType, SharedVariableSubscriptionType subscriptionType, int timeout, int attempts)
		{
			if (this.commandManager == null)
				throw new Exception("Cannot communicate with blackboard");

			//if ((this.ValueChanged == null) && (this.WriteNotification == null))
			//	throw new Exception("Subscription failed. SharedVariable.ValueChanged or SharedVariable.WriteNotification events must be handled first");

			Command cmdSubscribe;
			Response rspSubscribe;
			string sType;
			string rType;
			string parameters;
			bool success;
			int count;

			#region Switch Subscription Type

			switch (subscriptionType)
			{
				case SharedVariableSubscriptionType.None:
					throw new ArgumentException("Invalid SharedVariableSubscriptionType value");
					//Unsubscribe();
					//return;

				case SharedVariableSubscriptionType.Unknown:
					throw new ArgumentException("Invalid SharedVariableSubscriptionType value");

				case SharedVariableSubscriptionType.Creation:
					sType = "creation";
					break;

				case SharedVariableSubscriptionType.WriteAny:
					sType = "writeany";
					break;

				case SharedVariableSubscriptionType.WriteOthers:
					sType = "writeothers";
					break;

				case SharedVariableSubscriptionType.WriteModule:
					sType = "writemodule";
					throw new Exception("Unsupported option");
				//break;

				default:
					sType = "writeothers";
					subscriptionType = SharedVariableSubscriptionType.WriteOthers;
					break;
			}

			#endregion

			#region Switch Report Type
			switch (reportType)
			{
				case SharedVariableReportType.None:
					throw new ArgumentException("Invalid SharedVariableReportType value");
					//Unsubscribe();
					//return;

				case SharedVariableReportType.Unknown:
					throw new ArgumentException("Invalid SharedVariableReportType value");

				case SharedVariableReportType.SendContent:

					rType = "content";
					break;

				case SharedVariableReportType.Notify:
					rType = "notify";
					break;

				default:
					rType = "notify";
					break;
			}
			#endregion

			parameters = variableName + " subscribe=" + sType + " report=" + rType;

			cmdSubscribe = new Command("subscribe_var", parameters);
			success = false;
			count = 0;
			do
			{
				++count;
				if (!commandManager.SendAndWait(cmdSubscribe, timeout, out rspSubscribe))
					continue;
				success = rspSubscribe.Success;

			} while (!success && (count < attempts));

			if (!success)
			{
				if (rspSubscribe == null)
					throw new Exception("No response from blackboard");
				throw new Exception("Subscription failed");
			}
			this.SubscriptionReportType = reportType;
			this.SubscriptionType = subscriptionType;
		}

		/// <summary>
		/// Returns the string representation of the shared variable
		/// </summary>
		/// <returns>The string representation of the shared variable</returns>
		public override string ToString()
		{
			string s = this.TypeName;
			if (this.IsArray) s += "[]";
			s += " " + this.Name + " = " + (bufferedData != null ? bufferedData.ToString() : "null");
			return s;
		}

		/// <summary>
		/// Gets the value stored in the blackboard for the SharedVariable represented by this object. A return value indicates whether the acquisition operation succeeded
		/// </summary>
		/// <param name="value">When this method returns, contains the value stored in the blackboard for the SharedVariable represented by this object, if the conversion succeeded, or the default value of the type if the conversion failed.</param>
		/// <returns>true if the the data acquisition operation succeeded, false otherwise</returns>
		public bool TryRead(out T value)
		{
			Exception ex;
			return Read(out value, 300, out ex);
		}

		/// <summary>
		/// Gets the value stored in the blackboard for the SharedVariable represented by this object. A return value indicates whether the acquisition operation succeeded
		/// </summary>
		/// <param name="value">When this method returns, contains the value stored in the blackboard for the SharedVariable represented by this object, if the conversion succeeded, or the default value of the type if the conversion failed.</param>
		/// <param name="timeout">The amout of time to wait for a read confirmation from blackboard</param>
		/// <returns>true if the the data acquisition operation succeeded, false otherwise</returns>
		public bool TryRead(out T value, int timeout)
		{
			Exception ex;
			return Read(out value, timeout, out ex);
		}

		/// <summary>
		/// Writes the provided value to the blackboard variable asociated to this SharedVariable object. A return value indicates whether the write operation succeeded
		/// </summary>
		/// <param name="value">The value to write in to the blackboard shared variable</param>
		/// <returns>true if the the data write operation succeeded, false otherwise</returns>
		public bool TryWrite(T value)
		{
			Exception ex;
			return Write(value, 300, out ex);
		}

		/// <summary>
		/// Writes the provided value to the blackboard variable asociated to this SharedVariable object. A return value indicates whether the write operation succeeded
		/// </summary>
		/// <param name="value">The value to write in to the blackboard shared variable</param>
		/// <param name="timeout">The amout of time to wait for a write confirmation from blackboard. Use Zero to write without wait for confirmation. Use -1 to wait indefinitely</param>
		/// <returns>true if the the data write operation succeeded, false otherwise</returns>
		public bool TryWrite(T value, int timeout)
		{
			Exception ex;
			return Write(value, timeout, out ex);
		}

		/// <summary>
		/// Request the blackbard to stop notifying each time the shared variable is written
		/// </summary>
		/// <param name="timeout">The timeout for each unsubscription attempt in milliseconds</param>
		/// <param name="attempts">The number of unsubscription attempts to perform</param>
		public override void Unsubscribe(int timeout, int attempts)
		{
			//subscribe_var
			if (this.commandManager == null)
				throw new Exception("Cannot communicate with blackboard");

			Command cmdUnsubscribe;
			Response rspUnsubscribe;
			bool success;
			int count;

			cmdUnsubscribe = new Command("unsubscribe_var", variableName);
			success = false;
			count = 0;
			do
			{
				++count;
				if (!commandManager.SendAndWait(cmdUnsubscribe, timeout, out rspUnsubscribe))
					continue;
				success = rspUnsubscribe.Success;

			} while (!success && (count < attempts));

			if (!success)
			{
				if (rspUnsubscribe == null)
					throw new Exception("No response from blackboard");
				throw new Exception("Subscription removal failed");
			}
			this.SubscriptionType = SharedVariableSubscriptionType.None;
			this.SubscriptionReportType = SharedVariableReportType.None;
		}

		/// <summary>
		/// Updates the SharedVariable object with data provided from the blackboard due to a subscription
		/// </summary>
		/// <param name="svReport">The report which contains the information for update</param>
		/// <param name="ex">When this method returns contains null if the variable was updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if variable was updated successfully, false otherwise</returns>
		internal override bool Update(SharedVariableReport svReport, out Exception ex)
		{
			OnReportReceived(svReport);
			if (!IsValidUpdateData(svReport.VariableInfo.TypeName, svReport.VariableInfo.IsArray, svReport.VariableInfo.Length, svReport.VariableInfo.Name, out ex))
				return false;

			T value;
			T oldValue = BufferedData;
			if (!UpdateValue(svReport.SerializedData, svReport.Writer, out value, out ex))
				return false;
			SharedVariableSubscriptionReport<T> report;

			//report = new SharedVariableSubscriptionReport<T>(this, reportType, subscriptionType, writer, value, reportString);
			report = new SharedVariableSubscriptionReport<T>(this, svReport.ReportType, svReport.SubscriptionType, svReport.Writer, value);

			OnWriteNotification(report);
			// Check if value changed
			IComparable<T> cValue = value as IComparable<T>;
			if((cValue != null) && (cValue.CompareTo(oldValue) != 0))
				OnValueChanged(report);
			else if(!System.Collections.Generic.EqualityComparer<T>.Default.Equals(oldValue, value))
				OnValueChanged(report);
			return true;
		}

		/// <summary>
		/// Deserializes and updates the value of the shared variable
		/// </summary>
		/// <param name="serializedData">The serialized data containing the new value for the shared variable</param>
		/// <param name="writerModuleName">The name of the module which performs the write operation</param>
		/// <param name="ex">When this method returns contains null if the variable was updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if variable was updated successfully, false otherwise</returns>
		protected override bool UpdateValue(string serializedData, string writerModuleName, out Exception ex)
		{
			T value;
			return UpdateValue(serializedData, writerModuleName, out value, out ex);
		}

		/// <summary>
		/// Performs a read operation and updates the data stored in cache
		/// </summary>
		/// <param name="timeout">The amout of time to wait for a read confirmation from blackboard</param>
		/// <returns>true if the the data acquisition operation succeeded, false otherwise</returns>
		public override bool UpdateBufferedData(int timeout)
		{
			T value;
			return TryRead(out value, timeout);
		}
		
		/// <summary>
		/// Deserializes and updates the value of the shared variable
		/// </summary>
		/// <param name="serializedData">The serialized data containing the new value for the shared variable</param>
		/// <param name="value">When this method returns contains the new value for the shared variable if the variable was updated successfully, or the default value for T if the update failed</param>
		/// <param name="ex">When this method returns contains null if the variable was updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if variable was updated successfully, false otherwise</returns>
		protected bool UpdateValue(string serializedData, out T value, out Exception ex)
		{
			return UpdateValue(serializedData, null, out value, out ex);
		}

		/// <summary>
		/// Deserializes and updates the value of the shared variable
		/// </summary>
		/// <param name="serializedData">The serialized data containing the new value for the shared variable</param>
		/// <param name="writerModuleName">The name of the module which performs the write operation</param>
		/// <param name="value">When this method returns contains the new value for the shared variable if the variable was updated successfully, or the default value for T if the update failed</param>
		/// <param name="ex">When this method returns contains null if the variable was updated successfully, or the exception to be thrown if the update failed</param>
		/// <returns>true if variable was updated successfully, false otherwise</returns>
		protected bool UpdateValue(string serializedData, string writerModuleName, out T value, out Exception ex)
		{
			ex = null;
			value = default(T);

			try
			{
				value = Deserialize(serializedData);
				this.LastWriter = writerModuleName;
				this.BufferedData = value;
				this.lastUpdate = DateTime.Now;
			}
			catch (Exception dex)
			{
				ex = dex;
				return false;
			}
			OnUpdated();
			return true;
		}

		/// <summary>
		/// Writes the provided value to the blackboard variable asociated to this SharedVariable object
		/// </summary>
		/// <param name="value">The value to write in to the blackboard shared variable</param>
		public void Write(T value)
		{
			Exception ex;
			if (!Write(value, 300, out ex))
				throw ex;
		}

		/// <summary>
		/// Writes the provided value to the blackboard variable asociated to this SharedVariable object
		/// </summary>
		/// <param name="value">The value to write in to the blackboard shared variable</param>
		/// <param name="timeout">The amout of time to wait for a write confirmation from blackboard. Use Zero to write without wait for confirmation. Use -1 to wait indefinitely</param>
		public void Write(T value, int timeout)
		{
			Exception ex;
			if (!Write(value, timeout, out ex))
				throw ex;
		}

		/// <summary>
		/// Writes the provided value to the blackboard variable asociated to this SharedVariable object. A return value indicates whether the write operation succeeded
		/// </summary>
		/// <param name="value">The value to write in to the blackboard shared variable</param>
		/// <param name="timeout">The amout of time to wait for a write confirmation from blackboard</param>
		/// <param name="ex">When this method returns false contains the exception generated</param>
		/// <returns>true if the the data write operation succeeded, false otherwise</returns>
		protected bool Write(T value, int timeout, out Exception ex)
		{
			ex = null;
			if (commandManager == null)
			{
				ex = new Exception("Cannot communicate with blackboard");
				return false;
			}

			if (!SendUpdatedSerializedData(timeout, value, out ex))
				return false;
			BufferedData = value;
			this.lastUpdate = DateTime.Now;
			OnUpdated();
			return true;
		}

		/// <summary>
		/// Writes the provided value to the blackboard variable asociated to this SharedVariable object
		/// </summary>
		/// <param name="value">The value to write in to the blackboard shared variable</param>
		public override void Write<TSerializable>(TSerializable value)
		{
			if (typeof(TSerializable) != this.Type)
				throw new ArgumentException("Invalid data type");
			Write((T)((object)value));
		}

		/// <summary>
		/// Extracts the variable data from a response
		/// </summary>
		/// <param name="response">Response which contains the data to extract</param>
		/// <param name="variableData">When this method returns contains the content of the variable coded in the response parameters if the conversion succeeded, or the null if the conversion failed.</param>
		/// <param name="ex">Exeption to be thrown when extraction fails</param>
		/// <returns>true if extraction succeeded, false otherwise</returns>
		protected virtual bool XtractData(Response response, out string variableData, out Exception ex)
		{
			string variableType;
			bool isArray;
			int arrayLength;
			string variableName;

			ex = null;
			variableData = null;
			if (!ParseResponse(response, out variableType, out isArray, out arrayLength, out variableName, out variableData))
			{
				ex = new Exception("Response parameters has an invalid format");
				return false;
			}
			if (String.Compare(this.variableName, variableName) != 0)
			{
				ex = new Exception("Variable name mismatch");
				return false;
			}
			if (String.Compare(this.TypeName, variableType) != 0)
			{
				ex = new Exception("Variable type mismatch");
				return false;
			}
			return true;
		}

		#endregion

		#region Static Methods and Operators

		/// <summary>
		/// Converts a Shared Variable to the data type that it represents, returning the cached value
		/// </summary>
		/// <param name="variable">The shared variable from where get the value</param>
		/// <returns>The cached value in the hared variable object</returns>
		public static explicit operator T(SharedVariable<T> variable)
		{
			if (variable == null) return default(T);
			if (!variable.Initialized)
				throw new ArgumentException("Uninitialized variable. Add it to a CommandManager object first");
			return variable.BufferedData;
		}

		//public static implicit operator SharedVariable<T>(T value)
		//{
		//}

		#endregion
	}
}
