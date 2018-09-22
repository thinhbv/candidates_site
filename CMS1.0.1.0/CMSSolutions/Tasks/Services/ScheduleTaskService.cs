using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Tasks.Domain;

namespace CMSSolutions.Tasks.Services
{
    /// <summary>
    /// Schedule Task service
    /// </summary>
    [Feature(Constants.Areas.ScheduledTasks)]
    public class ScheduleTaskService : IScheduleTaskService
    {
        private readonly IRepository<ScheduleTask, Guid> taskRepository;

        public ScheduleTaskService(IRepository<ScheduleTask, Guid> taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void DeleteTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            taskRepository.Delete(task);
        }

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>Task</returns>
        public virtual ScheduleTask GetTaskById(Guid taskId)
        {
            if (taskId == Guid.Empty)
                return null;

            return taskRepository.GetById(taskId);
        }

        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>Task</returns>
        public virtual ScheduleTask GetTaskByType(string type)
        {
            if (String.IsNullOrWhiteSpace(type))
                return null;

            var query = taskRepository.Table;
            query = query.Where(st => st.Type == type);
            query = query.OrderByDescending(t => t.Id);

            var task = query.FirstOrDefault();
            return task;
        }

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <returns>Tasks</returns>
        public virtual IList<ScheduleTask> GetAllTasks()
        {
            var query = taskRepository.Table.AsNoTracking().OrderByDescending(t => t.Name);
            var tasks = query.ToList();
            return tasks;
        }

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void InsertTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            taskRepository.Insert(task);
        }

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void UpdateTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            taskRepository.Update(task);
        }

        public void CreateTaskIfNotExist(ScheduleTask task)
        {
            var oldTask = GetTaskByType(task.Type);
            if (oldTask == null)
            {
                InsertTask(task);
            }
        }
    }
}