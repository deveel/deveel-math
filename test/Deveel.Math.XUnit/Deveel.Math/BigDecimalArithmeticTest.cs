using System;
using System.Globalization;

using Xunit;
using Xunit.Extensions;

namespace Deveel.Math {
	public class BigDecimalArithmeticTest {
		#region Add

		[Theory]
		[InlineData("1231212478987482988429808779810457634781384756794987", 10,
			"747233429293018787918347987234564568", 10,
			"123121247898748373566323807282924555312937.1991359555", 10)]
		[InlineData("1231212478987482988429808779810457634781384756794987", -10,
			"747233429293018787918347987234564568", -10,
			"1.231212478987483735663238072829245553129371991359555E+61", -10)]
		[InlineData("1231212478987482988429808779810457634781384756794987", 15,
			"747233429293018787918347987234564568", -10,
			"7472334294161400358170962860775454459810457634.781384756794987", 15)]
		[InlineData("1231212478987482988429808779810457634781384756794987", -15,
			"747233429293018787918347987234564568", 10,
			"1231212478987482988429808779810457634781459480137916301878791834798.7234564568", 10)]
		[InlineData("0", -15, "0", 10, "0E-10", 10)]
		public void Add(string a, int aScale, string b, int bScale, string c, int cScale) {
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Add(bNumber);
			Assert.Equal(c, result.ToString());
			Assert.Equal(cScale, result.Scale);
		}

