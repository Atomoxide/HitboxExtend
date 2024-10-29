using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace TestPlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsTurnedOn { get; set; } = false;

    public void UpdateStatus()
    {
        if (IsTurnedOn)
        {
            MyHook.Initialize();
            Plugin.Logger.Info("plugin turned on");
            Plugin.Chat.Print("[Hitbox Extend] Turned on");
        }
        else if (!IsTurnedOn)
        {
            MyHook.Instance?.Dispose();
            Plugin.Logger.Info("plugin turned off");
            Plugin.Chat.Print("[Hitbox Extend] Turned off");
        }
    }
}
