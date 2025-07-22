using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram
{
    public struct BoardDimensions
    {
        public int RowSize;
        public int ColumnSize;

        public BoardDimensions()
        {
            ColumnSize = 3;
            RowSize = 3;
        }
    }
}
