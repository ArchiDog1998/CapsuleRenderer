using Grasshopper.GUI.Canvas;
using HarmonyLib;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Emit;

namespace CapsuleRenderer.Patch;

[HarmonyPatch(typeof(GH_CapsuleRenderEngine))]
internal class CapsuleRenderEnginePatch
{
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(GH_CapsuleRenderEngine.CreateRoundedRectangle), typeof(RectangleF), typeof(float), typeof(float), typeof(float), typeof(float))]
    static IEnumerable<CodeInstruction> TranspilerRounded(IEnumerable<CodeInstruction> instructions)
    {
        return MyTranspiler(instructions);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(GH_CapsuleRenderEngine.CreateJaggedRectangle), typeof(RectangleF), typeof(float), typeof(float), typeof(float), typeof(float), typeof(bool), typeof(bool))]
    static IEnumerable<CodeInstruction> TranspilerJagged(IEnumerable<CodeInstruction> instructions)
    {
        return MyTranspiler(instructions);
    }

    static IEnumerable<CodeInstruction> MyTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        byte state = 0; //0 : before; 1 : skip; 2 : after
        var result = new List<CodeInstruction>();
        foreach (var item in instructions)
        {
            switch (state)
            {
                case 0:
                    if (item.opcode == OpCodes.Ldloc_2)
                    {
                        state = 1;
                    }
                    else
                    {
                        result.Add(item);
                    }
                    break;
                case 1:
                    if (item.opcode == OpCodes.Stloc_S
                        && item.operand is LocalBuilder builder
                        && builder.LocalIndex == 5)
                    {
                        state = 2;
                    }
                    break;

                default:
                    result.Add(item);
                    break;
            }
        }

        return result;
    }

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

    [HarmonyPatch("InnerContourPen")]
    static void Postfix(ref Pen __result)
    {
        __result.Width = Data.OutLineWidth;
    }
}
