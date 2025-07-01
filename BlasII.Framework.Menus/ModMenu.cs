using BlasII.ModdingAPI;
using BlasII.ModdingAPI.Helpers;
using BlasII.ModdingAPI.Input;
using System;
using UnityEngine;

namespace BlasII.Framework.Menus;

/// <summary>
/// The abstract form of an in-game menu
/// </summary>
public abstract class ModMenu
{
    internal BlasIIMod OwnerMod { get; set; }
    internal MenuComponent UI { get; private set; }

    /// <summary>
    /// Determines which order menus will be opened
    /// </summary>
    protected internal abstract int Priority { get; }

    /// <summary>
    /// Called when the menus are first opened
    /// </summary>
    public virtual void OnStart() { }

    /// <summary>
    /// Called when the menus are closed by starting the game
    /// </summary>
    public virtual void OnFinish() { }

    /// <summary>
    /// Called when the menus are closed by returning to the title screen
    /// </summary>
    public virtual void OnCancel() { }

    /// <summary>
    /// Called when this specific menu is opened
    /// </summary>
    public virtual void OnShow() { }

    /// <summary>
    /// Called when this specific menu is closed
    /// </summary>
    public virtual void OnHide() { }

    /// <summary>
    /// Called every frame while this menu is active.
    /// By default it checks for enter/cancel input
    /// </summary>
    public virtual void OnUpdate()
    {
        if (Main.MenuFramework.InputHandler.GetButtonDown(ButtonType.UIConfirm))
        {
            Main.MenuFramework.ShowNextMenu();
        }
        else if (Main.MenuFramework.InputHandler.GetButtonDown(ButtonType.UICancel))
        {
            Main.MenuFramework.ShowPreviousMenu();
        }

        //if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        //{
        //    Main.MenuFramework.ShowNextMenu();
        //}
        //else if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Main.MenuFramework.ShowPreviousMenu();
        //}
    }

    /// <summary>
    /// Called when one of this menu's options changes its value.
    /// By default it plays a sound effect
    /// </summary>
    public virtual void OnOptionsChanged(string option)
    {
        AudioHelper.PlayEffectUI(AudioHelper.SfxUI.ChangeSelection);
    }

    /// <summary>
    /// Creates the specific UI for this menu if it doesnt already exist
    /// </summary>
    internal void CreateUI(bool isFirst, bool isLast)
    {
        if (UI != null)
            return;

        UI = Main.MenuFramework.CreateBaseMenu(OwnerMod.Name, isFirst, isLast);
        CreateUI(UI.transform.Find("Main Section"));
    }

    /// <summary>
    /// Adds all menu-specific UI to its base UI object
    /// </summary>
    protected internal abstract void CreateUI(Transform ui);

    /// <summary>
    /// Adds an event to occur whenever this object is clicked on
    /// </summary>
    public void AddClickable(RectTransform rect, bool allowTab, Action onClick) => UI.AddClickable(rect, allowTab, onClick, null);

    /// <summary>
    /// Adds an event to occur whenever this object is clicked on or clicked off
    /// </summary>
    public void AddClickable(RectTransform rect, bool allowTab, Action onClick, Action onUnclick) => UI.AddClickable(rect, allowTab, onClick, onUnclick);
}
