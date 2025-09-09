using cslox.Models;
using static cslox.Models.Token;
using static cslox.Models.Token.TokenTypes;

namespace cslox.Services;

public class Scanner
{
    private static Dictionary<string, TokenTypes> Keywords = new Dictionary<string, TokenTypes>(StringComparer.OrdinalIgnoreCase)
    {
        { "and", And },
        { "class", Class },
        { "else", Else },
        { "false", False },
        { "for", For },
        { "fun", Fun },
        { "if", If },
        { "nil", Nil },
        { "or", Or },
        { "print", Print },
        { "return", Return },
        { "super", Super },
        { "this", This },
        { "true", True },
        { "var", Var },
        { "while", While }
    };

    private string Source { get; set; }
    private List<Token> Tokens = new List<Token>();
    private int Start = 0;
    private int Current = 0;
    private int Line = 1;
    private bool IsAtEnd => Current >= Source.Length;


    public Scanner(string source)
    {
        Source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd)
        {
            Start = Current;
            ScanToken();
        }

        Tokens.Add(new Token(EOF, "", null, Line));
        return Tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(LeftParen); break;
            case ')': AddToken(RightParen); break;
            case '{': AddToken(LeftBrace); break;
            case '}': AddToken(RightBrace); break;
            case ',': AddToken(Comma); break;
            case '.': AddToken(Dot); break;
            case '-': AddToken(Minus); break;
            case '+': AddToken(Plus); break;
            case ';': AddToken(Semicolon); break;
            case '*': AddToken(Star); break;
            case '?': AddToken(Question); break;
            case ':': AddToken(Colon); break;
            case '!':
                AddToken(Match('=') ? BangEqual : Bang);
                break;
            case '=':
                AddToken(Match('=') ? EqualEqual : Equal);
                break;
            case '<':
                AddToken(Match('=') ? LessEqual : Less);
                break;
            case '>':
                AddToken(Match('=') ? GreaterEqual : Greater);
                break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd) { Advance(); }
                }
                else if (Match('*'))
                {
                    // Handle multi-line comments
                    while (Peek() != '*' && PeekNext() != '/')
                    {
                        if (Peek() == '\n') Line++;
                        Advance(); Advance();
                    }
                    Advance();
                }
                else
                {
                    AddToken(Slash);
                }
                break;
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                Line++;
                break;
            case '"':
                HandleString();
                break;
            default:
                if (char.IsDigit(c))
                {
                    HandleNumber();
                }
                else if (char.IsLetter(c))
                {
                    HandleIdentifier();
                }
                else
                {
                    Lox.Error(Line, $"Unexpected character '{c}'");
                }
                break;
        }
    }

    private char Advance()
    {
        return Source[Current++];
    }

    private void AddToken(Token.TokenTypes type)
    {
        AddToken(type, null);
    }

    private void AddToken(Token.TokenTypes type, object? literal)
    {
        string text = Source.Substring(Start, Current - Start);
        Tokens.Add(new Token(type, text, literal, Line));
    }

    private bool Match(char expected)
    {
        if (IsAtEnd) return false;
        if (Source[Current] != expected) return false;

        Current++;
        return true;
    }

    private char Peek()
    {
        if (IsAtEnd) return '\0';
        return Source[Current];
    }

    private void HandleString()
    {
        while (Peek() != '"' && !IsAtEnd)
        {
            if (Peek() == '\n') Line++;
            Advance();
        }

        if (IsAtEnd)
        {
            Lox.Error(Line, "Unterminated string.");
            return;
        }

        Advance();

        var value = Source.Substring(Start + 1, (Current - 1) - (Start + 1));
        AddToken(TokenTypes.String, value);
    }

    private void HandleNumber()
    {
        while (char.IsDigit(Peek()))
        {
            Advance();
        }

        if (Peek() == '.' && char.IsDigit(PeekNext()))
        {
            Advance();
            while (char.IsDigit(Peek())) Advance();
        }

        if (double.TryParse(Source.Substring(Start, (Current - Start)), out var value))
        {
            AddToken(Number, value);
        }
        else
        {
            Environment.Exit(1);
        }
    }

    private char PeekNext()
    {
        if (Current + 1 >= Source.Length)
        {
            return '\0';
        }
        return Source[Current + 1];
    }

    private void HandleIdentifier()
    {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }

        string text = Source.Substring(Start, (Current - Start));
        TokenTypes type = Identifier;

        if (Keywords.TryGetValue(text, out var parsedType))
        {
            type = parsedType;
        }

        AddToken(type);
    }

    private bool IsAlphaNumeric(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }
}

