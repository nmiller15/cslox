using cslox.Models;

namespace cslox.Models;

public abstract class Expr
{
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<T>
    {
        T visitBinaryExpr(Binary expr);
        T visitCallExpr(Call expr);
        T visitGetExpr(Get expr);
        T visitSetExpr(Set expr);
        T visitSuperExpr(Super expr);
        T visitThisExpr(This expr);
        T visitGroupingExpr(Grouping expr);
        T visitLiteralExpr(Literal expr);
        T visitLogicalExpr(Logical expr);
        T visitUnaryExpr(Unary expr);
        T visitTernaryExpr(Ternary expr);
        T visitVariableExpr(Variable expr);
        T visitAssignExpr(Assign expr);
    }

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

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitBinaryExpr(this);
        }
    }

    public class Call : Expr
    {
        public Expr Callee { get; }
        public Token Paren { get; }
        public List<Expr> Arguments { get; }

        public Call(Expr callee, Token paren, List<Expr> arguments)
        {
            Callee = callee;
            Paren = paren;
            Arguments = arguments;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitCallExpr(this);
        }
    }

    public class Get : Expr
    {
        public Expr oObject { get; }
        public Token Name { get; }

        public Get(Expr oobject, Token name)
        {
            oObject = oobject;
            Name = name;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitGetExpr(this);
        }
    }

    public class Set : Expr
    {
        public Expr oObject { get; }
        public Token Name { get; }
        public Expr Value { get; }

        public Set(Expr oobject, Token name, Expr value)
        {
            oObject = oobject;
            Name = name;
            Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitSetExpr(this);
        }
    }

    public class Super : Expr
    {
        public Token Keyword { get; }
        public Token Method { get; }

        public Super(Token keyword, Token method)
        {
            Keyword = keyword;
            Method = method;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitSuperExpr(this);
        }
    }

    public class This : Expr
    {
        public Token Keyword { get; }

        public This(Token keyword)
        {
            Keyword = keyword;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitThisExpr(this);
        }
    }

    public class Grouping : Expr
    {
        public Expr Expression { get; }

        public Grouping(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitGroupingExpr(this);
        }
    }

    public class Literal : Expr
    {
        public object Value { get; }

        public Literal(object value)
        {
            Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitLiteralExpr(this);
        }
    }

    public class Logical : Expr
    {
        public Expr Left { get; }
        public Token Operator { get; }
        public Expr Right { get; }

        public Logical(Expr left, Token oper, Expr right)
        {
            Left = left;
            Operator = oper;
            Right = right;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitLogicalExpr(this);
        }
    }

    public class Unary : Expr
    {
        public Token Operator { get; }
        public Expr Right { get; }

        public Unary(Token oper, Expr right)
        {
            Operator = oper;
            Right = right;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitUnaryExpr(this);
        }
    }

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

    public class Variable : Expr
    {
        public Token Name { get; }

        public Variable(Token name)
        {
            Name = name;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitVariableExpr(this);
        }
    }

    public class Assign : Expr
    {
        public Token Name { get; }
        public Expr Value { get; }

        public Assign(Token name, Expr value)
        {
            Name = name;
            Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.visitAssignExpr(this);
        }
    }


}
