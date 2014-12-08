using System.ComponentModel;
using System.Data.Common;


namespace SendManager
{
    public interface IDataFromDatabaseReader
    {
        void ReadData(object sender, DoWorkEventArgs e, int chunkSize,int amountOfRecordsToGet);
        void OpenConnection(DbConnection connection, string query);
        string[] GetHeader();
    }
}
