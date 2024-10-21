using MelonLoader;

namespace BlasII.Framework.Menus;

internal class Main : MelonMod
{
    public static MenuFramework MenuFramework { get; private set; }

    public override void OnLateInitializeMelon()
    {
        MenuFramework = new MenuFramework();
    }
}