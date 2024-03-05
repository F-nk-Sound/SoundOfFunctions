using System;

namespace Functions;

/// <summary>
/// Sine function.
/// </summary>
public record Sine(IFunctionAST Inner) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Math.Sin(Inner.Evaluate(ctx));

    public bool IsTerm => true;

    public string Latex => $"\\sin({Inner.Latex})";
}