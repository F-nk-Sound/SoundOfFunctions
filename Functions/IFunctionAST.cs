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
}
