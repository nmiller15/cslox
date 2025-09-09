using cslox.Models;
using static cslox.Models.Token.TokenTypes;

namespace cslox.Services;

public class Interpreter : IVisitor<object>
{
    public void Interpret(Expr expression)
    {
        try
        {
            object value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
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

    public object visitGroupingExpr(Grouping expr) => Evaluate(expr.Expression);

    public object visitLiteralExpr(Literal expr) => expr.Value;

    public object visitTernaryExpr(Ternary expr)
    {
        throw new NotImplementedException();
    }

    public object visitUnaryExpr(Unary expr)
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

    public object visitBinaryExpr(Binary expr)
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

    private object Evaluate(Expr expr) => expr.Accept(this);

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
}
