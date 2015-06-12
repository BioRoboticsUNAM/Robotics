namespace Robotics
{
	#region Enumerations
	#endregion

	#region General Delegates

	/// <summary>
	/// Represents a method that will handle an event with a sender and custom parameters
	/// </summary>
	/// <typeparam name="S">The type of the caller of the event</typeparam>
	/// <typeparam name="E">The parameters sent to the event</typeparam>
	/// <param name="sender">The object which raises the event</param>
	/// <param name="eventArgs">The parameters pased to the event handler method</param>
	public delegate void EventHandler<S, E>(S sender, E eventArgs);

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