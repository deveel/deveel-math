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
	public sealed class Bernoulli {
		private readonly List<Rational> numbers = new List<Rational>();

		public static readonly Bernoulli Default = new Bernoulli();

		public Bernoulli() {
			if (numbers.Count == 0) {
				numbers.Add(Rational.One);
				numbers.Add(new Rational(1, 6));
			}
		}

		public Rational this[int index] {
			get {
				if (index == 1)
					return (new Rational(-1, 2));
				if (index%2 != 0)
					return Rational.Zero;
				int nindx = index/2;
				if (numbers.Count <= nindx) {
					for (int i = 2*numbers.Count; i <= index; i += 2)
						this[i] = DoubleSum(i);
				}
				return numbers[nindx];
			}
			set {
				int nindx = index/2;
				if (nindx < numbers.Count)
					numbers[nindx] = value;
				else {
					while (numbers.Count < nindx)
						numbers.Add(Rational.Zero);
					numbers.Add(value);
				}
			}
		}

		private Rational DoubleSum(int n) {
			Rational resul = Rational.Zero;
			for (int k = 0; k <= n; k++) {
				Rational jsum = Rational.Zero;
				BigInteger bin = BigInteger.One;
				for (int j = 0; j <= k; j++) {
					BigInteger jpown = (BigInteger.ValueOf(j)).Pow(n);
					if (j%2 == 0)
						jsum = jsum.Add(bin.Multiply(jpown));
					else
						jsum = jsum.Subtract(bin.Multiply(jpown));

					/* update binomial(k,j) recursively
					*/
					bin = bin.Multiply(BigInteger.ValueOf(k - j)).Divide(BigInteger.ValueOf(j + 1));
				}
				resul = resul.Add(jsum.Divide(BigInteger.ValueOf(k + 1)));
			}
			return resul;
		}
	}
}