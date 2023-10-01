using IcarianEngine;
using IcarianEngine.Maths;

namespace LD54
{
    public class ObstacleSpawner : Scriptable
    {
        float m_timer = 0.0f;
        float m_spawnTime = 0.0f;

        public ObstacleSpawnerDef ObstacleSpawnerDef
        {
            get
            {
                return Def as ObstacleSpawnerDef;
            }
        }

        public override void Init()
        {
            ObstacleSpawnerDef def = ObstacleSpawnerDef;
            if (def == null)
            {
                Logger.Error("ObstacleSpawnerDef is null");

                return;
            }

            m_spawnTime = def.SpawnInterval;
            m_timer = m_spawnTime;
        }

        public override void Update()
        {
            ObstacleSpawnerDef def = ObstacleSpawnerDef;

            if (m_timer < 0)
            {
                float angle = Random.GetRandomFloat(0.0f, Mathf.TwoPI);

                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector3 pos = Transform.Translation + new Vector3(dir.X, 0.0f, dir.Y) * def.SpawnRadius;

                GameObject obstacleObject = GameObject.FromDef(def.ObstacleDef);
                obstacleObject.Transform.Translation = pos;

                m_timer = m_spawnTime;
                m_spawnTime -= def.SpawnDecay;
            }

            m_timer -= Time.DeltaTime;
        }
    }
}