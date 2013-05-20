namespace Robotics
{
	#region Enumerations
	#endregion

	#region General Delegates

	/// <summary>
	/// Represents the method that will handle an event that receives a string as parameter
	/// </summary>
	/// <param name="str">String to pass</param>
	public delegate void StringEventHandler(string str);

	/// <summary>
	/// Represents the method that will handle an event that receives an array of strings as parameter
	/// </summary>
	/// <param name="str">String to pass</param>
	public delegate void StringArrayEventHandler(string[] str);

	/// <summary>
	/// Represents the method that will handle an event that receives a double as parameter 
	/// </summary>
	/// <param name="str">double to pass</param>
	public delegate void DoubleEventHandler(double str);

	/// <summary>
	/// Represents the method that will handle an event that receives no parameters
	/// </summary>
	public delegate void VoidEventHandler();

	#endregion
}