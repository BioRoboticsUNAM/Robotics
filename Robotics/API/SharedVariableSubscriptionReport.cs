using System;
using System.Runtime.Serialization;

namespace Robotics.API
{
	/// <summary>
	/// Represents a modification report of a shared variable
	/// </summary>
	/// <typeparam name="T">The type of data stored in the shared variable</typeparam>
	public class SharedVariableSubscriptionReport<T>
	{
		/// <summary>
		/// The SharedVariable object that generates this report
		/// </summary>
		protected readonly SharedVariable variable;

		/// <summary>
		/// The type of report received
		/// </summary>
		protected readonly SharedVariableReportType reportType;

		/// <summary>
		/// The type of subscription of the report received
		/// </summary>
		protected readonly SharedVariableSubscriptionType subscriptionType;

		/// <summary>
		/// The name of the module that performed the write or create operation
		/// </summary>
		protected readonly string writer;

		/// <summary>
		/// The value of the data contained in the report
		/// </summary>
		protected readonly T value;

		/// <summary>
		/// Initializes a new instance of SharedVariableSubscriptionReport
		/// </summary>
		/// <param name="variable">The SharedVariable object that generates this report</param>
		/// <param name="subscriptionType">The type of subscription of the report received</param>
		/// <param name="reportType">The type of report received</param>
		/// <param name="writer">The name of the module that performed the write or create operation</param>
		/// <param name="value">The value of the data contained in the report</param>
		public SharedVariableSubscriptionReport(SharedVariable variable, SharedVariableReportType reportType, SharedVariableSubscriptionType subscriptionType, string writer, T value)
		{
			if (variable == null)
				throw new ArgumentNullException();
			this.reportType = reportType;
			this.subscriptionType = subscriptionType;
			this.value = value;
			this.variable = variable;
			this.writer = writer;
		}

		/// <summary>
		/// Gets the SharedVariable object that generates this report
		/// </summary>
		public SharedVariable Variable
		{
			get { return variable; }
		}

		/// <summary>
		/// Gets the type of report received
		/// </summary>
		public SharedVariableReportType ReportType
		{
			get { return reportType; }
		}

		/// <summary>
		/// Gets the type of subscription of the report received
		/// </summary>
		public SharedVariableSubscriptionType SubscriptionType
		{
			get { return subscriptionType; }
		}

		/// <summary>
		/// Gets the name of the module that performed the write or create operation
		/// </summary>
		public string WriterName
		{
			get { return writer; }
		}

		/// <summary>
		/// Gets the value of the data contained in the report
		/// </summary>
		public T Value
		{
			get { return value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string s = "ShVReport<" + typeof(T).Name + ">{ " + variable.TypeName;
			if (variable.IsArray)
			{
				s += "[";
				if (variable.Length > -1) s += variable.Length.ToString();
				s += "]";
			}
			s += " " + variable.Name + " " + value.ToString() + " } " +
				subscriptionType.ToString() + " % " +
				reportType.ToString() + " % " +
				writer;
			return s;
		}

	}
}
