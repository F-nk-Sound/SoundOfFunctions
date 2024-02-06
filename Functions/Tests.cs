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
}