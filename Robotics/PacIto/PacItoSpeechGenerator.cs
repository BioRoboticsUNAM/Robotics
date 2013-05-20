using System;
using Robotics;
using Robotics.API;

namespace Robotics.PacIto
{
	public partial class PacItoCommandManager
	{
		/// <summary>
		/// Controls remotely the speech generator of Pac-Ito
		/// </summary>
		public class PacItoSpeechGenerator : SharedResource
		{
			#region Variables

			/// <summary>
			/// Stores the reference to the CommandManager which this object serves
			/// </summary>
			private PacItoCommandManager cmdMan;

			/// <summary>
			/// Stores the default delay time for speech generator commands
			/// </summary>
			private int defaultDelay = 30000;

			#region Signatures

			/// <summary>
			/// Signature to parse spg_say responses
			/// </summary>
			private Signature sgnSay;

			/// <summary>
			/// Signature to parse spg_asay responses
			/// </summary>
			private Signature sgnAsyncSay;

			/// <summary>
			/// Signature to parse spg_shutup responses
			/// </summary>
			private Signature sgnShutUp;

			/// <summary>
			/// Signature to parse spg_read responses
			/// </summary>
			private Signature sgnRead;

			/// <summary>
			/// Signature to parse spg_aread responses
			/// </summary>
			private Signature sgnAsyncRead;

			/// <summary>
			/// Signature to parse spg_voice responses
			/// </summary>
			private Signature sgnVoice;

			#endregion

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of PacItoSpeechGenerator
			/// <param name="cmdMan">The reference to the CommandManager which this object serves</param>
			/// </summary>
			internal PacItoSpeechGenerator(PacItoCommandManager cmdMan)
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
			/// Gets or Sets the default delay time for speech generator commands
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

			#region Signatures

			/// <summary>
			/// Gets the Signature to parse spg_say responses
			/// </summary>
			public virtual Signature SgnSay
			{
				get { return sgnSay; }
			}

			/// <summary>
			/// Gets the Signature to parse spg_asay responses
			/// </summary>
			public virtual Signature SgnAsyncSay
			{
				get { return sgnAsyncSay; }
			}

			/// <summary>
			/// Gets the Signature to parse shutup responses
			/// </summary>
			public virtual Signature SgnShutUp
			{
				get { return sgnShutUp; }
			}

			/// <summary>
			/// Gets the Signature to parse spg_read responses
			/// </summary>
			public virtual Signature SgnRead
			{
				get { return sgnRead; }
			}

			/// <summary>
			/// Gets the Signature to parse spg_aread responses
			/// </summary>
			public virtual Signature SgnAsyncRead
			{
				get { return sgnAsyncRead; }
			}

			/// <summary>
			/// Gets the Signature to parse spg_voice responses
			/// </summary>
			public virtual Signature SgnVoice
			{
				get { return sgnVoice; }
			}

			#endregion

			#endregion

			#region Methodos

			/// <summary>
			/// Creates the SpeechGenerator Signatures
			/// </summary>
			private void CreateSignatures()
			{
				SignatureBuilder sb = new SignatureBuilder();
				sb.AddNewFromTypes(typeof(string));
				sgnSay = sb.GenerateSignature("spg_say");
				sgnAsyncSay = sb.GenerateSignature("spg_asay");
				sgnRead = sb.GenerateSignature("spg_read");
				sgnAsyncRead = sb.GenerateSignature("spg_aread");
				sgnVoice = sb.GenerateSignature("spg_voice");
				sgnShutUp = sb.GenerateSignature("spg_shutup");
			}

			#region Async Read

			/// <summary>
			/// Request speech generator to move to synthetize the text contained in the specified textfile
			/// </summary>
			/// <param name="file">The file which contains the text to be synthetized by the Speech Generator</param>
			/// <returns>true if Speech Generator synthetized the specified text. false otherwise</returns>
			public bool AsyncRead(string file)
			{
				return AsyncRead(file, DefaultDelay);
			}

