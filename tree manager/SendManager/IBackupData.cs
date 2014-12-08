namespace SendManager
{
    public interface IBackupData
    {
        void ClearBackup();
        void AddData(DataGrid cachedRecords, int startFrom);
        DataGrid GetData(int startFrom, int pageSize);
        int RecordsInBackup(int startFrom, int targetRecordAmount);
    }
}
