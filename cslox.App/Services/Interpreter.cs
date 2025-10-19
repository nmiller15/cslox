using cslox.Models;
using static cslox.Models.Token.TokenTypes;
using Environment = cslox.Models.Environment;
using Return = cslox.Models.Return;

namespace cslox.Services;

public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<Nothing>
{
    public Environment _globals = new();
    private Environment _environment;

    public Interpreter()
    {
        _environment = _globals;

        _globals.Define("clock", new ClockCallable());
    }

    private class ClockCallable : ILoxCallable
    {
        public int Arity() => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
        }

        public override string ToString() => "<native fn>";
    }

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            Lox.RuntimeError(error);
        }
    }

    private string Stringify(object obj)
    {
        if (obj == null) return "nil";
        if (obj.GetType() == typeof(double))
        {
            var text = obj.ToString();
            if (text!.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        return obj.ToString()!;
    }

    public Nothing visitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.eExpression);
        return default;
    }

    public Nothing visitFunctionStmt(Stmt.Function stmt)
    {
        LoxFunction function = new LoxFunction(stmt, _environment);
        _environment.Define(stmt.Name.Lexeme, function);
        return default;
    }

    public Nothing visitIfStmt(Stmt.If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenBranch);
        }
        else if (stmt.ElseBranch != null)
        {
            Execute(stmt.ElseBranch);
        }
        return default;
    }

    public Nothing visitPrintStmt(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Expression);
        Console.WriteLine(Stringify(value));
        return default;
    }

    public Nothing visitReturnStmt(Stmt.Return stmt)
    {
        object value = null;
        if (stmt.Value != null) value = Evaluate(stmt.Value);

        throw new Return(value);
    }

    public Nothing visitVarStmt(Stmt.Var stmt)
    {
        object value = null;
        if (stmt.Initializer != null)
        {
            value = Evaluate(stmt.Initializer);
        }
        _environment.Define(stmt.Name.Lexeme, value);
        return new Nothing();
    }

    public Nothing visitWhileStmt(Stmt.While stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.Body);
        }
        return new Nothing();
    }

    public object visitAssignExpr(Expr.Assign expr)
    {
        object value = Evaluate(expr.Value);
        _environment.Assign(expr.Name, value);
        return value;
    }

    public object visitGroupingExpr(Expr.Grouping expr) => Evaluate(expr.Expression);

    public object visitLiteralExpr(Expr.Literal expr) => expr.Value;

    public object visitLogicalExpr(Expr.Logical expr)
    {
        object left = Evaluate(expr.Left);

        if (expr.Operator.Type == Or)
        {
            if (IsTruthy(left)) { return left; }
        }
        else
        {
            if (!IsTruthy(left)) { return left; }
        }

        return Evaluate(expr.Right);
    }

    public object visitTernaryExpr(Expr.Ternary expr)
    {
        throw new NotImplementedException();
    }

    public object visitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case Bang:
                return IsTruthy(right);
            case Minus:
                CheckNumberOperand(expr.Operator, right);
                return -(double)right;
        }

        return null;
    }

    public object visitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case (Greater):
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;
            case (GreaterEqual):
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;
            case (Less):
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;
            case (LessEqual):
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;
            case (BangEqual):
                return !IsEqual(left, right);
            case (EqualEqual):
                return IsEqual(left, right);
            case (Minus):
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;
            case (Plus):
                if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
                {
                    return (double)left + (double)right;
                }
                if (left.GetType() == typeof(string) && right.GetType() == typeof(string))
                {
                    return (string)left + (string)right;
                }
                if (left.GetType() == typeof(string) || right.GetType() == typeof(string))
                {
                    return $"{left}{right}";
                }
                throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings.");
            case (Star):
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
            case (Slash):
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left / (double)right;
        }

        return null;
    }

    public object visitCallExpr(Expr.Call expr)
    {
        object callee = Evaluate(expr.Callee);

        List<object> arguments = new();
        foreach (Expr argument in expr.Arguments)
        {
            arguments.Add(Evaluate(argument));
        }

        if (callee is not ILoxCallable)
        {
            throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
        }

        ILoxCallable function = (ILoxCallable)callee;
        if (arguments.Count != function.Arity())
        {
            throw new RuntimeError(expr.Paren, "Expected " +
                    function.Arity() + " argument but got " +
                    arguments.Count + ".");
        }

        return function.Call(this, arguments);
    }

    private object Evaluate(Expr expr) => expr.Accept(this);

    private void Execute(Stmt stmt) => stmt.Accept(this);

    private bool IsTruthy(Object obj)
    {
        if (obj == null) return false;
        if (obj is bool b) return b;
        return true;
    }

    private bool IsEqual(object a, object b)
    {
        if (a == null && b == null) { return true; }
        if (a == null) { return false; }

        return a.Equals(b);
    }

    private void CheckNumberOperand(Token oper, object operand)
    {
        if (operand.GetType() == typeof(double)) return;
        throw new RuntimeError(oper, "Operand must be a number.");
    }

    private void CheckNumberOperands(Token oper, object left, object right)
    {
        if (left.GetType() == typeof(double) && right.GetType() == typeof(double)) return;
        throw new RuntimeError(oper, "Operands must be numbers.");
    }

    public object visitVariableExpr(Expr.Variable expr)
    {
        return _environment.Get(expr.Name);
    }

    public Nothing visitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(_environment));
        return new Nothing();
    }

    public void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        Environment previous = _environment;

        try
        {
            _environment = environment;
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = previous;
        }
    }
}
