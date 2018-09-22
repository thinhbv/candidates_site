using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.ContentManagement.Widgets.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Serialization;
using CMSSolutions.Serialization.Core;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Widgets.Services
{
    public interface IWidgetService : IGenericService<Widget, int>, IDependency
    {
        IList<IWidget> GetWidgets(int? pageId = null, bool includeDisabled = false);

        IList<IWidget> GetWidgets(IEnumerable<Widget> records);

        void EnableOrDisable(Widget record);

        IWidget GetWidget(string zoneName);

        Widget GetByZone(int zoneId);
    }

    [Feature(Constants.Areas.Widgets)]
    public class WidgetService : GenericService<Widget, int>, IWidgetService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;
        private readonly IZoneService zoneService;

        public WidgetService(IRepository<Widget, int> repository, 
            IEventBus eventBus, 
            ICacheManager cacheManager, 
            ISignals signals, IZoneService zoneService)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
            this.zoneService = zoneService;
        }

        #region IWidgetService Members

        public override void Insert(Widget record)
        {
            base.Insert(record);
            signals.Trigger("Widgets_Changed");
        }

        public override void Update(Widget record)
        {
            base.Update(record);
            signals.Trigger("Widgets_Changed", record.Id);
        }

        public override void Delete(Widget record)
        {
            var records = GetRecords(x => x.Id == record.Id || x.RefId == record.Id);
            DeleteMany(records);
            signals.Trigger("Widgets_Changed", record.Id);
        }

        public IList<IWidget> GetWidgets(IEnumerable<Widget> records)
        {
            var settings = new SharpSerializerXmlSettings
            {
                IncludeAssemblyVersionInTypeName = false,
                IncludeCultureInTypeName = false,
                IncludePublicKeyTokenInTypeName = false
            };
            var sharpSerializer = new SharpSerializer(settings);

            var result = new List<IWidget>();
            foreach (var record in records)
            {
                IWidget widget;
                try
                {
                    widget = (IWidget)sharpSerializer.DeserializeFromString(record.WidgetValues);
                }
                catch (InvalidCastException)
                {
                    continue;
                }
                catch (TypeLoadException)
                {
                    continue;
                }
                catch (DeserializingException)
                {
                    continue;
                }

                widget.Id = record.Id;
                widget.Title = record.Title;
                widget.ZoneId = record.ZoneId;
                widget.PageId = record.PageId;
                widget.Order = record.Order;
                widget.Enabled = record.Enabled;
                widget.DisplayCondition = record.DisplayCondition;
                widget.CultureCode = record.CultureCode;
                widget.RefId = record.RefId;
                result.Add(widget);
            }
            return result;
        }

        public void EnableOrDisable(Widget record)
        {
            if (record == null) return;
            record.Enabled = !record.Enabled;
            base.Update(record);
            signals.Trigger("Widgets_Changed", record.Id);
        }

        public Widget GetByZone(int zoneId)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@ZoneId", zoneId)
            };

            return ExecuteReaderRecord<Widget>("sp_Widgets_GetByZone", list.ToArray());
        }

        public IWidget GetWidget(string zoneName)
        {
            var zone = zoneService.GetByName(zoneName);
            if (zone == null)
            {
                return null;
            }
            var record = GetByZone(zone.Id);
            if (record == null)
            {
                return null;
            }

            var settings = new SharpSerializerXmlSettings
            {
                IncludeAssemblyVersionInTypeName = false,
                IncludeCultureInTypeName = false,
                IncludePublicKeyTokenInTypeName = false
            };

            var sharpSerializer = new SharpSerializer(settings);
            var widget = (IWidget)sharpSerializer.DeserializeFromString(record.WidgetValues);
            widget.Id = record.Id;
            widget.Title = record.Title;
            widget.ZoneId = record.ZoneId;
            widget.PageId = record.PageId;
            widget.Order = record.Order;
            widget.Enabled = record.Enabled;
            widget.DisplayCondition = record.DisplayCondition;
            widget.CultureCode = record.CultureCode;
            widget.RefId = record.RefId;

            return widget;
        }

        public IList<IWidget> GetWidgets(int? pageId = null, bool includeDisabled = false)
        {
            var key = string.Format("Widgets_GetWidgets_{0}_{1}", includeDisabled, pageId);
            if (includeDisabled)
            {
                return cacheManager.Get(key, ctx =>
                {
                    ctx.Monitor(signals.When("Widgets_Changed"));

                    var records = pageId.HasValue
                        ? GetRecords(x => x.PageId == pageId.Value)
                        : GetRecords(x => x.PageId == null);

                    return GetWidgets(records);
                });
            }

            return cacheManager.Get(key, ctx =>
            {
                ctx.Monitor(signals.When("Widgets_Changed"));

                var records = pageId.HasValue
                    ? GetRecords(x => x.Enabled && x.PageId == pageId.Value)
                    : GetRecords(x => x.Enabled && x.PageId == null);

                return GetWidgets(records);
            });
        }

        #endregion IWidgetService Members

        protected override IOrderedQueryable<Widget> MakeDefaultOrderBy(IQueryable<Widget> queryable)
        {
            return queryable.OrderBy(x => x.ZoneId).ThenBy(x => x.Order);
        }
    }
}