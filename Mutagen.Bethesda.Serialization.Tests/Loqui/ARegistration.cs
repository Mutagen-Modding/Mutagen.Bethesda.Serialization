using Loqui;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public abstract class ARegistration : ILoquiRegistration
{
    public static ProtocolKey StaticProtocolKey { get; } = new("Test");
    
    public string GetNthName(ushort index) => throw new NotImplementedException();

    public bool GetNthIsLoqui(ushort index) => throw new NotImplementedException();

    public bool GetNthIsEnumerable(ushort index) => throw new NotImplementedException();

    public bool GetNthIsSingleton(ushort index) => throw new NotImplementedException();

    public bool IsNthDerivative(ushort index) => throw new NotImplementedException();

    public Type GetNthType(ushort index) => throw new NotImplementedException();

    public ushort? GetNameIndex(StringCaseAgnostic name) => throw new NotImplementedException();

    public bool IsProtected(ushort index) => throw new NotImplementedException();

    public ProtocolKey ProtocolKey { get; } = StaticProtocolKey;
    public abstract ObjectKey ObjectKey { get; }
    public string GUID => throw new NotImplementedException();
    public ushort AdditionalFieldCount => throw new NotImplementedException();
    public ushort FieldCount => throw new NotImplementedException();
    public Type MaskType => throw new NotImplementedException();
    public Type ErrorMaskType => throw new NotImplementedException();
    public abstract Type ClassType { get; }
    public abstract Type GetterType { get; }
    public abstract Type SetterType { get; }
    public Type? InternalGetterType => null;
    public Type? InternalSetterType => null;
    public string FullName => $"{Namespace}.{Name}";
    public abstract string Name { get; }
    public string Namespace => "Mutagen.Bethesda.Serialization.Tests";
    public byte GenericCount => throw new NotImplementedException();
    public Type? GenericRegistrationType => null;
}