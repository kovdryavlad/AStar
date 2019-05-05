using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    class AStar
    {
        Node m_start;
        Node m_goal;
        Action<Node> reconstructAction;
        public Action invaliddateAct;

        public AStar(Action<Node> reconstructAction)
        {
            this.reconstructAction = reconstructAction;
        }

        public bool A(Node start, Node goal, Func<Node, double> heuristic)
        {
            m_start = start;
            m_goal = goal;

            List<Node> open = new List<Node>();
            List<Node> closed = new List<Node>();

            open.Add(start);

            while (open.Count > 0)
            {
                invaliddateAct();

                open = open.OrderBy(el => el.g + heuristic(el)).ToList();
                Node p = open[0];

                if (p == goal)
                    //    return reconstructPath(goal);
                    return true;

                open.Remove(p);
                closed.Add(p);

                //debug
                //(p.obj as CellForAStar).cell.Style.BackColor = System.Drawing.Color.LightGray;
                //(p.obj as CellForAStar).cell.Value = "";

                foreach (var neightbour in p.neighbours)
                {
                    Node neighbourNode = neightbour.node;

                    if (closed.Contains(neightbour.node))
                        continue;

                    double gOfthisWay = p.g + neightbour.cost;

                    if (!open.Contains(neightbour.node))
                    {
                        neighbourNode.parent = p;
                        neighbourNode.g = gOfthisWay;
                        open.Add(neighbourNode);

                        //debug
                        //(neighbourNode.obj as CellForAStar).cell.Value = "o";
                    }
                    else if (gOfthisWay < neighbourNode.g)
                    {
                        neighbourNode.parent = p;
                        neighbourNode.g = gOfthisWay;
                    }
                }
            }

            //решения нет
            return false;
        }

        public string reconstructPath()
        {
            string res = "";
            Action<Node> add2APath = n => res += n.ToString() + " -> ";

            Node head = m_goal;
            while (head != m_start)
            {
                add2APath(head);

                //
                reconstructAction?.Invoke(head);

                head = head.parent;
            }

            add2APath(m_start);

            return res;
        }
    }
}
