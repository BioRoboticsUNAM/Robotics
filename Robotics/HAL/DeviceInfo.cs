using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Stores information about a Laser
	/// </summary>
	public struct DeviceInfo
	{
		private string model;
		private string vendor;
		private string product;
		private string firmware;
		private string protocol;
		private string sn;

		internal DeviceInfo(string model, string vendor, string product, string firmware, string protocol, string sn)
		{
			this.model = model;
			this.vendor = vendor;
			this.product = product;
			this.firmware = firmware;
			this.protocol = protocol;
			this.sn = sn;
		}


		/// <summary>
		/// Returns the model of the component
		/// </summary>
		public string Model
		{
			get { return model; }
		}

		/// <summary>
		/// Returns the firware version of the component
		/// </summary>
		public string FirmwareVersion
		{
			get { return firmware; }
		}

		/// <summary>
		/// Returns the product name of the component
		/// </summary>
		public string ProductName
		{
			get { return product; }
		}

		/// <summary>
		/// Returns the protocol version used by the component
		/// </summary>
		public string ProtocolVersion
		{
			get { return protocol; }
		}

		/// <summary>
		/// Returns the component's serial number
		/// </summary>
		public string SerialNumber
		{
			get { return sn; }
		}
		/// <summary>
		/// Returns the vendor name of the component
		/// </summary>
		public string VendorName
		{
			get { return vendor; }
		}
	}
}
