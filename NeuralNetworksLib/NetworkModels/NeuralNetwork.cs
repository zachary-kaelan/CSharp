using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeuralNetworksLib.Exceptions;
using NeuralNetworksLib.Templates;
using ZachLib;
using ZachLib.Statistics;

namespace NeuralNetworksLib.NetworkModels
{
    public class NeuralNetwork : NetworkParameters, IReadOnlyList<Layer>
    {
        private Layer[] Layers { get; set; }
        public double CurrentError { get; private set; }
        public Vector CurrentErrors { get; private set; }
        public int Count { get; private set; }
#if DEBUG
        private long[] Milliseconds { get; set; }
#endif
        private double[] Outputs { get; set; }
        public Layer this[int index] { get => Layers[index]; }
        public NetworkParameters Parameters { get; private set; }

        private NeuralNetwork(string folderPath) : base(Utils.LoadJSON<NeuralNetworkInfo>(folderPath + "\\info.json"))
        {
#if DEBUG
            Milliseconds = new long[4];
#endif
            string layersPath = folderPath + @"\\Layers";
            foreach(var layerPath in Directory.GetFiles(layersPath))
            {
                Layer layer = Layer.LoadFrom(layerPath, this);
                if (layer.Type != LayerType.Inputs)
                    layer.Initialize(this);
                Layers[layer.LayerIndex] = layer;
            }
        }

        public NeuralNetwork(double learningRate, double dropoutRate, double momentum, bool doBiases, int[] layers) : base(learningRate, dropoutRate, momentum, doBiases)
        {
#if DEBUG
            Milliseconds = new long[4];
#endif

            if (layers.Length < 2)
                throw new InvalidNetworkParametersException("Number of layers must be more than 2.");
            Initialize(layers);
        }

        private void Initialize(int[] layers)
        {
            Count = layers.Length;
            Layers = new Layer[Count];
            bool doBiases = Options.HasFlag(NetworkOptions.Biases);
            Layers[0] = new Layer(LayerType.Inputs, layers[0], layers[1]);
            for(int i = 1; i < Count; ++i)
            {
                int layerCount = layers[i];
                bool isOutput = i == Count - 1;
                int inputs = layers[i - 1];
                int outputs = !isOutput ? layers[i + 1] : 0;

                Layer lyr = null;
                if (isOutput)
                    lyr = new Layer(LayerType.Outputs, layerCount, i, inputs);
                else
                    lyr = new Layer(LayerType.Hidden, layerCount, i, inputs, outputs);
                lyr.Initialize(this);

                Layers[i] = lyr;
            }
        }

        public double Train(Matrix xValues, Matrix yValues, int numEpochs = 0)
        {
            /*double err = 0;
            if (NumIterations == 0)
                err = TrainEpoch(xValues, yValues);*/
            for (int e = 0; e < numEpochs; ++e)
            {
                TrainEpoch(xValues, yValues);
            }
            return GetError(xValues, yValues);
        }

        private void TrainEpoch(Matrix xValues, Matrix yValues)
        {
#if DEBUG
            Stopwatch timer = Stopwatch.StartNew();
#endif
            for(int i = 0; i < xValues.NumRows; ++i)
            {
                Train(xValues[i], yValues[i]);
            }
#if DEBUG
            timer.Stop();
            Milliseconds[3] += timer.ElapsedMilliseconds;
#endif
            ++NumIterations;
        }

