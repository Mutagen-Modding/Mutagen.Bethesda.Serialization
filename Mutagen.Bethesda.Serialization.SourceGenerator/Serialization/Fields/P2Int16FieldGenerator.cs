﻿namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class P2Int16FieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "P2Int16",
        "Noggog.P2Int16",
    };
    
    public P2Int16FieldGenerator()
        : base("P2Int16", AssociatedTypes)
    {
    }
}