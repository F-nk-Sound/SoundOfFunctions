using System;

namespace Functions;

/// <summary>
/// Pi.
/// </summary>
public record Pi : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Math.PI;

    public bool IsTerm => true;

    public string Latex => $"\\pi";
}
