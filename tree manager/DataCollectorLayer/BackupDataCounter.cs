using SendManager;

namespace DataCollectorLayer
{
    public class BackupDataCounter
    {
        public int RecordsInBackup(IBackupData backupSource, int startFrom, int targetrecordAmount)
        {
            return backupSource.RecordsInBackup(startFrom, targetrecordAmount);
        }
    }
}
