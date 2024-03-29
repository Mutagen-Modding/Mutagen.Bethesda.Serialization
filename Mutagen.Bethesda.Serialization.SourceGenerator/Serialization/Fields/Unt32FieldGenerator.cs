﻿namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class UInt32FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "UInt32",
        "uint"
    };
    
    public UInt32FieldGenerator()
        : base("UInt32", AssociatedTypes)
    {
    }
}