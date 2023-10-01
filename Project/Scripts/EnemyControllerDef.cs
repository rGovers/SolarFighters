using IcarianEngine.Definitions;

namespace LD54
{
    public class EnemyControllerDef : ComponentDef
    {
        public float TurnSpeed = 1.0f;

        public float MoveForce = 2000.0f;
        public float FollowDistance = 10.0f;

        public float KickForce = 2.0f;

        public float ShootCooldown = 1.0f;

        public GameObjectDef BulletDef;

        public EnemyControllerDef()
        {
            ComponentType = typeof(EnemyController);
        }
    }
}