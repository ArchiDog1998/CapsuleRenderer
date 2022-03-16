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

            ExchangeMethod(typeof(GH_CapsuleRenderEngine).GetRuntimeMethods().Where(m => m.Name.Contains("RenderOutlines")).First(),
                typeof(CapsuleReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(RenderOutlines))).First());
        }

        public static void Render(this GH_Capsule cap, Graphics G, Image icon, GH_PaletteStyle style)
        {
            cap.MyRender(G, icon, style);
        }

        public static void MyRender(this GH_Capsule cap, Graphics G, Image icon, GH_PaletteStyle style)
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
            //cap.RenderEngine.RenderHighlight(G);
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
            engine.MyRenderOutlines(G, zoom, style);
        }

        public static void MyRenderOutlines(this GH_CapsuleRenderEngine engine, Graphics G, float zoom, GH_PaletteStyle style)
        {
            GH_Capsule capsule = (GH_Capsule)_capsuleInfo.GetValue(engine);

            if (zoom * (float)capsule.MaxRadius < 1f || capsule.OutlineShape == null)
            {
                //if (zoom >= 0.5f)
                //{
                //    Pen pen = (Pen)_innerContourPenInfo.Invoke(engine, new object[] { zoom });
                //    Rectangle rect = GH_Convert.ToRectangle(capsule.Box);
                //    rect.Inflate(-1, -1);
                //    G.DrawRectangle(pen, rect);
                //    pen.Dispose();
                //}
                Pen pen2 = new Pen(style.Edge);
                G.DrawRectangle(pen2, GH_Convert.ToRectangle(capsule.Box));
                pen2.Dispose();
            }
            else
            {
                //if (zoom >= 0.5f)
                //{
                //    Pen pen3 = (Pen)_innerContourPenInfo.Invoke(engine, new object[] {zoom});
                //    G.DrawPath(pen3, capsule.OutlineShape);
                //    pen3.Dispose();
                //}
                Pen pen4 = new Pen(style.Edge);
                pen4.LineJoin = LineJoin.Round;
                G.DrawPath(pen4, capsule.OutlineShape);
                pen4.Dispose();
            }
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
