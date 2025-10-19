using cslox.Models;

namespace cslox.Models;

public abstract class Stmt
{
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<T>
    {
        T visitBlockStmt(Block stmt);
        T visitExpressionStmt(Expression stmt);
        T visitFunctionStmt(Function stmt);
        T visitIfStmt(If stmt);
        T visitPrintStmt(Print stmt);
        T visitReturnStmt(Return stmt);
        T visitVarStmt(Var stmt);
        T visitWhileStmt(While stmt);
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

    public class Function : Stmt
    {
        public Token Name { get; }
        public List<Token> Parameters { get; }
        public List<Stmt> Body { get; }

        public Function(Token name, List<Token> parameters, List<Stmt> body)
        {
            Name = name;
            Parameters = parameters;
            Body = body;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitFunctionStmt(this);
        }
    }

    public class If : Stmt
    {
        public Expr Condition { get; }
        public Stmt ThenBranch { get; }
        public Stmt ElseBranch { get; }

        public If(Expr condition, Stmt thenbranch, Stmt elsebranch)
        {
            Condition = condition;
            ThenBranch = thenbranch;
            ElseBranch = elsebranch;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitIfStmt(this);
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

    public class Return : Stmt
    {
        public Token Keyword { get; }
        public Expr Value { get; }

        public Return(Token keyword, Expr value)
        {
            Keyword = keyword;
            Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitReturnStmt(this);
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

    public class While : Stmt
    {
        public Expr Condition { get; }
        public Stmt Body { get; }

        public While(Expr condition, Stmt body)
        {
            Condition = condition;
            Body = body;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitWhileStmt(this);
        }
    }


}
