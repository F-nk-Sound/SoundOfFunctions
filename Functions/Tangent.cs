using System;

namespace Functions;

/// <summary>
/// Tangent function.
/// </summary>
public record Tangent(IFunctionAST Inner) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Math.Tan(Inner.Evaluate(ctx));

    public bool IsTerm => true;

    public string Latex => $"tan({Inner.Latex})";
}