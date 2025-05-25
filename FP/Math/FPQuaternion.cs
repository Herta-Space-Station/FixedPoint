using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>A Quaternion representing an orientation.</summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FPQuaternion
    {
        /// <summary>
        ///     The size of the struct in-memory inside the Frame data-buffers or stack (when passed as value parameter).
        ///     Not related to the snapshot payload this occupies, which is bit-packed and compressed.
        /// </summary>
        public const int SIZE = 32;

        /// <summary>The X component of the quaternion.</summary>
        [FieldOffset(0)] public FP X;

        /// <summary>The Y component of the quaternion.</summary>
        [FieldOffset(8)] public FP Y;

        /// <summary>The Z component of the quaternion.</summary>
        [FieldOffset(16)] public FP Z;

        /// <summary>The W component of the quaternion.</summary>
        [FieldOffset(24)] public FP W;

        /// <summary>Quaternion corresponding to "no rotation".</summary>
        public static readonly FPQuaternion Identity = new FPQuaternion()
        {
            W =
            {
                RawValue = 65536
            }
        };

        /// <summary>
        ///     Returns this quaternion with magnitude of 1. Most API functions expect and return normalized quaternions,
        ///     so unless components get set manually, there should not be a need to normalize quaternions
        /// </summary>
        /// <seealso cref="M:Herta.FPQuaternion.Normalize(Herta.FPQuaternion)" />
        public FPQuaternion Normalized => FPQuaternion.Normalize(this);

        /// <summary>
        ///     Creates this quaternion's inverse. If this quaternion is normalized, use
        ///     <see cref="P:Herta.FPQuaternion.Conjugated" /> instead.
        /// </summary>
        /// <seealso cref="M:Herta.FPQuaternion.Inverse(Herta.FPQuaternion)" />
        public FPQuaternion Inverted => FPQuaternion.Inverse(this);

        /// <summary>
        ///     Creates this quaternion's conjugate. For normalized quaternions this property represents inverse rotation
        ///     and should be used instead of <see cref="P:Herta.FPQuaternion.Inverted" />
        /// </summary>
        /// <seealso cref="M:Herta.FPQuaternion.Conjugate(Herta.FPQuaternion)" />
        public FPQuaternion Conjugated => FPQuaternion.Conjugate(this);

        private long MagnitudeSqrRaw => (this.X.RawValue * this.X.RawValue + 32768L >> 16) + (this.Y.RawValue * this.Y.RawValue + 32768L >> 16) + (this.Z.RawValue * this.Z.RawValue + 32768L >> 16) + (this.W.RawValue * this.W.RawValue + 32768L >> 16);

        /// <summary>Returns square of this quaternion's magnitude.</summary>
        public FP MagnitudeSqr => FP.FromRaw(this.MagnitudeSqrRaw);

        /// <summary>Return this quaternion's magnitude.</summary>
        public FP Magnitude => FP.FromRaw(FPMath.SqrtRaw(this.MagnitudeSqrRaw));

        /// <summary>
        ///     Returns one of possible Euler angles representation, where rotations are performed around the Z axis, the X axis,
        ///     and the Y axis, in that order.
        /// </summary>
        public FPVector3 AsEuler => FPQuaternion.ToEulerZXY(this);

        /// <summary>Creates a new instance of FPQuaternion</summary>
        /// <param name="x">X component.</param>
        /// <param name="y">Y component.</param>
        /// <param name="z">Z component.</param>
        /// <param name="w">W component.</param>
        public FPQuaternion(FP x, FP y, FP z, FP w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        ///     Returns a string representation of the FPQuaternion in the format (X, Y, Z, W).
        /// </summary>
        /// <returns>A string representation of the FPQuaternion.</returns>
        public override string ToString()
        {
            NativeString builder = new NativeString(stackalloc char[128], 0);
            Format(ref builder);

            return builder.ToString();
        }

        public bool TryFormat(Span<char> destination, out int charsWritten)
        {
            NativeString builder = new NativeString(stackalloc char[128], 0);
            Format(ref builder);

            bool result = builder.TryCopyTo(destination);
            charsWritten = result ? builder.Length : 0;
            return result;
        }

        private void Format(ref NativeString builder)
        {
            builder.Append('(');
            builder.AppendFormattable(this.X.AsFloat, "f1", (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.Y.AsFloat, "f1", (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.Z.AsFloat, "f1", (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.W.AsFloat, "f1", (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(')');
        }

        /// <summary>
        ///     Returns a hash code for the current FPQuaternion object.
        /// </summary>
        /// <returns>A hash code for the current FPQuaternion object.</returns>
        public override int GetHashCode() => XxHash.Hash32(this);

        /// <summary>
        ///     Creates product of two quaternions. Can be used to combine two rotations. Just like
        ///     in the case of <see cref="T:Herta.FPMatrix4x4" /> the righmost operand gets applied first.
        ///     This method computes the equivalent to the following pseduo-code:
        ///     <code>
        /// FPQuaternion result;
        /// result.x = (left.w * right.x) + (left.x * right.w) + (left.y * right.z) - (left.z * right.y);
        /// result.y = (left.w * right.y) - (left.x * right.z) + (left.y * right.w) + (left.z * right.x);
        /// result.z = (left.w * right.z) + (left.x * right.y) - (left.y * right.x) + (left.z * right.w);
        /// result.w = (left.w * right.w) - (left.x * right.x) - (left.y * right.y) - (left.z * right.z);
        /// return result;
        /// </code>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static FPQuaternion Product(FPQuaternion left, FPQuaternion right)
        {
            long rawValue1 = left.X.RawValue;
            long rawValue2 = left.Y.RawValue;
            long rawValue3 = left.Z.RawValue;
            long rawValue4 = left.W.RawValue;
            long rawValue5 = right.X.RawValue;
            long rawValue6 = right.Y.RawValue;
            long rawValue7 = right.Z.RawValue;
            long rawValue8 = right.W.RawValue;
            FPQuaternion fpQuaternion;
            fpQuaternion.X.RawValue = (rawValue4 * rawValue5 + 32768L >> 16) + (rawValue1 * rawValue8 + 32768L >> 16) + (rawValue2 * rawValue7 + 32768L >> 16) - (rawValue3 * rawValue6 + 32768L >> 16);
            fpQuaternion.Y.RawValue = (rawValue4 * rawValue6 + 32768L >> 16) + (rawValue2 * rawValue8 + 32768L >> 16) + (rawValue3 * rawValue5 + 32768L >> 16) - (rawValue1 * rawValue7 + 32768L >> 16);
            fpQuaternion.Z.RawValue = (rawValue4 * rawValue7 + 32768L >> 16) + (rawValue3 * rawValue8 + 32768L >> 16) + (rawValue1 * rawValue6 + 32768L >> 16) - (rawValue2 * rawValue5 + 32768L >> 16);
            fpQuaternion.W.RawValue = (rawValue4 * rawValue8 + 32768L >> 16) - (rawValue1 * rawValue5 + 32768L >> 16) - (rawValue2 * rawValue6 + 32768L >> 16) - (rawValue3 * rawValue7 + 32768L >> 16);
            return fpQuaternion;
        }

        /// <summary>
        ///     Returns conjugate quaternion. This method computes the equivalent to the following pseduo-code:
        ///     <code>
        /// return new FPQuaternion(-value.X, -value.Y, -value.Z, value.W);
        /// </code>
        ///     Conjugate can be used instead of an inverse quaterion if <paramref name="value" /> is normalized.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FPQuaternion Conjugate(FPQuaternion value)
        {
            value.X.RawValue = -value.X.RawValue;
            value.Y.RawValue = -value.Y.RawValue;
            value.Z.RawValue = -value.Z.RawValue;
            return value;
        }

        /// <summary>Checks if the quaternion is the identity quaternion</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdentity(FPQuaternion value) => value.X.RawValue == 0L && value.Y.RawValue == 0L && value.Z.RawValue == 0L && value.W.RawValue == 65536L;

        /// <summary>
        ///     Checks if the quaternion is the invalid zero quaternion
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(FPQuaternion value) => value.X.RawValue == 0L && value.Y.RawValue == 0L && value.Z.RawValue == 0L && value.W.RawValue == 0L;

        /// <summary>
        ///     Returns the dot product between two rotations. This method computes the equivalent to the following pseduo-code:
        ///     <code>
        /// return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        /// </code>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FP Dot(FPQuaternion a, FPQuaternion b)
        {
            FP fp;
            fp.RawValue = (a.W.RawValue * b.W.RawValue + 32768L >> 16) + (a.X.RawValue * b.X.RawValue + 32768L >> 16) + (a.Y.RawValue * b.Y.RawValue + 32768L >> 16) + (a.Z.RawValue * b.Z.RawValue + 32768L >> 16);
            return fp;
        }

        /// <summary>
        ///     Creates a quaternion which rotates from <paramref name="fromVector" /> to <paramref name="toVector" /> (normalized
        ///     internally).
        ///     If these vectors are known to be normalized or have magnitude close to 1,
        ///     <see
        ///         cref="M:Herta.FPQuaternion.FromToRotationSkipNormalize(Herta.FPVector3,Herta.FPVector3)" />
        ///     can be used for better performance.
        /// </summary>
        /// <param name="fromVector"></param>
        /// <param name="toVector"></param>
        /// <returns></returns>
        public static FPQuaternion FromToRotation(FPVector3 fromVector, FPVector3 toVector)
        {
            long x1 = (fromVector.X.RawValue * fromVector.X.RawValue + 32768L >> 16) + (fromVector.Y.RawValue * fromVector.Y.RawValue + 32768L >> 16) + (fromVector.Z.RawValue * fromVector.Z.RawValue + 32768L >> 16);
            if (x1 == 0L)
                return FPQuaternion.Identity;
            long num1 = 4294967296L / FPMath.SqrtRaw(x1);
            fromVector.X.RawValue = fromVector.X.RawValue * num1 + 32768L >> 16;
            fromVector.Y.RawValue = fromVector.Y.RawValue * num1 + 32768L >> 16;
            fromVector.Z.RawValue = fromVector.Z.RawValue * num1 + 32768L >> 16;
            long x2 = (toVector.X.RawValue * toVector.X.RawValue + 32768L >> 16) + (toVector.Y.RawValue * toVector.Y.RawValue + 32768L >> 16) + (toVector.Z.RawValue * toVector.Z.RawValue + 32768L >> 16);
            if (x2 == 0L)
                return FPQuaternion.Identity;
            long num2 = 4294967296L / FPMath.SqrtRaw(x2);
            toVector.X.RawValue = toVector.X.RawValue * num2 + 32768L >> 16;
            toVector.Y.RawValue = toVector.Y.RawValue * num2 + 32768L >> 16;
            toVector.Z.RawValue = toVector.Z.RawValue * num2 + 32768L >> 16;
            FPQuaternion fpQuaternion;
            fpQuaternion.X.RawValue = (fromVector.Y.RawValue * toVector.Z.RawValue + 32768L >> 16) - (fromVector.Z.RawValue * toVector.Y.RawValue + 32768L >> 16);
            fpQuaternion.Y.RawValue = (fromVector.Z.RawValue * toVector.X.RawValue + 32768L >> 16) - (fromVector.X.RawValue * toVector.Z.RawValue + 32768L >> 16);
            fpQuaternion.Z.RawValue = (fromVector.X.RawValue * toVector.Y.RawValue + 32768L >> 16) - (fromVector.Y.RawValue * toVector.X.RawValue + 32768L >> 16);
            fpQuaternion.W.RawValue = 65536L + (fromVector.X.RawValue * toVector.X.RawValue + 32768L >> 16) + (fromVector.Y.RawValue * toVector.Y.RawValue + 32768L >> 16) + (fromVector.Z.RawValue * toVector.Z.RawValue + 32768L >> 16);
            return FPQuaternion.NormalizeSmall(fpQuaternion);
        }

        /// <summary>
        ///     Creates a quaternion which rotates from <paramref name="fromVector" /> to <paramref name="toVector" /> (not
        ///     normalized internally).
        ///     If these vectors are known to be normalized or have magnitude close to 1, use
        ///     <see
        ///         cref="M:Herta.FPQuaternion.FromToRotation(Herta.FPVector3,Herta.FPVector3)" />
        ///     instead.
        /// </summary>
        /// <param name="fromVector"></param>
        /// <param name="toVector"></param>
        /// <returns></returns>
        public static FPQuaternion FromToRotationSkipNormalize(
            FPVector3 fromVector,
            FPVector3 toVector)
        {
            FPQuaternion fpQuaternion;
            fpQuaternion.X.RawValue = (fromVector.Y.RawValue * toVector.Z.RawValue + 32768L >> 16) - (fromVector.Z.RawValue * toVector.Y.RawValue + 32768L >> 16);
            fpQuaternion.Y.RawValue = (fromVector.Z.RawValue * toVector.X.RawValue + 32768L >> 16) - (fromVector.X.RawValue * toVector.Z.RawValue + 32768L >> 16);
            fpQuaternion.Z.RawValue = (fromVector.X.RawValue * toVector.Y.RawValue + 32768L >> 16) - (fromVector.Y.RawValue * toVector.X.RawValue + 32768L >> 16);
            fpQuaternion.W.RawValue = (fromVector.X.RawValue * toVector.X.RawValue + 32768L >> 16) + (fromVector.Y.RawValue * toVector.Y.RawValue + 32768L >> 16) + (fromVector.Z.RawValue * toVector.Z.RawValue + 32768L >> 16);
            long x = ((fromVector.X.RawValue * fromVector.X.RawValue + 32768L >> 16) + (fromVector.Y.RawValue * fromVector.Y.RawValue + 32768L >> 16) + (fromVector.Z.RawValue * fromVector.Z.RawValue + 32768L >> 16)) * ((toVector.X.RawValue * toVector.X.RawValue + 32768L >> 16) + (toVector.Y.RawValue * toVector.Y.RawValue + 32768L >> 16) + (toVector.Z.RawValue * toVector.Z.RawValue + 32768L >> 16)) >> 16;
            fpQuaternion.W.RawValue += FPMath.SqrtRaw(x);
            return FPQuaternion.NormalizeSmall(fpQuaternion);
        }

        /// <summary>
        ///     Interpolates between <paramref name="a" /> and <paramref name="b" /> by <paramref name="t" /> and normalizes the
        ///     result afterwards. The parameter <paramref name="t" /> is clamped to the range [0, 1].
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPQuaternion Lerp(FPQuaternion a, FPQuaternion b, FP t)
        {
            if (t.RawValue < 0L)
                t.RawValue = 0L;
            else if (t.RawValue > 65536L)
                t.RawValue = 65536L;
            return FPQuaternion.LerpUnclamped(a, b, t);
        }

        /// <summary>
        ///     Interpolates between <paramref name="a" /> and <paramref name="b" /> by <paramref name="t" /> and normalizes the
        ///     result afterwards.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FPQuaternion LerpUnclamped(FPQuaternion a, FPQuaternion b, FP t)
        {
            FP fp;
            fp.RawValue = (a.W.RawValue * b.W.RawValue + 32768L >> 16) + (a.X.RawValue * b.X.RawValue + 32768L >> 16) + (a.Y.RawValue * b.Y.RawValue + 32768L >> 16) + (a.Z.RawValue * b.Z.RawValue + 32768L >> 16);
            if (fp.RawValue < 0L)
            {
                b.X.RawValue = -b.X.RawValue;
                b.Y.RawValue = -b.Y.RawValue;
                b.Z.RawValue = -b.Z.RawValue;
                b.W.RawValue = -b.W.RawValue;
                fp.RawValue = -fp.RawValue;
            }

            long num = 65536L - t.RawValue;
            a.X.RawValue = a.X.RawValue * num + 32768L >> 16;
            a.Y.RawValue = a.Y.RawValue * num + 32768L >> 16;
            a.Z.RawValue = a.Z.RawValue * num + 32768L >> 16;
            a.W.RawValue = a.W.RawValue * num + 32768L >> 16;
            b.X.RawValue = b.X.RawValue * t.RawValue + 32768L >> 16;
            b.Y.RawValue = b.Y.RawValue * t.RawValue + 32768L >> 16;
            b.Z.RawValue = b.Z.RawValue * t.RawValue + 32768L >> 16;
            b.W.RawValue = b.W.RawValue * t.RawValue + 32768L >> 16;
            a.X.RawValue += b.X.RawValue;
            a.Y.RawValue += b.Y.RawValue;
            a.Z.RawValue += b.Z.RawValue;
            a.W.RawValue += b.W.RawValue;
            return FPQuaternion.Normalize(a);
        }

        /// <summary>
        ///     Returns a rotation that rotates <paramref name="roll" /> radians around the z axis, <paramref name="pitch" />
        ///     radians around the x axis, and <paramref name="yaw" /> radians around the y axis.
        /// </summary>
        /// <param name="yaw">Yaw in radians</param>
        /// <param name="pitch">Pitch in radians</param>
        /// <param name="roll">Roll in radians</param>
        /// <returns></returns>
        public static FPQuaternion CreateFromYawPitchRoll(FP yaw, FP pitch, FP roll)
        {
            FP rad1;
            rad1.RawValue = roll.RawValue / 2L;
            FP rad2;
            rad2.RawValue = pitch.RawValue / 2L;
            FP rad3;
            rad3.RawValue = yaw.RawValue / 2L;
            long sinRaw1;
            long cosRaw1;
            FPMath.SinCosRaw(rad1, out sinRaw1, out cosRaw1);
            long sinRaw2;
            long cosRaw2;
            FPMath.SinCosRaw(rad2, out sinRaw2, out cosRaw2);
            long sinRaw3;
            long cosRaw3;
            FPMath.SinCosRaw(rad3, out sinRaw3, out cosRaw3);
            FPQuaternion fromYawPitchRoll;
            fromYawPitchRoll.X.RawValue = ((cosRaw3 * sinRaw2 + 32768L >> 16) * cosRaw1 + 32768L >> 16) + ((sinRaw3 * cosRaw2 + 32768L >> 16) * sinRaw1 + 32768L >> 16);
            fromYawPitchRoll.Y.RawValue = ((sinRaw3 * cosRaw2 + 32768L >> 16) * cosRaw1 + 32768L >> 16) - ((cosRaw3 * sinRaw2 + 32768L >> 16) * sinRaw1 + 32768L >> 16);
            fromYawPitchRoll.Z.RawValue = ((cosRaw3 * cosRaw2 + 32768L >> 16) * sinRaw1 + 32768L >> 16) - ((sinRaw3 * sinRaw2 + 32768L >> 16) * cosRaw1 + 32768L >> 16);
            fromYawPitchRoll.W.RawValue = ((cosRaw3 * cosRaw2 + 32768L >> 16) * cosRaw1 + 32768L >> 16) + ((sinRaw3 * sinRaw2 + 32768L >> 16) * sinRaw1 + 32768L >> 16);
            return fromYawPitchRoll;
        }

        /// <summary>
        ///     Returns the angle in degrees between two rotations <paramref name="a" /> and <paramref name="b" />.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Angle(FPQuaternion a, FPQuaternion b) => FP.FromRaw(FPQuaternion.AngleRadians(a, b).RawValue * 3754936L + 32768L >> 16);

        /// <summary>
        ///     Returns the angle in radians between two rotations <paramref name="a" /> and <paramref name="b" />.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FP AngleRadians(FPQuaternion a, FPQuaternion b)
        {
            FP fp1;
            fp1.RawValue = (a.X.RawValue * a.X.RawValue + 32768L >> 16) + (a.Y.RawValue * a.Y.RawValue + 32768L >> 16) + (a.Z.RawValue * a.Z.RawValue + 32768L >> 16) + (a.W.RawValue * a.W.RawValue + 32768L >> 16);
            long num = 4294967296L / fp1.RawValue;
            a.X.RawValue = -a.X.RawValue;
            a.Y.RawValue = -a.Y.RawValue;
            a.Z.RawValue = -a.Z.RawValue;
            a.X.RawValue = a.X.RawValue * num + 32768L >> 16;
            a.Y.RawValue = a.Y.RawValue * num + 32768L >> 16;
            a.Z.RawValue = a.Z.RawValue * num + 32768L >> 16;
            a.W.RawValue = a.W.RawValue * num + 32768L >> 16;
            long rawValue1 = b.X.RawValue;
            long rawValue2 = b.Y.RawValue;
            long rawValue3 = b.Z.RawValue;
            long rawValue4 = b.W.RawValue;
            long rawValue5 = a.X.RawValue;
            long rawValue6 = a.Y.RawValue;
            long rawValue7 = a.Z.RawValue;
            long rawValue8 = a.W.RawValue;
            FPQuaternion fpQuaternion;
            fpQuaternion.X.RawValue = (rawValue5 * rawValue4 + 32768L >> 16) + (rawValue1 * rawValue8 + 32768L >> 16) + (rawValue6 * rawValue3 + 32768L >> 16) - (rawValue7 * rawValue2 + 32768L >> 16);
            fpQuaternion.Y.RawValue = (rawValue6 * rawValue4 + 32768L >> 16) + (rawValue2 * rawValue8 + 32768L >> 16) + (rawValue7 * rawValue1 + 32768L >> 16) - (rawValue5 * rawValue3 + 32768L >> 16);
            fpQuaternion.Z.RawValue = (rawValue7 * rawValue4 + 32768L >> 16) + (rawValue3 * rawValue8 + 32768L >> 16) + (rawValue5 * rawValue2 + 32768L >> 16) - (rawValue6 * rawValue1 + 32768L >> 16);
            fpQuaternion.W.RawValue = (rawValue8 * rawValue4 + 32768L >> 16) - ((rawValue5 * rawValue1 + 32768L >> 16) + (rawValue6 * rawValue2 + 32768L >> 16) + (rawValue7 * rawValue3 + 32768L >> 16));
            FP fp2;
            fp2.RawValue = FPMath.Acos(fpQuaternion.W).RawValue << 1;
            if (fp2.RawValue > 205887L)
                fp2.RawValue = 411775L - fp2.RawValue;
            return fp2;
        }

        /// <summary>
        ///     Obsolete. Use one of the overloads that receive either only a forward direction (uses FPVector3.Up as up direction,
        ///     not ortho-normalized)
        ///     OR forward and up directions, which can be optionally ortho-normalized.
        /// </summary>
        [Obsolete("Use one of the overloads that receive either only a forward direction (uses FPVector3.Up as up direction, not ortho-normalized), OR forward and up directions, which can be optionally ortho-normalized.")]
        public static FPQuaternion LookRotation(FPVector3 forward, bool orthoNormalize) => FPQuaternion.LookRotation(forward);

        /// <summary>
        ///     Creates a rotation with the specified <paramref name="forward" /> direction and
        ///     <see cref="P:Herta.FPVector3.Up" />.
        /// </summary>
        /// <param name="forward"></param>
        /// <returns></returns>
        public static FPQuaternion LookRotation(FPVector3 forward) => FPQuaternion.LookRotation(forward, FPVector3.Up);

        /// <summary>
        ///     Creates a rotation with the specified <paramref name="forward" /> and <paramref name="up" /> directions.
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <param name="orthoNormalize"></param>
        /// <returns></returns>
        public static FPQuaternion LookRotation(
            FPVector3 forward,
            FPVector3 up,
            bool orthoNormalize = false)
        {
            forward = forward.Normalized;
            if (orthoNormalize)
            {
                up -= forward * FPVector3.Dot(up, forward);
                up = up.Normalized;
            }

            FPVector3 normalized = FPVector3.Cross(up, forward).Normalized;
            if (!orthoNormalize)
                up = FPVector3.Cross(forward, normalized);
            FP fp1 = normalized.X + up.Y + forward.Z;
            FPQuaternion fpQuaternion = new FPQuaternion();
            if (fp1.RawValue > 0L)
            {
                FP fp2 = FPMath.Sqrt(fp1 + FP._1);
                fpQuaternion.W = fp2 * FP._0_50;
                FP fp3 = FP._0_50 / fp2;
                fpQuaternion.X = (up.Z - forward.Y) * fp3;
                fpQuaternion.Y = (forward.X - normalized.Z) * fp3;
                fpQuaternion.Z = (normalized.Y - up.X) * fp3;
                return fpQuaternion;
            }

            if (normalized.X >= up.Y && normalized.X >= forward.Z)
            {
                FP fp4 = FPMath.Sqrt(FP._1 + normalized.X - up.Y - forward.Z);
                FP fp5 = FP._0_50 / fp4;
                fpQuaternion.X = FP._0_50 * fp4;
                fpQuaternion.Y = (normalized.Y + up.X) * fp5;
                fpQuaternion.Z = (normalized.Z + forward.X) * fp5;
                fpQuaternion.W = (up.Z - forward.Y) * fp5;
                return fpQuaternion;
            }

            if (up.Y > forward.Z)
            {
                FP fp6 = FPMath.Sqrt(FP._1 + up.Y - normalized.X - forward.Z);
                FP fp7 = FP._0_50 / fp6;
                fpQuaternion.X = (up.X + normalized.Y) * fp7;
                fpQuaternion.Y = FP._0_50 * fp6;
                fpQuaternion.Z = (forward.Y + up.Z) * fp7;
                fpQuaternion.W = (forward.X - normalized.Z) * fp7;
                return fpQuaternion;
            }

            FP fp8 = FPMath.Sqrt(FP._1 + forward.Z - normalized.X - up.Y);
            FP fp9 = FP._0_50 / fp8;
            fpQuaternion.X = (forward.X + normalized.Z) * fp9;
            fpQuaternion.Y = (forward.Y + up.Z) * fp9;
            fpQuaternion.Z = FP._0_50 * fp8;
            fpQuaternion.W = (normalized.Y - up.X) * fp9;
            return fpQuaternion;
        }

        [Obsolete("SimpleLookAt is a cheaper version of LookRotation, but there are no extensive tests to ensure its correctness and equivalency to Unity's Quaternion.LookRotation as the latter has. We recommend using LookRotation instead.")]
        public static FPQuaternion SimpleLookAt(FPVector3 direction) => FPQuaternion.SimpleLookAt(direction, FPVector3.Forward, FPVector3.Up);

        [Obsolete("SimpleLookAt is a cheaper version of LookRotation, but there are no extensive tests to ensure its correctness and equivalency to Unity's Quaternion.LookRotation as the latter has. We recommend using LookRotation instead.")]
        public static FPQuaternion SimpleLookAt(FPVector3 direction, FPVector3 up) => FPQuaternion.SimpleLookAt(direction, FPVector3.Forward, up);

        [Obsolete("SimpleLookAt is a cheaper version of LookRotation, but there are no extensive tests to ensure its correctness and equivalency to Unity's Quaternion.LookRotation as the latter has. We recommend using LookRotation instead.")]
        public static FPQuaternion SimpleLookAt(
            FPVector3 direction,
            FPVector3 forward,
            FPVector3 up)
        {
            FPVector3 axis = FPVector3.Cross(forward, direction);
            if (axis.SqrMagnitude < FP.EN4)
                axis = up;
            return FPQuaternion.AngleAxis(FPMath.Acos(FPVector3.Dot(forward, direction)), axis);
        }

        /// <summary>
        ///     Spherically interpolates between <paramref name="from" /> and <paramref name="to" /> by <paramref name="t" /> and
        ///     normalizes the result afterwards. <paramref name="t" /> is clamped to the range [0, 1].
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPQuaternion Slerp(FPQuaternion from, FPQuaternion to, FP t)
        {
            if (t.RawValue < 0L)
                t.RawValue = 0L;
            else if (t.RawValue > 65536L)
                t.RawValue = 65536L;
            return FPQuaternion.SlerpUnclamped(from, to, t);
        }

        /// <summary>
        ///     Spherically interpolates between <paramref name="from" /> and <paramref name="to" /> by <paramref name="t" /> and
        ///     normalizes the result afterwards.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FPQuaternion SlerpUnclamped(FPQuaternion from, FPQuaternion to, FP t)
        {
            FP fp1;
            fp1.RawValue = (from.W.RawValue * to.W.RawValue + 32768L >> 16) + (from.X.RawValue * to.X.RawValue + 32768L >> 16) + (from.Y.RawValue * to.Y.RawValue + 32768L >> 16) + (from.Z.RawValue * to.Z.RawValue + 32768L >> 16);
            if (fp1.RawValue < 0L)
            {
                to.X.RawValue = -to.X.RawValue;
                to.Y.RawValue = -to.Y.RawValue;
                to.Z.RawValue = -to.Z.RawValue;
                to.W.RawValue = -to.W.RawValue;
                fp1.RawValue = -fp1.RawValue;
            }

            if (fp1.RawValue >= 64881L)
                return FPQuaternion.LerpUnclamped(from, to, t);
            FP rad1 = FPMath.Acos(fp1);
            FP rad2;
            rad2.RawValue = (65536L - t.RawValue) * rad1.RawValue + 32768L >> 16;
            FP rad3;
            rad3.RawValue = t.RawValue * rad1.RawValue + 32768L >> 16;
            FP fp2;
            fp2.RawValue = 4294967296L / FPMath.Sin(rad1).RawValue;
            rad2 = FPMath.Sin(rad2);
            FP fp3 = FPMath.Sin(rad3);
            from.X.RawValue = from.X.RawValue * rad2.RawValue + 32768L >> 16;
            from.Y.RawValue = from.Y.RawValue * rad2.RawValue + 32768L >> 16;
            from.Z.RawValue = from.Z.RawValue * rad2.RawValue + 32768L >> 16;
            from.W.RawValue = from.W.RawValue * rad2.RawValue + 32768L >> 16;
            to.X.RawValue = to.X.RawValue * fp3.RawValue + 32768L >> 16;
            to.Y.RawValue = to.Y.RawValue * fp3.RawValue + 32768L >> 16;
            to.Z.RawValue = to.Z.RawValue * fp3.RawValue + 32768L >> 16;
            to.W.RawValue = to.W.RawValue * fp3.RawValue + 32768L >> 16;
            from.X.RawValue += to.X.RawValue;
            from.Y.RawValue += to.Y.RawValue;
            from.Z.RawValue += to.Z.RawValue;
            from.W.RawValue += to.W.RawValue;
            from.X.RawValue = from.X.RawValue * fp2.RawValue + 32768L >> 16;
            from.Y.RawValue = from.Y.RawValue * fp2.RawValue + 32768L >> 16;
            from.Z.RawValue = from.Z.RawValue * fp2.RawValue + 32768L >> 16;
            from.W.RawValue = from.W.RawValue * fp2.RawValue + 32768L >> 16;
            return FPQuaternion.Normalize(from);
        }

        /// <summary>
        ///     Rotates a rotation <paramref name="from" /> towards <paramref name="to" /> by an angular step of
        ///     <paramref name="maxDegreesDelta" />.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDegreesDelta"></param>
        /// <returns></returns>
        public static FPQuaternion RotateTowards(
            FPQuaternion from,
            FPQuaternion to,
            FP maxDegreesDelta)
        {
            FP fp1;
            fp1.RawValue = (from.W.RawValue * to.W.RawValue + 32768L >> 16) + (from.X.RawValue * to.X.RawValue + 32768L >> 16) + (from.Y.RawValue * to.Y.RawValue + 32768L >> 16) + (from.Z.RawValue * to.Z.RawValue + 32768L >> 16);
            if (fp1.RawValue < 0L)
            {
                to.X.RawValue = -to.X.RawValue;
                to.Y.RawValue = -to.Y.RawValue;
                to.Z.RawValue = -to.Z.RawValue;
                to.W.RawValue = -to.W.RawValue;
                fp1.RawValue = -fp1.RawValue;
            }

            FP rad1 = FPMath.Acos(fp1);
            FP fp2;
            fp2.RawValue = rad1.RawValue << 1;
            maxDegreesDelta.RawValue = maxDegreesDelta.RawValue * FP.Deg2Rad.RawValue + 32768L >> 16;
            if (maxDegreesDelta.RawValue >= fp2.RawValue)
                return to;
            maxDegreesDelta.RawValue = (maxDegreesDelta.RawValue << 16) / fp2.RawValue;
            FP rad2 = (1 - maxDegreesDelta) * rad1;
            FP rad3 = maxDegreesDelta * rad1;
            FP fp3 = FPMath.Sin(rad2);
            FP fp4 = FPMath.Sin(rad3);
            from.X.RawValue = from.X.RawValue * fp3.RawValue + 32768L >> 16;
            from.Y.RawValue = from.Y.RawValue * fp3.RawValue + 32768L >> 16;
            from.Z.RawValue = from.Z.RawValue * fp3.RawValue + 32768L >> 16;
            from.W.RawValue = from.W.RawValue * fp3.RawValue + 32768L >> 16;
            to.X.RawValue = to.X.RawValue * fp4.RawValue + 32768L >> 16;
            to.Y.RawValue = to.Y.RawValue * fp4.RawValue + 32768L >> 16;
            to.Z.RawValue = to.Z.RawValue * fp4.RawValue + 32768L >> 16;
            to.W.RawValue = to.W.RawValue * fp4.RawValue + 32768L >> 16;
            fp3.RawValue = 4294967296L / FPMath.Sin(rad1).RawValue;
            from.X.RawValue += to.X.RawValue;
            from.Y.RawValue += to.Y.RawValue;
            from.Z.RawValue += to.Z.RawValue;
            from.W.RawValue += to.W.RawValue;
            from.X.RawValue = from.X.RawValue * fp3.RawValue + 32768L >> 16;
            from.Y.RawValue = from.Y.RawValue * fp3.RawValue + 32768L >> 16;
            from.Z.RawValue = from.Z.RawValue * fp3.RawValue + 32768L >> 16;
            from.W.RawValue = from.W.RawValue * fp3.RawValue + 32768L >> 16;
            return from;
        }

        /// <summary>
        ///     Returns a rotation that rotates <paramref name="z" /> degrees around the z axis, <paramref name="x" /> degrees
        ///     around the x axis, and <paramref name="y" /> degrees around the y axis.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static FPQuaternion Euler(FP x, FP y, FP z)
        {
            x.RawValue = x.RawValue * FP.Deg2Rad.RawValue + 32768L >> 16;
            y.RawValue = y.RawValue * FP.Deg2Rad.RawValue + 32768L >> 16;
            z.RawValue = z.RawValue * FP.Deg2Rad.RawValue + 32768L >> 16;
            return FPQuaternion.CreateFromYawPitchRoll(y, x, z);
        }

        /// <summary>
        ///     Returns a rotation that rotates <paramref name="eulerAngles" />.z degrees around the z axis,
        ///     <paramref name="eulerAngles" />.x degrees around the x axis, and <paramref name="eulerAngles" />.y degrees around
        ///     the y axis.
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        public static FPQuaternion Euler(FPVector3 eulerAngles)
        {
            eulerAngles.X.RawValue = eulerAngles.X.RawValue * FP.Deg2Rad.RawValue + 32768L >> 16;
            eulerAngles.Y.RawValue = eulerAngles.Y.RawValue * FP.Deg2Rad.RawValue + 32768L >> 16;
            eulerAngles.Z.RawValue = eulerAngles.Z.RawValue * FP.Deg2Rad.RawValue + 32768L >> 16;
            return FPQuaternion.CreateFromYawPitchRoll(eulerAngles.Y, eulerAngles.X, eulerAngles.Z);
        }

        /// <summary>
        ///     Creates a rotation which rotates <paramref name="angle" /> degrees around <paramref name="axis" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static FPQuaternion AngleAxis(FP angle, FPVector3 axis)
        {
            axis = FPVector3.Normalize(axis);
            FP rad;
            rad.RawValue = (angle.RawValue * FP.Deg2Rad.RawValue + 32768L >> 16) / 2L;
            FP sin;
            FP cos;
            FPMath.SinCos(rad, out sin, out cos);
            FPQuaternion fpQuaternion;
            fpQuaternion.X.RawValue = axis.X.RawValue * sin.RawValue + 32768L >> 16;
            fpQuaternion.Y.RawValue = axis.Y.RawValue * sin.RawValue + 32768L >> 16;
            fpQuaternion.Z.RawValue = axis.Z.RawValue * sin.RawValue + 32768L >> 16;
            fpQuaternion.W = cos;
            return fpQuaternion;
        }

        /// <summary>
        ///     Creates a rotation which rotates <paramref name="radians" /> radians around <paramref name="axis" />.
        /// </summary>
        /// <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// <param name="radians"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static FPQuaternion RadianAxis(FP radians, FPVector3 axis)
        {
            axis = FPVector3.Normalize(axis);
            FP rad;
            rad.RawValue = radians.RawValue / 2L;
            FP sin;
            FP cos;
            FPMath.SinCos(rad, out sin, out cos);
            FPQuaternion fpQuaternion;
            fpQuaternion.X.RawValue = axis.X.RawValue * sin.RawValue + 32768L >> 16;
            fpQuaternion.Y.RawValue = axis.Y.RawValue * sin.RawValue + 32768L >> 16;
            fpQuaternion.Z.RawValue = axis.Z.RawValue * sin.RawValue + 32768L >> 16;
            fpQuaternion.W = cos;
            return fpQuaternion;
        }

        /// <summary>
        ///     Returns the Inverse of rotation <paramref name="value" />. If <paramref name="value" /> is normalized it
        ///     will be faster to call
        ///     <see cref="M:Herta.FPQuaternion.Conjugate(Herta.FPQuaternion)" />. If
        ///     <paramref name="value" />
        ///     has a magnitude close to 0, <paramref name="value" /> will be returned.
        ///     <remarks><see cref="T:Herta.FPLut" /> needs to be initialised.</remarks>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FPQuaternion Inverse(FPQuaternion value)
        {
            long magnitudeSqrRaw = value.MagnitudeSqrRaw;
            if (magnitudeSqrRaw == 0L)
                return value;
            long num = 4294967296L / FPMath.SqrtRaw(magnitudeSqrRaw);
            FPQuaternion fpQuaternion;
            fpQuaternion.X.RawValue = -value.X.RawValue * num + 32768L >> 16;
            fpQuaternion.Y.RawValue = -value.Y.RawValue * num + 32768L >> 16;
            fpQuaternion.Z.RawValue = -value.Z.RawValue * num + 32768L >> 16;
            fpQuaternion.W.RawValue = value.W.RawValue * num + 32768L >> 16;
            return fpQuaternion;
        }

        /// <summary>
        ///     Converts this quaternion <paramref name="value" /> to one with the same orientation but with a magnitude of 1. If
        ///     <paramref name="value" />
        ///     has a magnitude close to 0, <see cref="P:Herta.FPQuaternion.Identity" /> is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FPQuaternion Normalize(FPQuaternion value)
        {
            long magnitudeSqrRaw = value.MagnitudeSqrRaw;
            if (magnitudeSqrRaw == 0L)
                return FPQuaternion.Identity;
            long num = 4294967296L / FPMath.SqrtRaw(magnitudeSqrRaw);
            FPQuaternion fpQuaternion;
            fpQuaternion.X.RawValue = value.X.RawValue * num + 32768L >> 16;
            fpQuaternion.Y.RawValue = value.Y.RawValue * num + 32768L >> 16;
            fpQuaternion.Z.RawValue = value.Z.RawValue * num + 32768L >> 16;
            fpQuaternion.W.RawValue = value.W.RawValue * num + 32768L >> 16;
            return fpQuaternion;
        }

        internal static FPQuaternion NormalizeSmall(FPQuaternion value)
        {
            ulong x = (ulong)(value.X.RawValue * value.X.RawValue + value.Y.RawValue * value.Y.RawValue + value.Z.RawValue * value.Z.RawValue + value.W.RawValue * value.W.RawValue);
            if (x == 0UL)
                return FPQuaternion.Identity;
            FPMath.ExponentMantisaPair exponentMantissa = FPMath.GetSqrtExponentMantissa(x);
            long num = 17592186044416L / (long)exponentMantissa.Mantissa;
            value.X.RawValue = value.X.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            value.Y.RawValue = value.Y.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            value.Z.RawValue = value.Z.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            value.W.RawValue = value.W.RawValue * num >> 22 + exponentMantissa.Exponent - 8;
            return value;
        }

        internal static FPVector3 ToEulerZXY(FPQuaternion value)
        {
            long rawValue1 = value.X.RawValue;
            long rawValue2 = value.Y.RawValue;
            long rawValue3 = value.Z.RawValue;
            long rawValue4 = value.W.RawValue;
            FP fp;
            fp.RawValue = (rawValue4 * rawValue1 >> 15) - (rawValue2 * rawValue3 >> 15);
            fp.RawValue = fp.RawValue > FP._1.RawValue ? FP._1.RawValue : fp.RawValue;
            fp.RawValue = fp.RawValue < -FP._1.RawValue ? -FP._1.RawValue : fp.RawValue;
            FPVector3 eulerZxy = new FPVector3();
            eulerZxy.X = FPMath.Asin(fp);
            if (FPMath.Abs(fp) < 1)
            {
                FP y1;
                y1.RawValue = (rawValue1 * rawValue3 >> 15) + (rawValue4 * rawValue2 >> 15);
                FP x1;
                x1.RawValue = FP._1.RawValue - (rawValue1 * rawValue1 >> 15) - (rawValue2 * rawValue2 >> 15);
                FP y2;
                y2.RawValue = (rawValue1 * rawValue2 >> 15) + (rawValue4 * rawValue3 >> 15);
                FP x2;
                x2.RawValue = FP._1.RawValue - (rawValue1 * rawValue1 >> 15) - (rawValue3 * rawValue3 >> 15);
                eulerZxy.Y = FPMath.Atan2(y1, x1);
                eulerZxy.Z = FPMath.Atan2(y2, x2);
            }
            else
            {
                FP y;
                y.RawValue = (rawValue4 * rawValue2 >> 15) - (rawValue1 * rawValue3 >> 15);
                FP x;
                x.RawValue = FP._1.RawValue - (rawValue2 * rawValue2 >> 15) - (rawValue3 * rawValue3 >> 15);
                eulerZxy.Y = FPMath.Atan2(y, x);
                eulerZxy.Z = (FP)0;
            }

            eulerZxy.X.RawValue = eulerZxy.X.RawValue * FP.Rad2Deg.RawValue + 32768L >> 16;
            eulerZxy.Y.RawValue = eulerZxy.Y.RawValue * FP.Rad2Deg.RawValue + 32768L >> 16;
            eulerZxy.Z.RawValue = eulerZxy.Z.RawValue * FP.Rad2Deg.RawValue + 32768L >> 16;
            return eulerZxy;
        }

        /// <summary>
        ///     Computes product of two quaternions. Fully equivalent to Unity's Quaternion multiplication.
        ///     See
        ///     <see
        ///         cref="M:Herta.FPQuaternion.Product(Herta.FPQuaternion,Herta.FPQuaternion)" />
        ///     for details.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPQuaternion operator *(FPQuaternion left, FPQuaternion right) => FPQuaternion.Product(left, right);

        /// <summary>
        ///     Scales quaternion <paramref name="left" /> with <paramref name="right" />.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static FPQuaternion operator *(FPQuaternion left, FP right)
        {
            left.X.RawValue = left.X.RawValue * right.RawValue + 32768L >> 16;
            left.Y.RawValue = left.Y.RawValue * right.RawValue + 32768L >> 16;
            left.Z.RawValue = left.Z.RawValue * right.RawValue + 32768L >> 16;
            left.W.RawValue = left.W.RawValue * right.RawValue + 32768L >> 16;
            return left;
        }

        /// <summary>
        ///     Scales quaternion <paramref name="right" /> with <paramref name="left" />.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public static FPQuaternion operator *(FP left, FPQuaternion right) => right * left;

        /// <summary>
        ///     Adds each component of <paramref name="right" /> to <paramref name="left" />.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static FPQuaternion operator +(FPQuaternion left, FPQuaternion right)
        {
            left.X.RawValue += right.X.RawValue;
            left.Y.RawValue += right.Y.RawValue;
            left.Z.RawValue += right.Z.RawValue;
            left.W.RawValue += right.W.RawValue;
            return left;
        }

        /// <summary>
        ///     Subtracts each component of <paramref name="right" /> from <paramref name="left" />.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static FPQuaternion operator -(FPQuaternion left, FPQuaternion right)
        {
            left.X.RawValue -= right.X.RawValue;
            left.Y.RawValue -= right.Y.RawValue;
            left.Z.RawValue -= right.Z.RawValue;
            left.W.RawValue -= right.W.RawValue;
            return left;
        }

        /// <summary>
        ///     Rotates the point <paramref name="point" /> with rotation <paramref name="quat" />.
        /// </summary>
        /// <param name="quat"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static FPVector3 operator *(FPQuaternion quat, FPVector3 point)
        {
            long rawValue1 = quat.X.RawValue;
            long rawValue2 = quat.Y.RawValue;
            long rawValue3 = quat.Z.RawValue;
            long rawValue4 = quat.W.RawValue;
            long rawValue5 = point.X.RawValue;
            long rawValue6 = point.Y.RawValue;
            long rawValue7 = point.Z.RawValue;
            long rawValue8 = FP._1.RawValue;
            long num1 = rawValue1 << 1;
            long num2 = rawValue2 << 1;
            long num3 = rawValue3 << 1;
            long num4 = rawValue1 * num1 + 32768L >> 16;
            long num5 = rawValue2 * num2 + 32768L >> 16;
            long num6 = rawValue3 * num3 + 32768L >> 16;
            long num7 = rawValue1 * num2 + 32768L >> 16;
            long num8 = rawValue1 * num3 + 32768L >> 16;
            long num9 = rawValue2 * num3 + 32768L >> 16;
            long num10 = rawValue4 * num1 + 32768L >> 16;
            long num11 = rawValue4 * num2 + 32768L >> 16;
            long num12 = rawValue4 * num3 + 32768L >> 16;
            FPVector3 fpVector3;
            fpVector3.X.RawValue = ((rawValue8 - (num5 + num6)) * rawValue5 + 32768L >> 16) + ((num7 - num12) * rawValue6 + 32768L >> 16) + ((num8 + num11) * rawValue7 + 32768L >> 16);
            fpVector3.Y.RawValue = ((num7 + num12) * rawValue5 + 32768L >> 16) + ((rawValue8 - (num4 + num6)) * rawValue6 + 32768L >> 16) + ((num9 - num10) * rawValue7 + 32768L >> 16);
            fpVector3.Z.RawValue = ((num8 - num11) * rawValue5 + 32768L >> 16) + ((num9 + num10) * rawValue6 + 32768L >> 16) + ((rawValue8 - (num4 + num5)) * rawValue7 + 32768L >> 16);
            return fpVector3;
        }
    }
}