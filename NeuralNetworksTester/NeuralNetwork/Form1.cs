using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Threading;
using NeuralNetworksLib;
using NeuralNetworksLib.NetworkModels;
using ZachLib.Statistics;

namespace NeuralNet
{
    public partial class Form1 : Form
    {
        private NeuralNetworksLib.NetworkModels.NeuralNetwork nn = null;
        private static readonly object _nnLock = new object();
        private static Dispatcher UIThread = null;
        private Thread thread = null;
        private Graphics gfx = null;
        private volatile bool treeActive = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            nn = new NeuralNetworksLib.NetworkModels.NeuralNetwork(0.9, 0, 0, new int[] { 3, 3, 1 });
            ttpEpochs.SetToolTip(prgEpochs, "Not Running");
            ttpEpochs.Active = true;
            UIThread = Dispatcher.CurrentDispatcher;
            treeView1.TopNode = nn.ToTreeNode();
            gfx = nn.CreateDrawing(pictureBox1, 400, 100, 1.5f);
        }

        private void UpdateForm()
        {
            nn.UpdateTreeNode(treeView1.TopNode);
            nn.UpdateDrawing(gfx);
        }

        private static readonly Matrix inputs = new Matrix(
            new double[][]
            {
                new double[]
                {
                    0, 0, 1
                },
                new double[]
                {
                    1, 1, 1
                },
                new double[]
                {
                    1, 0, 1
                },
                new double[]
                {
                    0, 1, 1
                }
            }
        );
        private static readonly Matrix outputs = new Matrix(
            new double[][]
            {
                new double[] {0},
                new double[] {1},
                new double[] {1},
                new double[] {0}
            }
        );
        private void btnTrain_Click(object sender, EventArgs e)
        {
            var series = new Series();
            series.ChartType = SeriesChartType.Line;
            series.Color = Color.Red;
            chtError.Series.Add(series);

            thread = new Thread(
                new ThreadStart(
                    () =>
                    {
                        for (int i = 0; i < 12000; ++i)
                        {
                            nn.Train(inputs, outputs, 5);
                            UIThread.Invoke(
                                () =>
                                {
                                    prgEpochs.Value += 5;
                                    ttpEpochs.SetToolTip(prgEpochs, prgEpochs.Value.ToString() + " Epochs");
                                    series.Points.AddXY(i, nn.CurrentError);
                                }
                            );
                            if (i % 1000 == 0)
                                UIThread.Invoke(() => UpdateForm());
                            else if (i % 200 == 0)
                                UIThread.Invoke(() => nn.UpdateTreeNode(treeView1.TopNode));
                        }
                    }
                )
            );
            thread.Priority = ThreadPriority.AboveNormal;
            thread.Start();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            double[] checks = new double[3];
            foreach(var checkedIndex in chkLstInputs.CheckedIndices)
            {
                checks[(int)checkedIndex] = 1;
            }

            var outputs = nn.Run(checks);
            chkOutput.Checked = ((int)Math.Round(outputs[0])) == 1;
            UpdateForm();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_Enter(object sender, EventArgs e)
        {
            treeActive = true;
        }

        private void treeView1_Leave(object sender, EventArgs e)
        {
            treeActive = false;
        }
    }

}