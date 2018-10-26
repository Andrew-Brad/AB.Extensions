using System;
using BenchmarkDotNet.Attributes;

namespace AB.Benchmarks.ConsoleApp
{
    [CoreJob]
    public class IntGetFirstDigitBenchmark
    {
        [Params(-1,0,1, 987654321,-10,int.MaxValue, 1000000000,int.MinValue,000000)]
        public int InputInt { get; set; }

        public IntGetFirstDigitBenchmark()
        {
        }

        [Benchmark]
        public int ToString_Index_0()
        {
            int given = InputInt;
            if (given == Int32.MinValue) return 2;
            int firstDigit = (int)(Math.Abs(given).ToString()[0]) - 48;
            return firstDigit;
        }

        [Benchmark]
        public int WhileLoop()
        {
            int value = InputInt;
            if (value == Int32.MinValue) return 2;
            value = Math.Abs(value);
            while (value >= 10)
            {
                value = (value - (value % 10)) / 10;
            }
            return value;
        }

        [Benchmark]
        public int Nested_Operators()
        {
            int value = InputInt;
            if (value < 0 && (value = -value) < 0) return 2;
            return (value < 100) ? (value < 1) ? 0 : (value < 10)
                    ? value : value / 10 : (value < 1000000) ? (value < 10000)
                    ? (value < 1000) ? value / 100 : value / 1000 : (value < 100000)
                    ? value / 10000 : value / 100000 : (value < 100000000)
                    ? (value < 10000000) ? value / 1000000 : value / 10000000
                    : (value < 1000000000) ? value / 100000000 : value / 1000000000;
        }

        [Benchmark]
        public int If_Checks_Neat_Readable()
        {
            int value = InputInt;
            if (value < 0 && (value = -value) < 0) return 2;
            if (value >= 100000000) value /= 100000000;
            if (value >= 10000) value /= 10000;
            if (value >= 100) value /= 100;
            if (value >= 10) value /= 10;
            return value;
        }
    }
}
