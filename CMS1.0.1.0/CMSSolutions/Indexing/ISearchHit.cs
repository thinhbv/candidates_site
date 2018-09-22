using System;

namespace CMSSolutions.Indexing
{
    public interface ISearchHit
    {
        Guid ItemId { get; }

        float Score { get; }

        int GetInt(string name);

        double GetDouble(string name);

        bool GetBoolean(string name);

        string GetString(string name);

        DateTime GetDateTime(string name);
    }
}