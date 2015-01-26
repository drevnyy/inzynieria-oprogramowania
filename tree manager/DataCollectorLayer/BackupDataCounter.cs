using SendManager;

namespace DataCollectorLayer
{
    /**
     * sprawdza ile danych jest zcatchowanych na dysku
     */
    public class BackupDataCounter
    {
        public int RecordsInBackup(IBackupData backupSource, int startFrom, int targetrecordAmount)
        {
            return backupSource.RecordsInBackup(startFrom, targetrecordAmount);
        }
    }
}
