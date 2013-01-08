using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iSpreadsheets.Helpers
{
    public class CellName
    {
        public string FullName { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }

        private CellName() { }

        public CellName(string name, int row, int col)
        {
            this.FullName = name;
            this.Row = row;
            this.Col = col;
        }
    }
}
