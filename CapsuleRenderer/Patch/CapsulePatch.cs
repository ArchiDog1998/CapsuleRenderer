using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using HarmonyLib;
using System;
using System.Drawing;

namespace CapsuleRenderer.Patch;

[HarmonyPatch(typeof(GH_Capsule))]
internal class CapsulePatch
{
    [HarmonyPatch(nameof(GH_Capsule.CreateCapsule), typeof(Rectangle), typeof(GH_Palette))]
    static bool Prefix(ref GH_Capsule __result, Rectangle box, GH_Palette palette)
    {
        var highLight = Math.Min(box.Height - Data.CapsuleRadius, Data.CapsuleHighLight);
        __result = GH_Capsule.CreateCapsule(box, palette, Data.CapsuleRadius, highLight);
        return false;
    }

    [HarmonyPatch(nameof(GH_Capsule.CreateTextCapsule), typeof(RectangleF), typeof(RectangleF), typeof(GH_Palette), typeof(string), typeof(Font), typeof(GH_Orientation), typeof(int), typeof(int))]
    static bool Prefix(ref GH_Capsule __result, RectangleF box, RectangleF textbox, string text, Font font, int radius, int highlight)
    {
        if (!Data.UseTextCapsule) return true;
        if (!Data.UseVerticalTextCap)
        {
            ChangeRectF(ref box);
            ChangeRectF(ref textbox);
        }

        if (highlight == 6)
        {
            highlight = Math.Min((int)box.Height - Data.CapsuleRadius, Data.CapsuleHighLight);
        }
        if (radius == 3)
        {
            radius = Data.CapsuleRadius;
        }
        __result = GH_Capsule.CreateTextCapsule(GH_Convert.ToRectangle(box), GH_Convert.ToRectangle(textbox), GH_Palette.Normal, text, font,
            Data.UseVerticalTextCap ? GH_Orientation.vertical_center : GH_Orientation.horizontal_center, radius, highlight);
        return false;
    }

    private static void ChangeRectF(ref RectangleF rect)
    {
        const float height = 24;
        rect = new RectangleF(rect.X, rect.Y + (rect.Height - height) / 2, rect.Width, height);
    }
}
