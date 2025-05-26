using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CA2208
#pragma warning disable CA2231
#pragma warning disable CS8632

// ReSharper disable ALL

namespace Herta
{
    /// <summary>
    ///     Native temp buffer
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct NativeTempBuffer<T> : IDisposable where T : struct
    {
        /// <summary>
        ///     Array
        /// </summary>
        private readonly T[] _array;

        /// <summary>
        ///     Length
        /// </summary>
        private readonly int _length;

        /// <summary>
        ///     Is created
        /// </summary>
        public bool IsCreated => _array != null;

        /// <summary>
        ///     Length
        /// </summary>
        public int Length => _length;

        /// <summary>
        ///     Structure
        /// </summary>
        /// <param name="length">Length</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeTempBuffer(int length)
        {
            _array = ArrayPool<T>.Shared.Rent(length);
            _length = length;
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => ArrayPool<T>.Shared.Return(_array);

        /// <summary>
        ///     As span
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => _array.AsSpan(0, _length);

        /// <summary>
        ///     As readOnly span
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsReadOnlySpan() => _array.AsSpan(0, _length);

        /// <summary>
        ///     As memory
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> AsMemory() => new(_array, 0, _length);

        /// <summary>
        ///     As readOnly memory
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<T> AsReadOnlyMemory() => new(_array, 0, _length);

        /// <summary>
        ///     Equals
        /// </summary>
        /// <returns>Equals</returns>
        public override bool Equals(object? obj) => throw new NotSupportedException("CannotCallEquals");

        /// <summary>
        ///     Get hashCode
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode() => throw new NotSupportedException("CannotCallGetHashCode");

        /// <summary>
        ///     To string
        /// </summary>
        /// <returns>String</returns>
        public override string ToString() => $"NativeTempBuffer<{typeof(T).Name}>[{_length}]";

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(ReadOnlySpan<T> buffer)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(buffer.Length);
            buffer.CopyTo(temp.AsSpan());
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(1);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(2);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(3);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(4);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(5);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(6);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5, in T arg6)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(7);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            buffer[6] = arg6;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5, in T arg6, in T arg7)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(8);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            buffer[6] = arg6;
            buffer[7] = arg7;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5, in T arg6, in T arg7, in T arg8)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(9);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            buffer[6] = arg6;
            buffer[7] = arg7;
            buffer[8] = arg8;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5, in T arg6, in T arg7, in T arg8, in T arg9)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(10);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            buffer[6] = arg6;
            buffer[7] = arg7;
            buffer[8] = arg8;
            buffer[9] = arg9;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5, in T arg6, in T arg7, in T arg8, in T arg9, in T arg10)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(11);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            buffer[6] = arg6;
            buffer[7] = arg7;
            buffer[8] = arg8;
            buffer[9] = arg9;
            buffer[10] = arg10;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5, in T arg6, in T arg7, in T arg8, in T arg9, in T arg10, in T arg11)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(12);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            buffer[6] = arg6;
            buffer[7] = arg7;
            buffer[8] = arg8;
            buffer[9] = arg9;
            buffer[10] = arg10;
            buffer[11] = arg11;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5, in T arg6, in T arg7, in T arg8, in T arg9, in T arg10, in T arg11, in T arg12)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(13);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            buffer[6] = arg6;
            buffer[7] = arg7;
            buffer[8] = arg8;
            buffer[9] = arg9;
            buffer[10] = arg10;
            buffer[11] = arg11;
            buffer[12] = arg12;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5, in T arg6, in T arg7, in T arg8, in T arg9, in T arg10, in T arg11, in T arg12, in T arg13)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(14);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            buffer[6] = arg6;
            buffer[7] = arg7;
            buffer[8] = arg8;
            buffer[9] = arg9;
            buffer[10] = arg10;
            buffer[11] = arg11;
            buffer[12] = arg12;
            buffer[13] = arg13;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5, in T arg6, in T arg7, in T arg8, in T arg9, in T arg10, in T arg11, in T arg12, in T arg13, in T arg14)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(15);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            buffer[6] = arg6;
            buffer[7] = arg7;
            buffer[8] = arg8;
            buffer[9] = arg9;
            buffer[10] = arg10;
            buffer[11] = arg11;
            buffer[12] = arg12;
            buffer[13] = arg13;
            buffer[14] = arg14;
            return temp;
        }

        /// <summary>
        ///     Create
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeTempBuffer<T> Create(in T arg0, in T arg1, in T arg2, in T arg3, in T arg4, in T arg5, in T arg6, in T arg7, in T arg8, in T arg9, in T arg10, in T arg11, in T arg12, in T arg13, in T arg14, in T arg15)
        {
            NativeTempBuffer<T> temp = new NativeTempBuffer<T>(16);
            Span<T> buffer = temp.AsSpan();
            buffer[0] = arg0;
            buffer[1] = arg1;
            buffer[2] = arg2;
            buffer[3] = arg3;
            buffer[4] = arg4;
            buffer[5] = arg5;
            buffer[6] = arg6;
            buffer[7] = arg7;
            buffer[8] = arg8;
            buffer[9] = arg9;
            buffer[10] = arg10;
            buffer[11] = arg11;
            buffer[12] = arg12;
            buffer[13] = arg13;
            buffer[14] = arg14;
            buffer[15] = arg15;
            return temp;
        }

        /// <summary>
        ///     Empty
        /// </summary>
#pragma warning disable MY0001
        public static NativeTempBuffer<T> Empty => new();
#pragma warning restore MY0001
    }
}