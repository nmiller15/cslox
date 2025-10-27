using cslox.Models;
using cslox.Services;

namespace cslox.StandardLib;

public class ReadCallable : ILoxCallable
{
    public int Arity()
    {
        return 1;
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        Console.Write(arguments[0]);
        return Console.ReadLine();
    }

    public override string ToString()
    {
        return "<native fn read>";
    }
}
