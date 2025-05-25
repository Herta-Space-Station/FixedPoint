using System;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta.Components
{
    /// <summary>
    ///     The Transform2D is an entity component providing position and rotation a 2D object.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Transform2D : IFPComponent
    {
        /// <summary>
        ///     The size of the component (or struct/type) in-memory inside the Frame data-buffers or stack (when passed as value
        ///     parameter).
        ///     Not related to the snapshot payload this occupies, which is bit-packed and compressed.
        /// </summary>
        public const int SIZE = 32;

        internal const int OFFSET_POS = 0;
        internal const int OFFSET_ROT = 16;

        /// <summary>The world position of the entity.</summary>
        [FieldOffset(0)] public FPVector2 Position;

        /// <summary>The rotation in radians.</summary>
        [FieldOffset(16)] public FP Rotation;

        /// <summary>Create method to create a new Transform2D.</summary>
        /// <param name="position">Initial position</param>
        /// <param name="rotation">Initial rotation</param>
        /// <returns></returns>
        public static Transform2D Create(FPVector2 position = default(FPVector2), FP rotation = default(FP)) => new Transform2D()
        {
            Position = position,
            Rotation = rotation
        };

        /// <summary>Calculate the right vector.</summary>
        public FPVector2 Right
        {
            get
            {
                FPVector2 right = new FPVector2();
                long sinRaw;
                long cosRaw;
                FPMath.SinCosRaw(this.Rotation, out sinRaw, out cosRaw);
                right.X.RawValue = cosRaw;
                right.Y.RawValue = sinRaw;
                return right;
            }
        }

        /// <summary>Calculate the left vector.</summary>
        public FPVector2 Left
        {
            get
            {
                FPVector2 left = new FPVector2();
                long sinRaw;
                long cosRaw;
                FPMath.SinCosRaw(this.Rotation, out sinRaw, out cosRaw);
                left.X.RawValue = -cosRaw;
                left.Y.RawValue = -sinRaw;
                return left;
            }
        }

        /// <summary>Calculate the up vector.</summary>
        public FPVector2 Up
        {
            get
            {
                FPVector2 up = new FPVector2();
                long sinRaw;
                long cosRaw;
                FPMath.SinCosRaw(this.Rotation, out sinRaw, out cosRaw);
                up.X.RawValue = -sinRaw;
                up.Y.RawValue = cosRaw;
                return up;
            }
        }

        /// <summary>Calculate the down vector.</summary>
        public FPVector2 Down
        {
            get
            {
                FPVector2 down = new FPVector2();
                long sinRaw;
                long cosRaw;
                FPMath.SinCosRaw(this.Rotation, out sinRaw, out cosRaw);
                down.X.RawValue = sinRaw;
                down.Y.RawValue = -cosRaw;
                return down;
            }
        }

        /// <summary>Calculate the forward vector (same as right).</summary>
        public FPVector2 Forward => this.Right;

        /// <summary>Calculate the back vector (same as left).</summary>
        public FPVector2 Back => this.Left;

        /// <summary>
        ///     Transforms a <paramref name="point" /> from local to world space.
        ///     See also: <seealso cref="M:Herta.Components.Transform2D.InverseTransformPoint(Herta.FPVector2)" />.
        /// </summary>
        /// <param name="point">A point in local space.</param>
        /// <returns>The transformed point in world space.</returns>
        public FPVector2 TransformPoint(FPVector2 point)
        {
            long sinRaw;
            long cosRaw;
            FPMath.SinCosRaw(this.Rotation, out sinRaw, out cosRaw);
            FPVector2 fpVector2;
            fpVector2.X.RawValue = this.Position.X.RawValue + ((point.X.RawValue * cosRaw + 32768L >> 16) - (point.Y.RawValue * sinRaw + 32768L >> 16));
            fpVector2.Y.RawValue = this.Position.Y.RawValue + ((point.X.RawValue * sinRaw + 32768L >> 16) + (point.Y.RawValue * cosRaw + 32768L >> 16));
            return fpVector2;
        }

        /// <summary>
        ///     Transforms a <paramref name="point" /> from world to local space.
        ///     See also: <seealso cref="M:Herta.Components.Transform2D.TransformPoint(Herta.FPVector2)" />.
        /// </summary>
        /// <param name="point">A point in world space.</param>
        /// <returns>The transformed point in local space.</returns>
        public FPVector2 InverseTransformPoint(FPVector2 point)
        {
            long sinRaw;
            long cosRaw;
            FPMath.SinCosRaw(this.Rotation, out sinRaw, out cosRaw);
            point.X.RawValue -= this.Position.X.RawValue;
            point.Y.RawValue -= this.Position.Y.RawValue;
            FPVector2 fpVector2;
            fpVector2.X.RawValue = (point.X.RawValue * cosRaw + 32768L >> 16) + (point.Y.RawValue * sinRaw + 32768L >> 16);
            fpVector2.Y.RawValue = (point.Y.RawValue * cosRaw + 32768L >> 16) - (point.X.RawValue * sinRaw + 32768L >> 16);
            return fpVector2;
        }

        /// <summary>
        ///     Transforms a <paramref name="direction" /> from local to world space.
        ///     See also: <seealso cref="M:Herta.Components.Transform2D.InverseTransformDirection(Herta.FPVector2)" />.
        /// </summary>
        /// <param name="direction">A direction in local space.</param>
        /// <returns>The transformed direction in world space.</returns>
        public FPVector2 TransformDirection(FPVector2 direction)
        {
            long sinRaw;
            long cosRaw;
            FPMath.SinCosRaw(this.Rotation, out sinRaw, out cosRaw);
            FPVector2 fpVector2;
            fpVector2.X.RawValue = (direction.X.RawValue * cosRaw + 32768L >> 16) - (direction.Y.RawValue * sinRaw + 32768L >> 16);
            fpVector2.Y.RawValue = (direction.X.RawValue * sinRaw + 32768L >> 16) + (direction.Y.RawValue * cosRaw + 32768L >> 16);
            return fpVector2;
        }

        /// <summary>
        ///     Transforms a <paramref name="direction" /> from world to local space.
        ///     See also: <seealso cref="M:Herta.Components.Transform2D.TransformDirection(Herta.FPVector2)" />.
        /// </summary>
        /// <param name="direction">A direction in world space.</param>
        /// <returns>The transformed direction in local space.</returns>
        public FPVector2 InverseTransformDirection(FPVector2 direction)
        {
            long sinRaw;
            long cosRaw;
            FPMath.SinCosRaw(this.Rotation, out sinRaw, out cosRaw);
            FPVector2 fpVector2;
            fpVector2.X.RawValue = (direction.X.RawValue * cosRaw + 32768L >> 16) + (direction.Y.RawValue * sinRaw + 32768L >> 16);
            fpVector2.Y.RawValue = (direction.Y.RawValue * cosRaw + 32768L >> 16) - (direction.X.RawValue * sinRaw + 32768L >> 16);
            return fpVector2;
        }

        /// <summary>Overrides the hash code generation of this type.</summary>
        /// <returns>A hash code of the current state of this instance.</returns>
        public override int GetHashCode() => XxHash.Hash32(this);
    }
}