using Functions;
using Godot;
using Xunit;
using System;

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
		IFunctionAST ast = Bridge.Parse(input).Unwrap();
		Assert.IsType<Number>(ast);
		Assert.Equal(value, (ast as Number)!.Value);
	}

	[Theory]
	[InlineData("x")]
	[InlineData("x_1")]
	[InlineData("α_21x")]
	[InlineData("A_a32f")]
	public void VariablesParse(string input)
	{
		IFunctionAST ast = Bridge.Parse(input).Unwrap();
		Assert.IsType<Variable>(ast);
		Assert.Equal(input, (ast as Variable)!.Name);
	}

	[Theory]
	[InlineData("3 + 3", 6)]
	[InlineData("5 % 3", 2)]
	[InlineData("pi % 2pi", Math.PI)]
	public void ArithmeticCalculations(string input, double output)
	{
		IFunctionAST ast = Bridge.Parse(input).Unwrap();
		Assert.Equal(output, ast.EvaluateAtT(0));
	}

	[Theory]
	[InlineData("5x^2 + 3y + z")]
	[InlineData("4floor(t) + 2floor(2t) - 1")]
	public void ComplexFunctionsParse(string input)
	{
		IFunctionAST _ = Bridge.Parse(input).Unwrap();
	}

	[Theory]
	[InlineData("5e % 2pi", "5\\e % 2\\pi")]
	public void LatexDisplay(string input, string expected)
	{
		IFunctionAST ast = Bridge.Parse(input).Unwrap();
		Assert.Equal(expected, ast.Latex);
	}
}
