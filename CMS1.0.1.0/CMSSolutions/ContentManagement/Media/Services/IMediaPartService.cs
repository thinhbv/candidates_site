using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.ContentManagement.Media.Domain;
using CMSSolutions.ContentManagement.Media.Models;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Media.Services
{
    public interface IMediaPartService : IDependency
    {
        Guid GetMediaPartType<TKey>(BaseEntity<TKey> entity);

        IList<TMediaPart> GetMediaParts<TKey, TMediaPart>(BaseEntity<TKey> entity) where TMediaPart : IMediaPart, new();

        void SetMediaParts<TKey>(BaseEntity<TKey> entity, IEnumerable<IMediaPart> mediaParts);
    }

    [Feature(Constants.Areas.Media)]
    public class MediaPartService : IMediaPartService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;
        private readonly IRepository<MediaPart, Guid> mediaPartRepository;
        private readonly IRepository<MediaPartType, Guid> mediaPartTypeRepository;

        public MediaPartService(IRepository<MediaPart, Guid> mediaPartRepository, IRepository<MediaPartType, Guid> mediaPartTypeRepository, ICacheManager cacheManager, ISignals signals)
        {
            this.mediaPartRepository = mediaPartRepository;
            this.mediaPartTypeRepository = mediaPartTypeRepository;
            this.cacheManager = cacheManager;
            this.signals = signals;
        }

        public Guid GetMediaPartType<TKey>(BaseEntity<TKey> entity)
        {
            var type = GetFullTypeName(entity.GetType());

            var types = cacheManager.Get("MediaPartTypes_GetAllTypes", ctx =>
            {
                ctx.Monitor(signals.When("MediaPartTypes_Changed"));

                return mediaPartTypeRepository.Table.ToList();
            });

            var part = types.FirstOrDefault(x => x.Type == type);
            if (part != null)
            {
                return part.Id;
            }

            part = new MediaPartType
                       {
                           Id = Guid.NewGuid(),
                           Type = type
                       };

            mediaPartTypeRepository.Insert(part);
            signals.Trigger("MediaPartTypes_Changed");

            return part.Id;
        }

        public IList<TMediaPart> GetMediaParts<TKey, TMediaPart>(BaseEntity<TKey> entity) where TMediaPart : IMediaPart, new()
        {
            var id = entity.Id.GetHashCode();
            var partTypeId = GetMediaPartType(entity);
            var records = mediaPartRepository.Table.Where(x => x.MediaPartTypeId == partTypeId && x.ParentId == id).OrderBy(x => x.SortOrder).ToList();
            return records.Select(x => new TMediaPart
                                           {
                                               Url = x.Url,
                                               Caption = x.Caption,
                                               SortOrder = x.SortOrder
                                           }).ToList();
        }

        public void SetMediaParts<TKey>(BaseEntity<TKey> entity, IEnumerable<IMediaPart> mediaParts)
        {
            var mediaPartType = GetMediaPartType(entity);
            var parentId = entity.Id.GetHashCode();
            //delete old media parts
            var records = mediaPartRepository.Table.Where(x => x.MediaPartTypeId == mediaPartType && x.ParentId == parentId).OrderBy(x => x.SortOrder).ToList();
            mediaPartRepository.DeleteMany(records);

            // Insert new parts
            foreach (var mediaPart in mediaParts.Where(mediaPart => !string.IsNullOrEmpty(mediaPart.Url)))
            {
                var record = new MediaPart
                {
                    Id = Guid.NewGuid(),
                    Url = mediaPart.Url,
                    Caption = mediaPart.Caption,
                    SortOrder = mediaPart.SortOrder,
                    MediaPartTypeId = mediaPartType,
                    ParentId = parentId
                };
                mediaPartRepository.Insert(record);
            }
        }

        private static string GetFullTypeName(Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }
    }
}