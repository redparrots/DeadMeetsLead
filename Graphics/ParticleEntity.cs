using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics
{
    public class ParticleEntity : Entity
    {
        private float[] randomNumbers;
        public float[] RandomNumbers { get { return randomNumbers; } set { randomNumbers = value; } }

        private float timeElapsed;
        public float TimeElapsed { get { return timeElapsed; } set { timeElapsed = value; } }

        //private int seed;
        //public int Seed { get { return seed; } set { seed = value; } }

        private Vector3 direction;
        public Vector3 Direction { get { return direction; } set { direction = value; } }

        private Vector3 acceleration;
        public Vector3 Acceleration { get { return acceleration; } set { acceleration = value; } }

        private Matrix initialTranslation;
        public Matrix InitialTranslation
        {
            get { return initialTranslation; }
            set { initialTranslation = value; }
        }
    }
}
