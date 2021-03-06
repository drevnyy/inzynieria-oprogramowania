﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;


namespace SendManager
{
    public class Sender : ISender
    {
        public DataTable Send<T, TS>(string connectionString, string query, int start, int range)
            where TS : DbDataAdapter, IDbDataAdapter, IDisposable, new()
            where T : IDbConnection, new()
        {
            try
            {
                var connection = new T { ConnectionString = @connectionString };
                var sqlDataAdapter = new TS();
                connection.Open();
                using (sqlDataAdapter)
                {
                    using (sqlDataAdapter.SelectCommand = (DbCommand)connection.CreateCommand())
                    {
                        sqlDataAdapter.SelectCommand.CommandText = query;
                        var ds = new DataSet();
                        sqlDataAdapter.Fill(ds, start, range, "Table");
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception e)
            {
                var view = new DataTable { TableName = "Table" };
                view.Columns.Add(new DataColumn("wynik"));
                DataRow workRow = view.NewRow();
                workRow[0] = e.Message.ToString(CultureInfo.CreateSpecificCulture("en-us"));
                view.Rows.Add(workRow);
                return (view);
            }
        }

        public int CheckSize<T, TS>(string connectionString, string query)
            where TS : DbDataAdapter, IDisposable, new()
            where T : IDbConnection, new()
        {
            try
            {
                var connection = new T { ConnectionString = @connectionString };
                var sqlDataAdapter = new TS();
                connection.Open();
                using (sqlDataAdapter)
                {
                    using (sqlDataAdapter.SelectCommand = (DbCommand)connection.CreateCommand())
                    {
                        try
                        {
                            sqlDataAdapter.SelectCommand.CommandText = query;
                            var ds = new DataSet();
                            sqlDataAdapter.Fill(ds, 0, 10, "Table");
                            if (ds.Tables.Count==0)
                                return -1;
                            return GetObjectSize(ds.Tables[0]);
                        }
                        catch (SqlNotFilledException)
                        {
                            return 0;
                        }
                    }
                }
            }
            catch (DbException)
            {

                return 0;
            }
        }

        private int GetObjectSize(object testObject)
        {

            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, testObject);
            byte[] array = ms.ToArray();
            return array.Length;
        }
    }
}