﻿using LiteFramework;
using LiteFramework.Core.Log;
using LiteFramework.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace LiteMore
{
    public class Main : MonoBehaviour
    {
        private GUIStyle Style_;
        private ILogic Logic_;

        void Awake()
        {
            Camera.main.orthographicSize = Screen.height / 100.0f / 2.0f;
            Style_ = new GUIStyle { fontSize = 30, normal = { background = null, textColor = Color.white } };

            try
            {
                Logic_ = new GameLogic();
                //Logic_ = new TestLogic();
                LiteManager.Startup(this, Logic_);
            }
            catch (System.Exception Ex)
            {
                LLogger.LError($"{Ex.Message}\n{Ex.StackTrace}");
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
                LLogger.LError($"{Ex.Message}\n{Ex.StackTrace}");
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
            {
                LiteManager.TimeScale = 0.5f;
                LLogger.LWarning($"TimeScale = {LiteManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                LiteManager.TimeScale = 1.0f;
                LLogger.LWarning($"TimeScale = {LiteManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                LiteManager.TimeScale = 5.0f;
                LLogger.LWarning($"TimeScale = {LiteManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.PageUp))
            {
                LiteManager.TimeScale--;
                LLogger.LWarning($"TimeScale = {LiteManager.TimeScale}");
            }
            else if (Input.GetKeyDown(KeyCode.PageDown))
            {
                LiteManager.TimeScale++;
                LLogger.LWarning($"TimeScale = {LiteManager.TimeScale}");
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
                LLogger.LError($"{Ex.Message}\n{Ex.StackTrace}");
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

        void OnGUI()
        {
            //Logic_?.OnGUI();
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