using cslox.Models;
using cslox.Services;

namespace cslox.StandardLib.Math;

public class CeilCallable : ILoxCallable
{
    public int Arity() => 1;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return System.Math.Ceiling((double)arguments[0]);
    }
}

