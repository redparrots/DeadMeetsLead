
//#define DEBUG_RENDERER
//#define PROFILE_RENDERER
//#define LOG_RENDERTREE
//#define RENDERER_STATISTICS
#if BETA_RELEASE
//#define PROFILES_RENDERER
#endif

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Graphics.Content;
using Graphics.GraphicsDevice;
using SlimDX;
using SlimDX.Direct3D9;

#if PROFILE_RENDERER || LOG_RENDERTREE || RENDERER_STATISTICS
using System.IO;
#endif

namespace Graphics.Renderer
{
    public enum Results
    {
        OK,
        OutOfVideoMemory,
        VideoCardNotSupported,
        VideoCardNotRecommended
    }

    public partial class Renderer : IRenderer
    {
        #region Public Variables

        private Scene scene;
        public Scene Scene
        {
            get { return scene; }
            set { scene = value; }
        }

        private IDevice9StateManager stateManager;
        public IDevice9StateManager StateManager
        {
            get { return stateManager; }
            set { stateManager = value; }
        }

        private Graphics.Renderer.Settings settings;
        public Graphics.Renderer.Settings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public Renderer(Device device)
        {
            this.device = device;
            Settings = new Settings();
            Frame = 1;
        }

        public Matrix ShadowMapCamera { get; set; }
        
        public bool ForceUpdateTree { get; set; }

        public int Frame { get; set; }

        #endregion

        #region Initial//Ending Functions

        public static string GetTechniqueNameExtension(MetaModel metaModel, Settings settings, string[, , , , ,] techniqueNames)
        {
            bool receiveShadows = false;
            if (settings.ShadowQualityPriorityRelation[settings.ShadowQuality] + settings.PriorityRelation[metaModel.ReceivesShadows] > 3)
                receiveShadows = true;

            bool receiveAmbient = false;
            if (settings.LightingQualityPriorityRelation[settings.LightingQuality] + settings.PriorityRelation[metaModel.ReceivesAmbientLight] > 2)
                receiveAmbient = true;

            bool receiveDiffuse = false;
            if (settings.LightingQualityPriorityRelation[settings.LightingQuality] + settings.PriorityRelation[metaModel.ReceivesDiffuseLight] > 2)
                receiveDiffuse = true;

            bool receiveSpecular = false;
            if (settings.LightingQualityPriorityRelation[settings.LightingQuality] + settings.PriorityRelation[metaModel.ReceivesSpecular] > 3)
                receiveSpecular = true;

            if (GraphicsDevice.SettingConverters.PixelShaderVersion.Major == 3)
            {
                return "3" + techniqueNames[receiveAmbient ? 1 : 0, receiveDiffuse ? 1 : 0,
                    receiveShadows ? 1 : 0, settings.WaterEnable ? 1 : 0, metaModel.ReceivesFog ? 1 : 0, receiveSpecular ? 1 : 0];
            }
            else
            {
                return "2" + techniqueNames[receiveAmbient ? 1 : 0, receiveDiffuse ? 1 : 0,
                    receiveShadows ? 1 : 0, settings.WaterEnable ? 1 : 0, metaModel.ReceivesFog ? 1 : 0, receiveSpecular ? 1 : 0];
            }
        }

        
        private void DisableShadowBuffersAndMaps(Device device)
        {
            splatEffect.SetValue(EHShadowQuality, Settings.ShadowQualityRelation[Settings.ShadowQuality]);
            meshEffect.SetValue(EHShadowQuality, Settings.ShadowQualityRelation[Settings.ShadowQuality]);
            skinnedMeshEffect.SetValue(EHShadowQuality, Settings.ShadowQualityRelation[Settings.ShadowQuality]);

            shadowMapWidth = Settings.ShadowQualityRelation[Settings.ShadowQuality];
            shadowMapHeight = shadowMapWidth;

            if (shadowMap != null)
            {
                shadowMap.Dispose();
                shadowMap = null;
            }
            if (depthBufferShadowMapSurface != null)
            {
                depthBufferShadowMapSurface.Dispose();
                depthBufferShadowMapSurface = null;
            }
            if (shadowMapSurface != null)
            {
                shadowMapSurface.Dispose();
                shadowMapSurface = null;
            }

            //for (int i = 0; i < optimizedShadowMapVB.Length; i++)
            //{
            //    if (optimizedShadowMapVB[i] != null)
            //        optimizedShadowMapVB[i].Dispose();
            //}

            //if (vdS != null)
            //    vdS.Dispose();

            meshEffect.SetTexture(EHShadowTexture, null);
            skinnedMeshEffect.SetTexture(EHShadowTexture, null);
            splatEffect.SetTexture(EHShadowTexture, null);
        }

        private Results InitShadowBuffersAndMaps(Device device)
        {
            if (shadowMap != null)
            {
                shadowMap.Dispose();
                shadowMap = null;
            }
            if (depthBufferShadowMapSurface != null)
            {
                depthBufferShadowMapSurface.Dispose();
                depthBufferScreenSurface = null;
            }

            shadowMapWidth = Settings.ShadowQualityRelation[Settings.ShadowQuality];
            shadowMapHeight = shadowMapWidth;

            splatEffect.SetValue(EHShadowQuality, Settings.ShadowQualityRelation[Settings.ShadowQuality]);
            meshEffect.SetValue(EHShadowQuality, Settings.ShadowQualityRelation[Settings.ShadowQuality]);
            skinnedMeshEffect.SetValue(EHShadowQuality, Settings.ShadowQualityRelation[Settings.ShadowQuality]);

            //Needs to adjust the format at lower qualities
            try
            {
                shadowMap = new Texture(device, shadowMapWidth, shadowMapHeight, 1, Usage.RenderTarget, Format.R32F, Pool.Default);
            }
            catch (Exception e)
            {
                Application.Log(e.StackTrace.ToString());
                return Results.OutOfVideoMemory;
            }
            depthBufferShadowMapSurface = Surface.CreateDepthStencil(device, shadowMapWidth, shadowMapHeight, Format.D16, MultisampleType.None, 0, false);

            //instancedShadowMapVertexElements = new VertexElement[]
            //{
            //    new VertexElement(0, 0,  DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            //    new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
            //    new VertexElement(0, 24, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
            //    new VertexElement(1, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 1),
            //    new VertexElement(1, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 2),
            //    new VertexElement(1, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 3),
            //    new VertexElement(1, 48, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 4),
            //    VertexElement.VertexDeclarationEnd
            //};

            //optimizedShadowMapVB = new VertexBuffer[nMeshes];
            //vdS = new VertexDeclaration(device, instancedShadowMapVertexElements);

            //for (int i = 0; i < nMeshes; i++)
            //{
            //    //dynamic vertexBufferSizeForInstancing
            //    optimizedShadowMapVB[i] = new VertexBuffer(device, vertexBufferSizeForInstancing *
            //        Vector4.SizeInBytes * 4, Usage.None, VertexFormat.None, Pool.Managed);
            //}

            return Results.OK;
        }

        public Results Initialize(View view)
        {
            Device device = view.Device9;
            ContentPool content = view.Content;

            ShadowMapCamera = Matrix.LookAtLH(new Vector3(10, 10, 20), Vector3.Zero, Vector3.UnitZ)
                * Matrix.OrthoLH(40, 40, 1, 40);
            ForceUpdateTree = false;

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++)
                        for (int l = 0; l < 2; l++)
                            for (int m = 0; m < 2; m++)
                                for (int n = 0; n < 2; n++)
                                {
                                    string name = "";

                                    if (i == 0)
                                        name += "NoAmbient";
                                    else
                                        name += "Ambient";

                                    if (j == 0)
                                        name += "NoDiffuse";
                                    else
                                        name += "Diffuse";

                                    if (k == 0)
                                        name += "NoShadows";
                                    else
                                        name += "Shadows";

                                    if (l == 0)
                                        name += "NoWater";
                                    else
                                        name += "Water";

                                    if (m == 0)
                                        name += "NoFog";
                                    else
                                        name += "Fog";

                                    if (n == 0)
                                        name += "NoSpecular";
                                    else
                                        name += "Specular";

                                    techniqueNames[i, j, k, l, m, n] = name;
                                }

#if LOG_RENDERTREE
            renderTreeLogFile = new StreamWriter(Graphics.Application.ApplicationDataFolder + "Logs/RenderTreeLog.txt");
            renderTreeLogFile.WriteLine("=========== Render Tree ===========");
#endif
#if RENDERER_STATISTICS
            rendererSatisticsLogFile = new StreamWriter(Graphics.Application.ApplicationDataFolder + "Logs/RendererStatisticsLog.txt");
            rendererSatisticsLogFile.WriteLine("=========== Renderer Statistics ===========");
#endif
#if PROFILE_RENDERER
            rendererProfileLogFile = new StreamWriter(Graphics.Application.ApplicationDataFolder + "Logs/RendererProfileLog.txt");
            rendererProfileLogFile.WriteLine("=========== Renderer Profile ===========");
#endif

            RenderRoot = new RenderRoot(techniqueNames);

            //if (Settings.RenderWithPostEffect)
            //{
            //    depthBuffForPost = Surface.CreateDepthStencil(device, Scene.View.GraphicsDevice.Settings.Resolution.Width, Scene.View.GraphicsDevice.Settings.Resolution.Height,
            //        Format.D16, MultisampleType.None, 0, false);

            //    renderedImageTexture = new Texture(device, Scene.View.GraphicsDevice.Settings.Resolution.Width, Scene.View.GraphicsDevice.Settings.Resolution.Height,
            //        1, Usage.RenderTarget, Format.A32B32G32R32F, Pool.Default);

            //    postEffect = content.Acquire<Effect>(new EffectFromFile("Shaders.PostEffects.fx"));
            //}

            meshEffect = content.Acquire<Effect>(new EffectFromFile("Shaders.MeshEffects.fx"));
            skinnedMeshEffect = content.Acquire<Effect>(new EffectFromFile("Shaders.SkinnedMeshEffects.fx"));
            splatEffect = content.Acquire<Effect>(new EffectFromFile("Shaders.Ground.fx"));
            waterEffect = content.Acquire<Effect>(new EffectFromFile("Shaders.Water.fx"));

            shadowsEnable = Settings.ShadowsEnable;

            if (Settings.ShadowsEnable)
            {
                Results r = InitShadowBuffersAndMaps(device);

                if (r == Results.OutOfVideoMemory)
                    return Results.OutOfVideoMemory;
            }            

            optimizedVB = new VertexBuffer[nMeshes];

