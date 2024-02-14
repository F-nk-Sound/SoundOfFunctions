namespace Lexer.Tokens;

/// <summary>
/// Represents names such as "x" and "foo"
/// </summary>
/// <param name="Text"></param>
/// <returns></returns>
public record Ident(string Text) : IToken;