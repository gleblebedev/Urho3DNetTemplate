using System;
using Urho3DNet;
using Urho3DNetTemplate;

namespace rbfxSample.UniversalWindows
{
    internal class Program
    {
        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        [MTAThread]
        private static void Main()
        {
            Launcher.SdlHandleBackButton = true;
            Launcher.Run(_=> new UrhoApplication(_));
        }
    }
}