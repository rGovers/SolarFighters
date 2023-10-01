// using IcarianEngine.Unity;
// REALLY Copilot? REALLY? ^^^^
using IcarianEngine.Definitions;

namespace LD54
{
    public class ObstacleSpawnerDef : ComponentDef
    {
        // Copilot I am fucking not using Unity fucking stop it.
        // It is not a prefab it is a DEFINITION.
        // And I swear you trying using UnityEngine one more time I am canceling
        public GameObjectDef ObstacleDef;
        public float SpawnInterval = 10.0f;
        public float SpawnDecay = 0.1f;
        public float SpawnRadius = 5.0f;

        public ObstacleSpawnerDef()
        {
            ComponentType = typeof(ObstacleSpawner);
        }
    }
}