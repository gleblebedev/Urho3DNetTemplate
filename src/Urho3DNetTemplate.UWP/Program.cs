using System;
using Urho3DNet;

namespace Urho3DNetTemplate
{
    internal class Program
    {
        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        [MTAThread]
        private static void Main()
        {
            Urho3DNet.Launcher.SdlHandleBackButton = true;
            Urho3DNet.Launcher.Run(_=> new UrhoApplication(_));
        }
    }
}