using System;

namespace Deveel.Math {
	static class BigDecimalMath {
		private static BigDecimal[] factorialCache = new BigDecimal[100];
		private static readonly BigDecimal Two = new BigDecimal(2);
		private static readonly BigDecimal Three = new BigDecimal(3);
		private static readonly BigDecimal Ten = new BigDecimal(10);

		private static readonly BigDecimal LogOfTwo = BigDecimal.Parse(
				"0.69314718055994530941723212145817656807550013436025525412068000949339362196969471560586332699641868754200148102057068573368552023575813055703267075163507596193072757082837143519030703862389167347112335011536449795523912047517268157493206515552473413952588295045300709532636664265410423915781495204374043038550080194417064167151864471283996817178454695702627163106454615025720740248163777338963855069526066834113727387372292895649354702576265209885969320196505855476470330679365443254763274495125040606943814710468994650622016772042452452961268794654619316517468139267250410380254625965686914419287160829380317271436778265487756648508567407764845146443994046142260319309673540257444607030809608504748663852313818167675143866747664789088143714198549423151997354880375165861275352916610007105355824987941472950929311389715599820565439287170007218085761025236889213244971389320378439353088774825970171559107088236836275898425891853530243634214367061189236789192372314672321720534016492568727477823445353476481149418642386776774406069562657379600867076257199184734022651462837904883062033061144630073719489")
			;

		private static readonly BigDecimal LogOfThree = BigDecimal.Parse(
				"1.0986122886681096913952452369225257046474905578227494517346943336374942932186089668736157548137320887879700290659578657423680042259305198210528018707672774106031627691833813671793736988443609599037425703167959115211455919177506713470549401667755802222031702529468975606901065215056428681380363173732985777823669916547921318181490200301038236301222486527481982259910974524908964580534670088459650857484441190188570876474948670796130858294116021661211840014098255143919487688936798494302255731535329685345295251459213876494685932562794416556941578272310355168866102118469890439943063138255285736466882824988136822800634143910786893251456437510204451627561934973982116941585740535361758900975122233797736969687754354795135712982177017581242122351405810163272465588937249564919185242960796684234647069377237252655082032078333928055892853146873095132606458309184397496822230325765467533311823019649275257599132217851353390237482964339502546074245824934666866121881436526565429542767610505477795422933973323401173743193974579847018559548494059478353943841010602930762292228131207489306344534025277732685627")
			;

		private static readonly BigDecimal LogOfTen = BigDecimal.Parse(
				"2.3025850929940456840179914546843642076011014886287729760333279009675726096773524802359972050895982983419677840422862486334095254650828067566662873690987816894829072083255546808437998948262331985283935053089653777326288461633662222876982198867465436674744042432743651550489343149393914796194044002221051017141748003688084012647080685567743216228355220114804663715659121373450747856947683463616792101806445070648000277502684916746550586856935673420670581136429224554405758925724208241314695689016758940256776311356919292033376587141660230105703089634572075440370847469940168269282808481184289314848524948644871927809676271275775397027668605952496716674183485704422507197965004714951050492214776567636938662976979522110718264549734772662425709429322582798502585509785265383207606726317164309505995087807523710333101197857547331541421808427543863591778117054309827482385045648019095610299291824318237525357709750539565187697510374970888692180205189339507238539205144634197265287286965110862571492198849978748873771345686209167058498078280597511938544450099781311469159346662410718466923101075984383191913")
			;

		private const int ExpectedInitialPrecision = 17;

		static BigDecimalMath() {
			BigDecimal result = BigDecimal.One;
			factorialCache[0] = result;
			for (int i = 1; i < factorialCache.Length; i++) {
				result = Multiply(result, i);
				factorialCache[i] = result;
			}
		}

		public static BigDecimal DivideBigIntegers(BigInteger scaledDividend,
			BigInteger scaledDivisor,
			int scale,
			RoundingMode roundingMode) {
			BigInteger remainder;
			BigInteger quotient = BigMath.DivideAndRemainder(scaledDividend, scaledDivisor, out remainder);
			if (remainder.Sign == 0) {
				return new BigDecimal(quotient, scale);
			}

			int sign = scaledDividend.Sign * scaledDivisor.Sign;
			int compRem; // 'compare to remainder'
			if (scaledDivisor.BitLength < 63) {
				// 63 in order to avoid out of long after <<1
				long rem = remainder.ToInt64();
				long divisor = scaledDivisor.ToInt64();
				compRem = BigDecimal.LongCompareTo(System.Math.Abs(rem) << 1, System.Math.Abs(divisor));

				// To look if there is a carry
				compRem = BigDecimal.RoundingBehavior(BigInteger.TestBit(quotient, 0) ? 1 : 0,
					sign * (5 + compRem),
					roundingMode);

			} else {
				// Checking if:  remainder * 2 >= scaledDivisor 
				compRem = BigMath.Abs(remainder).ShiftLeftOneBit().CompareTo(BigMath.Abs(scaledDivisor));
				compRem = BigDecimal.RoundingBehavior(BigInteger.TestBit(quotient, 0) ? 1 : 0,
					sign * (5 + compRem),
					roundingMode);
			}
			if (compRem != 0) {
				if (quotient.BitLength < 63) {
					return BigDecimal.Create(quotient.ToInt64() + compRem, scale);
				}

				quotient += BigInteger.FromInt64(compRem);
				return new BigDecimal(quotient, scale);
			}

			// Constructing the result with the appropriate unscaled value
			return new BigDecimal(quotient, scale);
		}

