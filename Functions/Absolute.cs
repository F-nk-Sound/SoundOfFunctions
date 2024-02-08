using System;

namespace Functions;

/// <summary>
/// Absolute value (`|x|`).
/// </summary>
public record Absolute(IFunctionAST Inner) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Math.Abs(Inner.Evaluate(ctx));
}