using IcarianEngine.Definitions;

namespace LD54
{
    public class BulletControllerDef : ComponentDef
    {
        public float BulletSpeed = 50.0f;
        public float BulletLifeTime = 5.0f;
        public float BulletDamage = 10.0f;

        public BulletControllerDef()
        {
            ComponentType = typeof(BulletController);
        }
    }
}