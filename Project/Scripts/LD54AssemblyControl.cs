using IcarianEngine;
using IcarianEngine.Maths;
using IcarianEngine.Mod;
using IcarianEngine.Physics;
using IcarianEngine.Rendering;
using IcarianEngine.Rendering.Lighting;
using IcarianEngine.Rendering.UI;
using System.Collections.Generic;

namespace LD54
{
    // Yes this is disgusting, but it's a game jam so I don't care
    public class LD54AssemblyControl : AssemblyControl
    {
        static LD54AssemblyControl s_instance;

        Scene            m_scene = null;

        bool             m_flushCanvas = false;

        bool             m_fullscreen = false;
        bool             m_start = false;
        bool             m_loaded = false;    

        Canvas           m_canvas;
        GameObject       m_canvasGameObject;
        List<GameObject> m_flushObjects = new List<GameObject>();

        GameObject       m_cameraGameObject;
        GameObject       m_dirLightGameObject;

        void SceneLoaded(Scene a_scene, LoadStatus a_status)
        {
            if (a_status != LoadStatus.Loaded)
            {
                Logger.Error("Failed to load scene");

                return;
            }

            m_scene = a_scene;
            // m_scene.GenerateScene(Matrix4.Identity);

            Logger.Message("Scene loaded!");
        }

        static void UINormalButton(Canvas a_canvas, UIElement a_element)
        {
            a_element.Color = Color.White;
        }
        static void UIHoverButton(Canvas a_canvas, UIElement a_element)
        {
            a_element.Color = Color.Red;
        }
        static void UIPressedButton(Canvas a_canvas, UIElement a_element)
        {
            a_element.Color = Color.Green;
        }
        static void UIReleasedButton(Canvas a_canvas, UIElement a_element)
        {
            a_element.Color = Color.Blue;

            switch (a_element.Name)
            {
            case "Restart":
            {
                Logger.Message("Restarting game");

                // Wasteful to reload the scene just flush and re-generate
                s_instance.m_scene.FlushScene();

                // Clean up objects that were not in the scene
                foreach (GameObject gameObject in s_instance.m_flushObjects)
                {
                    if (gameObject != null && !gameObject.IsDisposed)
                    {
                        gameObject.Dispose();
                    }
                }

                s_instance.m_flushObjects.Clear();

                s_instance.m_scene.GenerateScene(Matrix4.Identity);

                // Canvas is currently being used by the game over screen
                // so cannot be disposed until the next frame
                s_instance.m_flushCanvas = true;

                break;
            }
            case "Quit":
            {
                Application.Close();

                break;
            }
            }
        }

        public override void Init()
        {
            s_instance = this;

            if (!Application.IsHeadless)
            {
                Monitor[] monitors = Application.GetMonitors();

                Application.SetFullscreen(monitors[0], true, monitors[0].Width, monitors[0].Height);
                m_fullscreen = true;
            }

            m_canvas = Canvas.FromFile("UI/MainMenu.ui");

            m_canvasGameObject = GameObject.Instantiate<GameObject>();
            CanvasRenderer renderer = m_canvasGameObject.AddComponent<CanvasRenderer>();
            renderer.Canvas = m_canvas;

            m_cameraGameObject = GameObject.Instantiate<GameObject>("MainCamera");
            Camera camera = m_cameraGameObject.AddComponent<Camera>();
            
            Scene.LoadSceneAsync("TestScene.iscene", SceneLoaded);

            GameObject playerPreview = GameObject.FromDef(GameObjectDefTable.Base_GameObject_PlayerN);
            playerPreview.Transform.Translation = new Vector3(2.5f, -33.0f, 0.5f);
            playerPreview.Transform.Rotation = Quaternion.FromAxisAngle(Vector3.Normalized(Vector3.UnitX + Vector3.UnitY), 0.8f);

            m_cameraGameObject.Transform.Translation = new Vector3(0.0f, -30.0f, 0.0f);
            // cameraGameObject.Transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, Mathf.PI * 0.5f);
            m_cameraGameObject.Transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, -Mathf.PI * 0.5f);

