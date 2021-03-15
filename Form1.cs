using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapGeneratorV3
{
    public partial class Form1 : Form
    {
        public struct Tile
        {
            public int x, y, type;
        };

        Tile[,] tileMap = new Tile[200, 200];

        List<Tile> tiles = new List<Tile>();

        Graphics graphics;

        SolidBrush[] brushes = { new SolidBrush(Color.White), new SolidBrush(Color.DarkBlue), new SolidBrush(Color.Red) };

        Pen pen = new Pen(Color.Black);

        Random random;

        public int tileSize;

        public int xCount;

        public int yCount;

        public int fillPercent;

        public bool useCustomSeed;

        public void UpdateInformation()
        {
            tileSize = (int)numericUpDown1.Value;
            xCount = (int)numericUpDown2.Value;
            yCount = (int)numericUpDown3.Value;
            fillPercent = (int)numericUpDown4.Value;
            useCustomSeed = checkBox1.Checked;
        }

        public void WriteMapTilesToList()
        {
            tiles.Clear();

            for (int x = 0; x < xCount; x++)
                for (int y = 0; y < yCount; y++)
                    tiles.Add(tileMap[x, y]);
        }

        List<Tile> fillDrawnTiles = new List<Tile>();

        public void Fill(int x, int y, int replaceWhat, int replaceWith)
        {
            if(x >= 0 && x < xCount && y >= 0 && y < yCount && tileMap[x,y].type == replaceWhat)
            {
                tileMap[x, y].type = replaceWith;
                fillDrawnTiles.Add(tileMap[x, y]);

                Fill(x + 1, y, replaceWhat, replaceWith);
                Fill(x - 1, y, replaceWhat, replaceWith);
                Fill(x, y + 1, replaceWhat, replaceWith);
                Fill(x, y - 1, replaceWhat, replaceWith);
            }

            WriteMapTilesToList();
        }

        public void GenerateMap(int seed, int _fillPercent)
        {
            random = new Random(seed);

            for (int x = 0; x < xCount; x++)
                for (int y = 0; y < yCount; y++)
                {
                    int randomNumber = random.Next(0, 100) < _fillPercent ? 1 : 0;

                    tileMap[x, y].x = x * (tileSize + 1);
                    tileMap[x, y].y = y * (tileSize + 1);
                    tileMap[x, y].type = randomNumber;
                }

            WriteMapTilesToList();
        }

        public void DrawTiles(List<Tile> _tiles)
        {
            graphics = panel1.CreateGraphics();

            foreach (Tile a in _tiles)
            {
                graphics.FillRectangle(brushes[a.type], a.x, a.y, tileSize, tileSize);
            }
        }

        public void DrawGrid()
        {
            graphics.DrawLine(pen, 0, 0, (tileSize + 1) * xCount - 1, 0);
            graphics.DrawLine(pen, 0, 0, 0, (tileSize + 1) * yCount - 1);

            for (int x = 0; x <= xCount; x++)
                graphics.DrawLine(pen, (tileSize + 1) * x - 1, 0, (tileSize + 1) * x - 1, (tileSize + 1) * yCount - 1);

            for (int y = 0; y <= yCount; y++)
                graphics.DrawLine(pen, 0, (tileSize + 1) * y - 1, (tileSize + 1) * xCount - 1, (tileSize + 1) * y - 1);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateInformation();

            int seed = useCustomSeed == true ? textBox1.Text.GetHashCode() : Environment.TickCount;

            GenerateMap(seed, fillPercent);
            DrawTiles(tiles);  
                      
            DrawGrid();
                        
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            Point cursorXY = panel1.PointToClient(Cursor.Position);

            fillDrawnTiles.Clear();
            Fill(cursorXY.X / (tileSize + 1), cursorXY.Y / (tileSize + 1), 0, 2);
            DrawTiles(fillDrawnTiles);

            DrawGrid();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = checkBox1.Checked;
        }
    }
}
