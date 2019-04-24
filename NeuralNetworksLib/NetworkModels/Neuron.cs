using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetworksLib.NetworkModels
{
    public enum LayerType
    {
        Inputs,
        Hidden,
        Outputs
    }

    public struct Neuron
    {
        public double Bias { get; private set; }
        public double Delta { get; private set; }
        public double Value { get; private set; }
        public double[] InputWeights { get; private set; }
        public int LayerIndex { get; private set; }

        public Neuron(double bias, double delta, double value, double[] weights, int index)
        {
            Bias = bias;
            Delta = delta;
            Value = value;
            InputWeights = weights;
            LayerIndex = index;
        }
    }

    internal class EventNeuron
    {
        public double Bias { get; private set; }
        private double _delta = 0;
        private double PreviousDelta { get; set; }
        public double Delta {
            get => _delta;
            set
            {
                _delta = value;
                Bias += Network.LearningRate * _delta;
                //NeuronBackFired?.Invoke(Key, new NeuronFiredEventArgs(Value, LayerIndex));
            }
        }
        private double _value = 0;
        public double Value {
            get => _value;
            set {
                _value = value;
                NeuronFired?.Invoke(Key, new NeuronFiredEventArgs(Value, LayerIndex));
            }
        }

        private int Key { get; set; }
        private int LayerIndex { get; set; }
        private NeuralNetwork Network { get; set; }

        public delegate void OnFire(object sender, NeuronFiredEventArgs e);
        public event OnFire NeuronFired;
        public delegate void OnBackFire(object sender, NeuronBackfiredEventArgs e);
        public event OnBackFire NeuronBackFired;

        private double CombinedValues { get; set; }
        private int Inputs { get; set; }
        private int Outputs { get; set; }
        public double[] Weights { get; private set; }
        /*private double[] InputWeights { get; set; }
        private double[] OutputWeights { get; set; }
        private int[] OutputNeurons { get; set; }
        private int[] InputNeurons { get; set; }*/
        private int PushedValues { get; set; }
        private bool DoDropout { get; set; }
        private bool DoMomentum { get; set; }
        private Layer LayerType { get; set; }

        public TreeNode ToTreeNode()
        {
            string name = "nrn" + Key.ToString();
            TreeNode node = new TreeNode("Neuron");
            node.Name = name;
            node.Nodes.Add(name + "Bias", "Bias: " + Bias.ToString("#.0000"));
            node.Nodes.Add(name + "Delta", "Delta: " + Delta.ToString("#.0000"));
            node.Nodes.Add(name + "Value", "Value: " + Value.ToString("#.0000"));

            TreeNode dendrites = new TreeNode("Dendrites");
            dendrites.Name = name + "Dendrites";
            name += "Weight";
            for (int w = 0; w < Inputs; ++w)
            {
                string str = w.ToString();
                dendrites.Nodes.Add(name + str, str + ": " + Weights[w].ToString("#.0000"));
            }
            node.Nodes.Add(dendrites);

            return node;
        }

        public void UpdateTree(TreeView tree)
        {
            string name = "nrn" + Key.ToString();
            tree.Nodes[name + "Bias"].Text = "Bias: " + Bias.ToString("#.0000");
            tree.Nodes[name + "Delta"].Text = "Delta: " + Delta.ToString("#.0000");
            tree.Nodes[name + "Value"].Text = "Value: " + Value.ToString("#.0000");

            name += "Weight";
            for (int w = 0; w < Inputs; ++w)
            {
                string str = w.ToString();
                tree.Nodes[name + str].Text = str + ": " + Weights[w].ToString("#.0000");
            }
        }
    }
}
