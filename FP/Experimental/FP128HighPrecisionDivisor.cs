#if NET7_0_OR_GREATER
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Herta
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct FP128HighPrecisionDivisor : IEquatable<FP128HighPrecisionDivisor>, IComparable<FP128HighPrecisionDivisor>
    {
        [FieldOffset(0)] public readonly Int128 RawValue;
        public double AsDouble => (double)RawValue / SCALE_DOUBLE;
        public const int PRECISION = 53;
        public static readonly Int128 SCALE_INT128 = (Int128)1 << PRECISION;
        public static readonly double SCALE_DOUBLE = (double)SCALE_INT128;
        public static readonly FP128HighPrecisionDivisor MaxValue = new(Int128.MaxValue >> PRECISION);
        public static readonly FP128HighPrecisionDivisor MinValue = new(Int128.MinValue >> PRECISION);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FP128HighPrecisionDivisor(in Int128 raw) => this = Unsafe.As<Int128, FP128HighPrecisionDivisor>(ref Unsafe.AsRef(in raw));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP128HighPrecisionDivisor(double d) => new((Int128)(d * SCALE_DOUBLE));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP128HighPrecisionDivisor operator +(FP128HighPrecisionDivisor a, FP128HighPrecisionDivisor b) => new(a.RawValue + b.RawValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP128HighPrecisionDivisor operator -(FP128HighPrecisionDivisor a, FP128HighPrecisionDivisor b) => new(a.RawValue - b.RawValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP128HighPrecisionDivisor operator *(FP128HighPrecisionDivisor a, FP128HighPrecisionDivisor b) => new((a.RawValue * b.RawValue) >> PRECISION);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP128HighPrecisionDivisor operator /(FP128HighPrecisionDivisor a, FP128HighPrecisionDivisor b) => new((a.RawValue << PRECISION) / b.RawValue);

        public bool Equals(FP128HighPrecisionDivisor other) => RawValue == other.RawValue;
        public override bool Equals(object? o) => o is FP128HighPrecisionDivisor f && Equals(f);
        public override int GetHashCode() => RawValue.GetHashCode();
        public int CompareTo(FP128HighPrecisionDivisor other) => RawValue.CompareTo(other.RawValue);
        public override string ToString() => AsDouble.ToString(CultureInfo.InvariantCulture);
        public string ToString(string format) => AsDouble.ToString(format, CultureInfo.InvariantCulture);
        public static bool operator ==(FP128HighPrecisionDivisor left, FP128HighPrecisionDivisor right) => left.Equals(right);
        public static bool operator !=(FP128HighPrecisionDivisor left, FP128HighPrecisionDivisor right) => !(left == right);
    }
}
#endif