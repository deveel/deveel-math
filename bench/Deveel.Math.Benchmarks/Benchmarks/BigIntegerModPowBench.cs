using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using Deveel.Math;
using DMathBigInt = Deveel.Math.BigInteger;
using SysBigInt = System.Numerics.BigInteger;

namespace Deveel.Math.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class BigIntegerModPowBench
{
    private DMathBigInt _dBase;
    private DMathBigInt _dExp;
    private DMathBigInt _dMod;
    private SysBigInt _sBase;
    private SysBigInt _sExp;
    private SysBigInt _sMod;

    [Params(128, 512)]
    public int BitSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        int byteLen = (BitSize + 7) / 8;
        var bytes = new byte[byteLen + 1];

        rnd.NextBytes(bytes);
        bytes[byteLen] = 0;
        bytes[0] |= 1; // ensure odd modulus
        _dMod = new DMathBigInt((byte[])bytes.Clone());
        _sMod = new SysBigInt((byte[])bytes.Clone());

        rnd.NextBytes(bytes);
        bytes[byteLen] = 0;
        _dBase = new DMathBigInt((byte[])bytes.Clone());
        _sBase = new SysBigInt((byte[])bytes.Clone());

        rnd.NextBytes(bytes);
        bytes[byteLen] = 0;
        _dExp = new DMathBigInt((byte[])bytes.Clone());
        _sExp = new SysBigInt((byte[])bytes.Clone());
    }

    [Benchmark]
    public DMathBigInt DMath_ModPow() => BigMath.ModPow(_dBase, _dExp, _dMod);

    [Benchmark]
    public SysBigInt Sys_ModPow() => SysBigInt.ModPow(_sBase, _sExp, _sMod);
}
