using cslox.Models;
using static cslox.Models.Token;
using static cslox.Models.Token.TokenTypes;

namespace cslox.Services;

public class Parser
{
    private class ParseError : Exception { }

    private readonly List<Token> Tokens;
    private int Current = 0;

    public Parser(List<Token> tokens)
    {
        Tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        var statements = new List<Stmt>();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Stmt Statement()
    {
        if (Match(If)) return IfStatement();
        if (Match(Print)) return PrintStatement();
        if (Match(While)) return WhileStatement();
        if (Match(LeftBrace)) return new Stmt.Block(Block());
        return ExpressionStatement();
    }

    private Stmt IfStatement()
    {
        Consume(LeftParen, "Expect '(' after 'if'.");
        Expr condition = Expression();
        Consume(RightParen, "Expect ')' after if condition.");

        Stmt thenBranch = Statement();
        Stmt elseBranch = null;
        if (Match(Else))
        {
            elseBranch = Statement();
        }

        return new Stmt.If(condition, thenBranch, elseBranch);
    }

    private Stmt PrintStatement()
    {
        var value = Expression();
        Consume(Semicolon, "Expect ';' after value.");
        return new Stmt.Print(value);
    }

    private Stmt WhileStatement()
    {
        Consume(LeftParen, "Expect '(' after 'while'.");
        Expr condition = Expression();
        Consume(RightParen, "Expect ')' after condition.");
        Stmt body = Statement();

        return new Stmt.While(condition, body);
    }

    private Stmt VarDeclaration()
    {
        Token name = Consume(Identifier, "Expect variable name.");

        Expr initializer = null;
        if (Match(Equal))
        {
            initializer = Expression();
        }

        Consume(Semicolon, "Expect ';' after variable declaration.");
        return new Stmt.Var(name, initializer);
    }

    private Stmt ExpressionStatement()
    {
        var expr = Expression();
        Consume(Semicolon, "Expect ';' after expression.");
        return new Stmt.Expression(expr);
    }

    private List<Stmt> Block()
    {
        List<Stmt> statements = new List<Stmt>();

        while (!Check(RightBrace) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(RightBrace, "Expect '}' after block.");
        return statements;
    }

    private Expr Assignment()
    {
        Expr expr = Or();

        if (Match(Equal))
        {
            Token equals = Previous();
            Expr value = Assignment();

            if (expr.GetType() == typeof(Expr.Variable))
            {
                Token name = ((Expr.Variable)expr).Name;
                return new Expr.Assign(name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Expr Or()
    {
        Expr expr = And();

        while (Match(TokenTypes.Or))
        {
            Token oper = Previous();
            Expr right = And();
            expr = new Expr.Logical(expr, oper, right);
        }

        return expr;
    }

    private Expr And()
    {
        Expr expr = Equality();

        while (Match(TokenTypes.And))
        {
            Token oper = Previous();
            Expr right = Equality();
            expr = new Expr.Logical(expr, oper, right);
        }

        return expr;
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
        return Assignment();
    }

    private Stmt Declaration()
    {
        try
        {
            if (Match(Var)) return VarDeclaration();
            return Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    // private Expr Block()
    // {
    //     Expr expr = Ternary();
    //
    //     while (Match(Comma))
    //     {
    //         Token oper = Previous();
    //         Expr right = Ternary();
    //         expr = new Expr.Binary(expr, oper, right);
    //     }
    //
    //     return expr;
    // }

    // private Expr Ternary()
    // {
    //     Expr expr = Equality();
    //
    //     if (Match(Question))
    //     {
    //         Token oper = Previous();
    //         Expr ifTrue = Ternary();
    //         Token separator = Consume(Colon, "Expect ':' after expression.");
    //         Expr ifFalse = Ternary();
    //
    //         expr = new Expr.Ternary(expr, oper, ifTrue, separator, ifFalse);
    //     }
    //
    //     return expr;
    // }

    private Expr Equality()
    {
        Expr expr = Compare();

        while (Match(BangEqual, EqualEqual))
        {
            Token oper = Previous();
            Expr right = Compare();
            expr = new Expr.Binary(expr, oper, right);
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
            expr = new Expr.Binary(expr, oper, right);
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
            expr = new Expr.Binary(expr, oper, right);
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
            expr = new Expr.Binary(expr, oper, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(Bang, Minus))
        {
            Token oper = Previous();
            Expr right = Unary();
            return new Expr.Unary(oper, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(False)) { return new Expr.Literal(false); }
        if (Match(True)) { return new Expr.Literal(true); }
        if (Match(Nil)) { return new Expr.Literal(null); }

        if (Match(Number, TokenTypes.String)) { return new Expr.Literal(Previous().Literal); }

        if (Match(Identifier)) { return new Expr.Variable(Previous()); }

        if (Match(LeftParen))
        {
            var expr = Expression();
            Consume(RightParen, "Expect ')' after expression.");

            return new Expr.Grouping(expr);
        }

        if (Match(Plus, Minus, Star, Slash, EqualEqual,
            BangEqual, Greater, GreaterEqual, Less, LessEqual))
        {
            var oper = Previous();
            Equality();
            throw Error(oper, $"Expect expression before '{oper.Lexeme}'");
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
