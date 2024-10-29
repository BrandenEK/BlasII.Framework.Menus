using BlasII.ModdingAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlasII.Framework.Menus;

internal class MenuCollection(IEnumerable<ModMenu> menus, Action onFinish, Action onCancel)
{
    private readonly List<ModMenu> _menus = new(menus.OrderBy(x => x.Priority));
    private readonly Action onFinish = onFinish;
    private readonly Action onCancel = onCancel;

    private int _currentMenu = -1;

    public bool IsEmpty => _menus.Count == 0;
    public bool IsActive => _currentMenu != -1;
    public ModMenu CurrentMenu => _menus[_currentMenu];

    /// <summary>
    /// Activates a specific menu
    /// </summary>
    private void ShowMenu(int index)
    {
        ModMenu menu = _menus[index];
        menu.UI?.gameObject.SetActive(true);
        menu.OnShow();

        _currentMenu = index;
    }

    /// <summary>
    /// Deactivates a specific menu
    /// </summary>
    private void HideMenu(int index)
    {
        ModMenu menu = _menus[index];
        menu.OnHide();
        menu.UI?.gameObject.SetActive(false);
    }

    /// <summary>
    /// Opens the first menu and initializes all menu UI
    /// </summary>
    public void StartMenu()
    {
        if (IsEmpty)
            return;

        for (int i = 0; i < _menus.Count; i++)
        {
            _menus[i].CreateUI(i == 0, i == _menus.Count - 1);
            _menus[i].OnStart();
        }

        ShowMenu(0);
    }

    /// <summary>
    /// Hides the current menu and shows the next one.  Calls onFinish at the end
    /// </summary>
    public void ShowNextMenu()
    {
        AudioHelper.PlayEffectUI(AudioHelper.SfxUI.OpenMenu);
        //Main.MenuFramework.SoundPlayer.Play(SoundPlayer.SfxType.EquipItem);

        // If there is another menu, move to it
        if (_currentMenu < _menus.Count - 1)
        {
            HideMenu(_currentMenu);
            ShowMenu(_currentMenu + 1);
            return;
        }

        _menus[_currentMenu].OnHide();

        // Otherwise, finish the menu
        _currentMenu = -1;
        foreach (var menu in _menus)
            menu.OnFinish();
        onFinish();
    }

    /// <summary>
    /// Hides the current menu and shows the previous one.  Calls onCancel at the beginning
    /// </summary>
    public void ShowPreviousMenu()
    {
        AudioHelper.PlayEffectUI(AudioHelper.SfxUI.CloseMenu);
        //Main.MenuFramework.SoundPlayer.Play(SoundPlayer.SfxType.UnequipItem);
        HideMenu(_currentMenu);

        // If there is another menu, move to it
        if (_currentMenu > 0)
        {
            ShowMenu(_currentMenu - 1);
            return;
        }

        // Otherwise, cancel the menu
        _currentMenu = -1;
        foreach (var menu in _menus)
            menu.OnCancel();
        onCancel();
    }

    /// <summary>
    /// Only call the OnFinish method once NewGame or LoadGame has been called
    /// </summary>
    public void DelayedFinish()
    {
        // Might not be needed if the OnFinish needs to be before newgame is called

        //foreach (var menu in _menus)
        //    menu.OnFinish();
    }
}
