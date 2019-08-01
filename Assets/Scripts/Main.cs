using LiteMore.Combat.Bullet;
using LiteMore.Combat.Emitter;
using LiteMore.Combat.Npc;
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
                if (!GameManager.Startup())
                {
                    Debug.LogError("GameManager Startup Failed");
                }
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
                GameManager.Tick(Time.deltaTime);
            }
            catch (System.Exception Ex)
            {
                Debug.LogError($"{Ex.Message}\n{Ex.StackTrace}");
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
            {
                GameManager.TimeScale = 0.5f;
                Debug.LogWarning($"TimeScale = {GameManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                GameManager.TimeScale = 1.0f;
                Debug.LogWarning($"TimeScale = {GameManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                GameManager.TimeScale = 5.0f;
                Debug.LogWarning($"TimeScale = {GameManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.PageUp))
            {
                GameManager.TimeScale--;
                Debug.LogWarning($"TimeScale = {GameManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.PageDown))
            {
                GameManager.TimeScale++;
                Debug.LogWarning($"TimeScale = {GameManager.TimeScale}");
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                GameManager.Restart();
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                GameManager.Shutdown();
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
                GameManager.Shutdown();
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
                GameManager.OnEnterBackground();
            }
            else
            {
                GameManager.OnEnterForeground();
            }
        }

        /*void OnGUI()
        {
            GUI.Label(new Rect(5, 100, 0, 40), $"Npc Count : {NpcManager.GetCount()}", Style_);
            GUI.Label(new Rect(5, 150, 0, 40), $"Bullet Count : {BulletManager.GetCount()}", Style_);
            GUI.Label(new Rect(5, 200, 0, 40), $"Emitter Count : {EmitterManager.GetCount()}", Style_);
        }*/

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