using System;
using System.Collections.Generic;
using CMSSolutions.Tasks.Domain;

namespace CMSSolutions.Tasks.Services
{
    /// <summary>
    /// Task service interface
    /// </summary>
    public interface IScheduleTaskService : IDependency
    {
        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="task">Task</param>
        void DeleteTask(ScheduleTask task);

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>Task</returns>
        ScheduleTask GetTaskById(Guid taskId);

        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>Task</returns>
        ScheduleTask GetTaskByType(string type);

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <returns>Tasks</returns>
        IList<ScheduleTask> GetAllTasks();

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        void InsertTask(ScheduleTask task);

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        void UpdateTask(ScheduleTask task);

        /// <summary>
        /// Create a task if it's does not exist
        /// </summary>
        /// <param name="task"></param>
        void CreateTaskIfNotExist(ScheduleTask task);
    }
}