            instancedVertexElements = new VertexElement[]
            {
                new VertexElement(0, 0,  DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                new VertexElement(0, 24, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                new VertexElement(1, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 1),
                new VertexElement(1, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 2),
                new VertexElement(1, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 3),
                new VertexElement(1, 48, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 4),
                //new VertexElement(1, 64, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 5),
                //new VertexElement(1, 76, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 6),
                //new VertexElement(1, 88, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 7),
                VertexElement.VertexDeclarationEnd
            };

            vd = new VertexDeclaration(device, instancedVertexElements);

            for (int i = 0; i < nMeshes; i++)
            {
                //dynamic vertexBufferSizeForInstancing
                optimizedVB[i] = new VertexBuffer(device, vertexBufferSizeForInstancing *
                    (Vector4.SizeInBytes * 4), Usage.None, VertexFormat.None, Pool.Managed);
            }

            screenRTSurface = device.GetRenderTarget(0);
            depthBufferScreenSurface = device.DepthStencilSurface;
            StateManager.SetRenderState(RenderState.Lighting, false);

            return Results.OK;
        }

        public void OnLostDevice(ContentPool content)
        {
            Release(content);
        }

        public void OnResetDevice(View view)
        {
            Initialize(view);
            StateManager.Reset();
            //foreach (Entity e in Scene.AllEntities) //needs investigation
            //    Add(e);
        }

        public void Release(ContentPool content)
        {
            //Textures begin
            if (shadowMap != null)
            {
                shadowMap.Dispose();
                shadowMap = null;
            }
            //if (renderedImageTexture != null)
            //{
            //    renderedImageTexture.Dispose();
            //    renderedImageTexture = null;
            //}
            //Textures end

            //Effects begin
            if (meshEffect != null)
            {
                content.Release(meshEffect);
                meshEffect = null;
            }
            if (skinnedMeshEffect != null)
            {
                content.Release(skinnedMeshEffect);
                skinnedMeshEffect = null;
            }
            if (splatEffect != null)
            {
                content.Release(splatEffect);
                splatEffect = null;
            }
            if (waterEffect != null)
            {
                content.Release(waterEffect);
                waterEffect = null;
            }
            //if (postEffect != null)
            //{
            //    Scene.View.Content.Release(postEffect);
            //    postEffect = null;
            //}
            //Effects end

            //Surfaces begin
            if (screenRTSurface != null)
            {
                screenRTSurface.Dispose();
                screenRTSurface = null;
            }
            if (depthBufferScreenSurface != null)
            {
                depthBufferScreenSurface.Dispose();
                depthBufferScreenSurface = null;
            }
            if (depthBufferShadowMapSurface != null)
            {
                depthBufferShadowMapSurface.Dispose();
                depthBufferShadowMapSurface = null;
            }
            if (shadowMapSurface != null)
            {
                if(!shadowMapSurface.Disposed)
                    shadowMapSurface.Dispose();
                shadowMapSurface = null;
            }
            if (depthBuffForPost != null)
            {
                depthBuffForPost.Dispose();
                depthBuffForPost = null;
            }
            if (renderedImage != null)
            {
                renderedImage.Dispose();
                renderedImage = null;
            }
            //Surfaces end

            if (optimizedVB != null)
                for (int i = 0; i < optimizedVB.Length; i++)
                    if (optimizedVB[i] != null)
                    {
                        optimizedVB[i].Dispose();
                        optimizedVB[i] = null;
                    }

            if (vd != null)
            {
                vd.Dispose();
                vd = null;
            }


#if LOG_RENDERTREE
            renderTreeLogFile.Close();
#endif
#if RENDERER_STATISTICS
            rendererSatisticsLogFile.Close();
#endif
#if PROFILE_RENDERER
            rendererProfileLogFile.Close();
#endif
        }

        #endregion

        #region Events

        public void Add(Entity entity, MetaResource<Model9, Model10> metaResource, Model9 model, string metaName)
        {
            if (RenderRoot != null)
            {
                RenderRoot.Insert(model, entity, (MetaModel)((MetaModel)metaResource).Clone(), metaName, Settings);
            }
            else
                throw new Exception("Renderer needs initialization.");
        }

        public void Remove(Entity entity, MetaResource<Model9, Model10> metaResource, Model9 model, string metaName)
        {
            MetaModel metaModel = (MetaModel)metaResource;
            if (metaModel.HasAlpha)
            {
                RenderRoot.AlphaObjects.Remove(new Common.Tuple<Model9, Entity, string, string>(model, entity, metaName, GetTechniqueNameExtension(metaModel, Settings, techniqueNames)));

#if LOG_RENDERTREE
                //renderTreeLogFile.WriteLine("R | Alpha");
                totalRemovedItems++;
#endif
            }
            else if (metaModel.SplatMapped)
            {
                if(entity.Scene.DesignMode)
                    RenderRoot.SplatObjects.Remove(new Common.Tuple<Model9, Entity, string>(model, entity, metaName));

                RenderRoot.SplatTechniques["Standard" + GetTechniqueNameExtension(metaModel, Settings, techniqueNames) + RenderRoot.GetSplatTechniqueExtention(model, Settings)].TextureCombinations[new SplatTextureCombination { BaseTexture = model.BaseTexture, MaterialTexture = model.MaterialTexture, SplatTexture = model.SplatTexture }].RenderObjects.Remove(new Common.Tuple<Model9,Entity,string>(model, entity, metaName));
#if LOG_RENDERTREE
                //renderTreeLogFile.WriteLine("R | Splat");
                totalRemovedItems++;
#endif
            }
            else if (model.SkinnedMesh != null)
            {
                foreach (Common.Tuple<CustomFrame, CustomMeshContainer> meshContainer in model.SkinnedMesh.MeshContainers)
                {
                    if (meshContainer.Second.SkinInfo != null)
                    {
                        string tmp = "SkinnedMesh" + GetTechniqueNameExtension(metaModel, Settings, techniqueNames);

                        RenderRoot.Techniques[tmp].SkinnedMeshes[model.SkinnedMesh].Textures[model.Texture].RenderObjects.Remove(new Common.Tuple<Model9, Entity, string>(model, entity, metaName));

                        if (RenderRoot.Techniques[tmp].SkinnedMeshes[model.SkinnedMesh].Textures[model.Texture].RenderObjects.Count == 0)
                            RenderRoot.Techniques[tmp].SkinnedMeshes[model.SkinnedMesh].Textures.Remove(model.Texture);

#if LOG_RENDERTREE
                        //renderTreeLogFile.WriteLine("R | SkinnedMesh");
                        totalRemovedItems++;
#endif
                    }
                    else
                    {
#if LOG_RENDERTREE
                        //renderTreeLogFile.WriteLine("R | Mesh");
                        totalRemovedItems++;
#endif
                        string tmp = "ShadowedSceneInstanced" + GetTechniqueNameExtension(metaModel, Settings, techniqueNames);

#if DEBUG_RENDERER
                        RenderRoot.Techniques[tmp].Meshes[meshContainer.Second.MeshData.Mesh].Textures[model.Texture].RenderObjects.Remove(new Common.Tuple<Model9, Entity, string>(model, entity, metaName));

                        if (RenderRoot.Techniques[tmp].Meshes[meshContainer.Second.MeshData.Mesh].Textures[model.Texture].RenderObjects.Count == 0)
                            RenderRoot.Techniques[tmp].Meshes[meshContainer.Second.MeshData.Mesh].Textures.Remove(model.Texture);
#else
                        if (!model.SkinnedMesh.MeshContainers[0].Second.OriginalMesh.Disposed)
                            RenderRoot.Techniques[tmp].Meshes[meshContainer.Second.MeshData.Mesh].Textures[model.Texture].RenderObjects.Remove(new Common.Tuple<Model9, Entity, string>(model, entity, metaName));

                        if (meshContainer.Second.MeshData != null)
                            if (RenderRoot.Techniques[tmp].Meshes[meshContainer.Second.MeshData.Mesh].Textures[model.Texture].RenderObjects.Count == 0)
                                RenderRoot.Techniques[tmp].Meshes[meshContainer.Second.MeshData.Mesh].Textures.Remove(model.Texture);
#endif
                    }
                }

                //RenderRoot.skinnedMeshes[model.SkinnedMesh].textures[model.Texture].RenderObjects.Remove(new Common.Tuple<Model9, Entity, string>(model, entity, metaName));
            }
            else if (model.XMesh != null)
            {
                string tmp = "ShadowedSceneInstanced" + GetTechniqueNameExtension(metaModel, Settings, techniqueNames);

                RenderRoot.Techniques[tmp].Meshes[model.XMesh].Textures[model.Texture].RenderObjects.Remove(new Common.Tuple<Model9, Entity, string>(model, entity, metaName));

                if (RenderRoot.Techniques[tmp].Meshes[model.XMesh].Textures[model.Texture].RenderObjects.Count == 0)
                    RenderRoot.Techniques[tmp].Meshes[model.XMesh].Textures.Remove(model.Texture);

                //RenderRoot.meshes[model.XMesh].textures[model.Texture].RenderObjects.Remove(new Common.Tuple<Model9, Entity, string>(model, entity, metaName));
#if LOG_RENDERTREE
                //renderTreeLogFile.WriteLine("R | XMesh");
                totalRemovedItems++;
#endif
            }
            if (!releaseWarningAdded)
            {
                releaseWarningAdded = true;
                Common.ProgramConfigurationInformation.AddWarning(new Common.ProgramConfigurationWarning
                {
                    Text = "RENDERER_RELEASE",
                    Type = Common.ProgramConfigurationWarningType.Performance | Common.ProgramConfigurationWarningType.Stability,
                    Importance = Common.Importance.Critical,
                    Description = "Content release is commented out in the renderer"
                });
            }
        }

        public static MetaModel GetLatestMetaModel(Entity entity, string id)
        {
            foreach (MetaModel v in entity.AllGraphics)
                if (v.InstanceID == id)
                    return v;
            return null;
        }

        #endregion

        #region InternalHelpFunctions

        public int GetNumberOfOldSplatObjects()
        {
            return RenderRoot.SplatObjects.Count;
        }

        public int GetNumberOfNewSplatObjects()
        {
            int newSplatObjects = 0;

            foreach (string technique in RenderRoot.SplatTechniques.Keys)
            {
                foreach (SplatTextureCombination stc in RenderRoot.SplatTechniques[technique].TextureCombinations.Keys)
                {
                    newSplatObjects += RenderRoot.SplatTechniques[technique].TextureCombinations[stc].RenderObjects.Count;
                }
            }

            return newSplatObjects;
        }

        public int GetNumberOfAlphaObjects()
        {
            return RenderRoot.AlphaObjects.Count;
        }

        public int GetNumberOfSkinnedMeshObjects()
        {
            int skinnedMeshObjects = 0;

            foreach (string technique in RenderRoot.Techniques.Keys)
            {
                //if (technique.StartsWith("Standard"))
                //{
                    foreach (SkinnedMesh skinnedMesh in RenderRoot.Techniques[technique].SkinnedMeshes.Keys)
                    {
                        foreach (Texture t in RenderRoot.Techniques[technique].SkinnedMeshes[skinnedMesh].Textures.Keys)
                        {
                            skinnedMeshObjects += RenderRoot.Techniques[technique].SkinnedMeshes[skinnedMesh].Textures[t].RenderObjects.Count;
                        }
                    }
                //}
            }

            return skinnedMeshObjects;
        }

        public int GetNumberOfXMeshes()
        {
            int meshObjects = 0;
            foreach (string technique in RenderRoot.Techniques.Keys)
            {
                foreach (SlimDX.Direct3D9.Mesh mesh in RenderRoot.Techniques[technique].Meshes.Keys)
                {
                    foreach (Texture t in RenderRoot.Techniques[technique].Meshes[mesh].Textures.Keys)
                    {
                        meshObjects += RenderRoot.Techniques[technique].Meshes[mesh].Textures[t].RenderObjects.Count;
                    }
                }
            }
            return meshObjects;
        }

        public int GetNumberOfItemsInTree()
        {
            return GetNumberOfAlphaObjects() +
                GetNumberOfNewSplatObjects() +
                GetNumberOfOldSplatObjects() +
                GetNumberOfSkinnedMeshObjects() +
                GetNumberOfXMeshes();
        }

        private void DrawInstancedIndexedPrimitives(Device device, SlimDX.Direct3D9.Mesh mesh, VertexBuffer vertexBuffer, VertexBuffer instancedVertexData, VertexElement[] vertexElements, int nIndexedElements)
        {
            device.SetStreamSourceFrequency(0, nIndexedElements, StreamSource.IndexedData);
            device.SetStreamSource(0, vertexBuffer, 0, D3DX.GetDeclarationVertexSize(vertexElements, 0));

            device.SetStreamSourceFrequency(1, 1, StreamSource.InstanceData);
            device.SetStreamSource(1, instancedVertexData, 0, D3DX.GetDeclarationVertexSize(vertexElements, 1));

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.VertexCount, 0, mesh.FaceCount);
        }

