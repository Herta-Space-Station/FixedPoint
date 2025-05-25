using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NativeCollections;

// ReSharper disable ALL

namespace Herta
{
    /// <summary>A serializable equivalent of Nullable&lt;FP&gt;.</summary>
    /// \ingroup MathAPI
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct NullableNonNegativeFP
    {
        /// <summary>Size of the struct in bytes.</summary>
        public const int SIZE = 8;

        /// <summary>The value.</summary>
        [FieldOffset(0)] public ulong _value;

        private const ulong HasValueBit = 9223372036854775808;
        private const ulong ValueMask = 9223372036854775807;

        /// <summary>
        ///     Returns <see langword="true" /> if this nullable has a value.
        /// </summary>
        public bool HasValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (this._value & 9223372036854775808UL) > 0UL;
        }

        /// <summary>Returns current value.</summary>
        /// <exception cref="T:System.NullReferenceException">
        ///     If
        ///     <see cref="P:Herta.NullableNonNegativeFP.HasValue" /> is <see langword="false" />
        /// </exception>
        public FP Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!this.HasValue)
                    throw new NullReferenceException();
                FP fp;
                fp.RawValue = (long)this._value & long.MaxValue;
                return fp;
            }
        }

        /// <summary>
        ///     If <see cref="P:Herta.NullableNonNegativeFP.HasValue" /> is <see langword="true" />, returns
        ///     <see cref="P:Herta.NullableNonNegativeFP.Value" />. Otherwise returns zero.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FP ValueOrDefault()
        {
            FP fp;
            fp.RawValue = (long)this._value & long.MaxValue;
            return fp;
        }

        /// <summary>
        ///     Implicitly converts the specified value of type FP to NullableNonNegativeFP.
        /// </summary>
        /// <param name="v">The value to be converted.</param>
        /// <returns>A NullableNonNegativeFP representing the converted value.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when the value is less than zero.</exception>
        public static implicit operator NullableNonNegativeFP(FP v)
        {
            if (v < 0)
            {
                NativeString builder = new NativeString(stackalloc char[128]);
                builder.Append("Non negative values only allowed: ");
                builder.AppendFormattable(v);
                throw new ArgumentOutOfRangeException(builder.ToString());
            }

            NullableNonNegativeFP nullableNonNegativeFp;
            nullableNonNegativeFp._value = (ulong)(v.RawValue | long.MinValue);
            return nullableNonNegativeFp;
        }

        /// <summary>Returns a string representation of the current value.</summary>
        /// <returns></returns>
        public override string ToString() => !this.HasValue ? "NULL" : this.Value.ToString();

        /// <summary>
        ///     Computes the hash code for the current instance of the FP struct.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => !this.HasValue ? 0 : XxHash.Hash32(Value);
    }
}