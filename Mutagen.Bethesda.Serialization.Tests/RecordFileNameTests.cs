using System.Text;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Serialization.Utility;
using Mutagen.Bethesda.Skyrim;
using Shouldly;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class RecordFileNameTests
{
    private const int MaxFileNameBytes = 255;
    private static readonly FormKey Key = FormKey.Factory("123456:Skyrim.esm");

    private static Npc NpcNamed(string editorId) => new(Key, SkyrimRelease.SkyrimSE) { EditorID = editorId };

    [Fact]
    public void ShortEditorIdWrittenWhole()
    {
        SerializationHelper.RecordFileNameProvider(NpcNamed("EditorID01"), ".json", 3)
            .ShouldBe("[3] EditorID01 - 123456_Skyrim.esm.json");
    }

    [Theory]
    [InlineData('a', null)]
    [InlineData('a', 12)]
    [InlineData('é', null)]
    public void LongEditorIdCutSoTheNameAndItsHashSiblingFit(char letter, int? number)
    {
        var name = SerializationHelper.RecordFileNameProvider(NpcNamed(new string(letter, 400)), ".json", number);
        var hashSibling = XxHashShortCircuitExporter.GetHashPath(name).Name.String;

        Encoding.UTF8.GetByteCount(hashSibling).ShouldBeLessThanOrEqualTo(MaxFileNameBytes);
        Encoding.UTF8.GetByteCount(hashSibling).ShouldBeGreaterThan(MaxFileNameBytes - Encoding.UTF8.GetByteCount(letter.ToString()));
        name.ShouldEndWith($"{letter} - 123456_Skyrim.esm.json");
    }

    [Fact]
    public void LongEditorIdCutSoTheFolderNameFits()
    {
        Encoding.UTF8.GetByteCount(SerializationHelper.RecordNameProvider(NpcNamed(new string('a', 400)), null))
            .ShouldBe(MaxFileNameBytes);
    }
}
