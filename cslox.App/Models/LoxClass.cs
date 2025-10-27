using cslox.Services;

namespace cslox.Models;

public class LoxClass : ILoxCallable
{
    public string Name { get; }
    private readonly Dictionary<string, LoxFunction> _methods;
    private readonly LoxClass _superclass;

    public LoxClass(string name, LoxClass superclass, Dictionary<string, LoxFunction> methods)
    {
        Name = name;
        _methods = methods;
        _superclass = superclass;
    }

    public void AddMethod(string name, LoxFunction method)
    {
        _methods[name] = method;
    }

    public LoxFunction FindMethod(string name)
    {
        if (_methods.ContainsKey(name))
        {
            return _methods[name];
        }

        if (_superclass != null)
        {
            return _superclass.FindMethod(name);
        }

        return null;
    }

    public override string ToString()
    {
        return Name;
    }

    public int Arity()
    {
        var initializer = FindMethod("init");
        if (initializer == null) return 0;
        return initializer.Arity();
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        var instance = new LoxInstance(this);
        var initializer = FindMethod("init");
        if (initializer != null)
        {
            initializer.Bind(instance).Call(interpreter, arguments);
        }
        return instance;
    }
}
