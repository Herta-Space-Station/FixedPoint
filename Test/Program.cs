using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Herta;

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
            Console.WriteLine(FP.FromDouble_SAFE(100.0).AsDouble);
        }

        private static void Test2()
        {
            for (int i = 0; i < 1024 * 1024; ++i)
            {
                FP a = FPRandom.Shared.Next(0, 1);
                if (a < 0 || a >= 1)
                    throw new Exception("error");

                Console.WriteLine(a);
            }

            Console.WriteLine("done");
        }

        private static void Test1()
        {
            MemoryStream memoryStream = new MemoryStream();
            GZipStream stream = new GZipStream(memoryStream, CompressionLevel.SmallestSize);
            stream.Write(MemoryMarshal.Cast<long, byte>(FPLut.Acos));
            Console.WriteLine(memoryStream.Length);
            Console.WriteLine(FPLut.Acos.Length);
            Console.WriteLine(MemoryMarshal.Cast<long, byte>(FPLut.Acos).Length);
        }
    }
}