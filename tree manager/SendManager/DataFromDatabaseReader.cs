using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using log4net;


namespace SendManager
{
    public class DataFromDatabaseReader : IDataFromDatabaseReader
    {
        private DbConnection _connection;
        private DbDataReader _dbReader;
        private bool _isOpen;

        public DataFromDatabaseReader()
        {
            _isOpen = false;
        }

        public void OpenConnection(DbConnection connection, string query)
        {
            if (string.IsNullOrEmpty(query))
                throw new NullReferenceException("query is empty");
            _connection = connection;
            using (DbCommand dbCommand = _connection.CreateCommand())
            {
               dbCommand.CommandText = query;

                    _connection.Open();

                    _dbReader = dbCommand.ExecuteReader();

                    _isOpen = true;
            }
        }

        public string[] GetHeader()
        {
            var headers = new List<String>();
            if (_isOpen)
            {
                if (_dbReader.RecordsAffected<0)
                {
                    int fieldCount = _dbReader.FieldCount;

                    for (int i = 0; i < fieldCount; i++)
                    {
                        // pobiera nazwę, gdy nazwa już istnieje na liście tworzy jej przeładowanie dodając index
                        // np adres, adres2
                        string name = _dbReader.GetName(i);
                        var overloadedName = GetOverloadedName(name, headers);

                        var finalName = (overloadedName != name) ? overloadedName : name;

                        if (string.IsNullOrEmpty(finalName) && finalName!="1")
                            finalName = "result:";

                        headers.Add(finalName);
                    }
                }
                else 
                    headers.Add("Querry Result:");
            }
            else
            {
                headers.Add("Connection has failed.");
            }
            return headers.ToArray();
        }

        private static string GetOverloadedName(string name, List<string> headers)
        {
            int nameMultipler = 0;
            string overloadedName = name;
            while (headers.Contains(overloadedName))
            {
                nameMultipler++;
                overloadedName = name + nameMultipler;
            }
            return overloadedName;
        }

        private int _chunkSize;
        public void ReadData(object sender, DoWorkEventArgs e,int chunkSize, int amountOfRecordsToGet)
        {
            var worker = sender as BackgroundWorker;
            if (worker == null)
                return;
            _chunkSize = chunkSize;

            int recordsDownloaded = 0;
            IBackupData data = new FileDumper(_chunkSize);
            if (_isOpen)
            {

                int fieldCount = _dbReader.FieldCount;
                var dataGrid = new DataGrid(0, fieldCount);
                if (_dbReader.HasRows)
                {
                    while (true)
                    {
                        dataGrid.Rows.Clear();

                        if (worker.WorkerSupportsCancellation && (worker.CancellationPending))
                        {
                            e.Cancel = true;
                            LogManager.GetLogger("info").Info("query canceled, records gathered: " + recordsDownloaded);
                            break;
                        }
                        int x = 0;
                        while (x < _chunkSize && _dbReader.Read())
                        {
                            var row = new object[fieldCount];

                            for (var i = 0; i < fieldCount; i++)
                            {
                                row[i] = _dbReader.GetValue(i);
                            }
                            dataGrid.Rows.Add(row);
                            x++;
                        }
                        if (x < _chunkSize)
                        {
                            dataGrid.Row = x;
                            if(worker.WorkerReportsProgress)
                            worker.ReportProgress(recordsDownloaded + x);
                            data.AddData(dataGrid, recordsDownloaded);

                            LogManager.GetLogger("info").Info("query finished, records gathered: "+(recordsDownloaded+x));
                            break;
                        }
                        dataGrid.Row = x;
                        data.AddData(dataGrid, recordsDownloaded);

                        if((recordsDownloaded/_chunkSize)%10==0)
                            if (worker.WorkerReportsProgress)
                            worker.ReportProgress(recordsDownloaded);

                        recordsDownloaded += _chunkSize;
                    }

                }
                else
                {
                    dataGrid.Row = 1;
                    if (_dbReader.RecordsAffected > 0)
                        dataGrid.Rows.Add(new object[] { "Affected rows: " + _dbReader.RecordsAffected });
                    else
                        dataGrid.Rows.Add(new object[] { "Brak wyników" });
                    if (worker.WorkerReportsProgress)
                    worker.ReportProgress(1);
                    data.AddData(dataGrid, 0);

                    LogManager.GetLogger("info").Info(dataGrid.Rows[0][0]);

                }

            }
            else
                LogManager.GetLogger("Error").Error("attempting to read closed database.");
        }
    }
}