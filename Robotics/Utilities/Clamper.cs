using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Utilities
{
	/// <summary>
	/// Provides methods to clamp several data types
	/// </summary>
	public static class Clamper
	{
		/// <summary>
		/// Retrieves a substring of the provided string which length is at most the number of specified characters
		/// </summary>
		/// <param name="s">The string to clamp</param>
		/// <param name="maxLength">The maximum length of the clamped string</param>
		/// <returns>A substring of the provided string which length is at most the number of specified characters</returns>
		public static string Clamp(string s, int maxLength)
		{
			if(s == null)
				return null;
			return s.Substring(0, Math.Min(s.Length, maxLength));
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static byte Clamp(byte value, byte min, byte max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static decimal Clamp(decimal value, decimal min, decimal max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static double Clamp(double value, double min, double max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static float Clamp(float value, float min, float max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static int Clamp(int value, int min, int max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static long Clamp(long value, long min, long max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		
		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static short Clamp(short value, short min, short max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static uint Clamp(uint value, uint min, uint max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static ulong Clamp(ulong value, ulong min, ulong max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static ushort Clamp(ushort value, ushort min, ushort max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}

		#region Extension methods

		/*
		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static byte Clamp(this byte value, byte min, byte max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static decimal Clamp(this decimal value, decimal min, decimal max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static double Clamp(this double value, double min, double max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static float Clamp(this float value, float min, float max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static int Clamp(this int value, int min, int max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static long Clamp(this long value, long min, long max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static sbyte Clamp(this sbyte value, sbyte min, sbyte max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static short Clamp(this short value, short min, short max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static uint Clamp(this uint value, uint min, uint max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static ulong Clamp(this ulong value, ulong min, ulong max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		/// <returns>The clamped value.</returns>
		public static ushort Clamp(this ushort value, ushort min, ushort max)
		{
			if (value > max) return max;
			if (value < min) return min;
			return value;
		}
		
		*/

		#endregion
	}
}
