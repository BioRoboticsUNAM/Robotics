using System;
using System.Collections.Generic;
using Robotics;
using Robotics.API;
using Robotics.HAL;

namespace Robotics.PacIto
{
	public partial class PacItoCommandManager
	{
		/// <summary>
		/// Controls remotely the manipulator of the base of Pac-Ito
		/// </summary>
		public class PacItoManipulatorManager : SharedResource
		{
			#region Variables

			/// <summary>
			/// Stores the reference to the CommandManager which this object serves
			/// </summary>
			private PacItoCommandManager cmdMan;

			/// <summary>
			/// Stores the default delay time for man commands
			/// </summary>
			private int defaultDelay = 10000;

			#region Signatures

			/// <summary>
			/// Signature to parse man_open responses
			/// </summary>
			private Signature sgnOpenGrip;

			/// <summary>
			/// Signature to parse man_close responses
			/// </summary>
			private Signature sgnCloseGrip;

			/// <summary>
			/// Signature to parse man_status responses
			/// </summary>
			private Signature sgnStatus;

			/// <summary>
			/// Signature to parse man_tilt responses
			/// </summary>
			private Signature sgnTilt;

			#endregion

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of PacItoManManager
			/// <param name="cmdMan">The reference to the CommandManager which this object serves</param>
			/// </summary>
			internal PacItoManipulatorManager(PacItoCommandManager cmdMan)
			{
				this.cmdMan = cmdMan;
				CreateManipulatorSignatures();
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets or Sets the default delay time for man commands
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
			/// Gets the CommandManager which this object serves
			/// </summary>
			protected PacItoCommandManager CmdMan
			{
				get { return cmdMan; }
			}

			#region Signatures

			/// <summary>
			/// Gets the signature to parse man_status responses
			/// </summary>
			public virtual Signature SgnStatus
			{
				get { return sgnStatus; }
			}

			/// <summary>
			/// Gets the signature to parse man_tilt responses
			/// </summary>
			public virtual Signature SgnTilt
			{
				get { return sgnTilt; }
			}

			/// <summary>
			/// Gets the Signature object to parse man_open responses
			/// </summary>
			public virtual Signature SgnOpenGrip
			{
				get { return sgnOpenGrip; }
			}

			/// <summary>
			/// Gets the Signature object to parse man_close responses
			/// </summary>
			public virtual Signature SgnCloseGrip
			{
				get { return sgnCloseGrip; }
			}

			#endregion

			#endregion

			#region Methods

			/// <summary>
			/// Creates the Man Signatures
			/// </summary>
			private void CreateManipulatorSignatures()
			{
				SignatureBuilder sb = new SignatureBuilder();

				//sgnManGoTo = sb.GenerateSignature("manGoto");
				//sb.Clear();
				sb.AddNewFromTypes(typeof(int));
				sb.AddNewFromTypes();
				sgnOpenGrip = sb.GenerateSignature("man_open");
				sgnCloseGrip = sb.GenerateSignature("man_close");
				sb.Clear();

				sb.AddNewFromTypes(typeof(double));
				sb.AddNewFromTypes();
				sgnTilt = sb.GenerateSignature("man_tilt");
				sb.Clear();

				sb.AddNewFromTypes(typeof(int), typeof(double));
				sb.AddNewFromTypes();
				sgnStatus = sb.GenerateSignature("man_status");
			}

			#region CloseGrip

			/// <summary>
			/// Request manipulator to close the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <returns>true if manipulator closed the grip. false otherwise</returns>
			public virtual bool CloseGrip(int percentage)
			{
				return CloseGrip(ref percentage, DefaultDelay);
			}

			/// <summary>
			/// Request manipulator to close the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <param name="timeOut">Amout of time to wait for an man response in milliseconds</param>
			/// <returns>true if manipulator closed the grip. false otherwise</returns>
			public virtual bool CloseGrip(int percentage, int timeOut)
			{
				return CloseGrip(ref percentage, timeOut);
			}

			/// <summary>
			/// Request manipulator to close the grip
			/// </summary>
			/// <returns>true if manipulator closed the grip. false otherwise</returns>
			public virtual bool CloseGrip()
			{
				return CloseGrip(0, DefaultDelay);
			}

			/// <summary>
			/// Request manipulator to close the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <returns>true if manipulator closed the grip. false otherwise</returns>
			public virtual bool CloseGrip(ref int percentage)
			{
				return CloseGrip(ref percentage, DefaultDelay);
			}

			/// <summary>
			/// Request manipulator to close the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <param name="timeOut">Amout of time to wait for an man response in milliseconds</param>
			/// <returns>true if manipulator closed the grip. false otherwise</returns>
			public virtual bool CloseGrip(ref int percentage, int timeOut)
			{
				// Stores the command to be sent to man
				Command cmdCloseGrip;
				// Stores the response from man and the candidate while moving
				Response rspCloseGrip = null;
				bool result;

				// 1. Prepare the command
				if (!GetResource()) return false;

				if ((percentage >= 0) || (percentage <= 100))
					cmdCloseGrip = new Command(sgnCloseGrip.CommandName, percentage.ToString(), CmdMan.AutoId++);
				else
				{
					cmdCloseGrip = new Command(sgnCloseGrip.CommandName, "", CmdMan.AutoId++);
					percentage = 0;
				}

				// 2. Send the manCloseGrip command			
				CmdMan.Console("\tClosing manipulator grip to [" + percentage.ToString() + "%]");
				if (!CmdMan.SendAndWait(cmdCloseGrip, timeOut, out rspCloseGrip))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse manipulator response
				if (!rspCloseGrip.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tManipulator did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnCloseGrip.Analyze(rspCloseGrip);
				result = saResult.Success &&
					(saResult.ParameterCount == 1) &&
					(saResult.Update<int>(0, ref percentage));
				if (!result)
					CmdMan.Console("\tInvalid response");
				else
					CmdMan.Console("\tClose manipulator grip complete");
				return result;
			}

			#endregion

			#region Status

			/// <summary>
			/// Request the manipulator aperture pecentage and tilt angle
			/// </summary>
			/// <param name="aperturePercentage">Percentage aperture of the gripper of the manipulator.</param>
			/// <param name="tilt">Tilt angle of the manipulator.</param>
			/// <returns>true if data fetch was successfully. false otherwise</returns>
			public virtual bool Status(out int aperturePercentage, out double tilt)
			{
				return Status(out aperturePercentage, out tilt, DefaultDelay);
			}

			/// <summary>
			/// Request the manipulator aperture pecentage and tilt angle
			/// </summary>
			/// <param name="aperturePercentage">Percentage aperture of the gripper of the manipulator.</param>
			/// <param name="tilt">Tilt angle of the manipulator.</param>
			/// <param name="timeOut">Amout of time to wait for an man response in milliseconds</param>
			/// <returns>true if data fetch was successfully. false otherwise</returns>
			public virtual bool Status(out int aperturePercentage, out double tilt, int timeOut)
			{
				// Stores the command to be sent to man
				Command cmdStatus;
				// Stores the response from man and the candidate while moving
				Response rspStatus = null;
				bool result;

				aperturePercentage = 0;
				tilt = 0;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				string parameters = "";
				cmdStatus = new Command(sgnStatus.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the manAbsPos command			
				CmdMan.Console("\tReading man orientation " + parameters);
				if (!CmdMan.SendAndWait(cmdStatus, timeOut, out rspStatus))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse man response
				if (!rspStatus.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tManipulator did not respond");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnStatus.Analyze(rspStatus);
				result = saResult.Success &&
					(saResult.ParameterCount == 2) &&
					(saResult.Update<int>(0, ref aperturePercentage) &
					saResult.Update<double>(1, ref tilt));
				if (!result)
					CmdMan.Console("\tInvalid response");
				else
					CmdMan.Console("\tGet man position complete");
				return result;
			}

			#endregion

			#region OpenGrip

			/// <summary>
			/// Request manipulator to open the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <returns>true if manipulator opend the grip. false otherwise</returns>
			public virtual bool OpenGrip(int percentage)
			{
				return OpenGrip(ref percentage, DefaultDelay);
			}

			/// <summary>
			/// Request manipulator to open the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <param name="timeOut">Amout of time to wait for an man response in milliseconds</param>
			/// <returns>true if manipulator opend the grip. false otherwise</returns>
			public virtual bool OpenGrip(int percentage, int timeOut)
			{
				return OpenGrip(ref percentage, timeOut);
			}

			/// <summary>
			/// Request manipulator to open the grip
			/// </summary>
			/// <returns>true if manipulator opend the grip. false otherwise</returns>
			public virtual bool OpenGrip()
			{
				return OpenGrip(50, DefaultDelay);
			}

			/// <summary>
			/// Request manipulator to open the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <returns>true if manipulator opend the grip. false otherwise</returns>
			public virtual bool OpenGrip(ref int percentage)
			{
				return OpenGrip(ref percentage, DefaultDelay);
			}

			/// <summary>
			/// Request manipulator to open the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <param name="timeOut">Amout of time to wait for an man response in milliseconds</param>
			/// <returns>true if manipulator opend the grip. false otherwise</returns>
			public virtual bool OpenGrip(ref int percentage, int timeOut)
			{
				// Stores the command to be sent to man
				Command cmdOpenGrip;
				// Stores the response from man and the candidate while moving
				Response rspOpenGrip = null;
				bool result;

				// 1. Prepare the command
				if (!GetResource()) return false;

				if ((percentage >= 0) || (percentage <= 100))
					cmdOpenGrip = new Command(sgnOpenGrip.CommandName, percentage.ToString(), CmdMan.AutoId++);
				else
				{
					cmdOpenGrip = new Command(sgnOpenGrip.CommandName, "", CmdMan.AutoId++);
					percentage = 50;
				}

				// 2. Send the manOpenGrip command			
				CmdMan.Console("\tClosing manipulator grip to [" + percentage.ToString() + "%]");
				if (!CmdMan.SendAndWait(cmdOpenGrip, timeOut, out rspOpenGrip))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse manipulator response
				if (!rspOpenGrip.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tManipulator did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnOpenGrip.Analyze(rspOpenGrip);
				result = saResult.Success &&
					(saResult.ParameterCount == 1) &&
					(saResult.Update<int>(0, ref percentage));
				if (!result)
					CmdMan.Console("\tInvalid response");
				else
					CmdMan.Console("\tOpen manipulator grip complete");
				return result;
			}

			#endregion

			#region Tilt

			/// <summary>
			/// Request manipulator to move to the specified tilt angle in radians
			/// </summary>
			/// <param name="tilt">The specified tilt angle in radians</param>
			/// <returns>true if manipulator moved to specified tilt. false otherwise</returns>
			public virtual bool Tilt(double tilt)
			{
				return Tilt(ref tilt, defaultDelay);
			}

			/// <summary>
			/// Request manipulator to move to the specified tilt angle in radians
			/// </summary>
			/// <param name="tilt">The specified tilt angle in radians</param>
			/// <param name="timeOut">Amout of time to wait for an man response in milliseconds</param>
			/// <returns>true if manipulator moved to specified tilt. false otherwise</returns>
			public virtual bool Tilt(double tilt, int timeOut)
			{
				return Tilt(ref tilt, timeOut);
			}

			/// <summary>
			/// Request manipulator to move to the specified tilt angle in radians
			/// </summary>
			/// <param name="tilt">The specified tilt angle in radians</param>
			/// <returns>true if manipulator moved to specified tilt. false otherwise</returns>
			public virtual bool Tilt(ref double tilt)
			{
				return Tilt(tilt, defaultDelay);
			}

			/// <summary>
			/// Request manipulator to move to the specified tilt angle in radians
			/// </summary>
			/// <param name="tilt">The specified tilt angle in radians</param>
			/// <param name="timeOut">Amout of time to wait for an man response in milliseconds</param>
			/// <returns>true if manipulator moved to specified tilt. false otherwise</returns>
			public virtual bool Tilt(ref double tilt, int timeOut)
			{
				// Stores the command to be sent to man
				Command cmdTilt;
				// Stores the response from man and the candidate while moving
				Response rspTilt = null;
				bool result;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdTilt = new Command(sgnTilt.CommandName, tilt.ToString("0.00"), CmdMan.AutoId++);

				// 2. Send the manGoto command			
				CmdMan.Console("\tSetting manipulator tilt [" + tilt + "]");
				if (!CmdMan.SendAndWait(cmdTilt, timeOut, out rspTilt))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse manipulator response
				if (!rspTilt.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tManipulator did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnTilt.Analyze(rspTilt);
				result = saResult.Success &&
					(saResult.ParameterCount == 1) &&
					(saResult.Update<double>(0, ref tilt));
				if (!result)
					CmdMan.Console("\tInvalid response");
				else
					CmdMan.Console("\tSet manipulator tilt complete");
				return result;
			}

			#endregion

			#endregion
		}
	}
}
