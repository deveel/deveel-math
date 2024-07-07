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
#if !PORTABLE
using System.Runtime.Serialization;
#endif
using System.Text;

namespace Deveel.Math {
	/// <summary>
	/// Immutable objects describing settings such as rounding 
	/// mode and digit precision for the numerical operations 
	/// provided by class <see cref="BigDecimal"/>.
	/// </summary>
#if !PORTABLE
	[Serializable]
#endif
	public sealed class MathContext : IEquatable<MathContext> {

		/// <summary>
		/// A <see cref="MathContext"/> which corresponds to the IEEE 754r quadruple decimal precision 
		/// format: 34 digit precision and <see cref="Deveel.Math.RoundingMode.HalfEven"/> rounding.
		/// </summary>
		public static readonly MathContext Decimal128 = new MathContext(34, RoundingMode.HalfEven);

		/// <summary>
		/// A <see cref="MathContext"/> which corresponds to the IEEE 754r single decimal precision 
		/// format: 7 digit precision and <see cref="Deveel.Math.RoundingMode.HalfEven"/> rounding.
		/// </summary>
		public static readonly MathContext Decimal32 = new MathContext(7, RoundingMode.HalfEven);

		/// <summary>
		/// A <see cref="MathContext"/> which corresponds to the IEEE 754r double decimal precision 
		/// format: 16 digit precision and <see cref="Deveel.Math.RoundingMode.HalfEven"/> rounding.
		/// </summary>
		public static readonly MathContext Decimal64 = new MathContext(16, RoundingMode.HalfEven);

		/// <summary>
		/// A <see cref="MathContext"/> for unlimited precision with <see cref="Deveel.Math.RoundingMode.HalfUp"/> rounding.
		/// </summary>
		public static readonly MathContext Unlimited = new MathContext(0, RoundingMode.HalfUp);

		/// <summary>
		/// The number of digits to be used for an operation; results are rounded to this precision.
		/// </summary>
		private readonly int precision;

		/// <summary>
		/// A <see cref="RoundingMode"/> object which specifies the algorithm to be used for rounding.
		/// </summary>
		private readonly RoundingMode roundingMode;

		/// <summary>
		/// An array of <see cref="char"/> containing: <c>'p','r','e','c','i','s','i','o','n','='</c>. 
		/// It's used to improve the methods related to <see cref="string"/> conversion.
		/// </summary>
		/// <seealso cref="MathContext(string)"/>
		/// <seealso cref="ToString"/>
		private static readonly char[] chPrecision = {'p', 'r', 'e', 'c', 'i', 's', 'i', 'o', 'n', '='};

		/// <summary>
		/// An array of <see cref="char"/> containing: <c>'r','o','u','n','d','i','n','g','M','o','d','e','='</c>. 
		/// It's used to improve the methods related to <see cref="string"/> conversion.
		/// </summary>
		/// <seealso cref="MathContext(string)"/>
		/// <seealso cref="ToString"/>
		private static readonly char[] chRoundingMode = {'r', 'o', 'u', 'n', 'd', 'i', 'n', 'g', 'M', 'o', 'd', 'e', '='};

		/// <summary>
		/// Constructs a new <see cref="MathContext"/> with the specified precision and with the 
		/// rounding mode <see cref="Deveel.Math.RoundingMode.HalfUp"/>.
		/// </summary>
		/// <param name="precision">The precision for the new context.</param>
		/// <remarks>
		/// If the precision passed is zero, then this implies that the computations have to 
		/// be performed exact, the rounding mode in this case is irrelevant.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// If <paramref name="precision"/> is smaller than zero.
		/// </exception>
		public MathContext(int precision)
			: this(precision, RoundingMode.HalfUp) {
		}

		/// <summary>
		///  Constructs a new <see cref="MathContext"/> with the specified precision and with the 
		/// specified rounding mode.
		/// </summary>
		/// <param name="precision">The precision for the new context.</param>
		/// <param name="roundingMode">The rounding mode for the new context.</param>
		/// <remarks>
		/// If the precision passed is zero, then this implies that the computations have to 
		/// be performed exact, the rounding mode in this case is irrelevant.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// If <paramref name="precision"/> is smaller than zero.
		/// </exception>
		public MathContext(int precision, RoundingMode roundingMode) {
			if (precision < 0) {
				// math.0C=Digits < 0
				throw new ArgumentException(Messages.math0C); //$NON-NLS-1$
			}
			this.precision = precision;
			this.roundingMode = roundingMode;
		}