        static bool releaseWarningAdded = false;

        private void UpdateEffect(Effect effect)
        {
            //effect.SetValue(EHAmbientFactor, new Vector4(Settings.AmbientFactor, 1));
            effect.SetValue(EHDiffuseColor, new Vector4(Settings.DiffuseColor, 1));
            effect.SetValue(EHFogColor, Settings.FogColor);
            effect.SetValue(EHSpecularColor, new Vector4(Settings.SpecularColor, 1));

            effect.SetValue(EHShadowBias, Settings.ShadowBias);
            effect.SetValue(EHFogDistance, Settings.FogDistance);
            effect.SetValue(EHLightDirection, Settings.LightDirection);
            effect.SetValue(EHCameraPosition, Scene.Camera.Position);

            effect.SetValue(EHWaterLevel, Settings.WaterLevel);
            effect.SetValue(EHFogExponent, Settings.FogExponent);
        }

        private void UpdateEffectWithMetaModel(Effect effect, MetaModel metaModel, bool withShadows)
        {
            //if (metaModel.HasAlpha)
                effect.SetValue(EHOpacity, metaModel.Opacity);

            if (metaModel.AmbientLight.HasValue)
                effect.SetValue(EHAmbientColor, metaModel.AmbientLight.Value);
            else
                effect.SetValue(EHAmbientColor, new Vector4(Settings.AmbientColor, 1));

            //if(metaModel.SpecularTexture != null)
                effect.SetValue(EHSpecularExponent, metaModel.SpecularExponent);

            StateManager.SetRenderState(RenderState.AlphaRef, metaModel.AlphaRef);

            StateManager.SetSamplerState(0, SamplerState.AddressU, metaModel.TextureAddress);
            StateManager.SetSamplerState(0, SamplerState.AddressV, metaModel.TextureAddress);

            //StateManager.SetSamplerState(1, SamplerState.AddressU, metaModel.TextureAddress);
            //StateManager.SetSamplerState(1, SamplerState.AddressV, metaModel.TextureAddress);
            //StateManager.SetSamplerState(2, SamplerState.AddressU, metaModel.TextureAddress);
            //StateManager.SetSamplerState(2, SamplerState.AddressV, metaModel.TextureAddress);

            //for (int i = 0; i < 1; i++)
            //{
            //    StateManager.SetSamplerState(i, SamplerState.MinFilter, Scene.View.GraphicsDevice.Settings.TextureFilter.TextureFilterMin);
            //    StateManager.SetSamplerState(i, SamplerState.MagFilter, Scene.View.GraphicsDevice.Settings.TextureFilter.TextureFilterMag);
            //    StateManager.SetSamplerState(i, SamplerState.MipFilter, TextureFilter.Point);
            //    StateManager.SetSamplerState(i, SamplerState.MaxAnisotropy, Scene.View.GraphicsDevice.Settings.TextureFilter.AnisotropicLevel);
            //}
        }

        [Obsolete]
        public void UpdateTree()
        {
            throw new NotSupportedException("No longer supported.");
            //foreach (var e in new List<Entity>(scene.AllEntities))
            //    if (e.ActiveInMain == Frame || e.ActiveInShadowMap == Frame)
            //    {
            //        var tmp = e.RendererGraphics;
            //    }

            //foreach (var e in Resources)
            //{
            //    if (e.Key.ActiveInMain == Frame || e.Key.ActiveInShadowMap == Frame)
            //    {
            //        //if (e.Key.Graphic != null)
            //            foreach (var v in e.Key.RendererGraphics)
            //            {
            //                var k = e.Value[v.Key];
            //                var oldModel = k.First;
            //                var oldMetaModel = k.Second;
            //                var currentMetaModel = v.Value;

            //                if (!((MetaModel)currentMetaModel).RendererEquals((MetaModel)oldMetaModel))
            //                {
            //                    RemoveModelEntity(oldModel, e.Key, (MetaModel)oldMetaModel, v.Key);
            //                    var model = Scene.View.Content.Acquire<Model9>(currentMetaModel);
            //                    Add(e.Key, (MetaModel)currentMetaModel, v.Key, model);
            //                    e.Value[v.Key] = new Common.Tuple<Model9, MetaModel>(model, (MetaModel)currentMetaModel.Clone());
            //                }
            //            }
            //    }
            //}
        }

        private void RenderSkinnedMeshWithoutSkinning(Common.Tuple<CustomFrame, CustomMeshContainer> SM, Effect effect, bool withShadows, Matrix viewProjection, Matrix[][] combinedTransforms)
        {
            if (withShadows)
            {
                effect.SetValue(EHShadowWorldViewProjection, combinedTransforms[0][0] * ShadowMapCamera);
                effect.CommitChanges();
                SM.Second.MeshData.Mesh.DrawSubset(0);
            }
            else
            {
                effect.SetValue(EHShadowWorldViewProjection, combinedTransforms[0][0] * ShadowMapCamera);
                effect.SetValue(EHWorldViewProjectionMatrix, combinedTransforms[0][0] * viewProjection);
                //effect.SetValue(EHWorldMatrix, SM.First.CombinedTransform);
                effect.SetValue(EHWorldMatrix, combinedTransforms[0][0]);
                //effect.SetValue(EHNormalMatrix, combinedTransforms[0].Second[0]);

                effect.CommitChanges();
                SM.Second.MeshData.Mesh.DrawSubset(0);
            }
        }

        private void RenderSkinnedMeshWithSkinning(Common.Tuple<CustomFrame, CustomMeshContainer> SM, Effect effect, bool withShadows, Matrix[][] combinedTransforms)
        {
            for (int i = 0; i < combinedTransforms.Length; i++)
            {
                if (combinedTransforms[i] != null)
                {
                    effect.SetValue(EHWorldMatrices, combinedTransforms[i]);
                    effect.SetValue(EHCurrentBoneCount, SM.Second.Influences - 1);

                    //if (!withShadows)
                        //effect.SetValue(EHNormalMatrices, combinedTransforms[i].Second);

                    effect.CommitChanges();

                    SM.Second.MeshData.Mesh.DrawSubset(i);
                }
            }
        }

        private void RenderSkinnedMesh(Common.Tuple<CustomFrame, CustomMeshContainer> SM, bool withShadows, Effect effect, bool hasSkinning, Matrix viewProjection, Dictionary<SlimDX.Direct3D9.Mesh, Matrix[][]> combinedTransforms)
        {
            if (SM.Second.SkinInfo != null && hasSkinning)
            {
                RenderSkinnedMeshWithSkinning(SM, effect, withShadows, combinedTransforms[SM.Second.MeshData.Mesh]);
            }
            else if (!hasSkinning && SM.Second.SkinInfo == null)
            {
                RenderSkinnedMeshWithoutSkinning(SM, effect, withShadows, viewProjection, combinedTransforms[SM.Second.MeshData.Mesh]);
            }
        }

