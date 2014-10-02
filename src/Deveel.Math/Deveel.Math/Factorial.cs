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
	public sealed class Factorial {
		private readonly List<IFactor> factors = new List<IFactor>();

		public static readonly Factorial Default = new Factorial();

		public Factorial() {
			if (factors.Count == 0) {
				factors.Add(IFactor.One);
				factors.Add(IFactor.One);
			}
		}

		public BigInteger this[int index] {
			get {
				GrowTo(index);
				return factors[index].Number;
			}
		}

		private void GrowTo(int n) {
			/* extend the internal list if needed. Size to be 2 for n<=1, 3 for n<=2 etc.
                */
			while (factors.Count <= n) {
				int lastn = factors.Count - 1;
				var nextn = new IFactor(lastn + 1);
				factors.Add(factors[lastn].Multiply(nextn));
			}
		}
	}
}