		[Theory]
		[InlineData("1231212478987482988429808779810457634781384756794987", 10,
			"747233429293018787918347987234564568", 10,
			"1.2313E+41", -37,
			5, RoundingMode.Up)]
		[InlineData("1231212478987482988429808779810457634781384756794987", -10,
			"747233429293018787918347987234564568", -10,
			"1.2312E+61", -57,
			5, RoundingMode.Floor)]
		[InlineData("1231212478987482988429808779810457634781384756794987", 15,
			"747233429293018787918347987234564568", -10,
			"7.47233429416141E+45", -31,
			15, RoundingMode.Ceiling)]
		public void AddWithContext(string a, int aScale, string b, int bScale, string c, int cScale, int precision, RoundingMode mode) {
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			MathContext mc = new MathContext(precision, mode);
			BigDecimal result = aNumber.Add(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(cScale, result.Scale);
		}

		#endregion

		#region Subtract

		[InlineData("1231212478987482988429808779810457634781384756794987", 10,
			"747233429293018787918347987234564568", 10,
			"123121247898748224119637948679166971643339.7522230419", 10)]
		[InlineData("1231212478987482988429808779810457634781384756794987", -10,
			"747233429293018787918347987234564568", -10,
			"1.231212478987482241196379486791669716433397522230419E+61", -10)]
		[InlineData("1231212478987482988429808779810457634781384756794987", 15,
			"747233429293018787918347987234564568", -10,
			"-7472334291698975400195996883915836900189542365.218615243205013", 15)]
		[InlineData("1231212478987482988429808779810457634781384756794987", -15,
			"747233429293018787918347987234564568", 10,
			"1231212478987482988429808779810457634781310033452057698121208165201.2765435432", 10)]
		public void Subtract(string a, int aScale, string b, int bScale, string c, int cScale) {
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Subtract(bNumber);
			Assert.Equal(c, result.ToString());
			Assert.Equal(cScale, result.Scale);			
		}

		[InlineData("1231212478987482988429808779810457634781384756794987", 10,
			"747233429293018787918347987234564568", 10,
			"1.23121247898749E+41", -27,
			15, RoundingMode.Ceiling)]
		[InlineData("1231212478987482988429808779810457634781384756794987", 15,
			"747233429293018787918347987234564568", -10,
			"-7.4723342916989754E+45", -29,
			17, RoundingMode.Down)]
		[InlineData("986798656676789766678767876078779810457634781384756794987", -15,
			"747233429293018787918347987234564568", 40,
			"9.867986566767897666787678760787798104576347813847567949870000000000000E+71", -2,
			70, RoundingMode.HalfDown)]
		public void SubtractWithContext(string a, int aScale, string b, int bScale, string c, int cScale, int precision, RoundingMode mode) {
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			MathContext mc = new MathContext(precision, RoundingMode.Ceiling);
			BigDecimal result = aNumber.Subtract(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(cScale, result.Scale);			
		}

		#endregion


		#region Multiply

		[InlineData("1231212478987482988429808779810457634781384756794987", 15,
			"747233429293018787918347987234564568", 10,
			"92000312286217574978643009574114545567010139156902666284589309.1880727173060570190220616", 25)]
		[InlineData("1231212478987482988429808779810457634781384756794987", -15,
			"747233429293018787918347987234564568", -10,
			"9.20003122862175749786430095741145455670101391569026662845893091880727173060570190220616E+111", -25)]
		[InlineData("1231212478987482988429808779810457634781384756794987", 10,
			"747233429293018787918347987234564568", -10,
			"920003122862175749786430095741145455670101391569026662845893091880727173060570190220616", 0)]
		[InlineData("1231212478987482988429808779810457634781384756794987",-15,
			"747233429293018787918347987234564568", 10,
			"9.20003122862175749786430095741145455670101391569026662845893091880727173060570190220616E+91", -5)]
		public void Multiply(string a, int aScale, string b, int bScale, string c, int cScale) {
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Multiply(bNumber);
			Assert.Equal(c, result.ToString());
			Assert.Equal(cScale, result.Scale);			
		}

		[InlineData("97665696756578755423325476545428779810457634781384756794987", -25,
			"87656965586786097685674786576598865", 10,
			"8.561078619600910561431314228543672720908E+108", -69,
			40, RoundingMode.HalfDown)]
		[InlineData("987667796597975765768768767866756808779810457634781384756794987", 100,
			"747233429293018787918347987234564568", -70,
			"7.3801839465418518653942222612429081498248509257207477E+68", -16,
			53, RoundingMode.HalfUp)]
		[InlineData("488757458676796558668876576576579097029810457634781384756794987", -63,
			"747233429293018787918347987234564568", 63,
			"3.6521591193960361339707130098174381429788164316E+98", -52,
			47, RoundingMode.HalfUp)]
		public void MultiplyWithContext(string a, int aScale, string b, int bScale, string c, int cScale, int precision, RoundingMode roundingMode) {
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			MathContext mc = new MathContext(precision, roundingMode);
			BigDecimal result = aNumber.Multiply(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(cScale, result.Scale);			
		}

		#endregion

		#region Divide

		[Fact]
		public void DivideBigDecimalBySevenHundred() {
			const string a = "45465464646546546464646446547777777777777777777777777777888888888888888888888888888888777777777778542222222222221333333335";
			const int b = 700;

			BigDecimal bd1 = null;
			bd1 = BigDecimal.Parse(a);
			Assert.NotNull(bd1);

			BigDecimal bd2 = new BigDecimal(b);

			BigDecimal result = null;
			 result = bd1 / bd2;
			Assert.NotNull(result);
		}

		/**
		 * Divide by zero
		 */
		[Fact]
		public void DivideByZero() {
			String a = "1231212478987482988429808779810457634781384756794987";
			int aScale = 15;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = BigDecimal.ValueOf(0L);
			Assert.Throws<ArithmeticException>(() => aNumber.Divide(bNumber));
		}

		[Fact]
		public void DivideExceptionRoundingMode() {
			String a = "1231212478987482988429808779810457634781384756794987";
			const int aScale = 15;
			String b = "747233429293018787918347987234564568";
			const int bScale = 10;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			Assert.Throws<ArithmeticException>(() => aNumber.Divide(bNumber, RoundingMode.Unnecessary));
		}

		[Fact]
		public void DivideExceptionInvalidRoundingMode() {
			String a = "1231212478987482988429808779810457634781384756794987";
			const int aScale = 15;
			String b = "747233429293018787918347987234564568";
			const int bScale = 10;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			Assert.Throws<ArgumentException>(() => aNumber.Divide(bNumber, 100));
		}

		[Fact]
		public void DivideExpLessZero() {
			// Divide: local variable exponent is less than zero

			const string a = "1231212478987482988429808779810457634781384756794987";
			const int aScale = 15;
			const string b = "747233429293018787918347987234564568";
			const int bScale = 10;
			string c = "1.64770E+10";
			const int resScale = -5;

			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Ceiling);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		[Fact]
		public void DivideExpEqualsZero() {
			// Divide: local variable exponent is equal to zero

			const string a = "1231212478987482988429808779810457634781384756794987";
			const int aScale = -15;
			string b = "747233429293018787918347987234564568";
			const int bScale = 10;
			string c = "1.64769459009933764189139568605273529E+40";
			const int resScale = -5;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Ceiling);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		[Fact]
		public void DivideExpGreaterZero() {
			// Divide: local variable exponent is greater than zero

			String a = "1231212478987482988429808779810457634781384756794987";
			int aScale = -15;
			String b = "747233429293018787918347987234564568";
			int bScale = 20;
			String c = "1.647694590099337641891395686052735285121058381E+50";
			int resScale = -5;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Ceiling);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: remainder is zero
		 */
		[Fact]
		public void DivideRemainderIsZero() {
			String a = "8311389578904553209874735431110";
			int aScale = -15;
			String b = "237468273682987234567849583746";
			int bScale = 20;
			String c = "3.5000000000000000000000000000000E+36";
			int resScale = -5;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Ceiling);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_UP, result is negative
		 */
		[Fact]
		public void DivideRoundUpNeg() {
			String a = "-92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "-1.24390557635720517122423359799284E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Up);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_UP, result is positive
		 */
		[Fact]
		public void DivideRoundUpPos() {
			String a = "92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "1.24390557635720517122423359799284E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Up);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_DOWN, result is negative
		 */
		[Fact]
		public void DivideRoundDownNeg() {
			String a = "-92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "-1.24390557635720517122423359799283E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Down);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_DOWN, result is positive
		 */
		[Fact]
		public void DivideRoundDownPos() {
			String a = "92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "1.24390557635720517122423359799283E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Down);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_FLOOR, result is positive
		 */
		[Fact]
		public void DivideRoundFloorPos() {
			String a = "92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "1.24390557635720517122423359799283E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Floor);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_FLOOR, result is negative
		 */
		[Fact]
		public void DivideRoundFloorNeg() {
			String a = "-92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "-1.24390557635720517122423359799284E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Floor);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_CEILING, result is positive
		 */
		[Fact]
		public void DivideRoundCeilingPos() {
			String a = "92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "1.24390557635720517122423359799284E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Ceiling);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_CEILING, result is negative
		 */
		[Fact]
		public void DivideRoundCeilingNeg() {
			String a = "-92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "-1.24390557635720517122423359799283E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.Ceiling);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_UP, result is positive; distance = -1
		 */
		[Fact]
		public void DivideRoundHalfUpPos() {
			String a = "92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "1.24390557635720517122423359799284E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfUp);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_UP, result is negative; distance = -1
		 */
		[Fact]
		public void DivideRoundHalfUpNeg() {
			String a = "-92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "-1.24390557635720517122423359799284E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfUp);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_UP, result is positive; distance = 1
		 */
		[Fact]
		public void DivideRoundHalfUpPos1() {
			String a = "92948782094488478231212478987482988798104576347813847567949855464535634534563456";
			int aScale = -24;
			String b = "74723342238476237823754692930187879183479";
			int bScale = 13;
			String c = "1.2439055763572051712242335979928354832010167729111113605E+76";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfUp);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_UP, result is negative; distance = 1
		 */
		[Fact]
		public void DivideRoundHalfUpNeg1() {
			String a = "-92948782094488478231212478987482988798104576347813847567949855464535634534563456";
			int aScale = -24;
			String b = "74723342238476237823754692930187879183479";
			int bScale = 13;
			String c = "-1.2439055763572051712242335979928354832010167729111113605E+76";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfUp);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_UP, result is negative; equidistant
		 */
		[Fact]
		public void DivideRoundHalfUpNeg2() {
			String a = "-37361671119238118911893939591735";
			int aScale = 10;
			String b = "74723342238476237823787879183470";
			int bScale = 15;
			String c = "-1E+5";
			int resScale = -5;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfUp);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_DOWN, result is positive; distance = -1
		 */
		[Fact]
		public void DivideRoundHalfDownPos() {
			String a = "92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "1.24390557635720517122423359799284E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfDown);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_DOWN, result is negative; distance = -1
		 */
		[Fact]
		public void DivideRoundHalfDownNeg() {
			String a = "-92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "-1.24390557635720517122423359799284E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfDown);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_DOWN, result is positive; distance = 1
		 */
		[Fact]
		public void DivideRoundHalfDownPos1() {
			String a = "92948782094488478231212478987482988798104576347813847567949855464535634534563456";
			int aScale = -24;
			String b = "74723342238476237823754692930187879183479";
			int bScale = 13;
			String c = "1.2439055763572051712242335979928354832010167729111113605E+76";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfDown);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_DOWN, result is negative; distance = 1
		 */
		[Fact]
		public void DivideRoundHalfDownNeg1() {
			String a = "-92948782094488478231212478987482988798104576347813847567949855464535634534563456";
			int aScale = -24;
			String b = "74723342238476237823754692930187879183479";
			int bScale = 13;
			String c = "-1.2439055763572051712242335979928354832010167729111113605E+76";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfDown);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_UP, result is negative; equidistant
		 */
		[Fact]
		public void DivideRoundHalfDownNeg2() {
			String a = "-37361671119238118911893939591735";
			int aScale = 10;
			String b = "74723342238476237823787879183470";
			int bScale = 15;
			String c = "0E+5";
			int resScale = -5;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfDown);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_EVEN, result is positive; distance = -1
		 */
		[Fact]
		public void DivideRoundHalfEvenPos() {
			String a = "92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "1.24390557635720517122423359799284E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfEven);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_EVEN, result is negative; distance = -1
		 */
		[Fact]
		public void DivideRoundHalfEvenNeg() {
			String a = "-92948782094488478231212478987482988429808779810457634781384756794987";
			int aScale = -24;
			String b = "7472334223847623782375469293018787918347987234564568";
			int bScale = 13;
			String c = "-1.24390557635720517122423359799284E+53";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfEven);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_EVEN, result is positive; distance = 1
		 */
		[Fact]
		public void DivideRoundHalfEvenPos1() {
			String a = "92948782094488478231212478987482988798104576347813847567949855464535634534563456";
			int aScale = -24;
			String b = "74723342238476237823754692930187879183479";
			int bScale = 13;
			String c = "1.2439055763572051712242335979928354832010167729111113605E+76";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfEven);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_EVEN, result is negative; distance = 1
		 */
		[Fact]
		public void DivideRoundHalfEvenNeg1() {
			String a = "-92948782094488478231212478987482988798104576347813847567949855464535634534563456";
			int aScale = -24;
			String b = "74723342238476237823754692930187879183479";
			int bScale = 13;
			String c = "-1.2439055763572051712242335979928354832010167729111113605E+76";
			int resScale = -21;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfEven);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide: rounding mode is ROUND_HALF_EVEN, result is negative; equidistant
		 */
		[Fact]
		public void DivideRoundHalfEvenNeg2() {
			String a = "-37361671119238118911893939591735";
			int aScale = 10;
			String b = "74723342238476237823787879183470";
			int bScale = 15;
			String c = "0E+5";
			int resScale = -5;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, resScale, RoundingMode.HalfEven);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide to BigDecimal
		 */
		[Fact]
		public void DivideBigDecimal1() {
			String a = "-37361671119238118911893939591735";
			int aScale = 10;
			String b = "74723342238476237823787879183470";
			int bScale = 15;
			String c = "-5E+4";
			int resScale = -4;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * Divide to BigDecimal
		 */
		[Fact]
		public void DivideBigDecimal2() {
			String a = "-37361671119238118911893939591735";
			int aScale = 10;
			String b = "74723342238476237823787879183470";
			int bScale = -15;
			String c = "-5E-26";
			int resScale = 26;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, scale, RoundingMode)
		 */
		[Fact]
		public void DivideBigDecimalScaleRoundingModeUp() {
			String a = "-37361671119238118911893939591735";
			int aScale = 10;
			String b = "74723342238476237823787879183470";
			int bScale = -15;
			int newScale = 31;
			RoundingMode rm = RoundingMode.Up;
			String c = "-5.00000E-26";
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, newScale, rm);
			Assert.Equal(c, result.ToString());
			Assert.Equal(newScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, scale, RoundingMode)
		 */
		[Fact]
		public void DivideBigDecimalScaleRoundingModeDown() {
			String a = "-37361671119238118911893939591735";
			int aScale = 10;
			String b = "74723342238476237823787879183470";
			int bScale = 15;
			int newScale = 31;
			RoundingMode rm = RoundingMode.Down;
			String c = "-50000.0000000000000000000000000000000";
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, newScale, rm);
			Assert.Equal(c, result.ToString());
			Assert.Equal(newScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, scale, RoundingMode)
		 */
		[Fact]
		public void DivideBigDecimalScaleRoundingModeCeiling() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 100;
			String b = "74723342238476237823787879183470";
			int bScale = 15;
			int newScale = 45;
			RoundingMode rm = RoundingMode.Ceiling;
			String c = "1E-45";
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, newScale, rm);
			Assert.Equal(c, result.ToString());
			Assert.Equal(newScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, scale, RoundingMode)
		 */
		[Fact]
		public void DivideBigDecimalScaleRoundingModeFloor() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 100;
			String b = "74723342238476237823787879183470";
			int bScale = 15;
			int newScale = 45;
			RoundingMode rm = RoundingMode.Floor;
			String c = "0E-45";
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, newScale, rm);
			Assert.Equal(c, result.ToString());
			Assert.Equal(newScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, scale, RoundingMode)
		 */
		[Fact]
		public void DivideBigDecimalScaleRoundingModeHalfUp() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = -51;
			String b = "74723342238476237823787879183470";
			int bScale = 45;
			int newScale = 3;
			RoundingMode rm = RoundingMode.HalfUp;
			String c = "50000260373164286401361913262100972218038099522752460421" +
					   "05959924024355721031761947728703598332749334086415670525" +
					   "3761096961.670";
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, newScale, rm);
			Assert.Equal(c, result.ToString());
			Assert.Equal(newScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, scale, RoundingMode)
		 */
		[Fact]
		public void DivideBigDecimalScaleRoundingModeHalfDown() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 5;
			String b = "74723342238476237823787879183470";
			int bScale = 15;
			int newScale = 7;
			RoundingMode rm = RoundingMode.HalfDown;
			String c = "500002603731642864013619132621009722.1803810";
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, newScale, rm);
			Assert.Equal(c, result.ToString());
			Assert.Equal(newScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, scale, RoundingMode)
		 */
		[Fact]
		public void DivideBigDecimalScaleRoundingModeHalfEven() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 5;
			String b = "74723342238476237823787879183470";
			int bScale = 15;
			int newScale = 7;
			RoundingMode rm = RoundingMode.HalfEven;
			String c = "500002603731642864013619132621009722.1803810";
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, newScale, rm);
			Assert.Equal(c, result.ToString());
			Assert.Equal(newScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideBigDecimalScaleMathContextUp() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 15;
			String b = "748766876876723342238476237823787879183470";
			int bScale = 10;
			int precision = 21;
			RoundingMode rm = RoundingMode.Up;
			MathContext mc = new MathContext(precision, rm);
			String c = "49897861180.2562512996";
			int resScale = 10;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideBigDecimalScaleMathContextDown() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 15;
			String b = "748766876876723342238476237823787879183470";
			int bScale = 70;
			int precision = 21;
			RoundingMode rm = RoundingMode.Down;
			MathContext mc = new MathContext(precision, rm);
			String c = "4.98978611802562512995E+70";
			int resScale = -50;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideBigDecimalScaleMathContextCeiling() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 15;
			String b = "748766876876723342238476237823787879183470";
			int bScale = 70;
			int precision = 21;
			RoundingMode rm = RoundingMode.Ceiling;
			MathContext mc = new MathContext(precision, rm);
			String c = "4.98978611802562512996E+70";
			int resScale = -50;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideBigDecimalScaleMathContextFloor() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 15;
			String b = "748766876876723342238476237823787879183470";
			int bScale = 70;
			int precision = 21;
			RoundingMode rm = RoundingMode.Floor;
			MathContext mc = new MathContext(precision, rm);
			String c = "4.98978611802562512995E+70";
			int resScale = -50;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideBigDecimalScaleMathContextHALF_UP() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 70;
			int precision = 21;
			RoundingMode rm = RoundingMode.HalfUp;
			MathContext mc = new MathContext(precision, rm);
			String c = "2.77923185514690367475E+26";
			int resScale = -6;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideBigDecimalScaleMathContextHalfDown() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 70;
			int precision = 21;
			RoundingMode rm = RoundingMode.HalfDown;
			MathContext mc = new MathContext(precision, rm);
			String c = "2.77923185514690367475E+26";
			int resScale = -6;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * divide(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideBigDecimalScaleMathContextHalfEven() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 70;
			int precision = 21;
			RoundingMode rm = RoundingMode.HalfEven;
			MathContext mc = new MathContext(precision, rm);
			String c = "2.77923185514690367475E+26";
			int resScale = -6;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Divide(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}


		/**
		 * BigDecimal.divide with a scale that's too large
		 * 
		 * Regression  for HARMONY-6271
		 */
		[Fact]
		public void DivideLargeScale() {
			BigDecimal arg1 = BigDecimal.Parse("320.0E+2147483647");
			BigDecimal arg2 = BigDecimal.Parse("6E-2147483647");
			Assert.Throws<ArithmeticException>(() => arg1.Divide(arg2, Int32.MaxValue, RoundingMode.Ceiling));
		}

		/**
		 * divideToIntegralValue(BigDecimal)
		 */
		[Fact]
		public void DivideToIntegralValue() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 70;
			String c = "277923185514690367474770683";
			int resScale = 0;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.DivideToIntegralValue(bNumber);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * divideToIntegralValue(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideToIntegralValueMathContextUp() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 70;
			int precision = 32;
			RoundingMode rm = RoundingMode.Up;
			MathContext mc = new MathContext(precision, rm);
			String c = "277923185514690367474770683";
			int resScale = 0;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.DivideToIntegralValue(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * divideToIntegralValue(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideToIntegralValueMathContextDown() {
			String a = "3736186567876876578956958769675785435673453453653543654354365435675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 70;
			int precision = 75;
			RoundingMode rm = RoundingMode.Down;
			MathContext mc = new MathContext(precision, rm);
			String c = "2.7792318551469036747477068339450205874992634417590178670822889E+62";
			int resScale = -1;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.DivideToIntegralValue(bNumber, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * divideAndRemainder(BigDecimal)
		 */
		[Fact]
		public void DivideAndRemainder1() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 70;
			String res = "277923185514690367474770683";
			int resScale = 0;
			String rem = "1.3032693871288309587558885943391070087960319452465789990E-15";
			int remScale = 70;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal remainder;
			BigDecimal quotient = aNumber.DivideAndRemainder(bNumber, out remainder);
			Assert.Equal(res, quotient.ToString());
			Assert.Equal(resScale, quotient.Scale);
			Assert.Equal(rem, remainder.ToString());
			Assert.Equal(remScale, remainder.Scale);
		}

		/**
		 * divideAndRemainder(BigDecimal)
		 */
		[Fact]
		public void DivideAndRemainder2() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = -45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 70;
			String res = "2779231855146903674747706830969461168692256919247547952" +
						 "2608549363170374005512836303475980101168105698072946555" +
						 "6862849";
			int resScale = 0;
			String rem = "3.4935796954060524114470681810486417234751682675102093970E-15";
			int remScale = 70;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal remainder;
			BigDecimal quotient = aNumber.DivideAndRemainder(bNumber, out remainder);
			Assert.Equal(res, quotient.ToString());
			Assert.Equal(resScale, quotient.Scale);
			Assert.Equal(rem, remainder.ToString());
			Assert.Equal(remScale, remainder.Scale);
		}

		/**
		 * divideAndRemainder(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideAndRemainderMathContextUp() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 70;
			int precision = 75;
			RoundingMode rm = RoundingMode.Up;
			MathContext mc = new MathContext(precision, rm);
			String res = "277923185514690367474770683";
			int resScale = 0;
			String rem = "1.3032693871288309587558885943391070087960319452465789990E-15";
			int remScale = 70;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal remainder;
			BigDecimal quotient = aNumber.DivideAndRemainder(bNumber, mc, out remainder);
			Assert.Equal(res, quotient.ToString());
			Assert.Equal(resScale, quotient.Scale);
			Assert.Equal(rem, remainder.ToString());
			Assert.Equal(remScale, remainder.Scale);
		}

		/**
		 * divideAndRemainder(BigDecimal, MathContext)
		 */
		[Fact]
		public void DivideAndRemainderMathContextDown() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 20;
			int precision = 15;
			RoundingMode rm = RoundingMode.Down;
			MathContext mc = new MathContext(precision, rm);
			String res = "0E-25";
			int resScale = 25;
			String rem = "3736186567876.876578956958765675671119238118911893939591735";
			int remScale = 45;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal remainder;
			BigDecimal quotient = aNumber.DivideAndRemainder(bNumber, mc, out remainder);
			Assert.Equal(res, quotient.ToString());
			Assert.Equal(resScale, quotient.Scale);
			Assert.Equal(rem, remainder.ToString());
			Assert.Equal(remScale, remainder.Scale);
		}

		#endregion

		#region Pow

		[InlineData("123121247898748298842980", 10, 10, "8004424019039195734129783677098845174704975003788210729597" +
					   "4875206425711159855030832837132149513512555214958035390490" +
					   "798520842025826.594316163502809818340013610490541783276343" +
					   "6514490899700151256484355936102754469438371850240000000000", 100)]
		[InlineData("123121247898748298842980", 10, 0, "1", 0)]
		[InlineData("0", 0, 0, "1", 0)]
		public void Pow(string a, int aScale, int exp, string c, int cScale) {
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal result = aNumber.Pow(exp);
			Assert.Equal(c, result.ToString());
			Assert.Equal(cScale, result.Scale);			
		}

		[InlineData("123121247898748298842980", 10, 10, "8.0044E+130", -126, 5, RoundingMode.HalfUp)]
		public void PowWithContext(string a, int aScale, int exp, string c, int cScale, int precision, RoundingMode roundingMode) {
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			MathContext mc = new MathContext(precision, roundingMode);
			BigDecimal result = aNumber.Pow(exp, mc);
			Assert.Equal(c, result.ToString());
			Assert.Equal(cScale, result.Scale);
		}

		#endregion

		/**
		 * remainder(BigDecimal)
		 */
		[Fact]
		public void Remainder1() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 10;
			String res = "3736186567876.876578956958765675671119238118911893939591735";
			int resScale = 45;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Remainder(bNumber);
			Assert.Equal(res, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * remainder(BigDecimal)
		 */
		[Fact]
		public void Remainder2() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = -45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 10;
			String res = "1149310942946292909508821656680979993738625937.2065885780";
			int resScale = 10;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Remainder(bNumber);
			Assert.Equal(res, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * remainder(BigDecimal, MathContext)
		 */
		[Fact]
		public void RemainderMathContextHALF_UP() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 10;
			int precision = 15;
			RoundingMode rm = RoundingMode.HalfUp;
			MathContext mc = new MathContext(precision, rm);
			String res = "3736186567876.876578956958765675671119238118911893939591735";
			int resScale = 45;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Remainder(bNumber, mc);
			Assert.Equal(res, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * remainder(BigDecimal, MathContext)
		 */
		[Fact]
		public void RemainderMathContextHALF_DOWN() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = -45;
			String b = "134432345432345748766876876723342238476237823787879183470";
			int bScale = 10;
			int precision = 75;
			RoundingMode rm = RoundingMode.HalfDown;
			MathContext mc = new MathContext(precision, rm);
			String res = "1149310942946292909508821656680979993738625937.2065885780";
			int resScale = 10;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal bNumber = new BigDecimal(BigInteger.Parse(b), bScale);
			BigDecimal result = aNumber.Remainder(bNumber, mc);
			Assert.Equal(res, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * round(BigDecimal, MathContext)
		 */
		[Fact]
		public void RoundMathContextHALF_DOWN() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = -45;
			int precision = 75;
			RoundingMode rm = RoundingMode.HalfDown;
			MathContext mc = new MathContext(precision, rm);
			String res = "3.736186567876876578956958765675671119238118911893939591735E+102";
			int resScale = -45;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal result = aNumber.Round(mc);
			Assert.Equal(res, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * round(BigDecimal, MathContext)
		 */
		[Fact]
		public void RoundMathContextHALF_UP() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			int precision = 15;
			RoundingMode rm = RoundingMode.HalfUp;
			MathContext mc = new MathContext(precision, rm);
			String res = "3736186567876.88";
			int resScale = 2;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal result = aNumber.Round(mc);
			Assert.Equal(res, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * round(BigDecimal, MathContext) when precision = 0
		 */
		[Fact]
		public void RoundMathContextPrecision0() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			int precision = 0;
			RoundingMode rm = RoundingMode.HalfUp;
			MathContext mc = new MathContext(precision, rm);
			String res = "3736186567876.876578956958765675671119238118911893939591735";
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal result = aNumber.Round(mc);
			Assert.Equal(res, result.ToString());
			Assert.Equal(aScale, result.Scale);
		}


		/**
		 * ulp() of a positive BigDecimal
		 */
		[Fact]
		public void UlpPos() {
			String a = "3736186567876876578956958765675671119238118911893939591735";
			int aScale = -45;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal result = aNumber.Ulp();
			String res = "1E+45";
			int resScale = -45;
			Assert.Equal(res, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * ulp() of a negative BigDecimal
		 */
		[Fact]
		public void UlpNeg() {
			String a = "-3736186567876876578956958765675671119238118911893939591735";
			int aScale = 45;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal result = aNumber.Ulp();
			String res = "1E-45";
			int resScale = 45;
			Assert.Equal(res, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}

		/**
		 * ulp() of a negative BigDecimal
		 */
		[Fact]
		public void UlpZero() {
			String a = "0";
			int aScale = 2;
			BigDecimal aNumber = new BigDecimal(BigInteger.Parse(a), aScale);
			BigDecimal result = aNumber.Ulp();
			String res = "0.01";
			int resScale = 2;
			Assert.Equal(res, result.ToString());
			Assert.Equal(resScale, result.Scale);
		}
	}
}