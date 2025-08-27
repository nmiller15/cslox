using cslox.Models;

namespace cslox.Models;

public class Ternary : Expr
{
    public Expr Condition { get; }
    public Token Operator { get; }
    public Expr IfTrue { get; }
    public Token Separator { get; }
    public Expr IfFalse { get; }

    public Ternary(Expr condition, Token oper, Expr iftrue, Token separator, Expr iffalse)
    {
        Condition = condition;
        Operator = oper;
        IfTrue = iftrue;
        Separator = separator;
        IfFalse = iffalse;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.visitTernaryExpr(this);
    }
}
