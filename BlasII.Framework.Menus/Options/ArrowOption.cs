﻿using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlasII.Framework.Menus.Options;

/// <summary>
/// An option that can be one of multiple different values
/// </summary>
[MelonLoader.RegisterTypeInIl2Cpp]
public class ArrowOption : MonoBehaviour
{
    private ModMenu _menu;
    private TextMeshProUGUI _text;
    private Image _leftArrow;
    private Image _rightArrow;

    private string[] _options;
    private int _currentOption = 0;

    /// <summary>
    /// The option that is currently selected
    /// </summary>
    public int CurrentOption
    {
        get => _currentOption;
        set
        {
            _currentOption = value;
            UpdateStatus();
        }
    }

    /// <summary>
    /// Change the option by a specific amount
    /// </summary>
    public void ChangeOption(int change)
    {
        int newOption = _currentOption + change;
        if (newOption < 0 || newOption >= _options.Length)
            return;

        CurrentOption = newOption;
        _menu.OnOptionsChanged();
    }

    /// <summary>
    /// Initializes the arrow option
    /// </summary>
    public void Initialize(ModMenu menu, TextMeshProUGUI optionText, Image leftArrow, Image rightArrow, string[] options)
    {
        _menu = menu;
        _text = optionText;
        _leftArrow = leftArrow;
        _rightArrow = rightArrow;

        _options = options;
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        _text.text = _menu.OwnerMod.LocalizationHandler.Localize(_options[_currentOption]);
        _leftArrow.sprite = _currentOption == 0
            ? Main.MenuFramework.IconLoader.ArrowLeftOff
            : Main.MenuFramework.IconLoader.ArrowLeftOn;
        _rightArrow.sprite = _currentOption == _options.Length - 1
            ? Main.MenuFramework.IconLoader.ArrowRightOff
            : Main.MenuFramework.IconLoader.ArrowRightOn;
    }
}
