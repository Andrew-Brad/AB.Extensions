using BenchmarkDotNet.Running;

namespace AB.Benchmarks.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<IntGetFirstDigitBenchmark>();
        }
    }
}
