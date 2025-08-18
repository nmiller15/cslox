using cslox.Models;

namespace cslox.Models;

public class Binary : Expr
{
    public Expr Left { get; }
    public Token Operator { get; }
    public Expr Right { get; }

    public Binary(Expr left, Token oper, Expr right)
    {
        Left = left;
        Operator = oper;
        Right = right;
    }
}
