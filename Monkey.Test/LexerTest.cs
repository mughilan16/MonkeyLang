using Monkey;

namespace Monkey.Test
{
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
                                 let add  fn(x, y) {
                                 x + y;
                                 };
                                 let result = add(five, ten);

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
    }
}