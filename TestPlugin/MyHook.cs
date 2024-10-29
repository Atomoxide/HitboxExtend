using Dalamud.Hooking;
using System;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System.Linq.Expressions;

namespace TestPlugin;

// declare delegate


public unsafe class MyHook : IDisposable
{
    private delegate void OnLoadingHitbox(GameObject* gameObject, float unkownVal);
    private delegate void OnUpdatingHitbox(GameObject* gameObject);
    private Hook<OnLoadingHitbox>? _onHitboxLoadedHook;
    private Hook<OnUpdatingHitbox>? _onHitboxUpdatedHook;

    public static void Initialize() { Instance = new MyHook(); }

    public static MyHook Instance { get; private set; } = null!;
    private MyHook()
    {
        try
        {
            _onHitboxLoadedHook = Plugin.GameInteropProvider.HookFromSignature<OnLoadingHitbox>(
                    "48 89 5C 24 08 57 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 48 8D 8B A0 01",
                    OnHitboxLoaded
                );

            // Nullable because this might not have been initialized from IFA above, e.g. the sig was invalid.
            _onHitboxLoadedHook?.Enable();
        }
        catch (Exception e)
        {
            Plugin.Logger.Error("Error initiating OnloadingHitbox hooks: " + e.Message);
        }

        try
        {
            _onHitboxUpdatedHook = Plugin.GameInteropProvider.HookFromSignature<OnUpdatingHitbox>(
                    "40 53 48 83 EC 20 66 83 B9 98 06 00 00 00",
                    onHitboxUpdated
                );
                _onHitboxUpdatedHook?.Enable();
        }
        catch (Exception e)
        {
            Plugin.Logger.Error("Error initiating OnUpdatingHitbox hooks: " + e.Message);
        }

    }


    ~MyHook()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        try
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        catch (Exception e)
        {
            Plugin.Logger.Info("Already disposed: " + e.Message);
        }
        
    }

    protected void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _onHitboxLoadedHook?.Disable();
        _onHitboxLoadedHook?.Dispose();

        _onHitboxUpdatedHook?.Disable();
        _onHitboxLoadedHook?.Dispose();

    }

    private unsafe void OnHitboxLoaded(GameObject* gameObject, float unkownVal)
    {
        this._onHitboxLoadedHook?.Original(gameObject, unkownVal);

        try
        {
            gameObject->HitboxRadius += 3.0f;
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error(ex, "An error occured when editing hitbox during hitbox loading.");
        }
    }

    private unsafe void onHitboxUpdated(GameObject* gameObject)
    {
        this._onHitboxUpdatedHook?.Original(gameObject);

        try
        {
            gameObject->HitboxRadius += 3.0f;
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error(ex, "An error occured when editing hitbox during hitbox updating.");
        }
    }
}
