using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Diagnosers;

var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core60).WithId("net6.0"))
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80).WithId("net8.0"))
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core90).WithId("net9.0"))
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core10_0).WithId("net10.0"))
    .AddDiagnoser(MemoryDiagnoser.Default);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
