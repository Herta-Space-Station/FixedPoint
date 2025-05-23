using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CA2208
#pragma warning disable CS8632

// ReSharper disable ALL

namespace Thief
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct SplitAnyRange<T> where T : IEquatable<T>
    {
        private readonly ReadOnlySpan<T> _buffer;
        private readonly ReadOnlySpan<T> _separator;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SplitAnyRange(ReadOnlySpan<T> buffer, in T separator)
        {
            if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(buffer)) || buffer.Length == 0)
                throw new ArgumentNullException(nameof(buffer));
            if (!typeof(T).IsValueType && separator == null)
                throw new ArgumentNullException(nameof(separator));
            _buffer = buffer;
            _separator = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in separator), 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SplitAnyRange(ReadOnlySpan<T> buffer, ReadOnlySpan<T> separator)
        {
            if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(buffer)) || buffer.Length == 0)
                throw new ArgumentNullException(nameof(buffer));
            if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(separator)) || separator.Length == 0)
                throw new ArgumentNullException(nameof(separator));
            _buffer = buffer;
            _separator = separator;
        }

        public Enumerator GetEnumerator() => new(_buffer, _separator);

        [StructLayout(LayoutKind.Sequential)]
        public ref struct Enumerator
        {
            private Range _current;
            private int _next;
            private readonly ReadOnlySpan<T> _buffer;
            private readonly ReadOnlySpan<T> _separator;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ReadOnlySpan<T> buffer, ReadOnlySpan<T> separator)
            {
                _current = default;
                _next = 0;
                _buffer = buffer;
                _separator = separator;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                ReadOnlySpan<T> buffer = _buffer.Slice(_next);
                int index = _separator.Length == 1 ? buffer.IndexOf(_separator[0]) : buffer.IndexOfAny(_separator);
                if (index < 0)
                {
                    if (buffer.Length > 0)
                    {
                        _current = new Range(_next, _next + buffer.Length);
                        _next = _buffer.Length;
                        return true;
                    }

                    return false;
                }

                _current = new Range(_next, _next + index);
                _next += index + 1;
                return true;
            }

            public Range Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _current;
            }
        }
    }
}