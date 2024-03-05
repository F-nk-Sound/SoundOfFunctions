namespace Functions;

/// <summary>
/// Division in a function.
/// </summary>
public record Divide(IFunctionAST Left, IFunctionAST Right) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Left.Evaluate(ctx) / Right.Evaluate(ctx);

    public bool IsTerm => false;

    public string Latex => $"\frac{{{Left.Latex}}}{{{Right.Latex}}}";
}