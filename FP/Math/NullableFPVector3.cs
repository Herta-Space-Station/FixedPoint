using System;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     A serializable equivalent of Nullable&lt;FPVector3&gt;.
    /// </summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct NullableFPVector3
    {
        /// <summary>Size of the struct in bytes.</summary>
        public const int SIZE = 32;

        /// <summary>
        ///     If 1, then <see cref="F:Herta.NullableFPVector3._value" /> is valid.
        /// </summary>
        [FieldOffset(0)] public long _hasValue;

        /// <summary>The value.</summary>
        [FieldOffset(8)] public FPVector3 _value;

        /// <summary>
        ///     Returns <see langword="true" /> if this nullable has a value.
        /// </summary>
        public bool HasValue => this._hasValue == 1L;

        /// <summary>Returns current value.</summary>
        /// <exception cref="T:System.NullReferenceException">
        ///     If <see cref="P:Herta.NullableFPVector3.HasValue" />
        ///     is <see langword="false" />
        /// </exception>
        public FPVector3 Value
        {
            get
            {
                if (this._hasValue == 0L)
                    throw new NullReferenceException();
                return this._value;
            }
        }

        /// <summary>
        ///     If <see cref="P:Herta.NullableFPVector3.HasValue" /> is <see langword="true" />, returns
        ///     <see cref="P:Herta.NullableFPVector3.Value" />. Otherwise returns <paramref name="v" />.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public FPVector3 ValueOrDefault(FPVector3 v) => this._hasValue != 1L ? v : this.Value;

        /// <summary>
        ///     Implicitly converts an FPVector3 to a NullableFPVector3.
        /// </summary>
        /// <param name="v">The FPVector3 to convert.</param>
        /// <returns>A NullableFPVector3 instance with the converted value.</returns>
        public static implicit operator NullableFPVector3(FPVector3 v) => new NullableFPVector3()
        {
            _value = v,
            _hasValue = 1
        };

        /// <summary>Gets the hash code of the NullableFPVector3 instance.</summary>
        /// <returns>The hash code of the NullableFPVector3.</returns>
        /// <remarks>
        ///     If <see cref="P:Herta.NullableFPVector3.HasValue" /> is <see langword="false" />, the hash code is
        ///     always 0.
        ///     If <see cref="P:Herta.NullableFPVector3.HasValue" /> is <see langword="true" />, the hash code is
        ///     calculated based on the value of <see cref="P:Herta.NullableFPVector3.Value" />.
        /// </remarks>
        public override int GetHashCode() => !this.HasValue ? 0 : XxHash.Hash32(Value);
    }
}