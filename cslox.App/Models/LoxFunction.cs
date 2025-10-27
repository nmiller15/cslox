using cslox.Services;

namespace cslox.Models;

public class LoxFunction : ILoxCallable
{
    private readonly Stmt.Function _declaration;
    private readonly Environment _closure;
    private readonly bool _isInitializer;

    public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
    {
        _declaration = declaration;
        _closure = closure;
        _isInitializer = isInitializer;
    }

    public LoxFunction Bind(LoxInstance instance)
    {
        var environment = new Environment(_closure);
        environment.Define("this", instance);
        return new LoxFunction(_declaration, environment, _isInitializer);
    }

    public int Arity() => _declaration.Parameters.Count;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        Environment environment = new Environment(_closure);
        for (int i = 0; i < _declaration.Parameters.Count; i++)
        {
            environment.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
        }

        try
        {
            interpreter.ExecuteBlock(_declaration.Body, environment);
        }
        catch (Return returnValue)
        {
            if (_isInitializer) return _closure.GetAt(0, "this");
            return returnValue.Value;
        }

        if (_isInitializer) return _closure.GetAt(0, "this");
        return null;
    }

    public override string ToString() => "<fn " + _declaration.Name.Lexeme + ">";
}
