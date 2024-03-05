using System;

namespace Functions;

/// <summary>
/// Sine function.
/// </summary>
public record Exponent(IFunctionAST Base, IFunctionAST Power) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Math.Pow(Base.Evaluate(ctx), Power.Evaluate(ctx));

    public bool IsTerm => true;

    public string Latex => $"{{{Base.Latex}}}^{{{Power.Latex}}}";
}