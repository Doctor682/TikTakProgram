using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTakProgram
{
    public class TikTakBoard
    {
        public Dictionary<CellAddress, Cell> _cellMap = new();
        public BoardDimensions Dimensions { get; }
        public TikTakBoard(BoardDimensions dims) => Dimensions = dims;

        public char this[int row, int col]
        {
            get
            {
                CellAddress addr = MakeAddress(row, col);
                return _cellMap.TryGetValue(addr, out var cell) ? cell.Value : '\0';
            }
            set
            {
                CellAddress addr = MakeAddress(row, col);
                if (_cellMap.ContainsKey(addr))
                    _cellMap[addr].Value = value;
                else
                    _cellMap[addr] = new Cell(value);
            }
        }

        private static CellAddress MakeAddress(int row, int col)
            => new CellAddress(((char)('A' + col)).ToString(), row + 1);
    }
}
