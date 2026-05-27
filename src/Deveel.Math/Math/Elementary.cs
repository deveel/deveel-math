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
	/// <summary>
	/// Static library that provides the basic arithmetic mutable operations for
	/// <see cref="BigInteger"/>. The operations provided are:
	/// <list type="bullet">
	/// <item><description>Addition</description></item>
	/// <item><description>Subtraction</description></item>
	/// <item><description>Comparison</description></item>
	/// </list>
	/// In addition, some <i>Inplace</i> (mutable) methods are provided.
	/// </summary>
	static class Elementary {

		/// <summary>
		/// Compares two arrays. All elements are treated as unsigned integers. The
		/// magnitude is the bit chain of elements in big-endian order.
		/// </summary>
		/// <param name="a">The first array.</param>
		/// <param name="b">The second array.</param>
		/// <param name="size">The size of arrays.</param>
		/// <returns>1 if a &gt; b, -1 if a &lt; b, 0 if a == b.</returns>
		internal static int CompareArrays(ReadOnlySpan<int> a, ReadOnlySpan<int> b, int size) {
			int i;
			for (i = size - 1; (i >= 0) && (a[i] == b[i]); i--) {
				;
			}
			return ((i < 0) ? BigInteger.EQUALS
					: (a[i] & 0xFFFFFFFFL) < (b[i] & 0xFFFFFFFFL) ? BigInteger.LESS
							: BigInteger.GREATER);
		}

		/// <summary>
		/// Adds two <see cref="BigInteger"/> values.
		/// </summary>
		/// <param name="op1">The first operand.</param>
		/// <param name="op2">The second operand.</param>
		/// <returns><paramref name="op1"/> + <paramref name="op2"/>.</returns>
		/// <example>
		/// <code>
		/// BigInteger a = new BigInteger(10);
		/// BigInteger b = new BigInteger(20);
		/// BigInteger result = Elementary.Add(a, b);
		/// // result == 30
		/// </code>
		/// </example>
		internal static BigInteger Add(BigInteger op1, BigInteger op2) {
			int[] resDigits;
			int resSign;
			int op1Sign = op1.Sign;
			int op2Sign = op2.Sign;

			if (op1Sign == 0) {
				return op2;
			}
			if (op2Sign == 0) {
				return op1;
			}
			int op1Len = op1.numberLength;
			int op2Len = op2.numberLength;

			if (op1Len + op2Len == 2) {
				long a = (op1.Digits[0] & 0xFFFFFFFFL);
				long b = (op2.Digits[0] & 0xFFFFFFFFL);
				long res;
				int valueLo;
				int valueHi;

				if (op1Sign == op2Sign) {
					res = a + b;
					valueLo = (int) res;
					valueHi = (int) Utils.URShift(res, 32);
					return ((valueHi == 0)
					        	? new BigInteger(op1Sign, valueLo)
					        	: new BigInteger(op1Sign, 2, new int[] {
					        	                                       	valueLo,
					        	                                       	valueHi
					        	                                       }));
				}
				return BigInteger.FromInt64((op1Sign < 0) ? (b - a) : (a - b));
			} else if (op1Sign == op2Sign) {
				resSign = op1Sign;
				resDigits = (op1Len >= op2Len)
				            	? add(op1.Digits.AsSpan(0, op1Len), op1Len,
				            	      op2.Digits.AsSpan(0, op2Len), op2Len)
				            	: add(op2.Digits.AsSpan(0, op2Len), op2Len, op1.Digits.AsSpan(0, op1Len),
				            	      op1Len);
			} else {
				int cmp = ((op1Len != op2Len)
							? ((op1Len > op2Len) ? 1 : -1)
							: CompareArrays(op1.Digits.AsSpan(0, op1Len), op2.Digits.AsSpan(0, op1Len), op1Len));

				if (cmp == BigInteger.EQUALS) {
					return BigInteger.Zero;
				}
				if (cmp == BigInteger.GREATER) {
					resSign = op1Sign;
					resDigits = subtract(op1.Digits.AsSpan(0, op1Len), op1Len, op2.Digits.AsSpan(0, op2Len), op2Len);
				} else {
					resSign = op2Sign;
					resDigits = subtract(op2.Digits.AsSpan(0, op2Len), op2Len, op1.Digits.AsSpan(0, op1Len), op1Len);
				}
			}
			BigInteger result = new BigInteger(resSign, resDigits.Length, resDigits);
			result.CutOffLeadingZeroes();
			return result;
		}

		/// <summary>
		/// Performs <c>res = a + b</c>.
		/// </summary>
		/// <param name="res">The result array (must have enough space).</param>
		/// <param name="a">The first addend array.</param>
		/// <param name="aSize">The number of elements in <paramref name="a"/>.</param>
		/// <param name="b">The second addend array.</param>
		/// <param name="bSize">The number of elements in <paramref name="b"/>.</param>
		private static void add(Span<int> res, ReadOnlySpan<int> a, int aSize, ReadOnlySpan<int> b, int bSize) {
			int i;
			long carry = (a[0] & 0xFFFFFFFFL) + (b[0] & 0xFFFFFFFFL);

			res[0] = (int)carry;
			carry >>= 32;

			if (aSize >= bSize) {
				for (i = 1; i < bSize; i++) {
					carry += (a[i] & 0xFFFFFFFFL) + (b[i] & 0xFFFFFFFFL);
					res[i] = (int)carry;
					carry >>= 32;
				}
				for (; i < aSize; i++) {
					carry += a[i] & 0xFFFFFFFFL;
					res[i] = (int)carry;
					carry >>= 32;
				}
			} else {
				for (i = 1; i < aSize; i++) {
					carry += (a[i] & 0xFFFFFFFFL) + (b[i] & 0xFFFFFFFFL);
					res[i] = (int)carry;
					carry >>= 32;
				}
				for (; i < bSize; i++) {
					carry += b[i] & 0xFFFFFFFFL;
					res[i] = (int)carry;
					carry >>= 32;
				}
			}
			if (carry != 0) {
				res[i] = (int)carry;
			}
		}

		/// <summary>
		/// Subtracts <paramref name="op2"/> from <paramref name="op1"/>.
		/// </summary>
		/// <param name="op1">The minuend.</param>
		/// <param name="op2">The subtrahend.</param>
		/// <returns><paramref name="op1"/> - <paramref name="op2"/>.</returns>
		/// <example>
		/// <code>
		/// BigInteger a = new BigInteger(30);
		/// BigInteger b = new BigInteger(10);
		/// BigInteger result = Elementary.Subtract(a, b);
		/// // result == 20
		/// </code>
		/// </example>
		internal static BigInteger Subtract(BigInteger op1, BigInteger op2) {
			int resSign;
			int[] resDigits;
			int op1Sign = op1.Sign;
			int op2Sign = op2.Sign;

			if (op2Sign == 0) {
				return op1;
			}
			if (op1Sign == 0) {
				return -op2;
			}
			int op1Len = op1.numberLength;
			int op2Len = op2.numberLength;
			if (op1Len + op2Len == 2) {
				long a = (op1.Digits[0] & 0xFFFFFFFFL);
				long b = (op2.Digits[0] & 0xFFFFFFFFL);
				if (op1Sign < 0) {
					a = -a;
				}
				if (op2Sign < 0) {
					b = -b;
				}
				return BigInteger.FromInt64(a - b);
			}
			int cmp = ((op1Len != op2Len) ? ((op1Len > op2Len) ? 1 : -1)
					: Elementary.CompareArrays(op1.Digits.AsSpan(0, op1Len), op2.Digits.AsSpan(0, op1Len), op1Len));

			if (cmp == BigInteger.LESS) {
				resSign = -op2Sign;
				resDigits = (op1Sign == op2Sign) ? subtract(op2.Digits.AsSpan(0, op2Len), op2Len,
						op1.Digits.AsSpan(0, op1Len), op1Len) : add(op2.Digits.AsSpan(0, op2Len), op2Len, op1.Digits.AsSpan(0, op1Len),
						op1Len);
			} else {
				resSign = op1Sign;
				if (op1Sign == op2Sign) {
					if (cmp == BigInteger.EQUALS) {
						return BigInteger.Zero;
					}
					resDigits = subtract(op1.Digits.AsSpan(0, op1Len), op1Len, op2.Digits.AsSpan(0, op2Len), op2Len);
				} else {
					resDigits = add(op1.Digits.AsSpan(0, op1Len), op1Len, op2.Digits.AsSpan(0, op2Len), op2Len);
				}
			}
			BigInteger res = new BigInteger(resSign, resDigits.Length, resDigits);
			res.CutOffLeadingZeroes();
			return res;
		}

		/// <summary>
		/// Performs <c>res = a - b</c>. It is assumed the magnitude of <paramref name="a"/>
		/// is not less than the magnitude of <paramref name="b"/>.
		/// </summary>
		/// <param name="res">The result array.</param>
		/// <param name="a">The minuend array.</param>
		/// <param name="aSize">The number of elements in <paramref name="a"/>.</param>
		/// <param name="b">The subtrahend array.</param>
		/// <param name="bSize">The number of elements in <paramref name="b"/>.</param>
		private static void subtract(Span<int> res, ReadOnlySpan<int> a, int aSize, ReadOnlySpan<int> b, int bSize) {
			int i;
			long borrow = 0;

			for (i = 0; i < bSize; i++) {
				borrow += (a[i] & 0xFFFFFFFFL) - (b[i] & 0xFFFFFFFFL);
				res[i] = (int)borrow;
				borrow >>= 32;
			}
			for (; i < aSize; i++) {
				borrow += a[i] & 0xFFFFFFFFL;
				res[i] = (int)borrow;
				borrow >>= 32;
			}
		}

		/// <summary>
		/// Adds the value represented by <paramref name="b"/> to the value represented by
		/// <paramref name="a"/>. It is assumed the magnitude of <paramref name="a"/> is not
		/// less than the magnitude of <paramref name="b"/>.
		/// </summary>
		/// <param name="a">The first addend array.</param>
		/// <param name="aSize">The number of elements in <paramref name="a"/>.</param>
		/// <param name="b">The second addend array.</param>
		/// <param name="bSize">The number of elements in <paramref name="b"/>.</param>
		/// <returns><c>a + b</c> as a new array.</returns>
		private static int[] add(ReadOnlySpan<int> a, int aSize, ReadOnlySpan<int> b, int bSize) {
			int[] res = new int[aSize + 1];
			add(res, a, aSize, b, bSize);
			return res;
		}

		/// <summary>
		/// Performs <c>op1 += op2</c>. <paramref name="op1"/> must have enough place to store
		/// the result (i.e. <c>op1.bitLength() &gt;= op2.bitLength()</c>). Both
		/// should be positive (i.e. <c>op1 &gt;= op2</c>).
		/// </summary>
		/// <param name="op1">The input minuend and the output result.</param>
		/// <param name="op2">The addend.</param>
		internal static void inplaceAdd(BigInteger op1, BigInteger op2) {
			add(op1.Digits.AsSpan(), op1.Digits.AsSpan(0, op1.numberLength), op1.numberLength, op2.Digits.AsSpan(0, op2.numberLength),
					op2.numberLength);
			op1.numberLength = System.Math.Min(System.Math.Max(op1.numberLength, op2.numberLength) + 1, op1.Digits.Length);
			op1.CutOffLeadingZeroes();
			op1.UnCache();
		}

		/// <summary>
		/// Adds an integer value to the array of integers remembering carry.
		/// </summary>
		/// <param name="a">The array of integers.</param>
		/// <param name="aSize">The number of elements to process.</param>
		/// <param name="addend">The value to add.</param>
		/// <returns>A possible generated carry (0 or 1).</returns>
		internal static int inplaceAdd(Span<int> a, int aSize, int addend) {
			long carry = addend & 0xFFFFFFFFL;

			for (int i = 0; (carry != 0) && (i < aSize); i++) {
				carry += a[i] & 0xFFFFFFFFL;
				a[i] = (int)carry;
				carry >>= 32;
			}
			return (int)carry;
		}

		/// <summary>
		/// Performs <c>op1 += addend</c>. The number must have place to hold a possible carry.
		/// </summary>
		/// <param name="op1">The big integer to modify.</param>
		/// <param name="addend">The integer to add.</param>
		internal static void inplaceAdd(BigInteger op1, int addend) {
			int carry = inplaceAdd(op1.Digits, op1.numberLength, addend);
			if (carry == 1) {
				op1.Digits[op1.numberLength] = 1;
				op1.numberLength++;
			}
			op1.UnCache();
		}

		/// <summary>
		/// Performs <c>op1 -= op2</c>. <paramref name="op1"/> must have enough place to store
		/// the result (i.e. <c>op1.bitLength() &gt;= op2.bitLength()</c>). Both
		/// should be positive (what implies that <c>op1 &gt;= op2</c>).
		/// </summary>
		/// <param name="op1">The input minuend and the output result.</param>
		/// <param name="op2">The subtrahend.</param>
		internal static void inplaceSubtract(BigInteger op1, BigInteger op2) {
			subtract(op1.Digits.AsSpan(), op1.Digits.AsSpan(0, op1.numberLength), op1.numberLength, op2.Digits.AsSpan(0, op2.numberLength),
					op2.numberLength);
			op1.CutOffLeadingZeroes();
			op1.UnCache();
		}

		/// <summary>
		/// Performs <c>res = b - a</c>.
		/// </summary>
		/// <param name="res">The result array.</param>
		/// <param name="a">The first array.</param>
		/// <param name="aSize">The number of elements in <paramref name="a"/>.</param>
		/// <param name="b">The second array.</param>
		/// <param name="bSize">The number of elements in <paramref name="b"/>.</param>
		private static void inverseSubtract(Span<int> res, ReadOnlySpan<int> a, int aSize, ReadOnlySpan<int> b, int bSize) {
			int i;
			long borrow = 0;
			if (aSize < bSize) {
				for (i = 0; i < aSize; i++) {
					borrow += (b[i] & 0xFFFFFFFFL) - (a[i] & 0xFFFFFFFFL);
					res[i] = (int)borrow;
					borrow >>= 32;
				}
				for (; i < bSize; i++) {
					borrow += b[i] & 0xFFFFFFFFL;
					res[i] = (int)borrow;
					borrow >>= 32;
				}
			} else {
				for (i = 0; i < bSize; i++) {
					borrow += (b[i] & 0xFFFFFFFFL) - (a[i] & 0xFFFFFFFFL);
					res[i] = (int)borrow;
					borrow >>= 32;
				}
				for (; i < aSize; i++) {
					borrow -= a[i] & 0xFFFFFFFFL;
					res[i] = (int)borrow;
					borrow >>= 32;
				}
			}

		}

		/// <summary>
		/// Subtracts the value represented by <paramref name="b"/> from the value represented
		/// by <paramref name="a"/>. It is assumed the magnitude of <paramref name="a"/> is not
		/// less than the magnitude of <paramref name="b"/>.
		/// </summary>
		/// <param name="a">The minuend array.</param>
		/// <param name="aSize">The number of elements in <paramref name="a"/>.</param>
		/// <param name="b">The subtrahend array.</param>
		/// <param name="bSize">The number of elements in <paramref name="b"/>.</param>
		/// <returns><c>a - b</c> as a new array.</returns>
		private static int[] subtract(ReadOnlySpan<int> a, int aSize, ReadOnlySpan<int> b, int bSize) {
			int[] res = new int[aSize];
			subtract(res, a, aSize, b, bSize);
			return res;
		}

		/// <summary>
		/// Same as <see cref="inplaceSubtract(BigInteger, BigInteger)"/> but without the
		/// restriction of non-positive values.
		/// </summary>
		/// <param name="op1">Should have enough space to save the result.</param>
		/// <param name="op2">The number to subtract.</param>
		internal static void completeInPlaceSubtract(BigInteger op1, BigInteger op2) {
			int resultSign = op1.CompareTo(op2);
			if (op1.Sign == 0) {
				Array.Copy(op2.Digits, 0, op1.Digits, 0, op2.numberLength);
				op1.Sign = -op2.Sign;
			} else if (op1.Sign != op2.Sign) {
				add(op1.Digits.AsSpan(), op1.Digits.AsSpan(0, op1.numberLength), op1.numberLength, op2.Digits.AsSpan(0, op2.numberLength),
					op2.numberLength);
				op1.Sign = resultSign;
			} else {
				int sign = unsignedArraysCompare(op1.Digits,
						op2.Digits, op1.numberLength, op2.numberLength);
				if (sign > 0) {
					subtract(op1.Digits.AsSpan(), op1.Digits.AsSpan(0, op1.numberLength), op1.numberLength, op2.Digits.AsSpan(0, op2.numberLength),
							op2.numberLength);
				} else {
					inverseSubtract(op1.Digits.AsSpan(), op1.Digits.AsSpan(0, op1.numberLength), op1.numberLength,
							op2.Digits.AsSpan(0, op2.numberLength), op2.numberLength);
					op1.Sign = -op1.Sign;
				}
			}
			op1.numberLength = System.Math.Max(op1.numberLength, op2.numberLength) + 1;
			op1.CutOffLeadingZeroes();
			op1.UnCache();
		}

		/// <summary>
		/// Same as <see cref="inplaceAdd(BigInteger, BigInteger)"/> but without the
		/// restriction of non-positive values.
		/// </summary>
		/// <param name="op1">Any number.</param>
		/// <param name="op2">Any number.</param>
		internal static void completeInPlaceAdd(BigInteger op1, BigInteger op2) {
			if (op1.Sign == 0)
				Array.Copy(op2.Digits, 0, op1.Digits, 0, op2.numberLength);
			else if (op2.Sign == 0)
				return;
			else if (op1.Sign == op2.Sign)
				add(op1.Digits.AsSpan(), op1.Digits.AsSpan(0, op1.numberLength), op1.numberLength, op2.Digits.AsSpan(0, op2.numberLength),
						op2.numberLength);
			else {
				int sign = unsignedArraysCompare(op1.Digits,
						op2.Digits, op1.numberLength, op2.numberLength);
				if (sign > 0)
					subtract(op1.Digits.AsSpan(), op1.Digits.AsSpan(0, op1.numberLength), op1.numberLength, op2.Digits.AsSpan(0, op2.numberLength),
							op2.numberLength);
				else {
					inverseSubtract(op1.Digits.AsSpan(), op1.Digits.AsSpan(0, op1.numberLength), op1.numberLength,
							op2.Digits.AsSpan(0, op2.numberLength), op2.numberLength);
					op1.Sign = -op1.Sign;
				}
			}
			op1.numberLength = System.Math.Max(op1.numberLength, op2.numberLength) + 1;
			op1.CutOffLeadingZeroes();
			op1.UnCache();
		}

		/// <summary>
		/// Compares two arrays, representing unsigned integers in little-endian order.
		/// Returns +1, 0, or -1 if <paramref name="a"/> is greater, equal, or lesser than <paramref name="b"/>.
		/// </summary>
		/// <param name="a">The first array.</param>
		/// <param name="b">The second array.</param>
		/// <param name="aSize">The number of elements in <paramref name="a"/>.</param>
		/// <param name="bSize">The number of elements in <paramref name="b"/>.</param>
		/// <returns>1 if a &gt; b, -1 if a &lt; b, 0 if a == b.</returns>
		private static int unsignedArraysCompare(ReadOnlySpan<int> a, ReadOnlySpan<int> b, int aSize, int bSize) {
			if (aSize > bSize)
				return 1;
			else if (aSize < bSize)
				return -1;

			else {
				int i;
				for (i = aSize - 1; i >= 0 && a[i] == b[i]; i--)
					;
				return i < 0 ? BigInteger.EQUALS : ((a[i] & 0xFFFFFFFFL) < (b[i] & 0xFFFFFFFFL) ? BigInteger.LESS
							: BigInteger.GREATER);
			}
		}


	}
}
