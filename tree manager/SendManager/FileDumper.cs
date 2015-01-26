using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SendManager
{
    /**
     * cache danych na dysku
     */
    public class FileDumper : IBackupData
    {

        private readonly string _fileName;
        private readonly int _bufferSize;
        private const string CacheDirectory = "cache";
        public FileDumper(int bufferSize)
        {
            _bufferSize = bufferSize;
            Directory.CreateDirectory(CacheDirectory);
            _fileName = Path.Combine(CacheDirectory, "backupRecords.txt");
        }

        public void ClearBackup()
        {
            Directory.Delete(CacheDirectory, true);
            Directory.CreateDirectory(CacheDirectory);
        }

        public void AddData(DataGrid cachedRecords, int sFrom)
        {
            int startFrom = GetNewFileIndex(sFrom);
            if (!File.Exists(_fileName + startFrom))
            {
                var file = File.Open(_fileName + startFrom, FileMode.Create);
                file.Close();
            }

            using (TextWriter fileStream = new StreamWriter(_fileName + startFrom))
            {
                for (int i = 0; i < cachedRecords.Row && cachedRecords.Rows[i][0] != null; i++)
                {
                    fileStream.WriteLine(string.Join("\t", cachedRecords.Rows[i]));
                }
                fileStream.Close();
            }

        }
        /// <summary>
        /// zwraca indeks pliku z którego mamy pobrać dane
        /// </summary>
        /// <param name="startFrom"></param>
        /// <returns></returns>
        private int GetNewFileIndex(int startFrom)
        {
            return (startFrom - (startFrom % _bufferSize)) + ((startFrom % _bufferSize) > 0 ? _bufferSize : 0);
        }
        /// <summary>
        /// pobiera określone dane z pliku
        /// </summary>
        /// <param name="startFrom"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public DataGrid GetData(int startFrom, int pageSize)
        {
            int wantedFileBuffer = BufferIndexOfRecordChunk(startFrom + 1);
            int amountOfRecordsToSkip = SkipRecords(startFrom);
            int recordsToCach = pageSize;
            int recordsCached = 0;
            var width = GetWidth(wantedFileBuffer);
            var dataGrid = new DataGrid(0, width);
            while (File.Exists(_fileName + wantedFileBuffer) && recordsToCach > 0)
            {
                List<string> lines;
                using (TextReader file = new StreamReader(_fileName + wantedFileBuffer))
                {
                    lines = file.ReadToEnd().Split('\n').ToList();
                    file.Close();
                }
                lines.Remove(lines.Last());
                if (lines.Count == 0)
                    return dataGrid;

                int recordsInThisFile = Math.Min(recordsToCach, lines.Count - amountOfRecordsToSkip);
                if (!lines.Skip(amountOfRecordsToSkip).Take(recordsInThisFile).Any())
                    break;
                foreach (var line in lines.Skip(amountOfRecordsToSkip).Take(recordsInThisFile))
                    dataGrid.Rows.Add(line.TrimEnd('\r').Split('\t'));

                recordsCached += recordsInThisFile;
                recordsToCach = pageSize - recordsCached;

                wantedFileBuffer = BufferIndexOfRecordChunk(startFrom + recordsCached + 1);
                amountOfRecordsToSkip = SkipRecords(startFrom + recordsCached);
            }
            dataGrid.Row = recordsCached;

            return dataGrid;
        }

        private int GetWidth(int wantedFileBuffer)
        {
            if (File.Exists(_fileName + wantedFileBuffer))
            {
                string lines;
                using (TextReader file = new StreamReader(_fileName + wantedFileBuffer))
                {
                    lines = file.ReadLine();
                    file.Close();
                }
                if (lines == null)
                    return 0;

                return lines.TrimEnd('\r').Split('\t').Count();
            }
            return 0;
        }

        private int SkipRecords(int startFrom)
        {
            return startFrom % _bufferSize;
        }

        public int RecordsInBackup(int startFrom, int targetRecordAmount)
        {
            int targetStartBuffer = BufferIndexOfRecordChunk(startFrom + 1);
            int maxAmountOfExistingRecords = 0;
            while (File.Exists(_fileName + targetStartBuffer) && maxAmountOfExistingRecords<targetRecordAmount)
            {
                maxAmountOfExistingRecords += _bufferSize;
                targetStartBuffer = BufferIndexOfRecordChunk(startFrom + maxAmountOfExistingRecords + 1);
            }

            return maxAmountOfExistingRecords;

            
        }

        private int BufferIndexOfRecordChunk(int index)
        {
            return index - (index % _bufferSize);
        }
    }
}
