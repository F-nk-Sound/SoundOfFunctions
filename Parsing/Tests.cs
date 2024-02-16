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
        IFunctionAST ast = Bridge.Parse(input);
        Assert.True(ast is Number);
        Assert.Equal(value, ((Number)ast).Value);
    }
}