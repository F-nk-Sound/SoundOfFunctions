using System;

namespace Functions;

/// <summary>
/// Floor. Rounds a number down to the nearest integer.
/// </summary>
public record Floor(IFunctionAST Inner) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Math.Floor(Inner.Evaluate(ctx));
}