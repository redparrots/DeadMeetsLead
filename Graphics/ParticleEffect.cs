#if BETA_RELEASE
//#define PROFILE_PARTICLESYSTEM
//#define DEBUG_PARTICLESYSTEM
#endif

#define USE_NEW_GRAPHICS_SYSTEM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics.Content;

namespace Graphics
{
    [Serializable]
    public class ParticleEffect : Graphics.Entity
    {

#if PROFILE_PARTICLESYSTEM

        public static event Action UpdateStart;
        public static event Action UpdateEnd;
        public static event Action AddStart;
        public static event Action AddEnd;
        public static event Action LogicStart;
        public static event Action LogicEnd;
        public static event Action CleanUpStart;
        public static event Action CleanUpEnd;
        public static event Action RandomStart;
        public static event Action RandomEnd;
        public static event Action CleanerStart;
        public static event Action CleanerEnd;
        public static event Action RemoveCheckStart;
        public static event Action RemoveCheckEnd;
        public static event Action MathStart;
        public static event Action MathEnd;
        public static event Action InterpolateStart;
        public static event Action InterpolateEnd;
        public static event Action ParticleUpdateWorldStart;
        public static event Action ParticleUpdateWorldEnd;
        public static event Action RemoveStart;
        public static event Action RemoveEnd;

#endif

        public ParticleEffect()
        {
            MinScale = MaxScale = 1;
            Updateable = true;
        }

        #region ParticleConfiguration

        private MetaModel particleModel;
        public MetaModel ParticleModel { get { return particleModel; } set { particleModel = value; } }

        private bool particleStay = true;
        public bool ParticleStay { get { return particleStay; } set { particleStay = value; } }

        private float particleFadeInTime;
        public float ParticleFadeInTime { get { return particleFadeInTime; } set { particleFadeInTime = value; } }

        private float particleFadeOutTime;
        public float ParticleFadeOutTime { get { return particleFadeOutTime; } set { particleFadeOutTime = value; } }

        private float initialScalingFactor;
        public float InitialScalingFactor { get { return initialScalingFactor; } set { initialScalingFactor = value; } }

        private Vector3 direction;
        public Vector3 Direction { get { return direction; } set { direction = value; } }

        private float horizontalSpreadAngle;
        /// <summary>
        /// Takes values between 0 -> 2*PI
        /// Normally you want this to be 2*PI for the spread to be in a "capped hemisphere"
        /// </summary>
        public float HorizontalSpreadAngle { get { return horizontalSpreadAngle; } set { horizontalSpreadAngle = value; } }

        private float verticalSpreadAngle;
        /// <summary>
        /// Takes values between 0 -> PI
        /// </summary>
        public float VerticalSpreadAngle { get { return verticalSpreadAngle; } set { verticalSpreadAngle = value; } }

        private float speedMin;
        public float SpeedMin { get { return speedMin; } set { speedMin = value; } }

        private float speedMax;
        public float SpeedMax { get { return speedMax; } set { speedMax = value; } }

        private float scaleSpeed;
        public float ScaleSpeed { get { return scaleSpeed; } set { scaleSpeed = value; } }

        private float minScale;
        public float MinScale { get { return minScale; } set { minScale = value; } }

        private float maxScale;
        public float MaxScale { get { return maxScale; } set { maxScale = value; } }

        private float timeToLiveMin;
        public float TimeToLiveMin { get { return timeToLiveMin; } set { timeToLiveMin = value; } }

        private float timeToLiveMax;
        public float TimeToLiveMax { get { return timeToLiveMax; } set { timeToLiveMax = value; } }

        private float accelerationMin;
        public float AccelerationMin { get { return accelerationMin; } set { accelerationMin = value; } }

        private float accelerationMax;
        public float AccelerationMax { get { return accelerationMax; } set { accelerationMax = value; } }

        private Vector3 acceleration;
        public Vector3 Acceleration { get { return acceleration; } set { acceleration = value; } }

        private int randomSeed;
        public int RandomSeed { get { return randomSeed; } set { randomSeed = value; } }

        private float timeToLive;
        public float TimeToLive { get { return timeToLive; } set { timeToLive = value; } }

        private bool randomScaling;
        public bool RandomScaling { get { return randomScaling; } set { randomScaling = value; } }

        private bool randomRotation;
        public bool RandomRotation
        {
            get
            {
                return randomRotation;
            }
            set
            {
                randomRotation = value;
                if(randomRotation)
                    throw new NotImplementedException("Random rotation is not available for particles");
            }
        }

        #endregion

        #region EffectConfiguration

        private float effectDuration;
        public float EffectDuration { get { return effectDuration; } set { effectDuration = value; } }

        private bool forever;
        public bool Forever { get { return forever; } set { forever = value; } }

        private bool withLength;
        public bool WithLength { get { return withLength; } set { withLength = value; } }

