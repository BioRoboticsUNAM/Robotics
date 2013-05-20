using System;
using Robotics;
using Robotics.API;

namespace Robotics.PacIto
{
	public partial class PacItoCommandManager
	{
		/// <summary>
		/// Controls remotely the speech recognizer of Pac-Ito
		/// </summary>
		public class PacItoSpeechRecognizer : SharedResource
		{
			#region Variables

			/// <summary>
			/// Stores the reference to the CommandManager which this object serves
			/// </summary>
			private PacItoCommandManager cmdMan;

			/// <summary>
			/// Stores the default delay time for speech recognizer commands
			/// </summary>
			private int defaultDelay = 1000;

			#region Signatures

			/// <summary>
			/// Signature to parse sprec_na responses
			/// </summary>
			private Signature sgnNa;

			/// <summary>
			/// Signature to parse spr_raw responses
			/// </summary>
			private Signature sgnRaw;

			/// <summary>
			/// Signature to parse spr_status responses
			/// </summary>
			private Signature sgnStatus;

			/// <summary>
			/// Signature to parse spr_grammar responses
			/// </summary>
			private Signature sgnGrammar;

			#endregion

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of PacItoSpeechRecognizer
			/// <param name="cmdMan">The reference to the CommandManager which this object serves</param>
			/// </summary>
			internal PacItoSpeechRecognizer(PacItoCommandManager cmdMan)
				: base()
			{
				this.cmdMan = cmdMan;
				CreateSignatures();
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets the CommandManager which this object serves
			/// </summary>
			protected PacItoCommandManager CmdMan
			{
				get { return cmdMan; }
			}

			/// <summary>
			/// Gets or Sets the default delay time for speech recognizer commands
			/// </summary>
			public int DefaultDelay
			{
				get { return defaultDelay; }
				set
				{
					if ((value < 100) || (value > 120000)) throw new ArgumentOutOfRangeException();
					defaultDelay = value;
				}
			}

			/// <summary>
			/// Gets or sets a value indicating whether the SpeechRecognizer is enabled
			/// </summary>
			public bool Enabled
			{
				get
				{
					string stat = "get";
					if (!Status(ref stat))
						throw new Exception("Communication problem");
					if (stat.StartsWith("enable"))
						return true;
					else if (stat.StartsWith("disable"))
						return false;
					else
						throw new Exception("Communication problem");
				}
				set
				{
					if (value) Enable();
					else Disable();
				}
			}

			/// <summary>
			/// Gets or sets the grammar file used by the Speech Recognizer
			/// </summary>
			public string Grammar
			{
				get
				{
					string path = "get";
					if (!GetGrammar(out path))
						throw new Exception("Communication problem");
					return path;
				}
				set
				{
					if (!LoadGrammar(value))
						throw new Exception("Communication problem");
				}
			}

			#region Signatures

			/// <summary>
			/// Gets the Signature to parse sprec_raw responses
			/// </summary>
			public virtual Signature SgnRaw
			{ get { return sgnRaw; } }

			/// <summary>
			/// Gets the Signature to parse sprec_status responses
			/// </summary>
			public virtual Signature SgnStatus
			{ get { return sgnStatus; } }

			/// <summary>
			/// Gets the Signature to parse load_grammar responses
			/// </summary>
			public virtual Signature SgnGrammar
			{ get { return sgnGrammar; } }

			#endregion

			#endregion

			#region Methodos

			/// <summary>
			/// Creates the Base Signatures
			/// </summary>
			private void CreateSignatures()
			{
				SignatureBuilder sb = new SignatureBuilder();
				sb.AddNewFromTypes(typeof(string));
				//sgnNa = sb.GenerateSignature("sprec_na");
				//sgnEna = sb.GenerateSignature("sprec_ena");
				//sgnRaw = sb.GenerateSignature("sprec_raw");
				//sgnStatus = sb.GenerateSignature("sprec_status");
				//sgnLoadGrammar = sb.GenerateSignature("load_grammar");
				//sgnGetGrammar = sb.GenerateSignature("get_grammar");

				sgnNa = sb.GenerateSignature("sprec_na");
				sgnRaw = sb.GenerateSignature("spr_raw");
				sgnStatus = sb.GenerateSignature("spr_status");
				sgnGrammar = sb.GenerateSignature("spr_grammar");
			}

			#region Enabling methods

			/// <summary>
			/// Commnds the SpeechRecognizer to enable the speech recognition
			/// </summary>
			/// <returns>true if speech recognition was enabled, false otherwise</returns>
			public bool Enable()
			{
				string stat = "enable";
				if (!Status(ref stat) || !stat.StartsWith("enable"))
					return false;
				return true;
			}

			/// <summary>
			/// Commnds the SpeechRecognizer to enable the speech recognition
			/// </summary>
			/// <returns>true if speech recognition was disabled, false otherwise</returns>
			public bool Disable()
			{
				string stat = "disable";
				if (!Status(ref stat) || !stat.StartsWith("disable"))
					return false;
				return true;
			}

			/// <summary>
			/// Commnds the SpeechRecognizer to enable, disable or get the status of the the speech recognition
			/// </summary>
			/// <param name="stat">Parameter to include in de sprec_status command.</param>
			/// <returns>true if command executed successfully. false otherwise</returns>
			private bool Status(ref string stat)
			{
				// Stores the command to be sent to speech generator
				Command cmdStatus;
				// Stores the response from speech generator and the candidate while lookingfor
				Response rspStatus = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				if ((stat != "enable") && (stat != "disable") && (stat != "get"))
					return false;

				cmdStatus = new Command(sgnStatus.CommandName, stat, CmdMan.AutoId++);

				// 2. Send the status command			
				if (!CmdMan.SendAndWait(cmdStatus, 300, out rspStatus))
				{
					// 2.1. Cant send socket. Operation canceled.
					FreeResource();
					return false;
				}

				// 3. End of command execution
				FreeResource();

				// 3.2. Parse speech generator response
				if (!rspStatus.Success)
				{
					// 3.2. Failed response
					return false;
				}

				// 4.0 Recover values from response
				stat = rspStatus.Parameters;

				return true;
			}

			#endregion

			#region Grammar Methods

			/// <summary>
			/// Commnds the SpeechRecognizer to load the specified grammar file
			/// </summary>
			/// <param name="grammarFile">The grammar file to load</param>
			/// <returns>true if grammar file was loaded, false otherwise</returns>
			public bool LoadGrammar(string grammarFile)
			{
				// Stores the command to be sent to speech generator
				Command cmdLoadGrammar;
				// Stores the response from speech generator and the candidate while lookingfor
				Response rspLoadGrammar = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdLoadGrammar = new Command(sgnGrammar.CommandName, grammarFile, CmdMan.AutoId++);

				// 2. Send the status command			
				if (!CmdMan.SendAndWait(cmdLoadGrammar, DefaultDelay, out rspLoadGrammar))
				{
					// 2.1. Cant send socket. Operation canceled.
					FreeResource();
					return false;
				}

				// 3. End of command execution
				FreeResource();

				// 3.2. Parse speech generator response
				if (!rspLoadGrammar.Success)
				{
					// 3.2. Failed response
					return false;
				}
				return true;
			}

			/// <summary>
			/// Retrieves the grammar file name of the SpeechRecognizer
			/// </summary>
			/// <param name="grammarFile">The name of the grammar file loaded</param>
			/// <returns>true if grammar file name was retrieved successflly, false otherwise</returns>
			public bool GetGrammar(out string grammarFile)
			{
				// Stores the command to be sent to speech generator
				Command cmdGetGrammar;
				// Stores the response from speech generator and the candidate while lookingfor
				Response rspGetGrammar = null;
				grammarFile = "none";

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdGetGrammar = new Command(sgnGrammar.CommandName, "", CmdMan.AutoId++);

				// 2. Send the status command			
				if (!CmdMan.SendAndWait(cmdGetGrammar, DefaultDelay, out rspGetGrammar))
				{
					// 2.1. Cant send socket. Operation canceled.
					FreeResource();
					return false;
				}

				// 3. End of command execution
				FreeResource();

				// 3.2. Parse speech generator response
				if (!rspGetGrammar.Success)
				{
					// 3.2. Failed response
					return false;
				}

				// 4.0 Recover values from response
				grammarFile = rspGetGrammar.Parameters;
				return true;
			}

			#endregion

			#endregion

		}
	}
}
