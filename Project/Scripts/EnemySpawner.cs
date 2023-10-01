using IcarianEngine;
using IcarianEngine.Definitions;
using IcarianEngine.Maths;
using IcarianEngine.Physics;

namespace LD54
{
    public class EnemySpawner : Scriptable
    {
        public EnemySpawnerDef EnemySpawnerDef
        {
            get
            {
                return Def as EnemySpawnerDef;
            }
        }

        float m_spawnTimer = 0.0f;
        float m_spawnDelay = 0.0f;

        public override void Init()
        {
            EnemySpawnerDef def = EnemySpawnerDef;
            if (def == null)
            {
                Logger.Error("Failed to find enemy spawner definition");

                return;
            }

            m_spawnDelay = def.SpawnInterval;
            m_spawnTimer = def.SpawnStartDelay + m_spawnDelay;
        }

        public override void Update()
        {
            EnemySpawnerDef def = EnemySpawnerDef;

            m_spawnTimer -= Time.DeltaTime;

            if (m_spawnTimer < 0)
            {
                m_spawnTimer = m_spawnDelay;
                m_spawnDelay -= def.SpawnDegradation;

                GameObject enemy = GameObject.FromDef(def.EnemyDef);
                enemy.Transform.Scale = Vector3.Zero;

                PhysicsBody physicsBody = enemy.GetComponent<PhysicsBody>();
                if (physicsBody != null)
                {
                    physicsBody.SetPosition(Transform.Translation);
                }
                else
                {
                    enemy.Transform.Translation = Transform.Translation;
                }
            }
        }
    }
}