        private void RenderSkinnedMeshes(Dictionary<SkinnedMesh, RenderSkinnedMesh> skinnedMeshes, Effect effect, bool withShadows, bool hasSkinning, Matrix viewProjection, string technique)
        {
            effect.Technique = technique;
            effect.Begin(fx);
            effect.BeginPass(0);

            foreach (SkinnedMesh skinnedMesh in skinnedMeshes.Keys)
            {
                foreach (Texture texture in skinnedMeshes[skinnedMesh].Textures.Keys)
                {
                    bool first = true;
                    effect.SetTexture(EHTexture, texture);

                    foreach (Common.Tuple<Model9, Entity, string> renderObject in skinnedMeshes[skinnedMesh].Textures[texture].RenderObjects)
                    {
                        Entity entity = renderObject.Second;
                        MetaModel metaModel = GetLatestMetaModel(renderObject.Second, renderObject.Third);

                        //if (Settings.TerrainQualityPriorityRelation[Settings.TerrainQuality] + Settings.PriorityRelation[metaModel.Visible] < 3)
                        //    continue;

                        if (first)
                        {
                            //if (Settings.TerrainQualityPriorityRelation[Settings.TerrainQuality] + Settings.PriorityRelation[metaModel.Visible] < 3)
                            //    break;

                            if (skinnedMeshes[skinnedMesh].Textures[texture].RenderObjects[0].First.SpecularTexture != null && !withShadows)
                                effect.SetTexture(EHSpecularMap, skinnedMeshes[skinnedMesh].Textures[texture].RenderObjects[0].First.SpecularTexture);

                            first = false;
                        }

                        if ((!withShadows && (entity.ActiveInMain == Frame)) || (withShadows && (entity.ActiveInShadowMap == Frame) && (Settings.PriorityRelation[metaModel.CastShadows] + Settings.ShadowQualityPriorityRelation[Settings.ShadowQuality] > 3)))
                        {
                            UpdateEffectWithMetaModel(effect, metaModel, withShadows);

                            foreach (var SM in skinnedMesh.MeshContainers)
                                RenderSkinnedMesh(SM, withShadows, effect, hasSkinning, viewProjection, metaModel.StoredFrameMatrices);
                        }
                    }
                }
            }

            effect.EndPass();
            effect.End();
        }

        private void RenderXMeshesWithInstancingShadows(Dictionary<SlimDX.Direct3D9.Mesh, RenderMesh> meshes, Device device)
        {
            int k = 0;

            device.VertexFormat = VertexFormat.None;
            device.VertexDeclaration = vd;

            foreach (SlimDX.Direct3D9.Mesh mesh in meshes.Keys)
            {
                device.Indices = meshes[mesh].IndexBuffer;

                foreach (Texture texture in meshes[mesh].Textures.Keys)
                {
                    bool first = true;
                    int numberOfInstancedDraws = 0;

                    DataStream ds = null;

                    foreach (Common.Tuple<Graphics.Content.Model9, Entity, string> renderObject in meshes[mesh].Textures[texture].RenderObjects)
                    {
                        Entity entity = renderObject.Second;

                        if (entity.ActiveInShadowMap == Frame)
                        {
                            MetaModel metaModel = GetLatestMetaModel(entity, renderObject.Third);

                            if (first)
                            {
                                //if ((Settings.TerrainQualityPriorityRelation[Settings.TerrainQuality] + Settings.PriorityRelation[metaModel.Visible] < 3) ||
                                //    !(Settings.PriorityRelation[metaModel.CastShadows] + Settings.ShadowQualityPriorityRelation[Settings.ShadowQuality] > 3))
                                //    break;

                                meshEffect.SetTexture(EHTexture, texture);
                                ds = optimizedVB[k % optimizedVB.Length].Lock(0, meshes[mesh].Textures[texture].RenderObjects.Count * (Vector4.SizeInBytes * 4), LockFlags.None);

                                UpdateEffectWithMetaModel(meshEffect, metaModel, true);

                                meshEffect.CommitChanges();
                                first = false;
                            }

                            Matrix worldMatrix;

                            if (renderObject.First.SkinnedMesh != null)
                                worldMatrix = metaModel.StoredFrameMatrices[mesh][0][0];
                            else
                                worldMatrix = metaModel.GetWorldMatrix(Scene.Camera, renderObject.Second);

                            ds.Write(worldMatrix);

                            numberOfInstancedDraws++;
                        }
                    }

                    if (numberOfInstancedDraws == 0)
                        continue;

                    optimizedVB[k % optimizedVB.Length].Unlock();

                    DrawInstancedIndexedPrimitives(device, mesh, meshes[mesh].VertexBuffer, optimizedVB[k % optimizedVB.Length], instancedVertexElements,
                        numberOfInstancedDraws);

                    k++;
                }
            }

            device.SetStreamSourceFrequency(0, 1, StreamSource.IndexedData);
            device.SetStreamSourceFrequency(1, 0, StreamSource.InstanceData);
            device.SetStreamSource(0, null, 0, 0);
            device.SetStreamSource(1, null, 0, 0);
            device.ResetStreamSourceFrequency(0);
        }

        private void RenderXMeshesWithInstancing(Dictionary<SlimDX.Direct3D9.Mesh, RenderMesh> meshes, Device device, Effect effect, string technique)
        {
            effect.Technique = technique;

            effect.Begin(fx);
            effect.BeginPass(0);

            int k = 0;

            device.VertexFormat = VertexFormat.None;
            device.VertexDeclaration = vd;

            foreach (SlimDX.Direct3D9.Mesh mesh in meshes.Keys)
            {
                device.Indices = meshes[mesh].IndexBuffer;

                foreach (Texture texture in meshes[mesh].Textures.Keys)
                {
                    bool first = true;
                    int numberOfInstancedDraws = 0;

                    DataStream ds = null;

                    foreach (Common.Tuple<Graphics.Content.Model9, Entity, string> renderObject in meshes[mesh].Textures[texture].RenderObjects)
                    {
                        Entity entity = renderObject.Second;

                        if (entity.ActiveInMain == Frame)
                        {
                            if (first)
                            {
                                MetaModel metaModel = GetLatestMetaModel(entity, renderObject.Third);

                                //if (Settings.PriorityRelation[metaModel.Visible] + Settings.TerrainQualityPriorityRelation[Settings.TerrainQuality] < 3)
                                //    break;

                                effect.SetTexture(EHTexture, texture);
                                ds = optimizedVB[k % optimizedVB.Length].Lock(0, meshes[mesh].Textures[texture].RenderObjects.Count * (Vector4.SizeInBytes * 4), LockFlags.None);

                                if (renderObject.First.SpecularTexture != null)
                                    effect.SetTexture(EHSpecularMap, renderObject.First.SpecularTexture);

                                UpdateEffectWithMetaModel(effect, metaModel, false);

                                effect.CommitChanges();
                                first = false;
                            }

                            Matrix worldMatrix;
                            //Matrix normalMatrix;

                            if (renderObject.First.SkinnedMesh != null)
                            {
                                Matrix[] storedMatrices =
                                    GetLatestMetaModel(renderObject.Second, renderObject.Third).StoredFrameMatrices[mesh][0];
                                worldMatrix = storedMatrices[0];
                                //normalMatrix = storedMatrices.Second[0];
                            }
                            else
                            {
                                MetaModel metaModel = GetLatestMetaModel(entity, renderObject.Third);
                                worldMatrix = metaModel.GetWorldMatrix(Scene.Camera, renderObject.Second);
                                //normalMatrix = Matrix.Transpose(Matrix.Invert(worldMatrix));
                            }

                            ds.Write(worldMatrix);

                            //ThreeTimesThreeMatrix matrix;

                            //matrix.M11 = normalMatrix.M11;
                            //matrix.M12 = normalMatrix.M12;
                            //matrix.M13 = normalMatrix.M13;
                            //matrix.M21 = normalMatrix.M21;
                            //matrix.M22 = normalMatrix.M22;
                            //matrix.M23 = normalMatrix.M23;
                            //matrix.M31 = normalMatrix.M31;
                            //matrix.M32 = normalMatrix.M32;
                            //matrix.M33 = normalMatrix.M33;
                            //ds.Write(matrix);

                            numberOfInstancedDraws++;
                        }
                    }

#if RENDERER_STATISTICS
                    if (maxNumberOfInstancedDrawCalls < numberOfInstancedDraws)
                        maxNumberOfInstancedDrawCalls = numberOfInstancedDraws;
                    if (minNumberOfInstancedDrawCalls > numberOfInstancedDraws)
                        minNumberOfInstancedDrawCalls = numberOfInstancedDraws;
#endif

                    if (numberOfInstancedDraws == 0)
                        continue;

                    optimizedVB[k % optimizedVB.Length].Unlock();

                    DrawInstancedIndexedPrimitives(device, mesh, meshes[mesh].VertexBuffer, optimizedVB[k % optimizedVB.Length], instancedVertexElements,
                            numberOfInstancedDraws);

                    k++;
                }
            }

            effect.EndPass();
            effect.End();

            device.SetStreamSourceFrequency(0, 1, StreamSource.IndexedData);
            device.SetStreamSourceFrequency(1, 0, StreamSource.InstanceData);
            device.SetStreamSource(0, null, 0, 0);
            device.SetStreamSource(1, null, 0, 0);
            device.ResetStreamSourceFrequency(0);
        }

        private void RenderXMeshes(Dictionary<SlimDX.Direct3D9.Mesh, RenderMesh> meshes, Effect effect, Matrix viewProjection, string technique)
        {
            effect.Technique = technique;

            effect.Begin(fx);
            effect.BeginPass(0);

            foreach (SlimDX.Direct3D9.Mesh mesh in meshes.Keys)
            {
                foreach (Texture texture in meshes[mesh].Textures.Keys)
                {
                    effect.SetTexture(EHTexture, texture);
                    if (meshes[mesh].Textures[texture].RenderObjects[0].First.SpecularTexture != null)
                        effect.SetTexture(EHSpecularMap, meshes[mesh].Textures[texture].RenderObjects[0].First.SpecularTexture);

                    foreach (Common.Tuple<Graphics.Content.Model9, Entity, string> renderObject in meshes[mesh].Textures[texture].RenderObjects)
                    {
                        if (renderObject.Second.ActiveInMain == Frame)
                        {
                            Matrix worldMatrix;
                            MetaModel metaModel = GetLatestMetaModel(renderObject.Second, renderObject.Third);

                            if (renderObject.First.SkinnedMesh != null)
                            {
                                worldMatrix = metaModel.StoredFrameMatrices[mesh][0][0];
                            }
                            else
                            {
                                worldMatrix = metaModel.GetWorldMatrix(Scene.Camera, renderObject.Second);
                                worldMatrix = Matrix.Identity;
                            }
                            UpdateEffectWithMetaModel(effect, metaModel, false);

                            effect.SetValue(EHShadowWorldViewProjection, worldMatrix * ShadowMapCamera);
                            effect.SetValue(EHWorldViewProjectionMatrix, worldMatrix * viewProjection);
                            effect.SetValue(EHWorldMatrix, worldMatrix);

                            effect.CommitChanges();

                            mesh.DrawSubset(0);
                        }
                    }
                }
            }

            effect.EndPass();
            effect.End();
        }

