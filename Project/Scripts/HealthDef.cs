using IcarianEngine.Definitions;

namespace LD54
{
    public class HealthDef : ComponentDef
    {
        public float MaxHealth = 100.0f;

        public float WallPushback = 10.0f;

        public HealthDef()
        {
            ComponentType = typeof(Health);
        }
    }
}