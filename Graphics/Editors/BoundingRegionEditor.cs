using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;
using Graphics.Content;
using SlimDX.Direct3D9;
using Common.Pathing;
using Graphics.Software;
using Graphics.GraphicsDevice;

namespace Graphics.Editors
{

    public class BoundingRegionEditor : InputHandler
    {
        WorldViewProbe worldProbing;

        public BoundingRegionEditor(View view, WorldViewProbe worldProbing)
        {
            this.view = view;
            this.worldProbing = worldProbing;
        }
        public float NodeScale = 1;

        public Common.Bounding.Region Region
        {
            get { return region; }
            set
            {
                if (region != null) Compile();
                Load(value);
            }
        }

        void Load(Common.Bounding.Region region)
        {
            Clear();
            hasChanged = false;
            this.region = region;
            if (region == null || region.Nodes == null) return;
            Dictionary<Vector3, Vertex> posToVertex = new Dictionary<Vector3, Vertex>();
            Dictionary<Common.Bounding.RegionNode, Face> nodeToFace =
                new Dictionary<Common.Bounding.RegionNode, Face>();
            for (int i = 0; i < region.Nodes.Length; i++)
            {
                var node = region.Nodes[i];
                foreach (Vector3 v in node.polygon)
                {
                    if (!posToVertex.ContainsKey(v))
                    {
                        Vertex ve = new Vertex(this)
                        {
                            Translation = v
                        };
                        vertices.Add(ve);
                        posToVertex.Add(v, ve);
                    }
                }
                Face face = new Face(this)
                {
                    Vertices = new Vertex[] { posToVertex[node.polygon[0]], posToVertex[node.polygon[1]], posToVertex[node.polygon[2]] },
                    Id = i
                };
                posToVertex[node.polygon[0]].Faces.Add(face);
                posToVertex[node.polygon[1]].Faces.Add(face);
                posToVertex[node.polygon[2]].Faces.Add(face);
                faces.Add(face);
                nodeToFace.Add(node, face);
            }

            foreach (var node in region.Nodes)
            {
                foreach (Common.Bounding.RegionEdge edge in node.Edges.Values)
                {
                    Face left = nodeToFace[edge.Left];
                    Face right = nodeToFace[edge.Right];
                    if (!left.ContainsEdgeTo(right) && !right.ContainsEdgeTo(left))
                    {
                        int index = 0;
                        while (left.Vertices[index].Translation != edge.PointA) index++;
                        left.Edges.Add(new Edge
                        {
                            Left = left,
                            Right = right,
                            LeftIndex = index
                        });
                    }
                }
            }
        }

        public void Compile()
        {
            if (!hasChanged || region == null) return;
            List<Common.Bounding.RegionNode> nodes = new List<Common.Bounding.RegionNode>();
            for (int i = 0; i < faces.Count; i++)
            {
                Face face = faces[i];
                face.Id = i;
                nodes.Add(new Common.Bounding.RegionNode(
                    new Vector3[] 
                    { 
                        face.Vertices[0].Translation, 
                        face.Vertices[1].Translation, 
                        face.Vertices[2].Translation 
                    }));
            }
            region.Nodes = nodes.ToArray();
            foreach (Face face in faces)
            {
                foreach (Edge edge in face.Edges)
                    Common.Bounding.RegionNode.Connect(
                        nodes[edge.Left.Id],
                        nodes[edge.Right.Id],
                        edge.PointA,
                        edge.PointB);
            }
        }

