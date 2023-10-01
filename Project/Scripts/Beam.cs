using IcarianEngine;
using IcarianEngine.Maths;
using IcarianEngine.Physics;

namespace LD54
{
    public class Beam : Scriptable
    {
        float m_timer = 0.0f;
        float m_particleTimer = 0.0f;

        void OnTriggerStart(PhysicsBody a_other)
        {
            Health health = a_other.GameObject.GetComponent<Health>();

            if (health != null)
            {
                Logger.Message("Git gud scrub");

                health.CurrentHealth -= 10.0f;
            }
        }

        public override void Init()
        {
            TriggerBody triggerBody = GetComponent<TriggerBody>();
            triggerBody.OnTriggerStartCallback += OnTriggerStart;

            Transform.Scale = new Vector3(0.01f);   

            m_timer = 3.5f;
        }

        public override void Update()
        {
            if (m_timer < 0)
            {
                Vector3 scale = Vector3.Lerp(Transform.Scale, Vector3.Zero, Time.DeltaTime * 7.5f);

                Transform.Scale = scale;

                if (scale.X < 0.1f)
                {
                    GameObject.Dispose();
                }
            }
            else
            {
                Transform.Scale = Vector3.Lerp(Transform.Scale, Vector3.One, Time.DeltaTime * 7.5f);

                if (m_particleTimer < 0.0f)
                {
                    Vector3 position = Transform.Translation;
                    uint count = Random.GetRandomUInt(2, 10);
                    for (uint i = 0; i < count; ++i)
                    {
                        Vector3 dir = Vector3.Normalized(new Vector3
                        (
                            Random.GetRandomFloat(-1.0f, 1.0f),
                            0.0f,
                            Random.GetRandomFloat(-1.0f, 1.0f)
                        ));

                        GameObject particleObject = GameObject.FromDef(GameObjectDefTable.Base_GameObject_Particle);
                        particleObject.Transform.Translation = position + Vector3.Up * 2.0f;
                        Particle particle = particleObject.GetComponent<Particle>();
                        particle.Velocity = dir * Random.GetRandomFloat(5.0f, 10.0f);
                    }

                    m_particleTimer += 0.1f;
                }

                m_particleTimer -= Time.DeltaTime;
                m_timer -= Time.DeltaTime;
            }
        }
    }
}