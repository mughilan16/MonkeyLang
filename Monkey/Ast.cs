using System.Linq.Expressions;

namespace Monkey;

public interface INode
{
    string TokenLiteral();
}

public interface IStatement: INode
{
    protected abstract void StatementNode();
    public new string TokenLiteral();
}

public interface IExpression: INode
{
    protected abstract void ExpressionNode();
    public new string TokenLiteral();
}

public class Identifier: INode
{
    public Token Token;
    public string Value;
    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

public class LetStatement: IStatement
{
    public Token Token { get; set; }
    public Identifier Name { get; set; }
    public IExpression Value { get; set; }

    public void StatementNode() { }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

public class ReturnStatement : IStatement
{
    public Token Token;
    public IExpression ReturnValue;
    public void StatementNode() {}
    
    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

public class Program: INode
{
    public List<IStatement> Statements = [];

    public string TokenLiteral()
    {
        return Statements.Count > 0 ? Statements[0].TokenLiteral() : "";
    }
}