        private void Clear()
        {
            vertices.Clear();
            faces.Clear();
            region = null;
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.Delete)
            {
                var mov = GetMouseOverVertex();
                var mof = GetMouseOverFace();
                if (mov != null)
                    mov.Remove();
                else if (mof != null)
                    mof.Remove();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Up || e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Up)
                {
                    candidateSelected++;
                }
                else
                {
                    candidateSelected--;
                }
                nextFace = candidateFaces[Common.Math.Clamp(candidateSelected, 0, candidateFaces.Count - 1)];
            }
        }


        Vertex GetMouseOverVertex()
        {
            var screenRay = worldProbing.ScreenRay();
            screenRay.Direction = Vector3.Normalize(screenRay.Direction);
            Vertex min = null;
            float minD = float.MaxValue;
            foreach (Vertex v in vertices)
            {
                var projPos = screenRay.Position + screenRay.Direction * Vector3.Dot(v.Translation - screenRay.Position, screenRay.Direction);
                float d = (projPos - v.Translation).Length();
                if (d < minD)
                {
                    min = v;
                    minD = d;
                }
            }
            if (minD < NodeScale * 2)
                return min;
            else
                return null;
        }

        Face GetMouseOverFace()
        {
            Face min = null;
            float minD = float.MaxValue;
            foreach (Face f in faces)
            {
                float d;
                Vector2 uv;
                if(f.Intersect(worldProbing.ScreenRay(), out d, out uv) && d < minD)
                {
                    min = f;
                    minD = d;
                }
            }
            return min;
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                moving = GetMouseOverVertex();
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left) moving = null;
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            Vector3 world;
            nextFace = null;
            if (rightMouseDown) return;
            if (worldProbing.Intersect(out world))
            {
                hasChanged = true;
                mouseWorld = world;
                var mo = GetMouseOverVertex();

                if (moving != null)
                {
                    hasChanged = true;
                    moving.Translation = world;
                }
                else if (mo == null)
                {
                    var moface = GetMouseOverFace();
                    if (moface != null) return;

                    candidateFaces = GetCandidateFaces(world);
                    if (candidateFaces.Count > 0)
                        nextFace = candidateFaces[Common.Math.Clamp(candidateSelected, 0, candidateFaces.Count - 1)];
                }
            }
        }

        List<Face> candidateFaces;
        int candidateSelected;

        List<Face> GetCandidateFaces(Vector3 position)
        {
            List<Face> faces = new List<Face>();
            Common.PriorityQueue<float, Vertex> q = new Common.PriorityQueue<float, Vertex>();
            foreach (Vertex v in vertices)
            {
                float d = (v.Translation - position).Length();
                if (d < 30)
                    q.Enqueue(d, v);
            }
            if (q.Count() < 3) return faces;
            Vertex a_ = q.Dequeue(), b_ = q.Dequeue();
            while (!q.IsEmpty)
            {
                Vertex a = a_, b = b_, c = q.Dequeue();
                Vector3 minX = Common.Math.MinX(a.Translation, b.Translation, c.Translation);
                if (b.Translation == minX) Common.Math.Swap(ref a, ref b);
                else if (c.Translation == minX) Common.Math.Swap(ref a, ref c);
                if (Vector2.Dot(Common.Math.ToVector2(
                    Common.Math.PerpendicularXY(b.Translation - a.Translation)),
                    Common.Math.ToVector2(c.Translation - a.Translation))
                    > 0)
                    Common.Math.Swap(ref b, ref c);

                var f = new Face(this);
                f.Vertices[0] = a;
                f.Vertices[1] = b;
                f.Vertices[2] = c;

                float d;
                Vector2 uv;
                if (f.Intersect(worldProbing.ScreenRay(), out d, out uv))
                    faces.Add(f);
            }
            return faces;
        }

        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button != System.Windows.Forms.MouseButtons.Left || moving != null) return;
            Vector3 world;
            if (worldProbing.Intersect(out world))
            {
                var mof = GetMouseOverFace();
                var mov = GetMouseOverVertex();

                hasChanged = true;
                if (nextFace != null)
                {
                    nextFace.Id = 0;
                    faces.Add(nextFace);
                    List<Face> neighbours = new List<Face>();
                    foreach (Vertex vertex in nextFace.Vertices)
                    {
                        foreach (Face f in vertex.Faces)
                            neighbours.Add(f);
                        vertex.Faces.Add(nextFace);
                    }
                    foreach (Face f in neighbours.Distinct())
                    {
                        bool found = false;
                        for (int l = 0; !found && l < 3; l++)
                            for (int r = 1; !found && r < 4; r++)
                                if (nextFace.Vertices[l] == f.Vertices[r % 3] &&
                                    (nextFace.Vertices[(l + 1) % 3] == f.Vertices[(r + 1) % 3] ||
                                    nextFace.Vertices[(l + 1) % 3] == f.Vertices[(r - 1) % 3]))
                                {
                                    nextFace.Edges.Add(new Edge
                                    {
                                        Left = nextFace,
                                        Right = f,
                                        LeftIndex = l
                                    });
                                    found = true;
                                }
                    }
                }
                else
                {
                    if (mof == null && mov == null)
                        vertices.Add(new Vertex(this) { Translation = world });
                }
            }
        }


        class Vertex
        {
            public Vertex(BoundingRegionEditor editor)
            {
                this.editor = editor;
            }
            public void Remove()
            {
                editor.vertices.Remove(this);
                foreach (Face f in new List<Face>(Faces))
                    f.Remove();
            }
            public void ReevaluateZ()
            {
                Vector3 oldpos = Translation;
            }
            public Vector3 Translation;
            BoundingRegionEditor editor;
            public List<Face> Faces = new List<Face>();
        }
        class Face
        {
            public Face(BoundingRegionEditor editor) : base() { this.editor = editor; }
            BoundingRegionEditor editor;
            public Vertex[] Vertices = new Vertex[3];
            public List<Edge> Edges = new List<Edge>();
            public int Id;
            public Vector3 Center { get { return (Vertices[0].Translation + Vertices[1].Translation + Vertices[2].Translation) / 3f; } }
            public bool ContainsEdgeTo(Face face)
            {
                foreach (Edge e in Edges) if (e.Left == face || e.Right == face) return true;
                return false;
            }

            public void Remove()
            {
                editor.faces.Remove(this);
                foreach (Vertex v in Vertices)
                {
                    foreach (Face f in v.Faces)
                        f.RemoveEdgesTo(this);
                    v.Faces.Remove(this);
                }
            }
            void RemoveEdgesTo(Face f)
            {
                List<Edge> edges = new List<Edge>();
                foreach (Edge e in Edges)
                    if (e.Left != f && e.Right != f)
                        edges.Add(e);
                Edges = edges;
            }
            public bool Intersect(Ray ray, out float d, out Vector2 uv)
            {
                return Triangle.Intersect(ray, true, out d, out uv);
            }
            public Triangle Triangle
            {
                get
                {
                    Triangle t = new Triangle
                    {
                        A = new Graphics.Software.Vertex.Position3(Vertices[0].Translation),
                        B = new Graphics.Software.Vertex.Position3(Vertices[1].Translation),
                        C = new Graphics.Software.Vertex.Position3(Vertices[2].Translation)
                    };
                    t.CalcCachedData();
                    return t;
                }
            }
        }
        class Edge
        {
            public Face Left, Right;
            public int LeftIndex;
            public Vector3 PointA { get { return Left.Vertices[LeftIndex].Translation; } }
            public Vector3 PointB { get { return Left.Vertices[(LeftIndex + 1) % 3].Translation; } }
        }

        List<Vertex> vertices = new List<Vertex>();
        List<Face> faces = new List<Face>();
        View view;
        Face nextFace;
        bool rightMouseDown = false;
        Common.Bounding.Region region;
        Vertex moving;
        Vector3 mouseWorld;
        bool hasChanged = false;

        public class Renderer9
        {
            public Renderer9(BoundingRegionEditor editor)
            {
                this.editor = editor;
            }
            public IDevice9StateManager StateManager;
            public Camera Camera;


            public virtual void Render(View view)
            {
                view.Device9.PixelShader = null;
                view.Device9.VertexShader = null;

                StateManager.SetRenderState(RenderState.Lighting, false);
                StateManager.SetRenderState(RenderState.AlphaTestEnable, true);
                StateManager.SetRenderState(RenderState.AlphaRef, 1);
                StateManager.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
                StateManager.SetRenderState(RenderState.AlphaBlendEnable, true);
                StateManager.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                StateManager.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                StateManager.SetRenderState(RenderState.ZEnable, false);
                StateManager.SetRenderState(RenderState.ZWriteEnable, false);
                StateManager.SetRenderState(RenderState.MultisampleAntialias, false);

                StateManager.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
                StateManager.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
                StateManager.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);
                StateManager.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);
                StateManager.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Point);

                view.Device9.SetTransform(TransformState.Projection, Camera.Projection);
                view.Device9.SetTransform(TransformState.View, Camera.View);
                view.Device9.SetTransform(TransformState.World, Matrix.Identity);

                StateManager.SetTexture(0, view.Content.Peek<SlimDX.Direct3D9.Texture>(faceTexture));

                var moface = editor.GetMouseOverFace();
                var mov = editor.GetMouseOverVertex();

                foreach (var v in editor.faces)
                    DrawFace(view, v);

                StateManager.SetTexture(0, view.Content.Peek<SlimDX.Direct3D9.Texture>(new TextureConcretizer { Texture = Software.ITexture.SingleColorTexture(Color.FromArgb(100, Color.DarkBlue)) }));
                if (editor.nextFace != null)
                    DrawFace(view, editor.nextFace);

                StateManager.SetTexture(0, view.Content.Peek<SlimDX.Direct3D9.Texture>(new TextureConcretizer { Texture = Software.ITexture.SingleColorTexture(Color.FromArgb(100, Color.Green)) }));
                if (mov == null && moface != null)
                    DrawFace(view, moface);

                foreach (var v in editor.faces)
                    DrawFaceLines(view, v);
                if (editor.nextFace != null)
                    DrawFaceLines(view, editor.nextFace);

                StateManager.SetTexture(0, view.Content.Peek<SlimDX.Direct3D9.Texture>(nodeModels.Texture));
                foreach (var p in editor.vertices)
                {
                    view.Device9.SetTransform(TransformState.World,
                        Matrix.Scaling(editor.NodeScale, editor.NodeScale, editor.NodeScale) * Matrix.Translation(p.Translation));
                    view.Content.Peek<Mesh9>(nodeModels.Mesh).Draw(view.Device9);
                }

                if (mov != null)
                {
                    StateManager.SetTexture(0, view.Content.Peek<SlimDX.Direct3D9.Texture>(new TextureConcretizer { Texture = Software.ITexture.SingleColorTexture(Color.Green) }));

                    view.Device9.SetTransform(TransformState.World,
                        Matrix.Scaling(editor.NodeScale, editor.NodeScale, editor.NodeScale) * Matrix.Translation(mov.Translation));
                    view.Content.Peek<Mesh9>(nodeModels.Mesh).Draw(view.Device9);
                }
            }

            void DrawFace(View view, Face face)
            {
                var triangle = view.Content.Peek<Mesh9>(new MeshConcretize
                {
                    MeshDescription = new Software.Meshes.TriangleMesh
                    {
                        PositionA = face.Vertices[0].Translation,
                        PositionB = face.Vertices[1].Translation,
                        PositionC = face.Vertices[2].Translation,
                        TwoSided = true
                    },
                    Layout = Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                });
                triangle.Draw(view.Device9);
            }
            void DrawFaceLines(View view, Face face)
            {
                view.Draw3DLines(Camera, Matrix.Identity, new Vector3[]
                    {
                        face.Vertices[0].Translation, 
                        face.Vertices[1].Translation, 
                        face.Vertices[2].Translation
                    }, Color.Blue);
            }

            BoundingRegionEditor editor;

            MetaResourceBase faceTexture = new TextureConcretizer { Texture = Software.ITexture.SingleColorTexture(Color.FromArgb(100, Color.LightBlue)) };
            MetaModel nodeModels = new MetaModel
            {
                Mesh = new MeshConcretize
                {
                    MeshDescription = new Software.Meshes.BoxMesh
                    {
                        Max = new Vector3(1, 1, 1),
                        Min = new Vector3(-1, -1, -1)
                    },
                    Layout = Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureConcretizer { Texture = Software.ITexture.SingleColorTexture(Color.White) }
            };
        }
    }
}
