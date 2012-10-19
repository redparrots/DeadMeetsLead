using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.IO;
using SlimDX.Direct3D9;

namespace Graphics.Content
{
    public enum Priority
    {
        Never,
        Low,
        Medium,
        High
    }

    public enum ModelOrientationRelation
    {
        Absolute,
        Relative
    }

    /// <summary>
    /// Use this to return structs from the Content
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StructBoxer<T> where T : struct { public T Value; }
    [Serializable]
    public class MetaModel : MetaResource<Model9, Model10>
    {
        public class Mapper9<MetaT, T> : MetaMapper<MetaT, T>
            where MetaT : MetaModel
            where T : Model9, new()
        {
            public override T Construct(MetaT metaResource, ContentPool content)
            {
                T t;
                t = new T
                {
                    XMesh = content.Acquire<SlimDX.Direct3D9.Mesh>(metaResource.XMesh),
                    Texture = content.Acquire<SlimDX.Direct3D9.Texture>(metaResource.Texture),
                    SkinnedMesh = content.Acquire<SkinnedMesh>(metaResource.SkinnedMesh),
                };
                if (metaResource.MaterialTexture != null)
                    t.MaterialTexture = new SlimDX.Direct3D9.Texture[metaResource.MaterialTexture.Length];
                if (metaResource.MaterialTexture != null)
                    for (int i = 0; i < metaResource.MaterialTexture.Length; i++)
                    {
                        t.MaterialTexture[i] = content.Acquire<SlimDX.Direct3D9.Texture>(metaResource.MaterialTexture[i]);
                    }

                if (metaResource.SplatTexutre != null)
                    t.SplatTexture = new SlimDX.Direct3D9.Texture[metaResource.SplatTexutre.Length];

                if (metaResource.SplatTexutre != null)
                    for (int i = 0; i < metaResource.SplatTexutre.Length; i++)
                    {
                        t.SplatTexture[i] = content.Acquire<SlimDX.Direct3D9.Texture>(metaResource.SplatTexutre[i]);
                    }

                if (metaResource.BaseTexture != null)
                    t.BaseTexture = content.Acquire<SlimDX.Direct3D9.Texture>(metaResource.BaseTexture);

                if (metaResource.SpecularTexture != null)
                    t.SpecularTexture = content.Acquire<SlimDX.Direct3D9.Texture>(metaResource.SpecularTexture);

                return t;
            }
            public override void Release(MetaT metaResource, ContentPool content, T resource)
            {
                content.Release(resource.XMesh);
                content.Release(resource.Mesh);
                content.Release(resource.Texture);
                content.Release(resource.SkinnedMesh);
                content.Release(resource.SpecularTexture);
                if (metaResource.SplatTexutre != null)
                    for (int i = 0; i < resource.SplatTexture.Length; i++)
                        content.Release(resource.SplatTexture[i]);

                if (metaResource.MaterialTexture != null)
                    for (int i = 0; i < resource.MaterialTexture.Length; i++)
                        content.Release(resource.MaterialTexture[i]);

                if (metaResource.BaseTexture != null)
                    content.Release(resource.BaseTexture);
            }
        }
        public class Mapper9 : Mapper9<MetaModel, Model9> { }

        public class Mapper10<MetaT, T> : MetaMapper<MetaT, T>
            where MetaT : MetaModel
            where T : Model10, new()
        {
            public override T Construct(MetaT metaResource, ContentPool content)
            {
                throw new NotImplementedException("splatting not added");
                return new T
                {
                    XMesh = content.Acquire<SlimDX.Direct3D10.Mesh>(metaResource.XMesh),
                    TextureShaderView =
                        content.Acquire<SlimDX.Direct3D10.ShaderResourceView>(new TextureShaderView
                        {
                            Texture = content.Acquire<SlimDX.Direct3D10.Texture2D>(metaResource.Texture)
                        }),
                    World = metaResource.World,
                    Visible = metaResource.Visible,
                    IsBillboard = metaResource.IsBillboard,
                    SkinnedMesh = content.Acquire<SkinnedMesh>(metaResource.SkinnedMesh),
                    HasAlpha = metaResource.HasAlpha,
                    AlphaRef = metaResource.AlphaRef,
                };
            }
            public override void Release(MetaT metaResource, ContentPool content, T resource)
            {
                throw new NotImplementedException("splatting not added");
                content.Release(resource.XMesh);
                content.Release(resource.Mesh);
                if (resource.TextureShaderView != null && !resource.TextureShaderView.Disposed)
                {
                    content.Release(resource.TextureShaderView.Resource);
                    content.Release(resource.TextureShaderView);
                }
                content.Release(resource.SkinnedMesh);
            }
        }
        public class Mapper10 : Mapper10<MetaModel, Model10> { }

