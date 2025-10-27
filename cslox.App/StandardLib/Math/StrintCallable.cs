using cslox.Models;
using cslox.Services;

namespace cslox.StandardLib.Math;

public class StrintCallable : ILoxCallable
{
    public int Arity() => 1;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        if (int.TryParse(arguments[0].ToString(), out var i))
        {
            return i;
        }
        return 0;
    }
}