        private void RenderXMeshesWithDevice(Device device, Dictionary<SlimDX.Direct3D9.Mesh, RenderMesh> meshes)
        {
            foreach (SlimDX.Direct3D9.Mesh mesh in meshes.Keys)
            {
                foreach (Texture texture in meshes[mesh].Textures.Keys)
                {
                    device.SetTexture(0, texture);
                    foreach (Common.Tuple<Graphics.Content.Model9, Entity, string> renderObject in meshes[mesh].Textures[texture].RenderObjects)
                    {
                        device.SetTransform(TransformState.World,
                            GetLatestMetaModel(renderObject.Second, renderObject.Third).GetWorldMatrix(Scene.Camera, renderObject.Second));
                        device.SetTransform(TransformState.View, Scene.Camera.View);
                        device.SetTransform(TransformState.Projection, Scene.Camera.Projection);
                        mesh.DrawSubset(0);
                    }
                }
            }
        }

        private void RenderSplatmappedMesh(Common.Tuple<Model9, Entity, string> modelEntity, string techniqueName, Matrix ViewProjection)
        {
            if (modelEntity.Second.ActiveInMain == Frame)
            {
                MetaModel metaModel = GetLatestMetaModel(modelEntity.Second, modelEntity.Third);

                Matrix world = metaModel.GetWorldMatrix(Scene.Camera, modelEntity.Second);

                splatEffect.SetValue(EHWorldViewProjectionMatrix, world * ViewProjection);
                splatEffect.SetValue(EHWorldMatrix, world);
                splatEffect.SetValue(EHShadowWorldViewProjection, world * ShadowMapCamera);

                //Test for solving air fog being affected by water depth                   

                UpdateEffectWithMetaModel(splatEffect, metaModel, false);
                if (modelEntity.First.BaseTexture != null)
                    splatEffect.SetTexture(EHBaseTexture, modelEntity.First.BaseTexture);
                
                splatEffect.SetTexture(EHSpecularMap, modelEntity.First.SpecularTexture);

                bool splat1 = false;
                bool splat2 = false;

                if (modelEntity.First.MaterialTexture[0] != null)
                {
                    splatEffect.SetTexture(EHTerrainTexture1, modelEntity.First.MaterialTexture[0]);
                    splat1 = true;
                }
                else
                {
                    splatEffect.SetTexture(EHTerrainTexture1, null);
                }

                if (modelEntity.First.MaterialTexture[1] != null)
                {
                    splatEffect.SetTexture(EHTerrainTexture2, modelEntity.First.MaterialTexture[1]);
                    splat1 = true;
                }
                else
                {
                    splatEffect.SetTexture(EHTerrainTexture2, null);
                }

                if (modelEntity.First.MaterialTexture[2] != null)
                {
                    splatEffect.SetTexture(EHTerrainTexture3, modelEntity.First.MaterialTexture[2]);
                    splat1 = true;
                }
                else
                {
                    splatEffect.SetTexture(EHTerrainTexture3, null);
                }

                if (modelEntity.First.MaterialTexture[3] != null)
                {
                    splatEffect.SetTexture(EHTerrainTexture4, modelEntity.First.MaterialTexture[3]);
                    splat1 = true;
                }
                else
                {
                    splatEffect.SetTexture(EHTerrainTexture4, null);
                }

                if (modelEntity.First.MaterialTexture[4] != null)
                {
                    splatEffect.SetTexture(EHTerrainTexture5, modelEntity.First.MaterialTexture[4]);
                    splat2 = true;
                }
                else
                {
                    splatEffect.SetTexture(EHTerrainTexture5, null);
                }

                if (modelEntity.First.MaterialTexture[5] != null)
                {
                    splatEffect.SetTexture(EHTerrainTexture6, modelEntity.First.MaterialTexture[5]);
                    splat2 = true;
                }
                else
                {
                    splatEffect.SetTexture(EHTerrainTexture6, null);
                }

                if (modelEntity.First.MaterialTexture[6] != null)
                {
                    splatEffect.SetTexture(EHTerrainTexture7, modelEntity.First.MaterialTexture[6]);
                    splat2 = true;
                }
                else
                {
                    splatEffect.SetTexture(EHTerrainTexture7, null);
                }
                
                if (modelEntity.First.MaterialTexture[7] != null)
                {
                    splatEffect.SetTexture(EHTerrainTexture8, modelEntity.First.MaterialTexture[7]);
                    splat2 = true;
                }
                else
                {
                    splatEffect.SetTexture(EHTerrainTexture8, null);
                }

                string techniqueExtention = "";

                if (splat1)
                {
                    techniqueExtention += "Splat1";
                    splatEffect.SetTexture(EHSplatTexture1, modelEntity.First.SplatTexture[0]);
                }
                else
                {
                    techniqueExtention += "NoSplat1";
                    splatEffect.SetTexture(EHSplatTexture1, null);
                }
                if (splat2)
                {
                    techniqueExtention += "Splat2";
                    splatEffect.SetTexture(EHSplatTexture2, modelEntity.First.SplatTexture[1]);
                }
                else
                {
                    techniqueExtention += "NoSplat2";
                    splatEffect.SetTexture(EHSplatTexture2, null);
                }

                if (Settings.TerrainQuality == Settings.TerrainQualities.Low)
                {
                    splatEffect.Technique = techniqueName + techniqueExtention + "Lowest";
                }
                else
                {
                    splatEffect.Technique = techniqueName + techniqueExtention + "NoLowest";
                }

                splatEffect.Begin(fx);
                splatEffect.BeginPass(0);

                if (modelEntity.First.XMesh != null)
                    modelEntity.First.XMesh.DrawSubset(0);

                splatEffect.EndPass();
                splatEffect.End();
            }
        }


        #endregion

        #region RenderFunctions

        public void PreRender(float dtime)
        {

#if RENDERER_STATISTICS
            rendererSatisticsLogFile.WriteLine("==== Frame " + Frame + " ====");
#endif

#if LOG_RENDERTREE
            renderTreeLogFile.WriteLine("==== Frame " + Frame + " ====");
#endif

#if PROFILE_RENDERER
            rendererProfileLogFile.WriteLine("==== Frame " + Frame + " ====");
#endif

            UpdateEffect(meshEffect);
            UpdateEffect(skinnedMeshEffect);
            UpdateEffect(splatEffect);
            UpdateEffect(waterEffect);

            if (shadowMapWidth != Settings.ShadowQualityRelation[Settings.ShadowQuality] || shadowsEnable != Settings.ShadowsEnable)
            {
                throw new NotSupportedException("Changing shadowquality is not available in-game");
                //ForceUpdateTree = true;
                //shadowsEnable = Settings.ShadowsEnable;

                //if (shadowsEnable)
                //{
                //    InitShadowBuffersAndMaps(device);
                //}
                //else
                //{
                //    DisableShadowBuffersAndMaps(device);
                //}
            }

            //if (scene.DesignMode || ForceUpdateTree)
            //{
            //    UpdateTree();
            //    ForceUpdateTree = false;
            //}

            if (shadowsEnable)
            {
#if PROFILE_RENDERER
                var t = System.DateTime.Now;
#endif
                shadowMapSurface = shadowMap.GetSurfaceLevel(0);
                device.SetRenderTarget(0, shadowMapSurface);
                device.DepthStencilSurface = depthBufferShadowMapSurface;
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);
                device.BeginScene();

                //check redundant states
                StateManager.SetRenderState(RenderState.AlphaTestEnable, true);
                StateManager.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
                StateManager.SetRenderState(RenderState.AlphaBlendEnable, false);
                StateManager.SetRenderState(RenderState.ZEnable, true);
                StateManager.SetRenderState(RenderState.ZWriteEnable, true);
                StateManager.SetRenderState(RenderState.MultisampleAntialias, false);

                meshEffect.Technique = "StandardShadowMap";

                meshEffect.Begin(fx);
                meshEffect.BeginPass(0);

                foreach (Common.Tuple<Model9, Entity, string> modelEntity in RenderRoot.SplatObjects)
                {
                    if (modelEntity.Second.ActiveInShadowMap == Frame)
                    {
                        MetaModel metaModel = GetLatestMetaModel(modelEntity.Second, modelEntity.Third);

                        if (Settings.PriorityRelation[metaModel.CastShadows] + Settings.ShadowQualityPriorityRelation[Settings.ShadowQuality] > 3)
                        {
                            UpdateEffectWithMetaModel(meshEffect, metaModel, true);
                            Matrix world = metaModel.GetWorldMatrix(Scene.Camera, modelEntity.Second);

                            meshEffect.SetValue(EHShadowWorldViewProjection, world * ShadowMapCamera);

                            meshEffect.CommitChanges();

                            if (modelEntity.First.XMesh != null)
                                modelEntity.First.XMesh.DrawSubset(0);
                        }
                    }
                }

                meshEffect.EndPass();
                meshEffect.End();


                //skinnedMeshEffect.Technique = "SkinnedMeshShadowMap";
                skinnedMeshEffect.SetValue(EHShadowViewProjection, ShadowMapCamera);

                foreach (string technique in RenderRoot.Techniques.Keys)
                {
                    RenderSkinnedMeshes(RenderRoot.Techniques[technique].SkinnedMeshes, skinnedMeshEffect, true, true, Matrix.Identity, "SkinnedMeshShadowMap");
                }

                //effect.Technique = "StandardShadowMap";

                meshEffect.SetValue(EHShadowViewProjection, ShadowMapCamera);
                meshEffect.Technique = "ShadowMapInstanced";
                meshEffect.Begin(fx);
                meshEffect.BeginPass(0);

                foreach (string technique in RenderRoot.Techniques.Keys)
                {
                    RenderXMeshesWithInstancingShadows(RenderRoot.Techniques[technique].Meshes, device);
                }

                meshEffect.EndPass();
                meshEffect.End();

                if (Settings.RenderAlphaObjects)
                {
                    //redundant states?
                    StateManager.SetRenderState(RenderState.AlphaTestEnable, true);
                    StateManager.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
                    StateManager.SetRenderState(RenderState.AlphaBlendEnable, true);
                    StateManager.SetRenderState(RenderState.ZEnable, true);
                    StateManager.SetRenderState(RenderState.ZWriteEnable, false);

                    // source = incoming pixel
                    // destination = existing pixel
                    // source.alpha * incoming_pixel + (1 - source.alpha) * exisiting_pixel

                    StateManager.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                    StateManager.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

                    // make private?
                    Common.PriorityQueue<int, Common.Tuple<Model9, Entity, string, string>> queue = new Common.PriorityQueue<int, Common.Tuple<Model9, Entity, string, string>>();

                    foreach (Common.Tuple<Model9, Entity, string, string> e in RenderRoot.AlphaObjects)
                    {
                        if (e.Second.ActiveInShadowMap == Frame)
                        {
                            float distance = (e.Second.Translation - Scene.Camera.Position).Length();
                            queue.Enqueue(-(int)distance, e);
                        }
                    }

                    while (!queue.IsEmpty)
                    {
                        Common.Tuple<Model9, Entity, string, string> modelEntity = queue.Dequeue();

                        MetaModel metaModel = GetLatestMetaModel(modelEntity.Second, modelEntity.Third);

                        if (Settings.PriorityRelation[metaModel.CastShadows] + Settings.ShadowQualityPriorityRelation[Settings.ShadowQuality] > 3)
                        {
                            Matrix world = metaModel.GetWorldMatrix(Scene.Camera, modelEntity.Second);

                            UpdateEffectWithMetaModel(meshEffect, metaModel, true);

                            if (modelEntity.First.SkinnedMesh != null)
                            {
                                UpdateEffectWithMetaModel(skinnedMeshEffect, metaModel, true);
                                foreach (var SM in modelEntity.First.SkinnedMesh.MeshContainers)
                                {
                                    if (SM.Second.SkinInfo != null)
                                    {
                                        skinnedMeshEffect.Begin(fx);
                                        skinnedMeshEffect.BeginPass(0);

                                        RenderSkinnedMeshWithSkinning(SM, skinnedMeshEffect, true, metaModel.StoredFrameMatrices[SM.Second.MeshData.Mesh]);

                                        skinnedMeshEffect.EndPass();
                                        skinnedMeshEffect.End();
                                    }
                                    else if (SM.Second.SkinInfo == null)
                                    {
                                        meshEffect.Technique = "StandardShadowMap";
                                        meshEffect.Begin(fx);
                                        meshEffect.BeginPass(0);

                                        RenderSkinnedMeshWithoutSkinning(SM, meshEffect, true, Matrix.Identity, metaModel.StoredFrameMatrices[SM.Second.MeshData.Mesh]);

                                        meshEffect.EndPass();
                                        meshEffect.End();
                                    }
                                }
                            }
                            else if (modelEntity.First.XMesh != null)
                            {
                                meshEffect.Technique = "StandardShadowMap";
                                meshEffect.SetValue(EHShadowWorldViewProjection, world * ShadowMapCamera);

                                meshEffect.Begin(fx);
                                meshEffect.BeginPass(0);

                                modelEntity.First.XMesh.DrawSubset(0);

                                meshEffect.EndPass();
                                meshEffect.End();
                            }
                        }
                    }
                }
                device.EndScene();
                //if (shadowMapSurface != null)
                //{
                //    shadowMapSurface.Dispose();
                //    shadowMapSurface = null;
                //}
#if PROFILE_RENDERER
                rendererProfileLogFile.WriteLine("Render Shadowmap: " + ((System.DateTime.Now.Second * 1000 + System.DateTime.Now.Millisecond) - (t.Second * 1000 + t.Millisecond)) + " ms");
#endif
            }

