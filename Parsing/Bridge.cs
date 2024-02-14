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
    delegate* unmanaged[Cdecl]<double, IntPtr> newNumber = &NewNumber;

    // Non-fields go below here
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    unsafe static IntPtr NewNumber(double value) => NewHandle(new Number(value));

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

    public static IFunctionAST Parse(string input)
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
                return (IFunctionAST)target;
            }
        }
    }
}