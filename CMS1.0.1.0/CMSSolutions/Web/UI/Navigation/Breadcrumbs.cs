using System.Collections;
using System.Collections.Generic;

namespace CMSSolutions.Web.UI.Navigation
{
    public class Breadcrumbs : IEnumerable<Breadcrumb>
    {
        private readonly IList<Breadcrumb> items;

        public Breadcrumbs()
        {
            items = new List<Breadcrumb>();
        }

        public IEnumerator<Breadcrumb> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Breadcrumb item)
        {
            items.Add(item);
        }

        public void Add(string text, string url = null, string iconCssClass = null)
        {
            items.Add(new Breadcrumb { Text = text, Url = url, IconCssClass = iconCssClass });
        }

        public void Insert(int index, string text, string url = null, string iconCssClass = null)
        {
            items.Insert(index, new Breadcrumb { Text = text, Url = url, IconCssClass = iconCssClass });
        }
    }
}