namespace Functions;

/// <summary>
/// Subtraction in a function.
/// </summary>
public record Subtract(IFunctionAST Left, IFunctionAST Right) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Left.Evaluate(ctx) - Right.Evaluate(ctx);
}