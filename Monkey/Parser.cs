using static Monkey.TokenType;

namespace Monkey;

public class Parser
{
    private readonly Lexer _lexer;
    private Token _curToken;
    private Token _peekToken;
    private readonly List<string> _errors;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        _errors = [];
        _curToken = _lexer.NextToken();
        _peekToken = _lexer.NextToken();
    }

    private void NextToken()
    {
        _curToken = _peekToken;
        _peekToken = _lexer.NextToken();
    }

    private void PeekError(TokenType t)
    {
        var msg = $"expected next token to be {t}, got {_peekToken.Type} instead";
        _errors.Add(msg);
    }

    private bool ExceptPeek(TokenType t)
    {
        if (_peekToken.Type == t)
        {
            NextToken();
            return true;
        }
        else
        {
            PeekError(t);
            return false;
        }
    }

    private LetStatement? ParseLetStatement()
    {
        var statement = new LetStatement { Token = _curToken };
        if (!ExceptPeek(Ident))
        {
            return null;
        }

        statement.Name = new Identifier
        {
            Token = _curToken,
            Value = _curToken.Literal
        };

        if (!ExceptPeek(Assign))
        {
            return null;
        }

        while (_curToken.Type != (Semicolon))
        {
            NextToken();
        }

        return statement;
    }

    private ReturnStatement ParseReturnStatement()
    {
        var statement = new ReturnStatement
        {
            Token = _curToken
        };
        NextToken();
        while (_curToken.Type == Semicolon)
        {
            NextToken();
        }

        return statement;
    }

    private IStatement? ParseStatement()
    {
        return _curToken.Type switch
        {
            Let => ParseLetStatement(),
            Return => ParseReturnStatement(),
            _ => null
        };
    }

    public List<string> Errors()
    {
        return _errors;
    }
    
    public Program ParseProgram()
    {
        Program program = new();
        while (_curToken.Type != Eof)
        {
            var statement = ParseStatement();
            if (statement != null)
            {
                program.Statements.Add(statement);
            }
            NextToken();
        }
        return program;
    }
}