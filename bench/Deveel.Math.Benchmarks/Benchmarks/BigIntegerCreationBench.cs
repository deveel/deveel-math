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

    [Benchmark]
    public DMathBigInt DMath_FromInt64() => DMathBigInt.FromInt64(123456789012345L);

    [Benchmark]
    public SysBigInt Sys_FromInt64() => new SysBigInt(123456789012345L);
    public SysBigInt Sys_FromBytes_Small() => new SysBigInt(_smallBytes);
    public SysBigInt Sys_FromBytes_Large() => new SysBigInt(_largeBytes);
    public SysBigInt Sys_Parse_Small() => SysBigInt.Parse(_smallString);
    public SysBigInt Sys_Parse_Large() => SysBigInt.Parse(_largeString);
}
