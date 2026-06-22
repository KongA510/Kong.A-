using Aras.IOM;
class Test { static void Main() { 
    // Test 1
    var t = typeof(Innovator);
    // Test 2  
    var m = t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
    foreach(var x in m) System.Console.WriteLine(x.Name + " -> " + x.ReturnType);
} }
