using cslox.Models;

namespace cslox.Models;

public abstract class Expr
{
    public abstract T Accept<T>(IVisitor<T> visitor);
}
