using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// ReSharper disable ALL

namespace Thief
{
    /// <summary>Represents a deterministic pseudo-random number generator with high performance.</summary>
    /// <remarks>Uses xoshiro256** algorithm for random number generation.</remarks>
    /// \ingroup MathAPI
    [StructLayout(LayoutKind.Sequential)]
    public struct FPRandom
    {
        // Internal state variables for the random number generator
        private ulong _s0;
        private ulong _s1;
        private ulong _s2;
        private ulong _s3;

        /// <summary>Gets a value indicating whether this instance has been initialized.</summary>
        /// <returns>True if initialized, false otherwise.</returns>
        public bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ((long)_s0 | (long)_s1 | (long)_s2 | (long)_s3) != 0L;
        }

        [ThreadStatic] private static FPRandom _shared;

        /// <summary>Gets a thread-safe shared instance of the random number generator.</summary>
        /// <remarks>Automatically initializes if not created.</remarks>
        public static ref FPRandom Shared
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref FPRandom shared = ref _shared;
                if (!shared.IsCreated)
                    shared.Initialize();
                return ref shared;
            }
        }

        /// <summary>Initializes the random number generator with cryptographically secure seed values.</summary>
        /// <remarks>Uses <see cref="RandomNumberGenerator" /> to fill the state.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Initialize()
        {
            Span<byte> data = MemoryMarshal.CreateSpan(ref Unsafe.As<ulong, byte>(ref _s0), 32);
            do
            {
                RandomNumberGenerator.Fill(data);
            } while (((long)_s0 | (long)_s1 | (long)_s2 | (long)_s3) == 0L);
        }

        /// <summary>Generates a random <see cref="FP" /> value between 0 and 1.</summary>
        /// <returns>A random fixed-point number.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FP Next()
        {
            ulong value = Next64() & 0xFFFF;
            return Unsafe.As<ulong, FP>(ref value);
        }

        /// <summary>Generates a random <see cref="FP" /> value within the specified range.</summary>
        /// <param name="minValue">The inclusive lower bound.</param>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random fixed-point number in the specified range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FP Next(FP minValue, FP maxValue)
        {
            if (minValue > maxValue)
                (minValue, maxValue) = (maxValue, minValue);

            ulong value = NextUInt64(Unsafe.As<FP, ulong>(ref Unsafe.AsRef(in minValue)), Unsafe.As<FP, ulong>(ref Unsafe.AsRef(in maxValue)));
            return Unsafe.As<ulong, FP>(ref value);
        }

        /// <summary>Generates a random 64-bit unsigned integer using xoshiro256** algorithm.</summary>
        /// <returns>A random 64-bit unsigned integer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong Next64()
        {
            long s0 = (long)_s0;
            ulong s1 = _s1;
            long s2 = (long)_s2;
            ulong s3 = _s3;
            ulong num1 = s1 * 5UL;
            ulong num2 = ((num1 << 7) | (num1 >> 57)) * 9UL;
            ulong num3 = s1 << 17;
            long num4 = s0;
            ulong num5 = (ulong)(s2 ^ num4);
            ulong num6 = s3 ^ s1;
            ulong num7 = s1 ^ num5;
            ulong num8 = (ulong)s0 ^ num6;
            ulong num9 = num5 ^ num3;
            ulong num10 = (num6 << 45) | (num6 >> 19);
            _s0 = num8;
            _s1 = num7;
            _s2 = num9;
            _s3 = num10;
            return num2;
        }

        /// <summary>Generates a random 64-bit unsigned integer within the specified range.</summary>
        /// <param name="minValue">The inclusive lower bound.</param>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random 64-bit unsigned integer in the specified range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong NextUInt64(ulong minValue, ulong maxValue)
        {
            ulong a = maxValue - minValue;
            ulong low;
            ulong num1 = MathHelpers.BigMul(a, Next64(), out low);
            if (low < a)
            {
                ulong num2 = unchecked(0UL - a) % a;
                while (low < num2)
                    num1 = MathHelpers.BigMul(a, Next64(), out low);
            }

            return num1 + minValue;
        }
    }
}