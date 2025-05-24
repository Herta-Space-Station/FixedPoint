using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     Represents a three-dimensional vector with integer components.
    /// </summary>
    /// \ingroup MathApi
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct IntVector3 : IEquatable<IntVector3>
    {
        /// <summary>The size of the struct in memory.</summary>
        public const int SIZE = 12;

        /// <summary>The X component of the vector.</summary>
        [FieldOffset(0)] public int X;

        /// <summary>The Y component of the vector.</summary>
        [FieldOffset(4)] public int Y;

        /// <summary>The Z component of the vector.</summary>
        [FieldOffset(8)] public int Z;

        /// <summary>
        ///     Constructs a new IntVector3 with the given components.
        /// </summary>
        public IntVector3(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        ///     A vector with the components (int.MaxValue, int.MaxValue, int.MaxValue).
        /// </summary>
        public static readonly IntVector3 MaxValue = new IntVector3(int.MaxValue, int.MaxValue, int.MaxValue);

        /// <summary>
        ///     A vector with the components (int.MinValue, int.MinValue, int.MinValue).
        /// </summary>
        public static readonly IntVector3 MinValue = new IntVector3(int.MinValue, int.MinValue, int.MinValue);

        /// <summary>
        ///     Represents an upward vector with x = 0, y = 1, and z = 0.
        /// </summary>
        public static readonly IntVector3 Up = new IntVector3(0, 1, 0);

        /// <summary>
        ///     Represents the downward direction with coordinates (0, -1, 0).
        /// </summary>
        public static readonly IntVector3 Down = new IntVector3(0, -1, 0);

        /// <summary>
        ///     Represents a 3-dimensional vector with integer components.
        /// </summary>
        /// <remarks>
        ///     This struct is part of the <see cref="N:Herta" /> namespace.
        /// </remarks>
        public static readonly IntVector3 Left = new IntVector3(-1, 0, 0);

        /// <summary>The right direction vector (1, 0, 0).</summary>
        /// <value>The right direction vector.</value>
        public static readonly IntVector3 Right = new IntVector3(1, 0, 0);

        /// <summary>The vector representing a coordinate of (1, 1, 1).</summary>
        public static readonly IntVector3 One = new IntVector3(1, 1, 1);

        /// <summary>
        ///     Represents a zero vector with all components set to 0.
        /// </summary>
        public static readonly IntVector3 Zero = new IntVector3(0, 0, 0);

        /// <summary>Returns a hash code for the vector.</summary>
        public override int GetHashCode() => HashCode.Combine<int, int, int>(this.X, this.Y, this.Z);

        /// <summary>Returns a string representation of the IntVector3.</summary>
        /// <returns>A string representation of the IntVector3.</returns>
        public override string ToString() => string.Format((IFormatProvider)CultureInfo.InvariantCulture, "({0}, {1}, {2})", (object)this.X, (object)this.Y, (object)this.Z);

        /// <summary>Calculates the magnitude (length) of the IntVector3.</summary>
        /// <value>The magnitude of the IntVector3.</value>
        public FP Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => FPMath.Sqrt((FP)(this.X * this.X + this.Y * this.Y + this.Z * this.Z));
        }

        /// <summary>Gets the square of the magnitude of the vector.</summary>
        /// <value>The square of the magnitude of the vector.</value>
        public int SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        /// <summary>
        ///     Calculates the distance between two IntVector3 points.
        /// </summary>
        /// <param name="a">The first IntVector3 point.</param>
        /// <param name="b">The second IntVector3 point.</param>
        /// <returns>The distance between the two IntVector3 points.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Distance(IntVector3 a, IntVector3 b) => (a - b).Magnitude;

        /// <summary>
        ///     Clamps a IntVector3 value between a minimum and maximum IntVector3 value.
        /// </summary>
        /// <param name="value">The IntVector3 value to clamp.</param>
        /// <param name="min">The minimum IntVector3 value.</param>
        /// <param name="max">The maximum IntVector3 value.</param>
        /// <returns>The clamped IntVector3 value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 Clamp(IntVector3 value, IntVector3 min, IntVector3 max) => new IntVector3(FPMath.Clamp(value.X, min.X, max.X), FPMath.Clamp(value.Y, min.Y, max.Y), FPMath.Clamp(value.Z, min.Z, max.Z));

        /// <summary>
        ///     Returns a new IntVector3 with the smallest components of the input vectors.
        /// </summary>
        /// <param name="a">The first IntVector3.</param>
        /// <param name="b">The second IntVector3.</param>
        /// <returns>A new IntVector3 with the smallest components of the input vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 Min(IntVector3 a, IntVector3 b) => new IntVector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

        /// <summary>
        ///     Returns a new IntVector3 with the largest components of the input vectors.
        /// </summary>
        /// <param name="a">The first IntVector3.</param>
        /// <param name="b">The second IntVector3.</param>
        /// <returns>A new IntVector3 with the largest components of the input vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 Max(IntVector3 a, IntVector3 b) => new IntVector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        /// <summary>
        ///     Rounds the components of the given FPVector3 to the nearest integer values.
        /// </summary>
        /// <param name="v">The vector to round.</param>
        /// <returns>A new IntVector3 with the rounded integer values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 RoundToInt(FPVector3 v) => new IntVector3(FPMath.RoundToInt(v.X), FPMath.RoundToInt(v.Y), FPMath.RoundToInt(v.Z));

        /// <summary>
        ///     Returns the largest integer less than or equal to the specified floating-point number.
        /// </summary>
        /// <param name="v">The number to round down.</param>
        /// <returns>The largest integer less than or equal to the specified number.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 FloorToInt(FPVector3 v) => new IntVector3(FPMath.FloorToInt(v.X), FPMath.FloorToInt(v.Y), FPMath.FloorToInt(v.Z));

        /// <summary>
        ///     Returns a new IntVector3 with the ceiling of the components of the input vector.
        /// </summary>
        /// <param name="v">The input vector.</param>
        /// <returns>A new IntVector3 with the ceiling of the components.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 CeilToInt(FPVector3 v) => new IntVector3(FPMath.CeilToInt(v.X), FPMath.CeilToInt(v.Y), FPMath.CeilToInt(v.Z));

        /// <summary>Converts a IntVector3 object to FPVector3 object.</summary>
        /// <param name="v">The IntVector3 object to convert.</param>
        /// <returns>A new FPVector3 object created using the values of the IntVector3 object.</returns>
        public static implicit operator FPVector3(IntVector3 v) => new FPVector3(v.X, v.Y, v.Z);

        /// <summary>Converts a FPVector3 object to a IntVector3 object.</summary>
        public static explicit operator IntVector3(FPVector3 v) => new IntVector3(v.X.AsInt, v.Y.AsInt, v.Z.AsInt);

        /// <summary>
        ///     Returns <see langword="true" /> if two vectors are exactly equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(IntVector3 a, IntVector3 b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

        /// <summary>
        ///     Returns <see langword="true" /> if two vectors are not exactly equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(IntVector3 a, IntVector3 b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z;

        /// <summary>
        ///     Negates each component of <paramref name="v" /> vector.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator -(IntVector3 v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            v.Z = -v.Z;
            return v;
        }

        /// <summary>
        ///     Multiplies each component of <paramref name="v" /> times <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator *(IntVector3 v, int s)
        {
            v.X *= s;
            v.Y *= s;
            v.Z *= s;
            return v;
        }

        /// <summary>
        ///     Multiplies each component of <paramref name="v" /> times <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator *(int s, IntVector3 v)
        {
            v.X *= s;
            v.Y *= s;
            v.Z *= s;
            return v;
        }

        /// <summary>
        ///     Divides each component of <paramref name="v" /> by <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator /(IntVector3 v, int s)
        {
            v.X /= s;
            v.Y /= s;
            v.Z /= s;
            return v;
        }

        /// <summary>
        ///     Subtracts <paramref name="b" /> from <paramref name="a" />
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator -(IntVector3 a, IntVector3 b) => new IntVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        /// <summary>Adds two vectors.</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator +(IntVector3 a, IntVector3 b) => new IntVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        /// <summary>
        ///     Returns true if the vector is exactly equal to another vector.
        /// </summary>
        public bool Equals(IntVector3 other) => this.X == other.X && this.Y == other.Y && this.Z == other.Z;

        /// <inheritdoc cref="M:Herta.IntVector3.Equals(Herta.IntVector3)" />
        public override bool Equals(object? obj) => obj is IntVector3 other && this.Equals(other);

        public IntVector3 XXX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 xxx;
                xxx.X = this.X;
                xxx.Y = this.X;
                xxx.Z = this.X;
                return xxx;
            }
        }

        public IntVector3 XXY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 xxy;
                xxy.X = this.X;
                xxy.Y = this.X;
                xxy.Z = this.Y;
                return xxy;
            }
        }

        public IntVector3 XXZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 xxz;
                xxz.X = this.X;
                xxz.Y = this.X;
                xxz.Z = this.Z;
                return xxz;
            }
        }

        public IntVector3 XYX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 xyx;
                xyx.X = this.X;
                xyx.Y = this.Y;
                xyx.Z = this.X;
                return xyx;
            }
        }

        public IntVector3 XYY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 xyy;
                xyy.X = this.X;
                xyy.Y = this.Y;
                xyy.Z = this.Y;
                return xyy;
            }
        }

        public IntVector3 XYZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 xyz;
                xyz.X = this.X;
                xyz.Y = this.Y;
                xyz.Z = this.Z;
                return xyz;
            }
        }

        public IntVector3 XZX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 xzx;
                xzx.X = this.X;
                xzx.Y = this.Z;
                xzx.Z = this.X;
                return xzx;
            }
        }

        public IntVector3 XZY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 xzy;
                xzy.X = this.X;
                xzy.Y = this.Z;
                xzy.Z = this.Y;
                return xzy;
            }
        }

        public IntVector3 XZZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 xzz;
                xzz.X = this.X;
                xzz.Y = this.Z;
                xzz.Z = this.Z;
                return xzz;
            }
        }

        public IntVector2 XX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector2 xx;
                xx.X = this.X;
                xx.Y = this.X;
                return xx;
            }
        }

        public IntVector2 XY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector2 xy;
                xy.X = this.X;
                xy.Y = this.Y;
                return xy;
            }
        }

        public IntVector2 XZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector2 xz;
                xz.X = this.X;
                xz.Y = this.Z;
                return xz;
            }
        }

        public IntVector3 YYY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 yyy;
                yyy.X = this.Y;
                yyy.Y = this.Y;
                yyy.Z = this.Y;
                return yyy;
            }
        }

        public IntVector3 YYZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 yyz;
                yyz.X = this.Y;
                yyz.Y = this.Y;
                yyz.Z = this.Z;
                return yyz;
            }
        }

        public IntVector3 YYX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 yyx;
                yyx.X = this.Y;
                yyx.Y = this.Y;
                yyx.Z = this.X;
                return yyx;
            }
        }

        public IntVector3 YZY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 yzy;
                yzy.X = this.Y;
                yzy.Y = this.Z;
                yzy.Z = this.Y;
                return yzy;
            }
        }

        public IntVector3 YZZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 yzz;
                yzz.X = this.Y;
                yzz.Y = this.Z;
                yzz.Z = this.Z;
                return yzz;
            }
        }

        public IntVector3 YZX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 yzx;
                yzx.X = this.Y;
                yzx.Y = this.Z;
                yzx.Z = this.X;
                return yzx;
            }
        }

        public IntVector3 YXY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 yxy;
                yxy.X = this.Y;
                yxy.Y = this.X;
                yxy.Z = this.Y;
                return yxy;
            }
        }

        public IntVector3 YXZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 yxz;
                yxz.X = this.Y;
                yxz.Y = this.X;
                yxz.Z = this.Z;
                return yxz;
            }
        }

        public IntVector3 YXX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 yxx;
                yxx.X = this.Y;
                yxx.Y = this.X;
                yxx.Z = this.X;
                return yxx;
            }
        }

        public IntVector2 YY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector2 yy;
                yy.X = this.Y;
                yy.Y = this.Y;
                return yy;
            }
        }

        public IntVector2 YZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector2 yz;
                yz.X = this.Y;
                yz.Y = this.Z;
                return yz;
            }
        }

        public IntVector2 YX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector2 yx;
                yx.X = this.Y;
                yx.Y = this.X;
                return yx;
            }
        }

        public IntVector3 ZZZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 zzz;
                zzz.X = this.Z;
                zzz.Y = this.Z;
                zzz.Z = this.Z;
                return zzz;
            }
        }

        public IntVector3 ZZX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 zzx;
                zzx.X = this.Z;
                zzx.Y = this.Z;
                zzx.Z = this.X;
                return zzx;
            }
        }

        public IntVector3 ZZY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 zzy;
                zzy.X = this.Z;
                zzy.Y = this.Z;
                zzy.Z = this.Y;
                return zzy;
            }
        }

        public IntVector3 ZXZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 zxz;
                zxz.X = this.Z;
                zxz.Y = this.X;
                zxz.Z = this.Z;
                return zxz;
            }
        }

        public IntVector3 ZXX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 zxx;
                zxx.X = this.Z;
                zxx.Y = this.X;
                zxx.Z = this.X;
                return zxx;
            }
        }

        public IntVector3 ZXY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 zxy;
                zxy.X = this.Z;
                zxy.Y = this.X;
                zxy.Z = this.Y;
                return zxy;
            }
        }

        public IntVector3 ZYZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 zyz;
                zyz.X = this.Z;
                zyz.Y = this.Y;
                zyz.Z = this.Z;
                return zyz;
            }
        }

        public IntVector3 ZYX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 zyx;
                zyx.X = this.Z;
                zyx.Y = this.Y;
                zyx.Z = this.X;
                return zyx;
            }
        }

        public IntVector3 ZYY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector3 zyy;
                zyy.X = this.Z;
                zyy.Y = this.Y;
                zyy.Z = this.Y;
                return zyy;
            }
        }

        public IntVector2 ZZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector2 zz;
                zz.X = this.Z;
                zz.Y = this.Z;
                return zz;
            }
        }

        public IntVector2 ZX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector2 zx;
                zx.X = this.Z;
                zx.Y = this.X;
                return zx;
            }
        }

        public IntVector2 ZY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                IntVector2 zy;
                zy.X = this.Z;
                zy.Y = this.Y;
                return zy;
            }
        }

        /// <summary>
        ///     Represents an equality comparer for IntVector3 objects.
        /// </summary>
        public class EqualityComparer : IEqualityComparer<IntVector3>
        {
            /// <summary>The global equality comparer instance.</summary>
            public static readonly IntVector3.EqualityComparer Instance = new IntVector3.EqualityComparer();

            private EqualityComparer()
            {
            }

            public bool Equals(IntVector3 x, IntVector3 y) => x == y;

            public int GetHashCode(IntVector3 obj) => obj.GetHashCode();
        }
    }
}