using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Robotics.API;

namespace SharedVariableInspector
{
	[DefaultPropertyAttribute("Name")]
	public class SharedVariableInspectorWrapper
	{
		#region Variables

		private SharedVariable sharedVar;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SharedVariableInspectorWrapper
		/// </summary>
		public SharedVariableInspectorWrapper(SharedVariable sharedVar)
		{
			if (sharedVar == null)
				throw new ArgumentNullException();
			this.sharedVar = sharedVar;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the type of the SharedVariable
		/// </summary>
		[CategoryAttribute("General")]
		[DisplayName("Allowed Writers")]
		[DescriptionAttribute("The name of the modules with write permission")]
		[TypeConverter(typeof(DynamicObjectConverter))]
		public object AllowedWriters
		{
			get
			{
				if ((sharedVar.AllowedWriters == null) || (sharedVar.AllowedWriters.Length < 1))
					return "(all)";
				return sharedVar.AllowedWriters;
			}
		}

		/// <summary>
		/// Gets the creation time of the shared variable
		/// </summary>
		[CategoryAttribute("General")]
		[DisplayName("Creation time")]
		[DescriptionAttribute("The local time in the blackboard host machine when the variable was created")]
		public DateTime CreationTime
		{
			get { return this.sharedVar.CreationTime; }
		}

		/// <summary>
		/// Gets a value indicating if the variable has been initialized (created or readed from blackboard)
		/// </summary>
		[Browsable(false)]
		[CategoryAttribute("General")]
		[DescriptionAttribute("Indicating if the variable has been initialized (created or readed from the Blackboard)")]
		public bool Initialized { get { return this.sharedVar.Initialized; } }

		/// <summary>
		/// Gets the type of the SharedVariable
		/// </summary>
		[CategoryAttribute("General")]
		[DisplayName("Data type")]
		[DescriptionAttribute("The serialization/deserialization type of the data stored in the shared variable")]
		public Type InternalType
		{
			get { return sharedVar.Type; }
		}

		/// <summary>
		/// Gets the local time when the value of the shared variable was last updated
		/// </summary>
		[CategoryAttribute("Update")]
		[DisplayName("Last updated")]
		[DescriptionAttribute("The local time when the value of the shared variable was last updated")]
		public DateTime LastUpdated
		{
			get { return this.sharedVar.LastUpdated; }
		}

		/// <summary>
		/// Gets the name of the module wich performed the last write operation over the shared variable if known,
		/// otherwise it returns null.
		/// This property returns always null if there is not a subscription to the shared variable.
		/// </summary>
		[CategoryAttribute("Update")]
		[DisplayName("Last writer module")]
		[DescriptionAttribute("The name of the module wich performed the last write operation over the shared variable.")]
		public string LastWriter
		{
			get { return String.IsNullOrEmpty(this.sharedVar.LastWriter) ? "(Unknown)" : this.sharedVar.LastWriter; }
		}

		/// <summary>
		/// Gets the name of the SharedVariable
		/// </summary>
		[CategoryAttribute("General")]
		[DisplayName("Variable name")]
		[ParenthesizePropertyName(true)]
		[DescriptionAttribute("The name of the SharedVariable as stored in the Blackboard")]
		public string Name
		{
			get { return this.sharedVar.Name; }
		}

		/// <summary>
		/// Gets the report type for the current subscription to the shared variable
		/// </summary>
		[CategoryAttribute("Subscription (Local)")]
		[DisplayName("Report type")]
		[DescriptionAttribute("The report type for the current subscription to the shared variable.")]
		public SharedVariableReportType SubscriptionReportType
		{
			get { return this.sharedVar.SubscriptionReportType; }
		}

		/// <summary>
		/// Gets the subscription type for the current subscription to the shared variable
		/// </summary>
		[CategoryAttribute("Subscription (Local)")]
		[DisplayName("Subscription type")]
		[DescriptionAttribute("The subscription type for the current subscription to the shared variable.")]
		public SharedVariableSubscriptionType SubscriptionType
		{
			get { return this.sharedVar.SubscriptionType; }
		}

		/// <summary>
		/// Gets the global name of the type of the SharedVariable
		/// </summary>
		[CategoryAttribute("General")]
		[DisplayName("Variable type")]
		[DescriptionAttribute("The type of the shared variable as stored in the Blackboard")]
		public string TypeName
		{
			get {
				string sType = this.sharedVar.TypeName;
				if (this.sharedVar.IsArray)
				{
					sType += "[";
					if (this.sharedVar.Length > 0)
						sType += this.sharedVar.Length.ToString();
					sType += "]";
				}
				return sType;
			}
		}

		/// <summary>
		/// Gets the value of the variable stored in cache
		/// </summary>
		[CategoryAttribute("Value")]
		[DisplayName("Variable value")]
		[DescriptionAttribute("The value (stored in cache) of the shared variable")]
		[TypeConverter(typeof(DynamicObjectConverter))]
		public object Value
		{
			get
			{
				object oValue = this.sharedVar.GetCachedValue();
				/*
				if (oValue == null)
					return "(null)";
				else if ((oValue is String) && (String.IsNullOrEmpty((String)oValue)))
					return "(empty string)";
				*/
				return oValue;
			}
		}

		#endregion

		#region Subclasses

		private class DynamicObjectConverter : ExpandableObjectConverter//TypeConverter
		{
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return base.CanConvertTo(context, destinationType);
			}

			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return false;
				if (sourceType == typeof(string))
					return true;

				return base.CanConvertFrom(context, sourceType);
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				System.Collections.IEnumerable eValue = value as System.Collections.IEnumerable;
				//if((eValue == null) || (eValue is String))
				if(eValue == null)
					return base.GetProperties(context, value, attributes);
				List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();

				int ix = 0;
				foreach (object o in eValue)
					descriptors.Add(new ObjectPropertyDescriptor(ix++, o));
				descriptors.Insert(0, new ObjectPropertyDescriptor("(Length)", ix));
				return new PropertyDescriptorCollection(descriptors.ToArray());
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				return base.ConvertFrom(context, culture, value);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{

				System.Collections.IEnumerable eValue = value as System.Collections.IEnumerable;
				if((eValue == null) || (eValue is String) || (destinationType != typeof(String)))
					return base.ConvertTo(context, culture, value, destinationType);

				StringBuilder sb = new StringBuilder(1024);
				int count = 0;
				foreach (object o in eValue)
				{
					++count;
					if (sb.Length >= 1024)
						continue;
					sb.Append(o);
					sb.Append(", ");
				}
				if(count > 0)
					sb.Length -= 2;
				return value.GetType().Name + " (" + count.ToString() + "elements) {" + sb.ToString() + "}";
				
			}
		}

		private class ObjectPropertyDescriptor : PropertyDescriptor
		{
			private object o;

			public ObjectPropertyDescriptor(int index, object o)
				: this(index, o, null)
			{}

			public ObjectPropertyDescriptor(string attributeName, object o)
				: this(attributeName, o, null)
			{}

			public ObjectPropertyDescriptor(int index, object o, Attribute[] attrs)
				//: base("Index [" + index.ToString().PadLeft(3, ' ') + "]", attrs)
				: base("Index [" + index.ToString() + "]", attrs)
			{
				this.o = o;
			}

			public ObjectPropertyDescriptor(string attributeName, object o, Attribute[] attrs)
				: base(attributeName, attrs)
			{
				this.o = o;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get
				{
					if(o == null)
					return typeof(Object);
					return o.GetType();
				}
			}

			public override object GetValue(object component)
			{
				return o;
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{
				return;
			}

			public override void SetValue(object component, object value)
			{
				return;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}

		#endregion
	}
}
