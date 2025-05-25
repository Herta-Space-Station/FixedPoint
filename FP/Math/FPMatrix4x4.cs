using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NativeCollections;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     Represents 4x4 column major matrix.
    ///     Each cell can be individually accessed as a field (M&lt;row&gt;&lt;column&gt;), with indexing
    ///     indexxing property[row, column] or with indexing property[index].
    /// </summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FPMatrix4x4
    {
        /// <summary>
        ///     The size of the struct in-memory inside the Frame data-buffers or stack (when passed as value parameter).
        ///     Not related to the snapshot payload this occupies, which is bit-packed and compressed.
        /// </summary>
        public const int SIZE = 128;

        /// <summary>First row, first column</summary>
        [FieldOffset(0)] public FP M00;

        /// <summary>Second row, first column</summary>
        [FieldOffset(8)] public FP M10;

        /// <summary>Third row, first column</summary>
        [FieldOffset(16)] public FP M20;

        /// <summary>Fourth row, first column</summary>
        [FieldOffset(24)] public FP M30;

        /// <summary>First row, second column</summary>
        [FieldOffset(32)] public FP M01;

        /// <summary>Second row, second column</summary>
        [FieldOffset(40)] public FP M11;

        /// <summary>Third row, second column</summary>
        [FieldOffset(48)] public FP M21;

        /// <summary>Fourth row, second column</summary>
        [FieldOffset(56)] public FP M31;

        /// <summary>First row, third column</summary>
        [FieldOffset(64)] public FP M02;

        /// <summary>Second row, third column</summary>
        [FieldOffset(72)] public FP M12;

        /// <summary>Third row, third column</summary>
        [FieldOffset(80)] public FP M22;

        /// <summary>Fourth row, third column</summary>
        [FieldOffset(88)] public FP M32;

        /// <summary>First row, fourth column</summary>
        [FieldOffset(96)] public FP M03;

        /// <summary>Second row, fourth column</summary>
        [FieldOffset(104)] public FP M13;

        /// <summary>Third row, fourth column</summary>
        [FieldOffset(112)] public FP M23;

        /// <summary>Fourth row, fourth column</summary>
        [FieldOffset(120)] public FP M33;

        /// <summary>Matrix with 0s in every cell.</summary>
        public static readonly FPMatrix4x4 Zero = new FPMatrix4x4();

        /// <summary>
        ///     Matrix with 1s in the main diagonal and 0s in all other cells.
        /// </summary>
        public static readonly FPMatrix4x4 Identity = new FPMatrix4x4()
        {
            M00 =
            {
                RawValue = 65536
            },
            M11 =
            {
                RawValue = 65536
            },
            M22 =
            {
                RawValue = 65536
            },
            M33 =
            {
                RawValue = 65536
            }
        };

        /// <summary>
        ///     Create from rows - first four values set the first row, second four values - second row etc.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPMatrix4x4 FromRows(
            FP m00,
            FP m01,
            FP m02,
            FP m03,
            FP m10,
            FP m11,
            FP m12,
            FP m13,
            FP m20,
            FP m21,
            FP m22,
            FP m23,
            FP m30,
            FP m31,
            FP m32,
            FP m33)
        {
            FPMatrix4x4 fpMatrix4x4;
            fpMatrix4x4.M00 = m00;
            fpMatrix4x4.M10 = m10;
            fpMatrix4x4.M20 = m20;
            fpMatrix4x4.M30 = m30;
            fpMatrix4x4.M01 = m01;
            fpMatrix4x4.M11 = m11;
            fpMatrix4x4.M21 = m21;
            fpMatrix4x4.M31 = m31;
            fpMatrix4x4.M02 = m02;
            fpMatrix4x4.M12 = m12;
            fpMatrix4x4.M22 = m22;
            fpMatrix4x4.M32 = m32;
            fpMatrix4x4.M03 = m03;
            fpMatrix4x4.M13 = m13;
            fpMatrix4x4.M23 = m23;
            fpMatrix4x4.M33 = m33;
            return fpMatrix4x4;
        }

        /// <summary>
        ///     Create from columns - first four values set the first colunn, second four values - second column etc.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPMatrix4x4 FromColumns(
            FP m00,
            FP m10,
            FP m20,
            FP m30,
            FP m01,
            FP m11,
            FP m21,
            FP m31,
            FP m02,
            FP m12,
            FP m22,
            FP m32,
            FP m03,
            FP m13,
            FP m23,
            FP m33)
        {
            FPMatrix4x4 fpMatrix4x4;
            fpMatrix4x4.M00 = m00;
            fpMatrix4x4.M10 = m10;
            fpMatrix4x4.M20 = m20;
            fpMatrix4x4.M30 = m30;
            fpMatrix4x4.M01 = m01;
            fpMatrix4x4.M11 = m11;
            fpMatrix4x4.M21 = m21;
            fpMatrix4x4.M31 = m31;
            fpMatrix4x4.M02 = m02;
            fpMatrix4x4.M12 = m12;
            fpMatrix4x4.M22 = m22;
            fpMatrix4x4.M32 = m32;
            fpMatrix4x4.M03 = m03;
            fpMatrix4x4.M13 = m13;
            fpMatrix4x4.M23 = m23;
            fpMatrix4x4.M33 = m33;
            return fpMatrix4x4;
        }

        /// <summary>Gets or sets cell M&lt;row&gt;&lt;column&gt;.</summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public FP this[int row, int column]
        {
            get => this[row + column * 4];
            set => this[row + column * 4] = value;
        }

        /// <summary>Gets or sets cell M&lt;index%4&gt;&lt;index/4&gt;</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public FP this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.M00;
                    case 1:
                        return this.M10;
                    case 2:
                        return this.M20;
                    case 3:
                        return this.M30;
                    case 4:
                        return this.M01;
                    case 5:
                        return this.M11;
                    case 6:
                        return this.M21;
                    case 7:
                        return this.M31;
                    case 8:
                        return this.M02;
                    case 9:
                        return this.M12;
                    case 10:
                        return this.M22;
                    case 11:
                        return this.M32;
                    case 12:
                        return this.M03;
                    case 13:
                        return this.M13;
                    case 14:
                        return this.M23;
                    case 15:
                        return this.M33;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.M00 = value;
                        break;
                    case 1:
                        this.M10 = value;
                        break;
                    case 2:
                        this.M20 = value;
                        break;
                    case 3:
                        this.M30 = value;
                        break;
                    case 4:
                        this.M01 = value;
                        break;
                    case 5:
                        this.M11 = value;
                        break;
                    case 6:
                        this.M21 = value;
                        break;
                    case 7:
                        this.M31 = value;
                        break;
                    case 8:
                        this.M02 = value;
                        break;
                    case 9:
                        this.M12 = value;
                        break;
                    case 10:
                        this.M22 = value;
                        break;
                    case 11:
                        this.M32 = value;
                        break;
                    case 12:
                        this.M03 = value;
                        break;
                    case 13:
                        this.M13 = value;
                        break;
                    case 14:
                        this.M23 = value;
                        break;
                    case 15:
                        this.M33 = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>Creates transposed matrix.</summary>
        public FPMatrix4x4 Transposed => FPMatrix4x4.FromColumns(this.M00, this.M01, this.M02, this.M03, this.M10, this.M11, this.M12, this.M13, this.M20, this.M21, this.M22, this.M23, this.M30, this.M31, this.M32, this.M33);

        /// <summary>
        ///     Returns <see langword="true" /> if this matrix is equal to the
        ///     <see cref="P:Herta.FPMatrix4x4.Identity" /> matrix
        /// </summary>
        public bool IsIdentity => this.M00 == 1 && this.M11 == 1 && this.M22 == 1 && this.M33 == 1 && (this.M01.RawValue | this.M02.RawValue | this.M03.RawValue | this.M10.RawValue | this.M12.RawValue | this.M13.RawValue | this.M20.RawValue | this.M21.RawValue | this.M23.RawValue | this.M30.RawValue | this.M31.RawValue | this.M32.RawValue) == 0L;

        /// <summary>
        ///     Creates inverse of look-at matrix, i.e. observer to world transformation. Equivalent to Unity's Matrix4x4.LookAt.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static FPMatrix4x4 InverseLookAt(FPVector3 from, FPVector3 to, FPVector3 up)
        {
            FPVector3 normalized1 = (to - from).Normalized;
            FPVector3 normalized2 = FPVector3.Cross(up.Normalized, normalized1).Normalized;
            FPVector3 fpVector3 = FPVector3.Cross(normalized1, normalized2);
            return FPMatrix4x4.FromRows(normalized2.X, fpVector3.X, normalized1.X, from.X, normalized2.Y, fpVector3.Y, normalized1.Y, from.Y, normalized2.Z, fpVector3.Z, normalized1.Z, from.Z, (FP)0, (FP)0, (FP)0, (FP)1);
        }

        /// <summary>
        ///     Creates look-at matrix, i.e. world to observer transformation. Unity's Matrix4x4.LookAt does the opposite - creates
        ///     observer to world transformation. To get same behaviour use
        ///     <see
        ///         cref="M:Herta.FPMatrix4x4.InverseLookAt(Herta.FPVector3,Herta.FPVector3,Herta.FPVector3)" />
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static FPMatrix4x4 LookAt(FPVector3 from, FPVector3 to, FPVector3 up)
        {
            FPVector3 normalized1 = (to - from).Normalized;
            FPVector3 normalized2 = FPVector3.Cross(up.Normalized, normalized1).Normalized;
            FPVector3 a = FPVector3.Cross(normalized1, normalized2);
            return FPMatrix4x4.FromRows(normalized2.X, normalized2.Y, normalized2.Z, FPVector3.Dot(normalized2, -from), a.X, a.Y, a.Z, FPVector3.Dot(a, -from), normalized1.X, normalized1.Y, normalized1.Z, FPVector3.Dot(normalized1, -from), (FP)0, (FP)0, (FP)0, (FP)1);
        }

        /// <summary>Creates a scaling matrix.</summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static FPMatrix4x4 Scale(FPVector3 scale) => FPMatrix4x4.FromColumns(scale.X, (FP)0, (FP)0, (FP)0, (FP)0, scale.Y, (FP)0, (FP)0, (FP)0, (FP)0, scale.Z, (FP)0, (FP)0, (FP)0, (FP)0, (FP)1);

        /// <summary>Creates a translation matrix.</summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public static FPMatrix4x4 Translate(FPVector3 translation) => FPMatrix4x4.FromRows((FP)1, (FP)0, (FP)0, translation.X, (FP)0, (FP)1, (FP)0, translation.Y, (FP)0, (FP)0, (FP)1, translation.Z, (FP)0, (FP)0, (FP)0, (FP)1);

        /// <summary>
        ///     Returns a string representation of the FPMatrix4x4 object.
        ///     The string representation consists of the values of the matrix elements formatted
        ///     as a 4x4 matrix.
        /// </summary>
        /// <returns>A string representation of the FPMatrix4x4 object.</returns>
        public override string ToString()
        {
            NativeString builder = new NativeString(stackalloc char[512], 0);
            Format(ref builder);

            return builder.ToString();
        }

        public bool TryFormat(Span<char> destination, out int charsWritten)
        {
            NativeString builder = new NativeString(stackalloc char[512], 0);
            Format(ref builder);

            bool result = builder.TryCopyTo(destination);
            charsWritten = result ? builder.Length : 0;
            return result;
        }

        private void Format(ref NativeString builder)
        {
            builder.Append('(');
            builder.Append('(');
            builder.AppendFormattable(this.M00.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M01.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M02.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M03.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(')');
            builder.Append(',');
            builder.Append(' ');
            builder.Append('(');
            builder.AppendFormattable(this.M10.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M11.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M12.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M13.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(')');
            builder.Append(',');
            builder.Append(' ');
            builder.Append('(');
            builder.AppendFormattable(this.M20.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M21.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M22.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M23.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(')');
            builder.Append(',');
            builder.Append(' ');
            builder.Append('(');
            builder.AppendFormattable(this.M30.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M31.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M32.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M33.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(')');
            builder.Append(')');
        }

        /// <summary>Computes the hash code for the current instance.</summary>
        /// <returns>The hash code for the current instance.</returns>
        public override int GetHashCode() => XxHash.Hash32(this);

        /// <summary>Multiplies two matrices.</summary>
        public static FPMatrix4x4 operator *(FPMatrix4x4 a, FPMatrix4x4 b)
        {
            FPMatrix4x4 fpMatrix4x4;
            fpMatrix4x4.M00.RawValue = (a.M00.RawValue * b.M00.RawValue + 32768L >> 16) + (a.M01.RawValue * b.M10.RawValue + 32768L >> 16) + (a.M02.RawValue * b.M20.RawValue + 32768L >> 16) + (a.M03.RawValue * b.M30.RawValue + 32768L >> 16);
            fpMatrix4x4.M01.RawValue = (a.M00.RawValue * b.M01.RawValue + 32768L >> 16) + (a.M01.RawValue * b.M11.RawValue + 32768L >> 16) + (a.M02.RawValue * b.M21.RawValue + 32768L >> 16) + (a.M03.RawValue * b.M31.RawValue + 32768L >> 16);
            fpMatrix4x4.M02.RawValue = (a.M00.RawValue * b.M02.RawValue + 32768L >> 16) + (a.M01.RawValue * b.M12.RawValue + 32768L >> 16) + (a.M02.RawValue * b.M22.RawValue + 32768L >> 16) + (a.M03.RawValue * b.M32.RawValue + 32768L >> 16);
            fpMatrix4x4.M03.RawValue = (a.M00.RawValue * b.M03.RawValue + 32768L >> 16) + (a.M01.RawValue * b.M13.RawValue + 32768L >> 16) + (a.M02.RawValue * b.M23.RawValue + 32768L >> 16) + (a.M03.RawValue * b.M33.RawValue + 32768L >> 16);
            fpMatrix4x4.M10.RawValue = (a.M10.RawValue * b.M00.RawValue + 32768L >> 16) + (a.M11.RawValue * b.M10.RawValue + 32768L >> 16) + (a.M12.RawValue * b.M20.RawValue + 32768L >> 16) + (a.M13.RawValue * b.M30.RawValue + 32768L >> 16);
            fpMatrix4x4.M11.RawValue = (a.M10.RawValue * b.M01.RawValue + 32768L >> 16) + (a.M11.RawValue * b.M11.RawValue + 32768L >> 16) + (a.M12.RawValue * b.M21.RawValue + 32768L >> 16) + (a.M13.RawValue * b.M31.RawValue + 32768L >> 16);
            fpMatrix4x4.M12.RawValue = (a.M10.RawValue * b.M02.RawValue + 32768L >> 16) + (a.M11.RawValue * b.M12.RawValue + 32768L >> 16) + (a.M12.RawValue * b.M22.RawValue + 32768L >> 16) + (a.M13.RawValue * b.M32.RawValue + 32768L >> 16);
            fpMatrix4x4.M13.RawValue = (a.M10.RawValue * b.M03.RawValue + 32768L >> 16) + (a.M11.RawValue * b.M13.RawValue + 32768L >> 16) + (a.M12.RawValue * b.M23.RawValue + 32768L >> 16) + (a.M13.RawValue * b.M33.RawValue + 32768L >> 16);
            fpMatrix4x4.M20.RawValue = (a.M20.RawValue * b.M00.RawValue + 32768L >> 16) + (a.M21.RawValue * b.M10.RawValue + 32768L >> 16) + (a.M22.RawValue * b.M20.RawValue + 32768L >> 16) + (a.M23.RawValue * b.M30.RawValue + 32768L >> 16);
            fpMatrix4x4.M21.RawValue = (a.M20.RawValue * b.M01.RawValue + 32768L >> 16) + (a.M21.RawValue * b.M11.RawValue + 32768L >> 16) + (a.M22.RawValue * b.M21.RawValue + 32768L >> 16) + (a.M23.RawValue * b.M31.RawValue + 32768L >> 16);
            fpMatrix4x4.M22.RawValue = (a.M20.RawValue * b.M02.RawValue + 32768L >> 16) + (a.M21.RawValue * b.M12.RawValue + 32768L >> 16) + (a.M22.RawValue * b.M22.RawValue + 32768L >> 16) + (a.M23.RawValue * b.M32.RawValue + 32768L >> 16);
            fpMatrix4x4.M23.RawValue = (a.M20.RawValue * b.M03.RawValue + 32768L >> 16) + (a.M21.RawValue * b.M13.RawValue + 32768L >> 16) + (a.M22.RawValue * b.M23.RawValue + 32768L >> 16) + (a.M23.RawValue * b.M33.RawValue + 32768L >> 16);
            fpMatrix4x4.M30.RawValue = (a.M30.RawValue * b.M00.RawValue + 32768L >> 16) + (a.M31.RawValue * b.M10.RawValue + 32768L >> 16) + (a.M32.RawValue * b.M20.RawValue + 32768L >> 16) + (a.M33.RawValue * b.M30.RawValue + 32768L >> 16);
            fpMatrix4x4.M31.RawValue = (a.M30.RawValue * b.M01.RawValue + 32768L >> 16) + (a.M31.RawValue * b.M11.RawValue + 32768L >> 16) + (a.M32.RawValue * b.M21.RawValue + 32768L >> 16) + (a.M33.RawValue * b.M31.RawValue + 32768L >> 16);
            fpMatrix4x4.M32.RawValue = (a.M30.RawValue * b.M02.RawValue + 32768L >> 16) + (a.M31.RawValue * b.M12.RawValue + 32768L >> 16) + (a.M32.RawValue * b.M22.RawValue + 32768L >> 16) + (a.M33.RawValue * b.M32.RawValue + 32768L >> 16);
            fpMatrix4x4.M33.RawValue = (a.M30.RawValue * b.M03.RawValue + 32768L >> 16) + (a.M31.RawValue * b.M13.RawValue + 32768L >> 16) + (a.M32.RawValue * b.M23.RawValue + 32768L >> 16) + (a.M33.RawValue * b.M33.RawValue + 32768L >> 16);
            return fpMatrix4x4;
        }

        /// <summary>Attempts to get a scale value from the matrix.</summary>
        public FPVector3 LossyScale
        {
            get
            {
                long x1 = (this.M00.RawValue * this.M00.RawValue + 32768L >> 16) + (this.M10.RawValue * this.M10.RawValue + 32768L >> 16) + (this.M20.RawValue * this.M20.RawValue + 32768L >> 16);
                long x2 = (this.M01.RawValue * this.M01.RawValue + 32768L >> 16) + (this.M11.RawValue * this.M11.RawValue + 32768L >> 16) + (this.M21.RawValue * this.M21.RawValue + 32768L >> 16);
                long x3 = (this.M02.RawValue * this.M02.RawValue + 32768L >> 16) + (this.M12.RawValue * this.M12.RawValue + 32768L >> 16) + (this.M22.RawValue * this.M22.RawValue + 32768L >> 16);
                return new FPVector3(FP.FromRaw(FPMath.SqrtRaw(x1) * (long)FPMath.SignInt(this.Determinant3x3)), FP.FromRaw(FPMath.SqrtRaw(x2)), FP.FromRaw(FPMath.SqrtRaw(x3)));
            }
        }

        /// <summary>
        ///     Creates inverted matrix. Matrix with determinant 0 can not be inverted and result with
        ///     <see cref="P:Herta.FPMatrix4x4.Zero" />.
        /// </summary>
        public FPMatrix4x4 Inverted
        {
            get
            {
                long num1 = (this.M00.RawValue * this.M11.RawValue + 32768L >> 16) - (this.M10.RawValue * this.M01.RawValue + 32768L >> 16);
                long num2 = (this.M00.RawValue * this.M12.RawValue + 32768L >> 16) - (this.M10.RawValue * this.M02.RawValue + 32768L >> 16);
                long num3 = (this.M00.RawValue * this.M13.RawValue + 32768L >> 16) - (this.M10.RawValue * this.M03.RawValue + 32768L >> 16);
                long num4 = (this.M01.RawValue * this.M12.RawValue + 32768L >> 16) - (this.M11.RawValue * this.M02.RawValue + 32768L >> 16);
                long num5 = (this.M01.RawValue * this.M13.RawValue + 32768L >> 16) - (this.M11.RawValue * this.M03.RawValue + 32768L >> 16);
                long num6 = (this.M02.RawValue * this.M13.RawValue + 32768L >> 16) - (this.M12.RawValue * this.M03.RawValue + 32768L >> 16);
                long num7 = (this.M22.RawValue * this.M33.RawValue + 32768L >> 16) - (this.M32.RawValue * this.M23.RawValue + 32768L >> 16);
                long num8 = (this.M21.RawValue * this.M33.RawValue + 32768L >> 16) - (this.M31.RawValue * this.M23.RawValue + 32768L >> 16);
                long num9 = (this.M21.RawValue * this.M32.RawValue + 32768L >> 16) - (this.M31.RawValue * this.M22.RawValue + 32768L >> 16);
                long num10 = (this.M20.RawValue * this.M33.RawValue + 32768L >> 16) - (this.M30.RawValue * this.M23.RawValue + 32768L >> 16);
                long num11 = (this.M20.RawValue * this.M32.RawValue + 32768L >> 16) - (this.M30.RawValue * this.M22.RawValue + 32768L >> 16);
                long num12 = (this.M20.RawValue * this.M31.RawValue + 32768L >> 16) - (this.M30.RawValue * this.M21.RawValue + 32768L >> 16);
                long num13 = (num1 * num7 + 32768L >> 16) - (num2 * num8 + 32768L >> 16) + (num3 * num9 + 32768L >> 16) + (num4 * num10 + 32768L >> 16) - (num5 * num11 + 32768L >> 16) + (num6 * num12 + 32768L >> 16);
                if (num13 == 0L)
                    return FPMatrix4x4.Zero;
                long num14 = 4294967296L / num13;
                long num15 = num1 * num14 + 32768L >> 16;
                long num16 = num2 * num14 + 32768L >> 16;
                long num17 = num3 * num14 + 32768L >> 16;
                long num18 = num4 * num14 + 32768L >> 16;
                long num19 = num5 * num14 + 32768L >> 16;
                long num20 = num6 * num14 + 32768L >> 16;
                long num21 = num7 * num14 + 32768L >> 16;
                long num22 = num8 * num14 + 32768L >> 16;
                long num23 = num9 * num14 + 32768L >> 16;
                long num24 = num10 * num14 + 32768L >> 16;
                long num25 = num11 * num14 + 32768L >> 16;
                long num26 = num12 * num14 + 32768L >> 16;
                FPMatrix4x4 inverted;
                inverted.M00.RawValue = (this.M11.RawValue * num21 + 32768L >> 16) - (this.M12.RawValue * num22 + 32768L >> 16) + (this.M13.RawValue * num23 + 32768L >> 16);
                inverted.M01.RawValue = -(this.M01.RawValue * num21 + 32768L >> 16) + (this.M02.RawValue * num22 + 32768L >> 16) - (this.M03.RawValue * num23 + 32768L >> 16);
                inverted.M02.RawValue = (this.M31.RawValue * num20 + 32768L >> 16) - (this.M32.RawValue * num19 + 32768L >> 16) + (this.M33.RawValue * num18 + 32768L >> 16);
                inverted.M03.RawValue = -(this.M21.RawValue * num20 + 32768L >> 16) + (this.M22.RawValue * num19 + 32768L >> 16) - (this.M23.RawValue * num18 + 32768L >> 16);
                inverted.M10.RawValue = -(this.M10.RawValue * num21 + 32768L >> 16) + (this.M12.RawValue * num24 + 32768L >> 16) - (this.M13.RawValue * num25 + 32768L >> 16);
                inverted.M11.RawValue = (this.M00.RawValue * num21 + 32768L >> 16) - (this.M02.RawValue * num24 + 32768L >> 16) + (this.M03.RawValue * num25 + 32768L >> 16);
                inverted.M12.RawValue = -(this.M30.RawValue * num20 + 32768L >> 16) + (this.M32.RawValue * num17 + 32768L >> 16) - (this.M33.RawValue * num16 + 32768L >> 16);
                inverted.M13.RawValue = (this.M20.RawValue * num20 + 32768L >> 16) - (this.M22.RawValue * num17 + 32768L >> 16) + (this.M23.RawValue * num16 + 32768L >> 16);
                inverted.M20.RawValue = (this.M10.RawValue * num22 + 32768L >> 16) - (this.M11.RawValue * num24 + 32768L >> 16) + (this.M13.RawValue * num26 + 32768L >> 16);
                inverted.M21.RawValue = -(this.M00.RawValue * num22 + 32768L >> 16) + (this.M01.RawValue * num24 + 32768L >> 16) - (this.M03.RawValue * num26 + 32768L >> 16);
                inverted.M22.RawValue = (this.M30.RawValue * num19 + 32768L >> 16) - (this.M31.RawValue * num17 + 32768L >> 16) + (this.M33.RawValue * num15 + 32768L >> 16);
                inverted.M23.RawValue = -(this.M20.RawValue * num19 + 32768L >> 16) + (this.M21.RawValue * num17 + 32768L >> 16) - (this.M23.RawValue * num15 + 32768L >> 16);
                inverted.M30.RawValue = -(this.M10.RawValue * num23 + 32768L >> 16) + (this.M11.RawValue * num25 + 32768L >> 16) - (this.M12.RawValue * num26 + 32768L >> 16);
                inverted.M31.RawValue = (this.M00.RawValue * num23 + 32768L >> 16) - (this.M01.RawValue * num25 + 32768L >> 16) + (this.M02.RawValue * num26 + 32768L >> 16);
                inverted.M32.RawValue = -(this.M30.RawValue * num18 + 32768L >> 16) + (this.M31.RawValue * num16 + 32768L >> 16) - (this.M32.RawValue * num15 + 32768L >> 16);
                inverted.M33.RawValue = (this.M20.RawValue * num18 + 32768L >> 16) - (this.M21.RawValue * num16 + 32768L >> 16) + (this.M22.RawValue * num15 + 32768L >> 16);
                return inverted;
            }
        }

        /// <summary>Calculates determinant of this matrix.</summary>
        public FP Determinant
        {
            get
            {
                long num1 = (this.M00.RawValue * this.M11.RawValue + 32768L >> 16) - (this.M10.RawValue * this.M01.RawValue + 32768L >> 16);
                long num2 = (this.M00.RawValue * this.M12.RawValue + 32768L >> 16) - (this.M10.RawValue * this.M02.RawValue + 32768L >> 16);
                long num3 = (this.M00.RawValue * this.M13.RawValue + 32768L >> 16) - (this.M10.RawValue * this.M03.RawValue + 32768L >> 16);
                long num4 = (this.M01.RawValue * this.M12.RawValue + 32768L >> 16) - (this.M11.RawValue * this.M02.RawValue + 32768L >> 16);
                long num5 = (this.M01.RawValue * this.M13.RawValue + 32768L >> 16) - (this.M11.RawValue * this.M03.RawValue + 32768L >> 16);
                long num6 = (this.M02.RawValue * this.M13.RawValue + 32768L >> 16) - (this.M12.RawValue * this.M03.RawValue + 32768L >> 16);
                long num7 = (this.M22.RawValue * this.M33.RawValue + 32768L >> 16) - (this.M32.RawValue * this.M23.RawValue + 32768L >> 16);
                long num8 = (this.M21.RawValue * this.M33.RawValue + 32768L >> 16) - (this.M31.RawValue * this.M23.RawValue + 32768L >> 16);
                long num9 = (this.M21.RawValue * this.M32.RawValue + 32768L >> 16) - (this.M31.RawValue * this.M22.RawValue + 32768L >> 16);
                long num10 = (this.M20.RawValue * this.M33.RawValue + 32768L >> 16) - (this.M30.RawValue * this.M23.RawValue + 32768L >> 16);
                long num11 = (this.M20.RawValue * this.M32.RawValue + 32768L >> 16) - (this.M30.RawValue * this.M22.RawValue + 32768L >> 16);
                long num12 = (this.M20.RawValue * this.M31.RawValue + 32768L >> 16) - (this.M30.RawValue * this.M21.RawValue + 32768L >> 16);
                return FP.FromRaw((num1 * num7 + 32768L >> 16) - (num2 * num8 + 32768L >> 16) + (num3 * num9 + 32768L >> 16) + (num4 * num10 + 32768L >> 16) - (num5 * num11 + 32768L >> 16) + (num6 * num12 + 32768L >> 16));
            }
        }

        /// <summary>
        ///     Calculates determinant, taking only rotation and scale parts of this matrix into account.
        /// </summary>
        public FP Determinant3x3 => FP.FromRaw(((this.M00.RawValue * this.M11.RawValue + 32768L >> 16) * this.M22.RawValue + 32768L >> 16) + ((this.M10.RawValue * this.M21.RawValue + 32768L >> 16) * this.M02.RawValue + 32768L >> 16) + ((this.M20.RawValue * this.M01.RawValue + 32768L >> 16) * this.M12.RawValue + 32768L >> 16) - (((this.M02.RawValue * this.M11.RawValue + 32768L >> 16) * this.M20.RawValue + 32768L >> 16) + ((this.M12.RawValue * this.M21.RawValue + 32768L >> 16) * this.M00.RawValue + 32768L >> 16) + ((this.M22.RawValue * this.M01.RawValue + 32768L >> 16) * this.M10.RawValue + 32768L >> 16)));

        /// <summary>
        ///     Attempts to get a rotation quaternion from this matrix.
        /// </summary>
        public FPQuaternion Rotation
        {
            get
            {
                long num1 = this.M00.RawValue + this.M11.RawValue + this.M22.RawValue;
                FP w;
                FP x;
                FP y;
                FP z;
                if (num1 > 0L)
                {
                    long num2 = FPMath.SqrtRaw(num1 + 65536L);
                    w.RawValue = num2 >> 1;
                    long num3 = 2147483648L / num2;
                    x.RawValue = (this.M21.RawValue - this.M12.RawValue) * num3 + 32768L >> 16;
                    y.RawValue = (this.M02.RawValue - this.M20.RawValue) * num3 + 32768L >> 16;
                    z.RawValue = (this.M10.RawValue - this.M01.RawValue) * num3 + 32768L >> 16;
                }
                else if (this.M00 > this.M11 & this.M00 > this.M22)
                {
                    long num4 = FPMath.SqrtRaw(65536L + (this.M00.RawValue - this.M11.RawValue - this.M22.RawValue));
                    x.RawValue = num4 >> 1;
                    long num5 = 2147483648L / num4;
                    w.RawValue = (this.M21.RawValue - this.M12.RawValue) * num5 + 32768L >> 16;
                    y.RawValue = (this.M01.RawValue + this.M10.RawValue) * num5 + 32768L >> 16;
                    z.RawValue = (this.M02.RawValue + this.M20.RawValue) * num5 + 32768L >> 16;
                }
                else if (this.M11 > this.M22)
                {
                    long num6 = FPMath.SqrtRaw(65536L + (this.M11.RawValue - this.M00.RawValue - this.M22.RawValue));
                    y.RawValue = num6 >> 1;
                    long num7 = 2147483648L / num6;
                    w.RawValue = (this.M02.RawValue - this.M20.RawValue) * num7 + 32768L >> 16;
                    x.RawValue = (this.M01.RawValue + this.M10.RawValue) * num7 + 32768L >> 16;
                    z.RawValue = (this.M12.RawValue + this.M21.RawValue) * num7 + 32768L >> 16;
                }
                else
                {
                    long num8 = FPMath.SqrtRaw(65536L + (this.M22.RawValue - this.M00.RawValue - this.M11.RawValue));
                    z.RawValue = num8 >> 1;
                    long num9 = 2147483648L / num8;
                    w.RawValue = (this.M10.RawValue - this.M01.RawValue) * num9 + 32768L >> 16;
                    x.RawValue = (this.M02.RawValue + this.M20.RawValue) * num9 + 32768L >> 16;
                    y.RawValue = (this.M12.RawValue + this.M21.RawValue) * num9 + 32768L >> 16;
                }

                return new FPQuaternion(x, y, z, w);
            }
        }

        /// <summary>
        ///     Transforms a position by this matrix. Works with regulard 3D transformations and with projective transformations.
        /// </summary>
        public FPVector3 MultiplyPoint(FPVector3 point)
        {
            long num = 4294967296L / ((this.M30.RawValue * point.X.RawValue + 32768L >> 16) + (this.M31.RawValue * point.Y.RawValue + 32768L >> 16) + (this.M32.RawValue * point.Z.RawValue + 32768L >> 16) + this.M33.RawValue);
            FPVector3 fpVector3 = this.MultiplyPoint3x4(point);
            fpVector3.X.RawValue = fpVector3.X.RawValue * num + 32768L >> 16;
            fpVector3.Y.RawValue = fpVector3.Y.RawValue * num + 32768L >> 16;
            fpVector3.Z.RawValue = fpVector3.Z.RawValue * num + 32768L >> 16;
            return fpVector3;
        }

        /// <summary>
        ///     Transforms a position by this matrix. Faster than
        ///     <see cref="M:Herta.FPMatrix4x4.MultiplyPoint(Herta.FPVector3)" />, but works only
        ///     with regulard 3D transformations.
        /// </summary>
        public FPVector3 MultiplyPoint3x4(FPVector3 point)
        {
            FPVector3 fpVector3;
            fpVector3.X.RawValue = (this.M00.RawValue * point.X.RawValue + 32768L >> 16) + (this.M01.RawValue * point.Y.RawValue + 32768L >> 16) + (this.M02.RawValue * point.Z.RawValue + 32768L >> 16) + this.M03.RawValue;
            fpVector3.Y.RawValue = (this.M10.RawValue * point.X.RawValue + 32768L >> 16) + (this.M11.RawValue * point.Y.RawValue + 32768L >> 16) + (this.M12.RawValue * point.Z.RawValue + 32768L >> 16) + this.M13.RawValue;
            fpVector3.Z.RawValue = (this.M20.RawValue * point.X.RawValue + 32768L >> 16) + (this.M21.RawValue * point.Y.RawValue + 32768L >> 16) + (this.M22.RawValue * point.Z.RawValue + 32768L >> 16) + this.M23.RawValue;
            return fpVector3;
        }

        /// <summary>
        ///     Transforms a direction by this matrix. Only rotation and scale part of the matrix is taken into account.
        /// </summary>
        public FPVector3 MultiplyVector(FPVector3 vector)
        {
            FPVector3 fpVector3;
            fpVector3.X.RawValue = (this.M00.RawValue * vector.X.RawValue + 32768L >> 16) + (this.M01.RawValue * vector.Y.RawValue + 32768L >> 16) + (this.M02.RawValue * vector.Z.RawValue + 32768L >> 16);
            fpVector3.Y.RawValue = (this.M10.RawValue * vector.X.RawValue + 32768L >> 16) + (this.M11.RawValue * vector.Y.RawValue + 32768L >> 16) + (this.M12.RawValue * vector.Z.RawValue + 32768L >> 16);
            fpVector3.Z.RawValue = (this.M20.RawValue * vector.X.RawValue + 32768L >> 16) + (this.M21.RawValue * vector.Y.RawValue + 32768L >> 16) + (this.M22.RawValue * vector.Z.RawValue + 32768L >> 16);
            return fpVector3;
        }

        /// <summary>
        ///     Creates a translation, rotation and scaling matrix.
        ///     Can be used to create local-to-world transformations.
        ///     Rotation is expected to be normalized.
        /// </summary>
        public static FPMatrix4x4 TRS(FPVector3 pos, FPQuaternion q, FPVector3 s)
        {
            FPMatrix4x4 fpMatrix4x4 = FPMatrix4x4.Rotate(q);
            fpMatrix4x4.M00.RawValue = fpMatrix4x4.M00.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix4x4.M10.RawValue = fpMatrix4x4.M10.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix4x4.M20.RawValue = fpMatrix4x4.M20.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix4x4.M01.RawValue = fpMatrix4x4.M01.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix4x4.M11.RawValue = fpMatrix4x4.M11.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix4x4.M21.RawValue = fpMatrix4x4.M21.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix4x4.M02.RawValue = fpMatrix4x4.M02.RawValue * s.Z.RawValue + 32768L >> 16;
            fpMatrix4x4.M12.RawValue = fpMatrix4x4.M12.RawValue * s.Z.RawValue + 32768L >> 16;
            fpMatrix4x4.M22.RawValue = fpMatrix4x4.M22.RawValue * s.Z.RawValue + 32768L >> 16;
            fpMatrix4x4.M03.RawValue = pos.X.RawValue;
            fpMatrix4x4.M13.RawValue = pos.Y.RawValue;
            fpMatrix4x4.M23.RawValue = pos.Z.RawValue;
            return fpMatrix4x4;
        }

        /// <summary>
        ///     Creates an inversion translation, rotation and scaling matrix. This is significantly faster
        ///     than inverting TRS matrix.
        ///     Can be used to create world-to-local transformations.
        ///     Rotation is expected to be normalized.
        /// </summary>
        public static FPMatrix4x4 InverseTRS(FPVector3 pos, FPQuaternion q, FPVector3 s)
        {
            FPMatrix4x4 fpMatrix4x4 = FPMatrix4x4.Rotate(q.Conjugated);
            fpMatrix4x4.M00.RawValue = fpMatrix4x4.M00.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix4x4.M01.RawValue = fpMatrix4x4.M01.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix4x4.M02.RawValue = fpMatrix4x4.M02.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix4x4.M10.RawValue = fpMatrix4x4.M10.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix4x4.M11.RawValue = fpMatrix4x4.M11.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix4x4.M12.RawValue = fpMatrix4x4.M12.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix4x4.M20.RawValue = fpMatrix4x4.M20.RawValue * s.Z.RawValue + 32768L >> 16;
            fpMatrix4x4.M21.RawValue = fpMatrix4x4.M21.RawValue * s.Z.RawValue + 32768L >> 16;
            fpMatrix4x4.M22.RawValue = fpMatrix4x4.M22.RawValue * s.Z.RawValue + 32768L >> 16;
            fpMatrix4x4.M03.RawValue = -(pos.X.RawValue * fpMatrix4x4.M00.RawValue + 32768L >> 16) - (pos.Y.RawValue * fpMatrix4x4.M01.RawValue + 32768L >> 16) - (pos.Z.RawValue * fpMatrix4x4.M02.RawValue + 32768L >> 16);
            fpMatrix4x4.M13.RawValue = -(pos.X.RawValue * fpMatrix4x4.M10.RawValue + 32768L >> 16) - (pos.Y.RawValue * fpMatrix4x4.M11.RawValue + 32768L >> 16) - (pos.Z.RawValue * fpMatrix4x4.M12.RawValue + 32768L >> 16);
            fpMatrix4x4.M23.RawValue = -(pos.X.RawValue * fpMatrix4x4.M20.RawValue + 32768L >> 16) - (pos.Y.RawValue * fpMatrix4x4.M21.RawValue + 32768L >> 16) - (pos.Z.RawValue * fpMatrix4x4.M22.RawValue + 32768L >> 16);
            return fpMatrix4x4;
        }

        /// <summary>
        ///     Creates a rotation matrix. Rotation is expected to be normalized.
        /// </summary>
        public static FPMatrix4x4 Rotate(FPQuaternion q)
        {
            long num1 = q.X.RawValue * 2L;
            long num2 = q.Y.RawValue * 2L;
            long num3 = q.Z.RawValue * 2L;
            long num4 = q.X.RawValue * num1 + 32768L >> 16;
            long num5 = q.Y.RawValue * num2 + 32768L >> 16;
            long num6 = q.Z.RawValue * num3 + 32768L >> 16;
            long num7 = q.X.RawValue * num2 + 32768L >> 16;
            long num8 = q.X.RawValue * num3 + 32768L >> 16;
            long num9 = q.Y.RawValue * num3 + 32768L >> 16;
            long num10 = q.W.RawValue * num1 + 32768L >> 16;
            long num11 = q.W.RawValue * num2 + 32768L >> 16;
            long num12 = q.W.RawValue * num3 + 32768L >> 16;
            FPMatrix4x4 fpMatrix4x4;
            fpMatrix4x4.M00.RawValue = 65536L - (num5 + num6);
            fpMatrix4x4.M10.RawValue = num7 + num12;
            fpMatrix4x4.M20.RawValue = num8 - num11;
            fpMatrix4x4.M30.RawValue = 0L;
            fpMatrix4x4.M01.RawValue = num7 - num12;
            fpMatrix4x4.M11.RawValue = 65536L - (num4 + num6);
            fpMatrix4x4.M21.RawValue = num9 + num10;
            fpMatrix4x4.M31.RawValue = 0L;
            fpMatrix4x4.M02.RawValue = num8 + num11;
            fpMatrix4x4.M12.RawValue = num9 - num10;
            fpMatrix4x4.M22.RawValue = 65536L - (num4 + num5);
            fpMatrix4x4.M32.RawValue = 0L;
            fpMatrix4x4.M03.RawValue = 0L;
            fpMatrix4x4.M13.RawValue = 0L;
            fpMatrix4x4.M23.RawValue = 0L;
            fpMatrix4x4.M33.RawValue = 65536L;
            return fpMatrix4x4;
        }
    }
}