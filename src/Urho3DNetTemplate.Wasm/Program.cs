using System;
using System.Runtime.InteropServices.JavaScript;
using Urho3DNet;
using Urho3DNetTemplate;
using Console = System.Console;

public static class Program
{
    public static void Main()
    {
        Launcher.Run(_ => new UrhoApplication(_));
    }
}

public partial class MyClass
{
    [JSExport]
    internal static string Greeting()
    {
        var text = $"Hello, World! Greetings from {GetHRef()}";
        Console.WriteLine(text);
        return text;
    }

    [JSImport("window.location.href", "main.js")]
    internal static partial string GetHRef();
}

public static class Launcher
{
    public static int Run(Func<Context, Application> factory)
    {
        return Run(factory, IntPtr.Zero);
    }

    public static int Run(Func<Context, Application> factory, IntPtr externalWindow)
    {
        using (SharedPtr<Context> context = new Context())
        {
            using (SharedPtr<Application> application = factory(context))
            {
                return application.Ptr.Run();
            }
        }
    }
}
