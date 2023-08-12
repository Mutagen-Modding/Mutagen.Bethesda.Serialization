using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class ListFieldGenerator : AListFieldGenerator
{
    private readonly IsMajorRecordTester _isMajorRecordTester;
    
    public ListFieldGenerator(
        IsMajorRecordTester isMajorRecordTester,
        Func<IOwned<SerializationFieldGenerator>> forFieldGenerator,
        IsGroupTester groupTester) 
        : base(forFieldGenerator, groupTester)
    {
        _isMajorRecordTester = isMajorRecordTester;
    }

    public override bool Applicable(
        LoquiTypeSet obj, 
        CustomizationSpecifications customization, 
        ITypeSymbol typeSymbol, 
        string? fieldName)
    {
        if (!base.Applicable(obj, customization, typeSymbol, fieldName)) return false;

        if (ShouldSkip(customization, obj, fieldName)) return true;
        
        if (customization.FilePerRecord)
        {
            if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
            return !_isMajorRecordTester.IsMajorRecord(namedTypeSymbol.TypeArguments[0]);
        }

        return true;
    }
    
    public override void GenerateForHasSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor, 
        string metaAccessor,
        StructuredStringBuilder sb, 
        CancellationToken cancel)
    {
        sb.AppendLine($"if ({fieldAccessor}{field.NullChar()}{GetCountAccessor(field)} > 0) return true;");
    }
    
    public override void GenerateForSerialize(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string? defaultValueAccessor,
        string writerAccessor,
        string kernelAccessor,
        string metaAccessor,
        bool isInsideCollection,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        if (ShouldSkip(compilation.Customization.Overall, obj, fieldName)) return;

        var nullable = field.IsNullable();

        field = field.PeelNullable();
        
        ITypeSymbol subType;
        if (field is IArrayTypeSymbol arr)
        {
            subType = arr.ElementType;
        }
        else if (field is INamedTypeSymbol namedTypeSymbol)
        {
            subType = GetSubtype(namedTypeSymbol);
        }
        else
        {
            return;
        }

        using (var i = sb.If(ands: true))
        {
            i.Add($"{fieldAccessor} is {{}} checked{fieldName}");
            if (!nullable)
            {
                i.Add($"checked{fieldName}{GetCountAccessor(field)} > 0");
            }
        }
        fieldAccessor = $"checked{fieldName}";
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"{kernelAccessor}.StartListSection({writerAccessor}, \"{fieldName}\");");
            sb.AppendLine($"foreach (var listItem in {fieldAccessor})");
            using (sb.CurlyBrace())
            {
                ForFieldGenerator().Value.GenerateSerializeForField(
                    compilation: compilation,
                    obj: obj,
                    fieldType: subType,
                    writerAccessor: writerAccessor, 
                    kernelAccessor: kernelAccessor,
                    metaDataAccessor: metaAccessor,
                    fieldName: null, 
                    fieldAccessor: "listItem",
                    defaultValueAccessor: null,
                    sb: sb,
                    cancel: cancel);
            }
            sb.AppendLine($"{kernelAccessor}.EndListSection({writerAccessor});");
        }
    }

    public override void GenerateForDeserializeSingleFieldInto(
        CompilationUnit compilation,
        LoquiTypeSet obj,
        ITypeSymbol field,
        string? fieldName,
        string fieldAccessor,
        string readerAccessor,
        string kernelAccessor,
        string metaAccessor,
        bool insideCollection,
        bool canSet,
        StructuredStringBuilder sb,
        CancellationToken cancel)
    {
        if (ShouldSkip(compilation.Customization.Overall, obj, fieldName)) return;

        field = field.PeelNullable();
        
        ITypeSymbol subType;
        if (field is IArrayTypeSymbol arr)
        {
            subType = arr.ElementType;
        }
        else if (field is INamedTypeSymbol namedTypeSymbol)
        {
            subType = GetSubtype(namedTypeSymbol);
        }
        else
        {
            return;
        }

        var nullable = field.IsNullable();

        if (insideCollection)
        {
            sb.AppendLine($"var ret ??= new();");
            fieldAccessor = "ret";
        }
        else if (nullable)
        {
            sb.AppendLine($"{fieldAccessor} ??= new();");
        }
        
        sb.AppendLine($"{kernelAccessor}.StartListSection({readerAccessor});");
        sb.AppendLine($"while ({kernelAccessor}.TryHasNextItem({readerAccessor}))");
        using (sb.CurlyBrace())
        {
            ForFieldGenerator().Value.GenerateDeserializeForField(
                compilation: compilation,
                obj: obj,
                fieldType: subType,
                readerAccessor: readerAccessor, 
                kernelAccessor: kernelAccessor,
                metaDataAccessor: metaAccessor,
                fieldName: fieldName, 
                fieldAccessor: "var item = ",
                sb: sb,
                cancel: cancel);
            sb.AppendLine($"{fieldAccessor}.Add(item);");
        }
        sb.AppendLine($"{kernelAccessor}.EndListSection({readerAccessor});");

        if (insideCollection)
        {
            sb.AppendLine($"return ret;");
        }
    }
}