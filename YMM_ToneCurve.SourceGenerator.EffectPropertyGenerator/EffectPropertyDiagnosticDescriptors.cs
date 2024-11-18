using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace YMM_ToneCurve.SourceGenerator.EffectPropertyGenerator
{
    static class EffectPropertyDiagnosticDescriptors
    {
        const string Category = "EffectPropertyGenerate";

        public static readonly DiagnosticDescriptor MustBePartial = new DiagnosticDescriptor(
            id: "EPG0001",
            title: "CustomShaderEffect must be partial",
            messageFormat: "CustomShaderEffect '{0}' must be partial",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MustInheritCustomEffectBase = new DiagnosticDescriptor(
            id: "EPG0001",
            title: "Class must inherit Vortice.Direct2D1.CustomEffectBase class",
            messageFormat: "'{0}' must inherit Vortice.Direct2D1.CustomEffectBase class",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
    }
}
