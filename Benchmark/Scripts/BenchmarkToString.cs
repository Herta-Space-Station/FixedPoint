using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Herta;

#pragma warning disable CS8618

// ReSharper disable ALL

namespace Benchmark
{
    /*
     * | Method | Count   | Mean         | Error        | StdDev      |
     * |--------|---------|--------------|--------------|-------------|
     * | D1     | 1,000   | 39.74 us     | 18.54 us     | 1.016 us    |
     * | D2     | 1,000   | 60.30 us     | 19.62 us     | 1.075 us    |
     * | D1     | 10,000  | 376.13 us    | 136.03 us    | 7.456 us    |
     * | D2     | 10,000  | 634.61 us    | 128.92 us    | 7.066 us    |
     * | D1     | 100,000 | 3,652.78 us  | 313.67 us    | 17.194 us   |
     * | D2     | 100,000 | 6,385.66 us  | 2,219.42 us  | 121.654 us  |
     */

    [ShortRunJob]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public class BenchmarkToString
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
            _ = b.ToStringInternal();
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP D2(FP a, in FP b)
        {
            _ = ToStringInternal(b);
            return a;
        }

        public static string ToStringInternal(FP fp)
        {
            long num = Math.Abs(fp.RawValue);
            string str = string.Format("{0}.{1}", (object)(num >> 16), (object)(num % 65536L).ToString((IFormatProvider)CultureInfo.InvariantCulture).PadLeft(5, '0'));
            return fp.RawValue < 0L ? "-" + str : str;
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