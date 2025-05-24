using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// ReSharper disable ALL

namespace Herta
{
    public static class XxHash
    {
        public static readonly uint XXHASH_32_SEED;

        static XxHash()
        {
            Span<byte> data = stackalloc byte[4];
            RandomNumberGenerator.Fill(data);
            XXHASH_32_SEED = Unsafe.ReadUnaligned<byte>(ref MemoryMarshal.GetReference(data));
        }

        public static uint Exchange(uint seed)
        {
            ref uint location1 = ref Unsafe.AsRef(in XxHash.XXHASH_32_SEED);
            return InterlockedHelpers.Exchange(ref location1, seed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash32<T>(in T obj) where T : unmanaged => Hash32(obj, XXHASH_32_SEED);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash32<T>(ReadOnlySpan<T> buffer) where T : unmanaged => Hash32(buffer, XXHASH_32_SEED);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash32(ReadOnlySpan<byte> buffer) => Hash32(buffer, XXHASH_32_SEED);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash32<T>(in T obj, uint seed) where T : unmanaged => Hash32(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in obj)), Unsafe.SizeOf<T>()), seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash32<T>(ReadOnlySpan<T> buffer, uint seed) where T : unmanaged => Hash32(MemoryMarshal.Cast<T, byte>(buffer), seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash32(ReadOnlySpan<byte> buffer, uint seed)
        {
            int length = buffer.Length;
            ref byte local1 = ref MemoryMarshal.GetReference(buffer);
            uint num1;
            if (buffer.Length >= 16)
            {
                uint num2 = seed + 606290984U;
                uint num3 = seed + 2246822519U;
                uint num4 = seed;
                uint num5 = seed - 2654435761U;
                for (; length >= 16; length -= 16)
                {
                    const nint elementOffset1 = 4;
                    const nint elementOffset2 = 8;
                    const nint elementOffset3 = 12;
                    nint byteOffset = buffer.Length - length;
                    ref byte local2 = ref Unsafe.AddByteOffset(ref local1, byteOffset);
                    uint num6 = num2 + Unsafe.ReadUnaligned<uint>(ref local2) * 2246822519U;
                    num2 = (uint)((((int)num6 << 13) | (int)(num6 >> 19)) * -1640531535);
                    uint num7 = num3 + Unsafe.ReadUnaligned<uint>(ref Unsafe.AddByteOffset(ref local2, elementOffset1)) * 2246822519U;
                    num3 = (uint)((((int)num7 << 13) | (int)(num7 >> 19)) * -1640531535);
                    uint num8 = num4 + Unsafe.ReadUnaligned<uint>(ref Unsafe.AddByteOffset(ref local2, elementOffset2)) * 2246822519U;
                    num4 = (uint)((((int)num8 << 13) | (int)(num8 >> 19)) * -1640531535);
                    uint num9 = num5 + Unsafe.ReadUnaligned<uint>(ref Unsafe.AddByteOffset(ref local2, elementOffset3)) * 2246822519U;
                    num5 = (uint)((((int)num9 << 13) | (int)(num9 >> 19)) * -1640531535);
                }

                num1 = (uint)((((int)num2 << 1) | (int)(num2 >> 31)) + (((int)num3 << 7) | (int)(num3 >> 25)) + (((int)num4 << 12) | (int)(num4 >> 20)) + (((int)num5 << 18) | (int)(num5 >> 14)) + buffer.Length);
            }
            else
                num1 = (uint)((int)seed + 374761393 + buffer.Length);

            for (; length >= 4; length -= 4)
            {
                nint byteOffset = buffer.Length - length;
                uint num10 = Unsafe.ReadUnaligned<uint>(ref Unsafe.AddByteOffset(ref local1, byteOffset));
                uint num11 = num1 + num10 * 3266489917U;
                num1 = (uint)((((int)num11 << 17) | (int)(num11 >> 15)) * 668265263);
            }

            nint byteOffset1 = buffer.Length - length;
            ref byte local3 = ref Unsafe.AddByteOffset(ref local1, byteOffset1);
            for (int index = 0; index < length; ++index)
            {
                nint byteOffset2 = index;
                uint num12 = Unsafe.AddByteOffset(ref local3, byteOffset2);
                uint num13 = num1 + num12 * 374761393U;
                num1 = (uint)((((int)num13 << 11) | (int)(num13 >> 21)) * -1640531535);
            }

#if NET7_0_OR_GREATER
            int num14 = ((int)num1 ^ (int)(num1 >> 15)) * -2048144777;
            int num15 = (num14 ^ (num14 >>> 13)) * -1028477379;
            return num15 ^ (num15 >>> 16);
#else
            int num14 = ((int)num1 ^ (int)(num1 >> 15)) * -2048144777;
            int num15 = (num14 ^ (int)((uint)num14 >> 13)) * -1028477379;
            return num15 ^ (int)((uint)num15 >> 16);
#endif
        }
    }
}