using System.ComponentModel;
using SendManager;

namespace DataCollectorLayer
{
    public interface IDataProvider
    {
        DataGrid GetData(int chunkSize, int startFrom, int pageSize);
        string[] GetHeader();
        void InitializeReading(object sender, DoWorkEventArgs e);
    }
}
