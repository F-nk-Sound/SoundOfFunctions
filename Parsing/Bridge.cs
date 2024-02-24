using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Functions;

namespace Parsing;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct CtorTable
{
    // Fields go below here
    // ### ALWAYS ENSURE FIELDS ARE IN THE CORRECT ORDER ON BOTH SIDES ###
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr> newAbsolute = &NewAbsolute;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr> newAdd = &NewAdd;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr> newCeil = &NewCeil;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr> newCosine = &NewCosine;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr> newDivide = &NewDivide;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr> newExponent = &NewExponent;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr> newFloor = &NewFloor;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr> newMultiply = &NewMultiply;
    delegate* unmanaged[Cdecl]<double, IntPtr> newNumber = &NewNumber;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr> newSine = &NewSine;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr> newSubtract = &NewSubtract;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr> newTangent = &NewTangent;
    delegate* unmanaged[Cdecl]<IntPtr, IntPtr> newVariable = &NewVariable;
    delegate* unmanaged[Cdecl]<byte*, nuint, IntPtr> newString = &NewString;

    // Non-fields go below here
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewNumber(double value) => NewHandle(new Number(value));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewString(byte* ptr, nuint len)
    {
        string str = Encoding.UTF8.GetString(ptr, (int)len);
        return NewHandle(str);
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewVariable(IntPtr name) => NewHandle(new Variable(StringFromHandle(name)));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewAdd(IntPtr l, IntPtr r) => NewHandle(
        new Add(
            FromHandle(l),
            FromHandle(r)
        )
    );

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewSubtract(IntPtr l, IntPtr r) => NewHandle(
       new Subtract(
           FromHandle(l),
           FromHandle(r)
       )
   );

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewMultiply(IntPtr l, IntPtr r) => NewHandle(
       new Multiply(
           FromHandle(l),
           FromHandle(r)
       )
   );

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewDivide(IntPtr l, IntPtr r) => NewHandle(
         new Divide(
             FromHandle(l),
             FromHandle(r)
         )
     );

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewExponent(IntPtr b, IntPtr p) => NewHandle(new Exponent(FromHandle(b), FromHandle(p)));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewAbsolute(IntPtr inner) => NewHandle(new Absolute(FromHandle(inner)));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewCeil(IntPtr inner) => NewHandle(new Ceil(FromHandle(inner)));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewFloor(IntPtr inner) => NewHandle(new Floor(FromHandle(inner)));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewSine(IntPtr inner) => NewHandle(new Sine(FromHandle(inner)));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewCosine(IntPtr inner) => NewHandle(new Cosine(FromHandle(inner)));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewTangent(IntPtr inner) => NewHandle(new Tangent(FromHandle(inner)));

    /// <summary>
    /// A list of all GCHandles currently allocated, to be cleared once the foreign code is done 
    /// using them.
    /// 
    /// Thread local so that multiple threads can use it freely.
    /// </summary>
    static ThreadLocal<List<GCHandle>> activeHandles = new() { Value = new() };

    static IntPtr NewHandle(object obj)
    {
        GCHandle handle = GCHandle.Alloc(obj);
        activeHandles.Value!.Add(handle);
        return GCHandle.ToIntPtr(handle);
    }

    static IFunctionAST FromHandle(IntPtr handle) => (IFunctionAST)GCHandle.FromIntPtr(handle).Target!;

    static string StringFromHandle(IntPtr handle) => (string)GCHandle.FromIntPtr(handle).Target!;

    /// <summary>
    /// Free all currently active handles to prevent a memory leak. May result in undesirable behavior if
    /// done while foreign code is storing these handles.
    /// </summary>
    public unsafe static void FreeHandles()
    {
        foreach (GCHandle handle in activeHandles.Value!)
            handle.Free();

        activeHandles.Value!.Clear();
    }

    public CtorTable() { }
}

public static class Bridge
{
    [DllImport(dllName: "fnky_parser", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fnky_parse")]
    private unsafe static extern IntPtr Parse(byte* input, nuint length, CtorTable* table);

    public static ParseResult Parse(string input)
    {
        CtorTable table = new();

        byte[] utf16Stream = Encoding.Unicode.GetBytes(input);
        byte[] utf8Stream = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Stream);

        unsafe
        {
            fixed (byte* utf8Ptr = utf8Stream)
            {
                nuint length = (nuint)utf8Stream.Length;
                IntPtr handle = Parse(utf8Ptr, length, &table);
                object? target = GCHandle.FromIntPtr(handle).Target;
                CtorTable.FreeHandles();
                if (target is null)
                    throw new NullReferenceException("Parsing module returned a null reference.");
                return target switch
                {
                    IFunctionAST ast => new Success(ast),
                    string error => new Failure(error),
                    _ => throw new Exception("Unknown return value from parsing module"),
                };
            }
        }
    }
}

/// <summary>
/// Represents a potentially successful parse attempt. 
/// Will be a Success if it was successful, and Failure if it encountered an error.
/// </summary>
public abstract class ParseResult 
{
    public IFunctionAST Unwrap() => this switch
    {
        Success s => s.Value,
        Failure f => throw new Exception(f.Error),
        _ => throw new ArgumentException("ParseResult was an unknown variant"),
    };
}

public sealed class Success : ParseResult
{
    public IFunctionAST Value { get; init; }

    public Success(IFunctionAST value)
    {
        Value = value;
    }
}

public sealed class Failure : ParseResult
{
    public string Error { get; init; }

    public Failure(string error)
    {
        Error = error;
    }
}