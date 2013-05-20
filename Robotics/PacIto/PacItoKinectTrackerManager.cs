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
		public class PacItoKinectTrackerManager : SharedResource
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
			/// Signature to parse kt_calibrate responses
			/// </summary>
			private Signature sgnCalibrate;

			/// <summary>
			/// Signature to parse kt_find responses
			/// </summary>
			private Signature sgnFindObject;
			
			/// <summary>
			/// Signature to parse kt_locate responses
			/// </summary>
			private Signature sgnLocate;

			/// <summary>
			/// Signature to parse kt_train responses
			/// </summary>
			private Signature sgnTrainObject;

			#endregion

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of PacItoKinectTrackerManager
			/// <param name="cmdMan">The reference to the CommandManager which this object serves</param>
			/// </summary>
			internal PacItoKinectTrackerManager(PacItoCommandManager cmdMan)
			{
				this.cmdMan = cmdMan;
				CreateKinectTrackerSignatures();
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
			/// Gets the signature to parse kt_calibrate responses
			/// </summary>
			public virtual Signature SgnCalibrate
			{
				get { return sgnCalibrate; }
			}

			/// <summary>
			/// Gets the Signature object to parse kt_find responses
			/// </summary>
			public virtual Signature SgnFindObject
			{
				get { return sgnFindObject; }
			}

			/// <summary>
			/// Gets the signature to parse kt_locate responses
			/// </summary>
			public virtual Signature SgnLocate
			{
				get { return sgnLocate; }
			}

			/// <summary>
			/// Gets the Signature object to parse kt_train responses
			/// </summary>
			public virtual Signature SgnTrainObject
			{
				get { return sgnTrainObject; }
			}

			#endregion

			#endregion

			#region Methods

			/// <summary>
			/// Creates the object finder Signatures
			/// </summary>
			private void CreateKinectTrackerSignatures()
			{
				SignatureBuilder sb = new SignatureBuilder();

				sb.AddNewFromTypes(typeof(string));
				sgnFindObject = sb.GenerateSignature("kt_find");
				sgnTrainObject = sb.GenerateSignature("kt_train");
				sb.Clear();

				// Calibra una persona y devuelve el Id de la persona calibrada
				sb.AddNewFromTypes(typeof(int));
				//sb.AddNewFromTypes();
				sgnCalibrate = sb.GenerateSignature("kt_calibrate");
				sb.Clear();
				// Locate
				// Skeleton Devuelve las coordenadas x, y, z del centro del esqueleto
				// Cammera Devuelve la ubicacion del centroide de la camisa de la persona detectada 2 double (ang in rad)
				sb.AddNewFromTypes(typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(double), typeof(double), typeof(double));
				sgnLocate = sb.GenerateSignature("kt_calibrate");
			}

			#region Calibrate

			/// <summary>
			/// Request KinectTracker to find specified object
			/// </summary>
			/// <param name="skeletonId">Id of the skeleton calibrated</param>
			/// <returns>true if calibration was successfull, false otherwise</returns>
			public virtual bool Calibrate(out int skeletonId)
			{
				return Calibrate(out skeletonId, DefaultDelay);
			}

			/// <summary>
			/// Request KinectTracker to find specified object
			/// </summary>
			/// <param name="skeletonId">Id of the skeleton calibrated</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if calibration was successfull, false otherwise</returns>
			public virtual bool Calibrate(out int skeletonId, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdCalibrate;
				// Stores the response from object finder and the candidate while moving
				Response rspCalibrate = null;
				skeletonId = -1;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdCalibrate = new Command(sgnCalibrate.CommandName, "", CmdMan.AutoId++);

				// 2. Send the object finderGoto command			
				CmdMan.Console("\tCalibrating skeleton [" + skeletonId + "]");
				if (!CmdMan.SendAndWait(cmdCalibrate, timeOut, out rspCalibrate))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse human finder response
				if (!rspCalibrate.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tCalibration failed");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnCalibrate.Analyze(rspCalibrate);
				if (saResult.Success && (saResult.ParameterCount == 1))
					saResult.Update<int>(0, ref skeletonId);

				CmdMan.Console("\tCalibrate complete");
				return true;
			}

			#endregion

			#region Find

			/// <summary>
			/// Request KinectTracker to find movement
			/// </summary>
			/// <returns>true if movement was found. false otherwise</returns>
			public virtual bool Find()
			{
				return Find(DefaultDelay);
			}

			/// <summary>
			/// Request KinectTracker to find movement
			/// </summary>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if movement was found. false otherwise</returns>
			public virtual bool Find(int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdFindObjectTop;
				// Stores the response from object finder and the candidate while moving
				Response rspFindObjectTop = null;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdFindObjectTop = new Command(sgnFindObject.CommandName, "", CmdMan.AutoId++);

				// 2. Send the object finderGoto command			
				CmdMan.Console("\tFinding movement");
				if (!CmdMan.SendAndWait(cmdFindObjectTop, timeOut, out rspFindObjectTop))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse human finder response
				return rspFindObjectTop.Success;
			}

			#region Old

			/*
			 * 
			 * 

			/// <summary>
			/// Request KinectTracker to find specified object
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool Find(ref string objectName)
			{
				return Find(ref objectName, DefaultDelay);
			}

			/// <summary>
			/// Request KinectTracker to find specified object
			/// </summary>
			/// <param name="objectName">Name of the object to find</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified object was found. false otherwise</returns>
			public virtual bool Find(ref string objectName, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdFindObjectTop;
				// Stores the response from object finder and the candidate while moving
				Response rspFindObjectTop = null;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdFindObjectTop = new Command(sgnFindObject.CommandName, objectName, CmdMan.AutoId++);

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

				// 3.1. Parse human finder response
				if (!rspFindObjectTop.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHuman not found");
					return false;
				}

				// 4.0 Recover values from response
				objectName = rspFindObjectTop.Parameters;

				CmdMan.Console("\tFind object complete");
				return true;
			}

			*/

			#endregion

			#endregion

			#region TrainObject

			/// <summary>
			/// Request KinectTracker to train (learn) specified object
			/// </summary>
			/// <param name="objectName">Name of the object to train</param>
			/// <returns>true if specified object was trained. false otherwise</returns>
			public virtual bool Train(string objectName)
			{
				return Train(objectName, DefaultDelay);
			}

			/// <summary>
			/// Request KinectTracker train (learn) specified object
			/// </summary>
			/// <param name="objectName">Name of the object to train</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if specified object was trained. false otherwise</returns>
			public virtual bool Train(string objectName, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdTrainObjectTop;
				// Stores the response from object finder and the candidate while moving
				Response rspTrainObjectTop = null;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdTrainObjectTop = new Command(sgnTrainObject.CommandName, objectName, CmdMan.AutoId++);

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

				// 3.1. Parse human finder response
				if (!rspTrainObjectTop.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tObject not found");
					return false;
				}
				CmdMan.Console("\tTrain object complete");
				return true;
			}

			#endregion

			#region LocateSkeleton

			/// <summary>
			/// Request KinectTracker for the centroid of the skeleton
			/// </summary>
			/// <param name="centroidX">X coordinate of the centroid of the Skeleton</param>
			/// <param name="centroidY">Y coordinate of the centroid of the Skeleton</param>
			/// <param name="centroidZ">Z coordinate of the centroid of the Skeleton</param>
			/// <returns>true if centroid was retrieved successfully, false otherwise</returns>
			public virtual bool LocateSkeleton(out double centroidX, out double centroidY, out double centroidZ)
			{
				return LocateSkeleton(out centroidX, out centroidY, out centroidZ, DefaultDelay);
			}

			/// <summary>
			/// Request KinectTracker for the centroid of the skeleton
			/// </summary>
			/// <param name="centroidX">X coordinate of the centroid of the Skeleton</param>
			/// <param name="centroidY">Y coordinate of the centroid of the Skeleton</param>
			/// <param name="centroidZ">Z coordinate of the centroid of the Skeleton</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if centroid was retrieved successfully, false otherwise</returns>
			public virtual bool LocateSkeleton(out double centroidX, out double centroidY, out double centroidZ, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdLocate;
				// Stores the response from object finder and the candidate while moving
				Response rspLocate = null;
				centroidX = 0;
				centroidY = 0;
				centroidZ = 0;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdLocate = new Command(sgnLocate.CommandName, "skeleton", CmdMan.AutoId++);

				// 2. Send the object finderGoto command			
				CmdMan.Console("\tLocating skeleton");
				if (!CmdMan.SendAndWait(cmdLocate, timeOut, out rspLocate))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse human finder response
				if (!rspLocate.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tSkeleton centroid not found");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnCalibrate.Analyze(rspLocate);
				if (saResult.Success && (saResult.ParameterCount == 3))
				{
					saResult.Update<double>(0, ref centroidX);
					saResult.Update<double>(1, ref centroidY);
					saResult.Update<double>(2, ref centroidZ);
				}

				CmdMan.Console("\tLocate Skeleton complete!");
				return true;
			}

			#endregion
			
			#region LocateCamera

			/// <summary>
			/// Request KinectTracker for the centroid of the human chest
			/// </summary>
			/// <param name="hFOV">Horizontal _miVariable of view</param>
			/// <param name="vFOV">Vertical _miVariable of view</param>
			/// <returns>true if centroid was retrieved successfully, false otherwise</returns>
			public virtual bool LocateCamera(out double hFOV, out double vFOV)
			{
				return LocateCamera(out hFOV, out vFOV, DefaultDelay);
			}

			/// <summary>
			/// Request KinectTracker for the centroid of the human chest
			/// </summary>
			/// <param name="hFOV">Horizontal _miVariable of view</param>
			/// <param name="vFOV">Vertical _miVariable of view</param>
			/// <param name="timeOut">Amout of time to wait for a response in milliseconds</param>
			/// <returns>true if centroid was retrieved successfully, false otherwise</returns>
			public virtual bool LocateCamera(out double hFOV, out double vFOV, int timeOut)
			{
				// Stores the command to be sent to object finder
				Command cmdLocate;
				// Stores the response from object finder and the candidate while moving
				Response rspLocate = null;
				hFOV = 0;
				vFOV = 0;

				// 1. Prepare the command
				if (!GetResource()) return false;

				cmdLocate = new Command(sgnLocate.CommandName, "camera", CmdMan.AutoId++);

				// 2. Send the object finderGoto command			
				CmdMan.Console("\tLocating human chest");
				if (!CmdMan.SendAndWait(cmdLocate, timeOut, out rspLocate))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.1. Parse human finder response
				if (!rspLocate.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tHuman chest centroid not found");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnCalibrate.Analyze(rspLocate);
				if (saResult.Success && (saResult.ParameterCount == 2))
				{
					saResult.Update<double>(0, ref hFOV);
					saResult.Update<double>(1, ref vFOV);
				}

				CmdMan.Console("\tLocate human chest complete!");
				return true;
			}

			#endregion

			#endregion

		}
	}
}
