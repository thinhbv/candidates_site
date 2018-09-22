using CMSSolutions.Environment.Extensions;
using CMSSolutions.Tasks;

namespace CMSSolutions.ContentManagement.Media
{
    [Feature(Constants.Areas.Media)]
    public class RemoveExpiredMediaFileTask : IScheduleTask
    {
        public string Name { get { return "Remove Expired Media File Task"; } }

        public bool Enabled { get { return true; } }

        public string CronExpression { get { return "0 0 0 1/1 * ? *"; } }

        public bool DisallowConcurrentExecution { get { return true; } }

        public void Execute(IWorkContextScope scope)
        {
        }
    }
}