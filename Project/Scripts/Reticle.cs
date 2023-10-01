using IcarianEngine;
using IcarianEngine.Maths;
using IcarianEngine.Physics;

namespace LD54
{
    public class Reticle : Scriptable
    {
        float m_time = 0.0f;
        float m_duration = 2.0f;

        bool  m_spawned = false;

        public float Duration
        {
            get
            {
                return m_duration;
            }
            set
            {
                m_duration = value;
            }
        }

        public override void Init()
        {
            Transform.Scale = new Vector3(0.01f);
        }

        public override void Update()
        {
            Transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, Time.TimePassed);

            if (m_time > m_duration)
            {
                Vector3 position = Transform.Translation;

                if (!m_spawned)
                {
                    GameObject obj = GameObject.FromDef(GameObjectDefTable.Base_GameObject_Beam);

                    PhysicsBody body = obj.GetComponent<PhysicsBody>();
                    if (body != null)
                    {
                        body.SetPosition(position);
                    }
                    else
                    {
                        obj.Transform.Translation = position;
                    }

                    m_spawned = true;
                }

                Transform.Translation = new Vector3(position.X, position.Y + Time.DeltaTime, position.Z);
                Transform.Scale = Vector3.Lerp(Transform.Scale, Vector3.Zero, Time.DeltaTime);

                if (position.Y > 10.0f)
                {
                    GameObject.Dispose();
                }
            }
            else
            {
                Vector3 targetScale = Vector3.One + new Vector3(Mathf.Abs(Mathf.Sin(Time.TimePassed))) * 0.5f;

                m_time += Time.DeltaTime;

                Transform.Scale = Vector3.Lerp(Vector3.Zero, targetScale, m_time / m_duration);
            }
        }
    }
}