using System;
using BenchmarkDotNet.Attributes;

namespace Deveel.Math.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class BigDecimalParsingBench
{
    private string _smallString = null!;
    private string _largeString = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallString = "12345.67890123456789";
        _largeString = "12345678901234567890.1234567890123456789";
    }

    [Benchmark]
    public BigDecimal DMath_Parse_Small() => BigDecimal.Parse(_smallString);

    [Benchmark]
    public decimal Sys_Parse_Small() => decimal.Parse(_smallString);

    [Benchmark]
    public BigDecimal DMath_Parse_Large() => BigDecimal.Parse(_largeString);

    [Benchmark]
    public BigDecimal DMath_ToString_Small() => BigDecimal.Parse(_smallString);

    [Benchmark]
    public string DMath_ToString() => BigDecimal.Parse(_smallString).ToString();

    [Benchmark]
    public string Sys_ToString() => decimal.Parse(_smallString).ToString();
}
