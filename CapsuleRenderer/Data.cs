using Grasshopper;
using SimpleGrasshopper.Attributes;
using System;

namespace CapsuleRenderer;

internal static partial class Data
{
    [Config("Render Highlight")]
    public static bool IsRenderHighLight
    {
        get => CentralSettings.CapsuleHighlight;
        set
        {
            CentralSettings.CapsuleHighlight = value;
            OnIsRenderHighLightChanged?.Invoke(value);
            OnPropertyChanged?.Invoke(nameof(IsRenderHighLight), value);
        }
    }
    public static event Action<bool> OnIsRenderHighLightChanged;

    [Config("Render Inner Out line")]
    public static bool IsRenderInnerOutLine
    {
        get => CentralSettings.CapsuleShine;
        set
        {
            CentralSettings.CapsuleShine = value;
            OnIsRenderInnerOutLineChanged?.Invoke(value);
            OnPropertyChanged?.Invoke(nameof(IsRenderInnerOutLine), value);
        }
    }
    public static event Action<bool> OnIsRenderInnerOutLineChanged;

    [Setting, Config("Render Flat")]
    private static readonly bool _IsCapsuleFlat = true;

    [Setting, Config("Use Text Capsule (Normal)")]
    private static readonly bool _UseTextCapsule = true;

    [Setting, Config("Use Text Vertical Capsule")]
    private static readonly bool _UseVerticalTextCap = true;

    [Range(0, 15)]
    [Setting, Config("Out Line Width", parent: "Render Inner Out line")]
    private static readonly int _OutLineWidth = 3;

    [Range(0, 50)]
    [Setting, Config("Capsule Highlight", parent: "Render Highlight")]
    private static readonly int _CapsuleHighLight = 8;

    [Range(0, 12)]
    [Setting, Config("Capsule Radius")]
    private static readonly int _CapsuleRadius = 6;

    [Setting, Config("Fix Jagged Edges (Restart required)")]
    private static readonly bool _FixJaggedEdges = true;

    static Data()
    {
        OnPropertyChanged += (s, e) => Instances.ActiveCanvas.Refresh();
    }
}
