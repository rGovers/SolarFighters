using IcarianEngine.Definitions;

namespace LD54
{
    public class PlayerControllerDef : ComponentDef
    {
        public float MovementSpeed = 5.0f;
        public float ShootRecharge = 0.1f;

        public GameObjectDef BulletDef;

        public PlayerControllerDef()
        {
            ComponentType = typeof(PlayerController);
        }
    }
}