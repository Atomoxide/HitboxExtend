using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace TestPlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("Extend Hitbox Configuration")
    {
        //Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        //        ImGuiWindowFlags.NoScrollWithMouse;

        //Size = new Vector2(232, 90);
        //SizeCondition = ImGuiCond.Always;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 120),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // can't ref a property, so use a local copy
        var configValue = Configuration.IsTurnedOn;
        if (ImGui.Checkbox("Enable (non persistent)", ref configValue))
        {
            Configuration.IsTurnedOn = configValue;
            Configuration.UpdateStatus();
        }
        ImGui.Text("Change only apply to new selectable targets. \nYou need to leave the current map zone \nand then come back to refresh target hitboxes");
    }
}