            //if (Settings.RenderWithPostEffect)
            //{
            //    renderedImage = renderedImageTexture.GetSurfaceLevel(0);
            //    device.SetRenderTarget(0, renderedImage);
            //    device.DepthStencilSurface = depthBuffForPost;
            //}
            //else
            //{
            device.SetRenderTarget(0, screenRTSurface);
            device.DepthStencilSurface = depthBufferScreenSurface;
            //}
        }
#if PROFILES_RENDERER
        public static event Action RenderSplatMapStart;
        public static event Action RenderSplatMapStop;

        public static event Action RenderAlphaStart;
        public static event Action RenderAlphaStop;

        public static event Action RenderSkinnedMeshesStart;
        public static event Action RenderSkinnedMeshesStop;

        public static event Action RenderXMeshesStart;
        public static event Action RenderXMeshesStop;
#endif
        public void Render(float dtime)
        {
            for (int i = 0; i < 16; i++)
            {
                StateManager.SetSamplerState(i, SamplerState.MinFilter, Settings.TextureFilter.TextureFilterMin);
                StateManager.SetSamplerState(i, SamplerState.MagFilter, Settings.TextureFilter.TextureFilterMag);
                StateManager.SetSamplerState(i, SamplerState.MipFilter, Settings.TextureFilter.TextureFilterMip);
                StateManager.SetSamplerState(i, SamplerState.MaxAnisotropy, Settings.TextureFilter.AnisotropicLevel);
            }

            StateManager.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
            StateManager.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);
            StateManager.SetRenderState(RenderState.CullMode, Settings.CullMode);
            StateManager.SetRenderState(RenderState.FillMode, Settings.FillMode);
            StateManager.SetRenderState(RenderState.MultisampleAntialias, true);


            StateManager.SetRenderState(RenderState.AlphaBlendEnable, false);
            StateManager.SetRenderState(RenderState.AlphaTestEnable, true);
            StateManager.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
            StateManager.SetRenderState(RenderState.ZEnable, true);
            StateManager.SetRenderState(RenderState.ZWriteEnable, true);

            Matrix ViewProjection = Scene.Camera.View * Scene.Camera.Projection;

#if PROFILE_RENDERER
            var t = System.DateTime.Now;
#endif

            meshEffect.SetValue(EHCameraPosition, Scene.Camera.Position);
            skinnedMeshEffect.SetValue(EHCameraPosition, Scene.Camera.Position);

            skinnedMeshEffect.SetValue(EHViewProjectionMatrix, ViewProjection);
            meshEffect.SetValue(EHViewProjectionMatrix, ViewProjection);

            foreach (string technique in RenderRoot.Techniques.Keys)
            {
                if (technique.StartsWith("Sk"))
                {
                    if (Settings.RenderSkinnedMeshes)
                    {
                        if (Settings.ShadowsEnable)
                        {
                            skinnedMeshEffect.SetTexture(EHShadowTexture, shadowMap);
                            skinnedMeshEffect.SetValue(EHShadowViewProjection, ShadowMapCamera);
                        }
#if PROFILES_RENDERER
                        if (RenderSkinnedMeshesStart != null)
                            RenderSkinnedMeshesStart();
#endif
                        RenderSkinnedMeshes(RenderRoot.Techniques[technique].SkinnedMeshes, skinnedMeshEffect, false, true, ViewProjection, technique);
#if PROFILES_RENDERER
                        if (RenderSkinnedMeshesStop != null)
                            RenderSkinnedMeshesStop();
#endif
                    }
                }
                else if (technique.StartsWith("ShadowedSceneI"))
                {
                    if (Settings.RenderXMeshes)
                    {
                        if (Settings.ShadowsEnable)
                        {
                            meshEffect.SetTexture(EHShadowTexture, shadowMap);
                            meshEffect.SetValue(EHShadowViewProjection, ShadowMapCamera);
                        }
#if PROFILES_RENDERER
                        if (RenderXMeshesStart != null)
                            RenderXMeshesStart();
#endif
                        RenderXMeshesWithInstancing(RenderRoot.Techniques[technique].Meshes, device, meshEffect, technique);
#if PROFILES_RENDERER
                        if (RenderXMeshesStop != null)
                            RenderXMeshesStop();
#endif
                    }
                }
                else if (technique.StartsWith("Sh"))
                {
                    if (settings.RenderXMeshes)
                    {
                        if (Settings.ShadowsEnable)
                        {
                            meshEffect.SetTexture(EHShadowTexture, shadowMap);
                            meshEffect.SetValue(EHShadowViewProjection, ShadowMapCamera);
                        }
#if PROFILES_RENDERER
                        if (RenderXMeshesStart != null)
                            RenderXMeshesStart();
#endif
                        RenderXMeshes(RenderRoot.Techniques[technique].Meshes, meshEffect, ViewProjection, technique);
#if PROFILES_RENDERER
                        if (RenderXMeshesStop != null)
                            RenderAlphaStop();
#endif
                    }
                }
            }

#if PROFILE_RENDERER
            rendererProfileLogFile.WriteLine("Render Meshes: " + ((System.DateTime.Now.Second * 1000 + System.DateTime.Now.Millisecond) - (t.Second * 1000 + t.Millisecond)) + " ms");
#endif

