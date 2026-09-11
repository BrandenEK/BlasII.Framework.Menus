using BlasII.ModdingAPI;
using System.Linq;
using UnityEngine;

namespace BlasII.Framework.Menus.Extensions;

internal static class RectExtensions
{
    public static bool OverlapsPoint(this RectTransform rect, Vector2 point)
    {
        point = new Vector2(point.x * 1920 / Screen.width, point.y * 1080 / Screen.height);
        return RectTransformUtility.RectangleContainsScreenPoint(rect, point, UICamera);
    }

    private static Camera x_camera;
    private static Camera UICamera
    {
        get
        {
            if (x_camera != null)
                return x_camera;

            x_camera = Object.FindObjectsOfType<Camera>().FirstOrDefault(x => x.name == "UI Pixel Perfect Camera");

            if (x_camera == null)
                ModLog.Error("Failed to cache UI camera");

            return x_camera;
        }
    }
}
