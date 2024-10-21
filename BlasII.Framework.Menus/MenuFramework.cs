using BlasII.ModdingAPI;

namespace BlasII.Framework.Menus;

/// <summary>
/// Framework that allows other mods to implement start game menus
/// </summary>
public class MenuFramework : BlasIIMod
{
    internal MenuFramework() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    protected override void OnInitialize()
    {
        // Perform initialization here
    }
}
