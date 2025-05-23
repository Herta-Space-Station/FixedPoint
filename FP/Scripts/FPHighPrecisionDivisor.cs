using System.Runtime.CompilerServices;

#pragma warning disable CA2208
#pragma warning disable CS8632

// ReSharper disable ALL

namespace Thief
{
    /// <summary>
    ///     Represents a high precision divisor for use with the Fixed Point math system.
    /// </summary>
    public struct FPHighPrecisionDivisor
    {
        /// <summary>The extra precision to shift.</summary>
        public const int ExtraPrecision = 16;

        /// <summary>The total precision.</summary>
        public const int TotalPrecision = 32;

        internal long RawValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FPHighPrecisionDivisor Cast(in long value) => Unsafe.As<long, FPHighPrecisionDivisor>(ref Unsafe.AsRef(in value));

        /// <summary>
        ///     &gt;Pi number.
        ///     <para>Closest double: 3.14159265346825</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor Pi = Cast(13493037704L);

        /// <summary>
        ///     1/Pi.
        ///     <para>Closest double: 0.318309886148199</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor PiInv = Cast(1367130551L);

        /// <summary>
        ///     2 * Pi.
        ///     <para>Closest double: 6.28318530716933</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor PiTimes2 = Cast(26986075409L);

        /// <summary>
        ///     Pi / 2.
        ///     <para>Closest double: 1.57079632673413</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor PiOver2 = Cast(6746518852L);

        /// <summary>
        ///     2 / Pi.
        ///     <para>Closest double: 0.636619772296399</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor PiOver2Inv = Cast(2734261102L);

        /// <summary>
        ///     Pi / 4.
        ///     <para>Closest double: 0.785398163367063</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor PiOver4 = Cast(3373259426L);

        /// <summary>
        ///     3 * Pi / 4.
        ///     <para>Closest double: 2.35619449010119</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor Pi3Over4 = Cast(10119778278L);

        /// <summary>
        ///     Degrees-to-radians conversion constant.
        ///     <para>Closest double: 0.0174532923847437</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor Deg2Rad = Cast(74961320L);

        /// <summary>
        ///     Radians-to-degrees conversion constant.
        ///     <para>Closest double: 57.2957795129623</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor Rad2Deg = Cast(246083499207L);

        /// <summary>
        ///     FP constant representing 180 degrees in radian.
        ///     <para>Closest double: 3.14159265346825</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor Rad_180 = Cast(13493037704L);

        /// <summary>
        ///     FP constant representing 90 degrees in radian.
        ///     <para>Closest double: 1.57079632673413</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor Rad_90 = Cast(6746518852L);

        /// <summary>
        ///     FP constant representing 45 degrees in radian.
        ///     <para>Closest double: 0.785398163367063</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor Rad_45 = Cast(3373259426L);

        /// <summary>
        ///     FP constant representing 22.5 degrees in radian.
        ///     <para>Closest double: 0.392699081683531</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor Rad_22_50 = Cast(1686629713L);

        /// <summary>
        ///     FP constant representing the number 0.01.
        ///     <para>Closest double: 0.00999999977648258</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _0_01 = Cast(42949672L);

        /// <summary>
        ///     FP constant representing the number 0.02.
        ///     <para>Closest double: 0.0199999997857958</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _0_02 = Cast(85899345L);

        /// <summary>
        ///     FP constant representing the number 0.03.
        ///     <para>Closest double: 0.029999999795109</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _0_03 = Cast(128849018L);

        /// <summary>
        ///     FP constant representing the number 0.04.
        ///     <para>Closest double: 0.0399999998044223</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _0_04 = Cast(171798691L);

        /// <summary>
        ///     FP constant representing the number 0.05.
        ///     <para>Closest double: 0.0499999998137355</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _0_05 = Cast(214748364L);

        /// <summary>
        ///     FP constant representing the number 0.10.
        ///     <para>Closest double: 0.0999999998603016</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _0_10 = Cast(429496729L);

