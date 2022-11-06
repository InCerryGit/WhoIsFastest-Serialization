#define Seria 
#define DeSeria
using BenchmarkDotNet.Running;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DemoClassProto;
using Google.Protobuf;
using MemoryPack;
using MemoryPack.Compression;
using MessagePack;
using ProtoBuf;


#if Seria
BenchmarkRunner.Run<SerializerPerformance>();
#endif
#if DeSeria
BenchmarkRunner.Run<DeserializerPerformance>();
#endif

var perf = new SerializerPerformance();
Console.WriteLine($"TextJson:{Encoding.UTF8.GetBytes(perf.TextJsonSerializer()).Length / 1024.0:F}KB");
Console.WriteLine($"ProtoBufDotNet:{perf.ProtoBufDotNet() / 1024.0:F}KB");
Console.WriteLine($"GoogleProtobuf:{perf.GoogleProtobuf() / 1024.0:F}KB");
Console.WriteLine($"MessagePack:{perf.MessagePack() / 1024.0:F}KB");
Console.WriteLine($"MessagePackLz4Block:{perf.MessagePackLz4Block() / 1024.0:F}KB");
Console.WriteLine($"MessagePackLz4BlockArray:{perf.MessagePackLz4BlockArray() / 1024.0:F}KB");
Console.WriteLine($"MemoryPack:{perf.MemoryPack() / 1024.0:F}KB");
Console.WriteLine($"MemoryPackBrotli:{perf.MemoryPackBrotli() / 1024.0:F}KB");

public static class TestData
{
    public static readonly DemoClass[] Origin = Enumerable.Range(0, 10000).Select(i =>
    {
        return new DemoClass
        {
            P1 = i,
            P2 = i % 2 == 0,
            P3 = $"Hello World {i}",
            P4 = i,
            P5 = i,
            Subs = new DemoSubClass[]
            {
                new() {P1 = i, P2 = i % 2 == 0, P3 = $"Hello World {i}", P4 = i, P5 = i,},
                new() {P1 = i, P2 = i % 2 == 0, P3 = $"Hello World {i}", P4 = i, P5 = i,},
                new() {P1 = i, P2 = i % 2 == 0, P3 = $"Hello World {i}", P4 = i, P5 = i,},
                new() {P1 = i, P2 = i % 2 == 0, P3 = $"Hello World {i}", P4 = i, P5 = i,},
            }
        };
    }).ToArray();

    public static readonly DemoClassProto.DemoClassArrayProto OriginProto;
    
    static TestData()
    {
        OriginProto = new DemoClassArrayProto();
        for (int i = 0; i < Origin.Length; i++)
        {
            OriginProto.DemoClass.Add(
                DemoClassProto.DemoClassProto.Parser.ParseJson(JsonSerializer.Serialize(Origin[i])));
        }
    }
}

