using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using TestPlugin.Windows;
using Dalamud.Game.Gui.Toast;


namespace TestPlugin;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IChatGui Chat { get; private set; }
    [PluginService] public static IToastGui Toasts { get; private set; }

    public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;

    public static IPluginLog Logger { get; private set; } = null!;

    private const string CommandName = "/hbe";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("Extend Hitbox");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin(IGameInteropProvider gameInteropProvider, IPluginLog logger)
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        var yoship = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "yoship.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, yoship);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "/hbe on: toggle on\n/hbe off: toggle off\n/hbe config: open config window"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        GameInteropProvider = gameInteropProvider;
        Logger = logger;
        Configuration.IsTurnedOn = false;
        
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
        MyHook.Instance?.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        //Logger.Information("args:" + args);
        if (args == "on" && !Configuration.IsTurnedOn)
        {
            MyHook.Initialize();
            Logger.Info("plugin turned on");
            Chat.Print("[Hitbox Extend] Turned on");
            Toasts.ShowQuest("Hitbox Extend Turned on",
                    new QuestToastOptions() { PlaySound = true, DisplayCheckmark = true });
            Configuration.IsTurnedOn = true;
        }
        else if (args == "off" && Configuration.IsTurnedOn) 
        {
            MyHook.Instance?.Dispose();
            Logger.Info("plugin turned off");
            Chat.Print("[Hitbox Extend] Turned off");
            Toasts.ShowQuest("Hitbox Extend Turned off",
                    new QuestToastOptions() { PlaySound = true, DisplayCheckmark = true });
            Configuration.IsTurnedOn = false;
        }
        else if (args == "config")
        {
            ToggleConfigUI();
        }
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
