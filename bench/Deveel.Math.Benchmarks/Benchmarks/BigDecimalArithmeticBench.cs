using System;
using BenchmarkDotNet.Attributes;
using Deveel.Math;

namespace Deveel.Math.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class BigDecimalArithmeticBench
{
    private BigDecimal _dA = null!;
    private BigDecimal _dB = null!;
    private decimal _sA;
    private decimal _sB;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        _dA = BigDecimal.Parse("12345.67890123456789");
        _sA = 12345.67890123456789m;
        _dB = BigDecimal.Parse("98765.43210987654321");
        _sB = 98765.43210987654321m;
    }

    [Benchmark]
    public BigDecimal DMath_Add() => _dA + _dB;

    [Benchmark]
    public decimal Sys_Add() => _sA + _sB;

    [Benchmark]
    public BigDecimal DMath_Subtract() => _dA - _dB;

    [Benchmark]
    public decimal Sys_Subtract() => _sA - _sB;

    [Benchmark]
    public BigDecimal DMath_Multiply() => _dA * _dB;

    [Benchmark]
    public decimal Sys_Multiply() => _sA * _sB;

    [Benchmark]
    public BigDecimal DMath_Divide() => _dA.Divide(_dB, RoundingMode.HalfUp);

    [Benchmark]
    public decimal Sys_Divide() => _sA / _sB;

    [Benchmark]
    public BigDecimal DMath_Negate() => -_dA;

    [Benchmark]
    public decimal Sys_Negate() => -_sA;
}