            m_dirLightGameObject = GameObject.Instantiate<GameObject>();
            m_dirLightGameObject.Transform.Rotation = Quaternion.FromAxisAngle(Vector3.Normalized(Vector3.UnitX + Vector3.UnitY), -2.0f);

            DirectionalLight dirLight = m_dirLightGameObject.AddComponent<DirectionalLight>();    
            dirLight.Color = Color.White;
            dirLight.Intensity = 0.5f;

            Physics.Gravity = Vector3.Zero;

            Logger.Message("Assembly Loaded!");
        }

        public static void AddFlushObject(GameObject a_object)
        {
            s_instance.m_flushObjects.Add(a_object);
        }
        public static void RemoveFlushObject(GameObject a_object)
        {
            s_instance.m_flushObjects.Remove(a_object);
        }

        public override void Update()
        {
            if (m_flushCanvas)
            {
                if (m_canvasGameObject != null)
                {
                    m_canvasGameObject.Dispose();
                    m_canvasGameObject = null;
                }

                if (m_canvas != null)
                {
                    m_canvas.Dispose();
                    m_canvas = null;
                }

                m_flushCanvas = false;
            }

            if (m_start)
            {
                m_cameraGameObject.Transform.Rotation = Quaternion.Lerp(m_cameraGameObject.Transform.Rotation, Quaternion.FromAxisAngle(Vector3.UnitX, Mathf.PI * 0.5f), Time.DeltaTime);
                m_dirLightGameObject.Transform.Rotation = Quaternion.Lerp(m_dirLightGameObject.Transform.Rotation, Quaternion.FromAxisAngle(Vector3.Normalized(Vector3.UnitX + Vector3.UnitY), 2.0f), Time.DeltaTime);

                if (!m_loaded && m_scene != null)
                {
                    m_loaded = true;

                    m_scene.GenerateScene(Matrix4.Identity);
                }
            }
            else if (Input.IsKeyPressed(KeyCode.Space))
            {
                m_start = true;

                m_flushCanvas = true;
            }

            if (Input.CtrlModifier)
            {
                if (Input.IsKeyPressed(KeyCode.Enter))
                {
                    m_fullscreen = !m_fullscreen;

                    Monitor[] monitors = Application.GetMonitors();
                    Application.SetFullscreen(monitors[0], m_fullscreen, monitors[0].Width, monitors[0].Height);
                }
            }

            // Want to advance the random state to make it less predictable
            Random.UpdateState();
        }

        public override void Close()
        {
            m_scene.Dispose();
        }

        public static GameObject GetPlayer()
        {
            if (s_instance.m_scene == null)
            {
                return null;
            }

            foreach (GameObject gameObject in s_instance.m_scene.GameObjects)
            {
                if (gameObject.Name == "Player")
                {
                    return gameObject;
                }
            }

            return null;
        }

        public static void GameOver()
        {
            GameObject player = GetPlayer();
            if (player != null)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();

                playerController.Lock = true;
            }

            s_instance.m_canvas = Canvas.FromFile("UI/GameOver.ui");

            if (s_instance.m_canvas != null)
            {
                s_instance.m_canvasGameObject = GameObject.Instantiate<GameObject>();
                CanvasRenderer renderer = s_instance.m_canvasGameObject.AddComponent<CanvasRenderer>();
                renderer.Canvas = s_instance.m_canvas;

                s_instance.m_canvas.CapturesInput = true;
            }
        }

        public static void PushWalls(float a_amount)
        {
            if (s_instance.m_scene == null)
            {
                return;
            }

            foreach (GameObject gameObject in s_instance.m_scene.GameObjects)
            {
                if (gameObject.Name == "Wall")
                {
                    WallController wallController = gameObject.GetComponent<WallController>();

                    if (wallController != null)
                    {
                        wallController.Push(a_amount);

                        return;
                    }
                }
            }
        }
    }
}
