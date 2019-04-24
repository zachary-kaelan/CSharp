using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworksLib.NetworkModels
{
    public class DrawingParameters
    {
        public float NeuronDiameter { get; set; }
        public float NeuronDistance { get; set; }
        public float LayerDistance { get; set; }
        public float CenterModifier { get; set; }
        public float BigFontYModifier { get; set; }
        public float SmallFontYModifier { get; set; }
        public float SmallFontXModifier { get; set; }
        public Font BigFont { get; set; }
        public Font SmallFont { get; set; }

        public float CurrentY { get; set; }
        public float CurrentX { get; set; }

        private float[] LayerWidths { get; set; }
        public bool Rotated { get; private set; }
        public float ZoomFactor { get; private set; }

        private int LayerIndex { get; set; }
        public Action NextLayer { get; private set; }
        public Action NextNeuron { get; private set; }
        public Action Reset { get; private set; }
        public PointF CurrentNeuronConnectorIn { get; private set; }
        public PointF CurrentNeuronConnectorOut { get; private set; }

        public static Brush BlackSmoke = new SolidBrush(Color.FromArgb(10, 10, 10));

        public void Initialize(NeuralNetwork network, RectangleF pbxBounds)
        {
            LayerIndex = -1;
            LayerWidths = new float[network.Count];
            float maxWidth = 0;
            float height = (network.Count * LayerDistance) + NeuronDiameter;
            for (int i = 0; i < network.Count; ++i)
            {
                var layer = network[i];
                var width = (layer.Count * NeuronDistance) + NeuronDiameter;
                if (width > maxWidth)
                    maxWidth = width;
                LayerWidths[i] = width;
            }

            bool pbxWidthWise = pbxBounds.Width > pbxBounds.Height;
            bool netWidthWise = maxWidth > height;
            if (pbxWidthWise != netWidthWise)
            {
                Rotated = true;
                NextLayer = () =>
                {
                    ++LayerIndex;
                    CurrentY = (LayerWidths[LayerIndex] / ZoomFactor) + 10;
                    CurrentX += LayerDistance;
                };
                NextNeuron = () =>
                {
                    CurrentY += NeuronDistance;
                    CurrentNeuronConnectorOut = new PointF(CurrentX + NeuronDiameter, CurrentY + CenterModifier);
                    CurrentNeuronConnectorIn = new PointF(CurrentX, CurrentY + CenterModifier);
                };
                Reset = () => CurrentX = 10;
            }
            else
            {
                Rotated = false;
                NextLayer = () =>
                {
                    ++LayerIndex;
                    CurrentX = (LayerWidths[LayerIndex] / ZoomFactor) + 10;
                    CurrentY += LayerDistance;
                };
                NextNeuron = () =>
                {
                    CurrentX += NeuronDistance;
                    CurrentNeuronConnectorOut = new PointF(CurrentX + CenterModifier, CurrentY + NeuronDiameter);
                    CurrentNeuronConnectorIn = new PointF(CurrentX + CenterModifier, CurrentY);
                };
                Reset = () => CurrentY = 10;
            }

            float zoomFactor = (
                pbxWidthWise ? pbxBounds.Width : pbxBounds.Height
            ) / (
                (netWidthWise ? maxWidth : height) + 10
            );
            InitializeDefault(zoomFactor);
        }

        public void InitializeDefault(float zoomFactor)
        {
            LayerWidths = LayerWidths.Select(w => (w / 2f) / zoomFactor).ToArray();
            NeuronDiameter = 30f * zoomFactor;
            NeuronDistance = 75f * zoomFactor;
            LayerDistance  = 75f * zoomFactor;
            BigFont = new Font("Arial", 8f * zoomFactor);
            SmallFont = new Font("Arial", 4f * zoomFactor);
            CenterModifier = (NeuronDiameter / 2) - (5 * zoomFactor);
            float fifth = NeuronDiameter / 5;
            BigFontYModifier = CenterModifier - fifth;
            SmallFontYModifier = CenterModifier + fifth;
            //float fontYModifier = (centerModifier - fifth);
            SmallFontXModifier = BigFontYModifier + (2 * zoomFactor);
            ZoomFactor = zoomFactor;
        }
    }
}
