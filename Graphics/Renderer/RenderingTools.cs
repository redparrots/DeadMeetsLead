#if BETA_RELEASE
//#define DEBUG_ANIMATIONS
//#define PROFILE_ANIMATIONS
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using SlimDX.Direct3D9;
using SlimDX;
using Graphics.Content;
using System.ComponentModel;

namespace Graphics
{
    namespace Renderer
    {
        public partial class Renderer
        {
            public struct ThreeTimesThreeMatrix
            {
                public float M11;
                public float M12;
                public float M13;
                public float M21;
                public float M22;
                public float M23;
                public float M31;
                public float M32;
                public float M33;
            }

            public static Matrix TurnToCamera(Camera camera, Matrix modelMatrix, Entity entity, ModelOrientationRelation orientationRelation)
            {
                Matrix billboardRotationMatrix = Matrix.Billboard(((LookatCamera)camera).Lookat,
                    camera.Position, ((LookatCamera)camera).Up,
                    -(((LookatCamera)camera).Lookat) - camera.Position);

                Matrix translate = Matrix.Translation(-((LookatCamera)camera).Lookat);

                if (orientationRelation == ModelOrientationRelation.Absolute)
                {
                    Matrix modelScaleRotationMatrix = modelMatrix;
                    modelScaleRotationMatrix.set_Rows(3, Matrix.Identity.get_Rows(3));

                    Matrix modelTranslationMatrix = modelMatrix;
                    modelTranslationMatrix.set_Rows(0, Matrix.Identity.get_Rows(0));
                    modelTranslationMatrix.set_Rows(1, Matrix.Identity.get_Rows(1));
                    modelTranslationMatrix.set_Rows(2, Matrix.Identity.get_Rows(2));

                    //Compensating for an issue where billboards are renderered upside down.
                    return TurnUpsideDown(modelScaleRotationMatrix * billboardRotationMatrix * modelTranslationMatrix * translate);
                }
                else
                {
                    var entityMatrix = entity.CombinedWorldMatrix;

                    Matrix entityScaleRotationMatrix = entityMatrix;
                    entityScaleRotationMatrix.set_Rows(3, Matrix.Identity.get_Rows(3));

                    Matrix entityTranslationMatrix = entityMatrix;
                    entityTranslationMatrix.set_Rows(0, Matrix.Identity.get_Rows(0));
                    entityTranslationMatrix.set_Rows(1, Matrix.Identity.get_Rows(1));
                    entityTranslationMatrix.set_Rows(2, Matrix.Identity.get_Rows(2));

                    //Compensating for an issue where billboards are renderered upside down.
                    return TurnUpsideDown(modelMatrix * entityScaleRotationMatrix * billboardRotationMatrix * entityTranslationMatrix * translate);
                }
            }

            private static Matrix TurnUpsideDown(Matrix matrix)
            {
                Matrix scaleCompensation = Matrix.Scaling(1, -1, 1);
                return scaleCompensation * matrix;
            }

            public static Matrix TurnToCameraAroundAxis(Camera camera, Matrix modelMatrix, Entity entity, Vector3 axis)
            {
                Vector3 look = camera.Position - entity.Translation;

                look = Vector3.Normalize(look);

                Vector3 forward = axis;

                forward = Vector3.Normalize(forward);

                Vector3 right = Vector3.Cross(look, forward);

                right = Vector3.Normalize(right);

                look = Vector3.Cross(forward, right);

                look = Vector3.Normalize(look);

                Matrix billboardRotationMatrix = Common.Math.MatrixFromVectors(right, look, forward, Vector3.Zero);

                var entityMatrix = entity.CombinedWorldMatrix;

                Matrix entityScaleRotationMatrix = entityMatrix;
                entityScaleRotationMatrix.set_Rows(3, Matrix.Identity.get_Rows(3));

                Matrix entityTranslationMatrix = entityMatrix;
                entityTranslationMatrix.set_Rows(0, Matrix.Identity.get_Rows(0));
                entityTranslationMatrix.set_Rows(1, Matrix.Identity.get_Rows(1));
                entityTranslationMatrix.set_Rows(2, Matrix.Identity.get_Rows(2));

                Matrix modelRotation = modelMatrix;
                modelRotation.set_Rows(3, Matrix.Identity.get_Rows(3));

                Matrix modelTranslation = modelMatrix;
                modelTranslation.set_Rows(0, Matrix.Identity.get_Rows(0));
                modelTranslation.set_Rows(1, Matrix.Identity.get_Rows(1));
                modelTranslation.set_Rows(2, Matrix.Identity.get_Rows(2));

                Matrix world = modelRotation * billboardRotationMatrix * modelTranslation *
                    entityScaleRotationMatrix *
                    Matrix.Translation(-modelTranslation.M41, -modelTranslation.M42, -modelTranslation.M43) *
                    entityTranslationMatrix;

                return world;
            }