        public class MapperBounding : MetaMapper<MetaModel, StructBoxer<BoundingBox>>
        {
            public override StructBoxer<BoundingBox> Construct(MetaModel metaResource, ContentPool content)
            {
                BoundingBox b = new BoundingBox();
                if (content.Device9 != null)
                {
                    var m = content.Peek<Model9>(metaResource);
                    if (m != null)
                    {
                        if (m.SkinnedMesh != null)
                            b = Common.Boundings.Transform(m.SkinnedMesh.Bounding, metaResource.World);
                        else if (m.XMesh != null)
                            b = Common.Boundings.Transform(Common.Boundings.BoundingBoxFromXMesh(m.XMesh), metaResource.World);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                return new StructBoxer<BoundingBox> { Value = b };
            }
            public override void Release(MetaModel metaResource, ContentPool content, StructBoxer<BoundingBox> resource)
            {
            }
        }

        public MetaModel()
        {
            World = Matrix.Identity;
            Visible = Priority.High;
            CastShadows = Priority.Never;
            ReceivesShadows = Priority.Never;
            ReceivesDiffuseLight = Priority.High;
            ReceivesAmbientLight = Priority.High;
            ReceivesSpecular = Priority.Never;
            Opacity = 1.0f;
            ReceivesFog = true;
            HasAlpha = false;
            TextureAddress = TextureAddress.Wrap;
            Animate = Priority.High;
            SpecularExponent = 10;
            OverrideZBuffer = false;
            OrientationRelation = ModelOrientationRelation.Relative;

            InstanceID = "" + idGenerator++;
        }
        public MetaModel(MetaModel copy)
            : this()
        {
            if (copy.XMesh != null)
                XMesh = (MetaResourceBase)copy.XMesh.Clone();
            if (copy.Mesh != null)
                Mesh = (MetaResource<Mesh9, Mesh10>)copy.Mesh.Clone();
            if (copy.Texture != null)
                Texture = (MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>)
                    copy.Texture.Clone();
            if (copy.SpecularTexture != null)
                SpecularTexture = (MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>)
                    copy.SpecularTexture.Clone();
            if (copy.SplatTexutre != null)
            {
                SplatTexutre = new MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>[copy.SplatTexutre.Length];

                for (int i = 0; i < copy.SplatTexutre.Length; i++)
                {
                    SplatTexutre[i] = (MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>)
                        copy.SplatTexutre[i].Clone();
                }
            }

            if (copy.MaterialTexture != null)
            {
                MaterialTexture = new MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>[copy.MaterialTexture.Length];

                for (int i = 0; i < copy.MaterialTexture.Length; i++)
                {
                    if(copy.MaterialTexture[i] != null)
                        MaterialTexture[i] = (MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>)
                            copy.MaterialTexture[i].Clone();
                }
            }

            if (copy.BaseTexture != null)
                BaseTexture = (MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>)copy.BaseTexture.Clone();

            if (copy.SkinnedMesh != null)
                SkinnedMesh = (MetaResource<SkinnedMesh>)copy.SkinnedMesh.Clone();

            AlphaRef = copy.AlphaRef;
            CastShadows = copy.CastShadows;
            HasAlpha = copy.HasAlpha;
            IsBillboard = copy.IsBillboard;
            IsWater = copy.IsWater;
            Opacity = copy.Opacity;
            ReceivesAmbientLight = copy.ReceivesAmbientLight;
            ReceivesDiffuseLight = copy.ReceivesDiffuseLight;
            ReceivesSpecular = copy.ReceivesSpecular;
            ReceivesShadows = copy.ReceivesShadows;
            Animate = copy.Animate;
            SplatMapped = copy.SplatMapped;
            Visible = copy.Visible;
            World = copy.World;
            DontSort = copy.DontSort;
            AxialDirection = copy.AxialDirection;
            IsAxialBillboard = copy.IsAxialBillboard;
            ReceivesFog = copy.ReceivesFog;
            TextureAddress = copy.TextureAddress;
            AmbientLight = copy.AmbientLight;
            SpecularExponent = copy.SpecularExponent;
            OverrideZBuffer = copy.OverrideZBuffer;
            StoredFrameMatrices = copy.StoredFrameMatrices;
            OrientationRelation = copy.OrientationRelation;
        }
        public override object Clone()
        {
            return new MetaModel(this);
        }

        public override bool Equals(object obj)
        {
            var o = obj as MetaModel;
            if (o == null) return false;

            return
                Object.Equals(XMesh, o.XMesh) &&
                Object.Equals(Mesh, o.Mesh) &&
                Object.Equals(Texture, o.Texture) &&
                Object.Equals(SkinnedMesh, o.SkinnedMesh) &&
                Common.Utils.SequenceEquals(SplatTexutre, o.SplatTexutre) &&
                Common.Utils.SequenceEquals(MaterialTexture, o.MaterialTexture) &&
                Object.Equals(BaseTexture, o.BaseTexture) &&
                Object.Equals(SpecularTexture, o.SpecularTexture);
        }
        public bool RendererEquals(object obj)
        {
            var o = obj as MetaModel;
            if (o == null) return false;

            return Equals(obj) && HasAlpha == o.HasAlpha;
        }
        public override int GetHashCode()
        {
            int result = 0;
            if (SplatTexutre != null)
                for (int i = 0; i < SplatTexutre.Length; i++)
                {
                    result ^= SplatTexutre[i].GetHashCode() + 1;
                }
            if (MaterialTexture != null)
                for (int i = 0; i < MaterialTexture.Length; i++)
                {
                    if(MaterialTexture[i] != null)
                        result ^= MaterialTexture[i].GetHashCode() + 1;
                }

            return GetType().GetHashCode() ^
                (XMesh != null ? XMesh.GetHashCode() : 1) ^
                (Mesh != null ? Mesh.GetHashCode() : 1) ^
                (Texture != null ? Texture.GetHashCode() : 1) ^
                (SkinnedMesh != null ? SkinnedMesh.GetHashCode() : 1) ^
                (BaseTexture != null ? BaseTexture.GetHashCode() : 1) ^
                (SpecularTexture != null ? SpecularTexture.GetHashCode() : 1) ^
                result;
        }

        public override string ToString()
        {
            return GetType().Name + "." +
                (XMesh != null ? XMesh.ToString() : "null") +
                (Mesh != null ? Mesh.ToString() : "null") +
                (Texture != null ? Texture.ToString() : "null") +
                World + IsBillboard +
                (SkinnedMesh != null ? SkinnedMesh.ToString() : "null");
        }

        public Matrix GetWorldMatrix(Camera camera, Entity entity)
        {
            if (IsBillboard)
            {
                return Graphics.Renderer.Renderer.TurnToCamera(camera, World, entity, OrientationRelation);
            }
            else if (IsAxialBillboard)
            {
                return Graphics.Renderer.Renderer.TurnToCameraAroundAxis(camera, World, entity, AxialDirection);
            }
            else
            {
                if (OrientationRelation == ModelOrientationRelation.Relative)
                    return World * entity.CombinedWorldMatrix;
                else
                    return World;
            }
        }

        public void SignalMetaModelChanged()
        {
            if (MetaModelChanged != null)
                MetaModelChanged(this, null);
        }
        public event EventHandler MetaModelChanged;
        
        private MetaResourceBase xMesh;
        public MetaResourceBase XMesh
        {
            get
            {
                return xMesh;
            }
            set
            {
                xMesh = value;
                SignalMetaModelChanged();
            }
        }

        private MetaResource<Mesh9, Mesh10> mesh;
        public MetaResource<Mesh9, Mesh10> Mesh
        {
            get
            {
                return mesh;
            }
            set
            {
                mesh = value;
                SignalMetaModelChanged();
            }
        }

        private MetaResource<Texture, SlimDX.Direct3D10.Texture2D> texture;
        public MetaResource<Texture, SlimDX.Direct3D10.Texture2D> Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                SignalMetaModelChanged();
            }
        }

