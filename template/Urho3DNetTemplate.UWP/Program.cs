using System;
using Urho3DNet;

namespace $ext_safeprojectname$
{
    internal class Program
    {
        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        [MTAThread]
        private static void Main()
        {
            Launcher.Run(_=> new UrhoApplication(_));
        }
    }
}