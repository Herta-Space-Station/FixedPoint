using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CA2208
#pragma warning disable CS8632

// ReSharper disable ALL

namespace Herta
{
    /// <summary>A collection of common math functions.</summary>
    /// \ingroup MathAPI
    public static class FPMath
    {
        /// <summary>
        ///     Returns the sign of <paramref name="value" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>1 when positive or zero, -1 when negative</returns>
        public static FP Sign(FP value) => value.RawValue >= 0L ? FP._1 : FP.Minus_1;

        /// <summary>
        ///     Returns the sign of <paramref name="value" /> if it is non-zero.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>1 when positive, 0 when zero, -1 when negative</returns>
        public static FP SignZero(FP value)
        {
            if (value.RawValue < 0L)
                return FP.Minus_1;
            return value.RawValue > 0L ? FP._1 : FP._0;
        }

        /// <summary>
        ///     Returns the sign of <paramref name="value" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>1 when positive or zero, -1 when negative</returns>
        public static int SignInt(FP value) => value.RawValue >= 0L ? 1 : -1;

        /// <summary>
        ///     Returns the sign of <paramref name="value" /> if it is non-zero.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>1 when positive, 0 when zero, -1 when negative</returns>
        public static int SignZeroInt(FP value)
        {
            if (value.RawValue < 0L)
                return -1;
            return value.RawValue > 0L ? 1 : 0;
        }

        /// <summary>
        ///     Returns the next power of two that is equal to, or greater than, the argument.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int NextPowerOfTwo(int value)
        {
            if (value <= 0)
                throw new InvalidOperationException("Number must be positive");
            return (int)MathB.RoundUpToPowerOf2((uint)value);
        }

        /// <summary>Returns the absolute value of the argument.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Abs(FP value)
        {
            long num = value.RawValue >> 63;
            value.RawValue = value.RawValue + num ^ num;
            return value;
        }

        /// <summary>
        ///     Returns <paramref name="value" /> rounded to the nearest integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Round(FP value)
        {
            long num = value.RawValue & (long)ushort.MaxValue;
            FP fp = FPMath.Floor(value);
            if (num < 32768L)
                return fp;
            if (num > 32768L)
                return fp + FP._1;
            return (fp.RawValue & FP._1.RawValue) != 0L ? fp + FP._1 : fp;
        }

        /// <summary>
        ///     Returns <paramref name="value" /> rounded to the nearest integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RoundToInt(FP value) => (value.RawValue & (long)ushort.MaxValue) >= FP._0_50.RawValue ? (int)((value.RawValue >> 16) + 1L) : (int)(value.RawValue >> 16);

        /// <summary>
        ///     Returns the largest integer smaller than or equal to <paramref name="value" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Floor(FP value)
        {
            value.RawValue &= -65536L;
            return value;
        }

        /// <inheritdoc cref="M:Herta.FPMath.Floor(Herta.FP)" />
        public static long FloorRaw(long value) => value & -65536L;

        /// <summary>
        ///     Returns the largest integer smaller than or equal to <paramref name="value" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int FloorToInt(FP value) => (int)(value.RawValue >> 16);

        /// <summary>
        ///     Returns the smallest integer larger than or equal to <paramref name="value" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Ceiling(FP value)
        {
            if ((value.RawValue & (long)ushort.MaxValue) != 0L)
                value.RawValue = (value.RawValue & -65536L) + 65536L;
            return value;
        }

        /// <summary>
        ///     Returns the smallest integer larger than or equal to <paramref name="value" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CeilToInt(FP value) => (value.RawValue & (long)ushort.MaxValue) >= 1L ? (int)((value.RawValue >> 16) + 1L) : (int)(value.RawValue >> 16);

        /// <summary>Returns the largest of two or more values.</summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public static FP Max(FP val1, FP val2) => val1.RawValue <= val2.RawValue ? val2 : val1;

        /// <summary>Returns the smallest of two or more values.</summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public static FP Min(FP val1, FP val2) => val1.RawValue >= val2.RawValue ? val2 : val1;

