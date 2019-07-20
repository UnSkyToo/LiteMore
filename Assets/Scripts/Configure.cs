using System;
using System.Collections.Generic;
using LiteMore.UI.Logic;
using UnityEngine;

namespace LiteMore
{
    public static class Configure
    {
        public const int WindowWidth = 1280;
        public const int WindowHeight = 720;
        public const int WindowLeft = -640;
        public const int WindowRight = 640;
        public const int WindowTop = 320;
        public const int WindowBottom = -320;
        public static readonly Vector2 WindowSize = new Vector2(WindowWidth, WindowHeight);
        public const float EnterBackgroundMaxTime = 90.0f;
    }
}