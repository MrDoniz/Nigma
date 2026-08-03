using System;
using System.Reflection;

class Program {
    static void Main() {
        var asm = Assembly.LoadFrom("Bee.TinyProfiler2.dll");
        foreach(var type in asm.GetExportedTypes()) {
            Console.WriteLine("Type: " + type.FullName);
            foreach(var method in type.GetMethods()) {
                if (method.DeclaringType == type)
                    Console.WriteLine("  " + method.ToString());
            }
        }
    }
}
