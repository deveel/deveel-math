using System;
using Xunit;

namespace Deveel.Math {
    public class MathContextExtendedTests {
        [Fact]
        public void MathContext_Equals_Object() {
            var mc1 = new MathContext(5, RoundingMode.HalfUp);
            var mc2 = new MathContext(5, RoundingMode.HalfUp);
            var mc3 = new MathContext(5, RoundingMode.Down);
            Assert.True(mc1.Equals((object)mc2));
            Assert.False(mc1.Equals((object)mc3));
            Assert.False(mc1.Equals((object)null));
            Assert.False(mc1.Equals("not a MathContext"));
        }

        [Fact]
        public void MathContext_TryParse_Valid() {
            MathContext result;
            Assert.True(MathContext.TryParse("precision=10 roundingMode=HalfUp", out result));
            Assert.Equal(10, result.Precision);
            Assert.Equal(RoundingMode.HalfUp, result.RoundingMode);
        }

        [Fact]
        public void MathContext_TryParse_Invalid() {
            MathContext result;
            Assert.False(MathContext.TryParse("invalid", out result));
        }

        [Fact]
        public void MathContext_Parse_Valid() {
            var result = MathContext.Parse("precision=5 roundingMode=Down");
            Assert.Equal(5, result.Precision);
            Assert.Equal(RoundingMode.Down, result.RoundingMode);
        }

        [Fact]
        public void MathContext_Parse_Invalid_Throws() {
            Assert.Throws<FormatException>(() => MathContext.Parse("garbage"));
        }

        [Fact]
        public void MathContext_OnDeserialized_NeverCalled_Test() {
            var mc = new MathContext(10, RoundingMode.HalfEven);
            Assert.Equal(10, mc.Precision);
            Assert.Equal(RoundingMode.HalfEven, mc.RoundingMode);
        }

        [Fact]
        public void MathContext_TryParse_PrecisionOnly() {
            MathContext result;
            Assert.False(MathContext.TryParse("precision=5", out result));
        }

        [Fact]
        public void MathContext_TryParse_InvalidPrecision() {
            MathContext result;
            Assert.False(MathContext.TryParse("precision=-1 roundingMode=HalfUp", out result));
        }

        [Fact]
        public void MathContext_Parse_PrecisionOnly_Throws() {
            Assert.Throws<FormatException>(() => MathContext.Parse("precision=10"));
        }

        [Fact]
        public void MathContext_Parse_InvalidPrecision_Throws() {
            Assert.Throws<FormatException>(() => MathContext.Parse("precision=-5 roundingMode=HalfUp"));
        }

        [Fact]
        public void MathContext_Parse_InvalidFormat_Throws() {
            Assert.Throws<FormatException>(() => MathContext.Parse("unknown"));
        }

        [Fact]
        public void MathContext_EqualityOperator() {
            var mc1 = new MathContext(5, RoundingMode.HalfUp);
            var mc2 = new MathContext(5, RoundingMode.HalfUp);
            Assert.True(mc1.Equals(mc2));
        }

        [Fact]
        public void MathContext_InequalityOperator() {
            var mc1 = new MathContext(5, RoundingMode.HalfUp);
            var mc2 = new MathContext(6, RoundingMode.HalfUp);
            Assert.False(mc1.Equals(mc2));
        }

        [Fact]
        public void MathContext_Constructor_InvalidPrecision_Throws() {
            Assert.Throws<ArgumentException>(() => new MathContext(-1, RoundingMode.HalfUp));
        }
    }
}
