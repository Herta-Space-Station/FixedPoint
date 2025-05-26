using System;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     A serializable equivalent of Nullable&lt;FPVector2&gt;.
    /// </summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct NullableFPVector2
    {
        /// <summary>Size of the struct in bytes.</summary>
        public const int SIZE = 24;

        /// <summary>
        ///     If 1, then <see cref="F:Herta.NullableFPVector2._value" /> is valid.
        /// </summary>
        [FieldOffset(0)] public long RawHasValue;

        /// <summary>The value.</summary>
        [FieldOffset(8)] public FPVector2 RawValue;

        /// <summary>
        ///     Returns <see langword="true" /> if this nullable has a value.
        /// </summary>
        public bool HasValue => this.RawHasValue == 1L;

        /// <summary>Returns current value.</summary>
        /// <exception cref="T:System.NullReferenceException">
        ///     If <see cref="P:Herta.NullableFPVector2.HasValue" />
        ///     is <see langword="false" />
        /// </exception>
        public FPVector2 Value
        {
            get
            {
                if (this.RawHasValue == 0L)
                    throw new NullReferenceException();
                return this.RawValue;
            }
        }

        /// <summary>
        ///     If <see cref="P:Herta.NullableFPVector2.HasValue" /> is <see langword="true" />, returns
        ///     <see cref="P:Herta.NullableFPVector2.Value" />. Otherwise returns <paramref name="v" />.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public FPVector2 ValueOrDefault(FPVector2 v) => this.RawHasValue != 1L ? v : this.Value;

        /// <summary>
        ///     Implicitly converts <paramref name="v" /> to NullableFPVector2.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator NullableFPVector2(FPVector2 v) => new NullableFPVector2()
        {
            RawValue = v,
            RawHasValue = 1
        };

        /// <summary>
        ///     Computes the hash code for the current NullableFPVector2 object.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => !this.HasValue ? 0 : XxHash.Hash32(Value);
    }
}