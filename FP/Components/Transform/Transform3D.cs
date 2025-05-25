using System;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta.Components
{
    /// <summary>
    ///     The Transform3D is an entity component providing position and rotation for a 3D object.
    ///     <para>
    ///         Always use the factory create method
    ///         <see cref="M:Herta.Components.Transform3D.Create(Herta.FPVector3)" /> or
    ///         <see cref="M:Herta.Components.Transform3D.Create(Herta.FPVector3,Herta.FPQuaternion)" />
    ///         or correctly initialize the rotation Quaternion manually.
    ///     </para>
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Transform3D:IFPComponent
    {
        /// <summary>
        ///     The size of the component (or struct/type) in-memory inside the Frame data-buffers or stack (when passed as value
        ///     parameter).
        ///     Not related to the snapshot payload this occupies, which is bit-packed and compressed.
        /// </summary>
        public const int SIZE = 64;

        internal const int OFFSET_POS = 0;
        internal const int OFFSET_ROT = 24;

        /// <summary>The world position of the entity.</summary>
        [FieldOffset(0)] public FPVector3 Position;

        /// <summary>The rotation of the entity.</summary>
        [FieldOffset(24)] public FPQuaternion Rotation;

        /// <summary>
        ///     Factory create a Transform3D component.
        ///     <para>
        ///         Sets the <see cref="F:Herta.Components.Transform3D.Rotation" /> to
        ///         <see cref="P:Herta.FPQuaternion.Identity" />.
        ///     </para>
        /// </summary>
        /// <param name="position">The initial world position.</param>
        /// <returns>New initialized Transform3D component.</returns>
        public static Transform3D Create(FPVector3 position = default(FPVector3)) => new Transform3D()
        {
            Position = position,
            Rotation = FPQuaternion.Identity
        };

        /// <summary>Factory create a Transform3D component.</summary>
        /// <param name="position">The initial world position.</param>
        /// <param name="rotation">The initial rotation.</param>
        /// <returns>New initialized Transform3D component.</returns>
        public static Transform3D Create(FPVector3 position, FPQuaternion rotation) => new Transform3D()
        {
            Position = position,
            Rotation = rotation
        };

        /// <summary>A vector pointing to local +X axis.</summary>
        public FPVector3 Right
        {
            get
            {
                FPVector3 right = new FPVector3();
                long num1 = this.Rotation.Y.RawValue << 1;
                long num2 = this.Rotation.Z.RawValue << 1;
                right.X.RawValue = 65536L - ((this.Rotation.Y.RawValue * num1 + 32768L >> 16) + (this.Rotation.Z.RawValue * num2 + 32768L >> 16));
                right.Y.RawValue = (this.Rotation.X.RawValue * num1 + 32768L >> 16) + (this.Rotation.W.RawValue * num2 + 32768L >> 16);
                right.Z.RawValue = (this.Rotation.X.RawValue * num2 + 32768L >> 16) - (this.Rotation.W.RawValue * num1 + 32768L >> 16);
                return right;
            }
        }

        /// <summary>A vector pointing to local +Y axis.</summary>
        public FPVector3 Up
        {
            get
            {
                FPVector3 up = new FPVector3();
                long num1 = this.Rotation.X.RawValue << 1;
                long num2 = this.Rotation.Y.RawValue << 1;
                long num3 = this.Rotation.Z.RawValue << 1;
                up.X.RawValue = (this.Rotation.X.RawValue * num2 + 32768L >> 16) - (this.Rotation.W.RawValue * num3 + 32768L >> 16);
                up.Y.RawValue = 65536L - ((this.Rotation.X.RawValue * num1 + 32768L >> 16) + (this.Rotation.Z.RawValue * num3 + 32768L >> 16));
                up.Z.RawValue = (this.Rotation.Y.RawValue * num3 + 32768L >> 16) + (this.Rotation.W.RawValue * num1 + 32768L >> 16);
                return up;
            }
        }

        /// <summary>A vector pointing to local +Z axis.</summary>
        public FPVector3 Forward
        {
            get
            {
                FPVector3 forward = new FPVector3();
                long num1 = this.Rotation.X.RawValue << 1;
                long num2 = this.Rotation.Y.RawValue << 1;
                long num3 = this.Rotation.Z.RawValue << 1;
                forward.X.RawValue = (this.Rotation.X.RawValue * num3 + 32768L >> 16) + (this.Rotation.W.RawValue * num2 + 32768L >> 16);
                forward.Y.RawValue = (this.Rotation.Y.RawValue * num3 + 32768L >> 16) - (this.Rotation.W.RawValue * num1 + 32768L >> 16);
                forward.Z.RawValue = 65536L - ((this.Rotation.X.RawValue * num1 + 32768L >> 16) + (this.Rotation.Y.RawValue * num2 + 32768L >> 16));
                return forward;
            }
        }

        /// <summary>A vector pointing to local -X axis.</summary>
        public FPVector3 Left => -this.Right;

        /// <summary>A vector pointing to local -Y axis.</summary>
        public FPVector3 Down => -this.Up;

        /// <summary>A vector pointing to local -Z axis.</summary>
        public FPVector3 Back => -this.Forward;

        /// <summary>Overrides the hash code generation of this type.</summary>
        /// <returns>A hash code of the current state of this instance.</returns>
        public override int GetHashCode() => XxHash.Hash32(this);

        /// <summary>Converts rotation to Euler angles triplet.</summary>
        /// <seealso cref="P:Herta.FPQuaternion.AsEuler" />
        /// .
        public FPVector3 EulerAngles => this.Rotation.AsEuler;

        /// <summary>
        ///     Creates local to world transformation. <see cref="F:Herta.Components.Transform3D.Rotation" /> is expected to be
        ///     normalized.
        /// </summary>
        public FPMatrix4x4 LocalToWorldMatrix => FPMatrix4x4.TRS(this.Position, this.Rotation, FPVector3.One);

        /// <summary>
        ///     Creates world to local transformation. <see cref="F:Herta.Components.Transform3D.Rotation" /> is expected to be
        ///     normalized.
        /// </summary>
        public FPMatrix4x4 WorldToLocalMatrix => FPMatrix4x4.InverseTRS(this.Position, this.Rotation, FPVector3.One);

        /// <summary>
        ///     Transforms a direction from world space to local space. <see cref="F:Herta.Components.Transform3D.Rotation" /> is
        ///     expected
        ///     to be normalized.
        /// </summary>
        public FPVector3 InverseTransformDirection(FPVector3 direction) => this.Rotation.Conjugated * direction;

        /// <summary>
        ///     Transforms a direction from local space to world space. <see cref="F:Herta.Components.Transform3D.Rotation" /> is
        ///     expected
        ///     to be normalized.
        /// </summary>
        public FPVector3 TransformDirection(FPVector3 direction) => this.Rotation * direction;

        /// <summary>
        ///     Transforms a position from world space to local space. <see cref="F:Herta.Components.Transform3D.Rotation" /> is
        ///     expected to
        ///     be normalized.
        /// </summary>
        public FPVector3 InverseTransformPoint(FPVector3 position) => this.Rotation.Conjugated * (position - this.Position);

        /// <summary>
        ///     Transforms a position from local space to world space. <see cref="F:Herta.Components.Transform3D.Rotation" /> is
        ///     expected to
        ///     be normalized.
        /// </summary>
        public FPVector3 TransformPoint(FPVector3 position) => this.Rotation * position + this.Position;

        /// <summary>
        ///     Rotates the transform so the forward vector points at <paramref name="position" />. If <paramref name="up" /> is
        ///     not set, will use <see cref="P:Herta.FPVector3.Up" /> instead.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="up"></param>
        public void LookAt(FPVector3 position, FPVector3? up = null)
        {
            FPVector3 forward = position - this.Position;
            if (forward.SqrMagnitude == 0)
                return;
            this.Rotation = FPQuaternion.LookRotation(forward, up ?? FPVector3.Up, true);
        }

        /// <summary>
        ///     Applies a rotation of <paramref name="angles" />.z degrees around the z-axis, <paramref name="angles" />.x degrees
        ///     around the x-axis, and <paramref name="angles" />.y degrees around the y-axis (in that order).
        /// </summary>
        /// <param name="angles">Euler angles, in degrees.</param>
        /// <param name="normalize">
        ///     If the resultant quaternion should be normalized before being set to the
        ///     <see cref="F:Herta.Components.Transform3D.Rotation" />.
        ///     This prevents rounding error from accumulating and potentially rounding the quaternion magnitude towards zero over
        ///     time.
        /// </param>
        public void Rotate(FPVector3 angles, bool normalize = true)
        {
            this.Rotation = FPQuaternion.Product(this.Rotation, FPQuaternion.Euler(angles));
            if (!normalize)
                return;
            this.Rotation = FPQuaternion.Normalize(this.Rotation);
        }

        /// <summary>
        ///     Applies a rotation of <paramref name="zAngle" /> degrees around the z-axis, <paramref name="xAngle" /> degrees
        ///     around the x-axis, and <paramref name="yAngle" /> degrees around the y-axis (in that order).
        /// </summary>
        /// <param name="xAngle"></param>
        /// <param name="yAngle"></param>
        /// <param name="zAngle"></param>
        /// <param name="normalize">
        ///     If the resultant quaternion should be normalized before being set to the
        ///     <see cref="F:Herta.Components.Transform3D.Rotation" />.
        ///     This prevents rounding error from accumulating and potentially rounding the quaternion magnitude towards zero over
        ///     time.
        /// </param>
        public void Rotate(FP xAngle, FP yAngle, FP zAngle, bool normalize = true)
        {
            this.Rotation = FPQuaternion.Product(this.Rotation, FPQuaternion.Euler(xAngle, yAngle, zAngle));
            if (!normalize)
                return;
            this.Rotation = FPQuaternion.Normalize(this.Rotation);
        }

        /// <summary>
        ///     Rotates the object around the given axis by the number of degrees defined by the given angle.
        /// </summary>
        /// <param name="axis">The axis around which to rotate.</param>
        /// <param name="angle">The angle of rotation around the axis.</param>
        /// <param name="normalize">
        ///     If the resultant quaternion should be normalized before being set to the
        ///     <see cref="F:Herta.Components.Transform3D.Rotation" />.
        ///     This prevents rounding error from accumulating and potentially rounding the quaternion magnitude towards zero over
        ///     time.
        /// </param>
        /// <param name="space">
        ///     If the <paramref name="axis" /> is defined in the Local space of the Transform (default) or in the
        ///     World space.
        /// </param>
        public void Rotate(FPVector3 axis, FP angle, bool normalize = true, TransformSpace space = TransformSpace.Local)
        {
            this.Rotation = space != TransformSpace.Local ? FPQuaternion.Product(FPQuaternion.AngleAxis(angle, axis), this.Rotation) : FPQuaternion.Product(this.Rotation, FPQuaternion.AngleAxis(angle, axis));
            if (!normalize)
                return;
            this.Rotation = FPQuaternion.Normalize(this.Rotation);
        }

        /// <summary>Rotates around a given pivot point and axis.</summary>
        /// <param name="point">The pivot point around which to rotate.</param>
        /// <param name="axis">The axis in WORLD space around which to rotate.</param>
        /// <param name="angle">The angle of rotation around the axis.</param>
        /// <param name="rotateTransform">
        ///     If the transform Rotation should also be modified.
        ///     If false, only the transform Position is modified.
        /// </param>
        public void RotateAround(FPVector3 point, FPVector3 axis, FP angle, bool rotateTransform = true)
        {
            FPVector3 fpVector3_1 = this.Position - point;
            FPVector3 fpVector3_2 = FPQuaternion.AngleAxis(angle, axis) * fpVector3_1;
            this.Position = point + fpVector3_2;
            if (!rotateTransform)
                return;
            this.Rotate(axis, angle, space: TransformSpace.World);
        }
    }
}