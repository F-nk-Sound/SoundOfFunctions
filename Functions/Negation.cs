namespace Functions;

public record Negation(IFunctionAST Inner) : IFunctionAST
{
    public bool IsTerm => true;

    public string Latex => $"-{Inner.Latex}";

    public double Evaluate(EvalContext ctx) => -Inner.Evaluate(ctx);
}