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
using System.Globalization;
using System.Text;

namespace Deveel.Math {
	public sealed partial class BigDecimal {
		private static bool TryParse(char[] inData, int offset, int len, IFormatProvider provider, out BigDecimal value,
			out Exception exception) {
			if (inData == null || inData.Length == 0) {
				exception = new FormatException("Cannot parse an empty string.");
				value = null;
				return false;
			}

			var numberformatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			if (numberformatInfo == null)
				numberformatInfo = NumberFormatInfo.CurrentInfo;

			var decSep = numberformatInfo.NumberDecimalSeparator;
			if (decSep.Length > 1) {
				exception = new NotSupportedException("More than one decimal separator not yet supported");
				value = null;
				return false;
			}

			var cDecSep = decSep[0];

			int begin = offset; // first index to be copied
			int last = offset + (len - 1); // last index to be copied

			if ((last >= inData.Length) || (offset < 0) || (len <= 0) || (last < 0)) {
				exception = new FormatException();
				value = null;
				return false;
			}

			var v = new BigDecimal();

			try {
				var unscaledBuffer = new StringBuilder(len);
				int bufLength = 0;
				// To skip a possible '+' symbol
				if ((offset <= last) && (inData[offset] == '+')) {
					offset++;
					begin++;
				}

				int counter = 0;
				bool wasNonZero = false;
				// Accumulating all digits until a possible decimal point
				for (;
					(offset <= last) &&
					(inData[offset] != cDecSep) &&
					(inData[offset] != 'e') &&
					(inData[offset] != 'E');
					offset++) {
					if (!wasNonZero) {
						if (inData[offset] == '0') {
							counter++;
						} else {
							wasNonZero = true;
						}
					}
				}

				unscaledBuffer.Append(inData, begin, offset - begin);
				bufLength += offset - begin;
				// A decimal point was found
				if ((offset <= last) && (inData[offset] == cDecSep)) {
					offset++;
					// Accumulating all digits until a possible exponent
					begin = offset;
					for (;
						(offset <= last) &&
						(inData[offset] != 'e') &&
						(inData[offset] != 'E');
						offset++) {
						if (!wasNonZero) {
							if (inData[offset] == '0') {
								counter++;
							} else {
								wasNonZero = true;
							}
						}
					}

					v._scale = offset - begin;
					bufLength += v._scale;
					unscaledBuffer.Append(inData, begin, v._scale);
				} else {
					v._scale = 0;
				}
				// An exponent was found
				if ((offset <= last) && ((inData[offset] == 'e') || (inData[offset] == 'E'))) {
					offset++;
					// Checking for a possible sign of scale
					begin = offset;
					if ((offset <= last) && (inData[offset] == '+')) {
						offset++;
						if ((offset <= last) && (inData[offset] != '-')) {
							begin++;
						}
					}

					// Accumulating all remaining digits
					String scaleString = new String(inData, begin, last + 1 - begin); // buffer for scale
					// Checking if the scale is defined            
					long newScale = (long)v._scale - Int32.Parse(scaleString, provider); // the new scale
					v._scale = (int)newScale;
					if (newScale != v._scale) {
						// math.02=Scale out of range.
						throw new FormatException(Messages.math02); //$NON-NLS-1$
					}
				}

				// Parsing the unscaled value
				if (bufLength < 19) {
					if (!Int64.TryParse(unscaledBuffer.ToString(), NumberStyles.Integer, provider, out v.smallValue)) {
						value = null;
						exception = new FormatException();
						return false;
					}

					v._bitLength = BitLength(v.smallValue);
				} else {
					v.SetUnscaledValue(BigInteger.Parse(unscaledBuffer.ToString()));
				}

				v._precision = unscaledBuffer.Length - counter;
				if (unscaledBuffer[0] == '-') {
					v._precision--;
				}

				value = v;
				exception = null;
				return true;
			} catch (Exception ex) {
				exception = ex;
				value = null;
				return false;
			}
		}

		public static bool TryParse(char[] chars, int offset, int length, out BigDecimal value) {
			return TryParse(chars, offset, length, NumberFormatInfo.InvariantInfo, out value);
		}

		public static bool TryParse(char[] chars, int offset, int length, IFormatProvider provider, out BigDecimal value) {
			return TryParse(chars, offset, length, null, provider, out value);
		}

