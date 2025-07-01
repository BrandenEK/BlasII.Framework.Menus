using BlasII.Framework.UI;
using BlasII.ModdingAPI;
using BlasII.ModdingAPI.Utils;
using Il2CppTGK.Game;
using Il2CppTGK.Game.Components.UI;
using UnityEngine;

namespace BlasII.Framework.Menus;

/// <summary>
/// Framework that allows other mods to implement start game menus
/// </summary>
public class MenuFramework : BlasIIMod
{
    internal MenuFramework() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION)
    {
        _mainMenuCache = new ObjectCache<MainMenuWindowLogic>(Object.FindObjectOfType<MainMenuWindowLogic>);
        _slotsMenuCache = new ObjectCache<GameObject>(() => _mainMenuCache.Value.slotsMenuView.transform.parent.gameObject);
    }

    internal static bool AllowGameStart { get; private set; }

    /// <summary>
    /// Loads and stores UI icons
    /// </summary>
    public IconLoader IconLoader { get; private set; }

    private InputBlocker _inputBlocker;

    // Menu objects
    private readonly ObjectCache<MainMenuWindowLogic> _mainMenuCache;
    private readonly ObjectCache<GameObject> _slotsMenuCache;

    // Menu collections
    private MenuCollection _newGameMenus;
    private MenuCollection _loadGameMenus;
    private MenuCollection CurrentMenuCollection => _isContinue ? _loadGameMenus : _newGameMenus;

    // Temporary settings
    private bool _enterNextFrame = false;
    private bool _cancelNextFrame = false;
    private bool _isContinue = false;
    private int _currentSlot = 0;

    /// <summary>
    /// Is a menu currently being shown
    /// </summary>
    public bool IsMenuActive => CurrentMenuCollection.IsActive;

    /// <summary>
    /// Load and setup ui
    /// </summary>
    protected override void OnInitialize()
    {
        IconLoader = new IconLoader(FileHandler);
        _inputBlocker = new InputBlocker();

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

    /// <summary>
    /// Updates the current menu
    /// </summary>
    protected override void OnUpdate()
    {
        if (_enterNextFrame)
        {
            _enterNextFrame = false;
            CurrentMenuCollection.ShowNextMenu();
        }
        if (_cancelNextFrame)
        {
            _cancelNextFrame = false;
            CurrentMenuCollection.ShowPreviousMenu();
            return;
        }

        if (IsMenuActive)
            CurrentMenuCollection.CurrentMenu.OnUpdate();
    }

    /// <summary>
    /// Opens the next menu in the queue, or starts the game
    /// </summary>
    public void ShowNextMenu()
    {
        _enterNextFrame = true;
    }

    /// <summary>
    /// Opens the previous menu in the queue, or returns to the main menu
    /// </summary>
    public void ShowPreviousMenu()
    {
        _cancelNextFrame = true;
    }

    /// <summary>
    /// If there are menus, start the menu process, otherwise, continue normally
    /// </summary>
    internal bool TryStartGame(int slot, bool isContinue)
    {
        _currentSlot = slot;
        _isContinue = isContinue;

        if (CurrentMenuCollection.IsEmpty)
            return true;

        if (isContinue)
            CoreCache.SaveData.LoadGame(slot);

        StartMenu();
        return false;
    }

    /// <summary>
    /// Whenever new game or continue is pressed, open the menus
    /// </summary>
    private void StartMenu()
    {
        _inputBlocker.BlockOtherInput();
        
        _slotsMenuCache.Value.SetActive(false);
        CurrentMenuCollection.StartMenu();
    }

    /// <summary>
    /// Whenever 'A' is pressed on the final menu, actually start the game
    /// </summary>
    private void OnFinishMenu()
    {
        _inputBlocker.UnblockOtherInput();

        AllowGameStart = true;

        if (_isContinue)
        {
            //CoreCache.SaveData.SaveGame(_currentSlot);
            _mainMenuCache.Value.LoadGame(_currentSlot);
        }
        else
        {
            _mainMenuCache.Value.NewGame(_currentSlot);
        }

        AllowGameStart = false;
    }

    /// <summary>
    /// Whenever 'B' is pressed on the first menu, return to slots screen
    /// </summary>
    private void OnCancelMenu()
    {
        _inputBlocker.UnblockOtherInput();

        _mainMenuCache.Value.OpenSlotMenu();
        _mainMenuCache.Value.slotsList.SelectElement(_currentSlot);
    }

    /// <summary>
    /// Creates a new empty menu UI
    /// </summary>
    internal MenuComponent CreateBaseMenu(string modName, bool isFirst, bool isLast)
    {
        // Duplicate slot menu
        GameObject settingsMenu = Object.Instantiate(_slotsMenuCache.Value, _slotsMenuCache.Value.transform.parent);
        settingsMenu.name = $"Menu {modName}";

        // Remove slot menu stuff
        Object.Destroy(settingsMenu.transform.Find("SlotsList").gameObject);

        // Set header text
        UIPixelTextWithShadow headerText = settingsMenu.transform.Find("Header").GetComponent<UIPixelTextWithShadow>();
        LocalizationHandler.AddPixelTextLocalizer(headerText, modName + " {0}", "title");

        // Change text of buttons
        var newBtn = settingsMenu.transform.Find("Buttons/Button A/New").gameObject;
        var continueBtn = settingsMenu.transform.Find("Buttons/Button A/Continue").gameObject;
        var cancelBtn = settingsMenu.transform.Find("Buttons/Back").gameObject;

        newBtn.SetActive(true);
        continueBtn.SetActive(false);
        cancelBtn.SetActive(true);

        LocalizationHandler.AddPixelTextLocalizer(
            newBtn.GetComponentInChildren<UIPixelTextWithShadow>(), isLast ? (_isContinue ? "btncnt" : "btnbgn") : "btnnxt");
        LocalizationHandler.AddPixelTextLocalizer(
            cancelBtn.GetComponentInChildren<UIPixelTextWithShadow>(), isFirst ? "btncnc" : "btnprv");

        // Create holder for options and all settings
        UIModder.Create(new RectCreationOptions()
        {
            Name = "Main Section",
            Parent = settingsMenu.transform,
            Position = new Vector2(0, -30),
            Size = new Vector2(1800, 750)
        });

        return settingsMenu.AddComponent<MenuComponent>();
    }

#if DEBUG
    /// <summary>
    /// Register test menus
    /// </summary>
    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterNewGameMenu(new TestMenu("New game 1st menu", 10, true));
        provider.RegisterNewGameMenu(new TestMenu("New game 2nd menu", 21, true));

        provider.RegisterLoadGameMenu(new TestMenu("Load game menu", 50, false));
    }
#endif
}