			/// <summary>
			/// Request speech generator to move to synthetize the text contained in the specified textfile
			/// </summary>
			/// <param name="file">The file which contains the text to be synthetized by the Speech Generator</param>
			/// <param name="timeOut">Amout of time to wait for an speech generator response in milliseconds</param>
			/// <returns>true if Speech Generator synthetized the specified text. false otherwise</returns>
			public bool AsyncRead(string file, int timeOut)
			{
				// Stores the command to be sent to speech generator
				Command cmdAsyncRead;
				// Stores the response from speech generator and the candidate while lookingfor
				Response rspAsyncRead = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdAsyncRead = new Command(sgnAsyncRead.CommandName, file, CmdMan.AutoId++);

				// 2. Send the say command			
				CmdMan.Console("\tAsyncReading: " + cmdAsyncRead.StringToSend);
				if (!CmdMan.SendAndWait(cmdAsyncRead, timeOut, out rspAsyncRead))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse speech generator response
				if (!rspAsyncRead.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's speech generator did not spoke");
					return false;
				}

				CmdMan.Console("\tAsyncReading complete [" + rspAsyncRead.Parameters + "]");
				return true;
			}

			#endregion

			#region Async Say

			/// <summary>
			/// Request speech generator to move to synthetize the specified text
			/// </summary>
			/// <param name="textToAsyncSay">The text to be synthetized by the Speech Generator</param>
			/// <returns>true if Speech Generator synthetized the specified text. false otherwise</returns>
			public bool AsyncSay(string textToAsyncSay)
			{
				return AsyncSay(textToAsyncSay, DefaultDelay);
			}

			/// <summary>
			/// Request speech generator to move to synthetize the specified text
			/// </summary>
			/// <param name="textToAsyncSay">The text to be synthetized by the Speech Generator</param>
			/// <param name="timeOut">Amout of time to wait for an speech generator response in milliseconds</param>
			/// <returns>true if Speech Generator synthetized the specified text. false otherwise</returns>
			public bool AsyncSay(string textToAsyncSay, int timeOut)
			{
				// Stores the command to be sent to speech generator
				Command cmdAsyncSay;
				// Stores the response from speech generator and the candidate while lookingfor
				Response rspAsyncSay = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdAsyncSay = new Command(sgnAsyncSay.CommandName, textToAsyncSay, CmdMan.AutoId++);

				// 2. Send the say command			
				CmdMan.Console("\tAsyncSaying: " + cmdAsyncSay.StringToSend);
				if (!CmdMan.SendAndWait(cmdAsyncSay, timeOut, out rspAsyncSay))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse speech generator response
				if (!rspAsyncSay.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's speech generator did not spoke");
					return false;
				}

				CmdMan.Console("\tTTS complete [" + rspAsyncSay.Parameters + "]");
				return true;
			}

			#endregion

			#region Say methods

			/// <summary>
			/// Request speech generator to move to synthetize the specified text
			/// </summary>
			/// <param name="textToSay">The text to be synthetized by the Speech Generator</param>
			/// <returns>true if Speech Generator synthetized the specified text. false otherwise</returns>
			public bool Say(string textToSay)
			{
				return Say(textToSay, DefaultDelay);
			}

			/// <summary>
			/// Request speech generator to move to synthetize the specified text
			/// </summary>
			/// <param name="textToSay">The text to be synthetized by the Speech Generator</param>
			/// <param name="timeOut">Amout of time to wait for an speech generator response in milliseconds</param>
			/// <returns>true if Speech Generator synthetized the specified text. false otherwise</returns>
			public bool Say(string textToSay, int timeOut)
			{
				// Stores the command to be sent to speech generator
				Command cmdSay;
				// Stores the response from speech generator and the candidate while lookingfor
				Response rspSay = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdSay = new Command(sgnSay.CommandName, textToSay, CmdMan.AutoId++);

				// 2. Send the say command			
				CmdMan.Console("\tSaying: " + cmdSay.StringToSend);
				if (!CmdMan.SendAndWait(cmdSay, timeOut, out rspSay))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse speech generator response
				if (!rspSay.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's speech generator did not spoke");
					return false;
				}

				CmdMan.Console("\tTTS complete [" + rspSay.Parameters + "]");
				return true;
			}

			#endregion

			#region ShutUp Methods

			/// <summary>
			/// Request speech generator to move to shut up
			/// </summary>
			/// <returns>true if Speech Generator stopped the speech synthesis. false otherwise</returns>
			public bool ShutUp()
			{
				// Stores the command to be sent to speech generator
				Command cmdShutUp;
				// Stores the response from speech generator and the candidate while lookingfor
				Response rspShutUp = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdShutUp = new Command(sgnShutUp.CommandName, "", CmdMan.AutoId++);

				// 2. Send the say command			
				CmdMan.Console("\tShutting up: " + cmdShutUp.StringToSend);
				if (!CmdMan.SendAndWait(cmdShutUp, 300, out rspShutUp))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse speech generator response
				if (!rspShutUp.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's speech generator did not respond");
					return false;
				}

				CmdMan.Console("\tShutUp complete");
				return true;
			}

