using cslox.Models;

namespace cslox.Models;

public class Unary : Expr
{
    public Token Operator { get; }
    public Expr Right { get; }

    public Unary(Token oper, Expr right)
    {
        Operator = oper;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.visitUnaryExpr(this);
    }
}