            public void CalcShadowmapCamera(Vector3 lightDirection, float enlargement)
            {
                lightDirection = new Vector3(Vector3.Normalize(lightDirection).X * shadowMapCameraFactor.X, Vector3.Normalize(lightDirection).Y * shadowMapCameraFactor.Y, Vector3.Normalize(lightDirection).Z * shadowMapCameraFactor.Z);
                Vector3[] viewFrustumCorners = Scene.Camera.GetCornersFromRectangle();

                Matrix shadowView = Matrix.LookAtLH(Vector3.Zero, lightDirection, Vector3.UnitZ);

                for (int i = 0; i < viewFrustumCorners.Length; i++)
                {
                    viewFrustumCorners[i] = Vector3.TransformCoordinate(new Vector3(viewFrustumCorners[i].X, viewFrustumCorners[i].Y, viewFrustumCorners[i].Z), shadowView);
                }

                float maxZ = viewFrustumCorners[0].Z;
                float minZ = viewFrustumCorners[0].Z;
                float maxY = viewFrustumCorners[0].Y;
                float minY = viewFrustumCorners[0].Y;
                float maxX = viewFrustumCorners[0].X;
                float minX = viewFrustumCorners[0].X;

                for (int i = 0; i < viewFrustumCorners.Length; i++)
                {
                    if (viewFrustumCorners[i].X > maxX)
                        maxX = viewFrustumCorners[i].X;

                    if (viewFrustumCorners[i].X < minX)
                        minX = viewFrustumCorners[i].X;

                    if (viewFrustumCorners[i].Y > maxY)
                        maxY = viewFrustumCorners[i].Y;

                    if (viewFrustumCorners[i].Y < minY)
                        minY = viewFrustumCorners[i].Y;

                    if (viewFrustumCorners[i].Z > maxZ)
                        maxZ = viewFrustumCorners[i].Z;

                    if (viewFrustumCorners[i].Z < minZ)
                        minZ = viewFrustumCorners[i].Z;
                }

                float width = enlargement + maxX - minX;
                float height = enlargement + maxY - minY;
                float distance = enlargement + maxZ - minZ;

                Vector3 sMapPos = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, minZ);

                sMapPos = Vector3.TransformCoordinate(sMapPos, Matrix.Invert(shadowView));

                ShadowMapCamera = Matrix.LookAtLH(sMapPos - lightDirection, sMapPos, Vector3.UnitZ) *
                    Matrix.OrthoLH(width, height, lightDirection.Length(), lightDirection.Length() + distance);
            }

            public class EntityAnimation : IDisposable
            {
#if PROFILE_ANIMATIONS
                public static event Action ResetMatricesStart;
                public static event Action ResetMatricesStop;
                public static event Action SetupACStrat;
                public static event Action SetupACStop;
                public static event Action AdvanceTimeStart;
                public static event Action AdvanceTimeStop;
                public static event Action AdjustFrameMatricesStrat;
                public static event Action AdjustFrameMatricesStop;
                public static event Action UpdateMatricesStart;
                public static event Action UpdateMatricesStop;
                public static event Action StoreMatricesStart;
                public static event Action StoreMatricesStop;
#endif
                public EntityAnimation(Entity entity, Content.ContentPool content)
                {
                    Content = content;
                    Entity = entity;
                    FrameCustomValues = new Dictionary<string, Matrix>();
                    FrameTransformation = new Dictionary<CustomFrame, Matrix>();
                    InitializeAnimation();
                }

