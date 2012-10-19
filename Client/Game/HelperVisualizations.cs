using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using SlimDX;
using System.Drawing;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Client.Game
{
    /// <summary>
    /// Visualizes things such as physics boundings or aggro range
    /// </summary>
    public class HelperVisualizations
    {
        public HelperVisualizations()
        {
            Settings = new HelperVisualizationsSettings();
        }
        public HelperVisualizationsSettings Settings { get; set; }
        public void Render(Graphics.BoundingVolumesRenderer bvRenderer, 
            View view,
            Scene scene,
            Common.Pathing.NavMesh navMesh,
            int activeFrame)
        {
            if (Settings.VisualBoundings)
            {
                foreach (var v in scene.AllEntities)
                    if(v.ActiveInMain == activeFrame)
                        bvRenderer.Draw(Matrix.Identity,
                            v.VisibilityWorldBounding, 
                            Color.Orange);
            }

            if (Settings.PhysicsBoundings)
            {
                bvRenderer.DrawFullChains = Settings.PhysicsBoundingsFullChains;
                foreach (var v in scene.AllEntities)
                    if (v is Client.Game.Map.GameEntity &&
                        v.ActiveInMain == activeFrame &&
                        (!Settings.PhysicsBoundingsHideGround || !(v is Client.Game.Map.GroundPiece)))
                        bvRenderer.Draw(Matrix.Identity,
                            ((Client.Game.Map.GameEntity)v).PhysicsLocalBounding != null ?
                            Common.Boundings.Transform(((Client.Game.Map.GameEntity)v).PhysicsLocalBounding, v.WorldMatrix)
                            : null,
                            Color.Blue);
                bvRenderer.DrawFullChains = true;
            }

            if (Settings.PathMesh)
                bvRenderer.Draw(Matrix.Identity, navMesh.BoundingRegion, Color.White);

            if (Settings.AggroRange)
            {
                foreach (var v in scene.AllEntities)
                    if (v.ActiveInMain == activeFrame && v is Map.NPC)
                        view.DrawCircle(scene.Camera, Matrix.Identity,
                            ((Map.NPC)v).Position, ((Map.NPC)v).InRangeRadius, 12, Color.Red);
            }
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter)), JsonObject, Serializable]
    public class HelperVisualizationsSettings
    {
        public bool VisualBoundings { get; set; }
        public bool PhysicsBoundings { get; set; }
        public bool PhysicsBoundingsHideGround { get; set; }
        public bool PhysicsBoundingsFullChains { get; set; }
        public bool AggroRange { get; set; }
        public bool PathMesh { get; set; }

        public HelperVisualizationsSettings()
        {
            VisualBoundings = false;
            PhysicsBoundings = false;
            PhysicsBoundingsHideGround = true;
            PhysicsBoundingsFullChains = false;
            AggroRange = false;
            PathMesh = false;
        }
    }
}
