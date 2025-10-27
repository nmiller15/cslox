namespace cslox.Models;

public class LoxInstance
{
    private LoxClass _klass;
    private readonly Dictionary<string, object> _fields = new();

    public LoxInstance(LoxClass klass)
    {
        _klass = klass;
    }

    public object Get(Token name)
    {
        if (_fields.ContainsKey(name.Lexeme))
        {
            return _fields[name.Lexeme];
        }

        var method = _klass.FindMethod(name.Lexeme);
        if (method != null) return method.Bind(this);

        throw new RuntimeError(name, "Undefined property '" + name.Lexeme + "'.");
    }

    public void Set(Token name, object value)
    {
        _fields[name.Lexeme] = value;
    }

    public override string ToString()
    {
        return _klass.Name + " instance";
    }
}
