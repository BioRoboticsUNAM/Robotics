using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tester
{
	[SerializableAttribute]
	internal class Config : ISerializable
	{
		#region Variables
		[NonSerializedAttribute]
		private static Config _default = new Config();
		
		private List<KeyValuePair<string, int>> modules;
		private List<AutoResponder> autoResponders;
		private List<Prompter> prompters;
		private List<QuickCommand> quickCommands;

		#endregion

		#region Constructors
		
		public Config()
		{
			modules = new List<KeyValuePair<string, int>>();
			autoResponders = new List<AutoResponder>();
			prompters = new List<Prompter>();
			quickCommands = new List<QuickCommand>();
		}

		public Config(SerializationInfo info, StreamingContext context)
		{
			modules = (List<KeyValuePair<string, int>>)info.GetValue("modules", typeof(List<KeyValuePair<string, int>>));
			autoResponders = (List<AutoResponder>)info.GetValue("autoResponders", typeof(List<AutoResponder>));
			prompters = (List<Prompter>)info.GetValue("prompters", typeof(List<Prompter>));
			quickCommands = (List<QuickCommand>)info.GetValue("quickCommands", typeof(List<QuickCommand>));
		}

		#endregion

		#region Properties
		
		public List<AutoResponder> AutoResponders
		{
			get { return autoResponders; }
		}

		public List<KeyValuePair<string, int>> Modules
		{
			get { return modules; }
		}

		public List<Prompter> Prompters
		{
			get { return prompters; }
		}

		public List<QuickCommand> QuickCommands
		{
			get { return quickCommands; }
		}

		#endregion

		#region Methodos

		public void Load()
		{
			BinaryFormatter formatter;
			FileStream stream;
			Config cfg;
			FileInfo fi;
			string filePath;

			try
			{
				fi = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
				filePath = fi.FullName.Remove(fi.FullName.LastIndexOf(fi.Extension)) + ".cfg";
				if (!File.Exists(filePath)) return;
				stream = File.OpenRead(filePath);

				formatter = new BinaryFormatter();
				cfg = (Config)formatter.Deserialize(stream);
				stream.Close();
			}
			catch { return; }
			this.autoResponders = cfg.autoResponders;
			this.modules = cfg.modules;
			this.prompters = cfg.prompters;
			this.quickCommands = cfg.quickCommands;
		}

		public void Save()
		{
			BinaryFormatter formatter;
			FileStream stream;
			FileInfo fi;
			string filePath;

			try
			{
				fi = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
				filePath = fi.FullName.Remove(fi.FullName.LastIndexOf(fi.Extension)) + ".cfg";
				stream = File.Open(filePath, FileMode.OpenOrCreate);

				formatter = new BinaryFormatter();
				formatter.Serialize(stream, this);
				stream.Close();

			}
			catch { return; }
			return;
		}

		#endregion

		#region Inherited Methodos

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("modules", modules, typeof(List<KeyValuePair<string, int>>));
			info.AddValue("autoResponders", autoResponders, typeof(List<AutoResponder>));
			info.AddValue("prompters", prompters, typeof(List<Prompter>));
			info.AddValue("quickCommands", quickCommands, typeof(List<QuickCommand>));
		}

		#endregion

		#endregion

		#region Static Properties
		
		public static Config Default
		{
			get { return _default; }
		}

		#endregion

		#region Static Methodos
		#endregion
	}
}