        private MetaResource<Texture, SlimDX.Direct3D10.Texture2D> specularTexture;
        public MetaResource<Texture, SlimDX.Direct3D10.Texture2D> SpecularTexture
        {
            get { return specularTexture; }
            set
            {
                specularTexture = value;
                SignalMetaModelChanged();
            }
        }

        private MetaResource<Texture, SlimDX.Direct3D10.Texture2D>[] splatTexutre;
        public MetaResource<Texture, SlimDX.Direct3D10.Texture2D>[] SplatTexutre
        {
            get { return splatTexutre; }
            set
            {
                splatTexutre = value;
                SignalMetaModelChanged();
            }
        }

        private MetaResource<Texture, SlimDX.Direct3D10.Texture2D>[] materialTexture;
        public MetaResource<Texture, SlimDX.Direct3D10.Texture2D>[] MaterialTexture
        {
            get { return materialTexture; }
            set
            { 
                materialTexture = value;
                SignalMetaModelChanged();
            }
        }

        private MetaResource<Texture, SlimDX.Direct3D10.Texture2D> baseTexture;
        public MetaResource<Texture, SlimDX.Direct3D10.Texture2D> BaseTexture
        {
            get { return baseTexture; }
            set
            {
                baseTexture = value;
                SignalMetaModelChanged();
            }
        }

