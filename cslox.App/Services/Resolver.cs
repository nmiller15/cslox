using cslox.Models;

namespace cslox.Services;

public class Resolver : Expr.IVisitor<Nothing>, Stmt.IVisitor<Nothing>
{
    private enum FunctionType
    {
        None,
        Function,
        Initializer,
        Method
    }

    private enum ClassType
    {
        None,
        Class,
        Subclass
    }

    private readonly Interpreter _interpreter;
    private readonly Stack<Dictionary<string, bool>> _scopes = new();
    private FunctionType CurrentFunction = FunctionType.None;
    private ClassType CurrentClass = ClassType.None;

    public Resolver(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }

    public void Resolve(List<Stmt> statements)
    {
        foreach (var statement in statements)
        {
            Resolve(statement);
        }
    }

    private void Resolve(Stmt stmt)
    {
        stmt.Accept(this);
    }

    private void Resolve(Expr expr)
    {
        expr.Accept(this);
    }

    private void ResolveFunction(Stmt.Function function, FunctionType type)
    {
        FunctionType enclosingFunction = CurrentFunction;
        CurrentFunction = type;

        BeginScope();
        foreach (Token param in function.Parameters)
        {
            Declare(param);
            Define(param);
        }
        Resolve(function.Body);
        EndScope();

        CurrentFunction = enclosingFunction;
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope()
    {
        _scopes.Pop();
    }

    private void Declare(Token name)
    {
        if (_scopes.Count == 0) return;

        var scope = _scopes.Peek();
        if (scope.ContainsKey(name.Lexeme))
        {
            Lox.Error(name, "Already a variable with this name in this scope.");
        }
        scope[name.Lexeme] = false;
    }

    private void Define(Token name)
    {
        if (_scopes.Count == 0) return;
        _scopes.Peek()[name.Lexeme] = true;
    }

    private void ResolveLocal(Expr expr, Token name)
    {
        for (int i = 0; i < _scopes.Count; i++)
        {
            if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
            {
                _interpreter.Resolve(expr, i);
                return;
            }
        }
    }

    public Nothing visitBlockStmt(Stmt.Block stmt)
    {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return default;
    }

    public Nothing visitClassStmt(Stmt.Class stmt)
    {
        var enclosingClass = CurrentClass;
        CurrentClass = ClassType.Class;

        Declare(stmt.Name);
        Define(stmt.Name);

        if (stmt.Superclass != null &&
                stmt.Name.Lexeme.Equals(stmt.Superclass.Name.Lexeme))
        {
            Lox.Error(stmt.Superclass.Name,
                    "A class can't inherit from itself.");
        }

        if (stmt.Superclass != null)
        {
            CurrentClass = ClassType.Subclass;
            Resolve(stmt.Superclass);
        }

        if (stmt.Superclass != null)
        {
            BeginScope();
            _scopes.Peek().Add("super", true);
        }

        BeginScope();
        _scopes.Peek().Add("this", true);

        foreach (var method in stmt.Methods)
        {
            var declaration = FunctionType.Method;
            if (method.Name.Lexeme.Equals("init"))
            {
                declaration = FunctionType.Initializer;
            }
            ResolveFunction(method, declaration);
        }

        EndScope();

        if (stmt.Superclass != null) EndScope();

        CurrentClass = enclosingClass;
        return default;
    }

    public Nothing visitExpressionStmt(Stmt.Expression stmt)
    {
        Resolve(stmt.eExpression);
        return default;
    }

    public Nothing visitIfStmt(Stmt.If stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.ThenBranch);
        if (stmt.ElseBranch != null) { Resolve(stmt.ElseBranch); }
        return default;
    }

    public Nothing visitPrintStmt(Stmt.Print stmt)
    {
        Resolve(stmt.Expression);
        return default;
    }

    public Nothing visitReturnStmt(Stmt.Return stmt)
    {
        if (CurrentFunction == FunctionType.None)
        {
            Lox.Error(stmt.Keyword, "Can't return from top-level code.");
        }
        if (stmt.Value != null)
        {
            if (CurrentFunction == FunctionType.Initializer)
                Lox.Error(stmt.Keyword, "Can't return a value from an initializer.");
            Resolve(stmt.Value);
        }

        return default;
    }

    public Nothing visitWhileStmt(Stmt.While stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.Body);
        return default;
    }

    public Nothing visitFunctionStmt(Stmt.Function stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);

        ResolveFunction(stmt, FunctionType.Function);
        return default;
    }

    public Nothing visitVarStmt(Stmt.Var stmt)
    {
        Declare(stmt.Name);
        if (stmt.Initializer != null)
        {
            Resolve(stmt.Initializer);
        }
        Define(stmt.Name);
        return default;
    }

    public Nothing visitAssignExpr(Expr.Assign expr)
    {
        Resolve(expr.Value);
        ResolveLocal(expr, expr.Name);
        return default;
    }

    public Nothing visitVariableExpr(Expr.Variable expr)
    {
        if (_scopes.Count > 0 && _scopes.Peek().TryGetValue(expr.Name.Lexeme, out var value) && value == false)
        {
            Lox.Error(expr.Name, "Cannot read local variable in its own initializer.");
        }

        ResolveLocal(expr, expr.Name);
        return default;
    }

    public Nothing visitBinaryExpr(Expr.Binary expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return default;
    }

    public Nothing visitCallExpr(Expr.Call expr)
    {
        Resolve(expr.Callee);

        foreach (Expr argument in expr.Arguments)
        {
            Resolve(argument);
        }

        return default;
    }

    public Nothing visitGetExpr(Expr.Get expr)
    {
        Resolve(expr.oObject);
        return default;
    }

    public Nothing visitGroupingExpr(Expr.Grouping expr)
    {
        Resolve(expr.Expression);
        return default;
    }

    public Nothing visitLiteralExpr(Expr.Literal expr)
    {
        return default;
    }

    public Nothing visitLogicalExpr(Expr.Logical expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return default;
    }

    public Nothing visitSetExpr(Expr.Set expr)
    {
        Resolve(expr.Value);
        Resolve(expr.oObject);
        return default;
    }

    public Nothing visitSuperExpr(Expr.Super expr)
    {
        if (CurrentClass == ClassType.None)
        {
            Lox.Error(expr.Keyword,
                    "Can't use 'super' outside of a class.");
        }
        else if (CurrentClass != ClassType.Subclass)
        {
            Lox.Error(expr.Keyword,
                    "Can't use 'super' in a class with no superclass.");
        }
        ResolveLocal(expr, expr.Keyword);
        return default;
    }

    public Nothing visitThisExpr(Expr.This expr)
    {
        if (CurrentClass == ClassType.None)
        {
            Lox.Error(expr.Keyword,
                    "Can't use 'this' outside of a class.");
            return default;
        }
        ResolveLocal(expr, expr.Keyword);
        return default;
    }

    public Nothing visitUnaryExpr(Expr.Unary expr)
    {
        Resolve(expr.Right);
        return default;
    }

    public Nothing visitTernaryExpr(Expr.Ternary expr)
    {
        throw new NotImplementedException();
    }
}
