namespace cslox.Models;

public class Token
{
    public enum TokenTypes
    {
        // Single-character tokens.
        LeftParen, RightParen, LeftBrace, RightBrace, Comma, Dot, Minus,
        Plus, Semicolon, Slash, Star,

        // One or two character tokens.
        Bang, BangEqual, Equal, EqualsEqual, Greater, GreaterEqual,
        Less, LessEqual,

        // Literals.
        Identifier, String, Number,

        // Keywords.
        And, Class, Else, False, Fun, For, If, Nil, Or, Print,
        Return, Super, This, True, Var, While
    }

    public TokenTypes Type { get; }
    public string Lexeme { get; }
    public object Literal { get; }
    public int Line { get; }

    public Token(TokenTypes type, string lexeme, object literal, int line)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
    }

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}

