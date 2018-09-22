using System.Linq;
using Autofac;
using CMSSolutions.ContentManagement.Lists.Services;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Tasks;

namespace CMSSolutions.ContentManagement.Lists.Routing
{
    [Feature(Constants.Areas.Lists)]
    public class ListPathConstraintUpdator : IShellEvents, IBackgroundTask
    {
        private readonly IListPathConstraint listPathConstraint;
        private readonly IListCategoryPathConstraint listCategoryPathConstraint;
        private readonly IComponentContext componentContext;

        public ListPathConstraintUpdator(IListPathConstraint listPathConstraint, IComponentContext componentContext, IListCategoryPathConstraint listCategoryPathConstraint)
        {
            this.listPathConstraint = listPathConstraint;
            this.componentContext = componentContext;
            this.listCategoryPathConstraint = listCategoryPathConstraint;
        }

        public int Priority { get { return 0; } }

        public void Activated()
        {
            Refresh();
        }

        public void Terminating()
        {
        }

        public void Sweep()
        {
            Refresh();
        }

        private void Refresh()
        {
            var listService = componentContext.Resolve<IListService>();
            listPathConstraint.SetPaths(listService.GetRecords().Where(x => x.Enabled).ToDictionary(k => k.Url, v => v.Id));

            var listCategoryService = componentContext.Resolve<IListCategoryService>();
            var categories = listCategoryService.GetRecords();
            listCategoryPathConstraint.SetPaths(categories);
        }
    }
}