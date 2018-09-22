using System;
using CMSSolutions.Tasks.Domain;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Tasks.Models
{
    public class ScheduleTaskModel : BaseModel<Guid>
    {
        [ControlText(Required = true)]
        public string Name { get; set; }

        [ControlText(Required = true, MaxLength = 255, LabelText = "Cron Expression")]
        public string CronExpression { get; set; }

        [ControlText(Required = true)]
        public string Type { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Enabled")]
        public bool Enabled { get; set; }

        public static implicit operator ScheduleTaskModel(ScheduleTask task)
        {
            return new ScheduleTaskModel
            {
                Id = task.Id,
                Name = task.Name,
                Type = task.Type,
                CronExpression = task.CronExpression,
                Enabled = task.Enabled
            };
        }
    }
}