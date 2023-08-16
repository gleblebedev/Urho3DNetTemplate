using System;
using System.Text;
using Newtonsoft.Json;
using Urho3DNet;
using File = System.IO.File;

namespace Urho3DNetTemplate
{
    public class SettingFile
    {
        static readonly string SOUND_MASTER = "Master";
        static readonly string SOUND_EFFECT = "Effect";
        //static readonly string SOUND_AMBIENT = "Ambient";
        //static readonly string SOUND_VOICE = "Voice";
        static readonly string SOUND_MUSIC = "Music";

        public bool Bloom { get; set; } = true;
        public bool SSAO { get; set; } = true;
        public float MasterVolume { get; set; } = 1.0f;
        public float MusicVolume { get; set; } = 1.0f;
        public float EffectVolume { get; set; } = 1.0f;

        public static SettingFile Load(Context context)
        {
            var json = context.GetSubsystem<VirtualFileSystem>().ReadAllText(new FileIdentifier("conf", "settings.json"));

            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    return JsonConvert.DeserializeObject<SettingFile>(json);
                }
                catch (Exception)
                {
                }
            }

            return new SettingFile();
        }

        public bool Save(Context context)
        {
            return context.GetSubsystem<VirtualFileSystem>().WriteAllText(new FileIdentifier("conf", "settings.json"), JsonConvert.SerializeObject(this));
        }

        public void Apply(Context context)
        {
            Audio audio = context.GetSubsystem<Audio>();

            audio.SetMasterGain(SOUND_MASTER, MasterVolume);
            audio.SetMasterGain(SOUND_MUSIC, MusicVolume);
            audio.SetMasterGain(SOUND_EFFECT, EffectVolume);
        }

        public void Apply(RenderPipeline renderPipeline)
        {
            var settings = renderPipeline.Settings;
            settings.Bloom.Enabled = Bloom;
            settings.Ssao.Enabled = SSAO;

            renderPipeline.Settings = settings;
        }
    }
}