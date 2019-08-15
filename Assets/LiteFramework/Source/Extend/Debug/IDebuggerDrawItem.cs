using System;

namespace LiteFramework.Extend.Debug
{
    internal interface IDebuggerDrawItem : IDisposable
    {
        string Name { get; set; }
        void Enter();
        void Exit();
        void Draw();
    }
}