// 
//  Copyright 2009-2024 Antonello Provenzano
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System.Numerics;

namespace Deveel.Math {
	static class Utils {
		public static int URShift(int number, int bits) {
			if (number >= 0)
				return number >> bits;
			return (number >> bits) + (2 << ~bits);
		}

		public static int URShift(int number, long bits) {
			return URShift(number, (int)bits);
		}

		public static long URShift(long number, int bits) {
			if (number >= 0)
				return number >> bits;
			return (number >> bits) + (2L << ~bits);
		}

		public static long URShift(long number, long bits) {
			return URShift(number, (int)bits);
		}

#if NET8_0_OR_GREATER
		public static int NumberOfLeadingZeros(int value) => BitOperations.LeadingZeroCount((uint)value);
		public static int NumberOfLeadingZeros(long value) => BitOperations.LeadingZeroCount((ulong)value);
		public static int NumberOfTrailingZeros(int value) => BitOperations.TrailingZeroCount((uint)value);
		public static int NumberOfTrailingZeros(long value) => (int)BitOperations.TrailingZeroCount((ulong)value);
		public static int BitCount(int x) => BitOperations.PopCount((uint)x);
		public static int BitCount(long x) => (int)BitOperations.PopCount((ulong)x);
		public static int HighestOneBit(int value) => 1 << (31 - BitOperations.LeadingZeroCount((uint)value));
		public static long HighestOneBit(long value) => 1L << (63 - BitOperations.LeadingZeroCount((ulong)value));
#else
		public static int NumberOfLeadingZeros(int value) {
			value |= URShift(value, 1);
			value |= URShift(value, 2);
			value |= URShift(value, 4);
			value |= URShift(value, 8);
			value |= URShift(value, 16);
			return BitCount(~value);
		}

		public static int NumberOfLeadingZeros(long value) {
			value |= URShift(value, 1);
			value |= URShift(value, 2);
			value |= URShift(value, 4);
			value |= URShift(value, 8);
			value |= URShift(value, 16);
			value |= URShift(value, 32);
			return BitCount(~value);
		}

		public static int NumberOfTrailingZeros(int value) {
			return BitCount((value & -value) - 1);
		}

		public static int NumberOfTrailingZeros(long value) {
			return BitCount((value & -value) - 1);
		}

		public static int BitCount(int x) {
			x = ((x >> 1) & 0x55555555) + (x & 0x55555555);
			x = ((x >> 2) & 0x33333333) + (x & 0x33333333);
			x = ((x >> 4) & 0x0f0f0f0f) + (x & 0x0f0f0f0f);
			x = ((x >> 8) & 0x00ff00ff) + (x & 0x00ff00ff);
			return ((x >> 16) & 0x0000ffff) + (x & 0x0000ffff);
		}

		public static int BitCount(long x) {
			x = ((x >> 1) & 0x5555555555555555L) + (x & 0x5555555555555555L);
			x = ((x >> 2) & 0x3333333333333333L) + (x & 0x3333333333333333L);
			int v = (int)(URShift(x, 32) + x);
			v = ((v >> 4) & 0x0f0f0f0f) + (v & 0x0f0f0f0f);
			v = ((v >> 8) & 0x00ff00ff) + (v & 0x00ff00ff);
			return ((v >> 16) & 0x0000ffff) + (v & 0x0000ffff);
		}

		public static int HighestOneBit(int value) {
			value |= URShift(value, 1);
			value |= URShift(value, 2);
			value |= URShift(value, 4);
			value |= URShift(value, 8);
			value |= URShift(value, 16);
			return value ^ URShift(value, 1);
		}

		public static long HighestOneBit(long value) {
			value |= URShift(value, 1);
			value |= URShift(value, 2);
			value |= URShift(value, 4);
			value |= URShift(value, 8);
			value |= URShift(value, 16);
			value |= URShift(value, 32);
			return value ^ URShift(value, 1);
		}
#endif

        /// <summary>
        /// Converts the specified <see cref="double"/> value to a <see cref="long"/>,
        /// handling special cases such as NaN, infinity, and overflow.
        /// </summary>
        /// <param name="d">The double value to convert.</param>
        /// <returns>
        /// Returns the <see cref="long"/> representation of the given double value,
        /// or the nearest limit value in case of overflow.
        /// </returns>
        public static long DoubleToLong(double d)
        {
            if (double.IsNaN(d))
            {
                return 0L;
            }
            if (d >= 9.2233720368547758E+18)
            {
                return 0x7fffffffffffffffL;
            }
            if (d <= -9.2233720368547758E+18)
            {
                return -9223372036854775808L;
            }
            return (long)d;
        }

 

	}
}