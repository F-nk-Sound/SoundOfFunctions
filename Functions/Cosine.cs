using System;

namespace Functions;

/// <summary>
/// Cosine function.
/// </summary>
public record Cosine(IFunctionAST Inner) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Math.Cos(Inner.Evaluate(ctx));
}