        /// <summary>
        ///     FP constant representing the number 0.20.
        ///     <para>Closest double: 0.199999999953434</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _0_20 = Cast(858993459L);

        /// <summary>
        ///     FP constant representing the number 0.33.
        ///     <para>Closest double: 0.333333333255723</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _0_33 = Cast(1431655765L);

        /// <summary>
        ///     FP constant representing the number 0.99.
        ///     <para>Closest double: 0.989999999990687</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _0_99 = Cast(4252017623L);

        /// <summary>
        ///     FP constant representing the number 1.01.
        ///     <para>Closest double: 1.00999999977648</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _1_01 = Cast(4337916968L);

        /// <summary>
        ///     FP constant representing the number 1.02.
        ///     <para>Closest double: 1.0199999997858</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _1_02 = Cast(4380866641L);

        /// <summary>
        ///     FP constant representing the number 1.03.
        ///     <para>Closest double: 1.02999999979511</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _1_03 = Cast(4423816314L);

        /// <summary>
        ///     FP constant representing the number 1.04.
        ///     <para>Closest double: 1.03999999980442</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _1_04 = Cast(4466765987L);

        /// <summary>
        ///     FP constant representing the number 1.05.
        ///     <para>Closest double: 1.04999999981374</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _1_05 = Cast(4509715660L);

        /// <summary>
        ///     FP constant representing the number 1.10.
        ///     <para>Closest double: 1.0999999998603</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _1_10 = Cast(4724464025L);

        /// <summary>
        ///     FP constant representing the number 1.20.
        ///     <para>Closest double: 1.19999999995343</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _1_20 = Cast(5153960755L);

        /// <summary>
        ///     FP constant representing the number 1.33.
        ///     <para>Closest double: 1.33333333325572</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _1_33 = Cast(5726623061L);

        /// <summary>
        ///     FP constant representing the number 1.99.
        ///     <para>Closest double: 1.98999999999069</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor _1_99 = Cast(8546984919L);

        /// <summary>
        ///     FP constant representing the epsilon value EN1.
        ///     <para>Closest double: 0.0999999998603016</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor EN1 = Cast(429496729L);

        /// <summary>
        ///     FP constant representing the epsilon value EN2.
        ///     <para>Closest double: 0.00999999977648258</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor EN2 = Cast(42949672L);

        /// <summary>
        ///     FP constant representing the epsilon value EN3.
        ///     <para>Closest double: 0.000999999931082129</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor EN3 = Cast(4294967L);

        /// <summary>
        ///     FP constant representing the epsilon value EN4.
        ///     <para>Closest double: 9.99998301267624E-05</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor EN4 = Cast(429496L);

        /// <summary>
        ///     FP constant representing the epsilon value EN5.
        ///     <para>Closest double: 9.99984331429005E-06</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor EN5 = Cast(42949L);

        /// <summary>
        ///     FP constant representing the Euler Number constant.
        ///     <para>Closest double: 2.71828182833269</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor E = Cast(11674931554L);

        /// <summary>
        ///     FP constant representing Log(E).
        ///     <para>Closest double: 1.44269504072145</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor Log2_E = Cast(6196328018L);

        /// <summary>
        ///     FP constant representing Log(10).
        ///     <para>Closest double: 3.32192809483968</para>
        /// </summary>
        public static readonly FPHighPrecisionDivisor Log2_10 = Cast(14267572527L);

