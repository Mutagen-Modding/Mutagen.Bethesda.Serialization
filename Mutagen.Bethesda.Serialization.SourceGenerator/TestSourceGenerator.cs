using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Serialization.SourceGenerator;

[Generator]
public class TestSourceGenerator3 : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        //File.AppendAllText("C:\\Test\\Log.txt", $"{nameof(TestSourceGenerator3)}\n");
        context.AddSource($"TestOutput3.g.cs", "//Test3");
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}
