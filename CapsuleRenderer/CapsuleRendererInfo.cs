using Grasshopper.GUI;
using Grasshopper.Kernel;
using HarmonyLib;
using System;
using System.Drawing;

namespace CapsuleRenderer;

public class CapsuleRendererInfo : GH_AssemblyInfo
{
    public override string Name => "Capsule Renderer";

    //Return a 24x24 pixel bitmap to represent this GHA library.
    public override Bitmap Icon => Properties.Resources.CapsuleRendererIcon_24;

    //Return a short string describing the purpose of this GHA library.
    public override string Description => "Advanced options for capsule rendering. Icon was made by ZCS.";

    public override Guid Id => new ("46D4702E-3C86-449A-ACAE-7348E5A9C098");

    //Return a string identifying you or your company.
    public override string AuthorName => "秋水";

    //Return a string representing your preferred contact details.
    public override string AuthorContact => "1123993881@qq.com";

    public override string Version => "0.9.2";
}

partial class SimpleAssemblyPriority
{
    protected override void DoWithEditor(GH_DocumentEditor editor)
    {
        var harmony = new Harmony("Grasshopper.CapsuleRenderer");
        harmony.PatchAll();

        base.DoWithEditor(editor);
    }
}