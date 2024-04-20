using System;

namespace Functions;

public record Log(IFunctionAST Base, IFunctionAST Antilog) : IFunctionAST
{
    public bool IsTerm => true;

    public string Latex => $"\\log_{{{Base.Latex}}}({Antilog.Latex})";

    public double Evaluate(EvalContext ctx) => Math.Log(Antilog.Evaluate(ctx), Base.Evaluate(ctx));
}