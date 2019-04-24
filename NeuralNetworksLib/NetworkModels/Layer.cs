using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeuralNetworksLib.Templates;
using ZachLib;
using ZachLib.Statistics;

namespace NeuralNetworksLib.NetworkModels
{
    public class Layer : IReadOnlyList<Neuron>
    {
        private Matrix InputSynapses { get; set; }
        public int NumOutputs { get; private set; }
        public int NumInputs { get; private set; }
        public int LayerIndex { get; private set; }
        public LayerType Type { get; private set; }
        private Vector InputValues { get; set; }
        public Vector Values { get; internal set; }
        private Vector Biases { get; set; }
        internal Vector Deltas { get; set; }
        private Matrix PrevSynapseUpdates { get; set; }
        private Vector PrevBiasUpdates { get; set; }
        
        internal Func<double, bool, double> ActivationFunction { get; set; }
        private NetworkParameters Parameters { get; set; }
        private Func<Vector, Vector> ValuesOp { get; set; }
        private Func<Vector, Vector> NoDropoutValuesOp { get; set; }
        private Func<Layer, (Matrix, Vector)> GetUpdatesOp { get; set; }
        private Func<Vector, Vector> DeltasOp { get; set; }
        private int Iteration { get; set; }

        public Neuron this[int index] {
            get => Type == LayerType.Inputs ? 
                new Neuron(
                    0, 0, Values[index],
                    new double[0],
                    index
                ) : new Neuron(
                    Biases[index], 
                    Deltas[index], 
                    Values[index], 
                    InputSynapses.GetColumn(index), 
                    index
                );
        }
        public int Count { get; private set; }
        public double this[int inputN, int outputN]
        {
            get => InputSynapses[inputN, outputN];
        }

        #region Constructors
        private static readonly Random GEN = new Random(1531407919);
        public Layer(LayerType type, params int[] lyrParams)
        {
            switch (type)
            {
                case LayerType.Inputs:
                    Type = LayerType.Inputs;
                    LayerIndex = 0;
                    Count = lyrParams[0];
                    NumOutputs = lyrParams[1];
                    NumInputs = 0;
                    InputSynapses = null;
                    break;

                case LayerType.Outputs:
                    Type = LayerType.Outputs;
                    Count = lyrParams[0];
                    LayerIndex = lyrParams[1];
                    NumInputs = lyrParams[2];
                    InputSynapses = Matrix.GenerateRandom(NumInputs, Count, () => 2 * GEN.NextDouble() - 1);
                    Deltas = new Vector(Count);
                    break;

                case LayerType.Hidden:
                    Type = LayerType.Hidden;
                    Count = lyrParams[0];
                    LayerIndex = lyrParams[1];
                    NumInputs = lyrParams[2];
                    NumOutputs = lyrParams[3];
                    InputSynapses = Matrix.GenerateRandom(NumInputs, Count, () => 2 * GEN.NextDouble() - 1);
                    Deltas = new Vector(Count);
                    break;
            }
            Iteration = 0;
            Values = new Vector(Count);
        }

        private Layer(string folderPath, NetworkParameters parameters)
        {
            Parameters = parameters;
            var info = Utils.LoadJSON<LayerInfo>(folderPath + "\\info.json");
            NumOutputs = info.NumOutputs;
            NumInputs = info.NumInputs;
            LayerIndex = info.LayerIndex;
            Type = info.Type;

            if (Type != LayerType.Inputs)
            {
                string format = folderPath + "\\{0}.{1}";
                InputSynapses = Matrix.LoadFile(String.Format(format, "input_synapses", "mtx"));
                
                bool doMomentum = parameters.Options.HasFlag(NetworkOptions.Momentum);
                if (parameters.Options.HasFlag(NetworkOptions.Biases))
                {
                    Biases = Vector.LoadFile(String.Format(format, "biases", "vec"));
                    if (doMomentum)
                        PrevBiasUpdates = Vector.LoadFile(String.Format(format, "prev_bias_updates", "vec"));
                }
                if (doMomentum)
                    PrevSynapseUpdates = Matrix.LoadFile(String.Format(format, "prev_synapse_updates", "mtx"));
            }
        }
        #endregion

