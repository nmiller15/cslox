namespace cslox.Models;

public class Environment
{
    public Environment Enclosing;
    private readonly Dictionary<string, object> _values = new();

    public Environment()
    {
        Enclosing = null;
    }

    public Environment(Environment enclosing)
    {
        Enclosing = enclosing;
    }

    public void Define(string name, object value)
    {
        _values[name] = value;
    }

    public Environment Ancestor(int distance)
    {
        var environment = this;
        for (int i = 0; i < distance; i++)
        {
            environment = environment.Enclosing;
        }

        return environment;
    }

    public object GetAt(int distance, string name)
    {
        // Console.WriteLine($"GetAt distance: {distance}, name: {name}");
        // if (name == "this")
        // {
        //     foreach (var kvp in Ancestor(distance)._values)
        //     {
        //         Console.WriteLine("Distance: " + distance);
        //         Console.WriteLine(" > Enclosing key: " + kvp.Key + " value: " + kvp.Value);
        //     }
        //     foreach (var kvp in Ancestor(1)._values)
        //     {
        //         Console.WriteLine("Distance: " + 1);
        //         Console.WriteLine(" > Enclosing key: " + kvp.Key + " value: " + kvp.Value);
        //     }
        // }
        return Ancestor(distance)._values[name];
    }

    public void AssignAt(int distance, Token name, object value)
    {
        Ancestor(distance)._values[name.Lexeme] = value;
    }


    public object Get(Token name)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            return _values[name.Lexeme];
        }

        if (Enclosing != null)
        {
            return Enclosing.Get(name);
        }

        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }


    public void Assign(Token name, object value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }

        if (Enclosing != null)
        {
            Enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }
}

