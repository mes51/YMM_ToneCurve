using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace YMM_ToneCurve.SourceGenerator.EffectPropertyGenerator
{
    [Generator(LanguageNames.CSharp)]
    public class EffectPropertyGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(static context =>
            {
                EffectPropertyEmiiter.RegisterAttributes(context);
            });

            EffectPropertyEmiiter.RegisterEmit(context);
        }
    }
}