        #region Initialize
        internal static Func<Layer, (Matrix, Vector)>[] UPDATES_OPS = new Func<Layer, (Matrix, Vector)>[]
        {
            lyr => (lyr.InputValues.DotToMatrix(lyr.Deltas), null),                                 // None
            lyr => (lyr.InputValues.DotToMatrix(lyr.Deltas) * lyr.Parameters.LearningRate, null),   // LearningRate
            lyr =>                                                                                  // Momentum
            {
                var prevSynapseUpdates = (Matrix)lyr.PrevSynapseUpdates.Clone();
                var synapseUpdates = lyr.InputValues.DotToMatrix(lyr.Deltas);
                lyr.PrevSynapseUpdates = synapseUpdates * lyr.Parameters.Momentum;
                synapseUpdates += prevSynapseUpdates;
                return (synapseUpdates, null);
            },
            lyr =>                                                                                  // LearningRate | Momentum
            {
                var prevSynapseUpdates = (Matrix)lyr.PrevSynapseUpdates.Clone();
                var synapseUpdates = lyr.InputValues.DotToMatrix(lyr.Deltas) * lyr.Parameters.LearningRate;
                lyr.PrevSynapseUpdates = synapseUpdates * lyr.Parameters.Momentum;
                synapseUpdates += prevSynapseUpdates;
                return (synapseUpdates, null);
            },
            lyr => (lyr.InputValues.DotToMatrix(lyr.Deltas), lyr.Deltas),                           // Biases
            lyr => (                                                                                // LearningRate | Biases
                lyr.InputValues.DotToMatrix(lyr.Deltas) * lyr.Parameters.LearningRate, 
                lyr.Deltas * lyr.Parameters.LearningRate
            ),
            lyr =>                                                                                  // Momentum | Biases
            {
                var prevSynapseUpdates = (Matrix)lyr.PrevSynapseUpdates.Clone();
                var synapseUpdates = lyr.InputValues.DotToMatrix(lyr.Deltas);
                lyr.PrevSynapseUpdates = synapseUpdates * lyr.Parameters.Momentum;
                synapseUpdates += prevSynapseUpdates;
                var prevBiasUpdates = lyr.PrevBiasUpdates;
                var biasUpdates = lyr.Deltas;
                lyr.PrevBiasUpdates = biasUpdates * lyr.Parameters.Momentum;
                biasUpdates += prevBiasUpdates;
                return (synapseUpdates, biasUpdates);
            },
            lyr =>                                                                                  // LearningRate | Momentum | Biases
            {
                var prevSynapseUpdates = (Matrix)lyr.PrevSynapseUpdates.Clone();
                var synapseUpdates = lyr.InputValues.DotToMatrix(lyr.Deltas) * lyr.Parameters.LearningRate;
                lyr.PrevSynapseUpdates = synapseUpdates * lyr.Parameters.Momentum;
                synapseUpdates += prevSynapseUpdates;
                var prevBiasUpdates = lyr.PrevBiasUpdates;
                var biasUpdates = lyr.Deltas * lyr.Parameters.LearningRate;
                lyr.PrevBiasUpdates = biasUpdates * lyr.Parameters.Momentum;
                biasUpdates += prevBiasUpdates;
                return (synapseUpdates, biasUpdates);
            }
        };

        public void ToggleDropout()
        {
            if (Parameters.Options.HasFlag(NetworkOptions.Biases))
                ValuesOp = v => v.ElementWiseOp(
                    Biases,
                    (e, b) => NNFunctions.Sigmoid(e + b)
                );
            else
                ValuesOp = v => v.ElementWiseChange(
                    e => NNFunctions.Sigmoid(e)
                );
        }

