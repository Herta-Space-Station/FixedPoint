using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     Represents a two-dimensional vector with integer components.
    /// </summary>
    /// \ingroup MathApi
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct IntVector2 : IEquatable<IntVector2>
    {
        /// <summary>The size of the struct in memory.</summary>
        public const int SIZE = 8;

        /// <summary>The X component of the vector.</summary>
        [FieldOffset(0)] public int X;

        /// <summary>The Y component of the vector.</summary>
        [FieldOffset(4)] public int Y;

        /// <summary>A vector with components (0,0);</summary>
        public static readonly IntVector2 Zero = new IntVector2(0, 0);

        /// <summary>A vector with components (1,1);</summary>
        public static readonly IntVector2 One = new IntVector2(1, 1);

        /// <summary>A vector with components (1,0);</summary>
        public static readonly IntVector2 Right = new IntVector2(1, 0);

        /// <summary>A vector with components (-1,0);</summary>
        public static readonly IntVector2 Left = new IntVector2(-1, 0);

        /// <summary>A vector with components (0,1);</summary>
        public static readonly IntVector2 Up = new IntVector2(0, 1);

        /// <summary>A vector with components (0,-1);</summary>
        public static readonly IntVector2 Down = new IntVector2(0, -1);

        /// <summary>
        ///     A vector with components (int.MaxValue, int.MaxValue);
        /// </summary>
        public static readonly IntVector2 MaxValue = new IntVector2(int.MaxValue, int.MaxValue);

        /// <summary>
        ///     A vector with components (int.MinValue, int.MinValue);
        /// </summary>
        public static readonly IntVector2 MinValue = new IntVector2(int.MinValue, int.MinValue);

        /// <summary>Initializes a new instance of the IntVector2 struct.</summary>
        /// <param name="x">The x-coordinate of the vector.</param>
        /// <param name="y">The y-coordinate of the vector.</param>
        public IntVector2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>Returns vector (X, 0, Y).</summary>
        public IntVector3 XOY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new IntVector3(this.X, 0, this.Y);
        }

        /// <summary>Returns vector (X, Y, 0).</summary>
        public IntVector3 XYO
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new IntVector3(this.X, this.Y, 0);
        }

        /// <summary>Returns vector (0, X, Y).</summary>
        public IntVector3 OXY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new IntVector3(0, this.X, this.Y);
        }

        /// <summary>Returns a hash code for this instance.</summary>
        public override int GetHashCode() => HashCode.Combine<int, int>(this.X, this.Y);

        /// <summary>
        ///     Returns a string that represents the current IntVector2.
        /// </summary>
        /// <returns>A string that represents the current IntVector2.</returns>
        public override string ToString() => string.Format((IFormatProvider)CultureInfo.InvariantCulture, "({0}, {1})", (object)this.X, (object)this.Y);

        /// <summary>Gets the magnitude of the vector.</summary>
        public FP Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => FPMath.Sqrt((FP)(this.X * this.X + this.Y * this.Y));
        }

        /// <summary>Gets the squared magnitude of the vector.</summary>
        public int SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.X * this.X + this.Y * this.Y;
        }

        /// <summary>
        ///     Clamps a IntVector2 value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The IntVector2 value to clamp</param>
        /// <param name="min">The minimum IntVector2 value to clamp to</param>
        /// <param name="max">The maximum IntVector2 value to clamp to</param>
        /// <returns>The clamped IntVector2 value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 Clamp(IntVector2 value, IntVector2 min, IntVector2 max) => new IntVector2(FPMath.Clamp(value.X, min.X, max.X), FPMath.Clamp(value.Y, min.Y, max.Y));

        /// <summary>
        ///     Calculates the distance between two IntVector2 points.
        /// </summary>
        /// <param name="a">The first IntVector2 point</param>
        /// <param name="b">The second IntVector2 point</param>
        /// <returns>The distance between the two IntVector2 points</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Distance(IntVector2 a, IntVector2 b) => (a - b).Magnitude;

        /// <summary>
        ///     Returns a new IntVector2 with the largest components of the input vectors.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 Max(IntVector2 a, IntVector2 b) => new IntVector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        /// <summary>
        ///     Returns a new IntVector2 with the smallest components of the input vectors.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 Min(IntVector2 a, IntVector2 b) => new IntVector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

        /// <summary>
        ///     Rounds a FPVector2 to the nearest whole numbers, and returns a new IntVector2.
        /// </summary>
        /// <param name="v">The FPVector2 to round.</param>
        /// <returns>A new IntVector2 with the rounded components of the input vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 RoundToInt(FPVector2 v) => new IntVector2(FPMath.RoundToInt(v.X), FPMath.RoundToInt(v.Y));

        /// <summary>
        ///     Returns the largest integer less than or equal to the specified floating-point number.
        /// </summary>
        /// <param name="v">The floating-point number.</param>
        /// <returns>The largest integer less than or equal to <paramref name="v" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 FloorToInt(FPVector2 v) => new IntVector2(FPMath.FloorToInt(v.X), FPMath.FloorToInt(v.Y));

        /// <summary>
        ///     Returns a new IntVector2 with the smallest integer greater than or equal to the components of the given FPVector2.
        /// </summary>
        /// <param name="v">The FPVector2 to ceil.</param>
        /// <returns>A new IntVector2 with the components ceil'd.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 CeilToInt(FPVector2 v) => new IntVector2(FPMath.CeilToInt(v.X), FPMath.CeilToInt(v.Y));

        /// <summary>
        ///     Implicitly converts a IntVector2 instance to an FPVector2 instance.
        ///     This conversion creates a new FPVector2 instance with the X and Y coordinates
        ///     from the input IntVector2 instance.
        /// </summary>
        /// <param name="v">The IntVector2 instance to convert.</param>
        /// <returns>A new FPVector2 instance with the X and Y coordinates from the input IntVector2 instance.</returns>
        public static implicit operator FPVector2(IntVector2 v) => new FPVector2(v.X, v.Y);

        /// <summary>
        ///     Converts a FPVector2 instance to a IntVector2 instance.
        /// </summary>
        public static explicit operator IntVector2(FPVector2 v) => new IntVector2(v.X.AsInt, v.Y.AsInt);

        /// <summary>Adds two IntVector2 instances.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator +(IntVector2 a, IntVector2 b) => new IntVector2(a.X + b.X, a.Y + b.Y);

        /// <summary>Subtracts the second IntVector2 from the first.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator -(IntVector2 a, IntVector2 b) => new IntVector2(a.X - b.X, a.Y - b.Y);

        /// <summary>Multiplies a IntVector2 by an integer scalar.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator *(IntVector2 a, int d) => new IntVector2(a.X * d, a.Y * d);

        /// <summary>Multiplies a IntVector2 by an integer scalar.</summary>
        /// <param name="d"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator *(int d, IntVector2 a) => new IntVector2(a.X * d, a.Y * d);

        /// <summary>Divides a IntVector2 by an integer scalar.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator /(IntVector2 a, int d) => new IntVector2(a.X / d, a.Y / d);

        /// <summary>Compares two IntVector2 instances for equality.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(IntVector2 lhs, IntVector2 rhs) => lhs.X == rhs.X && lhs.Y == rhs.Y;

        /// <summary>Compares two IntVector2 instances for inequality.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(IntVector2 lhs, IntVector2 rhs) => !(lhs == rhs);

        /// <summary>Negates an IntVector2.</summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator -(IntVector2 a) => new IntVector2(-a.X, -a.Y);

        /// <summary>
        ///     Determines whether the specified IntVector2 is equal to the current IntVector2.
        /// </summary>
        /// <param name="other">The IntVector2 to compare with the current IntVector2.</param>
        /// <returns>true if the specified IntVector2 is equal to the current IntVector2; otherwise, false.</returns>
        public bool Equals(IntVector2 other) => this.X == other.X && this.Y == other.Y;

        /// <summary>
        ///     Determines whether the specified object is equal to the current IntVector2.
        /// </summary>
        /// <param name="obj">The object to compare with the current IntVector2.</param>
        /// <returns>true if the specified object is equal to the current IntVector2; otherwise, false.</returns>
        public override bool Equals(object? obj) => obj is IntVector2 other && this.Equals(other);

        /// <summary>
        ///     Returns a new IntVector3 using the X, X and X components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector3 using the X, X and Y components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector3 using the X, Y and X components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector3 using the X, Y and Y components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector2 using the X and X components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector2 using the X and Y components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector3 using the Y, Y and Y components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector3 using the Y, Y and X components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector3 using the Y, X and Y components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector3 using the Y, X and X components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector2 using the Y and Y components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Returns a new IntVector2 using the Y and X components of this vector.
        /// </summary>
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

        /// <summary>
        ///     Represents an equality comparer for IntVector2 objects.
        /// </summary>
        public class EqualityComparer : IEqualityComparer<IntVector2>
        {
            /// <summary>The global equality comparer instance.</summary>
            public static readonly IntVector2.EqualityComparer Instance = new IntVector2.EqualityComparer();

            private EqualityComparer()
            {
            }

            /// <summary>
            ///     Determines whether the current instance is equal to the specified object.
            /// </summary>
            public bool Equals(IntVector2 x, IntVector2 y) => x == y;

            /// <summary>Computes the hash code for the IntVector2.</summary>
            /// <returns>The computed hash code.</returns>
            public int GetHashCode(IntVector2 obj) => obj.GetHashCode();
        }
    }
}