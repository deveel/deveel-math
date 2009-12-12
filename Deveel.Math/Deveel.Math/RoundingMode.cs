namespace Deveel.Math {
	public enum RoundingMode {

		/**
		 * Rounding mode where positive values are rounded towards positive infinity
		 * and negative values towards negative infinity.
		 * <br>
		 * Rule: {@code x.round().abs() >= x.abs()}
		 */
		Up = 0,

		/**
		 * Rounding mode where the values are rounded towards zero.
		 * <br>
		 * Rule: {@code x.round().abs() <= x.abs()}
		 */
		Down = 1,

		/**
		 * Rounding mode to round towards positive infinity. For positive values
		 * this rounding mode behaves as {@link #UP}, for negative values as
		 * {@link #DOWN}.
		 * <br>
		 * Rule: {@code x.round() >= x}
		 */
		Ceiling = 2,

		/**
		 * Rounding mode to round towards negative infinity. For positive values
		 * this rounding mode behaves as {@link #DOWN}, for negative values as
		 * {@link #UP}.
		 * <br>
		 * Rule: {@code x.round() <= x}
		 */
		Floor = 3,

		/**
		 * Rounding mode where values are rounded towards the nearest neighbor. Ties
		 * are broken by rounding up.
		 */
		HalfUp = 4,

		/**
		 * Rounding mode where values are rounded towards the nearest neighbor. Ties
		 * are broken by rounding down.
		 */
		HalfDown = 5,

		/**
		 * Rounding mode where values are rounded towards the nearest neighbor. Ties
		 * are broken by rounding to the even neighbor.
		 */
		HalfEven = 6,

		/**
		 * Rounding mode where the rounding operations throws an ArithmeticException
		 * for the case that rounding is necessary, i.e. for the case that the value
		 * cannot be represented exactly.
		 */
		Unnecessary = 7
	}
}