        private MetaResource<SkinnedMesh> skinnedMesh;
        public MetaResource<SkinnedMesh> SkinnedMesh
        {
            get { return skinnedMesh; }
            set
            {
                skinnedMesh = value;
                SignalMetaModelChanged();
            }
        }

        [Obsolete]
        public TextureAddress TextureAddress { get; set; }

        public bool alphaFirst = true;
        private bool hasAlpha;
        public bool HasAlpha
        {
            get { return hasAlpha; }
            set
            {
                hasAlpha = value;
                SignalMetaModelChanged();
            }
        }

        public Matrix World { get; set; }
        public float Opacity { get; set; }
        public bool IsBillboard { get; set; }
        public bool SplatMapped { get; set; }
        public bool IsWater { get; set; }
        bool receivesFog;
        public bool ReceivesFog
        {
            get { return receivesFog; }
            set
            {
                receivesFog = value;
                if (MetaModelChanged != null) MetaModelChanged(this, null);
            }
        }
        public bool OverrideZBuffer { get; set; }

        private Priority animate;
        public Priority Animate { get { return animate; } set { animate = value; } }
        private Priority castShadows;
        public Priority CastShadows
        {
            get { return castShadows; }
            set
            {
                castShadows = value;
                if (MetaModelChanged != null) MetaModelChanged(this, null);
            }
        }
        private Priority receivesDiffuseLight;
        public Priority ReceivesDiffuseLight
        {
            get { return receivesDiffuseLight; }
            set
            {
                receivesDiffuseLight = value;
                if (MetaModelChanged != null) MetaModelChanged(this, null);
            }
        }
        private Priority receivesAmbientLight;
        public Priority ReceivesAmbientLight
        {
            get { return receivesAmbientLight; }
            set
            {
                receivesAmbientLight = value;
                if (MetaModelChanged != null) MetaModelChanged(this, null);
            }
        }
        private Priority receivesSpecular;
        public Priority ReceivesSpecular
        {
            get { return receivesSpecular; }
            set
            {
                receivesSpecular = value;
                if (MetaModelChanged != null) MetaModelChanged(this, null);
            }
        }
        private Priority receivesShadows;
        public Priority ReceivesShadows
        {
            get { return receivesShadows; }
            set
            {
                receivesShadows = value;
                if (MetaModelChanged != null) MetaModelChanged(this, null);
            }
        }
        private Priority visible;
        public Priority Visible { get { return visible; } set { visible = value; } }

        public Color4? AmbientLight = null;
        public Dictionary<SlimDX.Direct3D9.Mesh, Matrix[][]> StoredFrameMatrices;

        /// <summary>
        /// High values => Removes more
        /// Low values => Removes less
        /// </summary>
        public int AlphaRef { get; set; }
        public int SpecularExponent { get; set; }
        public bool DontSort { get; set; }
        public bool IsAxialBillboard { get; set; }
        public Vector3 AxialDirection { get; set; }
        public ModelOrientationRelation OrientationRelation { get; set; }

        public string InstanceID { get; set; }
        static int idGenerator = 0;
    }

