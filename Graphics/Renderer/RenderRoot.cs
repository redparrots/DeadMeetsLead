using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;

namespace Graphics.Renderer
{
    public interface ITreeNode
    {
        void Insert(Model9 model, Entity entity, MetaModel metaModel, string metaName, Settings settings);
    }

    public class RenderRoot
    {
        public RenderRoot(string[, , , , ,] techniqueNames)
        {
            Techniques = new Dictionary<string, RenderTechnique>();
            AlphaObjects = new List<Common.Tuple<Model9, Entity, string, string>>();
            SplatObjects = new List<Common.Tuple<Model9, Entity, string>>();
            SplatTechniques = new Dictionary<string, RenderSplatMesh>();

            this.techniqueNames = techniqueNames;
        }

        public string GetSplatTechniqueExtention(Model9 model, Settings settings)
        {
            bool splat1 = false;
            bool splat2 = false;

            if (model.MaterialTexture[0] != null || model.MaterialTexture[1] != null || model.MaterialTexture[2] != null || model.MaterialTexture[3] != null)
                splat1 = true;

            if (model.MaterialTexture[4] != null || model.MaterialTexture[5] != null || model.MaterialTexture[6] != null || model.MaterialTexture[7] != null)
                splat2 = true;

            string techniqueExtention = "";

            if (splat1)
                techniqueExtention += "Splat1";
            else
                techniqueExtention += "NoSplat1";

            if (splat2)
                techniqueExtention += "Splat2";
            else
                techniqueExtention += "NoSplat2";

            if (settings.TerrainQuality == Settings.TerrainQualities.Low)
                return techniqueExtention + "Lowest";
            else
                return techniqueExtention + "NoLowest";
        }

        private bool IsVisible(MetaModel metaModel, Settings settings)
        {
            if (settings.PriorityRelation[metaModel.Visible] + settings.TerrainQualityPriorityRelation[settings.TerrainQuality] < 3)
                return false;

            return true;
        }

        public void Insert(Model9 model, Entity entity, MetaModel metaModel, string metaName, Settings settings)
        {
            //if (!IsVisible(metaModel, settings)) return;

            string techniqueNameEnding = Renderer.GetTechniqueNameExtension(metaModel, settings, techniqueNames);

            if (metaModel.HasAlpha)
            {
                //Renderer.totalAddedItems++;
                AlphaObjects.Add(new Common.Tuple<Model9, Entity, string, string>(model, entity, metaName, techniqueNameEnding));
            }
            else if (metaModel.SplatMapped)
            {
                //Renderer.totalAddedItems++;

                string techniqueName = "";
                techniqueName = "Standard" + Renderer.GetTechniqueNameExtension(metaModel, settings, techniqueNames);
                techniqueName += GetSplatTechniqueExtention(model, settings);

                RenderSplatMesh r;
                if (!SplatTechniques.TryGetValue(techniqueName, out r))
                    SplatTechniques[techniqueName] = r = new RenderSplatMesh();
                r.Insert(model, entity, metaModel, metaName);
                if(entity.Scene.DesignMode)
                    SplatObjects.Add(new Common.Tuple<Model9, Entity, string>(model, entity, metaName));
            }
            else if (metaModel.SkinnedMesh != null)
            {
                foreach (var SM in model.SkinnedMesh.MeshContainers)
                {
                    if (SM.Second.SkinInfo != null)
                    {
                        //Renderer.totalAddedItems++;
                        string techniqueName = "SkinnedMesh" + techniqueNameEnding;

                        RenderTechnique r;
                        if (!Techniques.TryGetValue(techniqueName, out r))
                            Techniques[techniqueName] = r = new RenderTechnique();
                        r.Insert(model, entity, metaModel, metaName, model.SkinnedMesh, null, false);
                    }
                    else
                    {
                        //Renderer.totalAddedItems++;

                        RenderTechnique r;
                        if (!Techniques.TryGetValue("ShadowedSceneInstanced" + techniqueNameEnding, out r))
                            Techniques["ShadowedSceneInstanced" + techniqueNameEnding] = r = new RenderTechnique();
                        r.Insert(model, entity, metaModel, metaName, null, SM.Second.MeshData.Mesh, true);
                    }
                }
            }
            else if (metaModel.XMesh != null)
            {
                //Renderer.totalAddedItems++;

                RenderTechnique r;
                if (!Techniques.TryGetValue("ShadowedSceneInstanced" + techniqueNameEnding, out r))
                    Techniques["ShadowedSceneInstanced" + techniqueNameEnding] = r = new RenderTechnique();
                r.Insert(model, entity, metaModel, metaName, null, model.XMesh, false);
            }
        }

        public List<Common.Tuple<Model9, Entity, string, string>> AlphaObjects;
        public List<Common.Tuple<Model9, Entity, string>> SplatObjects;
        public Dictionary<string, RenderSplatMesh> SplatTechniques;
        public Dictionary<string, RenderTechnique> Techniques;

        private string[, , , , ,] techniqueNames;
    }
}