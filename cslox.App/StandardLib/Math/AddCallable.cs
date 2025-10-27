using cslox.Models;
using cslox.Services;

namespace cslox.StandardLib.Math;

public class AddCallable : ILoxCallable
{
    public int Arity() => 2;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return (double)arguments[0] + (double)arguments[1];
    }
}
