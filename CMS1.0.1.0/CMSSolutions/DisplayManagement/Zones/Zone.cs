using System;
using System.Collections;
using System.Web.Mvc;
using CMSSolutions.DisplayManagement.Shapes;

namespace CMSSolutions.DisplayManagement.Zones
{
    public interface IZone : IEnumerable
    {
        string ZoneName { get; set; }

        Shape Add(object item, string position);

        IZone Add(Action<HtmlHelper> action, string position);
    }

    public class Zone : Shape, IZone
    {
        public virtual string ZoneName { get; set; }

        public IZone Add(Action<HtmlHelper> action, string position)
        {
            // pszmyd: Replaced the NotImplementedException with simply doing nothing
            return this;
        }
    }
}