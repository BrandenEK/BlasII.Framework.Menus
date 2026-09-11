using BlasII.ModdingAPI;
using System.Linq;
using UnityEngine;

namespace BlasII.Framework.Menus.Extensions;

internal static class RectExtensions
{
    public static bool OverlapsPoint(this RectTransform rect, Vector2 point)
    {
        point = new Vector2(point.x * 1920 / Screen.width, point.y * 1080 / Screen.height);

        Camera cam = Object.FindObjectsOfType<Camera>().First(x => x.name == "UI Pixel Perfect Camera");
        return RectTransformUtility.RectangleContainsScreenPoint(rect, point, cam);

        //float xScale = (float)Screen.width / 1920;
        //var scaling = new Vector3(xScale, xScale, (Screen.height - 1080 * xScale) * 0.5f);

        var position = rect.position;//Camera.main.WorldToScreenPoint(rect.position);
        //position = new Vector2(position.x * scaling.x, position.y * scaling.y + scaling.z);
        ModLog.Warn(rect.name + ": " + position);
        var size = new Vector2(rect.rect.width/* * scaling.x*/, rect.rect.height/* * scaling.y*/);

        float leftBound = position.x + size.x * -rect.pivot.x;
        float rightBound = position.x + size.x * (1 - rect.pivot.x);
        float lowerBound = position.y + size.y * -rect.pivot.y;
        float upperBound = position.y + size.y * (1 - rect.pivot.y);

        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        ModLog.Info($"Mouse: {point}, BL corner: ({leftBound}, {lowerBound}), TR corner: ({rightBound}, {upperBound})");
        ModLog.Info($"Mouse: {point}, BL corner: {corners[0]}, TR corner: {corners[2]}");
        return point.x >= leftBound && point.x <= rightBound && point.y >= lowerBound && point.y <= upperBound;
    }
}
