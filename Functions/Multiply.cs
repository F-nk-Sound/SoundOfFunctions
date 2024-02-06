namespace Functions;

/// <summary>
/// Multiplication in a function.
/// </summary>
public record Multiply(IFunctionAST Left, IFunctionAST Right) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Left.Evaluate(ctx) * Right.Evaluate(ctx);
}