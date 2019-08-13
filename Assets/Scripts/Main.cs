using LiteFramework;
using LiteFramework.Game.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore
{
    public class Main : MonoBehaviour
    {
        private GUIStyle Style_;

        void Awake()
        {
            Camera.main.orthographicSize = Screen.height / 2.0f;
            Style_ = new GUIStyle { fontSize = 30, normal = { background = null, textColor = Color.white } };

            try
            {
                if (!LiteManager.Startup(this))
                {
                    Debug.LogError("GameManager Startup Failed");
                }
                
                LogicManager.Attach(new GameLogic());
            }
            catch (System.Exception Ex)
            {
                Debug.LogError($"{Ex.Message}\n{Ex.StackTrace}");
            }
        }

        void Update()
        {
            try
            {
                LiteManager.Tick(Time.deltaTime);
            }
            catch (System.Exception Ex)
            {
                Debug.LogError($"{Ex.Message}\n{Ex.StackTrace}");
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
            {
                LiteManager.TimeScale = 0.5f;
                Debug.LogWarning($"TimeScale = {LiteManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                LiteManager.TimeScale = 1.0f;
                Debug.LogWarning($"TimeScale = {LiteManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                LiteManager.TimeScale = 5.0f;
                Debug.LogWarning($"TimeScale = {LiteManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.PageUp))
            {
                LiteManager.TimeScale--;
                Debug.LogWarning($"TimeScale = {LiteManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.PageDown))
            {
                LiteManager.TimeScale++;
                Debug.LogWarning($"TimeScale = {LiteManager.TimeScale}");
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                LiteManager.Restart();
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                LiteManager.Shutdown();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                EnableGizmos_ = !EnableGizmos_;
            }
#endif
        }

        void OnApplicationQuit()
        {
            try
            {
                LiteManager.Shutdown();
            }
            catch (System.Exception Ex)
            {
                Debug.LogError($"{Ex.Message}\n{Ex.StackTrace}");
            }
        }

        void OnApplicationPause(bool Pause)
        {
            if (Pause)
            {
                LiteManager.OnEnterBackground();
            }
            else
            {
                LiteManager.OnEnterForeground();
            }
        }

#if UNITY_EDITOR
        private static bool EnableGizmos_ = false;
        private static readonly Vector3[] FourCorners_ = new Vector3[4];

        void OnDrawGizmos()
        {
            if (!EnableGizmos_)
            {
                return;
            }

            foreach (var Entity in GameObject.FindObjectsOfType<MaskableGraphic>())
            {
                if (Entity.raycastTarget && (Entity.transform is RectTransform RectTransform))
                {
                    RectTransform.GetWorldCorners(FourCorners_);
                    Gizmos.color = Color.red;
                    for (var Index = 0; Index < 4; ++Index)
                    {
                        Gizmos.DrawLine(FourCorners_[Index], FourCorners_[(Index + 1) % 4]);
                    }
                }
            }
        }
#endif
    }
}