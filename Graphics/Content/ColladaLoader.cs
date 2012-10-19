using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics.Content
{
    public class ColladaLoader
    {

        private static Vector3[] ParseVertexData(float[] values)
        {
            if (values.Length == 0)
                return null;

            int size = (int)(values.Length / 3);
            Vector3[] vertices = new Vector3[size];
            for (int i = 0; i < size; i++)
                vertices[i] = new Vector3(values[i * 3], values[i * 3 + 2], values[i * 3 + 1]);
            /*
            // pi/2 rotation around the x-axis: x=x, y=-z, z=y (old z is -arr[k+2])
            //Vector4 v = Vector3.Transform(new Vector3(arr[k], arr[k + 2], arr[k + 1]), world);
            //Vector3 bajs = new Vector3(v.X / v.W, v.Y / v.W, v.Z / v.W);
            //vertexData[k / 3] = bajs;
            vertexData[k / 3] = new Vector3(arr[k], arr[k + 2], arr[k + 1]);*/
            return vertices;
        }

        private static Vector3[] ParseNormalData(float[] values)
        {
            if (values.Length == 0)
                return null;

            int size = (int)(values.Length / 3);
            Vector3[] normals = new Vector3[size];
            for (int i = 0; i < size; i++)
                normals[i] = new Vector3(values[i * 3], values[i * 3 + 2], values[i * 3 + 1]);

            /*
            // (REVIEW THIS) pi/2 rotation around the x-axis: x=x, y=z, z=-y (old z is -arr[k+2]
                            //Vector4 v = Vector3.Transform(new Vector3(arr[k], arr[k + 2], -arr[k + 1]), world);
                            //normalData[k / 3] = new Vector3(v.X / v.W, v.Y / v.W, v.Z / v.W);
                            normalData[k / 3] = new Vector3(arr[k], arr[k + 2], arr[k + 1]); */
            return normals;
        }

        private static Vector3[] ParseTexcoordData(float[] values, int coordCount)
        {
            if (values.Length == 0)
                return null;

            int size = (int)(values.Length / coordCount);
            Vector3[] texcoords = new Vector3[size];
            for (int i = 0; i < size; i++)
            {
                Vector3 t = Vector3.Zero;
                if(coordCount >= 1)
                    t.X = values[i * coordCount];
                if (coordCount >= 2)
                    t.Y = 1 - values[i * coordCount + 1];
                if (coordCount >= 3)
                    t.Z = values[i * coordCount + 2];
                texcoords[i] = t;
            }
            return texcoords;
        }

        private static ColladaMesh ExtractColladaGeometry(ColladaDocument.Geometry g)
        {
            ColladaMesh mesh = new ColladaMesh();
            var m = g.mesh;
            Vector3[] vertexData = null;
            Vector3[] normalData = null;
            Vector3[] texcoordData = null;

            if (m.primitives.Count != 1)
                throw new NotImplementedException("m.primitives.Count = " + m.primitives.Count);
            var p = m.primitives[0];


            Dictionary<string, float[]> data = new Dictionary<string, float[]>();
            for (int i = 0; i < m.sources.Count; i++)
            {
                ColladaDocument.Array<float> inputArr = (ColladaDocument.Array<float>)m.sources[i].array;
                if (data.ContainsKey(m.sources[i].id))
                    throw new Exception("A data entry called " + m.sources[i].id + " has already been parsed.");
                data[m.sources[i].id] = inputArr.arr;
            }

            int indlen = p.p.Length / p.stride;
            int[] vertexIndices = new int[indlen];
            int[] normalIndices = new int[indlen];
            int[] texcoordIndices = new int[indlen];
            foreach (ColladaDocument.Input input in p.Inputs)
            {
                switch (input.semantic.ToLower())
                {
                    case "vertex":
                        if (!(input.source is ColladaDocument.Vertices))
                            throw new Exception("VERTEX source should be of type ColladaDocument.Vertices");
                        ProcessVertex((ColladaDocument.Vertices)input.source, p.p, p.stride, input.offset, data,
                            ref vertexData, ref vertexIndices, ref normalData, ref normalIndices, ref texcoordData, ref texcoordIndices);
                        break;
                    case "normal":
                        if (!(input.source is ColladaDocument.Source))
                            throw new Exception("NORMAL source should be of type ColladaDocument.Vertices");
                        var nsource = (ColladaDocument.Source)input.source;
                        // set up indices
                        for (int i = 0; i < indlen; i++)
                            normalIndices[i] = p.p[i * p.stride + input.offset];
                        // load data
                        normalData = ParseNormalData(data[nsource.id]);
                        break;
                    case "texcoord":
                        if (!(input.source is ColladaDocument.Source))
                            throw new Exception("TEXCOORD source should be of type ColladaDocument.Vertices");
                        var tsource = (ColladaDocument.Source)input.source;
                        // set up indices
                        for (int i = 0; i < indlen; i++)
                            texcoordIndices[i] = p.p[i * p.stride + input.offset];
                        // load data
                        texcoordData = ParseTexcoordData(data[tsource.id], tsource.accessor.stride);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }


            if (vertexIndices.Length != normalIndices.Length || normalIndices.Length != texcoordIndices.Length)
                throw new NotImplementedException("all of them needs to be supplied and of the same amount right now");

            mesh.VertexData = new List<Vector3>(vertexData);
            mesh.VertexIndices = new List<int>(vertexIndices);

            if (normalData != null)
            {
                mesh.NormalData = new List<Vector3>(normalData);
                mesh.NormalIndices = new List<int>(normalIndices);
            }

            if (texcoordData != null)
            {
                mesh.TexcoordData = new List<Vector3>(texcoordData);
                mesh.TexcoordIndices = new List<int>(texcoordIndices);
            }

            return mesh;
        }

        private static void ProcessVertex(ColladaDocument.Vertices source, int[] p, int stride, int offset, Dictionary<string, float[]> data,
            ref Vector3[] vertexData, ref int[] vertexIndices,
            ref Vector3[] normalData, ref int[] normalIndices,
            ref Vector3[] texcoordData, ref int[] texcoordIndices)
        {
            int length = p.Length / stride;
            for (int i = 0; i < length; i++)
            {
                int value = p[i * stride + offset];
                foreach (var input in source.inputs)
                {
                    switch (input.semantic.ToLower())
                    {
                        case "position":
                            vertexIndices[i] = value;
                            break;
                        case "normal":
                            normalIndices[i] = value;
                            break;
                        case "texcoord":
                            texcoordIndices[i] = value;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            foreach (var input in source.inputs)
            {
                if (!(input.source is ColladaDocument.Source))
                    throw new Exception("CHILD OF VERTEX source should be of type ColladaDocument.Source");
                var csource = (ColladaDocument.Source)input.source;
                switch (input.semantic.ToLower())
                {
                    case "position":
                        vertexData = ParseVertexData(data[csource.id]);
                        break;
                    case "normal":
                        normalData = ParseNormalData(data[csource.id]);
                        break;
                    case "texcoord":
                        texcoordData = ParseTexcoordData(data[csource.id], csource.accessor.stride);
                        break;
                }
            }
        }

        public static Software.Mesh ParseCollada(String filename)
        {
            ColladaDocument doc = new ColladaDocument(filename);
            ColladaMesh resultMesh = new ColladaMesh();
            Dictionary<String, ColladaMesh> geometryMeshes = new Dictionary<string, ColladaMesh>();

            foreach (ColladaDocument.Geometry g in doc.geometries)
            {
                if (geometryMeshes.ContainsKey(g.id))
                    throw new Exception("Duplicate geometry entries");
                geometryMeshes[g.id] = ExtractColladaGeometry(g);
            }

            if (doc.visualScenes.Count != 1)
                throw new NotImplementedException("There has to be exactly one Visual Scene");

            foreach (ColladaDocument.Node n in doc.visualScenes[0].nodes)
                HandleNode(geometryMeshes, ref resultMesh, n, Matrix.Identity);

            resultMesh.BuildVertexList();

            return IndexedCollada(resultMesh.Vertices, resultMesh.Indices);
        }
        
        public static Software.Mesh IndexedCollada(List<ColladaLoader.Vertex> inVertices, List<int> inIndices)
        {
            var mesh = new Software.Mesh
            {
                MeshType = MeshType.Indexed,
                VertexStreamLayout = Graphics.Software.Vertex.Position3Normal3Texcoord3.Instance,
                NVertices = inVertices.Count,
                NFaces = inIndices.Count / 3
            };

            List<Graphics.Software.Vertex.Position3Normal3Texcoord3> verts = new List<Graphics.Software.Vertex.Position3Normal3Texcoord3>();
            foreach (var v in inVertices)
                verts.Add(new Graphics.Software.Vertex.Position3Normal3Texcoord3(v.Position, v.Normal, v.Texcoord));

            mesh.VertexBuffer = new Software.VertexBuffer<Graphics.Software.Vertex.Position3Normal3Texcoord3>(verts.ToArray());

            inIndices.Reverse();
            mesh.IndexBuffer = new Software.IndexBuffer(inIndices.ToArray());

            return mesh;
        }

        private static void HandleNode(Dictionary<string, ColladaMesh> geometryMeshes, ref ColladaMesh resultMesh, ColladaDocument.Node node, Matrix world)
        {
            if(node.transforms != null)
                world = ConcatTransforms(node.transforms, world);

            if (node.instances != null)
            {
                if (node.instances.Count != 1)
                    throw new Exception("Nodes need to have exactly one instance right now");

                if (!(node.instances[0] is ColladaDocument.InstanceGeometry))
                    return;     // SILENT FAIL

                ColladaDocument.Instance i = node.instances[0];
                ColladaMesh mesh = geometryMeshes[i.url.Fragment];
                resultMesh.AppendMesh(mesh, world);
                if (node.children != null && node.children.Count > 0)
                    throw new Exception("Tell Joakim.");
            }
            else if (node.children != null && node.children.Count > 0)
            {
                foreach (ColladaDocument.Node n in node.children)
                    HandleNode(geometryMeshes, ref resultMesh, n, world); 
            }
            else 
            {
                //throw new Exception("Node has no instance and no child. Keh?");
            }
        }

        private static Matrix ConcatTransforms(List<ColladaDocument.TransformNode> transforms, Matrix matrix)
        {
            foreach (var t in transforms)
            {
                if (t is ColladaDocument.Translate)
                {
                    if (t.Size != 3)
                        throw new Exception("Translation vectors needs to be of size 3 right now");
                    matrix = Matrix.Translation(t[0], t[2], t[1]) * matrix;
                }
                if (t is ColladaDocument.Rotate)
                {
                    if (t.Size != 4)
                        throw new Exception("Rotation vectors needs to be of size 3+1 right now");
                    matrix = Matrix.RotationAxis(new Vector3(t[0], t[2], t[1]), t[3] * (float)(Math.PI / 180f)) * matrix;
                }
                if (t is ColladaDocument.Scale)
                {
                    if (t.Size != 3)
                        throw new Exception("Translation vectors needs to be of size 3 right now");
                    matrix = Matrix.Scaling(t[0], t[2], t[1]) * matrix;
                }
            }
            return matrix;
        }

        public static Vector3 VectorTransform(Vector3 vector, Matrix transform)
        {
            Vector4 tmp = Vector3.Transform(vector, transform);
            return new Vector3(tmp.X, tmp.Y, tmp.Z);
        }




        public class ColladaMesh
        {
            public List<Vector3> VertexData = new List<Vector3>();
            public List<Vector3> NormalData = new List<Vector3>();
            public List<Vector3> TexcoordData = new List<Vector3>();
            public List<int> VertexIndices = new List<int>();
            public List<int> NormalIndices = new List<int>();
            public List<int> TexcoordIndices = new List<int>();

            public void AppendMesh(ColladaMesh mesh, Matrix worldMatrix)
            {
                Matrix normalMatrix = Matrix.Transpose(Matrix.Invert(worldMatrix));

                int vertexCount = VertexData.Count;
                int normalCount = NormalData.Count;
                int texcoordCount = TexcoordData.Count;

                foreach (Vector3 v in mesh.VertexData)
                    VertexData.Add(VectorTransform(v, worldMatrix));
                foreach (Vector3 v in mesh.NormalData)
                    NormalData.Add(Vector3.Normalize(VectorTransform(v, normalMatrix)));
                TexcoordData.AddRange(mesh.TexcoordData);

                foreach (int index in mesh.VertexIndices)
                    VertexIndices.Add(vertexCount + index);

                foreach (int index in mesh.NormalIndices)
                    NormalIndices.Add(normalCount + index);

                foreach (int index in mesh.TexcoordIndices)
                    TexcoordIndices.Add(texcoordCount + index);
            }

            public void BuildVertexList()
            {
                Dictionary<Vertex, int> vertices = new Dictionary<Vertex, int>();
                Vertices = new List<Vertex>();
                Indices = new List<int>();

                Vector3[] vertexData = VertexData.ToArray();
                Vector3[] normalData = NormalData.ToArray();
                Vector3[] texcoordData = TexcoordData.ToArray();
                int[] vertexIndices = VertexIndices.ToArray();
                int[] normalIndices = NormalIndices.ToArray();
                int[] texcoordIndices = TexcoordIndices.ToArray();

                for (int i = 0; i < vertexIndices.Length; i++)
                {
                    Vertex v = new Vertex()
                    {
                        Position = vertexData[vertexIndices[i]],
                        Normal = normalData[normalIndices[i]],
                        Texcoord = texcoordData[texcoordIndices[i]]
                    };
                    if (!vertices.ContainsKey(v))
                    {
                        Vertices.Add(v);
                        vertices.Add(v, vertices.Count);
                    }
                    Indices.Add(vertices[v]);
                }
            }

            public List<Vertex> Vertices;
            public List<int> Indices;
        }

        public class Vertex
        {
            public Vector3 Position { get; set; }
            public Vector3 Normal { get; set; }
            public Vector3 Texcoord { get; set; }

            public override bool Equals(object obj)
            {
                Vertex v = obj as Vertex;
                if (v == null)
                    return false;
                return Position == v.Position && Normal == v.Normal && Texcoord == v.Texcoord;
            }

            public override int GetHashCode()
            {
                return (Position.GetHashCode()*100) ^ (Normal.GetHashCode()*10) ^ Texcoord.GetHashCode();
            }
        }
    }
}
