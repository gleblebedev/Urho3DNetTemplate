﻿using Urho3DNet;

namespace $ext_safeprojectname$
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            Launcher.Run(_ =>new UrhoApplication(_));
        }
    }
}