            if (Settings.RenderSplatObjects)
            {
#if PROFILE_RENDERER
                t = System.DateTime.Now;
#endif
#if PROFILES_RENDERER
                if (RenderSplatMapStart != null)
                    RenderSplatMapStart();
#endif
                if (Settings.ShadowsEnable)
                    splatEffect.SetTexture(EHShadowTexture, shadowMap);

                splatEffect.SetValue(EHTextureSize, (1.0f / Settings.TextureSize));
                splatEffect.SetValue(EHCameraPosition, Scene.Camera.Position);

                if (!Scene.DesignMode)
                {
                    foreach (string technique in RenderRoot.SplatTechniques.Keys)
                    {
                        splatEffect.Technique = technique;

                        splatEffect.Begin(fx);
                        splatEffect.BeginPass(0);

                        foreach (SplatTextureCombination stc in RenderRoot.SplatTechniques[technique].TextureCombinations.Keys)
                        {
                            bool first = true;

                            foreach (Common.Tuple<Model9, Entity, string> ro in RenderRoot.SplatTechniques[technique].TextureCombinations[stc].RenderObjects)
                            {
                                if (ro.Second.ActiveInMain == Frame)
                                {
                                    MetaModel metaModel = GetLatestMetaModel(ro.Second, ro.Third);

                                    if (first)
                                    {
                                        splatEffect.SetTexture(EHSplatTexture1, stc.SplatTexture[0]);
                                        splatEffect.SetTexture(EHSplatTexture2, stc.SplatTexture[1]);

                                        splatEffect.SetTexture(EHBaseTexture, stc.BaseTexture);

                                        splatEffect.SetTexture(EHTerrainTexture1, stc.MaterialTexture[0]);
                                        splatEffect.SetTexture(EHTerrainTexture2, stc.MaterialTexture[1]);
                                        splatEffect.SetTexture(EHTerrainTexture3, stc.MaterialTexture[2]);
                                        splatEffect.SetTexture(EHTerrainTexture4, stc.MaterialTexture[3]);
                                        splatEffect.SetTexture(EHTerrainTexture5, stc.MaterialTexture[4]);
                                        splatEffect.SetTexture(EHTerrainTexture6, stc.MaterialTexture[5]);
                                        splatEffect.SetTexture(EHTerrainTexture7, stc.MaterialTexture[6]);
                                        splatEffect.SetTexture(EHTerrainTexture8, stc.MaterialTexture[7]);

                                        first = false;

                                        UpdateEffectWithMetaModel(splatEffect, metaModel, false);

                                        splatEffect.SetTexture(EHSpecularMap, ro.First.SpecularTexture);
                                    }

                                    Matrix world = metaModel.GetWorldMatrix(Scene.Camera, ro.Second);

                                    splatEffect.SetValue(EHWorldViewProjectionMatrix, world * ViewProjection);
                                    splatEffect.SetValue(EHWorldMatrix, world);
                                    splatEffect.SetValue(EHShadowWorldViewProjection, world * ShadowMapCamera);

                                    splatEffect.CommitChanges();
                                    
                                    ro.First.XMesh.DrawSubset(0);
                                }
                            }
                        }

                        splatEffect.EndPass();
                        splatEffect.End();
                    }
                }
                else
                {
                    string techniqueName = "";
                    if (RenderRoot.SplatObjects.Count > 0)
                        techniqueName = "Standard" + GetTechniqueNameExtension(
                            GetLatestMetaModel(RenderRoot.SplatObjects[0].Second, RenderRoot.SplatObjects[0].Third),
                            Settings, techniqueNames);

                    foreach (Common.Tuple<Model9, Entity, string> modelEntity in RenderRoot.SplatObjects)
                    {
                        RenderSplatmappedMesh(modelEntity, techniqueName, ViewProjection);
                    }

                    techniqueName = null;
                }

#if PROFILE_RENDERER
                rendererProfileLogFile.WriteLine("Render Ground: " + ((System.DateTime.Now.Second * 1000 + System.DateTime.Now.Millisecond) - (t.Second * 1000 + t.Millisecond)) + " ms");
#endif

#if PROFILES_RENDERER
                if (RenderSplatMapStop != null)
                    RenderSplatMapStop();
#endif
            }
            if (Settings.RenderAlphaObjects)
            {
#if PROFILE_RENDERER
                t = System.DateTime.Now;
#endif
#if PROFILES_RENDERER
                if (RenderAlphaStart != null)
                    RenderAlphaStart();
#endif
                //redundant
                StateManager.SetRenderState(RenderState.AlphaBlendEnable, true);
                StateManager.SetRenderState(RenderState.AlphaTestEnable, true);
                StateManager.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
                StateManager.SetRenderState(RenderState.ZEnable, true);
                StateManager.SetRenderState(RenderState.ZWriteEnable, false);
                StateManager.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
                StateManager.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
                StateManager.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
                StateManager.SetSamplerState(0, SamplerState.MaxAnisotropy, 1);

                // source = incoming pixel
                // destination = existing pixel
                // source.alpha * incoming_pixel + (1 - source.alpha) * exisiting_pixel

                StateManager.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                StateManager.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

                List<Common.Tuple<Model9, Entity, string, string>> overRidingZBuffer = new List<Common.Tuple<Model9, Entity, string, string>>();
                Common.PriorityQueue<int, Common.Tuple<Model9, Entity, string, string>> queue = new Common.PriorityQueue<int, Common.Tuple<Model9, Entity, string, string>>();

                foreach (Common.Tuple<Model9, Entity, string, string> e in RenderRoot.AlphaObjects)
                {
                    if (e.Second.ActiveInMain == Frame)
                    {
                        MetaModel metaModel = GetLatestMetaModel(e.Second, e.Third);
                        if (metaModel.OverrideZBuffer)
                            overRidingZBuffer.Add(e);
                        else
                        {
                            if (metaModel.DontSort)
                                queue.Enqueue(int.MinValue, e);
                            else if (metaModel.IsWater)
                                queue.Enqueue(int.MinValue + 1, e);
                            else
                            {
                                float distance = (e.Second.Translation - Scene.Camera.Position).Length();
                                queue.Enqueue(-(int)distance, e);
                            }
                        }
                    }
                }

                string previousTechnique = "";

                while (!queue.IsEmpty)
                {
                    Common.Tuple<Model9, Entity, string, string> modelEntity = queue.Dequeue();

                    MetaModel metaModel = GetLatestMetaModel(modelEntity.Second, modelEntity.Third);

                    //if (Settings.TerrainQualityPriorityRelation[Settings.TerrainQuality] + Settings.PriorityRelation[metaModel.Visible] < 3)
                    //    continue;

                    Matrix world = metaModel.GetWorldMatrix(Scene.Camera, modelEntity.Second);

                    if (metaModel.IsWater)
                    {
                        UpdateEffectWithMetaModel(waterEffect, metaModel, false);

                        string str = "NoSpecular";

                        if (settings.LightingQualityPriorityRelation[settings.LightingQuality] + settings.PriorityRelation[metaModel.ReceivesSpecular] > 3)
                            str = "Specular";

                        if (GraphicsDevice.SettingConverters.PixelShaderVersion.Major == 3)
                            str = "3" + str;
                        else
                            str = "2" + str;

                        waterEffect.Technique = "Water" + str;
                        waterOffest.X += dtime;
                        waterOffest.Y += dtime;

                        waterEffect.SetValue(EHWaterOffset, waterOffest);
                        waterEffect.SetValue(EHCamPos, scene.Camera.Position);
                        waterEffect.SetValue(EHSkyHeight, -50.0f);
                        waterEffect.SetValue(EHWorldViewProjectionMatrix, world * ViewProjection);
                        waterEffect.SetValue(EHWorldMatrix, world);
                        waterEffect.SetTexture(EHTexture, modelEntity.First.Texture);
                        waterEffect.SetValue(EHShadowWorldViewProjection, world * ShadowMapCamera);

                        if (modelEntity.First.SpecularTexture != null)
                            waterEffect.SetTexture(EHSpecularMap, modelEntity.First.SpecularTexture);

                        waterEffect.Begin(fx);
                        waterEffect.BeginPass(0);

                        modelEntity.First.XMesh.DrawSubset(0);

                        waterEffect.EndPass();
                        waterEffect.End();
                    }
                    else if (modelEntity.First.SkinnedMesh != null)
                    {
                        foreach (var SM in modelEntity.First.SkinnedMesh.MeshContainers)
                        {
                            if (SM.Second.SkinInfo != null)
                            {
                                skinnedMeshEffect.Technique = "SkinnedMesh" + GetTechniqueNameExtension(metaModel, Settings, techniqueNames);

                                UpdateEffectWithMetaModel(skinnedMeshEffect, metaModel, false);

                                skinnedMeshEffect.SetTexture(EHTexture, modelEntity.First.Texture);

                                if (modelEntity.First.SpecularTexture != null)
                                    skinnedMeshEffect.SetTexture(EHSpecularMap, modelEntity.First.SpecularTexture);

                                skinnedMeshEffect.Begin(fx);
                                skinnedMeshEffect.BeginPass(0);

                                RenderSkinnedMeshWithSkinning(SM, skinnedMeshEffect, false, metaModel.StoredFrameMatrices[SM.Second.MeshData.Mesh]);

                                skinnedMeshEffect.EndPass();
                                skinnedMeshEffect.End();
                            }
                            else if (SM.Second.SkinInfo == null)
                            {
                                if (modelEntity.Forth != previousTechnique)
                                {
                                    meshEffect.Technique = "ShadowedScene" + modelEntity.Forth;
                                    previousTechnique = modelEntity.Forth;
                                }
                                //meshEffect.Technique = "ShadowedScene" + GetTechniqueNameExtension(metaModel, Settings, techniqueNames);

                                UpdateEffectWithMetaModel(meshEffect, metaModel, false);

                                meshEffect.SetTexture(EHTexture, modelEntity.First.Texture);

                                if (modelEntity.First.SpecularTexture != null)
                                    meshEffect.SetTexture(EHSpecularMap, modelEntity.First.SpecularTexture);

                                meshEffect.Begin(fx);
                                meshEffect.BeginPass(0);

                                RenderSkinnedMeshWithoutSkinning(SM, meshEffect, false, ViewProjection, metaModel.StoredFrameMatrices[SM.Second.MeshData.Mesh]);

                                meshEffect.EndPass();
                                meshEffect.End();
                            }
                        }
                    }
                    else if (modelEntity.First.XMesh != null)
                    {
                        if (modelEntity.Forth != previousTechnique)
                        {
                            meshEffect.Technique = "ShadowedScene" + modelEntity.Forth;
                            previousTechnique = modelEntity.Forth;
                        }

                        meshEffect.SetValue(EHShadowWorldViewProjection, world * ShadowMapCamera);
                        meshEffect.SetValue(EHWorldViewProjectionMatrix, world * ViewProjection);
                        meshEffect.SetValue(EHWorldMatrix, world);

                        UpdateEffectWithMetaModel(meshEffect, metaModel, false);

                        meshEffect.SetTexture(EHTexture, modelEntity.First.Texture);

                        if (modelEntity.First.SpecularTexture != null)
                            meshEffect.SetTexture(EHSpecularMap, modelEntity.First.SpecularTexture);

                        meshEffect.Begin(fx);
                        meshEffect.BeginPass(0);

                        modelEntity.First.XMesh.DrawSubset(0);

                        meshEffect.EndPass();
                        meshEffect.End();
                    }
                }

                bool first = true;

                foreach (Common.Tuple<Model9, Entity, string, string> modelEntity in overRidingZBuffer)
                {
                    MetaModel metaModel = null;
                    Matrix world = Matrix.Identity;
                    if (first)
                    {
                        StateManager.SetRenderState(RenderState.ZEnable, false);
                        first = false;
                    }

                    metaModel = GetLatestMetaModel(modelEntity.Second, modelEntity.Third);
                    //if (Settings.TerrainQualityPriorityRelation[Settings.TerrainQuality] + Settings.PriorityRelation[metaModel.Visible] < 3)
                    //    continue;
                    world = metaModel.GetWorldMatrix(Scene.Camera, modelEntity.Second);

                    meshEffect.Technique = "ShadowedScene" + GetTechniqueNameExtension(metaModel, Settings, techniqueNames);

                    meshEffect.SetValue(EHShadowWorldViewProjection, world * ShadowMapCamera);
                    meshEffect.SetValue(EHWorldViewProjectionMatrix, world * ViewProjection);
                    //meshEffect.SetValue(EHNormalMatrix, Matrix.Transpose(Matrix.Invert(world)));
                    meshEffect.SetValue(EHWorldMatrix, world);

                    UpdateEffectWithMetaModel(meshEffect, metaModel, false);

                    meshEffect.SetTexture(EHTexture, modelEntity.First.Texture);

                    if (modelEntity.First.SpecularTexture != null)
                        meshEffect.SetTexture(EHSpecularMap, modelEntity.First.SpecularTexture);

                    meshEffect.Begin(fx);
                    meshEffect.BeginPass(0);

                    modelEntity.First.XMesh.DrawSubset(0);

                    meshEffect.EndPass();
                    meshEffect.End();
                }
#if PROFILE_RENDERER
                rendererProfileLogFile.WriteLine("Render Alpha Objects: " + ((System.DateTime.Now.Second * 1000 + System.DateTime.Now.Millisecond) - (t.Second * 1000 + t.Millisecond)) + " ms");
#endif
#if PROFILES_RENDERER
                if (RenderAlphaStop != null)
                    RenderAlphaStop();
#endif
            }

