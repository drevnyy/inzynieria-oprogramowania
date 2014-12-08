using System.Collections.Generic;

namespace SendManager
{
    public class DataGrid
    {
        public List<object[]> Rows { get; set; }
        public int Row { get; set; }
        public int Column{ get; private set; }

        public DataGrid(int row, int column)
        {
            Row=row;
            Column=column;
            Rows=new List<object[]>(); 
            for(int i=0; i<row; i++)
                Rows.Add(new object[column]);
        }

    }
}