        private int spawnsPerSecond;
        public int SpawnsPerSecond { get { return spawnsPerSecond; } set { spawnsPerSecond = value; } }

        private bool instant;
        public bool Instant { get { return instant; } set { instant = value; } }

        private int count;
        public int Count { get { return count; } set { count = value; } }

        #endregion

        #region Internal

        private int stillAlive;
        public int StillAlive { get { return stillAlive; } set { stillAlive = value; } }

        private float timeElapsed;
        public float TimeElapsed { get { return timeElapsed; } set { timeElapsed = value; } }

        private float accDtime;

        #endregion

        public void Stop()
        {
            End();
        }

        private bool end = false;
        private void End()
        {
            end = true;
        }

        List<Particle> particles = new List<Particle>();

#if DEBUG_PARTICLESYSTEM

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if (ParticleFadeInTime + ParticleFadeOutTime > TimeToLive)
                throw new ArgumentException("Time spent in fadeIn and fadeOut is longer than the duration of the particle");
        }
        
#endif

        private void SpawnGraphicParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Particle p;
                MetaModel m = (MetaModel)ParticleModel.Clone();
                particles.Add(p = new Particle
                {
                    MainGraphic = m,
                    Acceleration = Acceleration,
                    Direction = Direction,
                    TimeElapsed = 0,
                    InitialTranslation = ParticleStay ? CombinedWorldMatrix : Matrix.Identity,
                    InitialModelMatrix = m.World
                });
                m.OrientationRelation = ModelOrientationRelation.Absolute;
                m.Opacity = 0;
                p.RandomNumbers = new float[] { (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 
                                                (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble() };
                AddGraphic(m);
            }
        }

        private void SpawnEntityParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ParticleEntity p;

                AddChild(p = new Graphics.ParticleEntity
                {
                    MainGraphic = (MetaModel)ParticleModel.Clone(),
                    VisibilityLocalBounding = Vector3.Zero,
                    TimeElapsed = 0.0f,
                    Direction = Direction,
                    Acceleration = Acceleration,
                    InitialTranslation = ParticleStay ? CombinedWorldMatrix : Matrix.Identity,
                    OrientationRelation = ParticleStay ? OrientationRelation.Absolute : OrientationRelation.Relative,
                });
                p.RandomNumbers = new float[] { (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 
                                                (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble() };
                ((MetaModel)p.MainGraphic).Opacity = 0;
            }
        }
        static Random random = new Random();

        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            
            //A possible optimization would be to break here if the particle effect is not within the frustum
            //if(ActiveInMain == CurrenFrame)
            //  Do the below
            //else
            //  Do nothing

#if PROFILE_PARTICLESYSTEM
            if (UpdateStart != null)
                UpdateStart();
#endif

            TimeElapsed += e.Dtime;
            accDtime += e.Dtime;

            if (WithLength && TimeElapsed > EffectDuration)
                End();

#if PROFILE_PARTICLESYSTEM
            if (AddStart != null)
                AddStart();
#endif
            if (Instant)
            {
#if USE_NEW_GRAPHICS_SYSTEM
                if (particles.Count != Count)
                {
                    StillAlive = Count;
                    SpawnGraphicParticles(Count);
                }
#else
                if (Children.Count() != Count)
                {
                    StillAlive = Count;
                    ClearChildren();
                    SpawnEntityParticles(Count);
                }
#endif
            }
            else if (!end)
            {
                int spawns = (int)(accDtime * (float)SpawnsPerSecond);

                if (spawns > 0)
                    accDtime = 0;

#if USE_NEW_GRAPHICS_SYSTEM
                SpawnGraphicParticles(spawns);
#else
                SpawnEntityParticles(spawns);
#endif
                StillAlive += spawns;
            }

#if PROFILE_PARTICLESYSTEM
            if (AddEnd != null)
                AddEnd();