        public void Initialize(NetworkParameters networkParams)
        {
            Parameters = networkParams;
            bool doBiases = Parameters.Options.HasFlag(NetworkOptions.Biases);
            bool doDropout = Parameters.Options.HasFlag(NetworkOptions.Dropout);
            if (doBiases)
                Biases = new Vector(Count).ElementWiseChange(v => NNFunctions.GetRandom(false));
            
            var options = (byte)Parameters.Options;
            if (Type != LayerType.Outputs && doDropout)
            {
                if (doBiases)
                {
                    ValuesOp = v => v.ElementWiseOp(
                        Biases,
                        (e, b) => NNFunctions.GetRandom(false) < Parameters.DropoutRate ?
                            0 : NNFunctions.Sigmoid(e + b) * Parameters.DropoutMultiplier
                    );
                    NoDropoutValuesOp = v => v.ElementWiseOp(
                        Biases,
                        (e, b) => NNFunctions.Sigmoid(e + b)
                    );
                }
                else
                {
                    ValuesOp = v => v.ElementWiseChange(
                        e => NNFunctions.GetRandom(false) < Parameters.DropoutRate ?
                            0 : NNFunctions.Sigmoid(e) * Parameters.DropoutMultiplier
                    );
                    NoDropoutValuesOp = v => v.ElementWiseChange(
                        e => NNFunctions.Sigmoid(e)
                    );
                }
            }
            else if (doBiases)
                ValuesOp = v => v.ElementWiseOp(
                    Biases,
                    (e, b) => NNFunctions.Sigmoid(e + b)
                );
            else
                ValuesOp = v => v.ElementWiseChange(
                    e => NNFunctions.Sigmoid(e)
                );

            if (Parameters.Options.HasFlag(NetworkOptions.Momentum))
            {
                PrevSynapseUpdates = new Matrix(InputSynapses.NumRows, InputSynapses.NumCols);
                if (doBiases)
                    PrevBiasUpdates = new Vector(Count);
            }

            if (doDropout)
                options -= 8;

            GetUpdatesOp = UPDATES_OPS[options];
        }
        #endregion

        public Vector ForwardPropogate(Vector inputs)
        {
            InputValues = inputs;
            // disables dropout for testing error
            Values = ValuesOp(inputs.Dot(InputSynapses));
            return Values;
        }

        public Vector ForwardPropogate(Vector inputs, bool disableDropout)
        {
            InputValues = inputs;
            // disables dropout for testing error
            Values = (disableDropout ? NoDropoutValuesOp : ValuesOp)(inputs.Dot(InputSynapses));
            return Values;
        }

        public Vector BackPropogate(Vector errors)
        {
            Deltas = errors.ElementWiseOp(Values, (e, v) => e * (v * (1 - v)));
            Vector nextErrors = Deltas.Dot(InputSynapses.T());
            (Matrix synapseUpdates, Vector biasUpdates) = GetUpdatesOp(this);
            InputSynapses -= synapseUpdates;
            if (Parameters.Options.HasFlag(NetworkOptions.Biases))
                Biases -= biasUpdates;
            return nextErrors;
        }

        #region Presentation
        public class NeuronEnumerator : IEnumerator, IEnumerator<Neuron>
        {
            private Layer Layer { get; set; }
            object IEnumerator.Current { get => Current; }
            public Neuron Current
            {
                get
                {
                    if (Index == -1)
                        throw new InvalidOperationException();
                    else
                        return Layer[Index];
                }
            }
            private int Index { get; set; }
            private bool ReachedEnd { get; set; }
            private bool IterationChanged { get; set; }
            private int LastIteration { get; set; }

            public NeuronEnumerator(Layer layer)
            {
                Layer = layer;
                LastIteration = Layer.Iteration;
                IterationChanged = false;
                Reset();
            }

            public void Reset()
            {
                IterationChanged = LastIteration != Layer.Iteration;
                if (IterationChanged)
                    throw new InvalidOperationException("Collection changed while enumerating.");
                Index = -1;
                ReachedEnd = false;
            }

            public bool MoveNext()
            {
                IterationChanged = LastIteration != Layer.Iteration;
                if (IterationChanged)
                    throw new InvalidOperationException("Collection changed while enumerating.");
                ++Index;
                return Index < Layer.Count;
            }

