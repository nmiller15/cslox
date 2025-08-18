using cslox.Models;

namespace cslox.Models;

public class Unary : Expr
{
    public Token Operator { get; }
    public Expr Expression { get; }

    public Unary(Token oper, Expr expression)
    {
        Operator = oper;
        Expression = expression;
    }
}
