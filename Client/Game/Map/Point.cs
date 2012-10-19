using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;
using Graphics;
using SlimDX;

namespace Client.Game.Map
{
    /// <summary>
    /// A virtual point on the map, used for scripting
    /// </summary>
    [EditorDeployable(Group="Points"), Serializable]
    public class Point : GameEntity
    {
        public Point()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription =
                        new global::Graphics.Software.Meshes.BoxMesh(new Vector3(-0.1f, -0.1f, 0), new Vector3(0.1f, 0.1f, 0.2f), Facings.Frontside, false),
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureConcretizer
                {
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Blue)
                },
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };
            PickingLocalBounding = VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            VisibleInGame = false;
            Dynamic = false;
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if (Game.Instance != null && !VisibleInGame)
                MainGraphic = null;
        }
        public bool VisibleInGame { get; set; }
    }

    [EditorDeployable(Group="Points"), Serializable]
    public class Checkpoint : Point
    {
        public Checkpoint()
        {
            ((MetaModel)MainGraphic).Texture = new TextureConcretizer
            {
                TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(
                    System.Drawing.Color.Orange)
            };
            HitPointsPerc = 1;
            RagePerc = 1;
            Ammo = 0;
        }

        public float HitPointsPerc { get; set; }
        public float RagePerc { get; set; }
        public int Ammo { get; set; }
    }


    [EditorDeployable(Group = "Points"), Serializable]
    class Ruler : GameEntity
    {
        public Ruler()
        {
            for (int y = -5; y <= 5; y++)
                for (int x = -5; x <= 5; x++)
                    AddChild(new Point
                    {
                        Translation = new Vector3(x, y, 0)
                    });
        }
    }

}
