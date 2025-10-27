using cslox.Models;
using cslox.Services;

namespace cslox.StandardLib;

public class ClockCallable : ILoxCallable
{
    public int Arity() => 0;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
    }

    public override string ToString() => "<native fn clock>";
}
