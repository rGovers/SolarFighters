using IcarianEngine;
using IcarianEngine.Maths;

namespace LD54
{
    // I do not have a Particle System so have to fake it with GameObjects
    // Luckily, I have enough CPU time to brute force it.... Hopefully....
    // C# is unpredictable when it comes to performance plus InterOp with Native code is slow
    // Cough Garbage Collector Cough
    // And yes my maths classes are not optimized
    public class Particle : Scriptable
    {
        Vector3 m_velocity;

        public Vector3 Velocity
        {
            get
            {
                return m_velocity;
            }
            set
            {
                m_velocity = value;
            }
        }

        public override void Init()
        {
            
        }

        public override void Update()
        {
            float deltaTime = Time.DeltaTime;

            m_velocity += Vector3.Down * 9.8f * deltaTime;

            Transform.Translation += m_velocity * deltaTime;
            Vector3 scale = Vector3.Lerp(Transform.Scale, Vector3.Zero, deltaTime * 2);
            Transform.Scale = scale;

            if (scale.X < 0.1f)
            {
                GameObject.Dispose();
            }
        }
    }
}