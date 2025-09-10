using cslox.Models;

namespace cslox.Models;

public abstract class Stmt
{
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<T>
    {
        T visitExpressionStmt(Expression stmt);
        T visitPrintStmt(Print stmt);
    }

    public class Expression : Stmt
    {
        public Expr eExpression { get; }

        public Expression(Expr eexpression)
        {
            eExpression = eexpression;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitExpressionStmt(this);
        }
    }

    public class Print : Stmt
    {
        public Expr Expression { get; }

        public Print(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitPrintStmt(this);
        }
    }


}