            //maybe reset all effect and device variables?

            //skinnedMeshEffect.SetTexture("ShadowMap", null);
            //effect.SetTexture("ShadowMap", null);
            //for (int i = 0; i < 7; i++)
            //    device.SetTexture(i, null);

            //Texture.ToFile(renderedImageTexture, "test.png", ImageFileFormat.Png);

            //var s = shadowMapBack;
            //shadowMapBack = shadowMap;
            //shadowMap = s;
            //lastShadowMapCamera = shadowMapCamera;

            //if (Settings.RenderWithPostEffect)
            //{
            //    // This offsets the post effect quad half a pixel, otherwise it will turn out blurry
            //    ((Software.Meshes.IndexedPlane)screenPlane.MeshDescription).Position =
            //        new Vector3(-1 - 1 / ((float)Scene.Viewport.Width), -1 - 1 / ((float)Scene.Viewport.Height), 0);

            //    var sp = Scene.View.Content.Peek<SlimDX.Direct3D9.Mesh>(screenPlane);

            //    device.EndScene();

            //    device.SetRenderTarget(0, screenRTSurface);
            //    device.DepthStencilSurface = null;

            //    device.Clear(ClearFlags.Target, Color.Black, 1.0f, 0);

            //    device.BeginScene();

            //    //device.DepthStencilSurface = null;
            //    //StateManager.SetRenderState(RenderState.ZEnable, false);
            //    StateManager.SetRenderState(RenderState.AlphaBlendEnable, false);
            //    StateManager.SetRenderState(RenderState.AlphaTestEnable, false);

            //    postEffect.Technique = "Standard";
            //    //Texture.ToFile(renderedImageTexture, "testRenderImage.png", ImageFileFormat.Png);
            //    postEffect.SetTexture(EHTexture, renderedImageTexture);
            //    postEffect.SetValue(EHAdditiveLightFactor, Settings.AdditiveLightColor);
            //    postEffect.SetValue(EHPercentageLightIncrease, Settings.PercentageLightIncrease);
            //    postEffect.SetValue(EHColorChannelPercentageIncrease, Settings.ColorChannelPercentageIncrease);

            //    postEffect.Begin(fx);
            //    postEffect.BeginPass(0);

            //    sp.DrawSubset(0);

            //    postEffect.EndPass();
            //    postEffect.End();

            //    device.DepthStencilSurface = depthBufferScreenSurface;
            //}

#if RENDERER_STATISTICS
            rendererSatisticsLogFile.WriteLine("Min Instanced Draw Calls: " + minNumberOfInstancedDrawCalls);
            rendererSatisticsLogFile.WriteLine("Max Instanced Draw Calls: " + maxNumberOfInstancedDrawCalls);
#endif

#if LOG_RENDERTREE
            totalItemsTheoretically += totalAddedItems;
            totalItemsTheoretically -= totalRemovedItems;
            renderTreeLogFile.WriteLine("Total Items Added: " + totalAddedItems);
            renderTreeLogFile.WriteLine("Total Items Removed: " + totalRemovedItems);
            renderTreeLogFile.WriteLine("Total Items In Tree: " + GetNumberOfItemsInTree());
            renderTreeLogFile.WriteLine("Total Items In Tree Theoretical: " + totalItemsTheoretically);
            totalAddedItems = 0;
            totalRemovedItems = 0;
#endif

        }

        #endregion

        #region Private Variables

        Device device;

        int nMeshes = 64;
        int vertexBufferSizeForInstancing = 512;
        int shadowMapWidth, shadowMapHeight;
        bool shadowsEnable;

        string[, , , , ,] techniqueNames = new string[2, 2, 2, 2, 2, 2];


        VertexDeclaration vd;//, vdS;
        VertexElement[] instancedVertexElements;//, instancedShadowMapVertexElements;
        VertexBuffer[] optimizedVB;//, optimizedShadowMapVB;

        Content.MeshConcretize screenPlane = new Content.MeshConcretize
        {
            MeshDescription = new Software.Meshes.IndexedPlane
            {
                Position = new Vector3(-1, -1, 0),
                Size = new Vector2(2, 2),
                UVMin = new Vector2(0, 1),
                UVMax = new Vector2(1, 0),
                Facings = Graphics.Facings.Backside
            },
            Layout = Graphics.Software.Vertex.PositionNormalTexcoord.Instance
        };

        Vector2 waterOffest = Vector2.Zero;

        RenderRoot RenderRoot;

        //EffectHandle EHNormalMatrices = new EffectHandle("NormalMatrices");
        EffectHandle EHWorldMatrices = new EffectHandle("WorldMatrices");
        EffectHandle EHCurrentBoneCount = new EffectHandle("CurrentBoneCount");
        EffectHandle EHViewProjectionMatrix = new EffectHandle("ViewProjectionMatrix");
        EffectHandle EHWorldViewProjectionMatrix = new EffectHandle("WorldViewProjectionMatrix");
        //EffectHandle EHNormalMatrix = new EffectHandle("NormalMatrix");
        EffectHandle EHShadowViewProjection = new EffectHandle("ShadowViewProjection");
        EffectHandle EHShadowWorldViewProjection = new EffectHandle("ShadowWorldViewProjection");
        EffectHandle EHTexture = new EffectHandle("Texture");
        EffectHandle EHNormalTexture = new EffectHandle("BumpTexture");
        EffectHandle EHFogColor = new EffectHandle("FogColor");
        EffectHandle EHAmbientColor = new EffectHandle("AmbientColor");
        EffectHandle EHDiffuseColor = new EffectHandle("DiffuseColor");
        EffectHandle EHSpecularColor = new EffectHandle("SpecularColor");
        EffectHandle EHFogDistance = new EffectHandle("FogDistance");
        EffectHandle EHLightDirection = new EffectHandle("LightDirection");
        EffectHandle EHShadowQuality = new EffectHandle("ShadowQuality");
        EffectHandle EHShadowsEnable = new EffectHandle("ShadowsEnable");
        EffectHandle EHSpecularExponent = new EffectHandle("SpecularExponent");
        EffectHandle EHShadowTexture = new EffectHandle("ShadowMap");

        EffectHandle EHSplatTexture1 = new EffectHandle("SplatTexture1");
        EffectHandle EHSplatTexture2 = new EffectHandle("SplatTexture2");

        EffectHandle EHTerrainTexture1 = new EffectHandle("TerrainTexture1");
        EffectHandle EHTerrainTexture2 = new EffectHandle("TerrainTexture2");
        EffectHandle EHTerrainTexture3 = new EffectHandle("TerrainTexture3");
        EffectHandle EHTerrainTexture4 = new EffectHandle("TerrainTexture4");
        EffectHandle EHTerrainTexture5 = new EffectHandle("TerrainTexture5");
        EffectHandle EHTerrainTexture6 = new EffectHandle("TerrainTexture6");
        EffectHandle EHTerrainTexture7 = new EffectHandle("TerrainTexture7");
        EffectHandle EHTerrainTexture8 = new EffectHandle("TerrainTexture8");

        EffectHandle EHBaseTexture = new EffectHandle("BaseTexture");

        EffectHandle EHSpecularMap = new EffectHandle("SpecularMap");

        EffectHandle EHShadowBias = new EffectHandle("ShadowBias");
        EffectHandle EHWaterOffset = new EffectHandle("WaterOffset");
        EffectHandle EHCamPos = new EffectHandle("CamPos");
        EffectHandle EHSkyHeight = new EffectHandle("SkyHeight");
        EffectHandle EHWorldMatrix = new EffectHandle("WorldMatrix");
        EffectHandle EHWaterLevel = new EffectHandle("WaterLevel");
        EffectHandle EHReceivesDiffuseLight = new EffectHandle("ReceivesDiffuseLight");
        EffectHandle EHReceivesAmbientLight = new EffectHandle("ReceivesAmbientLight");
        EffectHandle EHReceivesShadows = new EffectHandle("ReceivesShadows");
        EffectHandle EHWaterEnable = new EffectHandle("WaterEnable");
        EffectHandle EHOpacity = new EffectHandle("Opacity");
        EffectHandle EHAdditiveLightFactor = new EffectHandle("AdditiveLightFactor");
        EffectHandle EHPercentageLightIncrease = new EffectHandle("PercentageLightIncrease");
        EffectHandle EHColorChannelPercentageIncrease = new EffectHandle("ColorChannelPercentageIncrease");
        EffectHandle EHTextureSize = new EffectHandle("TextureSize");
        EffectHandle EHFogExponent = new EffectHandle("FogExponent");
        EffectHandle EHCameraPosition = new EffectHandle("CameraPosition");

        //Vector3 shadowMapCameraFactor = new Vector3(-17.3f, -17.3f, -34.6f);
        Vector3 shadowMapCameraFactor = new Vector3(-1, -1, -2);

        Texture shadowMap;//, renderedImageTexture;

        Effect meshEffect, skinnedMeshEffect, splatEffect, waterEffect; //, postEffect;

        Surface screenRTSurface, depthBufferScreenSurface, depthBufferShadowMapSurface, shadowMapSurface, renderedImage, depthBuffForPost;

        FX fx = FX.DoNotSaveState;

#if LOG_RENDERTREE
        StreamWriter renderTreeLogFile;
        public static int totalRemovedItems = 0;
        public static int totalAddedItems = 0;
        private int totalItemsTheoretically = 0;
#endif
#if RENDERER_STATISTICS
        StreamWriter rendererSatisticsLogFile;
        int maxNumberOfInstancedDrawCalls = Int32.MinValue;
        int minNumberOfInstancedDrawCalls = Int32.MaxValue;
#endif
#if PROFILE_RENDERER
        StreamWriter rendererProfileLogFile;
#endif

        #endregion
    }
}

//LINE (Round 1) 1641 -> 1595 -> 1591 -> 1588 -> 1498 -> 1395 -> 1342 -> 1312 || (Round 2) 1922 -> 1814 -> 1993