using UnityEngine;

namespace LiteFramework.Extend.Debug
{
    internal class InfoDebugWindow : BaseDebugWindow
    {
        private Vector2 ScrollPosition_ = Vector2.zero;

        internal InfoDebugWindow()
            : base()
        {
        }

        internal override bool Startup()
        {
            return true;
        }

        internal override void Shutdown()
        {
        }

        internal override void Tick(float DeltaTime)
        {
        }

        internal override void Draw()
        {
            ScrollPosition_ = GUILayout.BeginScrollView(ScrollPosition_);
            {
                GUILayout.Label("<b>System Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Device Unique ID:", SystemInfo.deviceUniqueIdentifier);
                    DrawItem("Device Name:", SystemInfo.deviceName);
                    DrawItem("Device Type:", SystemInfo.deviceType.ToString());
                    DrawItem("Device Model:", SystemInfo.deviceModel);
                    DrawItem("Processor Type:", SystemInfo.processorType);
                    DrawItem("Processor Count:", SystemInfo.processorCount.ToString());
                    DrawItem("Processor Frequency:", $"{SystemInfo.processorFrequency} MHz");
                    DrawItem("System Memory Size:", $"{SystemInfo.systemMemorySize} MB");
#if UNITY_5_5_OR_NEWER
                    DrawItem("Operating System Family:", SystemInfo.operatingSystemFamily.ToString());
#endif
                    DrawItem("Operating System:", SystemInfo.operatingSystem);
#if UNITY_5_6_OR_NEWER
                    DrawItem("Battery Status:", SystemInfo.batteryStatus.ToString());
                    DrawItem("Battery Level:", $"{SystemInfo.batteryLevel:P1}");
#endif
#if UNITY_5_4_OR_NEWER
                    DrawItem("Supports Audio:", SystemInfo.supportsAudio.ToString());
#endif
                    DrawItem("Supports Location Service:", SystemInfo.supportsLocationService.ToString());
                    DrawItem("Supports Accelerometer:", SystemInfo.supportsAccelerometer.ToString());
                    DrawItem("Supports Gyroscope:", SystemInfo.supportsGyroscope.ToString());
                    DrawItem("Supports Vibration:", SystemInfo.supportsVibration.ToString());
                    DrawItem("Genuine:", Application.genuine.ToString());
                    DrawItem("Genuine Check Available:", Application.genuineCheckAvailable.ToString());
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }

        private void DrawItem(string Title, string Content)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(Title, GUILayout.Width(240));
                GUILayout.Label(Content);
            }
            GUILayout.EndHorizontal();
        }
    }
}