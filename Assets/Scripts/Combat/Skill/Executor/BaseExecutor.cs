namespace LiteMore.Combat.Skill.Executor
{
    public abstract class BaseExecutor
    {
        protected BaseExecutor()
        {
        }

        public abstract bool Execute(SkillArgs Args);
    }
}