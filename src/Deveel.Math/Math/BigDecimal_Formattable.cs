using System;
using System.Globalization;

namespace Deveel.Math
{
    public sealed partial class BigDecimal : IFormattable
    {
        private const string GeneralStringFormat = "G";
        private const string PlainStringFormat = "P";
        private const string EngineeringStringFormat = "E";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <remarks>
        /// <para>
        ///     The supported formats are
        ///     <list type="bullet">
        ///         <listheader>
        ///           <term>Format</term>
        ///           <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term><c>G</c></term>
        ///             <description>General format. The number is formatted as a fixed-point number.</description>
        ///         </item>
        ///         <item>
        ///             <term><c>P</c></term>
        ///             <description>Plain format. The number is formatted as a plain number, without any scientific
        ///             notation. If the string representation is used to create a new instance, this instance is 
        ///             generally not identical to this instance as the precision</description>
        ///         </item>
        ///         <item>
        ///             <term><c>E</c></term>
        ///             <description>Engineering format. The number is formatted as a fixed-point number in engineering
        ///             notation. If the scale is negative or if <c>scale - precision >= 6</c> 
        ///             then engineering notation is used. Engineering notation is similar to the scientific notation except 
        ///             that the exponent is made to be a multiple of 3 such that the integer part is lesser or equal to 1 
        ///             and greater than 1000.</description>
        ///         </item>
        ///     </list>
        /// </para>
        /// </remarks>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string ToString(string? format, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = NumberFormatInfo.CurrentInfo;

            if (String.IsNullOrWhiteSpace(format) ||
                format == GeneralStringFormat)
            {
                return DecimalString.ToString(this, provider);
            } else if (format == PlainStringFormat)
            {
                return DecimalString.ToPlainString(this, provider);
            } else if (format == EngineeringStringFormat)
            {
                return DecimalString.ToEngineeringString(this, provider);
            }

            throw new ArgumentException($"Format '{format}' was not recognized");
        }
    }
}
