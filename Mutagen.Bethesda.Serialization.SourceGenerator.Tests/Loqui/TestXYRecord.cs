using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class XYSubBlock
{
    public short BlockNumberX { get; set; }
    public short BlockNumberY { get; set; }
    public string SomeValue { get; set; } = string.Empty;
    public List<TestMajorRecord> Records { get; set; } = new();
    
    protected bool Equals(XYSubBlock other)
    {
        return BlockNumberX == other.BlockNumberX
               && BlockNumberY == other.BlockNumberY
               && SomeValue == other.SomeValue 
               && Records.SequenceEqual(other.Records);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((XYSubBlock)obj);
    }
}

public class XYBlock
{
    public short BlockNumberX { get; set; }
    public short BlockNumberY { get; set; }
    public string SomeValue { get; set; } = string.Empty;
    public List<XYSubBlock> SubBlocks { get; set; } = new();
    
    protected bool Equals(XYBlock other)
    {
        return BlockNumberX == other.BlockNumberX 
               && BlockNumberY == other.BlockNumberY 
               && SomeValue == other.SomeValue
               && SubBlocks.SequenceEqual(other.SubBlocks);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((XYBlock)obj);
    }
}

public class TestXYRecord : TestMajorRecord
{
    public int SomeValue { get; set; }
    public List<XYBlock> Blocks { get; set; } = new();

    public TestXYRecord(FormKey i, string s) 
        : base(i, s)
    {
    }
    
    protected bool Equals(TestXYRecord other)
    {
        return SomeValue == other.SomeValue && Blocks.SequenceEqual(other.Blocks);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TestXYRecord)obj);
    }

    public void Clear() => Blocks.Clear();
}

public class TestXYBlockGroup : IClearable
{
    public int SomeValue { get; set; }
    public List<TestXYRecord> Records { get; set; } = new();
    
    protected bool Equals(TestXYBlockGroup other)
    {
        return SomeValue == other.SomeValue && Records.SequenceEqual(other.Records);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TestXYBlockGroup)obj);
    }

    public void Clear() => Records.Clear();
}