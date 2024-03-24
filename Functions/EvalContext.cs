using System.Collections.Generic;

namespace Functions;

/// <summary>
/// Context useful for evaluating a function's AST. 
/// Contains:
/// - The value of all variables at this evaluation
/// </summary>
public class EvalContext
{
    public Dictionary<string, double>? Variables { get; set; }
}