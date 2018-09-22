namespace CMSSolutions.Tasks
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public interface IScheduleTask : IDependency
    {
        string Name { get; }

        bool Enabled { get; }

        string CronExpression { get; }

        bool DisallowConcurrentExecution { get; }

        /// <summary>
        /// Execute task
        /// </summary>
        void Execute(IWorkContextScope scope);
    }
}