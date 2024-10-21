using BlasII.ModdingAPI;

namespace BlasII.Framework.Menus;

/// <summary>
/// Framework that allows other mods to implement start game menus
/// </summary>
public class MenuFramework : BlasIIMod
{
    internal MenuFramework() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    /// <summary>
    /// Loads and stores UI icons
    /// </summary>
    public IconLoader IconLoader { get; private set; }

    private MenuCollection _newGameMenus;
    private MenuCollection _loadGameMenus;
    private MenuCollection CurrentMenuCollection => _isContinue ? _loadGameMenus : _newGameMenus;
    private bool IsMenuActive => CurrentMenuCollection.IsActive;

    private bool _enterNextFrame = false;
    private bool _isContinue = false;
    private int _currentSlot = 0;

    /// <summary>
    /// Load and setup ui
    /// </summary>
    protected override void OnInitialize()
    {
        IconLoader = new IconLoader(FileHandler);

        LocalizationHandler.RegisterDefaultLanguage("en");
    }

    /// <summary>
    /// Initialize the menu collections with all registered menus
    /// </summary>
    protected override void OnAllInitialized()
    {
        _newGameMenus = new MenuCollection(MenuRegister.NewGameMenus, OnFinishMenu, OnCancelMenu);
        _loadGameMenus = new MenuCollection(MenuRegister.LoadGameMenus, OnFinishMenu, OnCancelMenu);
    }
}
