using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace YMM_ToneCurve.SourceGenerator.EffectPropertyGenerator
{
    // https://github.com/Cysharp/MemoryPack/blob/ca28202a65b22b6fd3ea66b4c75eab252255a097/src/MemoryPack.Generator/MemoryPackGenerator.cs#L281
    // https://github.com/Cysharp/MemoryPack/blob/main/LICENSE.md
    class Comparer : IEqualityComparer<(TypeDeclarationSyntax, Compilation)>
    {
        public static readonly Comparer Instance = new Comparer();

        public bool Equals((TypeDeclarationSyntax, Compilation) x, (TypeDeclarationSyntax, Compilation) y)
        {
            return x.Item1.Equals(y.Item1);
        }

        public int GetHashCode((TypeDeclarationSyntax, Compilation) obj)
        {
            return obj.Item1.GetHashCode();
        }
    }

    static class Util
    {
        public static bool IsInherit(ITypeSymbol typeSymbol, string targetTypeFullName)
        {
            var baseType = typeSymbol;
            while (baseType != null)
            {
                if (baseType.ToString() == targetTypeFullName)
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }

            return false;
        }

        public static INamedTypeSymbol GetTypeSymbol(Compilation compilation, string symbolName)
        {
            return compilation.GetTypeByMetadataName(symbolName) ?? throw new InvalidOperationException($"{symbolName} is not found");
        }

        public static AttributeData[] GetAttributeData(ITypeSymbol targetTypeSymbol, ITypeSymbol attributeTypeSymbol)
        {
            var attributeData = targetTypeSymbol
                .GetAttributes()
                .Where(a => SymbolEqualityComparer.Default.Equals(attributeTypeSymbol, a.AttributeClass))
                .ToArray();
            if (attributeData.Length < 1)
            {
                throw new InvalidOperationException($"processing class is not applied {attributeTypeSymbol.Name}");
            }

            return attributeData;
        }
    }
}
