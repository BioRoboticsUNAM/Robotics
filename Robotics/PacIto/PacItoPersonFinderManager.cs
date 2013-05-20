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
		/// Controls remotely the person finder module of Pac-Ito
		/// </summary>
		public class PacItoPersonFinderManager : SharedResource
		{
			#region Variables

			/// <summary>
			/// Stores the reference to the CommandManager which this object serves
			/// </summary>
			private PacItoCommandManager cmdMan;

			/// <summary>
			/// Stores the default delay time for person finder commands
			/// </summary>
			private int defaultDelay = 10000;

			#region Signatures

			/// <summary>
			/// Signature to parse find_human responses
			/// </summary>
			private Signature sgnFindHuman;

			/// <summary>
			/// Signature to parse remember_human responses
			/// </summary>
			private Signature sgnRememberHuman;

			#endregion

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of PacItoPersonFinderManager
			/// <param name="cmdMan">The reference to the CommandManager which this object serves</param>
			/// </summary>
			internal PacItoPersonFinderManager(PacItoCommandManager cmdMan)
			{
				this.cmdMan = cmdMan;
				CreatePersonFinderSignatures();
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets or Sets the default delay time for person finder commands
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
			/// Gets the Signature object to parse pr_find responses
			/// </summary>
			public virtual Signature SgnFindHuman
			{
				get { return sgnFindHuman; }
			}

			/// <summary>
			/// Gets the Signature object to parse pf_remember responses
			/// </summary>
			public virtual Signature SgnRememberHuman
			{
				get { return sgnRememberHuman; }
			}

			#endregion

			#endregion

			#region Methods

			/// <summary>
			/// Creates the person finder Signatures
			/// </summary>
			private void CreatePersonFinderSignatures()
			{
				SignatureBuilder sb = new SignatureBuilder();
				sb.AddNewFromTypes(typeof(string), typeof(double), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(string), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(string));
				sgnFindHuman = sb.GenerateSignature("pf_find");
				sgnRememberHuman = sb.GenerateSignature("pf_remember");
			}

			#region FindHuman

			/// <summary>
			/// Request PersonFinder to find specified person
			/// </summary>
			/// <param name="humanName">Name of the person to find</param>
			/// <returns>true if specified person was found. false otherwise</returns>
			public virtual bool FindHuman(ref string humanName)
			{
				double hFOVorX = Double.NaN;
				double vFOVorY = Double.NaN;
				double z = Double.NaN;
				return FindHuman(ref humanName, out hFOVorX, out vFOVorY, out z, DefaultDelay);
			}

			/// <summary>
			/// Request PersonFinder to find specified person
			/// </summary>
			/// <param name="humanName">Name of the person to find</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified person was found. false otherwise</returns>
			public virtual bool FindHuman(ref string humanName, int timeOut)
			{
				double hFOVorX = Double.NaN;
				double vFOVorY = Double.NaN;
				double z = Double.NaN;
				return FindHuman(ref humanName, out hFOVorX, out vFOVorY, out z, timeOut);
			}

			/// <summary>
			/// Request PersonFinder to find specified person
			/// </summary>
			/// <param name="humanName">Name of the person to find</param>
			/// <param name="hFOVorX">When this method returns contains one of the following values:
			/// a) The horizontal fov if the response contains 3 parameters (name and FOVs)
			/// b) The x coordinate of the centroid of the face based on the center of the camera lens if the response contains 4 parameters (name and coords)
			/// c) Double.NaN if no response was received or the response does not contain position data</param>
			/// <param name="vFOVorY">When this method returns contains one of the following values:
			/// a) The vertical fov if the response contains 3 parameters (name and FOVs)
			/// b) The y coordinate of the centroid of the face based on the center of the camera lens if the response contains 4 parameters (name and coords)
			/// c) Double.NaN if no response was received or the response does not contain position data</param>
			/// <param name="z">When this method returns contains one of the following values:
			/// a) The z coordinate of the centroid of the face based on the center of the camera lens if the response contains 4 parameters (name and coords)
			/// b) Double.NaN if no response was received or the response does not contain coordinate position data</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified person was found. false otherwise</returns>
			public virtual bool FindHuman(ref string humanName, out double hFOVorX, out double vFOVorY, out double z, int timeOut)
			{
				// Stores the command to be sent to person finder
				Command cmdFindHuman;
				// Stores the response from person finder and the candidate while moving
				Response rspFindHuman = null;

				hFOVorX = Double.NaN;
				vFOVorY = Double.NaN;
				z = Double.NaN;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdFindHuman = new Command(sgnFindHuman.CommandName, humanName, CmdMan.AutoId++);

				// 2. Send the person finderGoto command			
				CmdMan.Console("\tFinding human [" + humanName + "]");
				if (!CmdMan.SendAndWait(cmdFindHuman, timeOut, out rspFindHuman))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse human finder response
				if (!rspFindHuman.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHuman not found");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult result = sgnFindHuman.Analyze(rspFindHuman);
				if (result.Success)
				{
					result.Update(0, ref humanName);
					if (result.ParameterCount >= 3)
					{
						result.Update(1, ref hFOVorX);
						result.Update(2, ref vFOVorY);
					}
					if (result.ParameterCount >= 4)
						result.Update(3, ref z);
				}

				CmdMan.Console("\tFind human complete");
				return result.Success;
			}

			#endregion

			#region RememberHuman

			/// <summary>
			/// Request PersonFinder to remember (learn) specified person
			/// </summary>
			/// <param name="humanName">Name of the person to remember</param>
			/// <returns>true if specified person was trained. false otherwise</returns>
			public virtual bool RememberHuman(string humanName)
			{
				return RememberHuman(humanName, DefaultDelay);
			}

			/// <summary>
			/// Request PersonFinder remember (learn) specified person
			/// </summary>
			/// <param name="humanName">Name of the person to remember</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified person was trained. false otherwise</returns>
			public virtual bool RememberHuman(string humanName, int timeOut)
			{
				// Stores the command to be sent to person finder
				Command cmdRememberHuman;
				// Stores the response from person finder and the candidate while moving
				Response rspRememberHuman = null;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdRememberHuman = new Command(sgnRememberHuman.CommandName, humanName, CmdMan.AutoId++);

				// 2. Send the person finderGoto command			
				CmdMan.Console("\tTrying to remember human [" + humanName + "]");
				if (!CmdMan.SendAndWait(cmdRememberHuman, timeOut, out rspRememberHuman))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse human finder response
				if (!rspRememberHuman.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHuman not found");
					return false;
				}

				CmdMan.Console("\tRemember human complete");
				return true;
			}

			#endregion

			#endregion
		}
	}
}
