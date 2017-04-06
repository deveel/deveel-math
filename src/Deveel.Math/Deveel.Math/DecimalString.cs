using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Deveel.Math {
	static class DecimalString {
		/// <summary>
		/// An array filled with character <c>'0'</c>.
		/// </summary>
		private static readonly char[] ChZeros = new char[100];

		static DecimalString() {
			// To fill all static arrays.
			int i = 0;

			for (; i < BigDecimal.ZeroScaledBy.Length; i++) {
				ChZeros[i] = '0';
			}

			for (; i < ChZeros.Length; i++) {
				ChZeros[i] = '0';
			}
		}

		public static bool TryParse(char[] inData, int offset, int len, IFormatProvider provider, out BigDecimal value,
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

					v.Scale = offset - begin;
					bufLength += v.Scale;
					unscaledBuffer.Append(inData, begin, v.Scale);
				} else {
					v.Scale = 0;
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
					string scaleString = new String(inData, begin, last + 1 - begin); // buffer for scale
					// Checking if the scale is defined            
					long newScale = (long)v.Scale - Int32.Parse(scaleString, provider); // the new scale
					v.Scale = (int)newScale;
					if (newScale != v.Scale) {
						// math.02=Scale out of range.
						throw new FormatException(Messages.math02); //$NON-NLS-1$
					}
				}

				// Parsing the unscaled value
				if (bufLength < 19) {
					long smallValue;
					if (!Int64.TryParse(unscaledBuffer.ToString(), NumberStyles.Integer, provider, out smallValue)) {
						value = null;
						exception = new FormatException();
						return false;
					}

					v.SmallValue = smallValue;
					v.BitLength = BigDecimal.CalcBitLength(v.SmallValue);
				} else {
					v.SetUnscaledValue(BigInteger.Parse(unscaledBuffer.ToString()));
				}

				v.Precision = unscaledBuffer.Length - counter;
				if (unscaledBuffer[0] == '-') {
					v.Precision--;
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

		public static string ToString(BigDecimal value, IFormatProvider provider) {
			var numberInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			if (numberInfo == null)
				numberInfo = NumberFormatInfo.InvariantInfo;

			var decimalSep = numberInfo.NumberDecimalSeparator;
			if (decimalSep.Length > 1)
				throw new NotSupportedException("Decimal separators with more than one character are not supported (yet).");

			if (value.BitLength < 32) {
				value.toStringImage = Conversion.ToDecimalScaledString(value.SmallValue, value.Scale);
				return value.toStringImage;
			}
			String intString = value.UnscaledValue.ToString();
			if (value.Scale == 0) {
				return intString;
			}
			int begin = (value.UnscaledValue.Sign < 0) ? 2 : 1;
			int end = intString.Length;
			long exponent = -(long)value.Scale + end - begin;
			StringBuilder result = new StringBuilder();

			result.Append(intString);
			if ((value.Scale > 0) && (exponent >= -6)) {
				if (exponent >= 0) {
					result.Insert(end - value.Scale, decimalSep);
				} else {
					result.Insert(begin - 1, "0" + decimalSep); //$NON-NLS-1$
					result.Insert(begin + 1, ChZeros, 0, -(int)exponent - 1);
				}
			} else {
				if (end - begin >= 1) {
					result.Insert(begin, decimalSep);
					end++;
				}
				result.Insert(end, new[] { 'E' });
				if (exponent > 0) {
					result.Insert(++end, new[] { '+' });
				}
				result.Insert(++end, Convert.ToString(exponent));
			}
			value.toStringImage = result.ToString();
			return value.toStringImage;
		}

		public static string ToEngineeringString(BigDecimal value, IFormatProvider provider) {
			var numberInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			if (numberInfo == null)
				numberInfo = NumberFormatInfo.InvariantInfo;

			var decimalSep = numberInfo.NumberDecimalSeparator;
			if (decimalSep.Length > 1)
				throw new NotSupportedException("Decimal separators with more than one character are not supported (yet).");

			String intString = value.UnscaledValue.ToString();
			if (value.Scale == 0) {
				return intString;
			}
			int begin = (value.UnscaledValue.Sign < 0) ? 2 : 1;
			int end = intString.Length;
			long exponent = -(long)value.Scale + end - begin;
			StringBuilder result = new StringBuilder(intString);

			if ((value.Scale > 0) && (exponent >= -6)) {
				if (exponent >= 0) {
					result.Insert(end - value.Scale, decimalSep);
				} else {
					result.Insert(begin - 1, "0" + decimalSep); //$NON-NLS-1$
					result.Insert(begin + 1, ChZeros, 0, -(int)exponent - 1);
				}
			} else {
				int delta = end - begin;
				int rem = (int)(exponent % 3);

				if (rem != 0) {
					// adjust exponent so it is a multiple of three
					if (value.UnscaledValue.Sign == 0) {
						// zero value
						rem = (rem < 0) ? -rem : 3 - rem;
						exponent += rem;
					} else {
						// nonzero value
						rem = (rem < 0) ? rem + 3 : rem;
						exponent -= rem;
						begin += rem;
					}
					if (delta < 3) {
						for (int i = rem - delta; i > 0; i--) {
							result.Insert(end++, new[] { '0' });
						}
					}
				}
				if (end - begin >= 1) {
					result.Insert(begin, decimalSep);
					end++;
				}
				if (exponent != 0) {
					result.Insert(end, new[] { 'E' });
					if (exponent > 0) {
						result.Insert(++end, new[] { '+' });
					}
					result.Insert(++end, Convert.ToString(exponent));
				}
			}
			return result.ToString();
		}


		public static String ToPlainString(BigDecimal value, IFormatProvider provider) {
			String intStr = value.UnscaledValue.ToString();
			if ((value.Scale == 0) || ((value.IsZero) && (value.Scale < 0))) {
				return intStr;
			}
			int begin = (value.Sign < 0) ? 1 : 0;
			int delta = value.Scale;
			// We take space for all digits, plus a possible decimal point, plus 'scale'
			StringBuilder result = new StringBuilder(intStr.Length + 1 + System.Math.Abs(value.Scale));

			if (begin == 1) {
				// If the number is negative, we insert a '-' CharHelper at front 
				result.Append('-');
			}
			if (value.Scale > 0) {
				delta -= (intStr.Length - begin);
				if (delta >= 0) {
					result.Append("0."); //$NON-NLS-1$
					// To append zeros after the decimal point
					for (; delta > ChZeros.Length; delta -= ChZeros.Length) {
						result.Append(ChZeros);
					}
					result.Append(ChZeros, 0, delta);
					result.Append(intStr.Substring(begin));
				} else {
					delta = begin - delta;
					result.Append(intStr.Substring(begin, delta - begin));
					result.Append('.');
					result.Append(intStr.Substring(delta));
				}
			} else {
				// (scale <= 0)
				result.Append(intStr.Substring(begin));
				// To append trailing zeros
				for (; delta < -ChZeros.Length; delta += ChZeros.Length) {
					result.Append(ChZeros);
				}
				result.Append(ChZeros, 0, -delta);
			}
			return result.ToString();
		}

	}
}
