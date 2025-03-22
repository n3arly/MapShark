using BenchmarkDotNet.Running;

namespace MapShark.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<MappingBenchmark>();
        }
    }
}