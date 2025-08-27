using cslox.Models;

namespace cslox.Models;

public interface IVisitor<T>
{
    T visitBinaryExpr(Binary expr);
    T visitGroupingExpr(Grouping expr);
    T visitLiteralExpr(Literal expr);
    T visitUnaryExpr(Unary expr);
    T visitTernaryExpr(Ternary expr);
}
