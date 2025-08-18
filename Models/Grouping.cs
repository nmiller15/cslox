using cslox.Models;

namespace cslox.Models;

public class Grouping : Expr
{
    public Expr Expression { get; }

    public Grouping(Expr expression)
    {
        Expression = expression;
    }
}
