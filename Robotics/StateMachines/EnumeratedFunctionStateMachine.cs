using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.StateMachines
{
	/// <summary>
	/// A state machine that uses several methods fot its execution, identified by an enumeration
	/// </summary>
	public class EnumeratedFunctionStateMachine<T> : SimpleStateMachine<EnumeratedFunctionState<T>>
		 where T : struct, IComparable, IConvertible, IFormattable
	{
	
	}
}
