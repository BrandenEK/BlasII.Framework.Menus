﻿using BlasII.Framework.UI;
using UnityEngine;

namespace BlasII.Framework.Menus;

internal interface ICursorController
{
    public void UpdatePosition(Vector2 mousePosition);
}

internal class RealCursor : ICursorController
{
    private readonly RectTransform _cursor;

    public RealCursor(Transform menu)
    {
        _cursor = UIModder.Create(new RectCreationOptions()
        {
            Name = "Cursor",
            Parent = menu,
            XRange = Vector2.zero,
            YRange = Vector2.zero,
            Pivot = new Vector2(0, 1),
            Size = Main.MenuFramework.IconLoader.Cursor.rect.size
        }).AddImage(new ImageCreationOptions()
        {
            Sprite = Main.MenuFramework.IconLoader.Cursor
        }).rectTransform;
    }

    public void UpdatePosition(Vector2 mousePosition)
    {
        Vector2 cursorPosition = new(
            Mathf.Clamp(mousePosition.x, 0, Screen.width - _cursor.sizeDelta.x / 2) / Screen.width * 1920,
            Mathf.Clamp(mousePosition.y, 0 + _cursor.sizeDelta.y / 2, Screen.height) / Screen.height * 1080);

        _cursor.anchoredPosition = cursorPosition;
    }
}

internal class FakeCursor : ICursorController
{
    public void UpdatePosition(Vector2 mousePosition) { }
}