    public class UnmanagedResource<T> : MetaResource<T>
    {
        public UnmanagedResource() { }
        public UnmanagedResource(UnmanagedResource<T> cpy) 
        {
            Resource = (DataLink<T>)cpy.Resource.Clone();
        }
        public DataLink<T> Resource { get; set; }
        public class Mapper : MetaMapper<UnmanagedResource<T>, T>
        {
            public override T Construct(UnmanagedResource<T> metaResource, ContentPool content)
            {
                return metaResource.Resource.Data;
            }
            public override void Release(UnmanagedResource<T> metaResource, ContentPool content, T resource)
            {
            }
        }
        public override object Clone()
        {
            return new UnmanagedResource<T>(this);
        }
        /*public override bool Equals(object obj)
        {
            var o = obj as UnmanagedResource<T>;
            if (o == null) return false;
            return Object.Equals(Resource, o.Resource);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + Resource.GetHashCode();
        }*/
    }

    public class UnmanagedResource<T9, T10> : MetaResource<T9, T10>
    {
        public DataLink<T9> Resource9 { get; set; }
        public DataLink<T10> Resource10 { get; set; }
        public class Mapper9 : MetaMapper<UnmanagedResource<T9, T10>, T9>
        {
            public override T9 Construct(UnmanagedResource<T9, T10> metaResource, ContentPool content)
            {
                return metaResource.Resource9.Data;
            }
            public override void Release(UnmanagedResource<T9, T10> metaResource, ContentPool content, T9 resource)
            {
            }
        }
        /*public override bool Equals(object obj)
        {
            var o = obj as UnmanagedResource<T9, T10>;
            if (o == null) return false;
            return Object.Equals(Resource9, o.Resource9);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + Resource9.GetHashCode();
        }*/
    }
    public class UnmanagedTextureMapper9 : UnmanagedResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>.Mapper9 { }


    [Serializable]
    public class FileResource<T> : MetaResource<T>
    {
        public string FileName { get; set; }
        protected string AbsolutePath(ContentPool content)
        {
            return content.ContentPath != null ? content.ContentPath + "/" + FileName : FileName;
        }

