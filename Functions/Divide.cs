namespace Functions;

/// <summary>
/// Division in a function.
/// </summary>
public record Divide(IFunctionAST Left, IFunctionAST Right) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Left.Evaluate(ctx) / Right.Evaluate(ctx);
}