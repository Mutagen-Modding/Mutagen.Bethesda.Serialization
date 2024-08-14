using System.Text;
using Loqui;

namespace Mutagen.Bethesda.Serialization.Utility;

public static class TypeHelper
{
    public static Type GetTypeToSerialize(Type type)
    {
        if (type.IsGenericType)
        {
            var classType = LoquiRegistration.StaticRegister.GetRegister(type.GetGenericTypeDefinition()).ClassType;
            if (type.GetGenericArguments().Length != 1)
            {
                throw new NotImplementedException();
            }

            var firstArg = type.GetGenericArguments().First();
            var genType = classType.MakeGenericType(new Type[] { firstArg });
            return genType;
        }
        else
        {
            var classType = LoquiRegistration.StaticRegister.GetRegister(type).ClassType;
            return classType;
        }
    }
    
    public static Type GetTypeFromString(string typeStr, string namespaceString)
    {
        int indexLeft = typeStr.IndexOf("<", StringComparison.OrdinalIgnoreCase);
        int indexRight = typeStr.IndexOf(">", StringComparison.OrdinalIgnoreCase);
        if (indexLeft != -1 && indexRight != -1)
        {
            var generics = typeStr.Substring(indexLeft + 1, indexRight - indexLeft - 1);
            if (generics.Contains(","))
            {
                throw new NotImplementedException();
            }

            var mainStr = $"{typeStr.Substring(0, indexLeft)}`1";
            var mainStr2 = $"{namespaceString}.{mainStr}, {namespaceString}";
            var mainType = Type.GetType(mainStr2);
            var genStr = $"{namespaceString}.{generics}, {namespaceString}";
            var genType = Type.GetType(genStr);
            if (mainType == null || genType == null)
            {
                throw new ArgumentException();
            }
            return mainType.MakeGenericType(new Type[] { genType });
        }
        else if (indexLeft != -1 || indexRight != -1)
        {
            throw new ArgumentException();
        }
        else
        {
            var finalStr = $"{namespaceString}.{typeStr}, {namespaceString}";
            return Type.GetType(finalStr)!;
        }
    }
    
    public static string GetNameWithDeclaringType(this Type t)
    {
        if (!t.IsGenericType)
        {
            if (t.DeclaringType == null)
            {
                return t.Name;
            }
            
            return $"{t.DeclaringType.Name}+{t.Name}";
        }
        string name = t.Name;
        var stringBuilder = new StringBuilder();
        int length = name.IndexOf("`", StringComparison.OrdinalIgnoreCase);
        string str = name.Substring(0, length);
        Type[] genericArguments = t.GetGenericArguments();
        if ("Nullable".Equals(str, StringComparison.CurrentCultureIgnoreCase))
        {
            if (genericArguments.Length > 1)
                throw new NotImplementedException();
            stringBuilder.Append(genericArguments[0].GetNameWithDeclaringType());
            stringBuilder.Append("?");
            return stringBuilder.ToString();
        }

        if (t.DeclaringType != null)
        {
            stringBuilder.Append(t.DeclaringType);
            stringBuilder.Append("+");
        }
        stringBuilder.Append(str);
        stringBuilder.Append("<");
        bool flag = true;
        foreach (Type t1 in genericArguments)
        {
            if (flag)
                flag = false;
            else
                stringBuilder.Append(", ");
            stringBuilder.Append(t1.GetNameWithDeclaringType());
        }
        stringBuilder.Append(">");
        return stringBuilder.ToString();
    }
}