using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Herta;

// ReSharper disable ALL

namespace Test
{
    internal sealed class Program
    {
        public static bool Compare<T1, T2>(in T1 arg1, in T2 arg2) where T1 : unmanaged where T2 : unmanaged => MemoryMarshal.CreateSpan(ref Unsafe.As<T1, byte>(ref Unsafe.AsRef(in arg1)), Unsafe.SizeOf<T1>()).SequenceEqual(MemoryMarshal.CreateSpan(ref Unsafe.As<T2, byte>(ref Unsafe.AsRef(in arg2)), Unsafe.SizeOf<T2>()));

        static void Main()
        {
            double val1 = 1.23456789;
            double val2 = -9.87654321;

            FP a = val1;
            a *= val2;
            Console.WriteLine(a.AsDouble);

            FP128HighPrecisionDivisor b = val1;
            b *= val2;
            Console.WriteLine(b.AsDouble);

            double c = val1;
            c *= val2;
            Console.WriteLine(c);

            FP128 d = val1;
            d *= val2;
            Console.WriteLine(d.AsDouble);
        }

        static void Test3()
        {
            for (uint num3 = 0; num3 < 65536; num3++)
            {
                uint num8;
                if (num3 == 0)
                {
                    num8 = 0;
                    goto lablel;
                }

                uint num5 = (uint)((ulong)(uint)(((int)num3 << 1) - 1) * 762939453125UL / 10000000000UL);
                uint num6 = (uint)((ulong)(uint)(((int)num3 << 1) + 1) * 762939453125UL / 10000000000UL);
                uint num7 = num5 + num6 >> 1;
                num8 = (int)(num5 / 10000U) != (int)(num6 / 10000U) ? ((int)(num5 / 100000U) != (int)(num6 / 100000U) ? ((int)(num5 / 1000000U) != (int)(num6 / 1000000U) ? (num7 + 500000U) / 1000000U * 10000U : (num7 + 50000U) / 100000U * 1000U) : (num7 + 5000U) / 10000U * 100U) : ((int)(num5 / 1000U) != (int)(num6 / 1000U) ? (num7 + 500U) / 1000U * 10U : (num7 + 50U) / 100U);

                lablel:

                uint a = FPLut.Fraction[(int)num3];

                if (!(num8 == a))
                    throw new Exception($"Mismatch at {num3}: expected {a}, got {num8}");
            }

            Console.WriteLine("All tests passed! All table entries match the original calculation.");
        }

        static void Test4()
        {
            while (true)
            {
                long value = Random.Shared.NextInt64(FP.MinValue.RawValue, FP.MaxValue.RawValue);
                FP a = Unsafe.As<long, FP>(ref value);
                (int Sign, ulong Integer, uint Fraction) b1 = GetDecimalParts(a);
                (int Sign, ulong Integer, uint Fraction) b2 = GetDecimalParts2(a);

                if (b1 != b2)
                    throw new Exception("error1");
            }
        }

        internal static (int Sign, ulong Integer, uint Fraction) GetDecimalParts2(FP v)
        {
            int num1;
            ulong num2;
            if (v.RawValue < 0L)
            {
                num1 = -1;
                num2 = (ulong)-v.RawValue;
            }
            else
            {
                num1 = 1;
                num2 = (ulong)v.RawValue;
            }

            uint num3 = (uint)(num2 & (ulong)ushort.MaxValue);
            ulong num4 = num2 >> 16;
            uint num5 = FPLut.Fraction[(int)num3];
            return (num1, num4, num5);
        }

        internal static (int Sign, ulong Integer, uint Fraction) GetDecimalParts(FP v)
        {
            int num1;
            ulong num2;
            if (v.RawValue < 0L)
            {
                num1 = -1;
                num2 = (ulong)-v.RawValue;
            }
            else
            {
                num1 = 1;
                num2 = (ulong)v.RawValue;
            }

            uint num3 = (uint)(num2 & (ulong)ushort.MaxValue);
            ulong num4 = num2 >> 16;
            if (num3 == 0U)
                return (num1, num4, 0U);
            uint num5 = (uint)((ulong)(uint)(((int)num3 << 1) - 1) * 762939453125UL / 10000000000UL);
            uint num6 = (uint)((ulong)(uint)(((int)num3 << 1) + 1) * 762939453125UL / 10000000000UL);
            uint num7 = num5 + num6 >> 1;
            uint num8 = (int)(num5 / 10000U) != (int)(num6 / 10000U) ? ((int)(num5 / 100000U) != (int)(num6 / 100000U) ? ((int)(num5 / 1000000U) != (int)(num6 / 1000000U) ? (num7 + 500000U) / 1000000U * 10000U : (num7 + 50000U) / 100000U * 1000U) : (num7 + 5000U) / 10000U * 100U) : ((int)(num5 / 1000U) != (int)(num6 / 1000U) ? (num7 + 500U) / 1000U * 10U : (num7 + 50U) / 100U);
            return (num1, num4, num8);
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