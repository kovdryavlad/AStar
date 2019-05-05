using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStar
{
    class CellForAStar
    {
        public CellForAStar(DataGridViewCell cell, int[] coords)
        {
            this.cell = cell;
            this.coords = coords;
        }
        public DataGridViewCell cell;
        public int[] coords;
    }
}
