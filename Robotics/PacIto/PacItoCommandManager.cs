using System;
//using System.Collections.Generic;
using System.Threading;
using Robotics;
using Robotics.API;
using Robotics.HAL;

namespace Robotics.PacIto
{
	/// <summary>
	/// Implements a CommandManager designed to control Robot Pac-Ito
	/// </summary>
	public partial class PacItoCommandManager : CommandManager
	{
		#region Variables

		/// <summary>
		/// Manager for the arm of the robot
		/// </summary>
		protected readonly PacItoArmManager robotArm;

		/// <summary>
		/// Manager for the base of the robot
		/// </summary>
		protected readonly PacItoBaseManager robotBase;

		/// <summary>
		/// Manager for the head of the robot
		/// </summary>
		protected readonly PacItoHeadManager robotHead;

		/// <summary>
		/// Manager for the Kineck
		/// </summary>
		protected readonly PacItoKinectTrackerManager robotKinectTracker;

		/// <summary>
		/// Manager for the manipulator of the robot
		/// </summary>
		protected readonly PacItoManipulatorManager robotManipulator;

		/// <summary>
		/// Manager for the part of the vision related to locate humans
		/// </summary>
		protected readonly PacItoPersonFinderManager robotPersonFinder;

		/// <summary>
		/// Manager for the part of the vision related to locate objects
		/// </summary>
		protected readonly PacItoObjectFinderManager robotObjectFinder;

		/// <summary>
		/// Manager for the Speech-Generator module of the robot
		/// </summary>
		protected readonly PacItoSpeechGenerator robotSpeechGenerator;

		/// <summary>
		/// Manager for the Speech-Recognition module of the robot
		/// </summary>
		protected readonly PacItoSpeechRecognizer robotSpeechRecognizer;

		#region Busy Flags

		

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of PacItoCommandManager
		/// </summary>
		public PacItoCommandManager() : base()
		{
			robotArm = new PacItoArmManager(this);
			robotBase = new PacItoBaseManager(this);
			robotHead = new PacItoHeadManager(this);
			robotManipulator = new PacItoManipulatorManager(this);
			robotObjectFinder = new PacItoObjectFinderManager(this);
			robotPersonFinder = new PacItoPersonFinderManager(this);
			robotSpeechGenerator = new PacItoSpeechGenerator(this);
			robotSpeechRecognizer = new PacItoSpeechRecognizer(this);
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the manager for the arm of the robot
		/// </summary>
		public PacItoArmManager RobotArm
		{ get { return robotArm; } }

		/// <summary>
		/// Gets the manager for the base of the robot
		/// </summary>
		public PacItoBaseManager RobotBase
		{ get{return  robotBase ; }}

		/// <summary>
		/// Gets the manager for the head of the robot
		/// </summary>
		public PacItoHeadManager RobotHead
		{ get{return  robotHead; }}

		/// <summary>
		/// Gets the manager for the Kineck
		/// </summary>
		public PacItoKinectTrackerManager RobotKinectTracker
		{ get { return robotKinectTracker; } }

		/// <summary>
		/// Gets the manager for the manipulator of the robot
		/// </summary>
		public PacItoManipulatorManager RobotManipulator
		{
			get { return robotManipulator; }
		}

		/// <summary>
		/// Gets the manager for the part of the vision related to locate humans
		/// </summary>
		public PacItoPersonFinderManager RobotPersonFinder
		{
			get { return robotPersonFinder; }
		}

		/// <summary>
		/// Gets the manager for the part of the vision related to locate objects
		/// </summary>
		public PacItoObjectFinderManager RobotObjectFinder
		{
			get { return robotObjectFinder; }
		}

		/// <summary>
		/// Gets the manager for the Speech-Recognition module of the robot
		/// </summary>
		public PacItoSpeechRecognizer RobotSpeechRecognizer
		{
			get { return robotSpeechRecognizer; }
		}

		/// <summary>
		/// Gets the manager for the Speech-Generator module of the robot
		/// </summary>
		public PacItoSpeechGenerator RobotSpeechGenerator
		{
			get { return robotSpeechGenerator; }
		}

		#region Default Delays

		/// <summary>
		/// Gets or Sets the default delay time for arm commands
		/// </summary>
		public int ArmDefaultDelay
		{
			get { return robotArm.DefaultDelay; }
			set
			{
				robotArm.DefaultDelay = value;
			}
		}

		/// <summary>
		/// Gets or Sets the default delay time for base commands
		/// </summary>
		public int BaseDefaultDelay
		{
			get { return robotBase.DefaultDelay; }
			set
			{
				robotBase.DefaultDelay = value;
			}
		}

		/// <summary>
		/// Gets or Sets the default delay time for head commands
		/// </summary>
		public int HeadDefaultDelay
		{
			get{return robotHead.DefaultDelay;}
			set
			{
				robotHead.DefaultDelay = value;
			}
		}

		#endregion

		#endregion

		#region Methods

		//public override 

		#region Console Methods

		/// <summary>
		/// When overriden writes the provided text to the Console implementation
		/// </summary>
		/// <param name="text">Text to write</param>
		protected virtual void Console(string text)
		{
		}

		#endregion

		#endregion
	}
}
