using Mutagen.Bethesda.Serialization.Tests.SourceGenerators;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests.Customization;

public class SortFieldTests : ATestsBase
{
    private string GetBasicSortTest(
        Action<StructuredStringBuilder> customizationBuilder,
        Action<StructuredStringBuilder>? memberBuilder = null)
    {
        var sb = new StructuredStringBuilder();

        GetObjWithMember(sb, memberBuilder ?? (sb =>
        {
            sb.AppendLine("public string Name { get; set; } = \"DefaultName\";");
            sb.AppendLine("public int Priority { get; set; } = 0;");
            sb.AppendLine("public int Id { get; set; } = 0;");
        }),
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SortableItem");

        // Add a container object with a list of sortable items
        sb.AppendLine();
        sb.AppendLine("public partial interface IContainerObject : IContainerObjectGetter");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableItem> Items { get; set; }");
        }

        sb.AppendLine("public partial interface IContainerObjectGetter : ILoquiObject");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableItem> Items { get; set; }");
        }

        using (var c = sb.Class("ContainerObject_Registration"))
        {
            c.Partial = true;
            c.BaseClass = "ARegistration";
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public override ObjectKey ObjectKey { get; } = new(StaticProtocolKey, 16, 0);");
            sb.AppendLine("public override Type ClassType => typeof(ContainerObject);");
            sb.AppendLine("public override Type GetterType => typeof(IContainerObjectGetter);");
            sb.AppendLine("public override Type SetterType => typeof(IContainerObject);");
            sb.AppendLine("public override string Name => nameof(ContainerObject);");
        }

