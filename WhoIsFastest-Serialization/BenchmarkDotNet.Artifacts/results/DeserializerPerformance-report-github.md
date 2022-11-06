``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.755)
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.100-rc.2.22477.23
  [Host]     : .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2
  Job-DLQWTS : .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2

Force=True  

```
|                   Method |      Mean |     Error |    StdDev | Ratio |      Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|------------------------- |----------:|----------:|----------:|------:|----------:|---------:|---------:|----------:|------------:|
|               MemoryPack |  9.754 ms | 0.1943 ms | 0.2313 ms |  0.20 | 1078.1250 | 484.3750 | 140.6250 |   5.65 MB |        0.57 |
|              MessagePack | 13.135 ms | 0.2445 ms | 0.2287 ms |  0.27 | 1078.1250 | 484.3750 | 140.6250 |   5.65 MB |        0.57 |
| MessagePackLz4BlockArray | 13.554 ms | 0.1852 ms | 0.1732 ms |  0.28 | 1078.1250 | 484.3750 | 140.6250 |   5.65 MB |        0.57 |
|      MessagePackLz4Block | 13.883 ms | 0.1666 ms | 0.1559 ms |  0.29 | 1078.1250 | 484.3750 | 140.6250 |   7.27 MB |        0.73 |
|         MemoryPackBrotli | 14.331 ms | 0.2601 ms | 0.2433 ms |  0.30 | 1078.1250 | 484.3750 | 140.6250 |   5.65 MB |        0.57 |
|           ProtobufDotNet | 15.281 ms | 0.0877 ms | 0.0733 ms |  0.32 | 1015.6250 | 468.7500 |  78.1250 |   5.65 MB |        0.57 |
|           GoogleProtobuf | 16.368 ms | 0.2209 ms | 0.2066 ms |  0.34 | 1296.8750 | 578.1250 | 203.1250 |   6.81 MB |        0.68 |
|       TextJsonSerializer | 48.014 ms | 0.2295 ms | 0.2035 ms |  1.00 | 1181.8182 | 545.4545 |  90.9091 |   9.97 MB |        1.00 |
