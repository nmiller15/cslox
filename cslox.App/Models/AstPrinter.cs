using System.Text;

namespace cslox.Models;

public class AstPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string visitBinaryExpr(Expr.Binary expr)
        => Parenthesize(expr.Operator.Lexeme,
                        expr.Left, expr.Right);

    public string visitGroupingExpr(Expr.Grouping expr)
        => Parenthesize("group", expr.Expression);

    public string visitLiteralExpr(Expr.Literal expr)
    {
        if (expr.Value == null) { return "nil"; }
        return expr.Value.ToString();
    }

    public string visitTernaryExpr(Expr.Ternary expr)
        => Parenthesize($"{expr.Operator.Lexeme}{expr.Separator.Lexeme}", expr.Condition, expr.IfTrue, expr.IfFalse);

    public string visitUnaryExpr(Expr.Unary expr)
        => Parenthesize(expr.Operator.Lexeme, expr.Right);

    public string visitVariableExpr(Expr.Variable expr)
    {
        throw new NotImplementedException();
    }

    public string visitAssignExpr(Expr.Assign expr)
    {
        throw new NotImplementedException();
    }

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

    public string visitLogicalExpr(Expr.Logical expr)
    {
        throw new NotImplementedException();
    }

    public string visitCallExpr(Expr.Call expr)
    {
        throw new NotImplementedException();
    }
}
