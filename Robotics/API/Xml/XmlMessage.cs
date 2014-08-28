using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Robotics.API.Xml
{
	/// <summary>
	/// Base class for Xml messages
	/// </summary>
	[Serializable]
	[XmlRoot("message", IsNullable=true, Namespace="")]
	public abstract class XmlMessage
	{
		#region Variables

		/// <summary>
		/// Stores the Source of the message, like a ConnectionManager or a Form capable of manage responses
		/// </summary>
		protected IMessageSource messageSource;
		/// <summary>
		/// Stores aditional data provided by the source of the message, like an IPEndPoint or a Delegate
		/// </summary>
		protected object messageSourceMetadata;
		/// <summary>
		/// Stores the source module of the command
		/// </summary>
		protected string sourceModule;
		/// <summary>
		/// Stores the destination module of the command
		/// </summary>
		protected string destinationModule;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the object source of the message, like a ConnectionManager or a Form
		/// </summary>
		[XmlIgnore]
		public IMessageSource MessageSource
		{
			get { return messageSource; }
			internal set { messageSource = value; }
		}

		/// <summary>
		/// Gets or sets the aditional data provided by the source of the message, like an IPEndPoint or a Delegate
		/// </summary>
		[XmlIgnore]
		public object MessageSourceMetadata
		{
			get { return messageSourceMetadata; }
			set { messageSourceMetadata = value; }
		}

		/// <summary>
		/// Gets or Sets the source module of the command
		/// </summary>
		[XmlAttribute("sourceModule")]
		public virtual string SourceModule
		{
			get { return sourceModule; }
			set { this.sourceModule = value; }
		}

		/// <summary>
		/// Gets or Sets the destination module of the command
		/// </summary>
		[XmlAttribute("destinationModule")]
		public virtual string DestinationModule
		{
			get { return destinationModule; }
			set { this.destinationModule = value; }
		}

		/// <summary>
		/// The type of the XmlMessage (empty set accessor, return the name of the type)
		/// </summary>
		[XmlAttribute("type")]
		public virtual string MesssageType
		{
			get { return this.GetType().Name; }
			set { }
		}

		#endregion

		#region Static Members

		/// <summary>
		/// Serializes the object to a string which can be sent to a module
		/// </summary>
		/// <param name="message">The mesage object to serialize</param>
		/// <returns>A string representation of the serialized object</returns>
		public static string Serialize(XmlMessage message)
		{
			if (message == null)
				return String.Empty;

			StringBuilder sb = new StringBuilder(8192);
			XmlSerializer serializer = new XmlSerializer(message.GetType());
			using (StringWriter writer = new StringWriter(sb))
			{
				serializer.Serialize(writer, message);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Deserializes a XmlMessage from the provided string representation of the object
		/// </summary>
		/// <param name="serializedData">The serialized data that represents an XmlMesage object</param>
		/// <returns>An XmlMessage object</returns>
		public static XmlMessage Deserialize(string serializedData)
		{
			if (String.IsNullOrEmpty(serializedData))
				return null;

			XmlSerializer serializer = new XmlSerializer(typeof(XmlMessage));
			using (StringReader reader = new StringReader(serializedData))
			try
			{
				return (XmlMessage)serializer.Deserialize(reader);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Deserializes a XmlMessage from the provided string representation of the object
		/// </summary>
		/// <typeparam name="T">The XmlMessage inherited type</typeparam>
		/// <param name="serializedData">The serialized data that represents an XmlMesage object</param>
		/// <returns>The deserialized object</returns>
		public static T Deserialize<T>(string serializedData) where T : XmlMessage, new()
		{
			if (String.IsNullOrEmpty(serializedData))
				return null;

			XmlSerializer serializer = new XmlSerializer(typeof(T));
			using (StringReader reader = new StringReader(serializedData))
				try
				{
					return (T)serializer.Deserialize(reader);
				}
				catch (Exception)
				{
					return null;
				}
		}

		#endregion
	}
}
