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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Hash64(ReadOnlySpan<byte> buffer, ulong seed = 0)
        {
            ref byte local1 = ref MemoryMarshal.GetReference(buffer);
            int length = buffer.Length;
            ulong num1;
            if (buffer.Length >= 32)
            {
                ulong num2 = seed + 6983438078262162902UL;
                ulong num3 = seed + 14029467366897019727UL;
                ulong num4 = seed;
                ulong num5 = seed - 11400714785074694791UL;
                for (; length >= 32; length -= 32)
                {
                    ref byte local2 = ref Unsafe.AddByteOffset(ref local1, (IntPtr)(buffer.Length - length));
                    ulong num6 = num2 + Unsafe.ReadUnaligned<ulong>(ref local2) * 14029467366897019727UL;
                    num2 = (ulong)((((long)num6 << 31) | (long)(num6 >> 33)) * -7046029288634856825L);
                    ulong num7 = num3 + Unsafe.ReadUnaligned<ulong>(ref Unsafe.AddByteOffset(ref local2, new IntPtr(8U))) * 14029467366897019727UL;
                    num3 = (ulong)((((long)num7 << 31) | (long)(num7 >> 33)) * -7046029288634856825L);
                    ulong num8 = num4 + Unsafe.ReadUnaligned<ulong>(ref Unsafe.AddByteOffset(ref local2, new IntPtr(16U))) * 14029467366897019727UL;
                    num4 = (ulong)((((long)num8 << 31) | (long)(num8 >> 33)) * -7046029288634856825L);
                    ulong num9 = num5 + Unsafe.ReadUnaligned<ulong>(ref Unsafe.AddByteOffset(ref local2, new IntPtr(24U))) * 14029467366897019727UL;
                    num5 = (ulong)((((long)num9 << 31) | (long)(num9 >> 33)) * -7046029288634856825L);
                }

                long num10 = (((long)num2 << 1) | (long)(num2 >> 63)) + (((long)num3 << 7) | (long)(num3 >> 57)) + (((long)num4 << 12) | (long)(num4 >> 52)) + (((long)num5 << 18) | (long)(num5 >> 46));
                ulong num11 = num2 * 14029467366897019727UL;
                long num12 = (((long)num11 << 31) | (long)(num11 >> 33)) * -7046029288634856825L;
                long num13 = (num10 ^ num12) * -7046029288634856825L + -8796714831421723037L;
                ulong num14 = num3 * 14029467366897019727UL;
                long num15 = (((long)num14 << 31) | (long)(num14 >> 33)) * -7046029288634856825L;
                long num16 = (num13 ^ num15) * -7046029288634856825L + -8796714831421723037L;
                ulong num17 = num4 * 14029467366897019727UL;
                long num18 = (((long)num17 << 31) | (long)(num17 >> 33)) * -7046029288634856825L;
                long num19 = (num16 ^ num18) * -7046029288634856825L + -8796714831421723037L;
                ulong num20 = num5 * 14029467366897019727UL;
                long num21 = (((long)num20 << 31) | (long)(num20 >> 33)) * -7046029288634856825L;
                num1 = (ulong)((num19 ^ num21) * -7046029288634856825L + -8796714831421723037L);
            }
            else
                num1 = seed + 2870177450012600261UL;

            ulong num22 = num1 + (ulong)buffer.Length;
            for (; length >= 8; length -= 8)
            {
                ulong num23 = Unsafe.ReadUnaligned<ulong>(ref Unsafe.AddByteOffset(ref local1, (IntPtr)(buffer.Length - length))) * 14029467366897019727UL;
                ulong num24 = (ulong)((((long)num23 << 31) | (long)(num23 >> 33)) * -7046029288634856825L);
                ulong num25 = num22 ^ num24;
                num22 = (ulong)((((long)num25 << 27) | (long)(num25 >> 37)) * -7046029288634856825L + -8796714831421723037L);
            }

            if (length >= 4)
            {
                ulong num26 = Unsafe.ReadUnaligned<uint>(ref Unsafe.AddByteOffset(ref local1, (IntPtr)(buffer.Length - length)));
                ulong num27 = num22 ^ (num26 * 11400714785074694791UL);
                num22 = (ulong)((((long)num27 << 23) | (long)(num27 >> 41)) * -4417276706812531889L + 1609587929392839161L);
                length -= 4;
            }

            for (int byteOffset = 0; byteOffset < length; ++byteOffset)
            {
                ulong num28 = Unsafe.AddByteOffset(ref Unsafe.AddByteOffset(ref local1, (IntPtr)(buffer.Length - length)), (IntPtr)byteOffset);
                ulong num29 = num22 ^ (num28 * 2870177450012600261UL);
                num22 = (ulong)((((long)num29 << 11) | (long)(num29 >> 53)) * -7046029288634856825L);
            }

#if NET7_0_OR_GREATER
            long num30 = (long)num22;
            long num31 = (num30 ^ (num30 >>> 33)) * -4417276706812531889L;
            long num32 = (num31 ^ (num31 >>> 29)) * 1609587929392839161L;
            return num32 ^ (num32 >>> 32);
#else
            long num30 = (long)num22;
            long num31 = (num30 ^ (long)((ulong)num30 >> 33)) * -4417276706812531889L;
            long num32 = (num31 ^ (long)((ulong)num31 >> 29)) * 1609587929392839161L;
            return num32 ^ (long)((ulong)num32 >> 32);
#endif
        }
    }
}