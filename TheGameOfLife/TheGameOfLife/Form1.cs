using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheGameOfLife
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static Thread GameOfLifeThread = null;
        public static GameOfLife game = null;

        private void lblGame_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            game = new GameOfLife(
                
            );
        }
    }

    public class GameOfLife
    {
        private char[][] Grid { get; set; }
        private char Player { get; set; }
        private char Opponent { get; set; }
        private BackgroundWorker GameThread { get; set; }

        public GameOfLife(string text, char player)
        {
            this.Grid = text.Split('\n').Select(l => l.ToCharArray()).ToArray();
            this.Player = player;
            this.Opponent = player == 'w' ? 'b' : 'w';
            this.GameThread = new BackgroundWorker();
        }

        private void SetupGrid()
        {
            Random gen = new Random();

            String.IsNullOrWhiteSpace(txtInput.Text) ?
                    Enumerable.Range(0, 29).Select(
                        i => new String(
                            Enumerable.Range(0, 29).Select(
                                j =>
                                {
                                    double num = gen.NextDouble();
                                    double sideFactor = ((double)(i * j)) / 784.0;

                                }
                            )
                        )
                    );
        }

        private void GameThread_DoTick(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 29; ++i)
            {
                for (int j = 0; j < 29; ++j)
                {
                    char cellState = Grid[i][j];
                    IEnumerable<char> surroundingCells = this.GetSurroundingCells(i, j);
                    int aliveCount = surroundingCells.Count(c => c != '-');

                    if (cellState == '-' && aliveCount == 3)
                        Grid[i][j] = surroundingCells.Count(c => c == 'w') > 1 ? 'w' : 'b';
                    else if (aliveCount > 3 || aliveCount < 2)
                        Grid[i][j] = '-';
                }
            }
        }

        private IEnumerable<char> GetSurroundingCells(int x, int y)
        {
            for (int i = Math.Max(0, x - 1); i <= Math.Min(28, x + 1); ++i)
            {
                for (int j = Math.Max(0, y - 1); i <= Math.Min(28, j + 1); ++j)
                {
                    if (i == x && y == j)
                        continue;
                    yield return Grid[i][j];
                }
            }
        }
    }
}
