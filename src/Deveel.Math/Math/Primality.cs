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
	/// Provides primality probabilistic methods for <see cref="BigInteger"/>.
	/// </summary>
	static class Primality {

		/// <summary>
		/// All prime numbers with bit length less than 10 bits.
		/// </summary>
		private static readonly int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
            31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101,
            103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167,
            173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239,
            241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313,
            317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397,
            401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467,
            479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 563, 569,
            571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643,
            647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733,
            739, 743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823,
            827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911,
            919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997, 1009,
            1013, 1019, 1021 };

		/// <summary>
		/// All <see cref="BigInteger"/> prime numbers with bit length less than 8 bits.
		/// </summary>
		private static readonly BigInteger[] BIprimes = new BigInteger[primes.Length];

		/// <summary>
		/// Encodes how many iterations of Miller-Rabin test are needed to get an
		/// error bound not greater than 2<sup>(-100)</sup>. For example:
		/// for a 1000-bit number we need 4 iterations, since
		/// <c>BITS[3] &lt; 1000 &lt;= BITS[4]</c>.
		/// </summary>
		private static readonly int[] BITS = { 0, 0, 1854, 1233, 927, 747, 627, 543,
            480, 431, 393, 361, 335, 314, 295, 279, 265, 253, 242, 232, 223,
            216, 181, 169, 158, 150, 145, 140, 136, 132, 127, 123, 119, 114,
            110, 105, 101, 96, 92, 87, 83, 78, 73, 69, 64, 59, 54, 49, 44, 38,
            32, 26, 1 };

		/// <summary>
		/// Encodes how many i-bit primes there are in the table for
		/// <c>i = 2, ..., 10</c>. For example <c>offsetPrimes[6]</c> says that from
		/// index 11 there are 7 consecutive 6-bit prime numbers in the array.
		/// </summary>
		private static readonly int[][] offsetPrimes;

		static Primality() {
			for (int i = 0; i < primes.Length; i++) {
				BIprimes[i] = BigInteger.FromInt64(primes[i]);
			}

			offsetPrimes = new int[11][];
			offsetPrimes[0] = null;
			offsetPrimes[1] = null;
			offsetPrimes[2] = new int[] {0, 2};
			offsetPrimes[3] = new int[] {2, 2};
			offsetPrimes[4] = new int[] {4, 2};
			offsetPrimes[5] = new int[] {6, 5};
			offsetPrimes[6] = new int[] {11, 7};
			offsetPrimes[7] = new int[] {18, 13};
			offsetPrimes[8] = new int[] {31, 23};
			offsetPrimes[9] = new int[] {54, 43};
			offsetPrimes[10] = new int[] {97, 75};
		}

		/// <summary>
		/// Finds the next probable prime greater than or equal to <paramref name="n"/>.
		/// Uses the sieve of Eratosthenes to discard composite numbers in the range
		/// <c>[n, n + 1024]</c>, then applies the Miller-Rabin test.
		/// </summary>
		/// <param name="n">The starting point.</param>
		/// <returns>The next probable prime.</returns>
		/// <example>
		/// <code>
		/// BigInteger n = new BigInteger(18);
		/// BigInteger next = Primality.NextProbablePrime(n);
		/// // next == 19
		/// </code>
		/// </example>
		public static BigInteger NextProbablePrime(BigInteger n) {
			int i, j;
			int certainty;
			int gapSize = 1024;
			int[] modules = new int[primes.Length];
			bool[] isDivisible = new bool[gapSize];
			BigInteger startPoint;
			BigInteger probPrime;
			if ((n.numberLength == 1) && (n.Digits[0] >= 0)
					&& (n.Digits[0] < primes[primes.Length - 1])) {
				for (i = 0; n.Digits[0] >= primes[i]; i++) {
					;
				}
				return BIprimes[i];
			}
			startPoint = new BigInteger(1, n.numberLength,
					new int[n.numberLength + 1]);
			Array.Copy(n.Digits, 0, startPoint.Digits, 0, n.numberLength);
			if (BigInteger.TestBit(n, 0)) {
				Elementary.inplaceAdd(startPoint, 2);
			} else {
				startPoint.Digits[0] |= 1;
			}
			j = startPoint.BitLength;
			for (certainty = 2; j < BITS[certainty]; certainty++) {
				;
			}
			for (i = 0; i < primes.Length; i++) {
				modules[i] = Division.Remainder(startPoint, primes[i]) - gapSize;
			}
			while (true) {
				for (int k = 0; k < isDivisible.Length; k++)
					isDivisible[k] = false;

				for (i = 0; i < primes.Length; i++) {
					modules[i] = (modules[i] + gapSize) % primes[i];
					j = (modules[i] == 0) ? 0 : (primes[i] - modules[i]);
					for (; j < gapSize; j += primes[i]) {
						isDivisible[j] = true;
					}
				}
				for (j = 0; j < gapSize; j++) {
					if (!isDivisible[j]) {
						probPrime = startPoint.Copy();
						Elementary.inplaceAdd(probPrime, j);

						if (MillerRabin(probPrime, certainty)) {
							return probPrime;
						}
					}
				}
				Elementary.inplaceAdd(startPoint, gapSize);
			}
		}

		/// <summary>
		/// Generates a random probable prime of the specified bit length with the given certainty.
		/// </summary>
		/// <param name="bitLength">The bit length of the prime (must be &gt;= 2).</param>
		/// <param name="certainty">The desired certainty level.</param>
		/// <param name="rnd">The random number generator.</param>
		/// <returns>A probable prime <see cref="BigInteger"/>.</returns>
		/// <example>
		/// <code>
		/// Random rnd = new Random();
		/// BigInteger prime = Primality.ConsBigInteger(128, 10, rnd);
		/// // prime is a 128-bit probable prime
		/// </code>
		/// </example>
		public static BigInteger ConsBigInteger(int bitLength, int certainty, Random rnd) {
        if (bitLength <= 10) {
            int[] rp = offsetPrimes[bitLength];
            return BIprimes[rp[0] + rnd.Next(rp[1])];
        }
        int shiftCount = (-bitLength) & 31;
        int last = (bitLength + 31) >> 5;
        BigInteger n = new BigInteger(1, last, new int[last]);

        last--;
        do {
            for (int i = 0; i < n.numberLength; i++) {
                n.Digits[i] = rnd.Next();
            }
			n.Digits[last] |= Int32.MinValue;
            n.Digits[last] = Utils.URShift(n.Digits[last], shiftCount);
            n.Digits[0] |= 1;
        } while (!IsProbablePrime(n, certainty));
        return n;
    }

		/// <summary>
		/// Tests whether <paramref name="n"/> is a probable prime with the specified
		/// certainty.
		/// </summary>
		/// <param name="n">The number to test.</param>
		/// <param name="certainty">The desired certainty level.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="n"/> is probably prime, <c>false</c> otherwise.
		/// </returns>
		/// <example>
		/// <code>
		/// BigInteger n = new BigInteger(17);
		/// bool isPrime = Primality.IsProbablePrime(n, 10);
		/// // isPrime == true
		/// </code>
		/// </example>
		public static bool IsProbablePrime(BigInteger n, int certainty) {
			if ((certainty <= 0) || ((n.numberLength == 1) && (n.Digits[0] == 2))) {
				return true;
			}
			if (!BigInteger.TestBit(n, 0)) {
				return false;
			}
			if ((n.numberLength == 1) && ((n.Digits[0] & 0XFFFFFC00) == 0)) {
				return (Array.BinarySearch(primes, n.Digits[0]) >= 0);
			}
			for (int j = 1; j < primes.Length; j++) {
				if (Division.RemainderArrayByInt(n.Digits, n.numberLength, primes[j]) == 0) {
					return false;
				}
			}
			int i;
			int bitLength = n.BitLength;

			for (i = 2; bitLength < BITS[i]; i++) {
				;
			}
			certainty = System.Math.Min(i, 1 + ((certainty - 1) >> 1));

			return MillerRabin(n, certainty);
		}

		/// <summary>
		/// The Miller-Rabin primality test.
		/// </summary>
		/// <param name="n">The input number to be tested.</param>
		/// <param name="t">The number of trials.</param>
		/// <returns>
		/// <c>false</c> if the number is definitely composite, otherwise
		/// <c>true</c> with probability 1 - 4<sup>(-t)</sup>.
		/// </returns>
		private static bool MillerRabin(BigInteger n, int t) {
			BigInteger x;
			BigInteger y;
			BigInteger n_minus_1 = n - BigInteger.One;
			int bitLength = n_minus_1.BitLength;
			int k = n_minus_1.LowestSetBit;
			BigInteger q = n_minus_1 >> k;
			Random rnd = new Random();

			for (int i = 0; i < t; i++) {
				if (i < primes.Length) {
					x = BIprimes[i];
				} else {
					do {
						x = new BigInteger(bitLength, rnd);
					} while ((x.CompareTo(n) >= BigInteger.EQUALS) || (x.Sign == 0)
							|| x.IsOne);
				}
				y = BigMath.ModPow(x, q, n);
				if (y.IsOne || y.Equals(n_minus_1)) {
					continue;
				}
				if (WitnessLoop(y, k, n)) return false;
			}
			return true;
		}

		private static bool WitnessLoop(BigInteger y, int k, BigInteger n) {
			BigInteger n_minus_1 = n - BigInteger.One;
			for (int j = 1; j < k; j++) {
				if (y.Equals(n_minus_1)) {
					return false;
				}
				y = (y * y) % n;
				if (y.IsOne) {
					return true;
				}
			}
			return !y.Equals(n_minus_1);
		}

	}
}
