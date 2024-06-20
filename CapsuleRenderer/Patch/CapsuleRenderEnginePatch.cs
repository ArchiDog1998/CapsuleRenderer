using Grasshopper.GUI.Canvas;
using HarmonyLib;
using System.Drawing;

namespace CapsuleRenderer.Patch;

[HarmonyPatch(typeof(GH_CapsuleRenderEngine))]

internal class CapsuleRenderEnginePatch
{
    [HarmonyPatch(nameof(GH_CapsuleRenderEngine.CreateRoundedRectangle), typeof(RectangleF), typeof(float), typeof(float), typeof(float), typeof(float))]
    [HarmonyPatch(nameof(GH_CapsuleRenderEngine.CreateJaggedRectangle), typeof(RectangleF), typeof(float), typeof(float), typeof(float), typeof(float), typeof(bool), typeof(bool))]
    static void Prefix(RectangleF rec, ref float R0, ref float R1, ref float R2, ref float R3)
    {
        AdjustRadius(ref R0, ref R1, ref R2, ref R3, rec.Width, rec.Height);
    }

    private static void AdjustRadius(ref float upperLeft, ref float upperRight, ref float lowerRight, ref float lowerLeft, float width, float height)
    {
        AdjustRadius(ref upperLeft, ref upperRight, width);
        AdjustRadius(ref upperRight, ref lowerRight, height);
        AdjustRadius(ref lowerRight, ref lowerLeft, width);
        AdjustRadius(ref lowerLeft, ref upperLeft, height);
    }

    private static void AdjustRadius(ref float r0, ref float r1, float length)
    {
        if (r0 + r1 <= length) return;

        float resize = length / (r0 + r1);

        r0 *= resize;
        r1 *= resize;
    }
}