		public static BigDecimal DividePrimitiveLongs(long scaledDividend,
			long scaledDivisor,
			int scale,
			RoundingMode roundingMode) {
			long quotient = scaledDividend / scaledDivisor;
			long remainder = scaledDividend % scaledDivisor;
			int sign = System.Math.Sign(scaledDividend) * System.Math.Sign(scaledDivisor);
			if (remainder != 0) {
				// Checking if:  remainder * 2 >= scaledDivisor
				int compRem; // 'compare to remainder'
				compRem = BigDecimal.LongCompareTo(System.Math.Abs(remainder) << 1, System.Math.Abs(scaledDivisor));

				// To look if there is a carry
				quotient += BigDecimal.RoundingBehavior(((int) quotient) & 1, sign * (5 + compRem), roundingMode);
			}

			// Constructing the result with the appropriate unscaled value
			return BigDecimal.Create(quotient, scale);
		}

		public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor) {
			BigInteger p = dividend.UnscaledValue;
			BigInteger q = divisor.UnscaledValue;
			BigInteger gcd; // greatest common divisor between 'p' and 'q'
			BigInteger quotient;
			BigInteger remainder;
			long diffScale = (long) dividend.Scale - divisor.Scale;
			int newScale; // the new scale for final quotient
			int k; // number of factors "2" in 'q'
			int l = 0; // number of factors "5" in 'q'
			int i = 1;
			int lastPow = BigDecimal.FivePow.Length - 1;

			if (divisor.IsZero) {
				// math.04=Division by zero
				throw new ArithmeticException(Messages.math04); //$NON-NLS-1$
			}

			if (p.Sign == 0) {
				return BigDecimal.GetZeroScaledBy(diffScale);
			}

			// To divide both by the GCD
			gcd = BigMath.Gcd(p, q);
			p = p / gcd;
			q = q / gcd;

			// To simplify all "2" factors of q, dividing by 2^k
			k = q.LowestSetBit;
			q = q >> k;

			// To simplify all "5" factors of q, dividing by 5^l
			do {
				quotient = BigMath.DivideAndRemainder(q, BigDecimal.FivePow[i], out remainder);
				if (remainder.Sign == 0) {
					l += i;
					if (i < lastPow) {
						i++;
					}
					q = quotient;
				} else {
					if (i == 1) {
						break;
					}

					i = 1;
				}
			} while (true);

			// If  abs(q) != 1  then the quotient is periodic
			if (!BigMath.Abs(q).Equals(BigInteger.One)) {
				// math.05=Non-terminating decimal expansion; no exact representable decimal result.
				throw new ArithmeticException(Messages.math05); //$NON-NLS-1$
			}

			// The sign of the is fixed and the quotient will be saved in 'p'
			if (q.Sign < 0) {
				p = -p;
			}

			// Checking if the new scale is out of range
			newScale = BigDecimal.ToIntScale(diffScale + System.Math.Max(k, l));

			// k >= 0  and  l >= 0  implies that  k - l  is in the 32-bit range
			i = k - l;

			p = (i > 0)
				? Multiplication.MultiplyByFivePow(p, i)
				: p << -i;
			return new BigDecimal(p, newScale);
		}

