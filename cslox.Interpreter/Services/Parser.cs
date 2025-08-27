using cslox.Models;
using static cslox.Models.Token;
using static cslox.Models.Token.TokenTypes;

namespace cslox.Interpreter.Services;

public class Parser
{
    private class ParseError : Exception { }

    private readonly List<Token> Tokens;
    private int Current = 0;

    public Parser(List<Token> tokens)
    {
        Tokens = tokens;
    }

    public Expr Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError error)
        {
            return null;
        }
    }

    private bool Match(params TokenTypes[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private bool Check(TokenTypes type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) Current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenTypes.EOF;
    }

    private Token Peek()
    {
        return Tokens[Current];
    }

    private Token Previous()
    {
        return Tokens[Current - 1];
    }

    private Expr Expression()
    {
        return Block();
    }

    private Expr Block()
    {
        Expr expr = Equality();

        while (Match(Comma))
        {
            Token oper = Previous();
            Expr right = Equality();
            expr = new Binary(expr, oper, right);
        }

        return expr;
    }

    private Expr Equality()
    {
        Expr expr = Compare();

        while (Match(BangEqual, Equal))
        {
            Token oper = Previous();
            Expr right = Compare();
            expr = new Binary(expr, oper, right);
        }

        return expr;
    }

    private Expr Compare()
    {
        Expr expr = Term();

        while (Match(Greater, Less, GreaterEqual, LessEqual))
        {
            Token oper = Previous();
            Expr right = Term();
            expr = new Binary(expr, oper, right);
        }

        return expr;
    }

    private Expr Term()
    {
        Expr expr = Factor();

        while (Match(Plus, Minus))
        {
            Token oper = Previous();
            Expr right = Factor();
            expr = new Binary(expr, oper, right);
        }

        return expr;
    }

    private Expr Factor()
    {
        Expr expr = Unary();

        while (Match(Star, Slash))
        {
            Token oper = Previous();
            Expr right = Unary();
            expr = new Binary(expr, oper, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(Bang, Minus))
        {
            Token oper = Previous();
            Expr right = Unary();
            return new Unary(oper, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(False)) { return new Literal(false); }
        if (Match(True)) { return new Literal(true); }
        if (Match(Nil)) { return new Literal(null); }

        if (Match(Number, TokenTypes.String)) { return new Literal(Previous().Literal); }

        if (Match(LeftParen))
        {
            var expr = Expression();
            Consume(RightParen, "Expect ')' after expression.");

            return new Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private Token Consume(TokenTypes type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message)
    {
        Lox.Error(token, message);
        return new ParseError();
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == Semicolon) return;

            switch (Peek().Type)
            {
                case Class:
                case Fun:
                case Var:
                case For:
                case If:
                case While:
                case Print:
                case Return:
                    return;
            }

            Advance();
        }
    }

}
