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
    [ShortRunJob]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public sealed class BenchmarkEmpty
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
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP D2(FP a, in FP b)
        {
            return a;
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