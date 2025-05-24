using System.Runtime.CompilerServices;
#if NET5_0_OR_GREATER
using System;
#endif

// ReSharper disable ALL

namespace Herta
{
    internal static class MathHelpers
    {
        /// <summary>Produces the full product of two unsigned 64-bit numbers.</summary>
        /// <param name="a">The first number to multiply.</param>
        /// <param name="b">The second number to multiply.</param>
        /// <param name="low">The low 64-bit of the product of the specified numbers.</param>
        /// <returns>The high 64-bit of the product of the specified numbers.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong BigMul(ulong a, ulong b, out ulong low)
        {
#if NET5_0_OR_GREATER
            return Math.BigMul(a, b, out low);
#else
            uint al = (uint)a;
            uint ah = (uint)(a >> 32);
            uint bl = (uint)b;
            uint bh = (uint)(b >> 32);
            ulong mull = (ulong)al * bl;
            ulong t = (ulong)ah * bl + (mull >> 32);
            ulong tl = (ulong)al * bh + (uint)t;
            low = (tl << 32) | (uint)mull;
            return (ulong)ah * bh + (t >> 32) + (tl >> 32);
#endif
        }
    }
}