                public event Action<int> AnimationDone;
                /// <summary>
                /// Usage:
                /// StoredFrameMatrices[Tuple(frame, meshContainer)][material id].First == Combined transform or bone matrices array
                /// StoredFrameMatrices[Tuple(frame, meshContainer)][material id].Second == Same as first but invert transposed for lighting
                /// </summary>
                public Dictionary<SlimDX.Direct3D9.Mesh, Matrix[][]> StoredFrameMatrices =
                    new Dictionary<SlimDX.Direct3D9.Mesh, Matrix[][]>();

                public Dictionary<CustomFrame, Matrix> FrameTransformation { get; private set; }

                public Dictionary<string, Matrix> FrameCustomValues { get; private set; }

                public List<string> HiddenFrames;

                public CustomFrame GetFrame(string name)
                {
                    foreach (var v in FrameTransformation)
                        if (v.Key.Name == name) return v.Key;
                    return null;
                }

                public void HideFrame(string name, Model9 model)
                {
                    model.SkinnedMesh.ForEachFrame((frame) =>
                    {
                        if (frame.Name == name)
                        {
                            frame = null;
                        }
                    });
                }

                public BoundingBox? GetBoundingBox()
                {
                    BoundingBox? b = null;
                    foreach (var v in StoredFrameMatrices)
                    {
                        var mb = Common.Boundings.Transform(Common.Boundings.BoundingBoxFromXMesh(v.Key),
                            v.Value[0][0]);
                        if (b == null)
                            b = mb;
                        else
                            b = BoundingBox.Merge(b.Value, mb);
                    }
                    return b;
                }

#if DEBUG_ANIMATIONS
                int i = 0;
#endif

