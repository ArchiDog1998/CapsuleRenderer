using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CapsuleRenderer
{
    public static class CapsuleReplacer
    {
        private static readonly FieldInfo _capsuleInfo = typeof(GH_CapsuleRenderEngine).GetRuntimeFields().Where(f => f.Name.Contains("m_capsule")).First();
        private static readonly MethodInfo _innerContourPenInfo = typeof(GH_CapsuleRenderEngine).GetRuntimeMethods().Where(m => m.Name.Contains("InnerContourPen")).First();
        private static readonly FieldInfo _highLightCacheInfo = typeof(GH_Capsule).GetRuntimeFields().Where(f => f.Name.Contains("cache_highlight")).First();

        private static readonly MethodInfo _createJaggedEdgeLeftInfo = typeof(GH_CapsuleRenderEngine).GetRuntimeMethods().Where(m => m.Name.Contains("CreateJaggedEdgeLeft")).First();
        private static readonly MethodInfo _createJaggedEdgeRightInfo = typeof(GH_CapsuleRenderEngine).GetRuntimeMethods().Where(m => m.Name.Contains("CreateJaggedEdgeRight")).First();


        public static void Init()
        {
            ExchangeMethod(
                typeof(GH_Capsule).GetRuntimeMethods().Where(m =>
                {
                    if (!m.Name.Contains("Render")) return false;

                    var pars = m.GetParameters();
                    if(pars.Length != 3) return false;

                    if(pars[2].ParameterType == typeof(GH_PaletteStyle))
                    {
                        return true;
                    }
                    return false;
                }).First(),
                typeof(CapsuleReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(Render))).First()
            );

            ExchangeMethod(
                typeof(GH_Capsule).GetRuntimeMethods().Where(m =>
                {
                    if (!m.Name.Contains("CreateCapsule")) return false;

                    var pars = m.GetParameters();
                    if(pars.Length != 2) return false;

                    if(pars[0].ParameterType == typeof(Rectangle))
                    {
                        return true;
                    }
                    return false;
                }).First(),
                typeof(CapsuleReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(CreateCapsule))).First()
            );

            ExchangeMethod(typeof(GH_CapsuleRenderEngine).GetRuntimeMethods().Where(m => m.Name.Contains("RenderOutlines")).First(),
                typeof(CapsuleReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(RenderOutlines))).First());

            ExchangeMethod(typeof(GH_CapsuleRenderEngine).GetRuntimeMethods().Where(m => m.Name.Contains("RenderBackground")).First(),
                typeof(CapsuleReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(RenderBackground))).First()); 
            
            ExchangeMethod(typeof(GH_CapsuleRenderEngine).GetRuntimeMethods().Where(m => 
            {
                if (!m.Name.Contains("CreateRoundedRectangle")) return false;

                var pars = m.GetParameters();
                if (pars.Length != 5) return false;

                if (pars[1].ParameterType == typeof(float))
                {
                    return true;
                }
                return false;
            }).First(),
                typeof(CapsuleReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(CreateRoundedRectangle))).First());

            ExchangeMethod(typeof(GH_CapsuleRenderEngine).GetRuntimeMethods().Where(m => m.Name.Contains("CreateJaggedRectangle")).First(),
                typeof(CapsuleReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(CreateJaggedRectangle))).First()); 
        }

        public static void Render(this GH_Capsule cap, Graphics G, Image icon, GH_PaletteStyle style)
        {
            float zoom = G.Transform.Elements[0];
            if (style.Fill.A < byte.MaxValue)
            {
                cap.RenderEngine.RenderGrips_Alternative(G);
            }
            else
            {
                cap.RenderEngine.RenderGrips(G);
            }
            cap.RenderEngine.RenderBackground(G, zoom, style);

            if (Datas.IsRenderHighLight)
                cap.RenderEngine.RenderHighlight(G);

            cap.RenderEngine.RenderOutlines(G, zoom, style);
            if (icon != null)
            {
                cap.RenderEngine.RenderIcon(G, icon);
            }
            else if (cap.Text != null && cap.Font != null)
            {
                cap.RenderEngine.RenderText(G, style.Text);
            }
        }

        public static void RenderOutlines(this GH_CapsuleRenderEngine engine, Graphics G, float zoom, GH_PaletteStyle style)
        {
            GH_Capsule capsule = (GH_Capsule)_capsuleInfo.GetValue(engine);
            bool isShowInnerOutLine = Datas.IsRenderInnerOutLine && zoom >= 0.5f;

            if (zoom * (float)capsule.MaxRadius < 1f || capsule.OutlineShape == null)
            {
                if (isShowInnerOutLine)
                {
                    Pen pen = (Pen)_innerContourPenInfo.Invoke(engine, new object[] { zoom });
                    Rectangle rect = GH_Convert.ToRectangle(capsule.Box);
                    rect.Inflate(-1, -1);
                    G.DrawRectangle(pen, rect);
                    pen.Dispose();
                }
                Pen pen2 = new Pen(style.Edge, (float)Datas.OutLineWidth);
                G.DrawRectangle(pen2, GH_Convert.ToRectangle(capsule.Box));
                pen2.Dispose();
            }
            else
            {
                if (isShowInnerOutLine)
                {
                    Pen pen3 = (Pen)_innerContourPenInfo.Invoke(engine, new object[] { zoom });
                    G.DrawPath(pen3, capsule.OutlineShape);
                    pen3.Dispose();
                }
                Pen pen4 = new Pen(style.Edge, (float)Datas.OutLineWidth);
                pen4.LineJoin = LineJoin.Round;
                G.DrawPath(pen4, capsule.OutlineShape);
                pen4.Dispose();
            }
        }


        public static  void RenderBackground(this GH_CapsuleRenderEngine engine, Graphics G, float zoom, GH_PaletteStyle style)
        {
            GH_Capsule capsule = (GH_Capsule)_capsuleInfo.GetValue(engine);

            if (capsule.OutlineShape != null)
            {
                Brush brush = Datas.IsCapsuleFlat ? new SolidBrush(GH_GraphicsUtil.OffsetColour(style.Fill, 20)) :
                    style.CreateBrush(capsule.Box, zoom);

                if ((double)zoom < 0.4)
                {
                    G.FillRectangle(brush, capsule.Box);
                }
                else
                {
                    G.FillPath(brush, capsule.OutlineShape);
                }
                brush.Dispose();
            }
        }

        public static GH_Capsule CreateCapsule(Rectangle box, GH_Palette palette)
        {
            return GH_Capsule.CreateCapsule(box, palette, Datas.CapsuleRadius, Datas.CapsuleHighLight);
        }

        public static GraphicsPath CreateRoundedRectangle(RectangleF rec, float R0, float R1, float R2, float R3)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            if (R0 <= 0f && R1 <= 0f && R2 <= 0f && R3 <= 0f)
            {
                graphicsPath.AddRectangle(rec);
                return graphicsPath;
            }
            if (rec.Width < 0.1f)
            {
                rec.Width = 0.1f;
            }
            if (rec.Height < 0.1f)
            {
                rec.Height = 0.1f;
            }

            R0 = Math.Min(Math.Min(R0, rec.Width), rec.Height);
            R1 = Math.Min(Math.Min(R1, rec.Width), rec.Height);
            R2 = Math.Min(Math.Min(R2, rec.Width), rec.Height);
            R3 = Math.Min(Math.Min(R3, rec.Width), rec.Height);

            float D0 = 2f * R0;
            float D1 = 2f * R1;
            float D2 = 2f * R2;
            float D3 = 2f * R3;

            if ((double)D0 > 0.0)
            {
                graphicsPath.AddArc(rec.Left, rec.Top, D0, D0, 180f, 90f);
            }
            else
            {
                graphicsPath.AddLine(rec.Left, rec.Top, rec.Right - R1, rec.Top);
            }
            if ((double)D1 > 0.0)
            {
                graphicsPath.AddArc(rec.Right - D1, rec.Top, D1, D1, 270f, 90f);
            }
            else
            {
                graphicsPath.AddLine(rec.Right, rec.Top, rec.Right, rec.Bottom - R2);
            }
            if ((double)D2 > 0.0)
            {
                graphicsPath.AddArc(rec.Right - D2, rec.Bottom - D2, D2, D2, 0f, 90f);
            }
            else
            {
                graphicsPath.AddLine(rec.Right, rec.Bottom, rec.Left + R3, rec.Bottom);
            }
            if ((double)D3 > 0.0)
            {
                graphicsPath.AddArc(rec.Left, rec.Bottom - D3, D3, D3, 90f, 90f);
            }
            else
            {
                graphicsPath.AddLine(rec.Left, rec.Bottom, rec.Left, rec.Top + R0);
            }
            graphicsPath.CloseAllFigures();
            return graphicsPath;
        }

        public static GraphicsPath CreateJaggedRectangle(RectangleF rec, float R0, float R1, float R2, float R3, bool jaggedLeft, bool jaggedRight)
        {
            if (!jaggedLeft && !jaggedRight)
            {
                return GH_CapsuleRenderEngine.CreateRoundedRectangle(rec, R0, R1, R2, R3);
            }
            GraphicsPath graphicsPath = new GraphicsPath();
            if (rec.Width < 0.1f)
            {
                rec.Width = 0.1f;
            }
            if (rec.Height < 0.1f)
            {
                rec.Height = 0.1f;
            }

            R0 = Math.Min(Math.Min(R0, rec.Width), rec.Height);
            R1 = Math.Min(Math.Min(R1, rec.Width), rec.Height);
            R2 = Math.Min(Math.Min(R2, rec.Width), rec.Height);
            R3 = Math.Min(Math.Min(R3, rec.Width), rec.Height);

            float D0 = 2f * R0;
            float D1 = 2f * R1;
            float D2 = 2f * R2;
            float D3 = 2f * R3;

            if (jaggedLeft)
            {
                graphicsPath.AddLines((PointF[])_createJaggedEdgeLeftInfo.Invoke(null, new object[] {rec}));
            }
            else
            {
                if (D3 > 0f)
                {
                    graphicsPath.AddArc(rec.Left, rec.Bottom - D3, D3, D3, 90f, 90f);
                }
                else
                {
                    graphicsPath.AddLine(rec.Left, rec.Bottom, rec.Left, rec.Top + R0);
                }
                if (D0 > 0f)
                {
                    graphicsPath.AddArc(rec.Left, rec.Top, D0, D0, 180f, 90f);
                }
                else
                {
                    graphicsPath.AddLine(rec.Left, rec.Top, rec.Right - R1, rec.Top);
                }
            }
            if (jaggedRight)
            {
                graphicsPath.AddLines((PointF[])_createJaggedEdgeRightInfo.Invoke(null, new object[] {rec}));
            }
            else
            {
                if (D1 > 0f)
                {
                    graphicsPath.AddArc(rec.Right - D1, rec.Top, D1, D1, 270f, 90f);
                }
                else
                {
                    graphicsPath.AddLine(rec.Right, rec.Top, rec.Right, rec.Bottom - R2);
                }
                if (D2 > 0f)
                {
                    graphicsPath.AddArc(rec.Right - D2, rec.Bottom - D2, D2, D2, 0f, 90f);
                }
                else
                {
                    graphicsPath.AddLine(rec.Right, rec.Bottom, rec.Left + R3, rec.Bottom);
                }
            }
            graphicsPath.CloseAllFigures();
            return graphicsPath;
        }
        internal static bool ExchangeMethod(MethodInfo targetMethod, MethodInfo injectMethod)
        {
            if (targetMethod == null || injectMethod == null)
            {
                return false;
            }
            RuntimeHelpers.PrepareMethod(targetMethod.MethodHandle);
            RuntimeHelpers.PrepareMethod(injectMethod.MethodHandle);
            unsafe
            {
                if (IntPtr.Size == 4)
                {
                    int* tar = (int*)targetMethod.MethodHandle.Value.ToPointer() + 2;
                    int* inj = (int*)injectMethod.MethodHandle.Value.ToPointer() + 2;
                    var relay = *tar;
                    *tar = *inj;
                    *inj = relay;
                }
                else
                {
                    long* tar = (long*)targetMethod.MethodHandle.Value.ToPointer() + 1;
                    long* inj = (long*)injectMethod.MethodHandle.Value.ToPointer() + 1;
                    var relay = *tar;
                    *tar = *inj;
                    *inj = relay;
                }
            }
            return true;
        }
    }
}
