using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSSolutions.DisplayManagement.Descriptors;
using CMSSolutions.DisplayManagement.Zones;
using CMSSolutions.Extensions;

namespace CMSSolutions.DisplayManagement.Shapes
{
    public class CoreShapes : IShapeTableProvider
    {
        public void Discover(ShapeTableBuilder builder)
        {
            // the root page shape named 'Layout' is wrapped with 'Document'
            // and has an automatic zone creating behavior
            builder.Describe("Layout")
                .Configure(descriptor => descriptor.Wrappers.Add("Document"))
                .OnCreating(creating => creating.Create = () => new ZoneHolding(() => creating.New.Zone()))
                .OnCreated(created =>
                {
                    var layout = created.Shape;

                    layout.Head = created.New.DocumentZone(ZoneName: "Head");
                    layout.Body = created.New.DocumentZone(ZoneName: "Body");
                    layout.Tail = created.New.DocumentZone(ZoneName: "Tail");

                    layout.Body.Add(created.New.PlaceChildContent(Source: layout));

                    layout.Content = created.New.Zone();
                    layout.Content.ZoneName = "Content";
                    layout.Content.Add(created.New.PlaceChildContent(Source: layout));
                });

            // 'Zone' shapes are built on the Zone base class
            // They have class="zone zone-{name}"
            // and the template can be specialized with "Zone-{Name}" base file name
            builder.Describe("Zone")
                .OnCreating(creating => creating.Create = () => new Zone())
                .OnDisplaying(displaying =>
                {
                    var zone = displaying.Shape;
                    string zoneName = zone.ZoneName;
                    zone.Classes.Add("zone-" + zoneName.HtmlClassify());
                    zone.Classes.Add("zone");

                    // Zone__[ZoneName] e.g. Zone-SideBar
                    zone.Metadata.Alternates.Add("Zone__" + zoneName);
                });
        }

        [Shape]
        public void Zone(dynamic Display, dynamic Shape, TextWriter Output)
        {
            string id = Shape.Id;
            IEnumerable<string> classes = Shape.Classes;
            IDictionary<string, string> attributes = Shape.Attributes;
            var zoneWrapper = GetTagBuilder("div", id, classes, attributes);
            Output.Write(zoneWrapper.ToString(TagRenderMode.StartTag));
            foreach (var item in OrderedHack(Shape))
                Output.Write(Display(item));
            Output.Write(zoneWrapper.ToString(TagRenderMode.EndTag));
        }

        [Shape]
        public IHtmlString PlaceChildContent(dynamic Source)
        {
            return Source.Metadata.ChildContent;
        }

        [Shape]
        public IHtmlString Message(dynamic Shape)
        {
            return new MvcHtmlString(string.Format("<div class=\"alert alert-{0}\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-hidden=\"true\">&times;</button>{1}</div>", Convert.ToString(Shape.Type).ToLowerInvariant(), Shape.Message.ToString().Replace("\n", "<br />")));
        }

        private static TagBuilder GetTagBuilder(string tagName, string id, IEnumerable<string> classes, IDictionary<string, string> attributes)
        {
            var tagBuilder = new TagBuilder(tagName);
            tagBuilder.MergeAttributes(attributes, false);
            foreach (var cssClass in classes ?? Enumerable.Empty<string>())
                tagBuilder.AddCssClass(cssClass);
            if (!string.IsNullOrWhiteSpace(id))
                tagBuilder.GenerateId(id);
            return tagBuilder;
        }

        private static IEnumerable<dynamic> OrderedHack(dynamic shape)
        {
            IEnumerable<dynamic> unordered = shape;
            if (unordered == null || unordered.Count() < 2)
                return shape;

            var i = 1;
            var progress = 1;
            var flatPositionComparer = new FlatPositionComparer();
            var ordering = unordered.Select(item =>
            {
                var position = (item == null || item.GetType().GetProperty("Metadata") == null || item.Metadata.GetType().GetProperty("Position") == null)
                                   ? null
                                   : item.Metadata.Position;
                return new { item, position };
            }).ToList();

            // since this isn't sticking around (hence, the "hack" in the name), throwing (in) a gnome
            while (i < ordering.Count())
            {
                if (flatPositionComparer.Compare(ordering[i].position, ordering[i - 1].position) > -1)
                {
                    if (i == progress)
                        progress = ++i;
                    else
                        i = progress;
                }
                else
                {
                    var higherThanItShouldBe = ordering[i];
                    ordering[i] = ordering[i - 1];
                    ordering[i - 1] = higherThanItShouldBe;
                    if (i > 1)
                        --i;
                }
            }

            return ordering.Select(ordered => ordered.item).ToList();
        }
    }
}