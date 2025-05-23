using System;

// ReSharper disable ALL

namespace Thief
{
    /// <summary>Represents a plane in three-dimensional space.</summary>
    /// <remarks>
    ///     A plane is defined by an origin point and a normal vector, which is perpendicular to the plane.
    ///     The equation of the plane is represented as a mathematical equation: Ax + By + Cz + D = 0,
    ///     where A, B, C, and D are coefficients that define the plane.
    /// </remarks>
    [Serializable]
    public struct Plane
    {
        /// <summary>Represents the coefficient for a plane.</summary>
        public FP equation0;

        /// <summary>Represents the coefficient for a plane.</summary>
        public FP equation1;

        /// <summary>Represents the coefficient for a plane.</summary>
        public FP equation2;

        /// <summary>Represents the equation coefficient for a plane.</summary>
        public FP equation3;

        /// <summary>
        ///     Represents the origin point of a plane in three-dimensional space.
        /// </summary>
        /// <remarks>
        ///     The origin point is the starting point or reference point for the plane. It is used to define the position of the
        ///     plane in space.
        /// </remarks>
        public FPVector3 origin;

        /// <summary>
        ///     Represents a normal vector in three-dimensional space.
        /// </summary>
        /// <remarks>
        ///     A normal vector is a vector that is perpendicular to a plane.
        ///     In the context of the Plane struct, the normal vector represents the direction that the plane faces.
        /// </remarks>
        public FPVector3 normal;

        /// <summary>
        ///     Represents a plane in three-dimensional space defined by an origin point and a normal vector.
        /// </summary>
        /// <remarks>
        ///     A plane is typically defined by a point on the plane and a normal vector perpendicular to the plane.
        ///     The equation of the plane is given by: equation0 * x + equation1 * y + equation2 * z + equation3 = 0
        /// </remarks>
        public Plane(FPVector3 origin, FPVector3 normal)
        {
            this.origin = origin;
            this.normal = normal;
            this.equation0 = normal.X;
            this.equation1 = normal.Y;
            this.equation2 = normal.Z;
            this.equation3 = -(normal.X * origin.X + normal.Y * origin.Y + normal.Z * origin.Z);
        }

        /// <summary>Creates a plane in three-dimensional space.</summary>
        public Plane(FPVector3 p1, FPVector3 p2, FPVector3 p3)
        {
            this.normal = FPVector3.Cross(p2 - p1, p3 - p1).Normalized;
            this.origin = p1;
            this.equation0 = this.normal.X;
            this.equation1 = this.normal.Y;
            this.equation2 = this.normal.Z;
            this.equation3 = -(this.normal.X * this.origin.X + this.normal.Y * this.origin.Y + this.normal.Z * this.origin.Z);
        }

        /// <summary>
        ///     Determines whether the plane is front-facing to a given direction.
        /// </summary>
        /// <param name="direction">The direction to compare the plane's normal vector to.</param>
        /// <returns><see langword="true" /> if the plane is front-facing to the direction, <see langword="false" /> otherwise.</returns>
        public bool IsFrontFacingTo(FPVector3 direction) => FPVector3.Dot(this.normal, direction) <= FP._0;

        /// <summary>
        ///     Returns the signed distance from a point to the plane.
        /// </summary>
        /// <param name="point">The point from which to calculate the distance.</param>
        /// <returns>The signed distance from the point to the plane.</returns>
        public FP SignedDistanceTo(FPVector3 point) => FPVector3.Dot(point, this.normal) + this.equation3;
    }
}