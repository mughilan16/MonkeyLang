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
            new(TokenType.Assign, "="),
            new(TokenType.Plus, "+"),
            new(TokenType.Lparan, "("),
            new(TokenType.Rparan, ")"),
            new(TokenType.Lbrace, "{"),
            new(TokenType.Rbrace, "}"),
            new(TokenType.Comma, ","),
            new(TokenType.Semicolon, ";")
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
            new(TokenType.Let, "let"),
            new(TokenType.Ident, "five"),
            new(TokenType.Assign, "="),
            new(TokenType.Int, "5"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Let, "let"),
            new(TokenType.Ident, "ten"),
            new(TokenType.Assign, "="),
            new(TokenType.Int, "10"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Let, "let"),
            new(TokenType.Ident, "add"),
            new(TokenType.Assign, "="),
            new(TokenType.Function, "fn"),
            new(TokenType.Lparan, "("),
            new(TokenType.Ident, "x"),
            new(TokenType.Comma, ","),
            new(TokenType.Ident, "y"),
            new(TokenType.Rparan, ")"),
            new(TokenType.Lbrace, "{"),
            new(TokenType.Ident, "x"),
            new(TokenType.Plus, "+"),
            new(TokenType.Ident, "y"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Rbrace, "}"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Let, "let"),
            new(TokenType.Ident, "result"),
            new(TokenType.Assign, "="),
            new(TokenType.Ident, "add"),
            new(TokenType.Lparan, "("),
            new(TokenType.Ident, "five"),
            new(TokenType.Comma, ","),
            new(TokenType.Ident, "ten"),
            new(TokenType.Rparan, ")"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Bang, "!"),
            new(TokenType.Minus, "-"),
            new(TokenType.Slash, "/"),
            new(TokenType.Asterisk, "*"),
            new(TokenType.Int, "5"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Int, "5"),
            new(TokenType.Lt, "<"),
            new(TokenType.Int, "10"),
            new(TokenType.Gt, ">"),
            new(TokenType.Int, "5"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.If, "if"),
            new(TokenType.Lparan, "("),
            new(TokenType.Int, "5"),
            new(TokenType.Lt, "<"),
            new(TokenType.Int, "10"),
            new(TokenType.Rparan, ")"),
            new(TokenType.Lbrace, "{"),
            new(TokenType.Return, "return"),
            new(TokenType.True, "true"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Rbrace, "}"),
            new(TokenType.Else, "else"),
            new(TokenType.Lbrace, "{"),
            new(TokenType.Return, "return"),
            new(TokenType.False, "false"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Rbrace, "}"),
            new(TokenType.Int, "10"),
            new(TokenType.Eq, "=="),
            new(TokenType.Int, "10"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Int, "10"),
            new(TokenType.NotEq, "!="),
            new(TokenType.Int, "9"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Eof, ""),
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
}