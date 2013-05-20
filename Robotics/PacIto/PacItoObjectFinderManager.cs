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
		/// Controls remotely the object finder of Pac-Ito
		/// </summary>
		public class PacItoObjectFinderManager : SharedResource
		{
			#region Variables

			/// <summary>
			/// Stores the reference to the CommandManager which this object serves
			/// </summary>
			private PacItoCommandManager cmdMan;

			/// <summary>
			/// Stores the default delay time for object finder commands
			/// </summary>
			private int defaultDelay = 10000;

			#region Signatures

			/// <summary>
			/// Signature to parse oft_find responses
			/// </summary>
			private Signature sgnFindObjectTop;

			/// <summary>
			/// Signature to parse ofm_find responses
			/// </summary>
			private Signature sgnFindObjectMiddle;

			/// <summary>
			/// Signature to parse ofb_find responses
			/// </summary>
			private Signature sgnFindObjectBottom;

			/// <summary>
			/// Signature to parse oft_train responses
			/// </summary>
			private Signature sgnTrainObjectTop;

			/// <summary>
			/// Signature to parse ofm_train responses
			/// </summary>
			private Signature sgnTrainObjectMiddle;

			/// <summary>
			/// Signature to parse ofb_train responses
			/// </summary>
			private Signature sgnTrainObjectBottom;

			#endregion

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of PacItoObjectFinderManager
			/// <param name="cmdMan">The reference to the CommandManager which this object serves</param>
			/// </summary>
			internal PacItoObjectFinderManager(PacItoCommandManager cmdMan)
			{
				this.cmdMan = cmdMan;
				CreateObjectFinderSignatures();
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets or Sets the default delay time for object finder commands
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
			/// Gets the Signature object to parse oft_find responses
			/// </summary>
			public virtual Signature SgnFindObjectTop
			{
				get { return sgnFindObjectTop; }
			}

			/// <summary>
			/// Gets the Signature object to parse oft_train responses
			/// </summary>
			public virtual Signature SgnTrainObjectTop
			{
				get { return sgnTrainObjectTop; }
			}

			/// <summary>
			/// Gets the Signature object to parse ofm_find responses
			/// </summary>
			public virtual Signature SgnFindObjectMiddle
			{
				get { return sgnFindObjectMiddle; }
			}

			/// <summary>
			/// Gets the Signature object to parse ofm_train responses
			/// </summary>
			public virtual Signature SgnTrainObjectMiddle
			{
				get { return sgnTrainObjectMiddle; }
			}

			/// <summary>
			/// Gets the Signature object to parse ofb_find responses
			/// </summary>
			public virtual Signature SgnFindObjectBottom
			{
				get { return sgnFindObjectBottom; }
			}

			/// <summary>
			/// Gets the Signature object to parse ofb_train responses
			/// </summary>
			public virtual Signature SgnTrainObjectBottom
			{
				get { return sgnTrainObjectBottom; }
			}

			#endregion

			#endregion

			#region Methods

			/// <summary>
			/// Creates the object finder Signatures
			/// </summary>
			private void CreateObjectFinderSignatures()
			{
				SignatureBuilder sb = new SignatureBuilder();

				sb.AddNewFromTypes(typeof(string), typeof(double), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(string), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(string));
				sgnFindObjectTop = sb.GenerateSignature("oft_find");
				sgnFindObjectMiddle = sb.GenerateSignature("ofm_find");
				sgnFindObjectBottom = sb.GenerateSignature("ofb_find");

				sb.Clear();
				sb.AddNewFromTypes(typeof(string));
				sgnTrainObjectTop = sb.GenerateSignature("oft_train");
				sgnTrainObjectMiddle = sb.GenerateSignature("ofm_train");
				sgnTrainObjectBottom = sb.GenerateSignature("ofb_train");
			}

			#region FindObject

			/// <summary>
			/// Request ObjectFinder to find specified object with the camera located on Top
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool FindObjectTop(ref string objectName)
			{
				double x, y, z;
				return FindObjectTop(ref objectName, out x, out y, out z, DefaultDelay);
			}

			/// <summary>
			/// Request ObjectFinder to find specified object with the camera located on Top
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <param name="x">When this method returns contains one of this values:
			/// a) The x-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the HFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="y">When this method returns contains one of this values:
			/// a) The y-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the VFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="z">When this method returns contains one of this values:
			/// a) The z-coordinate of the object position if the response contains three position values
			/// b) Double.NaN
			/// </param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool FindObjectTop(ref string objectName, out double x, out double y, out double z)
			{
				return FindObjectTop(ref objectName, out x, out y, out z, DefaultDelay);
			}

			/// <summary>
			/// Request ObjectFinder to find specified object with the camera located on Top
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <param name="x">When this method returns contains one of this values:
			/// a) The x-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the HFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="y">When this method returns contains one of this values:
			/// a) The y-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the VFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="z">When this method returns contains one of this values:
			/// a) The z-coordinate of the object position if the response contains three position values
			/// b) Double.NaN
			/// </param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool FindObjectTop(ref string objectName, out double x, out double y, out double z, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdFindObjectTop;
				// Stores the response from object finder and the candidate while moving
				Response rspFindObjectTop = null;
				x = Double.NaN;
				y = Double.NaN;
				z = Double.NaN;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdFindObjectTop = new Command(sgnFindObjectTop.CommandName, objectName, CmdMan.AutoId++);

				// 2. Send the object finderGoto command			
				CmdMan.Console("\tFinding object [" + objectName + "]");
				if (!CmdMan.SendAndWait(cmdFindObjectTop, timeOut, out rspFindObjectTop))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspFindObjectTop.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHuman not found");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult result = sgnFindObjectTop.Analyze(rspFindObjectTop);
				if(result.Success)
				{
					result.Update<string>(0, ref objectName);
					if(result.ParameterCount == 4)
					{
						result.Update<double>(1, ref x);
						result.Update<double>(2, ref y);
						result.Update<double>(3, ref z);
					}
					else if (result.ParameterCount == 3)
					{
						result.Update<double>(1, ref x);
						result.Update<double>(2, ref y);
						z = Double.NaN;
					}
				}

				CmdMan.Console("\tFind object complete");
				return true;
			}

			/// <summary>
			/// Request ObjectFinder to find specified object with the camera located on Middle
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool FindObjectMiddle(ref string objectName)
			{
				double x, y, z;
				return FindObjectMiddle(ref objectName, out x, out y, out z, DefaultDelay);
			}

			/// <summary>
			/// Request ObjectFinder to find specified object with the camera located on Middle
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <param name="x">When this method returns contains one of this values:
			/// a) The x-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the HFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="y">When this method returns contains one of this values:
			/// a) The y-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the VFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="z">When this method returns contains one of this values:
			/// a) The z-coordinate of the object position if the response contains three position values
			/// b) Double.NaN
			/// </param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool FindObjectMiddle(ref string objectName, out double x, out double y, out double z)
			{
				return FindObjectMiddle(ref objectName, out x, out y, out z, DefaultDelay);
			}

			/// <summary>
			/// Request ObjectFinder to find specified object with the camera located on Middle
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <param name="x">When this method returns contains one of this values:
			/// a) The x-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the HFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="y">When this method returns contains one of this values:
			/// a) The y-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the VFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="z">When this method returns contains one of this values:
			/// a) The z-coordinate of the object position if the response contains three position values
			/// b) Double.NaN
			/// </param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool FindObjectMiddle(ref string objectName, out double x, out double y, out double z, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdFindObjectMiddle;
				// Stores the response from object finder and the candidate while moving
				Response rspFindObjectMiddle = null;
				x = Double.NaN;
				y = Double.NaN;
				z = Double.NaN;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdFindObjectMiddle = new Command(sgnFindObjectMiddle.CommandName, objectName, CmdMan.AutoId++);

				// 2. Send the object finderGoto command			
				CmdMan.Console("\tFinding object [" + objectName + "]");
				if (!CmdMan.SendAndWait(cmdFindObjectMiddle, timeOut, out rspFindObjectMiddle))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspFindObjectMiddle.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHuman not found");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult result = sgnFindObjectMiddle.Analyze(rspFindObjectMiddle);
				if (result.Success)
				{
					result.Update<string>(0, ref objectName);
					if (result.ParameterCount == 4)
					{
						result.Update<double>(1, ref x);
						result.Update<double>(2, ref y);
						result.Update<double>(3, ref z);
					}
					else if (result.ParameterCount == 3)
					{
						result.Update<double>(1, ref x);
						result.Update<double>(2, ref y);
						z = Double.NaN;
					}
				}

				CmdMan.Console("\tFind object complete");
				return true;
			}

			/// <summary>
			/// Request ObjectFinder to find specified object with the camera located on Bottom
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool FindObjectBottom(ref string objectName)
			{
				double x, y, z;
				return FindObjectBottom(ref objectName, out x, out y, out z, DefaultDelay);
			}

			/// <summary>
			/// Request ObjectFinder to find specified object with the camera located on Bottom
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <param name="x">When this method returns contains one of this values:
			/// a) The x-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the HFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="y">When this method returns contains one of this values:
			/// a) The y-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the VFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="z">When this method returns contains one of this values:
			/// a) The z-coordinate of the object position if the response contains three position values
			/// b) Double.NaN
			/// </param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool FindObjectBottom(ref string objectName, out double x, out double y, out double z)
			{
				return FindObjectBottom(ref objectName, out x, out y, out z, DefaultDelay);
			}

			/// <summary>
			/// Request ObjectFinder to find specified object with the camera located on Bottom
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <param name="x">When this method returns contains one of this values:
			/// a) The x-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the HFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="y">When this method returns contains one of this values:
			/// a) The y-coordinate of the object position if the response contains three position values
			/// b) The location of the object respect to the VFOV of the camera (angle measured in radians) if the response contains two position values.
			/// c) Double.NaN if the response does not contains position values
			/// </param>
			/// <param name="z">When this method returns contains one of this values:
			/// a) The z-coordinate of the object position if the response contains three position values
			/// b) Double.NaN
			/// </param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool FindObjectBottom(ref string objectName, out double x, out double y, out double z, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdFindObjectBottom;
				// Stores the response from object finder and the candidate while moving
				Response rspFindObjectBottom = null;
				x = Double.NaN;
				y = Double.NaN;
				z = Double.NaN;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdFindObjectBottom = new Command(sgnFindObjectBottom.CommandName, objectName, CmdMan.AutoId++);

				// 2. Send the object finderGoto command			
				CmdMan.Console("\tFinding object [" + objectName + "]");
				if (!CmdMan.SendAndWait(cmdFindObjectBottom, timeOut, out rspFindObjectBottom))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspFindObjectBottom.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHuman not found");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult result = sgnFindObjectBottom.Analyze(rspFindObjectBottom);
				if (result.Success)
				{
					result.Update<string>(0, ref objectName);
					if (result.ParameterCount == 4)
					{
						result.Update<double>(1, ref x);
						result.Update<double>(2, ref y);
						result.Update<double>(3, ref z);
					}
					else if (result.ParameterCount == 3)
					{
						result.Update<double>(1, ref x);
						result.Update<double>(2, ref y);
						z = Double.NaN;
					}
				}

				CmdMan.Console("\tFind object complete");
				return true;
			}

			#endregion

			#region TrainObject

			/// <summary>
			/// Request ObjectFinder to train (learn) specified object
			/// </summary>
			/// <param name="objectName">Name of the object to train</param>
			/// <returns>true if specified object was trained. false otherwise</returns>
			public virtual bool TrainObjectTop(string objectName)
			{
				return TrainObjectTop(objectName, DefaultDelay);
			}

			/// <summary>
			/// Request ObjectFinder train (learn) specified object
			/// </summary>
			/// <param name="objectName">Name of the object to train</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified object was trained. false otherwise</returns>
			public virtual bool TrainObjectTop(string objectName, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdTrainObjectTop;
				// Stores the response from object finder and the candidate while moving
				Response rspTrainObjectTop = null;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdTrainObjectTop = new Command(sgnTrainObjectTop.CommandName, objectName, CmdMan.AutoId++);

				// 2. Send the object finderGoto command			
				CmdMan.Console("\tTrying to train object [" + objectName + "]");
				if (!CmdMan.SendAndWait(cmdTrainObjectTop, timeOut, out rspTrainObjectTop))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspTrainObjectTop.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tObject not found");
					return false;
				}
				CmdMan.Console("\tTrain object complete");
				return true;
			}

			/// <summary>
			/// Request ObjectFinder to train (learn) specified object
			/// </summary>
			/// <param name="objectName">Name of the object to train</param>
			/// <returns>true if specified object was trained. false otherwise</returns>
			public virtual bool TrainObjectMiddle(string objectName)
			{
				return TrainObjectMiddle(objectName, DefaultDelay);
			}

			/// <summary>
			/// Request ObjectFinder train (learn) specified object
			/// </summary>
			/// <param name="objectName">Name of the object to train</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified object was trained. false otherwise</returns>
			public virtual bool TrainObjectMiddle(string objectName, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdTrainObjectMiddle;
				// Stores the response from object finder and the candidate while moving
				Response rspTrainObjectMiddle = null;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdTrainObjectMiddle = new Command(sgnTrainObjectMiddle.CommandName, objectName, CmdMan.AutoId++);

				// 2. Send the object finderGoto command			
				CmdMan.Console("\tTrying to train object [" + objectName + "]");
				if (!CmdMan.SendAndWait(cmdTrainObjectMiddle, timeOut, out rspTrainObjectMiddle))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspTrainObjectMiddle.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tObject not found");
					return false;
				}
				CmdMan.Console("\tTrain object complete");
				return true;
			}

			/// <summary>
			/// Request ObjectFinder to train (learn) specified object
			/// </summary>
			/// <param name="objectName">Name of the object to train</param>
			/// <returns>true if specified object was trained. false otherwise</returns>
			public virtual bool TrainObjectBottom(string objectName)
			{
				return TrainObjectBottom(objectName, DefaultDelay);
			}

			/// <summary>
			/// Request ObjectFinder train (learn) specified object
			/// </summary>
			/// <param name="objectName">Name of the object to train</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified object was trained. false otherwise</returns>
			public virtual bool TrainObjectBottom(string objectName, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdTrainObjectBottom;
				// Stores the response from object finder and the candidate while moving
				Response rspTrainObjectBottom = null;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdTrainObjectBottom = new Command(sgnTrainObjectBottom.CommandName, objectName, CmdMan.AutoId++);

				// 2. Send the object finderGoto command			
				CmdMan.Console("\tTrying to train object [" + objectName + "]");
				if (!CmdMan.SendAndWait(cmdTrainObjectBottom, timeOut, out rspTrainObjectBottom))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse head response
				if (!rspTrainObjectBottom.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tObject not found");
					return false;
				}
				CmdMan.Console("\tTrain object complete");
				return true;
			}

			#endregion

			#endregion

		}
	}
}