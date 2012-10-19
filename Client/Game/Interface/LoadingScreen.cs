using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Interface;
using Graphics.Content;

namespace Client.Game.Interface
{
    public class LoadingScreen : Graphics.Interface.Control
    {
        public LoadingScreen() : this(null, Vector2.Zero)
        {
        }
        public LoadingScreen(string picturePath, Vector2 pictureSize)
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/LoadingScreen/background.png"),
                TextureAdressMode = SlimDX.Direct3D9.TextureAddress.Wrap,       
            };

            Common.Tuple<string, Vector2> picture;
            if (picturePath != null)
                picture = new Common.Tuple<string, Vector2>(picturePath, pictureSize);
            else
                picture = defaultPictures[new Random().Next(defaultPictures.Length)];

            var picMetaTexture = new TextureFromFile(picture.First) { DontScale = true };
            var pic = new Control
            {
                Background = new ImageGraphic
                {
                    Texture = picMetaTexture,
                    SizeMode = SizeMode.AutoAdjust
                },
                Anchor = Orientation.Center,
                Size = picture.Second
            };

            Dock = System.Windows.Forms.DockStyle.Fill;

            pic.AddChild(progress);
            AddChild(pic);
        }

        Common.Tuple<string, Vector2>[] defaultPictures = 
        {
            new Common.Tuple<string, Vector2>("Interface/LoadingScreen/Tier1Loading1.png", new Vector2(1772, 892))
        };

        public void UpdateProgress(float inc)
        {
            progress.Value += inc;
            Invalidate();
        }

        public float Percentage { get { return progress.Value / progress.MaxValue; } }

        //protected override void OnUpdate(UpdateEventArgs e)
        //{
        //    base.OnUpdate(e);
        //    progress.Value += e.Dtime * 100;
        //    if (progress.Value > 100) progress.Value = 0;
        //}

        ProgressBar progress = new ProgressBar
        {
            Size = new Vector2(600, 30),
            Anchor = Orientation.Bottom,
            Value = 0f,
            MaxValue = 1f,
            TextAnchor = Orientation.Center,
            Position = new Vector2(0, 100),
            Background = (BorderGraphic)InterfaceScene.DefaultSlimBorder.Clone(),
            //ProgressGraphic = InterfaceScene.DefaultFormBorder,
            ProgressGraphic = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer { TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Gold) },
                BorderSize = new Vector2(3, 3)
            }
        };

        Game game;
    }
}
