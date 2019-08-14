using UnityEngine;

namespace LiteFramework.Extend.Debug
{
    internal class SettingDebugWindow : BaseDebugWindow
    {
        private Vector2 ScrollPosition_ = Vector2.zero;

        internal SettingDebugWindow()
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
                GUILayout.Label("<b>Window Settings</b>");
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Position:", GUILayout.Width(60f));
                        GUILayout.Label("Drag window caption to move position.");
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        var Width = Debugger.NormalBounds.width;
                        GUILayout.Label("Width:", GUILayout.Width(60f));
                        if (GUILayout.RepeatButton("-", GUILayout.Width(30f)))
                        {
                            Width--;
                        }
                        Width = GUILayout.HorizontalSlider(Width, 100f, Screen.width - 20f);
                        if (GUILayout.RepeatButton("+", GUILayout.Width(30f)))
                        {
                            Width++;
                        }
                        Width = Mathf.Clamp(Width, 100f, Screen.width - 20f);
                        if (!Mathf.Approximately(Width, Debugger.NormalBounds.width))
                        {
                            Debugger.NormalBounds = new Rect(Debugger.NormalBounds.position, new Vector2(Width, Debugger.NormalBounds.height));
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        var Height = Debugger.NormalBounds.height;
                        GUILayout.Label("Height:", GUILayout.Width(60f));
                        if (GUILayout.RepeatButton("-", GUILayout.Width(30f)))
                        {
                            Height--;
                        }
                        Height = GUILayout.HorizontalSlider(Height, 100f, Screen.height - 20f);
                        if (GUILayout.RepeatButton("+", GUILayout.Width(30f)))
                        {
                            Height++;
                        }
                        Height = Mathf.Clamp(Height, 100f, Screen.height - 20f);
                        if (!Mathf.Approximately(Height, Debugger.NormalBounds.height))
                        {
                            Debugger.NormalBounds = new Rect(Debugger.NormalBounds.position, new Vector2(Debugger.NormalBounds.width, Height));
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        var Scale = Debugger.Scale;
                        GUILayout.Label("Scale:", GUILayout.Width(60f));
                        if (GUILayout.RepeatButton("-", GUILayout.Width(30f)))
                        {
                            Scale -= 0.01f;
                        }
                        Scale = GUILayout.HorizontalSlider(Scale, 0.5f, 4f);
                        if (GUILayout.RepeatButton("+", GUILayout.Width(30f)))
                        {
                            Scale += 0.01f;
                        }
                        Scale = Mathf.Clamp(Scale, 0.5f, 4f);
                        if (!Mathf.Approximately(Scale, Debugger.Scale))
                        {
                            Debugger.Scale = Scale;
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Scale:", GUILayout.Height(60f));
                        if (GUILayout.Button("0.5x", GUILayout.Height(60f)))
                        {
                            Debugger.Scale = 0.5f;
                        }
                        if (GUILayout.Button("1.0x", GUILayout.Height(60f)))
                        {
                            Debugger.Scale = 1f;
                        }
                        if (GUILayout.Button("1.5x", GUILayout.Height(60f)))
                        {
                            Debugger.Scale = 1.5f;
                        }
                        if (GUILayout.Button("2.0x", GUILayout.Height(60f)))
                        {
                            Debugger.Scale = 2f;
                        }
                        if (GUILayout.Button("2.5x", GUILayout.Height(60f)))
                        {
                            Debugger.Scale = 2.5f;
                        }
                        if (GUILayout.Button("3.0x", GUILayout.Height(60f)))
                        {
                            Debugger.Scale = 3f;
                        }
                        if (GUILayout.Button("3.5x", GUILayout.Height(60f)))
                        {
                            Debugger.Scale = 3.5f;
                        }
                        if (GUILayout.Button("4.0x", GUILayout.Height(60f)))
                        {
                            Debugger.Scale = 4f;
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("TimeScale:", GUILayout.Height(60f));
                        if (GUILayout.Button("0.25x", GUILayout.Height(60f)))
                        {
                            LiteManager.TimeScale = 0.25f;
                        }
                        if (GUILayout.Button("0.5x", GUILayout.Height(60f)))
                        {
                            LiteManager.TimeScale = 0.5f;
                        }
                        if (GUILayout.Button("1.0x", GUILayout.Height(60f)))
                        {
                            LiteManager.TimeScale = 1.0f;
                        }
                        if (GUILayout.Button("2.0x", GUILayout.Height(60f)))
                        {
                            LiteManager.TimeScale = 2.0f;
                        }
                        if (GUILayout.Button("3.0x", GUILayout.Height(60f)))
                        {
                            LiteManager.TimeScale = 3.0f;
                        }
                        if (GUILayout.Button("4.0x", GUILayout.Height(60f)))
                        {
                            LiteManager.TimeScale = 4.0f;
                        }
                        if (GUILayout.Button("5.0x", GUILayout.Height(60f)))
                        {
                            LiteManager.TimeScale = 5.0f;
                        }
                        if (GUILayout.Button("10.0x", GUILayout.Height(60f)))
                        {
                            LiteManager.TimeScale = 10f;
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Reset Layout", GUILayout.Height(30f)))
                    {
                        Debugger.ResetLayout();
                    }

                    if (GUILayout.Button("Restart Game", GUILayout.Height(30f)))
                    {
                        LiteManager.Restart();
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }
    }
}