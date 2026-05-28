using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using DMathBigInt = Deveel.Math.BigInteger;
using SysBigInt = System.Numerics.BigInteger;

namespace Deveel.Math.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class BigIntegerCreationBench
{
    private byte[] _smallBytes = null!;
    private byte[] _largeBytes = null!;
    private string _smallString = null!;
    private string _largeString = null!;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        _smallBytes = new byte[32];
        rnd.NextBytes(_smallBytes);
        _smallBytes[31] = 0; // ensure positive
        _smallString = "12345678901234567890";

        _largeBytes = new byte[256];
        rnd.NextBytes(_largeBytes);
        _largeBytes[255] = 0;
        _largeString = new string('9', 500);
    }

    [Benchmark(Description = "DMath BigInteger FromInt64")]
    public DMathBigInt DMath_FromInt64() => DMathBigInt.FromInt64(123456789012345L);

    [Benchmark(Description = "System BigInteger FromInt64")]
    public SysBigInt Sys_FromInt64() => new SysBigInt(123456789012345L);

    [Benchmark(Description = "DMath BigInteger FromBytes (small)")]
    public DMathBigInt DMath_FromBytes_Small() => new DMathBigInt((byte[])_smallBytes.Clone());

    [Benchmark(Description = "System BigInteger FromBytes (small)")]
    public SysBigInt Sys_FromBytes_Small() => new SysBigInt(_smallBytes);

    [Benchmark(Description = "DMath BigInteger FromBytes (large)")]
    public DMathBigInt DMath_FromBytes_Large() => new DMathBigInt((byte[])_largeBytes.Clone());

    [Benchmark(Description = "System BigInteger FromBytes (large)")]
    public SysBigInt Sys_FromBytes_Large() => new SysBigInt(_largeBytes);

    [Benchmark(Description = "DMath BigInteger Parse (small)")]
    public DMathBigInt DMath_Parse_Small() => DMathBigInt.Parse(_smallString);

    [Benchmark(Description = "System BigInteger Parse (small)")]
    public SysBigInt Sys_Parse_Small() => SysBigInt.Parse(_smallString);

    [Benchmark(Description = "DMath BigInteger Parse (large)")]
    public DMathBigInt DMath_Parse_Large() => DMathBigInt.Parse(_largeString);

    [Benchmark(Description = "System BigInteger Parse (large)")]
    public SysBigInt Sys_Parse_Large() => SysBigInt.Parse(_largeString);
}