        public double GetError(Matrix xValues, Matrix yValues)
        {
            //xValues[0].Va
            Vector sumOfSquaredDifferences = new Vector(yValues.NumCols);
            var outputLayer = Layers[Count - 1];
            if (Options.HasFlag(NetworkOptions.Dropout))
            {
                for (int i = 0; i < xValues.NumRows; ++i)
                {
                    var inputs = xValues[i];
                    for (int l = 1; l < Count - 1; ++l)
                    {
                        inputs = Layers[l].ForwardPropogate(inputs, true);
                    }
                    inputs = Layers[Count - 1].ForwardPropogate(inputs);
                    sumOfSquaredDifferences += outputLayer.Values.ElementWiseOp(
                        yValues[i], (output, expectedOutput) =>
                        {
                            var error = expectedOutput - output;
                            return error * error;
                        }
                    );
                }
            }
            else
            {
                for (int i = 0; i < xValues.NumRows; ++i)
                {
                    var inputs = xValues[i];
                    for (int l = 1; l < Count; ++l)
                    {
                        inputs = Layers[l].ForwardPropogate(inputs);
                    }
                    sumOfSquaredDifferences += outputLayer.Values.ElementWiseOp(
                        yValues[i], (output, expectedOutput) =>
                        {
                            var error = expectedOutput - output;
                            return error * error;
                        }
                    );
                }
            }

            Vector stdDeviations = sumOfSquaredDifferences.ElementWiseChange(s => Math.Sqrt(s / xValues.NumRows));
            return stdDeviations.Sum();
        }

#region Propogation
        public Vector Run(Vector inputs)
        {
            Layers[0].Values = inputs;
            for (int i = 1; i < Count; ++i)
            {
                inputs = Layers[i].ForwardPropogate(inputs);
            }
            Outputs = inputs;
            return Outputs;
        }
        
        private void Train(Vector xValues, Vector yValues)
        {
            ForwardPropogate(xValues);
            BackPropogate(yValues);
        }

        private void ForwardPropogate(Vector xValues)
        {
#if DEBUG
            var timer = Stopwatch.StartNew();
#endif
            Run(xValues);
#if DEBUG
            timer.Stop();
            Milliseconds[0] += timer.ElapsedMilliseconds;
            Milliseconds[2] += timer.ElapsedMilliseconds;
#endif
        }

        private void BackPropogate(Vector yValues)
        {
#if DEBUG
            var timer = Stopwatch.StartNew();
#endif
            var outputLayer = Layers[Count - 1];
            var errors = outputLayer.Values - yValues;
            var deltas = outputLayer.BackPropogate(errors);
            for (int i = Count - 2; i > 0; --i)
            {
                deltas = Layers[i].BackPropogate(deltas);
            }

            Layers[0].Deltas = deltas.ElementWiseOp(Layers[0].Values, (e, v) => e * (v * (1 - v)));
            CurrentError = (double)errors;
            CurrentErrors = errors;
#if DEBUG
            timer.Stop();
            Milliseconds[1] += timer.ElapsedMilliseconds;
            Milliseconds[2] += timer.ElapsedMilliseconds;
#endif
        }
        #endregion

#region Presentation
#if DEBUG
        private const string BENCHMARK_STRING_FORMAT = 
            "Forward Propogation: {0}\r\n" +
            "Backward Propogation: {1}\r\n" +
            "Propogation: {2}\r\n" +
            "Epoch of Training: {3}\r\n" +
            "Total: {4}";
        public string GetBenchmarkString()
        {
            var dblCounter = (double)NumIterations;
            return String.Format(
                BENCHMARK_STRING_FORMAT,
                Milliseconds[0] / dblCounter,
                Milliseconds[1] / dblCounter,
                Milliseconds[2] / dblCounter,
                Milliseconds[3] / dblCounter,
                Milliseconds[3]
            );
        }
#endif

        private static readonly Brush BlackSmoke = new SolidBrush(Color.FromArgb(Color.WhiteSmoke.A, 10, 10, 10));
        private static readonly Brush DarkYellow = new SolidBrush(Color.FromArgb(224, 224, 0));
        public Graphics CreateDrawing(PictureBox pbx, int X, int Y, float zoomFactor = 1.0f)
        {
            var drawingParams = new DrawingParameters();
            drawingParams.Initialize(this, pbx.Bounds);

            Bitmap pic = new Bitmap(pbx.Width, pbx.Height);
            Graphics gfx = Graphics.FromImage(pic);
            gfx.FillRectangle(Brushes.White, gfx.ClipBounds);
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            PointF[] prevLayerPoints = Array.Empty<PointF>();
            foreach(var layer in Layers)
            {
                prevLayerPoints = layer.CreateDrawing(gfx, drawingParams, prevLayerPoints);
            }

            pbx.Image = pic;
            return gfx;
        }