		///// <summary>
		///// Constructs a new <see cref="MathContext"/> from a string.
		///// </summary>
		///// <param name="val">
		///// A string describing the precision and rounding mode for the new context.
		///// </param>
		///// <remarks>
		///// The string has to specify the precision and the rounding mode to be used and has to 
		///// follow the following syntax: "precision=&lt;precision&gt; roundingMode=&lt;roundingMode&gt;".<br/>
		///// This is the same form as the one returned by the <see cref="ToString"/> method.
		///// </remarks>
		///// <exception cref="FormatException">
		///// Thrown if the given string is in an incorrect format.
		///// </exception>
		///// <exception cref="ArgumentException">
		///// If the precision value parsed from the string is less than zero.
		///// </exception>
		//public MathContext(String val) {
		//	char[] charVal = val.ToCharArray();
		//	int i; // Index of charVal
		//	int j; // Index of chRoundingMode
		//	int digit; // It will contain the digit parsed

		//	if ((charVal.Length < 27) || (charVal.Length > 45)) {
		//		// math.0E=bad string format
		//		throw new FormatException(Messages.math0E); //$NON-NLS-1$
		//	}
		//	// Parsing "precision=" String
		//	for (i = 0; (i < chPrecision.Length) && (charVal[i] == chPrecision[i]); i++) {
		//		;
		//	}

		//	if (i < chPrecision.Length) {
		//		// math.0E=bad string format
		//		throw new FormatException(Messages.math0E); //$NON-NLS-1$
		//	}
		//	// Parsing the value for "precision="...
		//	digit = CharHelper.toDigit(charVal[i], 10);
		//	if (digit == -1) {
		//		// math.0E=bad string format
		//		throw new FormatException(Messages.math0E); //$NON-NLS-1$
		//	}
			
		//	precision = precision * 10 + digit;
		//	i++;

		//	do {
		//		digit = CharHelper.toDigit(charVal[i], 10);
		//		if (digit == -1) {
		//			if (charVal[i] == ' ') {
		//				// It parsed all the digits
		//				i++;
		//				break;
		//			}
		//			// It isn't  a valid digit, and isn't a white space
		//			// math.0E=bad string format
		//			throw new ArgumentException(Messages.math0E); //$NON-NLS-1$
		//		}
		//		// Accumulating the value parsed
		//		precision = precision * 10 + digit;
		//		if (precision < 0) {
		//			// math.0E=bad string format
		//			throw new ArgumentException(Messages.math0E); //$NON-NLS-1$
		//		}
		//		i++;
		//	} while (true);
		//	// Parsing "roundingMode="
		//	for (j = 0; (j < chRoundingMode.Length)
		//			&& (charVal[i] == chRoundingMode[j]); i++, j++) {
		//		;
		//	}

		//	if (j < chRoundingMode.Length) {
		//		// math.0E=bad string format
		//		throw new FormatException(Messages.math0E); //$NON-NLS-1$
		//	}
		//	// Parsing the value for "roundingMode"...
		//	roundingMode = (RoundingMode)Enum.Parse(typeof(RoundingMode), new string(charVal, i, charVal.Length - i), true);
		//}

		/// <summary>
		/// Get the precision of the context.
		/// </summary>
		/// <remarks>
		/// The precision is the number of digits used for an operation. Results are rounded to 
		/// this precision. The precision is guaranteed to be non negative. If the precision is zero, 
		/// then the computations have to be performed exact, results are not rounded in this case.
		/// </remarks>
		public int Precision {
			get { return precision; }
		}

		/// <summary>
		/// Gets the rounding mode of the context, that is the strategy used to round results.
		/// </summary>
		/// <seealso cref="Deveel.Math.RoundingMode"/>
		public RoundingMode RoundingMode {
			get { return roundingMode; }
		}

		public bool Equals(MathContext other) {
			return (other.precision == precision) &&
			       (other.roundingMode == roundingMode);
		}

