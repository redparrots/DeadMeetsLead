using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using SlimDX;
using System.IO;

namespace Graphics.Content
{
    public class Font : MetaResource<FontImplementation>
    {
        public class Mapper : MetaMapper<Font, FontImplementation>
        {
            public override FontImplementation Construct(Font metaResource, ContentPool content)
            {
                return new FontImplementation(metaResource.SystemFont, metaResource.Color, metaResource.Backdrop, metaResource.Encoding);
            }
            public override void Release(Font metaResource, ContentPool content, FontImplementation resource)
            {
            }
        }

        public Font() 
        {
            Backdrop = Color.Transparent;
            Encoding = DefaultEncoding;
        }
        public Font(Font cpy)
        {
            SystemFont = (System.Drawing.Font)cpy.SystemFont.Clone();
            Color = cpy.Color;
            Backdrop = cpy.Backdrop;
            Encoding = cpy.Encoding;
        }
        public Font(System.Drawing.Font font, Color color, Color backdrop)
            : this()
        {
            SystemFont = (System.Drawing.Font)font.Clone();
            Color = color;
            Backdrop = backdrop;
        }
        public Font(System.Drawing.Font font, Color color)
            : this(font, color, Color.Transparent)
        {
        }
        public Font(System.Drawing.Font font)
            : this(font, Color.White)
        {
        }

        public System.Drawing.Font SystemFont { get; set; }
        public Color Color { get; set; }
        public Color Backdrop { get; set; }
        public Encoding Encoding { get; set; }

        public static Encoding DefaultEncoding = Encoding.ASCII;

        public override bool Equals(object obj)
        {
            var o = obj as Font;
            if (o == null) return false;
            return
                Object.Equals(SystemFont, o.SystemFont) &&
                Object.Equals(Color, o.Color) &&
                Object.Equals(Backdrop, o.Backdrop) &&
                Object.Equals(Encoding, o.Encoding);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^
                SystemFont.GetHashCode() ^
                Color.GetHashCode() ^
                Backdrop.GetHashCode() ^
                Encoding.GetHashCode();
        }

        public override object Clone()
        {
            return new Font(this);
        }

    }


    public class FontImplementation : Software.ByteBufferTexture
    {
        public FontImplementation(System.Drawing.Font font, Color color, Color backDrop, Encoding encoding)
        {
            this.encoding = encoding;
            int maxCharacterWidth = (int)font.Size + 1;
            Bitmap b = new Bitmap(100, 100);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b);
            CharacterHeight = (int)Math.Ceiling(g.MeasureString(".", font).Height);
            TextureSize = (int)Math.Ceiling(Math.Sqrt(
                (maxCharacterWidth + characterMargin) * (int)(CharacterHeight + characterMargin) * 256));
            TextureSize = (int)Math.Pow(2, Math.Ceiling(Math.Log(TextureSize, 2)));
            b = new Bitmap(TextureSize, TextureSize);
            g = System.Drawing.Graphics.FromImage(b);

            Draw(g, font);
            var bmpData = b.LockBits(new Rectangle(0, 0, TextureSize, TextureSize), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes  = Math.Abs(bmpData.Stride) * TextureSize;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);



