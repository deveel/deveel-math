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
using System.Globalization;
using System.Text;
using System.Runtime.Serialization;
using static System.Formats.Asn1.AsnWriter;
using System.Net.NetworkInformation;

namespace Deveel.Math {
	/// <summary>
	/// This class represents immutable arbitrary precision decimal numbers.
	/// </summary>
	/// <remarks>
	/// Each <see cref="BigDecimal"/> instance is represented with a unscaled 
	/// arbitrary precision mantissa (the unscaled value) and a scale. The value 
	/// of the <see cref="BigDecimal"/> is <see cref="UnscaledValue"/> 10^(-<see cref="Scale"/>).
	/// </remarks>
	[Serializable]
	[System.Diagnostics.DebuggerDisplay("{ToString()}")]
	public sealed partial class BigDecimal : IComparable<BigDecimal>, IEquatable<BigDecimal>, ISerializable
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
		[NonSerialized]
		internal string toStringImage;

		/// <summary>
		/// An array with powers of five that fit in the type <see cref="long"/>
		/// (<c>5^0,5^1,...,5^27</c>).
		/// </summary>
		internal static readonly BigInteger[] FivePow;

		/// <summary>
		/// An array with powers of ten that fit in the type <see cref="long"/> 
		/// (<c>10^0,10^1,...,10^18</c>).
		/// </summary>
		internal static readonly BigInteger[] TenPow;

