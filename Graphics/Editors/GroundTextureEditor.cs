using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace Graphics.Editors
{
    public enum GroundTexturePencilType
    {
        Add,
        AddSaturate,
        AddReplace,
        Smooth,
        Flatten,
        Set,
    }

    public enum GroundTexturePencilShape
    {
        Circle,
        Square
    }

    public class GroundTexturePencil
    {
        public GroundTexturePencil()
        {
            MousePaintPeriod = 1f/60f;
            Colors = new float[4];
            Shape = GroundTexturePencilShape.Circle;
            Hardness = 0;
        }
        public GroundTexturePencilType Type { get; set; }
        public float Radius { get; set; }
        public GroundTexturePencilShape Shape { get; set; }
        public float Hardness { get; set; }
        public Vector4 Color
        {
            get
            {
                return new Vector4(Colors[0], Colors[1], Colors[2], Colors[3]);
            }
            set
            {
                Colors[0] = value.X;
                Colors[1] = value.Y;
                Colors[2] = value.Z;
                Colors[3] = value.W;
            }
        }
        public float[] Colors { get; set; }
        /// <summary>
        /// Period between to "paints" while pressing the mouse
        /// </summary>
        public float MousePaintPeriod { get; set; }
    }

    public class TextureValuesChangedEvent : EventArgs
    {
        public RectangleF ChangedRegion { get; set; }
    }

    public delegate void TextureValuesChangedEventHandler(object sender, TextureValuesChangedEvent e);
    public class GroundTextureEditor : InputHandler
    {
        public Software.ITexture[] SoftwareTexture { get; set; }
        public SlimDX.Direct3D9.Texture[] Texture9 { get; set; }
        public Vector3 Position { get; set; }
        public System.Drawing.SizeF Size { get; set; }
        public Camera Camera { get; set; }
        public Graphics.GraphicsDevice.Viewport Viewport { get; set; }
        public event TextureValuesChangedEventHandler TextureValuesChanged;
        public Graphics.WorldViewProbe GroundIntersect { get; set; }

        public GroundTexturePencil Pencil { get; set; }

        public GroundTextureEditor()
        {
            Pencil = new GroundTexturePencil
            {
                Type = GroundTexturePencilType.Add,
                Radius = 1,
                Color = new Vector4(1, 1, 1, 0)
            };
        }

        static Vector4[] ConvertToVector4(float[] o)
        {
            Vector4[] tmp = new Vector4[2];

            tmp[0] = new Vector4(tmp[0].X = o[0], tmp[0].Y = o[1], tmp[0].Z = o[2], tmp[0].W = o[3]);
            tmp[1] = new Vector4(tmp[1].X = o[4], tmp[1].Y = o[5], tmp[1].Z = o[6], tmp[1].W = o[7]);

            return tmp;
        }

        static float[] ConvertToArray(Vector4[] o)
        {
            float[] tmp = new float[8];
            tmp[0] = o[0].X;
            tmp[1] = o[0].Y;
            tmp[2] = o[0].Z;
            tmp[3] = o[0].W;
            tmp[4] = o[1].X;
            tmp[5] = o[1].Y;
            tmp[6] = o[1].Z;
            tmp[7] = o[1].W;
            return tmp;
        }

        static float Saturate(float a)
        {
            if (a > 1f)
                return 1;
            if (a < 0f)
            {
                return 0;
            }
            return a;
        }

        static Vector4[] Splat(Vector4[] originalValues, float[] pencil)
        {
            float[] ov = ConvertToArray(originalValues);
            float[] op = pencil;

            float scale = 0;

            int increaseIndex = -1;

            float numberOfPositiveTextures = 0;
            float negSum = 0;
            float posSum = 0;
            float over1Sum = 0;

            for (int i = 0; i < ov.Length; i++)
            {
                if (ov[i] > 0)
                    posSum += ov[i];
                if (op[i] > 0)
                    increaseIndex = i;
            }
            if (increaseIndex != -1)
            {
                for (int i = 0; i < ov.Length; i++)
                    if (ov[i] > 0 && increaseIndex != i)
                        numberOfPositiveTextures += 1f;

                if (posSum < 1.0)
                    scale = 1;
                posSum = 0;
                scale += numberOfPositiveTextures;

                scale = op[increaseIndex] / scale;

                for (int i = 0; i < ov.Length; i++)
                {
                    if (increaseIndex == i)
                    {
                        if (ov[i] + op[i] > 1)
                            over1Sum = (ov[i] + op[i]) - 1f;
                        ov[i] = Saturate(ov[i] + op[i]);
                    }
                    else
                    {
                        if (ov[i] - scale < 0 && ov[i] > 0)
                            negSum += ov[i] - scale;

                        ov[i] = Saturate(ov[i] - scale);
                        posSum += ov[i];
                    }
                }
                negSum += over1Sum;
            }
            else
                negSum = op[0];

            while (negSum < 0 && posSum > 0)
            {
                numberOfPositiveTextures = 0;
                posSum = 0;
                scale = 0;

                for (int i = 0; i < ov.Length; i++)
                {
                    if (ov[i] > 0 && i != increaseIndex)
                        numberOfPositiveTextures += 1f;
                    if (ov[i] > 0)
                        posSum += ov[i];
                }

                if (posSum < 1)
                    scale = 1;

                posSum = 0;

                scale += numberOfPositiveTextures;

                scale = (-negSum) / scale;

                negSum = 0;

                for (int i = 0; i < ov.Length; i++)
                {
                    if (ov[i] - scale < 0 && ov[i] > 0)
                        negSum += ov[i] - scale;
                    if (i != increaseIndex)
                        ov[i] = Saturate(ov[i] - scale);

                    posSum += ov[i];
                }
            }

            return ConvertToVector4(ov);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            painting = true;
            paintingAcc = 0;
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            painting = false;
            DoMousePaint();
        }
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.Q)
                Pencil.Radius *= 0.8f;
            else if (e.KeyCode == System.Windows.Forms.Keys.W)
                Pencil.Radius /= 0.8f;
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if(painting)
            {
                paintingAcc += e.Dtime;
                if (paintingAcc > Pencil.MousePaintPeriod)
                {
                    paintingAcc = 0;
                    DoMousePaint();
                }
            }
        }

        bool painting = false;
        float paintingAcc;

        void DoMousePaint()
        {
#if DEBUG
            RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("StartClick");
#endif
            Vector3 worldPos;
            if (!GroundIntersect.Intersect(out worldPos)) return;
            Draw(Common.Math.ToVector2(worldPos), Pencil);
#if DEBUG
            RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("EndClick");
#endif
        }

        //float Saturate(float a)
        //{
        //    if (a > 1.0f)
        //        return 1.0f;
        //    if (a < 0.0f)
        //        return 0.0f;
        //    return a;
        //}

        public void Draw(Vector2 pos, GroundTexturePencil pencil)
        {
            System.Drawing.Point hmPos = WorldPositionToTexture(pos);
            System.Drawing.Point hmRadius = WorldSizeToTexture(new Vector2(pencil.Radius, pencil.Radius));

            int yStart = Math.Max(0, hmPos.Y - hmRadius.Y);
            int yEnd = Math.Min(SoftwareTexture[0].Size.Height - 1, hmPos.Y + hmRadius.Y);

            if (yStart == yEnd) return;

            int xStart = Math.Max(0, hmPos.X - hmRadius.X);
            int xEnd = Math.Min(SoftwareTexture[0].Size.Width - 1, hmPos.X + hmRadius.X);

            if (xStart == xEnd) return;

            Vector4 sum = Vector4.Zero;
            int count = 0;
            if (pencil.Type == GroundTexturePencilType.Smooth)
            {
                for (int y = yStart; y <= yEnd; y++)
                    for (int x = xStart; x <= xEnd; x++)
                    {
                        var v = SoftwareTexture[0][y, x];
                        sum.X += v.R;
                        sum.Y += v.G;
                        sum.Z += v.B;
                        sum.W += v.A;
                        count++;
                    }
            }
            Vector4 avg = sum / (float)count;

            Vector4 goalColor = Vector4.Zero;
            if (pencil.Type == GroundTexturePencilType.Smooth)
                goalColor = avg;
            else if (pencil.Type == GroundTexturePencilType.Flatten)
            {
                var v = SoftwareTexture[0][(yStart + yEnd) / 2, (xStart + xEnd) / 2];
                goalColor = new Vector4(v.R, v.G, v.B, v.A);
            }
            else if (pencil.Type == GroundTexturePencilType.Set)
                goalColor = pencil.Color;

            for (int y = yStart; y <= yEnd; y++)
                for (int x = xStart; x <= xEnd; x++)
                {
                    var wp = TextureToWorld(new Point(x, y));
                    var v1 = SoftwareTexture[0][y, x];
                    
                    float d = (pos - wp).Length();

                    if (pencil.Shape == GroundTexturePencilShape.Circle &&
                        d > pencil.Radius) continue;

                    float p = d / pencil.Radius; // d : [0, 1]
                    p = (float)System.Math.Pow(p, pencil.Hardness + 1);
                    p *= 3; // d : [0, 3]

                    float red1 = (float)Common.Math.Gaussian(pencil.Colors[0], 0, 1, p);
                    float green1 = (float)Common.Math.Gaussian(pencil.Colors[1], 0, 1, p);
                    float blue1 = (float)Common.Math.Gaussian(pencil.Colors[2], 0, 1, p);
                    float alpha1 = (float)Common.Math.Gaussian(pencil.Colors[3], 0, 1, p);


                    if (pencil.Type == GroundTexturePencilType.Add)
                    {
                        v1.R += red1;
                        v1.G += green1;
                        v1.B += blue1;
                        v1.A += alpha1;
                    }
                    else if (pencil.Type == GroundTexturePencilType.AddSaturate)
                    {
                        v1.R = Saturate(v1.R + red1);
                        v1.G = Saturate(v1.G + green1);
                        v1.B = Saturate(v1.B + blue1);
                        v1.A = Saturate(v1.A + alpha1);
                    }
                    else if (pencil.Type == GroundTexturePencilType.AddReplace)
                    {
                        //var v2 = SoftwareTexture[1][y, x];

                        float red2 = (float)Common.Math.Gaussian(pencil.Colors[4], 0, 1, p);
                        float green2 = (float)Common.Math.Gaussian(pencil.Colors[5], 0, 1, p);
                        float blue2 = (float)Common.Math.Gaussian(pencil.Colors[6], 0, 1, p);
                        float alpha2 = (float)Common.Math.Gaussian(pencil.Colors[7], 0, 1, p);

                        //float scale = 0;

                        //if (v1.R > 0 && pencil.Colors[0] < 0)
                        //    scale++;
                        //if (v1.G > 0 && pencil.Colors[1] < 0)
                        //    scale++;
                        //if (v1.B > 0 && pencil.Colors[2] < 0)
                        //    scale++;
                        //if (v1.A > 0 && pencil.Colors[3] < 0)
                        //    scale++;
                        //if (v2.R > 0 && pencil.Colors[4] < 0)
                        //    scale++;
                        //if (v2.G > 0 && pencil.Colors[5] < 0)
                        //    scale++;
                        //if (v2.B > 0 && pencil.Colors[6] < 0)
                        //    scale++;
                        //if (v2.A > 0 && pencil.Colors[7] < 0)
                        //    scale++;

                        //if (scale != 0)
                        //    scale = 1f / scale;
                        //else
                        //    scale = 1f;

                        //if (scale < 1)
                        //    //Console.WriteLine("hej");
                        //if(pencil.Colors[0] > 0)
                        //    v1.R = Saturate(v1.R + red1);
                        //else
                        //    v1.R = Saturate(v1.R + red1 / scale);

                        //if(pencil.Colors[1] > 0)
                        //    v1.G = Saturate(v1.G + green1);
                        //else
                        //    v1.G = Saturate(v1.G + green1 / scale);

                        //if(pencil.Colors[2] > 0)
                        //    v1.B = Saturate(v1.B + blue1);
                        //else
                        //    v1.B = Saturate(v1.B + blue1 / scale);

                        //if(pencil.Colors[3] > 0)
                        //    v1.A = Saturate(v1.A + alpha1);
                        //else
                        //    v1.A = Saturate(v1.A + alpha1 / scale);



                        //if (pencil.Colors[4] > 0)
                        //    v2.R = Saturate(v2.R + red2);
                        //else
                        //    v2.R = Saturate(v2.R + red2 / scale);

                        //if (pencil.Colors[5] > 0)
                        //    v2.G = Saturate(v2.G + green2);
                        //else
                        //    v2.G = Saturate(v2.G + green2 / scale);

                        //if (pencil.Colors[6] > 0)
                        //    v2.B = Saturate(v2.B + blue2);
                        //else
                        //    v2.B = Saturate(v2.B + blue2 / scale);

                        //if (pencil.Colors[7] > 0)
                        //    v2.A = Saturate(v2.A + alpha2);
                        //else
                        //    v2.A = Saturate(v2.A + alpha2 / scale);

                        var v2 = SoftwareTexture[1][y, x];

                        Vector4[] tmp = Splat(new Vector4[] { new Vector4(v1.R, v1.G, v1.B, v1.A), new Vector4(v2.R, v2.G, v2.B, v2.A) },
                            new float[] { red1, green1, blue1, alpha1, red2, green2, blue2, alpha2 });

                        v1.R = tmp[0].X;
                        v1.G = tmp[0].Y;
                        v1.B = tmp[0].Z;
                        v1.A = tmp[0].W;

                        v2.R = tmp[1].X;
                        v2.G = tmp[1].Y;
                        v2.B = tmp[1].Z;
                        v2.A = tmp[1].W;

                        SoftwareTexture[1][y, x] = v2;

                        //float a = v1.R + v1.G + v1.B + v1.A + v2.R + v2.G + v2.B + v2.A;
                    }
                    else if (pencil.Type == GroundTexturePencilType.Set)
                    {
                        v1.R = goalColor.X;
                        v1.G = goalColor.Y;
                        v1.B = goalColor.Z;
                        v1.A = goalColor.W;
                    }
                    else if (pencil.Type == GroundTexturePencilType.Smooth
                        || pencil.Type == GroundTexturePencilType.Flatten)
                    {
                        v1.R = v1.R * (1 - red1) + goalColor.X * red1;
                        v1.G = v1.G * (1 - green1) + goalColor.Y * green1;
                        v1.B = v1.B * (1 - blue1) + goalColor.Z * blue1;
                        v1.A = v1.A * (1 - alpha1) + goalColor.W * alpha1;
                    }
                    else
                        throw new ArgumentException();

                    SoftwareTexture[0][y, x] = v1;
                }

            if (Texture9 != null && Texture9.Length > 0 && Texture9[0] != null)
            {
                Rectangle r = new Rectangle(xStart, yStart, xEnd - xStart, yEnd - yStart);
                var dr = Texture9[0].LockRectangle(0, r, SlimDX.Direct3D9.LockFlags.None);
                SoftwareTexture[0].WriteRect(dr, r);
                Texture9[0].UnlockRectangle(0);
            }
            if (Texture9 != null && Texture9.Length > 1 && Texture9[1] != null)
            {
                Rectangle r = new Rectangle(xStart, yStart, xEnd - xStart, yEnd - yStart);
                var dr = Texture9[1].LockRectangle(0, r, SlimDX.Direct3D9.LockFlags.None);
                SoftwareTexture[1].WriteRect(dr, r);
                Texture9[1].UnlockRectangle(0);
            }

            if (TextureValuesChanged != null)
                TextureValuesChanged(this, new TextureValuesChangedEvent { 
                    ChangedRegion = new RectangleF(
                        xStart / (float)SoftwareTexture[0].Size.Width,
                        yStart / (float)SoftwareTexture[0].Size.Height,
                        (xEnd - xStart + 1) / (float)SoftwareTexture[0].Size.Width,
                        (yEnd - yStart + 1) / (float)SoftwareTexture[0].Size.Height
                        ) });
        }

        System.Drawing.Point WorldPositionToTexture(Vector2 world)
        {
            return WorldSizeToTexture(world - Common.Math.ToVector2(Position));
        }
        System.Drawing.Point WorldSizeToTexture(Vector2 world)
        {
            return new Point(
                (int)(SoftwareTexture[0].Size.Width * world.X / Size.Width),
                (int)(SoftwareTexture[0].Size.Height * world.Y / Size.Height));
        }
        Vector2 TextureToWorld(System.Drawing.Point point)
        {
            return new Vector2(
                point.X * Size.Width / SoftwareTexture[0].Size.Width + Position.X,
                point.Y * Size.Height / SoftwareTexture[0].Size.Height + Position.Y);
        }
    }
    public class GroundTextureEditorRenderer
    {
        public GroundTextureEditorRenderer(GroundTextureEditor editor)
        {
            this.editor = editor;
        }
        public void Render(View view, Camera camera)
        {
            Vector3 worldPos;
            if (!editor.GroundIntersect.Intersect(out worldPos)) return;
            view.DrawCircle(camera, Matrix.Identity, worldPos, editor.Pencil.Radius, 12, Color.Yellow);
        }
        GroundTextureEditor editor;
    }
}
