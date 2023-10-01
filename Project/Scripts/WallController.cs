using IcarianEngine;
using IcarianEngine.Definitions;
using IcarianEngine.Maths;
using IcarianEngine.Physics;

namespace LD54
{
    public class WallController : Scriptable
    {
        public WallControllerDef WallControllerDef
        {
            get
            {
                return Def as WallControllerDef;
            }
        }

        GameObject m_upperWall;
        GameObject m_lowerWall;
        GameObject m_leftWall;
        GameObject m_rightWall;

        RigidBody  m_upperWallRigidBody;
        RigidBody  m_lowerWallRigidBody;
        RigidBody  m_leftWallRigidBody;
        RigidBody  m_rightWallRigidBody;

        float      m_wallVelocity = 0.0f;
        float      m_wallMovement = 0.0f;
        float      m_currentWallMovement = 0.0f;

        void OnCollisionStart(PhysicsBody a_other, CollisionData a_data)
        {
            PlayerController playerController = a_other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                LD54AssemblyControl.GameOver();
            }
        }

        public override void Init()
        {
            WallControllerDef def = WallControllerDef;
            if (def == null)
            {
                Logger.Error("Failed to find wall controller definition");

                return;
            }

            if (def.UpperWallDef == null)
            {
                Logger.Error("Failed to find upper wall definition");

                return;
            }

            m_upperWall = GameObject.FromDef(def.UpperWallDef);
            m_lowerWall = GameObject.FromDef(def.UpperWallDef);

            m_upperWallRigidBody = m_upperWall.GetComponent<RigidBody>();
            m_lowerWallRigidBody = m_lowerWall.GetComponent<RigidBody>();
            
            m_upperWallRigidBody.OnCollisionStartCallback += OnCollisionStart;
            m_lowerWallRigidBody.OnCollisionStartCallback += OnCollisionStart;

            m_upperWallRigidBody.SetPosition(Vector3.UnitZ * def.UpperWallDistance);
            m_lowerWallRigidBody.SetPosition(Vector3.UnitZ * -def.UpperWallDistance);

            if (def.SideWallDef == null)
            {
                Logger.Error("Failed to find side wall definition");

                return;
            }

            m_leftWall = GameObject.FromDef(def.SideWallDef);
            m_rightWall = GameObject.FromDef(def.SideWallDef);
            

            m_leftWallRigidBody = m_leftWall.GetComponent<RigidBody>();
            m_rightWallRigidBody = m_rightWall.GetComponent<RigidBody>();

            m_leftWallRigidBody.OnCollisionStartCallback += OnCollisionStart;
            m_rightWallRigidBody.OnCollisionStartCallback += OnCollisionStart;

            m_leftWallRigidBody.SetPosition(Vector3.UnitX * -def.SideWallDistance);
            m_rightWallRigidBody.SetPosition(Vector3.UnitX * def.SideWallDistance);

            LD54AssemblyControl.AddFlushObject(m_upperWall);
            LD54AssemblyControl.AddFlushObject(m_lowerWall);
            LD54AssemblyControl.AddFlushObject(m_leftWall);
            LD54AssemblyControl.AddFlushObject(m_rightWall);
        }

        public override void Update()
        {
            WallControllerDef def = WallControllerDef;

            m_currentWallMovement = Mathf.Lerp(m_currentWallMovement, m_wallMovement, Time.DeltaTime * 0.5f);

            Vector3 upperPos = Vector3.UnitZ * (def.UpperWallDistance - m_currentWallMovement);

            m_upperWallRigidBody.SetPosition(upperPos);
            m_lowerWallRigidBody.SetPosition(-upperPos);

            m_currentWallMovement = Mathf.Lerp(m_currentWallMovement, m_wallMovement, Time.DeltaTime * 0.5f);

            Vector3 rightPos = Vector3.UnitX * (def.SideWallDistance - m_currentWallMovement);

            m_leftWallRigidBody.SetPosition(-rightPos);
            m_rightWallRigidBody.SetPosition(rightPos);

            m_upperWallRigidBody.SetRotation(Quaternion.Identity);
            m_lowerWallRigidBody.SetRotation(Quaternion.Identity);
            m_leftWallRigidBody.SetRotation(Quaternion.Identity);
            m_rightWallRigidBody.SetRotation(Quaternion.Identity);

            m_wallMovement += m_wallVelocity * Time.DeltaTime;
            m_wallVelocity += def.WallAcceleration * Time.DeltaTime;
        }

        public void Push(float a_amount)
        {
            m_wallMovement -= a_amount;
            if (m_wallMovement < 0.0f)
            {
                m_wallMovement = 0.0f;
            }

            m_wallVelocity *= 0.75f;
        }
    }
}