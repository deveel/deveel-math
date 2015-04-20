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
using System.Collections.Generic;

namespace Deveel.Math {
	public sealed class Prime {
		private List<BigInteger> numbers = new List<BigInteger>();

		private static BigInteger NMax = BigInteger.ValueOf(-1);

		public Prime() {
			if (numbers.Count == 0) {
				numbers = new List<BigInteger>();
				numbers.Add(BigInteger.ValueOf(2));
				numbers.Add(BigInteger.ValueOf(3));
				numbers.Add(BigInteger.ValueOf(5));
				numbers.Add(BigInteger.ValueOf(7));
				numbers.Add(BigInteger.ValueOf(11));
				numbers.Add(BigInteger.ValueOf(13));
				numbers.Add(BigInteger.ValueOf(17));
			}

			NMax = numbers[numbers.Count - 1];
		}

		public BigInteger this[int i] {
			get {
				/* If the current list is too small, increase in intervals
* of 5 until the list has at least i elements.
*/
				while (i >= numbers.Count) {
					GrowTo(NMax.Add(BigInteger.ValueOf(5)));
				}
				return (numbers[i]);
			}
		}

		private void GrowTo(BigInteger n) {
			while (NMax.CompareTo(n) == -1) {
				NMax = NMax.Add(BigInteger.One);
				bool isp = true;
				for (int p = 0; p < numbers.Count; p++) {
					/*
					* Test the list of known primes only up to sqrt(n)
					*/
					if (numbers[p].Multiply(numbers[p]).CompareTo(NMax) == 1)
						break;

					/*
					* The next case means that the p'th number in the list of known primes divides
					* nMax and nMax cannot be a prime.
					*/
					if (NMax.Remainder(numbers[p]).CompareTo(BigInteger.Zero) == 0) {
						isp = false;
						break;
					}
				}
				if (isp)
					numbers.Add(NMax);
			}
		}

		public bool Contains(BigInteger n) {
			switch (MillerRabin(n)) {
				case -1:
					return false;
				case 1:
					return true;
			}
			GrowTo(n);
			return (numbers.Contains(n));
		}

		public bool IsSpp(BigInteger n, BigInteger a) {
			BigInteger two = BigInteger.Parse("" + 2);


			/* numbers less than 2 are not prime 
                */
			if (n.CompareTo(two) == -1)
				return false;
			/* 2 is prime 
                */
			if (n.CompareTo(two) == 0)
				return true;
			/* even numbers >2 are not prime
                */
			if (n.Remainder(two).CompareTo(BigInteger.Zero) == 0)
				return false;

			/* q= n- 1 = d *2^s with d odd
                        */
			BigInteger q = n.Subtract(BigInteger.One);
			int s = q.LowestSetBit;
			BigInteger d = q.ShiftRight(s);

			/* test whether a^d = 1 (mod n)
                        */
			if (a.ModPow(d, n).CompareTo(BigInteger.One) == 0)
				return true;

			/* test whether a^(d*2^r) = -1 (mod n), 0<=r<s 
                        */
			for (int r = 0; r < s; r++) {
				if (a.ModPow(d.ShiftLeft(r), n).CompareTo(q) == 0)
					return true;
			}
			return false;
		}

		public int MillerRabin(BigInteger n) {
			/* list of limiting numbers which fail tests on k primes, A014233 in the OEIS
                */
			string[] mr = {
				"2047", "1373653", "25326001", "3215031751", "2152302898747", "3474749660383",
				"341550071728321"
			};
			int mrLim = 0;
			while (mrLim < mr.Length) {
				int l = n.CompareTo(BigInteger.Parse(mr[mrLim]));
				if (l < 0)
					break;
					/* if one of the pseudo-primes: this is a composite
                        */
				else if (l == 0)
					return -1;
				mrLim++;
			}
			/* cannot test candidates larger than the last in the mr list
                */
			if (mrLim == mr.Length)
				return 0;

			/* test the bases prime(1), prime(2) up to prime(mrLim+1)
                */
			for (int p = 0; p <= mrLim; p++)
				if (IsSpp(n, this[p]) == false)
					return -1;
			return 1;
		}

		public BigInteger Pi(BigInteger n) {
			/* If the current list is too small, increase in intervals
			* of 5 until the list has at least i elements.
			*/
			GrowTo(n);
			BigInteger r = BigInteger.ValueOf(0);
			for (int i = 0; i < numbers.Count; i++)
				if (numbers[i].CompareTo(n) <= 0)
					r = r.Add(BigInteger.One);
			return r;
		}

		public BigInteger NextPrime(BigInteger n) {
			/* if n <=1, return 2 */
			if (n.CompareTo(BigInteger.One) <= 0)
				return (numbers[0]);

			/* If the currently largest element in the list is too small, increase in intervals
			* of 5 until the list has at least i elements.
			*/
			while (numbers[numbers.Count - 1].CompareTo(n) <= 0) {
				GrowTo(NMax.Add(BigInteger.ValueOf(5)));
			}
			for (int i = 0; i < numbers.Count; i++)
				if (numbers[i].CompareTo(n) == 1)
					return numbers[i];

			return numbers[numbers.Count - 1];
		}

		public BigInteger PreviousPrime(BigInteger n) {
			/* if n <=2, return 0 */
			if (n.CompareTo(BigInteger.One) <= 0)
				return BigInteger.Zero;

			/* If the currently largest element in the list is too small, increase in intervals
			* of 5 until the list has at least i elements.
			*/
			while (numbers[numbers.Count - 1].CompareTo(n) < 0)
				GrowTo(NMax.Add(BigInteger.ValueOf(5)));

			for (int i = 0; i < numbers.Count; i++)
				if (numbers[i].CompareTo(n) >= 0)
					return numbers[i - 1];
			return numbers[numbers.Count - 1];
		}
	}
}