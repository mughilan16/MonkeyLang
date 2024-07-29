using System.Diagnostics;

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
        Assert.That(program.Statements, Has.Count.EqualTo(3));
        string[] tests = ["x", "y", "foobar"];
        foreach (var (name, i) in tests.Select((t, i) => (t, i)))
        {
            var s = program.Statements[i];
            Assert.That(s.TokenLiteral(), Is.EqualTo("let"));
            Assert.That(s, Is.InstanceOf(typeof(LetStatement)));
            var letStatement = (LetStatement)s;
            Assert.That(letStatement.Name?.Value, Is.EqualTo(name));
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
        Assert.Pass();
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
        Assert.Pass();
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
    private static void TestIdentifier(IExpression? expression, string value)
    {
        Assert.That(expression, Is.InstanceOf<Identifier>());
        var ident = (Identifier) expression;
        Assert.Multiple(() =>
        {
            Assert.That(ident.Value, Is.EqualTo(value));
            Assert.That(ident.TokenLiteral(), Is.EqualTo(value));
        });
    }
    private record Prefix(string Input, string Operator, long IntegerValue)
    {
        public string Input { get; set; } = Input;
        public string Operator { get; set; } = Operator;
        public long IntegerValue { get; set; } = IntegerValue;
    };

    public record Infix<T1, T2>(string Input, T1 LeftValue, string Operator, T2 RightValue)
    {
        public string Input { get; set; } = Input;
        public T1 LeftValue { get; set; } = LeftValue;
        public string Operator { get; set; } = Operator;
        public T2 RightValue { get; set; } = RightValue;
    }
    
    private static void TestIntegerLiteral(IExpression? il, long value)
    {
        Assert.That(il, Is.InstanceOf<IntegerLiteral>());
        var integer = (IntegerLiteral)il;
        Assert.Multiple(() =>
        {
            Assert.That(integer.Value, Is.EqualTo(value));
            Assert.That(integer.TokenLiteral(), Is.EqualTo(value.ToString()));
        });
    }
    
    private static void TestBooleanLiteral(IExpression? il, bool value)
    {
        Assert.That(il, Is.InstanceOf<Boolean>());
        var boolean = (Boolean)il;
        Assert.Multiple(() =>
        {
            Assert.That(boolean.Value, Is.EqualTo(value));
            Assert.That(boolean.TokenLiteral(), Is.EqualTo(value.ToString().ToLower()));
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
        Assert.Pass();
    }
    

    [Test]
    public void TestParsingInfixExpressions()
    {
        var infixes = new List<object>{
            new Infix<long, long>("5 + 5;", 5, "+", 5),
            new Infix<long, long>("5 - 5;", 5, "-", 5),
            new Infix<long, long>("5 * 5;", 5, "*", 5),
            new Infix<long, long>("5 / 5;", 5, "/", 5),
            new Infix<long, long>("5 > 5;", 5, ">", 5),
            new Infix<bool, bool>("true == true;", true, "==", true),
            new Infix<bool, bool>("true != false", true, "!=", false),
            new Infix<bool, bool>("false == false", false, "==", false)
        };
        foreach (var infix in infixes)
        {
            switch (infix)
            {
            case Infix<long, long> longInfix:
                TestInfix(longInfix);
                break;
            case Infix<bool, bool> boolInfix:
                TestInfix(boolInfix);
                break;
            }
        }
        Assert.Pass();
    }

    private static void TestInfix<T1, T2>(Infix<T1, T2> infix)
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
            TestLiteralExpression(expression.Right, infix.RightValue);
    }
    
    [Test]
    public void TestParsingInfixExpressions2()
    {
        Lexer l1 = new("5 + 10");
        Parser p1 = new(l1);
        CheckParserErrors(p1);
        var program1 = p1.ParseProgram();
        Assert.That(program1.Statements[0], Is.InstanceOf<ExpressionStatement>());
        Assert.That(program1.Statements[0], Is.InstanceOf<ExpressionStatement>());
        var exp1 = (ExpressionStatement)program1.Statements[0];
        TestInfixExpression(exp1.Expression, 5, "+", 10);
        Lexer l2 = new("alice * bob");
        Parser p2 = new(l2);
        CheckParserErrors(p2);
        var program2 = p2.ParseProgram();
        Assert.That(program2.Statements[0], Is.InstanceOf<ExpressionStatement>());
        Assert.That(program2.Statements[0], Is.InstanceOf<ExpressionStatement>());
        var exp2 = (ExpressionStatement)program2.Statements[0];
        TestInfixExpression(exp2.Expression, "alice", "*", "bob");
        Assert.Pass();
    }
    
    private record Test(string Input, string Expected)
    {
        public string Input { get; set; } = Input;
        public string Expected { get; set; } = Expected;
    };

    [Test]
    public void TestOperatorPrecedenceParsing()
    {
        var tests = new List<Test>
        {
            new("-a * b", "((-a) * b)"),
            new("!-a", "(!(-a))"),
            new("a + b + c", "((a + b) + c)"),
            new("a + b - c", "((a + b) - c)"),
            new("a * b * c", "((a * b) * c)"),
            new("a * b / c", "((a * b) / c)"),
            new("a + b / c", "(a + (b / c))"),
            new("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)"),
            new("3 + 4; -5 * 5", "(3 + 4)((-5) * 5)"),
            new("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))"),
            new("5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))"),
            new("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
            new("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
            new("true", "true"),
            new("false", "false"),
            new("3 > 5 == false", "((3 > 5) == false)"),
            new("3 < 5 == true", "((3 < 5) == true)"),
            new("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)"),
            new("(5 + 5) * 2", "((5 + 5) * 2)"),
            new("2 / (5 + 5)", "(2 / (5 + 5))"),
            new("-(5 + 5)", "(-(5 + 5))"),
            new("!(true == true)", "(!(true == true))")
        };
        foreach (var t in tests)
        {
            Lexer l = new(t.Input);
            Parser p = new(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);
            var actual = program.String();
            Assert.That(actual, Is.EqualTo(t.Expected));
        }

        Assert.Pass();
    }

    private static void TestLiteralExpression<T>(IExpression? expression, T value)
    {
        switch (value)
        {
            case int intValue:
                TestIntegerLiteral(expression, intValue);
                break;
            case long longValue:
                TestIntegerLiteral(expression, longValue);
                break;
            case string stringValue:
                TestIdentifier(expression, stringValue);
                break;
            case bool boolValue:
                TestBooleanLiteral(expression, boolValue);
                break;
        }
    }

    private void TestInfixExpression<T1, T2>(IExpression? expression, T1 left, string @operator, T2 right)
    {
        Assert.That(expression, Is.InstanceOf<InfixExpression>());
        var opExp = (InfixExpression)expression;
        Assert.That(opExp, Is.Not.EqualTo(null));
        TestLiteralExpression(opExp.Left, left);
        Assert.That(opExp.Operator, Is.EqualTo(@operator));
        TestLiteralExpression(opExp.Right, right);
    }

    [Test]
    public void TestIfExpression()
    {
        var input = "if (x < y) { x }";
        Lexer l = new(input);
        Parser p = new(l);
        var program = p.ParseProgram();
        CheckParserErrors(p);
        
        Assert.That(program.Statements, Has.Count.EqualTo(1));
        Assert.That(program.Statements[0], Is.InstanceOf<ExpressionStatement>());
        
        var statement = (ExpressionStatement)program.Statements[0];
        Assert.That(statement.Expression, Is.InstanceOf<IfExpression>());
        
        var exp = (IfExpression)statement.Expression;
        TestInfixExpression(exp.Condition, "x", "<", "y");

        Debug.Assert(exp.Consequence.Statements != null, "exp.Consequence.Statements != null");
        Assert.That(exp.Consequence.Statements[0], Is.InstanceOf<ExpressionStatement>());

        var consequence = (ExpressionStatement)exp.Consequence.Statements[0];
        TestIdentifier(consequence.Expression, "x");
        
        Assert.That(exp.Alternative, Is.Null);
    }

    public record Input
    {
        public string input;
        public List<string> expectedParams;
    }

    [Test]
    public void TestFunctionLiteralParsing()
    {
        Input[] tests = new[] {
            new Input{
                input = "fn() {};",
                expectedParams = []
            },
            new Input{
                input = "fn(x) {};",
                expectedParams = ["x"]
            },
            new Input{
                input = "fn(x, y, z) {};",
                expectedParams = ["x", "y", "z"]
            },
        };

        foreach (var test in tests)
        {
            Lexer l = new(test.input);
            Parser p = new(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);
        
            Assert.That(program.Statements, Has.Count.EqualTo(1));
            Assert.That(program.Statements[0], Is.InstanceOf<ExpressionStatement>());
            var exp = (ExpressionStatement)program.Statements[0];
            Assert.That(exp.Expression, Is.InstanceOf<FunctionLiteral>());
            var functionLiteral = (FunctionLiteral)exp.Expression;
            Assert.That(functionLiteral.Parameters, Has.Count.EqualTo(test.expectedParams.Count));

            var i = 0;
            foreach (var param in test.expectedParams)
            {
                TestLiteralExpression(functionLiteral.Parameters[i], param);
                i++;
            }
        }
    }
}
