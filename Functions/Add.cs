namespace Functions;

/// <summary>
/// Addition in a function.
/// </summary>
public record Add(IFunctionAST Left, IFunctionAST Right) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Left.Evaluate(ctx) + Right.Evaluate(ctx);
}