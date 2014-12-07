using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using DataCollectorLayer;
using SendManager;

namespace tree_manager.viewModel
{


    public class VirtualizedPageGrid : VirtualList<object[]>
    {
        private readonly IDataProvider _dataReader;
        private readonly int _startFrom;
        public VirtualizedPageGrid(int startFrom, IDataProvider provider, int maxRowCount, IBackupData backupData)
            : base(maxRowCount, backupData)
        {
            _startFrom = startFrom;
            _dataReader = provider;
        }

        public override string[] GetCaptions()
        {
            return _dataReader.GetHeader();
        }

        public override Page<object[]> GetPage(int pageNumber)
        {
            if (!CanLoadPage(pageNumber, BackupData))
            {
                return null;
            }

            var newPageSize = Math.Min(Count - pageNumber * PageSize, PageSize);

            var neededData = _dataReader.GetData(newPageSize, _startFrom + PageSize * pageNumber, PageSize);
            var newPage = new Page<object[]>();

            for (int row = 0; row < newPageSize && row < neededData.Row; row++)
            {
                newPage.Add(neededData.Rows[row]);
            }
            return newPage;

        }

        private bool CanLoadPage(int pageNumber, IBackupData dumper)
        {
            bool exist = new BackupDataCounter().RecordsInBackup(dumper, _startFrom + pageNumber * PageSize, PageSize) == PageSize;
            return exist;
        }

        public void TrimSize(int recordsCached)
        {
            if (recordsCached < Count)
                Count = recordsCached;
        }

        public ObservableCollection<DataGridColumn> GetColumns()
        {
            var columns = new ObservableCollection<DataGridColumn>();
            for (int i = 0; i < Columns.Length; i++)
            {
                columns.Add(new DataGridTextColumn
                {
                    Header = Columns[i],
                    Binding = new Binding(string.Format(".[{0}]", i)) { Mode = BindingMode.OneWay }
                });
            }
            return columns;
        }

    }
}
