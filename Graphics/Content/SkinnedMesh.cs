using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace Graphics.Content
{
    public class SkinnedMesh : IDisposable
    {
        public SkinnedMesh(System.IO.Stream modelName, Device device)
        {
            RootFrame = Frame.LoadHierarchyFromX(device, modelName, MeshFlags.Managed, new CustomAllocateHierarchy(), null, out AnimationController);

            SetupBoneMatrices(RootFrame);
            BoundingSphere = SlimDX.Direct3D9.Frame.CalculateBoundingSphere(RootFrame);


            MeshContainers = new List<Common.Tuple<CustomFrame, CustomMeshContainer>>();
            GetMeshContainers(RootFrame, MeshContainers);

            Frames = new List<CustomFrame>();
            ForEachFrame((f) => Frames.Add(f));

            Bounding = CalcBoundingValues(RootFrame).Value;

            Animations = new Dictionary<String, float>();

            if(AnimationController != null)
                for (int i = 0; i < AnimationController.AnimationSetCount; i++)
                {
                    using(var animSet = AnimationController.GetAnimationSet<AnimationSet>(i))
                        Animations.Add(animSet.Name, (float)animSet.Period);
                }
            ((CustomFrame)RootFrame).SaveMatrices();
        }

        public void RemoveMeshContainerByFrameName(String name)
        {
            foreach (var v in MeshContainers)
                if (v.First.Name == name)
                {
                    MeshContainers.Remove(v);
                    break;
                }
        }

        public BoundingBox Bounding;

        BoundingBox GetMinMaxValues(MeshContainer mc)
        {
            DataStream ds = mc.MeshData.Mesh.LockVertexBuffer(LockFlags.ReadOnly);
            List<Vector3> vertices = new List<Vector3>();
            do
            {
                vertices.Add(ds.Read<Vector3>());
                ds.Seek(mc.MeshData.Mesh.BytesPerVertex - Vector3.SizeInBytes, System.IO.SeekOrigin.Current);   
            } while (ds.Position < ds.Length);

            var bb = BoundingBox.FromPoints(vertices.ToArray());

            mc.MeshData.Mesh.UnlockVertexBuffer();
            return bb;
        }

        public List<Common.Tuple<CustomFrame, CustomMeshContainer>> MeshContainers;

        public List<CustomFrame> Frames;

        void GetMeshContainers(Frame frame, List<Common.Tuple<CustomFrame, CustomMeshContainer>> meshContainers)
        {
            MeshContainer meshContainer = frame.MeshContainer;
            while (meshContainer != null)
            {
                meshContainers.Add(new Common.Tuple<CustomFrame, CustomMeshContainer>(
                    (CustomFrame)frame, (CustomMeshContainer)meshContainer));
                meshContainer = meshContainer.NextMeshContainer;
            }

            if (frame.Sibling != null)
                GetMeshContainers(frame.Sibling, meshContainers);

            if (frame.FirstChild != null)
                GetMeshContainers(frame.FirstChild, meshContainers);
        }

        BoundingBox? CalcBoundingValues(Frame frame)
        {
            BoundingBox? b = null;
            if (frame.MeshContainer != null)
                b = GetMinMaxValues(frame.MeshContainer);

            if (frame.Sibling != null)
            {
                var b2 = CalcBoundingValues(frame.Sibling);
                if (b2 != null)
                {
                    if (b != null) b = BoundingBox.Merge(b.Value, b2.Value);
                    else b = b2;
                }
            }

            if (frame.FirstChild != null)
            {
                var b2 = CalcBoundingValues(frame.FirstChild);
                if (b2 != null)
                {
                    if (b != null) b = BoundingBox.Merge(b.Value, b2.Value);
                    else b = b2;
                }
            }
            return b;
        }

        public void Dispose()
        {
            Frame.DestroyHierarchy(RootFrame, new CustomAllocateHierarchy());
            if(AnimationController != null)
                AnimationController.Dispose();
        }

        void SetupBoneMatrices(Frame frame)
        {
            if (frame.MeshContainer != null)
                SetupBoneMatrices(frame.MeshContainer as CustomMeshContainer);
            if (frame.Sibling != null)
                SetupBoneMatrices(frame.Sibling);
            if (frame.FirstChild != null)
                SetupBoneMatrices(frame.FirstChild);
        }

        void SetupBoneMatrices(CustomMeshContainer meshContainer)
        {
            if (meshContainer.SkinInfo == null)
                return;

            meshContainer.BoneMatricesLookup = new CustomFrame[meshContainer.SkinInfo.BoneCount];
            for (int i = 0; i < meshContainer.SkinInfo.BoneCount; i++)
            {
                CustomFrame frame = (CustomFrame)RootFrame.FindChild(meshContainer.SkinInfo.GetBoneName(i));
                meshContainer.BoneMatricesLookup[i] = frame;
            }
        }

        public void UpdateFrameMatrices(Frame frame, Matrix matrix)
        {
            CustomFrame cframe = frame as CustomFrame;
            
            cframe.CombinedTransform = cframe.TransformationMatrix * matrix;

            if (cframe.Sibling != null)
                UpdateFrameMatrices(cframe.Sibling, matrix);

            if (cframe.FirstChild != null)
                UpdateFrameMatrices(cframe.FirstChild, cframe.CombinedTransform);
        }


        public void ForEachFrame(Action<CustomFrame> action)
        {
            ForEachFrame(RootFrame, action);
        }
        void ForEachFrame(Frame frame, Action<CustomFrame> action)
        {
            CustomFrame cframe = frame as CustomFrame;

            action(cframe);

            if (cframe.Sibling != null)
                ForEachFrame(cframe.Sibling, action);

            if (cframe.FirstChild != null)
                ForEachFrame(cframe.FirstChild, action);
        }

        public Frame RootFrame, SaveFrame;
        public AnimationController AnimationController;
        public BoundingSphere BoundingSphere;

        public Dictionary<String, float> Animations;

        public static Matrix InitSkinnedMeshFromMaya =
            Matrix.RotationY((float)Math.PI) * Matrix.RotationX((float)Math.PI / 2) * Matrix.RotationZ((float)Math.PI / 2);
    }
}