        public override object Clone()
        {
            FileResource<T> c = (FileResource<T>)Activator.CreateInstance(GetType());
            c.FileName = FileName;
            return c;
        }
        public override bool Equals(object obj)
        {
            var o = obj as FileResource<T>;
            if (o == null) return false;
            return Object.Equals(FileName, o.FileName);
        }
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ FileName.GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + FileName;
        }
    }

    [Serializable]
    public class FileResource<D3D9T, D3D10T> : MetaResource<D3D9T, D3D10T>
    {
        public FileResource() { }
        public FileResource(FileResource<D3D9T, D3D10T> copy) { FileName = copy.FileName; }
        public string FileName { get; set; }
        protected string AbsolutePath(ContentPool content)
        {
            return AbsolutePath(content, FileName);
        }
        protected static string AbsolutePath(ContentPool content, String filename)
        {
            return content.ContentPath != null ? content.ContentPath + "/" + filename : filename;
        }

        protected static Stream GetStream(ContentPool content, String filename, string resourceName)
        {
            string path = AbsolutePath(content, filename);
            if(Common.FileSystem.Instance.FileExists(path))
                return Common.FileSystem.Instance.OpenRead(path);
            else
            {
                var mrs = typeof(EffectFromFile).Assembly.GetManifestResourceStream(resourceName);
                if (mrs == null)
                    mrs = System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName);
                if (mrs == null)
                    mrs = Shaders.Shaders.GetShader(resourceName);

                if(mrs == null)
                    Application.Log("Cannot find resource: filename=" + filename + ", resourceName=" + resourceName);

                return mrs;
            }
        }


        public override object Clone()
        {
            FileResource<D3D9T, D3D10T> c = (FileResource<D3D9T, D3D10T>)Activator.CreateInstance(GetType());
            c.FileName = FileName;
            return c;
        }
        public override bool Equals(object obj)
        {
            var o = obj as FileResource<D3D9T, D3D10T>;
            if (o == null) return false;
            return Object.Equals(FileName, o.FileName);
        }
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ (FileName != null ? FileName.GetHashCode() : 1);
        }
        public override string ToString()
        {
            return GetType().Name + "." + FileName;
        }
    }

    [Serializable]
    public class MeshFromFile : FileResource<SlimDX.Direct3D9.Mesh, SlimDX.Direct3D10.Mesh>
    {
        public MeshFromFile() { Flags = MeshFlags.Managed; }
        public MeshFromFile(String filename) : this() { FileName = filename; }
        public MeshFromFile(MeshFromFile copy) : base(copy) { Flags = copy.Flags; }

        public override object Clone()
        {
            return new MeshFromFile(this);
        }

        public SlimDX.Direct3D9.MeshFlags Flags { get; set; }

        public class MapperSoftware : MetaMapper<MeshFromFile, Software.Mesh>
        {
            public override Software.Mesh Construct(MeshFromFile metaResource, ContentPool content)
            {
                var mesh = content.Peek<SlimDX.Direct3D9.Mesh>(metaResource);
                return new Graphics.Software.Mesh(mesh);
            }

            public override void Release(MeshFromFile metaResource, ContentPool content, Software.Mesh resource)
            {
            }
        }

        public class Mapper9 : MetaMapper<MeshFromFile, SlimDX.Direct3D9.Mesh>
        {
            public override SlimDX.Direct3D9.Mesh Construct(MeshFromFile metaResource, ContentPool content)
            {
                Stream s = GetStream(content, metaResource.FileName, metaResource.FileName);
                var v = SlimDX.Direct3D9.Mesh.FromStream(content.Device9,
                    s, metaResource.Flags);
                s.Close();
                v.Tag = metaResource.FileName;
                return v;
            }
        }
        public class MapperBoundingBox : MetaMapper<MeshFromFile, StructBoxer<BoundingBox>>
        {
            public override StructBoxer<BoundingBox> Construct(MeshFromFile metaResource, ContentPool content)
            {
                var mesh = content.Peek<SlimDX.Direct3D9.Mesh>(metaResource);
                return new StructBoxer<BoundingBox> { Value = Common.Boundings.BoundingBoxFromXMesh(mesh) };
            }

            public override void Release(MeshFromFile metaResource, ContentPool content, StructBoxer<BoundingBox> resource)
            {
            }
        }
    }

    [Serializable]
    public class ColladaMeshFromFile : FileResource<Software.Mesh>
    {
        public ColladaMeshFromFile() { }
        public ColladaMeshFromFile(String filename) { FileName = filename; }
        public bool RecalcNormals { get; set; }

        public class Mapper : MetaMapper<ColladaMeshFromFile, Software.Mesh>
        {
            public override Software.Mesh Construct(ColladaMeshFromFile metaResource, ContentPool content)
            {
                var sysmemMesh = ColladaLoader.ParseCollada(metaResource.AbsolutePath(content));
                if (metaResource.RecalcNormals) sysmemMesh.RecalcNormals();
                return sysmemMesh;
            }
            public override void Release(ColladaMeshFromFile metaResource, ContentPool content, Software.Mesh resource)
            {
            }
        }

        /*public override bool Equals(object obj)
        {
            var o = obj as ColladaMeshFromFile;
            if (o == null) return false;
            return base.Equals(obj) && RecalcNormals == o.RecalcNormals;
        }*/
    }






    [Serializable]
    public class EffectFromFile : FileResource<SlimDX.Direct3D9.Effect, SlimDX.Direct3D10.Effect>
    {
        public EffectFromFile() { }
        public EffectFromFile(String filename) { FileName = filename; }

        public class Mapper9 : MetaMapper<EffectFromFile, SlimDX.Direct3D9.Effect>
        {
            public override SlimDX.Direct3D9.Effect Construct(EffectFromFile metaResource, ContentPool content)
            {
                Stream s = GetStream(content, metaResource.FileName, metaResource.FileName + "o");
                var e = SlimDX.Direct3D9.Effect.FromStream(content.Device9, s, null,
                    new Include9 { Content = content }, null, SlimDX.Direct3D9.ShaderFlags.None);
                s.Close();
                return e;
            }
            public override void OnLostDevice(EffectFromFile metaResource, ContentPool content, SlimDX.Direct3D9.Effect resource)
            {
                resource.OnLostDevice();
            }
            public override SlimDX.Direct3D9.Effect OnResetDevice(EffectFromFile metaResource, ContentPool content, SlimDX.Direct3D9.Effect oldResource)
            {
                oldResource.OnResetDevice();
                return oldResource;
            }
        }

        public class Mapper10 : MetaMapper<EffectFromFile, SlimDX.Direct3D10.Effect>
        {
            public override SlimDX.Direct3D10.Effect Construct(EffectFromFile metaResource, ContentPool content)
            {
                Stream s = GetStream(content, metaResource.FileName, metaResource.FileName + "o");
                var e = SlimDX.Direct3D10.Effect.FromStream(content.Device10, s,
                    "fx_4_0", SlimDX.D3DCompiler.ShaderFlags.Debug, SlimDX.D3DCompiler.EffectFlags.None,
                    null, new Include10 { Content = content }, null);
                s.Close();
                return e;
            }
        }

        class Include10 : SlimDX.D3DCompiler.Include
        {
            public ContentPool Content;
            public void Close(Stream stream)
            {
                stream.Close();
            }
            public void Open(SlimDX.D3DCompiler.IncludeType type, string fileName, Stream parentStream, out Stream stream)
            {
                stream = GetStream(Content, fileName, fileName + "o");
            }
        }

        class Include9 : SlimDX.Direct3D9.Include
        {
            public ContentPool Content;
            public void Close(Stream stream)
            {
                stream.Close();
            }
            public void Open(SlimDX.Direct3D9.IncludeType type, string fileName, Stream parentStream, out Stream stream)
            {
                stream = GetStream(Content, fileName, fileName + "o");
            }
        }
    }

    [Serializable]
    public class SkinnedMeshFromFile : FileResource<SkinnedMesh>
    {
        public SkinnedMeshFromFile() { }
        public SkinnedMeshFromFile(String filename) { FileName = filename; }

        public class Mapper : MetaMapper<SkinnedMeshFromFile, SkinnedMesh>
        {
            public override SkinnedMesh Construct(SkinnedMeshFromFile metaResource, ContentPool content)
            {
                string str = content.ContentPath != null ? content.ContentPath + "/" + metaResource.FileName : metaResource.FileName;
                Stream s = null;

                if (!Common.FileSystem.Instance.FileExists(str))
                    Application.Log("SkinnedMeshFromFile: File doesn't exist: " + str);
                else
                    s = Common.FileSystem.Instance.OpenRead(str);
                
                return new SkinnedMesh(s, content.Device9);
            }
        }
        public class MapperBoundingBox : MetaMapper<SkinnedMeshFromFile, StructBoxer<BoundingBox>>
        {
            public override StructBoxer<BoundingBox> Construct(SkinnedMeshFromFile metaResource, ContentPool content)
            {
                var mesh = content.Peek<SkinnedMesh>(metaResource);
                return new StructBoxer<BoundingBox> { Value = mesh.Bounding };
            }

            public override void Release(SkinnedMeshFromFile metaResource, ContentPool content, StructBoxer<BoundingBox> resource)
            {
            }
        }
    }

    public class VertexStreamLayoutFromEffect : MetaResource<SlimDX.Direct3D9.VertexFormat, SlimDX.Direct3D10.InputLayout>
    {
        public Software.Vertex.IVertex Layout { get; set; }
        public SlimDX.D3DCompiler.ShaderSignature Signature10 { get; set; }

        /*public override bool Equals(object obj)
        {
            var o = obj as VertexStreamLayoutFromEffect;
            if (o == null) return false;
            return Object.Equals(Layout, o.Layout) && Object.Equals(Signature10, o.Signature10);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + Layout + (Signature10 != null ? Signature10.GetHashCode().ToString() : "null");
        }*/

        public class Mapper9 : MetaMapper<VertexStreamLayoutFromEffect, SlimDX.Direct3D9.VertexFormat>
        {
            public override SlimDX.Direct3D9.VertexFormat Construct(VertexStreamLayoutFromEffect metaResource, ContentPool content)
            {
                return metaResource.Layout.VertexFormat;
            }
            public override void Release(VertexStreamLayoutFromEffect metaResource, ContentPool content, SlimDX.Direct3D9.VertexFormat resource)
            {
            }
        }

        public class Mapper10 : MetaMapper<VertexStreamLayoutFromEffect, SlimDX.Direct3D10.InputLayout>
        {
            public override SlimDX.Direct3D10.InputLayout Construct(VertexStreamLayoutFromEffect metaResource, ContentPool content)
            {
                return new SlimDX.Direct3D10.InputLayout(content.Device10,
                    metaResource.Signature10,
                    metaResource.Layout.InputElements);
            }
        }

    }
}