		public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor, MathContext mc) {
			/* Calculating how many zeros must be append to 'dividend'
			 * to obtain a  quotient with at least 'mc.precision()' digits */
			long traillingZeros = mc.Precision + 2L
			                      + divisor.AproxPrecision() - dividend.AproxPrecision();
			long diffScale = (long) dividend.Scale - divisor.Scale;
			long newScale = diffScale; // scale of the final quotient
			int compRem; // to compare the remainder
			int i = 1; // index   
			int lastPow = BigDecimal.TenPow.Length - 1; // last power of ten
			BigInteger integerQuot; // for temporal results
			BigInteger quotient = dividend.UnscaledValue;
			BigInteger remainder;

			// In special cases it reduces the problem to call the dual method
			if ((mc.Precision == 0) || (dividend.IsZero) || (divisor.IsZero))
				return Divide(dividend, divisor);

			if (traillingZeros > 0) {
				// To append trailing zeros at end of dividend
				quotient = dividend.UnscaledValue * Multiplication.PowerOf10(traillingZeros);
				newScale += traillingZeros;
			}
			quotient = BigMath.DivideAndRemainder(quotient, divisor.UnscaledValue, out remainder);
			integerQuot = quotient;

			// Calculating the exact quotient with at least 'mc.precision()' digits
			if (remainder.Sign != 0) {
				// Checking if:   2 * remainder >= divisor ?
				compRem = remainder.ShiftLeftOneBit().CompareTo(divisor.UnscaledValue);

				// quot := quot * 10 + r;     with 'r' in {-6,-5,-4, 0,+4,+5,+6}
				integerQuot = (integerQuot * BigInteger.Ten) +
				              BigInteger.FromInt64(quotient.Sign * (5 + compRem));
				newScale++;
			} else {
				// To strip trailing zeros until the preferred scale is reached
				while (!BigInteger.TestBit(integerQuot, 0)) {
					quotient = BigMath.DivideAndRemainder(integerQuot, BigDecimal.TenPow[i], out remainder);
					if ((remainder.Sign == 0)
					    && (newScale - i >= diffScale)) {
						newScale -= i;
						if (i < lastPow) {
							i++;
						}
						integerQuot = quotient;
					} else {
						if (i == 1) {
							break;
						}

						i = 1;
					}
				}
			}

			// To perform rounding
			return new BigDecimal(integerQuot, BigDecimal.ToIntScale(newScale), mc);
		}

		public static BigDecimal DivideToIntegralValue(BigDecimal dividend, BigDecimal divisor) {
			BigInteger integralValue; // the integer of result
			BigInteger powerOfTen; // some power of ten
			BigInteger quotient;
			BigInteger remainder;
			long newScale = (long) dividend.Scale - divisor.Scale;
			long tempScale = 0;
			int i = 1;
			int lastPow = BigDecimal.TenPow.Length - 1;

			if (divisor.IsZero) {
				// math.04=Division by zero
				throw new ArithmeticException(Messages.math04); //$NON-NLS-1$
			}

			if ((divisor.AproxPrecision() + newScale > dividend.AproxPrecision() + 1L)
			    || (dividend.IsZero)) {
				/* If the divisor's integer part is greater than this's integer part,
				 * the result must be zero with the appropriate scale */
				integralValue = BigInteger.Zero;
			} else if (newScale == 0) {
				integralValue = dividend.UnscaledValue / divisor.UnscaledValue;
			} else if (newScale > 0) {
				powerOfTen = Multiplication.PowerOf10(newScale);
				integralValue = dividend.UnscaledValue / (divisor.UnscaledValue * powerOfTen);
				integralValue = integralValue * powerOfTen;
			} else {
				// (newScale < 0)
				powerOfTen = Multiplication.PowerOf10(-newScale);
				integralValue = (dividend.UnscaledValue * powerOfTen) / divisor.UnscaledValue;

				// To strip trailing zeros approximating to the preferred scale
				while (!BigInteger.TestBit(integralValue, 0)) {
					quotient = BigMath.DivideAndRemainder(integralValue, BigDecimal.TenPow[i], out remainder);
					if ((remainder.Sign == 0)
					    && (tempScale - i >= newScale)) {
						tempScale -= i;
						if (i < lastPow) {
							i++;
						}
						integralValue = quotient;
					} else {
						if (i == 1) {
							break;
						}

						i = 1;
					}
				}

				newScale = tempScale;
			}

			return ((integralValue.Sign == 0)
				? BigDecimal.GetZeroScaledBy(newScale)
				: new BigDecimal(integralValue, BigDecimal.ToIntScale(newScale)));
		}

		public static BigDecimal DivideToIntegralValue(BigDecimal dividend, BigDecimal divisor, MathContext mc) {
			int mcPrecision = mc.Precision;
			int diffPrecision = dividend.Precision - divisor.Precision;
			int lastPow = BigDecimal.TenPow.Length - 1;
			long diffScale = (long) dividend.Scale - divisor.Scale;
			long newScale = diffScale;
			long quotPrecision = diffPrecision - diffScale + 1;
			BigInteger quotient;
			BigInteger remainder;

			// In special cases it call the dual method
			if ((mcPrecision == 0) || (dividend.IsZero) || (divisor.IsZero)) {
				return DivideToIntegralValue(dividend, divisor);
			}

			// Let be:   this = [u1,s1]   and   divisor = [u2,s2]
			if (quotPrecision <= 0) {
				quotient = BigInteger.Zero;
			} else if (diffScale == 0) {
				// CASE s1 == s2:  to calculate   u1 / u2 
				quotient = dividend.UnscaledValue / divisor.UnscaledValue;
			} else if (diffScale > 0) {
				// CASE s1 >= s2:  to calculate   u1 / (u2 * 10^(s1-s2)  
				quotient = dividend.UnscaledValue / (divisor.UnscaledValue * Multiplication.PowerOf10(diffScale));

				// To chose  10^newScale  to get a quotient with at least 'mc.precision()' digits
				newScale = System.Math.Min(diffScale, System.Math.Max(mcPrecision - quotPrecision + 1, 0));

				// To calculate: (u1 / (u2 * 10^(s1-s2)) * 10^newScale
				quotient = quotient * Multiplication.PowerOf10(newScale);
			} else {
				// CASE s2 > s1:   
				/* To calculate the minimum power of ten, such that the quotient 
				 *   (u1 * 10^exp) / u2   has at least 'mc.precision()' digits. */
				long exp = System.Math.Min(-diffScale, System.Math.Max((long) mcPrecision - diffPrecision, 0));
				long compRemDiv;

				// Let be:   (u1 * 10^exp) / u2 = [q,r]  
				quotient = BigMath.DivideAndRemainder(dividend.UnscaledValue * Multiplication.PowerOf10(exp),
					divisor.UnscaledValue,
					out remainder);
				newScale += exp; // To fix the scale
				exp = -newScale; // The remaining power of ten

				// If after division there is a remainder...
				if ((remainder.Sign != 0) && (exp > 0)) {
					// Log10(r) + ((s2 - s1) - exp) > mc.precision ?
					compRemDiv = (new BigDecimal(remainder)).Precision
					             + exp - divisor.Precision;
					if (compRemDiv == 0) {
						// To calculate:  (r * 10^exp2) / u2
						remainder = (remainder * Multiplication.PowerOf10(exp)) / divisor.UnscaledValue;
						compRemDiv = System.Math.Abs(remainder.Sign);
					}
					if (compRemDiv > 0) {
						// The quotient won't fit in 'mc.precision()' digits
						// math.06=Division impossible
						throw new ArithmeticException(Messages.math06); //$NON-NLS-1$
					}
				}
			}

			// Fast return if the quotient is zero
			if (quotient.Sign == 0) {
				return BigDecimal.GetZeroScaledBy(diffScale);
			}

			BigInteger strippedBI = quotient;
			BigDecimal integralValue = new BigDecimal(quotient);
			long resultPrecision = integralValue.Precision;
			int i = 1;

			// To strip trailing zeros until the specified precision is reached
			while (!BigInteger.TestBit(strippedBI, 0)) {
				quotient = BigMath.DivideAndRemainder(strippedBI, BigDecimal.TenPow[i], out remainder);
				if ((remainder.Sign == 0) &&
				    ((resultPrecision - i >= mcPrecision)
				     || (newScale - i >= diffScale))) {
					resultPrecision -= i;
					newScale -= i;
					if (i < lastPow) {
						i++;
					}
					strippedBI = quotient;
				} else {
					if (i == 1) {
						break;
					}

					i = 1;
				}
			}

			// To check if the result fit in 'mc.precision()' digits
			if (resultPrecision > mcPrecision) {
				// math.06=Division impossible
				throw new ArithmeticException(Messages.math06); //$NON-NLS-1$
			}

			integralValue.Scale = BigDecimal.ToIntScale(newScale);
			integralValue.SetUnscaledValue(strippedBI);
			return integralValue;
		}


		public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor, int scale, RoundingMode roundingMode) {
			// Let be: this = [u1,s1]  and  divisor = [u2,s2]
			if (divisor.IsZero) {
				// math.04=Division by zero
				throw new ArithmeticException(Messages.math04); //$NON-NLS-1$
			}

			long diffScale = ((long) dividend.Scale - divisor.Scale) - scale;
			if (dividend.BitLength < 64 && divisor.BitLength < 64) {
				if (diffScale == 0)
					return DividePrimitiveLongs(dividend.SmallValue, divisor.SmallValue, scale, roundingMode);

				if (diffScale > 0) {
					if (diffScale < BigDecimal.LongTenPow.Length &&
					    divisor.BitLength + BigDecimal.LongTenPowBitLength[(int) diffScale] < 64) {
						return DividePrimitiveLongs(dividend.SmallValue,
							divisor.SmallValue * BigDecimal.LongTenPow[(int) diffScale],
							scale,
							roundingMode);
					}
				} else {
					// diffScale < 0
					if (-diffScale < BigDecimal.LongTenPow.Length &&
					    dividend.BitLength + BigDecimal.LongTenPowBitLength[(int) -diffScale] < 64) {
						return DividePrimitiveLongs(dividend.SmallValue * BigDecimal.LongTenPow[(int) -diffScale],
							divisor.SmallValue,
							scale,
							roundingMode);
					}

				}
			}

			BigInteger scaledDividend = dividend.UnscaledValue;
			BigInteger scaledDivisor = divisor.UnscaledValue; // for scaling of 'u2'

			if (diffScale > 0) {
				// Multiply 'u2'  by:  10^((s1 - s2) - scale)
				scaledDivisor = Multiplication.MultiplyByTenPow(scaledDivisor, (int) diffScale);
			} else if (diffScale < 0) {
				// Multiply 'u1'  by:  10^(scale - (s1 - s2))
				scaledDividend = Multiplication.MultiplyByTenPow(scaledDividend, (int) -diffScale);
			}
			return DivideBigIntegers(scaledDividend, scaledDivisor, scale, roundingMode);
		}

		public static BigDecimal DivideAndRemainder(BigDecimal dividend, BigDecimal divisor, out BigDecimal remainder) {
			var quotient = DivideToIntegralValue(dividend, divisor);
			remainder = Subtract(dividend, Multiply(quotient, divisor));
			return quotient;
		}

		public static BigDecimal DivideAndRemainder(BigDecimal dividend,
			BigDecimal divisor,
			MathContext mc,
			out BigDecimal remainder) {
			var quotient = DivideToIntegralValue(dividend, divisor, mc);
			remainder = Subtract(dividend, Multiply(quotient, divisor));
			return quotient;
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

		public static BigDecimal Multiply(BigDecimal value, BigDecimal multiplicand) {
			long newScale = (long) value.Scale + multiplicand.Scale;

			if ((value.IsZero) || (multiplicand.IsZero)) {
				return BigDecimal.GetZeroScaledBy(newScale);
			}
			/* Let be: this = [u1,s1] and multiplicand = [u2,s2] so:
			 * this x multiplicand = [ s1 * s2 , s1 + s2 ] */
			if (value.BitLength + multiplicand.BitLength < 64) {
				return BigDecimal.Create(value.SmallValue * multiplicand.SmallValue, BigDecimal.ToIntScale(newScale));
			}

			return new BigDecimal(value.UnscaledValue * multiplicand.UnscaledValue, BigDecimal.ToIntScale(newScale));
		}


		public static BigDecimal Pow(BigDecimal number, int n) {
			if (n == 0) {
				return BigDecimal.One;
			}

			if ((n < 0) || (n > 999999999)) {
				// math.07=Invalid Operation
				throw new ArithmeticException(Messages.math07); //$NON-NLS-1$
			}

			long newScale = number.Scale * (long) n;

			// Let be: this = [u,s]   so:  this^n = [u^n, s*n]
			return ((number.IsZero)
				? BigDecimal.GetZeroScaledBy(newScale)
				: new BigDecimal(BigMath.Pow(number.UnscaledValue, n), BigDecimal.ToIntScale(newScale)));
		}

		public static BigDecimal Pow(BigDecimal number, int n, MathContext mc) {
			// The ANSI standard X3.274-1996 algorithm
			int m = System.Math.Abs(n);
			int mcPrecision = mc.Precision;
			int elength = (int) System.Math.Log10(m) + 1; // decimal digits in 'n'
			int oneBitMask; // mask of bits
			BigDecimal accum; // the single accumulator
			MathContext newPrecision = mc; // MathContext by default

			// In particular cases, it reduces the problem to call the other 'pow()'
			if ((n == 0) || ((number.IsZero) && (n > 0))) {
				return Pow(number, n);
			}

			if ((m > 999999999) || ((mcPrecision == 0) && (n < 0))
			    || ((mcPrecision > 0) && (elength > mcPrecision))) {
				// math.07=Invalid Operation
				throw new ArithmeticException(Messages.math07); //$NON-NLS-1$
			}

			if (mcPrecision > 0) {
				newPrecision = new MathContext(mcPrecision + elength + 1,
					mc.RoundingMode);
			}

			// The result is calculated as if 'n' were positive        
			accum = BigMath.Round(number, newPrecision);
			oneBitMask = Utils.HighestOneBit(m) >> 1;

			while (oneBitMask > 0) {
				accum = BigMath.Multiply(accum, accum, newPrecision);
				if ((m & oneBitMask) == oneBitMask) {
					accum = BigMath.Multiply(accum, number, newPrecision);
				}
				oneBitMask >>= 1;
			}

			// If 'n' is negative, the value is divided into 'ONE'
			if (n < 0) {
				accum = Divide(BigDecimal.One, accum, newPrecision);
			}

			// The final value is rounded to the destination precision
			accum.InplaceRound(mc);
			return accum;
		}

		public static BigDecimal Sqrt(BigDecimal x, MathContext mathContext) {
			switch (x.Sign) {
				case 0:
					return BigDecimal.Zero;
				case -1:
					throw new ArithmeticException("Illegal sqrt(x) for x < 0: x = " + x);
			}

			int maxPrecision = mathContext.Precision + 4;
			BigDecimal acceptableError = BigMath.MovePointLeft(BigDecimal.One, mathContext.Precision + 1);

			BigDecimal result = System.Math.Sqrt(x.ToDouble());
			int adaptivePrecision = ExpectedInitialPrecision;
			BigDecimal last;

			do {
				last = result;
				adaptivePrecision = adaptivePrecision * 3;
				if (adaptivePrecision > maxPrecision) {
					adaptivePrecision = maxPrecision;
				}
				MathContext mc = new MathContext(adaptivePrecision, mathContext.RoundingMode);
				result = Divide(Add(Divide(x, result, mc), last, mc), Two, mc);
			} while (adaptivePrecision < maxPrecision || BigMath.Abs(Subtract(result, last)).CompareTo(acceptableError) > 0);

			return BigMath.Round(result, mathContext);
		}


		public static BigDecimal Scale(BigDecimal number, int newScale, RoundingMode roundingMode) {
			long diffScale = newScale - (long) number.Scale;

			// Let be:  'number' = [u,s]        
			if (diffScale == 0) {
				return number;
			}

			if (diffScale > 0) {
				// return  [u * 10^(s2 - s), newScale]
				if (diffScale < BigDecimal.LongTenPow.Length &&
				    (number.BitLength + BigDecimal.LongTenPowBitLength[(int) diffScale]) < 64) {
					return BigDecimal.Create(number.SmallValue * BigDecimal.LongTenPow[(int) diffScale], newScale);
				}

				return new BigDecimal(Multiplication.MultiplyByTenPow(number.UnscaledValue, (int) diffScale), newScale);
			}

			// diffScale < 0
			// return  [u,s] / [1,newScale]  with the appropriate scale and rounding
			if (number.BitLength < 64 && -diffScale < BigDecimal.LongTenPow.Length) {
				return DividePrimitiveLongs(number.SmallValue, BigDecimal.LongTenPow[(int) -diffScale], newScale, roundingMode);
			}

			return DivideBigIntegers(number.UnscaledValue, Multiplication.PowerOf10(-diffScale), newScale, roundingMode);
		}

		public static BigDecimal MovePoint(BigDecimal number, long newScale) {
			if (number.IsZero) {
				return BigDecimal.GetZeroScaledBy(System.Math.Max(newScale, 0));
			}
			/* When:  'n'== Integer.MIN_VALUE  isn't possible to call to movePointRight(-n)  
			 * since  -Integer.MIN_VALUE == Integer.MIN_VALUE */
			if (newScale >= 0) {
				if (number.BitLength < 64) {
					return BigDecimal.Create(number.SmallValue, BigDecimal.ToIntScale(newScale));
				}

				return new BigDecimal(number.UnscaledValue, BigDecimal.ToIntScale(newScale));
			}

			if (-newScale < BigDecimal.LongTenPow.Length &&
			    number.BitLength + BigDecimal.LongTenPowBitLength[(int) -newScale] < 64) {
				return BigDecimal.Create(number.SmallValue * BigDecimal.LongTenPow[(int) -newScale], 0);
			}

			return new BigDecimal(Multiplication.MultiplyByTenPow(number.UnscaledValue, (int) -newScale), 0);
		}

		public static BigDecimal Add(BigDecimal value, BigDecimal augend) {
			int diffScale = value.Scale - augend.Scale;

			// Fast return when some operand is zero
			if (value.IsZero) {
				if (diffScale <= 0)
					return augend;
				if (augend.IsZero)
					return value;
			} else if (augend.IsZero) {
				if (diffScale >= 0)
					return value;
			}

			// Let be:  this = [u1,s1]  and  augend = [u2,s2]
			if (diffScale == 0) {
				// case s1 == s2: [u1 + u2 , s1]
				if (System.Math.Max(value.BitLength, augend.BitLength) + 1 < 64) {
					return BigDecimal.Create(value.SmallValue + augend.SmallValue, value.Scale);
				}

				return new BigDecimal(value.UnscaledValue + augend.UnscaledValue, value.Scale);
			}

			if (diffScale > 0)

				// case s1 > s2 : [(u1 + u2) * 10 ^ (s1 - s2) , s1]
				return AddAndMult10(value, augend, diffScale);

			// case s2 > s1 : [(u2 + u1) * 10 ^ (s2 - s1) , s2]
			return AddAndMult10(augend, value, -diffScale);
		}

		private static BigDecimal AddAndMult10(BigDecimal thisValue, BigDecimal augend, int diffScale) {
			if (diffScale < BigDecimal.LongTenPow.Length &&
			    System.Math.Max(thisValue.BitLength, augend.BitLength + BigDecimal.LongTenPowBitLength[diffScale]) + 1 < 64) {
				return BigDecimal.Create(thisValue.SmallValue + augend.SmallValue * BigDecimal.LongTenPow[diffScale],
					thisValue.Scale);
			}

			return new BigDecimal(
				thisValue.UnscaledValue + Multiplication.MultiplyByTenPow(augend.UnscaledValue, diffScale),
				thisValue.Scale);
		}

		public static BigDecimal Add(BigDecimal value, BigDecimal augend, MathContext mc) {
			BigDecimal larger; // operand with the largest unscaled value
			BigDecimal smaller; // operand with the smallest unscaled value
			BigInteger tempBi;
			long diffScale = (long) value.Scale - augend.Scale;

			// Some operand is zero or the precision is infinity  
			if ((augend.IsZero) || (value.IsZero) || (mc.Precision == 0)) {
				return BigMath.Round(Add(value, augend), mc);
			}

			// Cases where there is room for optimizations
			if (value.AproxPrecision() < diffScale - 1) {
				larger = augend;
				smaller = value;
			} else if (augend.AproxPrecision() < -diffScale - 1) {
				larger = value;
				smaller = augend;
			} else {
				// No optimization is done 
				return BigMath.Round(Add(value, augend), mc);
			}

			if (mc.Precision >= larger.AproxPrecision()) {
				// No optimization is done
				return BigMath.Round(Add(value, augend), mc);
			}

			// Cases where it's unnecessary to add two numbers with very different scales 
			var largerSignum = larger.Sign;
			if (largerSignum == smaller.Sign) {
				tempBi = Multiplication.MultiplyByPositiveInt(larger.UnscaledValue, 10) +
				         BigInteger.FromInt64(largerSignum);
			} else {
				tempBi = larger.UnscaledValue - BigInteger.FromInt64(largerSignum);
				tempBi = Multiplication.MultiplyByPositiveInt(tempBi, 10) +
				         BigInteger.FromInt64(largerSignum * 9);
			}

			// Rounding the improved adding 
			larger = new BigDecimal(tempBi, larger.Scale + 1);
			return BigMath.Round(larger, mc);
		}

		public static BigDecimal Subtract(BigDecimal value, BigDecimal subtrahend) {
			if (subtrahend == null)
				throw new ArgumentNullException("subtrahend");

			int diffScale = value.Scale - subtrahend.Scale;

			// Fast return when some operand is zero
			if (value.IsZero) {
				if (diffScale <= 0) {
					return -subtrahend;
				}
				if (subtrahend.IsZero) {
					return value;
				}
			} else if (subtrahend.IsZero) {
				if (diffScale >= 0) {
					return value;
				}
			}

			// Let be: this = [u1,s1] and subtrahend = [u2,s2] so:
			if (diffScale == 0) {
				// case s1 = s2 : [u1 - u2 , s1]
				if (System.Math.Max(value.BitLength, subtrahend.BitLength) + 1 < 64) {
					return BigDecimal.Create(value.SmallValue - subtrahend.SmallValue, value.Scale);
				}

				return new BigDecimal(value.UnscaledValue - subtrahend.UnscaledValue, value.Scale);
			}

			if (diffScale > 0) {
				// case s1 > s2 : [ u1 - u2 * 10 ^ (s1 - s2) , s1 ]
				if (diffScale < BigDecimal.LongTenPow.Length &&
				    System.Math.Max(value.BitLength, subtrahend.BitLength + BigDecimal.LongTenPowBitLength[diffScale]) + 1 < 64) {
					return BigDecimal.Create(value.SmallValue - subtrahend.SmallValue * BigDecimal.LongTenPow[diffScale], value.Scale);
				}

				return new BigDecimal(
					value.UnscaledValue - Multiplication.MultiplyByTenPow(subtrahend.UnscaledValue, diffScale),
					value.Scale);
			}

			// case s2 > s1 : [ u1 * 10 ^ (s2 - s1) - u2 , s2 ]
			diffScale = -diffScale;
			if (diffScale < BigDecimal.LongTenPow.Length &&
			    System.Math.Max(value.BitLength + BigDecimal.LongTenPowBitLength[diffScale], subtrahend.BitLength) + 1 < 64) {
				return BigDecimal.Create(value.SmallValue * BigDecimal.LongTenPow[diffScale] - subtrahend.SmallValue,
					subtrahend.Scale);
			}

			return new BigDecimal(Multiplication.MultiplyByTenPow(value.UnscaledValue, diffScale) -
			                      subtrahend.UnscaledValue,
				subtrahend.Scale);
		}

		public static BigDecimal Subtract(BigDecimal value, BigDecimal subtrahend, MathContext mc) {
			if (subtrahend == null)
				throw new ArgumentNullException("subtrahend");
			if (mc == null)
				throw new ArgumentNullException("mc");

			long diffScale = subtrahend.Scale - (long) value.Scale;

			// Some operand is zero or the precision is infinity  
			if ((subtrahend.IsZero) || (value.IsZero) || (mc.Precision == 0))
				return BigMath.Round(Subtract(value, subtrahend), mc);

			// Now:   this != 0   and   subtrahend != 0
			if (subtrahend.AproxPrecision() < diffScale - 1) {
				// Cases where it is unnecessary to subtract two numbers with very different scales
				if (mc.Precision < value.AproxPrecision()) {
					var thisSignum = value.Sign;
					BigInteger tempBI;
					if (thisSignum != subtrahend.Sign) {
						tempBI = Multiplication.MultiplyByPositiveInt(value.UnscaledValue, 10) +
						         BigInteger.FromInt64(thisSignum);
					} else {
						tempBI = value.UnscaledValue - BigInteger.FromInt64(thisSignum);
						tempBI = Multiplication.MultiplyByPositiveInt(tempBI, 10) +
						         BigInteger.FromInt64(thisSignum * 9);
					}

					// Rounding the improved subtracting
					var leftOperand = new BigDecimal(tempBI, value.Scale + 1); // it will be only the left operand (this) 
					return BigMath.Round(leftOperand, mc);
				}
			}

			// No optimization is done
			return BigMath.Round(Subtract(value, subtrahend), mc);
		}

		public static BigDecimal Factorial(int n) {
			if (n < 0)
				throw new ArithmeticException("Illegal factorial(n) for n < 0: n = " + n);

			if (n < factorialCache.Length)
				return factorialCache[n];

			BigDecimal result = factorialCache[factorialCache.Length - 1];
			for (int i = factorialCache.Length; i <= n; i++) {
				result = Multiply(result, i);
			}

			return result;
		}

		public static BigDecimal Root(BigDecimal n, BigDecimal x, MathContext mathContext) {
			switch (x.Sign) {
				case 0:
					return BigDecimal.Zero;
				case -1:
					throw new ArithmeticException("Illegal root(x) for x < 0: x = " + x);
			}

			int maxPrecision = mathContext.Precision + 4;
			BigDecimal acceptableError = BigMath.MovePointLeft(BigDecimal.One, mathContext.Precision + 1);

			BigDecimal nMinus1 = Subtract(n, BigDecimal.One);
			BigDecimal result = Divide(x, Two, MathContext.Decimal32);
			int adaptivePrecision = 2; // first approximation has really bad precision
			BigDecimal step;

			do {
				adaptivePrecision = adaptivePrecision * 3;
				if (adaptivePrecision > maxPrecision) {
					adaptivePrecision = maxPrecision;
				}
				MathContext mc = new MathContext(adaptivePrecision, mathContext.RoundingMode);

				step = Divide(Subtract(Divide(x, Pow(result, nMinus1, mc), mc), result, mc), n, mc);
				result = Add(result, step, mc);
			} while (adaptivePrecision < maxPrecision || BigMath.Abs(step).CompareTo(acceptableError) > 0);

			return BigMath.Round(result, mathContext);
		}

		public static BigDecimal Log(BigDecimal x, MathContext mathContext) {
			// http://en.wikipedia.org/wiki/Natural_logarithm

			if (x.Sign <= 0)
				throw new ArithmeticException("Illegal log(x) for x <= 0: x = " + x);

			if (x.CompareTo(BigDecimal.One) == 0)
				return BigDecimal.Zero;

			BigDecimal result;
			switch (x.CompareTo(Ten)) {
				case 0:
					result = LogTen(mathContext);
					break;
				case 1:
					result = LogUsingExponent(x, mathContext);
					break;
				default:
					result = LogUsingTwoThree(x, mathContext);
					break;
			}

			return BigMath.Round(result, mathContext);
		}

		private static BigDecimal LogUsingNewton(BigDecimal x, MathContext mathContext) {
			// https://en.wikipedia.org/wiki/Natural_logarithm in chapter 'High Precision'
			// y = y + 2 * (x-exp(y)) / (x+exp(y))

			int maxPrecision = mathContext.Precision + 4;
			BigDecimal acceptableError = BigMath.MovePointLeft(BigDecimal.One, mathContext.Precision + 1);

			BigDecimal result = System.Math.Log(x.ToDouble());
			int adaptivePrecision = ExpectedInitialPrecision;
			BigDecimal step;

			do {
				adaptivePrecision = adaptivePrecision * 3;
				if (adaptivePrecision > maxPrecision) {
					adaptivePrecision = maxPrecision;
				}
				MathContext mc = new MathContext(adaptivePrecision, mathContext.RoundingMode);

				BigDecimal expY = BigDecimalMath.Exp(result, mc);
				step = Divide(BigMath.Multiply(Two, Subtract(x, expY, mc), mc), Add(x, expY, mc), mc);
				result = Add(result, step);
			} while (adaptivePrecision < maxPrecision || BigMath.Abs(step).CompareTo(acceptableError) > 0);

			return result;
		}

		private static BigDecimal LogUsingTwoThree(BigDecimal x, MathContext mathContext) {
			MathContext mc = new MathContext(mathContext.Precision + 4, mathContext.RoundingMode);

			int factorOfTwo = 0;
			int powerOfTwo = 1;
			int factorOfThree = 0;
			int powerOfThree = 1;

			double value = x.ToDouble();
			if (value < 0.1) {
				// never happens when called by logUsingExponent()
				while (value < 0.6) {
					value *= 2;
					factorOfTwo--;
					powerOfTwo *= 2;
				}
			} else if (value < 0.115) {
				// (0.1 - 0.11111 - 0.115) -> (0.9 - 1.0 - 1.035)
				factorOfThree = -2;
				powerOfThree = 9;
			} else if (value < 0.14) {
				// (0.115 - 0.125 - 0.14) -> (0.92 - 1.0 - 1.12)
				factorOfTwo = -3;
				powerOfTwo = 8;
			} else if (value < 0.2) {
				// (0.14 - 0.16667 - 0.2) - (0.84 - 1.0 - 1.2)
				factorOfTwo = -1;
				powerOfTwo = 2;
				factorOfThree = -1;
				powerOfThree = 3;
			} else if (value < 0.3) {
				// (0.2 - 0.25 - 0.3) -> (0.8 - 1.0 - 1.2)
				factorOfTwo = -2;
				powerOfTwo = 4;
			} else if (value < 0.42) {
				// (0.3 - 0.33333 - 0.42) -> (0.9 - 1.0 - 1.26)
				factorOfThree = -1;
				powerOfThree = 3;
			} else if (value < 0.7) {
				// (0.42 - 0.5 - 0.7) -> (0.84 - 1.0 - 1.4)
				factorOfTwo = -1;
				powerOfTwo = 2;
			} else if (value < 1.4) {
				// (0.7 - 1.0 - 1.4) -> (0.7 - 1.0 - 1.4)
				// do nothing
			} else if (value < 2.5) {
				// (1.4 - 2.0 - 2.5) -> (0.7 - 1.0 - 1.25)
				factorOfTwo = 1;
				powerOfTwo = 2;
			} else if (value < 3.5) {
				// (2.5 - 3.0 - 3.5) -> (0.833333 - 1.0 - 1.166667)
				factorOfThree = 1;
				powerOfThree = 3;
			} else if (value < 5.0) {
				// (3.5 - 4.0 - 5.0) -> (0.875 - 1.0 - 1.25)
				factorOfTwo = 2;
				powerOfTwo = 4;
			} else if (value < 7.0) {
				// (5.0 - 6.0 - 7.0) -> (0.833333 - 1.0 - 1.166667)
				factorOfThree = 1;
				powerOfThree = 3;
				factorOfTwo = 1;
				powerOfTwo = 2;
			} else if (value < 8.5) {
				// (7.0 - 8.0 - 8.5) -> (0.875 - 1.0 - 1.0625)
				factorOfTwo = 3;
				powerOfTwo = 8;
			} else if (value < 10.0) {
				// (8.5 - 9.0 - 10.0) -> (0.94444 - 1.0 - 1.11111)
				factorOfThree = 2;
				powerOfThree = 9;
			} else {
				while (value > 1.4) {
					// never happens when called by logUsingExponent()
					value /= 2;
					factorOfTwo++;
					powerOfTwo *= 2;
				}
			}

			BigDecimal correctedX = x;
			BigDecimal result = BigDecimal.Zero;

			if (factorOfTwo > 0) {
				correctedX = Divide(correctedX, powerOfTwo, mc);
				result = Add(result, BigMath.Multiply(LogTwo(mc), factorOfTwo, mc), mc);
			} else if (factorOfTwo < 0) {
				correctedX = BigMath.Multiply(correctedX, powerOfTwo, mc);
				result = Subtract(result, BigMath.Multiply(LogTwo(mc), -factorOfTwo, mc), mc);
			}

			if (factorOfThree > 0) {
				correctedX = Divide(correctedX, powerOfThree, mc);
				result = Add(result, BigMath.Multiply(LogThree(mc), factorOfThree, mc), mc);
			} else if (factorOfThree < 0) {
				correctedX = BigMath.Multiply(correctedX, powerOfThree, mc);
				result = Subtract(result, BigMath.Multiply(LogThree(mc), -factorOfThree, mc), mc);
			}

			result = Add(result, LogUsingNewton(correctedX, mc));

			return result;
		}

		private static BigDecimal LogUsingExponent(BigDecimal x, MathContext mathContext) {
			MathContext mc = new MathContext(mathContext.Precision + 4, mathContext.RoundingMode);

			int exponent = BigMath.Exponent(x);
			BigDecimal mantissa = BigMath.Mantissa(x);

			BigDecimal result = LogUsingTwoThree(mantissa, mc);
			if (exponent != 0) {
				result = Add(result, BigMath.Multiply(exponent, LogTen(mc), mc), mc);
			}
			return result;
		}

		private static BigDecimal LogTen(MathContext mathContext) {
			if (mathContext.Precision < LogOfTen.Precision) {
				return LogOfTen;
			}

			return LogUsingNewton(Ten, mathContext);
		}

		private static BigDecimal LogTwo(MathContext mathContext) {
			if (mathContext.Precision < LogOfTwo.Precision) {
				return LogOfTwo;
			}

			return LogUsingNewton(Two, mathContext);
		}

		private static BigDecimal LogThree(MathContext mathContext) {
			if (mathContext.Precision < LogOfThree.Precision) {
				return LogOfThree;
			}

			return LogUsingNewton(Three, mathContext);
		}

		public static BigDecimal Exp(BigDecimal x, MathContext mathContext) {
			if (x.Sign == 0) {
				return BigDecimal.One;
			}

			return ExpIntegralFractional(x, mathContext);
		}

		private static BigDecimal ExpIntegralFractional(BigDecimal x, MathContext mathContext) {
			BigDecimal integralPart = BigMath.IntegralPart(x);

			if (integralPart.Sign == 0) {
				return ExpTaylor(x, mathContext);
			}

			BigDecimal fractionalPart = Subtract(x, integralPart);

			MathContext mc = new MathContext(mathContext.Precision + 5, mathContext.RoundingMode);

			BigDecimal z = Add(BigDecimal.One, Divide(fractionalPart, integralPart, mc));
			BigDecimal t = ExpTaylor(z, mc);

			BigDecimal result = BigDecimal.One;
			while (integralPart.CompareTo(Int32.MaxValue) >= 0) {
				result = BigMath.Multiply(result, Pow(t, Int32.MaxValue, mc), mc);
				integralPart = Subtract(integralPart, Int32.MaxValue);
			}

			result = BigMath.Multiply(result, Pow(t, integralPart.ToInt32(), mc));

			return BigMath.Round(result, mathContext);
		}

		private static BigDecimal ExpTaylor(BigDecimal x, MathContext mathContext) {
			MathContext mc = new MathContext(mathContext.Precision + 4, mathContext.RoundingMode);

			x = Divide(x, 256, mc);

			BigDecimal result = ExpCalculator.Instance.Calculate(x, mc);
			result = BigDecimalMath.Pow(result, 256, mc);
			return BigMath.Round(result, mathContext);
		}
	}
}
