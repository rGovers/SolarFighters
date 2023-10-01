using IcarianEngine;
using IcarianEngine.Maths;
using IcarianEngine.Physics;

namespace LD54
{
    public class EnemyController : Scriptable
    {
        public EnemyControllerDef EnemyControllerDef
        {
            get
            {
                return Def as EnemyControllerDef;
            }
        }

        Health           m_health;
        PlayerController m_playerController;

        RigidBody        m_physicsBody;

        float            m_currentAngle = 0.0f;

        float            m_shootCooldown = 0.0f;

        float            m_targetKickForce = 0.0f;
        float            m_kickForce = 0.0f;  

        public override void Init()
        {
            EnemyControllerDef def = EnemyControllerDef;
            if (def == null)
            {
                Logger.Error("EnemyController requires a EnemyControllerDef");

                return;
            }

            m_health = GetComponent<Health>();
            if (m_health == null)
            {
                Logger.Error("EnemyController requires a Health component");

                return;
            }

            m_physicsBody = GetComponent<RigidBody>();
            if (m_physicsBody == null)
            {
                Logger.Error("EnemyController requires a RigidBody component");

                return;
            }

            GameObject player = LD54AssemblyControl.GetPlayer();
            if (player == null)
            {
                Logger.Error("Failed to find player");

                return;
            }

            m_playerController = player.GetComponent<PlayerController>();
            if (m_playerController == null)
            {
                Logger.Error("Failed to find player controller");

                return;
            }

            m_shootCooldown = def.ShootCooldown;

            LD54AssemblyControl.AddFlushObject(GameObject);
        }

        public override void Update()
        {   
            EnemyControllerDef def = EnemyControllerDef;

            Transform.Scale = Vector3.Lerp(Transform.Scale, Vector3.One, Time.DeltaTime * 1.0f);

            Vector3 playerPosition = m_playerController.Transform.Translation;
            Vector3 position = Transform.Translation;

            Vector2 diff = playerPosition.XZ - position.XZ;
            float dist = diff.Magnitude;
            if (dist > 0.0f)
            {
                Vector2 dir = diff / dist;
                float angle = Mathf.Atan2(dir.X, dir.Y);

                float halfLerp = EnemyControllerDef.TurnSpeed * Time.DeltaTime * 0.5f;

                if (angle - m_currentAngle > Mathf.PI)
                {
                    m_currentAngle += Mathf.TwoPI;
                }
                else if (angle - m_currentAngle < -Mathf.PI)
                {
                    m_currentAngle -= Mathf.TwoPI;
                }

                m_currentAngle = Mathf.Lerp(m_currentAngle, angle, halfLerp);

                float angleDiff = angle - m_currentAngle;

                Quaternion kickRotation = Quaternion.FromAxisAngle(Vector3.UnitX, m_targetKickForce);
                Quaternion direction = Quaternion.FromAxisAngle(Vector3.UnitY, m_currentAngle);
                Quaternion roll = Quaternion.FromAxisAngle(Vector3.UnitZ, angleDiff * 0.5f);

                m_physicsBody.SetRotation(direction * kickRotation * roll);

                m_currentAngle = Mathf.Lerp(m_currentAngle, angle, halfLerp);

                if (dist < EnemyControllerDef.FollowDistance)
                {
                    m_physicsBody.AddForce(new Vector3(-dir.X, 0.0f, -dir.Y) * def.MoveForce * 0.5f, ForceMode.Acceleration);
                }
                else
                {
                    m_physicsBody.AddForce((new Vector3(dir.X, 0.0f, dir.Y) + (Transform.Right * 0.2f)) * def.MoveForce, ForceMode.Acceleration);
                }
            }

            m_physicsBody.Velocity *= 1.0f - Time.DeltaTime * 0.5f;
            m_physicsBody.SetPosition(new Vector3(Transform.Translation.X, 0.0f, Transform.Translation.Z));

            Vector3 forward = Transform.Forward;

            m_shootCooldown -= Time.DeltaTime;
            if (m_shootCooldown < 0 && Vector2.Dot(-forward.XZ, diff) > 1.0f) 
            {
                GameObject bullet = GameObject.FromDef(def.BulletDef);

                BulletController bulletController = bullet.GetComponent<BulletController>();
                bulletController.SpawnedBy = GameObject;

                bullet.Transform.Translation = Transform.Translation - Transform.Forward * 1.0f;
                
                Quaternion rotation = Transform.Rotation;
                Vector4 axisAngle = rotation.ToAxisAngle();

                bullet.Transform.Rotation = Quaternion.FromAxisAngle(axisAngle.XYZ, axisAngle.W + Mathf.PI);

                m_kickForce += def.KickForce;

                m_shootCooldown = def.ShootCooldown;
            }

            m_targetKickForce = Mathf.Lerp(m_targetKickForce, m_kickForce, Time.DeltaTime * 2.0f);

            m_kickForce -= Mathf.Sign(m_kickForce) * Time.DeltaTime * 2.5f;

            if (m_health.CurrentHealth <= 0.0f)
            {
                LD54AssemblyControl.RemoveFlushObject(GameObject);
                
                GameObject.Dispose();
            }
        }   
    }
}