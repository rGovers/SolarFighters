using IcarianEngine;
using IcarianEngine.Maths;
using IcarianEngine.Physics;
using IcarianEngine.Rendering;
using IcarianEngine.Rendering.Lighting;

namespace LD54
{
    // Scriptable component that can be attached to a GameObject
    public class PlayerController : Scriptable
    {
        Camera      m_camera;
        PhysicsBody m_physicsBody;

        float       m_shootTimer = 0.0f;

        Health      m_health;
        PointLight  m_light;

        bool        m_lock = false;

        float       m_prevAngle = 0.0f;
        float       m_roll = 0.0f;

        public PlayerControllerDef PlayerControllerDef
        {
            get
            {
                return Def as PlayerControllerDef;
            }
        }

        public bool Lock
        {
            get
            {
                return m_lock;
            }
            set
            {
                m_lock = value;
            }
        }

        public override void Init()
        {
            GameObject cameraGameObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (cameraGameObject == null)
            {
                Logger.Error("Failed to find main camera");

                return;
            }

            m_camera = cameraGameObject.GetComponent<Camera>();
            if (m_camera == null)
            {
                Logger.Error("Failed to find camera component");

                return;
            }

            m_physicsBody = GetComponent<PhysicsBody>();
            if (m_physicsBody == null)
            {
                Logger.Error("PlayerController requires a PhysicsBody component");

                return;
            }

            PlayerControllerDef def = PlayerControllerDef;

            if (def.BulletDef == null)
            {
                Logger.Error("Failed to find bullet definition");

                return;
            }

            m_health = GetComponent<Health>();
            if (m_health == null)
            {
                Logger.Error("PlayerController requires a Health component");

                return;
            }

            m_light = GetComponent<PointLight>();
            if (m_light == null)
            {
                Logger.Error("PlayerController requires a PointLight component");

                return;
            }
        }

        public override void Update()
        {
            if (m_lock)
            {
                return;
            }

            PlayerControllerDef def = PlayerControllerDef;

            Vector2 movement = Vector2.Zero;

            if (Input.IsKeyDown(KeyCode.W))
            {
                movement.Y -= 1.0f;
            }
            if (Input.IsKeyDown(KeyCode.S))
            {
                movement.Y += 1.0f;
            }
            if (Input.IsKeyDown(KeyCode.A))
            {
                movement.X -= 1.0f;
            }
            if (Input.IsKeyDown(KeyCode.D))
            {
                movement.X += 1.0f;
            }

            if (movement != Vector2.Zero)
            {
                float magnitude = movement.Magnitude;
                if (magnitude > 1.0f)
                {
                    movement /= magnitude;
                }

                Vector3 position = Transform.Translation + new Vector3(movement.X, 0.0f, movement.Y) * def.MovementSpeed * Time.DeltaTime;
                m_physicsBody.SetPosition(position);
            }

            Vector2 cursorPos = Input.CursorPosition;
            Vector2 screenSize = new Vector2(Application.Width, Application.Height);
            Vector2 screenPos = cursorPos / screenSize;

            Vector3 worldPos = m_camera.ScreenToWorld(new Vector3(screenPos, 0.9975f), screenSize);
            Vector3 playerPos = Transform.Translation;
            
            Vector2 dir = Vector2.Normalized(playerPos.XZ - worldPos.XZ);
            float angle = Mathf.Atan2(dir.X, dir.Y);

            float angleDiff = m_prevAngle - angle;

            if (angleDiff > Mathf.PI)
            {
                angleDiff -= Mathf.TwoPI;
                m_prevAngle -= Mathf.TwoPI;
            }
            else if (angleDiff < -Mathf.PI)
            {
                angleDiff += Mathf.TwoPI;
                m_prevAngle += Mathf.TwoPI;
            }

            m_roll = Mathf.Lerp(m_roll, angleDiff * 50.0f, Time.DeltaTime);

            Quaternion dirRotation = Quaternion.FromAxisAngle(Vector3.UnitY, angle);
            Quaternion rollRotation = Quaternion.FromAxisAngle(Vector3.UnitZ, m_roll);

            Quaternion rotation = dirRotation * rollRotation;

            m_prevAngle = angle;

            m_physicsBody.SetRotation(rotation);

            if (m_shootTimer > 0)
            {
                m_shootTimer -= Time.DeltaTime;
            }
            else if (Input.IsMousePressed(MouseButton.Left))
            {
                GameObject bulletObject = GameObject.FromDef(def.BulletDef);

                BulletController bulletController = bulletObject.GetComponent<BulletController>();
                bulletController.SpawnedBy = GameObject;
                bulletController.CanPushWalls = true;

                bulletObject.Transform.Translation = playerPos + Transform.Forward * 2.5f;
                bulletObject.Transform.Rotation = rotation;

                m_shootTimer = def.ShootRecharge;
            }

            float currentHealth = m_health.CurrentHealth;

            m_light.Radius = Mathf.Lerp(m_light.Radius, currentHealth * 0.1f, Time.DeltaTime);
            m_light.Intensity = Mathf.Lerp(m_light.Intensity, currentHealth * 0.2f, Time.DeltaTime);

            if (currentHealth <= 0.0f)
            {
                LD54AssemblyControl.GameOver();
            }
        }
    }
}
