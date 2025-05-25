using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>Represents a 2D Vector</summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FPVector2 : IEquatable<FPVector2>
    {
        /// <summary>
        ///     The size of the component (or struct/type) in-memory inside the Frame data-buffers or stack (when passed as value
        ///     parameter).
        ///     Not related to the snapshot payload this occupies, which is bit-packed and compressed.
        /// </summary>
        public const int SIZE = 16;

        /// <summary>The X component of the vector.</summary>
        [FieldOffset(0)] public FP X;

        /// <summary>The Y component of the vector.</summary>
        [FieldOffset(8)] public FP Y;

        /// <summary>
        ///     Normalizes the given vector. If the vector is too short to normalize,
        ///     <see cref="P:Herta.FPVector2.Zero" /> will be returned.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <returns>A normalized vector.</returns>
        public static FPVector2 Normalize(FPVector2 value)
        {
            ulong x = (ulong)(value.X.RawValue * value.X.RawValue + value.Y.RawValue * value.Y.RawValue);
            if (x == 0UL)
                return new FPVector2();
            FPMath.ExponentMantisaPair exponentMantissa = FPMath.GetSqrtExponentMantissa(x);
            long num = 17592186044416L / (long)exponentMantissa.Mantissa;
            value.X.RawValue = value.X.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            value.Y.RawValue = value.Y.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            return value;
        }

        /// <summary>
        ///     Normalizes the given vector. If the vector is too short to normalize,
        ///     <see cref="P:Herta.FPVector2.Zero" /> will be returned.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <param name="magnitude">The original vector's magnitude.</param>
        /// <returns>A normalized vector.</returns>
        public static FPVector2 Normalize(FPVector2 value, out FP magnitude)
        {
            ulong x = (ulong)(value.X.RawValue * value.X.RawValue + value.Y.RawValue * value.Y.RawValue);
            if (x == 0UL)
            {
                magnitude.RawValue = 0L;
                return new FPVector2();
            }

            FPMath.ExponentMantisaPair exponentMantissa = FPMath.GetSqrtExponentMantissa(x);
            long num = 17592186044416L / (long)exponentMantissa.Mantissa;
            value.X.RawValue = value.X.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            value.Y.RawValue = value.Y.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            magnitude.RawValue = (long)exponentMantissa.Mantissa << exponentMantissa.Exponent;
            magnitude.RawValue >>= 14;
            return value;
        }

        /// <summary>A vector with components (0,0);</summary>
        public static readonly FPVector2 Zero = new FPVector2();

        /// <summary>A vector with components (1,1);</summary>
        public static readonly FPVector2 One = new FPVector2()
        {
            X =
            {
                RawValue = 65536
            },
            Y =
            {
                RawValue = 65536
            }
        };

        /// <summary>A vector with components (1,0);</summary>
        public static readonly FPVector2 Right = new FPVector2()
        {
            X =
            {
                RawValue = 65536
            }
        };

        /// <summary>A vector with components (-1,0);</summary>
        public static readonly FPVector2 Left = new FPVector2()
        {
            X =
            {
                RawValue = -65536
            }
        };

        /// <summary>A vector with components (0,1);</summary>
        public static readonly FPVector2 Up = new FPVector2()
        {
            Y =
            {
                RawValue = 65536
            }
        };

        /// <summary>A vector with components (0,-1);</summary>
        public static readonly FPVector2 Down = new FPVector2()
        {
            Y =
            {
                RawValue = -65536
            }
        };

        /// <summary>
        ///     A vector with components
        ///     (FP.MinValue,FP.MinValue);
        /// </summary>
        public static readonly FPVector2 MinValue = new FPVector2()
        {
            X =
            {
                RawValue = long.MinValue
            },
            Y =
            {
                RawValue = long.MinValue
            }
        };

        /// <summary>
        ///     A vector with components
        ///     (FP.MaxValue,FP.MaxValue);
        /// </summary>
        public static readonly FPVector2 MaxValue = new FPVector2()
        {
            X =
            {
                RawValue = long.MaxValue
            },
            Y =
            {
                RawValue = long.MaxValue
            }
        };

        /// <summary>
        ///     A vector with components
        ///     (FP.UseableMin,FP.UseableMin);
        /// </summary>
        public static readonly FPVector2 UseableMin = new FPVector2()
        {
            X =
            {
                RawValue = (long)int.MinValue
            },
            Y =
            {
                RawValue = (long)int.MinValue
            }
        };

        /// <summary>
        ///     A vector with components
        ///     (FP.UseableMax,FP.UseableMax);
        /// </summary>
        public static readonly FPVector2 UseableMax = new FPVector2()
        {
            X =
            {
                RawValue = (long)int.MaxValue
            },
            Y =
            {
                RawValue = (long)int.MaxValue
            }
        };

        /// <summary>Gets the length of the vector.</summary>
        /// <returns>Returns the length of the vector.</returns>
        public FP Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FP magnitude;
                magnitude.RawValue = FPMath.SqrtRaw((this.X.RawValue * this.X.RawValue + 32768L >> 16) + (this.Y.RawValue * this.Y.RawValue + 32768L >> 16));
                return magnitude;
            }
        }

        /// <summary>Gets the squared length of the vector.</summary>
        /// <returns>Returns the squared length of the vector.</returns>
        public FP SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FP sqrMagnitude;
                sqrMagnitude.RawValue = (this.X.RawValue * this.X.RawValue + 32768L >> 16) + (this.Y.RawValue * this.Y.RawValue + 32768L >> 16);
                return sqrMagnitude;
            }
        }

        /// <summary>Gets a normalized version of the vector.</summary>
        /// <returns>Returns a normalized version of the vector.</returns>
        public FPVector2 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => FPVector2.Normalize(this);
        }

        /// <summary>Initializes a new instance of the FPVector2 struct.</summary>
        /// <param name="x">The x-coordinate of the vector.</param>
        /// <param name="y">The y-coordinate of the vector.</param>
        public FPVector2(int x, int y)
        {
            this.X.RawValue = (long)x << 16;
            this.Y.RawValue = (long)y << 16;
        }

        /// <summary>Creates a new FPVector2 instance.</summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        public FPVector2(FP x, FP y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>Creates a new FPVector2 instance.</summary>
        /// <param name="value">A value to be assigned to both components</param>
        public FPVector2(FP value)
        {
            this.X = value;
            this.Y = value;
        }

        /// <summary>Returns vector (X, 0, Y).</summary>
        public FPVector3 XOY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new FPVector3(this.X, FP._0, this.Y);
        }

        /// <summary>Returns vector (X, Y, 0).</summary>
        public FPVector3 XYO
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new FPVector3(this.X, this.Y, FP._0);
        }

        /// <summary>Returns vector (0, X, Y).</summary>
        public FPVector3 OXY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new FPVector3(FP._0, this.X, this.Y);
        }

        /// <summary>
        ///     Determines whether the current FPVector2 instance is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified object is equal to the current FPVector2 instance; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object? obj) => obj is FPVector2 fpVector2 && this == fpVector2;

        /// <summary>
        ///     Determines whether an FPVector2 instance is equal to another FPVector2 instance.
        /// </summary>
        /// <param name="other">The other FPVector2 instance to compare to.</param>
        /// <returns><see langword="true" /> if the two instances are equal; otherwise, <see langword="false" />.</returns>
        public bool Equals(FPVector2 other) => this == other;

        /// <summary>
        ///     Computes the hash code for the current FPVector2 object.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => XxHash.Hash32(this);

        /// <summary>
        ///     Returns a string that represents the current FPVector2 instance.
        /// </summary>
        /// <returns>A string representation of the current FPVector2 instance.</returns>
        public override string ToString()
        {
            NativeString builder = new NativeString(stackalloc char[64], 0);
            Format(ref builder);

            return builder.ToString();
        }

        public bool TryFormat(Span<char> destination, out int charsWritten)
        {
            NativeString builder = new NativeString(stackalloc char[64], 0);
            Format(ref builder);

            bool result = builder.TryCopyTo(destination);
            charsWritten = result ? builder.Length : 0;
            return result;
        }

        private void Format(ref NativeString builder)
        {
            builder.Append('(');
            builder.AppendFormattable(this.X.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.Y.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(')');
        }

        /// <summary>Calculates the distance between two vectors</summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The distance between the vectors</returns>
        public static FP Distance(FPVector2 a, FPVector2 b)
        {
            long num1 = a.X.RawValue - b.X.RawValue;
            long num2 = a.Y.RawValue - b.Y.RawValue;
            FP fp;
            fp.RawValue = FPMath.SqrtRaw((num1 * num1 + 32768L >> 16) + (num2 * num2 + 32768L >> 16));
            return fp;
        }

        /// <summary>Calculates the squared distance between two vectors</summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The squared distance between the vectors</returns>
        public static FP DistanceSquared(FPVector2 a, FPVector2 b)
        {
            long num1 = a.X.RawValue - b.X.RawValue;
            long num2 = a.Y.RawValue - b.Y.RawValue;
            FP fp;
            fp.RawValue = (num1 * num1 + 32768L >> 16) + (num2 * num2 + 32768L >> 16);
            return fp;
        }

        /// <summary>Calculates the dot product of two vectors.</summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        public static FP Dot(FPVector2 a, FPVector2 b)
        {
            long num1 = a.X.RawValue * b.X.RawValue + 32768L >> 16;
            long num2 = a.Y.RawValue * b.Y.RawValue + 32768L >> 16;
            FP fp;
            fp.RawValue = num1 + num2;
            return fp;
        }

        /// <summary>Clamps the magnitude of a vector</summary>
        /// <param name="vector">Vector to clamp</param>
        /// <param name="maxLength">Max length of the supplied vector</param>
        /// <returns>The resulting (potentially clamped) vector</returns>
        public static FPVector2 ClampMagnitude(FPVector2 vector, FP maxLength)
        {
            long x = (vector.X.RawValue * vector.X.RawValue + 32768L >> 16) + (vector.Y.RawValue * vector.Y.RawValue + 32768L >> 16);
            if (x <= maxLength.RawValue * maxLength.RawValue + 32768L >> 16)
                return vector;
            long num = FPMath.SqrtRaw(x);
            if (num <= FP.Epsilon.RawValue)
                return new FPVector2();
            vector.X.RawValue = (vector.X.RawValue << 16) / num;
            vector.Y.RawValue = (vector.Y.RawValue << 16) / num;
            vector.X.RawValue = vector.X.RawValue * maxLength.RawValue + 32768L >> 16;
            vector.Y.RawValue = vector.Y.RawValue * maxLength.RawValue + 32768L >> 16;
            return vector;
        }

        /// <summary>
        ///     Rotates each vector of <paramref name="vectors" /> by <paramref name="radians" /> radians.
        /// </summary>
        /// Rotation is counterclockwise.
        /// <param name="vectors"></param>
        /// <param name="radians"></param>
        public static void Rotate(Span<FPVector2> vectors, FP radians)
        {
            for (int index = 0; index < vectors.Length; ++index)
                vectors[index] = FPVector2.Rotate(vectors[index], radians);
        }

        /// <summary>
        ///     Rotates <paramref name="vector" /> by <paramref name="radians" /> radians.
        /// </summary>
        /// Rotation is counterclockwise.
        /// <param name="vector"></param>
        /// <param name="radians"></param>
        public static FPVector2 Rotate(FPVector2 vector, FP radians)
        {
            long sinRaw;
            long cosRaw;
            FPMath.SinCosRaw(radians, out sinRaw, out cosRaw);
            long num1 = (vector.X.RawValue * cosRaw + 32768L >> 16) - (vector.Y.RawValue * sinRaw + 32768L >> 16);
            long num2 = (vector.X.RawValue * sinRaw + 32768L >> 16) + (vector.Y.RawValue * cosRaw + 32768L >> 16);
            vector.X.RawValue = num1;
            vector.Y.RawValue = num2;
            return vector;
        }

        /// <summary>
        ///     Rotates each vector of <paramref name="vectors" /> by an angle <paramref name="sin" /> is the sine of and
        ///     <paramref name="cos" /> is the cosine of.
        /// </summary>
        /// Rotation is performed counterclockwise.
        /// <param name="vectors"></param>
        /// <param name="sin"></param>
        /// <param name="cos"></param>
        public static void Rotate(Span<FPVector2> vectors, FP sin, FP cos)
        {
            for (int index = 0; index < vectors.Length; ++index)
                vectors[index] = FPVector2.Rotate(vectors[index], sin, cos);
        }

        /// <summary>
        ///     Rotates <paramref name="vector" /> by an angle <paramref name="sin" /> is the sine of and <paramref name="cos" />
        ///     is the cosine of.
        /// </summary>
        /// Rotation is performed counterclockwise.
        /// <param name="vector"></param>
        /// <param name="sin"></param>
        /// <param name="cos"></param>
        /// <returns></returns>
        public static FPVector2 Rotate(FPVector2 vector, FP sin, FP cos)
        {
            long num1 = (vector.X.RawValue * cos.RawValue + 32768L >> 16) - (vector.Y.RawValue * sin.RawValue + 32768L >> 16);
            long num2 = (vector.X.RawValue * sin.RawValue + 32768L >> 16) + (vector.Y.RawValue * cos.RawValue + 32768L >> 16);
            vector.X.RawValue = num1;
            vector.Y.RawValue = num2;
            return vector;
        }

        /// <summary>
        ///     The perp-dot product (a 2D equivalent of the 3D cross product) of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cross product of both vectors.</returns>
        public static FP Cross(FPVector2 a, FPVector2 b)
        {
            FP fp;
            fp.RawValue = (a.X.RawValue * b.Y.RawValue + 32768L >> 16) - (a.Y.RawValue * b.X.RawValue + 32768L >> 16);
            return fp;
        }

        internal static long CrossRaw(FPVector2 a, FPVector2 b) => (a.X.RawValue * b.Y.RawValue + 32768L >> 16) - (a.Y.RawValue * b.X.RawValue + 32768L >> 16);

        /// <summary>Reflects a vector off the line defined by a normal.</summary>
        /// <param name="vector">Vector to be reflected.</param>
        /// <param name="normal">Normal along which the vector is reflected. Expected to be normalized.</param>
        /// <returns></returns>
        public static FPVector2 Reflect(FPVector2 vector, FPVector2 normal)
        {
            FP fp;
            fp.RawValue = (vector.X.RawValue * normal.X.RawValue + 32768L >> 15) + (vector.Y.RawValue * normal.Y.RawValue + 32768L >> 15);
            vector.X.RawValue -= fp.RawValue * normal.X.RawValue + 32768L >> 16;
            vector.Y.RawValue -= fp.RawValue * normal.Y.RawValue + 32768L >> 16;
            return vector;
        }

        /// <summary>
        ///     Clamps each component of <paramref name="value" /> to the range [<paramref name="min" />, <paramref name="max" />]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static FPVector2 Clamp(FPVector2 value, FPVector2 min, FPVector2 max) => new FPVector2(FPMath.Clamp(value.X, min.X, max.X), FPMath.Clamp(value.Y, min.Y, max.Y));

        /// <summary>
        ///     Linearly interpolates between <paramref name="start" /> and <paramref name="end" /> by <paramref name="t" />.
        ///     <paramref name="t" /> is clamped to the range [0, 1]
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FPVector2 Lerp(FPVector2 start, FPVector2 end, FP t)
        {
            if (t.RawValue < 0L)
                t.RawValue = 0L;
            if (t.RawValue > 65536L)
                t.RawValue = 65536L;
            start.X.RawValue += (end.X.RawValue - start.X.RawValue) * t.RawValue + 32768L >> 16;
            start.Y.RawValue += (end.Y.RawValue - start.Y.RawValue) * t.RawValue + 32768L >> 16;
            return start;
        }

        /// <summary>
        ///     Linearly interpolates between <paramref name="start" /> and <paramref name="end" /> by <paramref name="t" />.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FPVector2 LerpUnclamped(FPVector2 start, FPVector2 end, FP t)
        {
            start.X.RawValue += (end.X.RawValue - start.X.RawValue) * t.RawValue + 32768L >> 16;
            start.Y.RawValue += (end.Y.RawValue - start.Y.RawValue) * t.RawValue + 32768L >> 16;
            return start;
        }

        /// <summary>
        ///     Gets a vector with the maximum x and y values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the maximum x and y values of both vectors.</returns>
        public static FPVector2 Max(FPVector2 value1, FPVector2 value2) => new FPVector2(FPMath.Max(value1.X, value2.X), FPMath.Max(value1.Y, value2.Y));

        /// <summary>
        ///     Gets a vector with the maximum x and y values of all the vectors. If
        ///     <paramref name="vectors" /> is <see langword="null" /> or empty, return
        ///     <see cref="P:Herta.FPVector2.Zero" />.
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static FPVector2 Max(ReadOnlySpan<FPVector2> vectors)
        {
            if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(vectors)) || vectors.Length == 0)
                return new FPVector2();
            FPVector2 vector = vectors[0];
            for (int index = 1; index < vectors.Length; ++index)
            {
                vector.X.RawValue = Math.Max(vector.X.RawValue, vectors[index].X.RawValue);
                vector.Y.RawValue = Math.Max(vector.Y.RawValue, vectors[index].Y.RawValue);
            }

            return vector;
        }

        /// <summary>
        ///     Gets a vector with the minimum x and y values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the minimum x and y values of both vectors.</returns>
        public static FPVector2 Min(FPVector2 value1, FPVector2 value2) => new FPVector2(FPMath.Min(value1.X, value2.X), FPMath.Min(value1.Y, value2.Y));

        /// <summary>
        ///     Gets a vector with the min x and y values of all the vectors. If
        ///     <paramref name="vectors" /> is <see langword="null" /> or empty, return
        ///     <see cref="P:Herta.FPVector2.Zero" />.
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static FPVector2 Min(ReadOnlySpan<FPVector2> vectors)
        {
            if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(vectors)) || vectors.Length == 0)
                return new FPVector2();
            FPVector2 vector = vectors[0];
            for (int index = 1; index < vectors.Length; ++index)
            {
                vector.X.RawValue = Math.Min(vector.X.RawValue, vectors[index].X.RawValue);
                vector.Y.RawValue = Math.Min(vector.Y.RawValue, vectors[index].Y.RawValue);
            }

            return vector;
        }

        /// <summary>
        ///     Multiplies each component of the vector by the same components of the provided vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FPVector2 Scale(FPVector2 a, FPVector2 b)
        {
            FPVector2 fpVector2;
            fpVector2.X = a.X * b.X;
            fpVector2.Y = a.Y * b.Y;
            return fpVector2;
        }

        /// <summary>
        ///     Returns the angle in degrees between <paramref name="a" /> and <paramref name="b" />.
        ///     <remarks>
        ///         See also:
        ///         <see
        ///             cref="M:Herta.FPVector2.Radians(Herta.FPVector2,Herta.FPVector2)" />
        ///         ,
        ///         <seealso
        ///             cref="M:Herta.FPVector2.RadiansSigned(Herta.FPVector2,Herta.FPVector2)" />
        ///         ,
        ///         <seealso
        ///             cref="M:Herta.FPVector2.RadiansSkipNormalize(Herta.FPVector2,Herta.FPVector2)" />
        ///         ,
        ///         <seealso
        ///             cref="M:Herta.FPVector2.RadiansSignedSkipNormalize(Herta.FPVector2,Herta.FPVector2)" />
        ///         ,
        ///     </remarks>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FP Angle(FPVector2 a, FPVector2 b)
        {
            long x1 = (a.X.RawValue * a.X.RawValue + 32768L >> 16) + (a.Y.RawValue * a.Y.RawValue + 32768L >> 16);
            if (x1 == 0L)
                return new FP();
            long num1 = FPMath.SqrtRaw(x1);
            a.X.RawValue = (a.X.RawValue << 16) / num1;
            a.Y.RawValue = (a.Y.RawValue << 16) / num1;
            long x2 = (b.X.RawValue * b.X.RawValue + 32768L >> 16) + (b.Y.RawValue * b.Y.RawValue + 32768L >> 16);
            if (x2 == 0L)
                return new FP();
            long num2 = FPMath.SqrtRaw(x2);
            b.X.RawValue = (b.X.RawValue << 16) / num2;
            b.Y.RawValue = (b.Y.RawValue << 16) / num2;
            long num3 = (a.X.RawValue * b.X.RawValue + 32768L >> 16) + (a.Y.RawValue * b.Y.RawValue + 32768L >> 16);
            long num4 = num3 >= -65536L ? (num3 <= 65536L ? FPLut.Acos[(int)(num3 + 65536L)] : FPLut.Acos[131072]) : FPLut.Acos[0];
            a.X.RawValue = num4 * FP.Rad2Deg.RawValue + 32768L >> 16;
            return a.X;
        }

        /// <summary>Returns vector rotated by 90 degrees clockwise.</summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static FPVector2 CalculateRight(FPVector2 vector) => new FPVector2(vector.Y, -vector.X);

        /// <summary>
        ///     Returns vector rotated by 90 degrees counterclockwise.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static FPVector2 CalculateLeft(FPVector2 vector) => new FPVector2(-vector.Y, vector.X);

        /// <summary>
        ///     Returns <see langword="true" /> if this vector is on the right side of <paramref name="vector" />
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool IsRightOf(FPVector2 vector) => FPVector2.Dot(FPVector2.CalculateRight(vector), this) > FP._0;

        /// <summary>
        ///     Returns <see langword="true" /> if this vector is on the left side of <paramref name="vector" />
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool IsLeftOf(FPVector2 vector) => FPVector2.Dot(FPVector2.CalculateLeft(vector), this) > FP._0;

        /// <summary>
        ///     Returns determinant of two 2d vectors which is handy to check the angle between them (method is identical to
        ///     FPVector2.Cross).
        ///     Determinant == 0 -&gt; Vector1 and Vector2 are collinear
        ///     Determinant less than 0 -&gt; Vector1 is left of Vector2
        ///     Determinant greater than 0 -&gt; Vector1 is right of Vector2
        /// </summary>
        /// <param name="v1">Vector1</param>
        /// <param name="v2">Vector2</param>
        /// <returns>Determinant</returns>
        public static FP Determinant(FPVector2 v1, FPVector2 v2) => new FP()
        {
            RawValue = (v1.X.RawValue * v2.Y.RawValue + 32768L >> 16) - (v1.Y.RawValue * v2.X.RawValue + 32768L >> 16)
        };

        /// <summary>Returns radians between two vectors.</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FP Radians(FPVector2 a, FPVector2 b) => FPMath.Acos(FPMath.Clamp(FPVector2.Dot(FPVector2.Normalize(a), FPVector2.Normalize(b)), -FP._1, FP._1));

        /// <summary>
        ///     Returns radians between two vectors. Vectors are assumed to be normalized.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FP RadiansSkipNormalize(FPVector2 a, FPVector2 b) => FPMath.Acos(FPMath.Clamp(FPVector2.Dot(a, b), -FP._1, FP._1));

        /// <summary>
        ///     Returns radians between two vectors. The result will be a negative number if
        ///     <paramref name="b" /> is on the right side of <paramref name="a" />.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FP RadiansSigned(FPVector2 a, FPVector2 b) => FPVector2.Radians(a, b) * FPMath.Sign(a.X * b.Y - a.Y * b.X);

        /// <summary>
        ///     Returns radians between two vectors. The result will be a negative number if
        ///     <paramref name="b" /> is on the right side of <paramref name="a" />. Vectors are assumed to be normalized.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FP RadiansSignedSkipNormalize(FPVector2 a, FPVector2 b) => FPVector2.RadiansSkipNormalize(a, b) * FPMath.Sign(a.X * b.Y - a.Y * b.X);

        /// <summary>
        ///     Interpolates between <paramref name="start" /> and <paramref name="end" /> with smoothing at the limits.
        ///     Equivalent of calling
        ///     <see
        ///         cref="M:Herta.FPMath.SmoothStep(Herta.FP,Herta.FP,Herta.FP)" />
        ///     for each component pair.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FPVector2 SmoothStep(FPVector2 start, FPVector2 end, FP t) => new FPVector2(FPMath.SmoothStep(start.X, end.X, t), FPMath.SmoothStep(start.Y, end.Y, t));

        /// <summary>
        ///     Equivalent of calling
        ///     <see
        ///         cref="M:Herta.FPMath.Hermite(Herta.FP,Herta.FP,Herta.FP,Herta.FP,Herta.FP)" />
        ///     for each component.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="tangent1"></param>
        /// <param name="value2"></param>
        /// <param name="tangent2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FPVector2 Hermite(
            FPVector2 value1,
            FPVector2 tangent1,
            FPVector2 value2,
            FPVector2 tangent2,
            FP t)
        {
            FPVector2 fpVector2;
            fpVector2.X = FPMath.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, t);
            fpVector2.Y = FPMath.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, t);
            return fpVector2;
        }

        /// <summary>
        ///     Equivalent of calling
        ///     <see
        ///         cref="M:Herta.FPMath.Barycentric(Herta.FP,Herta.FP,Herta.FP,Herta.FP,Herta.FP)" />
        ///     for each component.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static FPVector2 Barycentric(
            FPVector2 value1,
            FPVector2 value2,
            FPVector2 value3,
            FP t1,
            FP t2)
        {
            return new FPVector2(FPMath.Barycentric(value1.X, value2.X, value3.X, t1, t2), FPMath.Barycentric(value1.Y, value2.Y, value3.Y, t1, t2));
        }

        /// <summary>
        ///     Equivalent of calling
        ///     <see
        ///         cref="M:Herta.FPMath.CatmullRom(Herta.FP,Herta.FP,Herta.FP,Herta.FP,Herta.FP)" />
        ///     for each component.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FPVector2 CatmullRom(
            FPVector2 value1,
            FPVector2 value2,
            FPVector2 value3,
            FPVector2 value4,
            FP t)
        {
            return new FPVector2(FPMath.CatmullRom(value1.X, value2.X, value3.X, value4.X, t), FPMath.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, t));
        }

        /// <summary>
        ///     Returns <see langword="true" /> if the polygon defined by <paramref name="vertices" /> is convex.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static bool IsPolygonConvex(ReadOnlySpan<FPVector2> vertices)
        {
            bool flag1 = false;
            bool flag2 = false;
            for (int index1 = 0; index1 < vertices.Length; ++index1)
            {
                int index2 = (index1 + 1) % vertices.Length;
                int index3 = (index2 + 1) % vertices.Length;
                FP fp = FPVector2.CrossProductLength(vertices[index1], vertices[index2], vertices[index3]);
                if (fp < 0)
                    flag1 = true;
                else if (fp > 0)
                    flag2 = true;
                if (flag1 & flag2)
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Calculates the cross product length for three points (A, B, C).
        ///     This represents the signed area of the parallelogram formed by vectors BA and BC.
        /// </summary>
        /// <param name="A">The first point</param>
        /// <param name="B">The second point (base point)</param>
        /// <param name="C">The third point</param>
        /// <returns>The signed cross product length (BA × BC)</returns>
        public static FP CrossProductLength(FPVector2 A, FPVector2 B, FPVector2 C)
        {
            FP fp1 = A.X - B.X;
            FP fp2 = A.Y - B.Y;
            FP fp3 = C.X - B.X;
            FP fp4 = C.Y - B.Y;
            return fp1 * fp4 - fp2 * fp3;
        }

        /// <summary>Checks if the vertices of a polygon are clock-wise</summary>
        /// <param name="vertices">The vertices of the polygon</param>
        /// <returns><see langword="true" /> if the vertices are clock-wise aligned.</returns>
        public static bool IsClockWise(ReadOnlySpan<FPVector2> vertices)
        {
            FPVector2 fpVector2_1 = new FPVector2(vertices[1].X - vertices[0].X, vertices[1].Y - vertices[0].Y);
            FPVector2 fpVector2_2 = new FPVector2(vertices[2].X - vertices[1].X, vertices[2].Y - vertices[1].Y);
            return fpVector2_1.X * fpVector2_2.Y - fpVector2_1.Y * fpVector2_2.X < 0;
        }

        /// <summary>
        ///     Checks if the vertices of a polygon are counter clock-wise.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon</param>
        /// <returns><see langword="true" /> if the vertices are counter clock-wise aligned.</returns>
        public static bool IsCounterClockWise(ReadOnlySpan<FPVector2> vertices) => !FPVector2.IsClockWise(vertices);

        /// <summary>
        ///     Checks if the vertices of a polygon are clock-wise if not makes them counter clock-wise.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon</param>
        public static void MakeCounterClockWise(Span<FPVector2> vertices)
        {
            if (!FPVector2.IsClockWise(vertices))
                return;
            FPVector2.FlipWindingOrder(vertices);
        }

        /// <summary>
        ///     Flips the winding order of the vertices if they are in counter clock-wise order. This ensures that the vertices are
        ///     in clock-wise order.
        /// </summary>
        /// <param name="vertices">The array of vertices</param>
        public static void MakeClockWise(Span<FPVector2> vertices)
        {
            if (!FPVector2.IsCounterClockWise(vertices))
                return;
            FPVector2.FlipWindingOrder(vertices);
        }

        /// <summary>
        ///     Reverses the order of vertices in an array, effectively flipping the winding order of a polygon.
        /// </summary>
        /// <param name="vertices">The array of vertices representing the polygon</param>
        public static void FlipWindingOrder(Span<FPVector2> vertices) => vertices.Reverse();

        /// <summary>
        ///     Calculates a normal for each edge of a polygon defined by <paramref name="source" />.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static bool CalculatePolygonNormals(ReadOnlySpan<FPVector2> source, Span<FPVector2> destination)
        {
            if (destination.Length < source.Length)
                return false;

            for (int index1 = 0; index1 < source.Length; ++index1)
            {
                int index2 = index1 + 1 < source.Length ? index1 + 1 : 0;
                FPVector2 fpVector2 = source[index2] - source[index1];
                destination[index1] = new FPVector2(fpVector2.Y, -fpVector2.X).Normalized;
            }

            return true;
        }

        /// <summary>
        ///     Returns <see langword="true" /> if all normals of a polygon defined by <paramref name="vertices" /> are non-zeroed.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static bool PolygonNormalsAreValid(ReadOnlySpan<FPVector2> vertices)
        {
            for (int index1 = 0; index1 < vertices.Length; ++index1)
            {
                int index2 = (index1 + 1) % vertices.Length;
                if (vertices[index2].X.RawValue == vertices[index1].X.RawValue && vertices[index2].Y.RawValue == vertices[index1].Y.RawValue)
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Shifts polygon defined by <paramref name="source" /> so that (0,0) becomes its center.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static bool RecenterPolygon(ReadOnlySpan<FPVector2> source, Span<FPVector2> destination)
        {
            if (destination.Length < source.Length)
                return false;

            FPVector2 polygonCentroid = FPVector2.CalculatePolygonCentroid(source);

            for (int index = 0; index < source.Length; ++index)
                destination[index] = source[index] - polygonCentroid;

            return true;
        }

        /// <summary>
        ///     Retruns an area of a polygon defined by <paramref name="vertices" />.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static FP CalculatePolygonArea(ReadOnlySpan<FPVector2> vertices)
        {
            FP fp = FP._0;
            for (int index = 0; index < vertices.Length; ++index)
            {
                FPVector2 vertex = vertices[(index + 1) % vertices.Length];
                fp += vertices[index].X * vertex.Y - vertices[index].Y * vertex.X;
            }

            return fp / 2;
        }

        /// <summary>
        ///     Returns a centroid of a polygon defined by <paramref name="vertices" />.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static FPVector2 CalculatePolygonCentroid(ReadOnlySpan<FPVector2> vertices)
        {
            FPVector2 polygonCentroid = new FPVector2();
            FP fp1 = FP._0;
            FP fp2 = FP._0;
            FP fp3 = FP._0;
            FP fp4 = FP._0;
            FP fp5 = FP._0;
            FP fp6 = FP._0;
            for (int index = 0; index < vertices.Length; ++index)
            {
                FP x1 = vertices[index].X;
                FP y1 = vertices[index].Y;
                FP x2 = vertices[(index + 1) % vertices.Length].X;
                FP y2 = vertices[(index + 1) % vertices.Length].Y;
                FP fp7 = x1 * y2 - x2 * y1;
                fp1 += fp7;
                polygonCentroid.X += (x1 + x2) * fp7;
                polygonCentroid.Y += (y1 + y2) * fp7;
            }

            if (fp1 == FP._0)
                throw new ArgumentException("Not a valid polygon");
            FP fp8 = fp1 * FP._0_50;
            polygonCentroid.X /= FP._6 * fp8;
            polygonCentroid.Y /= FP._6 * fp8;
            return polygonCentroid;
        }

        /// <summary>
        ///     Calculates the mass moment of inertia factor of a polygon defined by <paramref name="vertices" />.
        /// </summary>
        /// <remarks>To compute a body mass moment of inertia, multiply the factor by the body mass.</remarks>
        /// <param name="vertices">The 2D vertices that define the polygon.</param>
        /// <returns>The mass moment of inertia factor of the polygon.</returns>
        public static FP CalculatePolygonInertiaFactor(ReadOnlySpan<FPVector2> vertices)
        {
            FP polygonInertiaFactor = new FP();
            long num1 = 0;
            for (int index = 0; index < vertices.Length; ++index)
            {
                FPVector2 vertex1 = vertices[index];
                FPVector2 vertex2 = vertices[(index + 1) % vertices.Length];
                long num2 = (vertex1.X.RawValue * vertex2.Y.RawValue + 32768L >> 16) - (vertex1.Y.RawValue * vertex2.X.RawValue + 32768L >> 16);
                num1 += num2;
                long num3 = (vertex1.X.RawValue * vertex1.X.RawValue + 32768L >> 16) + (vertex1.Y.RawValue * vertex1.Y.RawValue + 32768L >> 16);
                long num4 = (vertex2.X.RawValue * vertex2.X.RawValue + 32768L >> 16) + (vertex2.Y.RawValue * vertex2.Y.RawValue + 32768L >> 16);
                long num5 = (vertex1.X.RawValue * vertex2.X.RawValue + 32768L >> 16) + (vertex1.Y.RawValue * vertex2.Y.RawValue + 32768L >> 16);
                polygonInertiaFactor.RawValue += num2 * (num3 + num4 + num5) + 32768L >> 16;
            }

            polygonInertiaFactor.RawValue = (polygonInertiaFactor.RawValue << 16) / (num1 * FP._6.RawValue + 32768L >> 16);
            return polygonInertiaFactor;
        }

        /// <summary>
        ///     Calculates the support point in a direction <paramref name="localDir" /> of a polygon defined by
        ///     <paramref name="vertices" />.
        ///     <remarks>A support point is the furthest point of a shape in a given direction.</remarks>
        ///     <remarks>Both support point and direction are expressed in the local space of the polygon.</remarks>
        ///     <remarks>The polygon vertices are expected to be counterclockwise.</remarks>
        /// </summary>
        /// <param name="vertices">The 2D vertices that define the polygon.</param>
        /// <param name="localDir">The direction, in local space, in which the support point will be calculated.</param>
        /// <returns>The support point, in local space.</returns>
        public static FPVector2 CalculatePolygonLocalSupport(
            ReadOnlySpan<FPVector2> vertices,
            ref FPVector2 localDir)
        {
            int verticesCount = vertices.Length;
            FPVector2 a1 = vertices[0];
            FPVector2 polygonLocalSupport = a1;
            long rawValue1 = FPVector2.Dot(a1, localDir).RawValue;
            FPVector2 a2 = vertices[1];
            long rawValue2 = FPVector2.Dot(a2, localDir).RawValue;
            if (rawValue2 > rawValue1)
            {
                polygonLocalSupport = a2;
                long num = rawValue2;
                for (int index = 2; index < verticesCount; ++index)
                {
                    FPVector2 a3 = vertices[index];
                    long rawValue3 = FPVector2.Dot(a3, localDir).RawValue;
                    if (rawValue3 > num)
                    {
                        polygonLocalSupport = a3;
                        num = rawValue3;
                    }
                    else
                        break;
                }
            }
            else
            {
                FPVector2 a4 = vertices[verticesCount - 1];
                long rawValue4 = FPVector2.Dot(a4, localDir).RawValue;
                if (rawValue4 > rawValue1)
                {
                    polygonLocalSupport = a4;
                    long num = rawValue4;
                    for (int index = verticesCount - 2; index > 1; --index)
                    {
                        FPVector2 a5 = vertices[index];
                        long rawValue5 = FPVector2.Dot(a5, localDir).RawValue;
                        if (rawValue5 > num)
                        {
                            polygonLocalSupport = a5;
                            num = rawValue5;
                        }
                        else
                            break;
                    }
                }
            }

            return polygonLocalSupport;
        }

        /// <summary>
        ///     Returns a radius of a centered polygon defined by <paramref name="vertices" />.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static FP CalculatePolygonRadius(ReadOnlySpan<FPVector2> vertices)
        {
            FP polygonRadius = FP._0;
            for (int index = 0; index < vertices.Length; ++index)
            {
                FP magnitude = vertices[index].Magnitude;
                if (magnitude > polygonRadius)
                    polygonRadius = magnitude;
            }

            return polygonRadius;
        }

        /// <summary>
        ///     Calculate a position between the points specified by <paramref name="from" /> and <paramref name="to" />, moving no
        ///     farther than the distance specified by <paramref name="maxDelta" />.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDelta"></param>
        /// <returns></returns>
        /// s
        public static FPVector2 MoveTowards(FPVector2 from, FPVector2 to, FP maxDelta)
        {
            FPVector2 fpVector2 = to - from;
            FP magnitude = fpVector2.Magnitude;
            return magnitude.RawValue <= maxDelta.RawValue || magnitude.RawValue == 0L ? to : from + fpVector2 / magnitude * maxDelta;
        }

        /// <summary>
        ///     Returns <see langword="true" /> if two vectors are exactly equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FPVector2 a, FPVector2 b) => a.X.RawValue == b.X.RawValue && a.Y.RawValue == b.Y.RawValue;

        /// <summary>
        ///     Returns <see langword="true" /> if two vectors are not exactly equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FPVector2 a, FPVector2 b) => a.X.RawValue != b.X.RawValue || a.Y.RawValue != b.Y.RawValue;

        /// <summary>
        ///     Negates each component of <paramref name="v" /> vector.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator -(FPVector2 v)
        {
            v.X.RawValue = -v.X.RawValue;
            v.Y.RawValue = -v.Y.RawValue;
            return v;
        }

        /// <summary>Adds two vectors.</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator +(FPVector2 a, FPVector2 b)
        {
            a.X.RawValue += b.X.RawValue;
            a.Y.RawValue += b.Y.RawValue;
            return a;
        }

        /// <summary>
        ///     Subtracts <paramref name="b" /> from <paramref name="a" />
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator -(FPVector2 a, FPVector2 b)
        {
            a.X.RawValue -= b.X.RawValue;
            a.Y.RawValue -= b.Y.RawValue;
            return a;
        }

        /// <summary>
        ///     Multiplies each component of <paramref name="v" /> times <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator *(FPVector2 v, FP s)
        {
            v.X.RawValue = v.X.RawValue * s.RawValue + 32768L >> 16;
            v.Y.RawValue = v.Y.RawValue * s.RawValue + 32768L >> 16;
            return v;
        }

        /// <summary>
        ///     Multiplies each component of <paramref name="v" /> times <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator *(FP s, FPVector2 v)
        {
            v.X.RawValue = v.X.RawValue * s.RawValue + 32768L >> 16;
            v.Y.RawValue = v.Y.RawValue * s.RawValue + 32768L >> 16;
            return v;
        }

        /// <summary>
        ///     Multiplies each component of <paramref name="v" /> times <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator *(FPVector2 v, int s)
        {
            v.X.RawValue *= (long)s;
            v.Y.RawValue *= (long)s;
            return v;
        }

        /// <summary>
        ///     Multiplies each component of <paramref name="v" /> times <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator *(int s, FPVector2 v)
        {
            v.X.RawValue *= (long)s;
            v.Y.RawValue *= (long)s;
            return v;
        }

        /// <summary>
        ///     Divides each component of <paramref name="v" /> by <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator /(FPVector2 v, FP s)
        {
            v.X.RawValue = (v.X.RawValue << 16) / s.RawValue;
            v.Y.RawValue = (v.Y.RawValue << 16) / s.RawValue;
            return v;
        }

        /// <summary>
        ///     Divides each component of <paramref name="v" /> by <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator /(FPVector2 v, int s)
        {
            v.X.RawValue /= (long)s;
            v.Y.RawValue /= (long)s;
            return v;
        }

        /// <summary>
        ///     Returns a new FPVector3 using the X, X and X components of this vector.
        /// </summary>
        public FPVector3 XXX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xxx;
                xxx.X = this.X;
                xxx.Y = this.X;
                xxx.Z = this.X;
                return xxx;
            }
        }

        /// <summary>
        ///     Returns a new FPVector3 using the X, X and Y components of this vector.
        /// </summary>
        public FPVector3 XXY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xxy;
                xxy.X = this.X;
                xxy.Y = this.X;
                xxy.Z = this.Y;
                return xxy;
            }
        }

        /// <summary>
        ///     Returns a new FPVector3 using the X, Y and X components of this vector.
        /// </summary>
        public FPVector3 XYX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xyx;
                xyx.X = this.X;
                xyx.Y = this.Y;
                xyx.Z = this.X;
                return xyx;
            }
        }

        /// <summary>
        ///     Returns a new FPVector3 using the X, Y and Y components of this vector.
        /// </summary>
        public FPVector3 XYY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xyy;
                xyy.X = this.X;
                xyy.Y = this.Y;
                xyy.Z = this.Y;
                return xyy;
            }
        }

        /// <summary>
        ///     Returns a new FPVector2 using the X and X components of this vector.
        /// </summary>
        public FPVector2 XX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector2 xx;
                xx.X = this.X;
                xx.Y = this.X;
                return xx;
            }
        }

        /// <summary>
        ///     Returns a new FPVector2 using the X and Y components of this vector.
        /// </summary>
        public FPVector2 XY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector2 xy;
                xy.X = this.X;
                xy.Y = this.Y;
                return xy;
            }
        }

        /// <summary>
        ///     Returns a new FPVector3 using the Y, Y and Y components of this vector.
        /// </summary>
        public FPVector3 YYY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 yyy;
                yyy.X = this.Y;
                yyy.Y = this.Y;
                yyy.Z = this.Y;
                return yyy;
            }
        }

        /// <summary>
        ///     Returns a new FPVector3 using the Y, Y and X components of this vector.
        /// </summary>
        public FPVector3 YYX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 yyx;
                yyx.X = this.Y;
                yyx.Y = this.Y;
                yyx.Z = this.X;
                return yyx;
            }
        }

        /// <summary>
        ///     Returns a new FPVector3 using the Y, X and Y components of this vector.
        /// </summary>
        public FPVector3 YXY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 yxy;
                yxy.X = this.Y;
                yxy.Y = this.X;
                yxy.Z = this.Y;
                return yxy;
            }
        }

        /// <summary>
        ///     Returns a new FPVector3 using the Y, X and X components of this vector.
        /// </summary>
        public FPVector3 YXX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 yxx;
                yxx.X = this.Y;
                yxx.Y = this.X;
                yxx.Z = this.X;
                return yxx;
            }
        }

        /// <summary>
        ///     Returns a new FPVector2 using the Y and Y components of this vector.
        /// </summary>
        public FPVector2 YY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector2 yy;
                yy.X = this.Y;
                yy.Y = this.Y;
                return yy;
            }
        }

        /// <summary>
        ///     Returns a new FPVector2 using the Y and X components of this vector.
        /// </summary>
        public FPVector2 YX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector2 yx;
                yx.X = this.Y;
                yx.Y = this.X;
                return yx;
            }
        }

        /// <summary>
        ///     Represents an equality comparer for FPVector2 objects.
        /// </summary>
        public class EqualityComparer : IEqualityComparer<FPVector2>
        {
            /// <summary>The global FPVector2 equality comparer instance.</summary>
            public static readonly FPVector2.EqualityComparer Instance = new FPVector2.EqualityComparer();

            private EqualityComparer()
            {
            }

            bool IEqualityComparer<FPVector2>.Equals(FPVector2 x, FPVector2 y) => x == y;

            int IEqualityComparer<FPVector2>.GetHashCode(FPVector2 obj) => obj.GetHashCode();
        }
    }
}