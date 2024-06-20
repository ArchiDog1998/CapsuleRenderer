using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using HarmonyLib;
using Rhino.Geometry;
using System;
using System.Drawing;

namespace CapsuleRenderer.Patch;

[HarmonyPatch(typeof(GH_Capsule))]
internal class CapsulePatch
{
    [HarmonyPatch(nameof(GH_Capsule.CreateCapsule), typeof(Rectangle), typeof(GH_Palette))]
    static bool Prefix(ref GH_Capsule __result, Rectangle box, GH_Palette palette)
    {
        int highLight = Math.Min(box.Height - Datas.CapsuleRadius, Datas.CapsuleHighLight);
        __result = GH_Capsule.CreateCapsule(box, palette, Datas.CapsuleRadius, highLight);
        return false;
    }

    [HarmonyPatch(nameof(GH_Capsule.CreateCapsule), typeof(Rectangle), typeof(GH_Palette), typeof(string), typeof(Font), typeof(GH_Orientation), typeof(int), typeof(int))]

    static bool Prefix(ref GH_Capsule __result, RectangleF box, RectangleF textbox, GH_Palette palette, string text, Font font, GH_Orientation orientation, int radius, int highlight)
    {
        if (!Datas.UseTextCapsule) return true;
        if (!Datas.UseVerticalTextCap)
        {
            ChangeRectF(ref box);
            ChangeRectF(ref textbox);
        }

        highlight = (int)Math.Min(box.Height - Datas.CapsuleRadius, Datas.CapsuleHighLight);
        __result = GH_Capsule.CreateTextCapsule(GH_Convert.ToRectangle(box), GH_Convert.ToRectangle(textbox), GH_Palette.Normal, text, font,
            Datas.UseVerticalTextCap ? GH_Orientation.vertical_center : GH_Orientation.horizontal_center, Datas.CapsuleRadius, highlight);
        return false;
    }

    private static void ChangeRectF(ref RectangleF rect)
    {
        const float height = 24;
        rect = new RectangleF(rect.X, rect.Y + (rect.Height - height) / 2, rect.Width, height);
    }
}