        using (var c = sb.Class("ContainerObject"))
        {
            c.Partial = true;
            c.Interfaces.Add("IContainerObject");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableItem> Items { get; set; } = new();");
            sb.AppendLine("public ILoquiRegistration Registration { get; } = new ContainerObject_Registration();");
            sb.AppendLine();
            using (var f = sb.Function("public void SomeFunction"))
            {
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine("var theObj = new ContainerObject();");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(theObj);");
            }
        }

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize<IContainerObjectGetter>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<IContainerObjectGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                customizationBuilder(sb);
            }
        }

        return sb.ToString();
    }

    [Fact]
    public Task SingleFieldSort()
    {
        var source = GetBasicSortTest(sb =>
        {
            sb.AppendLine("builder.SortList(x => x.Items).ByField(x => x.Name);");
        });

        return TestHelper.VerifySerialization(source);
    }

    [Fact]
    public Task MultipleFieldSort()
    {
        var source = GetBasicSortTest(sb =>
        {
            sb.AppendLine("builder");
            using (sb.IncreaseDepth())
            {
                sb.AppendLine(".SortList(x => x.Items).ByField(x => x.Priority)");
                sb.AppendLine(".ThenByField(x => x.Name)");
                sb.AppendLine(".ThenByField(x => x.Id);");
            }
        });

        return TestHelper.VerifySerialization(source);
    }

    [Fact]
    public Task SortWithMajorRecords()
    {
        var sb = new StructuredStringBuilder();

        // Create a major record that can be sorted
        GetObjWithMember(sb, sb =>
        {
            sb.AppendLine("public string EditorID { get; set; } = \"DefaultEditorID\";");
            sb.AppendLine("public int Priority { get; set; } = 0;");
            sb.AppendLine("public override string ToString() => EditorID ?? \"NULL\";");
        },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
                sb.AppendLine("using Mutagen.Bethesda.Plugins.Records;");
            },
            objName: "SortableMajorRecord");

        // Add a mod with a list of major records
        sb.AppendLine();
        sb.AppendLine("public partial interface ITestModGetter : IModGetter, ILoquiObject");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public IReadOnlyList<SortableMajorRecord> SortableRecords { get; }");
        }

        sb.AppendLine("public partial interface ITestMod : ITestModGetter");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public new List<SortableMajorRecord> SortableRecords { get; set; }");
        }

        using (var c = sb.Class("TestMod"))
        {
            c.Partial = true;
            c.BaseClass = "AMod";
            c.Interfaces.Add("ITestMod");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableMajorRecord> SortableRecords { get; set; } = new();");
            sb.AppendLine("IReadOnlyList<SortableMajorRecord> ITestModGetter.SortableRecords => SortableRecords;");
            sb.AppendLine();
            using (var f = sb.Function("public void SomeFunction"))
            {
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine("var theMod = new TestMod();");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(theMod);");
            }
        }

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize<ITestModGetter>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<ITestModGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder");
                using (sb.IncreaseDepth())
                {
                    sb.AppendLine(".SortList(x => x.SortableRecords).ByField(x => x.Priority)");
                    sb.AppendLine(".ThenByField(x => x.EditorID);");
                }
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }

    [Fact]
    public Task NullableListSort()
    {
        var sb = new StructuredStringBuilder();

        GetObjWithMember(sb, sb =>
        {
            sb.AppendLine("public string Name { get; set; } = \"DefaultName\";");
            sb.AppendLine("public int Priority { get; set; } = 0;");
        },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SortableItem");

        // Add a container object with a nullable list of sortable items
        sb.AppendLine();
        sb.AppendLine("public partial interface INullableContainerObject : INullableContainerObjectGetter");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableItem>? Items { get; set; }");
        }

        sb.AppendLine("public partial interface INullableContainerObjectGetter : ILoquiObject");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableItem>? Items { get; set; }");
        }

        using (var c = sb.Class("NullableContainerObject_Registration"))
        {
            c.Partial = true;
            c.BaseClass = "ARegistration";
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public override ObjectKey ObjectKey { get; } = new(StaticProtocolKey, 17, 0);");
            sb.AppendLine("public override Type ClassType => typeof(NullableContainerObject);");
            sb.AppendLine("public override Type GetterType => typeof(INullableContainerObjectGetter);");
            sb.AppendLine("public override Type SetterType => typeof(INullableContainerObject);");
            sb.AppendLine("public override string Name => nameof(NullableContainerObject);");
        }

        using (var c = sb.Class("NullableContainerObject"))
        {
            c.Partial = true;
            c.Interfaces.Add("INullableContainerObject");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableItem>? Items { get; set; }");
            sb.AppendLine("public ILoquiRegistration Registration { get; } = new NullableContainerObject_Registration();");
            sb.AppendLine();
            using (var f = sb.Function("public void SomeFunction"))
            {
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine("var theObj = new NullableContainerObject();");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(theObj);");
            }
        }

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize<INullableContainerObjectGetter>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<INullableContainerObjectGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("builder.SortList(x => x.Items).ByField(x => x.Name);");
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }
    
    [Fact]
    public Task SortListWithNullForgivingOperator()
    {
        var sb = new StructuredStringBuilder();

        GetObjWithMember(sb, sb =>
        {
            sb.AppendLine("public string Name { get; set; } = \"DefaultName\";");
            sb.AppendLine("public int Priority { get; set; } = 0;");
        },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SortableItem");

        // Add a container object with a nullable list of sortable items
        sb.AppendLine();
        sb.AppendLine("public partial interface IContainerObject : IContainerObjectGetter");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public new List<SortableItem>? Items { get; set; }");
        }

        sb.AppendLine("public partial interface IContainerObjectGetter : ILoquiObject");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableItem>? Items { get; set; }");
        }

        using (var c = sb.Class("ContainerObject_Registration"))
        {
            c.Partial = true;
            c.BaseClass = "ARegistration";
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public override ObjectKey ObjectKey { get; } = new(StaticProtocolKey, 18, 0);");
            sb.AppendLine("public override Type ClassType => typeof(ContainerObject);");
            sb.AppendLine("public override Type GetterType => typeof(IContainerObjectGetter);");
            sb.AppendLine("public override Type SetterType => typeof(IContainerObject);");
            sb.AppendLine("public override string Name => nameof(ContainerObject);");
        }

        using (var c = sb.Class("ContainerObject"))
        {
            c.Partial = true;
            c.Interfaces.Add("IContainerObject");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableItem>? Items { get; set; }");
            sb.AppendLine("public ILoquiRegistration Registration { get; } = new ContainerObject_Registration();");
            sb.AppendLine();
            using (var f = sb.Function("public void SomeFunction"))
            {
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine("var theObj = new ContainerObject();");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(theObj);");
            }
        }

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize<IContainerObjectGetter>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<IContainerObjectGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                // Test the null-forgiving operator here!
                sb.AppendLine("builder.SortList(x => x.Items!).ByField(x => x.Name);");
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }

    [Fact]
    public Task SortListWithoutNullForgivingOperator()
    {
        var sb = new StructuredStringBuilder();

        GetObjWithMember(sb, sb =>
        {
            sb.AppendLine("public string Name { get; set; } = \"DefaultName\";");
            sb.AppendLine("public int Priority { get; set; } = 0;");
        },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "SortableItem");

        // Add a container object with a nullable list of sortable items
        sb.AppendLine();
        sb.AppendLine("public partial interface IContainerObject : IContainerObjectGetter");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public new List<SortableItem>? Items { get; set; }");
        }

        sb.AppendLine("public partial interface IContainerObjectGetter : ILoquiObject");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableItem>? Items { get; set; }");
        }

        using (var c = sb.Class("ContainerObject_Registration"))
        {
            c.Partial = true;
            c.BaseClass = "ARegistration";
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public override ObjectKey ObjectKey { get; } = new(StaticProtocolKey, 19, 0);");
            sb.AppendLine("public override Type ClassType => typeof(ContainerObject);");
            sb.AppendLine("public override Type GetterType => typeof(IContainerObjectGetter);");
            sb.AppendLine("public override Type SetterType => typeof(IContainerObject);");
            sb.AppendLine("public override string Name => nameof(ContainerObject);");
        }

        using (var c = sb.Class("ContainerObject"))
        {
            c.Partial = true;
            c.Interfaces.Add("IContainerObject");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("public List<SortableItem>? Items { get; set; }");
            sb.AppendLine("public ILoquiRegistration Registration { get; } = new ContainerObject_Registration();");
            sb.AppendLine();
            using (var f = sb.Function("public void SomeFunction"))
            {
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine("var theObj = new ContainerObject();");
                sb.AppendLine("MutagenTestConverter.Instance.Convert(theObj);");
            }
        }

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize<IContainerObjectGetter>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<IContainerObjectGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                // Test without null-forgiving operator - should work with updated interface!
                sb.AppendLine("builder.SortList(x => x.Items).ByField(x => x.Name);");
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }

    [Fact]
    public Task SortListOfStringsWithoutByField()
    {
        var sb = new StructuredStringBuilder();

        GetObjWithMember(sb, sb =>
        {
            sb.AppendLine("public List<string> Names { get; set; } = new();");
        },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "StringContainer");

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize<IStringContainerGetter>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<IStringContainerGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                // Sort strings directly without ByField
                sb.AppendLine("builder.SortList(x => x.Names);");
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }

    [Fact]
    public Task SortListOfIntsWithoutByField()
    {
        var sb = new StructuredStringBuilder();

        GetObjWithMember(sb, sb =>
        {
            sb.AppendLine("public List<int> Numbers { get; set; } = new();");
        },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "IntContainer");

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize<IIntContainerGetter>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<IIntContainerGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                // Sort integers directly without ByField
                sb.AppendLine("builder.SortList(x => x.Numbers);");
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }

    [Fact]
    public Task SortNullableListOfStringsWithoutByField()
    {
        var sb = new StructuredStringBuilder();

        GetObjWithMember(sb, sb =>
        {
            sb.AppendLine("public List<string>? Names { get; set; }");
        },
            namespaceBuilder: sb =>
            {
                sb.AppendLine("using Mutagen.Bethesda.Serialization.Customizations;");
            },
            objName: "NullableStringContainer");

        sb.AppendLine();
        using (var c = sb.Class("Customization"))
        {
            c.Interfaces.Add("ICustomize<INullableStringContainerGetter>");
        }
        using (sb.CurlyBrace())
        {
            using (var f = sb.Function("public void CustomizeFor"))
            {
                f.Add("ICustomizationBuilder<INullableStringContainerGetter> builder");
            }
            using (sb.CurlyBrace())
            {
                // Sort nullable list of strings directly without ByField
                sb.AppendLine("builder.SortList(x => x.Names);");
            }
        }

        return TestHelper.VerifySerialization(sb.ToString());
    }
}