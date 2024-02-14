using System;
using System.Collections.Generic;
using Lexer.Tokens;

namespace Lexer;

public class Lexer 
{
    public List<IToken> Run(string input)
    {
        List<IToken> toks = new();
        ReadOnlySpan<char> stream = input;

        while (true)
        {
            if (ParseIdent(ref stream) is Ident ident)
                toks.Add(ident);
            
        }
    }

    Ident? ParseIdent(ref ReadOnlySpan<char> stream)
    {

    }
}