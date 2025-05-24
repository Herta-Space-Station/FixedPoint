using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>Represents a 3D Vector</summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FPVector3 : IEquatable<FPVector3>
    {
        /// <summary>The size of the vector (3 FP values).</summary>
        public const int SIZE = 24;

        /// <summary>The X component of the vector.</summary>
        [FieldOffset(0)] public FP X;

        /// <summary>The Y component of the vector.</summary>
        [FieldOffset(8)] public FP Y;

        /// <summary>The Z component of the vector.</summary>
        [FieldOffset(16)] public FP Z;

        /// <summary>
        ///     Normalizes the given vector. If the vector is too short to normalize,
        ///     <see cref="P:Herta.FPVector3.Zero" /> will be returned.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <returns>A normalized vector.</returns>
        public static FPVector3 Normalize(FPVector3 value)
        {
            ulong x = (ulong)(value.X.RawValue * value.X.RawValue + value.Y.RawValue * value.Y.RawValue + value.Z.RawValue * value.Z.RawValue);
            if (x == 0UL)
                return new FPVector3();
            FPMath.ExponentMantisaPair exponentMantissa = FPMath.GetSqrtExponentMantissa(x);
            long num = 17592186044416L / (long)exponentMantissa.Mantissa;
            value.X.RawValue = value.X.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            value.Y.RawValue = value.Y.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            value.Z.RawValue = value.Z.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            return value;
        }

        /// <summary>
        ///     Normalizes the given vector. If the vector is too short to normalize,
        ///     <see cref="P:Herta.FPVector3.Zero" /> will be returned.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <param name="magnitude">The original vector's magnitude.</param>
        /// <returns>A normalized vector.</returns>
        public static FPVector3 Normalize(FPVector3 value, out FP magnitude)
        {
            ulong x = (ulong)(value.X.RawValue * value.X.RawValue + value.Y.RawValue * value.Y.RawValue + value.Z.RawValue * value.Z.RawValue);
            if (x == 0UL)
            {
                magnitude.RawValue = 0L;
                return new FPVector3();
            }

            FPMath.ExponentMantisaPair exponentMantissa = FPMath.GetSqrtExponentMantissa(x);
            long num = 17592186044416L / (long)exponentMantissa.Mantissa;
            value.X.RawValue = value.X.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            value.Y.RawValue = value.Y.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            value.Z.RawValue = value.Z.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            magnitude.RawValue = (long)exponentMantissa.Mantissa << exponentMantissa.Exponent;
            magnitude.RawValue >>= 14;
            return value;
        }

        /// <summary>A vector with components (0,0,0);</summary>
        public static readonly FPVector3 Zero = new FPVector3();

        /// <summary>A vector with components (-1,0,0);</summary>
        public static readonly FPVector3 Left = new FPVector3()
        {
            X =
            {
                RawValue = -65536
            }
        };

        /// <summary>A vector with components (1,0,0);</summary>
        public static readonly FPVector3 Right = new FPVector3()
        {
            X =
            {
                RawValue = 65536
            }
        };

        /// <summary>A vector with components (0,1,0);</summary>
        public static readonly FPVector3 Up = new FPVector3()
        {
            Y =
            {
                RawValue = 65536
            }
        };

        /// <summary>A vector with components (0,-1,0);</summary>
        public static readonly FPVector3 Down = new FPVector3()
        {
            Y =
            {
                RawValue = -65536
            }
        };

        /// <summary>A vector with components (0,0,-1);</summary>
        public static readonly FPVector3 Back = new FPVector3()
        {
            Z =
            {
                RawValue = -65536
            }
        };

        /// <summary>A vector with components (0,0,1);</summary>
        public static readonly FPVector3 Forward = new FPVector3()
        {
            Z =
            {
                RawValue = 65536
            }
        };

        /// <summary>A vector with components (1,1,1);</summary>
        public static readonly FPVector3 One = new FPVector3()
        {
            X =
            {
                RawValue = 65536
            },
            Y =
            {
                RawValue = 65536
            },
            Z =
            {
                RawValue = 65536
            }
        };

        /// <summary>
        ///     A vector with components
        ///     (FP.MinValue,FP.MinValue,FP.MinValue);
        /// </summary>
        public static readonly FPVector3 MinValue = new FPVector3()
        {
            X =
            {
                RawValue = long.MinValue
            },
            Y =
            {
                RawValue = long.MinValue
            },
            Z =
            {
                RawValue = long.MinValue
            }
        };

        /// <summary>
        ///     A vector with components
        ///     (FP.MaxValue,FP.MaxValue,FP.MaxValue);
        /// </summary>
        public static readonly FPVector3 MaxValue = new FPVector3()
        {
            X =
            {
                RawValue = long.MaxValue
            },
            Y =
            {
                RawValue = long.MaxValue
            },
            Z =
            {
                RawValue = long.MaxValue
            }
        };

        /// <summary>
        ///     A vector with components
        ///     (FP.UseableMin,FP.UseableMin,FP.UseableMin);
        /// </summary>
        public static readonly FPVector3 UseableMin = new FPVector3()
        {
            X =
            {
                RawValue = (long)int.MinValue
            },
            Y =
            {
                RawValue = (long)int.MinValue
            },
            Z =
            {
                RawValue = (long)int.MinValue
            }
        };

        /// <summary>
        ///     A vector with components
        ///     (FP.UseableMax,FP.UseableMax,FP.UseableMax);
        /// </summary>
        public static readonly FPVector3 UseableMax = new FPVector3()
        {
            X =
            {
                RawValue = (long)int.MaxValue
            },
            Y =
            {
                RawValue = (long)int.MaxValue
            },
            Z =
            {
                RawValue = (long)int.MaxValue
            }
        };

        /// <summary>Gets the squared length of the vector.</summary>
        /// <returns>Returns the squared length of the vector.</returns>
        public FP SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FP sqrMagnitude;
                sqrMagnitude.RawValue = (this.X.RawValue * this.X.RawValue + 32768L >> 16) + (this.Y.RawValue * this.Y.RawValue + 32768L >> 16) + (this.Z.RawValue * this.Z.RawValue + 32768L >> 16);
                return sqrMagnitude;
            }
        }

        /// <summary>Gets the length of the vector.</summary>
        /// <returns>Returns the length of the vector.</returns>
        public FP Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => FPMath.Sqrt(this.SqrMagnitude);
        }

        /// <summary>Gets a normalized version of the vector.</summary>
        /// <returns>Returns a normalized version of the vector.</returns>
        public FPVector3 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => FPVector3.Normalize(this);
        }

        /// <summary>
        ///     Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>
        public FPVector3(int x, int y, int z)
        {
            this.X.RawValue = (long)x << 16;
            this.Y.RawValue = (long)y << 16;
            this.Z.RawValue = (long)z << 16;
        }

        /// <summary>
        ///     Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        public FPVector3(int x, int y)
        {
            this.X.RawValue = (long)x << 16;
            this.Y.RawValue = (long)y << 16;
            this.Z = FP._0;
        }

        /// <summary>
        ///     Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>
        public FPVector3(FP x, FP y, FP z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        ///     Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        public FPVector3(FP x, FP y)
        {
            this.X = x;
            this.Y = y;
            this.Z = FP._0;
        }

        /// <summary>Builds a string from the FPVector3.</summary>
        /// <returns>A string containing all three components.</returns>
        public override string ToString() => string.Format((IFormatProvider)CultureInfo.InvariantCulture, "({0}, {1}, {2})", (object)this.X.AsFloat, (object)this.Y.AsFloat, (object)this.Z.AsFloat);

        /// <summary>Tests if an object is equal to this vector.</summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>Returns <see langword="true" /> if they are euqal, otherwise <see langword="false" />.</returns>
        public override bool Equals(object? obj) => obj is FPVector3 fpVector3 && this == fpVector3;

        /// <summary>
        ///     Determines whether the current instance is equal to the specified FPVector3.
        /// </summary>
        /// <param name="other">The FPVector3 to compare with the current instance.</param>
        /// <returns>
        ///     <see langword="true" /> if the current instance is equal to the specified FPVector3; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public bool Equals(FPVector3 other) => this == other;

        /// <summary>Gets the hashcode of the vector.</summary>
        /// <returns>Returns the hashcode of the vector.</returns>
        public override int GetHashCode() => XxHash.Hash32(this);

        /// <summary>
        ///     Returns a vector where each component is the absolute value of same component in <paramref name="value" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FPVector3 Abs(FPVector3 value)
        {
            long num1 = value.X.RawValue >> 63;
            value.X.RawValue = value.X.RawValue + num1 ^ num1;
            long num2 = value.Y.RawValue >> 63;
            value.Y.RawValue = value.Y.RawValue + num2 ^ num2;
            long num3 = value.Z.RawValue >> 63;
            value.Z.RawValue = value.Z.RawValue + num3 ^ num3;
            return value;
        }

        /// <summary>
        ///     Linearly interpolates between <paramref name="start" /> and <paramref name="end" /> by <paramref name="t" />.
        ///     <paramref name="t" /> is clamped to the range [0, 1]
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FPVector3 Lerp(FPVector3 start, FPVector3 end, FP t)
        {
            if (t.RawValue < 0L)
                t.RawValue = 0L;
            if (t.RawValue > 65536L)
                t.RawValue = 65536L;
            start.X.RawValue += (end.X.RawValue - start.X.RawValue) * t.RawValue + 32768L >> 16;
            start.Y.RawValue += (end.Y.RawValue - start.Y.RawValue) * t.RawValue + 32768L >> 16;
            start.Z.RawValue += (end.Z.RawValue - start.Z.RawValue) * t.RawValue + 32768L >> 16;
            return start;
        }

        /// <summary>
        ///     Linearly interpolates between <paramref name="start" /> and <paramref name="end" /> by <paramref name="t" />.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FPVector3 LerpUnclamped(FPVector3 start, FPVector3 end, FP t)
        {
            start.X.RawValue += (end.X.RawValue - start.X.RawValue) * t.RawValue + 32768L >> 16;
            start.Y.RawValue += (end.Y.RawValue - start.Y.RawValue) * t.RawValue + 32768L >> 16;
            start.Z.RawValue += (end.Z.RawValue - start.Z.RawValue) * t.RawValue + 32768L >> 16;
            return start;
        }

        /// <summary>
        ///     Spherically interpolates between <paramref name="from" /> and <paramref name="to" /> by <paramref name="t" />,
        ///     which is clamped to the range [0, 1].
        /// </summary>
        /// <remarks>
        ///     Input vectors are normalized and treated as directions.
        ///     The resultant vector has direction spherically interpolated using the angle and magnitude linearly interpolated
        ///     between the magnitudes of <paramref name="to" /> and <paramref name="from" />.
        /// </remarks>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 Slerp(FPVector3 from, FPVector3 to, FP t)
        {
            if (t.RawValue < 0L)
                t.RawValue = 0L;
            else if (t.RawValue > 65536L)
                t.RawValue = 65536L;
            return FPVector3.SlerpUnclamped(from, to, t);
        }

        /// <summary>
        ///     Spherically interpolates between <paramref name="from" /> and <paramref name="to" /> by <paramref name="t" />.
        /// </summary>
        /// <remarks>
        ///     Input vectors are normalized and treated as directions.
        ///     The resultant vector has direction spherically interpolated using the angle and magnitude linearly interpolated
        ///     between the magnitudes of <paramref name="to" /> and <paramref name="from" />.
        /// </remarks>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FPVector3 SlerpUnclamped(FPVector3 from, FPVector3 to, FP t)
        {
            FP magnitude1;
            from = FPVector3.Normalize(from, out magnitude1);
            FP magnitude2;
            to = FPVector3.Normalize(to, out magnitude2);
            magnitude1.RawValue += (magnitude2.RawValue - magnitude1.RawValue) * t.RawValue + 32768L >> 16;
            FP rad1;
            rad1.RawValue = (from.X.RawValue * to.X.RawValue + 32768L >> 16) + (from.Y.RawValue * to.Y.RawValue + 32768L >> 16) + (from.Z.RawValue * to.Z.RawValue + 32768L >> 16);
            FP rad2;
            rad2.RawValue = rad1.RawValue >= -65536L ? (rad1.RawValue <= 65536L ? FPLut.Acos[(int)(rad1.RawValue + 65536L)] : FPLut.Acos[131072]) : FPLut.Acos[0];
            FP sin;
            FP cos;
            FPMath.SinCosHighPrecision(rad2, out sin, out cos);
            if (sin.RawValue > -396L && sin.RawValue < 396L)
            {
                if (cos.RawValue > 0L)
                {
                    from.X.RawValue = from.X.RawValue * magnitude1.RawValue + 32768L >> 16;
                    from.Y.RawValue = from.Y.RawValue * magnitude1.RawValue + 32768L >> 16;
                    from.Z.RawValue = from.Z.RawValue * magnitude1.RawValue + 32768L >> 16;
                    return from;
                }

                if ((to.Y.RawValue | to.Z.RawValue) == 0L)
                {
                    to.Y.RawValue = to.X.RawValue;
                    to.X.RawValue = -to.Z.RawValue;
                    to.Z.RawValue = to.Y.RawValue;
                    to.Y.RawValue = 0L;
                }
                else
                {
                    to.X.RawValue = to.Y.RawValue;
                    to.Y.RawValue = -to.Z.RawValue;
                    to.Z.RawValue = to.X.RawValue;
                    to.X.RawValue = 0L;
                }

                to = to.Normalized;
                t.RawValue <<= 1;
                sin.RawValue = 65536L;
                cos.RawValue = 0L;
                rad2.RawValue = FPLut.Acos[65536];
            }

            rad1.RawValue = t.RawValue * rad2.RawValue + 32768L >> 16;
            long sinRaw;
            long cosRaw;
            FPMath.SinCosRaw(rad1, out sinRaw, out cosRaw);
            rad1.RawValue = (sin.RawValue * cosRaw + 32768L >> 16) - (cos.RawValue * sinRaw + 32768L >> 16);
            from.X.RawValue = (from.X.RawValue * rad1.RawValue + to.X.RawValue * sinRaw) / sin.RawValue;
            from.Y.RawValue = (from.Y.RawValue * rad1.RawValue + to.Y.RawValue * sinRaw) / sin.RawValue;
            from.Z.RawValue = (from.Z.RawValue * rad1.RawValue + to.Z.RawValue * sinRaw) / sin.RawValue;
            from.X.RawValue = from.X.RawValue * magnitude1.RawValue + 32768L >> 16;
            from.Y.RawValue = from.Y.RawValue * magnitude1.RawValue + 32768L >> 16;
            from.Z.RawValue = from.Z.RawValue * magnitude1.RawValue + 32768L >> 16;
            return from;
        }

        /// <summary>
        ///     Multiplies each component of the vector by the same components of the provided vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FPVector3 Scale(FPVector3 a, FPVector3 b)
        {
            a.X.RawValue = a.X.RawValue * b.X.RawValue + 32768L >> 16;
            a.Y.RawValue = a.Y.RawValue * b.Y.RawValue + 32768L >> 16;
            a.Z.RawValue = a.Z.RawValue * b.Z.RawValue + 32768L >> 16;
            return a;
        }

        /// <summary>Clamps the magnitude of a vector</summary>
        /// <param name="vector">Vector to clamp</param>
        /// <param name="maxLength">Max length of the supplied vector</param>
        /// <returns>The resulting (potentially clamped) vector</returns>
        public static FPVector3 ClampMagnitude(FPVector3 vector, FP maxLength)
        {
            long num = maxLength.RawValue * maxLength.RawValue + 32768L >> 16;
            if ((vector.X.RawValue * vector.X.RawValue + 32768L >> 16) + (vector.Y.RawValue * vector.Y.RawValue + 32768L >> 16) + (vector.Z.RawValue * vector.Z.RawValue + 32768L >> 16) > num)
            {
                vector = FPVector3.Normalize(vector);
                vector.X.RawValue = vector.X.RawValue * maxLength.RawValue + 32768L >> 16;
                vector.Y.RawValue = vector.Y.RawValue * maxLength.RawValue + 32768L >> 16;
                vector.Z.RawValue = vector.Z.RawValue * maxLength.RawValue + 32768L >> 16;
            }

            return vector;
        }

        /// <summary>
        ///     Gets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the minimum x,y and z values of both vectors.</returns>
        public static FPVector3 Min(FPVector3 value1, FPVector3 value2)
        {
            value1.X = value1.X.RawValue < value2.X.RawValue ? value1.X : value2.X;
            value1.Y = value1.Y.RawValue < value2.Y.RawValue ? value1.Y : value2.Y;
            value1.Z = value1.Z.RawValue < value2.Z.RawValue ? value1.Z : value2.Z;
            return value1;
        }

        /// <summary>
        ///     Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the maximum x,y and z values of both vectors.</returns>
        public static FPVector3 Max(FPVector3 value1, FPVector3 value2)
        {
            value1.X = value1.X.RawValue > value2.X.RawValue ? value1.X : value2.X;
            value1.Y = value1.Y.RawValue > value2.Y.RawValue ? value1.Y : value2.Y;
            value1.Z = value1.Z.RawValue > value2.Z.RawValue ? value1.Z : value2.Z;
            return value1;
        }

        /// <summary>Calculates the distance between two vectors</summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The distance between the vectors</returns>
        public static FP Distance(FPVector3 a, FPVector3 b)
        {
            a.X.RawValue -= b.X.RawValue;
            a.Y.RawValue -= b.Y.RawValue;
            a.Z.RawValue -= b.Z.RawValue;
            a.X.RawValue = FPMath.SqrtRaw((a.X.RawValue * a.X.RawValue + 32768L >> 16) + (a.Y.RawValue * a.Y.RawValue + 32768L >> 16) + (a.Z.RawValue * a.Z.RawValue + 32768L >> 16));
            return a.X;
        }

        /// <summary>Calculates the squared distance between two vectors</summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The squared distance between the vectors</returns>
        public static FP DistanceSquared(FPVector3 a, FPVector3 b)
        {
            a.X.RawValue -= b.X.RawValue;
            a.Y.RawValue -= b.Y.RawValue;
            a.Z.RawValue -= b.Z.RawValue;
            a.X.RawValue = (a.X.RawValue * a.X.RawValue + 32768L >> 16) + (a.Y.RawValue * a.Y.RawValue + 32768L >> 16) + (a.Z.RawValue * a.Z.RawValue + 32768L >> 16);
            return a.X;
        }

        /// <summary>The cross product of two vectors.</summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cross product of both vectors.</returns>
        public static FPVector3 Cross(FPVector3 a, FPVector3 b)
        {
            FPVector3 fpVector3;
            fpVector3.X.RawValue = (a.Y.RawValue * b.Z.RawValue + 32768L >> 16) - (a.Z.RawValue * b.Y.RawValue + 32768L >> 16);
            fpVector3.Y.RawValue = (a.Z.RawValue * b.X.RawValue + 32768L >> 16) - (a.X.RawValue * b.Z.RawValue + 32768L >> 16);
            fpVector3.Z.RawValue = (a.X.RawValue * b.Y.RawValue + 32768L >> 16) - (a.Y.RawValue * b.X.RawValue + 32768L >> 16);
            return fpVector3;
        }

        /// <summary>Calculates the dot product of two vectors.</summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        public static FP Dot(FPVector3 a, FPVector3 b)
        {
            a.X.RawValue = (a.X.RawValue * b.X.RawValue + 32768L >> 16) + (a.Y.RawValue * b.Y.RawValue + 32768L >> 16) + (a.Z.RawValue * b.Z.RawValue + 32768L >> 16);
            return a.X;
        }

        /// <summary>
        ///     Returns the signed angle in degrees between <paramref name="a" /> and <paramref name="b" /> when rotated around an
        ///     <paramref name="axis" />.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static FP SignedAngle(FPVector3 a, FPVector3 b, FPVector3 axis)
        {
            FP fp = FPVector3.Angle(a, b);
            FPVector3 fpVector3;
            fpVector3.X.RawValue = (a.Y.RawValue * b.Z.RawValue + 32768L >> 16) - (a.Z.RawValue * b.Y.RawValue + 32768L >> 16);
            fpVector3.Y.RawValue = (a.Z.RawValue * b.X.RawValue + 32768L >> 16) - (a.X.RawValue * b.Z.RawValue + 32768L >> 16);
            fpVector3.Z.RawValue = (a.X.RawValue * b.Y.RawValue + 32768L >> 16) - (a.Y.RawValue * b.X.RawValue + 32768L >> 16);
            a.X.RawValue = (fpVector3.X.RawValue * axis.X.RawValue + 32768L >> 16) + (fpVector3.Y.RawValue * axis.Y.RawValue + 32768L >> 16) + (fpVector3.Z.RawValue * axis.Z.RawValue + 32768L >> 16);
            if (a.X.RawValue < 0L)
                fp.RawValue = -fp.RawValue;
            return fp;
        }

        /// <summary>
        ///     Returns the angle in degrees between <paramref name="a" /> and <paramref name="b" />.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FP Angle(FPVector3 a, FPVector3 b)
        {
            long x1 = (a.X.RawValue * a.X.RawValue + 32768L >> 16) + (a.Y.RawValue * a.Y.RawValue + 32768L >> 16) + (a.Z.RawValue * a.Z.RawValue + 32768L >> 16);
            if (x1 == 0L)
                return new FP();
            long num1 = 4294967296L / FPMath.SqrtRaw(x1);
            a.X.RawValue = a.X.RawValue * num1 + 32768L >> 16;
            a.Y.RawValue = a.Y.RawValue * num1 + 32768L >> 16;
            a.Z.RawValue = a.Z.RawValue * num1 + 32768L >> 16;
            long x2 = (b.X.RawValue * b.X.RawValue + 32768L >> 16) + (b.Y.RawValue * b.Y.RawValue + 32768L >> 16) + (b.Z.RawValue * b.Z.RawValue + 32768L >> 16);
            if (x2 == 0L)
                return new FP();
            long num2 = 4294967296L / FPMath.SqrtRaw(x2);
            b.X.RawValue = b.X.RawValue * num2 + 32768L >> 16;
            b.Y.RawValue = b.Y.RawValue * num2 + 32768L >> 16;
            b.Z.RawValue = b.Z.RawValue * num2 + 32768L >> 16;
            long num3 = (a.X.RawValue * b.X.RawValue + 32768L >> 16) + (a.Y.RawValue * b.Y.RawValue + 32768L >> 16) + (a.Z.RawValue * b.Z.RawValue + 32768L >> 16);
            long num4 = num3 >= -65536L ? (num3 <= 65536L ? FPLut.Acos[(int)(num3 + 65536L)] : FPLut.Acos[131072]) : FPLut.Acos[0];
            a.X.RawValue = num4 * FP.Rad2Deg.RawValue + 32768L >> 16;
            return a.X;
        }

        /// <summary>
        ///     Calculate a position between the points specified by <paramref name="from" /> and <paramref name="to" />, moving no
        ///     farther than the distance specified by <paramref name="maxDelta" />.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDelta"></param>
        /// <returns></returns>
        public static FPVector3 MoveTowards(FPVector3 from, FPVector3 to, FP maxDelta)
        {
            FPVector3 fpVector3;
            fpVector3.X.RawValue = to.X.RawValue - from.X.RawValue;
            fpVector3.Y.RawValue = to.Y.RawValue - from.Y.RawValue;
            fpVector3.Z.RawValue = to.Z.RawValue - from.Z.RawValue;
            long num = FPMath.SqrtRaw((fpVector3.X.RawValue * fpVector3.X.RawValue + 32768L >> 16) + (fpVector3.Y.RawValue * fpVector3.Y.RawValue + 32768L >> 16) + (fpVector3.Z.RawValue * fpVector3.Z.RawValue + 32768L >> 16));
            if (num <= maxDelta.RawValue || num == 0L)
                return to;
            from.X.RawValue += (fpVector3.X.RawValue << 16) / num * maxDelta.RawValue + 32768L >> 16;
            from.Y.RawValue += (fpVector3.Y.RawValue << 16) / num * maxDelta.RawValue + 32768L >> 16;
            from.Z.RawValue += (fpVector3.Z.RawValue << 16) / num * maxDelta.RawValue + 32768L >> 16;
            return from;
        }

        /// <summary>Projects a vector onto another vector.</summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static FPVector3 Project(FPVector3 vector, FPVector3 normal)
        {
            FP fp = FPVector3.Dot(normal, normal);
            return fp < FP.Epsilon ? FPVector3.Zero : normal * FPVector3.Dot(vector, normal) / fp;
        }

        /// <summary>
        ///     Projects a vector onto a plane defined by a normal orthogonal to the plane.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="planeNormal"></param>
        /// <returns>The location of the vector on the plane. </returns>
        public static FPVector3 ProjectOnPlane(FPVector3 vector, FPVector3 planeNormal) => vector - FPVector3.Project(vector, planeNormal);

        /// <summary>Reflects a vector off the plane defined by a normal.</summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static FPVector3 Reflect(FPVector3 vector, FPVector3 normal) => -2 * FPVector3.Dot(normal, vector) * normal + vector;

        /// <summary>
        ///     Creates barycentric coordinates for a point inside a triangle. This method has precision issues due to multiple dot
        ///     product in row this marked internal..
        /// </summary>
        /// <param name="p">Point of interest in triangle</param>
        /// <param name="p0">Vertex 1</param>
        /// <param name="p1">Vertex 2</param>
        /// <param name="p2">Vertex 3</param>
        /// <param name="u">Barycentric variable for p0</param>
        /// <param name="v">Barycentric variable for p1</param>
        /// <param name="w">Barycentric variable for p2</param>
        /// <returns>
        ///     <see langword="true" />, if point is inside the triangle. Out parameter are not set if the point is outside
        ///     the triangle.
        /// </returns>
        internal static bool Barycentric(
            FPVector3 p,
            FPVector3 p0,
            FPVector3 p1,
            FPVector3 p2,
            out FP u,
            out FP v,
            out FP w)
        {
            v.RawValue = 0L;
            w.RawValue = 0L;
            u.RawValue = 0L;
            FPVector3 fpVector3_1 = new FPVector3();
            FPVector3 fpVector3_2;
            fpVector3_2.X.RawValue = p1.X.RawValue - p0.X.RawValue;
            fpVector3_2.Y.RawValue = p1.Y.RawValue - p0.Y.RawValue;
            fpVector3_2.Z.RawValue = p1.Z.RawValue - p0.Z.RawValue;
            FPVector3 fpVector3_3;
            fpVector3_3.X.RawValue = p2.X.RawValue - p0.X.RawValue;
            fpVector3_3.Y.RawValue = p2.Y.RawValue - p0.Y.RawValue;
            fpVector3_3.Z.RawValue = p2.Z.RawValue - p0.Z.RawValue;
            fpVector3_1.X.RawValue = p.X.RawValue - p0.X.RawValue;
            fpVector3_1.Y.RawValue = p.Y.RawValue - p0.Y.RawValue;
            fpVector3_1.Z.RawValue = p.Z.RawValue - p0.Z.RawValue;
            long num1 = (fpVector3_2.X.RawValue * fpVector3_2.X.RawValue + 32768L >> 16) + (fpVector3_2.Y.RawValue * fpVector3_2.Y.RawValue + 32768L >> 16) + (fpVector3_2.Z.RawValue * fpVector3_2.Z.RawValue + 32768L >> 16);
            long num2 = (fpVector3_2.X.RawValue * fpVector3_3.X.RawValue + 32768L >> 16) + (fpVector3_2.Y.RawValue * fpVector3_3.Y.RawValue + 32768L >> 16) + (fpVector3_2.Z.RawValue * fpVector3_3.Z.RawValue + 32768L >> 16);
            long num3 = (fpVector3_3.X.RawValue * fpVector3_3.X.RawValue + 32768L >> 16) + (fpVector3_3.Y.RawValue * fpVector3_3.Y.RawValue + 32768L >> 16) + (fpVector3_3.Z.RawValue * fpVector3_3.Z.RawValue + 32768L >> 16);
            long num4 = (fpVector3_1.X.RawValue * fpVector3_2.X.RawValue + 32768L >> 16) + (fpVector3_1.Y.RawValue * fpVector3_2.Y.RawValue + 32768L >> 16) + (fpVector3_1.Z.RawValue * fpVector3_2.Z.RawValue + 32768L >> 16);
            long num5 = (fpVector3_1.X.RawValue * fpVector3_3.X.RawValue + 32768L >> 16) + (fpVector3_1.Y.RawValue * fpVector3_3.Y.RawValue + 32768L >> 16) + (fpVector3_1.Z.RawValue * fpVector3_3.Z.RawValue + 32768L >> 16);
            long num6 = (num1 * num3 + 32768L >> 16) - (num2 * num2 + 32768L >> 16);
            if (num6 < 0L)
                return false;
            float num7 = (float)(num2 * num5 - num3 * num4);
            float num8 = (float)(num2 * num4 - num1 * num5);
            v.RawValue = ((num3 * num4 + 32768L >> 16) - (num2 * num5 + 32768L >> 16) << 16) / num6;
            w.RawValue = ((num1 * num5 + 32768L >> 16) - (num2 * num4 + 32768L >> 16) << 16) / num6;
            u.RawValue = FP._1.RawValue - v.RawValue - w.RawValue;
            return true;
        }

        /// <summary>
        ///     Returns <see langword="true" /> if two vectors are exactly equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FPVector3 a, FPVector3 b) => a.X.RawValue == b.X.RawValue && a.Y.RawValue == b.Y.RawValue && a.Z.RawValue == b.Z.RawValue;

        /// <summary>
        ///     Returns <see langword="true" /> if two vectors are not exactly equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FPVector3 a, FPVector3 b) => a.X.RawValue != b.X.RawValue || a.Y.RawValue != b.Y.RawValue || a.Z.RawValue != b.Z.RawValue;

        /// <summary>
        ///     Negates each component of <paramref name="v" /> vector.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator -(FPVector3 v)
        {
            v.X.RawValue = -v.X.RawValue;
            v.Y.RawValue = -v.Y.RawValue;
            v.Z.RawValue = -v.Z.RawValue;
            return v;
        }

        /// <summary>
        ///     Multiplies each component of <paramref name="v" /> times <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator *(FPVector3 v, FP s)
        {
            v.X.RawValue = v.X.RawValue * s.RawValue + 32768L >> 16;
            v.Y.RawValue = v.Y.RawValue * s.RawValue + 32768L >> 16;
            v.Z.RawValue = v.Z.RawValue * s.RawValue + 32768L >> 16;
            return v;
        }

        /// <summary>
        ///     Multiplies each component of <paramref name="v" /> times <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator *(FP s, FPVector3 v)
        {
            v.X.RawValue = v.X.RawValue * s.RawValue + 32768L >> 16;
            v.Y.RawValue = v.Y.RawValue * s.RawValue + 32768L >> 16;
            v.Z.RawValue = v.Z.RawValue * s.RawValue + 32768L >> 16;
            return v;
        }

        /// <summary>
        ///     Divides each component of <paramref name="v" /> by <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator /(FPVector3 v, FP s)
        {
            v.X.RawValue = (v.X.RawValue << 16) / s.RawValue;
            v.Y.RawValue = (v.Y.RawValue << 16) / s.RawValue;
            v.Z.RawValue = (v.Z.RawValue << 16) / s.RawValue;
            return v;
        }

        /// <summary>
        ///     Divides each component of <paramref name="v" /> by <paramref name="s" />.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator /(FPVector3 v, int s)
        {
            v.X.RawValue /= (long)s;
            v.Y.RawValue /= (long)s;
            v.Z.RawValue /= (long)s;
            return v;
        }

        /// <summary>
        ///     Subtracts <paramref name="b" /> from <paramref name="a" />
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator -(FPVector3 a, FPVector3 b)
        {
            a.X.RawValue -= b.X.RawValue;
            a.Y.RawValue -= b.Y.RawValue;
            a.Z.RawValue -= b.Z.RawValue;
            return a;
        }

        /// <summary>Adds two vectors.</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator +(FPVector3 a, FPVector3 b)
        {
            a.X.RawValue += b.X.RawValue;
            a.Y.RawValue += b.Y.RawValue;
            a.Z.RawValue += b.Z.RawValue;
            return a;
        }

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

        public FPVector3 XXZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xxz;
                xxz.X = this.X;
                xxz.Y = this.X;
                xxz.Z = this.Z;
                return xxz;
            }
        }

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

        public FPVector3 XYZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xyz;
                xyz.X = this.X;
                xyz.Y = this.Y;
                xyz.Z = this.Z;
                return xyz;
            }
        }

        public FPVector3 XZX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xzx;
                xzx.X = this.X;
                xzx.Y = this.Z;
                xzx.Z = this.X;
                return xzx;
            }
        }

        public FPVector3 XZY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xzy;
                xzy.X = this.X;
                xzy.Y = this.Z;
                xzy.Z = this.Y;
                return xzy;
            }
        }

        public FPVector3 XZZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xzz;
                xzz.X = this.X;
                xzz.Y = this.Z;
                xzz.Z = this.Z;
                return xzz;
            }
        }

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

        public FPVector2 XZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector2 xz;
                xz.X = this.X;
                xz.Y = this.Z;
                return xz;
            }
        }

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

        public FPVector3 YYZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 yyz;
                yyz.X = this.Y;
                yyz.Y = this.Y;
                yyz.Z = this.Z;
                return yyz;
            }
        }

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

        public FPVector3 YZY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 yzy;
                yzy.X = this.Y;
                yzy.Y = this.Z;
                yzy.Z = this.Y;
                return yzy;
            }
        }

        public FPVector3 YZZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 yzz;
                yzz.X = this.Y;
                yzz.Y = this.Z;
                yzz.Z = this.Z;
                return yzz;
            }
        }

        public FPVector3 YZX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 yzx;
                yzx.X = this.Y;
                yzx.Y = this.Z;
                yzx.Z = this.X;
                return yzx;
            }
        }

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

        public FPVector3 YXZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 yxz;
                yxz.X = this.Y;
                yxz.Y = this.X;
                yxz.Z = this.Z;
                return yxz;
            }
        }

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

        public FPVector2 YZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector2 yz;
                yz.X = this.Y;
                yz.Y = this.Z;
                return yz;
            }
        }

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

        public FPVector3 ZZZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 zzz;
                zzz.X = this.Z;
                zzz.Y = this.Z;
                zzz.Z = this.Z;
                return zzz;
            }
        }

        public FPVector3 ZZX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 zzx;
                zzx.X = this.Z;
                zzx.Y = this.Z;
                zzx.Z = this.X;
                return zzx;
            }
        }

        public FPVector3 ZZY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 zzy;
                zzy.X = this.Z;
                zzy.Y = this.Z;
                zzy.Z = this.Y;
                return zzy;
            }
        }

        public FPVector3 ZXZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 zxz;
                zxz.X = this.Z;
                zxz.Y = this.X;
                zxz.Z = this.Z;
                return zxz;
            }
        }

        public FPVector3 ZXX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 zxx;
                zxx.X = this.Z;
                zxx.Y = this.X;
                zxx.Z = this.X;
                return zxx;
            }
        }

        public FPVector3 ZXY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 zxy;
                zxy.X = this.Z;
                zxy.Y = this.X;
                zxy.Z = this.Y;
                return zxy;
            }
        }

        public FPVector3 ZYZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 zyz;
                zyz.X = this.Z;
                zyz.Y = this.Y;
                zyz.Z = this.Z;
                return zyz;
            }
        }

        public FPVector3 ZYX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 zyx;
                zyx.X = this.Z;
                zyx.Y = this.Y;
                zyx.Z = this.X;
                return zyx;
            }
        }

        public FPVector3 ZYY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 zyy;
                zyy.X = this.Z;
                zyy.Y = this.Y;
                zyy.Z = this.Y;
                return zyy;
            }
        }

        public FPVector3 XYO
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xyo;
                xyo.X = this.X;
                xyo.Y = this.Y;
                xyo.Z = new FP();
                return xyo;
            }
        }

        public FPVector3 XOZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 xoz;
                xoz.X = this.X;
                xoz.Y = new FP();
                xoz.Z = this.Z;
                return xoz;
            }
        }

        public FPVector3 OYZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector3 oyz;
                oyz.X = new FP();
                oyz.Y = this.Y;
                oyz.Z = this.Z;
                return oyz;
            }
        }

        public FPVector2 ZZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector2 zz;
                zz.X = this.Z;
                zz.Y = this.Z;
                return zz;
            }
        }

        public FPVector2 ZX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector2 zx;
                zx.X = this.Z;
                zx.Y = this.X;
                return zx;
            }
        }

        public FPVector2 ZY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FPVector2 zy;
                zy.X = this.Z;
                zy.Y = this.Y;
                return zy;
            }
        }

        /// <summary>
        ///     Represents an equality comparer for FPVector3 objects.
        /// </summary>
        public class EqualityComparer : IEqualityComparer<FPVector3>
        {
            public static readonly FPVector3.EqualityComparer Instance = new FPVector3.EqualityComparer();

            private EqualityComparer()
            {
            }

            bool IEqualityComparer<FPVector3>.Equals(FPVector3 x, FPVector3 y) => x == y;

            int IEqualityComparer<FPVector3>.GetHashCode(FPVector3 obj) => obj.GetHashCode();
        }
    }
}