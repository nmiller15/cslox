using cslox.Models;

namespace cslox.Models;

public class Literal : Expr
{
    public object Value { get; }

    public Literal(object value)
    {
        Value = value;
    }
}
