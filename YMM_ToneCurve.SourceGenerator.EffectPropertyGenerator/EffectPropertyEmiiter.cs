using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace YMM_ToneCurve.SourceGenerator.EffectPropertyGenerator
{
    static class EffectPropertyEmiiter
    {
        static readonly string Namespace = typeof(EffectPropertyEmiiter).Namespace;

        static readonly string GenerateEffectPropertyAttributeName = $"{Namespace}.GenerateEffectPropertyAttribute";

        public static void RegisterAttributes(IncrementalGeneratorPostInitializationContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            context.AddSource(GenerateEffectPropertyAttributeName, $"namespace {Namespace};" + $$"""

using System;
using Vortice.Direct2D1;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
sealed class GenerateEffectPropertyAttribute : Attribute
{
    public string PropertyNameBase { get; }

    public Type ManagedPropertyType { get; }

    public PropertyType PropertyType { get; }

    public int PropertyBeginIndex { get; }

    public int Count { get; }

    public GenerateEffectPropertyAttribute(string propertyNameBase, Type managedPropertyType, PropertyType propertyType, int propertyBeginIndex, int count)
    {
        PropertyNameBase = propertyNameBase;
        ManagedPropertyType = managedPropertyType;
        PropertyType = propertyType;
        PropertyBeginIndex = propertyBeginIndex;
        Count = count;
    }
}
""");
        }

        public static void RegisterEmit(IncrementalGeneratorInitializationContext context)
        {
            var typeDefinitions = context.SyntaxProvider.ForAttributeWithMetadataName(
                GenerateEffectPropertyAttributeName,
                static (node, token) => node is ClassDeclarationSyntax,
                static (context, token) => (TypeDeclarationSyntax)context.TargetNode
            );

            var source = typeDefinitions.Combine(context.CompilationProvider).WithComparer(Comparer.Instance);

            context.RegisterSourceOutput(source, (context, source) =>
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                Emit(context, source.Item1, source.Item2);
            });
        }

        static void Emit(SourceProductionContext context, TypeDeclarationSyntax syntax, Compilation compilation)
        {
            var semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            if (semanticModel == null)
            {
                return;
            }

            context.CancellationToken.ThrowIfCancellationRequested();

            if (semanticModel.GetDeclaredSymbol(syntax, context.CancellationToken) is not INamedTypeSymbol typeSymbol)
            {
                return;
            }

            context.CancellationToken.ThrowIfCancellationRequested();

            var generateEffectPropertyAttributeSymbol = Util.GetTypeSymbol(compilation, GenerateEffectPropertyAttributeName);

            context.CancellationToken.ThrowIfCancellationRequested();

            var generateEffectPropertyAttribute = Util.GetAttributeData(typeSymbol, generateEffectPropertyAttributeSymbol);

            context.CancellationToken.ThrowIfCancellationRequested();

            if (!Validate(context, syntax, compilation, typeSymbol))
            {
                return;
            }

            context.CancellationToken.ThrowIfCancellationRequested();

            var propertyDefinitions = new StringBuilder();
            foreach (var (attributeData, i) in generateEffectPropertyAttribute.Select((a, i) => (a, i)))
            {
                var propertyNameBase = (attributeData.ConstructorArguments[0].Value as string) ?? "";
                var managedPropertyType = attributeData.ConstructorArguments[1].Value as INamedTypeSymbol;
                var propertyType = (int)(attributeData.ConstructorArguments[2].Value ?? 0);
                var propertyBeginIndex = (int)(attributeData.ConstructorArguments[3].Value ?? -1);
                var count = (int)(attributeData.ConstructorArguments[4].Value ?? 0);

                if (string.IsNullOrEmpty(propertyNameBase) || managedPropertyType == null || propertyBeginIndex < 0 || count < 1)
                {
                    continue;
                }

                for (var pi = 0; pi < count; pi++)
                {
                    propertyDefinitions.AppendLine($$"""
    [CustomEffectProperty((PropertyType){{propertyType}}, {{propertyBeginIndex + pi}})]
    {{managedPropertyType.ContainingNamespace}}.{{managedPropertyType.Name}} {{propertyNameBase}}{{pi + 1}} { get; set; }
""");

                    context.CancellationToken.ThrowIfCancellationRequested();
                }

                propertyDefinitions.AppendLine();

                propertyDefinitions.AppendLine($$"""
    public {{managedPropertyType.ContainingNamespace}}.{{managedPropertyType.Name}}[] {{propertyNameBase}}
    {
        get
        {
            return [
""");

                for (var pi = 0; pi < count; pi++)
                {
                    propertyDefinitions.AppendLine($"                {propertyNameBase}{pi + 1},");
                }

                propertyDefinitions.AppendLine($$"""
            ];
        }
        set
        {
""");

                for (var pi = 0; pi < count; pi++)
                {
                    propertyDefinitions.AppendLine($$"""
            if (value.Length < {{pi + 1}})
            {
                return;
            }
            {{propertyNameBase}}{{pi + 1}} = value[{{pi}}];
""");
                }

                propertyDefinitions.AppendLine("""
        }
    }
""");
            }

            var fileName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
    .Replace("global::", "")
    .Replace("<", "_")
    .Replace(">", "_") + ".EffectPropertyGenerator.g.cs";

            var code = $$"""
// <auto-generated/>
#nullable enable
#pragma warning disable CS8600
#pragma warning disable CS8601
#pragma warning disable CS8602
#pragma warning disable CS8603
#pragma warning disable CS8604
#pragma warning disable CS1522

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Vortice.Direct2D1;

namespace {{typeSymbol.ContainingNamespace}};

partial class {{typeSymbol.Name}}
{
{{propertyDefinitions}}
}
""";
            context.AddSource(fileName, code);
        }

        static bool Validate(SourceProductionContext context, TypeDeclarationSyntax syntax, Compilation compilation, INamedTypeSymbol typeSymbol)
        {
            if (!syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(EffectPropertyDiagnosticDescriptors.MustBePartial, syntax.Identifier.GetLocation(), typeSymbol.Name)
                );
                return false;
            }

            if (!Util.IsInherit(typeSymbol, "Vortice.Direct2D1.CustomEffectBase"))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(EffectPropertyDiagnosticDescriptors.MustInheritCustomEffectBase, syntax.Identifier.GetLocation(), typeSymbol.Name)
                );
                return false;
            }

            return true;
        }
    }
}
