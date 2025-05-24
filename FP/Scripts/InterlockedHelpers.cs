using System.Runtime.CompilerServices;
using System.Threading;

namespace Herta
{
    internal static class InterlockedHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Exchange(ref uint location1, uint value)
        {
#if NET5_0_OR_GREATER
            return Interlocked.Exchange(ref location1, value);
#else
            return (uint)Interlocked.Exchange(ref Unsafe.As<uint, int>(ref location1), (int)value);
#endif
        }
    }
}