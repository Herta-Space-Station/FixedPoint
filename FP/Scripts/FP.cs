using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NativeCollections;

#pragma warning disable CA2208
#pragma warning disable CS8632

// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     A fixed-point number. 16 lower bits are used for the decimal part, 48 for the integral part.
    ///     <para>
    ///         It provides various methods for performing mathematical operations and converting between different data
    ///         types.
    ///     </para>
    ///     <para>
    ///         However, a majority of internal code and the multiplication operator perform fast multiplication,
    ///         where the result can use at most 32 bits for the integral part and overflows are not detected.
    ///         This means that you should stay in <see cref="T:System.Int16" /> range.
    ///         <seealso cref="P:Herta.FP.UseableMax" />
    ///         <seealso cref="P:Herta.FP.UseableMin" />
    ///     </para>
    /// </summary>
    /// \ingroup MathAPI
    /// <remarks>
    ///     The precision of the decimal part is 5 digits.
    ///     The decimal fraction normalizer is 1E5.
    ///     The size of an FP object is 8 bytes.
    ///     The raw value of one is equal to FPLut.ONE.
    ///     The raw value of zero is 0.
    ///     The precision value is equal to FPLut.PRECISION.
    ///     The number of bits in an FP object is equal to the size of a long (64 bits).
    ///     The MulRound constant is 0.
    ///     The MulShift constant is equal to the precision value.
    ///     The MulShiftTrunc constant is equal to the precision value.
    /// </remarks>
    /// <seealso cref="T:Herta.FPLut" />
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct FP : IEquatable<FP>, IComparable<FP>
    {
        /// <summary>
        ///     Represents the size of a variable in bytes.
        ///     <para>The SIZE constant is used to determine the size of a variable in bytes.</para>
        /// </summary>
        public const int SIZE = 8;

        internal const int DecimalFractionDigits = 5;
        internal const double DecimalFractionNormalizer = 100000.0;
        private const int FRACTIONS_COUNT = 5;

        /// <summary>The value of one as a fixed-point number.</summary>
        public const long RAW_ONE = 65536;

        /// <summary>
        ///     Represents a constant that holds the raw value of zero for the <see cref="T:Herta.FP" /> struct.
        /// </summary>
        public const long RAW_ZERO = 0;

        /// <summary>
        ///     Represents the precision used for Fixed Point calculations.
        /// </summary>
        /// <remarks>
        ///     The Precision constant is used to determine the number of decimal places in Fixed Point calculations.
        /// </remarks>
        public const int Precision = 16;

        /// <summary>The size in bits of the fixed-point number. (64)</summary>
        public const int Bits = 64;

        /// <summary>
        ///     Represents the value of the rounding constant used in Fixed Point multiplication.
        /// </summary>
        public const long MulRound = 32768;

        /// <summary>
        ///     Represents the bit shift used in Fixed Point multiplication.
        /// </summary>
        public const int MulShift = 16;

        public const int MulShiftTrunc = 16;

        /// <summary>The raw integer value of the fixed-point number.</summary>
        [FieldOffset(0)] public long RawValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FP Cast(in long value) => Unsafe.As<long, FP>(ref Unsafe.AsRef(in value));

        /// <summary>
        ///     The smallest FP unit that is not 0.
        ///     <para>Closest double: 1.52587890625E-05</para>
        /// </summary>
        public static readonly FP SmallestNonZero = Cast(1L);

        /// <summary>
        ///     Minimum FP value, but values outside of <see cref="P:Herta.FP.UseableMin" /> and
        ///     <see cref="P:Herta.FP.UseableMax" /> (inclusive) can overflow when multiplied.
        ///     <para>Closest double: -140737488355328</para>
        /// </summary>
        public static readonly FP MinValue = Cast(long.MinValue);

        /// <summary>
        ///     Maximum FP value, but values outside of <see cref="P:Herta.FP.UseableMin" /> and
        ///     <see cref="P:Herta.FP.UseableMax" /> (inclusive) can overflow when multiplied.
        ///     <para>Closest double: 140737488355328</para>
        /// </summary>
        public static readonly FP MaxValue = Cast(long.MaxValue);

        /// <summary>
        ///     Represents the highest negative FP number that can be multiplied with itself and not cause an overflow (exceeding
        ///     long range).
        ///     <para>Closest double: -32768</para>
        /// </summary>
        public static readonly FP UseableMin = Cast((long)int.MinValue);

        /// <summary>
        ///     Represents the highest FP number that can be multiplied with itself and not cause an overflow (exceeding long
        ///     range).
        ///     <para>Closest double: 32767.9999847412</para>
        /// </summary>
        public static readonly FP UseableMax = Cast((long)int.MaxValue);

        /// <summary>
        ///     Pi number.
        ///     <para>Closest double: 3.14158630371094</para>
        /// </summary>
        public static readonly FP Pi = Cast(205887L);

        /// <summary>
        ///     1/Pi.
        ///     <para>Closest double: 0.318313598632813</para>
        /// </summary>
        public static readonly FP PiInv = Cast(20861L);

        /// <summary>
        ///     2 * Pi.
        ///     <para>Closest double: 6.28318786621094</para>
        /// </summary>
        public static readonly FP PiTimes2 = Cast(411775L);

        /// <summary>
        ///     Pi / 2.
        ///     <para>Closest double: 1.57080078125</para>
        /// </summary>
        public static readonly FP PiOver2 = Cast(102944L);

        /// <summary>
        ///     2 / Pi.
        ///     <para>Closest double: 0.636627197265625</para>
        /// </summary>
        public static readonly FP PiOver2Inv = Cast(41722L);

        /// <summary>
        ///     Pi / 4.
        ///     <para>Closest double: 0.785400390625</para>
        /// </summary>
        public static readonly FP PiOver4 = Cast(51472L);

        /// <summary>
        ///     3 * Pi / 4.
        ///     <para>Closest double: 2.356201171875</para>
        /// </summary>
        public static readonly FP Pi3Over4 = Cast(154416L);

        /// <summary>
        ///     4 * Pi / 3.
        ///     <para>Closest double: 4.18879699707031</para>
        /// </summary>
        public static readonly FP Pi4Over3 = Cast(274517L);

        /// <summary>
        ///     Degrees-to-radians conversion constant.
        ///     <para>Closest double: 0.0174560546875</para>
        /// </summary>
        public static readonly FP Deg2Rad = Cast(1144L);

        /// <summary>
        ///     Radians-to-degrees conversion constant.
        ///     <para>Closest double: 57.2957763671875</para>
        /// </summary>
        public static readonly FP Rad2Deg = Cast(3754936L);

        /// <summary>
        ///     FP constant representing the number 0.
        ///     <para>Closest double: 0</para>
        /// </summary>
        public static readonly FP _0 = Cast(0L);

        /// <summary>
        ///     FP constant representing the number 1.
        ///     <para>Closest double: 1</para>
        /// </summary>
        public static readonly FP _1 = Cast(65536L);

        /// <summary>
        ///     FP constant representing the number 2.
        ///     <para>Closest double: 2</para>
        /// </summary>
        public static readonly FP _2 = Cast(131072L);

        /// <summary>
        ///     FP constant representing the number 3.
        ///     <para>Closest double: 3</para>
        /// </summary>
        public static readonly FP _3 = Cast(196608L);

        /// <summary>
        ///     FP constant representing the number 4.
        ///     <para>Closest double: 4</para>
        /// </summary>
        public static readonly FP _4 = Cast(262144L);

        /// <summary>
        ///     FP constant representing the number 5.
        ///     <para>Closest double: 5</para>
        /// </summary>
        public static readonly FP _5 = Cast(327680L);

        /// <summary>
        ///     FP constant representing the number 6.
        ///     <para>Closest double: 6</para>
        /// </summary>
        public static readonly FP _6 = Cast(393216L);

        /// <summary>
        ///     FP constant representing the number 7.
        ///     <para>Closest double: 7</para>
        /// </summary>
        public static readonly FP _7 = Cast(458752L);

        /// <summary>
        ///     FP constant representing the number 8.
        ///     <para>Closest double: 8</para>
        /// </summary>
        public static readonly FP _8 = Cast(524288L);

        /// <summary>
        ///     FP constant representing the number 9.
        ///     <para>Closest double: 9</para>
        /// </summary>
        public static readonly FP _9 = Cast(589824L);

        /// <summary>
        ///     FP constant representing the number 10.
        ///     <para>Closest double: 10</para>
        /// </summary>
        public static readonly FP _10 = Cast(655360L);

        /// <summary>
        ///     FP constant representing the number 99.
        ///     <para>Closest double: 99</para>
        /// </summary>
        public static readonly FP _99 = Cast(6488064L);

        /// <summary>
        ///     FP constant representing the number 100.
        ///     <para>Closest double: 100</para>
        /// </summary>
        public static readonly FP _100 = Cast(6553600L);

        /// <summary>
        ///     FP constant representing the number 180.
        ///     <para>Closest double: 180</para>
        /// </summary>
        public static readonly FP _180 = Cast(11796480L);

        /// <summary>
        ///     FP constant representing the number 200.
        ///     <para>Closest double: 200</para>
        /// </summary>
        public static readonly FP _200 = Cast(13107200L);

        /// <summary>
        ///     FP constant representing the number 360.
        ///     <para>Closest double: 360</para>
        /// </summary>
        public static readonly FP _360 = Cast(23592960L);

        /// <summary>
        ///     FP constant representing the number 1000.
        ///     <para>Closest double: 1000</para>
        /// </summary>
        public static readonly FP _1000 = Cast(65536000L);

        /// <summary>
        ///     FP constant representing the number 10000.
        ///     <para>Closest double: 10000</para>
        /// </summary>
        public static readonly FP _10000 = Cast(655360000L);

        /// <summary>
        ///     FP constant representing the number 0.01.
        ///     <para>Closest double: 0.0099945068359375</para>
        /// </summary>
        public static readonly FP _0_01 = Cast(655L);

        /// <summary>
        ///     FP constant representing the number 0.02.
        ///     <para>Closest double: 0.0200042724609375</para>
        /// </summary>
        public static readonly FP _0_02 = Cast(1311L);

        /// <summary>
        ///     FP constant representing the number 0.03.
        ///     <para>Closest double: 0.029998779296875</para>
        /// </summary>
        public static readonly FP _0_03 = Cast(1966L);

        /// <summary>
        ///     FP constant representing the number 0.04.
        ///     <para>Closest double: 0.0399932861328125</para>
        /// </summary>
        public static readonly FP _0_04 = Cast(2621L);

        /// <summary>
        ///     FP constant representing the number 0.05.
        ///     <para>Closest double: 0.0500030517578125</para>
        /// </summary>
        public static readonly FP _0_05 = Cast(3277L);

        /// <summary>
        ///     FP constant representing the number 0.10.
        ///     <para>Closest double: 0.100006103515625</para>
        /// </summary>
        public static readonly FP _0_10 = Cast(6554L);

        /// <summary>
        ///     FP constant representing the number 0.20.
        ///     <para>Closest double: 0.199996948242188</para>
        /// </summary>
        public static readonly FP _0_20 = Cast(13107L);

        /// <summary>
        ///     FP constant representing the number 0.25.
        ///     <para>Closest double: 0.25</para>
        /// </summary>
        public static readonly FP _0_25 = Cast(16384L);

        /// <summary>
        ///     FP constant representing the number 0.50.
        ///     <para>Closest double: 0.5</para>
        /// </summary>
        public static readonly FP _0_50 = Cast(32768L);

        /// <summary>
        ///     FP constant representing the number 0.75.
        ///     <para>Closest double: 0.75</para>
        /// </summary>
        public static readonly FP _0_75 = Cast(49152L);

        /// <summary>
        ///     FP constant representing the number 0.33.
        ///     <para>Closest double: 0.333328247070313</para>
        /// </summary>
        public static readonly FP _0_33 = Cast(21845L);

        /// <summary>
        ///     FP constant representing the number 0.99.
        ///     <para>Closest double: 0.990005493164063</para>
        /// </summary>
        public static readonly FP _0_99 = Cast(64881L);

        /// <summary>
        ///     FP constant representing the number -1.
        ///     <para>Closest double: -1</para>
        /// </summary>
        public static readonly FP Minus_1 = Cast(-65536L);

        /// <summary>
        ///     FP constant representing 360 degrees in radian.
        ///     <para>Closest double: 6.28318786621094</para>
        /// </summary>
        public static readonly FP Rad_360 = Cast(411775L);

        /// <summary>
        ///     FP constant representing 180 degrees in radian.
        ///     <para>Closest double: 3.14158630371094</para>
        /// </summary>
        public static readonly FP Rad_180 = Cast(205887L);

        /// <summary>
        ///     FP constant representing 90 degrees in radian.
        ///     <para>Closest double: 1.57080078125</para>
        /// </summary>
        public static readonly FP Rad_90 = Cast(102944L);

        /// <summary>
        ///     FP constant representing 45 degrees in radian.
        ///     <para>Closest double: 0.785400390625</para>
        /// </summary>
        public static readonly FP Rad_45 = Cast(51472L);

        /// <summary>
        ///     FP constant representing 22.5 degrees in radian.
        ///     <para>Closest double: 0.3927001953125</para>
        /// </summary>
        public static readonly FP Rad_22_50 = Cast(25736L);

        /// <summary>
        ///     FP constant representing the number 1.01.
        ///     <para>Closest double: 1.00999450683594</para>
        /// </summary>
        public static readonly FP _1_01 = Cast(66191L);

        /// <summary>
        ///     FP constant representing the number 1.02.
        ///     <para>Closest double: 1.02000427246094</para>
        /// </summary>
        public static readonly FP _1_02 = Cast(66847L);

        /// <summary>
        ///     FP constant representing the number 1.03.
        ///     <para>Closest double: 1.02999877929688</para>
        /// </summary>
        public static readonly FP _1_03 = Cast(67502L);

        /// <summary>
        ///     FP constant representing the number 1.04.
        ///     <para>Closest double: 1.03999328613281</para>
        /// </summary>
        public static readonly FP _1_04 = Cast(68157L);

        /// <summary>
        ///     FP constant representing the number 1.05.
        ///     <para>Closest double: 1.05000305175781</para>
        /// </summary>
        public static readonly FP _1_05 = Cast(68813L);

        /// <summary>
        ///     FP constant representing the number 1.10.
        ///     <para>Closest double: 1.10000610351563</para>
        /// </summary>
        public static readonly FP _1_10 = Cast(72090L);

        /// <summary>
        ///     FP constant representing the number 1.20.
        ///     <para>Closest double: 1.19999694824219</para>
        /// </summary>
        public static readonly FP _1_20 = Cast(78643L);

        /// <summary>
        ///     FP constant representing the number 1.25.
        ///     <para>Closest double: 1.25</para>
        /// </summary>
        public static readonly FP _1_25 = Cast(81920L);

        /// <summary>
        ///     FP constant representing the number 1.50.
        ///     <para>Closest double: 1.5</para>
        /// </summary>
        public static readonly FP _1_50 = Cast(98304L);

        /// <summary>
        ///     FP constant representing the number 1.75.
        ///     <para>Closest double: 1.75</para>
        /// </summary>
        public static readonly FP _1_75 = Cast(114688L);

        /// <summary>
        ///     FP constant representing the number 1.33.
        ///     <para>Closest double: 1.33332824707031</para>
        /// </summary>
        public static readonly FP _1_33 = Cast(87381L);

        /// <summary>
        ///     FP constant representing the number 1.99.
        ///     <para>Closest double: 1.99000549316406</para>
        /// </summary>
        public static readonly FP _1_99 = Cast(130417L);

        /// <summary>
        ///     FP constant representing the epsilon value EN1.
        ///     <para>Closest double: 0.100006103515625</para>
        /// </summary>
        public static readonly FP EN1 = Cast(6554L);

        /// <summary>
        ///     FP constant representing the epsilon value EN2.
        ///     <para>Closest double: 0.0099945068359375</para>
        /// </summary>
        public static readonly FP EN2 = Cast(655L);

        /// <summary>
        ///     FP constant representing the epsilon value EN3.
        ///     <para>Closest double: 0.001007080078125</para>
        /// </summary>
        public static readonly FP EN3 = Cast(66L);

        /// <summary>
        ///     FP constant representing the epsilon value EN4.
        ///     <para>Closest double: 0.0001068115234375</para>
        /// </summary>
        public static readonly FP EN4 = Cast(7L);

        /// <summary>
        ///     FP constant representing the epsilon value EN5.
        ///     <para>Closest double: 1.52587890625E-05</para>
        /// </summary>
        public static readonly FP EN5 = Cast(1L);

        /// <summary>
        ///     FP constant representing Epsilon <see cref="P:Herta.FP.EN3" />.
        ///     <para>Closest double: 0.001007080078125</para>
        /// </summary>
        public static readonly FP Epsilon = Cast(66L);

        /// <summary>
        ///     FP constant representing the Euler Number constant.
        ///     <para>Closest double: 2.71827697753906</para>
        /// </summary>
        public static readonly FP E = Cast(178145L);

        /// <summary>
        ///     FP constant representing Log(E).
        ///     <para>Closest double: 1.44268798828125</para>
        /// </summary>
        public static readonly FP Log2_E = Cast(94548L);

        /// <summary>
        ///     FP constant representing Log(10).
        ///     <para>Closest double: 3.32192993164063</para>
        /// </summary>
        public static readonly FP Log2_10 = Cast(217706L);

        /// <summary>Returns integral part as long.</summary>
        public long AsLong
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.RawValue >> 16;
        }

        /// <summary>Return integral part as int.</summary>
        public int AsInt
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (int)(this.RawValue >> 16);
        }

        /// <summary>Return integral part as int.</summary>
        public short AsShort
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (short)(this.RawValue >> 16);
        }

        /// <summary>Converts to float.</summary>
        public float AsFloat
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)this.RawValue / 65536f;
        }

        /// <summary>
        ///     Converts to double. The returned value is not exact, but rather the one that has the least
        ///     significant digits given FP's precision.
        /// </summary>
        public double AsRoundedDouble
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                (int Sign, ulong Integer, uint Fraction) decimalParts = this.GetDecimalParts();
                return (double)decimalParts.Sign * ((double)decimalParts.Integer + (double)decimalParts.Fraction / 100000.0);
            }
        }

        /// <summary>Converts to double.</summary>
        public double AsDouble
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (double)this.RawValue / 65536.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal FP(in long v) => this = Unsafe.As<long, FP>(ref Unsafe.AsRef(in v));

        /// <summary>
        ///     Compares this instance of FP to another instance and returns an integer that indicates whether this instance is
        ///     less than, equal to, or greater than the other instance.
        /// </summary>
        /// <param name="other">The other instance to compare.</param>
        /// <returns>A signed integer that indicates the relative values of this instance and the other instance.</returns>
        public int CompareTo(FP other) => this.RawValue.CompareTo(other.RawValue);

        /// <summary>
        ///     Determines whether the current instance is equal to another instance of FP.
        /// </summary>
        /// <param name="other">The instance to compare with the current instance.</param>
        /// <returns>
        ///     <see langword="true" /> if the current instance is equal to the other instance; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public bool Equals(FP other) => this.RawValue == other.RawValue;

        /// <summary>
        ///     Determines whether the current instance of FP is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance of FP.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified object is equal to the current instance of FP; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object? obj) => obj is FP fp && this.RawValue == fp.RawValue;

        /// <summary>
        ///     Computes the hash code for the current instance of the FP struct.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => XxHash.Hash32(this);

        /// <summary>
        ///     Returns a string representation of the current FP value.
        /// </summary>
        /// <returns>A string representation of the current FP value.</returns>
        public override string ToString()
        {
            NativeString builder = new NativeString(stackalloc char[32], 0);
            Format(ref builder);

            return builder.ToString();
        }

        public bool TryFormat(Span<char> destination, out int charsWritten)
        {
            NativeString builder = new NativeString(stackalloc char[32]);
            Format(ref builder);

            bool result = builder.TryCopyTo(destination);
            charsWritten = result ? builder.Length : 0;
            return result;
        }

        private void Format(ref NativeString builder)
        {
            (int Sign, ulong Integer, uint Fraction) = this.GetDecimalParts();
            if (Fraction == 0U)
            {
                builder.AppendFormattable(this.RawValue >> 16);
                return;
            }

            if (Sign < 0)
                builder.Append('-');
            builder.AppendFormattable(Integer);
            builder.Append('.');
            if (Fraction == 0)
            {
                builder.Append('0');
                return;
            }

            builder.AppendFormattable(Fraction, "D5");
            builder.TrimEnd('0');
        }

        /// <summary>
        ///     Converts the value of the current FP object to its equivalent string representation using the legacy format.
        /// </summary>
        /// <returns>
        ///     The string representation of the value of the current FP object, formatted using the legacy format.
        /// </returns>
        [Obsolete("Use ToString instead.")]
        public string ToStringLegacy() => this.AsFloat.ToString((IFormatProvider)CultureInfo.InvariantCulture);

        /// <summary>
        ///     Returns a string that represents the <see cref="T:Herta.FP" />.
        /// </summary>
        /// <returns>String representation of the FP.</returns>
        public string ToString(string format) => this.AsDouble.ToString(format, (IFormatProvider)CultureInfo.InvariantCulture);

        /// <summary>
        ///     Returns a string that represents the <see cref="T:Herta.FP" /> using a custom format.
        /// </summary>
        /// <returns>String representation of the FP.</returns>
        public string ToStringInternal()
        {
            Span<char> buffer = stackalloc char[32 * 2];
            NativeString builder1 = new NativeString(buffer.Slice(0, 32), 0);
            NativeString builder2 = new NativeString(buffer.Slice(32), 0);
            FormatInternal(ref builder1, ref builder2);

            return builder1.ToString();
        }

        public bool TryFormatInternal(Span<char> destination, out int charsWritten)
        {
            Span<char> buffer = stackalloc char[32 * 2];
            NativeString builder1 = new NativeString(buffer.Slice(0, 32), 0);
            NativeString builder2 = new NativeString(buffer.Slice(32), 0);
            FormatInternal(ref builder1, ref builder2);

            bool result = builder1.TryCopyTo(destination);
            charsWritten = result ? builder1.Length : 0;
            return result;
        }

        private void FormatInternal(ref NativeString builder1, ref NativeString builder2)
        {
            long num = Math.Abs(this.RawValue);
            if (this.RawValue < 0L)
                builder1.Append('-');
            builder1.AppendFormattable(num >> 16, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder1.Append('.');
            builder2.AppendFormattable(num % 65536L, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder2.PadLeft(5, '0');
            builder1.Append(builder2.Text);
        }

        /// <summary>
        ///     Converts a double value to an instance of the FP, with rounding to the nearest representable FP.
        /// </summary>
        /// <param name="value">The rounded double value to convert.</param>
        /// <returns>The FP value that represents the rounded double value.</returns>
        public static FP FromRoundedDouble_UNSAFE(double value) => new FP((long)Math.Round(value * 65536.0));

        /// <summary>
        ///     Converts a double value to an instance of the FP, with rounding towards zero..
        ///     To round towards nearest representable FP, use
        ///     <see cref="M:Herta.FP.FromRoundedDouble_UNSAFE(System.Double)" />.
        ///     This method is marked as unsafe because it is not deterministic.
        /// </summary>
        /// <param name="value">The double value to convert.</param>
        /// <returns>An instance of the FP struct that represents the converted value.</returns>
        public static FP FromDouble_UNSAFE(double value) => new FP((long)(value * 65536.0));

        public static FP FromDouble_SAFE(double value)
        {
            const long Q16_SHIFT = 16; // 16 fractional bits (Q48.16 format)
            const long DOUBLE_MANTISSA_BITS = 52;
            const long DOUBLE_EXPONENT_BIAS = 1023;

            // Reinterpret the double as a long (IEEE 754 binary64 format)
            long bits = Unsafe.As<double, long>(ref value);

            // Extract sign (1 bit)
            bool isNegative = (bits & unchecked((long)0x8000000000000000)) != 0;

            // Extract exponent (11 bits, biased)
            long exponent = ((bits >> (int)DOUBLE_MANTISSA_BITS) & 0x7FF) - DOUBLE_EXPONENT_BIAS;

            // Extract mantissa (52 bits, with implicit leading 1)
            long mantissa = (bits & 0x000FFFFFFFFFFFFF) | 0x0010000000000000;

            // Compute the shift needed to align the mantissa to Q48.16
            long shift = (DOUBLE_MANTISSA_BITS - Q16_SHIFT) - exponent;

            // Shift the mantissa into Q48.16 fixed-point
            long fixedPointValue;
            if (shift >= 0)
                fixedPointValue = mantissa >> (int)shift;
            else
                fixedPointValue = mantissa << (int)(-shift);

            // Apply sign
            fixedPointValue = isNegative ? -fixedPointValue : fixedPointValue;

            return new FP(fixedPointValue);
        }

        /// <summary>
        ///     Converts a single-precision floating-point value to an instance of the FP, with rounding to the nearest
        ///     representable FP.
        ///     This method is marked as unsafe because it is not deterministic.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static FP FromRoundedFloat_UNSAFE(float value) => new FP((long)Math.Round((double)value * 65536.0));

        /// <summary>
        ///     Converts a single-precision floating-point value to an instance of the FP, with rounding towards zero..
        ///     To round towards nearest representable FP, use
        ///     <see cref="M:Herta.FP.FromRoundedFloat_UNSAFE(System.Single)" />.
        ///     This method is marked as unsafe because it is not deterministic.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static FP FromFloat_UNSAFE(float value) => new FP(checked((long)unchecked((double)value * 65536.0)));

        public static FP FromFloat_SAFE(float value)
        {
            const int Q16_SHIFT = 16; // 16 fractional bits (Q48.16 format)
            const int FLOAT_MANTISSA_BITS = 23;
            const int FLOAT_EXPONENT_BIAS = 127;

            // Reinterpret the float as an int (IEEE 754 binary32 format)
            int bits = Unsafe.As<float, int>(ref value);

            // Handle zero specially to avoid issues with denormals
            if (bits == 0)
                return new FP(0);

            // Extract sign (1 bit)
            bool isNegative = (bits & 0x80000000) != 0;

            // Extract exponent (8 bits, biased)
            int exponent = ((bits >> FLOAT_MANTISSA_BITS) & 0xFF) - FLOAT_EXPONENT_BIAS;

            // Extract mantissa (23 bits, with implicit leading 1)
            int mantissa = (bits & 0x007FFFFF) | 0x00800000;

            // Compute the shift needed to align the mantissa to Q48.16
            int shift = (FLOAT_MANTISSA_BITS - Q16_SHIFT) - exponent;

            // Shift the mantissa into Q48.16 fixed-point
            long fixedPointValue;
            if (shift >= 0)
            {
                fixedPointValue = (long)mantissa >> shift;
            }
            else
            {
                // For left shifts, we need to handle potential overflow
                if (shift < -41) // More than 41 left shifts would overflow long
                {
                    fixedPointValue = isNegative ? long.MinValue : long.MaxValue;
                }
                else
                {
                    fixedPointValue = (long)mantissa << -shift;
                }
            }

            // Apply sign
            fixedPointValue = isNegative ? -fixedPointValue : fixedPointValue;

            return new FP(fixedPointValue);
        }

        /// <summary>Converts a raw integer value to an instance of FP.</summary>
        /// <param name="value">The raw integer value to convert.</param>
        /// <returns>A new instance of FP that represents the same value as the raw integer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP FromRaw(long value)
        {
            FP fp;
            fp.RawValue = value;
            return fp;
        }

        /// <summary>
        ///     Creates an instance of FP from a string representation of a float value.
        ///     This method is marked as unsafe because it is not deterministic.
        /// </summary>
        /// <param name="value">The string representation of the float value.</param>
        /// <returns>An instance of FP representing the float value.</returns>
        [Obsolete("Use FromString instead.")]
        public static FP FromString_UNSAFE(ReadOnlySpan<char> value) => FP.FromFloat_UNSAFE((float)double.Parse(value, NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)CultureInfo.InvariantCulture));

        public static FP Parse(ReadOnlySpan<char> s)
        {
            if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(s)))
                return FP._0;
            s = s.Trim();
            if (s.Length == 0)
                return FP._0;
            bool flag1 = (s[0] == '-');
            if (flag1)
                s = s.Slice(1);
            bool flag2 = s[0] == '.';
            Span<Range> strArray = stackalloc Range[2];
            int length = 0;
            foreach (Range range in new NativeSplitAnyRange<char>(s, ". "))
            {
                if (range.Start.Value == range.End.Value)
                    continue;

                if (length == 2)
                    throw new FormatException(s.ToString());

                strArray[length++] = range;
            }

            long v;
            switch (length)
            {
                case 1:
                    s = s[strArray[0].Start.Value..strArray[0].End.Value];
                    v = !flag2 ? FP.ParseInteger(s) : FP.ParseFractions(s);
                    break;
                case 2:
                    ReadOnlySpan<char> local1 = s[strArray[0].Start.Value..strArray[0].End.Value];
                    ReadOnlySpan<char> local2 = s[strArray[1].Start.Value..strArray[1].End.Value];
                    v = checked(FP.ParseInteger(local1) + FP.ParseFractions(local2));
                    break;
                default:
                    throw new FormatException(s.ToString());
            }

            return flag1 ? new FP(-v) : new FP(v);
        }

        public static bool TryParse(ReadOnlySpan<char> s, out FP result)
        {
            if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(s)))
            {
                result = FP._0;
                return true;
            }

            s = s.Trim();
            if (s.Length == 0)
            {
                result = FP._0;
                return true;
            }

            bool flag1 = (s[0] == '-');
            if (flag1)
                s = s.Slice(1);
            bool flag2 = s[0] == '.';
            Span<Range> strArray = stackalloc Range[2];
            int length = 0;
            foreach (Range range in new NativeSplitAnyRange<char>(s, ". "))
            {
                if (range.Start.Value == range.End.Value)
                    continue;

                if (length == 2)
                {
                    result = default;
                    return false;
                }

                strArray[length++] = range;
            }

            long v;
            switch (length)
            {
                case 1:
                    Range range = strArray[0];
                    s = s[range.Start.Value..range.End.Value];
                    v = !flag2 ? FP.ParseInteger(s) : FP.ParseFractions(s);
                    break;
                case 2:
                    Range range1 = strArray[0];
                    Range range2 = strArray[1];
                    ReadOnlySpan<char> local1 = s[range1.Start.Value..range1.End.Value];
                    ReadOnlySpan<char> local2 = s[range2.Start.Value..range2.End.Value];
                    v = checked(FP.ParseInteger(local1) + FP.ParseFractions(local2));
                    break;
                default:
                    result = default;
                    return false;
            }

            result = flag1 ? new FP(-v) : new FP(v);
            return true;
        }

        /// <summary>
        ///     Converts a string representation of a fixed-point number to an instance of the FP struct.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="T:System.FormatException"></exception>
        public static FP FromString(ReadOnlySpan<char> s) => Parse(s);

        private static long ParseInteger(ReadOnlySpan<char> format) => long.Parse(format) * 65536L;

        private static long ParseFractions(ReadOnlySpan<char> format)
        {
            long num;
            switch (format.Length)
            {
                case 0:
                    return 0;
                case 1:
                    num = 10L;
                    break;
                case 2:
                    num = 100L;
                    break;
                case 3:
                    num = 1000L;
                    break;
                case 4:
                    num = 10000L;
                    break;
                case 5:
                    num = 100000L;
                    break;
                case 6:
                    num = 1000000L;
                    break;
                case 7:
                    num = 10000000L;
                    break;
                default:
                    if (format.Length > 14)
                        format = format.Slice(0, 14);
                    num = 100000000L;
                    for (int index = 8; index < format.Length; ++index)
                        num *= 10L;
                    break;
            }

            return (long.Parse(format) * 65536L + num / 2L) / num;
        }

        internal static long RawMultiply(FP x, FP y) => x.RawValue * y.RawValue + 32768L >> 16;

        internal static long RawMultiply(FP x, FP y, FP z)
        {
            y.RawValue = x.RawValue * y.RawValue + 32768L >> 16;
            return y.RawValue * z.RawValue + 32768L >> 16;
        }

        internal static long RawMultiply(FP x, FP y, FP z, FP a)
        {
            y.RawValue = x.RawValue * y.RawValue + 32768L >> 16;
            z.RawValue = y.RawValue * z.RawValue + 32768L >> 16;
            return z.RawValue * a.RawValue + 32768L >> 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP MulTruncate(FP x, FP y) => FP.FromRaw(x.RawValue * y.RawValue >> 16);

        internal (int Sign, ulong Integer, uint Fraction) GetDecimalParts()
        {
            int num1;
            ulong num2;
            if (this.RawValue < 0L)
            {
                num1 = -1;
                num2 = (ulong)-this.RawValue;
            }
            else
            {
                num1 = 1;
                num2 = (ulong)this.RawValue;
            }

            uint num3 = (uint)(num2 & (ulong)ushort.MaxValue);
            ulong num4 = num2 >> 16;
            uint num5 = FPLut.Fraction[(int)num3];
            return (num1, num4, num5);
        }

        /// <summary>Negates the value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator -(FP a)
        {
            a.RawValue = -a.RawValue;
            return a;
        }

        /// FP.Operators.cs
        /// <summary>Converts the value to its absolute version.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator +(FP a)
        {
            return a;
        }

        /// <summary>Represents the operator to add two FP values.</summary>
        /// <param name="a">The first FP value.</param>
        /// <param name="b">The second FP value.</param>
        /// <returns>The sum of the two FP values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator +(FP a, FP b)
        {
            a.RawValue += b.RawValue;
            return a;
        }

        /// <summary>
        ///     Overloads the addition operator to add an integer value to an FP value.
        /// </summary>
        /// <param name="a">The FP value.</param>
        /// <param name="b">The integer value to add.</param>
        /// <returns>The result of adding the integer value to the FP value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator +(FP a, int b)
        {
            a.RawValue += (long)b << 16;
            return a;
        }

        /// <summary>
        ///     Represents the operator overloading for adding an integer and an FP value.
        /// </summary>
        /// <param name="a">The integer value.</param>
        /// <param name="b">The FP value.</param>
        /// <returns>The result of adding the integer and FP values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator +(int a, FP b)
        {
            b.RawValue = ((long)a << 16) + b.RawValue;
            return b;
        }

        /// <summary>Subtracts two FP (fixed point) values.</summary>
        /// <param name="a">The first FP value.</param>
        /// <param name="b">The second FP value.</param>
        /// <returns>The result of subtracting the second FP value from the first FP value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator -(FP a, FP b)
        {
            a.RawValue -= b.RawValue;
            return a;
        }

        /// <summary>Subtracts an integer value from an FP value.</summary>
        /// <param name="a">The FP value.</param>
        /// <param name="b">The integer value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator -(FP a, int b)
        {
            a.RawValue -= (long)b << 16;
            return a;
        }

        /// <summary>
        ///     Represents an overloaded operator for negating the value of an integer.
        /// </summary>
        /// <param name="a">The integer value to be negated.</param>
        /// <param name="b">The FP value to subtract from.</param>
        /// <returns>The result of subtracting the FP value from the negated integer value.</returns>
        /// <remarks>
        ///     This operator subtracts the FP value from the negated integer value by shifting the integer value to the left by
        ///     the precision of FP,
        ///     then subtracting the raw value of FP from it. The result is then returned as a new FP value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator -(int a, FP b)
        {
            b.RawValue = ((long)a << 16) - b.RawValue;
            return b;
        }

        /// <summary>Represents the operator to multiply two FP values.</summary>
        /// <param name="a">The first FP value.</param>
        /// <param name="b">The Second FP value</param>
        /// <returns>The product.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator *(FP a, FP b)
        {
            a.RawValue = a.RawValue * b.RawValue + 32768L >> 16;
            return a;
        }

        /// <summary>
        ///     Represents the operator for multiplying a floating-point value by an integer value.
        /// </summary>
        /// <param name="a">The floating-point value.</param>
        /// <param name="b">The integer value.</param>
        /// <returns>The result of multiplying the floating-point value by the integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator *(FP a, int b)
        {
            a.RawValue *= (long)b;
            return a;
        }

        /// <summary>Multiplies an integer value by an FP value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator *(int a, FP b)
        {
            b.RawValue = (long)a * b.RawValue;
            return b;
        }

        /// <summary>
        ///     Represents an operator to perform division on two FP (fixed point) numbers.
        /// </summary>
        /// <param name="a">The dividend.</param>
        /// <param name="b">The divisor.</param>
        /// <returns>The result of the division operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator /(FP a, FP b)
        {
            a.RawValue = (a.RawValue << 16) / b.RawValue;
            return a;
        }

        /// <summary>Divides an FP value by an integer value.</summary>
        /// <param name="a">The first FP value.</param>
        /// <param name="b">The second Int32 value.</param>
        /// <returns>The divided result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator /(FP a, int b)
        {
            a.RawValue /= (long)b;
            return a;
        }

        /// <summary>
        ///     This operator takes an integer value (`a`) and an `FP` value (`b`) and returns the result of the division of `a` by
        ///     `b`.
        /// </summary>
        /// <param name="a">The integer value to be divided.</param>
        /// <param name="b">The `FP` value to divide by.</param>
        /// <returns>An `FP` value representing the result of the division of `a` by `b`.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator /(int a, FP b)
        {
            b.RawValue = ((long)a << 32) / b.RawValue;
            return b;
        }

        /// <summary>Divides an FP value by a high precision divisor.</summary>
        /// <param name="a">The FP value.</param>
        /// <param name="b">The HighPrecisionDivisor.</param>
        /// <returns>The result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator /(FP a, FPHighPrecisionDivisor b)
        {
            a.RawValue = FPHighPrecisionDivisor.RawDiv(a.RawValue, b.RawValue);
            return a;
        }

        /// <summary>Modulo operator for FP values.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator %(FP a, FP b)
        {
            a.RawValue %= b.RawValue;
            return a;
        }

        /// <summary>Modulo operator for FP and integer values.</summary>
        /// <param name="a">The FP value.</param>
        /// <param name="b">The integer value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator %(FP a, int b)
        {
            a.RawValue %= (long)b << 16;
            return a;
        }

        /// <summary>Modulo operator for integer and FP values.</summary>
        /// <param name="a">The integer value.</param>
        /// <param name="b">The FP value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator %(int a, FP b)
        {
            b.RawValue = ((long)a << 16) % b.RawValue;
            return b;
        }

        /// <summary>Modulo operator for FP and high precision divisor.</summary>
        /// <param name="a">The FP value.</param>
        /// <param name="b">The high precision divisor.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator %(FP a, FPHighPrecisionDivisor b)
        {
            a.RawValue = FPHighPrecisionDivisor.RawMod(a.RawValue, b.RawValue);
            return a;
        }

        /// <summary>Represents the operator to compare two FP values.</summary>
        /// <param name="a">The first FP value.</param>
        /// <param name="b">The second FP value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FP a, FP b) => a.RawValue < b.RawValue;

        /// <summary>
        ///     Represents the operator to compare an FP value with an integer value.
        /// </summary>
        /// <param name="a">The FP value.</param>
        /// <param name="b">The integer value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FP a, int b) => a.RawValue < (long)b << 16;

        /// <summary>
        ///     Represents the operator to compare an integer value with an FP value.
        /// </summary>
        /// <param name="a">The integer value.</param>
        /// <param name="b">The FP value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(int a, FP b) => (long)a << 16 < b.RawValue;

        /// <summary>Represents the operator to compare two FP values.</summary>
        /// <param name="a">The first FP value.</param>
        /// <param name="b">The second FP value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FP a, FP b) => a.RawValue <= b.RawValue;

        /// <summary>
        ///     Code that defines the operator for less than or equal to comparison between a FP (Fixed Point) value and an integer
        ///     value.
        /// </summary>
        /// <param name="a">The FP value to compare</param>
        /// <param name="b">The integer value to compare</param>
        /// <returns>
        ///     Returns <see langword="true" /> if the FP value is less than or equal to the integer value, otherwise
        ///     <see langword="false" />
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FP a, int b) => a.RawValue <= (long)b << 16;

        /// <summary>
        ///     Code that defines the operator for less than or equal to comparison between an integer value and a FP (Fixed Point)
        ///     value.
        /// </summary>
        /// <param name="a">The integer value to compare</param>
        /// <param name="b">The FP value to compare</param>
        /// <returns>
        ///     Returns <see langword="true" /> if the integer value is less than or equal to the FP value, otherwise
        ///     <see langword="false" />
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(int a, FP b) => (long)a << 16 <= b.RawValue;

        /// <summary>Represents the operator to compare two FP values.</summary>
        /// <param name="a">The first FP value.</param>
        /// <param name="b">The second FP value.</param>
        /// <returns><see langword="true" /> if the first value is greater than the second, otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FP a, FP b) => a.RawValue > b.RawValue;

        /// <summary>
        ///     Represents the operator to compare an FP value with an integer value.
        /// </summary>
        /// <param name="a">The FP value.</param>
        /// <param name="b">The integer value.</param>
        /// <returns><see langword="true" /> if the FP value is greater than the integer value, otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FP a, int b) => a.RawValue > (long)b << 16;

        /// <summary>
        ///     Represents the operator to compare an integer value with an FP value.
        /// </summary>
        /// <param name="a">The integer value.</param>
        /// <param name="b">The FP value.</param>
        /// <returns><see langword="true" /> if the integer value is greater than the FP value, otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(int a, FP b) => (long)a << 16 > b.RawValue;

        /// <summary>Represents the operator to compare two FP values.</summary>
        /// <param name="a">The first FP value.</param>
        /// <param name="b">The second FP value.</param>
        /// <returns>
        ///     <see langword="true" /> if the first value is greater than or equal to the second, otherwise
        ///     <see langword="false" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FP a, FP b) => a.RawValue >= b.RawValue;

        /// <summary>
        ///     Represents the operator to compare an FP value with an integer value.
        /// </summary>
        /// <param name="a">The FP value.</param>
        /// <param name="b">The integer value.</param>
        /// <returns>
        ///     <see langword="true" /> if the FP value is greater than or equal to the integer value, otherwise
        ///     <see langword="false" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FP a, int b) => a.RawValue >= (long)b << 16;

        /// <summary>
        ///     Represents the operator to compare an integer value with an FP value.
        /// </summary>
        /// <param name="a">The integer value.</param>
        /// <param name="b">The FP value.</param>
        /// <returns>
        ///     <see langword="true" /> if the integer value is greater than or equal to the FP value, otherwise
        ///     <see langword="false" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(int a, FP b) => (long)a << 16 >= b.RawValue;

        /// <summary>Compares two FP values for equality.</summary>
        /// <param name="a">The first FP value.</param>
        /// <param name="b">The second FP value.</param>
        /// <returns><see langword="true" /> if the two values are equal, otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FP a, FP b) => a.RawValue == b.RawValue;

        /// <summary>
        ///     Compares an FP value with an integer value for equality.
        /// </summary>
        /// <param name="a">The FP value.</param>
        /// <param name="b">The integer value.</param>
        /// <returns><see langword="true" /> if the two values are equal, otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FP a, int b) => a.RawValue == (long)b << 16;

        /// <summary>
        ///     Compares an integer value with an FP value for equality.
        /// </summary>
        /// <param name="a">The integer value.</param>
        /// <param name="b">The FP value.</param>
        /// <returns><see langword="true" /> if the two values are equal, otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(int a, FP b) => (long)a << 16 == b.RawValue;

        /// <summary>Compares two FP values for inequality.</summary>
        /// <param name="a">The first FP value.</param>
        /// <param name="b">The second FP value.</param>
        /// <returns><see langword="true" /> if the two values are not equal, otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FP a, FP b) => a.RawValue != b.RawValue;

        /// <summary>
        ///     Compares an FP value with an integer value for inequality.
        /// </summary>
        /// <param name="a">The FP value.</param>
        /// <param name="b">The integer value.</param>
        /// <returns><see langword="true" /> if the two values are not equal, otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FP a, int b) => a.RawValue != (long)b << 16;

        /// <summary>
        ///     Compares an integer value with an FP value for inequality.
        /// </summary>
        /// <param name="a">The integer value.</param>
        /// <param name="b">The FP value.</param>
        /// <returns><see langword="true" /> if the two values are not equal, otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(int a, FP b) => (long)a << 16 != b.RawValue;

        /// <summary>Converts an integer value to an FP value.</summary>
        /// <param name="value">The integer value to convert.</param>
        /// <returns>The FP value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(int value)
        {
            FP fp;
            fp.RawValue = (long)value << 16;
            return fp;
        }

        /// <summary>Converts an integer value to an FP value.</summary>
        /// <param name="value">The integer value to convert.</param>
        /// <returns>The FP value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(uint value)
        {
            FP fp;
            fp.RawValue = (long)value << 16;
            return fp;
        }

        /// <summary>Converts an integer value to an FP value.</summary>
        /// <param name="value">The integer value to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(short value)
        {
            FP fp;
            fp.RawValue = (long)value << 16;
            return fp;
        }

        /// <summary>Converts an integer value to an FP value.</summary>
        /// <param name="value">The integer value to convert.</param>
        /// <returns>The FP value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(ushort value)
        {
            FP fp;
            fp.RawValue = (long)value << 16;
            return fp;
        }

        /// <summary>
        ///     Implicitly converts a signed byte value to an instance of the FP struct.
        /// </summary>
        /// <param name="value">The signed byte value to be converted.</param>
        /// <returns>An instance of the FP struct representing the converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(sbyte value)
        {
            FP fp;
            fp.RawValue = (long)value << 16;
            return fp;
        }

        /// <summary>
        ///     Implicit conversion operator for converting a Byte value to an FP (Fixed Point) value.
        /// </summary>
        /// <param name="value">The Byte value to be converted.</param>
        /// <returns>An FP value representing the converted Byte value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP(byte value)
        {
            FP fp;
            fp.RawValue = (long)value << 16;
            return fp;
        }

        /// <summary>Converts an integer value to an FP value.</summary>
        /// <param name="value">The integer value to convert.</param>
        /// <returns>The FP value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(FP value) => (int)(value.RawValue >> 16);

        /// <summary>Converts an FP value to an integer value.</summary>
        /// <param name="value">The FP value to convert.</param>
        /// <returns>The integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator long(FP value) => value.RawValue >> 16;

        /// <summary>Converts an FP value to a float value.</summary>
        /// <param name="value">The FP value to convert.</param>
        /// <returns>The float value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float(FP value) => (float)value.RawValue / 65536f;

        /// <summary>Converts an FP value to a double value.</summary>
        /// <param name="value">The FP value to convert.</param>
        /// <returns>The double value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double(FP value) => (double)value.RawValue / 65536.0;

        public static implicit operator FP(float value) => FromFloat_SAFE(value);

        public static implicit operator FP(double value) => FromDouble_SAFE(value);

        /// <summary>
        ///     Holds <see cref="T:Herta.FP" /> constants in raw (long) form.
        /// </summary>
        public static class Raw
        {
            /// <summary>
            ///     The smallest FP unit that is not 0.
            ///     <para>Closest double: 1.52587890625E-05</para>
            /// </summary>
            public const long SmallestNonZero = 1;

            /// <summary>
            ///     Minimum FP value, but values outside of <see cref="F:Herta.FP.Raw.UseableMin" /> and
            ///     <see cref="F:Herta.FP.Raw.UseableMax" /> (inclusive) can overflow when multiplied.
            ///     <para>Closest double: -140737488355328</para>
            /// </summary>
            public const long MinValue = -9223372036854775808;

            /// <summary>
            ///     Maximum FP value, but values outside of <see cref="F:Herta.FP.Raw.UseableMin" /> and
            ///     <see cref="F:Herta.FP.Raw.UseableMax" /> (inclusive) can overflow when multiplied.
            ///     <para>Closest double: 140737488355328</para>
            /// </summary>
            public const long MaxValue = 9223372036854775807;

            /// <summary>
            ///     Represents the highest negative FP number that can be multiplied with itself and not cause an overflow (exceeding
            ///     long range).
            ///     <para>Closest double: -32768</para>
            /// </summary>
            public const long UseableMin = -2147483648;

            /// <summary>
            ///     Represents the highest FP number that can be multiplied with itself and not cause an overflow (exceeding long
            ///     range).
            ///     <para>Closest double: 32767.9999847412</para>
            /// </summary>
            public const long UseableMax = 2147483647;

            /// <summary>
            ///     Pi number.
            ///     <para>Closest double: 3.14158630371094</para>
            /// </summary>
            public const long Pi = 205887;

            /// <summary>
            ///     1/Pi.
            ///     <para>Closest double: 0.318313598632813</para>
            /// </summary>
            public const long PiInv = 20861;

            /// <summary>
            ///     2 * Pi.
            ///     <para>Closest double: 6.28318786621094</para>
            /// </summary>
            public const long PiTimes2 = 411775;

            /// <summary>
            ///     Pi / 2.
            ///     <para>Closest double: 1.57080078125</para>
            /// </summary>
            public const long PiOver2 = 102944;

            /// <summary>
            ///     2 / Pi.
            ///     <para>Closest double: 0.636627197265625</para>
            /// </summary>
            public const long PiOver2Inv = 41722;

            /// <summary>
            ///     Pi / 4.
            ///     <para>Closest double: 0.785400390625</para>
            /// </summary>
            public const long PiOver4 = 51472;

            /// <summary>
            ///     3 * Pi / 4.
            ///     <para>Closest double: 2.356201171875</para>
            /// </summary>
            public const long Pi3Over4 = 154416;

            /// <summary>
            ///     4 * Pi / 3.
            ///     <para>Closest double: 4.18879699707031</para>
            /// </summary>
            public const long Pi4Over3 = 274517;

            /// <summary>
            ///     Degrees-to-radians conversion constant.
            ///     <para>Closest double: 0.0174560546875</para>
            /// </summary>
            public const long Deg2Rad = 1144;

            /// <summary>
            ///     Radians-to-degrees conversion constant.
            ///     <para>Closest double: 57.2957763671875</para>
            /// </summary>
            public const long Rad2Deg = 3754936;

            /// <summary>
            ///     FP constant representing the number 0.
            ///     <para>Closest double: 0</para>
            /// </summary>
            public const long _0 = 0;

            /// <summary>
            ///     FP constant representing the number 1.
            ///     <para>Closest double: 1</para>
            /// </summary>
            public const long _1 = 65536;

            /// <summary>
            ///     FP constant representing the number 2.
            ///     <para>Closest double: 2</para>
            /// </summary>
            public const long _2 = 131072;

            /// <summary>
            ///     FP constant representing the number 3.
            ///     <para>Closest double: 3</para>
            /// </summary>
            public const long _3 = 196608;

            /// <summary>
            ///     FP constant representing the number 4.
            ///     <para>Closest double: 4</para>
            /// </summary>
            public const long _4 = 262144;

            /// <summary>
            ///     FP constant representing the number 5.
            ///     <para>Closest double: 5</para>
            /// </summary>
            public const long _5 = 327680;

            /// <summary>
            ///     FP constant representing the number 6.
            ///     <para>Closest double: 6</para>
            /// </summary>
            public const long _6 = 393216;

            /// <summary>
            ///     FP constant representing the number 7.
            ///     <para>Closest double: 7</para>
            /// </summary>
            public const long _7 = 458752;

            /// <summary>
            ///     FP constant representing the number 8.
            ///     <para>Closest double: 8</para>
            /// </summary>
            public const long _8 = 524288;

            /// <summary>
            ///     FP constant representing the number 9.
            ///     <para>Closest double: 9</para>
            /// </summary>
            public const long _9 = 589824;

            /// <summary>
            ///     FP constant representing the number 10.
            ///     <para>Closest double: 10</para>
            /// </summary>
            public const long _10 = 655360;

            /// <summary>
            ///     FP constant representing the number 99.
            ///     <para>Closest double: 99</para>
            /// </summary>
            public const long _99 = 6488064;

            /// <summary>
            ///     FP constant representing the number 100.
            ///     <para>Closest double: 100</para>
            /// </summary>
            public const long _100 = 6553600;

            /// <summary>
            ///     FP constant representing the number 180.
            ///     <para>Closest double: 180</para>
            /// </summary>
            public const long _180 = 11796480;

            /// <summary>
            ///     FP constant representing the number 200.
            ///     <para>Closest double: 200</para>
            /// </summary>
            public const long _200 = 13107200;

            /// <summary>
            ///     FP constant representing the number 360.
            ///     <para>Closest double: 360</para>
            /// </summary>
            public const long _360 = 23592960;

            /// <summary>
            ///     FP constant representing the number 1000.
            ///     <para>Closest double: 1000</para>
            /// </summary>
            public const long _1000 = 65536000;

            /// <summary>
            ///     FP constant representing the number 10000.
            ///     <para>Closest double: 10000</para>
            /// </summary>
            public const long _10000 = 655360000;

            /// <summary>
            ///     FP constant representing the number 0.01.
            ///     <para>Closest double: 0.0099945068359375</para>
            /// </summary>
            public const long _0_01 = 655;

            /// <summary>
            ///     FP constant representing the number 0.02.
            ///     <para>Closest double: 0.0200042724609375</para>
            /// </summary>
            public const long _0_02 = 1311;

            /// <summary>
            ///     FP constant representing the number 0.03.
            ///     <para>Closest double: 0.029998779296875</para>
            /// </summary>
            public const long _0_03 = 1966;

            /// <summary>
            ///     FP constant representing the number 0.04.
            ///     <para>Closest double: 0.0399932861328125</para>
            /// </summary>
            public const long _0_04 = 2621;

            /// <summary>
            ///     FP constant representing the number 0.05.
            ///     <para>Closest double: 0.0500030517578125</para>
            /// </summary>
            public const long _0_05 = 3277;

            /// <summary>
            ///     FP constant representing the number 0.10.
            ///     <para>Closest double: 0.100006103515625</para>
            /// </summary>
            public const long _0_10 = 6554;

            /// <summary>
            ///     FP constant representing the number 0.20.
            ///     <para>Closest double: 0.199996948242188</para>
            /// </summary>
            public const long _0_20 = 13107;

            /// <summary>
            ///     FP constant representing the number 0.25.
            ///     <para>Closest double: 0.25</para>
            /// </summary>
            public const long _0_25 = 16384;

            /// <summary>
            ///     FP constant representing the number 0.50.
            ///     <para>Closest double: 0.5</para>
            /// </summary>
            public const long _0_50 = 32768;

            /// <summary>
            ///     FP constant representing the number 0.75.
            ///     <para>Closest double: 0.75</para>
            /// </summary>
            public const long _0_75 = 49152;

            /// <summary>
            ///     FP constant representing the number 0.33.
            ///     <para>Closest double: 0.333328247070313</para>
            /// </summary>
            public const long _0_33 = 21845;

            /// <summary>
            ///     FP constant representing the number 0.99.
            ///     <para>Closest double: 0.990005493164063</para>
            /// </summary>
            public const long _0_99 = 64881;

            /// <summary>
            ///     FP constant representing the number -1.
            ///     <para>Closest double: -1</para>
            /// </summary>
            public const long Minus_1 = -65536;

            /// <summary>
            ///     FP constant representing 360 degrees in radian.
            ///     <para>Closest double: 6.28318786621094</para>
            /// </summary>
            public const long Rad_360 = 411775;

            /// <summary>
            ///     FP constant representing 180 degrees in radian.
            ///     <para>Closest double: 3.14158630371094</para>
            /// </summary>
            public const long Rad_180 = 205887;

            /// <summary>
            ///     FP constant representing 90 degrees in radian.
            ///     <para>Closest double: 1.57080078125</para>
            /// </summary>
            public const long Rad_90 = 102944;

            /// <summary>
            ///     FP constant representing 45 degrees in radian.
            ///     <para>Closest double: 0.785400390625</para>
            /// </summary>
            public const long Rad_45 = 51472;

            /// <summary>
            ///     FP constant representing 22.5 degrees in radian.
            ///     <para>Closest double: 0.3927001953125</para>
            /// </summary>
            public const long Rad_22_50 = 25736;

            /// <summary>
            ///     FP constant representing the number 1.01.
            ///     <para>Closest double: 1.00999450683594</para>
            /// </summary>
            public const long _1_01 = 66191;

            /// <summary>
            ///     FP constant representing the number 1.02.
            ///     <para>Closest double: 1.02000427246094</para>
            /// </summary>
            public const long _1_02 = 66847;

            /// <summary>
            ///     FP constant representing the number 1.03.
            ///     <para>Closest double: 1.02999877929688</para>
            /// </summary>
            public const long _1_03 = 67502;

            /// <summary>
            ///     FP constant representing the number 1.04.
            ///     <para>Closest double: 1.03999328613281</para>
            /// </summary>
            public const long _1_04 = 68157;

            /// <summary>
            ///     FP constant representing the number 1.05.
            ///     <para>Closest double: 1.05000305175781</para>
            /// </summary>
            public const long _1_05 = 68813;

            /// <summary>
            ///     FP constant representing the number 1.10.
            ///     <para>Closest double: 1.10000610351563</para>
            /// </summary>
            public const long _1_10 = 72090;

            /// <summary>
            ///     FP constant representing the number 1.20.
            ///     <para>Closest double: 1.19999694824219</para>
            /// </summary>
            public const long _1_20 = 78643;

            /// <summary>
            ///     FP constant representing the number 1.25.
            ///     <para>Closest double: 1.25</para>
            /// </summary>
            public const long _1_25 = 81920;

            /// <summary>
            ///     FP constant representing the number 1.50.
            ///     <para>Closest double: 1.5</para>
            /// </summary>
            public const long _1_50 = 98304;

            /// <summary>
            ///     FP constant representing the number 1.75.
            ///     <para>Closest double: 1.75</para>
            /// </summary>
            public const long _1_75 = 114688;

            /// <summary>
            ///     FP constant representing the number 1.33.
            ///     <para>Closest double: 1.33332824707031</para>
            /// </summary>
            public const long _1_33 = 87381;

            /// <summary>
            ///     FP constant representing the number 1.99.
            ///     <para>Closest double: 1.99000549316406</para>
            /// </summary>
            public const long _1_99 = 130417;

            /// <summary>
            ///     FP constant representing the epsilon value EN1.
            ///     <para>Closest double: 0.100006103515625</para>
            /// </summary>
            public const long EN1 = 6554;

            /// <summary>
            ///     FP constant representing the epsilon value EN2.
            ///     <para>Closest double: 0.0099945068359375</para>
            /// </summary>
            public const long EN2 = 655;

            /// <summary>
            ///     FP constant representing the epsilon value EN3.
            ///     <para>Closest double: 0.001007080078125</para>
            /// </summary>
            public const long EN3 = 66;

            /// <summary>
            ///     FP constant representing the epsilon value EN4.
            ///     <para>Closest double: 0.0001068115234375</para>
            /// </summary>
            public const long EN4 = 7;

            /// <summary>
            ///     FP constant representing the epsilon value EN5.
            ///     <para>Closest double: 1.52587890625E-05</para>
            /// </summary>
            public const long EN5 = 1;

            /// <summary>
            ///     FP constant representing Epsilon <see cref="F:Herta.FP.Raw.EN3" />.
            ///     <para>Closest double: 0.001007080078125</para>
            /// </summary>
            public const long Epsilon = 66;

            /// <summary>
            ///     FP constant representing the Euler Number constant.
            ///     <para>Closest double: 2.71827697753906</para>
            /// </summary>
            public const long E = 178145;

            /// <summary>
            ///     FP constant representing Log(E).
            ///     <para>Closest double: 1.44268798828125</para>
            /// </summary>
            public const long Log2_E = 94548;

            /// <summary>
            ///     FP constant representing Log(10).
            ///     <para>Closest double: 3.32192993164063</para>
            /// </summary>
            public const long Log2_10 = 217706;
        }

        /// <summary>
        ///     Compares <see cref="T:Herta.FP" />s.
        /// </summary>
        public class Comparer : System.Collections.Generic.Comparer<FP>
        {
            /// <summary>A global FP comparer instance.</summary>
            public static readonly FP.Comparer Instance = new FP.Comparer();

            private Comparer()
            {
            }

            /// <summary>
            ///     Compares two instances of FP and returns an integer that indicates whether the first instance is less than, equal
            ///     to, or greater than the second instance.
            /// </summary>
            /// <param name="x">The first instance to compare.</param>
            /// <param name="y">The second instance to compare.</param>
            /// <returns>
            ///     A signed integer that indicates the relative values of x and y, as shown in the following table:
            ///     - Less than zero: x is less than y.
            ///     - Zero: x equals y.
            ///     - Greater than zero: x is greater than y.
            /// </returns>
            public override int Compare(FP x, FP y) => x.RawValue.CompareTo(y.RawValue);
        }

        /// <summary>
        ///     Equality comparer for <see cref="T:Herta.FP" />s.
        /// </summary>
        public class EqualityComparer : IEqualityComparer<FP>
        {
            /// <summary>A global FP equality comparer instance.</summary>
            public static readonly FP.EqualityComparer Instance = new FP.EqualityComparer();

            private EqualityComparer()
            {
            }

            bool IEqualityComparer<FP>.Equals(FP x, FP y) => x.RawValue == y.RawValue;

            int IEqualityComparer<FP>.GetHashCode(FP num) => num.RawValue.GetHashCode();
        }
    }
}