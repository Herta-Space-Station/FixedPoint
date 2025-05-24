// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     The lookup table for the inverse cosine function.
    ///     The table stores precalculated values of the inverse cosine function for specific angles.
    ///     The values are in fixed-point format with a precision of 16 bits.
    /// </summary>
    public static partial class FPLut
    {
        /// <summary>
        ///     The number of bits used for the fractional part of the fixed-point numbers.
        /// </summary>
        public const int PRECISION = 16;

        /// <summary>The value of PI in fixed-point format.</summary>
        public const long PI = 205887;

        /// <summary>The value of PI times 2 in fixed-point format.</summary>
        public const long PITIMES2 = 411775;

        /// <summary>The value of PI divided by 2 in fixed-point format.</summary>
        public const long PIOVER2 = 102944;

        /// <summary>The value of one in fixed-point format.</summary>
        public const long ONE = 65536;

        public const int SQRT_RESOLUTION_SPACE = 3;
        public const int SQRT_LUT_SIZE_BASE_2 = 16;
        public const int SQRT_VALUE_STEP = 3;
        public const int SQRT_LUT_SIZE_BASE_10 = 65536;
        public const int SqrtAdditionalPrecisionBits = 6;
        public const int Log2LutSizeExponent = 6;
        public const int Log2AdditionalPrecisionBits = 15;

        /// <summary>
        ///     How much Log2 additional precision (AP) result needs to be shifted to allow for a safe FPHighPrecision division.
        ///     There must be a way to calculate this, but I have a brain fog atm.
        ///     6 is the safe choice here - max log2 is 48.
        /// </summary>
        public const int Log2APShiftForHPDivision = 6;

        public const int ExpNegativeLutPrecision = 42;
        public const int ExpNegativeLutCount = 30;
        public const int ExpNonNegativeLutCount = 33;
        public const int ExpOverflowingThreshold = 20;
    }
}