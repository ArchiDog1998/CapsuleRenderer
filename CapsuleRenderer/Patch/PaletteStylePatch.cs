using Grasshopper.GUI.Canvas;
using HarmonyLib;

namespace CapsuleRenderer.Patch;

[HarmonyPatch(typeof(GH_PaletteStyle))]

internal class PaletteStylePatch
{
    [HarmonyPatch(nameof(GH_PaletteStyle.CreateBrush))]

    static void Prefix(ref float zoom)
    {
        if (Datas.IsCapsuleFlat)
        {
            zoom = 0.2f;
        }
    }
}
