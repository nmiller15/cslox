using cslox.Models;

namespace cslox.Models;

public class Grouping : Expr
{
    public Expr Expression { get; }

    public Grouping(Expr expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.visitGroupingExpr(this);
    }
}
