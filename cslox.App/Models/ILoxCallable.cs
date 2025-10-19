using cslox.Services;

namespace cslox.Models;

public interface ILoxCallable
{
    int Arity();
    object Call(Interpreter interpreter, List<object> arguments);
}

