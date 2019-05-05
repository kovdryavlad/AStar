using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    class Neightbour
    {
        public Node node;
        public double cost;

        public Neightbour(Node node, double cost)
        {
            this.node = node;
            this.cost = cost;
        }
    }
}
