using System;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>Represents an 2D axis aligned bounding box (AABB).</summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FPBounds2
    {
        /// <summary>
        ///     The size of the struct in-memory inside the Frame data-buffers or stack (when passed as value parameter).
        ///     Not related to the snapshot payload this occupies, which is bit-packed and compressed.
        /// </summary>
        public const int SIZE = 32;

        /// <summary>Center of the bounding box.</summary>
        [FieldOffset(0)] public FPVector2 Center;

        /// <summary>Extents of the bounding box (half of the size).</summary>
        [FieldOffset(16)] public FPVector2 Extents;

        /// <summary>
        ///     Gets or sets the maximal point of the box. This is always equal to
        ///     <see cref="F:Herta.FPBounds2.Center" /> + <see cref="F:Herta.FPBounds2.Extents" />.
        ///     Setting this property will not affect <see cref="P:Herta.FPBounds2.Min" />.
        /// </summary>
        public FPVector2 Max
        {
            get => this.Center + this.Extents;
            set => this.SetMinMax(this.Min, value);
        }

        /// <summary>
        ///     Gets or sets the minimal point of the box. This is always equal to
        ///     <see cref="F:Herta.FPBounds2.Center" /> - <see cref="F:Herta.FPBounds2.Extents" />.
        ///     Setting this property will not affect <see cref="P:Herta.FPBounds2.Max" />.
        /// </summary>
        public FPVector2 Min
        {
            get => this.Center - this.Extents;
            set => this.SetMinMax(value, this.Max);
        }

        /// <summary>
        ///     Create a new Bounds with the given center and extents.
        /// </summary>
        /// <param name="center">Center point.</param>
        /// <param name="extents">Extents (half the size).</param>
        public FPBounds2(FPVector2 center, FPVector2 extents)
        {
            this.Center = center;
            this.Extents = extents;
        }

        /// <summary>
        ///     Expand bounds by 0.5 * <paramref name="amount" /> in both directions.
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(FP amount) => this.Extents += new FPVector2(amount * FP._0_50, amount * FP._0_50);

        /// <summary>
        ///     Expand bounds by 0.5 * <paramref name="amount" /> in both directions.
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(FPVector2 amount) => this.Extents += amount * FP._0_50;

        /// <summary>Set the bounds to the given min and max points.</summary>
        /// <param name="min">Minimum position.</param>
        /// <param name="max">Maximum position.</param>
        public void SetMinMax(FPVector2 min, FPVector2 max)
        {
            this.Extents = (max - min) * FP._0_50;
            this.Center = min + this.Extents;
        }

        /// <summary>
        ///     Expand bounds to contain <paramref name="point" /> (if needed).
        /// </summary>
        /// <param name="point"></param>
        public void Encapsulate(FPVector2 point) => this.SetMinMax(FPVector2.Min(this.Min, point), FPVector2.Max(this.Max, point));

        /// <summary>
        ///     Expand bounds to contain <paramref name="bounds" /> (if needed).
        /// </summary>
        /// <param name="bounds"></param>
        public void Encapsulate(FPBounds2 bounds)
        {
            this.Encapsulate(bounds.Center - bounds.Extents);
            this.Encapsulate(bounds.Center + bounds.Extents);
        }

        /// <summary>
        ///     Returns <see langword="true" /> if there is an intersection between bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool Intersects(FPBounds2 bounds) => this.Min.X <= bounds.Max.X && this.Max.X >= bounds.Min.X && this.Min.Y <= bounds.Max.Y && this.Max.Y >= bounds.Min.Y;

        /// <summary>
        ///     Returns <see langword="true" /> if the <paramref name="point" /> is inside the bounds.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(FPVector2 point)
        {
            FP fp1 = FPMath.Abs(point.X - this.Center.X);
            FP fp2 = FPMath.Abs(point.Y - this.Center.Y);
            return fp1 <= this.Extents.X && fp2 <= this.Extents.Y;
        }

        /// <summary>Computes the hash code of the FPBounds2 instance.</summary>
        /// <returns>The hash code of the FPBounds2 object.</returns>
        public override int GetHashCode() => XxHash.Hash32(this);
    }
}