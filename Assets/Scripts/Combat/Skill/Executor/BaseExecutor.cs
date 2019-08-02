using System.Collections.Generic;

namespace LiteMore.Combat.Skill.Executor
{
    public abstract class BaseExecutor
    {
        protected BaseExecutor()
        {
        }

        public abstract bool Execute(string Name, Dictionary<string, object> Args);
    }
}