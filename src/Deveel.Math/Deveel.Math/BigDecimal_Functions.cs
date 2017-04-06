// 
//  Copyright 2009-2017  Deveel
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
	public sealed partial class BigDecimal {
		/**
		 * Returns a new {@code BigDecimal} instance with the same value as {@code
		 * this} but with a unscaled value where the trailing zeros have been
		 * removed. If the unscaled value of {@code this} has n trailing zeros, then
		 * the scale and the precision of the result has been reduced by n.
		 *
		 * @return a new {@code BigDecimal} instance equivalent to this where the
		 *         trailing zeros of the unscaled value have been removed.
		 */

		public BigDecimal StripTrailingZeros() {
			int i = 1; // 1 <= i <= 18
			int lastPow = TenPow.Length - 1;
			long newScale = _scale;

			if (IsZero) {
				return BigDecimal.Parse("0");
			}
			BigInteger strippedBI = GetUnscaledValue();
			BigInteger quotient;
			BigInteger remainder;

			// while the number is even...
			while (!BigInteger.TestBit(strippedBI, 0)) {
				// To divide by 10^i
				quotient = BigMath.DivideAndRemainder(strippedBI, TenPow[i], out remainder);
				// To look the remainder
				if (remainder.Sign == 0) {
					// To adjust the scale
					newScale -= i;
					if (i < lastPow) {
						// To set to the next power
						i++;
					}
					strippedBI = quotient;
				} else {
					if (i == 1) {
						// 'this' has no more trailing zeros
						break;
					}
					// To set to the smallest power of ten
					i = 1;
				}
			}
			return new BigDecimal(strippedBI, ToIntScale(newScale));
		}
	}
}