			#endregion

			#region Read methods

			/// <summary>
			/// Request speech generator to move to synthetize the text contained in the specified textfile
			/// </summary>
			/// <param name="file">The file which contains the text to be synthetized by the Speech Generator</param>
			/// <returns>true if Speech Generator synthetized the specified text. false otherwise</returns>
			public bool Read(string file)
			{
				return Read(file, DefaultDelay);
			}

			/// <summary>
			/// Request speech generator to move to synthetize the text contained in the specified textfile
			/// </summary>
			/// <param name="file">The file which contains the text to be synthetized by the Speech Generator</param>
			/// <param name="timeOut">Amout of time to wait for an speech generator response in milliseconds</param>
			/// <returns>true if Speech Generator synthetized the specified text. false otherwise</returns>
			public bool Read(string file, int timeOut)
			{
				// Stores the command to be sent to speech generator
				Command cmdRead;
				// Stores the response from speech generator and the candidate while lookingfor
				Response rspRead = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdRead = new Command(sgnRead.CommandName, file, CmdMan.AutoId++);

				// 2. Send the say command			
				CmdMan.Console("\tReading: " + cmdRead.StringToSend);
				if (!CmdMan.SendAndWait(cmdRead, timeOut, out rspRead))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse speech generator response
				if (!rspRead.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's speech generator did not spoke");
					return false;
				}

				CmdMan.Console("\tReading complete [" + rspRead.Parameters + "]");
				return true;
			}

			#endregion

			#region Voice methods

			/// <summary>
			/// Request speech generator to use the specified voice
			/// </summary>
			/// <param name="voiceName">The name of the voice to set. Use an empty string to get the actual voice.</param>
			/// <returns>true if Speech Generator changed the voice successfully. false otherwise</returns>
			public bool Voice(string voiceName)
			{
				return Voice(ref voiceName, DefaultDelay);
			}

			/// <summary>
			/// Request speech generator to use the specified voice
			/// </summary>
			/// <param name="voiceName">The name of the voice to set. Use an empty string to get the actual voice.</param>
			/// <returns>true if Speech Generator changed the voice successfully. false otherwise</returns>
			public bool Voice(ref string voiceName)
			{
				return Voice(ref voiceName, DefaultDelay);
			}

			/// <summary>
			/// Request speech generator to use the specified voice
			/// </summary>
			/// <param name="voiceName">The name of the voice to set. Use an empty string to get the actual voice.</param>
			/// <param name="timeOut">Amout of time to wait for an speech generator response in milliseconds</param>
			/// <returns>true if Speech Generator changed the voice successfully. false otherwise</returns>
			public bool Voice(string voiceName, int timeOut)
			{
				return Voice(ref voiceName, timeOut);
			}

			/// <summary>
			/// Request speech generator to use the specified voice
			/// </summary>
			/// <param name="voiceName">The name of the voice to set. Use an empty string to get the actual voice.</param>
			/// <param name="timeOut">Amout of time to wait for an speech generator response in milliseconds</param>
			/// <returns>true if Speech Generator changed the voice successfully. false otherwise</returns>
			public bool Voice(ref string voiceName, int timeOut)
			{
				// Stores the command to be sent to speech generator
				Command cmdVoice;
				// Stores the response from speech generator and the candidate while lookingfor
				Response rspVoice = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdVoice = new Command(sgnVoice.CommandName, voiceName, CmdMan.AutoId++);

				// 2. Send the say command			
				CmdMan.Console("\tSet voice: " + cmdVoice.StringToSend);
				if (!CmdMan.SendAndWait(cmdVoice, timeOut, out rspVoice))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse speech generator response
				if (!rspVoice.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tCannot change selected voice");
					return false;
				}

				// 4.0 Recover values from response
				voiceName = rspVoice.Parameters;

				CmdMan.Console("\tSet voice complete! [" + rspVoice.Parameters + "]");
				return true;
			}

			#endregion

			#endregion

		}
	}
}