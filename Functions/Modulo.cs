using System;
using Godot;

namespace Functions;

/// <summary>
/// Multiplication in a function.
/// </summary>
public record Modulo(IFunctionAST Left, IFunctionAST Right) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Mathf.PosMod(Left.Evaluate(ctx), Right.Evaluate(ctx));

    public bool IsTerm => false;

    public string Latex => $"{Left.SingleTermLatex} \\% {Right.SingleTermLatex}";
}
