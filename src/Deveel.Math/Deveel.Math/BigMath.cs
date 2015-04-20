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
	/// <summary>
	///     Provides mathematical operations over big numbers
	/// </summary>
	public static class BigMath {
		private const int TaylorNterm = 8;

		/// <summary>
		///     The base of the natural logarithm in a predefined accuracy.
		/// </summary>
		public static readonly BigDecimal E = BigDecimal.Parse("2.71828182845904523536028747135266249775724709369995957496696762772407663035354" +
		                                                     "759457138217852516642742746639193200305992181741359662904357290033429526059563" +
		                                                     "073813232862794349076323382988075319525101901157383418793070215408914993488416" +
		                                                     "750924476146066808226480016847741185374234544243710753907774499206955170276183" +
		                                                     "860626133138458300075204493382656029760673711320070932870912744374704723069697" +
		                                                     "720931014169283681902551510865746377211125238978442505695369677078544996996794" +
		                                                     "686445490598793163688923009879312773617821542499922957635148220826989519366803" +
		                                                     "318252886939849646510582093923982948879332036250944311730123819706841614039701" +
		                                                     "983767932068328237646480429531180232878250981945581530175671736133206981125099" +
		                                                     "618188159304169035159888851934580727386673858942287922849989208680582574927961" +
		                                                     "048419844436346324496848756023362482704197862320900216099023530436994184914631" +
		                                                     "409343173814364054625315209618369088870701676839642437814059271456354906130310" +
		                                                     "720851038375051011574770417189861068739696552126715468895703503540212340784981" +
		                                                     "933432106817012100562788023519303322474501585390473041995777709350366041699732" +
		                                                     "972508868769664035557071622684471625607988265178713419512466520103059212366771" +
		                                                     "943252786753985589448969709640975459185695638023637016211204774272283648961342" +
		                                                     "251644507818244235294863637214174023889344124796357437026375529444833799801612" +
		                                                     "549227850925778256209262264832627793338656648162772516401910590049164499828931");

		/// <summary>
		///     Euler's constant Pi.
		/// </summary>
		public static readonly BigDecimal Pi = BigDecimal.Parse("3.14159265358979323846264338327950288419716939937510582097494459230781640628620" +
		                                                      "899862803482534211706798214808651328230664709384460955058223172535940812848111" +
		                                                      "745028410270193852110555964462294895493038196442881097566593344612847564823378" +
		                                                      "678316527120190914564856692346034861045432664821339360726024914127372458700660" +
		                                                      "631558817488152092096282925409171536436789259036001133053054882046652138414695" +
		                                                      "194151160943305727036575959195309218611738193261179310511854807446237996274956" +
		                                                      "735188575272489122793818301194912983367336244065664308602139494639522473719070" +
		                                                      "217986094370277053921717629317675238467481846766940513200056812714526356082778" +
		                                                      "577134275778960917363717872146844090122495343014654958537105079227968925892354" +
		                                                      "201995611212902196086403441815981362977477130996051870721134999999837297804995" +
		                                                      "105973173281609631859502445945534690830264252230825334468503526193118817101000" +
		                                                      "313783875288658753320838142061717766914730359825349042875546873115956286388235" +
		                                                      "378759375195778185778053217122680661300192787661119590921642019893809525720106" +
		                                                      "548586327886593615338182796823030195203530185296899577362259941389124972177528" +
		                                                      "347913151557485724245415069595082953311686172785588907509838175463746493931925" +
		                                                      "506040092770167113900984882401285836160356370766010471018194295559619894676783" +
		                                                      "744944825537977472684710404753464620804668425906949129331367702898915210475216" +
		                                                      "205696602405803815019351125338243003558764024749647326391419927260426992279678" +
		                                                      "235478163600934172164121992458631503028618297455570674983850549458858692699569" +
		                                                      "092721079750930295532116534498720275596023648066549911988183479775356636980742" +
		                                                      "654252786255181841757467289097777279380008164706001614524919217321721477235014");

		/// <summary>
		///     Euler-Mascheroni constant lower-case gamma.
		/// </summary>
		public static readonly BigDecimal Gamma = BigDecimal.Parse("0.577215664901532860606512090082402431" +
		                                                         "0421593359399235988057672348848677267776646709369470632917467495146314472498070" +
		                                                         "8248096050401448654283622417399764492353625350033374293733773767394279259525824" +
		                                                         "7094916008735203948165670853233151776611528621199501507984793745085705740029921" +
		                                                         "3547861466940296043254215190587755352673313992540129674205137541395491116851028" +
		                                                         "0798423487758720503843109399736137255306088933126760017247953783675927135157722" +
		                                                         "6102734929139407984301034177717780881549570661075010161916633401522789358679654" +
		                                                         "9725203621287922655595366962817638879272680132431010476505963703947394957638906" +
		                                                         "5729679296010090151251959509222435014093498712282479497471956469763185066761290" +
		                                                         "6381105182419744486783638086174945516989279230187739107294578155431600500218284" +
		                                                         "4096053772434203285478367015177394398700302370339518328690001558193988042707411" +
		                                                         "5422278197165230110735658339673487176504919418123000406546931429992977795693031" +
		                                                         "0050308630341856980323108369164002589297089098548682577736428825395492587362959" +
		                                                         "6133298574739302373438847070370284412920166417850248733379080562754998434590761" +
		                                                         "6431671031467107223700218107450444186647591348036690255324586254422253451813879" +
		                                                         "1243457350136129778227828814894590986384600629316947188714958752549236649352047" +
		                                                         "3243641097268276160877595088095126208404544477992299157248292516251278427659657" +
		                                                         "0832146102982146179519579590959227042089896279712553632179488737642106606070659" +
		                                                         "8256199010288075612519913751167821764361905705844078357350158005607745793421314" +
		                                                         "49885007864151716151945");

		/// <summary>
		///     Natural logarithm of 2.
		/// </summary>
		public static readonly BigDecimal Log2 = BigDecimal.Parse("0.693147180559945309417232121458176568075" +
		                                                        "50013436025525412068000949339362196969471560586332699641868754200148102057068573" +
		                                                        "368552023575813055703267075163507596193072757082837143519030703862389167347112335" +
		                                                        "011536449795523912047517268157493206515552473413952588295045300709532636664265410" +
		                                                        "423915781495204374043038550080194417064167151864471283996817178454695702627163106" +
		                                                        "454615025720740248163777338963855069526066834113727387372292895649354702576265209" +
		                                                        "885969320196505855476470330679365443254763274495125040606943814710468994650622016" +
		                                                        "772042452452961268794654619316517468139267250410380254625965686914419287160829380" +
		                                                        "317271436778265487756648508567407764845146443994046142260319309673540257444607030" +
		                                                        "809608504748663852313818167675143866747664789088143714198549423151997354880375165" +
		                                                        "861275352916610007105355824987941472950929311389715599820565439287170007218085761" +
		                                                        "025236889213244971389320378439353088774825970171559107088236836275898425891853530" +
		                                                        "243634214367061189236789192372314672321720534016492568727477823445353476481149418" +
		                                                        "642386776774406069562657379600867076257199184734022651462837904883062033061144630" +
		                                                        "073719489002743643965002580936519443041191150608094879306786515887090060520346842" +
		                                                        "973619384128965255653968602219412292420757432175748909770675268711581705113700915" +
		                                                        "894266547859596489065305846025866838294002283300538207400567705304678700184162404" +
		                                                        "418833232798386349001563121889560650553151272199398332030751408426091479001265168" +
		                                                        "243443893572472788205486271552741877243002489794540196187233980860831664811490930" +
		                                                        "667519339312890431641370681397776498176974868903887789991296503619270710889264105" +
		                                                        "230924783917373501229842420499568935992206602204654941510613");

		public static BigDecimal PiRound(MathContext mc) {
			if (mc.Precision < Pi.Precision)
				return Pi.Round(mc);

			int[] a = {1, 0, 0, -1, -1, -1, 0, 0};
			BigDecimal S = BroadhurstBbp(1, 1, a, mc);
			return MultiplyRound(S, 8);
		}

		public static BigDecimal GammaRound(MathContext mc) {
			if (mc.Precision < Gamma.Precision)
				return Gamma.Round(mc);

			double eps = PrecisionToError(0.577, mc.Precision);

			// Euler-Stieltjes as shown in Dilcher, Aequat Math 48 (1) (1994) 55-85
			var mcloc = new MathContext(2 + mc.Precision);
			BigDecimal resul = BigDecimal.One;
			resul = resul.Add(Log(2, mcloc));
			resul = resul.Subtract(Log(3, mcloc));

			// how many terms: zeta-1 falls as 1/2^(2n+1), so the
			// terms drop faster than 1/2^(4n+2). Set 1/2^(4kmax+2) < eps.
			// Leading term zeta(3)/(4^1*3) is 0.017. Leading zeta(3) is 1.2. Log(2) is 0.7

			var kmax = (int) ((System.Math.Log(eps/0.7) - 2.0)/4d);
			mcloc = new MathContext(1 + ErrorToPrecision(1.2, eps/kmax));
			for (int n = 1;; n++) {
				// zeta is close to 1. Division of zeta-1 through
				// 4^n*(2n+1) means divion through roughly 2^(2n+1)
				BigDecimal c = Zeta(2*n + 1, mcloc).Subtract(BigDecimal.One);
				BigInteger fourn = BigInteger.ValueOf(2*n + 1);
				fourn = fourn.ShiftLeft(2*n);
				c = DivideRound(c, fourn);
				resul = resul.Subtract(c);
				if (c.ToDouble() < 0.1*eps)
					break;
			}
			return resul.Round(mc);
		}

		public static BigDecimal Sqrt(BigDecimal x, MathContext mc) {
			if (x.CompareTo(BigDecimal.Zero) < 0)
				throw new ArithmeticException("negative argument " + x + " of square root");

			if (x.Abs().Subtract(new BigDecimal(System.Math.Pow(10d, -mc.Precision))).CompareTo(BigDecimal.Zero) < 0)
				return ScalePrecision(BigDecimal.Zero, mc);

			/* start the computation from a double precision estimate */
			var s = new BigDecimal(System.Math.Sqrt(x.ToDouble()), mc);
			BigDecimal half = BigDecimal.ValueOf(2);

			/* increase the local accuracy by 2 digits */
			var locmc = new MathContext(mc.Precision + 2, mc.RoundingMode);

			/* relative accuracy requested is 10^(-precision) 
                */
			double eps = System.Math.Pow(10.0, -mc.Precision);
			while (true) {
				/* s = s -(s/2-x/2s); test correction s-x/s for being
                        * smaller than the precision requested. The relative correction is 1-x/s^2,
                        * (actually half of this, which we use for a little bit of additional protection).
                        */
				if (System.Math.Abs(BigDecimal.One.Subtract(x.Divide(s.Pow(2, locmc), locmc)).ToDouble()) < eps)
					break;
				s = s.Add(x.Divide(s, locmc)).Divide(half, locmc);
			}

			return s;
		}

		public static BigDecimal Sqrt(BigDecimal x) {
			if (x.CompareTo(BigDecimal.Zero) < 0)
				throw new ArithmeticException("negative argument " + x + " of square root");

			return Root(2, x);
		}

		public static BigDecimal Zeta(int n, MathContext mc) {
			if (n <= 0)
				throw new NotSupportedException("Zeta at negative argument " + n + " not supported");
			if (n == 1)
				throw new ArithmeticException("Pole at zeta(1) ");

			if (n%2 == 0) {
				/* Even indices. Abramowitz-Stegun 23.2.16. Start with 2^(n-1)*B(n)/n!
                        */
				Rational b = Bernoulli.Default[n].Abs();
				b = b.Divide(Factorial.Default[n]);
				b = b.Multiply(BigInteger.One.ShiftLeft(n - 1));

				/* to be multiplied by pi^n. Absolute error in the result of pi^n is n times
                        * error in pi times pi^(n-1). Relative error is n*error(pi)/pi, requested by mc.
                        * Need one more digit in pi if n=10, two digits if n=100 etc, and add one extra digit.
                        */
				var mcpi = new MathContext(mc.Precision + (int) (System.Math.Log10(10.0*n)));
				BigDecimal piton = PiRound(mcpi).Pow(n, mc);
				return MultiplyRound(piton, b);
			}
			if (n == 3) {
				/* Broadhurst BBP <a href="http://arxiv.org/abs/math/9803067">arXiv:math/9803067</a>
                        * Error propagation: S31 is roughly 0.087, S33 roughly 0.131
                        */
				int[] a31 = {1, -7, -1, 10, -1, -7, 1, 0};
				int[] a33 = {1, 1, -1, -2, -1, 1, 1, 0};
				BigDecimal S31 = BroadhurstBbp(3, 1, a31, mc);
				BigDecimal S33 = BroadhurstBbp(3, 3, a33, mc);
				S31 = S31.Multiply(new BigDecimal(48));
				S33 = S33.Multiply(new BigDecimal(32));
				return S31.Add(S33).Divide(new BigDecimal(7), mc);
			}
			if (n == 5) {
				/* Broadhurst BBP <a href=http://arxiv.org/abs/math/9803067">arXiv:math/9803067</a>
                        * Error propagation: S51 is roughly -11.15, S53 roughly 22.165, S55 is roughly 0.031
                        * 9*2048*S51/6265 = -3.28. 7*2038*S53/61651= 5.07. 738*2048*S55/61651= 0.747.
                        * The result is of the order 1.03, so we add 2 digits to S51 and S52 and one digit to S55.
                        */
				int[] a51 = {31, -1614, -31, -6212, -31, -1614, 31, 74552};
				int[] a53 = {173, 284, -173, -457, -173, 284, 173, -111};
				int[] a55 = {1, 0, -1, -1, -1, 0, 1, 1};
				BigDecimal S51 = BroadhurstBbp(5, 1, a51, new MathContext(2 + mc.Precision));
				BigDecimal S53 = BroadhurstBbp(5, 3, a53, new MathContext(2 + mc.Precision));
				BigDecimal S55 = BroadhurstBbp(5, 5, a55, new MathContext(1 + mc.Precision));
				S51 = S51.Multiply(new BigDecimal(18432));
				S53 = S53.Multiply(new BigDecimal(14336));
				S55 = S55.Multiply(new BigDecimal(1511424));
				return S51.Add(S53).Subtract(S55).Divide(new BigDecimal(62651), mc);
			}
			/* Cohen et al Exp Math 1 (1) (1992) 25
                        */
			var betsum = new Rational();
			var bern = new Bernoulli();
			var fact = new Factorial();
			for (int npr = 0; npr <= (n + 1)/2; npr++) {
				Rational b = bern[2*npr].Multiply(bern[n + 1 - 2*npr]);
				b = b.Divide(fact[2*npr]).Divide(fact[n + 1 - 2*npr]);
				b = b.Multiply(1 - 2*npr);
				if (npr%2 == 0)
					betsum = betsum.Add(b);
				else
					betsum = betsum.Subtract(b);
			}
			betsum = betsum.Divide(n - 1);
			/* The first term, including the facor (2pi)^n, is essentially most
                        * of the result, near one. The second term below is roughly in the range 0.003 to 0.009.
                        * So the precision here is matching the precisionn requested by mc, and the precision
                        * requested for 2*pi is in absolute terms adjusted.
                        */
			var mcloc = new MathContext(2 + mc.Precision + (int) (System.Math.Log10(n)));
			BigDecimal ftrm = PiRound(mcloc).Multiply(new BigDecimal(2));
			ftrm = ftrm.Pow(n);
			ftrm = MultiplyRound(ftrm, betsum.ToBigDecimal(mcloc));
			var exps = new BigDecimal(0);

			/* the basic accuracy of the accumulated terms before multiplication with 2
                        */
			double eps = System.Math.Pow(10d, -mc.Precision);

			if (n%4 == 3) {
				/* since the argument n is at least 7 here, the drop
                                * of the terms is at rather constant pace at least 10^-3, for example
                                * 0.0018, 0.2e-7, 0.29e-11, 0.74e-15 etc for npr=1,2,3.... We want 2 times these terms
                                * fall below eps/10.
                                */
				int kmax = mc.Precision/3;
				eps /= kmax;
				/* need an error of eps for 2/(exp(2pi)-1) = 0.0037
                                * The absolute error is 4*exp(2pi)*err(pi)/(exp(2pi)-1)^2=0.0075*err(pi)
                                */
				BigDecimal exp2p = PiRound(new MathContext(3 + ErrorToPrecision(3.14, eps/0.0075)));
				exp2p = Exp(exp2p.Multiply(new BigDecimal(2)));
				BigDecimal c = exp2p.Subtract(BigDecimal.One);
				exps = DivideRound(1, c);
				for (int npr = 2; npr <= kmax; npr++) {
					/* the error estimate above for npr=1 is the worst case of
                                        * the absolute error created by an error in 2pi. So we can
                                        * safely re-use the exp2p value computed above without
                                        * reassessment of its error.
                                        */
					c = PowRound(exp2p, npr).Subtract(BigDecimal.One);
					c = MultiplyRound(c, (BigInteger.ValueOf(npr)).Pow(n));
					c = DivideRound(1, c);
					exps = exps.Add(c);
				}
			} else {
				/* since the argument n is at least 9 here, the drop
                                * of the terms is at rather constant pace at least 10^-3, for example
                                * 0.0096, 0.5e-7, 0.3e-11, 0.6e-15 etc. We want these terms
                                * fall below eps/10.
                                */
				int kmax = (1 + mc.Precision)/3;
				eps /= kmax;
				/* need an error of eps for 2/(exp(2pi)-1)*(1+4*Pi/8/(1-exp(-2pi)) = 0.0096
                                * at k=7 or = 0.00766 at k=13 for example.
                                * The absolute error is 0.017*err(pi) at k=9, 0.013*err(pi) at k=13, 0.012 at k=17
                                */
				BigDecimal twop = PiRound(new MathContext(3 + ErrorToPrecision(3.14, eps/0.017)));
				twop = twop.Multiply(new BigDecimal(2));
				BigDecimal exp2p = Exp(twop);
				BigDecimal c = exp2p.Subtract(BigDecimal.One);
				exps = DivideRound(1, c);
				c = BigDecimal.One.Subtract(DivideRound(1, exp2p));
				c = DivideRound(twop, c).Multiply(new BigDecimal(2));
				c = DivideRound(c, n - 1).Add(BigDecimal.One);
				exps = MultiplyRound(exps, c);
				for (int npr = 2; npr <= kmax; npr++) {
					c = PowRound(exp2p, npr).Subtract(BigDecimal.One);
					c = MultiplyRound(c, (BigInteger.ValueOf(npr)).Pow(n));

					BigDecimal d = DivideRound(1, exp2p.Pow(npr));
					d = BigDecimal.One.Subtract(d);
					d = DivideRound(twop, d).Multiply(new BigDecimal(2*npr));
					d = DivideRound(d, n - 1).Add(BigDecimal.One);

					d = DivideRound(d, c);

					exps = exps.Add(d);
				}
			}
			exps = exps.Multiply(new BigDecimal(2));
			return ftrm.Subtract(exps, mc);
		}

		public static double Zeta1(int n) {
			/* precomputed static table in double precision
				*/
			double[] zmin1 = {
				0d, 0d,
				6.449340668482264364724151666e-01,
				2.020569031595942853997381615e-01, 8.232323371113819151600369654e-02,
				3.692775514336992633136548646e-02, 1.734306198444913971451792979e-02,
				8.349277381922826839797549850e-03, 4.077356197944339378685238509e-03,
				2.008392826082214417852769232e-03, 9.945751278180853371459589003e-04,
				4.941886041194645587022825265e-04, 2.460865533080482986379980477e-04,
				1.227133475784891467518365264e-04, 6.124813505870482925854510514e-05,
				3.058823630702049355172851064e-05, 1.528225940865187173257148764e-05,
				7.637197637899762273600293563e-06, 3.817293264999839856461644622e-06,
				1.908212716553938925656957795e-06, 9.539620338727961131520386834e-07,
				4.769329867878064631167196044e-07, 2.384505027277329900036481868e-07,
				1.192199259653110730677887189e-07, 5.960818905125947961244020794e-08,
				2.980350351465228018606370507e-08, 1.490155482836504123465850663e-08,
				7.450711789835429491981004171e-09, 3.725334024788457054819204018e-09,
				1.862659723513049006403909945e-09, 9.313274324196681828717647350e-10,
				4.656629065033784072989233251e-10, 2.328311833676505492001455976e-10,
				1.164155017270051977592973835e-10, 5.820772087902700889243685989e-11,
				2.910385044497099686929425228e-11, 1.455192189104198423592963225e-11,
				7.275959835057481014520869012e-12, 3.637979547378651190237236356e-12,
				1.818989650307065947584832101e-12, 9.094947840263889282533118387e-13,
				4.547473783042154026799112029e-13, 2.273736845824652515226821578e-13,
				1.136868407680227849349104838e-13, 5.684341987627585609277182968e-14,
				2.842170976889301855455073705e-14, 1.421085482803160676983430714e-14,
				7.105427395210852712877354480e-15, 3.552713691337113673298469534e-15,
				1.776356843579120327473349014e-15, 8.881784210930815903096091386e-16,
				4.440892103143813364197770940e-16, 2.220446050798041983999320094e-16,
				1.110223025141066133720544570e-16, 5.551115124845481243723736590e-17,
				2.775557562136124172581632454e-17, 1.387778780972523276283909491e-17,
				6.938893904544153697446085326e-18, 3.469446952165922624744271496e-18,
				1.734723476047576572048972970e-18, 8.673617380119933728342055067e-19,
				4.336808690020650487497023566e-19, 2.168404344997219785013910168e-19,
				1.084202172494241406301271117e-19, 5.421010862456645410918700404e-20,
				2.710505431223468831954621312e-20, 1.355252715610116458148523400e-20,
				6.776263578045189097995298742e-21, 3.388131789020796818085703100e-21,
				1.694065894509799165406492747e-21, 8.470329472546998348246992609e-22,
				4.235164736272833347862270483e-22, 2.117582368136194731844209440e-22,
				1.058791184068023385226500154e-22, 5.293955920339870323813912303e-23,
				2.646977960169852961134116684e-23, 1.323488980084899080309451025e-23,
				6.617444900424404067355245332e-24, 3.308722450212171588946956384e-24,
				1.654361225106075646229923677e-24, 8.271806125530344403671105617e-25,
				4.135903062765160926009382456e-25, 2.067951531382576704395967919e-25,
				1.033975765691287099328409559e-25, 5.169878828456431320410133217e-26,
				2.584939414228214268127761771e-26, 1.292469707114106670038112612e-26,
				6.462348535570531803438002161e-27, 3.231174267785265386134814118e-27,
				1.615587133892632521206011406e-27, 8.077935669463162033158738186e-28,
				4.038967834731580825622262813e-28, 2.019483917365790349158762647e-28,
				1.009741958682895153361925070e-28, 5.048709793414475696084771173e-29,
				2.524354896707237824467434194e-29, 1.262177448353618904375399966e-29,
				6.310887241768094495682609390e-30, 3.155443620884047239109841220e-30,
				1.577721810442023616644432780e-30, 7.888609052210118073520537800e-31
			};
			if (n <= 0)
				throw new NotSupportedException("Zeta at negative argument " + n + " not supported.");
			if (n == 1)
				throw new ArithmeticException("Pole at zeta(1) ");

			if (n < zmin1.Length)
				/* look it up if available */
				return zmin1[n];
			else {
				/* Result is roughly 2^(-n), desired accuracy 18 digits. If zeta(n) is computed, the equivalent accuracy
						* in relative units is higher, because zeta is around 1.
						*/
				double eps = 1.0e-18*System.Math.Pow(2d, (double) (-n));
				MathContext mc = new MathContext(ErrorToPrecision(eps));
				return Zeta(n, mc).Subtract(BigDecimal.One).ToDouble();
			}
		}

		public static BigDecimal Cbrt(BigDecimal x) {
			if (x.CompareTo(BigDecimal.Zero) < 0)
				return Root(3, x.Negate()).Negate();
			return Root(3, x);
		}

		public static BigDecimal Root(int n, BigDecimal x) {
			if (x.CompareTo(BigDecimal.Zero) < 0)
				throw new ArithmeticException("negative argument " + x + " of root");
			if (n <= 0)
				throw new ArithmeticException("negative power " + n + " of root");

			if (n == 1)
				return x;

			/* start the computation from a double precision estimate */
			var s = new BigDecimal(System.Math.Pow(x.ToDouble(), 1.0/n));

			/* this creates nth with nominal precision of 1 digit
                */
			var nth = new BigDecimal(n);

			/* Specify an internal accuracy within the loop which is
                * slightly larger than what is demanded by 'eps' below.
                */
			BigDecimal xhighpr = ScalePrecision(x, 2);
			var mc = new MathContext(2 + x.Precision);

			/* Relative accuracy of the result is eps.
                */
			double eps = x.Ulp().ToDouble()/(2*n*x.ToDouble());
			for (;;) {
				/* s = s -(s/n-x/n/s^(n-1)) = s-(s-x/s^(n-1))/n; test correction s/n-x/s for being
                        * smaller than the precision requested. The relative correction is (1-x/s^n)/n,
                        */
				BigDecimal c = xhighpr.Divide(s.Pow(n - 1), mc);
				c = s.Subtract(c);
				var locmc = new MathContext(c.Precision);
				c = c.Divide(nth, locmc);
				s = s.Subtract(c);
				if (System.Math.Abs(c.ToDouble()/s.ToDouble()) < eps)
					break;
			}
			return s.Round(new MathContext(ErrorToPrecision(eps)));
		}

		public static BigDecimal Hypot(BigDecimal x, BigDecimal y) {
			/* compute x^2+y^2
                */
			BigDecimal z = x.Pow(2).Add(y.Pow(2));

			/* truncate to the precision set by x and y. Absolute error = 2*x*xerr+2*y*yerr,
                * where the two errors are 1/2 of the ulp's.  Two intermediate protectio digits.
                */
			BigDecimal zerr = x.Abs().Multiply(x.Ulp()).Add(y.Abs().Multiply(y.Ulp()));
			var mc = new MathContext(2 + ErrorToPrecision(z, zerr));

			/* Pull square root */
			z = Sqrt(z.Round(mc));

			/* Final rounding. Absolute error in the square root is (y*yerr+x*xerr)/z, where zerr holds 2*(x*xerr+y*yerr).
                */
			mc = new MathContext(ErrorToPrecision(z.ToDouble(), 0.5*zerr.ToDouble()/z.ToDouble()));
			return z.Round(mc);
		}

		public static BigDecimal Hypot(int n, BigDecimal x) {
			/* compute n^2+x^2 in infinite precision
                */
			BigDecimal z = (new BigDecimal(n)).Pow(2).Add(x.Pow(2));

			/* Truncate to the precision set by x. Absolute error = in z (square of the result) is |2*x*xerr|,
                * where the error is 1/2 of the ulp. Two intermediate protection digits.
                * zerr is a signed value, but used only in conjunction with err2prec(), so this feature does not harm. 
                */
			double zerr = x.ToDouble()*x.Ulp().ToDouble();
			var mc = new MathContext(2 + ErrorToPrecision(z.ToDouble(), zerr));

			/* Pull square root */
			z = Sqrt(z.Round(mc));

			/* Final rounding. Absolute error in the square root is x*xerr/z, where zerr holds 2*x*xerr.
                */
			mc = new MathContext(ErrorToPrecision(z.ToDouble(), 0.5*zerr/z.ToDouble()));
			return z.Round(mc);
		}

		public static BigDecimal Log(BigDecimal x) {
			/* the value is undefined if x is negative.
                */
			if (x.CompareTo(BigDecimal.Zero) < 0)
				throw new ArithmeticException("Cannot take log of negative " + x);
			if (x.CompareTo(BigDecimal.One) == 0) {
				/* log 1. = 0. */
				return ScalePrecision(BigDecimal.Zero, x.Precision - 1);
			}
			if (System.Math.Abs(x.ToDouble() - 1.0) <= 0.3) {
				/* The standard Taylor series around x=1, z=0, z=x-1. Abramowitz-Stegun 4.124.
                        * The absolute error is err(z)/(1+z) = err(x)/x.
                        */
				BigDecimal z = ScalePrecision(x.Subtract(BigDecimal.One), 2);
				BigDecimal zpown = z;
				double eps = 0.5*x.Ulp().ToDouble()/System.Math.Abs(x.ToDouble());
				BigDecimal resul = z;
				for (int k = 2;; k++) {
					zpown = MultiplyRound(zpown, z);
					BigDecimal c = DivideRound(zpown, k);
					if (k%2 == 0)
						resul = resul.Subtract(c);
					else
						resul = resul.Add(c);
					if (System.Math.Abs(c.ToDouble()) < eps)
						break;
				}
				var mc = new MathContext(ErrorToPrecision(resul.ToDouble(), eps));
				return resul.Round(mc);
			} else {
				double xDbl = x.ToDouble();
				double xUlpDbl = x.Ulp().ToDouble();

				/* Map log(x) = log root[r](x)^r = r*log( root[r](x)) with the aim
                        * to move roor[r](x) near to 1.2 (that is, below the 0.3 appearing above), where log(1.2) is roughly 0.2.
                        */
				var r = (int) (System.Math.Log(xDbl)/0.2);

				/* Since the actual requirement is a function of the value 0.3 appearing above,
                        * we avoid the hypothetical case of endless recurrence by ensuring that r >= 2.
                        */
				r = System.Math.Max(2, r);

				/* Compute r-th root with 2 additional digits of precision
                        */
				BigDecimal xhighpr = ScalePrecision(x, 2);
				BigDecimal resul = Root(r, xhighpr);
				resul = Log(resul).Multiply(new BigDecimal(r));

				/* error propagation: log(x+errx) = log(x)+errx/x, so the absolute error
                        * in the result equals the relative error in the input, xUlpDbl/xDbl .
                        */
				var mc = new MathContext(ErrorToPrecision(resul.ToDouble(), xUlpDbl/xDbl));
				return resul.Round(mc);
			}
		}

		public static BigDecimal Log(int n, MathContext mc) {
			/* the value is undefined if x is negative.
                */
			if (n <= 0)
				throw new ArithmeticException("Cannot take log of negative " + n);
			if (n == 1)
				return BigDecimal.Zero;
			if (n == 2) {
				if (mc.Precision < Log2.Precision)
					return Log2.Round(mc);
				/* Broadhurst <a href="http://arxiv.org/abs/math/9803067">arXiv:math/9803067</a>
                                * Error propagation: the error in log(2) is twice the error in S(2,-5,...).
                                */
				int[] a = {2, -5, -2, -7, -2, -5, 2, -3};
				BigDecimal S = BroadhurstBbp(2, 1, a, new MathContext(1 + mc.Precision));
				S = S.Multiply(new BigDecimal(8));
				S = Sqrt(DivideRound(S, 3));
				return S.Round(mc);
			}
			if (n == 3) {
				/* summation of a series roughly proportional to (7/500)^k. Estimate count
                        * of terms to estimate the precision (drop the favorable additional
                        * 1/k here): 0.013^k <= 10^(-precision), so k*log10(0.013) <= -precision
                        * so k>= precision/1.87.
                        */
				var kmax = (int) (mc.Precision/1.87);
				var mcloc = new MathContext(mc.Precision + 1 + (int) (System.Math.Log10(kmax*0.693/1.098)));
				BigDecimal log3 = MultiplyRound(Log(2, mcloc), 19);

				/* log3 is roughly 1, so absolute and relative error are the same. The
                        * result will be divided by 12, so a conservative error is the one
                        * already found in mc
                        */
				double eps = PrecisionToError(1.098, mc.Precision)/kmax;
				var r = new Rational(7153, 524288);
				var pk = new Rational(7153, 524288);
				for (int k = 1;; k++) {
					Rational tmp = pk.Divide(k);
					if (tmp.ToDouble() < eps)
						break;

					/* how many digits of tmp do we need in the sum?
                                */
					mcloc = new MathContext(ErrorToPrecision(tmp.ToDouble(), eps));
					BigDecimal c = pk.Divide(k).ToBigDecimal(mcloc);
					if (k%2 != 0)
						log3 = log3.Add(c);
					else
						log3 = log3.Subtract(c);
					pk = pk.Multiply(r);
				}
				log3 = DivideRound(log3, 12);
				return log3.Round(mc);
			}
			if (n == 5) {
				/* summation of a series roughly proportional to (7/160)^k. Estimate count
                        * of terms to estimate the precision (drop the favorable additional
                        * 1/k here): 0.046^k <= 10^(-precision), so k*log10(0.046) <= -precision
                        * so k>= precision/1.33.
                        */
				var kmax = (int) (mc.Precision/1.33);
				var mcloc = new MathContext(mc.Precision + 1 + (int) (System.Math.Log10(kmax*0.693/1.609)));
				BigDecimal log5 = MultiplyRound(Log(2, mcloc), 14);

				/* log5 is roughly 1.6, so absolute and relative error are the same. The
                        * result will be divided by 6, so a conservative error is the one
                        * already found in mc
                        */
				double eps = PrecisionToError(1.6, mc.Precision)/kmax;
				var r = new Rational(759, 16384);
				var pk = new Rational(759, 16384);
				for (int k = 1;; k++) {
					Rational tmp = pk.Divide(k);
					if (tmp.ToDouble() < eps)
						break;

					/* how many digits of tmp do we need in the sum?
                                */
					mcloc = new MathContext(ErrorToPrecision(tmp.ToDouble(), eps));
					BigDecimal c = pk.Divide(k).ToBigDecimal(mcloc);
					log5 = log5.Subtract(c);
					pk = pk.Multiply(r);
				}
				log5 = DivideRound(log5, 6);
				return log5.Round(mc);
			}
			if (n == 7) {
				/* summation of a series roughly proportional to (1/8)^k. Estimate count
                        * of terms to estimate the precision (drop the favorable additional
                        * 1/k here): 0.125^k <= 10^(-precision), so k*log10(0.125) <= -precision
                        * so k>= precision/0.903.
                        */
				var kmax = (int) (mc.Precision/0.903);
				var mcloc = new MathContext(mc.Precision + 1 + (int) (System.Math.Log10(kmax*3*0.693/1.098)));
				BigDecimal log7 = MultiplyRound(Log(2, mcloc), 3);

				/* log7 is roughly 1.9, so absolute and relative error are the same.
                        */
				double eps = PrecisionToError(1.9, mc.Precision)/kmax;
				var r = new Rational(1, 8);
				var pk = new Rational(1, 8);
				for (int k = 1;; k++) {
					Rational tmp = pk.Divide(k);
					if (tmp.ToDouble() < eps)
						break;

					/* how many digits of tmp do we need in the sum?
                                */
					mcloc = new MathContext(ErrorToPrecision(tmp.ToDouble(), eps));
					BigDecimal c = pk.Divide(k).ToBigDecimal(mcloc);
					log7 = log7.Subtract(c);
					pk = pk.Multiply(r);
				}
				return log7.Round(mc);
			} else {
				/* At this point one could either forward to the log(BigDecimal) signature (implemented)
                        * or decompose n into Ifactors and use an implemenation of all the prime bases.
                        * Estimate of the result; convert the mc argument to an  absolute error eps
                        * log(n+errn) = log(n)+errn/n = log(n)+eps
                        */
				double res = System.Math.Log(n);
				double eps = PrecisionToError(res, mc.Precision);
				/* errn = eps*n, convert absolute error in result to requirement on absolute error in input
                        */
				eps *= n;
				/* Convert this absolute requirement of error in n to a relative error in n
                        */
				var mcloc = new MathContext(1 + ErrorToPrecision(n, eps));
				/* Padd n with a number of zeros to trigger the required accuracy in
                        * the standard signature method
                        */
				BigDecimal nb = ScalePrecision(new BigDecimal(n), mcloc);
				return Log(nb);
			}
		}

		public static BigDecimal Log(Rational r, MathContext mc) {
			/* the value is undefined if x is negative.
                */
			if (r.CompareTo(Rational.Zero) <= 0)
				throw new ArithmeticException("Cannot take log of negative " + r);
			if (r.CompareTo(Rational.One) == 0)
				return BigDecimal.Zero;
			/* log(r+epsr) = log(r)+epsr/r. Convert the precision to an absolute error in the result.
                        * eps contains the required absolute error of the result, epsr/r.
                        */
			double eps = PrecisionToError(System.Math.Log(r.ToDouble()), mc.Precision);

			/* Convert this further into a requirement of the relative precision in r, given that
                        * epsr/r is also the relative precision of r. Add one safety digit.
                        */
			var mcloc = new MathContext(1 + ErrorToPrecision(eps));

			BigDecimal resul = Log(r.ToBigDecimal(mcloc));

			return resul.Round(mc);
		}

		public static BigDecimal Pow(BigDecimal x, BigDecimal y) {
			if (x.CompareTo(BigDecimal.Zero) < 0)
				throw new ArithmeticException("Cannot power negative " + x);
			if (x.CompareTo(BigDecimal.Zero) == 0)
				return BigDecimal.Zero;
			/* return x^y = exp(y*log(x)) ;
                        */
			BigDecimal logx = Log(x);
			BigDecimal ylogx = y.Multiply(logx);
			BigDecimal resul = Exp(ylogx);

			/* The estimation of the relative error in the result is |log(x)*err(y)|+|y*err(x)/x| 
                        */
			double errR = System.Math.Abs(logx.ToDouble())*y.Ulp().ToDouble()/2d
			              + System.Math.Abs(y.ToDouble()*x.Ulp().ToDouble()/2d/x.ToDouble());
			var mcR = new MathContext(ErrorToPrecision(1.0, errR));
			return resul.Round(mcR);
		}

		public static BigDecimal PowRound(BigDecimal x, int n) {
			/** Special cases: x^1=x and x^0 = 1
                */
			if (n == 1)
				return x;
			if (n == 0)
				return BigDecimal.One;
			/* The relative error in the result is n times the relative error in the input.
                        * The estimation is slightly optimistic due to the integer rounding of the logarithm.
                        * Since the standard BigDecimal.pow can only handle positive n, we split the algorithm.
                        */
			var mc = new MathContext(x.Precision - (int) System.Math.Log10(System.Math.Abs(n)));
			if (n > 0)
				return x.Pow(n, mc);
			return BigDecimal.One.Divide(x.Pow(-n), mc);
		}

		public static BigDecimal PowRound(BigDecimal x, BigInteger n) {
			/** For now, the implementation forwards to the cases where n
                * is in the range of the standard integers. This might, however, be
                * implemented to decompose larger powers into cascaded calls to smaller ones.
                */
			if (n.CompareTo(Rational.MaxInt32) > 0 ||
			    n.CompareTo(Rational.MinInt32) < 0)
				throw new NotSupportedException("Big power for " + n + " not supported");
			return PowRound(x, n.ToInt32());
		}

		public static BigDecimal PowRound(BigDecimal x, Rational q) {
			/** Special cases: x^1=x and x^0 = 1
                */
			if (q.CompareTo(BigInteger.One) == 0)
				return x;
			if (q.Sign == 0)
				return BigDecimal.One;
			if (q.IsInteger) {
				/* We are sure that the denominator is positive here, because normalize() has been
                        * called during constrution etc.
                        */
				return PowRound(x, q.Numerator);
			}
			/* Refuse to operate on the general negative basis. The integer q have already been handled above.
                        */
			if (x.CompareTo(BigDecimal.Zero) < 0)
				throw new ArithmeticException("Cannot power negative " + x);
			if (q.IsIntegerFraction) {
				/* Newton method with first estimate in double precision.
                                * The disadvantage of this first line here is that the result must fit in the
                                * standard range of double precision numbers exponents.
                                */
				double estim = System.Math.Pow(x.ToDouble(), q.ToDouble());
				var res = new BigDecimal(estim);

				/* The error in x^q is q*x^(q-1)*Delta(x).
                                * The relative error is q*Delta(x)/x, q times the relative error of x.
                                */
				var reserr = new BigDecimal(0.5*q.Abs().ToDouble()
				                            *x.Ulp().Divide(x.Abs(), MathContext.Decimal64).ToDouble());

				/* The main point in branching the cases above is that this conversion
                                * will succeed for numerator and denominator of q.
                                */
				int qa = q.Numerator.ToInt32();
				int qb = q.Denominator.ToInt32();

				/* Newton iterations. */
				BigDecimal xpowa = PowRound(x, qa);
				for (;;) {
					/* numerator and denominator of the Newton term.  The major
                                        * disadvantage of this implementation is that the updates of the powers
                                        * of the new estimate are done in full precision calling BigDecimal.pow(),
                                        * which becomes slow if the denominator of q is large.
                                        */
					BigDecimal nu = res.Pow(qb).Subtract(xpowa);
					BigDecimal de = MultiplyRound(res.Pow(qb - 1), q.Denominator);

					/* estimated correction */
					BigDecimal eps = nu.Divide(de, MathContext.Decimal64);

					BigDecimal err = res.Multiply(reserr, MathContext.Decimal64);
					int precDiv = 2 + ErrorToPrecision(eps, err);
					if (precDiv <= 0) {
						/* The case when the precision is already reached and any precision
                                                * will do. */
						eps = nu.Divide(de, MathContext.Decimal32);
					} else {
						eps = nu.Divide(de, new MathContext(precDiv));
					}

					res = SubtractRound(res, eps);
					/* reached final precision if the relative error fell below reserr,
                                        * |eps/res| < reserr
                                        */
					if (eps.Divide(res, MathContext.Decimal64).Abs().CompareTo(reserr) < 0) {
						/* delete the bits of extra precision kept in this
                                                * working copy.
                                                */
						return res.Round(new MathContext(ErrorToPrecision(reserr.ToDouble())));
					}
				}
			}
			/* The error in x^q is q*x^(q-1)*Delta(x) + Delta(q)*x^q*log(x).
                                * The relative error is q/x*Delta(x) + Delta(q)*log(x). Convert q to a floating point
                                * number such that its relative error becomes negligible: Delta(q)/q << Delta(x)/x/log(x) .
                                */
			int precq = 3 + ErrorToPrecision((x.Ulp().Divide(x, MathContext.Decimal64)).ToDouble()
			                                 /System.Math.Log(x.ToDouble()));

			/* Perform the actual calculation as exponentiation of two floating point numbers.
                                */
			return Pow(x, q.ToBigDecimal(new MathContext(precq)));
		}

		public static double PrecisionToError(double x, int prec) {
			return 5d*System.Math.Abs(x)*System.Math.Pow(10d, -prec);
		}

		public static int ErrorToPrecision(double xerr) {
			// Example: an error of xerr=+-0.5 a precision of 1 (digit), an error of
			// +-0.05 a precision of 2 (digits)
			return 1 + (int) (System.Math.Log10(System.Math.Abs(0.5/xerr)));
		}

		public static int ErrorToPrecision(double x, double xerr) {
			// Example: an error of xerr=+-0.5 at x=100 represents 100+-0.5 with
			// a precision = 3 (digits).
			return 1 + (int) (System.Math.Log10(System.Math.Abs(0.5*x/xerr)));
		}

		public static int ErrorToPrecision(BigDecimal x, BigDecimal xerr) {
			return ErrorToPrecision(xerr.Divide(x, MathContext.Decimal64).ToDouble());
		}

		private static BigDecimal BroadhurstBbp(int n, int p, int[] a, MathContext mc) {
			/* Explore the actual magnitude of the result first with a quick estimate.
                */
			double x = 0.0;
			for (int k = 1; k < 10; k++)
				x += a[(k - 1)%8]/System.Math.Pow(2d, p*(k + 1)/2d)/System.Math.Pow(k, n);

			/* Convert the relative precision and estimate of the result into an absolute precision.
                */
			double eps = PrecisionToError(x, mc.Precision);

			/* Divide this through the number of terms in the sum to account for error accumulation
                * The divisor 2^(p(k+1)/2) means that on the average each 8th term in k has shrunk by
                * relative to the 8th predecessor by 1/2^(4p).  1/2^(4pc) = 10^(-precision) with c the 8term
                * cycles yields c=log_2( 10^precision)/4p = 3.3*precision/4p  with k=8c
                */
			var kmax = (int) (6.6*mc.Precision/p);

			/* Now eps is the absolute error in each term */
			eps /= kmax;
			BigDecimal res = BigDecimal.Zero;
			for (int c = 0;; c++) {
				var r = new Rational();
				for (int k = 0; k < 8; k++) {
					var tmp = new Rational(BigInteger.ValueOf(a[k]), (BigInteger.ValueOf((1 + 8*c + k))).Pow(n));
					/* floor( (pk+p)/2)
                                */
					int pk1h = p*(2 + 8*c + k)/2;
					tmp = tmp.Divide(BigInteger.One.ShiftLeft(pk1h));
					r = r.Add(tmp);
				}

				if (System.Math.Abs(r.ToDouble()) < eps)
					break;
				var mcloc = new MathContext(1 + ErrorToPrecision(r.ToDouble(), eps));
				res = res.Add(r.ToBigDecimal(mcloc));
			}
			return res.Round(mc);
		}

		public static BigDecimal Add(BigDecimal x, BigInteger y) {
			return x.Add(new BigDecimal(y));
		}

		public static BigDecimal AddRound(BigDecimal x, BigDecimal y) {
			BigDecimal resul = x.Add(y);
			/* The estimation of the absolute error in the result is |err(y)|+|err(x)| 
                */
			double errR = System.Math.Abs(y.Ulp().ToDouble()/2d) + System.Math.Abs(x.Ulp().ToDouble()/2d);
			MathContext mc = new MathContext(ErrorToPrecision(resul.ToDouble(), errR));
			return resul.Round(mc);
		}

		public static BigComplex AddRound(BigComplex x, BigDecimal y) {
			BigDecimal R = AddRound(x.Real, y);
			return new BigComplex(R, x.Imaginary);
		}

		public static BigComplex AddRound(BigComplex x, BigComplex y) {
			BigDecimal R = AddRound(x.Real, y.Real);
			BigDecimal I = AddRound(x.Imaginary, y.Imaginary);
			return new BigComplex(R, I);
		}

		public static BigDecimal DivideRound(BigDecimal x, BigDecimal y) {
			/* The estimation of the relative error in the result is |err(y)/y|+|err(x)/x| 
                */
			var mc = new MathContext(System.Math.Min(x.Precision, y.Precision));
			BigDecimal resul = x.Divide(y, mc);
			/* If x and y are precise integer values that may have common factors,
                * the method above will truncate trailing zeros, which may result in
                * a smaller apparent accuracy than starte... add missing trailing zeros now.
                */
			return ScalePrecision(resul, mc);
		}

		public static BigDecimal DivideRound(int n, BigDecimal x) {
			// The estimation of the relative error in the result is |err(x)/x| 
			var mc = new MathContext(x.Precision);
			return new BigDecimal(n).Divide(x, mc);
		}

		public static BigDecimal DivideRound(BigInteger n, BigDecimal x) {
			// The estimation of the relative error in the result is |err(x)/x| 
			var mc = new MathContext(x.Precision);
			return new BigDecimal(n).Divide(x, mc);
		}

		public static BigDecimal DivideRound(BigDecimal x, BigInteger n) {
			// The estimation of the relative error in the result is |err(x)/x| 
			var mc = new MathContext(x.Precision);
			return x.Divide(new BigDecimal(n), mc);
		}

		public static BigDecimal DivideRound(BigDecimal x, int n) {
			// The estimation of the relative error in the result is |err(x)/x| 
			var mc = new MathContext(x.Precision);
			return x.Divide(new BigDecimal(n), mc);
		}

		public static BigComplex DivideRound(BigComplex x, BigComplex y) {
			return MultiplyRound(x, InvertRound(y));
		}

		public static BigComplex DivideRound(BigInteger n, BigComplex x) {
			/* catch case of real-valued denominator first
                */
			if (x.Imaginary.CompareTo(BigDecimal.Zero) == 0)
				return new BigComplex(DivideRound(n, x.Real), BigDecimal.Zero);
			else if (x.Real.CompareTo(BigDecimal.Zero) == 0)
				return new BigComplex(BigDecimal.Zero, DivideRound(n, x.Imaginary).Negate());

			BigComplex z = InvertRound(x);
			/* n/(x+iy) = nx/(x^2+y^2) -nyi/(x^2+y^2)       
                */
			BigDecimal repart = MultiplyRound(z.Real, n);
			BigDecimal impart = MultiplyRound(z.Imaginary, n);
			return new BigComplex(repart, impart);
		}

		public static BigDecimal MultiplyRound(BigDecimal x, int n) {
			BigDecimal resul = x.Multiply(new BigDecimal(n));
			// The estimation of the absolute error in the result is |n*err(x)|
			var mc = new MathContext(n != 0 ? x.Precision : 0);
			return resul.Round(mc);
		}

		public static BigDecimal MultiplyRound(BigDecimal x, BigInteger n) {
			BigDecimal resul = x.Multiply(new BigDecimal(n));
			// The estimation of the absolute error in the result is |n*err(x)|
			var mc = new MathContext(n.CompareTo(BigInteger.Zero) != 0 ? x.Precision : 0);
			return resul.Round(mc);
		}

		public static BigDecimal MultiplyRound(BigDecimal x, BigDecimal y) {
			BigDecimal resul = x.Multiply(y);
			// The estimation of the relative error in the result is the sum of the relative
			// errors |err(y)/y|+|err(x)/x| 
			var mc = new MathContext(System.Math.Min(x.Precision, y.Precision));
			return resul.Round(mc);
		}

		public static BigComplex MultiplyRound(BigComplex x, BigDecimal y) {
			BigDecimal R = MultiplyRound(x.Real, y);
			BigDecimal I = MultiplyRound(x.Imaginary, y);
			return new BigComplex(R, I);
		}

		public static BigComplex MultiplyRound(BigComplex x, BigComplex y) {
			BigDecimal R = SubtractRound(MultiplyRound(x.Real, y.Real), MultiplyRound(x.Imaginary, y.Imaginary));
			BigDecimal I = AddRound(MultiplyRound(x.Real, y.Imaginary), MultiplyRound(x.Imaginary, y.Real));
			return new BigComplex(R, I);
		}

		public static BigDecimal MultiplyRound(BigDecimal x, Rational f) {
			if (f.CompareTo(BigInteger.Zero) == 0)
				return BigDecimal.Zero;
			/* Convert the rational value with two digits of extra precision
                        */
			var mc = new MathContext(2 + x.Precision);
			BigDecimal fbd = f.ToBigDecimal(mc);

			/* and the precision of the product is then dominated by the precision in x
                        */
			return MultiplyRound(x, fbd);
		}

		public static BigDecimal SubtractRound(BigDecimal x, BigDecimal y) {
			BigDecimal resul = x.Subtract(y);
			// The estimation of the absolute error in the result is |err(y)|+|err(x)| 
			double errR = System.Math.Abs(y.Ulp().ToDouble()/2d) + System.Math.Abs(x.Ulp().ToDouble()/2d);
			var mc = new MathContext(ErrorToPrecision(resul.ToDouble(), errR));
			return resul.Round(mc);
		}

		public static BigComplex SubtractRound(BigComplex x, BigComplex y) {
			BigDecimal R = SubtractRound(x.Real, y.Real);
			BigDecimal I = SubtractRound(x.Imaginary, y.Imaginary);
			return new BigComplex(R, I);
		}

		public static BigComplex InvertRound(BigComplex z) {
			if (z.Imaginary.CompareTo(BigDecimal.Zero) == 0) {
				/* In this case with vanishing Im(x), the result is  simply 1/Re z.
                        */
				MathContext mc = new MathContext(z.Real.Precision);
				return new BigComplex(BigDecimal.One.Divide(z.Real, mc));
			} else if (z.Real.CompareTo(BigDecimal.Zero) == 0) {
				/* In this case with vanishing Re(z), the result is  simply -i/Im z
                        */
				MathContext mc = new MathContext(z.Imaginary.Precision);
				return new BigComplex(BigDecimal.Zero, BigDecimal.One.Divide(z.Imaginary, mc).Negate());
			} else {
				/* 1/(x.re+I*x.im) = 1/(x.re+x.im^2/x.re) - I /(x.im +x.re^2/x.im)
                        */
				BigDecimal R = AddRound(z.Real, DivideRound(MultiplyRound(z.Imaginary, z.Imaginary), z.Real));
				BigDecimal I = AddRound(z.Imaginary, DivideRound(MultiplyRound(z.Real, z.Real), z.Imaginary));
				MathContext mc = new MathContext(1 + R.Precision);
				R = BigDecimal.One.Divide(R, mc);
				mc = new MathContext(1 + I.Precision);
				I = BigDecimal.One.Divide(I, mc);
				return new BigComplex(R, I.Negate());
			}
		}

		public static BigDecimal ScalePrecision(BigDecimal x, int d) {
			return x.SetScale(d + x.Scale);
		}

		public static BigDecimal ScalePrecision(BigDecimal x, MathContext mc) {
			int diffPr = mc.Precision - x.Precision;
			if (diffPr > 0)
				return ScalePrecision(x, diffPr);
			return x;
		}

		public static BigDecimal Exp(BigDecimal x) {
			/* To calculate the value if x is negative, use exp(-x) = 1/exp(x)
                */
			if (x.CompareTo(BigDecimal.Zero) < 0) {
				BigDecimal invx = Exp(x.Negate());
				/* Relative error in inverse of invx is the same as the relative errror in invx.
                        * This is used to define the precision of the result.
                        */
				var mc = new MathContext(invx.Precision);
				return BigDecimal.One.Divide(invx, mc);
			}
			if (x.CompareTo(BigDecimal.Zero) == 0) {
				/* recover the valid number of digits from x.ulp(), if x hits the
                        * zero. The x.precision() is 1 then, and does not provide this information.
                        */
				return ScalePrecision(BigDecimal.One, -(int) (System.Math.Log10(x.Ulp().ToDouble())));
			}
			/* Push the number in the Taylor expansion down to a small
                        * value where TAYLOR_NTERM terms will do. If x<1, the n-th term is of the order
                        * x^n/n!, and equal to both the absolute and relative error of the result
                        * since the result is close to 1. The x.ulp() sets the relative and absolute error
                        * of the result, as estimated from the first Taylor term.
                        * We want x^TAYLOR_NTERM/TAYLOR_NTERM! < x.ulp, which is guaranteed if
                        * x^TAYLOR_NTERM < TAYLOR_NTERM*(TAYLOR_NTERM-1)*...*x.ulp.
                        */
			double xDbl = x.ToDouble();
			double xUlpDbl = x.Ulp().ToDouble();
			if (System.Math.Pow(xDbl, TaylorNterm) < TaylorNterm*(TaylorNterm - 1.0)*(TaylorNterm - 2.0)*xUlpDbl) {
				/* Add TAYLOR_NTERM terms of the Taylor expansion (Euler's sum formula)
                                */
				BigDecimal resul = BigDecimal.One;

				/* x^i */
				BigDecimal xpowi = BigDecimal.One;

				/* i factorial */
				BigInteger ifac = BigInteger.One;

				/* TAYLOR_NTERM terms to be added means we move x.ulp() to the right
                                * for each power of 10 in TAYLOR_NTERM, so the addition won't add noise beyond
                                * what's already in x.
                                */
				var mcTay = new MathContext(ErrorToPrecision(1d, xUlpDbl/TaylorNterm));
				for (int i = 1; i <= TaylorNterm; i++) {
					ifac = ifac.Multiply(BigInteger.ValueOf(i));
					xpowi = xpowi.Multiply(x);
					BigDecimal c = xpowi.Divide(new BigDecimal(ifac), mcTay);
					resul = resul.Add(c);
					if (System.Math.Abs(xpowi.ToDouble()) < i &&
					    System.Math.Abs(c.ToDouble()) < 0.5*xUlpDbl)
						break;
				}
				/* exp(x+deltax) = exp(x)(1+deltax) if deltax is <<1. So the relative error
                                * in the result equals the absolute error in the argument.
                                */
				var mc = new MathContext(ErrorToPrecision(xUlpDbl/2d));
				return resul.Round(mc);
			} else {
				/* Compute exp(x) = (exp(0.1*x))^10. Division by 10 does not lead
                                * to loss of accuracy.
                                */
				var exSc = (int) (1.0 - System.Math.Log10(TaylorNterm*(TaylorNterm - 1.0)*(TaylorNterm - 2.0)*xUlpDbl
				                                          /System.Math.Pow(xDbl, TaylorNterm))/(TaylorNterm - 1.0));
				BigDecimal xby10 = x.ScaleByPowerOfTen(-exSc);
				BigDecimal expxby10 = Exp(xby10);

				/* Final powering by 10 means that the relative error of the result
                                * is 10 times the relative error of the base (First order binomial expansion).
                                * This looses one digit.
                                */
				var mc = new MathContext(expxby10.Precision - exSc);
				/* Rescaling the powers of 10 is done in chunks of a maximum of 8 to avoid an invalid operation
                                * response by the BigDecimal.pow library or integer overflow.
                                */
				while (exSc > 0) {
					int exsub = System.Math.Min(8, exSc);
					exSc -= exsub;
					var mctmp = new MathContext(expxby10.Precision - exsub + 2);
					int pex = 1;
					while (exsub-- > 0)
						pex *= 10;
					expxby10 = expxby10.Pow(pex, mctmp);
				}
				return expxby10.Round(mc);
			}
		}

		public static BigDecimal Exp(MathContext mc) {
			/* look it up if possible */
			if (mc.Precision < E.Precision)
				return E.Round(mc);

			/* Instantiate a 1.0 with the requested pseudo-accuracy
                        * and delegate the computation to the public method above.
                        */
			BigDecimal uni = ScalePrecision(BigDecimal.One, mc.Precision);
			return Exp(uni);
		}

		public static BigDecimal Sin(BigDecimal x) {
			if (x.CompareTo(BigDecimal.Zero) < 0)
				return Sin(x.Negate()).Negate();
			if (x.CompareTo(BigDecimal.Zero) == 0)
				return BigDecimal.Zero;
			/* reduce modulo 2pi
                        */
			BigDecimal res = Mod2Pi(x);
			double errpi = 0.5*System.Math.Abs(x.Ulp().ToDouble());
			var mc = new MathContext(2 + ErrorToPrecision(3.14159, errpi));
			BigDecimal p = PiRound(mc);
			mc = new MathContext(x.Precision);
			if (res.CompareTo(p) > 0) {
				/* pi<x<=2pi: sin(x)= - sin(x-pi)
                                */
				return Sin(SubtractRound(res, p)).Negate();
			}
			if (res.Multiply(BigDecimal.ValueOf(2)).CompareTo(p) > 0) {
				/* pi/2<x<=pi: sin(x)= sin(pi-x)
                                */
				return Sin(SubtractRound(p, res));
			}
			/* for the range 0<=x<Pi/2 one could use sin(2x)=2sin(x)cos(x)
                                * to split this further. Here, use the sine up to pi/4 and the cosine higher up.
                                */
			if (res.Multiply(BigDecimal.ValueOf(4)).CompareTo(p) > 0) {
				/* x>pi/4: sin(x) = cos(pi/2-x)
                                        */
				return Cos(SubtractRound(p.Divide(BigDecimal.ValueOf(2)), res));
			}
			/* Simple Taylor expansion, sum_{i=1..infinity} (-1)^(..)res^(2i+1)/(2i+1)! */
			BigDecimal resul = res;

			/* x^i */
			BigDecimal xpowi = res;

			/* 2i+1 factorial */
			BigInteger ifac = BigInteger.One;

			/* The error in the result is set by the error in x itself.
                                        */
			double xUlpDbl = res.Ulp().ToDouble();

			/* The error in the result is set by the error in x itself.
                                        * We need at most k terms to squeeze x^(2k+1)/(2k+1)! below this value.
                                        * x^(2k+1) < x.ulp; (2k+1)*log10(x) < -x.precision; 2k*log10(x)< -x.precision;
                                        * 2k*(-log10(x)) > x.precision; 2k*log10(1/x) > x.precision
                                        */
			int k = (int) (res.Precision/System.Math.Log10(1.0/res.ToDouble()))/2;
			var mcTay = new MathContext(ErrorToPrecision(res.ToDouble(), xUlpDbl/k));
			for (int i = 1;; i++) {
				/* TBD: at which precision will 2*i or 2*i+1 overflow? 
                                                */
				ifac = ifac.Multiply(BigInteger.ValueOf(2*i));
				ifac = ifac.Multiply(BigInteger.ValueOf((2*i + 1)));
				xpowi = xpowi.Multiply(res).Multiply(res).Negate();
				BigDecimal corr = xpowi.Divide(new BigDecimal(ifac), mcTay);
				resul = resul.Add(corr);
				if (corr.Abs().ToDouble() < 0.5*xUlpDbl)
					break;
			}
			/* The error in the result is set by the error in x itself.
                                        */
			mc = new MathContext(res.Precision);
			return resul.Round(mc);
		}

		public static BigDecimal Cos(BigDecimal x) {
			if (x.CompareTo(BigDecimal.Zero) < 0)
				return Cos(x.Negate());
			if (x.CompareTo(BigDecimal.Zero) == 0)
				return BigDecimal.One;
			/* reduce modulo 2pi
                        */
			BigDecimal res = Mod2Pi(x);
			double errpi = 0.5*System.Math.Abs(x.Ulp().ToDouble());
			var mc = new MathContext(2 + ErrorToPrecision(3.14159, errpi));
			BigDecimal p = PiRound(mc);
			mc = new MathContext(x.Precision);
			if (res.CompareTo(p) > 0) {
				/* pi<x<=2pi: cos(x)= - cos(x-pi)
                                */
				return Cos(SubtractRound(res, p)).Negate();
			}
			if (res.Multiply(BigDecimal.ValueOf(2)).CompareTo(p) > 0) {
				/* pi/2<x<=pi: cos(x)= -cos(pi-x)
                                */
				return Cos(SubtractRound(p, res)).Negate();
			}
			/* for the range 0<=x<Pi/2 one could use cos(2x)= 1-2*sin^2(x)
                                * to split this further, or use the cos up to pi/4 and the sine higher up.
                                        throw new ProviderException("Not implemented: cosine ") ;
                                */
			if (res.Multiply(BigDecimal.ValueOf(4)).CompareTo(p) > 0) {
				/* x>pi/4: cos(x) = sin(pi/2-x)
                                        */
				return Sin(SubtractRound(p.Divide(BigDecimal.ValueOf(2)), res));
			}
			/* Simple Taylor expansion, sum_{i=0..infinity} (-1)^(..)res^(2i)/(2i)! */
			BigDecimal resul = BigDecimal.One;

			/* x^i */
			BigDecimal xpowi = BigDecimal.One;

			/* 2i factorial */
			BigInteger ifac = BigInteger.One;

			/* The absolute error in the result is the error in x^2/2 which is x times the error in x.
                                        */
			double xUlpDbl = 0.5*res.Ulp().ToDouble()*res.ToDouble();

			/* The error in the result is set by the error in x^2/2 itself, xUlpDbl.
                                        * We need at most k terms to push x^(2k+1)/(2k+1)! below this value.
                                        * x^(2k) < xUlpDbl; (2k)*log(x) < log(xUlpDbl);
                                        */
			int k = (int) (System.Math.Log(xUlpDbl)/System.Math.Log(res.ToDouble()))/2;
			var mcTay = new MathContext(ErrorToPrecision(1d, xUlpDbl/k));
			for (int i = 1;; i++) {
				/* TBD: at which precision will 2*i-1 or 2*i overflow? 
                                                */
				ifac = ifac.Multiply(BigInteger.ValueOf((2*i - 1)));
				ifac = ifac.Multiply(BigInteger.ValueOf((2*i)));
				xpowi = xpowi.Multiply(res).Multiply(res).Negate();
				BigDecimal corr = xpowi.Divide(new BigDecimal(ifac), mcTay);
				resul = resul.Add(corr);
				if (corr.Abs().ToDouble() < 0.5*xUlpDbl)
					break;
			}
			/* The error in the result is governed by the error in x itself.
                                        */
			mc = new MathContext(ErrorToPrecision(resul.ToDouble(), xUlpDbl));
			return resul.Round(mc);
		}

		public static BigDecimal Tan(BigDecimal x) {
			if (x.CompareTo(BigDecimal.Zero) == 0)
				return BigDecimal.Zero;
			if (x.CompareTo(BigDecimal.Zero) < 0) {
				return Tan(x.Negate()).Negate();
			}
			/* reduce modulo pi
                        */
			BigDecimal res = ModPi(x);

			/* absolute error in the result is err(x)/cos^2(x) to lowest order
                        */
			double xDbl = res.ToDouble();
			double xUlpDbl = x.Ulp().ToDouble()/2d;
			double eps = xUlpDbl/2d/System.Math.Pow(System.Math.Cos(xDbl), 2d);

			if (xDbl > 0.8) {
				/* tan(x) = 1/cot(x) */
				BigDecimal co = Cot(x);
				var mc = new MathContext(ErrorToPrecision(1d/co.ToDouble(), eps));
				return BigDecimal.One.Divide(co, mc);
			} else {
				BigDecimal xhighpr = ScalePrecision(res, 2);
				BigDecimal xhighprSq = MultiplyRound(xhighpr, xhighpr);

				BigDecimal resul = xhighpr.Plus();

				/* x^(2i+1) */
				BigDecimal xpowi = xhighpr;

				var b = new Bernoulli();

				/* 2^(2i) */
				BigInteger fourn = BigInteger.ValueOf(4);
				/* (2i)! */
				BigInteger fac = BigInteger.ValueOf(2);

				for (int i = 2;; i++) {
					Rational f = b[2*i].Abs();
					fourn = fourn.ShiftLeft(2);
					fac = fac.Multiply(BigInteger.ValueOf((2*i))).Multiply(BigInteger.ValueOf((2*i - 1)));
					f = f.Multiply(fourn).Multiply(fourn.Subtract(BigInteger.One)).Divide(fac);
					xpowi = MultiplyRound(xpowi, xhighprSq);
					BigDecimal c = MultiplyRound(xpowi, f);
					resul = resul.Add(c);
					if (System.Math.Abs(c.ToDouble()) < 0.1*eps)
						break;
				}
				var mc = new MathContext(ErrorToPrecision(resul.ToDouble(), eps));
				return resul.Round(mc);
			}
		}

		public static BigDecimal Cot(BigDecimal x) {
			if (x.CompareTo(BigDecimal.Zero) == 0) {
				throw new ArithmeticException("Cannot take cot of zero " + x);
			}
			if (x.CompareTo(BigDecimal.Zero) < 0) {
				return Cot(x.Negate()).Negate();
			}
			/* reduce modulo pi
                        */
			BigDecimal res = ModPi(x);

			/* absolute error in the result is err(x)/sin^2(x) to lowest order
                        */
			double xDbl = res.ToDouble();
			double xUlpDbl = x.Ulp().ToDouble()/2d;
			double eps = xUlpDbl/2d/System.Math.Pow(System.Math.Sin(xDbl), 2d);

			BigDecimal xhighpr = ScalePrecision(res, 2);
			BigDecimal xhighprSq = MultiplyRound(xhighpr, xhighpr);

			var mc = new MathContext(ErrorToPrecision(xhighpr.ToDouble(), eps));
			BigDecimal resul = BigDecimal.One.Divide(xhighpr, mc);

			/* x^(2i-1) */
			BigDecimal xpowi = xhighpr;

			var b = new Bernoulli();

			/* 2^(2i) */
			var fourn = BigInteger.Parse("4");
			/* (2i)! */
			BigInteger fac = BigInteger.One;

			for (int i = 1;; i++) {
				Rational f = b[2*i];
				fac = fac.Multiply(BigInteger.ValueOf((2*i))).Multiply(BigInteger.ValueOf((2*i - 1)));
				f = f.Multiply(fourn).Divide(fac);
				BigDecimal c = MultiplyRound(xpowi, f);
				if (i%2 == 0)
					resul = resul.Add(c);
				else
					resul = resul.Subtract(c);
				if (System.Math.Abs(c.ToDouble()) < 0.1*eps)
					break;

				fourn = fourn.ShiftLeft(2);
				xpowi = MultiplyRound(xpowi, xhighprSq);
			}
			mc = new MathContext(ErrorToPrecision(resul.ToDouble(), eps));
			return resul.Round(mc);
		}

		public static double Cot(double x) {
			return 1d/System.Math.Tan(x);
		}

		public static BigDecimal Asin(BigDecimal x) {
			if (x.CompareTo(BigDecimal.One) > 0 ||
			    x.CompareTo(BigDecimal.One.Negate()) < 0) {
				throw new ArithmeticException("Out of range argument " + x + " of asin");
			}
			if (x.CompareTo(BigDecimal.Zero) == 0)
				return BigDecimal.Zero;
			if (x.CompareTo(BigDecimal.One) == 0) {
				/* arcsin(1) = pi/2
                        */
				double errpi = System.Math.Sqrt(x.Ulp().ToDouble());
				var mc = new MathContext(ErrorToPrecision(3.14159, errpi));
				return PiRound(mc).Divide(new BigDecimal(2));
			}
			if (x.CompareTo(BigDecimal.Zero) < 0) {
				return Asin(x.Negate()).Negate();
			}
			if (x.ToDouble() > 0.7) {
				BigDecimal xCompl = BigDecimal.One.Subtract(x);
				double xDbl = x.ToDouble();
				double xUlpDbl = x.Ulp().ToDouble()/2d;
				double eps = xUlpDbl/2d/System.Math.Sqrt(1d - System.Math.Pow(xDbl, 2d));

				BigDecimal xhighpr = ScalePrecision(xCompl, 3);
				BigDecimal xhighprV = DivideRound(xhighpr, 4);

				BigDecimal resul = BigDecimal.One;

				/* x^(2i+1) */
				BigDecimal xpowi = BigDecimal.One;

				/* i factorial */
				BigInteger ifacN = BigInteger.One;
				BigInteger ifacD = BigInteger.One;

				for (int i = 1;; i++) {
					ifacN = ifacN.Multiply(BigInteger.ValueOf((2*i - 1)));
					ifacD = ifacD.Multiply(BigInteger.ValueOf(i));
					if (i == 1)
						xpowi = xhighprV;
					else
						xpowi = MultiplyRound(xpowi, xhighprV);
					BigDecimal c = DivideRound(MultiplyRound(xpowi, ifacN),
						ifacD.Multiply(BigInteger.ValueOf((2*i + 1))));
					resul = resul.Add(c);
					/* series started 1+x/12+... which yields an estimate of the sum's error
                                */
					if (System.Math.Abs(c.ToDouble()) < xUlpDbl/120d)
						break;
				}
				/* sqrt(2*z)*(1+...)
                        */
				xpowi = Sqrt(xhighpr.Multiply(new BigDecimal(2)));
				resul = MultiplyRound(xpowi, resul);

				var mc = new MathContext(resul.Precision);
				BigDecimal pihalf = PiRound(mc).Divide(new BigDecimal(2));

				mc = new MathContext(ErrorToPrecision(resul.ToDouble(), eps));
				return pihalf.Subtract(resul, mc);
			} else {
				/* absolute error in the result is err(x)/sqrt(1-x^2) to lowest order
                        */
				double xDbl = x.ToDouble();
				double xUlpDbl = x.Ulp().ToDouble()/2d;
				double eps = xUlpDbl/2d/System.Math.Sqrt(1d - System.Math.Pow(xDbl, 2d));

				BigDecimal xhighpr = ScalePrecision(x, 2);
				BigDecimal xhighprSq = MultiplyRound(xhighpr, xhighpr);

				BigDecimal resul = xhighpr.Plus();

				/* x^(2i+1) */
				BigDecimal xpowi = xhighpr;

				/* i factorial */
				BigInteger ifacN = BigInteger.One;
				BigInteger ifacD = BigInteger.One;

				for (int i = 1;; i++) {
					ifacN = ifacN.Multiply(BigInteger.ValueOf((2*i - 1)));
					ifacD = ifacD.Multiply(BigInteger.ValueOf((2*i)));
					xpowi = MultiplyRound(xpowi, xhighprSq);
					BigDecimal c = DivideRound(MultiplyRound(xpowi, ifacN),
						ifacD.Multiply(BigInteger.ValueOf((2*i + 1))));
					resul = resul.Add(c);
					if (System.Math.Abs(c.ToDouble()) < 0.1*eps)
						break;
				}
				var mc = new MathContext(ErrorToPrecision(resul.ToDouble(), eps));
				return resul.Round(mc);
			}
		}

		public static BigDecimal Mod2Pi(BigDecimal x) {
			/* write x= 2*pi*k+r with the precision in r defined by the precision of x and not
                * compromised by the precision of 2*pi, so the ulp of 2*pi*k should match the ulp of x.
                * First get a guess of k to figure out how many digits of 2*pi are needed.
                */
			var k = (int) (0.5*x.ToDouble()/System.Math.PI);

			/* want to have err(2*pi*k)< err(x)=0.5*x.ulp, so err(pi) = err(x)/(4k) with two safety digits
                */
			double err2pi;
			if (k != 0)
				err2pi = 0.25*System.Math.Abs(x.Ulp().ToDouble()/k);
			else
				err2pi = 0.5*System.Math.Abs(x.Ulp().ToDouble());
			var mc = new MathContext(2 + ErrorToPrecision(6.283, err2pi));
			BigDecimal twopi = PiRound(mc).Multiply(new BigDecimal(2));

			/* Delegate the actual operation to the BigDecimal class, which may return
                * a negative value of x was negative .
                */
			BigDecimal res = x.Remainder(twopi);
			if (res.CompareTo(BigDecimal.Zero) < 0)
				res = res.Add(twopi);

			/* The actual precision is set by the input value, its absolute value of x.ulp()/2.
                */
			mc = new MathContext(ErrorToPrecision(res.ToDouble(), x.Ulp().ToDouble()/2d));
			return res.Round(mc);
		}

		public static BigDecimal ModPi(BigDecimal x) {
			/* write x= pi*k+r with the precision in r defined by the precision of x and not
                * compromised by the precision of pi, so the ulp of pi*k should match the ulp of x.
                * First get a guess of k to figure out how many digits of pi are needed.
                */
			var k = (int) (x.ToDouble()/System.Math.PI);

			/* want to have err(pi*k)< err(x)=x.ulp/2, so err(pi) = err(x)/(2k) with two safety digits
                */
			double errpi;
			if (k != 0)
				errpi = 0.5*System.Math.Abs(x.Ulp().ToDouble()/k);
			else
				errpi = 0.5*System.Math.Abs(x.Ulp().ToDouble());
			var mc = new MathContext(2 + ErrorToPrecision(3.1416, errpi));
			BigDecimal onepi = PiRound(mc);
			BigDecimal pihalf = onepi.Divide(new BigDecimal(2));

			/* Delegate the actual operation to the BigDecimal class, which may return
                * a negative value of x was negative .
                */
			BigDecimal res = x.Remainder(onepi);
			if (res.CompareTo(pihalf) > 0)
				res = res.Subtract(onepi);
			else if (res.CompareTo(pihalf.Negate()) < 0)
				res = res.Add(onepi);

			/* The actual precision is set by the input value, its absolute value of x.ulp()/2.
                */
			mc = new MathContext(ErrorToPrecision(res.ToDouble(), x.Ulp().ToDouble()/2d));
			return res.Round(mc);
		}

		public static double Psi(double x) {
			/* the single positive zero of psi(x)
				*/
			double psi0 = 1.46163214496836234126265954232572132846819;
			if (x > 2.0) {
				/* Reduce to a value near x=1 with the standard recurrence formula.
						* Abramowitz-Stegun 6.3.5
						*/
				int m = (int) (x - 0.5);
				double xmin1 = x - m;
				double resul = 0d;
				for (int i = 1; i <= m; i++)
					resul += 1d/(x - i);
				return resul + Psi(xmin1);
			} else if (System.Math.Abs(x - psi0) < 0.55) {
				/* Taylor approximation around the local zero
						*/
				double[] psiT0 = {
					9.67672245447621170427e-01, -4.42763168983592106093e-01,
					2.58499760955651010624e-01, -1.63942705442406527504e-01, 1.07824050691262365757e-01,
					-7.21995612564547109261e-02, 4.88042881641431072251e-02, -3.31611264748473592923e-02,
					2.25976482322181046596e-02, -1.54247659049489591388e-02, 1.05387916166121753881e-02,
					-7.20453438635686824097e-03, 4.92678139572985344635e-03, -3.36980165543932808279e-03,
					2.30512632673492783694e-03, -1.57693677143019725927e-03, 1.07882520191629658069e-03,
					-7.38070938996005129566e-04, 5.04953265834602035177e-04, -3.45468025106307699556e-04,
					2.36356015640270527924e-04, -1.61706220919748034494e-04, 1.10633727687474109041e-04,
					-7.56917958219506591924e-05, 5.17857579522208086899e-05, -3.54300709476596063157e-05,
					2.42400661186013176527e-05, -1.65842422718541333752e-05, 1.13463845846638498067e-05,
					-7.76281766846209442527e-06, 5.31106092088986338732e-06, -3.63365078980104566837e-06,
					2.48602273312953794890e-06, -1.70085388543326065825e-06, 1.16366753635488427029e-06,
					-7.96142543124197040035e-07, 5.44694193066944527850e-07, -3.72661612834382295890e-07,
					2.54962655202155425666e-07, -1.74436951177277452181e-07, 1.19343948298302427790e-07,
					-8.16511518948840884084e-08, 5.58629968353217144428e-08, -3.82196006191749421243e-08,
					2.61485769519618662795e-08, -1.78899848649114926515e-08, 1.22397314032336619391e-08,
					-8.37401629767179054290e-09, 5.72922285984999377160e-09
				};
				double xdiff = x - psi0;
				double resul = 0d;
				for (int i = psiT0.Length - 1; i >= 0; i--)
					resul = resul*xdiff + psiT0[i];
				return resul*xdiff;
			} else if (x < 0d) {
				/* Reflection formula */
				double xmin = 1d - x;
				return Psi(xmin) + System.Math.PI/System.Math.Tan(System.Math.PI*xmin);
			} else {
				double xmin1 = x - 1;
				double resul = 0d;
				for (int k = 26; k >= 1; k--) {
					resul -= Zeta1(2*k + 1);
					resul *= xmin1*xmin1;
				}
				/* 0.422... = 1 -gamma */
				return resul + 0.422784335098467139393487909917597568
					   + 0.5/xmin1 - 1d/(1 - xmin1*xmin1) - System.Math.PI/(2d*System.Math.Tan(System.Math.PI*xmin1));
			}
		}
	}
}