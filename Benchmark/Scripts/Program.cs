using BenchmarkDotNet.Running;

namespace Benchmark
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkEmpty>();
        }
    }
}