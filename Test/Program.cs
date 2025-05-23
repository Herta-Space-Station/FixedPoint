using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Thief;

namespace Test
{
    internal sealed class Program
    {
        public static bool Compare<T1, T2>(in T1 arg1, in T2 arg2) where T1 : unmanaged where T2 : unmanaged
        {
            return MemoryMarshal.CreateSpan(ref Unsafe.As<T1, byte>(ref Unsafe.AsRef(in arg1)), Unsafe.SizeOf<T1>()).SequenceEqual(MemoryMarshal.CreateSpan(ref Unsafe.As<T2, byte>(ref Unsafe.AsRef(in arg2)), Unsafe.SizeOf<T2>()));
        }

        private static void Main()
        {
            FP a = 0.01;
            a += 0.1;
            string str = a.ToString();
            Console.WriteLine(str);
            FP b = FP.Parse("0.11");
            Console.WriteLine(b.AsDouble);
        }
    }
}