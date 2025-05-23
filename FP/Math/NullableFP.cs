using System;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace Thief
{
    /// <summary>A serializable equivalent of Nullable&lt;FP&gt;.</summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct NullableFP
    {
        /// <summary>Size of the struct in bytes.</summary>
        public const int SIZE = 16;

        /// <summary>
        ///     If 1, then <see cref="F:Thief.NullableFP._value" /> is valid.
        /// </summary>
        [FieldOffset(0)] public long _hasValue;

        /// <summary>The value.</summary>
        [FieldOffset(8)] public FP _value;

        /// <summary>
        ///     Returns <see langword="true" /> if this nullable has a value.
        /// </summary>
        public bool HasValue => this._hasValue == 1L;

        /// <summary>Returns current value.</summary>
        /// <exception cref="T:System.NullReferenceException">
        ///     If <see cref="P:Thief.NullableFP.HasValue" /> is
        ///     <see langword="false" />
        /// </exception>
        public FP Value
        {
            get
            {
                if (this._hasValue == 0L)
                    throw new NullReferenceException();
                return this._value;
            }
        }

        /// <summary>
        ///     If <see cref="P:Thief.NullableFP.HasValue" /> is <see langword="true" />, returns
        ///     <see cref="P:Thief.NullableFP.Value" />. Otherwise returns <paramref name="v" />.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public FP ValueOrDefault(FP v) => this._hasValue != 1L ? v : this.Value;

        /// <summary>
        ///     Converts <paramref name="v" /> to NullableFP.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator NullableFP(FP v) => new NullableFP()
        {
            _value = v,
            _hasValue = 1
        };

        /// <summary>
        ///     Computes the hash code for the current instance of the NullableFP struct.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => this._hasValue == 0L ? 0 : this._value.GetHashCode();
    }
}