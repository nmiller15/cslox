using cslox.Models;
using cslox.Services;

namespace cslox.StandardLib.Math;

public class RandCallable : ILoxCallable
{
    public int Arity() => 0;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        var random = new System.Random();
        return random.NextDouble();
    }
}
