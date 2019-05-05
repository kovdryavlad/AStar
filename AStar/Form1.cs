using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStar
{
    public partial class Form1 : Form
    {
        int m_columns = 44;
        int m_rows = 23;

        //int m_columns = 10;
        //int m_rows = 9;

        int[] startCoords = new int[2];     //x,y
        int[] finishCoords = new int[2];

        Node a = new Node(),
             b = new Node(),
             c = new Node(),
             d = new Node(),
             e = new Node(),
         start = new Node(),
        finish = new Node();

        public Form1()
        {
            InitializeComponent();

            fillDataGridfView();
            //TestMap();

            dataGridView1.KeyDown += DataGridView1_KeyDown;
            //TestNethod();
        }

        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            var grid = (DataGridView)sender;
            var currentCell = grid.CurrentCell;

            switch (e.KeyCode)
            {
                case Keys.Q: fillDataGridfView(); break;
                case Keys.B: SetBorder(currentCell); break;
                case Keys.R: RandomMap(); break;
                case Keys.S: SetStart(currentCell); break;
                case Keys.F: SetFinish(currentCell); break;
                case Keys.Space: Solve(); break;
            }
        }

        private void RandomMap()
        {
            fillDataGridfView();

            //int CounterMax = (m_rows + 1) * m_columns / 4;
            int CounterMax = (m_rows + 1) * m_columns / 3;
            Random r = new Random();

            for (int i = 0; i < CounterMax; i++)
            {
                int y = r.Next(0, m_rows + 1);
                int x = r.Next(0, m_columns);

                dataGridView1.Rows[y].Cells[x].Style.BackColor = Color.Orange;
            }
        }

        private void Solve()
        {
            //очищаем предыдущий вывод
            for (int i = 0; i <= m_rows; i++)
                for (int j = 0; j < m_columns; j++)
                    dataGridView1.Rows[i].Cells[j].Value = "";

            //формирование всех нод
            Node[,] nodes = new Node[m_rows + 1, m_columns];

            for (int i = 0; i <= m_rows; i++)
                for (int j = 0; j < m_columns; j++)
                {
                    var cellsForStar = new CellForAStar(dataGridView1.Rows[i].Cells[j], new[] { i, j });

                    var node = new Node();
                    node.obj = cellsForStar;
                    node.name = string.Format("i:{0}; j:{1}", i, j);
                    nodes[i, j] = node;
                }

            //расстановка соседей
            for (int i = 0; i <= m_rows; i++)
                for (int j = 0; j < m_columns; j++)
                    nodes[i, j].neighbours = getNeighbours(nodes, new[] { i, j });


            var start = nodes[startCoords[0], startCoords[1]];
            var finish = nodes[finishCoords[0], finishCoords[1]];

            Func<Node, double> heuristicDistance = (node) => {

                var cell1 = (finish.obj as CellForAStar).coords;
                var cell2 = (node.obj as CellForAStar).coords;

                int x1 = cell1[0];
                int y1 = cell1[1];

                int x2 = cell2[0];
                int y2 = cell2[1];

                //Euclidean
                //return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

                //Manhattan
                return Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
            };

            Action<Node> reconstructAction = node => (node.obj as CellForAStar).cell.Value = "zz";

            AStar solver = new AStar(reconstructAction);
            solver.invaliddateAct = () => {
              
               // this.Refresh();
            };

            bool solveExist = solver.A(start, finish, heuristicDistance);

            if (solveExist)
                solver.reconstructPath();
            else
                MessageBox.Show("Solution is not exist!");
        }

        private Neightbour[] getNeighbours(Node[,] cells, int[] coords)
        {
            List<Neightbour> neightbours = new List<Neightbour>();

            int y = coords[0], x = coords[1];

            Action<int, int> tryAddElement = (xCord, yCord) =>
            {
                if (CheckCoords(xCord, yCord))
                    neightbours.Add(new Neightbour(cells[yCord, xCord], 1));
            };

            //tryAddElement(x - 1, y - 1);
            tryAddElement(x - 1, y);
            //tryAddElement(x - 1, y + 1);

            tryAddElement(x, y - 1);
            tryAddElement(x, y + 1);

            //tryAddElement(x + 1, y - 1);
            tryAddElement(x + 1, y);
            //tryAddElement(x + 1, y + 1);

            return neightbours.Where(el => (el.node.obj as CellForAStar).cell.Style.BackColor != Color.Orange).ToArray();
        }

        bool CheckCoords(int x, int y) => x >= 0 && y >= 0 && x < m_columns && y <= m_rows;


        private void SetFinish(DataGridViewCell currentCell)
        {
            ClearCell(finishCoords);
            finishCoords = getCoordsOfCurrentCell();

            dataGridView1.Rows[finishCoords[0]].Cells[finishCoords[1]].Style.BackColor = Color.Blue;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void SetStart(DataGridViewCell currentCell)
        {
            ClearCell(startCoords);
            startCoords = getCoordsOfCurrentCell();

            dataGridView1.Rows[startCoords[0]].Cells[startCoords[1]].Style.BackColor = Color.Green;
        }

        int[] getCoordsOfCurrentCell() {

            int c = dataGridView1.CurrentCell.ColumnIndex;
            int r = dataGridView1.CurrentRow.Index;

            return new[] { r, c };
        }

        private void ClearCell(int[] crds) =>
            dataGridView1.Rows[crds[0]].Cells[crds[1]].Style.BackColor = Color.White;


        private static void SetBorder(DataGridViewCell currentCell)
        {
            //currentCell.Value = "zz";
            if (currentCell.Style.BackColor != Color.Orange)
                currentCell.Style.BackColor = Color.Orange;
            else
                currentCell.Style.BackColor = Color.White;
        }

        void fillDataGridfView()
        {
            dataGridView1.Columns.Clear();
            for (int i = 0; i < m_columns; i++)
            {
                var column = new DataGridViewTextBoxColumn();
                column.Width = 25;
                dataGridView1.Columns.Add(column);
            }

            dataGridView1.Rows.Clear();
            for (int i = 0; i < m_rows; i++)
            {
                //var row = new DataGridViewTextBoxColumn();
                dataGridView1.Rows.Add("");
            }

            startCoords = new int[] { 0, 0 };
            dataGridView1.Rows[startCoords[0]].Cells[startCoords[1]].Style.BackColor = Color.Green;

            finishCoords = new int[] { m_rows, m_columns - 1 };
            dataGridView1.Rows[finishCoords[0]].Cells[finishCoords[1]].Style.BackColor = Color.Blue;
        }

        //testMap
        void TestMap()
        {
            dataGridView1.Rows[4].Cells[2].Style.BackColor = Color.Orange;
            dataGridView1.Rows[4].Cells[3].Style.BackColor = Color.Orange;
            dataGridView1.Rows[4].Cells[4].Style.BackColor = Color.Orange;

            dataGridView1.Rows[6].Cells[4].Style.BackColor = Color.Orange;
            dataGridView1.Rows[7].Cells[4].Style.BackColor = Color.Orange;
            dataGridView1.Rows[8].Cells[4].Style.BackColor = Color.Orange;
            dataGridView1.Rows[9].Cells[4].Style.BackColor = Color.Orange;
        }
        private void TestNethod()
        {
            start.name = "start";
            start.neighbours = new[] {
                new Neightbour(a, 1.5),
                new Neightbour(d, 4.5)
            };

            finish.name = "finish";
            finish.neighbours = new[] {
                new Neightbour(c, 4),
                new Neightbour(e, 2)
            };

            a.h = 4;
            a.name = "a";
            a.neighbours = new[] {
                new Neightbour(b, 2),
                new Neightbour(a, 1.5)
            };

            b.h = 2;
            b.name = "b";
            b.neighbours = new[] {
                new Neightbour(a, 2),
                new Neightbour(c, 3)
            };

            c.h = 4;
            c.name = "c";
            c.neighbours = new[] {
                new Neightbour(b, 3),
                new Neightbour(finish, 4)
            };

            //
            d.h = 4.5;
            d.name = "d";
            d.neighbours = new[] {
                new Neightbour(a, 2),
                new Neightbour(e, 3)
            };

            e.h = 2;
            e.name = "e";
            e.neighbours = new[] {
                new Neightbour(d, 3),
                new Neightbour(finish, 2)
            };

            //AStar solver = new AStar();
            //solver.A(start, finish,  );
        }
    }
}
