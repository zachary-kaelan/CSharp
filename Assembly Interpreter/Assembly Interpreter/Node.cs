using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assembly_Interpreter
{
    public class NodeManager
    {
        public Stack<short> StackMemoryNode { get; set; }
        private Node[,] nodes { get; set; }
        private Dictionary<byte, Node> nodeMap { get; set; }
        private Dictionary<string, PropertyInfo> nodeProperties { get; set; }
        private int columns { get; set; }
        private int rows { get; set; }
        private int maxIndex { get; set; }

        public NodeManager(byte nodeColumns, byte nodeRows)
        {
            columns = nodeColumns;
            rows = nodeRows;
            maxIndex = columns * rows;

            nodeProperties = typeof(Node).GetProperties().ToDictionary(p => p.Name, p => p);
            nodes = new Node[nodeColumns, nodeRows];
            nodeMap = new Dictionary<byte, Node>();
            for (int i = 0; i < nodeRows; ++i)
            {
                for (int j = 0; j < nodeColumns; ++j)
                {
                    nodes[i, j] = new Node(new KeyValuePair<byte, byte>((byte)(i + 1), (byte)(j + 1)));
                    nodes[i, j].PropertyChanged += Node_PropertyChanged;
                    nodeMap.Add(nodes[i, j].ADDR, nodes[i, j]);
                }
            }
        }

        private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Node node = null;
            try
            {
                node = (Node)sender;
            }
            catch
            {
                return;
            }

            PropertyInfo nodeProp = nodeProperties[e.PropertyName];
            byte targetAddress = node.ADDR;
            byte relativeAddress = (byte)(node.ADDR - Node.RESERVED_BYTES);
            string targetProperty = null;
            bool isSuccessful = true;

            switch (e.PropertyName)
            {
                case "LEFT":
                    isSuccessful = (relativeAddress - 1) % columns == 0;
                    --targetAddress;
                    targetProperty = "RIGHT";
                    break;

                case "RIGHT":
                    isSuccessful = relativeAddress % columns == 0;
                    ++targetAddress;
                    targetProperty = "LEFT";
                    break;

                case "UP":
                    isSuccessful = relativeAddress > columns;
                    targetAddress -= (byte)columns;
                    targetProperty = "DOWN";
                    break;

                case "DOWN":
                    isSuccessful = relativeAddress <= (maxIndex - columns);
                    targetAddress += (byte)rows;
                    targetProperty = "UP";
                    break;
            }
            
            if (isSuccessful && targetAddress != node.ADDR && nodeMap.TryGetValue(targetAddress, out Node targetNode))
            {
                Nullable<short> targetValue = (short?)nodeProp.GetValue(sender);
                PropertyInfo prop = nodeProperties[targetProperty];
                Nullable<short> currentValue = (short?)prop.GetValue(targetNode);

                if (!currentValue.HasValue)
                {
                    prop.SetValue(targetNode, targetValue);
                    nodeProp.SetValue(node, null);
                }

                targetValue = null;
                prop = null;
                currentValue = null;
            }
            
            nodeProp = null;
            targetProperty = null;
        }
    }

    public class Node : INotifyPropertyChanged
    {
        public static Nullable<short> NIL { get { return 0; } set { value = null; } }
        public const byte RESERVED_BYTES = 16;
        public event PropertyChangedEventHandler PropertyChanged;

        public Nullable<short> LEFT { get; set; }
        public Nullable<short> RIGHT { get; set; }
        public Nullable<short> UP { get; set; }
        public Nullable<short> DOWN { get; set; }
        public byte ADDR { get; set; }

        private short ACC { get; set; }
        private short BAK { get; set; }

        private byte executionIndex
        {
            get
            {
                return executionIndex;
            }
            set
            {
                executionIndex = (byte)Math.Max(0, Math.Min(this.intructions.Length, value));
            }
        }
        private Instruction[] intructions { get; set; }

        public Node(KeyValuePair<byte, byte> location)
        {
            ACC = 0;
            BAK = 0;
            LEFT = null;
            RIGHT = null;
            UP = null;
            DOWN = null;
            ADDR = (byte)(RESERVED_BYTES + (location.Key * location.Value));
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExecuteInstruction(Instruction op)
        {
            switch (op.FUNC)
            {
                case (byte)Instruct.ADD:
                    ACC += (byte)op.SRC;
                    break;

                case (byte)Instruct.JEZ:
                    if (ACC == 0)
                        executionIndex = (byte)op.SRC;
                    break;

                case (byte)Instruct.JGZ:
                    if (ACC > 0)
                        executionIndex = (byte)op.SRC;
                    break;

                case (byte)Instruct.JLZ:
                    if (ACC < 0)
                        executionIndex = (byte)op.SRC;
                    break;

                case (byte)Instruct.JMP:
                    executionIndex = (byte)op.SRC;
                    break;

                case (byte)Instruct.JNZ:
                    if (ACC != 0)
                        executionIndex = (byte)op.SRC;
                    break;

                case (byte)Instruct.JRO:
                    --executionIndex;
                    executionIndex += (byte)op.SRC;
                    break;

                case (byte)Instruct.MOV:
                    short SRC = (op.SRC.GetType().IsEquivalentTo(typeof(short)) ? (short)op.SRC : READ((byte)op.SRC));
                    (Register)op.DEST;
                    break;
            }
        }

        private short READ(byte SRC)
        {
            PropertyInfo srcProp = this.GetType().GetProperty(((Register)SRC).ToString());
            SpinWait.SpinUntil(() => ((Nullable<short>)srcProp.GetValue(this)).HasValue);
            short value = ((Nullable<short>)srcProp.GetValue(this)).Value;
            srcProp.SetValue(this, null);
            srcProp = null;
            return value;
        }
    }
}