                public void Update(Model9 model, float dtime, Matrix world)
                {
                    if (model != null && model.SkinnedMesh != null)
                    {
#if PROFILE_ANIMATIONS
                        if (ResetMatricesStart != null)
                            ResetMatricesStart();
#endif
                        ((CustomFrame)model.SkinnedMesh.RootFrame).ResetMatrices();
#if PROFILE_ANIMATIONS
                        if (ResetMatricesStop != null)
                            ResetMatricesStop();

                        if (SetupACStrat != null)
                            SetupACStrat();
#endif
                        if (TrackFadeTime[CurrentTrack] > 0)
                        {
#if DEBUG_ANIMATIONS
                            if (i % 15 == 0 && Entity.Name != "MainCharacter")
                            {
                                Console.WriteLine("Previous: " + (TrackFadeTime[CurrentTrack] / FadeTime));
                                Console.WriteLine("Current: " + (FadeTime - TrackFadeTime[CurrentTrack]) / FadeTime);
                            }
                            i++;
#endif

                            AnimationController.SetTrackWeight(previousTrack, TrackFadeTime[CurrentTrack] / FadeTime);
                            AnimationController.SetTrackWeight(CurrentTrack, (FadeTime - TrackFadeTime[CurrentTrack]) / FadeTime);
                            TrackFadeTime[CurrentTrack] -= dtime;
                        }
                        else
                        {
                            AnimationController.DisableTrack(previousTrack);
                            AnimationController.SetTrackSpeed(previousTrack, 0);
                            AnimationController.SetTrackWeight(previousTrack, 0);

                            AnimationController.SetTrackWeight(CurrentTrack, 1);
                        }

                        if (!TrackPaused[CurrentTrack] && !Looping[CurrentTrack])
                        {
                            if (TrackDurations[CurrentTrack] - dtime > 0)
                                TrackDurations[CurrentTrack] -= dtime;
                            else
                            {
                                TrackDurations[CurrentTrack] = 0;
                                AnimationController.SetTrackPosition(CurrentTrack, TrackDurations[CurrentTrack] - 0.01f);
                                AnimationController.SetTrackSpeed(CurrentTrack, 0);
                                TrackPaused[CurrentTrack] = true;
                                if (AnimationDone != null)
                                    AnimationDone(0);
                            }
                        }
#if PROFILE_ANIMATIONS
                        if (SetupACStop != null)
                            SetupACStop();

                        if (AdvanceTimeStart != null)
                            AdvanceTimeStart();
#endif
                        AnimationController.AdvanceTime(dtime, null);
#if PROFILE_ANIMATIONS
                        if (AdvanceTimeStop != null)
                            AdvanceTimeStop();

                        if (AdjustFrameMatricesStrat != null)
                            AdjustFrameMatricesStrat();
#endif
                        foreach (var v in FrameCustomValues)
                        {
#if DEBUG
                            if (v.Value.M22 == float.NaN || v.Value.M21 == float.NaN || v.Value.M23 == float.NaN || v.Value.M24 == float.NaN)
                                throw new Exception("Matrix contains NaN");
#endif
                            model.SkinnedMesh.ForEachFrame((f) =>
                            {
                                if (f.Name == v.Key)
                                    f.TransformationMatrix *= v.Value;
                            });
                        }
#if PROFILE_ANIMATIONS
                        if (AdjustFrameMatricesStop != null)
                            AdjustFrameMatricesStop();

                        if (UpdateMatricesStart != null)
                            UpdateMatricesStart();
#endif
                        model.SkinnedMesh.UpdateFrameMatrices(model.SkinnedMesh.RootFrame, world);
#if PROFILE_ANIMATIONS
                        if (UpdateMatricesStop != null)
                            UpdateMatricesStop();

                        if (StoreMatricesStart != null)
                            StoreMatricesStart();
#endif
                        foreach (var frame in SkinnedMesh.Frames)
                        {
                            if (frame.Name == "sword1" || frame.Name == "rifle" || frame.Name == "joint3")
                                FrameTransformation[frame] = frame.CombinedTransform;
                        }
                        foreach (Common.Tuple<CustomFrame, CustomMeshContainer> meshContainer in SkinnedMesh.MeshContainers)
                        {
                            string name = meshContainer.First.Name;

                            if (meshContainer.Second != null)
                            {
                                if (meshContainer.Second.SkinInfo != null)
                                {
                                    BoneCombination[] combinations = meshContainer.Second.BoneCombinations;

                                    Matrix[][] combinationMatrices = new Matrix[combinations.Length][];

                                    for (int i = 0; i < combinations.Length; i++)
                                    {
                                        Matrix[] boneMatrices = new Matrix[meshContainer.Second.PaletteEntries];

                                        for (int pe = 0; pe < meshContainer.Second.PaletteEntries; pe++)
                                        {
                                            int index = combinations[i].BoneIds[pe];
                                            if (index != -1)
                                            {
                                                boneMatrices[pe] = meshContainer.Second.BoneOffsets[index] *
                                                    meshContainer.Second.BoneMatricesLookup[index].CombinedTransform;
                                            }
                                        }

                                        combinationMatrices[i] = boneMatrices;
                                    }

                                    StoredFrameMatrices[meshContainer.Second.MeshData.Mesh] = combinationMatrices;

                                }
                                else
                                {
                                    Matrix[][] combinedTransform = new Matrix[1][];

                                    combinedTransform[0] = new Matrix[] { meshContainer.First.CombinedTransform };

                                    StoredFrameMatrices[meshContainer.Second.MeshData.Mesh] = combinedTransform;
                                }
                            }
                        }
#if PROFILE_ANIMATIONS
                        if (StoreMatricesStop != null)
                            StoreMatricesStop();
#endif
                        //model.SkinnedMesh.ForEachFrame((frame) =>
                        //{
                        //    FrameTransformation[frame] = frame.CombinedTransform;

                        //    frame.ForEachMeshContainer((mc) =>
                        //    {
                        //        if (mc.SkinInfo != null)
                        //        {
                        //            BoneCombination[] combinations = mc.BoneCombinations;

                        //            Common.Tuple<Matrix[], Matrix[]>[] combinationMatrices =
                        //                new Common.Tuple<Matrix[], Matrix[]>[combinations.Length];

                        //            for (int i = 0; i < combinations.Length; i++)
                        //            {
                        //                Matrix[] boneMatrices = new Matrix[mc.PaletteEntries];
                        //                Matrix[] normalMatrices = new Matrix[mc.PaletteEntries];

                        //                for (int pe = 0; pe < mc.PaletteEntries; pe++)
                        //                {
                        //                    int index = combinations[i].BoneIds[pe];
                        //                    if (index != -1)
                        //                    {
                        //                        boneMatrices[pe] = mc.BoneOffsets[index] *
                        //                            mc.BoneMatricesLookup[index].CombinedTransform;
                        //                        normalMatrices[pe] = SlimDX.Matrix.Transpose(
                        //                            SlimDX.Matrix.Invert(boneMatrices[pe]));
                        //                    }
                        //                }

                        //                combinationMatrices[i] = new Common.Tuple<Matrix[], Matrix[]>(
                        //                    boneMatrices, normalMatrices);
                        //            }

                        //            StoredFrameMatrices[mc.MeshData.Mesh] = combinationMatrices;

                        //        }
                        //        else
                        //        {
                        //            Common.Tuple<Matrix[], Matrix[]>[] combinedTransform =
                        //                new Common.Tuple<Matrix[], Matrix[]>[1];

                        //            combinedTransform[0] = new Common.Tuple<Matrix[], Matrix[]>(
                        //                new Matrix[1] { frame.CombinedTransform },
                        //                new Matrix[1] { Matrix.Transpose(Matrix.Invert(frame.CombinedTransform)) });

                        //            StoredFrameMatrices[mc.MeshData.Mesh] = combinedTransform;
                        //        }
                        //    });
                        //});
                    }
                }

