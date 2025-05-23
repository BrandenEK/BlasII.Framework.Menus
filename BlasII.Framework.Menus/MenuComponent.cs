﻿using BlasII.Framework.Menus.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace BlasII.Framework.Menus;

[MelonLoader.RegisterTypeInIl2Cpp]
internal class MenuComponent : MonoBehaviour
{
    private Clickable _clickedSetting = null;

    private readonly List<Clickable> _clickables = new();
    private ICursorController _cursorController;

    private void OnEnable()
    {
        _cursorController ??= _clickables.Count > 0
            ? new RealCursor(transform)
            : new FakeCursor();

        _cursorController.UpdatePosition(Input.mousePosition);
    }

    private void OnDisable()
    {
        _clickedSetting?.OnUnclick();
        _clickedSetting = null;
    }

    private void Update()
    {
        _cursorController.UpdatePosition(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            HandleTab();
        }
    }

    /// <summary>
    /// Marks the option as selected
    /// </summary>
    private void SelectOption(Clickable clickable)
    {
        _clickedSetting = clickable;
        clickable.OnClick();
    }

    /// <summary>
    /// Marks the current option as unselected
    /// </summary>
    private void DeselectCurrentOption()
    {
        _clickedSetting?.OnUnclick();
        _clickedSetting = null;
    }

    /// <summary>
    /// Deselect the current option, the select one if the cursor is hovering over it
    /// </summary>
    private void HandleClick()
    {
        DeselectCurrentOption();

        foreach (var clickable in _clickables)
        {
            if (clickable.Rect.OverlapsPoint(Input.mousePosition))
            {
                SelectOption(clickable);
                break;
            }
        }
    }

    /// <summary>
    /// Deselect the current tabable option, then select the next tabable one
    /// </summary>
    private void HandleTab()
    {
        if (_clickedSetting == null || !_clickedSetting.AllowTab)
            return;

        int currIdx = _clickables.IndexOf(_clickedSetting);
        DeselectCurrentOption();

        while (true)
        {
            if (++currIdx >= _clickables.Count)
                currIdx = 0;

            if (!_clickables[currIdx].AllowTab)
                continue;

            SelectOption(_clickables[currIdx]);
            break;
        }
    }

    public void AddClickable(RectTransform rect, bool allowTab, System.Action onClick, System.Action onUnclick)
    {
        _clickables.Add(new Clickable(rect, allowTab, onClick, onUnclick));
    }

    class Clickable(RectTransform rect, bool allowTab, System.Action onClick, System.Action onUnclick)
    {
        private readonly System.Action _onClick = onClick;
        private readonly System.Action _onUnclick = onUnclick;

        public RectTransform Rect { get; } = rect;
        public bool AllowTab { get; } = allowTab;

        internal void OnClick() => _onClick?.Invoke();

        internal void OnUnclick() => _onUnclick?.Invoke();
    }
}
