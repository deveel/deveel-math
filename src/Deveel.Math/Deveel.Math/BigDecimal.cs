// 
//  Copyright 2009-2014  Deveel
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
#if !PORTABLE
using System.Runtime.Serialization;
#endif

namespace Deveel.Math {
	/// <summary>
	/// This class represents immutable arbitrary precision decimal numbers.
	/// </summary>
	/// <remarks>
	/// Each <see cref="BigDecimal"/> instance is represented with a unscaled 
	/// arbitrary precision mantissa (the unscaled value) and a scale. The value 
	/// of the <see cref="BigDecimal"/> is <see cref="UnscaledValue"/> 10^(-<see cref="Scale"/>).
	/// </remarks>
#if !PORTABLE
	[Serializable]
#endif
	[System.Diagnostics.DebuggerDisplay("{ToString()}")]
	public sealed partial class BigDecimal : IComparable<BigDecimal>, IEquatable<BigDecimal>
#if !PORTABLE
		, ISerializable
#endif
	{

		/// <summary>
		/// The constant zero as a <see cref="BigDecimal"/>.
		/// </summary>
		public static readonly BigDecimal Zero = new BigDecimal(0, 0);

		/// <summary>
		/// The constant one as a <see cref="BigDecimal"/>.
		/// </summary>
		public static readonly BigDecimal One = new BigDecimal(1, 0);

		/// <summary>
		/// The constant ten as a <see cref="BigDecimal"/>.
		/// </summary>
		public static readonly BigDecimal Ten = new BigDecimal(10, 0);

		/// <summary>
		/// The double closer to <c>Log10(2)</c>.
		/// </summary>
		private const double Log10Of2 = 0.3010299956639812;

		/// <summary>
		/// The <see cref="string"/> representation is cached.
		/// </summary>
#if !PORTABLE
		[NonSerialized]
#endif
		private string toStringImage;

		/// <summary>
		/// An array with powers of five that fit in the type <see cref="long"/>
		/// (<c>5^0,5^1,...,5^27</c>).
		/// </summary>
		private static readonly BigInteger[] FivePow;

		/// <summary>
		/// An array with powers of ten that fit in the type <see cref="long"/> 
		/// (<c>10^0,10^1,...,10^18</c>).
		/// </summary>
		private static readonly BigInteger[] TenPow;

		/// <summary>
		/// An array with powers of ten that fit in the type <see cref="long"/> 
		/// (<c>10^0,10^1,...,10^18</c>).
		/// </summary>
		private static readonly long[] LongTenPow = new long[] {
			1L,
			10L,
			100L,
			1000L,
			10000L,
			100000L,
			1000000L,
			10000000L,
			100000000L,
			1000000000L,
			10000000000L,
			100000000000L,
			1000000000000L,
			10000000000000L,
			100000000000000L,
			1000000000000000L,
			10000000000000000L,
			100000000000000000L,
			1000000000000000000L,
		};


		private static readonly long[] LongFivePow = new long[] {
			1L,
			5L,
			25L,
			125L,
			625L,
			3125L,
			15625L,
			78125L,
			390625L,
			1953125L,
			9765625L,
			48828125L,
			244140625L,
			1220703125L,
			6103515625L,
			30517578125L,
			152587890625L,
			762939453125L,
			3814697265625L,
			19073486328125L,
			95367431640625L,
			476837158203125L,
			2384185791015625L,
			11920928955078125L,
			59604644775390625L,
			298023223876953125L,
			1490116119384765625L,
			7450580596923828125L,
		};

		private static readonly int[] LongFivePowBitLength = new int[LongFivePow.Length];
		private static readonly int[] LongTenPowBitLength = new int[LongTenPow.Length];

		private const int BiScaledByZeroLength = 11;

		/// <summary>
		/// An array with the first <see cref="BigInteger"/> scaled by zero.
		/// (<c>[0,0],[1,0],...,[10,0]</c>).
		/// </summary>
		private static readonly BigDecimal[] BiScaledByZero = new BigDecimal[BiScaledByZeroLength];

		/// <summary>
		/// An array with the zero number scaled by the first positive scales.
		/// (<c>0*10^0, 0*10^1, ..., 0*10^10</c>).
		/// </summary>
		private static readonly BigDecimal[] ZeroScaledBy = new BigDecimal[11];

		/// <summary>
		/// An array filled with character <c>'0'</c>.
		/// </summary>
		private static readonly char[] ChZeros = new char[100];

		static BigDecimal() {
			// To fill all static arrays.
			int i = 0;

			for (; i < ZeroScaledBy.Length; i++) {
				BiScaledByZero[i] = new BigDecimal(i, 0);
				ZeroScaledBy[i] = new BigDecimal(0, i);
				ChZeros[i] = '0';
			}

			for (; i < ChZeros.Length; i++) {
				ChZeros[i] = '0';
			}
			for (int j = 0; j < LongFivePowBitLength.Length; j++) {
				LongFivePowBitLength[j] = BitLength(LongFivePow[j]);
			}
			for (int j = 0; j < LongTenPowBitLength.Length; j++) {
				LongTenPowBitLength[j] = BitLength(LongTenPow[j]);
			}

			// Taking the references of useful powers.
			TenPow = Multiplication.BigTenPows;
			FivePow = Multiplication.BigFivePows;
		}

		/// <summary>
		/// The arbitrary precision integer (unscaled value) in the internal
		/// representation of <see cref="BigDecimal"/>.
		/// </summary>
		private BigInteger intVal;

#if !PORTABLE
		[NonSerialized]
#endif
		private int _bitLength;

#if !PORTABLE
		[NonSerialized]
#endif
		private long smallValue;

		/// <summary>
		/// The 32-bit integer scale in the internal representation 
		/// of <see cref="BigDecimal"/>.
		/// </summary>
		private int _scale;

		/// <summary>
		/// Represent the number of decimal digits in the unscaled value.
		/// </summary>
		/// <remarks>
		/// This precision is calculated the first time, and used in the following 
		/// calls of method <see cref="Precision"/>. Note that some call to the private 
		/// method <see cref="InplaceRound"/> could update this field.
		/// </remarks>
		/// <seealso cref="Precision"/>
		/// <seealso cref="InplaceRound"/>
#if !PORTABLE
		[NonSerialized]
#endif
		private int _precision = 0;

		#region .ctor

		private BigDecimal(long smallValue, int scale) {
			this.smallValue = smallValue;
			_scale = scale;
			_bitLength = BitLength(smallValue);
		}

		private BigDecimal(int smallValue, int scale) {
			this.smallValue = smallValue;
			_scale = scale;
			_bitLength = BitLength(smallValue);
		}

		private BigDecimal() {

		}


		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the 64bit 
		/// double <paramref name="val"/>. The constructed big decimal is 
		/// equivalent to the given double.
		/// </summary>
		/// <param name="val">The double value to be converted to a 
		/// <see cref="BigDecimal"/> instance.</param>
		/// <remarks>
		/// For example, <c>new BigDecimal(0.1)</c> is equal to <c>0.1000000000000000055511151231257827021181583404541015625</c>. 
		/// This happens as <c>0.1</c> cannot be represented exactly in binary.
		/// <para>
		/// To generate a big decimal instance which is equivalent to <c>0.1</c> use the
		/// <see cref="BigDecimal.Parse(string)"/> method.
		/// </para>
		/// </remarks>
		/// <exception cref="FormatException">
		/// If <paramref name="val"/> is infinity or not a number.
		/// </exception>
		public BigDecimal(double val) {
			if (Double.IsInfinity(val) || Double.IsNaN(val)) {
				// math.03=Infinity or NaN
				throw new FormatException(Messages.math03); //$NON-NLS-1$
			}

			long bits = BitConverter.DoubleToInt64Bits(val); // IEEE-754

			// Extracting the exponent, note that the bias is 1023
			_scale = 1075 - (int) ((bits >> 52) & 0x7FFL);
			// Extracting the 52 bits of the mantisa.
			long mantisa = (_scale == 1075) ? (bits & 0xFFFFFFFFFFFFFL) << 1 : (bits & 0xFFFFFFFFFFFFFL) | 0x10000000000000L;
			if (mantisa == 0) {
				_scale = 0;
				_precision = 1;
			}
			// To simplify all factors '2' in the mantisa 
			if (_scale > 0) {
				int trailingZeros = System.Math.Min(_scale, Utils.NumberOfTrailingZeros(mantisa));
				long mantisa2 = (long) (((ulong) mantisa) >> trailingZeros);
				mantisa = Utils.URShift(mantisa, trailingZeros);
				_scale -= trailingZeros;
			}
			// Calculating the new unscaled value and the new scale
			if ((bits >> 63) != 0) {
				mantisa = -mantisa;
			}
			int mantisaBits = BitLength(mantisa);
			if (_scale < 0) {
				_bitLength = mantisaBits == 0 ? 0 : mantisaBits - _scale;
				if (_bitLength < 64) {
					smallValue = mantisa << (-_scale);
				} else {
					intVal = BigInteger.FromInt64(mantisa).ShiftLeft(-_scale);
				}
				_scale = 0;
			} else if (_scale > 0) {
				// m * 2^e =  (m * 5^(-e)) * 10^e
				if (_scale < LongFivePow.Length && mantisaBits + LongFivePowBitLength[_scale] < 64) {
					smallValue = mantisa*LongFivePow[_scale];
					_bitLength = BitLength(smallValue);
				} else {
					SetUnscaledValue(Multiplication.MultiplyByFivePow(BigInteger.FromInt64(mantisa), _scale));
				}
			} else {
				// scale == 0
				smallValue = mantisa;
				_bitLength = mantisaBits;
			}
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the 64bit 
		/// double <paramref name="val"/>. The constructed big decimal is 
		/// equivalent to the given double.
		/// </summary>
		/// <param name="val">The double value to be converted to a 
		/// <see cref="BigDecimal"/> instance.</param>
		/// <param name="mc">The rounding mode and precision for the result of 
		/// this operation.</param>
		/// <remarks>
		/// For example, <c>new BigDecimal(0.1)</c> is equal to <c>0.1000000000000000055511151231257827021181583404541015625</c>. 
		/// This happens as <c>0.1</c> cannot be represented exactly in binary.
		/// <para>
		/// To generate a big decimal instance which is equivalent to <c>0.1</c> use the
		/// <see cref="BigDecimal(string)"/> constructor.
		/// </para>
		/// </remarks>
		/// <exception cref="FormatException">
		/// If <paramref name="val"/> is infinity or not a number.
		/// </exception>
		/// <exception cref="ArithmeticException">
		/// if <see cref="MathContext.Precision"/> of <paramref name="mc"/> is greater than 0
		/// and <see cref="MathContext.RoundingMode"/> is equal to <see cref="RoundingMode.Unnecessary"/>
		/// and the new big decimal cannot be represented within the given precision without rounding.
		/// </exception>
		public BigDecimal(double val, MathContext mc)
			: this(val) {
			InplaceRound(mc);
		}


		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the
		/// <paramref name="val">given big integer</paramref>.
		/// </summary>
		/// <param name="val">The value to be converted to a <see cref="BigDecimal"/> instance.</param>
		/// <remarks>
		/// The <see cref="Scale"/> of the result is <c>0</c>.
		/// </remarks>
		public BigDecimal(BigInteger val)
			: this(val, 0) {
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the
		/// <paramref name="val">given big integer</paramref>.
		/// </summary>
		/// <param name="val">The value to be converted to a <see cref="BigDecimal"/> instance.</param>
		/// <param name="mc">The rounding mode and precision for the result of this operation.</param>
		/// <remarks>
		/// The <see cref="Scale"/> of the result is <c>0</c>.
		/// </remarks>
		/// <exception cref="ArithmeticException">
		/// If <see cref="MathContext.Precision"/> is greater than 0 and <see cref="MathContext.RoundingMode"/> is
		/// equal to <see cref="RoundingMode.Unnecessary"/> and the new big decimal cannot be represented  within the 
		/// given precision without rounding.
		/// </exception>
		public BigDecimal(BigInteger val, MathContext mc)
			: this(val) {
			InplaceRound(mc);
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from a <paramref name="unscaledValue">given unscaled value</paramref> 
		/// and a given scale.
		/// </summary>
		/// <param name="unscaledValue">Represents the unscaled value of the decimal.</param>
		/// <param name="scale">The scale of this <see cref="BigDecimal"/></param>
		/// <remarks>
		/// The value of this instance is <c><paramref name="unscaledValue"/> 10^(-<paramref name="scale"/>)</c>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// If <paramref name="unscaledValue"/> is <b>null</b>.
		/// </exception>
		public BigDecimal(BigInteger unscaledValue, int scale) {
			if (unscaledValue == null) {
				throw new NullReferenceException();
			}
			_scale = scale;
			SetUnscaledValue(unscaledValue);
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from a <paramref name="unscaledValue">given unscaled 
		/// value</paramref> and a given scale.
		/// </summary>
		/// <param name="unscaledValue">Represents the unscaled value of this big decimal.</param>
		/// <param name="scale">The scale factor of the decimal.</param>
		/// <param name="mc">The context used to round the result of the operations.</param>
		/// <remarks>
		/// The value of this instance is <c><paramref name="unscaledValue"/> 10^(-<paramref name="scale"/>)</c>. 
		/// The result is rounded according to the specified math context.
		/// </remarks>
		/// <exception cref="ArithmeticException">
		/// If <see cref="MathContext.Precision"/> is greater than zero, the
		/// <see cref="MathContext.RoundingMode"/> is set to <see cref="RoundingMode.Unnecessary"/>
		/// and the decimal cannot be represented within the given precision without rounding.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// If the given <paramref name="unscaledValue"/> is null.
		/// </exception>
		public BigDecimal(BigInteger unscaledValue, int scale, MathContext mc)
			: this(unscaledValue, scale) {
			InplaceRound(mc);
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the given int 
		/// <paramref name="val"/>.
		/// </summary>
		/// <param name="val">The integer value to convert to a decimal.</param>
		/// <remarks>
		/// The scale factor of the result is zero.
		/// </remarks>
		public BigDecimal(int val)
			: this(val, 0) {
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the given <paramref name="val">integer value</paramref>. 
		/// </summary>
		/// <param name="val">Integer value to be converted to a <see cref="BigDecimal"/> instance.</param>
		/// <param name="mc">The rounding mode and precision for the result of this operation.</param>
		/// <remarks>
		/// The scale of the result is {@code 0}. The result is rounded according to the specified math context.
		/// </remarks>
		/// <exception cref="ArithmeticException">
		/// Thrown if precision is greater than 0 and <see cref="RoundingMode"/> is
		/// <see cref="RoundingMode.Unnecessary"/> and the new big decimal cannot be represented
		/// within the given precision without rounding. 
		/// </exception>
		public BigDecimal(int val, MathContext mc)
			: this(val, 0) {
			InplaceRound(mc);
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the given long <paramref name="val"/>,
		/// with a scale of <c>0</c>.
		/// </summary>
		/// <param name="val">The long value to be converted to a <see cref="BigDecimal"/></param>
		public BigDecimal(long val)
			: this(val, 0) {
		}


		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the given long <paramref name="val"/>,
		/// with a scale of <c>0</c> and the value rounded according to the specified context.
		/// </summary>
		/// <param name="val">The long value to be converted to a <see cref="BigDecimal"/></param>
		/// <param name="mc">The context that defines the rounding mode and precision to apply to the
		/// value obtained from the given integer.</param>
		/// <exception cref="ArithmeticException">
		/// If the <see cref="MathContext.Precision"/> value specified is greater than <c>0</c> and
		/// the <see cref="MathContext.RoundingMode"/> is <see cref="RoundingMode.Unnecessary"/> and
		/// the new <see cref="BigDecimal"/> cannot be represented within the given precision
		/// without rounding.
		/// </exception>
		public BigDecimal(long val, MathContext mc)
			: this(val) {
			InplaceRound(mc);
		}

		#endregion

		/// <summary>
		/// Gets the sign of the decimal, where <c>-1</c> if the value is less than 0,
		/// <c>0</c> if the value is 0 and <c>1</c> if the value is greater than 0.
		/// </summary>
		/// <seealso cref="Sign"/>
		public int Sign {
			get {
				if (_bitLength < 64)
					return System.Math.Sign(smallValue);
				return GetUnscaledValue().Sign;
			}
		}

		/// <summary>
		/// Gets a value indicating if the decimal is equivalent to zero.
		/// </summary>
		public bool IsZero {
			get {
				//Watch out: -1 has a bitLength=0
				return _bitLength == 0 && smallValue != -1;
			}
		}

		/// <summary>
		/// Gets the scale value of this <see cref="BigDecimal"/> instance
		/// </summary>
		/// <remarks>
		/// The scale is the number of digits behind the decimal point. The value of 
		/// this <see cref="BigDecimal"/> is the <c>unsignedValue * 10^(-scale)</c>. 
		/// If the scale is negative, then this <see cref="BigDecimal"/> represents a big integer.
		/// </remarks>
		public int Scale {
			get { return _scale; }
		}

		/// <summary>
		/// Gets the precision value of this <see cref="BigDecimal"/> instance.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The precision is the number of decimal digits used to represent this decimal.
		/// It is equivalent to the number of digits of the unscaled value.
		/// </para>
		/// <para>
		///  The precision of <c>0</c> is <c>1</c> (independent of the scale).
		/// </para>
		/// </remarks>
		public int Precision {
			get {
				// Checking if the precision already was calculated
				if (_precision > 0) {
					return _precision;
				}
				int bitLength = this._bitLength;
				int decimalDigits = 1; // the precision to be calculated
				double doubleUnsc = 1; // intVal in 'double'

				if (bitLength < 1024) {
					// To calculate the precision for small numbers
					if (bitLength >= 64) {
						doubleUnsc = GetUnscaledValue().ToDouble();
					} else if (bitLength >= 1) {
						doubleUnsc = smallValue;
					}
					decimalDigits += (int) System.Math.Log10(System.Math.Abs(doubleUnsc));
				} else {
					// (bitLength >= 1024)
					/* To calculate the precision for large numbers
				 * Note that: 2 ^(bitlength() - 1) <= intVal < 10 ^(precision()) */
					decimalDigits += (int) ((bitLength - 1)*Log10Of2);
					// If after division the number isn't zero, exists an aditional digit
					if (GetUnscaledValue().Divide(Multiplication.PowerOf10(decimalDigits)).Sign != 0) {
						decimalDigits++;
					}
				}
				_precision = decimalDigits;
				return _precision;
			}
		}

		/// <summary>
		/// Gets the unscaled value (mantissa) of this <see cref="BigDecimal"/> instance as 
		/// a <see cref="BigInteger"/>.
		/// </summary>
		/// <remarks>
		/// The unscaled value can be computed as <c>(this * 10^(scale))</c>.
		/// </remarks>
		public BigInteger UnscaledValue {
			get { return GetUnscaledValue(); }
		}

		#region Public Methods

		/// <summary>
		/// Returns a new <see cref="BigDecimal"/> instance whose value is equal to 
		/// <paramref name="unscaledVal"/> 10^(-<paramref name="scale"/>). The scale 
		/// of the result is <see cref="scale"/>, and its unscaled value is <see cref="unscaledVal"/>.
		/// </summary>
		/// <param name="unscaledVal">The unscaled value to be used to construct 
		/// the new <see cref="BigDecimal"/>.</param>
		/// <param name="scale">The scale to be used to construct the new <see cref="BigDecimal"/>.</param>
		/// <returns>
		/// Returns a <see cref="BigDecimal"/> instance with the value <c><see cref="unscaledVal"/> 
		/// * 10^(-<see cref="scale"/>)</c>.
		/// </returns>
		public static BigDecimal ValueOf(long unscaledVal, int scale) {
			if (scale == 0)
				return ValueOf(unscaledVal);
			if ((unscaledVal == 0) && (scale >= 0) &&
			    (scale < ZeroScaledBy.Length)) {
				return ZeroScaledBy[scale];
			}

			return new BigDecimal(unscaledVal, scale);
		}

		/// <summary>
		/// Returns a new <see cref="BigDecimal"/> instance whose value is equal 
		/// to <paramref name="unscaledVal"/>. The scale of the result is <c>0</c>, 
		/// and its unscaled value is <paramref name="unscaledVal"/>.
		/// </summary>
		/// <param name="unscaledVal">The value to be converted to a <see cref="BigDecimal"/>.</param>
		/// <returns>
		/// Returns a <see cref="BigDecimal"/> instance with the value <paramref name="unscaledVal"/>.
		/// </returns>
		public static BigDecimal ValueOf(long unscaledVal) {
			if ((unscaledVal >= 0) && (unscaledVal < BiScaledByZeroLength)) {
				return BiScaledByZero[(int) unscaledVal];
			}
			return new BigDecimal(unscaledVal, 0);
		}


		/// <summary>
		/// Adds a value to the current instance of <see cref="BigDecimal"/>.
		/// The scale of the result is the maximum of the scales of the two arguments.
		/// </summary>
		/// <param name="augend">The value to be added to this instance.</param>
		/// <returns>
		/// Returns a new {@code BigDecimal} whose value is <c>this + <paramref name="augend"/></c>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// If the given <paramref name="augend"/> is <c>null</c>.
		/// </exception>
		public BigDecimal Add(BigDecimal augend) {
			int diffScale = _scale - augend._scale;
			// Fast return when some operand is zero
			if (IsZero) {
				if (diffScale <= 0)
					return augend;
				if (augend.IsZero)
					return this;
			} else if (augend.IsZero) {
				if (diffScale >= 0)
					return this;
			}
			// Let be:  this = [u1,s1]  and  augend = [u2,s2]
			if (diffScale == 0) {
				// case s1 == s2: [u1 + u2 , s1]
				if (System.Math.Max(_bitLength, augend._bitLength) + 1 < 64) {
					return ValueOf(smallValue + augend.smallValue, _scale);
				}
				return new BigDecimal((GetUnscaledValue() + augend.GetUnscaledValue()), _scale);
			}
			if (diffScale > 0)
				// case s1 > s2 : [(u1 + u2) * 10 ^ (s1 - s2) , s1]
				return AddAndMult10(this, augend, diffScale);

			// case s2 > s1 : [(u2 + u1) * 10 ^ (s2 - s1) , s2]
			return AddAndMult10(augend, this, -diffScale);
		}

		private static BigDecimal AddAndMult10(BigDecimal thisValue, BigDecimal augend, int diffScale) {
			if (diffScale < LongTenPow.Length &&
			    System.Math.Max(thisValue._bitLength, augend._bitLength + LongTenPowBitLength[diffScale]) + 1 < 64) {
				return ValueOf(thisValue.smallValue + augend.smallValue*LongTenPow[diffScale], thisValue._scale);
			}
			return new BigDecimal(
				thisValue.GetUnscaledValue() + Multiplication.MultiplyByTenPow(augend.GetUnscaledValue(), diffScale),
				thisValue._scale);
		}

		/// <summary>
		/// Adds a value to the current instance of <see cref="BigDecimal"/>,
		/// rounding the result according to the provided context.
		/// </summary>
		/// <param name="augend">The value to be added to this instance.</param>
		/// <param name="mc">The rounding mode and precision for the result of 
		/// this operation.</param>
		/// <returns>
		/// Returns a new <see cref="BigDecimal"/> whose value is <c>this + <paramref name="augend"/></c>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// If the given <paramref name="augend"/> or <paramref name="mc"/> is <c>null</c>.
		/// </exception>
		public BigDecimal Add(BigDecimal augend, MathContext mc) {
			BigDecimal larger; // operand with the largest unscaled value
			BigDecimal smaller; // operand with the smallest unscaled value
			BigInteger tempBi;
			long diffScale = (long) this._scale - augend._scale;

			// Some operand is zero or the precision is infinity  
			if ((augend.IsZero) || (IsZero) || (mc.Precision == 0)) {
				return Add(augend).Round(mc);
			}
			// Cases where there is room for optimizations
			if (AproxPrecision() < diffScale - 1) {
				larger = augend;
				smaller = this;
			} else if (augend.AproxPrecision() < -diffScale - 1) {
				larger = this;
				smaller = augend;
			} else {
// No optimization is done 
				return Add(augend).Round(mc);
			}
			if (mc.Precision >= larger.AproxPrecision()) {
				// No optimization is done
				return Add(augend).Round(mc);
			}

			// Cases where it's unnecessary to add two numbers with very different scales 
			var largerSignum = larger.Sign;
			if (largerSignum == smaller.Sign) {
				tempBi = Multiplication.MultiplyByPositiveInt(larger.GetUnscaledValue(), 10) +
					BigInteger.FromInt64(largerSignum);
			} else {
				tempBi = larger.GetUnscaledValue() - BigInteger.FromInt64(largerSignum);
				tempBi = Multiplication.MultiplyByPositiveInt(tempBi, 10) +
					BigInteger.FromInt64(largerSignum*9);
			}
			// Rounding the improved adding 
			larger = new BigDecimal(tempBi, larger._scale + 1);
			return larger.Round(mc);
		}

		/// <summary>
		/// Subtracts the given value from this instance of <see cref="BigDecimal"/>.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="subtrahend">The value to be subtracted from this <see cref="BigDecimal"/>.</param>
		/// <returns>
		/// Returns an instance of <see cref="BigDecimal"/> that is the result of the
		/// subtraction of the given <paramref name="subtrahend"/> from this instance.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// If the given <paramref name="subtrahend"/> is <c>null</c>.
		/// </exception>
		public BigDecimal Subtract(BigDecimal subtrahend) {
			if (subtrahend == null)
				throw new ArgumentNullException("subtrahend");

			int diffScale = _scale - subtrahend._scale;

			// Fast return when some operand is zero
			if (IsZero) {
				if (diffScale <= 0) {
					return subtrahend.Negate();
				}
				if (subtrahend.IsZero) {
					return this;
				}
			} else if (subtrahend.IsZero) {
				if (diffScale >= 0) {
					return this;
				}
			}
			// Let be: this = [u1,s1] and subtrahend = [u2,s2] so:
			if (diffScale == 0) {
				// case s1 = s2 : [u1 - u2 , s1]
				if (System.Math.Max(_bitLength, subtrahend._bitLength) + 1 < 64) {
					return ValueOf(smallValue - subtrahend.smallValue, _scale);
				}
				return new BigDecimal(GetUnscaledValue() - subtrahend.GetUnscaledValue(), _scale);
			}
			if (diffScale > 0) {
				// case s1 > s2 : [ u1 - u2 * 10 ^ (s1 - s2) , s1 ]
				if (diffScale < LongTenPow.Length &&
				    System.Math.Max(_bitLength, subtrahend._bitLength + LongTenPowBitLength[diffScale]) + 1 < 64) {
					return ValueOf(smallValue - subtrahend.smallValue*LongTenPow[diffScale], _scale);
				}
				return new BigDecimal(
					GetUnscaledValue() - Multiplication.MultiplyByTenPow(subtrahend.GetUnscaledValue(), diffScale),
					_scale);
			}

			// case s2 > s1 : [ u1 * 10 ^ (s2 - s1) - u2 , s2 ]
			diffScale = -diffScale;
			if (diffScale < LongTenPow.Length &&
			    System.Math.Max(_bitLength + LongTenPowBitLength[diffScale], subtrahend._bitLength) + 1 < 64) {
				return ValueOf(smallValue*LongTenPow[diffScale] - subtrahend.smallValue, subtrahend._scale);
			}

			return new BigDecimal(Multiplication.MultiplyByTenPow(GetUnscaledValue(), diffScale) -
				subtrahend.GetUnscaledValue(), subtrahend._scale);
		}

		/// <summary>
		/// Subtracts the given value from this instance of <see cref="BigDecimal"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This overload rounds the result of the operation to the <paramref name="mc">context</paramref>
		/// provided as argument.
		/// </para>
		/// </remarks>
		/// <param name="subtrahend">The value to be subtracted from this <see cref="BigDecimal"/>.</param>
		/// <param name="mc">The context used to round the result of this operation.</param>
		/// <returns>
		/// Returns an instance of <see cref="BigDecimal"/> that is the result of the
		/// subtraction of the given <paramref name="subtrahend"/> from this instance.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// If either of the given <paramref name="subtrahend"/> or <paramref name="mc"/> are <c>null</c>.
		/// </exception>
		public BigDecimal Subtract(BigDecimal subtrahend, MathContext mc) {
			if (subtrahend == null)
				throw new ArgumentNullException("subtrahend");
			if (mc == null)
				throw new ArgumentNullException("mc");

			long diffScale = subtrahend._scale - (long) _scale;

			// Some operand is zero or the precision is infinity  
			if ((subtrahend.IsZero) || (IsZero) || (mc.Precision == 0))
				return Subtract(subtrahend).Round(mc);

			// Now:   this != 0   and   subtrahend != 0
			if (subtrahend.AproxPrecision() < diffScale - 1) {
				// Cases where it is unnecessary to subtract two numbers with very different scales
				if (mc.Precision < AproxPrecision()) {
					var thisSignum = Sign;
					BigInteger tempBI;
					if (thisSignum != subtrahend.Sign) {
						tempBI = Multiplication.MultiplyByPositiveInt(this.GetUnscaledValue(), 10) +
							BigInteger.FromInt64(thisSignum);
					} else {
						tempBI = GetUnscaledValue()-BigInteger.FromInt64(thisSignum);
						tempBI = Multiplication.MultiplyByPositiveInt(tempBI, 10) +
							BigInteger.FromInt64(thisSignum*9);
					}
					// Rounding the improved subtracting
					var leftOperand = new BigDecimal(tempBI, _scale + 1); // it will be only the left operand (this) 
					return leftOperand.Round(mc);
				}
			}

			// No optimization is done
			return Subtract(subtrahend).Round(mc);
		}

		/**
		 * Returns a new {@code BigDecimal} whose value is {@code this *
		 * multiplicand}. The scale of the result is the sum of the scales of the
		 * two arguments.
		 *
		 * @param multiplicand
		 *            value to be multiplied with {@code this}.
		 * @return {@code this * multiplicand}.
		 * @throws NullPointerException
		 *             if {@code multiplicand == null}.
		 */

		public BigDecimal Multiply(BigDecimal multiplicand) {
			long newScale = (long) _scale + multiplicand._scale;

			if ((IsZero) || (multiplicand.IsZero)) {
				return GetZeroScaledBy(newScale);
			}
			/* Let be: this = [u1,s1] and multiplicand = [u2,s2] so:
			 * this x multiplicand = [ s1 * s2 , s1 + s2 ] */
			if (_bitLength + multiplicand._bitLength < 64) {
				return ValueOf(smallValue*multiplicand.smallValue, ToIntScale(newScale));
			}
			return new BigDecimal(GetUnscaledValue().Multiply(multiplicand.GetUnscaledValue()), ToIntScale(newScale));
		}

		/**
		 * Returns a new {@code BigDecimal} whose value is {@code this *
		 * multiplicand}. The result is rounded according to the passed context
		 * {@code mc}.
		 *
		 * @param multiplicand
		 *            value to be multiplied with {@code this}.
		 * @param mc
		 *            rounding mode and precision for the result of this operation.
		 * @return {@code this * multiplicand}.
		 * @throws NullPointerException
		 *             if {@code multiplicand == null} or {@code mc == null}.
		 */

		public BigDecimal Multiply(BigDecimal multiplicand, MathContext mc) {
			BigDecimal result = Multiply(multiplicand);

			result.InplaceRound(mc);
			return result;
		}

		/**
		 * Returns a new {@code BigDecimal} whose value is {@code this / divisor}.
		 * As scale of the result the parameter {@code scale} is used. If rounding
		 * is required to meet the specified scale, then the specified rounding mode
		 * {@code roundingMode} is applied.
		 *
		 * @param divisor
		 *            value by which {@code this} is divided.
		 * @param scale
		 *            the scale of the result returned.
		 * @param roundingMode
		 *            rounding mode to be used to round the result.
		 * @return {@code this / divisor} rounded according to the given rounding
		 *         mode.
		 * @throws NullPointerException
		 *             if {@code divisor == null} or {@code roundingMode == null}.
		 * @throws ArithmeticException
		 *             if {@code divisor == 0}.
		 * @throws ArithmeticException
		 *             if {@code roundingMode == RoundingMode.UNNECESSAR}Y and
		 *             rounding is necessary according to the given scale and given
		 *             precision.
		 */

		public BigDecimal Divide(BigDecimal divisor, int scale, RoundingMode roundingMode) {
			// Let be: this = [u1,s1]  and  divisor = [u2,s2]
			if (divisor.IsZero) {
				// math.04=Division by zero
				throw new ArithmeticException(Messages.math04); //$NON-NLS-1$
			}

			long diffScale = ((long) _scale - divisor._scale) - scale;
			if (_bitLength < 64 && divisor._bitLength < 64) {
				if (diffScale == 0)
					return DividePrimitiveLongs(smallValue, divisor.smallValue, scale, roundingMode);
				if (diffScale > 0) {
					if (diffScale < LongTenPow.Length &&
					    divisor._bitLength + LongTenPowBitLength[(int) diffScale] < 64) {
						return DividePrimitiveLongs(smallValue, divisor.smallValue*LongTenPow[(int) diffScale], scale, roundingMode);
					}
				} else {
					// diffScale < 0
					if (-diffScale < LongTenPow.Length &&
					    _bitLength + LongTenPowBitLength[(int) -diffScale] < 64) {
						return DividePrimitiveLongs(smallValue*LongTenPow[(int) -diffScale], divisor.smallValue, scale, roundingMode);
					}

				}
			}
			BigInteger scaledDividend = GetUnscaledValue();
			BigInteger scaledDivisor = divisor.GetUnscaledValue(); // for scaling of 'u2'

			if (diffScale > 0) {
				// Multiply 'u2'  by:  10^((s1 - s2) - scale)
				scaledDivisor = Multiplication.MultiplyByTenPow(scaledDivisor, (int) diffScale);
			} else if (diffScale < 0) {
				// Multiply 'u1'  by:  10^(scale - (s1 - s2))
				scaledDividend = Multiplication.MultiplyByTenPow(scaledDividend, (int) -diffScale);
			}
			return DivideBigIntegers(scaledDividend, scaledDivisor, scale, roundingMode);
		}

		#endregion

		#region Operations

		#endregion

		/**
		 * Returns a new {@code BigDecimal} whose value is {@code this}, rounded
		 * according to the passed context {@code mc}.
		 * <p>
		 * If {@code mc.precision = 0}, then no rounding is performed.
		 * <p>
		 * If {@code mc.precision > 0} and {@code mc.roundingMode == UNNECESSARY},
		 * then an {@code ArithmeticException} is thrown if the result cannot be
		 * represented exactly within the given precision.
		 *
		 * @param mc
		 *            rounding mode and precision for the result of this operation.
		 * @return {@code this} rounded according to the passed context.
		 * @throws ArithmeticException
		 *             if {@code mc.precision > 0} and {@code mc.roundingMode ==
		 *             UNNECESSARY} and this cannot be represented within the given
		 *             precision.
		 */

		/**
		 * Compares this {@code BigDecimal} with {@code val}. Returns one of the
		 * three values {@code 1}, {@code 0}, or {@code -1}. The method behaves as
		 * if {@code this.subtract(val)} is computed. If this difference is > 0 then
		 * 1 is returned, if the difference is < 0 then -1 is returned, and if the
		 * difference is 0 then 0 is returned. This means, that if two decimal
		 * instances are compared which are equal in value but differ in scale, then
		 * these two instances are considered as equal.
		 *
		 * @param val
		 *            value to be compared with {@code this}.
		 * @return {@code 1} if {@code this > val}, {@code -1} if {@code this < val},
		 *         {@code 0} if {@code this == val}.
		 * @throws NullPointerException
		 *             if {@code val == null}.
		 */

		public int CompareTo(BigDecimal val) {
			int thisSign = Sign;
			int valueSign = val.Sign;

			if (thisSign == valueSign) {
				if (this._scale == val._scale && this._bitLength < 64 && val._bitLength < 64) {
					return (smallValue < val.smallValue) ? -1 : (smallValue > val.smallValue) ? 1 : 0;
				}
				long diffScale = (long) this._scale - val._scale;
				int diffPrecision = this.AproxPrecision() - val.AproxPrecision();
				if (diffPrecision > diffScale + 1) {
					return thisSign;
				} else if (diffPrecision < diffScale - 1) {
					return -thisSign;
				} else {
// thisSign == val.signum()  and  diffPrecision is aprox. diffScale
					BigInteger thisUnscaled = this.GetUnscaledValue();
					BigInteger valUnscaled = val.GetUnscaledValue();
					// If any of both precision is bigger, append zeros to the shorter one
					if (diffScale < 0) {
						thisUnscaled = thisUnscaled.Multiply(Multiplication.PowerOf10(-diffScale));
					} else if (diffScale > 0) {
						valUnscaled = valUnscaled.Multiply(Multiplication.PowerOf10(diffScale));
					}
					return thisUnscaled.CompareTo(valUnscaled);
				}
			} else if (thisSign < valueSign) {
				return -1;
			} else {
				return 1;
			}
		}

		/**
		 * Returns {@code true} if {@code x} is a {@code BigDecimal} instance and if
		 * this instance is equal to this big decimal. Two big decimals are equal if
		 * their unscaled value and their scale is equal. For example, 1.0
		 * (10*10^(-1)) is not equal to 1.00 (100*10^(-2)). Similarly, zero
		 * instances are not equal if their scale differs.
		 *
		 * @param x
		 *            object to be compared with {@code this}.
		 * @return true if {@code x} is a {@code BigDecimal} and {@code this == x}.
		 */

		public override bool Equals(object obj) {
			if (ReferenceEquals(this, obj))
				return true;

			if (!(obj is BigDecimal))
				return false;

			return Equals((BigDecimal) obj);
		}

		public bool Equals(BigDecimal other) {
			if (ReferenceEquals(this, other))
				return true;

			if (other == null)
				return false;

			return other._scale == _scale
			       && (_bitLength < 64
				       ? (other.smallValue == smallValue)
				       : intVal.Equals(other.intVal));
		}

		/**
		 * Returns the minimum of this {@code BigDecimal} and {@code val}.
		 *
		 * @param val
		 *            value to be used to compute the minimum with this.
		 * @return {@code min(this, val}.
		 * @throws NullPointerException
		 *             if {@code val == null}.
		 */

		/**
		 * Returns a hash code for this {@code BigDecimal}.
		 *
		 * @return hash code for {@code this}.
		 */

		public override int GetHashCode() {
			int hashCode;
			if (_bitLength < 64) {
				hashCode = (int) (smallValue & 0xffffffff);
				hashCode = 33*hashCode + (int) ((smallValue >> 32) & 0xffffffff);
				hashCode = 17*hashCode + _scale;
				return hashCode;
			}

			hashCode = 17*intVal.GetHashCode() + _scale;
			return hashCode;
		}

		/**
		 * Returns a canonical string representation of this {@code BigDecimal}. If
		 * necessary, scientific notation is used. This representation always prints
		 * all significant digits of this value.
		 * <p>
		 * If the scale is negative or if {@code scale - precision >= 6} then
		 * scientific notation is used.
		 *
		 * @return a string representation of {@code this} in scientific notation if
		 *         necessary.
		 */

		/**
		 * Returns the unit in the last place (ULP) of this {@code BigDecimal}
		 * instance. An ULP is the distance to the nearest big decimal with the same
		 * precision.
		 * <p>
		 * The amount of a rounding error in the evaluation of a floating-point
		 * operation is often expressed in ULPs. An error of 1 ULP is often seen as
		 * a tolerable error.
		 * <p>
		 * For class {@code BigDecimal}, the ULP of a number is simply 10^(-scale).
		 * <p>
		 * For example, {@code new BigDecimal(0.1).ulp()} returns {@code 1E-55}.
		 *
		 * @return unit in the last place (ULP) of this {@code BigDecimal} instance.
		 */

		public BigDecimal Ulp() {
			return ValueOf(1, _scale);
		}

		/* Private Methods */

		/**
		 * It does all rounding work of the public method
		 * {@code round(MathContext)}, performing an inplace rounding
		 * without creating a new object.
		 *
		 * @param mc
		 *            the {@code MathContext} for perform the rounding.
		 * @see #round(MathContext)
		 */

		private void InplaceRound(MathContext mc) {
			int mcPrecision = mc.Precision;
			if (AproxPrecision() - mcPrecision <= 0 || mcPrecision == 0) {
				return;
			}
			int discardedPrecision = Precision - mcPrecision;
			// If no rounding is necessary it returns immediately
			if ((discardedPrecision <= 0)) {
				return;
			}
			// When the number is small perform an efficient rounding
			if (this._bitLength < 64) {
				SmallRound(mc, discardedPrecision);
				return;
			}
			// Getting the integer part and the discarded fraction
			BigInteger sizeOfFraction = Multiplication.PowerOf10(discardedPrecision);
			BigInteger fraction;
			BigInteger integer = GetUnscaledValue().DivideAndRemainder(sizeOfFraction, out fraction);
			long newScale = (long) _scale - discardedPrecision;
			int compRem;
			BigDecimal tempBD;
			// If the discarded fraction is non-zero, perform rounding
			if (fraction.Sign != 0) {
				// To check if the discarded fraction >= 0.5
				compRem = (fraction.Abs().ShiftLeftOneBit().CompareTo(sizeOfFraction));
				// To look if there is a carry
				compRem = RoundingBehavior(integer.TestBit(0) ? 1 : 0, fraction.Sign*(5 + compRem),
					mc.RoundingMode);
				if (compRem != 0) {
					integer += BigInteger.FromInt64(compRem);
				}
				tempBD = new BigDecimal(integer);
				// If after to add the increment the precision changed, we normalize the size
				if (tempBD.Precision > mcPrecision) {
					integer = integer.Divide(BigInteger.Ten);
					newScale--;
				}
			}
			// To update all internal fields
			_scale = ToIntScale(newScale);
			_precision = mcPrecision;
			SetUnscaledValue(integer);
		}

		private static int LongCompareTo(long value1, long value2) {
			return value1 > value2 ? 1 : (value1 < value2 ? -1 : 0);
		}

		/**
		 * This method implements an efficient rounding for numbers which unscaled
		 * value fits in the type {@code long}.
		 *
		 * @param mc
		 *            the context to use
		 * @param discardedPrecision
		 *            the number of decimal digits that are discarded
		 * @see #round(MathContext)
		 */

		private void SmallRound(MathContext mc, int discardedPrecision) {
			long sizeOfFraction = LongTenPow[discardedPrecision];
			long newScale = (long) _scale - discardedPrecision;
			long unscaledVal = smallValue;
			// Getting the integer part and the discarded fraction
			long integer = unscaledVal/sizeOfFraction;
			long fraction = unscaledVal%sizeOfFraction;
			int compRem;
			// If the discarded fraction is non-zero perform rounding
			if (fraction != 0) {
				// To check if the discarded fraction >= 0.5
				compRem = LongCompareTo(System.Math.Abs(fraction) << 1, sizeOfFraction);
				// To look if there is a carry
				integer += RoundingBehavior(((int) integer) & 1, System.Math.Sign(fraction)*(5 + compRem),
					mc.RoundingMode);
				// If after to add the increment the precision changed, we normalize the size
				if (System.Math.Log10(System.Math.Abs(integer)) >= mc.Precision) {
					integer /= 10;
					newScale--;
				}
			}
			// To update all internal fields
			_scale = ToIntScale(newScale);
			_precision = mc.Precision;
			smallValue = integer;
			_bitLength = BitLength(integer);
			intVal = null;
		}

		/**
		 * Return an increment that can be -1,0 or 1, depending of
		 * {@code roundingMode}.
		 *
		 * @param parityBit
		 *            can be 0 or 1, it's only used in the case
		 *            {@code HALF_EVEN}
		 * @param fraction
		 *            the mantisa to be analyzed
		 * @param roundingMode
		 *            the type of rounding
		 * @return the carry propagated after rounding
		 */

		private static int RoundingBehavior(int parityBit, int fraction, RoundingMode roundingMode) {
			int increment = 0; // the carry after rounding

			switch (roundingMode) {
				case RoundingMode.Unnecessary:
					if (fraction != 0) {
						// math.08=Rounding necessary
						throw new ArithmeticException(Messages.math08); //$NON-NLS-1$
					}
					break;
				case RoundingMode.Up:
					increment = System.Math.Sign(fraction);
					break;
				case RoundingMode.Down:
					break;
				case RoundingMode.Ceiling:
					increment = System.Math.Max(System.Math.Sign(fraction), 0);
					break;
				case RoundingMode.Floor:
					increment = System.Math.Min(System.Math.Sign(fraction), 0);
					break;
				case RoundingMode.HalfUp:
					if (System.Math.Abs(fraction) >= 5) {
						increment = System.Math.Sign(fraction);
					}
					break;
				case RoundingMode.HalfDown:
					if (System.Math.Abs(fraction) > 5) {
						increment = System.Math.Sign(fraction);
					}
					break;
				case RoundingMode.HalfEven:
					if (System.Math.Abs(fraction) + parityBit > 5) {
						increment = System.Math.Sign(fraction);
					}
					break;
			}
			return increment;
		}

		/**
		 * If {@code intVal} has a fractional part throws an exception,
		 * otherwise it counts the number of bits of value and checks if it's out of
		 * the range of the primitive type. If the number fits in the primitive type
		 * returns this number as {@code long}, otherwise throws an
		 * exception.
		 *
		 * @param bitLengthOfType
		 *            number of bits of the type whose value will be calculated
		 *            exactly
		 * @return the exact value of the integer part of {@code BigDecimal}
		 *         when is possible
		 * @throws ArithmeticException when rounding is necessary or the
		 *             number don't fit in the primitive type
		 */

		private long ValueExact(int bitLengthOfType) {
			BigInteger bigInteger = ToBigIntegerExact();

			if (bigInteger.BitLength < bitLengthOfType) {
				// It fits in the primitive type
				return bigInteger.ToInt64();
			}
			// math.08=Rounding necessary
			throw new ArithmeticException(Messages.math08); //$NON-NLS-1$
		}

		/**
		 * If the precision already was calculated it returns that value, otherwise
		 * it calculates a very good approximation efficiently . Note that this
		 * value will be {@code precision()} or {@code precision()-1}
		 * in the worst case.
		 *
		 * @return an approximation of {@code precision()} value
		 */

		private int AproxPrecision() {
			return ((_precision > 0) ? _precision : (int) ((_bitLength - 1)*Log10Of2)) + 1;
		}

		/**
	 * It tests if a scale of type {@code long} fits in 32 bits. It
	 * returns the same scale being casted to {@code int} type when is
	 * possible, otherwise throws an exception.
	 *
	 * @param longScale
	 *            a 64 bit scale
	 * @return a 32 bit scale when is possible
	 * @throws ArithmeticException when {@code scale} doesn't
	 *             fit in {@code int} type
	 * @see #scale
	 */

		private static int ToIntScale(long longScale) {
			if (longScale < Int32.MinValue) {
				// math.09=Overflow
				throw new ArithmeticException(Messages.math09); //$NON-NLS-1$
			} else if (longScale > Int32.MaxValue) {
				// math.0A=Underflow
				throw new ArithmeticException(Messages.math0A); //$NON-NLS-1$
			} else {
				return (int) longScale;
			}
		}

		/**
		 * It returns the value 0 with the most approximated scale of type
		 * {@code int}. if {@code longScale > Integer.MAX_VALUE} the
		 * scale will be {@code Integer.MAX_VALUE}; if
		 * {@code longScale < Integer.MIN_VALUE} the scale will be
		 * {@code Integer.MIN_VALUE}; otherwise {@code longScale} is
		 * casted to the type {@code int}.
		 *
		 * @param longScale
		 *            the scale to which the value 0 will be scaled.
		 * @return the value 0 scaled by the closer scale of type {@code int}.
		 * @see #scale
		 */

		private static BigDecimal GetZeroScaledBy(long longScale) {
			if (longScale == (int) longScale) {
				return ValueOf(0, (int) longScale);
			}
			if (longScale >= 0) {
				return new BigDecimal(0, Int32.MaxValue);
			}
			return new BigDecimal(0, Int32.MinValue);
		}


		private BigInteger GetUnscaledValue() {
			if (intVal == null)
				intVal = BigInteger.FromInt64(smallValue);
			return intVal;
		}

		private void SetUnscaledValue(BigInteger unscaledValue) {
			intVal = unscaledValue;
			_bitLength = unscaledValue.BitLength;
			if (_bitLength < 64) {
				smallValue = unscaledValue.ToInt64();
			}
		}

		private static int BitLength(long smallValue) {
			if (smallValue < 0) {
				smallValue = ~smallValue;
			}
			return 64 - Utils.NumberOfLeadingZeros(smallValue);
		}

		private static int BitLength(int smallValue) {
			if (smallValue < 0) {
				smallValue = ~smallValue;
			}
			return 32 - Utils.NumberOfLeadingZeros(smallValue);
		}

		/*
		[OnSerializing]
		internal void BeforeSerialization(StreamingContext context) {
			GetUnscaledValue();
		}

		[OnDeserialized]
		internal void AfterDeserialization(StreamingContext context) {
			intVal.AfterDeserialization(context);
			_bitLength = intVal.BitLength;
			if (_bitLength < 64) {
				smallValue = intVal.ToInt64();
			}
		}
		*/

#if !PORTABLE
		private BigDecimal(SerializationInfo info, StreamingContext context) {
			intVal = (BigInteger) info.GetValue("intVal", typeof(BigInteger));
			_scale = info.GetInt32("scale");
			_bitLength = intVal.BitLength;
			if (_bitLength < 64) {
				smallValue = intVal.ToInt64();
			}
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			GetUnscaledValue();
			info.AddValue("intVal", intVal, typeof(BigInteger));
			info.AddValue("scale", _scale);
		}

#endif
	}
}