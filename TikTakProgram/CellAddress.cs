using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TikTakProgram
{
    public struct CellAddress
    {
        public int Row { get; }
        public string Column { get; }
        public CellAddress(string column, int row)
        {
            Column = column;
            Row = row;
        }

        public override string ToString()
        {
            return $"{(Column)}{Row}";
        }
    }
}
