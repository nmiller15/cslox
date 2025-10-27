using cslox.Models;
using cslox.Services;

namespace cslox.StandardLib.Math;

public class RoundCallable : ILoxCallable
{
    public int Arity() => 1;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return System.Math.Round((double)arguments[0]);
    }
}
