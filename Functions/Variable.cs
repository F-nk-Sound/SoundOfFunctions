namespace Functions;

/// <summary>
/// Named variable in a function. Should probably only ever be `x`.
/// </summary>
public record Variable(string Name) : IFunctionAST
{
    public double Evaluate(EvalContext ctx) => ctx.Variables[Name];
}