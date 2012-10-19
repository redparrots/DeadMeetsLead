using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using Graphics.Content;
using SlimDX;

namespace Client.Game
{
    public class Water : Entity
    {
        public Water(Map.Map map)
        {
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = true,
                IsWater = true,
                Texture = new TextureFromFile("Models/Props/Sky1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/WaterSpecular1.png"),
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesSpecular = Priority.Never,
                SpecularExponent = 3,
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Wrap,
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Position = Common.Math.ToVector3(map.Settings.Position) + 
                            Vector3.UnitZ * map.Settings.WaterHeight,
                        Size = new Vector2(map.Ground.Size.Width, map.Ground.Size.Height),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1),
                        Facings = Facings.Frontside
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
            };
            //VisibilityLocalBounding = new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000))
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000)), false, true);
        }
    }
}