		public override bool Equals(object obj) {
			if (!(obj is MathContext))
				return false;

			return Equals((MathContext) obj);
		}

		public override int GetHashCode() {
			// Make place for the necessary bits to represent 8 rounding modes
			return ((precision << 3) | (int)roundingMode);
		}

		/// <summary>
		/// Returns the string representation for this <see cref="MathContext"/> instance.
		/// </summary>
		/// <remarks>
		/// The string has the form <c>"precision=&lt;precision&gt; roundingMode=&lt;roundingMode&gt;"</c> where 
		/// <c>&lt;precision&gt;</c> is an integer describing the number of digits used for operations 
		/// and <c>&lt;roundingMode&gt;</c> is the string representation of the rounding mode.
		/// </remarks>
		/// <returns>
		/// Returns the string that describes the current context.
		/// </returns>
		public override string ToString() {
			var sb = new StringBuilder(45);

			sb.Append(chPrecision);
			sb.Append(precision);
			sb.Append(' ');
			sb.Append(chRoundingMode);
			sb.Append(roundingMode);
			return sb.ToString();
		}

#if !PORTABLE
		[OnDeserialized]
		private void OnDeserialized(StreamingContext context) {
			if (precision < 0) {
				// math.0F=bad precision value
				throw new SerializationException(Messages.math0F); //$NON-NLS-1$
			}
		}
#endif

		private static bool TryParse(string s, out MathContext context, out Exception exception) {
			char[] charVal = s.ToCharArray();
			int i; // Index of charVal
			int j; // Index of chRoundingMode
			int digit; // It will contain the digit parsed

			if ((charVal.Length < 27) || (charVal.Length > 45)) {
				// math.0E=bad string format
				exception = new FormatException(Messages.math0E); //$NON-NLS-1$
				context = null;
				return false;
			}

			// Parsing "precision=" String
			for (i = 0; (i < chPrecision.Length) && (charVal[i] == chPrecision[i]); i++) { }

			if (i < chPrecision.Length) {
				// math.0E=bad string format
				throw new FormatException(Messages.math0E); //$NON-NLS-1$
			}
			// Parsing the value for "precision="...
			digit = CharHelper.toDigit(charVal[i], 10);
			if (digit == -1) {
				// math.0E=bad string format
				exception = new FormatException(Messages.math0E); //$NON-NLS-1$
				context = null;
				return false;
			}

			var precision = digit;
			i++;

			do {
				digit = CharHelper.toDigit(charVal[i], 10);
				if (digit == -1) {
					if (charVal[i] == ' ') {
						// It parsed all the digits
						i++;
						break;
					}

					// It isn't  a valid digit, and isn't a white space
					// math.0E=bad string format
					exception = new ArgumentException(Messages.math0E); //$NON-NLS-1$
					context = null;
					return false;
				}
				// Accumulating the value parsed
				precision = precision * 10 + digit;
				if (precision < 0) {
					// math.0E=bad string format
					exception = new ArgumentException(Messages.math0E); //$NON-NLS-1$
					context = null;
					return false;
				}

				i++;
			} while (true);
			// Parsing "roundingMode="
			for (j = 0; (j < chRoundingMode.Length)
					&& (charVal[i] == chRoundingMode[j]); i++, j++) {
				;
			}

			if (j < chRoundingMode.Length) {
				// math.0E=bad string format
				throw new FormatException(Messages.math0E); //$NON-NLS-1$
			}

			RoundingMode roundingMode;

			try {
				// Parsing the value for "roundingMode"...
				roundingMode = (RoundingMode)Enum.Parse(typeof(RoundingMode), new string(charVal, i, charVal.Length - i), true);
			} catch (Exception ex) {
				exception = ex;
				context = null;
				return false;
			}

			exception = null;
			context = new MathContext(precision, roundingMode);
			return true;
		}

		public static bool TryParse(string s, out MathContext context) {
			Exception error;
			return TryParse(s, out context, out error);
		}

		public static MathContext Parse(string s) {
			Exception error;
			MathContext context;
			if (!TryParse(s, out context, out error))
				throw error;

			return context;
		}
	}
}