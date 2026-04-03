using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(HwoodiwissHelper.Benchmarks.HwoodiwissHelperBenchmarks).Assembly).Run(args);
