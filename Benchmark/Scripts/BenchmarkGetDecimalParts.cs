using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Herta;

#pragma warning disable CS8618

// ReSharper disable ALL

namespace Benchmark
{
    /*
     * | Method | Count   | Mean       | Error      | StdDev     |
     * |--------|---------|-----------:|-----------:|-----------:|
     * | D1     | 1,000   |   1.100 us |  0.1557 us | 0.0085 us  |
     * | D2     | 1,000   |   2.037 us |  0.0893 us | 0.0049 us  |
     * | D1     | 10,000  |  10.930 us |  1.2352 us | 0.0677 us  |
     * | D2     | 10,000  |  20.220 us |  2.7212 us | 0.1492 us  |
     * | D1     | 100,000 | 111.190 us | 47.3295 us | 2.5943 us  |
     * | D2     | 100,000 | 203.701 us | 19.1931 us | 1.0520 us  |
     */

    [ShortRunJob]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public class BenchmarkGetDecimalParts
    {
        [Params(1000, 10000, 100000)] public int Count { get; set; }

        public List<FP> Input;

        public FP FP1;
        public FP FP2;

        [GlobalSetup]
        public void Init()
        {
            Input = new List<FP>(Count);
            for (int i = 0; i < Count; i++)
            {
                Input.Add(Random.Shared.NextInt64());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP D1(FP a, in FP b)
        {
            _ = GetDecimalParts2(a);
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP D2(FP a, in FP b)
        {
            _ = GetDecimalParts(a);
            return a;
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

            uint num3 = (uint)(num2 & 0xFFFF);
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

        [BenchmarkCategory("D")]
        [Benchmark]
        public void D1()
        {
            foreach (FP i in Input)
            {
                FP1 = D1(FP1, i);
            }
        }

        [BenchmarkCategory("D")]
        [Benchmark]
        public void D2()
        {
            foreach (FP i in Input)
            {
                FP2 = D2(FP2, i);
            }
        }
    }
}