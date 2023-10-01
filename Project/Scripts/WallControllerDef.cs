using IcarianEngine.Definitions;

namespace LD54
{
    public class WallControllerDef : ComponentDef
    {
        public GameObjectDef UpperWallDef;
        public float UpperWallDistance = 10.0f;
        public GameObjectDef SideWallDef;
        public float SideWallDistance = 10.0f;

        public float WallAcceleration = 0.1f;

        public WallControllerDef()
        {
            ComponentType = typeof(WallController);
        }
    }
}