using System;
using System.Globalization;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     Represents 2x2 column major matrix, which can be used for 2D scaling and rotation.
    ///     Each cell can be individually accessed as a field (M&lt;row&gt;&lt;column&gt;).
    /// </summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FPMatrix2x2
    {
        /// <summary>
        ///     The size of the struct in-memory inside the Frame data-buffers or stack (when passed as value parameter).
        ///     Not related to the snapshot payload this occupies, which is bit-packed and compressed.
        /// </summary>
        public const int SIZE = 32;

        /// <summary>
        ///     The value of the element at the first row and first column of a 2x2 matrix.
        /// </summary>
        [FieldOffset(0)] public FP M00;

        /// <summary>
        ///     The value of the element at the second row and second column of a 2x2 matrix.
        /// </summary>
        [FieldOffset(8)] public FP M10;

        /// <summary>
        ///     The value of the element at the first row and second column of a 2x2 matrix.
        /// </summary>
        [FieldOffset(16)] public FP M01;

        /// <summary>
        ///     The value of the element at the second row and first column of a 2x2 matrix.
        /// </summary>
        [FieldOffset(24)] public FP M11;

        /// <summary>Matrix with 0s in every cell.</summary>
        public static readonly FPMatrix2x2 Zero = new FPMatrix2x2();

        /// <summary>
        ///     Matrix with 1s in the main diagonal and 0s in all other cells.
        /// </summary>
        public static readonly FPMatrix2x2 Identity = new FPMatrix2x2()
        {
            M00 =
            {
                RawValue = 65536
            },
            M11 =
            {
                RawValue = 65536
            }
        };

        /// <summary>
        ///     Create from columns - first two values set the first row, second two values - second row.
        /// </summary>
        public static FPMatrix2x2 FromRows(FP m00, FP m01, FP m10, FP m11)
        {
            FPMatrix2x2 fpMatrix2x2;
            fpMatrix2x2.M00 = m00;
            fpMatrix2x2.M10 = m10;
            fpMatrix2x2.M01 = m01;
            fpMatrix2x2.M11 = m11;
            return fpMatrix2x2;
        }

        /// <summary>
        ///     Create from rows - first vector set the first row, second vector set the second row.
        /// </summary>
        public static FPMatrix2x2 FromRows(FPVector2 row0, FPVector2 row1)
        {
            FPMatrix2x2 fpMatrix2x2;
            fpMatrix2x2.M00 = row0.X;
            fpMatrix2x2.M10 = row1.X;
            fpMatrix2x2.M01 = row0.Y;
            fpMatrix2x2.M11 = row1.Y;
            return fpMatrix2x2;
        }

        /// <summary>
        ///     Create from columns - first two values set the first colunn, second two values - second column.
        /// </summary>
        public static FPMatrix2x2 FromColumns(FP m00, FP m10, FP m01, FP m11)
        {
            FPMatrix2x2 fpMatrix2x2;
            fpMatrix2x2.M00 = m00;
            fpMatrix2x2.M10 = m10;
            fpMatrix2x2.M01 = m01;
            fpMatrix2x2.M11 = m11;
            return fpMatrix2x2;
        }

        /// <summary>
        ///     Create from columns - first vector set the first column, second vector set second column.
        /// </summary>
        public static FPMatrix2x2 FromColumns(FPVector2 column0, FPVector2 column1)
        {
            FPMatrix2x2 fpMatrix2x2;
            fpMatrix2x2.M00 = column0.X;
            fpMatrix2x2.M10 = column0.Y;
            fpMatrix2x2.M01 = column1.X;
            fpMatrix2x2.M11 = column1.Y;
            return fpMatrix2x2;
        }

        /// <summary>Creates a rotation matrix.</summary>
        /// <param name="rotation">Rotation in radians.</param>
        public static FPMatrix2x2 Rotate(FP rotation)
        {
            FPMatrix2x2 fpMatrix2x2;
            FPMath.SinCos(rotation, out fpMatrix2x2.M01, out fpMatrix2x2.M00);
            fpMatrix2x2.M10 = -fpMatrix2x2.M01;
            fpMatrix2x2.M11 = fpMatrix2x2.M00;
            return fpMatrix2x2;
        }

        /// <summary>
        ///     Returns <see langword="true" /> if this matrix is equal to the
        ///     <see cref="P:Herta.FPMatrix2x2.Identity" /> matrix
        /// </summary>
        public bool IsIdentity => this.M00.RawValue == 65536L && this.M11.RawValue == 65536L && (this.M01.RawValue | this.M10.RawValue) == 0L;

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
                        return this.M01;
                    case 3:
                        return this.M11;
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
                        this.M01 = value;
                        break;
                    case 3:
                        this.M11 = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>Creates a scaling matrix.</summary>
        public static FPMatrix2x2 Scale(FPVector2 scale) => FPMatrix2x2.FromColumns(scale.X, (FP)0, (FP)0, scale.Y);

        /// <summary>Transforms a direction by this matrix.</summary>
        public FPVector2 MultiplyVector(FPVector2 v)
        {
            FPVector2 fpVector2;
            fpVector2.X.RawValue = (this.M00.RawValue * v.X.RawValue + 32768L >> 16) + (this.M01.RawValue * v.Y.RawValue + 32768L >> 16);
            fpVector2.Y.RawValue = (this.M10.RawValue * v.X.RawValue + 32768L >> 16) + (this.M11.RawValue * v.Y.RawValue + 32768L >> 16);
            return fpVector2;
        }

        /// <summary>
        ///     Returns a string representation of the current FPMatrix2x2 object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current FPMatrix2x2 object. The string is formatted as "(({0}, {1}), ({2}, {3}))"
        ///     where {0} represents the value of M00, {1} represents the value of M01, {2} represents the value of M10, and {3}
        ///     represents the value of M11. The values are formatted using the InvariantCulture.
        /// </returns>
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
            builder.Append('(');
            builder.AppendFormattable(this.M00.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M01.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(')');
            builder.Append(',');
            builder.Append(' ');
            builder.Append('(');
            builder.AppendFormattable(this.M10.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(',');
            builder.Append(' ');
            builder.AppendFormattable(this.M11.AsFloat, default, (IFormatProvider)CultureInfo.InvariantCulture);
            builder.Append(')');
            builder.Append(')');
        }

        /// <summary>Calculates the hash code for the FPMatrix2x2 object.</summary>
        /// <returns>The hash code value for the current instance.</returns>
        public override int GetHashCode() => XxHash.Hash32(this);

        /// <summary>Adds two matrices.</summary>
        public static FPMatrix2x2 operator +(FPMatrix2x2 a, FPMatrix2x2 b)
        {
            a.M00.RawValue += b.M00.RawValue;
            a.M01.RawValue += b.M01.RawValue;
            a.M10.RawValue += b.M10.RawValue;
            a.M11.RawValue += b.M11.RawValue;
            return a;
        }

        /// <summary>Subtracts two matrices.</summary>
        public static FPMatrix2x2 operator -(FPMatrix2x2 a, FPMatrix2x2 b)
        {
            a.M00.RawValue -= b.M00.RawValue;
            a.M01.RawValue -= b.M01.RawValue;
            a.M10.RawValue -= b.M10.RawValue;
            a.M11.RawValue -= b.M11.RawValue;
            return a;
        }

        /// <summary>Multiplies two matrices.</summary>
        public static FPMatrix2x2 operator *(FPMatrix2x2 a, FPMatrix2x2 b)
        {
            FPMatrix2x2 fpMatrix2x2;
            fpMatrix2x2.M00.RawValue = (a.M00.RawValue * b.M00.RawValue + 32768L >> 16) + (a.M01.RawValue * b.M10.RawValue + 32768L >> 16);
            fpMatrix2x2.M01.RawValue = (a.M00.RawValue * b.M01.RawValue + 32768L >> 16) + (a.M01.RawValue * b.M11.RawValue + 32768L >> 16);
            fpMatrix2x2.M10.RawValue = (a.M10.RawValue * b.M00.RawValue + 32768L >> 16) + (a.M11.RawValue * b.M10.RawValue + 32768L >> 16);
            fpMatrix2x2.M11.RawValue = (a.M10.RawValue * b.M01.RawValue + 32768L >> 16) + (a.M11.RawValue * b.M11.RawValue + 32768L >> 16);
            return fpMatrix2x2;
        }

        /// <summary>Multiplies a vector by a matrix.</summary>
        public static FPVector2 operator *(FPMatrix2x2 m, FPVector2 vector)
        {
            FPVector2 fpVector2;
            fpVector2.X.RawValue = (m.M00.RawValue * vector.X.RawValue + 32768L >> 16) + (m.M01.RawValue * vector.Y.RawValue + 32768L >> 16);
            fpVector2.Y.RawValue = (m.M10.RawValue * vector.X.RawValue + 32768L >> 16) + (m.M11.RawValue * vector.Y.RawValue + 32768L >> 16);
            return fpVector2;
        }

        /// <summary>Multiplies a matrix by a factor.</summary>
        public static FPMatrix2x2 operator *(FP a, FPMatrix2x2 m)
        {
            m.M00.RawValue = a.RawValue * m.M00.RawValue + 32768L >> 16;
            m.M01.RawValue = a.RawValue * m.M01.RawValue + 32768L >> 16;
            m.M10.RawValue = a.RawValue * m.M10.RawValue + 32768L >> 16;
            m.M11.RawValue = a.RawValue * m.M11.RawValue + 32768L >> 16;
            return m;
        }

        /// <summary>Attempts to get a scale value from the matrix.</summary>
        public FPVector2 LossyScale
        {
            get
            {
                long x1 = (this.M00.RawValue * this.M00.RawValue + 32768L >> 16) + (this.M10.RawValue * this.M10.RawValue + 32768L >> 16);
                long x2 = (this.M01.RawValue * this.M01.RawValue + 32768L >> 16) + (this.M11.RawValue * this.M11.RawValue + 32768L >> 16);
                return new FPVector2(FP.FromRaw(FPMath.SqrtRaw(x1) * (long)FPMath.SignInt(this.Determinant)), FP.FromRaw(FPMath.SqrtRaw(x2)));
            }
        }

        /// <summary>
        ///     Creates inverted matrix. Matrix with determinant 0 can not be inverted and result with
        ///     <see cref="P:Herta.FPMatrix2x2.Zero" />.
        /// </summary>
        public FPMatrix2x2 Inverted
        {
            get
            {
                long num1 = (this.M00.RawValue * this.M11.RawValue + 32768L >> 16) - (this.M10.RawValue * this.M01.RawValue + 32768L >> 16);
                if (num1 == 0L)
                    return FPMatrix2x2.Zero;
                long num2 = 4294967296L / num1;
                FPMatrix2x2 inverted;
                inverted.M00.RawValue = this.M11.RawValue * num2 + 32768L >> 16;
                inverted.M01.RawValue = -(this.M01.RawValue * num2 + 32768L >> 16);
                inverted.M10.RawValue = -(this.M10.RawValue * num2 + 32768L >> 16);
                inverted.M11.RawValue = this.M00.RawValue * num2 + 32768L >> 16;
                return inverted;
            }
        }

        /// <summary>Calculates determinant of this matrix.</summary>
        public FP Determinant => FP.FromRaw((this.M00.RawValue * this.M11.RawValue + 32768L >> 16) - (this.M10.RawValue * this.M01.RawValue + 32768L >> 16));
    }
}