using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>A collection of collision helper functions.</summary>
    /// \ingroup MathAPI
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct FPCollision
    {
        private const int ClosestDistanceMaxShiftLeft = 4;
        private const int ClosestDistanceMaxShiftRight = 8;
        private const int ClosestDistanceShiftPerIterationLeft = 1;
        private const int ClosestDistanceShiftPerIterationRight = 2;
        private const long ClosestDistanceMinThresholdRaw = 8;
        private const long ClosestDistanceMaxThresholdRaw = 2147483647;

        /// <summary>
        ///     Returns the center of a triangle defined by three vertices.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static FPVector2 TriangleCenter(FPVector2 v0, FPVector2 v1, FPVector2 v2) => FPVector2.Lerp(FPVector2.Lerp(v0, v1, FP._0_50), v2, FP._0_50);

        private static bool ClosestPointOnLine(
            FPVector2 line_p0,
            FPVector2 line_p1,
            FPVector2 c_center,
            FP c_radius,
            out FPVector2 point)
        {
            FPVector2 a = c_center - line_p0;
            FPVector2 fpVector2 = c_center - line_p1;
            FPVector2 b = line_p1 - line_p0;
            FP fp1 = c_radius * c_radius;
            FP sqrMagnitude = b.SqrMagnitude;
            FP fp2 = FPVector2.Dot(a, b) / sqrMagnitude;
            if (fp2.RawValue < 0L && a.SqrMagnitude.RawValue > fp1.RawValue)
            {
                point = new FPVector2();
                return false;
            }

            if (fp2.RawValue > 65536L && fpVector2.SqrMagnitude.RawValue > fp1.RawValue)
            {
                point = new FPVector2();
                return false;
            }

            point = line_p0 + b * fp2;
            point.X = FPMath.Clamp(point.X, FPMath.Min(line_p0.X, line_p1.X), FPMath.Max(line_p0.X, line_p1.X));
            point.Y = FPMath.Clamp(point.Y, FPMath.Min(line_p0.Y, line_p1.Y), FPMath.Max(line_p0.Y, line_p1.Y));
            return true;
        }

        /// <summary>
        ///     Returns <see langword="true" /> if a point <paramref name="point" /> lies on a line crossing <paramref name="p1" />
        ///     and <paramref name="p2" />.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool IsPointOnLine(FPVector2 p1, FPVector2 p2, FPVector2 point)
        {
            long num1 = p1.X.RawValue - p2.X.RawValue;
            if (num1 == 0L)
            {
                if (p1.Y.RawValue != p2.Y.RawValue)
                    return point.X.RawValue == p1.X.RawValue;
                return point.X.RawValue == p1.X.RawValue && point.Y.RawValue == p1.Y.RawValue;
            }

            long num2 = (p1.Y.RawValue - p2.Y.RawValue << 16) / num1;
            long num3 = p2.Y.RawValue - (num2 * p2.X.RawValue + 32768L >> 16);
            return point.Y.RawValue == (num2 * point.X.RawValue + 32768L >> 16) + num3;
        }

        /// <summary>
        ///     Returns <see langword="true" /> if a point <paramref name="point" /> lies on a segment defined by
        ///     <paramref name="p1" /> and <paramref name="p2" />.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        internal static bool IsPointOnLineSegment(FPVector2 p1, FPVector2 p2, FPVector2 point)
        {
            if (((point.Y.RawValue - p1.Y.RawValue) * (p2.X.RawValue - p1.X.RawValue) + 32768L >> 16) - ((point.X.RawValue - p1.X.RawValue) * (p2.Y.RawValue - p1.Y.RawValue) + 32768L >> 16) != 0L)
                return false;
            long num1 = ((point.X.RawValue - p1.X.RawValue) * (p2.X.RawValue - p1.X.RawValue) + 32768L >> 16) + ((point.Y.RawValue - p1.Y.RawValue) * (p2.Y.RawValue - p1.Y.RawValue) + 32768L >> 16);
            if (num1 < 0L)
                return false;
            long num2 = ((p2.X.RawValue - p1.X.RawValue) * (p2.X.RawValue - p1.X.RawValue) + 32768L >> 16) + ((p2.Y.RawValue - p1.Y.RawValue) * (p2.Y.RawValue - p1.Y.RawValue) + 32768L >> 16);
            return num1 <= num2;
        }

        /// <summary>
        ///     Returns the closest point on a segment to a given point.
        /// </summary>
        /// <param name="point">The point to find the closest point on the segment to.</param>
        /// <param name="p1">The start point of the segment.</param>
        /// <param name="p2">The end point of the segment.</param>
        /// <returns>The closest point on the segment to the given point.</returns>
        public static FPVector3 ClosestPointOnSegment(
            FPVector3 point,
            FPVector3 p1,
            FPVector3 p2)
        {
            long num1 = p1.X.RawValue - p2.X.RawValue;
            long num2 = p1.Y.RawValue - p2.Y.RawValue;
            long num3 = p1.Z.RawValue - p2.Z.RawValue;
            long num4 = (num1 * num1 + 32768L >> 16) + (num2 * num2 + 32768L >> 16) + (num3 * num3 + 32768L >> 16);
            if (num4 == 0L)
                return p1;
            long num5 = Math.Max(0L, Math.Min(65536L, (((point.X.RawValue - p1.X.RawValue) * (p2.X.RawValue - p1.X.RawValue) + 32768L >> 16) + ((point.Y.RawValue - p1.Y.RawValue) * (p2.Y.RawValue - p1.Y.RawValue) + 32768L >> 16) + ((point.Z.RawValue - p1.Z.RawValue) * (p2.Z.RawValue - p1.Z.RawValue) + 32768L >> 16) << 16) / num4));
            FPVector3 fpVector3;
            fpVector3.X.RawValue = p1.X.RawValue + (num5 * (p2.X.RawValue - p1.X.RawValue) + 32768L >> 16);
            fpVector3.Y.RawValue = p1.Y.RawValue + (num5 * (p2.Y.RawValue - p1.Y.RawValue) + 32768L >> 16);
            fpVector3.Z.RawValue = p1.Z.RawValue + (num5 * (p2.Z.RawValue - p1.Z.RawValue) + 32768L >> 16);
            return fpVector3;
        }

        /// <summary>
        ///     Cast point <paramref name="point" /> on a line crossing <paramref name="p1" /> and <paramref name="p2" />.
        ///     The result is clamped to lie on a segment defined by <paramref name="p1" /> and <paramref name="p2" />.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static FPVector2 ClosestPointOnSegment(
            FPVector2 point,
            FPVector2 p1,
            FPVector2 p2)
        {
            long num1 = p1.X.RawValue - p2.X.RawValue;
            long num2 = p1.Y.RawValue - p2.Y.RawValue;
            long num3 = (num1 * num1 + 32768L >> 16) + (num2 * num2 + 32768L >> 16);
            if (num3 == 0L)
                return p1;
            long num4 = Math.Max(0L, Math.Min(65536L, (((point.X.RawValue - p1.X.RawValue) * (p2.X.RawValue - p1.X.RawValue) + 32768L >> 16) + ((point.Y.RawValue - p1.Y.RawValue) * (p2.Y.RawValue - p1.Y.RawValue) + 32768L >> 16) << 16) / num3));
            FPVector2 fpVector2;
            fpVector2.X.RawValue = p1.X.RawValue + (num4 * (p2.X.RawValue - p1.X.RawValue) + 32768L >> 16);
            fpVector2.Y.RawValue = p1.Y.RawValue + (num4 * (p2.Y.RawValue - p1.Y.RawValue) + 32768L >> 16);
            return fpVector2;
        }

        /// <summary>
        ///     Clamps a 2D point to an axis-aligned bounding box (AABB).
        /// </summary>
        /// <param name="point">The point to clamp.</param>
        /// <param name="boxExtents">The half extents of the AABB. The AABB is centered at the origin.</param>
        /// <param name="clampedPoint">The clamped point will be stored in this output parameter.</param>
        /// <returns>
        ///     Returns <see langword="true" /> if the original point is inside the AABB, or <see langword="false" /> if it is
        ///     outside. The clamped point will always be inside the AABB.
        /// </returns>
        public static bool ClampPointToAABB(
            FPVector2 point,
            FPVector2 boxExtents,
            out FPVector2 clampedPoint)
        {
            bool aabb = true;
            if (point.X < -boxExtents.X)
            {
                aabb = false;
                point.X = -boxExtents.X;
            }
            else if (point.X > boxExtents.X)
            {
                aabb = false;
                point.X = boxExtents.X;
            }

            if (point.Y < -boxExtents.Y)
            {
                aabb = false;
                point.Y = -boxExtents.Y;
            }
            else if (point.Y > boxExtents.Y)
            {
                aabb = false;
                point.Y = boxExtents.Y;
            }

            clampedPoint = point;
            return aabb;
        }

        /// <summary>
        ///     Computes the closes point in segment A to a segment B.
        /// </summary>
        /// <param name="segment1Start">Start point of segment A.</param>
        /// <param name="segment1End">End point of segment A.</param>
        /// <param name="segment2Start">Start point of segment A.</param>
        /// <param name="segment2End">End point of segment B.</param>
        /// <returns></returns>
        public static FPVector2 ClosestPointBetweenSegments(
            FPVector2 segment1Start,
            FPVector2 segment1End,
            FPVector2 segment2Start,
            FPVector2 segment2End)
        {
            FPVector2 fpVector2_1 = segment1End - segment1Start;
            FPVector2 fpVector2_2 = segment2End - segment2Start;
            FPVector2 b1 = segment1Start - segment2Start;
            FP fp1 = FPVector2.Dot(fpVector2_1, fpVector2_1);
            FP fp2 = FPVector2.Dot(fpVector2_1, fpVector2_2);
            FP fp3 = FPVector2.Dot(fpVector2_2, fpVector2_2);
            FP fp4 = FPVector2.Dot(fpVector2_1, b1);
            FP fp5 = FPVector2.Dot(fpVector2_2, b1);
            FP fp6 = fp1 * fp3 - fp2 * fp2;
            FP fp7 = fp6;
            FP fp8 = fp6;
            FP fp9;
            FP fp10;
            if (fp6 < FP.Epsilon)
            {
                fp9 = FP._0;
                fp7 = FP._1;
                fp10 = fp5;
                fp8 = fp3;
            }
            else
            {
                fp9 = fp2 * fp5 - fp3 * fp4;
                fp10 = fp1 * fp5 - fp2 * fp4;
                if (fp9 < FP._0)
                {
                    fp9 = FP._0;
                    fp10 = fp5;
                    fp8 = fp3;
                }
                else if (fp9 > fp7)
                {
                    fp9 = fp7;
                    fp10 = fp5 + fp2;
                    fp8 = fp3;
                }
            }

            if (fp10 < FP._0)
            {
                fp10 = FP._0;
                if (-fp4 < FP._0)
                    fp9 = FP._0;
                else if (-fp4 > fp1)
                {
                    fp9 = fp7;
                }
                else
                {
                    fp9 = -fp4;
                    fp7 = fp1;
                }
            }
            else if (fp10 > fp8)
            {
                fp10 = fp8;
                if (-fp4 + fp2 < FP._0)
                    fp9 = FP._0;
                else if (-fp4 + fp2 > fp1)
                {
                    fp9 = fp7;
                }
                else
                {
                    fp9 = -fp4 + fp2;
                    fp7 = fp1;
                }
            }

            FP fp11 = FPMath.Abs(fp9) < FP.Epsilon ? FP._0 : fp9 / fp7;
            FP fp12 = FPMath.Abs(fp10) < FP.Epsilon ? FP._0 : fp10 / fp8;
            FPVector2 a = segment1Start + fp11 * fpVector2_1;
            FPVector2 b2 = segment2Start + fp12 * fpVector2_2;
            FP fp13 = FPVector2.Distance(a, b2);
            if (fp13 < FP.Epsilon)
                return a;
            FP fp14 = FPVector2.Distance(segment1Start, segment2Start);
            FP fp15 = FPVector2.Distance(segment1Start, segment2End);
            FP fp16 = FPVector2.Distance(segment1End, segment2Start);
            FP fp17 = FPVector2.Distance(segment1End, segment2End);
            FP fp18 = fp13;
            FPVector2 fpVector2_3 = a;
            if (fp14 < fp18)
            {
                fp18 = fp14;
                fpVector2_3 = segment1Start;
            }

            if (fp15 < fp18)
            {
                fp18 = fp15;
                fpVector2_3 = segment1Start;
            }

            if (fp16 < fp18)
            {
                fp18 = fp16;
                fpVector2_3 = segment1End;
            }

            if (fp17 < fp18)
                fpVector2_3 = segment1End;
            return fpVector2_3;
        }

        /// <summary>
        ///     Casts a point <paramref name="pt" /> on a triangle defined by three vertices.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static FPVector2 ClosestPointOnTriangle(
            FPVector2 pt,
            FPVector2 t0,
            FPVector2 t1,
            FPVector2 t2)
        {
            if (FPCollision.TriangleContainsPointInclusive(pt, t0, t1, t2))
                return pt;
            FPVector2 fpVector2_1 = FPCollision.ClosestPointOnSegment(pt, t0, t1);
            FPVector2 fpVector2_2 = FPCollision.ClosestPointOnSegment(pt, t1, t2);
            FPVector2 fpVector2_3 = FPCollision.ClosestPointOnSegment(pt, t2, t0);
            long num1 = fpVector2_1.X.RawValue - pt.X.RawValue;
            long num2 = fpVector2_1.Y.RawValue - pt.Y.RawValue;
            long num3 = fpVector2_2.X.RawValue - pt.X.RawValue;
            long num4 = fpVector2_2.Y.RawValue - pt.Y.RawValue;
            long num5 = fpVector2_3.X.RawValue - pt.X.RawValue;
            long num6 = fpVector2_3.Y.RawValue - pt.Y.RawValue;
            FP fp1;
            fp1.RawValue = (num1 * num1 + 32768L >> 16) + (num2 * num2 + 32768L >> 16);
            FP fp2;
            fp2.RawValue = (num3 * num3 + 32768L >> 16) + (num4 * num4 + 32768L >> 16);
            FP fp3;
            fp3.RawValue = (num5 * num5 + 32768L >> 16) + (num6 * num6 + 32768L >> 16);
            if (fp1 < fp2 && fp1 < fp3)
                return fpVector2_1;
            return fp2 < fp3 ? fpVector2_2 : fpVector2_3;
        }

        /// <summary>
        ///     Casts a point <paramref name="pt" /> on a circle.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static FPVector2 ClosestPointOnCicle(FPVector2 center, FP radius, FPVector2 pt) => FPVector2.Normalize(pt - center) * radius + center;

        /// <summary>
        ///     Checks if <paramref name="pt" /> is inside a triangle, excluding vertices and edges. Works for CW and CWW.
        /// </summary>
        /// <param name="pt">Point to check</param>
        /// <param name="v0">vertex position 0</param>
        /// <param name="v1">vertex position 1</param>
        /// <param name="v2">vertex position 2</param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="pt" /> is inside the triangle. <see langword="false" /> if point is
        ///     outside or if the point is located on an edge or vertex.
        /// </returns>
        public static bool TriangleContainsPointExclusive(
            FPVector2 pt,
            FPVector2 v0,
            FPVector2 v1,
            FPVector2 v2)
        {
            long num1 = ((pt.X.RawValue - v1.X.RawValue) * (v0.Y.RawValue - v1.Y.RawValue) + 32768L >> 16) - ((v0.X.RawValue - v1.X.RawValue) * (pt.Y.RawValue - v1.Y.RawValue) + 32768L >> 16);
            long num2 = ((pt.X.RawValue - v2.X.RawValue) * (v1.Y.RawValue - v2.Y.RawValue) + 32768L >> 16) - ((v1.X.RawValue - v2.X.RawValue) * (pt.Y.RawValue - v2.Y.RawValue) + 32768L >> 16);
            long num3 = ((pt.X.RawValue - v0.X.RawValue) * (v2.Y.RawValue - v0.Y.RawValue) + 32768L >> 16) - ((v2.X.RawValue - v0.X.RawValue) * (pt.Y.RawValue - v0.Y.RawValue) + 32768L >> 16);
            return (num1 < 0L && num2 < 0L && num3 < 0L) | (num1 > 0L && num2 > 0L && num3 > 0L);
        }

        /// <summary>
        ///     Checks if <paramref name="pt" /> is inside a triangle, excluding vertices and edges. This only checks the XZ
        ///     component like the triangle is in 2D! Works for CW and CWW.
        /// </summary>
        /// <param name="pt">Point to check</param>
        /// <param name="v0">vertex position 0</param>
        /// <param name="v1">vertex position 1</param>
        /// <param name="v2">vertex position 2</param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="pt" /> is inside the triangle. <see langword="false" /> if point is
        ///     outside or if the point is located on an edge or vertex.
        /// </returns>
        public static bool TriangleContainsPointExclusive(
            FPVector3 pt,
            FPVector3 v0,
            FPVector3 v1,
            FPVector3 v2)
        {
            long num1 = ((pt.X.RawValue - v1.X.RawValue) * (v0.Z.RawValue - v1.Z.RawValue) + 32768L >> 16) - ((v0.X.RawValue - v1.X.RawValue) * (pt.Z.RawValue - v1.Z.RawValue) + 32768L >> 16);
            long num2 = ((pt.X.RawValue - v2.X.RawValue) * (v1.Z.RawValue - v2.Z.RawValue) + 32768L >> 16) - ((v1.X.RawValue - v2.X.RawValue) * (pt.Z.RawValue - v2.Z.RawValue) + 32768L >> 16);
            long num3 = ((pt.X.RawValue - v0.X.RawValue) * (v2.Z.RawValue - v0.Z.RawValue) + 32768L >> 16) - ((v2.X.RawValue - v0.X.RawValue) * (pt.Z.RawValue - v0.Z.RawValue) + 32768L >> 16);
            return (num1 < 0L && num2 < 0L && num3 < 0L) | (num1 > 0L && num2 > 0L && num3 > 0L);
        }

        /// <summary>
        ///     Checks if <paramref name="pt" /> is inside a triangle, including edges and vertices. Works for CW and CWW.
        /// </summary>
        /// <param name="pt">Point to check</param>
        /// <param name="v0">vertex position 0</param>
        /// <param name="v1">vertex position 1</param>
        /// <param name="v2">vertex position 2</param>
        /// <returns><see langword="true" /> if <paramref name="pt" /> is inside the triangle or is located on an edge or vertex.</returns>
        public static bool TriangleContainsPointInclusive(
            FPVector2 pt,
            FPVector2 v0,
            FPVector2 v1,
            FPVector2 v2)
        {
            long num1 = ((pt.X.RawValue - v1.X.RawValue) * (v0.Y.RawValue - v1.Y.RawValue) + 32768L >> 16) - ((v0.X.RawValue - v1.X.RawValue) * (pt.Y.RawValue - v1.Y.RawValue) + 32768L >> 16);
            long num2 = ((pt.X.RawValue - v2.X.RawValue) * (v1.Y.RawValue - v2.Y.RawValue) + 32768L >> 16) - ((v1.X.RawValue - v2.X.RawValue) * (pt.Y.RawValue - v2.Y.RawValue) + 32768L >> 16);
            long num3 = ((pt.X.RawValue - v0.X.RawValue) * (v2.Y.RawValue - v0.Y.RawValue) + 32768L >> 16) - ((v2.X.RawValue - v0.X.RawValue) * (pt.Y.RawValue - v0.Y.RawValue) + 32768L >> 16);
            return !((num1 < 0L || num2 < 0L || num3 < 0L) & (num1 > 0L || num2 > 0L || num3 > 0L));
        }

        /// <summary>
        ///     Checks if <paramref name="pt" /> is inside a triangle, including edges and vertices.  This only checks the XZ
        ///     component like the triangle is in 2D! Works for CW and CWW.
        /// </summary>
        /// <param name="pt">Point to check</param>
        /// <param name="v0">vertex position 0</param>
        /// <param name="v1">vertex position 1</param>
        /// <param name="v2">vertex position 2</param>
        /// <returns><see langword="true" /> if <paramref name="pt" /> is inside the triangle or is located on an edge or vertex.</returns>
        public static bool TriangleContainsPointInclusive(
            FPVector3 pt,
            FPVector3 v0,
            FPVector3 v1,
            FPVector3 v2)
        {
            long num1 = ((pt.X.RawValue - v1.X.RawValue) * (v0.Z.RawValue - v1.Z.RawValue) + 32768L >> 16) - ((v0.X.RawValue - v1.X.RawValue) * (pt.Z.RawValue - v1.Z.RawValue) + 32768L >> 16);
            long num2 = ((pt.X.RawValue - v2.X.RawValue) * (v1.Z.RawValue - v2.Z.RawValue) + 32768L >> 16) - ((v1.X.RawValue - v2.X.RawValue) * (pt.Z.RawValue - v2.Z.RawValue) + 32768L >> 16);
            long num3 = ((pt.X.RawValue - v0.X.RawValue) * (v2.Z.RawValue - v0.Z.RawValue) + 32768L >> 16) - ((v2.X.RawValue - v0.X.RawValue) * (pt.Z.RawValue - v0.Z.RawValue) + 32768L >> 16);
            return !((num1 < 0L || num2 < 0L || num3 < 0L) & (num1 > 0L || num2 > 0L || num3 > 0L));
        }

        /// <summary>
        ///     Checks if <paramref name="point" /> is inside a circle, including its circumference. Works for CW and CWW.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="point"></param>
        /// <returns><see langword="true" /> in point <paramref name="point" /> is inside the circle.</returns>
        public static bool CircleContainsPoint(FPVector2 center, FP radius, FPVector2 point) => FPVector2.DistanceSquared(center, point).RawValue <= radius.RawValue * radius.RawValue + 32768L >> 16;

        /// <summary>Circle-circle intersection test.</summary>
        /// <param name="a_origin"></param>
        /// <param name="a_radius"></param>
        /// <param name="b_origin"></param>
        /// <param name="b_radius"></param>
        /// <returns></returns>
        public static bool CircleIntersectsCircle(
            FPVector2 a_origin,
            FP a_radius,
            FPVector2 b_origin,
            FP b_radius)
        {
            long num1 = a_origin.X.RawValue - b_origin.X.RawValue;
            long num2 = a_origin.Y.RawValue - b_origin.Y.RawValue;
            long num3 = a_radius.RawValue + b_radius.RawValue;
            return (num1 * num1 + 32768L >> 16) + (num2 * num2 + 32768L >> 16) <= num3 * num3 + 32768L >> 16;
        }

        /// <summary>Circle-AABB intersection test.</summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool CircleIntersectsAABB(
            FPVector2 center,
            FP radius,
            FPVector2 min,
            FPVector2 max)
        {
            long num1 = center.X.RawValue < min.X.RawValue ? min.X.RawValue : (center.X.RawValue > max.X.RawValue ? max.X.RawValue : center.X.RawValue);
            long num2 = center.Y.RawValue < min.Y.RawValue ? min.Y.RawValue : (center.Y.RawValue > max.Y.RawValue ? max.Y.RawValue : center.Y.RawValue);
            long num3 = num1 - center.X.RawValue;
            long num4 = num2 - center.Y.RawValue;
            return (num3 * num3 + 32768L >> 16) + (num4 * num4 + 32768L >> 16) <= radius.RawValue * radius.RawValue + 32768L >> 16;
        }

        /// <summary>Circle-triangle intersection test.</summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static bool CircleIntersectsTriangle(
            FPVector2 center,
            FP radius,
            FPVector2 v1,
            FPVector2 v2,
            FPVector2 v3)
        {
            long num1 = radius.RawValue * radius.RawValue + 32768L >> 16;
            long num2 = center.X.RawValue - v1.X.RawValue;
            long num3 = center.Y.RawValue - v1.Y.RawValue;
            long num4 = (num2 * num2 + 32768L >> 16) + (num3 * num3 + 32768L >> 16) - num1;
            if (num4 <= 0L)
                return true;
            long num5 = center.X.RawValue - v2.X.RawValue;
            long num6 = center.Y.RawValue - v2.Y.RawValue;
            long num7 = (num5 * num5 + 32768L >> 16) + (num6 * num6 + 32768L >> 16) - num1;
            if (num7 <= 0L)
                return true;
            long num8 = center.X.RawValue - v3.X.RawValue;
            long num9 = center.Y.RawValue - v3.Y.RawValue;
            long num10 = (num8 * num8 + 32768L >> 16) + (num9 * num9 + 32768L >> 16) - num1;
            if (num10 <= 0L)
                return true;
            long num11 = v2.X.RawValue - v1.X.RawValue;
            long num12 = v2.Y.RawValue - v1.Y.RawValue;
            long num13 = v3.X.RawValue - v2.X.RawValue;
            long num14 = v3.Y.RawValue - v2.Y.RawValue;
            long num15 = v1.X.RawValue - v3.X.RawValue;
            long num16 = v1.Y.RawValue - v3.Y.RawValue;
            if (num12 * num2 + 32768L >> 16 >= num11 * num3 + 32768L >> 16 && num14 * num5 + 32768L >> 16 >= num13 * num6 + 32768L >> 16 && num16 * num8 + 32768L >> 16 >= num15 * num9 + 32768L >> 16)
                return true;
            long num17 = (num2 * num11 + 32768L >> 16) + (num3 * num12 + 32768L >> 16);
            if (num17 > 0L)
            {
                long num18 = (num11 * num11 + 32768L >> 16) + (num12 * num12 + 32768L >> 16);
                if (num17 < num18 && num4 * num18 + 32768L >> 16 <= num17 * num17 + 32768L >> 16)
                    return true;
            }

            long num19 = (num5 * num13 + 32768L >> 16) + (num6 * num14 + 32768L >> 16);
            if (num19 > 0L)
            {
                long num20 = (num13 * num13 + 32768L >> 16) + (num14 * num14 + 32768L >> 16);
                if (num19 < num20 && num7 * num20 + 32768L >> 16 <= num19 * num19 + 32768L >> 16)
                    return true;
            }

            long num21 = (num8 * num15 + 32768L >> 16) + (num9 * num16 + 32768L >> 16);
            if (num21 > 0L)
            {
                long num22 = (num15 * num15 + 32768L >> 16) + (num16 * num16 + 32768L >> 16);
                if (num21 < num22 && num10 * num22 + 32768L >> 16 <= num21 * num21 + 32768L >> 16)
                    return true;
            }

            return false;
        }

        /// <summary>Line segment-AABB intersection test.</summary>
        /// <param name="p1">First point that defines the line segment in world space.</param>
        /// <param name="p2">Second point that defines the line segment in world space.</param>
        /// <param name="aabbCenter">The center of the AABB in world space.</param>
        /// <param name="aabbExtents">The distance between the AABB center and the max X and Y values in world space.</param>
        /// <returns><see langword="true" /> if the line segment intersects the AABB and <see langword="false" /> otherwise.</returns>
        public static bool LineIntersectsAABB_SAT(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 aabbCenter,
            FPVector2 aabbExtents)
        {
            p1.X.RawValue -= aabbCenter.X.RawValue;
            p1.Y.RawValue -= aabbCenter.Y.RawValue;
            p2.X.RawValue -= aabbCenter.X.RawValue;
            p2.Y.RawValue -= aabbCenter.Y.RawValue;
            return FPCollision.LineIntersectsAABB_SAT(p1, p2, aabbExtents);
        }

        /// <summary>
        ///     Line segment-AABB intersection test in the LOCAL space of the AABB.
        /// </summary>
        /// <param name="p1">First point that defines the line segment, relative to the AABB center.</param>
        /// <param name="p2">Second point that defines the line segment, relative to the AABB center.</param>
        /// <param name="aabbExtents">The distance between the AABB center and the max X and Y values.</param>
        /// <returns><see langword="true" /> if the line segment intersects the AABB and <see langword="false" /> otherwise.</returns>
        public static bool LineIntersectsAABB_SAT(FPVector2 p1, FPVector2 p2, FPVector2 aabbExtents)
        {
            FP x1 = aabbExtents.X;
            FP fp1;
            fp1.RawValue = -aabbExtents.X.RawValue;
            FP x2;
            FP x3;
            if (p1.X.RawValue > p2.X.RawValue)
            {
                x2 = p1.X;
                x3 = p2.X;
            }
            else
            {
                x3 = p1.X;
                x2 = p2.X;
            }

            if (x2.RawValue < fp1.RawValue || x3.RawValue > x1.RawValue)
                return false;
            FP y1 = aabbExtents.Y;
            FP fp2;
            fp2.RawValue = -aabbExtents.Y.RawValue;
            FP y2;
            FP y3;
            if (p1.X.RawValue > p2.X.RawValue)
            {
                y2 = p1.Y;
                y3 = p2.Y;
            }
            else
            {
                y3 = p1.Y;
                y2 = p2.Y;
            }

            return y2.RawValue >= fp2.RawValue && y3.RawValue <= y1.RawValue;
        }

        /// <summary>
        ///     Line segment-AABB intersection test in world space with computation of intersection points, normal and penetration.
        ///     If an intersection is detected, the test always returns two intersection points, which can be either intersections
        ///     between the line
        ///     segment and an edge of the AABB or a segment point itself, if inside the AABB.
        /// </summary>
        /// <param name="p1">First point that defines the line segment in world space.</param>
        /// <param name="p2">Second point that defines the line segment in world space.</param>
        /// <param name="normal">Normal along which the line segment <paramref name="penetration" /> will be computed.</param>
        /// <param name="aabbCenter">The center of the AABB in world space.</param>
        /// <param name="aabbExtents">The distance between the AABB center and the max X and Y values in world space.</param>
        /// <param name="i1">First intersection point.</param>
        /// <param name="i2">Second intersection point.</param>
        /// <param name="penetration">The penetration of the line segment along the <paramref name="normal" />.</param>
        /// <returns><see langword="true" /> if the line segment intersects the AABB and <see langword="false" /> otherwise.</returns>
        public static bool LineIntersectsAABB2(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 normal,
            FPVector2 aabbCenter,
            FPVector2 aabbExtents,
            out FPVector2 i1,
            out FPVector2 i2,
            out FP penetration)
        {
            p1.X.RawValue -= aabbCenter.X.RawValue;
            p1.Y.RawValue -= aabbCenter.Y.RawValue;
            p2.X.RawValue -= aabbCenter.X.RawValue;
            p2.Y.RawValue -= aabbCenter.Y.RawValue;
            if (!FPCollision.LineIntersectsAABB2(p1, p2, normal, aabbExtents, out i1, out i2, out penetration))
                return false;
            i1.X.RawValue += aabbCenter.X.RawValue;
            i1.Y.RawValue += aabbCenter.Y.RawValue;
            i2.X.RawValue += aabbCenter.X.RawValue;
            i2.Y.RawValue += aabbCenter.Y.RawValue;
            return true;
        }

        /// <summary>
        ///     Line segment-AABB intersection test in the LOCAL space of the AABB with computation of intersection points, normal
        ///     and penetration.
        ///     If an intersection is detected, the test always returns two intersection points, which can be either intersections
        ///     between the line
        ///     segment and an edge of the AABB or a segment point itself, if inside the AABB.
        /// </summary>
        /// <param name="p1">First point that defines the line segment, relative to the AABB center.</param>
        /// <param name="p2">Second point that defines the line segment, relative to the AABB center.</param>
        /// <param name="normal">Normal along which the line segment <paramref name="penetration" /> will be computed.</param>
        /// <param name="aabbExtents">The distance between the AABB center and the max X and Y values in world space.</param>
        /// <param name="i1">First intersection point, relative to the AABB center.</param>
        /// <param name="i2">Second intersection point, relative to the AABB center.</param>
        /// <param name="penetration">The penetration of the line segment along the <paramref name="normal" />.</param>
        /// <returns><see langword="true" /> if the line segment intersects the AABB and <see langword="false" /> otherwise.</returns>
        public static bool LineIntersectsAABB2(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 normal,
            FPVector2 aabbExtents,
            out FPVector2 i1,
            out FPVector2 i2,
            out FP penetration)
        {
            FPVector2 fpVector2_1;
            fpVector2_1.X.RawValue = p2.X.RawValue - p1.X.RawValue;
            fpVector2_1.Y.RawValue = p2.Y.RawValue - p1.Y.RawValue;
            FPVector2 fpVector2_2;
            fpVector2_2.X.RawValue = -aabbExtents.X.RawValue - p1.X.RawValue;
            fpVector2_2.Y.RawValue = -aabbExtents.Y.RawValue - p1.Y.RawValue;
            FPVector2 fpVector2_3;
            fpVector2_3.X.RawValue = aabbExtents.X.RawValue - p1.X.RawValue;
            fpVector2_3.Y.RawValue = aabbExtents.Y.RawValue - p1.Y.RawValue;
            long num1 = (long)int.MinValue;
            long num2 = (long)int.MaxValue;
            if (fpVector2_1.X.RawValue == 0L && (fpVector2_2.X.RawValue > 0L || fpVector2_3.X.RawValue < 0L))
            {
                i1 = new FPVector2();
                i2 = new FPVector2();
                penetration.RawValue = (long)int.MaxValue;
                return false;
            }

            if (fpVector2_1.Y.RawValue == 0L && (fpVector2_2.Y.RawValue > 0L || fpVector2_3.Y.RawValue < 0L))
            {
                i1 = new FPVector2();
                i2 = new FPVector2();
                penetration.RawValue = (long)int.MaxValue;
                return false;
            }

            if (fpVector2_1.X.RawValue != 0L)
            {
                FP fp1;
                fp1.RawValue = (fpVector2_2.X.RawValue << 16) / fpVector2_1.X.RawValue;
                FP fp2;
                fp2.RawValue = (fpVector2_3.X.RawValue << 16) / fpVector2_1.X.RawValue;
                if (fp1.RawValue > fp2.RawValue)
                {
                    num1 = fp2.RawValue;
                    num2 = fp1.RawValue;
                }
                else
                {
                    num1 = fp1.RawValue;
                    num2 = fp2.RawValue;
                }

                if (num1 > num2 || num2 < 0L)
                {
                    i1 = new FPVector2();
                    i2 = new FPVector2();
                    penetration.RawValue = (long)int.MaxValue;
                    return false;
                }
            }

            if (fpVector2_1.Y.RawValue != 0L)
            {
                FP fp3;
                fp3.RawValue = (fpVector2_2.Y.RawValue << 16) / fpVector2_1.Y.RawValue;
                FP fp4;
                fp4.RawValue = (fpVector2_3.Y.RawValue << 16) / fpVector2_1.Y.RawValue;
                long rawValue1;
                long rawValue2;
                if (fp3.RawValue > fp4.RawValue)
                {
                    rawValue1 = fp4.RawValue;
                    rawValue2 = fp3.RawValue;
                }
                else
                {
                    rawValue1 = fp3.RawValue;
                    rawValue2 = fp4.RawValue;
                }

                if (rawValue1 > num1)
                    num1 = rawValue1;
                if (rawValue2 < num2)
                    num2 = rawValue2;
                if (num1 > num2 || num2 < 0L)
                {
                    i1 = new FPVector2();
                    i2 = new FPVector2();
                    penetration.RawValue = (long)int.MaxValue;
                    return false;
                }
            }

            if (num2 > 65536L && num1 > 65536L)
            {
                i1 = new FPVector2();
                i2 = new FPVector2();
                penetration.RawValue = (long)int.MaxValue;
                return false;
            }

            if (num1 >= 0L && num1 <= 65536L)
            {
                i1.X.RawValue = p1.X.RawValue + (fpVector2_1.X.RawValue * num1 + 32768L >> 16);
                i1.Y.RawValue = p1.Y.RawValue + (fpVector2_1.Y.RawValue * num1 + 32768L >> 16);
            }
            else
                i1 = p1;

            if (num2 >= 0L && num2 <= 65536L)
            {
                i2.X.RawValue = p1.X.RawValue + (fpVector2_1.X.RawValue * num2 + 32768L >> 16);
                i2.Y.RawValue = p1.Y.RawValue + (fpVector2_1.Y.RawValue * num2 + 32768L >> 16);
            }
            else
                i2 = p2;

            FP fp5;
            fp5.RawValue = normal.X.RawValue >= 0L ? normal.X.RawValue : -normal.X.RawValue;
            FP fp6;
            fp6.RawValue = normal.Y.RawValue >= 0L ? normal.Y.RawValue : -normal.Y.RawValue;
            FP fp7;
            FP fp8;
            if (fp5.RawValue > fp6.RawValue)
            {
                fp7.RawValue = i1.X.RawValue >= 0L ? aabbExtents.X.RawValue - i1.X.RawValue : aabbExtents.X.RawValue + i1.X.RawValue;
                fp8.RawValue = i2.X.RawValue >= 0L ? aabbExtents.X.RawValue - i2.X.RawValue : aabbExtents.X.RawValue + i2.X.RawValue;
            }
            else
            {
                fp7.RawValue = i1.Y.RawValue >= 0L ? aabbExtents.Y.RawValue - i1.Y.RawValue : aabbExtents.Y.RawValue + i1.Y.RawValue;
                fp8.RawValue = i2.Y.RawValue >= 0L ? aabbExtents.Y.RawValue - i2.Y.RawValue : aabbExtents.Y.RawValue + i2.Y.RawValue;
            }

            penetration = fp7.RawValue > fp8.RawValue ? fp7 : fp8;
            return true;
        }

        /// <summary>
        ///     Line segment-AABB intersection test in world space with computation of intersection points and penetration.
        /// </summary>
        /// <param name="p1">First point that defines the line segment in world space.</param>
        /// <param name="p2">Second point that defines the line segment in world space.</param>
        /// <param name="aabbCenter">The center of the AABB in world space.</param>
        /// <param name="aabbExtents">The distance between the AABB center and the max X and Y values in world space.</param>
        /// <param name="i1">First intersection point in world space.</param>
        /// <param name="i2">Second intersection point in world space.</param>
        /// <param name="penetration">The penetration of the line segment along the closest AABB normal.</param>
        /// <returns>
        ///     The number of intersections found between the line segment and the AABB edges.
        ///     If less than 2, the respective intersection point will be default.
        /// </returns>
        public static int LineIntersectsAABB(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 aabbCenter,
            FPVector2 aabbExtents,
            out FPVector2 i1,
            out FPVector2 i2,
            out FP penetration)
        {
            p1.X.RawValue -= aabbCenter.X.RawValue;
            p1.Y.RawValue -= aabbCenter.Y.RawValue;
            p2.X.RawValue -= aabbCenter.X.RawValue;
            p2.Y.RawValue -= aabbCenter.Y.RawValue;
            int num = FPCollision.LineIntersectsAABB(p1, p2, aabbExtents, out i1, out i2, out penetration);
            i1.X.RawValue += aabbCenter.X.RawValue;
            i1.Y.RawValue += aabbCenter.Y.RawValue;
            i2.X.RawValue += aabbCenter.X.RawValue;
            i2.Y.RawValue += aabbCenter.Y.RawValue;
            return num;
        }

        /// <summary>
        ///     Line segment-AABB intersection test in the LOCAL space of the AABB with computation of intersection points and
        ///     penetration.
        /// </summary>
        /// <param name="p1">First point that defines the line segment, relative to the AABB center.</param>
        /// <param name="p2">Second point that defines the line segment, relative to the AABB center.</param>
        /// <param name="aabbExtents">The distance between the AABB center and the max X and Y values in world space.</param>
        /// <param name="i1">First intersection point, relative to the AABB center.</param>
        /// <param name="i2">Second intersection point, relative to the AABB center.</param>
        /// <param name="penetration">The penetration of the line segment along the closest AABB normal.</param>
        /// <returns>
        ///     The number of intersections found between the line segment and the AABB edges.
        ///     If less than 2, the respective intersection point will be default.
        /// </returns>
        public static int LineIntersectsAABB(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 aabbExtents,
            out FPVector2 i1,
            out FPVector2 i2,
            out FP penetration)
        {
            i1 = new FPVector2();
            i2 = new FPVector2();
            penetration = new FP();
            int num = 0;
            FPVector2 fpVector2_1;
            fpVector2_1.X = p1.X.RawValue < p2.X.RawValue ? p1.X : p2.X;
            fpVector2_1.Y = p1.Y.RawValue < p2.Y.RawValue ? p1.Y : p2.Y;
            FPVector2 fpVector2_2;
            fpVector2_2.X = p1.X.RawValue > p2.X.RawValue ? p1.X : p2.X;
            fpVector2_2.Y = p1.Y.RawValue > p2.Y.RawValue ? p1.Y : p2.Y;
            if (fpVector2_1.X.RawValue > aabbExtents.X.RawValue || fpVector2_2.X.RawValue < -aabbExtents.X.RawValue || fpVector2_1.Y.RawValue > aabbExtents.Y.RawValue || fpVector2_2.Y.RawValue < -aabbExtents.Y.RawValue)
                return 0;
            FPVector2 p1_1;
            p1_1.X.RawValue = -aabbExtents.X.RawValue;
            p1_1.Y.RawValue = -aabbExtents.Y.RawValue;
            FPVector2 fpVector2_3;
            fpVector2_3.X.RawValue = aabbExtents.X.RawValue;
            fpVector2_3.Y.RawValue = -aabbExtents.Y.RawValue;
            FPVector2 fpVector2_4;
            fpVector2_4.X.RawValue = -aabbExtents.X.RawValue;
            fpVector2_4.Y.RawValue = aabbExtents.Y.RawValue;
            FPVector2 p2_1;
            p2_1.X.RawValue = aabbExtents.X.RawValue;
            p2_1.Y.RawValue = aabbExtents.Y.RawValue;
            penetration.RawValue = (long)int.MaxValue;
            FPVector2 point;
            FP distance;
            if (FPCollision.LineIntersectsLine(p1_1, fpVector2_3, p1, p2, out point, out distance))
            {
                i1 = point;
                if (distance.RawValue < penetration.RawValue)
                    penetration = distance;
                ++num;
            }

            if (FPCollision.LineIntersectsLine(fpVector2_4, p2_1, p1, p2, out point, out distance))
            {
                ++num;
                if (distance.RawValue < penetration.RawValue)
                    penetration = distance;
                if (num > 1)
                {
                    i2 = point;
                    return num;
                }

                i1 = point;
            }

            if (FPCollision.LineIntersectsLine(p1_1, fpVector2_4, p1, p2, out point, out distance))
            {
                ++num;
                if (distance.RawValue < penetration.RawValue)
                    penetration = distance;
                if (num > 1)
                {
                    i2 = point;
                    return num;
                }

                i1 = point;
            }

            if (FPCollision.LineIntersectsLine(fpVector2_3, p2_1, p1, p2, out point, out distance))
            {
                ++num;
                if (distance.RawValue < penetration.RawValue)
                    penetration = distance;
                if (num > 1)
                {
                    i2 = point;
                    return num;
                }

                i1 = point;
            }

            if (FPCollision.InsideAABB(p1, aabbExtents, out penetration))
            {
                FPVector2 fpVector2_5 = p1;
                ++num;
                if (distance.RawValue < penetration.RawValue)
                    penetration = distance;
                if (num > 1)
                {
                    i2 = fpVector2_5;
                    return num;
                }

                i1 = fpVector2_5;
            }

            if (FPCollision.InsideAABB(p2, aabbExtents, out penetration))
            {
                FPVector2 fpVector2_6 = p2;
                ++num;
                if (distance.RawValue < penetration.RawValue)
                    penetration = distance;
                if (num > 1)
                {
                    i2 = fpVector2_6;
                    return num;
                }

                i1 = fpVector2_6;
            }

            return num;
        }

        /// <summary>Line segment-line segment intersection test.</summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static bool LineIntersectsLine(FPVector2 p1, FPVector2 p2, FPVector2 q1, FPVector2 q2)
        {
            FPVector2 fpVector2 = new FPVector2();
            fpVector2.X.RawValue = p2.X.RawValue - p1.X.RawValue;
            fpVector2.Y.RawValue = p2.Y.RawValue - p1.Y.RawValue;
            FPVector2 b = new FPVector2();
            b.X.RawValue = q2.X.RawValue - q1.X.RawValue;
            b.Y.RawValue = q2.Y.RawValue - q1.Y.RawValue;
            FPVector2 a = new FPVector2();
            a.X.RawValue = q1.X.RawValue - p1.X.RawValue;
            a.Y.RawValue = q1.Y.RawValue - p1.Y.RawValue;
            long num1 = FPVector2.CrossRaw(fpVector2, b);
            long num2 = FPVector2.CrossRaw(a, fpVector2);
            if (num1 == 0L && num2 == 0L || num1 == 0L && num2 != 0L)
                return false;
            long num3 = (FPVector2.CrossRaw(a, b) << 16) / num1;
            long num4 = (num2 << 16) / num1;
            return num1 != 0L && 0L <= num3 && num3 <= 65536L && 0L <= num4 && num4 <= 65536L;
        }

        /// <summary>Line segment-line segment intersection test.</summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <param name="point">Point of collision</param>
        /// <param name="distance">Distance along p segment where the collision happens</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LineIntersectsLine(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 q1,
            FPVector2 q2,
            out FPVector2 point,
            out FP distance)
        {
            return FPCollision.LineIntersectsLine(p1, p2, q1, q2, out point, out distance, out FP _);
        }

        /// <summary>Determines if two lines intersect.</summary>
        /// <param name="p1">The starting point of the first line.</param>
        /// <param name="p2">The ending point of the first line.</param>
        /// <param name="q1">The starting point of the second line.</param>
        /// <param name="q2">The ending point of the second line.</param>
        /// <param name="point">The intersection point, if the lines intersect.</param>
        /// <param name="distance">The distance between the intersection point and point p1, if the lines intersect.</param>
        /// <param name="normalizedDist">
        ///     The normalized distance between the intersection point and point p1, if the lines
        ///     intersect.
        /// </param>
        /// <returns><see langword="true" /> if the lines intersect, <see langword="false" /> otherwise.</returns>
        public static bool LineIntersectsLine(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 q1,
            FPVector2 q2,
            out FPVector2 point,
            out FP distance,
            out FP normalizedDist)
        {
            FPVector2 fpVector2 = new FPVector2();
            fpVector2.X.RawValue = p2.X.RawValue - p1.X.RawValue;
            fpVector2.Y.RawValue = p2.Y.RawValue - p1.Y.RawValue;
            FPVector2 b = new FPVector2();
            b.X.RawValue = q2.X.RawValue - q1.X.RawValue;
            b.Y.RawValue = q2.Y.RawValue - q1.Y.RawValue;
            FPVector2 a = new FPVector2();
            a.X.RawValue = q1.X.RawValue - p1.X.RawValue;
            a.Y.RawValue = q1.Y.RawValue - p1.Y.RawValue;
            long num1 = FPVector2.CrossRaw(fpVector2, b);
            long num2 = FPVector2.CrossRaw(a, fpVector2);
            if (num1 == 0L && num2 == 0L)
            {
                point = new FPVector2();
                distance = new FP();
                normalizedDist = new FP();
                return false;
            }

            if (num1 == 0L && num2 != 0L)
            {
                point = new FPVector2();
                distance = new FP();
                normalizedDist = new FP();
                return false;
            }

            normalizedDist.RawValue = (FPVector2.CrossRaw(a, b) << 16) / num1;
            long num3 = (num2 << 16) / num1;
            if (num1 != 0L && 0L <= normalizedDist.RawValue && normalizedDist.RawValue <= 65536L && 0L <= num3 && num3 <= 65536L)
            {
                fpVector2.X.RawValue = fpVector2.X.RawValue * normalizedDist.RawValue + 32768L >> 16;
                fpVector2.Y.RawValue = fpVector2.Y.RawValue * normalizedDist.RawValue + 32768L >> 16;
                distance = fpVector2.Magnitude;
                fpVector2.X.RawValue += p1.X.RawValue;
                fpVector2.Y.RawValue += p1.Y.RawValue;
                point = fpVector2;
                return true;
            }

            point = new FPVector2();
            distance = new FP();
            normalizedDist = new FP();
            return false;
        }

        /// <summary>Line segment-line segment intersection test.</summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <param name="point">Point of collision</param>
        /// <returns></returns>
        public static bool LineIntersectsLine(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 q1,
            FPVector2 q2,
            out FPVector2 point)
        {
            FPVector2 fpVector2 = new FPVector2();
            fpVector2.X.RawValue = p2.X.RawValue - p1.X.RawValue;
            fpVector2.Y.RawValue = p2.Y.RawValue - p1.Y.RawValue;
            FPVector2 b = new FPVector2();
            b.X.RawValue = q2.X.RawValue - q1.X.RawValue;
            b.Y.RawValue = q2.Y.RawValue - q1.Y.RawValue;
            FPVector2 a = new FPVector2();
            a.X.RawValue = q1.X.RawValue - p1.X.RawValue;
            a.Y.RawValue = q1.Y.RawValue - p1.Y.RawValue;
            long num1 = FPVector2.CrossRaw(fpVector2, b);
            long num2 = FPVector2.CrossRaw(a, fpVector2);
            if (num1 == 0L && num2 == 0L)
            {
                point = new FPVector2();
                return false;
            }

            if (num1 == 0L && num2 != 0L)
            {
                point = new FPVector2();
                return false;
            }

            long num3 = (FPVector2.CrossRaw(a, b) << 16) / num1;
            long num4 = (num2 << 16) / num1;
            if (num1 != 0L && 0L <= num3 && num3 <= 65536L && 0L <= num4 && num4 <= 65536L)
            {
                fpVector2.X.RawValue = fpVector2.X.RawValue * num3 + 32768L >> 16;
                fpVector2.Y.RawValue = fpVector2.Y.RawValue * num3 + 32768L >> 16;
                fpVector2.X.RawValue += p1.X.RawValue;
                fpVector2.Y.RawValue += p1.Y.RawValue;
                point = fpVector2;
                return true;
            }

            point = new FPVector2();
            return false;
        }

        /// <summary>
        ///     Line segment-line segment intersection test. Assumes lines are not colinear nor parallel.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <param name="point"></param>
        public static void LineIntersectsLineAlwaysHit(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 q1,
            FPVector2 q2,
            out FPVector2 point)
        {
            FPVector2 a1 = new FPVector2();
            a1.X.RawValue = p2.X.RawValue - p1.X.RawValue;
            a1.Y.RawValue = p2.Y.RawValue - p1.Y.RawValue;
            FPVector2 b = new FPVector2();
            b.X.RawValue = q2.X.RawValue - q1.X.RawValue;
            b.Y.RawValue = q2.Y.RawValue - q1.Y.RawValue;
            FPVector2 a2 = new FPVector2();
            a2.X.RawValue = q1.X.RawValue - p1.X.RawValue;
            a2.Y.RawValue = q1.Y.RawValue - p1.Y.RawValue;
            long num1 = FPVector2.CrossRaw(a1, b);
            long num2 = (FPVector2.CrossRaw(a2, b) << 16) / num1;
            a1.X.RawValue = a1.X.RawValue * num2 + 32768L >> 16;
            a1.Y.RawValue = a1.Y.RawValue * num2 + 32768L >> 16;
            a1.X.RawValue += p1.X.RawValue;
            a1.Y.RawValue += p1.Y.RawValue;
            point = a1;
        }

        /// <summary>
        ///     Returns <see langword="true" /> if <paramref name="point" /> is inside centered AABB.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="extents"></param>
        /// <param name="penetration"></param>
        /// <returns></returns>
        public static bool InsideAABB(FPVector2 point, FPVector2 extents, out FP penetration)
        {
            FP val1_1 = extents.X - point.X;
            FP val2_1 = extents.X + point.X;
            FP val1_2 = extents.Y - point.Y;
            FP val2_2 = extents.Y + point.Y;
            if (val1_1 >= 0 && val2_1 >= 0 && val1_2 >= 0 && val2_2 >= 0)
            {
                penetration = FPMath.Min(FPMath.Min(val1_1, val2_1), FPMath.Min(val1_2, val2_2));
                return true;
            }

            penetration = FP.UseableMax;
            return false;
        }

        /// <summary>Line segment-circle intersection test.</summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool LineIntersectsCircleManifold(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 position,
            FP radius,
            out FPVector2 point)
        {
            point = new FPVector2();
            long num1 = position.X.RawValue - p1.X.RawValue;
            long num2 = position.Y.RawValue - p1.Y.RawValue;
            long num3 = p2.X.RawValue - p1.X.RawValue;
            long num4 = p2.Y.RawValue - p1.Y.RawValue;
            long num5 = FPMath.SqrtRaw((num3 * num3 + 32768L >> 16) + (num4 * num4 + 32768L >> 16));
            long num6 = (num3 << 16) / num5;
            long num7 = (num4 << 16) / num5;
            long num8 = (num6 * num1 + 32768L >> 16) + (num7 * num2 + 32768L >> 16);
            if (num8 < 0L)
            {
                if (FPMath.SqrtRaw(((p1.X.RawValue - position.X.RawValue) * (p1.X.RawValue - position.X.RawValue) + 32768L >> 16) + ((p1.Y.RawValue - position.Y.RawValue) * (p1.Y.RawValue - position.Y.RawValue) + 32768L >> 16)) > radius.RawValue)
                    return false;
                point.X.RawValue = p1.X.RawValue;
                point.Y.RawValue = p1.Y.RawValue;
                return true;
            }

            if (num8 > num5)
            {
                if (FPMath.SqrtRaw(((p2.X.RawValue - position.X.RawValue) * (p2.X.RawValue - position.X.RawValue) + 32768L >> 16) + ((p2.Y.RawValue - position.Y.RawValue) * (p2.Y.RawValue - position.Y.RawValue) + 32768L >> 16)) > radius.RawValue)
                    return false;
                point.X.RawValue = p2.X.RawValue;
                point.Y.RawValue = p2.Y.RawValue;
                return true;
            }

            long num9 = p1.X.RawValue + (num6 * num8 + 32768L >> 16);
            long num10 = p1.Y.RawValue + (num7 * num8 + 32768L >> 16);
            if (FPMath.SqrtRaw(((num9 - position.X.RawValue) * (num9 - position.X.RawValue) + 32768L >> 16) + ((num10 - position.Y.RawValue) * (num10 - position.Y.RawValue) + 32768L >> 16)) > radius.RawValue)
                return false;
            point.X.RawValue = num9;
            point.Y.RawValue = num10;
            return true;
        }

        /// <summary>Line segment-circle intersection test.</summary>
        /// <param name="p1">Start point of the line segment.</param>
        /// <param name="p2">End point of the line segment.</param>
        /// <param name="position">Position of the center of the circle in world space.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="ignoreIfStartPointInside">
        ///     If the intersection should be ignored if the start point of the line segment (
        ///     <paramref name="p1" />) is inside the circle.
        /// </param>
        /// <returns><see langword="true" /> if an intersection is detected. <see langword="false" /> otherwise.</returns>
        public static bool LineIntersectsCircle(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 position,
            FP radius,
            bool ignoreIfStartPointInside = false)
        {
            return FPCollision.LineIntersectsCircle(p1, p2, position, radius, out FPVector2 _, out FP _, ignoreIfStartPointInside);
        }

        /// <summary>Line segment-circle intersection test.</summary>
        /// <param name="p1">Start point of the line segment.</param>
        /// <param name="p2">End point of the line segment.</param>
        /// <param name="position">Position of the center of the circle in world space.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="point">Intersection point, if intersecting. Default otherwise.</param>
        /// <param name="ignoreIfStartPointInside">
        ///     If the intersection should be ignored if the start point of the line segment (
        ///     <paramref name="p1" />) is inside the circle.
        /// </param>
        /// <returns><see langword="true" /> if an intersection is detected. <see langword="false" /> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LineIntersectsCircle(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 position,
            FP radius,
            out FPVector2 point,
            bool ignoreIfStartPointInside = false)
        {
            return FPCollision.LineIntersectsCircle(p1, p2, position, radius, out point, out FP _, ignoreIfStartPointInside);
        }

        /// <summary>Line segment-circle intersection test.</summary>
        /// <param name="p1">Start point of the line segment.</param>
        /// <param name="p2">End point of the line segment.</param>
        /// <param name="position">Position of the center of the circle in world space.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="point">Intersection point, if intersecting. Default otherwise.</param>
        /// <param name="normalizedDist">
        ///     Normalize distance from <paramref name="p1" /> to <paramref name="p2" /> of the
        ///     intersection point, if intersecting. Default otherwise.
        /// </param>
        /// <param name="ignoreIfStartPointInside">
        ///     If the intersection should be ignored if the start point of the line segment (
        ///     <paramref name="p1" />) is inside the circle.
        /// </param>
        /// <returns><see langword="true" /> if an intersection is detected. <see langword="false" /> otherwise.</returns>
        public static bool LineIntersectsCircle(
            FPVector2 p1,
            FPVector2 p2,
            FPVector2 position,
            FP radius,
            out FPVector2 point,
            out FP normalizedDist,
            bool ignoreIfStartPointInside = false)
        {
            FPVector2 fpVector2_1;
            fpVector2_1.X.RawValue = p2.X.RawValue - p1.X.RawValue;
            fpVector2_1.Y.RawValue = p2.Y.RawValue - p1.Y.RawValue;
            FP magnitude1;
            FPVector2 fpVector2_2 = FPVector2.Normalize(fpVector2_1, out magnitude1);
            FP fp1;
            fp1.RawValue = radius.RawValue * radius.RawValue + 32768L >> 16;
            if (magnitude1.RawValue == 0L || fp1.RawValue == 0L)
            {
                point = new FPVector2();
                normalizedDist = new FP();
                return false;
            }

            FPVector2 fpVector2_3;
            fpVector2_3.X.RawValue = p1.X.RawValue - position.X.RawValue;
            fpVector2_3.Y.RawValue = p1.Y.RawValue - position.Y.RawValue;
            FP magnitude2;
            FPVector2 fpVector2_4 = FPVector2.Normalize(fpVector2_3, out magnitude2);
            FP fp2;
            fp2.RawValue = (fpVector2_4.X.RawValue * fpVector2_2.X.RawValue + 32768L >> 16) + (fpVector2_4.Y.RawValue * fpVector2_2.Y.RawValue + 32768L >> 16);
            FP fp3;
            fp3.RawValue = 65536L - (fp2.RawValue * fp2.RawValue + 32768L >> 16);
            FP fp4;
            if (fp3.RawValue > 0L)
                fp4.RawValue = magnitude2.RawValue * FPMath.SqrtRaw(fp3.RawValue) + 32768L >> 16;
            else
                fp4 = new FP();
            if (fp4.RawValue > radius.RawValue)
            {
                point = new FPVector2();
                normalizedDist = new FP();
                return false;
            }

            FP fp5;
            fp5.RawValue = FPMath.SqrtRaw(fp1.RawValue - (fp4.RawValue * fp4.RawValue + 32768L >> 16));
            FP fp6;
            fp6.RawValue = -fp2.RawValue * magnitude2.RawValue + 32768L >> 16;
            FP fp7;
            fp7.RawValue = fp6.RawValue - fp5.RawValue;
            if (0L <= fp7.RawValue && fp7.RawValue <= magnitude1.RawValue)
            {
                normalizedDist.RawValue = (fp7.RawValue << 16) / magnitude1.RawValue;
                point.X.RawValue = p1.X.RawValue + (fpVector2_2.X.RawValue * fp7.RawValue + 32768L >> 16);
                point.Y.RawValue = p1.Y.RawValue + (fpVector2_2.Y.RawValue * fp7.RawValue + 32768L >> 16);
                return true;
            }

            if (!ignoreIfStartPointInside)
            {
                fp7.RawValue = fp6.RawValue + fp5.RawValue;
                if (0L <= fp7.RawValue && fp7.RawValue <= magnitude1.RawValue)
                {
                    normalizedDist.RawValue = (fp7.RawValue << 16) / magnitude1.RawValue;
                    point.X.RawValue = p1.X.RawValue + (fpVector2_2.X.RawValue * fp7.RawValue + 32768L >> 16);
                    point.Y.RawValue = p1.Y.RawValue + (fpVector2_2.Y.RawValue * fp7.RawValue + 32768L >> 16);
                    return true;
                }
            }

            point = new FPVector2();
            normalizedDist = new FP();
            return false;
        }

        /// <summary>Circle-polygon intersection test.</summary>
        /// <param name="circleCenter"></param>
        /// <param name="circleRadius"></param>
        /// <param name="polygonPosition"></param>
        /// <param name="polygonRotationSinInverse"></param>
        /// <param name="polygonRotationCosInverse"></param>
        /// <param name="polygonVertices"></param>
        /// <param name="polygonNormals"></param>
        /// <returns></returns>
        public static bool CircleIntersectsPolygon(
            FPVector2 circleCenter,
            FP circleRadius,
            FPVector2 polygonPosition,
            FP polygonRotationSinInverse,
            FP polygonRotationCosInverse,
            ReadOnlySpan<FPVector2> polygonVertices,
            ReadOnlySpan<FPVector2> polygonNormals)
        {
            FPVector2 a = FPVector2.Rotate(circleCenter - polygonPosition, polygonRotationSinInverse, polygonRotationCosInverse);
            FP fp1 = -FP.MaxValue;
            int index1 = 0;
            for (int index2 = 0; index2 < polygonVertices.Length; ++index2)
            {
                FP fp2 = FPVector2.Dot(polygonNormals[index2], a - polygonVertices[index2]);
                if (fp2 > circleRadius)
                    return false;
                if (fp2 > fp1)
                {
                    fp1 = fp2;
                    index1 = index2;
                }
            }

            if (fp1 < FP.Epsilon)
                return true;
            FPVector2 polygonVertex1 = polygonVertices[index1];
            FPVector2 polygonVertex2 = polygonVertices[(index1 + 1) % polygonVertices.Length];
            FP fp3 = FPVector2.Dot(a - polygonVertex1, polygonVertex2 - polygonVertex1);
            FP fp4 = FPVector2.Dot(a - polygonVertex2, polygonVertex1 - polygonVertex2);
            FP fp5 = circleRadius * circleRadius;
            if (fp3 <= FP._0)
                return !(FPVector2.DistanceSquared(a, polygonVertex1) > fp5);
            return fp4 <= FP._0 ? !(FPVector2.DistanceSquared(a, polygonVertex2) > fp5) : !(FPVector2.Dot(a - polygonVertex1, polygonNormals[index1]) > circleRadius);
        }

        /// <summary>Circle-polygon intersection test.</summary>
        /// <param name="circleCenter"></param>
        /// <param name="circleRadius"></param>
        /// <param name="polygonPosition"></param>
        /// <param name="polygonRotation"></param>
        /// <param name="polygonVertices"></param>
        /// <param name="polygonNormals"></param>
        /// <returns></returns>
        public static bool CircleIntersectsPolygon(
            FPVector2 circleCenter,
            FP circleRadius,
            FPVector2 polygonPosition,
            FP polygonRotation,
            ReadOnlySpan<FPVector2> polygonVertices,
            ReadOnlySpan<FPVector2> polygonNormals)
        {
            FP sin;
            FP cos;
            FPMath.SinCos(-polygonRotation, out sin, out cos);
            return FPCollision.CircleIntersectsPolygon(circleCenter, circleRadius, polygonPosition, sin, cos, polygonVertices, polygonNormals);
        }

        /// <summary>Box-Box (2D) intersection test.</summary>
        /// <param name="aCenter"></param>
        /// <param name="aExtents"></param>
        /// <param name="aRotation"></param>
        /// <param name="bCenter"></param>
        /// <param name="bExtents"></param>
        /// <param name="bRotation"></param>
        /// <returns></returns>
        public static bool BoxIntersectsBox(
            FPVector2 aCenter,
            FPVector2 aExtents,
            FP aRotation,
            FPVector2 bCenter,
            FPVector2 bExtents,
            FP bRotation)
        {
            FPCollision.Box a = new FPCollision.Box(aCenter, aExtents, aRotation);
            FPCollision.Box b = new FPCollision.Box(bCenter, bExtents, bRotation);
            return FPCollision.Project(new FPVector2(a.UR.X - a.UL.X, a.UR.Y - a.UL.Y), a, b) & FPCollision.Project(new FPVector2(a.UR.X - a.LR.X, a.UR.Y - a.LR.Y), a, b) & FPCollision.Project(new FPVector2(b.UL.X - b.LL.X, b.UL.Y - b.LL.Y), a, b) & FPCollision.Project(new FPVector2(b.UL.X - b.UR.X, b.UL.Y - b.UR.Y), a, b);
        }

        private static bool Project(FPVector2 axis, FPCollision.Box a, FPCollision.Box b)
        {
            a.UL = FPCollision.Project(axis, a.UL);
            a.UR = FPCollision.Project(axis, a.UR);
            a.LL = FPCollision.Project(axis, a.LL);
            a.LR = FPCollision.Project(axis, a.LR);
            b.UL = FPCollision.Project(axis, b.UL);
            b.UR = FPCollision.Project(axis, b.UR);
            b.LL = FPCollision.Project(axis, b.LL);
            b.LR = FPCollision.Project(axis, b.LR);
            FP maxValue1 = FP.MaxValue;
            FP minValue1 = FP.MinValue;
            FP maxValue2 = FP.MaxValue;
            FP minValue2 = FP.MinValue;
            FP val1_1 = FPVector2.Dot(axis, a.UL);
            FP val2_1 = FPMath.Min(val1_1, maxValue1);
            FP val2_2 = FPMath.Max(val1_1, minValue1);
            FP val1_2 = FPVector2.Dot(axis, a.UR);
            FP val2_3 = FPMath.Min(val1_2, val2_1);
            FP val2_4 = FPMath.Max(val1_2, val2_2);
            FP val1_3 = FPVector2.Dot(axis, a.LL);
            FP val2_5 = FPMath.Min(val1_3, val2_3);
            FP val2_6 = FPMath.Max(val1_3, val2_4);
            FP val1_4 = FPVector2.Dot(axis, a.LR);
            FP fp1 = FPMath.Min(val1_4, val2_5);
            FP fp2 = FPMath.Max(val1_4, val2_6);
            FP val1_5 = FPVector2.Dot(axis, b.UL);
            FP val2_7 = FPMath.Min(val1_5, maxValue2);
            FP val2_8 = FPMath.Max(val1_5, minValue2);
            FP val1_6 = FPVector2.Dot(axis, b.UR);
            FP val2_9 = FPMath.Min(val1_6, val2_7);
            FP val2_10 = FPMath.Max(val1_6, val2_8);
            FP val1_7 = FPVector2.Dot(axis, b.LL);
            FP val2_11 = FPMath.Min(val1_7, val2_9);
            FP val2_12 = FPMath.Max(val1_7, val2_10);
            FP val1_8 = FPVector2.Dot(axis, b.LR);
            FP fp3 = FPMath.Min(val1_8, val2_11);
            FP fp4 = FPMath.Max(val1_8, val2_12);
            return fp3 <= fp2 && fp4 >= fp1;
        }

        private static FPVector2 Project(FPVector2 axis, FPVector2 point)
        {
            FP fp1 = point.X * axis.X + point.Y * axis.Y;
            FP fp2 = axis.X * axis.X + axis.Y * axis.Y;
            return new FPVector2(fp1 / fp2 * axis.X, fp1 / fp2 * axis.Y);
        }

        /// <summary>
        ///     Uses barycentric coordinates to calculate the closest point on a triangle. In conjunction with Fixed Point math
        ///     this can get quite inaccurate when the triangle become large (more than 100 units) or tiny (less then 0.01 units).
        /// </summary>
        /// <param name="p">Point</param>
        /// <param name="a">Vertex 0</param>
        /// <param name="b">Vertex 1</param>
        /// <param name="c">Vertex 2</param>
        /// <param name="closestPoint">Resulting point on the triangle</param>
        /// <param name="barycentricCoordinates">Barycentric coordinates of the point inside the triangle.</param>
        /// <returns>Squared distance to point on the triangle.</returns>
        public static FP ClosestDistanceToTriangle(
            FPVector3 p,
            FPVector3 a,
            FPVector3 b,
            FPVector3 c,
            out FPVector3 closestPoint,
            out FPVector3 barycentricCoordinates)
        {
            barycentricCoordinates.X.RawValue = 0L;
            barycentricCoordinates.Y.RawValue = 0L;
            barycentricCoordinates.Z.RawValue = 0L;
            long rawValue1 = a.X.RawValue;
            long rawValue2 = a.Y.RawValue;
            long rawValue3 = a.Z.RawValue;
            long rawValue4 = b.X.RawValue;
            long rawValue5 = b.Y.RawValue;
            long rawValue6 = b.Z.RawValue;
            long rawValue7 = c.X.RawValue;
            long rawValue8 = c.Y.RawValue;
            long rawValue9 = c.Z.RawValue;
            long num1 = rawValue1 - p.X.RawValue;
            long num2 = rawValue2 - p.Y.RawValue;
            long num3 = rawValue3 - p.Z.RawValue;
            long num4 = rawValue4 - p.X.RawValue;
            long num5 = rawValue5 - p.Y.RawValue;
            long num6 = rawValue6 - p.Z.RawValue;
            long num7 = rawValue7 - p.X.RawValue;
            long num8 = rawValue8 - p.Y.RawValue;
            long num9 = rawValue9 - p.Z.RawValue;
            long num10 = num4 - num1;
            long num11 = num5 - num2;
            long num12 = num6 - num3;
            long num13 = num7 - num1;
            long num14 = num8 - num2;
            long num15 = num9 - num3;
            long num16 = (num11 * num15 + 32768L >> 16) - (num12 * num14 + 32768L >> 16);
            long num17 = (num12 * num13 + 32768L >> 16) - (num10 * num15 + 32768L >> 16);
            long num18 = (num10 * num14 + 32768L >> 16) - (num11 * num13 + 32768L >> 16);
            long num19 = (num16 * num16 >> 16) + (num17 * num17 >> 16) + (num18 * num18 >> 16);
            int num20;
            for (num20 = 0; (Math.Abs(num16) < 8L || Math.Abs(num17) < 8L || Math.Abs(num18) < 8L || Math.Abs(num19) < 8L) && num20 < 4; num19 = (num16 * num16 >> 16) + (num17 * num17 >> 16) + (num18 * num18 >> 16))
            {
                ++num20;
                num1 <<= 1;
                num2 <<= 1;
                num3 <<= 1;
                num4 <<= 1;
                num5 <<= 1;
                num6 <<= 1;
                num7 <<= 1;
                num8 <<= 1;
                num9 <<= 1;
                num10 = num4 - num1;
                num11 = num5 - num2;
                num12 = num6 - num3;
                num13 = num7 - num1;
                num14 = num8 - num2;
                num15 = num9 - num3;
                num16 = (num11 * num15 + 32768L >> 16) - (num12 * num14 + 32768L >> 16);
                num17 = (num12 * num13 + 32768L >> 16) - (num10 * num15 + 32768L >> 16);
                num18 = (num10 * num14 + 32768L >> 16) - (num11 * num13 + 32768L >> 16);
            }

            for (; (Math.Abs(num16) > FP.UseableMax.RawValue || Math.Abs(num17) > FP.UseableMax.RawValue || Math.Abs(num18) > FP.UseableMax.RawValue || Math.Abs(num19) > (long)int.MaxValue) && -num20 < 8; num19 = (num16 * num16 >> 16) + (num17 * num17 >> 16) + (num18 * num18 >> 16))
            {
                num20 -= 2;
                num1 >>= 2;
                num2 >>= 2;
                num3 >>= 2;
                num4 >>= 2;
                num5 >>= 2;
                num6 >>= 2;
                num7 >>= 2;
                num8 >>= 2;
                num9 >>= 2;
                num10 = num4 - num1;
                num11 = num5 - num2;
                num12 = num6 - num3;
                num13 = num7 - num1;
                num14 = num8 - num2;
                num15 = num9 - num3;
                num16 = (num11 * num15 + 32768L >> 16) - (num12 * num14 + 32768L >> 16);
                num17 = (num12 * num13 + 32768L >> 16) - (num10 * num15 + 32768L >> 16);
                num18 = (num10 * num14 + 32768L >> 16) - (num11 * num13 + 32768L >> 16);
            }

            FP fp1;
            fp1.RawValue = (num10 * num7 + 32768L >> 16) + (num11 * num8 + 32768L >> 16) + (num12 * num9 + 32768L >> 16);
            FP fp2;
            fp2.RawValue = (num13 * num7 + 32768L >> 16) + (num14 * num8 + 32768L >> 16) + (num15 * num9 + 32768L >> 16);
            if (fp2.RawValue <= 0L && fp1.RawValue >= fp2.RawValue)
            {
                barycentricCoordinates.Z.RawValue = 65536L;
                closestPoint = c;
            }
            else
            {
                FP fp3;
                fp3.RawValue = (num10 * num1 + 32768L >> 16) + (num11 * num2 + 32768L >> 16) + (num12 * num3 + 32768L >> 16);
                FP fp4;
                fp4.RawValue = (num13 * num1 + 32768L >> 16) + (num14 * num2 + 32768L >> 16) + (num15 * num3 + 32768L >> 16);
                if (fp3.RawValue >= 0L && fp4.RawValue >= 0L)
                {
                    barycentricCoordinates.X.RawValue = 65536L;
                    closestPoint = a;
                }
                else
                {
                    FP fp5;
                    fp5.RawValue = (fp1.RawValue * fp4.RawValue + 32768L >> 16) - (fp3.RawValue * fp2.RawValue + 32768L >> 16);
                    if (fp5.RawValue <= 0L && fp4.RawValue <= 0L && fp2.RawValue >= 0L)
                    {
                        long num21;
                        long num22;
                        if (fp4.RawValue == fp2.RawValue)
                        {
                            num21 = 32768L;
                            num22 = 32768L;
                        }
                        else
                        {
                            num22 = (fp4.RawValue << 16) / (fp4.RawValue - fp2.RawValue);
                            num21 = 65536L - num22;
                        }

                        closestPoint.X.RawValue = num21 * a.X.RawValue + num22 * c.X.RawValue + 32768L >> 16;
                        closestPoint.Y.RawValue = num21 * a.Y.RawValue + num22 * c.Y.RawValue + 32768L >> 16;
                        closestPoint.Z.RawValue = num21 * a.Z.RawValue + num22 * c.Z.RawValue + 32768L >> 16;
                        barycentricCoordinates.X.RawValue = num21;
                        barycentricCoordinates.Z.RawValue = num22;
                    }
                    else
                    {
                        FP fp6;
                        fp6.RawValue = (num10 * num4 + 32768L >> 16) + (num11 * num5 + 32768L >> 16) + (num12 * num6 + 32768L >> 16);
                        FP fp7;
                        fp7.RawValue = (num13 * num4 + 32768L >> 16) + (num14 * num5 + 32768L >> 16) + (num15 * num6 + 32768L >> 16);
                        if (fp6.RawValue <= 0L && fp7.RawValue >= fp6.RawValue)
                        {
                            barycentricCoordinates.Y.RawValue = 65536L;
                            closestPoint = b;
                        }
                        else
                        {
                            FP fp8;
                            fp8.RawValue = (fp6.RawValue * fp2.RawValue + 32768L >> 16) - (fp1.RawValue * fp7.RawValue + 32768L >> 16);
                            if (fp8.RawValue <= 0L && fp6.RawValue >= fp7.RawValue && fp2.RawValue >= fp1.RawValue)
                            {
                                FP fp9;
                                fp9.RawValue = fp6.RawValue - fp7.RawValue + (fp2.RawValue - fp1.RawValue);
                                long num23;
                                long num24;
                                if (fp9.RawValue == 0L)
                                {
                                    num23 = 32768L;
                                    num24 = 32768L;
                                }
                                else
                                {
                                    num23 = (fp6.RawValue - fp7.RawValue << 16) / fp9.RawValue;
                                    num24 = 65536L - num23;
                                }

                                closestPoint.X.RawValue = num23 * c.X.RawValue + num24 * b.X.RawValue + 32768L >> 16;
                                closestPoint.Y.RawValue = num23 * c.Y.RawValue + num24 * b.Y.RawValue + 32768L >> 16;
                                closestPoint.Z.RawValue = num23 * c.Z.RawValue + num24 * b.Z.RawValue + 32768L >> 16;
                                barycentricCoordinates.Y.RawValue = num24;
                                barycentricCoordinates.Z.RawValue = num23;
                            }
                            else
                            {
                                FP fp10;
                                fp10.RawValue = (fp3.RawValue * fp7.RawValue + 32768L >> 16) - (fp6.RawValue * fp4.RawValue + 32768L >> 16);
                                if (fp10.RawValue <= 0L && fp3.RawValue <= 0L && fp6.RawValue >= 0L)
                                {
                                    long num25;
                                    long num26;
                                    if (fp3.RawValue == fp6.RawValue)
                                    {
                                        num25 = 32768L;
                                        num26 = 32768L;
                                    }
                                    else
                                    {
                                        num26 = (fp3.RawValue << 16) / (fp3.RawValue - fp6.RawValue);
                                        num25 = 65536L - num26;
                                    }

                                    closestPoint.X.RawValue = num25 * a.X.RawValue + num26 * b.X.RawValue + 32768L >> 16;
                                    closestPoint.Y.RawValue = num25 * a.Y.RawValue + num26 * b.Y.RawValue + 32768L >> 16;
                                    closestPoint.Z.RawValue = num25 * a.Z.RawValue + num26 * b.Z.RawValue + 32768L >> 16;
                                    barycentricCoordinates.X.RawValue = num25;
                                    barycentricCoordinates.Y.RawValue = num26;
                                }
                                else
                                {
                                    FP fp11;
                                    fp11.RawValue = fp8.RawValue + fp5.RawValue + fp10.RawValue;
                                    long num27;
                                    long num28;
                                    long num29;
                                    if (fp11.RawValue == 0L)
                                    {
                                        num27 = 21845L;
                                        num28 = 21845L;
                                        num29 = 21845L;
                                    }
                                    else
                                    {
                                        num28 = (fp5.RawValue << 16) / fp11.RawValue;
                                        num29 = (fp10.RawValue << 16) / fp11.RawValue;
                                        num27 = 65536L - num28 - num29;
                                    }

                                    closestPoint.X.RawValue = num27 * a.X.RawValue + num28 * b.X.RawValue + num29 * c.X.RawValue + 32768L >> 16;
                                    closestPoint.Y.RawValue = num27 * a.Y.RawValue + num28 * b.Y.RawValue + num29 * c.Y.RawValue + 32768L >> 16;
                                    closestPoint.Z.RawValue = num27 * a.Z.RawValue + num28 * b.Z.RawValue + num29 * c.Z.RawValue + 32768L >> 16;
                                    barycentricCoordinates.X.RawValue = num27;
                                    barycentricCoordinates.Y.RawValue = num28;
                                    barycentricCoordinates.Z.RawValue = num29;
                                }
                            }
                        }
                    }
                }
            }

            FP triangle;
            triangle.RawValue = ((p.X.RawValue - closestPoint.X.RawValue) * (p.X.RawValue - closestPoint.X.RawValue) + 32768L >> 16) + ((p.Y.RawValue - closestPoint.Y.RawValue) * (p.Y.RawValue - closestPoint.Y.RawValue) + 32768L >> 16) + ((p.Z.RawValue - closestPoint.Z.RawValue) * (p.Z.RawValue - closestPoint.Z.RawValue) + 32768L >> 16);
            return triangle;
        }

        /// <summary>
        ///     Checks if a point is inside the extents of an AABB centered at the origin (local space) and clamp it otherwise.
        /// </summary>
        /// <param name="point">Point in the local space of the AABB.</param>
        /// <param name="aabbExtents">Extents of the AABB.</param>
        /// <param name="clampedPoint">
        ///     Clamped point inside the AABB. Equals to <paramref name="point" /> if it is already inside
        ///     the AABB.
        /// </param>
        /// <returns><see langword="true" /> if the point is already inside the AABB.</returns>
        public static bool ClampPointToLocalAABB(
            FPVector3 point,
            FPVector3 aabbExtents,
            out FPVector3 clampedPoint)
        {
            bool localAabb = true;
            if (point.X.RawValue < -aabbExtents.X.RawValue)
            {
                localAabb = false;
                point.X.RawValue = -aabbExtents.X.RawValue;
            }
            else if (point.X.RawValue > aabbExtents.X.RawValue)
            {
                localAabb = false;
                point.X.RawValue = aabbExtents.X.RawValue;
            }

            if (point.Y.RawValue < -aabbExtents.Y.RawValue)
            {
                localAabb = false;
                point.Y.RawValue = -aabbExtents.Y.RawValue;
            }
            else if (point.Y.RawValue > aabbExtents.Y.RawValue)
            {
                localAabb = false;
                point.Y.RawValue = aabbExtents.Y.RawValue;
            }

            if (point.Z.RawValue < -aabbExtents.Z.RawValue)
            {
                localAabb = false;
                point.Z.RawValue = -aabbExtents.Z.RawValue;
            }
            else if (point.Z.RawValue > aabbExtents.Z.RawValue)
            {
                localAabb = false;
                point.Z.RawValue = aabbExtents.Z.RawValue;
            }

            clampedPoint = point;
            return localAabb;
        }

        /// <summary>
        ///     Calculates the closest point on the line segment defined by two given points to a given point.
        /// </summary>
        /// <param name="point">The point to which the closest point on the segment is calculated.</param>
        /// <param name="a">The first point of the line segment.</param>
        /// <param name="b">The second point of the line segment.</param>
        /// <returns>The closest point on the line segment to the given point.</returns>
        public static FPVector3 ClosestPointInSegment(
            FPVector3 point,
            FPVector3 a,
            FPVector3 b)
        {
            FPVector3 fpVector3_1;
            fpVector3_1.X.RawValue = b.X.RawValue - a.X.RawValue;
            fpVector3_1.Y.RawValue = b.Y.RawValue - a.Y.RawValue;
            fpVector3_1.Z.RawValue = b.Z.RawValue - a.Z.RawValue;
            FP fp1;
            fp1.RawValue = (fpVector3_1.X.RawValue * fpVector3_1.X.RawValue + 32768L >> 16) + (fpVector3_1.Y.RawValue * fpVector3_1.Y.RawValue + 32768L >> 16) + (fpVector3_1.Z.RawValue * fpVector3_1.Z.RawValue + 32768L >> 16);
            if (fp1.RawValue <= 0L)
                return a;
            FPVector3 fpVector3_2;
            fpVector3_2.X.RawValue = point.X.RawValue - a.X.RawValue;
            fpVector3_2.Y.RawValue = point.Y.RawValue - a.Y.RawValue;
            fpVector3_2.Z.RawValue = point.Z.RawValue - a.Z.RawValue;
            FP fp2;
            fp2.RawValue = (fpVector3_2.X.RawValue * fpVector3_1.X.RawValue + 32768L >> 16) + (fpVector3_2.Y.RawValue * fpVector3_1.Y.RawValue + 32768L >> 16) + (fpVector3_2.Z.RawValue * fpVector3_1.Z.RawValue + 32768L >> 16);
            fp2.RawValue = (fp2.RawValue << 16) / fp1.RawValue;
            if (fp2.RawValue < 0L)
                return a;
            if (fp2.RawValue > 65536L)
                return b;
            a.X.RawValue += (b.X.RawValue - a.X.RawValue) * fp2.RawValue + 32768L >> 16;
            a.Y.RawValue += (b.Y.RawValue - a.Y.RawValue) * fp2.RawValue + 32768L >> 16;
            a.Z.RawValue += (b.Z.RawValue - a.Z.RawValue) * fp2.RawValue + 32768L >> 16;
            return a;
        }

        /// <summary>
        ///     Computes the closes point in segment A to a segment B.
        /// </summary>
        /// <param name="segmentStartA">Start point of segment A.</param>
        /// <param name="segmentEndA">End point of segment A.</param>
        /// <param name="segmentStartB">Start point of segment A.</param>
        /// <param name="segmentEndB">End point of segment B.</param>
        /// <returns></returns>
        public static FPVector3 ClosestPointBetweenSegments(
            FPVector3 segmentStartA,
            FPVector3 segmentEndA,
            FPVector3 segmentStartB,
            FPVector3 segmentEndB)
        {
            FPVector3 fpVector3_1;
            fpVector3_1.X.RawValue = segmentEndA.X.RawValue - segmentStartA.X.RawValue;
            fpVector3_1.Y.RawValue = segmentEndA.Y.RawValue - segmentStartA.Y.RawValue;
            fpVector3_1.Z.RawValue = segmentEndA.Z.RawValue - segmentStartA.Z.RawValue;
            FPVector3 fpVector3_2;
            fpVector3_2.X.RawValue = segmentEndB.X.RawValue - segmentStartB.X.RawValue;
            fpVector3_2.Y.RawValue = segmentEndB.Y.RawValue - segmentStartB.Y.RawValue;
            fpVector3_2.Z.RawValue = segmentEndB.Z.RawValue - segmentStartB.Z.RawValue;
            FPVector3 b1;
            b1.X.RawValue = segmentStartA.X.RawValue - segmentStartB.X.RawValue;
            b1.Y.RawValue = segmentStartA.Y.RawValue - segmentStartB.Y.RawValue;
            b1.Z.RawValue = segmentStartA.Z.RawValue - segmentStartB.Z.RawValue;
            FP fp1 = FPVector3.Dot(fpVector3_1, fpVector3_1);
            {
                fp1.RawValue = (fpVector3_1.X.RawValue * fpVector3_1.X.RawValue + 32768L >> 16) + (fpVector3_1.Y.RawValue * fpVector3_1.Y.RawValue + 32768L >> 16) + (fpVector3_1.Z.RawValue * fpVector3_1.Z.RawValue + 32768L >> 16);
            }
            ;
            FP fp2 = FPVector3.Dot(fpVector3_1, fpVector3_2);
            {
                fp2.RawValue = (fpVector3_1.X.RawValue * fpVector3_2.X.RawValue + 32768L >> 16) + (fpVector3_1.Y.RawValue * fpVector3_2.Y.RawValue + 32768L >> 16) + (fpVector3_1.Z.RawValue * fpVector3_2.Z.RawValue + 32768L >> 16);
            }
            ;
            FP fp3 = FPVector3.Dot(fpVector3_2, fpVector3_2);
            {
                fp3.RawValue = (fpVector3_2.X.RawValue * fpVector3_2.X.RawValue + 32768L >> 16) + (fpVector3_2.Y.RawValue * fpVector3_2.Y.RawValue + 32768L >> 16) + (fpVector3_2.Z.RawValue * fpVector3_2.Z.RawValue + 32768L >> 16);
            }
            ;
            FP fp4 = FPVector3.Dot(fpVector3_1, b1);
            {
                fp4.RawValue = (fpVector3_1.X.RawValue * b1.X.RawValue + 32768L >> 16) + (fpVector3_1.Y.RawValue * b1.Y.RawValue + 32768L >> 16) + (fpVector3_1.Z.RawValue * b1.Z.RawValue + 32768L >> 16);
            }
            ;
            FP fp5 = FPVector3.Dot(fpVector3_2, b1);
            {
                fp5.RawValue = (fpVector3_2.X.RawValue * b1.X.RawValue + 32768L >> 16) + (fpVector3_2.Y.RawValue * b1.Y.RawValue + 32768L >> 16) + (fpVector3_2.Z.RawValue * b1.Z.RawValue + 32768L >> 16);
            }
            ;
            FP fp6 = fp1 * fp3 - fp2 * fp2;
            FP fp7 = fp6;
            FP fp8 = fp6;
            FP fp9;
            FP fp10;
            if (fp6 < FP.Epsilon)
            {
                fp9 = FP._0;
                fp7 = FP._1;
                fp10 = fp5;
                fp8 = fp3;
            }
            else
            {
                fp9 = fp2 * fp5 - fp3 * fp4;
                fp10 = fp1 * fp5 - fp2 * fp4;
                if (fp9 < FP._0)
                {
                    fp9 = FP._0;
                    fp10 = fp5;
                    fp8 = fp3;
                }
                else if (fp9 > fp7)
                {
                    fp9 = fp7;
                    fp10 = fp5 + fp2;
                    fp8 = fp3;
                }
            }

            if (fp10 < FP._0)
            {
                fp10 = FP._0;
                if (-fp4 < FP._0)
                    fp9 = FP._0;
                else if (-fp4 > fp1)
                {
                    fp9 = fp7;
                }
                else
                {
                    fp9 = -fp4;
                    fp7 = fp1;
                }
            }
            else if (fp10 > fp8)
            {
                fp10 = fp8;
                if (-fp4 + fp2 < FP._0)
                    fp9 = FP._0;
                else if (-fp4 + fp2 > fp1)
                {
                    fp9 = fp7;
                }
                else
                {
                    fp9 = -fp4 + fp2;
                    fp7 = fp1;
                }
            }

            FP fp11 = FPMath.Abs(fp9) < FP.Epsilon ? FP._0 : fp9 / fp7;
            FP fp12 = FPMath.Abs(fp10) < FP.Epsilon ? FP._0 : fp10 / fp8;
            FPVector3 a = segmentStartA + fp11 * fpVector3_1;
            FPVector3 b2 = segmentStartB + fp12 * fpVector3_2;
            FP fp13 = FPVector3.Distance(a, b2);
            if (fp13 < FP.Epsilon)
                return a;
            FP fp14 = FPVector3.Distance(segmentStartA, segmentStartB);
            FP fp15 = FPVector3.Distance(segmentStartA, segmentEndB);
            FP fp16 = FPVector3.Distance(segmentEndA, segmentStartB);
            FP fp17 = FPVector3.Distance(segmentEndA, segmentEndB);
            FP fp18 = fp13;
            FPVector3 fpVector3_3 = a;
            if (fp14 < fp18)
            {
                fp18 = fp14;
                fpVector3_3 = segmentStartA;
            }

            if (fp15 < fp18)
            {
                fp18 = fp15;
                fpVector3_3 = segmentStartA;
            }

            if (fp16 < fp18)
            {
                fp18 = fp16;
                fpVector3_3 = segmentEndA;
            }

            if (fp17 < fp18)
                fpVector3_3 = segmentEndA;
            return fpVector3_3;
        }

        private struct Box
        {
            public FPVector2 UL;
            public FPVector2 UR;
            public FPVector2 LL;
            public FPVector2 LR;

            public Box(FPVector2 center, FPVector2 extents, FP rotation)
            {
                this.UL.X = -extents.X;
                this.UL.Y = +extents.Y;
                this.UR.X = +extents.X;
                this.UR.Y = +extents.Y;
                this.LR.X = +extents.X;
                this.LR.Y = -extents.Y;
                this.LL.X = -extents.X;
                this.LL.Y = -extents.Y;
                this.UL = center + FPVector2.Rotate(this.UL, rotation);
                this.UR = center + FPVector2.Rotate(this.UR, rotation);
                this.LR = center + FPVector2.Rotate(this.LR, rotation);
                this.LL = center + FPVector2.Rotate(this.LL, rotation);
            }
        }
    }
}