        public void UpdateDrawing(Graphics gfx)
        {
            foreach(var layer in Layers)
            {
                layer.UpdateDrawing(gfx);
            }
        }

        public TreeNode ToTreeNode()
        {
            TreeNode network = new TreeNode("Network");
            network.Name = "net";
            network.Nodes.Add("netCurErr", "Current Error: " + CurrentError.ToString("#.0000"));
            network.Nodes.Add("netLearnRate", "Learning Rate: " + LearningRate.ToString("#.0000"));
            network.Nodes.Add("netDropoutRate", "Dropout Rate: " + DropoutRate.ToString("#.0000"));
            network.Nodes.Add("netMomentum", "Momentum: " + Momentum.ToString("#.0000"));
            network.Nodes.Add(
                new TreeNode("Layers", Layers.Select(l => l.ToTreeNode()).ToArray())
                {
                    Name = "netLayers"
                }
            );
            return network;
        }

        public void UpdateTreeNode(TreeNode network)
        {
            network.Nodes[0].Text = "Current Error: " + CurrentError.ToString("#.0000");
            network.Nodes[1].Text = "Learning Rate: " + LearningRate.ToString("#.0000");
            network.Nodes[2].Text = "Dropout Rate: " + DropoutRate.ToString("#.0000");
            network.Nodes[3].Text = "Momentum: " + Momentum.ToString("#.0000");
            var layersNode = network.Nodes[4];
            for(int l = 0; l < Count; ++l)
            {
                Layers[l].UpdateTreeNode(layersNode.Nodes[l]);
            }
        }

        public IEnumerator<Layer> GetEnumerator()
        {
            return ((IReadOnlyList<Layer>)Layers).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyList<Layer>)Layers).GetEnumerator();
        }
#endregion

#region Files
        public long SaveAs(string folderPath)
        {
            var info = new NeuralNetworkInfo(this);
            var dirInfo = new DirectoryInfo(folderPath);
            bool append = dirInfo.Exists;

            if (!append)
            {
                dirInfo.Create();
                info.SaveAs(folderPath + "info.json");
                dirInfo.Refresh();
            }
            else
            {
                var oldInfo = Utils.LoadJSON<NeuralNetworkInfo>(folderPath + "info.json");

                switch (info.CompareTo(oldInfo))
                {
                    case 0:
                        return dirInfo.GetTotalSize();

                    case 1:
                        info.SaveAs(folderPath + "info.json");
                        dirInfo.Refresh();
                        return dirInfo.GetTotalSize();

                    case 2:
                        info.SaveAs(folderPath + "info.json");
                        break;

                }
            }

            string layersPath = folderPath + @"\\Layers";
            if (!append)
                Directory.CreateDirectory(layersPath);
            for (int l = 0; l < Count; ++l)
            {
                Layers[l].SaveAs(layersPath, append);
            }
            dirInfo.Refresh();
            return dirInfo.GetTotalSize();
        }

        public static NeuralNetwork LoadFrom(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new FileNotFoundException("Network folder does not exist.");
            return new NeuralNetwork(folderPath);
        }

        private class NeuralNetworkInfo : NetworkParameters, IComparable<NeuralNetworkInfo>
        {
            public int Count { get; set; }

            public NeuralNetworkInfo(NeuralNetwork network) : base(network)
            {
                Count = network.Count;
            }

            public int CompareTo(NeuralNetworkInfo other)
            {
                if (NumIterations != other.NumIterations || Count != other.Count)
                    return 2;
                else if (
                    Options != other.Options ||
                    LearningRate != other.LearningRate ||
                    Momentum != other.Momentum ||
                    DropoutRate != other.DropoutRate
                )
                    return 0;
                return 1;
            }

        }
#endregion
    }
}
