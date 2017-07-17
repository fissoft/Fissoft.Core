using System;

namespace Fissoft
{
	public static class NumericExtensions
	{
		/// <summary>
		/// Determine if two numbers are close in value.
		/// </summary>
		/// <param name="left">First number.</param>
		/// <param name="right">Second number.</param>
		/// <returns>
		/// True if the first number is close in value to the second, false
		/// otherwise.
		/// </returns>
		public static bool AreClose(double left, double right)
		{
			if (left == right)
			{
				return true;
			}
			double num = ((Math.Abs(left) + Math.Abs(right)) + 10.0) * 2.2204460492503131E-16;
			double num2 = left - right;
			return ((-num < num2) && (num > num2));
		}

		/// <summary>
		/// Determine if one number is greater than another.
		/// </summary>
		/// <param name="left">First number.</param>
		/// <param name="right">Second number.</param>
		/// <returns>
		/// True if the first number is greater than the second, false
		/// otherwise.
		/// </returns>
		public static bool IsGreaterThan(double left, double right)
		{
			return ((left > right) && !AreClose(left, right));
		}

		/// <summary>
		/// Determine if one number is less than or close to another.
		/// </summary>
		/// <param name="left">First number.</param>
		/// <param name="right">Second number.</param>
		/// <returns>
		/// True if the first number is less than or close to the second, false
		/// otherwise.
		/// </returns>
		public static bool IsLessThanOrClose(double left, double right)
		{
			if (left >= right)
			{
				return AreClose(left, right);
			}
			return true;
		}
		/// <summary>
		/// Check if a number isn't really a number.
		/// </summary>
		/// <param name="value">The number to check.</param>
		/// <returns>
		/// True if the number is not a number, false if it is a number.
		/// </returns>
		public static bool IsNaN(this double value)
		{
			return double.IsNaN(value);
		}

		/// <summary>
		/// Check if a number is zero.
		/// </summary>
		/// <param name="value">The number to check.</param>
		/// <returns>True if the number is zero, false otherwise.</returns>
		public static bool IsZero(this double value)
		{
			return (Math.Abs(value) < 2.2204460492503131E-15);
		}
	}
}