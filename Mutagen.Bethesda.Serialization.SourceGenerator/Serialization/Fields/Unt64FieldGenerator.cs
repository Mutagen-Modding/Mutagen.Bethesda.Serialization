﻿namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class UInt64FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "UInt64",
        "ulong"
    };
    
    public UInt64FieldGenerator()
        : base("UInt64", AssociatedTypes)
    {
    }
}