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

namespace Deveel.Math {
	public struct BigComplex {
		public BigComplex(BigDecimal real)
			: this(real, BigDecimal.Zero) {
		}

		public BigComplex(BigDecimal real, BigDecimal imaginary)
			: this() {
			Real = real;
			Imaginary = imaginary;
		}

		public BigComplex(double x, double y)
			: this(new BigDecimal(x), new BigDecimal(y)) {
		}

		public BigDecimal Real { get; private set; }

		public BigDecimal Imaginary { get; private set; }

		public BigComplex Multiply(BigComplex oth, MathContext mc) {
			BigDecimal a = Real.Add(Imaginary).Multiply(oth.Real);
			BigDecimal b = oth.Real.Add(oth.Imaginary).Multiply(Imaginary);
			BigDecimal c = oth.Imaginary.Subtract(oth.Real).Multiply(Real);
			BigDecimal x = a.Subtract(b, mc);
			BigDecimal y = a.Add(c, mc);
			return new BigComplex(x, y);
		}

		public BigComplex Divide(BigComplex oth, MathContext mc) {
			/* lazy implementation: (x+iy)/(a+ib)= (x+iy)* 1/(a+ib) */
			return Multiply(oth.Inverse(mc), mc);
		}

		public BigComplex Add(BigDecimal oth) {
			BigDecimal x = Real.Add(oth);
			return new BigComplex(x, Imaginary);
		}

		public BigComplex Subtract(BigComplex oth) {
			BigDecimal x = Real.Subtract(oth.Real);
			BigDecimal y = Imaginary.Subtract(oth.Imaginary);
			return new BigComplex(x, y);
		}

		public BigComplex Inverse(MathContext mc) {
			BigDecimal hyp = Norm();
			/* 1/(x+iy)= (x-iy)/(x^2+y^2 */
			return new BigComplex(Real.Divide(hyp, mc), Imaginary.Divide(hyp, mc).Negate());
		}

		public BigDecimal Norm() {
			return Real.Multiply(Real).Add(Imaginary.Multiply(Imaginary));
		}

		public BigDecimal Abs(MathContext mc) {
			return BigMath.Sqrt(Norm(), mc);
		}

		public BigComplex Sqrt(MathContext mc) {
			BigDecimal half = new BigDecimal(2);
			/* compute l=sqrt(re^2+im^2), then u=sqrt((l+re)/2)
                * and v= +- sqrt((l-re)/2 as the new real and imaginary parts.
                */
			BigDecimal l = Abs(mc);
			if (l.CompareTo(BigDecimal.Zero) == 0)
				return new BigComplex(BigMath.ScalePrecision(BigDecimal.Zero, mc),
					BigMath.ScalePrecision(BigDecimal.Zero, mc));
			BigDecimal u = BigMath.Sqrt(l.Add(Real).Divide(half, mc), mc);
			BigDecimal v = BigMath.Sqrt(l.Subtract(Real).Divide(half, mc), mc);
			if (Imaginary.CompareTo(BigDecimal.Zero) >= 0)
				return new BigComplex(u, v);
			else
				return new BigComplex(u, v.Negate());
		}

		public BigComplex Conjugation() {
			return new BigComplex(Real, Imaginary.Negate());
		}

		public override string ToString() {
			return String.Format("{0},{1}", Real, Imaginary);
		}
	}
}