[RPlotExporter]
[GcForce(true)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SerializerPerformance
{

    [Benchmark(Baseline = true)]
    public string TextJsonSerializer()
    {
        return SerializerUtil.TextJsonSerializer(TestData.Origin);
    }

    [Benchmark]
    public long ProtoBufDotNet()
    {
        using var stream = new MemoryStream(409600);
        SerializerUtil.ProtoBufDotNet(TestData.Origin, stream);
        return stream.Length;
    }

    [Benchmark]
    public long MessagePack()
    {
        return SerializerUtil.MessagePack(TestData.Origin).Length;
    }
    
    [Benchmark]
    public long MessagePackLz4Block()
    {
        return SerializerUtil.MessagePackLz4Block(TestData.Origin).Length;
    }

    [Benchmark]
    public long MessagePackLz4BlockArray()
    {
        return SerializerUtil.MessagePackLz4BlockArray(TestData.Origin).Length;
    }

    [Benchmark]
    public long MemoryPack()
    {
        return SerializerUtil.MemoryPack(TestData.Origin).Length;
    }

    [Benchmark]
    public long MemoryPackBrotli()
    {
        return SerializerUtil.MemoryPackBrotli(TestData.Origin).Length;
    }

    [Benchmark]
    public long GoogleProtobuf()
    {
        return SerializerUtil.GoogleProtobuf(TestData.OriginProto).Length;
    }
}

public static class SerializerUtil
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TextJsonSerializer<T>(T origin)
    {
        return JsonSerializer.Serialize(origin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProtoBufDotNet<T>(T origin, Stream stream)
    {
        Serializer.Serialize(stream, origin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] MessagePack<T>(T origin)
    {
        return global::MessagePack.MessagePackSerializer.Serialize(origin);
    }

    public static readonly MessagePackSerializerOptions MpLz4BOptions =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] MessagePackLz4Block<T>(T origin)
    {
        return global::MessagePack.MessagePackSerializer.Serialize(origin, MpLz4BOptions);
    }

    public static readonly MessagePackSerializerOptions MpLz4BaOptions =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] MessagePackLz4BlockArray<T>(T origin)
    {
        return global::MessagePack.MessagePackSerializer.Serialize(origin, MpLz4BaOptions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] MemoryPack<T>(T origin)
    {
        return global::MemoryPack.MemoryPackSerializer.Serialize(origin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] MemoryPackBrotli<T>(T origin)
    {
        using var compressor = new BrotliCompressor();
        global::MemoryPack.MemoryPackSerializer.Serialize(compressor, origin);
        return compressor.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GoogleProtobuf<T>(T origin) where T : IMessage<T>
    {
        return origin.ToByteArray();
    }
}

[GcForce(true)]
[RPlotExporter]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class DeserializerPerformance
{
    private static readonly string RawJson = SerializerUtil.TextJsonSerializer(TestData.Origin);
    private static readonly byte[] RawGoogleProtobuf = SerializerUtil.GoogleProtobuf(TestData.OriginProto);
    private static readonly byte[] RawMsgPack = SerializerUtil.MessagePack(TestData.Origin);
    private static readonly byte[] RawMsgPackLz4B = SerializerUtil.MessagePackLz4Block(TestData.Origin);
    private static readonly byte[] RawMsgPackLz4BA = SerializerUtil.MessagePackLz4BlockArray(TestData.Origin);
    private static readonly byte[] RawMemoryPack = SerializerUtil.MemoryPack(TestData.Origin);
    private static readonly byte[] RawMemoryPackBrotli = SerializerUtil.MemoryPackBrotli(TestData.Origin);

    [Benchmark(Baseline = true)]
    public DemoClass[] TextJsonSerializer()
    {
        return JsonSerializer.Deserialize<DemoClass[]>(RawJson)!;
    }

    [Benchmark]
    public DemoClassArrayProto GoogleProtobuf()
    {
        return DemoClassArrayProto.Parser.ParseFrom(RawGoogleProtobuf);
    }

    [Benchmark]
    public DemoClass[] ProtobufDotNet()
    {
        using var stream = new MemoryStream(RawGoogleProtobuf);
        return Serializer.Deserialize<DemoClass[]>(stream);
    }

    [Benchmark]
    public DemoClass[] MessagePack()
    {
        return global::MessagePack.MessagePackSerializer.Deserialize<DemoClass[]>(RawMsgPack);
    }
    
    [Benchmark]
    public DemoClass[] MessagePackLz4Block()
    {
        return global::MessagePack.MessagePackSerializer.Deserialize<DemoClass[]>(RawMsgPackLz4B, SerializerUtil.MpLz4BOptions);
    }
    
    [Benchmark]
    public DemoClass[] MessagePackLz4BlockArray()
    {
        return global::MessagePack.MessagePackSerializer.Deserialize<DemoClass[]>(RawMsgPackLz4BA, SerializerUtil.MpLz4BaOptions);
    }
    
    [Benchmark]
    public DemoClass[] MemoryPack()
    {
        return global::MemoryPack.MemoryPackSerializer.Deserialize<DemoClass[]>(RawMemoryPack)!;
    }
    
    [Benchmark]
    public DemoClass[] MemoryPackBrotli()
    {
        using var decompressor = new BrotliDecompressor();
        var decompressedBuffer = decompressor.Decompress(RawMemoryPackBrotli);
        return MemoryPackSerializer.Deserialize<DemoClass[]>(decompressedBuffer)!;
    }
}

[MemoryPackable]
[MessagePackObject]
[ProtoContract]
public partial class DemoClass
{
    [Key(0)] [ProtoMember(1)] public int P1 { get; set; }
    [Key(1)] [ProtoMember(2)] public bool P2 { get; set; }
    [Key(2)] [ProtoMember(3)] public string P3 { get; set; } = null!;
    [Key(3)] [ProtoMember(4)] public double P4 { get; set; }
    [Key(4)] [ProtoMember(5)] public long P5 { get; set; }
    [Key(5)] [ProtoMember(6)] public DemoSubClass[] Subs { get; set; } = null!;

    public override bool Equals(object? obj)
    {
        if (obj is DemoClass other == false) return false;
        if (other.Subs.Length != Subs.Length) return false;
        for (int i = 0; i < other.Subs.Length; i++)
        {
            if (other.Subs[i].Equals(Subs[i]) == false)
                return false;
        }

        return P1 == other.P1 &&
               P2 == other.P2 &&
               P3 == other.P3 &&
               P4.Equals(other.P4) &&
               P5 == other.P5;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(P1, P2, P3, P4, P5, Subs);
    }
}

[MemoryPackable]
[MessagePackObject]
[ProtoContract]
public partial class DemoSubClass
{
    [Key(0)] [ProtoMember(1)] public int P1 { get; set; }
    [Key(1)] [ProtoMember(2)] public bool P2 { get; set; }
    [Key(2)] [ProtoMember(3)] public string P3 { get; set; } = null!;
    [Key(3)] [ProtoMember(4)] public double P4 { get; set; }
    [Key(4)] [ProtoMember(5)] public long P5 { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is DemoSubClass other == false) return false;

        return P1 == other.P1 &&
               P2 == other.P2 &&
               P3 == other.P3 &&
               P4.Equals(other.P4) &&
               P5 == other.P5;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(P1, P2, P3, P4, P5);
    }
}