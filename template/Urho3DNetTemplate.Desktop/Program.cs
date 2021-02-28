using System;
using Urho3DNet;

namespace $safeprojectname$
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Launcher.Run(_ => new UrhoApplication(_));
        }
    }
}
