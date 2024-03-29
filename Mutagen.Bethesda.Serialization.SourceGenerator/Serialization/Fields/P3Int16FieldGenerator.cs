﻿namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class P3Int16FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "P3Int16",
        "Noggog.P3Int16",
    };
    
    public P3Int16FieldGenerator()
        : base("P3Int16", AssociatedTypes)
    {
    }
}