using cslox.Models;

namespace cslox.Models;

public class Literal : Expr
{
    public object Value { get; }

    public Literal(object value)
    {
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.visitLiteralExpr(this);
    }
}
