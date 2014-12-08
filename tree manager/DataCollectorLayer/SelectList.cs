using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectorLayer
{
    public class SelectList
    {
        int fromAge;

        public int FromAge
        {
            get { return fromAge; }
        }
        int toAge;

        public int ToAge
        {
            get { return toAge; }
        }
        string name;

        public string Name
        {
            get { return name; }
        }
        string illName;

        public string IllName
        {
            get { return illName; }
        }
        string species;

        public string Species
        {
            get { return species; }
        }

        public SelectList(int FAge=0, int TAge=100000, string n="", string iN="", string s="")
        {
            fromAge = FAge;
            toAge = TAge;
            name = n;
            illName = iN;
            species = s;
        }
    }
}
