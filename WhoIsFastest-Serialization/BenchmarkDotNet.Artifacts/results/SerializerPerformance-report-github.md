``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.755)
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.100-rc.2.22477.23
  [Host]     : .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2
  Job-DLQWTS : .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2

Force=True  

```
|                   Method |      Mean |     Error |    StdDev | Ratio |     Gen0 |     Gen1 |     Gen2 |  Allocated | Alloc Ratio |
|------------------------- |----------:|----------:|----------:|------:|---------:|---------:|---------:|-----------:|------------:|
|               MemoryPack |  2.947 ms | 0.0260 ms | 0.0231 ms |  0.12 | 128.9063 | 128.9063 | 128.9063 | 2279.89 KB |        0.34 |
|              MessagePack |  4.084 ms | 0.0405 ms | 0.0359 ms |  0.16 |  93.7500 |  93.7500 |  93.7500 | 1660.88 KB |        0.25 |
|         MemoryPackBrotli |  4.702 ms | 0.0553 ms | 0.0490 ms |  0.19 |        - |        - |        - |  148.28 KB |        0.02 |
| MessagePackLz4BlockArray |  4.705 ms | 0.0336 ms | 0.0314 ms |  0.19 |  15.6250 |  15.6250 |  15.6250 |  259.02 KB |        0.04 |
|      MessagePackLz4Block |  5.032 ms | 0.0164 ms | 0.0146 ms |  0.20 |  31.2500 |  31.2500 |  31.2500 |  516.15 KB |        0.08 |
|           GoogleProtobuf |  6.742 ms | 0.0227 ms | 0.0212 ms |  0.27 | 125.0000 | 125.0000 | 125.0000 | 1761.99 KB |        0.27 |
|           ProtoBufDotNet |  8.705 ms | 0.0492 ms | 0.0436 ms |  0.34 | 343.7500 | 343.7500 | 343.7500 | 6002.73 KB |        0.91 |
|       TextJsonSerializer | 25.318 ms | 0.2552 ms | 0.2387 ms |  1.00 | 281.2500 | 281.2500 | 281.2500 | 6629.02 KB |        1.00 |
