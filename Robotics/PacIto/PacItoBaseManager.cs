using System;
using System.Text.RegularExpressions;
using Robotics;
using Robotics.Mathematics;
using Robotics.API;
using Robotics.HAL;

namespace Robotics.PacIto
{
	public partial class PacItoCommandManager
	{
		/// <summary>
		/// Controls remotely the DiferentialPair base of Pac-Ito
		/// </summary>
		public class PacItoBaseManager : SharedResource
		{
			#region Variables

			/// <summary>
			/// Stores the reference to the CommandManager which this object serves
			/// </summary>
			private PacItoCommandManager cmdMan;

			/// <summary>
			/// Stores the default delay time for base commands
			/// </summary>
			private int defaultDelay = 15000;

			/// <summary>
			/// Regular expression used to extract obstacles
			/// </summary>
			protected static Regex rxObstacles = new Regex(@"(?<r>\d+(\.\d+)?)\s+(?<t>\d+(\.\d+)?)");

			#region Signatures

			/// <summary>
			/// Signature to parse move responses
			/// </summary>
			private Signature sgnMove;

			/// <summary>
			/// Signature to parse goto responses
			/// </summary>
			private Signature sgnGoTo;

			/// <summary>
			/// Signature to parse obstacle responses
			/// </summary>
			private Signature sgnObstacle;

			/// <summary>
			/// Signature to parse position responses
			/// </summary>
			private Signature sgnPosition;

			/// <summary>
			/// Signature to parse addobject responses
			/// </summary>
			private Signature sgnAddObject;

