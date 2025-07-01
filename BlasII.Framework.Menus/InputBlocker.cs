using BlasII.ModdingAPI;
using Il2CppTGK.Game;
using Il2CppTGK.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlasII.Framework.Menus;

internal class InputBlocker
{
    // 8192 = UI Confirm
    // 524288 = UI Cancel
    private readonly IEnumerable<InputData> _inputs;

    public InputBlocker()
    {
        // Store inputs to block/unblock
        IEnumerable<InputData> inputs = Resources.FindObjectsOfTypeAll<InputData>().OrderBy(x => x.mask);
        _inputs = inputs.Where(x => x.mask != 8192 && x.mask != 524288);
    }

    public void BlockOtherInput()
    {
        ModLog.Info("Blocking input other than confirm/cancel");

        CoreCache.Input.ClearAllInputBlocks();
        foreach (var input in _inputs)
            CoreCache.Input.BlockInputAction(input);
    }

    public void UnblockOtherInput()
    {
        ModLog.Info("Unblocking input other than confirm/cancel");

        foreach (var input in _inputs)
            CoreCache.Input.UnblockInputAction(input);
    }
}
