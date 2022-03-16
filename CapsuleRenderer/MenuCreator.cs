using Grasshopper.GUI.Base;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapsuleRenderer
{
    internal static class MenuCreator
    {
        public static ToolStripMenuItem CreateMajorMenu()
        {
            ToolStripMenuItem major = new ToolStripMenuItem("Capsule Renderer") { ToolTipText = "Advanced options for capsule rendering." };

            major.DropDownItems.Add(CreateCheckBox("Render Highlight", Datas.IsRenderHighLight, (boolean) => Datas.IsRenderHighLight = boolean));
            major.DropDownItems.Add(CreateCheckBox("Render Inner Out line", Datas.IsRenderInnerOutLine, (boolean) => Datas.IsRenderInnerOutLine = boolean));
            major.DropDownItems.Add(CreateCheckBox("Render Flat", Datas.IsCapsuleFlat, (boolean) => Datas.IsCapsuleFlat = boolean));

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Out Line Width", Datas.OutLineWidth, (v) => Datas.OutLineWidth = (int)v, Datas.outLineWidthDefault, 5, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Capsule Radius", Datas.CapsuleRadius, (v) => Datas.CapsuleRadius = (int)v, Datas.capsuleRadiusDefault, 12, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Capsule Highlight", Datas.OutLineWidth, (v) => Datas.CapsuleHighLight = (int)v, Datas.capsuleHighLightDefault, 50, 0);
            return major;
        }

        private static ToolStripMenuItem CreateCheckBox(string itemName, bool valueDefault, Action<bool> valueChange)
        {

            ToolStripMenuItem click = new ToolStripMenuItem(itemName);
            CreateCheckBox(ref click, valueDefault, valueChange);
            return click;
        }
        private static void CreateCheckBox(ref ToolStripMenuItem click, bool valueDefault, Action<bool> valueChange)
        {
            click.Checked = valueDefault;
            CreateCheckBox(ref click, valueChange);
        }
        private static void CreateCheckBox(ref ToolStripMenuItem click, Action<bool> valueChange)
        {
            click.Click += (sender, e) =>
            {
                ToolStripMenuItem item = (ToolStripMenuItem)sender;
                item.Checked = !item.Checked;
                valueChange.Invoke(item.Checked);
                if (item.HasDropDownItems)
                {
                    foreach (ToolStripItem it in item.DropDownItems)
                    {
                        it.Enabled = item.Checked;
                    }
                }
            };
            click.DropDownOpening += Click_DropDownOpening;
        }

        private static void Click_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.DropDownOpening -= Click_DropDownOpening;

            if (item.HasDropDownItems)
            {
                foreach (ToolStripItem it in item.DropDownItems)
                {
                    it.Enabled = item.Checked;
                }
            }
        }


        private static void CreateTextLabel(ToolStripMenuItem item, string name, string tooltips = null)
        {
            ToolStripLabel textBox = new ToolStripLabel(name);
            textBox.TextAlign = ContentAlignment.MiddleCenter;
            textBox.Font = new Font(textBox.Font, FontStyle.Bold);
            if (!string.IsNullOrEmpty(tooltips))
                textBox.ToolTipText = tooltips;
            item.DropDownItems.Add(textBox);
        }

        private static void CreateNumberBox(ToolStripMenuItem item, string itemName, double originValue, Action<double> valueChange, double valueDefault, double Max, double Min, int decimalPlace = 0)
        {
            item.DropDown.Closing -= DropDown_Closing;
            item.DropDown.Closing += DropDown_Closing;

            CreateTextLabel(item, itemName, $"Value from {Min} to {Max}");

            Grasshopper.GUI.GH_DigitScroller slider = new Grasshopper.GUI.GH_DigitScroller
            {
                MinimumValue = (decimal)Min,
                MaximumValue = (decimal)Max,
                DecimalPlaces = decimalPlace,
                Value = (decimal)originValue,
                Size = new Size(150, 24),
            };
            slider.ValueChanged += Slider_ValueChanged;

            void Slider_ValueChanged(object sender, GH_DigitScrollerEventArgs e)
            {
                double result = (double)e.Value;
                result = result >= Min ? result : Min;
                result = result <= Max ? result : Max;
                slider.Value = (decimal)result;

                valueChange.Invoke(result);

            }

            GH_DocumentObject.Menu_AppendCustomItem(item.DropDown, slider);

            //Add a Reset Item.
            ToolStripMenuItem resetItem = new ToolStripMenuItem("Reset Value", Properties.Resources.ResetIcons_24);
            resetItem.Click += (sender, e) =>
            {
                slider.Value = (decimal)valueDefault;
                valueChange.Invoke(valueDefault);
            };
            item.DropDownItems.Add(resetItem);
        }

        private static void DropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked;
        }
    }
}
