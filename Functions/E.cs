using System;

namespace Functions;

/// <summary>
/// Euler's number.
/// </summary>
public record E : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Math.E;

    public bool IsTerm => true;

    public string Latex => $"e";
}
