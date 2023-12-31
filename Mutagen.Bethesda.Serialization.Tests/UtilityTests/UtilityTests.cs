﻿namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.UtilityTests;

public class UtilityTests
{
    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}