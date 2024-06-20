using Grasshopper;
using SimpleGrasshopper.Attributes;

namespace CapsuleRenderer;

internal static partial class Datas
{
    [Config("Render Highlight")]
    public static bool IsRenderHighLight
    {
        get => CentralSettings.CapsuleHighlight;
        set => CentralSettings.CapsuleHighlight = value;
    }

    [Config("Render Inner Out line")]
    public static bool IsRenderInnerOutLine
    {
        get => CentralSettings.CapsuleShine;
        set => CentralSettings.CapsuleShine = value;
    }

    [Setting, Config("Render Flat")]
    private static readonly bool _IsCapsuleFlat = true;

    [Setting, Config("Use Text Capsule (Normal)")]
    private static readonly bool _UseTextCapsule = true;

    [Setting, Config("Use Text Vertical Capsule")]
    private static readonly bool _UseVerticalTextCap = true;

    [Range(0, 5)]
    [Setting, Config("Out Line Width")]
    private static readonly int _OutLineWidth = 1;

    [Range(0, 12)]
    [Setting, Config("Capsule Radius")]
    private static readonly int _CapsuleRadius = 6;

    [Range(0, 50)]
    [Setting, Config("Capsule Highlight")]
    private static readonly int _CapsuleHighLight = 8;


    [Range(0, 50)]
    [Setting, Config("Capsule Offset Distance")]
    private static readonly double _CapsuleOffsetDistance = 2;
}
