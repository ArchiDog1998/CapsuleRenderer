using Grasshopper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapsuleRenderer
{
    internal static class Datas
    {
        private static readonly string _nameSpace = "CapsuleRenderer.";

        private static readonly bool isRenderHighLightDefault = false;
        internal static bool IsRenderHighLight 
        {
            get => Instances.Settings.GetValue(_nameSpace + nameof(IsRenderHighLight), isRenderHighLightDefault);
            set
            {
                Instances.Settings.SetValue(_nameSpace + nameof(IsRenderHighLight), value);
                Instances.ActiveCanvas.Refresh();
            }
        }

        private static readonly bool isRenderInnerOutLineDefault = true;
        internal static bool IsRenderInnerOutLine
        {
            get => Instances.Settings.GetValue(_nameSpace + nameof(IsRenderInnerOutLine), isRenderInnerOutLineDefault);
            set
            {
                Instances.Settings.SetValue(_nameSpace + nameof(IsRenderInnerOutLine), value);
                Instances.ActiveCanvas.Refresh();
            }
        }

        private static readonly bool isCapsuleFlatDefault = true;
        internal static bool IsCapsuleFlat
        {
            get => Instances.Settings.GetValue(_nameSpace + nameof(IsCapsuleFlat), isCapsuleFlatDefault);
            set
            {
                Instances.Settings.SetValue(_nameSpace + nameof(IsCapsuleFlat), value);
                Instances.ActiveCanvas.Refresh();
            }
        }

        internal static readonly int outLineWidthDefault = 1;
        internal static int OutLineWidth
        {
            get => Instances.Settings.GetValue(_nameSpace + nameof(OutLineWidth), outLineWidthDefault);
            set
            {
                Instances.Settings.SetValue(_nameSpace + nameof(OutLineWidth), value);
                Instances.ActiveCanvas.Refresh();
            }
        }

        internal static readonly int capsuleRadiusDefault = 6;
        internal static int CapsuleRadius
        {
            get => Instances.Settings.GetValue(_nameSpace + nameof(CapsuleRadius), capsuleRadiusDefault);
            set
            {
                Instances.Settings.SetValue(_nameSpace + nameof(CapsuleRadius), value);
                Instances.ActiveCanvas.Refresh();
            }
        }

        internal static readonly int capsuleHighLightDefault = 8;
        internal static int CapsuleHighLight
        {
            get => Instances.Settings.GetValue(_nameSpace + nameof(CapsuleHighLight), capsuleHighLightDefault);
            set
            {
                Instances.Settings.SetValue(_nameSpace + nameof(CapsuleHighLight), value);
                Instances.ActiveCanvas.Refresh();
            }
        }

        internal static readonly double _capsuleOffsetDistanceDefault = 2;
        internal static double CapsuleOffsetDistance
        {
            get => Instances.Settings.GetValue(_nameSpace + nameof(CapsuleOffsetDistance), _capsuleOffsetDistanceDefault);
            set
            {
                Instances.Settings.SetValue(_nameSpace + nameof(CapsuleOffsetDistance), value);
                Instances.ActiveCanvas.Refresh();

            }
        }
    }
}
