namespace Lexer.Tokens;

/// <summary>
/// Represents symbols like {, ), +, etc.
/// </summary>
/// <param name="Value"></param>
/// <returns></returns>
public record Symbol(string Value): IToken;