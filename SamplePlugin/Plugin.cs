using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
//using TestPlugin.Windows;


namespace TestPlugin;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IChatGui Chat { get; private set; }

    public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;

    public static IPluginLog Logger { get; private set; } = null!;

    private const string CommandName = "/hbe";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("SamplePlugin");
    //private ConfigWindow ConfigWindow { get; init; }
    //private MainWindow MainWindow { get; init; }

    public Plugin(IGameInteropProvider gameInteropProvider, IPluginLog logger)
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        //ConfigWindow = new ConfigWindow(this);
        //MainWindow = new MainWindow(this, goatImagePath);

        //WindowSystem.AddWindow(ConfigWindow);
        //WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "/hbe on/off: toggle on/off"
        });

        //PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        //PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        //PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        GameInteropProvider = gameInteropProvider;
        Logger = logger;
        
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        //ConfigWindow.Dispose();
        //MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
        //MyHook.Initialize();
        MyHook.Instance?.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        //Logger.Information("args:" + args);
        //ToggleMainUI();
        if (args == "on")
        {
            MyHook.Initialize();
            Logger.Info("plugin turned on");
            Chat.Print("[Hitbox Extend] Turned on");
        }
        else if (args == "off") 
        {
            MyHook.Instance?.Dispose();
            Logger.Info("plugin turned off");
            Chat.Print("[Hitbox Extend] Turned off");

        }
    }

    //private void DrawUI() => WindowSystem.Draw();

    //public void ToggleConfigUI() => ConfigWindow.Toggle();
    //public void ToggleMainUI() => MainWindow.Toggle();
}