                public void FreezeAtEnd()
                {
                    TrackDurations[CurrentTrack] = 0;
                    TrackFadeTime[CurrentTrack] = 0;
                    var constructedModel = Content.Peek<Model9>(Entity.AllGraphics.First());
                    Update(constructedModel, 0, Matrix.Identity);
                }

                public void SetTrackAnimationSpeed(float speed)
                {
                    AnimationController.SetTrackSpeed(CurrentTrack, speed);
                    float scale = speed / TrackSpeed[CurrentTrack];
                    TrackDurations[CurrentTrack] /= scale;
                    TrackSpeed[CurrentTrack] = speed;
                }

                public void PlayAnimation(AnimationPlayParameters parms)
                {
#if DEBUG_ANIMATIONS
                    if (Entity.Name != "MainCharacter")
                        Console.WriteLine("----------------------------------\nNew Animation: " + parms.Animation + "\nFadetime: " + parms.FadeTime);
#endif
                    var constructedModel = Content.Peek<Model9>(Entity.AllGraphics.First());

                    if (parms.TimeType == AnimationTimeType.Length)
                    {
                        if (constructedModel.SkinnedMesh.Animations.ContainsKey(parms.Animation))
                        {
                            using (var animSet = AnimationController.GetAnimationSet<AnimationSet>(parms.Animation))
                                parms.Time = (float)animSet.Period / parms.Time;
                        }
                    }

                    if (!constructedModel.SkinnedMesh.Animations.ContainsKey(parms.Animation))
                        throw new ArgumentException("No such animation for this skinmesh: " + parms.Animation);

                    CurrentTrack = (CurrentTrack + 1) % 2;
                    previousTrack = (previousTrack + 1) % 2;

                    PlayingAnimations[CurrentTrack] = parms.Animation;
                    TrackPaused[CurrentTrack] = false;
                    Looping[CurrentTrack] = parms.Loop;
                    TrackDurations[CurrentTrack] = currentTrackDuration = constructedModel.SkinnedMesh.Animations[parms.Animation] / parms.Time;
                    TrackFadeTime[CurrentTrack] = FadeTime = parms.FadeTime;
                    TrackSpeed[CurrentTrack] = parms.Time;

                    //AnimationController.SetTrackSpeed(previousTrack, 0);
                    AnimationController.SetTrackSpeed(CurrentTrack, parms.Time);
                    AnimationController.EnableTrack(CurrentTrack);
                    AnimationController.SetTrackPriority(CurrentTrack, TrackPriority.Low);
                    AnimationController.SetTrackWeight(CurrentTrack, 0);
                    using (var animSet = AnimationController.GetAnimationSet<AnimationSet>(parms.Animation))
                        AnimationController.SetTrackAnimationSet(CurrentTrack, animSet);
                    AnimationController.SetTrackPosition(CurrentTrack,
                        parms.StartTime * constructedModel.SkinnedMesh.Animations[parms.Animation]);
                }