		/// <summary>
		/// An array with powers of ten that fit in the type <see cref="long"/> 
		/// (<c>10^0,10^1,...,10^18</c>).
		/// </summary>
		internal static readonly long[] LongTenPow = new long[] {
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
		internal static readonly int[] LongTenPowBitLength = new int[LongTenPow.Length];

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
		internal static readonly BigDecimal[] ZeroScaledBy = new BigDecimal[11];

		/// <summary>
		/// The arbitrary precision integer (unscaled value) in the internal
		/// representation of <see cref="BigDecimal"/>.
		/// </summary>
		private BigInteger intVal;

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
		[NonSerialized]
		private int _precision = 0;

        #region .ctor

        static BigDecimal()
        {
            // To fill all static arrays.
            int i = 0;

            for (; i < ZeroScaledBy.Length; i++)
            {
                BiScaledByZero[i] = new BigDecimal(i, 0);
                ZeroScaledBy[i] = new BigDecimal(0, i);
            }

            for (int j = 0; j < LongFivePowBitLength.Length; j++)
            {
                LongFivePowBitLength[j] = CalcBitLength(LongFivePow[j]);
            }
            for (int j = 0; j < LongTenPowBitLength.Length; j++)
            {
                LongTenPowBitLength[j] = CalcBitLength(LongTenPow[j]);
            }

            // Taking the references of useful powers.
            TenPow = Multiplication.BigTenPows;
            FivePow = Multiplication.BigFivePows;
        }

        private BigDecimal(long smallValue, int scale) {
			this.SmallValue = smallValue;
			_scale = scale;
			BitLength = CalcBitLength(smallValue);
		}

		private BigDecimal(int smallValue, int scale) {
			this.SmallValue = smallValue;
			_scale = scale;
			BitLength = CalcBitLength(smallValue);
		}

		internal BigDecimal() {
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the 64bit 
		/// double <paramref name="value"/>. The constructed big decimal is 
		/// equivalent to the given double.
		/// </summary>
		/// <param name="value">The double value to be converted to a 
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
		/// If <paramref name="value"/> is infinity or not a number.
		/// </exception>
		public BigDecimal(double value) {
			if (Double.IsInfinity(value) || Double.IsNaN(value)) {
				// math.03=Infinity or NaN
				throw new FormatException(Messages.math03); //$NON-NLS-1$
			}

			long bits = BitConverter.DoubleToInt64Bits(value); // IEEE-754

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
			int mantisaBits = CalcBitLength(mantisa);
			if (_scale < 0) {
				BitLength = mantisaBits == 0 ? 0 : mantisaBits - _scale;
				if (BitLength < 64) {
					SmallValue = mantisa << (-_scale);
				} else {
					intVal = BigInteger.FromInt64(mantisa) << (-_scale);
				}
				_scale = 0;
			} else if (_scale > 0) {
				// m * 2^e =  (m * 5^(-e)) * 10^e
				if (_scale < LongFivePow.Length && mantisaBits + LongFivePowBitLength[_scale] < 64) {
					SmallValue = mantisa*LongFivePow[_scale];
					BitLength = CalcBitLength(SmallValue);
				} else {
					SetUnscaledValue(Multiplication.MultiplyByFivePow(BigInteger.FromInt64(mantisa), _scale));
				}
			} else {
				// scale == 0
				SmallValue = mantisa;
				BitLength = mantisaBits;
			}
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the 64bit 
		/// double <paramref name="value"/>. The constructed big decimal is 
		/// equivalent to the given double.
		/// </summary>
		/// <param name="value">The double value to be converted to a 
		/// <see cref="BigDecimal"/> instance.</param>
		/// <param name="context">The rounding mode and precision for the result of 
		/// this operation.</param>
		/// <remarks>
		/// For example, <c>new BigDecimal(0.1)</c> is equal to <c>0.1000000000000000055511151231257827021181583404541015625</c>. 
		/// This happens as <c>0.1</c> cannot be represented exactly in binary.
		/// <para>
		/// To generate a big decimal instance which is equivalent to <c>0.1</c> use the
		/// <see cref="BigDecimal.Parse(string)"/> method.
		/// </para>
		/// </remarks>
		/// <exception cref="FormatException">
		/// If <paramref name="value"/> is infinity or not a number.
		/// </exception>
		/// <exception cref="ArithmeticException">
		/// if <see cref="MathContext.Precision"/> of <paramref name="context"/> is greater than 0
		/// and <see cref="MathContext.RoundingMode"/> is equal to <see cref="RoundingMode.Unnecessary"/>
		/// and the new big decimal cannot be represented within the given precision without rounding.
		/// </exception>
		public BigDecimal(double value, MathContext context)
			: this(value) {
			InplaceRound(context);
		}


		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the
		/// <paramref name="value">given big integer</paramref>.
		/// </summary>
		/// <param name="value">The value to be converted to a <see cref="BigDecimal"/> instance.</param>
		/// <remarks>
		/// The <see cref="Scale"/> of the result is <c>0</c>.
		/// </remarks>
		public BigDecimal(BigInteger value)
			: this(value, 0) {
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the
		/// <paramref name="value">given big integer</paramref>.
		/// </summary>
		/// <param name="value">
		/// The value to be converted to a <see cref="BigDecimal"/> instance.
		/// </param>
		/// <param name="context">
		/// The rounding mode and precision for the result of this operation.
		/// </param>
		/// <remarks>
		/// The <see cref="Scale"/> of the result is <c>0</c>.
		/// </remarks>
		/// <exception cref="ArithmeticException">
		/// If <see cref="MathContext.Precision"/> is greater than 0 and <see cref="MathContext.RoundingMode"/> is
		/// equal to <see cref="RoundingMode.Unnecessary"/> and the new big decimal cannot be represented  within the 
		/// given precision without rounding.
		/// </exception>
		public BigDecimal(BigInteger value, MathContext context)
			: this(value) {
			InplaceRound(context);
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from a 
		/// <paramref name="unscaledValue">given unscaled value</paramref> 
		/// and a given scale.
		/// </summary>
		/// <param name="unscaledValue">
		/// Represents the unscaled value of the decimal.
		/// </param>
		/// <param name="scale">
		/// The scale of this <see cref="BigDecimal"/>
		/// </param>
		/// <remarks>
		/// The value of this instance is <c><paramref name="unscaledValue"/> 10^(-<paramref name="scale"/>)</c>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// If <paramref name="unscaledValue"/> is <b>null</b>.
		/// </exception>
		public BigDecimal(BigInteger unscaledValue, int scale) {
			ArgumentNullException.ThrowIfNull(unscaledValue);

			_scale = scale;
			SetUnscaledValue(unscaledValue);
		}

        /// <summary>
        /// Constructs a new <see cref="BigDecimal"/> instance from 
        /// a <paramref name="unscaledValue">given unscaled value</paramref> 
        /// and a given scale.
        /// </summary>
        /// <param name="unscaledValue">
        /// Represents the unscaled value of this big decimal.
        /// </param>
        /// <param name="scale">
        /// The scale factor of the decimal.
        /// </param>
        /// <param name="context">
        /// The context used to round the result of the operations.
        /// </param>
        /// <remarks>
        /// <para>
        /// The value of this instance is <c><paramref name="unscaledValue"/> 10^(-<paramref name="scale"/>)</c>. 
        /// </para>
        /// <para>
		/// The result is rounded according to the specified math context.
		/// </para>
        /// </remarks>
        /// <exception cref="ArithmeticException">
        /// If <see cref="MathContext.Precision"/> is greater than zero, the
        /// <see cref="MathContext.RoundingMode"/> is set to <see cref="RoundingMode.Unnecessary"/>
        /// and the decimal cannot be represented within the given precision without rounding.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the given <paramref name="unscaledValue"/> is null.
        /// </exception>
        public BigDecimal(BigInteger unscaledValue, int scale, MathContext context)
			: this(unscaledValue, scale) {
			InplaceRound(context);
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the 
		/// given integer <paramref name="value"/>.
		/// </summary>
		/// <param name="value">
		/// The integer value to convert to a decimal.
		/// </param>
		/// <remarks>
		/// The scale factor of the result is zero.
		/// </remarks>
		public BigDecimal(int value)
			: this(value, 0) {
		}

        /// <summary>
        /// Constructs a new <see cref="BigDecimal"/> instance from the 
        /// given <paramref name="value">integer value</paramref>.
        /// </summary>
        /// <param name="value">
        /// Integer value to be converted to a <see cref="BigDecimal"/> instance.
        /// </param>
        /// <param name="context">
        /// The rounding mode and precision for the result of this operation.
        /// </param>
        /// <remarks>
        /// <para>
        /// The scale of the result is <c>0</c>. 
        /// </para>
        /// <para>
		/// The result is rounded according to the specified math context.
		/// </para>
        /// </remarks>
        /// <exception cref="ArithmeticException">
        /// Thrown if precision is greater than 0 and <see cref="RoundingMode"/> is
        /// <see cref="RoundingMode.Unnecessary"/> and the new big decimal cannot be represented
        /// within the given precision without rounding. 
        /// </exception>
        public BigDecimal(int value, MathContext context)
			: this(value, 0) {
			InplaceRound(context);
		}

		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the 
		/// given long integer <paramref name="value"/>, with a scale of <c>0</c>.
		/// </summary>
		/// <param name="value">
		/// The long integer value to be converted to a <see cref="BigDecimal"/>
		/// </param>
		public BigDecimal(long value)
			: this(value, 0) {
		}


		/// <summary>
		/// Constructs a new <see cref="BigDecimal"/> instance from the 
		/// given long <paramref name="value"/>, with a scale of <c>0</c> 
		/// and the value rounded according to the specified context.
		/// </summary>
		/// <param name="value">
		/// The long value to be converted to a <see cref="BigDecimal"/>
		/// </param>
		/// <param name="context">
		/// The context that defines the rounding mode and precision to apply to the
		/// value obtained from the given integer.
		/// </param>
		/// <exception cref="ArithmeticException">
		/// If the <see cref="MathContext.Precision"/> value specified is greater than <c>0</c> and
		/// the <see cref="MathContext.RoundingMode"/> is <see cref="RoundingMode.Unnecessary"/> and
		/// the new <see cref="BigDecimal"/> cannot be represented within the given precision
		/// without rounding.
		/// </exception>
		public BigDecimal(long value, MathContext context)
			: this(value) {
			InplaceRound(context);
		}

        private BigDecimal(SerializationInfo info, StreamingContext context)
        {
            intVal = (BigInteger)info.GetValue("intVal", typeof(BigInteger));
            _scale = info.GetInt32("scale");
            BitLength = intVal.BitLength;
            if (BitLength < 64)
            {
                SmallValue = intVal.ToInt64();
            }
        }

        #endregion

        /// <summary>
        /// Gets the sign of the decimal, where <c>-1</c> if the value is less than 0,
        /// <c>0</c> if the value is 0 and <c>1</c> if the value is greater than 0.
        /// </summary>
        /// <seealso cref="Sign"/>
        public int Sign {
			get {
				if (BitLength < 64)
					return System.Math.Sign(SmallValue);

				return GetUnscaledValue().Sign;
			}
		}

		internal int BitLength { get; set; }

		internal long SmallValue { get; set; }

        /// <summary>
        /// Gets a value indicating if the decimal is equivalent to zero.
        /// </summary>
        public bool IsZero =>
                //Watch out: -1 has a BitLength=0
                BitLength == 0 && SmallValue != -1;

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
			internal set { _scale = value; }
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
				int bitLength = this.BitLength;
				int decimalDigits = 1; // the precision to be calculated
				double doubleUnsc = 1; // intVal in 'double'

				if (bitLength < 1024) {
					// To calculate the precision for small numbers
					if (bitLength >= 64) {
						doubleUnsc = GetUnscaledValue().ToDouble();
					} else if (bitLength >= 1) {
						doubleUnsc = SmallValue;
					}
					decimalDigits += (int) System.Math.Log10(System.Math.Abs(doubleUnsc));
				} else {
					// (bitLength >= 1024)
					/* To calculate the precision for large numbers
				 * Note that: 2 ^(Bitlength - 1) <= intVal < 10 ^(precision()) */
					decimalDigits += (int) ((bitLength - 1)*Log10Of2);
					// If after division the number isn't zero, exists an aditional digit
					if ((GetUnscaledValue() / Multiplication.PowerOf10(decimalDigits)).Sign != 0) {
						decimalDigits++;
					}
				}
				_precision = decimalDigits;
				return _precision;
			}
			internal set { _precision = value; }
		}

        /// <summary>
        /// Gets the unscaled value (mantissa) of this <see cref="BigDecimal"/> instance as 
        /// a <see cref="BigInteger"/>.
        /// </summary>
        /// <remarks>
        /// The unscaled value can be computed as <c>(this * 10^(scale))</c>.
        /// </remarks>
        public BigInteger UnscaledValue => GetUnscaledValue();

        #region Public Methods

        /// <summary>
        /// Returns a new <see cref="BigDecimal"/> instance whose value is equal to 
        /// <paramref name="unscaledValue"/> 10^(-<paramref name="scale"/>). The scale 
        /// of the result is <see cref="Scale"/>, and its unscaled value is <see cref="UnscaledValue"/>.
        /// </summary>
        /// <param name="unscaledValue">The unscaled value to be used to construct 
        /// the new <see cref="BigDecimal"/>.</param>
        /// <param name="scale">The scale to be used to construct the new <see cref="BigDecimal"/>.</param>
        /// <returns>
        /// Returns a <see cref="BigDecimal"/> instance with the value <c><see cref="UnscaledValue"/> 
        /// * 10^(-<see cref="Scale"/>)</c>.
        /// </returns>
        public static BigDecimal Create(long unscaledValue, int scale) {
			if (scale == 0)
				return Create(unscaledValue);
			if ((unscaledValue == 0) && (scale >= 0) &&
			    (scale < ZeroScaledBy.Length)) {
				return ZeroScaledBy[scale];
			}

			return new BigDecimal(unscaledValue, scale);
		}

		/// <summary>
		/// Returns a new <see cref="BigDecimal"/> instance whose value is equal 
		/// to <paramref name="unscaledValue"/>. The scale of the result is <c>0</c>, 
		/// and its unscaled value is <paramref name="unscaledValue"/>.
		/// </summary>
		/// <param name="unscaledValue">
		/// The value to be converted to a <see cref="BigDecimal"/>.
		/// </param>
		/// <returns>
		/// Returns a <see cref="BigDecimal"/> instance with the value <paramref name="unscaledValue"/>.
		/// </returns>
		public static BigDecimal Create(long unscaledValue) {
			if ((unscaledValue >= 0) && (unscaledValue < BiScaledByZeroLength)) {
				return BiScaledByZero[(int) unscaledValue];
			}
			return new BigDecimal(unscaledValue, 0);
        }


        #endregion

        /// <summary>
        /// Compares this <see cref="BigDecimal"/> with the 
        /// given <paramref name="other"/> value.
        /// </summary>
        /// <param name="other">
		/// The other <see cref="BigDecimal"/> to compare with this instance.
		/// </param>
        /// <remarks>
        /// <para>
        /// The method behaves as if <c>this.Subtract(other)</c> is computed: if this 
        /// difference is greater than 0 then 1 is returned, if the difference is lesser than 
        /// 0 then -1 is returned, and if the difference is 0 then 0 is returned.
        /// </para>
        /// <para>
        /// This means, that if two decimal instances are compared which are equal in value but 
		/// differ in scale, then these two instances are considered as equal.
        /// </para>
        /// </remarks>
        /// <returns>
        /// Returns <c>-1</c> if this is one is greather than the <paramref name="other"/>,
        /// or <c>1</c> if the <paramref name="other"/> is greater, otherwise <c>0</c>
        /// if the two instances are equal.
        /// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the given <paramref name="other"/> is <b>null</b>.
		/// </exception>
        public int CompareTo(BigDecimal other) {
			ArgumentNullException.ThrowIfNull(other, nameof(other));

			int thisSign = Sign;
			int valueSign = other.Sign;

			if (thisSign == valueSign) {
				if (this._scale == other._scale && this.BitLength < 64 && other.BitLength < 64) {
					return (SmallValue < other.SmallValue) ? -1 : (SmallValue > other.SmallValue) ? 1 : 0;
				}
				long diffScale = (long) this._scale - other._scale;
				int diffPrecision = this.Precision - other.Precision;
				if (diffPrecision > diffScale + 1) {
					return thisSign;
				} else if (diffPrecision < diffScale - 1) {
					return -thisSign;
				} else {
					// thisSign == val.signum()  and  diffPrecision is aprox. diffScale
					BigInteger thisUnscaled = this.GetUnscaledValue();
					BigInteger valUnscaled = other.GetUnscaledValue();
					// If any of both precision is bigger, append zeros to the shorter one
					if (diffScale < 0) {
						thisUnscaled = thisUnscaled * Multiplication.PowerOf10(-diffScale);
					} else if (diffScale > 0) {
						valUnscaled = valUnscaled * Multiplication.PowerOf10(diffScale);
					}
					return thisUnscaled.CompareTo(valUnscaled);
				}
			} else if (thisSign < valueSign) {
				return -1;
			} else {
				return 1;
			}
        }

        /// <summary>
        /// Checks if the given <paramref name="obj"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">
		/// The object to compare with this instance.
		/// </param>
		/// <remarks>
		/// Two big decimals are equal if their unscaled value and their scale is equal.
		/// For example, 1.0 (10*10^(-1)) is not equal to 1.00 (100*10^(-2)). Similarly, 
		/// zero instances are not equal if their scale differs.
        /// </remarks>
        /// <returns>
        /// Returns <c>true</c> if <paramref name="obj"/> is a <see cref="BigDecimal"/> instance 
        /// and if this instance is equal to this big decimal.
        /// </returns>
		/// <seealso cref="Equals(BigDecimal)"/>
        public override bool Equals(object obj) {
			if (ReferenceEquals(this, obj))
				return true;

			if (!(obj is BigDecimal))
				return false;

			return Equals((BigDecimal) obj);
		}

        /// <summary>
        /// Checks if the given <paramref name="other"/> is equal to this instance.
        /// </summary>
        /// <param name="other">
		/// The object to compare with this instance.
		/// </param>
		/// <remarks>
		/// Two big decimals are equal if their unscaled value and their scale is equal.
		/// For example, 1.0 (10*10^(-1)) is not equal to 1.00 (100*10^(-2)). Similarly, 
		/// zero instances are not equal if their scale differs.
        /// </remarks>
        /// <returns>
        /// Returns <c>true</c> if <paramref name="other"/> is equal to this big decimal.
        /// </returns>
		/// <seealso cref="Equals(BigDecimal)"/>
		public bool Equals(BigDecimal other) {
			if (ReferenceEquals(this, other))
				return true;

			if (other == null)
				return false;

			return other._scale == _scale
			       && (BitLength < 64
				       ? (other.SmallValue == SmallValue)
				       : intVal.Equals(other.intVal));
		}

		/// <inheritdoc/>
		public override int GetHashCode() {
			int hashCode;
			if (BitLength < 64) {
				hashCode = (int) (SmallValue & 0xffffffff);
				hashCode = 33*hashCode + (int) ((SmallValue >> 32) & 0xffffffff);
				hashCode = 17*hashCode + _scale;
				return hashCode;
			}

			hashCode = 17*intVal.GetHashCode() + _scale;
			return hashCode;
        }

        /// <summary>
        /// It does all rounding work of the public method <see cref="BigMath.Round(BigDecimal, MathContext)"/>, 
		/// performing an inplace rounding without creating a new object.
        /// </summary>
        /// <param name="mc">
		/// The <see cref="MathContext"/> to use for rounding.
		/// </param>
		/// <seealso cref="BigMath.Round(BigDecimal, MathContext)"/>
        internal void InplaceRound(MathContext mc) {
			int mcPrecision = mc.Precision;
			if (Precision - mcPrecision <= 0 || mcPrecision == 0) {
				return;
			}
			int discardedPrecision = Precision - mcPrecision;
			// If no rounding is necessary it returns immediately
			if ((discardedPrecision <= 0)) {
				return;
			}
			// When the number is small perform an efficient rounding
			if (this.BitLength < 64) {
				SmallRound(mc, discardedPrecision);
				return;
			}
			// Getting the integer part and the discarded fraction
			BigInteger sizeOfFraction = Multiplication.PowerOf10(discardedPrecision);
			BigInteger fraction;
			BigInteger integer = BigMath.DivideAndRemainder(GetUnscaledValue(), sizeOfFraction, out fraction);
			long newScale = (long) _scale - discardedPrecision;
			int compRem;
			BigDecimal tempBD;
			// If the discarded fraction is non-zero, perform rounding
			if (fraction.Sign != 0) {
				// To check if the discarded fraction >= 0.5
				compRem = (BigMath.Abs(fraction).ShiftLeftOneBit().CompareTo(sizeOfFraction));
				// To look if there is a carry
				compRem = RoundingBehavior(BigInteger.TestBit(integer, 0) ? 1 : 0, fraction.Sign*(5 + compRem),
					mc.RoundingMode);
				if (compRem != 0) {
					integer += BigInteger.FromInt64(compRem);
				}
				tempBD = new BigDecimal(integer);
				// If after to add the increment the precision changed, we normalize the size
				if (tempBD.Precision > mcPrecision) {
					integer = integer / BigInteger.Ten;
					newScale--;
				}
			}
			// To update all internal fields
			_scale = ToIntScale(newScale);
			_precision = mcPrecision;
			SetUnscaledValue(integer);
		}

		internal static int LongCompareTo(long value1, long value2) {
			return value1 > value2 ? 1 : (value1 < value2 ? -1 : 0);
        }


        /// <summary>
        /// This method implements an efficient rounding for numbers which unscaled 
		/// value fits in the type <see cref="System.Int64"/>.
        /// </summary>
        /// <param name="mc">
		/// The <see cref="MathContext"/> to use for rounding.
		/// </param>
        /// <param name="discardedPrecision">
		/// The number of decimal digits that are discarded.
		/// </param>
		/// <see cref="BigMath.Round(BigDecimal, MathContext)"/>
        private void SmallRound(MathContext mc, int discardedPrecision) {
			long sizeOfFraction = LongTenPow[discardedPrecision];
			long newScale = (long) _scale - discardedPrecision;
			long unscaledVal = SmallValue;
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
			SmallValue = integer;
			BitLength = CalcBitLength(integer);
			intVal = null;
        }

        /// <summary>
        /// Return an increment that can be -1,0 or 1, depending 
        /// of <paramref name="roundingMode"/>.
        /// </summary>
        /// <param name="parityBit">
        /// A value that can be 0 or 1, and that it's only used 
        /// in the case <paramref name="roundingMode"/> is <see cref="RoundingMode.HalfEven"/>.
        /// </param>
        /// <param name="fraction">
        /// The mantisa to be analyzed
        /// </param>
        /// <param name="roundingMode">
		/// The rounding mode to be used.
		/// </param>
        /// <returns>
		/// Returns the carry propagated after rounding.
		/// </returns>
        /// <exception cref="ArithmeticException"></exception>
        internal static int RoundingBehavior(int parityBit, int fraction, RoundingMode roundingMode) {
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

        /// <summary>
        /// Returns the exact value of the integer part of this <see cref="BigDecimal"/> instance.
        /// </summary>
        /// <param name="bitLengthOfType">
		/// The number of bits of the type whose value will be calculated exactly.
		/// </param>
		/// <remarks>
		/// </remarks>
        /// <returns>
		/// Returns the exact value of the integer part of this <see cref="BigDecimal"/> instance.
		/// </returns>
        /// <exception cref="ArithmeticException">
		/// Thrown when rounding is necessary or the number doesn't fit in the primitive type.
		/// </exception>
        private long ValueExact(int bitLengthOfType) {
			BigInteger bigInteger = ToBigIntegerExact();

			if (bigInteger.BitLength < bitLengthOfType) {
				// It fits in the primitive type
				return bigInteger.ToInt64();
			}
			// math.08=Rounding necessary
			throw new ArithmeticException(Messages.math08); //$NON-NLS-1$
        }

        /// <summary>
        /// If the precision already was calculated it returns that value, otherwise
		/// it calculates a very good approximation efficiently
        /// </summary>
		/// <remarks>
		/// Note that this value will be <see cref="Precision"/> or 
		/// <c><see cref="Precision"/> - 1</c> in the worst case.
        /// </remarks>
        /// <returns>
		/// Returns an approximation of the <see cref="Precision"/> value.
		/// </returns>
        internal int AproxPrecision() {
			return ((_precision > 0) ? _precision : (int) ((BitLength - 1)*Log10Of2)) + 1;
        }

        /// <summary>
        /// Tests if a scale of type <see cref="long"/> fits in 32 bits.
        /// </summary>
        /// <param name="longScale">
		/// A 64 bit scale.
		/// </param>
		/// <remarks>
		/// It returns the same scale being casted to <see cref="int"/> type when 
		/// is possible, otherwise throws an exception.
        /// </remarks>
        /// <returns>
        /// Returns a 32 bit scale when is possible.
        /// </returns>
        /// <exception cref="ArithmeticException">
        /// Thrown when the scale doesn't fit in <see cref="int"/> type.
        /// </exception>
        internal static int ToIntScale(long longScale) {
			if (longScale < Int32.MinValue)
				// math.09=Overflow
				throw new ArithmeticException(Messages.math09); //$NON-NLS-1$

			if (longScale > Int32.MaxValue)
				// math.0A=Underflow
				throw new ArithmeticException(Messages.math0A); //$NON-NLS-1$

			return (int) longScale;
        }

        /// <summary>
        /// Returns the value 0 with the most approximated scale of type <see cref="int"/>.
        /// </summary>
        /// <param name="longScale">
		/// The scale to which the value 0 will be scaled.
		/// </param>
		/// <remarks>
		/// If <paramref name="longScale"/> is greater than <see cref="Int32.MaxValue"/> the 
		/// scale will be <see cref="Int32.MaxValue"/>; if <paramref name="longScale"/> is smaller
		/// than <see cref="Int32.MaxValue"/> the scale will be <see cref="Int32.MinValue"/>; otherwise 
		/// <paramref name="longScale"/> is casted to the type <see cref="int"/>.
		/// </remarks>
        /// <returns>
		/// Returns the value 0 scaled by the closer scale of type <see cref="int"/>.
		/// </returns>
        internal static BigDecimal GetZeroScaledBy(long longScale)
        {
            if (longScale == (int)longScale)
            {
                return Create(0, (int)longScale);
            }
            if (longScale >= 0)
            {
                return new BigDecimal(0, Int32.MaxValue);
            }
            return new BigDecimal(0, Int32.MinValue);
        }

        private BigInteger GetUnscaledValue() {
			if (intVal == null)
				intVal = BigInteger.FromInt64(SmallValue);
			return intVal;
		}

		internal void SetUnscaledValue(BigInteger unscaledValue) {
			intVal = unscaledValue;
			BitLength = unscaledValue.BitLength;
			if (BitLength < 64) {
				SmallValue = unscaledValue.ToInt64();
			}
		}

		internal static int CalcBitLength(long smallValue) {
			if (smallValue < 0) {
				smallValue = ~smallValue;
			}
			return 64 - Utils.NumberOfLeadingZeros(smallValue);
		}

		internal static int CalcBitLength(int smallValue) {
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

        /// <summary>
        /// Scales this number to given scale, using the 
        /// specified rounding mode.
        /// </summary>
        /// <param name="newScale">
		/// The new scale of the number to be returned.
		/// </param>
        /// <param name="roundingMode">
		/// The mode to be used to round the result.
		/// </param>
        /// <remarks>
        /// <para>
        /// If the new scale is greater than the old scale, then additional zeros are 
        /// added to the unscaled value: in this case no rounding is necessary.
        /// </para>
        /// <para>
        /// If the new scale is smaller than the old scale, then trailing digits are 
		/// removed. If these trailing digits are not zero, then the remaining unscaled 
		/// value has to be rounded. For this rounding operation the specified rounding 
		/// mode is used.
        /// </para>
        /// </remarks>
        /// <returns>
		/// Returns a new <see cref="BigDecimal"/> instance with the same value as this instance,
		/// but with the scale of the given value and the rounding mode specified.
		/// </returns>
		/// <exception cref="ArithmeticException">
		/// Thrown if the rounding mode is <see cref="RoundingMode.Unnecessary"/> and the
		/// result cannot be represented within the given precision without rounding.
		/// </exception>
        public BigDecimal ScaleTo(int newScale, RoundingMode roundingMode)
        {
            if (!Enum.IsDefined(typeof(RoundingMode), roundingMode))
                throw new ArgumentException();

            return BigDecimalMath.Scale(this, newScale, roundingMode);
        }


        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			GetUnscaledValue();
			info.AddValue("intVal", intVal, typeof(BigInteger));
			info.AddValue("scale", _scale);
		}
	}
}