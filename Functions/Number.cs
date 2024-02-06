namespace Functions;

/// <summary>
/// A literal number in a function.
/// </summary>
public record Number(double Value) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => Value;
}