using cslox.Models;
using cslox.Services;

namespace cslox.StandardLib.Math;

public class StrdoubCallable : ILoxCallable
{
    public int Arity() => 1;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        if (double.TryParse(arguments[0].ToString(), out var d))
        {
            return d;
        }
        return 0;
    }
}
