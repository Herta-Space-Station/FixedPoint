#if NET7_0_OR_GREATER
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Herta
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct FP128 : IEquatable<FP128>, IComparable<FP128>
    {
        [FieldOffset(0)] public readonly Int128 RawValue;
        public double AsDouble => (double)RawValue / SCALE_DOUBLE;
        public const int PRECISION = 32;
        public static readonly Int128 SCALE_INT128 = (Int128)1 << PRECISION;
        public static readonly double SCALE_DOUBLE = (double)SCALE_INT128;
        public static readonly FP128 MaxValue = new(Int128.MaxValue >> PRECISION);
        public static readonly FP128 MinValue = new(Int128.MinValue >> PRECISION);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FP128(in Int128 raw) => this = Unsafe.As<Int128, FP128>(ref Unsafe.AsRef(in raw));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FP128(double d) => new((Int128)(d * SCALE_DOUBLE));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP128 operator +(FP128 a, FP128 b) => new(a.RawValue + b.RawValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP128 operator -(FP128 a, FP128 b) => new(a.RawValue - b.RawValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP128 operator *(FP128 a, FP128 b) => new((a.RawValue * b.RawValue) >> PRECISION);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP128 operator /(FP128 a, FP128 b) => new((a.RawValue << PRECISION) / b.RawValue);

        public bool Equals(FP128 other) => RawValue == other.RawValue;
        public override bool Equals(object? o) => o is FP128 f && Equals(f);
        public override int GetHashCode() => RawValue.GetHashCode();
        public int CompareTo(FP128 other) => RawValue.CompareTo(other.RawValue);
        public override string ToString() => AsDouble.ToString(CultureInfo.InvariantCulture);
        public string ToString(string format) => AsDouble.ToString(format, CultureInfo.InvariantCulture);
        public static bool operator ==(FP128 left, FP128 right) => left.Equals(right);
        public static bool operator !=(FP128 left, FP128 right) => !(left == right);
    }
}
#endif