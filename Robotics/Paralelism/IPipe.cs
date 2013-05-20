using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Paralelism
{
	/// <summary>
	/// Represents a pipe which can be used to communicate two Filter objects.
	/// </summary>
	/// <typeparam name="T">The type of data transmited through the pipe</typeparam>
	public interface IPipe<T>
	{
		#region Properties

		/// <summary>
		/// Gets the maximum capacity of the pipe
		/// </summary>
		int Capacity { get; }

		#endregion

		#region Methodos

		/// <summary>
		/// Reads data from the pipe.
		/// </summary>
		/// <returns>The data written on the other side of the pipe</returns>
		T Read();

		/// <summary>
		/// Writes data to the pipe.
		/// </summary>
		/// <param name="data">The data to be readed on the other side of the pipe</param>
		void Write(T data);

		/// <summary>
		/// Tries to read data from the pipe before the timeout elapses.
		/// </summary>
		/// <param name="timeout">The maximum amount of time in milliseconds to wait for the read operation to complete</param>
		/// <param name="data">When this method returns contains a data written on the other side of the pipe
		/// if the read succeded, or the default value of T if the timeout elapsed</param>
		/// <returns>true if the read operation suceeded, false otherwise.</returns>
		bool TryRead(int timeout, out T data);

		/// <summary>
		/// Writes data to the pipe before the timeout elapses.
		/// </summary>
		/// <param name="timeout">The maximum amount of time in milliseconds to wait for the write operation to complete</param>
		/// <param name="data">The data to be readed by the other side of the pipe.</param>
		/// <returns>true if the write operation suceeded, false otherwise.</returns>
		bool TryWrite(T data, int timeout);

		#endregion
	}
}
