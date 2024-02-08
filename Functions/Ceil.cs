using System;

namespace Functions;

/// <summary>
/// Ceil. Rounds a number up to the nearest integer.
/// </summary>
public record Ceil(IFunctionAST Inner) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Math.Ceiling(Inner.Evaluate(ctx));
}