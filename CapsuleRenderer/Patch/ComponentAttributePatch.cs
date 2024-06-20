using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CapsuleRenderer.Patch;

[HarmonyPatch(typeof(GH_ComponentAttributes))]
internal class ComponentAttributePatch
{
    [HarmonyPatch("RenderComponentCapsule", typeof(GH_Canvas), typeof(Graphics), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool))]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var result = new List<CodeInstruction>();
        bool find = !Data.FixJaggedEdges;
        foreach (var instruction in instructions)
        {
            result.Add(instruction);

            if (find) continue;
            if (instruction.opcode != OpCodes.Stloc_3) continue;
            find = true;

            result.Add(new(OpCodes.Ldloc_2));
            result.Add(new(OpCodes.Ldarga_S, 4));
            result.Add(new(OpCodes.And));
            result.Add(new(OpCodes.Stloc_2));

            result.Add(new(OpCodes.Ldloc_3));
            result.Add(new(OpCodes.Ldarga_S, 4));
            result.Add(new(OpCodes.And));
            result.Add(new(OpCodes.Stloc_3));
        }
        return result;
    }
}
