using cslox.Models;
using cslox.Services;

namespace cslox.StandardLib.Math;

public class FloorCallable : ILoxCallable
{
    public int Arity() => 1;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return System.Math.Floor((double)arguments[0]);
    }
}