			#endregion

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of PacItoCommandManager
			/// <param name="cmdMan">The reference to the CommandManager which this object serves</param>
			/// </summary>
			internal PacItoBaseManager(PacItoCommandManager cmdMan)
				: base()
			{
				this.cmdMan = cmdMan;
				CreateBaseSignatures();
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
			/// Gets or Sets the default delay time for base commands
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
			/// Gets the Signature object to parse mv responses
			/// </summary>
			public virtual Signature SgnBaseMove
			{
				get { return sgnMove; }
			}

			/// <summary>
			/// Signature to parse goto responses
			/// </summary>
			public virtual Signature SgnGoTo
			{
				get { return sgnGoTo; }
			}

			/// <summary>
			/// Signature to parse obstacle responses
			/// </summary>
			public virtual Signature SgnObstacle
			{
				get { return  sgnObstacle;}
			}

			/// <summary>
			/// Signature to parse position responses
			/// </summary>
			public virtual Signature SgnPosition
			{
				get { return  sgnPosition;}
			}

			/// <summary>
			/// Signature to parse addobject responses
			/// </summary>
			public virtual Signature SgnAddObject
			{
				get { return sgnAddObject; }
			}

			#endregion

			#endregion

			#region Methodos

			/// <summary>
			/// Creates the Base Signatures
			/// </summary>
			private void CreateBaseSignatures()
			{
				SignatureBuilder sb = new SignatureBuilder();

				//sb.AddNewFromTypes(typeof(double), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(double));
				sb.AddNewFromTypes(typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(double), typeof(double), typeof(double));
				sgnMove = sb.GenerateSignature("mp_move");
				
				sb.Clear();
				sb.AddNewFromTypes(typeof(string), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(string), typeof(double), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(string), typeof(string), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(string), typeof(string), typeof(double), typeof(double), typeof(double));
				sgnGoTo = sb.GenerateSignature("mp_goto");

				sb.Clear();
				sb.AddNewFromTypes(typeof(string));
				sgnObstacle = sb.GenerateSignature("mp_obstacle");

				sb.Clear();
				sb.AddNewFromTypes(typeof(double), typeof(double), typeof(double));
				sgnPosition = sb.GenerateSignature("mp_position");

				sb.Clear();
				sb.AddNewFromTypes(typeof(string), typeof(double), typeof(double));
				sb.AddNewFromTypes(typeof(string), typeof(double), typeof(double), typeof(double), typeof(double));
				sgnAddObject = sb.GenerateSignature("mp_addobject");
			}

			#region Base Movement methods

			#region Move

			/// <summary>
			/// Request robot base to move to the specified orientation
			/// </summary>
			/// <param name="distance">The distance the robot must move</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool MoveBase(double distance)
			{
				double angle = 0;
				return MoveBase(ref distance, ref angle, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified orientation
			/// </summary>
			/// <param name="distance">The distance the robot must move</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool MoveBase(ref double distance)
			{
				double angle = 0;
				return MoveBase(ref distance, ref angle, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified orientation
			/// </summary>
			/// <param name="distance">The distance the robot must move</param>
			/// <param name="angle">The angle the robot must turn before move</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool MoveBase(double distance, double angle)
			{
				return MoveBase(ref distance, ref angle, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified orientation
			/// </summary>
			/// <param name="distance">The distance the robot must move</param>
			/// <param name="angle">The angle the robot must turn before move</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool MoveBase(ref double distance, ref double angle)
			{
				return MoveBase(ref distance, ref angle, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified orientation
			/// </summary>
			/// <param name="distance">The distance the robot must move</param>
			/// <param name="angle">The angle the robot must turn before move</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool MoveBase(double distance, double angle, int timeOut)
			{
				return MoveBase(ref distance, ref angle, timeOut);
			}

			/// <summary>
			/// Request robot base to move to the specified orientation
			/// </summary>
			/// <param name="distance">The distance the robot must move</param>
			/// <param name="angle">The angle the robot must turn before move</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool MoveBase(ref double distance, ref double angle, int timeOut)
			{
				// Stores the command to be sent to robot base
				Command cmdMove;
				// Stores the response from robot base and the candidate while lookingfor
				Response rspMove = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = distance.ToString("0.00") + " " + angle.ToString("0.00") + " 0.00";
				cmdMove = new Command(sgnMove.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tMoving base with " + cmdMove.StringToSend);
				if (!CmdMan.SendAndWait(cmdMove, timeOut, out rspMove))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse base response
				if (!rspMove.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's base did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnMove.Analyze(rspMove);
				if (saResult.Success)
				{
					saResult.Update<double>(0, ref distance);
					saResult.Update<double>(1, ref angle);
				}

				CmdMan.Console("\tMove robot complete on [" + rspMove.Parameters + "]");
				return true;
			}

			#endregion

			#region GoTo

			#region GoToRoom

			/// <summary>
			/// Request robot base to move to the specified room
			/// </summary>
			/// <param name="room">The name of the destination room</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToRoom(string room)
			{
				return GoToRoom(room, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified room
			/// </summary>
			/// <param name="room">The name of the destination room</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToRoom(string room, int timeOut)
			{
				// Stores the command to be sent to robot base
				Command cmdGoToRoom;
				// Stores the response from robot base and the candidate while lookingfor
				Response rspGoToRoom = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = "room " + room;
				cmdGoToRoom = new Command(sgnGoTo.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tMoving base to room " + cmdGoToRoom.StringToSend);
				if (!CmdMan.SendAndWait(cmdGoToRoom, timeOut, out rspGoToRoom))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse base response
				if (!rspGoToRoom.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's base did not move");
					return false;
				}

				CmdMan.Console("\tMove robot complete to room [" + rspGoToRoom.Parameters + "]");
				return true;
			}

			/// <summary>
			/// Request robot base to move to the specified room
			/// </summary>
			/// <param name="room">The name of the destination room</param>
			/// <param name="angle">The angle the robot must turn on arrival</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToRoom(string room, ref double angle)
			{
				return GoToRoom(room, ref angle, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified room
			/// </summary>
			/// <param name="room">The name of the destination room</param>
			/// <param name="angle">The angle the robot must turn on arrival</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToRoom(string room, ref double angle, int timeOut)
			{
				// Stores the command to be sent to robot base
				Command cmdGoToRoom;
				// Stores the response from robot base and the candidate while lookingfor
				Response rspGoToRoom = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = "room " + room;
				cmdGoToRoom = new Command(sgnGoTo.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tMoving base to room " + cmdGoToRoom.StringToSend);
				if (!CmdMan.SendAndWait(cmdGoToRoom, timeOut, out rspGoToRoom))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse base response
				if (!rspGoToRoom.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's base did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnMove.Analyze(rspGoToRoom);
				if (saResult.Success)
					saResult.Update<double>(2, ref angle);

				CmdMan.Console("\tMove robot complete to room [" + rspGoToRoom.Parameters + "]");
				return true;
			}

			#endregion

			#region GoToRegion

			/// <summary>
			/// Request robot base to move to the specified region
			/// </summary>
			/// <param name="region">The name of the destination region</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToRegion(string region)
			{
				return GoToRegion(region, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified region
			/// </summary>
			/// <param name="region">The name of the destination region</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToRegion(string region, int timeOut)
			{
				// Stores the command to be sent to robot base
				Command cmdGoToRegion;
				// Stores the response from robot base and the candidate while lookingfor
				Response rspGoToRegion = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = "region " + region;
				cmdGoToRegion = new Command(sgnGoTo.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tMoving base to region " + cmdGoToRegion.StringToSend);
				if (!CmdMan.SendAndWait(cmdGoToRegion, timeOut, out rspGoToRegion))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse base response
				if (!rspGoToRegion.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's base did not move");
					return false;
				}

				CmdMan.Console("\tMove robot complete to region [" + rspGoToRegion.Parameters + "]");
				return true;
			}

			/// <summary>
			/// Request robot base to move to the specified region
			/// </summary>
			/// <param name="region">The name of the destination region</param>
			/// <param name="angle">The angle the robot must turn on arrival</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToRegion(string region, ref double angle)
			{
				return GoToRegion(region, ref angle, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified region
			/// </summary>
			/// <param name="region">The name of the destination region</param>
			/// <param name="angle">The angle the robot must turn on arrival</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToRegion(string region, ref double angle, int timeOut)
			{
				// Stores the command to be sent to robot base
				Command cmdGoToRegion;
				// Stores the response from robot base and the candidate while lookingfor
				Response rspGoToRegion = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = "region " + region;
				cmdGoToRegion = new Command(sgnGoTo.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tMoving base to region " + cmdGoToRegion.StringToSend);
				if (!CmdMan.SendAndWait(cmdGoToRegion, timeOut, out rspGoToRegion))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse base response
				if (!rspGoToRegion.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's base did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnMove.Analyze(rspGoToRegion);
				if (saResult.Success)
				{
					saResult.Update<double>(2, ref angle);
				}

				CmdMan.Console("\tMove robot complete to region [" + rspGoToRegion.Parameters + "]");
				return true;
			}

			#endregion

			#region GoToXY

			/// <summary>
			/// Request robot base to move to the specified region
			/// </summary>
			/// <param name="x">The x coordinate position of the robot in the map</param>
			/// <param name="y">The y coordinate position of the robot in the map</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToXY(ref double x, ref double y)
			{
				return GoToXY(ref x, ref y, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified region
			/// </summary>
			/// <param name="x">The x coordinate position of the robot in the map</param>
			/// <param name="y">The y coordinate position of the robot in the map</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToXY(ref double x, ref double y, int timeOut)
			{
				// Stores the command to be sent to robot base
				Command cmdGoToXY;
				// Stores the response from robot base and the candidate while lookingfor
				Response rspGoToXY = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = "xy " + x.ToString("0.00") + " " + y.ToString("0.00");
				cmdGoToXY = new Command(sgnGoTo.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tMoving base to region " + cmdGoToXY.StringToSend);
				if (!CmdMan.SendAndWait(cmdGoToXY, timeOut, out rspGoToXY))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse base response
				if (!rspGoToXY.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's base did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnMove.Analyze(rspGoToXY);
				if (saResult.Success)
				{
					saResult.Update<double>(1, ref x);
					saResult.Update<double>(2, ref y);
				}

				CmdMan.Console("\tMove robot complete to region [" + rspGoToXY.Parameters + "]");
				return true;
			}

			/// <summary>
			/// Request robot base to move to the specified region
			/// </summary>
			/// <param name="x">The x coordinate position of the robot in the map</param>
			/// <param name="y">The y coordinate position of the robot in the map</param>
			/// <param name="angle">The angle the robot must turn on arrival</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToXY(ref double x, ref double y, ref double angle)
			{
				return GoToXY(ref x, ref y, ref angle, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified region
			/// </summary>
			/// <param name="x">The x coordinate position of the robot in the map</param>
			/// <param name="y">The y coordinate position of the robot in the map</param>
			/// <param name="angle">The angle the robot must turn on arrival</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool GoToXY(ref double x, ref double y, ref double angle, int timeOut)
			{
				// Stores the command to be sent to robot base
				Command cmdGoToXY;
				// Stores the response from robot base and the candidate while lookingfor
				Response rspGoToXY = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = "xy " + x.ToString("0.00") + " " + y.ToString("0.00") + " " + angle.ToString("0.00");
				cmdGoToXY = new Command(sgnGoTo.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tMoving base to region " + cmdGoToXY.StringToSend);
				if (!CmdMan.SendAndWait(cmdGoToXY, timeOut, out rspGoToXY))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse base response
				if (!rspGoToXY.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tRobot's base did not move");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnMove.Analyze(rspGoToXY);
				if (saResult.Success)
				{
					saResult.Update<double>(1, ref x);
					saResult.Update<double>(2, ref y);
					saResult.Update<double>(3, ref angle);
				}

				CmdMan.Console("\tMove robot complete to region [" + rspGoToXY.Parameters + "]");
				return true;
			}

			#endregion

			#endregion

			#region Obstacle

			/// <summary>
			/// Request robot base the array of obstacles detected
			/// </summary>
			/// <param name="treshold">The treshold for obstacle detection</param>
			/// <param name="obstacles">An array containing the distances and angles of the detected obstacles</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool Obstacle(double treshold, out Vector2[] obstacles)
			{
				return Obstacle(treshold, out obstacles, DefaultDelay);
			}

			/// <summary>
			/// Request robot base the array of obstacles detected
			/// </summary>
			/// <param name="treshold">The treshold for obstacle detection</param>
			/// <param name="obstacles">An array containing the distances and angles of the detected obstacles</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool Obstacle(double treshold, out Vector2[] obstacles, int timeOut)
			{
				// Stores the command to be sent to robot base
				Command cmdObstacle;
				// Stores the response from robot base and the candidate while lookingfor
				Response rspObstacle = null;
				obstacles = new Vector2[0];

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = treshold.ToString("0.00");
				cmdObstacle = new Command(sgnMove.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tChecking for obstacles " + cmdObstacle.StringToSend);
				if (!CmdMan.SendAndWait(cmdObstacle, timeOut, out rspObstacle))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse base response
				if (!rspObstacle.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tCheck for obstacles failed");
					return false;
				}

				// Get obstacles
				MatchCollection mc = rxObstacles.Matches(rspObstacle.Parameters);
				if (mc.Count > 0)
				{
					int i =0 ;
					obstacles = new Vector2[mc.Count];
					foreach (Match m in mc)
					{
						if (!m.Success)
						{
							obstacles[i++] = new Vector2();
							continue;
						}
						obstacles[i++] = new Vector2(
							Double.Parse(m.Result("${r}")),
							Double.Parse(m.Result("${t}")));
					}
				}

				CmdMan.Console("\tCheck for obstacles complete [" + obstacles.Length + "]");
				return true;
			}

			#endregion

			#region Position

			/// <summary>
			/// Sets the position of the robot
			/// </summary>
			/// <param name="x">The x coordinate position of the robot in the map</param>
			/// <param name="y">The y coordinate position of the robot in the map</param>
			/// <param name="angle">The orientation of the robot in the map</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool Position(double x, double y, double angle)
			{
				return Position(x, y, angle, DefaultDelay);
			}

			/// <summary>
			/// Sets the position of the robot
			/// </summary>
			/// <param name="x">The x coordinate position of the robot in the map</param>
			/// <param name="y">The y coordinate position of the robot in the map</param>
			/// <param name="angle">The orientation of the robot in the map</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool Position(double x, double y, double angle, int timeOut)
			{
				// Stores the command to be sent to robot base
				Command cmdPosition;
				// Stores the response from robot base and the candidate while lookingfor
				Response rspPosition = null;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				string parameters = x.ToString("0.00") + " " + y.ToString("0.00") + " " + angle.ToString("0.00");
				cmdPosition = new Command(sgnPosition.CommandName, parameters, CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tSet robot position " + cmdPosition.StringToSend);
				if (!CmdMan.SendAndWait(cmdPosition, timeOut, out rspPosition))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse base response
				if (!rspPosition.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tFailed to set robot position");
					return false;
				}

				CmdMan.Console("\tSet robot position complete [" + rspPosition.Parameters + "]");
				return true;
			}

			/// <summary>
			/// Request robot base to move to the specified region
			/// </summary>
			/// <param name="x">The x coordinate position of the robot in the map</param>
			/// <param name="y">The y coordinate position of the robot in the map</param>
			/// <param name="angle">The angle the robot must turn on arrival</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool Position(out double x, out double y, out double angle)
			{
				return Position(out x, out y, out angle, DefaultDelay);
			}

			/// <summary>
			/// Request robot base to move to the specified region
			/// </summary>
			/// <param name="x">The x coordinate position of the robot in the map</param>
			/// <param name="y">The y coordinate position of the robot in the map</param>
			/// <param name="angle">The angle the robot must turn on arrival</param>
			/// <param name="timeOut">Amout of time to wait for an arm response in milliseconds</param>
			/// <returns>true if robot moved to the specified location. false otherwise</returns>
			public bool Position(out double x, out double y, out double angle, int timeOut)
			{
				// Stores the command to be sent to robot base
				Command cmdPosition;
				// Stores the response from robot base and the candidate while lookingfor
				Response rspPosition = null;
				x = 0;
				y = 0;
				angle = 0;

				// 1. Prepare the command
				if (!GetResource())
					return false;
				cmdPosition = new Command(sgnPosition.CommandName, "", CmdMan.AutoId++);

				// 2. Send the lookat command			
				CmdMan.Console("\tGeting robot position " + cmdPosition.StringToSend);
				if (!CmdMan.SendAndWait(cmdPosition, timeOut, out rspPosition))
				{
					// 2.1. Cant send socket. Operation canceled.
					CmdMan.Console("\tCan't send command. Operation canceled");
					FreeResource();
					return false;
				}

				// 3. End of move
				FreeResource();

				// 3.2. Parse base response
				if (!rspPosition.Success)
				{
					// 3.2. Failed response
					CmdMan.Console("\tCan not get robot position");
					return false;
				}

				// 4.0 Recover values from response
				SignatureAnalysisResult saResult = sgnPosition.Analyze(rspPosition);
				if (saResult.Success && saResult.ParameterCount >= 3)
				{
					saResult.Update<double>(0, ref x);
					saResult.Update<double>(1, ref y);
					saResult.Update<double>(2, ref angle);
				}

				CmdMan.Console("\tRobot Position [" + rspPosition.Parameters + "]");
				return true;
			}

			#endregion

			#region Add Object

			#endregion

			#endregion

			#endregion

		}
	}
}