            if (LogicStart != null)
                LogicStart();
#endif
            
#if USE_NEW_GRAPHICS_SYSTEM
            foreach(Particle particle in particles)
#else
            foreach (ParticleEntity particle in Children)
#endif
            {
                particle.TimeElapsed += e.Dtime;
                if (particle.TimeElapsed > TimeToLive)
                {
                    particlesToBeRemoved.Add(particle);
                    StillAlive--;
                }

#if PROFILE_PARTICLESYSTEM
                if (InterpolateStart != null)
                    InterpolateStart();
#endif

                if (particle.TimeElapsed <= ParticleFadeInTime)
                    ((Graphics.Content.MetaModel)particle.MainGraphic).Opacity = particle.TimeElapsed / particleFadeInTime;
                else if (particle.TimeElapsed >= timeToLive - particleFadeOutTime)
                    ((Graphics.Content.MetaModel)particle.MainGraphic).Opacity = (timeToLive - particle.TimeElapsed) / particleFadeOutTime;
                else
                    ((Graphics.Content.MetaModel)particle.MainGraphic).Opacity = 1f;

#if PROFILE_PARTICLESYSTEM
                if(InterpolateEnd != null)
                    InterpolateEnd();
#endif
                Direction = Vector3.Normalize(particle.Direction);

                float angle = particle.RandomNumbers[0] * HorizontalSpreadAngle;
                float vAngle = particle.RandomNumbers[1] * VerticalSpreadAngle;

                var spreadAngle = Common.Math.SphericalToCartesianCoordinates(vAngle, angle);

                Matrix directionMatrix = Common.Math.MatrixFromNormal(particle.Direction);

                Vector3 newDirection = Vector3.Normalize(Vector3.TransformNormal(spreadAngle, directionMatrix));

                float speed = (speedMin + (SpeedMax - SpeedMin) * particle.RandomNumbers[2]);

                Vector3 initialVelocityForThisParticle = newDirection * speed;

                Matrix translationMatrix;
                Matrix scalingMatrix;
                //Matrix rotationMatrix;

                translationMatrix = Matrix.Translation(initialVelocityForThisParticle * particle.TimeElapsed + new Vector3(
                    ((AccelerationMin + (AccelerationMax - AccelerationMin) * particle.RandomNumbers[3]) * particle.Acceleration.X) * particle.TimeElapsed * particle.TimeElapsed,
                    ((AccelerationMin + (AccelerationMax - AccelerationMin) * particle.RandomNumbers[4]) * particle.Acceleration.Y) * particle.TimeElapsed * particle.TimeElapsed,
                    ((AccelerationMin + (AccelerationMax - AccelerationMin) * particle.RandomNumbers[5]) * particle.Acceleration.Z) * particle.TimeElapsed * particle.TimeElapsed)
                    );

                if (RandomScaling)
                {
                    float randomScaleFactor = MinScale + (MaxScale - MinScale) * particle.RandomNumbers[6];
                    scalingMatrix = Matrix.Scaling((particle.TimeElapsed * ScaleSpeed) * randomScaleFactor + 1.0f * InitialScalingFactor * (randomScaleFactor * 0.3f + 0.85f),
                                                   (particle.TimeElapsed * ScaleSpeed) * randomScaleFactor + 1.0f * InitialScalingFactor * (randomScaleFactor * 0.3f + 0.85f),
                                                   1.0f);
                }
                else
                {
                    scalingMatrix = Matrix.Scaling((particle.TimeElapsed * ScaleSpeed) + 1.0f * InitialScalingFactor,
                                                   (particle.TimeElapsed * ScaleSpeed) + 1.0f * InitialScalingFactor,
                                                   1.0f);
                }

                //if (RandomRotation)
                //{
                //    Vector3 billBoardDirection = -(((LookatCamera)Scene.Camera).Lookat) - Scene.Camera.Position;
                //    rotationMatrix = Matrix.RotationAxis(billBoardDirection, (float)(Math.PI * 2.0d * r.NextDouble()));
                //    throw new NotImplementedException();
                //}
                //else
                //{
                //    rotationMatrix = Matrix.Identity;
                //}

#if PROFILE_PARTICLESYSTEM
                if (ParticleUpdateWorldStart != null)
                    ParticleUpdateWorldStart();
#endif
                //v.WorldMatrix = scalingMatrix * rotationMatrix * translationMatrix * v.InitialTranslation;
#if USE_NEW_GRAPHICS_SYSTEM
                particle.MainGraphic.World = particle.InitialModelMatrix * scalingMatrix * translationMatrix * particle.InitialTranslation;
#else
                particle.WorldMatrix = scalingMatrix * translationMatrix * particle.InitialTranslation;
#endif
#if PROFILE_PARTICLESYSTEM
                if (ParticleUpdateWorldEnd != null)
                    ParticleUpdateWorldEnd();
#endif
            }

#if PROFILE_PARTICLESYSTEM

            if (LogicEnd != null)
                LogicEnd();

            if (RemoveStart != null)
                RemoveStart();
#endif
            if (StillAlive == 0 && (Instant || end))
                remove = true;

#if USE_NEW_GRAPHICS_SYSTEM
            foreach (Particle p in particlesToBeRemoved)
            {
                RemoveGraphic(p.MainGraphic);
                particles.Remove(p);
            }
#else
            foreach (Graphics.Entity entity in particlesToBeRemoved)
                entity.Remove();
#endif

            particlesToBeRemoved.Clear();

            if (remove)
            {
                if (!IsRemoved)
                    Remove();
            }
#if PROFILE_PARTICLESYSTEM

            if (RemoveEnd != null)
                RemoveEnd();

            if (UpdateEnd != null)
                UpdateEnd();
#endif
        }

        private bool remove = false;
#if USE_NEW_GRAPHICS_SYSTEM
        List<Particle> particlesToBeRemoved = new List<Particle>();
#else
        List<Graphics.Entity> particlesToBeRemoved = new List<Graphics.Entity>();
#endif
        
    }
}
