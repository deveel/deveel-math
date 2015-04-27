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
using System.Collections.ObjectModel;

namespace Deveel.Math {
	public sealed class IFactor : IComparable<IFactor>, IComparable {
		private readonly IList<int> primeexp;

		public static readonly IFactor One = new IFactor(1);

		public static readonly IFactor Zero = new IFactor(0);

		public IFactor(int number) {
			Number = BigInteger.Parse("" + number);
			primeexp = new List<int>();

			if (number > 1) {
				int primindx = 0;
				Prime primes = new Prime();
				/* Test division against all primes.
                        */
				while (number > 1) {
					int ex = 0;
					/* primindx=0 refers to 2, =1 to 3, =2 to 5, =3 to 7 etc
                                */
					int p = primes[primindx];
					while (number%p == 0) {
						ex++;
						number /= p;
						if (number == 1)
							break;
					}
					if (ex > 0) {
						primeexp.Add(p);
						primeexp.Add(ex);
					}
					primindx++;
				}
			} else if (number == 1) {
				primeexp.Add(1);
				primeexp.Add(0);
			}
		}

		public IFactor(BigInteger number) {
			Number = number;
			primeexp = new List<int>();
			if (number.CompareTo(BigInteger.One) == 0) {
				primeexp.Add(1);
				primeexp.Add(0);
			} else {
				int primindx = 0;
				Prime primes = new Prime();
				/* Test for division against all primes.
                        */
				while (number.CompareTo(BigInteger.One) == 1) {
					int ex = 0;
					BigInteger p = primes[primindx];
					while (number.Remainder(p).CompareTo(BigInteger.Zero) == 0) {
						ex++;
						number = number.Divide(p);
						if (number.CompareTo(BigInteger.One) == 0)
							break;
					}
					if (ex > 0) {
						primeexp.Add(p);
						primeexp.Add(ex);
					}
					primindx++;
				}
			}
		}

		public IEnumerable<int> PrimeExp {
			get { return new ReadOnlyCollection<int>(primeexp); }
		}

		public BigInteger Number { get; private set; }

		public int CompareTo(IFactor other) {
			throw new NotImplementedException();
		}

		public int CompareTo(object obj) {
			throw new NotImplementedException();
		}

		public IFactor Multiply(int oth) {
			/* the optimization is to factorize oth _before_ multiplying
                */
			return (Multiply(new IFactor(oth)));
		}

		public IFactor Multiply(IFactor oth) {
			/* This might be done similar to the lcm() implementation by adding
                * the powers of the components and calling the constructor with the
                * list of exponents. This here is the simplest implementation, but slow because
                * it calls another prime factorization of the product:
                * return( new Ifactor(n.multiply(oth.n))) ;
                */
			return MultiplyGcdLcm(oth, 0);
		}

		private IFactor MultiplyGcdLcm(IFactor oth, int type) {
			IFactor prod = new IFactor(0);
			/* skip the case where 0*something =0, falling thru to the empty representation for 0
                */
			if (primeexp.Count != 0 && oth.primeexp.Count != 0) {
				/* Cases of 1 times something return something.
                        * Cases of lcm(1, something) return something.
                        * Cases of gcd(1, something) return 1.
                        */
				if (primeexp[0] == 1 && type == 0)
					return oth;
				if (primeexp[0] == 1 && type == 2)
					return oth;
				if (primeexp[0] == 1 && type == 1)
					return this;
				if (oth.primeexp[0] == 1 && type == 0)
					return this;
				if (oth.primeexp[0] == 1 && type == 2)
					return this;
				else if (oth.primeexp[0] == 1 && type == 1)
					return oth;
				else {
					int idxThis = 0;
					int idxOth = 0;
					switch (type) {
						case 0:
							prod.Number = Number.Multiply(oth.Number);
							break;
						case 1:
							prod.Number = Number.Gcd(oth.Number);
							break;
						case 2:
							/* the awkward way, lcm = product divided by gcd
                                        */
							prod.Number = Number.Multiply(oth.Number).Divide(Number.Gcd(oth.Number));
							break;
					}

					/* scan both representations left to right, increasing prime powers
                                */
					while (idxOth < oth.primeexp.Count || idxThis < primeexp.Count) {
						if (idxOth >= oth.primeexp.Count) {
							/* exhausted the list in oth.primeexp; copy over the remaining 'this'
                                                * if multiplying or lcm, discard if gcd.
                                                */
							if (type == 0 || type == 2) {
								prod.primeexp.Add(primeexp[idxThis]);
								prod.primeexp.Add(primeexp[idxThis + 1]);
							}
							idxThis += 2;
						} else if (idxThis >= primeexp.Count) {
							/* exhausted the list in primeexp; copy over the remaining 'oth'
                                                */
							if (type == 0 || type == 2) {
								prod.primeexp.Add(oth.primeexp[idxOth]);
								prod.primeexp.Add(oth.primeexp[idxOth + 1]);
							}
							idxOth += 2;
						} else {
							int p;
							int ex;
							switch (primeexp[idxThis].CompareTo(oth.primeexp[idxOth])) {
								case 0:
									/* same prime bases p in both factors */
									p = primeexp[idxThis];
									switch (type) {
										case 0:
											/* product means adding exponents */
											ex = primeexp[idxThis + 1] + oth.primeexp[idxOth + 1];
											break;
										case 1:
											/* gcd means minimum of exponents */
											ex = System.Math.Min(primeexp[idxThis + 1], oth.primeexp[idxOth + 1]);
											break;
										default:
											/* lcm means maximum of exponents */
											ex = System.Math.Max(primeexp[idxThis + 1], oth.primeexp[idxOth + 1]);
											break;
									}
									prod.primeexp.Add(p);
									prod.primeexp.Add(ex);
									idxOth += 2;
									idxThis += 2;
									break;
								case 1:
									/* this prime base bigger than the other and taken later */
									if (type == 0 || type == 2) {
										prod.primeexp.Add(oth.primeexp[idxOth]);
										prod.primeexp.Add(oth.primeexp[idxOth + 1]);
									}
									idxOth += 2;
									break;
								default:
									/* this prime base smaller than the other and taken now */
									if (type == 0 || type == 2) {
										prod.primeexp.Add(primeexp[idxThis]);
										prod.primeexp.Add(primeexp[idxThis + 1]);
									}
									idxThis += 2;
									break;
							}
						}
					}
				}
			}
			return prod;
		}

		public Rational Root(int r) {
			if (r == 0)
				throw new ArithmeticException("Cannot pull zeroth root of " + ToString());
			else if (r < 0) {
				/* a^(-1/b)= 1/(a^(1/b))
                        */
				Rational invRoot = Root(-r);
				return Rational.One.Divide(invRoot);
			} else {
				BigInteger pows = BigInteger.One;
				for (int i = 0; i < primeexp.Count; i += 2) {
					/* all exponents must be multiples of r to succeed (that is, to
                                * stay in the range of rational results).
                                */
					int ex = primeexp[i + 1];
					if (ex%r != 0)
						throw new ArithmeticException("Cannot pull " + r + "th root of " + ToString());

					pows.Multiply(BigInteger.ValueOf(primeexp[i]).Pow(ex/r));
				}
				/* convert result to a Rational; unfortunately this will loose the prime factorization */
				return new Rational(pows);
			}
		}
	}
}