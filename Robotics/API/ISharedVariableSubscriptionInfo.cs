using System;

namespace Robotics.API
{

	/// <summary>
	/// Gets subscription information about a shared variable
	/// </summary>
	public interface ISharedVariableSubscriptionInfo
	{
		/// <summary>
		/// Gets the name of the subscriber module
		/// </summary>
		string ModuleName { get; }

		/// <summary>
		/// Gets the subscription type of the subscriber module
		/// </summary>
		SharedVariableSubscriptionType SubscriptionType { get; }

		/// <summary>
		/// Gets the report type for the subscription of the subscriber module
		/// </summary>
		SharedVariableReportType ReportType { get; }

		/// <summary>
		/// Gets information about the shared variable related to the subscription
		/// </summary>
		ISharedVariableInfo VariableInfo { get; }

		/// <summary>
		/// Gets the name of the writer module for subscription types which allows to set an specific module
		/// </summary>
		string WriterModule { get; }
	}
}