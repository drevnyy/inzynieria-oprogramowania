using System;
using System.Data;
using System.Data.Common;


namespace SendManager
{
    public interface ISender
    {
        DataTable Send<T, TS>(string connectionString, string query, int start, int range)
            where TS : DbDataAdapter, IDbDataAdapter, IDisposable, new()
            where T : IDbConnection, new();

        int CheckSize<T, TS>(string connectionString, string query)
            where TS : DbDataAdapter, IDisposable, new()
            where T : IDbConnection, new();

        //[CR][JD] Co oznacza poniższy komentarz?

    }
}
