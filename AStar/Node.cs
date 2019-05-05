using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    class Node
    {
        public object obj;

        public Node parent;
        public Neightbour[] neighbours;

        public double h;
        public double g;

        //public double F => g + h;

        public string name;

        public override string ToString() => name;
    }
}
