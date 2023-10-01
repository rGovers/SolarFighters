using IcarianEngine;
using IcarianEngine.Maths;
using IcarianEngine.Physics;

namespace LD54
{
    public class BulletController : Scriptable
    {
        public BulletControllerDef BulletControllerDef
        {
            get
            {
                return Def as BulletControllerDef;
            }
        }

        float       m_lifeTime = 0.0f;

        GameObject  m_spawnedBy;

        TriggerBody m_triggerBody;

        bool        m_canPushWalls = false;

        public GameObject SpawnedBy
        {
            get
            {
                return m_spawnedBy;
            }
            set
            {
                m_spawnedBy = value;
            }
        }

        public bool CanPushWalls
        {
            get
            {
                return m_canPushWalls;
            }
            set
            {
                m_canPushWalls = value;
            }
        }

        void OnTriggerStart(PhysicsBody a_other)
        {
            if (a_other.GameObject == m_spawnedBy)
            {
                return;
            }

            Health health = a_other.GetComponent<Health>();
            if (health != null)
            {
                HealthDef healthDef = health.HealthDef;
                health.CurrentHealth -= BulletControllerDef.BulletDamage;

                if (m_canPushWalls)
                {
                    LD54AssemblyControl.PushWalls(healthDef.WallPushback);
                }

                uint count = Random.GetRandomUInt(1, 5);
                for (uint i = 0; i < count; ++i)
                {
                    GameObject particleObj = GameObject.FromDef(GameObjectDefTable.Base_GameObject_Particle);
                    particleObj.Transform.Translation = Transform.Translation;
                    particleObj.Transform.Rotation = Quaternion.FromEuler(new Vector3(
                        Random.GetRandomFloat() * Mathf.PI,
                        Random.GetRandomFloat() * Mathf.PI,
                        Random.GetRandomFloat() * Mathf.PI)
                    );

                    Vector3 offset = new Vector3(
                        Random.GetRandomFloat() * 0.5f,
                        Random.GetRandomFloat() * 0.5f,
                        Random.GetRandomFloat() * 0.5f);

                    Particle particle = particleObj.GetComponent<Particle>();
                    particle.Velocity = -Transform.Forward + offset * 5.0f;
                }

                m_lifeTime = BulletControllerDef.BulletLifeTime;
            }
        }

        public override void Init()
        {
            m_triggerBody = GameObject.GetComponent<TriggerBody>();
            if (m_triggerBody == null)
            {
                Logger.Error("BulletController requires a TriggerBody component");

                return;
            }

            m_triggerBody.OnTriggerStartCallback += OnTriggerStart;
        }

        public override void Update()
        {
            BulletControllerDef def = BulletControllerDef;

            Vector3 pos = Transform.Translation + Transform.Forward * def.BulletSpeed * Time.DeltaTime;

            // Transform.Translation += Transform.Forward * -def.BulletSpeed * Time.DeltaTime;
            m_triggerBody.SetPosition(pos);

            m_lifeTime += Time.DeltaTime;

            if (m_lifeTime > def.BulletLifeTime)
            {
                GameObject.Dispose();
            }
        }
    }
}