using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     Represents 3x3 column major matrix.
    ///     Each cell can be individually accessed as a field (M&lt;row&gt;&lt;column&gt;), with indexing
    ///     indexing property[row, column] or with indexing property[index].
    /// </summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FPMatrix3x3
    {
        /// <summary>
        ///     The size of the struct in-memory inside the Frame data-buffers or stack (when passed as value parameter).
        ///     Not related to the snapshot payload this occupies, which is bit-packed and compressed.
        /// </summary>
        public const int SIZE = 72;

        /// <summary>First row, first column</summary>
        [FieldOffset(0)] public FP M00;

        /// <summary>Second row, first column</summary>
        [FieldOffset(8)] public FP M10;

        /// <summary>Third row, first column</summary>
        [FieldOffset(16)] public FP M20;

        /// <summary>First row, second column</summary>
        [FieldOffset(24)] public FP M01;

        /// <summary>Second row, second column</summary>
        [FieldOffset(32)] public FP M11;

        /// <summary>Third row, second column</summary>
        [FieldOffset(40)] public FP M21;

        /// <summary>First row, third column</summary>
        [FieldOffset(48)] public FP M02;

        /// <summary>Second row, third column</summary>
        [FieldOffset(56)] public FP M12;

        /// <summary>Third row, third column</summary>
        [FieldOffset(64)] public FP M22;

        /// <summary>Matrix with 0s in every cell.</summary>
        public static readonly FPMatrix3x3 Zero = new FPMatrix3x3();

        /// <summary>
        ///     Matrix with 1s in the main diagonal and 0s in all other cells.
        /// </summary>
        public static readonly FPMatrix3x3 Identity = new FPMatrix3x3()
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
            }
        };

        /// <summary>
        ///     Create from rows - first three values set the first row, second three values - second row etc.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPMatrix3x3 FromRows(
            FP m00,
            FP m01,
            FP m02,
            FP m10,
            FP m11,
            FP m12,
            FP m20,
            FP m21,
            FP m22)
        {
            FPMatrix3x3 fpMatrix3x3;
            fpMatrix3x3.M00 = m00;
            fpMatrix3x3.M10 = m10;
            fpMatrix3x3.M20 = m20;
            fpMatrix3x3.M01 = m01;
            fpMatrix3x3.M11 = m11;
            fpMatrix3x3.M21 = m21;
            fpMatrix3x3.M02 = m02;
            fpMatrix3x3.M12 = m12;
            fpMatrix3x3.M22 = m22;
            return fpMatrix3x3;
        }

        /// <summary>
        ///     Create from rows - the first vector set the first row, second vector - second row etc.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPMatrix3x3 FromRows(FPVector3 row0, FPVector3 row1, FPVector3 row2)
        {
            FPMatrix3x3 fpMatrix3x3;
            fpMatrix3x3.M00 = row0.X;
            fpMatrix3x3.M10 = row1.X;
            fpMatrix3x3.M20 = row2.X;
            fpMatrix3x3.M01 = row0.Y;
            fpMatrix3x3.M11 = row1.Y;
            fpMatrix3x3.M21 = row2.Y;
            fpMatrix3x3.M02 = row0.Z;
            fpMatrix3x3.M12 = row1.Z;
            fpMatrix3x3.M22 = row2.Z;
            return fpMatrix3x3;
        }

        /// <summary>
        ///     Create from columns - first three values set the first column, second three values - second column etc.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPMatrix3x3 FromColumns(
            FP m00,
            FP m10,
            FP m20,
            FP m01,
            FP m11,
            FP m21,
            FP m02,
            FP m12,
            FP m22)
        {
            FPMatrix3x3 fpMatrix3x3;
            fpMatrix3x3.M00 = m00;
            fpMatrix3x3.M10 = m10;
            fpMatrix3x3.M20 = m20;
            fpMatrix3x3.M01 = m01;
            fpMatrix3x3.M11 = m11;
            fpMatrix3x3.M21 = m21;
            fpMatrix3x3.M02 = m02;
            fpMatrix3x3.M12 = m12;
            fpMatrix3x3.M22 = m22;
            return fpMatrix3x3;
        }

        /// <summary>
        ///     Create from columns - the first vector set the first column, second vector - second column etc.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPMatrix3x3 FromColumns(
            FPVector3 column0,
            FPVector3 column1,
            FPVector3 column2)
        {
            FPMatrix3x3 fpMatrix3x3;
            fpMatrix3x3.M00 = column0.X;
            fpMatrix3x3.M10 = column0.Y;
            fpMatrix3x3.M20 = column0.Z;
            fpMatrix3x3.M01 = column1.X;
            fpMatrix3x3.M11 = column1.Y;
            fpMatrix3x3.M21 = column1.Z;
            fpMatrix3x3.M02 = column2.X;
            fpMatrix3x3.M12 = column2.Y;
            fpMatrix3x3.M22 = column2.Z;
            return fpMatrix3x3;
        }

        /// <summary>Gets or sets cell M&lt;row&gt;&lt;column&gt;.</summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public FP this[int row, int column]
        {
            get => this[row + column * 3];
            set => this[row + column * 3] = value;
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
                        return this.M01;
                    case 4:
                        return this.M11;
                    case 5:
                        return this.M21;
                    case 6:
                        return this.M02;
                    case 7:
                        return this.M12;
                    case 8:
                        return this.M22;
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
                        this.M01 = value;
                        break;
                    case 4:
                        this.M11 = value;
                        break;
                    case 5:
                        this.M21 = value;
                        break;
                    case 6:
                        this.M02 = value;
                        break;
                    case 7:
                        this.M12 = value;
                        break;
                    case 8:
                        this.M22 = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>Creates transposed matrix.</summary>
        public FPMatrix3x3 Transposed => FPMatrix3x3.FromColumns(this.M00, this.M01, this.M02, this.M10, this.M11, this.M12, this.M20, this.M21, this.M22);

        /// <summary>
        ///     Returns <see langword="true" /> if this matrix is equal to the
        ///     <see cref="P:Herta.FPMatrix3x3.Identity" /> matrix
        /// </summary>
        public bool IsIdentity => this.M00.RawValue == 65536L && this.M11.RawValue == 65536L && this.M22.RawValue == 65536L && (this.M01.RawValue | this.M02.RawValue | this.M10.RawValue | this.M12.RawValue | this.M20.RawValue | this.M21.RawValue) == 0L;

        /// <summary>Creates a scaling matrix.</summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static FPMatrix3x3 Scale(FPVector3 scale) => FPMatrix3x3.FromColumns(scale.X, (FP)0, (FP)0, (FP)0, scale.Y, (FP)0, (FP)0, (FP)0, scale.Z);

        /// <summary>
        ///     Converts the FPMatrix3x3 to a string representation.
        ///     The returned string has the format "(({0}, {1}, {2}), ({3}, {4}, {5}), ({6}, {7}, {8}))",
        ///     where {0} to {8} are the formatted string representations of the matrix elements.
        /// </summary>
        /// <returns>A string representation of the FPMatrix3x3.</returns>
        public override string ToString() => string.Format((IFormatProvider)CultureInfo.InvariantCulture, "(({0}, {1}, {2}), ({3}, {4}, {5}), ({6}, {7}, {8}))", (object)this.M00.AsFloat, (object)this.M01.AsFloat, (object)this.M02.AsFloat, (object)this.M10.AsFloat, (object)this.M11.AsFloat, (object)this.M12.AsFloat, (object)this.M20.AsFloat, (object)this.M21.AsFloat, (object)this.M22.AsFloat);

        /// <summary>
        ///     Calculates the hash code for the current FPMatrix3x3 instance.
        /// </summary>
        /// <returns>The calculated hash code.</returns>
        public override int GetHashCode() => XxHash.Hash32(this);

        /// <summary>Adds two matrices.</summary>
        public static FPMatrix3x3 operator +(FPMatrix3x3 a, FPMatrix3x3 b)
        {
            a.M00.RawValue += b.M00.RawValue;
            a.M01.RawValue += b.M01.RawValue;
            a.M02.RawValue += b.M02.RawValue;
            a.M10.RawValue += b.M10.RawValue;
            a.M11.RawValue += b.M11.RawValue;
            a.M12.RawValue += b.M12.RawValue;
            a.M20.RawValue += b.M20.RawValue;
            a.M21.RawValue += b.M21.RawValue;
            a.M22.RawValue += b.M22.RawValue;
            return a;
        }

        /// <summary>Subtracts two matrices.</summary>
        public static FPMatrix3x3 operator -(FPMatrix3x3 a, FPMatrix3x3 b)
        {
            a.M00.RawValue -= b.M00.RawValue;
            a.M01.RawValue -= b.M01.RawValue;
            a.M02.RawValue -= b.M02.RawValue;
            a.M10.RawValue -= b.M10.RawValue;
            a.M11.RawValue -= b.M11.RawValue;
            a.M12.RawValue -= b.M12.RawValue;
            a.M20.RawValue -= b.M20.RawValue;
            a.M21.RawValue -= b.M21.RawValue;
            a.M22.RawValue -= b.M22.RawValue;
            return a;
        }

        /// <summary>Multiplies two matrices.</summary>
        public static FPMatrix3x3 operator *(FPMatrix3x3 a, FPMatrix3x3 b)
        {
            FPMatrix3x3 fpMatrix3x3;
            fpMatrix3x3.M00.RawValue = (a.M00.RawValue * b.M00.RawValue + 32768L >> 16) + (a.M01.RawValue * b.M10.RawValue + 32768L >> 16) + (a.M02.RawValue * b.M20.RawValue + 32768L >> 16);
            fpMatrix3x3.M01.RawValue = (a.M00.RawValue * b.M01.RawValue + 32768L >> 16) + (a.M01.RawValue * b.M11.RawValue + 32768L >> 16) + (a.M02.RawValue * b.M21.RawValue + 32768L >> 16);
            fpMatrix3x3.M02.RawValue = (a.M00.RawValue * b.M02.RawValue + 32768L >> 16) + (a.M01.RawValue * b.M12.RawValue + 32768L >> 16) + (a.M02.RawValue * b.M22.RawValue + 32768L >> 16);
            fpMatrix3x3.M10.RawValue = (a.M10.RawValue * b.M00.RawValue + 32768L >> 16) + (a.M11.RawValue * b.M10.RawValue + 32768L >> 16) + (a.M12.RawValue * b.M20.RawValue + 32768L >> 16);
            fpMatrix3x3.M11.RawValue = (a.M10.RawValue * b.M01.RawValue + 32768L >> 16) + (a.M11.RawValue * b.M11.RawValue + 32768L >> 16) + (a.M12.RawValue * b.M21.RawValue + 32768L >> 16);
            fpMatrix3x3.M12.RawValue = (a.M10.RawValue * b.M02.RawValue + 32768L >> 16) + (a.M11.RawValue * b.M12.RawValue + 32768L >> 16) + (a.M12.RawValue * b.M22.RawValue + 32768L >> 16);
            fpMatrix3x3.M20.RawValue = (a.M20.RawValue * b.M00.RawValue + 32768L >> 16) + (a.M21.RawValue * b.M10.RawValue + 32768L >> 16) + (a.M22.RawValue * b.M20.RawValue + 32768L >> 16);
            fpMatrix3x3.M21.RawValue = (a.M20.RawValue * b.M01.RawValue + 32768L >> 16) + (a.M21.RawValue * b.M11.RawValue + 32768L >> 16) + (a.M22.RawValue * b.M21.RawValue + 32768L >> 16);
            fpMatrix3x3.M22.RawValue = (a.M20.RawValue * b.M02.RawValue + 32768L >> 16) + (a.M21.RawValue * b.M12.RawValue + 32768L >> 16) + (a.M22.RawValue * b.M22.RawValue + 32768L >> 16);
            return fpMatrix3x3;
        }

        /// <summary>Multiplies a vector by a matrix.</summary>
        public static FPVector3 operator *(FPMatrix3x3 m, FPVector3 vector)
        {
            FPVector3 fpVector3;
            fpVector3.X.RawValue = (m.M00.RawValue * vector.X.RawValue + 32768L >> 16) + (m.M01.RawValue * vector.Y.RawValue + 32768L >> 16) + (m.M02.RawValue * vector.Z.RawValue + 32768L >> 16);
            fpVector3.Y.RawValue = (m.M10.RawValue * vector.X.RawValue + 32768L >> 16) + (m.M11.RawValue * vector.Y.RawValue + 32768L >> 16) + (m.M12.RawValue * vector.Z.RawValue + 32768L >> 16);
            fpVector3.Z.RawValue = (m.M20.RawValue * vector.X.RawValue + 32768L >> 16) + (m.M21.RawValue * vector.Y.RawValue + 32768L >> 16) + (m.M22.RawValue * vector.Z.RawValue + 32768L >> 16);
            return fpVector3;
        }

        /// <summary>Multiplies a matrix by a factor.</summary>
        public static FPMatrix3x3 operator *(FP a, FPMatrix3x3 m)
        {
            m.M00.RawValue = a.RawValue * m.M00.RawValue + 32768L >> 16;
            m.M01.RawValue = a.RawValue * m.M01.RawValue + 32768L >> 16;
            m.M02.RawValue = a.RawValue * m.M02.RawValue + 32768L >> 16;
            m.M10.RawValue = a.RawValue * m.M10.RawValue + 32768L >> 16;
            m.M11.RawValue = a.RawValue * m.M11.RawValue + 32768L >> 16;
            m.M12.RawValue = a.RawValue * m.M12.RawValue + 32768L >> 16;
            m.M20.RawValue = a.RawValue * m.M20.RawValue + 32768L >> 16;
            m.M21.RawValue = a.RawValue * m.M21.RawValue + 32768L >> 16;
            m.M22.RawValue = a.RawValue * m.M22.RawValue + 32768L >> 16;
            return m;
        }

        /// <summary>Attempts to get a scale value from the matrix.</summary>
        public FPVector3 LossyScale
        {
            get
            {
                long x1 = (this.M00.RawValue * this.M00.RawValue + 32768L >> 16) + (this.M10.RawValue * this.M10.RawValue + 32768L >> 16) + (this.M20.RawValue * this.M20.RawValue + 32768L >> 16);
                long x2 = (this.M01.RawValue * this.M01.RawValue + 32768L >> 16) + (this.M11.RawValue * this.M11.RawValue + 32768L >> 16) + (this.M21.RawValue * this.M21.RawValue + 32768L >> 16);
                long x3 = (this.M02.RawValue * this.M02.RawValue + 32768L >> 16) + (this.M12.RawValue * this.M12.RawValue + 32768L >> 16) + (this.M22.RawValue * this.M22.RawValue + 32768L >> 16);
                return new FPVector3(FP.FromRaw(FPMath.SqrtRaw(x1) * (long)FPMath.SignInt(this.Determinant)), FP.FromRaw(FPMath.SqrtRaw(x2)), FP.FromRaw(FPMath.SqrtRaw(x3)));
            }
        }

        /// <summary>
        ///     Creates inverted matrix. Matrix with determinant 0 can not be inverted and result with
        ///     <see cref="P:Herta.FPMatrix3x3.Zero" />.
        /// </summary>
        public FPMatrix3x3 Inverted
        {
            get
            {
                long num1 = (this.M11.RawValue * this.M22.RawValue + 32768L >> 16) - (this.M12.RawValue * this.M21.RawValue + 32768L >> 16);
                long num2 = (this.M10.RawValue * this.M22.RawValue + 32768L >> 16) - (this.M12.RawValue * this.M20.RawValue + 32768L >> 16);
                long num3 = (this.M10.RawValue * this.M21.RawValue + 32768L >> 16) - (this.M11.RawValue * this.M20.RawValue + 32768L >> 16);
                long num4 = (this.M00.RawValue * num1 + 32768L >> 16) - (this.M01.RawValue * num2 + 32768L >> 16) + (this.M02.RawValue * num3 + 32768L >> 16);
                if (num4 == 0L)
                    return FPMatrix3x3.Zero;
                long num5 = 4294967296L / num4;
                FPMatrix3x3 inverted;
                inverted.M00.RawValue = num1 * num5 + 32768L >> 16;
                inverted.M01.RawValue = -(((this.M01.RawValue * this.M22.RawValue + 32768L >> 16) - (this.M02.RawValue * this.M21.RawValue + 32768L >> 16)) * num5 + 32768L >> 16);
                inverted.M02.RawValue = ((this.M01.RawValue * this.M12.RawValue + 32768L >> 16) - (this.M02.RawValue * this.M11.RawValue + 32768L >> 16)) * num5 + 32768L >> 16;
                inverted.M10.RawValue = -(num2 * num5 + 32768L >> 16);
                inverted.M11.RawValue = ((this.M00.RawValue * this.M22.RawValue + 32768L >> 16) - (this.M02.RawValue * this.M20.RawValue + 32768L >> 16)) * num5 + 32768L >> 16;
                inverted.M12.RawValue = -(((this.M00.RawValue * this.M12.RawValue + 32768L >> 16) - (this.M02.RawValue * this.M10.RawValue + 32768L >> 16)) * num5 + 32768L >> 16);
                inverted.M20.RawValue = num3 * num5 + 32768L >> 16;
                inverted.M21.RawValue = -(((this.M00.RawValue * this.M21.RawValue + 32768L >> 16) - (this.M01.RawValue * this.M20.RawValue + 32768L >> 16)) * num5 + 32768L >> 16);
                inverted.M22.RawValue = ((this.M00.RawValue * this.M11.RawValue + 32768L >> 16) - (this.M01.RawValue * this.M10.RawValue + 32768L >> 16)) * num5 + 32768L >> 16;
                return inverted;
            }
        }

        /// <summary>Calculates determinant of this matrix.</summary>
        public FP Determinant => FP.FromRaw((this.M00.RawValue * ((this.M11.RawValue * this.M22.RawValue + 32768L >> 16) - (this.M12.RawValue * this.M21.RawValue + 32768L >> 16)) + 32768L >> 16) - (this.M01.RawValue * ((this.M10.RawValue * this.M22.RawValue + 32768L >> 16) - (this.M12.RawValue * this.M20.RawValue + 32768L >> 16)) + 32768L >> 16) + (this.M02.RawValue * ((this.M10.RawValue * this.M21.RawValue + 32768L >> 16) - (this.M11.RawValue * this.M20.RawValue + 32768L >> 16)) + 32768L >> 16));

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
        ///     Creates a rotation matrix. Rotation is expected to be normalized.
        /// </summary>
        public static FPMatrix3x3 Rotate(FPQuaternion q)
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
            FPMatrix3x3 fpMatrix3x3;
            fpMatrix3x3.M00.RawValue = 65536L - (num5 + num6);
            fpMatrix3x3.M10.RawValue = num7 + num12;
            fpMatrix3x3.M20.RawValue = num8 - num11;
            fpMatrix3x3.M01.RawValue = num7 - num12;
            fpMatrix3x3.M11.RawValue = 65536L - (num4 + num6);
            fpMatrix3x3.M21.RawValue = num9 + num10;
            fpMatrix3x3.M02.RawValue = num8 + num11;
            fpMatrix3x3.M12.RawValue = num9 - num10;
            fpMatrix3x3.M22.RawValue = 65536L - (num4 + num5);
            return fpMatrix3x3;
        }

        /// <summary>
        ///     Creates a rotation and scaling matrix.
        ///     Rotation is expected to be normalized.
        /// </summary>
        public static FPMatrix3x3 RotateScale(FPQuaternion q, FPVector3 s)
        {
            FPMatrix3x3 fpMatrix3x3 = FPMatrix3x3.Rotate(q);
            fpMatrix3x3.M00.RawValue = fpMatrix3x3.M00.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix3x3.M10.RawValue = fpMatrix3x3.M10.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix3x3.M20.RawValue = fpMatrix3x3.M20.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix3x3.M01.RawValue = fpMatrix3x3.M01.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix3x3.M11.RawValue = fpMatrix3x3.M11.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix3x3.M21.RawValue = fpMatrix3x3.M21.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix3x3.M02.RawValue = fpMatrix3x3.M02.RawValue * s.Z.RawValue + 32768L >> 16;
            fpMatrix3x3.M12.RawValue = fpMatrix3x3.M12.RawValue * s.Z.RawValue + 32768L >> 16;
            fpMatrix3x3.M22.RawValue = fpMatrix3x3.M22.RawValue * s.Z.RawValue + 32768L >> 16;
            return fpMatrix3x3;
        }

        /// <summary>
        ///     Creates an inverse rotation and scaling matrix. This is significantly faster than inverting a RotateScale matrix.
        ///     Rotation is expected to be normalized.
        /// </summary>
        public static FPMatrix3x3 InverseRotateScale(FPQuaternion q, FPVector3 s)
        {
            FPMatrix3x3 fpMatrix3x3 = FPMatrix3x3.Rotate(q.Conjugated);
            fpMatrix3x3.M00.RawValue = fpMatrix3x3.M00.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix3x3.M01.RawValue = fpMatrix3x3.M01.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix3x3.M02.RawValue = fpMatrix3x3.M02.RawValue * s.X.RawValue + 32768L >> 16;
            fpMatrix3x3.M10.RawValue = fpMatrix3x3.M10.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix3x3.M11.RawValue = fpMatrix3x3.M11.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix3x3.M12.RawValue = fpMatrix3x3.M12.RawValue * s.Y.RawValue + 32768L >> 16;
            fpMatrix3x3.M20.RawValue = fpMatrix3x3.M20.RawValue * s.Z.RawValue + 32768L >> 16;
            fpMatrix3x3.M21.RawValue = fpMatrix3x3.M21.RawValue * s.Z.RawValue + 32768L >> 16;
            fpMatrix3x3.M22.RawValue = fpMatrix3x3.M22.RawValue * s.Z.RawValue + 32768L >> 16;
            return fpMatrix3x3;
        }
    }
}