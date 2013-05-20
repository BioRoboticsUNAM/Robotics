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
		/// Controls remotely the 7DOF antropomorphic arm of Pac-Ito
		/// </summary>
		public class PacItoArmManager : SharedResource, IAnthropomorphicArm
		{
			#region Variables

			/// <summary>
			/// Stores the reference to the CommandManager which this object serves
			/// </summary>
			private PacItoCommandManager cmdMan;

			/// <summary>
			/// Stores the default delay time for arm commands
			/// </summary>
			private int defaultDelay = 3000;

			#region Signatures

			/// <summary>
			/// Signature to parse armGoto responses
			/// </summary>
			private Signature sgnArmGoTo;

			/// <summary>
			/// Signature to parse armSetAbsPos responses
			/// </summary>
			private Signature sgnArmSetAbsPos;

			/// <summary>
			/// Signature to parse armGetAbsPos responses
			/// </summary>
			private Signature sgnArmGetAbsPos;

			/// <summary>
			/// Signature to parse armMove responses
			/// </summary>
			private Signature sgnArmMove;

			/// <summary>
			/// Signature to parse armSetOrientation responses
			/// </summary>
			private Signature sgnArmSetOrientation;

			/// <summary>
			/// Signature to parse armGetOrientation responses
			/// </summary>
			private Signature sgnArmGetOrientation;

			/// <summary>
			/// Signature to parse armSetRelPos responses
			/// </summary>
			private Signature sgnArmSetRelPos;

			/// <summary>
			/// Signature to parse ra_torque responses
			/// </summary>
			private Signature sgnArmTorque;

			/// <summary>
			/// Signature to parse armOpenGrip responses
			/// </summary>
			private Signature sgnArmOpenGrip;

			/// <summary>
			/// Signature to parse armCloseGrip responses
			/// </summary>
			private Signature sgnArmCloseGrip;

			#endregion

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of PacItoArmManager
			/// <param name="cmdMan">The reference to the CommandManager which this object serves</param>
			/// </summary>
			internal PacItoArmManager(PacItoCommandManager cmdMan)
			{
				this.cmdMan = cmdMan;
				CreateArmSignatures();
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets or Sets the default delay time for arm commands
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
			/// Gets the Signature object to parse armGoto responses
			/// </summary>
			public virtual Signature SgnArmGoTo
			{
				get { return sgnArmGoTo; }
			}

			/// <summary>
			/// Gets the Signature object to parse armSetAbsPos responses
			/// </summary>
			public virtual Signature SgnArmSetAbsPos
			{
				get { return sgnArmSetAbsPos; }
			}

			/// <summary>
			/// Gets the Signature object to parse armGetAbsPos responses
			/// </summary>
			public virtual Signature SgnArmGetAbsPos
			{
				get { return sgnArmGetAbsPos; }
			}

			/// <summary>
			/// Gets the Signature object to parse armSetOrientation responses
			/// </summary>
			public virtual Signature SgnArmSetOrientation
			{
				get { return sgnArmSetOrientation; }
			}

			/// <summary>
			/// Gets the Signature object to parse armGetOrientation responses
			/// </summary>
			public virtual Signature SgnArmGetOrientation
			{
				get { return sgnArmGetOrientation; }
			}

			/// <summary>
			/// Gets the Signature object to parse armSetRelPos responses
			/// </summary>
			public virtual Signature SgnArmSetRelPos
			{
				get { return sgnArmSetRelPos; }
			}

			/// <summary>
			/// Gets the Signature object to parse armOpenGrip responses
			/// </summary>
			public virtual Signature SgnArmOpenGrip
			{
				get { return sgnArmOpenGrip; }
			}

			/// <summary>
			/// Gets the Signature object to parse armCloseGrip responses
			/// </summary>
			public virtual Signature SgnArmCloseGrip
			{
				get { return sgnArmCloseGrip; }
			}

			#endregion

			#endregion

			#region Methods

			/// <summary>
			/// Creates the Arm Signatures
			/// </summary>
			private void CreateArmSignatures()
			{
				SignatureBuilder sb = new SignatureBuilder();

				//sgnArmGoTo = sb.GenerateSignature("armGoto");
				//sb.Clear();
				sb.AddNewFromTypes(typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double));
				sgnArmGetAbsPos = sb.GenerateSignature("ra_abspos");
				sb.AddNewFromTypes(typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(double), typeof(double), typeof(double));
				sgnArmSetAbsPos = sb.GenerateSignature("ra_abspos");
				sgnArmSetRelPos = sb.GenerateSignature("ra_relpos");
				sb.Clear();
				sb.AddNewFromTypes(typeof(double), typeof(double), typeof(double));
				sgnArmSetOrientation = sb.GenerateSignature("ra_orientation");
				sgnArmGetOrientation = sb.GenerateSignature("ra_orientation");
				sb.Clear();
				sb.AddNewFromTypes(typeof(int));
				sb.AddNewFromTypes();
				sgnArmOpenGrip = sb.GenerateSignature("ra_opengrip");
				sgnArmCloseGrip = sb.GenerateSignature("ra_closegrip");
				sb.Clear();
				sb.AddNewFromTypes(typeof(string));
				sgnArmGoTo = sb.GenerateSignature("ra_goto");
				sgnArmMove = sb.GenerateSignature("ra_move");
				sgnArmTorque = sb.GenerateSignature("ra_torque");
			}

			#region ArmCloseGrip

			/// <summary>
			/// Request arm to close the grip
			/// </summary>
			/// <returns>true if arm closed the grip. false otherwise</returns>
			public virtual bool CloseGrip()
			{
				return CloseGrip(-1, DefaultDelay);
			}

			/// <summary>
			/// Request arm to close the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <returns>true if arm closed the grip. false otherwise</returns>
			public virtual bool CloseGrip(int percentage)
			{
				return CloseGrip(percentage, DefaultDelay);
			}

			/// <summary>
			/// Request arm to close the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm closed the grip. false otherwise</returns>
			public virtual bool CloseGrip(int percentage, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdCloseGrip;
				// Stores the response from arm and the candidate while moving
				Response rspCloseGrip = null;

				// 1. Prepare the command
				if (!GetResource()) return false;

				if ((percentage >= 0) || (percentage <= 100))
					cmdCloseGrip = new Command(sgnArmCloseGrip.CommandName, percentage.ToString(), CmdMan.AutoId++);
				else
				{
					cmdCloseGrip = new Command(sgnArmCloseGrip.CommandName, "", CmdMan.AutoId++);
					percentage = -1;
				}

				// 2. Send the armCloseGrip command			
				CmdMan.Console("\tClosing arm grip to [" + percentage.ToString() + "%]");
				if (!CmdMan.SendAndWait(cmdCloseGrip, timeOut, out rspCloseGrip))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspCloseGrip.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}
				CmdMan.Console("\tClose arm grip complete");
				return true;
			}

			#endregion

			#region ArmGetAbsolutePosition

			/// <summary>
			/// Request the arm for it's current position and orientation
			/// </summary>
			/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
			/// <returns>true if data acquisition was successfull. false otherwise</returns>
			public virtual bool GetAbsolutePosition(out double x, out double y, out double z)
			{
				return GetAbsolutePosition(out x, out y, out z, DefaultDelay);
			}

			/// <summary>
			/// Request the arm for it's current position and orientation
			/// </summary>
			/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if data acquisition was successfull. false otherwise</returns>
			public virtual bool GetAbsolutePosition(out double x, out double y, out double z, int timeOut)
			{
				double roll;
				double pitch;
				double yaw;
				double elbow;
				return GetAbsolutePosition(out x, out y, out z, out roll, out pitch, out yaw, out elbow, DefaultDelay);
			}

			/// <summary>
			/// Request the arm for it's current position and orientation
			/// </summary>
			/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <param name="elbow">Angle in radians of the elbow of the arm.</param>
			/// <returns>true if data acquisition was successfull. false otherwise</returns>
			public virtual bool GetAbsolutePosition(out double x, out double y, out double z, out double roll, out double pitch, out double yaw, out double elbow)
			{
				return GetAbsolutePosition(out x, out y, out z, out roll, out pitch, out yaw, out elbow, DefaultDelay);
			}

			/// <summary>
			/// Request the arm for it's current position and orientation
			/// </summary>
			/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <param name="elbow">Angle in radians of the elbow of the arm.</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if data acquisition was successfull. false otherwise</returns>
			public virtual bool GetAbsolutePosition(out double x, out double y, out double z, out double roll, out double pitch, out double yaw, out double elbow, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdArmAbsPos;
				// Stores the response from arm and the candidate while moving
				Response rspArmAbsPos = null;
				bool result;

				x = 0;
				y = 0;
				z = 0;
				roll = 0;
				pitch = 0;
				yaw = 0;
				elbow = 0;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				string parameters = "";
				cmdArmAbsPos = new Command(sgnArmGetAbsPos.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the armAbsPos command			
				CmdMan.Console("\tReading arm position " + parameters);
				if (!CmdMan.SendAndWait(cmdArmAbsPos, timeOut, out rspArmAbsPos))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse arm response
				if (!rspArmAbsPos.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not respond");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnArmGetAbsPos.Analyze(rspArmAbsPos);
				result = saResult.Success &&
					(saResult.ParameterCount == 7) &&
					(saResult.GetParameter<double>(0, out x) &
					saResult.GetParameter<double>(1, out y) &
					saResult.GetParameter<double>(2, out z) &
					saResult.GetParameter<double>(3, out roll) &
					saResult.GetParameter<double>(4, out pitch) &
					saResult.GetParameter<double>(5, out yaw) &
					saResult.GetParameter<double>(6, out elbow));
				if (!result)
					CmdMan.Console("\tInvalid response");
				else
					CmdMan.Console("\tGet arm position complete");
				return result;
			}

			#endregion

			#region ArmGetOrientation

			/// <summary>
			/// Request arm to move to the specified orientation
			/// </summary>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <returns>true if arm moved to the specified orientation. false otherwise</returns>
			public virtual bool GetOrientation(out double roll, out double pitch, out double yaw)
			{
				return GetOrientation(out roll, out pitch, out yaw, DefaultDelay);
			}

			/// <summary>
			/// Request arm to move to the specified orientation
			/// </summary>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm moved to the specified orientation. false otherwise</returns>
			public virtual bool GetOrientation(out double roll, out double pitch, out double yaw, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdArmOrientation;
				// Stores the response from arm and the candidate while moving
				Response rspArmOrientation = null;
				bool result;

				roll = 0;
				pitch = 0;
				yaw = 0;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				string parameters = "";
				cmdArmOrientation = new Command(sgnArmGetOrientation.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the armAbsPos command			
				CmdMan.Console("\tReading arm orientation " + parameters);
				if (!CmdMan.SendAndWait(cmdArmOrientation, timeOut, out rspArmOrientation))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse arm response
				if (!rspArmOrientation.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not respond");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnArmGetAbsPos.Analyze(rspArmOrientation);
				result = saResult.Success &&
					(saResult.ParameterCount == 3) &&
					(saResult.Update<double>(0, ref roll) &
					saResult.Update<double>(1, ref pitch) &
					saResult.Update<double>(2, ref yaw));
				if (!result)
					CmdMan.Console("\tInvalid response");
				else
					CmdMan.Console("\tGet arm position complete");
				return result;
			}

			#endregion

			#region ArmGoTo

			/// <summary>
			/// Request arm to move to the specified position
			/// </summary>
			/// <param name="position">Name of the position to move at</param>
			/// <returns>true if arm moved to the specified position. false otherwise</returns>
			public virtual bool GoTo(string position)
			{
				return GoTo(position, DefaultDelay);
			}

			/// <summary>
			/// Request arm to move to the specified position
			/// </summary>
			/// <param name="position">Name of the position to move at</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm moved to the specified position. false otherwise</returns>
			public virtual bool GoTo(string position, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdMoveArm;
				// Stores the response from arm and the candidate while moving
				Response rspMoveArm = null;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				cmdMoveArm = new Command(sgnArmGoTo.CommandName, position, CmdMan.AutoId++);

				// 2. Send the armGoto command			
				CmdMan.Console("\tMoving arm to position [" + position + "]");
				if (!CmdMan.SendAndWait(cmdMoveArm, timeOut, out rspMoveArm))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspMoveArm.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}
				CmdMan.Console("\tMoving arm to position complete");
				return true;
			}

			#endregion

			#region ArmMove

			/// <summary>
			/// Moves the arm through a secuence of positions
			/// </summary>
			/// <param name="movement">The name of the movement to perform</param>
			/// <returns>true if arm executed the specified movement. false otherwise</returns>
			public virtual bool Move(string movement)
			{
				return Move(movement, defaultDelay);
			}

			/// <summary>
			/// Moves the arm through a secuence of positions
			/// </summary>
			/// <param name="movement">The name of the movement to perform</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm executed the specified movement. false otherwise</returns>
			public virtual bool Move(string movement, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdMoveArm;
				// Stores the response from arm and the candidate while moving
				Response rspMoveArm = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdMoveArm = new Command(sgnArmMove.CommandName, movement, CmdMan.AutoId++);

				// 2. Send the armGoto command			
				CmdMan.Console("\tMoving arm to position [" + movement + "]");
				if (!CmdMan.SendAndWait(cmdMoveArm, timeOut, out rspMoveArm))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspMoveArm.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}
				CmdMan.Console("\tMoving arm to position complete");
				return true;
			}

			#endregion

			#region ArmOpenGrip

			/// <summary>
			/// Request arm to open the grip
			/// </summary>
			/// <returns>true if arm opened the grip. false otherwise</returns>
			public virtual bool OpenGrip()
			{
				return OpenGrip(-1, DefaultDelay);
			}

			/// <summary>
			/// Request arm to open the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <returns>true if arm opened the grip. false otherwise</returns>
			public virtual bool OpenGrip(int percentage)
			{
				return OpenGrip(percentage, DefaultDelay);
			}

			/// <summary>
			/// Request arm to open the grip
			/// </summary>
			/// <param name="percentage">Percentage aperture of the grip</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm opened the grip. false otherwise</returns>
			public virtual bool OpenGrip(int percentage, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdOpenGrip;
				// Stores the response from arm and the candidate while moving
				Response rspOpenGrip = null;

				// 1. Prepare the command
				if (!GetResource()) return false;

				if ((percentage >= 0) && (percentage <= 100))
					cmdOpenGrip = new Command(sgnArmOpenGrip.CommandName, percentage.ToString(), CmdMan.AutoId++);
				else
				{
					cmdOpenGrip = new Command(sgnArmOpenGrip.CommandName, "", CmdMan.AutoId++);
					percentage = -1;
				}

				// 2. Send the armOpenGrip command			
				CmdMan.Console("\tOpening arm grip to [" + percentage.ToString() + "%]");
				if (!CmdMan.SendAndWait(cmdOpenGrip, timeOut, out rspOpenGrip))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse head response
				if (!rspOpenGrip.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}
				CmdMan.Console("\tOpen arm grip complete");
				return true;
			}

			#endregion

			#region ArmSetAbsolutePosition

			/// <summary>
			/// Request arm to move to the specified position
			/// </summary>
			/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetAbsolutePosition(ref double x, ref double y, ref double z)
			{
				return SetAbsolutePosition(ref x, ref y, ref z, DefaultDelay);
			}

			/// <summary>
			/// Request arm to move to the specified position
			/// </summary>
			/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetAbsolutePosition(ref double x, ref double y, ref double z, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdArmAbsPos;
				// Stores the response from arm and the candidate while moving
				Response rspArmAbsPos = null;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				string parameters =
					x.ToString("0.00") + " " +
					y.ToString("0.00") + " " +
					z.ToString("0.00");
				cmdArmAbsPos = new Command(sgnArmSetAbsPos.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the armAbsPos command			
				CmdMan.Console("\tMoving arm to position " + parameters);
				if (!CmdMan.SendAndWait(cmdArmAbsPos, timeOut, out rspArmAbsPos))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse arm response
				if (!rspArmAbsPos.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnArmSetAbsPos.Analyze(rspArmAbsPos);
				if (saResult.Success)
				{
					saResult.Update<double>(0, ref x);
					saResult.Update<double>(1, ref y);
					saResult.Update<double>(2, ref z);
				}
				CmdMan.Console("\tMoving arm to position complete");
				return true;
			}

			/// <summary>
			/// Request arm to move to the specified position and orientation
			/// </summary>
			/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetAbsolutePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw)
			{
				return SetAbsolutePosition(ref x, ref y, ref z, ref roll, ref pitch, ref yaw, DefaultDelay);
			}

			/// <summary>
			/// Request arm to move to the specified position and orientation
			/// </summary>
			/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetAbsolutePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdArmAbsPos;
				// Stores the response from arm and the candidate while moving
				Response rspArmAbsPos = null;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				string parameters =
					x.ToString("0.00") + " " +
					y.ToString("0.00") + " " +
					z.ToString("0.00") + " " +
					roll.ToString("0.00") + " " +
					pitch.ToString("0.00") + " " +
					yaw.ToString("0.00");
				cmdArmAbsPos = new Command(sgnArmSetAbsPos.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the armAbsPos command			
				CmdMan.Console("\tMoving arm to position " + parameters);
				if (!CmdMan.SendAndWait(cmdArmAbsPos, timeOut, out rspArmAbsPos))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse arm response
				if (!rspArmAbsPos.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnArmSetAbsPos.Analyze(rspArmAbsPos);
				if (saResult.Success)
				{
					saResult.Update<double>(0, ref x);
					saResult.Update<double>(1, ref y);
					saResult.Update<double>(2, ref z);
					saResult.Update<double>(3, ref roll);
					saResult.Update<double>(4, ref pitch);
					saResult.Update<double>(5, ref yaw);
				}

				CmdMan.Console("\tMoving arm to position complete");
				return true;
			}

			/// <summary>
			/// Request arm to move to the specified position and orientation
			/// </summary>
			/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <param name="elbow">Angle in radians of the elbow of the arm.</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetAbsolutePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw, ref double elbow)
			{
				return SetAbsolutePosition(ref x, ref y, ref z, ref roll, ref pitch, ref yaw, ref elbow, DefaultDelay);
			}

			/// <summary>
			/// Request arm to move to the specified position and orientation
			/// </summary>
			/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <param name="elbow">Angle in radians of the elbow of the arm.</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetAbsolutePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw, ref double elbow, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdArmAbsPos;
				// Stores the response from arm and the candidate while moving
				Response rspArmAbsPos = null;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				string parameters =
					x.ToString("0.00") + " " +
					y.ToString("0.00") + " " +
					z.ToString("0.00") + " " +
					roll.ToString("0.00") + " " +
					pitch.ToString("0.00") + " " +
					yaw.ToString("0.00") + " " +
					elbow.ToString("0.00");
				cmdArmAbsPos = new Command(sgnArmSetAbsPos.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the armAbsPos command			
				CmdMan.Console("\tMoving arm to position " + parameters);
				if (!CmdMan.SendAndWait(cmdArmAbsPos, timeOut, out rspArmAbsPos))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse arm response
				if (!rspArmAbsPos.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnArmSetAbsPos.Analyze(rspArmAbsPos);
				if (saResult.Success)
				{
					saResult.Update<double>(0, ref x);
					saResult.Update<double>(1, ref y);
					saResult.Update<double>(2, ref z);
					saResult.Update<double>(3, ref roll);
					saResult.Update<double>(4, ref pitch);
					saResult.Update<double>(5, ref yaw);
					saResult.Update<double>(6, ref elbow);
				}

				CmdMan.Console("\tMoving arm to position complete");
				return true;
			}

			#endregion

			#region ArmSetOrientation

			/// <summary>
			/// Request arm to move to the specified orientation
			/// </summary>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <returns>true if arm moved to the specified orientation. false otherwise</returns>
			public virtual bool SetOrientation(ref double roll, ref double pitch, ref double yaw)
			{
				return SetOrientation(ref roll, ref pitch, ref yaw, DefaultDelay);
			}

			/// <summary>
			/// Request arm to move to the specified orientation
			/// </summary>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm moved to the specified orientation. false otherwise</returns>
			public virtual bool SetOrientation(ref double roll, ref double pitch, ref double yaw, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdArmOrientation;
				// Stores the response from arm and the candidate while moving
				Response rspArmOrientation = null;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				string parameters =
					roll.ToString("0.00") + " " +
					pitch.ToString("0.00") + " " +
					yaw.ToString("0.00");
				cmdArmOrientation = new Command(sgnArmSetOrientation.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the armSetRelPos command			
				CmdMan.Console("\tSetting arm orientation " + parameters);
				if (!CmdMan.SendAndWait(cmdArmOrientation, timeOut, out rspArmOrientation))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse arm response
				if (!rspArmOrientation.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnArmSetOrientation.Analyze(rspArmOrientation);
				if (saResult.Success)
				{
					saResult.Update<double>(0, ref roll);
					saResult.Update<double>(1, ref pitch);
					saResult.Update<double>(2, ref yaw);
				}

				CmdMan.Console("\tSet arm orientation complete");
				return true;
			}

			#endregion

			#region ArmSetRelativePosition

			/// <summary>
			/// Request arm to move to the specified position relative to its current position
			/// </summary>
			/// <param name="x">Relative x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Relative y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Relative z coordinate position of the actuator of the arm.</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetRelativePosition(ref double x, ref double y, ref double z)
			{
				return SetRelativePosition(ref x, ref y, ref z, DefaultDelay);
			}

			/// <summary>
			/// Request arm to move to the specified position  relative to its current position
			/// </summary>
			/// <param name="x">Relative x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Relative y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Relative z coordinate position of the actuator of the arm.</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetRelativePosition(ref double x, ref double y, ref double z, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdArmRelPos;
				// Stores the response from arm and the candidate while moving
				Response rspArmRelPos = null;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				string parameters =
					x.ToString("0.00") + " " +
					y.ToString("0.00") + " " +
					z.ToString("0.00");
				cmdArmRelPos = new Command(sgnArmSetRelPos.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the armSetRelPos command			
				CmdMan.Console("\tMoving arm to relative position " + parameters);
				if (!CmdMan.SendAndWait(cmdArmRelPos, timeOut, out rspArmRelPos))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse arm response
				if (!rspArmRelPos.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnArmSetRelPos.Analyze(rspArmRelPos);
				if (saResult.Success)
				{
					saResult.Update<double>(0, ref x);
					saResult.Update<double>(1, ref y);
					saResult.Update<double>(2, ref z);
				}

				CmdMan.Console("\tMoving arm to position complete");
				return true;
			}

			/// <summary>
			/// Request arm to move to the specified position and orientation relative to its current position and orientation
			/// </summary>
			/// <param name="x">Relative x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Relative y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Relative z coordinate position of the actuator of the arm.</param>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetRelativePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw)
			{
				return SetRelativePosition(ref x, ref y, ref z, ref roll, ref pitch, ref yaw, DefaultDelay);
			}

			/// <summary>
			/// Request arm to move to the specified position and orientation relative to its current position and orientation
			/// </summary>
			/// <param name="x">Relative x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Relative y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Relative z coordinate position of the actuator of the arm.</param>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetRelativePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdArmRelPos;
				// Stores the response from arm and the candidate while moving
				Response rspArmRelPos = null;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				string parameters =
					x.ToString("0.00") + " " +
					y.ToString("0.00") + " " +
					z.ToString("0.00") + " " +
					roll.ToString("0.00") + " " +
					pitch.ToString("0.00") + " " +
					yaw.ToString("0.00");
				cmdArmRelPos = new Command(sgnArmSetRelPos.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the armSetRelPos command			
				CmdMan.Console("\tMoving arm to relative position " + parameters);
				if (!CmdMan.SendAndWait(cmdArmRelPos, timeOut, out rspArmRelPos))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse arm response
				if (!rspArmRelPos.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnArmSetRelPos.Analyze(rspArmRelPos);
				if (saResult.Success)
				{
					saResult.Update<double>(0, ref x);
					saResult.Update<double>(1, ref y);
					saResult.Update<double>(2, ref z);
					saResult.Update<double>(3, ref roll);
					saResult.Update<double>(4, ref pitch);
					saResult.Update<double>(5, ref yaw);
				}

				CmdMan.Console("\tMoving arm to position complete");
				return true;
			}

			/// <summary>
			/// Request arm to move to the specified position and orientation relative to its current position and orientation
			/// </summary>
			/// <param name="x">Relative x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Relative y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Relative z coordinate position of the actuator of the arm.</param>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <param name="elbow">Angle in radians of the elbow of the arm.</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetRelativePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw, ref double elbow)
			{
				return SetRelativePosition(ref x, ref y, ref z, ref roll, ref pitch, ref yaw, ref elbow, DefaultDelay);
			}

			/// <summary>
			/// Request arm to move to the specified position and orientation relative to its current position and orientation
			/// </summary>
			/// <param name="x">Relative x coordinate position of the actuator of the arm.</param>
			/// <param name="y">Relative y coordinate position of the actuator of the arm.</param>
			/// <param name="z">Relative z coordinate position of the actuator of the arm.</param>
			/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
			/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
			/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
			/// <param name="elbow">Angle in radians of the elbow of the arm.</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm moved to specified position. false otherwise</returns>
			public virtual bool SetRelativePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw, ref double elbow, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdArmRelPos;
				// Stores the response from arm and the candidate while moving
				Response rspArmRelPos = null;

				// 1. Prepare the command
				if (!GetResource()) return false;
				
				string parameters =
					x.ToString("0.00") + " " +
					y.ToString("0.00") + " " +
					z.ToString("0.00") + " " +
					roll.ToString("0.00") + " " +
					pitch.ToString("0.00") + " " +
					yaw.ToString("0.00") + " " +
					elbow.ToString("0.00");
				cmdArmRelPos = new Command(sgnArmSetRelPos.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the armSetRelPos command			
				CmdMan.Console("\tMoving arm to relative position " + parameters);
				if (!CmdMan.SendAndWait(cmdArmRelPos, timeOut, out rspArmRelPos))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse arm response
				if (!rspArmRelPos.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tArm did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnArmSetRelPos.Analyze(rspArmRelPos);
				if (saResult.Success)
				{
					saResult.Update<double>(0, ref x);
					saResult.Update<double>(1, ref y);
					saResult.Update<double>(2, ref z);
					saResult.Update<double>(3, ref roll);
					saResult.Update<double>(4, ref pitch);
					saResult.Update<double>(5, ref yaw);
					saResult.Update<double>(6, ref elbow);
				}

				CmdMan.Console("\tMoving arm to position complete");
				return true;
			}

			#endregion

			#region ArmTorque

			/// <summary>
			/// Moves the arm through a secuence of positions
			/// </summary>
			/// <param name="torque">The name of the movement to perform</param>
			/// <returns>true if arm executed the specified movement. false otherwise</returns>
			public virtual bool Torque(string torque)
			{
				return Torque(torque, defaultDelay);
			}

			/// <summary>
			/// Moves the arm through a secuence of positions
			/// </summary>
			/// <param name="torque">The name of the movement to perform</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if arm executed the specified movement. false otherwise</returns>
			public virtual bool Torque(string torque, int timeOut)
			{
				// Stores the command to be sent to arm
				Command cmdMoveArm;
				// Stores the response from arm and the candidate while moving
				Response rspMoveArm = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;

				cmdMoveArm = new Command(sgnArmTorque.CommandName, torque, CmdMan.AutoId++);

				// 2. Send the armGoto command			
				CmdMan.Console("\tSetting arm torque [" + torque + "]");
				if (!CmdMan.SendAndWait(cmdMoveArm, timeOut, out rspMoveArm))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspMoveArm.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tCan not set arm torque");
					return false;
				}
				CmdMan.Console("\tSet arm torque complete");
				return true;
			}

			#endregion

			#endregion

		}
	}
}