using System.Buffers;
using System.Text.Json;
using Google.Protobuf;
using MemoryPack.Compression;
using Xunit;
using ProtoBuf;

namespace WhoIsFastest_Serialization.Tests;

public class ShouldBeWork
{
    private readonly DemoClass _originObj = new DemoClass
    {
        P1 = 65535,
        P2 = false,
        P3 = "null",
        P4 = 65535.35,
        P5 = 65535,
        Subs = new DemoSubClass[]
        {
            new() {P1 = 1, P2 = true, P3 = "1", P4 = 2, P5 = 3},
            new() {P1 = 4, P2 = true, P3 = "5", P4 = 6, P5 = 7},
            new() {P1 = 8, P2 = true, P3 = "9", P4 = 10, P5 = 10}
        }
    };

    [Fact]
    public void TextJson()
    {
        var json = JsonSerializer.Serialize(_originObj);
        var newObj = JsonSerializer.Deserialize<DemoClass>(json);
        Assert.Equal(_originObj, newObj);
    }

    [Fact]
    public void ProtoBufDotNet()
    {
        var buffer = new ArrayBufferWriter<byte>();
        Serializer.Serialize(buffer, _originObj);
        var newObj = Serializer.Deserialize<DemoClass>(buffer.WrittenSpan);
        Assert.Equal(_originObj, newObj);
    }

    [Fact]
    public void MessagePackCSharp()
    {
        var buffer = MessagePack.MessagePackSerializer.Serialize(_originObj);
        var newObj = MessagePack.MessagePackSerializer.Deserialize<DemoClass>(buffer);
        Assert.Equal(_originObj, newObj);
    }
    
    [Fact]
    public void MemoryPackCSharp()
    {
        var buffer = MemoryPack.MemoryPackSerializer.Serialize(_originObj);
        var newObj = MemoryPack.MemoryPackSerializer.Deserialize<DemoClass>(buffer);
        Assert.Equal(_originObj, newObj);
    }

    [Fact]
    public void GoogleProtoBuffer()
    {
        var originObj = DemoClassProto.DemoClassProto.Parser.ParseJson(JsonSerializer.Serialize(_originObj));
        var buffer = originObj.ToByteArray();
        var newObj = DemoClassProto.DemoClassProto.Parser.ParseFrom(buffer);
        Assert.Equal(originObj, newObj);
    }
}