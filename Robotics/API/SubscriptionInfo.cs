using System;

namespace Robotics.API
{
	/// <summary>
	/// Encapsulates information about a shared variable subscription
	/// </summary>
	[Serializable]
	public class SubscriptionInfo : ISharedVariableSubscriptionInfo
	{
		#region Variables

		/// <summary>
		/// The name of the subscriber module
		/// </summary>
		private string moduleName;

		/// <summary>
		/// The subscription type of the subscriber module
		/// </summary>
		private SharedVariableSubscriptionType subscriptionType;

		/// <summary>
		/// The report type for the subscription of the subscriber module
		/// </summary>
		private SharedVariableReportType reportType;

		/// <summary>
		/// The object which contains information about the shared variable related to the subscription
		/// </summary>
		private ISharedVariableInfo variableInfo;

		/// <summary>
		/// The name of the writer module for subscription types which allows to set an specific module
		/// </summary>
		private string writerModule;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SubscriptionInfo.
		/// Default construtor provided for serialization purposes only
		/// </summary>
		public SubscriptionInfo()
		{
			this.variableInfo = null;
			this.moduleName = null;
			this.writerModule = null;
			this.subscriptionType = SharedVariableSubscriptionType.None;
			this.reportType = SharedVariableReportType.None;
		}

		/// <summary>
		/// Initializes a new instance of SubscriptionInfo
		/// Construtor provided for serialization purposes only
		/// </summary>
		/// <param name="variableInfo">The object which contains information about the shared variable related to the subscription</param>
		internal SubscriptionInfo(ISharedVariableInfo variableInfo)
		{

			this.VariableInfo = variableInfo;
			this.moduleName = null;
			this.subscriptionType = SharedVariableSubscriptionType.None;
			this.reportType = SharedVariableReportType.None;
			this.writerModule = null;
		}

		/// <summary>
		/// Initializes a new instance of SubscriptionInfo
		/// </summary>
		/// <param name="variableInfo">The object which contains information about the shared variable related to the subscription</param>
		/// <param name="moduleName">The name of the subscriber module</param>
		/// <param name="subscriptionType">The subscription type for the subscriber module</param>
		/// <param name="reportType">The report type for the subscription of the subscriber module</param>
		public SubscriptionInfo(ISharedVariableInfo variableInfo, string moduleName,
			SharedVariableSubscriptionType subscriptionType, SharedVariableReportType reportType)
			: this(variableInfo, moduleName, subscriptionType, reportType, null) { }

		/// <summary>
		/// Initializes a new instance of SubscriptionInfo
		/// </summary>
		/// <param name="variableInfo">The object which contains information about the shared variable related to the subscription</param>
		/// <param name="moduleName">The name of the subscriber module</param>
		/// <param name="subscriptionType">The subscription type for the subscriber module</param>
		/// <param name="reportType">The report type for the subscription of the subscriber module</param>
		/// <param name="writerModule">The name of the writer module for subscription types which allows to set an specific module</param>
		public SubscriptionInfo(ISharedVariableInfo variableInfo, string moduleName,
			SharedVariableSubscriptionType subscriptionType, SharedVariableReportType reportType, string writerModule)
		{
			this.VariableInfo = variableInfo;
			this.ModuleName = moduleName;
			this.SubscriptionType = subscriptionType;
			this.ReportType = reportType;
			this.WriterModule = writerModule;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the name of the subscriber module
		/// </summary>
		public string ModuleName
		{
			get { return this.moduleName; }
			set
			{
				if (String.IsNullOrEmpty(value))
					throw new ArgumentNullException();
				this.moduleName = value;
			}
		}

		/// <summary>
		/// Gets or sets the subscription type of the subscriber module
		/// </summary>
		public SharedVariableSubscriptionType SubscriptionType
		{
			get { return this.subscriptionType; }
			set
			{
				if (value == SharedVariableSubscriptionType.Unknown)
					throw new ArgumentException("Invalid value"); this.subscriptionType = value;
			}
		}

		/// <summary>
		/// Gets or sets the report type for the subscription of the subscriber module
		/// </summary>
		public SharedVariableReportType ReportType
		{
			get { return this.reportType; }
			set
			{
				if (value == SharedVariableReportType.Unknown)
					throw new ArgumentException("Invalid value");
				this.reportType = value;
			}
		}

		/// <summary>
		/// Gets or sets information about the shared variable related to the subscription
		/// </summary>
		public ISharedVariableInfo VariableInfo
		{
			get { return this.variableInfo; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				this.variableInfo = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the writer module for subscription types which allows to set an specific module
		/// </summary>
		public string WriterModule
		{
			get { return this.writerModule; }
			set
			{
				if (this.subscriptionType != SharedVariableSubscriptionType.WriteModule)
				{
					this.writerModule = null;
					return;
				}
				else if (String.IsNullOrEmpty(value))
					throw new ArgumentNullException();
				this.writerModule = value;
			}
		}

		#endregion
	}
}
