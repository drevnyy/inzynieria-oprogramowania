using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectorLayer
{
    public interface IStringParser
    {
        string Parse(SelectList list);
    }
}
