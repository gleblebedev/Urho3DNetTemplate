using Urho3DNet;

namespace Urho3DNetTemplate
{
    /// <summary>
    ///     This class represents an Urho3D plugin application.
    /// </summary>
    [LoadablePlugin]
    [Preserve(AllMembers = true)]
    public class UrhoPluginApplication : PluginApplication
    {
        public UrhoPluginApplication(Context context) : base(context)
        {
        }

        protected override void Load()
        {
            Context.RegisterFactories(GetType().Assembly);
        }

        protected override void Unload()
        {
            Context.RemoveFactories(GetType().Assembly);
        }

        public override bool IsMain()
        {
            return true;
        }

        protected override void Start(bool isMain)
        {
            base.Start(isMain);
        }

        protected override void Stop()
        {
            base.Stop();
        }

        protected override void Suspend(Archive output)
        {
            base.Suspend(output);
        }

        protected override void Resume(Archive input, bool differentVersion)
        {
            base.Resume(input, differentVersion);
        }
    }
}