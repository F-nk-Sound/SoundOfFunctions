using Functions;
using Xunit;

namespace Parsing;

public class Tests
{
    [Theory]
    [InlineData("42", 42)]
    [InlineData("-12", -12)]
    [InlineData("24.09", 24.09)]
    [InlineData("-99999999.99", -99999999.99)]
    public void NumbersParse(string input, double value)
    {
        object ast = Bridge.Parse(input);
        if (ast is Number num)
            Assert.Equal(value, num.Value);
        else
            Assert.Fail((string)ast);
    }

    [Theory]
    [InlineData("x")]
    [InlineData("x_1")]
    [InlineData("α_21x")]
    [InlineData("A_a32f")]
    public void VariablesParse(string input)
    {
        object ast = Bridge.Parse(input);
        if (ast is Variable var)
            Assert.Equal(input, var.Name);
        else
            Assert.Fail((string)ast);
    }

    [Theory]
    [InlineData("3 + 3", 6)]
    public void ArithmeticCalculations(string input, double output) {
        object ast = Bridge.Parse(input);
        if (ast is IFunctionAST func)
            Assert.Equal(output, func.EvaluateAtT(0));
        else
            Assert.Fail((string)ast);
    }

    [Theory]
    [InlineData("5x^2 + 3y + z")]
    [InlineData("4floor(t) + 2floor(2t) - 1")]
    public void ComplexFunctionsParse(string input)
    {
        object ast = Bridge.Parse(input);
        Assert.True(ast is IFunctionAST);
    }
}