        /// <summary>
        ///     Returns the value of the divisor as a <see cref="T:Thief.FP" />.
        /// </summary>
        public FP AsFP => FP.FromRaw(this.RawValue >> 16);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long RawMod(long standardPrecisionRaw, long highPrecisionRaw)
        {
            long num1 = standardPrecisionRaw;
            int num2 = (int)((ulong)num1 >> 63);
            return (num1 << 16) % highPrecisionRaw + (long)((num2 << 16) - num2) >> 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long RawModPositive(long standardPrecisionRaw, long highPrecisionRaw) => (standardPrecisionRaw << 16) % highPrecisionRaw >> 16;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long RawDiv(long standardPrecisionRaw, long highPrecisionRaw)
        {
            long num1 = standardPrecisionRaw;
            long num2 = (long)((ulong)num1 >> 63);
            return ((num1 << 32) + ((num2 << 32) - num2)) / highPrecisionRaw;
        }

        /// <summary>
        ///     Holds <see cref="T:Thief.FPHighPrecisionDivisor" /> constants in raw (long) form.
        /// </summary>
        public static class Raw
        {
            /// <summary>
            ///     &gt;Pi number.
            ///     <para>Closest double: 3.14159265346825</para>
            /// </summary>
            public const long Pi = 13493037704;

            /// <summary>
            ///     1/Pi.
            ///     <para>Closest double: 0.318309886148199</para>
            /// </summary>
            public const long PiInv = 1367130551;

            /// <summary>
            ///     2 * Pi.
            ///     <para>Closest double: 6.28318530716933</para>
            /// </summary>
            public const long PiTimes2 = 26986075409;

            /// <summary>
            ///     Pi / 2.
            ///     <para>Closest double: 1.57079632673413</para>
            /// </summary>
            public const long PiOver2 = 6746518852;

            /// <summary>
            ///     2 / Pi.
            ///     <para>Closest double: 0.636619772296399</para>
            /// </summary>
            public const long PiOver2Inv = 2734261102;

            /// <summary>
            ///     Pi / 4.
            ///     <para>Closest double: 0.785398163367063</para>
            /// </summary>
            public const long PiOver4 = 3373259426;

            /// <summary>
            ///     3 * Pi / 4.
            ///     <para>Closest double: 2.35619449010119</para>
            /// </summary>
            public const long Pi3Over4 = 10119778278;

            /// <summary>
            ///     Degrees-to-radians conversion constant.
            ///     <para>Closest double: 0.0174532923847437</para>
            /// </summary>
            public const long Deg2Rad = 74961320;

            /// <summary>
            ///     Radians-to-degrees conversion constant.
            ///     <para>Closest double: 57.2957795129623</para>
            /// </summary>
            public const long Rad2Deg = 246083499207;

            /// <summary>
            ///     FP constant representing 180 degrees in radian.
            ///     <para>Closest double: 3.14159265346825</para>
            /// </summary>
            public const long Rad_180 = 13493037704;

            /// <summary>
            ///     FP constant representing 90 degrees in radian.
            ///     <para>Closest double: 1.57079632673413</para>
            /// </summary>
            public const long Rad_90 = 6746518852;

            /// <summary>
            ///     FP constant representing 45 degrees in radian.
            ///     <para>Closest double: 0.785398163367063</para>
            /// </summary>
            public const long Rad_45 = 3373259426;

            /// <summary>
            ///     FP constant representing 22.5 degrees in radian.
            ///     <para>Closest double: 0.392699081683531</para>
            /// </summary>
            public const long Rad_22_50 = 1686629713;

            /// <summary>
            ///     FP constant representing the number 0.01.
            ///     <para>Closest double: 0.00999999977648258</para>
            /// </summary>
            public const long _0_01 = 42949672;

            /// <summary>
            ///     FP constant representing the number 0.02.
            ///     <para>Closest double: 0.0199999997857958</para>
            /// </summary>
            public const long _0_02 = 85899345;

            /// <summary>
            ///     FP constant representing the number 0.03.
            ///     <para>Closest double: 0.029999999795109</para>
            /// </summary>
            public const long _0_03 = 128849018;

            /// <summary>
            ///     FP constant representing the number 0.04.
            ///     <para>Closest double: 0.0399999998044223</para>
            /// </summary>
            public const long _0_04 = 171798691;

            /// <summary>
            ///     FP constant representing the number 0.05.
            ///     <para>Closest double: 0.0499999998137355</para>
            /// </summary>
            public const long _0_05 = 214748364;

            /// <summary>
            ///     FP constant representing the number 0.10.
            ///     <para>Closest double: 0.0999999998603016</para>
            /// </summary>
            public const long _0_10 = 429496729;

            /// <summary>
            ///     FP constant representing the number 0.20.
            ///     <para>Closest double: 0.199999999953434</para>
            /// </summary>
            public const long _0_20 = 858993459;

            /// <summary>
            ///     FP constant representing the number 0.33.
            ///     <para>Closest double: 0.333333333255723</para>
            /// </summary>
            public const long _0_33 = 1431655765;

            /// <summary>
            ///     FP constant representing the number 0.99.
            ///     <para>Closest double: 0.989999999990687</para>
            /// </summary>
            public const long _0_99 = 4252017623;

            /// <summary>
            ///     FP constant representing the number 1.01.
            ///     <para>Closest double: 1.00999999977648</para>
            /// </summary>
            public const long _1_01 = 4337916968;

            /// <summary>
            ///     FP constant representing the number 1.02.
            ///     <para>Closest double: 1.0199999997858</para>
            /// </summary>
            public const long _1_02 = 4380866641;

            /// <summary>
            ///     FP constant representing the number 1.03.
            ///     <para>Closest double: 1.02999999979511</para>
            /// </summary>
            public const long _1_03 = 4423816314;

            /// <summary>
            ///     FP constant representing the number 1.04.
            ///     <para>Closest double: 1.03999999980442</para>
            /// </summary>
            public const long _1_04 = 4466765987;

            /// <summary>
            ///     FP constant representing the number 1.05.
            ///     <para>Closest double: 1.04999999981374</para>
            /// </summary>
            public const long _1_05 = 4509715660;

            /// <summary>
            ///     FP constant representing the number 1.10.
            ///     <para>Closest double: 1.0999999998603</para>
            /// </summary>
            public const long _1_10 = 4724464025;

            /// <summary>
            ///     FP constant representing the number 1.20.
            ///     <para>Closest double: 1.19999999995343</para>
            /// </summary>
            public const long _1_20 = 5153960755;

            /// <summary>
            ///     FP constant representing the number 1.33.
            ///     <para>Closest double: 1.33333333325572</para>
            /// </summary>
            public const long _1_33 = 5726623061;

            /// <summary>
            ///     FP constant representing the number 1.99.
            ///     <para>Closest double: 1.98999999999069</para>
            /// </summary>
            public const long _1_99 = 8546984919;

            /// <summary>
            ///     FP constant representing the epsilon value EN1.
            ///     <para>Closest double: 0.0999999998603016</para>
            /// </summary>
            public const long EN1 = 429496729;

            /// <summary>
            ///     FP constant representing the epsilon value EN2.
            ///     <para>Closest double: 0.00999999977648258</para>
            /// </summary>
            public const long EN2 = 42949672;

            /// <summary>
            ///     FP constant representing the epsilon value EN3.
            ///     <para>Closest double: 0.000999999931082129</para>
            /// </summary>
            public const long EN3 = 4294967;

            /// <summary>
            ///     FP constant representing the epsilon value EN4.
            ///     <para>Closest double: 9.99998301267624E-05</para>
            /// </summary>
            public const long EN4 = 429496;

            /// <summary>
            ///     FP constant representing the epsilon value EN5.
            ///     <para>Closest double: 9.99984331429005E-06</para>
            /// </summary>
            public const long EN5 = 42949;

            /// <summary>
            ///     FP constant representing the Euler Number constant.
            ///     <para>Closest double: 2.71828182833269</para>
            /// </summary>
            public const long E = 11674931554;

            /// <summary>
            ///     FP constant representing Log(E).
            ///     <para>Closest double: 1.44269504072145</para>
            /// </summary>
            public const long Log2_E = 6196328018;

            /// <summary>
            ///     FP constant representing Log(10).
            ///     <para>Closest double: 3.32192809483968</para>
            /// </summary>
            public const long Log2_10 = 14267572527;
        }
    }
}