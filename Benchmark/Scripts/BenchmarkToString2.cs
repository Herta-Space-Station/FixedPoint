using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Herta;

#pragma warning disable CS8618

// ReSharper disable ALL

namespace Benchmark
{
    /*
     * | Method | Count   | Mean         | Error         | StdDev      |
     * |--------|---------|-------------:|--------------:|------------:|
     * | D1     | 1,000   |    48.17 us  |      6.084 us |    0.333 us |
     * | D2     | 1,000   |    67.75 us  |     62.459 us |    3.424 us |
     * | D1     | 10,000  |   490.93 us  |    419.268 us |   22.981 us |
     * | D2     | 10,000  |   706.45 us  |    128.611 us |    7.050 us |
     * | D1     | 100,000 | 4,891.30 us  |  3,244.859 us |  177.862 us |
     * | D2     | 100,000 | 8,564.04 us  | 11,861.660 us |  650.178 us |
     */

    [ShortRunJob]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public class BenchmarkToString2
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
            _ = b.ToString();
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP D2(FP a, in FP b)
        {
            _ = ToString(b);
            return a;
        }

        public static string ToString(FP v)
        {
            (int Sign, ulong Integer, uint Fraction) = GetDecimalParts(v);
            if (Fraction == 0U)
                return (v.RawValue >> 16).ToString();
            StringBuilder stringBuilder = new StringBuilder();
            if (Sign < 0)
                stringBuilder.Append('-');
            stringBuilder.Append(Integer);
            stringBuilder.Append('.');
            if (Fraction < 10000U)
            {
                stringBuilder.Append('0');
                if (Fraction < 1000U)
                {
                    stringBuilder.Append('0');
                    if (Fraction < 100U)
                    {
                        stringBuilder.Append('0');
                        if (Fraction < 10U)
                            stringBuilder.Append('0');
                    }
                }
            }

            if (Fraction % 10000U == 0U)
                Fraction /= 10000U;
            else if (Fraction % 1000U == 0U)
                Fraction /= 1000U;
            else if (Fraction % 100U == 0U)
                Fraction /= 100U;
            else if (Fraction % 10U == 0U)
                Fraction /= 10U;
            stringBuilder.Append(Fraction);

            return stringBuilder.ToString();
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