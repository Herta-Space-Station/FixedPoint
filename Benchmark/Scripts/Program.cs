using BenchmarkDotNet.Running;

// ReSharper disable ALL

namespace Benchmark
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkToString2>();
        }
    }
}