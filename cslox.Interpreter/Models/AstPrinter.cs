using System.Text;

namespace cslox.Models;

public class AstPrinter : IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string visitBinaryExpr(Binary expr)
        => Parenthesize(expr.Operator.Lexeme,
                        expr.Left, expr.Right);

    public string visitGroupingExpr(Grouping expr)
        => Parenthesize("group", expr.Expression);

    public string visitLiteralExpr(Literal expr)
    {
        if (expr.Value == null) { return "nil"; }
        return expr.Value.ToString();
    }

    public string visitUnaryExpr(Unary expr)
        => Parenthesize(expr.Operator.Lexeme, expr.Right);

    private string Parenthesize(string name, params Expr[] exprs)
    {
        var builder = new StringBuilder();
        builder
            .Append("(")
            .Append(name);

        foreach (var expr in exprs)
        {
            builder
                .Append(" ")
                .Append(expr.Accept(this));
        }

        builder.Append(")");

        return builder.ToString();
    }
}
