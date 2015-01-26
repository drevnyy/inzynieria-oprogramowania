using System.ComponentModel;
using System.Data;
using System.Data.Common;
using SendManager;

namespace DataCollectorLayer
{
    public class DataProvider : IDataProvider
    {
        private readonly IDataFromDatabaseReader _databaseAccess;
        private readonly IBackupData _backupData;
        private readonly int _chunkSize;
        private readonly int _recordsOnScreen;
        public DataProvider(string query, IDbConnection con, IBackupData backupData, 
            IDataFromDatabaseReader databaseReader, int chunkSize, int recordsOnScreen)
        {
            _chunkSize = chunkSize;
            _recordsOnScreen = recordsOnScreen;
            _backupData = backupData;
            _backupData.ClearBackup();
            _databaseAccess = databaseReader;
            var connection = (DbConnection)con;
            if (connection != null)
                _databaseAccess.OpenConnection(connection, query);
        }
        public DataProvider(SelectList list, IDbConnection con, IBackupData backupData,
    IDataFromDatabaseReader databaseReader, int chunkSize, int recordsOnScreen,IStringParser parser)
        {
            string query=parser.Parse(list);
            _chunkSize = chunkSize;
            _recordsOnScreen = recordsOnScreen;
            _backupData = backupData;
            _backupData.ClearBackup();
            _databaseAccess = databaseReader;
            var connection = (DbConnection)con;
            if (connection != null)
                _databaseAccess.OpenConnection(connection, query);
        }
        /// <summary>
        /// rozpoczyna pobieranie danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InitializeReading(object sender, DoWorkEventArgs e)
        {
            _databaseAccess.ReadData(sender, e, _chunkSize, _recordsOnScreen);
        }
        /// <summary>
        /// zwraca określoną partię danych
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="startFrom"></param>
        /// <param name="chunkSize"></param>
        /// <returns>datagrid</returns>
        public DataGrid GetData(int pageSize, int startFrom, int chunkSize)
        {
            startFrom = startFrom < 0 ? 0 : startFrom;
            DataGrid data = _backupData.GetData(startFrom, pageSize);

            return data;
        }
        /// <summary>
        /// zwraca nagłówek
        /// </summary>
        /// <returns>header</returns>
        public string[] GetHeader()
        {
            return _databaseAccess.GetHeader();
        }
    }
}
