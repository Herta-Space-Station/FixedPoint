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
    public class Benchmark2
    {
        public List<FP> Input1;
        public List<FP128> Input2;
        public List<TrueSync.FP> Input3;

        public FP FP1;
        public FP128 FP2;
        public TrueSync.FP FP3;

        [Params(1000, 10000, 100000)] public int Count { get; set; }

        [GlobalSetup]
        public void Init()
        {
            Input1 = new(Count);
            Input2 = new(Count);
            Input3 = new(Count);
            for (int i = 0; i < Count; i++)
            {
                double a = Random.Shared.NextDouble();
                Input1.Add(a);
                Input2.Add(a);
                Input3.Add(a);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP D1(FP a, in FP b)
        {
            return a * b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP128 D2(FP128 a, in FP128 b)
        {
            return a * b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TrueSync.FP D3(TrueSync.FP a, in TrueSync.FP b)
        {
            return a * b;
        }

        [BenchmarkCategory("D")]
        [Benchmark]
        public void D1()
        {
            foreach (FP i in Input1)
            {
                FP1 = D1(FP1, i);
            }
        }

        [BenchmarkCategory("D")]
        [Benchmark]
        public void D2()
        {
            foreach (FP128 i in Input2)
            {
                FP2 = D2(FP2, i);
            }
        }

        [BenchmarkCategory("D")]
        [Benchmark]
        public void D3()
        {
            foreach (TrueSync.FP i in Input3)
            {
                FP3 = D3(FP3, i);
            }
        }
    }
}