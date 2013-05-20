using System;
//using System.Collections.Generic;
using System.Threading;
using Robotics;
using Robotics.API;
using Robotics.HAL;

namespace Robotics.PacIto
{
	public partial class PacItoCommandManager
	{
		/// <summary>
		/// Controls remotely the 3DoF Head of Pac-Ito
		/// </summary>
		public class PacItoHeadManager : SharedResource
		{
			#region Variables

			/// <summary>
			/// Stores the reference to the CommandManager which this object serves
			/// </summary>
			private PacItoCommandManager cmdMan;

			/// <summary>
			/// Stores the default delay time for head commands
			/// </summary>
			private int defaultDelay = 2000;

			#region Signatures

			/// <summary>
			/// Signature to parse lookat responses
			/// </summary>
			private Signature sgnHeadLookAt;

			/// <summary>
			/// Signature to parse show responses
			/// </summary>
			private Signature sgnHeadShow;

			#endregion

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of PacItoCommandManager
			/// <param name="cmdMan">The reference to the CommandManager which this object serves</param>
			/// </summary>
			internal PacItoHeadManager(PacItoCommandManager cmdMan)
				: base()
			{
				this.cmdMan = cmdMan;
				CreateHeadSignatures();
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
			/// Gets or Sets the default delay time for head commands
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
			/// Gets the Signature object to parse lookat responses
			/// </summary>
			public virtual Signature SgnHeadLookAt
			{
				get { return sgnHeadLookAt; }
			}

			#endregion

			#endregion

			#region Methodos

			/// <summary>
			/// Creates the Arm Signatures
			/// </summary>
			private void CreateHeadSignatures()
			{
				SignatureBuilder sb = new SignatureBuilder();

				sb.AddNewFromTypes(typeof(double), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(double), typeof(double));
				sgnHeadLookAt = sb.GenerateSignature("hd_lookat");
				sb.Clear();
				sb.AddNewFromTypes(typeof(string), typeof(double));
				sb.AddNewFromTypes(typeof(string));
				sgnHeadShow = sb.GenerateSignature("hd_show");
			}

			#region Face Movement methods

			/// <summary>
			/// Request head to move the specified orientation
			/// </summary>
			/// <param name="pan">The pan of the face</param>
			/// <param name="tilt">The tilt of the face</param>
			/// <returns>true if head moved to the specified location. false otherwise</returns>
			public bool LookAt(double pan, double tilt)
			{
				return LookAt(ref pan, ref tilt, DefaultDelay);
			}

			/// <summary>
			/// Request head to move the specified orientation
			/// </summary>
			/// <param name="pan">The pan of the face</param>
			/// <param name="tilt">The tilt of the face</param>
			/// <returns>true if head moved to the specified location. false otherwise</returns>
			public bool LookAt(ref double pan, ref double tilt)
			{
				return LookAt(ref pan, ref tilt, DefaultDelay);
			}

			/// <summary>
			/// Request head to move the specified orientation
			/// </summary>
			/// <param name="pan">The pan of the face</param>
			/// <param name="tilt">The tilt of the face</param>
			/// <param name="timeOut">Amout of time to wait for a head response in milliseconds</param>
			/// <returns>true if head moved to the specified location. false otherwise</returns>
			public bool LookAt(double pan, double tilt, int timeOut)
			{
				return LookAt(ref pan, ref tilt, timeOut);
			}

			/// <summary>
			/// Request head to move the specified orientation
			/// </summary>
			/// <param name="pan">The pan of the face</param>
			/// <param name="tilt">The tilt of the face</param>
			/// <param name="timeOut">Amout of time to wait for a head response in milliseconds</param>
			/// <returns>true if head moved to the specified location. false otherwise</returns>
			public bool LookAt(ref double pan, ref double tilt, int timeOut)
			{
				// Stores the command to be sent to head
				Command cmdLookAt;
				// Stores the response from head and the candidate while lookingfor
				Response rspLookAt = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = pan.ToString("0.00") + " " + tilt.ToString("0.00");
				cmdLookAt = new Command(sgnHeadLookAt.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tMoving head to position " + parameters);
				if (!CmdMan.SendAndWait(cmdLookAt, timeOut, out rspLookAt))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspLookAt.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHead did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnHeadLookAt.Analyze(rspLookAt);
				if (saResult.Success && (saResult.ParameterCount == 2))
				{
					saResult.Update<double>(0, ref pan);
					saResult.Update<double>(1, ref tilt);
				}

				CmdMan.Console("\tMoving head to position complete");
				return true;
			}

			/// <summary>
			/// Request head to move the specified orientation
			/// </summary>
			/// <param name="neck">The neck angle of the face</param>
			/// <param name="pan">The pan of the face</param>
			/// <param name="tilt">The tilt of the face</param>
			/// <returns>true if head moved to the specified location. false otherwise</returns>
			public bool LookAt(double neck, double pan, double tilt)
			{
				return LookAt(ref neck, ref pan, ref tilt, DefaultDelay);
			}

			/// <summary>
			/// Request head to move the specified orientation
			/// </summary>
			/// <param name="neck">The neck angle of the face</param>
			/// <param name="pan">The pan of the face</param>
			/// <param name="tilt">The tilt of the face</param>
			/// <returns>true if head moved to the specified location. false otherwise</returns>
			public bool LookAt(ref double neck, ref double pan, double tilt)
			{
				return LookAt(ref neck, ref pan, ref tilt, DefaultDelay);
			}

			/// <summary>
			/// Request head to move the specified orientation
			/// </summary>
			/// <param name="neck">The neck angle of the face</param>
			/// <param name="pan">The pan  of the face</param>
			/// <param name="tilt">The tilt of the face</param>
			/// <param name="timeOut">Amout of time to wait for a head response in milliseconds</param>
			/// <returns>true if head moved to the specified location. false otherwise</returns>
			public bool LookAt(double neck, double pan, double tilt, int timeOut)
			{
				return LookAt(ref neck, ref pan, ref tilt, timeOut);
			}

			/// <summary>
			/// Request head to move the specified orientation
			/// </summary>
			/// <param name="neck">The neck angle of the face</param>
			/// <param name="pan">The pan  of the face</param>
			/// <param name="tilt">The tilt of the face</param>
			/// <param name="timeOut">Amout of time to wait for a head response in milliseconds</param>
			/// <returns>true if head moved to the specified location. false otherwise</returns>
			public bool LookAt(ref double neck, ref double pan, ref double tilt, int timeOut)
			{
				// Stores the command to be sent to head
				Command cmdLookAt;
				// Stores the response from head and the candidate while lookingfor
				Response rspLookAt = null;

				// 1. Prepare the command
				if (!GetResource()) return false;
				string parameters = neck.ToString("0.00") + " " + pan.ToString("0.00") + " " + tilt.ToString("0.00");
				cmdLookAt = new Command(sgnHeadLookAt.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tMoving head to position " + parameters);
				if (!CmdMan.SendAndWait(cmdLookAt, timeOut, out rspLookAt))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspLookAt.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHead did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnHeadLookAt.Analyze(rspLookAt);
				if (saResult.Success && (saResult.ParameterCount == 3))
				{
					saResult.Update<double>(0, ref neck);
					saResult.Update<double>(1, ref pan);
					saResult.Update<double>(2, ref tilt);
				}

				CmdMan.Console("\tMoving head to position complete");
				return true;
			}

			/// <summary>
			/// Request head to show the specified expression
			/// </summary>
			/// <param name="expression">The expression to be shown by the face</param>
			/// <returns>true if head showed the specified expression. false otherwise</returns>
			public bool Show(string expression)
			{
				return Show(expression, defaultDelay);
			}

			/// <summary>
			/// Request head to show the specified expression
			/// </summary>
			/// <param name="expression">The expression to be shown by the face</param>
			/// <param name="timeOut">Amout of time to wait for a head response in milliseconds</param>
			/// <returns>true if head showed the specified expression. false otherwise</returns>
			public bool Show(string expression, int timeOut)
			{
				// Stores the command to be sent to head
				Command cmdShow;
				// Stores the response from head and the candidate while lookingfor
				Response rspShow = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = expression;
				cmdShow = new Command(sgnHeadShow.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the show command			
				CmdMan.Console("\tShowing head expression " + parameters);
				if (!CmdMan.SendAndWait(cmdShow, timeOut, out rspShow))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspShow.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHead did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnHeadLookAt.Analyze(rspShow);
				if (saResult.Success && (saResult.ParameterCount == 2))
					saResult.Update<string>(0, ref expression);

				CmdMan.Console("\tShow expression complete");
				return true;
			}

			/// <summary>
			/// Request head to show the specified expression
			/// </summary>
			/// <param name="expression">The expression to be shown by the face</param>
			/// <param name="showTime">The amoun of time in seconds the expression will be shown</param>
			/// <returns>true if head showed the specified expression. false otherwise</returns>
			public bool Show(string expression, double showTime)
			{
				return Show(expression, showTime, defaultDelay);
			}

			/// <summary>
			/// Request head to show the specified expression
			/// </summary>
			/// <param name="expression">The expression to be shown by the face</param>
			/// <param name="showTime">The amoun of time in seconds the expression will be shown</param>
			/// <param name="timeOut">Amout of time to wait for a head response in milliseconds</param>
			/// <returns>true if head showed the specified expression. false otherwise</returns>
			public bool Show(string expression, double showTime, int timeOut)
			{
				// Stores the command to be sent to head
				Command cmdShow;
				// Stores the response from head and the candidate while lookingfor
				Response rspShow = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = expression + " " + showTime.ToString("0.00");
				cmdShow = new Command(sgnHeadShow.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the show command			
				CmdMan.Console("\tShowing head expression " + parameters);
				if (!CmdMan.SendAndWait(cmdShow, timeOut, out rspShow))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspShow.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHead did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnHeadLookAt.Analyze(rspShow);
				if (saResult.Success && (saResult.ParameterCount == 2))
				{
					saResult.Update<string>(0, ref expression);
					saResult.Update<double>(1, ref showTime);
				}

				CmdMan.Console("\tShow expression complete");
				return true;
			}			

			#endregion

			#endregion
		}
	}
}
