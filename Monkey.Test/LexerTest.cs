using System.Diagnostics;
using NuGet.Frameworks;

namespace Monkey.Test;

public class LexerTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestNextToken1()
    {
        const string input = "=+(){},;";
        var expected = new List<Token>
        {
            new Token(TokenType.Assign, "="),
            new Token(TokenType.Plus, "+"),
            new Token(TokenType.Lparan, "("),
            new Token(TokenType.Rparan, ")"),
            new Token(TokenType.Lbrace, "{"),
            new Token(TokenType.Rbrace, "}"),
            new Token(TokenType.Comma, ","),
            new Token(TokenType.Semicolon, ";")
        };

        var l = new Lexer(input);
        foreach (var t in expected)
        {
            var tok = l.NextToken();
            Assert.Multiple(() =>
            {
                Assert.That(t.Type, Is.EqualTo(tok.Type));
                Assert.That(t.Literal, Is.EqualTo(tok.Literal));
            });
        }

        Assert.Pass();
    }

    [Test]
    public void TestNextToken2()
    {
        const string input = """
                             let five = 5; 
                             let ten = 10;
                             let add = fn(x, y) {
                             x + y;
                             };
                             let result = add(five, ten);
                             !-/*5;
                             5 < 10 > 5;

                             if (5 < 10) {
                                return true;
                             } else {
                                return false;
                             }
                             10 == 10;
                             10 != 9;
                             """;
        var expected = new List<Token>
        {
            new Token(TokenType.Let, "let"),
            new Token(TokenType.Ident, "five"),
            new Token(TokenType.Assign, "="),
            new Token(TokenType.Int, "5"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.Let, "let"),
            new Token(TokenType.Ident, "ten"),
            new Token(TokenType.Assign, "="),
            new Token(TokenType.Int, "10"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.Let, "let"),
            new Token(TokenType.Ident, "add"),
            new Token(TokenType.Assign, "="),
            new Token(TokenType.Function, "fn"),
            new Token(TokenType.Lparan, "("),
            new Token(TokenType.Ident, "x"),
            new Token(TokenType.Comma, ","),
            new Token(TokenType.Ident, "y"),
            new Token(TokenType.Rparan, ")"),
            new Token(TokenType.Lbrace, "{"),
            new Token(TokenType.Ident, "x"),
            new Token(TokenType.Plus, "+"),
            new Token(TokenType.Ident, "y"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.Rbrace, "}"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.Let, "let"),
            new Token(TokenType.Ident, "result"),
            new Token(TokenType.Assign, "="),
            new Token(TokenType.Ident, "add"),
            new Token(TokenType.Lparan, "("),
            new Token(TokenType.Ident, "five"),
            new Token(TokenType.Comma, ","),
            new Token(TokenType.Ident, "ten"),
            new Token(TokenType.Rparan, ")"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.Bang, "!"),
            new Token(TokenType.Minus, "-"),
            new Token(TokenType.Slash, "/"),
            new Token(TokenType.Asterisk, "*"),
            new Token(TokenType.Int, "5"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.Int, "5"),
            new Token(TokenType.Lt, "<"),
            new Token(TokenType.Int, "10"),
            new Token(TokenType.Gt, ">"),
            new Token(TokenType.Int, "5"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.If, "if"),
            new Token(TokenType.Lparan, "("),
            new Token(TokenType.Int, "5"),
            new Token(TokenType.Lt, "<"),
            new Token(TokenType.Int, "10"),
            new Token(TokenType.Rparan, ")"),
            new Token(TokenType.Lbrace, "{"),
            new Token(TokenType.Return, "return"),
            new Token(TokenType.True, "true"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.Rbrace, "}"),
            new Token(TokenType.Else, "else"),
            new Token(TokenType.Lbrace, "{"),
            new Token(TokenType.Return, "return"),
            new Token(TokenType.False, "false"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.Rbrace, "}"),
            new Token(TokenType.Int, "10"),
            new Token(TokenType.Eq, "=="),
            new Token(TokenType.Int, "10"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.Int, "10"),
            new Token(TokenType.NotEq, "!="),
            new Token(TokenType.Int, "9"),
            new Token(TokenType.Semicolon, ";"),
            new Token(TokenType.Eof, ""),
        };

        var l = new Lexer(input);
        foreach (var t in expected)
        {
            var tok = l.NextToken();
            Assert.Multiple(() =>
            {
                Assert.That(tok.Type, Is.EqualTo(t.Type));
                Assert.That(tok.Literal, Is.EqualTo(t.Literal));
            });
        }

        Assert.Pass();
    }

    [Test]
    public void TestLetStatement()
    {
        const string input = """
                             let x = 5;
                             let y = 10;
                             let foobar = 838383;
                             """;

        Lexer l = new(input);
        Parser p = new(l);
        
        var program = p.ParseProgram();
        CheckParserErrors(p);
        Assert.That(program, Is.Not.Null);
        Assert.That(program.Statements.Count, Is.EqualTo(3));
        string[] tests = ["x", "y", "foobar"];
        foreach (var (name, i) in tests.Select((t, i) => (t, i)))
        {
            var s = program.Statements[i];
            Assert.That(s.TokenLiteral(), Is.EqualTo("let"));
            Assert.That(s, Is.InstanceOf(typeof(LetStatement)));
            var letStatement = (LetStatement)s;
            Assert.That(letStatement.Name.Value, Is.EqualTo(name));
            Assert.That(letStatement.Name.TokenLiteral(), Is.EqualTo(name));
        }
        Assert.Pass();
    }

    private static void CheckParserErrors(Parser p)
    {
        var errors = p.Errors();
        if (errors.Count == 0)
        {
            return;
        }

        Console.Error.WriteLine($"Parser has {errors.Count} errors");
        foreach (var message in errors)
        {
            Console.Error.WriteLine($"parser error: {message}");
        }
        Assert.Fail();
    }

    [Test]
    public void TestReturnStatement()
    {
        const string input = """
                             return 5;
                             return 10;
                             return 993322;
                             """;
        Lexer l = new(input);
        Parser p = new(l);
        var program = p.ParseProgram();
        CheckParserErrors(p);
        Assert.That(program.Statements.Count, Is.EqualTo(3));

        foreach (var statement in program.Statements)
        {
            Assert.That(statement, Is.InstanceOf(typeof(ReturnStatement)));
            var returnStatement = (ReturnStatement)statement;
            Assert.That(returnStatement.TokenLiteral(), Is.EqualTo("return"));
        }
    }

    [Test]
    public void TestIdentifierExpression()
    {
        const string input = "foobar";
        Lexer l = new(input);
        Parser p = new(l);
        var program = p.ParseProgram();
        CheckParserErrors(p);
        Assert.That(program.Statements, Has.Count.EqualTo(1));
        Assert.That(program.Statements[0], Is.InstanceOf(typeof(ExpressionStatement)));
        var expressionStatement = (ExpressionStatement) program.Statements[0];
        Assert.That(expressionStatement.Expression, Is.InstanceOf(typeof(Identifier)));
        var ident = (Identifier) expressionStatement.Expression;
        Assert.That(ident.Value, Is.EqualTo("foobar"));
        Assert.That(ident.TokenLiteral(), Is.EqualTo("foobar"));
    }
}