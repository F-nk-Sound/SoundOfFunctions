using Godot;
using System;
using Parsing;
using Functions;

public partial class ParseWrapper : Node {
	public IFunctionAST ParseString(string str) {
		return Bridge.Parse(str).Unwrap();
	}
	public IFunctionAST getFive(String str) {
		return Bridge.Parse(str).Unwrap();
	}
}
