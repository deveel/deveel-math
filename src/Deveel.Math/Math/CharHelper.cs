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

using System;

namespace Deveel.Math {
#if NET_2_0
	static
#else
	sealed 
#endif
	/// <summary>
	/// Provides helper methods for converting between numeric digits and their
	/// character representations in various radices (numeral systems),
	/// mirroring the functionality of Java's <c>java.lang.Character</c>.
	/// </summary>
	class CharHelper {
#if !NET_2_0
		private CharHelper() {
		}
#endif
		/// <summary>
		/// The minimum radix supported for digit conversion (2).
		/// </summary>
		public const int MIN_RADIX = 2;
		/// <summary>
		/// The maximum radix supported for digit conversion (36).
		/// </summary>
		public const int MAX_RADIX = 36;

		/// <summary>
		/// Converts the specified digit to its character representation in the given radix.
		/// </summary>
		/// <param name="digit">The numeric digit value to convert (must be non-negative and less than <paramref name="radix"/>).</param>
		/// <param name="radix">The radix of the numeral system (must be between <see cref="MIN_RADIX"/> and <see cref="MAX_RADIX"/>).</param>
		/// <returns>The character representation of <paramref name="digit"/> in the given <paramref name="radix"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="radix"/> is outside the valid range, or when <paramref name="digit"/> is not valid for the given radix.</exception>
		public static char forDigit(int digit, int radix) {
			if (radix < MIN_RADIX || radix > MAX_RADIX)
				throw new ArgumentOutOfRangeException("radix");

			if (digit < 0 || digit >= radix)
				throw new ArgumentOutOfRangeException("digit");

			if (digit < 10)
				return (char)(digit + (int)'0');

			return (char)(digit - 10 + (int)'a');
		}


		/// <summary>
		/// Converts a character to its corresponding numeric digit value in the given radix.
		/// </summary>
		/// <param name="ch">The character to convert (e.g., '0'-'9', 'a'-'z', or 'A'-'Z').</param>
		/// <param name="radix">The radix of the numeral system (must be between <see cref="MIN_RADIX"/> and <see cref="MAX_RADIX"/>).</param>
		/// <returns>The numeric value of <paramref name="ch"/> in the given <paramref name="radix"/>, or -1 if the character is not valid for the radix.</returns>
		public static int toDigit(char ch, int radix) {
			if (radix < MIN_RADIX || radix > MAX_RADIX)
				return -1;

			int digit = -1;

			ch = Char.ToLowerInvariant(ch);

			if ((ch >= '0') && (ch <= '9')) {
				digit = ((int)ch - (int)'0');
			} else {
				if ((ch >= 'a') && (ch <= 'z')) {
					digit = ((int)ch - (int)'a') + 10;
				}
			}

			return digit < radix ? digit : -1;
		}
	}
}