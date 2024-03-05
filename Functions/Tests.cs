using Parsing;
using Xunit;

namespace Functions.Tests;

public class Tests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(0.33, 1)]
    [InlineData(2.26, 1)]
    [InlineData(1.63, -1)]
    [InlineData(-1.37, -1)]
    public void SquareWave(double t, double output)
    {
        // 4floor(t) - 2floor(2t) + 1
        // source: https://en.wikipedia.org/wiki/Square_wave
        IFunctionAST f = new Add(
            new Subtract(
                new Multiply(
                    new Number(4),
                    new Floor(
                        new Variable("t")
                    )
                ),
                new Multiply(
                    new Number(2),
                    new Floor(
                        new Multiply(
                            new Number(2),
                            new Variable("t")
                        )
                    )
                )
            ),
            new Number(1)
        );

        Assert.Equal(f.EvaluateAtT(t), output);
    }

    [Theory]
    [InlineData("4floor(t) - 2floor(2t) + 1", "4\\lfloor t \\rfloor - 2\\lfloor 2t \\rfloor + 1")]
    [InlineData("21x/367 + 5", "\\frac{21x}{367} + 5")]
    [InlineData("5sin(x^2)^2", "5sin(x^{2})^{2}")]
    [InlineData("12 + 3 + 4 + 5 + 6 + 19", "12 + 3 + 4 + 5 + 6 + 19")]
    [InlineData("abs(floor(ceil(x)))", "|\\lfloor \\lceil x \\rceil \\rfloor|")]
    public void FunctionToLatex(string function, string latex)
    {
        Assert.Equal(latex, Bridge.Parse(function).Unwrap().Latex);
    }
}