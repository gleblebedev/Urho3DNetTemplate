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
    }
}