using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectorLayer
{
    public class Sqlparser : IStringParser
    {
        public string Parse(SelectList list)
        {
            string wiek="";
            if (list.ToAge > list.FromAge)
                wiek = " wiek BETWEEN " + list.FromAge + " AND " + list.ToAge;
            else
                wiek = " wiek BETWEEN " + list.FromAge + " AND 100000";
            string nazwa = (!string.IsNullOrEmpty(list.Name) ? (" AND nazwa = '"+list.Name+"' ") : "");
            string nazwaChoroby = (!string.IsNullOrEmpty(list.IllName) ? (" AND nazwa_choroby = '" + list.IllName + "' ") : "");
            string gatunek = (!string.IsNullOrEmpty(list.Species) ? (" AND odmiana = '" + list.Species + "' ") : "");
            string where = "WHERE (";
            string footer = ");";

            return "SELECT * FROM trees " +
                where +
                wiek +
                nazwa +
                nazwaChoroby+
                gatunek+
                footer;
        }
    }
}
