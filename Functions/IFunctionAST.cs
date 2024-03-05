namespace Functions;

/// <summary>
/// Interface for representing abstract syntax tree nodes for functions.
/// </summary>
public interface IFunctionAST
{
    /// <summary>
    /// Get the value of this node of the AST.
    /// This effectively "interprets" the AST, producing a final value when the top node is 
    /// evaluated; it is also a recursive algorithm. 
    /// 
    /// May throw arbitrary exceptions depending on the AST's value. May return NaN, for example,
    /// if a divide by 0 occurs.
    /// 
    /// The supplied evaluation context should contain a value for every variable that is expected
    /// to be referenced. 
    /// </summary>
    /// <param name="ctx">The environment to evaluate this function in.</param>
    /// <returns>The value of this node.</returns>
    public double Evaluate(EvalContext ctx);

    /// <summary>
    /// Calls `Evaluate()` where the evaluation context is only aware of the variable `t`.
    /// 
    /// May throw arbitrary exceptions depending on the AST's value. May return NaN, for example,
    /// if a divide by 0 occurs.
    /// </summary>
    /// <param name="t">The value of `t`.</param>
    /// <returns>The value of this node.</returns>
    public double EvaluateAtT(double t) => Evaluate(new() { Variables = new() { { "t", t } } });

    /// <summary>
    /// If this node is a single term (such as `3xy`) or multiple terms (such as `3 + xy`).
    /// 
    /// True means this is a single term.
    /// </summary>
    public bool IsTerm { get; }

    /// <summary>
    /// If this node is a single term, returns it. If it is multiple terms, wraps it in parenthesis.
    /// See `IFunctionAST.IsTerm`.
    /// </summary>
    /// <value></value>
    public string SingleTermLatex => IsTerm switch
    {
        true => Latex,
        false => $"({Latex})",
    };

    /// <summary>
    /// Gets the Latex representing this function AST.
    /// </summary>
    public string Latex { get; }
}