            public void Dispose()
            {

            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<Neuron> GetEnumerator() => new NeuronEnumerator(this);

        private PointF[] PreviousLayerPoints { get; set; }
        private KeyValuePair<Tuple<RectangleF, PointF, PointF, PointF>, KeyValuePair<PointF, PointF>[]>[] LayerPoints { get; set; }
        private Font BigFont { get; set; }
        private Font SmallFont { get; set; }
        private float Diameter { get; set; }
        private float ZoomFactor { get; set; }
        public PointF[] CreateDrawing(Graphics gfx, DrawingParameters parameters, PointF[] prevLayerPoints)
        {
            parameters.NextLayer();
            bool notFirst = Type != LayerType.Inputs;
            bool notLast = Type != LayerType.Outputs;
            PreviousLayerPoints = prevLayerPoints;
            PointF[] layerPoints = new PointF[Count];
            LayerPoints = new KeyValuePair<Tuple<RectangleF, PointF, PointF, PointF>, KeyValuePair<PointF, PointF>[]>[Count];
            BigFont = parameters.BigFont;
            SmallFont = parameters.SmallFont;
            Diameter = parameters.NeuronDiameter;
            ZoomFactor = parameters.ZoomFactor;

            for (int n = 0; n < Count; ++n)
            {
                KeyValuePair<PointF, PointF>[] weightPoints = new KeyValuePair<PointF, PointF>[prevLayerPoints.Length];
                var thisPoint = parameters.CurrentNeuronConnectorIn;
                if (notLast)
                    layerPoints[n] = parameters.CurrentNeuronConnectorOut;
                if (notFirst)
                {
                    for (int n2 = 0; n2 < prevLayerPoints.Length; ++n2)
                    {
                        var weight = InputSynapses[n2, n];
                        var prevPoint = prevLayerPoints[n2];
                        gfx.DrawLine(
                            NNFunctions.CreatePenFromWeight(weight, ZoomFactor),
                            prevPoint,
                            thisPoint
                        );

                        var side1 = (prevPoint.X - thisPoint.X);
                        var side2 = (prevPoint.Y - thisPoint.Y);
                        var lineLength = (float)Math.Sqrt((side1 * side1) + (side2 * side2));
                        var sectionLength = lineLength / (prevLayerPoints.Length + 1);
                        var finalOffsetLength = sectionLength * (n2 + 1);
                        var finalOffsetRatio = finalOffsetLength / lineLength;
                        var weightX = thisPoint.X - (side1 * finalOffsetRatio);
                        var weightY = thisPoint.Y - (side2 * finalOffsetRatio);
                        weightPoints[n2] = new KeyValuePair<PointF, PointF>(prevPoint, new PointF(weightX, weightY));

                        gfx.DrawString(
                            weight.ToString(".00"), 
                            SmallFont, 
                            Brushes.Blue, 
                            weightX,
                            weightY
                        );
                    }
                }

                var neuronPoints = new Tuple<RectangleF, PointF, PointF, PointF>(
                    new RectangleF(parameters.CurrentX, parameters.CurrentY, Diameter, Diameter),
                    parameters.CurrentNeuronConnectorIn,
                    new PointF(parameters.CurrentX + parameters.BigFontYModifier, parameters.CurrentY + parameters.BigFontYModifier),
                    new PointF(parameters.CurrentX + parameters.SmallFontXModifier, parameters.CurrentY + parameters.SmallFontYModifier)
                );
                LayerPoints[n] = new KeyValuePair<Tuple<RectangleF, PointF, PointF, PointF>, KeyValuePair<PointF, PointF>[]>(neuronPoints, weightPoints);

                gfx.FillEllipse(Brushes.WhiteSmoke, neuronPoints.Item1);
                gfx.DrawEllipse(Pens.Gray, neuronPoints.Item1);
                gfx.DrawString(Values[n].ToString("#.00"), BigFont, DrawingParameters.BlackSmoke, neuronPoints.Item3);
                if (notFirst)
                {
                    var bias = Biases[n];
                    gfx.DrawString(
                        (bias > 0 ? "+" : "") + bias.ToString("#.00"), 
                        SmallFont, Brushes.YellowGreen, neuronPoints.Item4
                    );
                }

                parameters.NextNeuron();
            }

            return layerPoints;
        }

        public void UpdateDrawing(Graphics gfx)
        {
            for (int n = 0; n < Count; ++n)
            {
                var point = LayerPoints[n];
                for (int n2 = 0; n2 < point.Value.Length; ++n2)
                {
                    var weight = InputSynapses[n2, n];
                    var weightPoints = point.Value[n2];
                    gfx.DrawLine(
                        NNFunctions.CreatePenFromWeight(weight, ZoomFactor),
                        weightPoints.Key,
                        point.Key.Item2
                    );
                    gfx.DrawString(
                        weight.ToString(".00"),
                        SmallFont,
                        Brushes.Blue,
                        weightPoints.Value
                    );
                }

                gfx.FillEllipse(Brushes.WhiteSmoke, point.Key.Item1);
                gfx.DrawEllipse(Pens.Gray, point.Key.Item1);
                gfx.DrawString(
                    Values[n].ToString("#.00"), 
                    BigFont, 
                    DrawingParameters.BlackSmoke, 
                    point.Key.Item3
                );
                var bias = Biases[n];
                gfx.DrawString(
                    (bias > 0 ? "+" : "") + bias.ToString("#.00"), 
                    SmallFont, 
                    Brushes.YellowGreen, 
                    point.Key.Item4
                );
            }
        }

        private const string NEURON_FORMAT = "{0}{1}.{2}";
        public TreeNode ToTreeNode()
        {
            TreeNode node = new TreeNode("Layer " + LayerIndex);
            node.Name = "lyr" + LayerIndex;
            node.ToolTipText = Type.ToString();
            int n = 0;

            foreach(var neuron in this)
            {
                TreeNode neuronNode = new TreeNode("Neuron " + n);
                var name = String.Format(NEURON_FORMAT, "nrn", LayerIndex, n);
                ++n;
                neuronNode.Name = name;
                name += ".";

                neuronNode.Nodes.Add(
                    new TreeNode("Value: " + neuron.Value.ToString(".00"))
                    {
                        Name = name + "Value"
                    }
                );
                neuronNode.Nodes.Add(
                    new TreeNode("Bias: " + neuron.Bias)
                    {
                        Name = name + "Bias"
                    }
                );
                neuronNode.Nodes.Add(
                    new TreeNode("Delta: " + neuron.Delta)
                    {
                        Name = name + "Delta"
                    }
                );

                name += "Dendrites";
                TreeNode dendrites = new TreeNode("Dendrites") { Name = name };
                name += ".";
                for (int d = 0; d < neuron.InputWeights.Length; ++d)
                {
                    var weight = neuron.InputWeights[d];
                    dendrites.Nodes.Add(
                        new TreeNode(weight.ToString(".00"))
                        {
                            Name = name + d,
                            ToolTipText = weight.ToString(),
                            ForeColor = NNFunctions.CreatePenFromWeight(weight, 1.0f).Color
                        }
                    );
                }

                node.Nodes.Add(neuronNode);
            }

            return node;
        }

        public void UpdateTreeNode(TreeNode node)
        {
            int n = 0;
            foreach (var neuron in this)
            {
                var neuronNode = node.Nodes[n];
                ++n;

                neuronNode.Nodes[0].Text = "Value: " + neuron.Value;
                neuronNode.Nodes[1].Text = "Bias: " + neuron.Bias;
                neuronNode.Nodes[2].Text = "Delta: " + neuron.Delta;

                var dendrites = neuronNode.Nodes[3];

                for (int d = 0; d < neuron.InputWeights.Length; ++d)
                {
                    var weight = neuron.InputWeights[d];
                    var dendrite = dendrites.Nodes[d];
                    dendrite.ToolTipText = weight.ToString();
                    dendrite.ForeColor = NNFunctions.CreatePenFromWeight(weight, 1.0f).Color;
                }
            }
        }
        #endregion

        internal void SaveAs(string folderPath, bool append)
        {
            string layerPath = folderPath + @"\\" + LayerIndex + " - " + Type.ToString();

            if (!append)
            {
                Directory.CreateDirectory(layerPath);
                new LayerInfo(this).SaveAs(layerPath + "\\info.json");
            }

            if (Type != LayerType.Inputs)
            {
                InputSynapses.Serialize(layerPath, "input_synapses");
                bool doBiases = Parameters.Options.HasFlag(NetworkOptions.Biases);
                if (Parameters.Options.HasFlag(NetworkOptions.Momentum))
                {
                    PrevSynapseUpdates.Serialize(layerPath, "prev_synapse_updates");
                    if (doBiases)
                        PrevBiasUpdates.SerializeToFile(layerPath, "prev_bias_updates");
                }

                if (doBiases)
                    Biases.SerializeToFile(layerPath, "biases");
            }
        }

        internal static Layer LoadFrom(string folderPath, NetworkParameters parameters = null)
        {
            if (!Directory.Exists(folderPath))
                throw new FileNotFoundException("Layer folder does not exist.");
            Layer layer = new Layer(folderPath, parameters);
            return layer;
        }

        internal void SaveAs(string folderPath) => SaveAs(folderPath, Directory.Exists(folderPath));

        private struct LayerInfo
        {
            public int NumOutputs { get; private set; }
            public int NumInputs { get; private set; }
            public int LayerIndex { get; private set; }
            public LayerType Type { get; private set; }

            public LayerInfo(Layer layer)
            {
                NumOutputs = layer.NumOutputs;
                NumInputs = layer.NumInputs;
                LayerIndex = layer.LayerIndex;
                Type = layer.Type;
            }
        }
    }

}