		public static bool TryParse(char[] chars, int offset, int length, MathContext context, out BigDecimal value) {
			return TryParse(chars, offset, length, context, null, out value);
		}

		public static bool TryParse(char[] chars, int offset, int length, MathContext context, IFormatProvider provider,
			out BigDecimal value) {
			Exception error;
			if (!TryParse(chars, offset, length, provider, out value, out error))
				return false;

			if (context != null)
				value.InplaceRound(context);

			return true;
		}

		public static bool TryParse(char[] chars, out BigDecimal value) {
			return TryParse(chars, (MathContext)null, out value);
		}

		public static bool TryParse(char[] chars, MathContext context, out BigDecimal value) {
			return TryParse(chars, context, NumberFormatInfo.InvariantInfo, out value);
		}

		public static bool TryParse(char[] chars, IFormatProvider provider, out BigDecimal value) {
			return TryParse(chars, null, provider, out value);
		}

		public static bool TryParse(char[] chars, MathContext context, IFormatProvider provider, out BigDecimal value) {
			if (chars == null) {
				value = null;
				return false;
			}

			return TryParse(chars, 0, chars.Length, context, provider, out value);
		}

		public static BigDecimal Parse(char[] chars, int offset, int length, IFormatProvider provider) {
			return Parse(chars, offset, length, null, provider);
		}

		public static BigDecimal Parse(char[] chars, int offset, int length) {
			return Parse(chars, offset, length, (MathContext)null);
		}

		public static BigDecimal Parse(char[] chars, int offset, int length, MathContext context) {
			return Parse(chars, offset, length, context, NumberFormatInfo.InvariantInfo);
		}

		public static BigDecimal Parse(char[] chars, int offset, int length, MathContext context, IFormatProvider provider) {
			Exception error;
			BigDecimal value;
			if (!TryParse(chars, offset, length, provider, out value, out error))
				throw error;

			if (context != null)
				value.InplaceRound(context);

			return value;
		}

		public static BigDecimal Parse(char[] chars, IFormatProvider provider) {
			return Parse(chars, null, provider);
		}

		public static BigDecimal Parse(char[] chars) {
			return Parse(chars, (MathContext)null);
		}

		public static BigDecimal Parse(char[] chars, MathContext context) {
			return Parse(chars, context, NumberFormatInfo.InvariantInfo);
		}

		public static BigDecimal Parse(char[] chars, MathContext context, IFormatProvider provider) {
			if (chars == null)
				throw new ArgumentNullException("chars");

			return Parse(chars, 0, chars.Length, context, provider);
		}

		public static bool TryParse(string s, out BigDecimal value) {
			return TryParse(s, (MathContext)null, out value);
		}

		public static bool TryParse(string s, MathContext context, out BigDecimal value) {
			return TryParse(s, context, NumberFormatInfo.InvariantInfo, out value);
		}

		public static bool TryParse(string s, IFormatProvider provider, out BigDecimal value) {
			return TryParse(s, null, provider, out value);
		}

		public static bool TryParse(string s, MathContext context, IFormatProvider provider, out BigDecimal value) {
			if (String.IsNullOrEmpty(s)) {
				value = null;
				return false;
			}

			var data = s.ToCharArray();

			Exception error;
			if (!TryParse(data, 0, data.Length, provider, out value, out error))
				return false;

			if (context != null)
				value.InplaceRound(context);

			return true;
		}

		public static BigDecimal Parse(string s, IFormatProvider provider) {
			return Parse(s, null, provider);
		}

		public static BigDecimal Parse(string s) {
			return Parse(s, (MathContext)null);
		}

		public static BigDecimal Parse(string s, MathContext context) {
			return Parse(s, context, NumberFormatInfo.InvariantInfo);
		}

		public static BigDecimal Parse(string s, MathContext context, IFormatProvider provider) {
			if (String.IsNullOrEmpty(s))
				throw new FormatException();

			var data = s.ToCharArray();

			Exception error;
			BigDecimal value;
			if (!TryParse(data, 0, data.Length, provider, out value, out error))
				throw error;

			if (context != null)
				value.InplaceRound(context);

			return value;
		}
	}
}
