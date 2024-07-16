namespace Monkey.Test;

[TestFixture]
public class ParserTest
{
    
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
            Assert.That(statement, Is.InstanceOf<ReturnStatement>());
            var returnStatement = (ReturnStatement)statement;
            Assert.That(returnStatement.TokenLiteral(), Is.EqualTo("return"));
        }
    }

    [Test]
    public void TestIdentifierExpression()
    {
        const string input = "foobar;";
        Lexer l = new(input);
        Parser p = new(l);
        var program = p.ParseProgram();
        CheckParserErrors(p);
        Assert.That(program.Statements, Has.Count.EqualTo(1));
        Assert.That(program.Statements[0], Is.InstanceOf<ExpressionStatement>());
        var expressionStatement = (ExpressionStatement) program.Statements[0];
        Assert.That(expressionStatement.Expression, Is.InstanceOf<Identifier>());
        var ident = (Identifier) expressionStatement.Expression;
        Assert.Multiple(() =>
        {
            Assert.That(ident.Value, Is.EqualTo("foobar"));
            Assert.That(ident.TokenLiteral(), Is.EqualTo("foobar"));
        });
    }

    [Test]
    public void TestIntegerLiteralExpression()
    {
        const string input = "5;";
        Lexer l = new(input);
        Parser p = new(l);
        var program = p.ParseProgram();
        CheckParserErrors(p);
        
        Assert.That(program.Statements, Has.Count.EqualTo(1));
        Assert.That(program.Statements[0], Is.InstanceOf<ExpressionStatement>());
        var statement = (ExpressionStatement)program.Statements[0];
        Assert.That(statement.Expression, Is.InstanceOf<IntegerLiteral>());
        var intLiteral = (IntegerLiteral)statement.Expression;
        Assert.Multiple(() =>
        {
            Assert.That(intLiteral.Value, Is.EqualTo(5));
            Assert.That(intLiteral.TokenLiteral(), Is.EqualTo("5"));
        });
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
    private record Prefix(string Input, string Operator, long IntegerValue)
    {
        public string Input { get; set; } = Input;
        public string Operator { get; set; } = Operator;
        public long IntegerValue { get; set; } = IntegerValue;
    };

    private record Infix(string Input, long LeftValue, string Operator, long RightValue)
    {
        public string Input { get; set; } = Input;
        public long LeftValue { get; set; } = LeftValue;
        public string Operator { get; set; } = Operator;
        public long RightValue { get; set; } = RightValue;
    }
    
    private static void TestIntegerLiteral(IExpression il, long value)
    {
        Assert.That(il, Is.InstanceOf<IntegerLiteral>());
        var integer = (IntegerLiteral)il;
        Assert.Multiple(() =>
        {
            Assert.That(integer.Value, Is.EqualTo(value));
            Assert.That(integer.TokenLiteral(), Is.EqualTo(value.ToString()));
        });
    }
    
    [Test]
    public void TestParsingPrefixExpression()
    {
        var prefixes = new List<Prefix>
        {
            new("!5;", "!", 5),
            new("-15", "-", 15)
        };
        foreach (var prefix in prefixes)
        {
            Lexer l = new(prefix.Input);
            Parser p = new(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);
            Assert.That(program.Statements, Has.Count.EqualTo(1));
            Assert.That(program.Statements[0], Is.InstanceOf<ExpressionStatement>());
            var statement = (ExpressionStatement)program.Statements[0];
            Assert.That(statement.Expression, Is.InstanceOf<PrefixExpression>());
            var expression = (PrefixExpression)statement.Expression;
            Assert.That(expression.Operator, Is.EqualTo(prefix.Operator));
            if (expression.Right != null) TestIntegerLiteral(expression.Right, prefix.IntegerValue);
        }
    }


    [Test]
    public void TestParsingInfixExpressions()
    {
        var infixes = new List<Infix>
        {
            new("5 + 5;", 5, "+", 5),
            new("5 - 5;", 5, "-", 5),
            new("5 * 5;", 5, "*", 5),
            new("5 / 5;", 5, "/", 5),
            new("5 > 5;", 5, ">", 5),
            new("5 == 5;", 5, "==", 5),
            new("5 != 5;", 5, "!=", 5),
        };
        foreach (var infix in infixes)
        {
            Lexer l = new(infix.Input);
            Parser p = new(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);
            Assert.That(program.Statements, Has.Count.EqualTo(1));
            Assert.That(program.Statements[0], Is.InstanceOf<ExpressionStatement>());
            var statement = (ExpressionStatement)program.Statements[0];
            Assert.That(statement.Expression, Is.InstanceOf<InfixExpression>());
            var expression = (InfixExpression)statement.Expression;
            Assert.That(expression.Operator, Is.EqualTo(infix.Operator));
            if (expression.Right != null) TestIntegerLiteral(expression.Right, infix.RightValue);
        }
    }
    
}