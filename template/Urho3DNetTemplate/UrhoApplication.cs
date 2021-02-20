using System.Diagnostics;
using Urho3DNet;

namespace $safeprojectname$
{
    public class UrhoApplication: Application
    {
        public UrhoApplication(Context context) : base(context)
        {
        }

        public override void Setup()
        {
            if (Debugger.IsAttached)
            {
                EngineParameters[Urho3D.EpFullScreen] = false;
                EngineParameters[Urho3D.EpWindowResizable] = true;
            }
            else
            {
                EngineParameters[Urho3D.EpFullScreen] = true;
            }
            EngineParameters[Urho3D.EpWindowTitle] = "$safeprojectname$";
            base.Setup();
        }

        public override void Start()
        {
            Context.Renderer.DefaultZone.FogColor = new Color(0.1f, 0.2f, 0.4f, 1.0f);
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
