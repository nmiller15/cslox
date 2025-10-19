using cslox.Services;

namespace cslox.Models;

public class LoxFunction : ILoxCallable
{
    private readonly Stmt.Function _declaration;
    private readonly Environment _closure;

    public LoxFunction(Stmt.Function declaration, Environment closure)
    {
        _declaration = declaration;
        _closure = closure;
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
            return returnValue.Value;
        }
        return null;
    }

    public override string ToString() => "<fn " + _declaration.Name.Lexeme + ">";
}
