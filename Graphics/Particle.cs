using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics
{
    public class Particle
    {
        private float timeElapsed;
        public float TimeElapsed { get { return timeElapsed; } set { timeElapsed = value; } }

        //private int seed;
        //public int Seed { get { return seed; } set { seed = value; } }

        private Vector3 direction;
        public Vector3 Direction { get { return direction; } set { direction = value; } }

        private Vector3 acceleration;
        public Vector3 Acceleration { get { return acceleration; } set { acceleration = value; } }

        //private bool fadeIn;
        //public bool FadeIn { get { return fadeIn; } set { fadeIn = value; } }

        private Matrix initialModelMatrix;
        public Matrix InitialModelMatrix
        {
            get { return initialModelMatrix; }
            set { initialModelMatrix = value; }
        }

        private Matrix initialTranslation;
        public Matrix InitialTranslation
        {
            get { return initialTranslation; }
            set { initialTranslation = value; }
        }

        //private Common.Interpolator interpolator;
        //public Common.Interpolator Interpolator { get { return interpolator; } set { interpolator = value; } }

        private Graphics.Content.MetaModel metaModel;
        public Graphics.Content.MetaModel MainGraphic { get { return metaModel; } set { metaModel = value; } }

        private float[] randomNumbers;
        public float[] RandomNumbers { get { return randomNumbers; } set { randomNumbers = value; } }
    }
}
