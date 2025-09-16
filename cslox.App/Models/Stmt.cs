using cslox.Models;

namespace cslox.Models;

public abstract class Stmt
{
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<T>
    {
        T visitBlockStmt(Block stmt);
        T visitExpressionStmt(Expression stmt);
        T visitPrintStmt(Print stmt);
        T visitVarStmt(Var stmt);
    }

    public class Block : Stmt
    {
        public List<Stmt> Statements { get; }

        public Block(List<Stmt> statements)
        {
            Statements = statements;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitBlockStmt(this);
        }
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

    public class Var : Stmt
    {
        public Token Name { get; }
        public Expr Initializer { get; }

        public Var(Token name, Expr initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitVarStmt(this);
        }
    }


}