        /// <summary>Returns the smallest of two or more values.</summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static FP Min(ReadOnlySpan<FP> numbers)
        {
            if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(numbers)) || numbers.Length == 0)
                return new FP();
            FP val1 = numbers[0];
            for (int index = 1; index < numbers.Length; ++index)
                val1 = FPMath.Min(val1, numbers[index]);
            return val1;
        }

        /// <summary>Returns the minimum of three values.</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public static FP Min(FP a, FP b, FP c)
        {
            if (a > b)
                a = b;
            if (a > c)
                a = c;
            return a;
        }

        /// <summary>Returns the maximum of three values.</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static FP Max(FP a, FP b, FP c)
        {
            if (a < b)
                a = b;
            if (a < c)
                a = c;
            return a;
        }

        /// <summary>Returns the largest of two or more values.</summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static FP Max(ReadOnlySpan<FP> numbers)
        {
            if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(numbers)) || numbers.Length == 0)
                return new FP();
            FP val1 = numbers[0];
            for (int index = 1; index < numbers.Length; ++index)
                val1 = FPMath.Max(val1, numbers[index]);
            return val1;
        }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void MinMax(FP a, FP b, out FP min, out FP max)
        {
            if (a.RawValue < b.RawValue)
            {
                min = a;
                max = b;
            }
            else
            {
                min = b;
                max = a;
            }
        }

        /// <summary>
        ///     Clamps the given value between the given minimum and maximum values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static FP Clamp(FP value, FP min, FP max)
        {
            if (value.RawValue < min.RawValue)
                return min;
            return value.RawValue > max.RawValue ? max : value;
        }

        /// <summary>Clamps the given value between 0 and 1.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Clamp01(FP value)
        {
            if (value.RawValue < 0L)
                return FP._0;
            return value.RawValue > 65536L ? FP._1 : value;
        }

        /// <summary>
        ///     Clamps the given value between the given minimum and maximum values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            return value > max ? max : value;
        }

        /// <summary>
        ///     Clamps the given value between the given minimum and maximum values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static long Clamp(long value, long min, long max)
        {
            if (value < min)
                return min;
            return value > max ? max : value;
        }

        /// <summary>
        ///     Clamps the given value between <see cref="P:Herta.FP.UseableMin" /> and
        ///     <see cref="P:Herta.FP.UseableMax" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP ClampUseable(FP value)
        {
            if (value.RawValue < (long)int.MinValue)
                return FP.FromRaw((long)int.MinValue);
            return value.RawValue > (long)int.MaxValue ? FP.FromRaw((long)int.MaxValue) : value;
        }

        /// <summary>Returns the fractional part of the argument.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Fraction(FP value)
        {
            value.RawValue &= (long)ushort.MaxValue;
            return value;
        }

        /// <summary>
        ///     Loops the value <paramref name="t" />, so that it is never larger than <paramref name="length" /> and never smaller
        ///     than 0.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static FP Repeat(FP t, FP length)
        {
            FP fp;
            fp.RawValue = FPMath.RepeatRaw(t.RawValue, length.RawValue);
            return fp;
        }

        internal static long RepeatRaw(long t, long length) => t - (FPMath.FloorRaw((t << 16) / length) * length + 32768L >> 16);

        /// <summary>
        ///     Linearly interpolates between <paramref name="start" /> and <paramref name="end" /> by <paramref name="t" />.
        ///     <paramref name="t" /> is clamped to the range [0, 1]. The difference between <paramref name="start" /> and
        ///     <paramref name="end" />
        ///     is converted to a [-Pi/2, Pi/2] range.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FP LerpRadians(FP start, FP end, FP t)
        {
            long num = FPMath.RepeatRaw(end.RawValue - start.RawValue, FP.PiTimes2.RawValue);
            if (num > FP.Pi.RawValue)
                num -= FP.PiTimes2.RawValue;
            start.RawValue += num * FPMath.Clamp01(t).RawValue + 32768L >> 16;
            return start;
        }

        /// <summary>
        ///     Linearly interpolates between <paramref name="start" /> and <paramref name="end" /> by <paramref name="t" />.
        ///     <paramref name="t" /> is clamped to the range [0, 1]
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FP Lerp(FP start, FP end, FP t)
        {
            if (t.RawValue < 0L)
                t.RawValue = 0L;
            if (t.RawValue > 65536L)
                t.RawValue = 65536L;
            start.RawValue += (end.RawValue - start.RawValue) * t.RawValue + 32768L >> 16;
            return start;
        }

        /// <summary>
        ///     Linearly interpolates between <paramref name="start" /> and <paramref name="end" /> by <paramref name="t" />.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FP LerpUnclamped(FP start, FP end, FP t)
        {
            start.RawValue += (end.RawValue - start.RawValue) * t.RawValue + 32768L >> 16;
            return start;
        }

        /// <summary>
        ///     Calculates the linear parameter that produces the interpolant <paramref name="value" /> within the range [
        ///     <paramref name="start" />, <paramref name="end" />].
        ///     The result is clamped to the range [0, 1].
        ///     <remarks>Returns 0 if <paramref name="start" /> and <paramref name="end" /> are equal.</remarks>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP InverseLerp(FP start, FP end, FP value)
        {
            if (start.RawValue == end.RawValue)
                return new FP();
            value.RawValue = (value.RawValue - start.RawValue << 16) / (end.RawValue - start.RawValue);
            if (value.RawValue < 0L)
                value.RawValue = 0L;
            if (value.RawValue > 65536L)
                value.RawValue = 65536L;
            return value;
        }

        /// <summary>
        ///     Calculates the linear parameter that produces the interpolant <paramref name="value" /> within the range [
        ///     <paramref name="start" />, <paramref name="end" />].
        ///     <remarks>The resultant factor is NOT clamped to the range [0, 1].</remarks>
        ///     <remarks>Returns 0 if <paramref name="start" /> and <paramref name="end" /> are equal.</remarks>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP InverseLerpUnclamped(FP start, FP end, FP value)
        {
            if (start.RawValue == end.RawValue)
                return new FP();
            value.RawValue = (value.RawValue - start.RawValue << 16) / (end.RawValue - start.RawValue);
            return value;
        }

        /// <summary>
        ///     Moves a value <paramref name="from" /> towards <paramref name="to" /> by a value no greater than
        ///     <paramref name="maxDelta" />.
        ///     Negative values of <paramref name="maxDelta" /> push the value away from <paramref name="to" />.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDelta"></param>
        /// <returns></returns>
        public static FP MoveTowards(FP from, FP to, FP maxDelta)
        {
            FP fp1;
            fp1.RawValue = to.RawValue - from.RawValue;
            if (FPMath.Abs(fp1).RawValue <= maxDelta.RawValue)
                return to;
            FP fp2;
            fp2.RawValue = fp1.RawValue >= 0L ? from.RawValue + maxDelta.RawValue : from.RawValue - maxDelta.RawValue;
            return fp2;
        }

        /// <summary>
        ///     Interpolates between <paramref name="start" /> and <paramref name="end" /> with smoothing at the limits.
        ///     Equivalent of calling
        ///     <see
        ///         cref="M:Herta.FPMath.Hermite(Herta.FP,Herta.FP,Herta.FP,Herta.FP,Herta.FP)" />
        ///     with tangents set to 0 and clamping <paramref name="t" /> between 0 and 1.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FP SmoothStep(FP start, FP end, FP t) => FPMath.Hermite(start, FP._0, end, FP._0, FPMath.Clamp01(t));

        /// <summary>
        ///     Returns square root of <paramref name="value" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when <paramref name="value" /> is less than 0</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Sqrt(FP value)
        {
            value.RawValue = FPMath.SqrtRaw(value.RawValue);
            return value;
        }

        /// <summary>
        ///     Returns square root of <paramref name="x" />.
        /// </summary>
        /// <param name="x">The value to square.</param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The number is not positive.</exception>
        public static long SqrtRaw(long x)
        {
            if (x <= 65536L)
            {
                if (x < 0L)
                {
                    NativeString builder = new NativeString(stackalloc char[128]);
                    builder.Append("The number has to be positive: ");
                    builder.AppendFormattable(x);
                    throw new ArgumentOutOfRangeException(nameof(x), builder.ToString());
                }

                return (long)(FPLut.SqrtAprox[(int)x] >> 6);
            }

            int log2 = MathB.Log2((ulong)x);
            int num2 = log2 & ~1;
            int num3 = num2 - 16 + 2;

            int index = (int)(x >> num3);
            long raw = FPLut.SqrtAprox[index];
            long scaled = raw << (num3 >> 1);
            return scaled >> 6;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static FPMath.ExponentMantisaPair GetSqrtExponentMantissa(ulong x)
        {
            if (x <= 65536UL)
            {
                return new FPMath.ExponentMantisaPair()
                {
                    Exponent = 0,
                    Mantissa = FPLut.SqrtAprox[(int)x]
                };
            }

            int log2 = MathB.Log2((ulong)x);
            int num2 = log2 & ~1;
            int num3 = num2 - 16 + 2;

            return new ExponentMantisaPair
            {
                Exponent = num3 >> 1,
                Mantissa = FPLut.SqrtAprox[(int)(x >> num3)]
            };
        }

        /// <summary>Performs barycentric interpolation.</summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns>
        ///     <paramref name="value1" /> + (<paramref name="value2" /> - <paramref name="value1" />) * <paramref name="t1" /> + (
        ///     <paramref name="value3" /> - <paramref name="value1" />) * <paramref name="t2" />
        /// </returns>
        public static FP Barycentric(FP value1, FP value2, FP value3, FP t1, FP t2)
        {
            value1.RawValue = value1.RawValue + ((value2.RawValue - value1.RawValue) * t1.RawValue + 32768L >> 16) + ((value3.RawValue - value1.RawValue) * t2.RawValue + 32768L >> 16);
            return value1;
        }

        /// <summary>Performs Cotmull-Rom interpolation.</summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FP CatmullRom(FP value1, FP value2, FP value3, FP value4, FP t)
        {
            FP fp1;
            fp1.RawValue = t.RawValue * t.RawValue + 32768L >> 16;
            FP fp2;
            fp2.RawValue = fp1.RawValue * t.RawValue + 32768L >> 16;
            value1.RawValue = (value2.RawValue << 1) + ((value3.RawValue - value1.RawValue) * t.RawValue + 32768L >> 16) + (((value1.RawValue << 1) - (FP._5.RawValue * value2.RawValue + 32768L >> 16) + (value3.RawValue << 2) - value4.RawValue) * fp1.RawValue + 32768L >> 16) + (((FP._3.RawValue * value2.RawValue + 32768L >> 16) - value1.RawValue - (FP._3.RawValue * value3.RawValue + 32768L >> 16) + value4.RawValue) * fp2.RawValue + 32768L >> 16) >> 1;
            return value1;
        }

        /// <summary>Performs cubic Hermite interpolation.</summary>
        /// <param name="value1"></param>
        /// <param name="tangent1"></param>
        /// <param name="value2"></param>
        /// <param name="tangent2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FP Hermite(FP value1, FP tangent1, FP value2, FP tangent2, FP t)
        {
            FP fp1;
            fp1.RawValue = t.RawValue * t.RawValue + 32768L >> 16;
            FP fp2;
            fp2.RawValue = fp1.RawValue * t.RawValue + 32768L >> 16;
            FP fp3;
            if (t == FP._0)
                fp3 = value1;
            else if (t == FP._1)
                fp3 = value2;
            else
                fp3.RawValue = (((value1.RawValue << 1) - (value2.RawValue << 1) + tangent2.RawValue + tangent1.RawValue) * fp2.RawValue + 32768L >> 16) + (((FP._3.RawValue * value2.RawValue + 32768L >> 16) - (FP._3.RawValue * value1.RawValue + 32768L >> 16) - (tangent1.RawValue << 1) - tangent2.RawValue) * fp1.RawValue + 32768L >> 16) + (tangent1.RawValue * t.RawValue + 32768L >> 16) + value1.RawValue;
            return fp3;
        }

        /// <summary>
        ///     Performs modulo operation without forcing the sign of the dividend: So that ModuloClamped(-9, 10) = 1.
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="n">Divisor</param>
        /// <returns>Remainder after division</returns>
        /// <exception cref="T:System.InvalidOperationException">
        ///     When n &gt; Int64.MaxValue &gt;&gt; 2 or n &lt; Int64.MinValue
        ///     &gt;&gt; 2
        /// </exception>
        /// <exception cref="T:System.DivideByZeroException">When n == 0</exception>
        public static long ModuloClamped(long a, long n)
        {
            if (n > 2305843009213693951L)
                throw new InvalidOperationException("N too big");
            if (n < -2305843009213693952L)
                throw new InvalidOperationException("N too small");
            return (a % n + n) % n;
        }

        /// <summary>
        ///     Performs modulo operation without forcing the sign of the dividend: So that ModuloClamped(-9, 10) = 1.
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="n">Divisor</param>
        /// <returns>Remainder after division</returns>
        /// <exception cref="T:System.InvalidOperationException">
        ///     When n &gt; <see cref="P:Herta.FP.UseableMax" /> or
        ///     n &lt; <see cref="P:Herta.FP.UseableMin" />
        /// </exception>
        /// <exception cref="T:System.DivideByZeroException">When n == 0</exception>
        public static FP ModuloClamped(FP a, FP n)
        {
            if (n > FP.UseableMax)
                throw new InvalidOperationException("N too big");
            if (n < FP.UseableMin)
                throw new InvalidOperationException("N too small");
            return new FP()
            {
                RawValue = (a.RawValue % n.RawValue + n.RawValue) % n.RawValue
            };
        }

        /// <summary>
        ///     Calculates the smallest signed angle between any two angles. F.e. angle between -179 and 179 is -2. Rotation is
        ///     ccw.
        /// </summary>
        /// <param name="source">Source angle in degrees</param>
        /// <param name="target">Target angle in degrees</param>
        /// <returns></returns>
        public static FP AngleBetweenDegrees(FP source, FP target)
        {
            long num = target.RawValue - source.RawValue;
            return new FP()
            {
                RawValue = FPMath.ModuloClamped(num + 11796480L, 23592960L) - 11796480L
            };
        }

        /// <summary>Same as AngleBetweenDegrees using Raw optimization.</summary>
        /// <param name="source">Source angle in degrees (Raw)</param>
        /// <param name="target">Target angle in degrees (Raw)</param>
        /// <returns></returns>
        public static long AngleBetweenDegreesRaw(long source, long target) => FPMath.ModuloClamped(target - source + 11796480L, 23592960L) - 11796480L;

        /// <summary>
        ///     Calculates the smallest signed angle between any two angles.
        /// </summary>
        /// <param name="source">Source angle in radians</param>
        /// <param name="target">Target angle in radians</param>
        /// <returns></returns>
        public static FP AngleBetweenRadians(FP source, FP target)
        {
            long num = target.RawValue - source.RawValue;
            return new FP()
            {
                RawValue = FPMath.ModuloClamped(num + FP.Pi.RawValue, FP.PiTimes2.RawValue) - FP.Pi.RawValue
            };
        }

        /// <summary>Same as AngleBetweenDegrees using Raw optimization.</summary>
        /// <param name="source">Source angle in radians (Raw)</param>
        /// <param name="target">Target angle in radians (Raw)</param>
        /// <returns></returns>
        public static long AngleBetweenRadiansRaw(long source, long target) => FPMath.ModuloClamped(target - source + FP.Pi.RawValue, FP.PiTimes2.RawValue) - FP.Pi.RawValue;

        /// <summary>
        ///     Returns floor of the logarithm of <paramref name="value" /> in base 2. It is much
        ///     faster than calling <see cref="M:Herta.FPMath.Log2(Herta.FP)" /> and then
        ///     <see cref="M:Herta.FPMath.FloorToInt(Herta.FP)" />
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2FloorToInt(FP value) => FPMath.Log2FloorToIntRaw(value.RawValue);

        /// <summary>
        ///     Returns celining of the logarithm of <paramref name="value" /> in base 2. It is much
        ///     faster than calling <see cref="M:Herta.FPMath.Log2(Herta.FP)" /> and then
        ///     <see cref="M:Herta.FPMath.CeilToInt(Herta.FP)" />
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2CeilingToInt(FP value)
        {
            int num = 0;
            if ((value.RawValue & value.RawValue - 1L) != 0L)
                num = 1;
            return FPMath.Log2FloorToIntRaw(value.RawValue) + num;
        }

        /// <summary>
        ///     Returns logarithm of <paramref name="value" /> in base 2.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Log2(FP value)
        {
            value.RawValue = FPMath.Log2RawAdditionalPrecision(value.RawValue) >> 15;
            return value;
        }

        /// <summary>
        ///     Returns natural logarithm of <paramref name="value" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Ln(FP value)
        {
            value.RawValue = FPMath.Log2RawAdditionalPrecision(value.RawValue);
            value.RawValue >>= 6;
            value.RawValue = FPHighPrecisionDivisor.RawDiv(value.RawValue, 6196328018L);
            value.RawValue >>= 9;
            return value;
        }

        /// <summary>
        ///     Returns logarithm of <paramref name="value" /> in base 10.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Log10(FP value)
        {
            value.RawValue = FPMath.Log2RawAdditionalPrecision(value.RawValue);
            value.RawValue >>= 6;
            value.RawValue = FPHighPrecisionDivisor.RawDiv(value.RawValue, 14267572527L);
            value.RawValue >>= 9;
            return value;
        }

        /// <summary>
        ///     Returns logarithm of <paramref name="value" /> in base <paramref name="logBase" />.
        ///     It is much more performant and precise to use Log2, Log10 and Ln if <paramref name="logBase" /> is 2, 10 or e.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="value"></param>
        /// <param name="logBase"></param>
        /// <returns></returns>
        public static FP Log(FP value, FP logBase)
        {
            value.RawValue = FPMath.Log2RawAdditionalPrecision(value.RawValue);
            logBase.RawValue = FPMath.Log2RawAdditionalPrecision(logBase.RawValue);
            value.RawValue = (value.RawValue << 16) / logBase.RawValue;
            return value;
        }

        private static int Log2FloorToIntRaw(long x)
        {
            ulong num1 = x > 0L ? (ulong)x : throw new ArgumentOutOfRangeException(nameof(x), "The number has to be positive");
            return MathB.Log2(num1) - 16;
        }

        private static long Log2RawAdditionalPrecision(long x)
        {
            ReadOnlySpan<uint> log2ApproxLut = FPLut.Log2Approx;
            int intRaw = FPMath.Log2FloorToIntRaw(x);
            uint num1 = (uint)x << 48 - intRaw;
            uint index = num1 >> 26;
            uint num2 = log2ApproxLut[(int)index + 1] - log2ApproxLut[(int)index];
            uint num3 = log2ApproxLut[(int)index + 2] - log2ApproxLut[(int)index];
            int num4 = (int)(num3 >> 1) - (int)num2;
            int num5 = ((int)num2 << 1) - (int)(num3 >> 1);
            uint num6 = num1 & 67108863U;
            int num7 = (int)((((long)num4 * (long)num6 >> 26) + (long)num5) * (long)num6 >> 26);
            uint num8 = (uint)((ulong)log2ApproxLut[(int)index] + (ulong)num7) + 16384U;
            return ((long)intRaw << 31) + (long)num8;
        }

        /// <summary>
        ///     Returns e raised to the specified power. The max relative error is ~0.3% in the range of [-6, 32].
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static FP Exp(FP x)
        {
            long num1 = x.RawValue >> 16;
            long num2 = x.RawValue & (long)ushort.MaxValue;
            if (num2 >= 32768L)
            {
                num2 -= 65536L;
                ++num1;
            }

            if (num1 < -30L)
                return (FP)0;
            if (num1 >= 33L)
                return FP.MaxValue;
            long num3 = 65536L + num2 + (num2 * num2 / 2L >> 16) + (num2 * num2 * num2 / 6L >> 32) + (num2 * num2 * num2 * num2 / 24L >> 48);
            long num4 = FPLut.ExpIntegral[(int)(30L + num1)];
            FP fp;
            if (num1 < 0L)
            {
                long num5 = num3 * num4;
                fp.RawValue = num5 + 2199023255552L >> 42;
            }
            else
                fp.RawValue = num1 <= 20L ? num3 * num4 >> 16 : num3 * num4;

            return fp;
        }

        /// <summary>
        ///     Returns the sine of angle <paramref name="rad" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="rad">Angle in radians</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Sin(FP rad)
        {
            long rawValue = rad.RawValue;
            long num1 = rawValue >> 63;
            long index = (rawValue + num1 ^ num1) % 411775L;
            long num2 = (long)(int)(FPLut.SinCos[(int)index] & (long)uint.MaxValue) + num1 ^ num1;
            rad.RawValue = num2;
            return rad;
        }

        /// <summary>
        ///     Returns the high precision sine of angle <paramref name="rad" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="rad">Angle in radians</param>
        /// <returns></returns>
        public static FP SinHighPrecision(FP rad)
        {
            long rawValue = rad.RawValue;
            long num1 = rawValue >> 63;
            long index = ((rawValue + num1 ^ num1) << 16) % 26986075409L >> 16;
            long num2 = (long)(int)(FPLut.SinCos[(int)index] & (long)uint.MaxValue) + num1 ^ num1;
            rad.RawValue = num2;
            return rad;
        }

        /// <summary>
        ///     Returns the cosine of angle <paramref name="rad" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="rad">Angle in radians</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Cos(FP rad)
        {
            long rawValue = rad.RawValue;
            long num1 = rawValue >> 63;
            long index = (rawValue + num1 ^ num1) % 411775L;
            long num2 = FPLut.SinCos[(int)index] >> 32;
            rad.RawValue = num2;
            return rad;
        }

        /// <summary>
        ///     Returns the high precision cosine of angle <paramref name="rad" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="rad">Angle in radians</param>
        /// <returns></returns>
        public static FP CosHighPrecision(FP rad)
        {
            long rawValue = rad.RawValue;
            long num1 = rawValue >> 63;
            long index = ((rawValue + num1 ^ num1) << 16) % 26986075409L >> 16;
            long num2 = FPLut.SinCos[(int)index] >> 32;
            rad.RawValue = num2;
            return rad;
        }

        /// <summary>
        ///     Calculates sine and cosine of angle <paramref name="rad" />. It is faster than
        ///     calling <see cref="M:Herta.FPMath.Sin(Herta.FP)" />  and
        ///     <see cref="M:Herta.FPMath.Cos(Herta.FP)" /> separately.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="rad">Angle in radians</param>
        /// <param name="sin"></param>
        /// <param name="cos"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCos(FP rad, out FP sin, out FP cos)
        {
            long rawValue = rad.RawValue;
            long num1 = rawValue >> 63;
            long index = (rawValue + num1 ^ num1) % 411775L;
            long num2 = FPLut.SinCos[(int)index];
            cos.RawValue = num2 >> 32;
            sin.RawValue = (long)(int)(num2 & (long)uint.MaxValue) + num1 ^ num1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCosRaw(FP rad, out long sinRaw, out long cosRaw)
        {
            long rawValue = rad.RawValue;
            long num1 = rawValue >> 63;
            long index = (rawValue + num1 ^ num1) % 411775L;
            long num2 = FPLut.SinCos[(int)index];
            cosRaw = num2 >> 32;
            sinRaw = (long)(int)(num2 & (long)uint.MaxValue) + num1 ^ num1;
        }

        /// <summary>
        ///     Calculates high precision sine and cosine of angle <paramref name="rad" />. It is faster than
        ///     calling <see cref="M:Herta.FPMath.SinHighPrecision(Herta.FP)" />  and
        ///     <see cref="M:Herta.FPMath.CosHighPrecision(Herta.FP)" /> separately.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="rad">Angle in radians</param>
        /// <param name="sin"></param>
        /// <param name="cos"></param>
        public static void SinCosHighPrecision(FP rad, out FP sin, out FP cos)
        {
            long rawValue = rad.RawValue;
            long num1 = rawValue >> 63;
            long index = ((rawValue + num1 ^ num1) << 16) % 26986075409L >> 16;
            long num2 = FPLut.SinCos[(int)index];
            cos.RawValue = num2 >> 32;
            sin.RawValue = (long)(int)(num2 & (long)uint.MaxValue) + num1 ^ num1;
        }

        /// <summary>
        ///     Returns the tangent of angle <paramref name="rad" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="rad">Angle in radians</param>
        /// <returns></returns>
        public static FP Tan(FP rad)
        {
            if (rad.RawValue < -205887L)
                rad.RawValue %= -205887L;
            if (rad.RawValue > 205887L)
                rad.RawValue %= 205887L;
            rad.RawValue = FPLut.Tan[(int)(rad.RawValue + 205887L)];
            return rad;
        }

        /// <summary>
        ///     Returns the arc-sine of <paramref name="value" /> - the angle in radians whose sine is <paramref name="value" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Asin(FP value)
        {
            if (value.RawValue < -65536L || value.RawValue > 65536L)
                return FP.MinValue;
            value.RawValue = FPLut.Asin[(int)(value.RawValue + 65536L)];
            return value;
        }

        /// <summary>
        ///     Returns the arc-cosine of <paramref name="value" /> - the angle in radians whose cosine is
        ///     <paramref name="value" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Acos(FP value)
        {
            if (value.RawValue < -65536L || value.RawValue > 65536L)
                return FP.MinValue;
            value.RawValue = FPLut.Acos[(int)(value.RawValue + 65536L)];
            return value;
        }

        /// <summary>
        ///     Returns the arc-tangent of <paramref name="value" /> - the angle in radians whose tangent is
        ///     <paramref name="value" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FP Atan(FP value)
        {
            long num = value.RawValue >> 63;
            value.RawValue = value.RawValue + num ^ num;
            if (value.RawValue <= 393216L)
            {
                value.RawValue = FPLut.Atan[(int)value.RawValue] + num ^ num;
                return value;
            }

            if (value.RawValue <= 16384000L)
            {
                value.RawValue = value.RawValue - 393216L >> 12;
                value.RawValue = FPLut.Atan[(int)(393216L + value.RawValue)] + num ^ num;
                return value;
            }

            if (value.RawValue <= 655360000L)
            {
                value.RawValue = value.RawValue - 16384000L >> 20;
                value.RawValue = FPLut.Atan[(int)(397120L + value.RawValue)] + num ^ num;
                return value;
            }

            value.RawValue = FPLut.Atan[FPLut.Atan.Length - 1] + num ^ num;
            return value;
        }

        /// <summary>
        ///     Returns the angle in radians whose <see cref="M:Herta.FPMath.Tan(Herta.FP)" /> is
        ///     <paramref name="y" />/<paramref name="x" />. This function returns correct angle even if x is zero.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static FP Atan2(FP y, FP x)
        {
            if (x.RawValue > 0L)
                return FPMath.Atan(y / x);
            if (x.RawValue < 0L)
            {
                y.RawValue = y.RawValue < 0L ? FPMath.Atan(y / x).RawValue - 205887L : FPMath.Atan(y / x).RawValue + 205887L;
                return y;
            }

            if (y.RawValue > 0L)
                return FP.PiOver2;
            return y.RawValue == 0L ? FP._0 : -FP.PiOver2;
        }

        internal struct ExponentMantisaPair
        {
            public int Exponent;
            public int Mantissa;
        }
    }
}