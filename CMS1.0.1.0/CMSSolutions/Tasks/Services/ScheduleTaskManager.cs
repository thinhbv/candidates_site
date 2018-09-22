using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Castle.Core.Logging;
using JetBrains.Annotations;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Quartz;
using CMSSolutions.Tasks.Domain;

namespace CMSSolutions.Tasks.Services
{
    public interface IScheduleTaskManager : IDependency
    {
        void ScheduleJob(ScheduleTask task);

        void RunNow(Guid id);
    }

    /// <summary>
    /// Represents task manager
    /// </summary>
    [Feature(Constants.Areas.ScheduledTasks)]
    public class ScheduleTaskManager : IScheduleTaskManager, IShellEvents
    {
        private readonly IComponentContext componentContext;
        private readonly IScheduler scheduler;

        public ScheduleTaskManager(IComponentContext componentContext, IScheduler scheduler)
        {
            this.componentContext = componentContext;
            this.scheduler = scheduler;
            Logger = NullLogger.Instance;
        }

        public int Priority { get { return 0; } }

        public ILogger Logger { get; set; }

        public void Activated()
        {
            var scheduleTaskService = componentContext.Resolve<IScheduleTaskService>();

            // Collect all tasks
            var tasks = scheduleTaskService.GetAllTasks();
            var scheduleTasks = componentContext.Resolve<IEnumerable<IScheduleTask>>();

            var flag = false;

            foreach (var task in scheduleTasks)
            {
                var type = task.GetType();
                var typeFullName = type.FullName + ", " + type.Assembly.GetName().Name;

                if (tasks.All(x => x.Type != typeFullName))
                {
                    if (task.Enabled)
                    {
                        flag = true;
                    }

                    scheduleTaskService.InsertTask(new ScheduleTask
                                                       {
                                                           Id = Guid.NewGuid(),
                                                           Enabled = task.Enabled,
                                                           Name = task.Name,
                                                           CronExpression = task.CronExpression,
                                                           Type = typeFullName,
                                                           DisallowConcurrentExecution = task.DisallowConcurrentExecution
                                                       });
                }
            }

            if (flag)
            {
                tasks = scheduleTaskService.GetAllTasks();
            }

            foreach (var task in tasks)
            {
                ScheduleJob(task);
            }
        }

        public void Terminating()
        {
        }

        public void ScheduleJob(ScheduleTask task)
        {
            var key = new JobKey("Task_" + task.Id);

            if (scheduler.CheckExists(key))
            {
                scheduler.DeleteJob(key);
            }

            if (task.Enabled)
            {
                var job = task.DisallowConcurrentExecution ? JobBuilder.Create<DisallowConcurrentGenericJob>().WithIdentity(key).Build() : JobBuilder.Create<GenericJob>().WithIdentity(key).Build();
                job.JobDataMap.Add("IWorkContextAccessor", componentContext.Resolve<IWorkContextAccessor>());
                job.JobDataMap.Add("ScheduleTask", task);
                var trigger = TriggerBuilder.Create().WithCronSchedule(task.CronExpression).Build();
                scheduler.ScheduleJob(job, trigger);
            }
        }

        public void RunNow(Guid id)
        {
            var key = new JobKey("Task_" + id);
            if (!scheduler.CheckExists(key))
            {
                return;
            }

            var job = scheduler.GetJobDetail(key);
            var task = (ScheduleTask)job.JobDataMap["ScheduleTask"];
            var type = Type.GetType(task.Type);
            if (type != null)
            {
                var workContextAccessor = (IWorkContextAccessor)job.JobDataMap["IWorkContextAccessor"];
                using (var workContextScope = workContextAccessor.CreateWorkContextScope())
                {
                    var scheduleTask = Activator.CreateInstance(type) as IScheduleTask;
                    if (scheduleTask != null)
                    {
                        var scheduleTaskService = workContextScope.Resolve<IScheduleTaskService>();
                        var taskRecord = scheduleTaskService.GetTaskById(task.Id);
                        taskRecord.LastStartUtc = DateTime.UtcNow;

                        try
                        {
                            scheduleTask.Execute(workContextScope);
                            taskRecord.LastSuccessUtc = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            Logger.ErrorFormat(ex, "Have error when execute scheduler task.");
                        }
                        finally
                        {
                            taskRecord.LastEndUtc = DateTime.UtcNow;
                            scheduleTaskService.UpdateTask(taskRecord);
                        }
                    }
                }
            }
        }

        [UsedImplicitly]
        public class GenericJob : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                var task = (ScheduleTask)context.MergedJobDataMap["ScheduleTask"];
                var type = Type.GetType(task.Type);
                if (type != null)
                {
                    var workContextAccessor = (IWorkContextAccessor)context.MergedJobDataMap["IWorkContextAccessor"];
                    using (var workContextScope = workContextAccessor.CreateWorkContextScope())
                    {
                        var scheduleTask = Activator.CreateInstance(type) as IScheduleTask;
                        if (scheduleTask != null)
                        {
                            var scheduleTaskService = workContextScope.Resolve<IScheduleTaskService>();
                            var taskRecord = scheduleTaskService.GetTaskById(task.Id);
                            taskRecord.LastStartUtc = DateTime.UtcNow;

                            try
                            {
                                scheduleTask.Execute(workContextScope);
                                taskRecord.LastSuccessUtc = DateTime.UtcNow;
                            }
                            catch (Exception ex)
                            {
                                var logger = workContextScope.WorkContext.ResolveWithParameters<ILogger>(new TypedParameter(typeof(Type), typeof(ScheduleTaskManager)));
                                logger.ErrorFormat(ex, "Have error when execute scheduler task.");
                            }
                            finally
                            {
                                taskRecord.LastEndUtc = DateTime.UtcNow;
                                scheduleTaskService.UpdateTask(taskRecord);
                            }
                        }
                    }
                }
            }
        }

        [UsedImplicitly]
        [DisallowConcurrentExecution]
        public class DisallowConcurrentGenericJob : GenericJob
        {
        }
    }
}