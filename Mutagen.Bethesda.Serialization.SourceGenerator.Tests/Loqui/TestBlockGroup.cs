using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class SubBlock
{
    public int BlockNumber { get; set; }
    public string SomeValue { get; set; } = string.Empty;
    public List<TestMajorRecord> Records { get; set; } = new();
    
    protected bool Equals(SubBlock other)
    {
        return BlockNumber == other.BlockNumber && SomeValue == other.SomeValue && Records.SequenceEqual(other.Records);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SubBlock)obj);
    }
}

public class Block
{
    public int BlockNumber { get; set; }
    public string SomeValue { get; set; }= string.Empty;
    public List<SubBlock> SubBlocks { get; set; } = new();
    
    protected bool Equals(Block other)
    {
        return BlockNumber == other.BlockNumber && SomeValue == other.SomeValue && SubBlocks.SequenceEqual(other.SubBlocks);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Block)obj);
    }
}

public class TestBlockGroup : IClearable
{
    public int SomeValue { get; set; }
    public List<Block> Blocks { get; set; } = new();
    
    protected bool Equals(TestBlockGroup other)
    {
        return SomeValue == other.SomeValue && Blocks.SequenceEqual(other.Blocks);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TestBlockGroup)obj);
    }

    public void Clear() => Blocks.Clear();
}