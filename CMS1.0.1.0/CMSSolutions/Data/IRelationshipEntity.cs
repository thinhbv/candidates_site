namespace CMSSolutions.Data
{
    public interface IRelationshipEntity<TKey> where TKey : struct
    {
        TKey? ParentId { get; set; }
    }
}