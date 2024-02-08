using System;

namespace Functions;

/// <summary>
/// Named variable in a function. Should probably only ever be `x`.
/// </summary>
public record Variable(string Name) : IFunctionAST
{
    public double Evaluate(EvalContext ctx)
    {
        if (ctx.Variables.TryGetValue(Name, out double value))
            return value;
        else
            throw new UnboundVariableException() { Variable = Name };
    }
}

/// <summary>
/// Thrown when a variable in a function AST is evaluated without that variable being present in the 
/// evaluation context. Make sure to add all relevant variables to the evaluation context before
/// calling `IFunctionAST.Evaluate`.
/// </summary>
public class UnboundVariableException : Exception
{
    public string Variable { get; init; }

    public UnboundVariableException() : base() { }

    public UnboundVariableException(string message) : base(message) { }

    public UnboundVariableException(string message, Exception inner) : base(message, inner) { }
}