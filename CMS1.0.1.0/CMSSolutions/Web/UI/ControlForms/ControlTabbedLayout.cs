using System.Collections.Generic;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlTabbedLayout<TModel>
    {
        private readonly IList<ControlGroupedLayout<TModel>> groups;

        public ControlTabbedLayout(string title)
        {
            Title = title;
            groups = new List<ControlGroupedLayout<TModel>>();
        }

        public string Title { get; set; }

        public IList<ControlGroupedLayout<TModel>> Groups { get { return groups; }}

        public ControlGroupedLayout<TModel> AddGroup(string title = null)
        {
            var group = new ControlGroupedLayout<TModel>(title);
            groups.Add(group);
            return group;
        }
    }
}