            for(int i=0; i < rgbValues.Length; i+=4)
            {
                Color c = Color.FromArgb(
                    rgbValues[i + 3],
                    rgbValues[i + 2],
                    rgbValues[i + 1],
                    rgbValues[i + 0]);
                float col = c.R * color.A / 255f / 255f;
                float back = c.G * backDrop.A / 255f / 255f;
                float v = col + back;
                if (v == 0)
                {
                    col = 1;
                    back = 0;
                }
                else
                {
                    col /= v;
                    back /= v;
                }
                float x1 = col + back;

                rgbValues[i + 3] = (byte)(Math.Min(255, c.R * color.A / 255f + c.G * backDrop.A / 255f));
                rgbValues[i + 2] = (byte)(color.R * col + backDrop.R * back);
                rgbValues[i + 1] = (byte)(color.G * col + backDrop.G * back);
                rgbValues[i + 0] = (byte)(color.B * col + backDrop.B * back);
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            b.UnlockBits(bmpData);


            MemoryStream m = new MemoryStream();
            b.Save(m, System.Drawing.Imaging.ImageFormat.Png);

            //b.Save(Common.Utils.UniqueFilename("testfont", ".png"), System.Drawing.Imaging.ImageFormat.Png);
            //var f = font.Name + font.Size + font.Unit + color + backDrop + ".png";
            //b.Save(f, System.Drawing.Imaging.ImageFormat.Png);

            m.Position = 0;
            Data = m.ToArray();
        }
        void Draw(System.Drawing.Graphics g, System.Drawing.Font font)
        {
            g.Clear(Color.FromArgb(255, 0, 0, 0));
            /*LinearGradientBrush lgb = new LinearGradientBrush(new Rectangle(0, 0, 256, 256), Color.White, Color.DarkRed, -45);
            g.FillRectangle(lgb, 0, 0, 256, 256);*/
            g.PageUnit = GraphicsUnit.Pixel;
            if (font.Size < 70)
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            else
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            PointF p = new PointF(characterMargin, characterMargin);
            characterUV = new PointF[256];
            characterWidth = new float[256];
            //StringFormat stringFormat = new StringFormat { Alignment = StringAlignment.Near, Trimming = StringTrimming.None, LineAlignment = StringAlignment.Near, FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip };
            Brush brush = new SolidBrush(Color.Red);

            int pad = (int)(font.Size * 0.1f);
            Random r = new Random();
            Brush brushBackDrop = new SolidBrush(Color.Green);
            for (int i = 0; i < 256; i++)
            {
                if (i == CaretChar)
                {
                    g.DrawLine(new Pen(brushBackDrop), p, new PointF(p.X + 1, p.Y + CharacterHeight + 1));
                    g.DrawLine(new Pen(brush), p, new PointF(p.X, p.Y + CharacterHeight));
                    characterWidth[i] = 2;
                }
                else
                {
                    String s = encoding.GetString(new byte[] { (byte)i }); ;

                    g.DrawString(s, font, brushBackDrop, new PointF(p.X + pad + 1, p.Y + 1), StringFormat.GenericTypographic);
                    g.DrawString(s, font, brush, new PointF(p.X + pad, p.Y), StringFormat.GenericTypographic);
                    SizeF si = g.MeasureString(s, font, 1000, StringFormat.GenericTypographic);
                    si.Width += pad;
                    characterWidth[i] = (float)System.Math.Ceiling(si.Width) + 1;
                }
                //g.FillRectangle(new SolidBrush(Color.FromArgb(r.Next(256), r.Next(256), r.Next(256))),
                //    p.X, p.Y, CharacterWidth[i], CharacterHeight);
                characterUV[i] = p;
                characterUV[i].X /= ((float)TextureSize);
                characterUV[i].Y /= ((float)TextureSize);
                p.X += characterWidth[i] + characterMargin;
                if (p.X >= TextureSize - (characterWidth[i] + characterMargin)*2)
                {
                    p.X = 0;
                    p.Y += CharacterHeight + characterMargin;
                }
            }
            byte space = encoding.GetBytes(" ")[0];
            // MeasureString has some problems with space so we use the width of the font's underscore instead
            characterWidth[space] = g.MeasureString("_", font, 1000, StringFormat.GenericTypographic).Width;
            g.Flush();
        }

        public float TextWidth(String text)
        {
            float w = 0;
            foreach (byte c in GetString(text))
                w += (float)Math.Ceiling(characterWidth[c]);
            return w;
        }
        public float TextHeight(String text)
        {
            float h = CharacterHeight;
            foreach (byte v in GetString(text))
                if (v == '\n') h += CharacterHeight;
            return CharacterHeight;
        }
        public Vector2 TextSize(String text)
        {
            var ss = text.Split('\n');
            float h = 0;
            float w = 0;
            foreach (var s in ss)
            {
                w = Math.Max(w, TextWidth(s));
                h += CharacterHeight;
            }
            return new Vector2(w, h);
        }
        byte[] GetString(string input)
        {
            return encoding.GetBytes(input);
        }
        public float CharacterWidth(char input)
        {
            return characterWidth[GetString("" + input)[0]];
        }
        public PointF CharacterUV(char input)
        {
            return characterUV[GetString("" + input)[0]];
        }
        float[] characterWidth;
        public float CharacterHeight { get; private set; }
        PointF[] characterUV;
        public int TextureSize;
        public static readonly char CaretChar = (char)11;
        Encoding encoding;
        float characterMargin = 4;
    }
}
