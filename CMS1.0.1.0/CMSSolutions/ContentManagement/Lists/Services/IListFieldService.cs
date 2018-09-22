using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.ContentManagement.Lists.Fields;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Serialization;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Lists.Services
{
    public interface IListFieldService : IGenericService<ListField, int>, IDependency
    {
        IList<IListField> GetFields(int listId);
    }

    [Feature(Constants.Areas.Lists)]
    public class ListFieldService : GenericService<ListField, int>, IListFieldService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public ListFieldService(IRepository<ListField, int> repository, IEventBus eventBus, ICacheManager cacheManager, ISignals signals)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
        }

        protected override IOrderedQueryable<ListField> MakeDefaultOrderBy(IQueryable<ListField> queryable)
        {
            return queryable.OrderBy(x => x.Position);
        }

        public override void Insert(ListField record)
        {
            base.Insert(record);
            signals.Trigger("Fields_Changed");
        }

        public override void Update(ListField record)
        {
            Repository.Update(record);
            signals.Trigger("Fields_Changed", record.Id);
            signals.Trigger("FieldsOfList_Changed", record.ListId);
        }

        public override void Delete(ListField record)
        {
            Repository.Delete(record);
            signals.Trigger("Fields_Changed", record.Id);
            signals.Trigger("FieldsOfList_Changed", record.ListId);
        }

        public IList<IListField> GetFields(int listId)
        {
            return cacheManager.Get("Fields_GetFields_" + listId, ctx =>
            {
                ctx.Monitor(signals.When("FieldsOfList_Changed"));
                var records = Repository.Table.Where(x => x.ListId == listId).OrderBy(x => x.Position).ToList();
                var result = new List<IListField>();

                var settings = new SharpSerializerXmlSettings
                {
                    IncludeAssemblyVersionInTypeName = false,
                    IncludeCultureInTypeName = false,
                    IncludePublicKeyTokenInTypeName = false
                };

                var sharpSerializer = new SharpSerializer(settings);

                foreach (var record in records)
                {
                    if (string.IsNullOrEmpty(record.FieldProperties))
                    {
                        continue;
                    }

                    var field = (IListField)sharpSerializer.DeserializeFromString(record.FieldProperties);
                    field.Id = record.Id;
                    result.Add(field);
                }

                return result;
            });
        }
    }
}