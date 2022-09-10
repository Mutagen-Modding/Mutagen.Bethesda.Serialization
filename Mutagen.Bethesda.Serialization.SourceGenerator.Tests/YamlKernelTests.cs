using Mutagen.Bethesda.Serialization.Yaml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

[UsesVerify]
public class YamlKernelTests : AKernelTest<YamlSerializationWriterKernel, YamlWritingUnit>
{
    public class TestClass
    {
        // public string Test = "Hello";
        // public string Test2 = "Hello2";
        public int[] Test3 = new[] { 1, 2, 3 };
    }
    
    [Fact]
    public void Test()
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(new TestClass());
    }
}