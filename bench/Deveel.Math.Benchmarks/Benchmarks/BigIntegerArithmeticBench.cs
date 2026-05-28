using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using DMathBigInt = Deveel.Math.BigInteger;
using SysBigInt = System.Numerics.BigInteger;

namespace Deveel.Math.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class BigIntegerArithmeticBench
{
    private DMathBigInt _dA;
    private DMathBigInt _dB;
    private SysBigInt _sA;
    private SysBigInt _sB;

    [Params(64, 256, 1024, 4096)]
    public int BitSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        int byteLen = (BitSize + 7) / 8;
        var bytes = new byte[byteLen + 1];
        rnd.NextBytes(bytes);
        bytes[byteLen] = 0;
        _dA = new DMathBigInt((byte[])bytes.Clone());
        _sA = new SysBigInt((byte[])bytes.Clone());

        rnd.NextBytes(bytes);
        bytes[byteLen] = 0;
        _dB = new DMathBigInt((byte[])bytes.Clone());
        _sB = new SysBigInt((byte[])bytes.Clone());
    }

    [Benchmark(Description = "DMath BigInteger Add")]
    public DMathBigInt DMath_Add() => _dA + _dB;

    [Benchmark(Description = "System BigInteger Add")]
    public SysBigInt Sys_Add() => _sA + _sB;

    [Benchmark(Description = "DMath BigInteger Subtract")]
    public DMathBigInt DMath_Subtract() => _dA - _dB;

    [Benchmark(Description = "System BigInteger Subtract")]
    public SysBigInt Sys_Subtract() => _sA - _sB;

    [Benchmark(Description = "DMath BigInteger Multiply")]
    public DMathBigInt DMath_Multiply() => _dA * _dB;

    [Benchmark(Description = "System BigInteger Multiply")]
    public SysBigInt Sys_Multiply() => _sA * _sB;

    [Benchmark(Description = "DMath BigInteger Divide")]
    public DMathBigInt DMath_Divide() => _dA / _dB;

    [Benchmark(Description = "System BigInteger Divide")]
    public SysBigInt Sys_Divide() => _sA / _sB;

    [Benchmark(Description = "DMath BigInteger Negate")]
    public DMathBigInt DMath_Negate() => -_dA;

    [Benchmark(Description = "System BigInteger Negate")]
    public SysBigInt Sys_Negate() => -_sA;
}
