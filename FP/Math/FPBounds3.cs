using System;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>Represents an 3D axis aligned bounding box (AABB).</summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FPBounds3
    {
        /// <summary>
        ///     ///
        ///     <summary>
        ///         The size of the struct in-memory inside the Frame data-buffers or stack (when passed as value parameter).
        ///         Not related to the snapshot payload this occupies, which is bit-packed and compressed.
        ///     </summary>
        /// </summary>
        public const int SIZE = 48;

        /// <summary>Center of the bounding box.</summary>
        [FieldOffset(0)] public FPVector3 Center;

        /// <summary>Extents of the bounding box (half of the size).</summary>
        [FieldOffset(24)] public FPVector3 Extents;

        /// <summary>
        ///     Gets or sets the maximal point of the box. This is always equal to
        ///     <see cref="F:Herta.FPBounds3.Center" /> + <see cref="F:Herta.FPBounds3.Extents" />.
        ///     Setting this property will not affect <see cref="P:Herta.FPBounds3.Min" />.
        /// </summary>
        public FPVector3 Max
        {
            get => this.Center + this.Extents;
            set => this.SetMinMax(this.Min, value);
        }

        /// <summary>
        ///     Gets or sets the minimal point of the box. This is always equal to
        ///     <see cref="F:Herta.FPBounds3.Center" /> - <see cref="F:Herta.FPBounds3.Extents" />.
        ///     Setting this property will not affect <see cref="P:Herta.FPBounds3.Max" />.
        /// </summary>
        public FPVector3 Min
        {
            get => this.Center - this.Extents;
            set => this.SetMinMax(value, this.Max);
        }

        /// <summary>
        ///     Create a new Bounds with the given center and extents.
        /// </summary>
        /// <param name="center">Center point.</param>
        /// <param name="extents">Extents (half the size).</param>
        public FPBounds3(FPVector3 center, FPVector3 extents)
        {
            this.Center = center;
            this.Extents = extents;
        }

        /// <summary>
        ///     Expand bounds by 0.5 * <paramref name="amount" /> in both directions.
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(FP amount) => this.Extents += new FPVector3(amount * FP._0_50, amount * FP._0_50, amount * FP._0_50);

        /// <summary>
        ///     Expand bounds by 0.5 * <paramref name="amount" /> in both directions.
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(FPVector3 amount) => this.Extents += amount * FP._0_50;

        /// <summary>
        ///     Set the bounds to the given <paramref name="min" /> and <paramref name="max" /> points.
        /// </summary>
        /// <param name="min">Minimum position.</param>
        /// <param name="max">Maximum position.</param>
        public void SetMinMax(FPVector3 min, FPVector3 max)
        {
            this.Extents = (max - min) * FP._0_50;
            this.Center = min + this.Extents;
        }

        /// <summary>
        ///     Expand bounds to contain <paramref name="point" /> (if needed).
        /// </summary>
        /// <param name="point"></param>
        public void Encapsulate(FPVector3 point) => this.SetMinMax(FPVector3.Min(this.Min, point), FPVector3.Max(this.Max, point));

        /// <summary>
        ///     Expand bounds to contain <paramref name="bounds" /> (if needed).
        /// </summary>
        /// <param name="bounds"></param>
        public void Encapsulate(FPBounds3 bounds)
        {
            this.Encapsulate(bounds.Center - bounds.Extents);
            this.Encapsulate(bounds.Center + bounds.Extents);
        }

        /// <summary>
        ///     Returns <see langword="true" /> if there is an intersection between bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool Intersects(FPBounds3 bounds) => this.Min.X <= bounds.Max.X && this.Max.X >= bounds.Min.X && this.Min.Y <= bounds.Max.Y && this.Max.Y >= bounds.Min.Y && this.Min.Z <= bounds.Max.Z && this.Max.Z >= bounds.Min.Z;

        /// <summary>
        ///     Returns <see langword="true" /> if the <paramref name="point" /> is inside the bounds.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(FPVector3 point)
        {
            FP fp1 = FPMath.Abs(point.X - this.Center.X);
            FP fp2 = FPMath.Abs(point.Y - this.Center.Y);
            FP fp3 = FPMath.Abs(point.Z - this.Center.Z);
            return fp1 <= this.Extents.X && fp2 <= this.Extents.Y && fp3 <= this.Extents.Z;
        }

        /// <summary>
        ///     Computes a hash code for the current instance of the <see cref="T:Herta.FPBounds3" /> class.
        /// </summary>
        /// <remarks>
        ///     The hash code is computed by combining the hash codes of the <see cref="F:Herta.FPBounds3.Center" />
        ///     and <see cref="F:Herta.FPBounds3.Extents" />.
        /// </remarks>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => (17 * 31 + this.Center.GetHashCode()) * 31 + this.Extents.GetHashCode();
    }
}