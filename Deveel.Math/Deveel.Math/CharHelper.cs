using System;

namespace Deveel.Math {
#if NET_2_0
	static
#else
	sealed 
#endif
	class CharHelper {
#if !NET_2_0
		private CharHelper() {
		}
#endif
		public const int MIN_RADIX = 2;
		public const int MAX_RADIX = 36;

		public static char forDigit(int digit, int radix) {
			if (radix < MIN_RADIX || radix > MAX_RADIX)
				throw new ArgumentOutOfRangeException("radix");

			if (digit < 0 || digit >= radix)
				throw new ArgumentOutOfRangeException("digit");

			if (digit < 10)
				return (char)(digit + (int)'0');

			return (char)(digit - 10 + (int)'a');
		}


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