                public void InitializeAnimation()
                {
                    var metaModel = (MetaModel)Entity.AllGraphics.First();
                    if (metaModel.SkinnedMesh != null)
                    {
                        SkinnedMesh = Content.Acquire<SkinnedMesh>(metaModel.SkinnedMesh);

                        if (SkinnedMesh.AnimationController != null)
                        {
                            AnimationController = SkinnedMesh.AnimationController.Clone(
                                SkinnedMesh.AnimationController.MaxAnimationOutputs,
                                SkinnedMesh.AnimationController.MaxAnimationSets,
                                SkinnedMesh.AnimationController.MaxTracks,
                                SkinnedMesh.AnimationController.MaxEvents);
                        }
                    }
                    var constructedModel = Content.Peek<Model9>(metaModel);
                    Update(constructedModel, 0, Matrix.Identity);
                }

                public String GetInformation()
                {
                    var td = AnimationController.GetTrackDescription(CurrentTrack);
                    var tas = AnimationController.GetTrackAnimationSet(CurrentTrack);
                    if (td == null || tas == null) return " - ";
                    return tas.Name + ", Enabled:" +
                        td.Enabled + ", [" + td.Position.ToString("##.0") + "/" + tas.Period.ToString("##.0") + "] " + td.Speed;
                }

                public void Dispose()
                {
                    if (AnimationController != null)
                        AnimationController.Dispose();
                    if (SkinnedMesh != null)
                        Content.Release(SkinnedMesh);
                }

                public float FadeTime = 0.2f;
                private int previousTrack = 0;
                private bool wasFading = false;

                private static int maxTracks = 2;

                public int CurrentTrack = 1;
                public Content.ContentPool Content;
                public Entity Entity;
                public AnimationController AnimationController;
                public SkinnedMesh SkinnedMesh;
                float currentTrackDuration;
                public float[] TrackSpeed = new float[maxTracks];
                public float[] TrackFadeTime = new float[maxTracks];
                public float[] TrackDurations = new float[maxTracks];
                public bool[] Looping = new bool[maxTracks];
                public string[] PlayingAnimations = new string[maxTracks];
                public bool[] TrackPaused = new bool[maxTracks];
                public String PlayingAnimation { get { return PlayingAnimations[CurrentTrack] ?? ""; } }
                public bool Paused { get { return TrackPaused[CurrentTrack]; } }
            }

            public class MetaEntityAnimation : MetaResourceBase
            {
                public Content.DataLink<Entity> Entity { get; set; }

                public class Mapper : MetaMapper<MetaEntityAnimation, EntityAnimation>
                {
                    public override EntityAnimation Construct(MetaEntityAnimation metaResource, ContentPool content)
                    {
                        return new EntityAnimation(metaResource.Entity.Data, content);
                    }
                }

                public MetaEntityAnimation() { }
                public MetaEntityAnimation(MetaEntityAnimation copy)
                {
                    Entity = (Content.DataLink<Entity>)copy.Entity.Clone();
                }
                public override object Clone()
                {
                    return new MetaEntityAnimation(this);
                }

                public override bool Equals(object obj)
                {
                    var o = obj as MetaEntityAnimation;
                    if (o == null) return false;
                    return
                        Object.Equals(Entity, o.Entity);
                }

                public override int GetHashCode()
                {
                    return GetType().GetHashCode() ^ Entity.GetHashCode();
                }

                public override string ToString()
                {
                    return GetType().Name + "." + Entity.GetHashCode();
                }

            }
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class AnimationPlayParameters
    {
        public string Animation { get; set; }
        public bool Loop { get; set; }
        public float Time { get; set; }
        public AnimationTimeType TimeType { get; set; }
        public float StartTime { get; set; }
        public float FadeTime { get; set; }

        public AnimationPlayParameters()
        {
            Loop = true;
            Time = 1;
            TimeType = AnimationTimeType.Speed;
            StartTime = 0;
            FadeTime = 0.2f;
        }

        public AnimationPlayParameters(string animation)
            : this()
        {
            Animation = animation;
        }

        public AnimationPlayParameters(string animation, bool loop)
            : this()
        {
            Animation = animation;
            Loop = loop;
        }

        public AnimationPlayParameters(string animation, bool loop, float time, AnimationTimeType timeType)
            : this()
        {
            Animation = animation;
            Loop = loop;
            Time = time;
            TimeType = timeType;
        }

        public AnimationPlayParameters(string animation, bool loop, float time,
            AnimationTimeType timeType, float animationStart)
        {
            Animation = animation;
            Loop = loop;
            Time = time;
            TimeType = timeType;
            StartTime = animationStart;
            FadeTime = 0.2f;
        }
    }
}