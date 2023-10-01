using IcarianEngine.Definitions;

namespace LD54
{
    public class EnemySpawnerDef : ComponentDef
    {
        public GameObjectDef EnemyDef;
        public float SpawnInterval = 10.0f;
        public float SpawnStartDelay = 5.0f;
        public float SpawnDegradation = 0.1f;

        public EnemySpawnerDef()
        {
            ComponentType = typeof(EnemySpawner);
        }
    }
}