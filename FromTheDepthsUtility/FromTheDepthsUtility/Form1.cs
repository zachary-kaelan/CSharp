using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FromTheDepthsUtility
{
    public partial class Form1 : Form
    {
        public const float Crackers = 1;
        public const float Cokers = 1;
        public const float Desalters = 1;
        public const float Altitude = 10;
        public const float Flares = 7;

        public const string ListBoxFormat = "{0}, {1}, {2}, {3}";

        public Form1()
        {
            InitializeComponent();

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnSimulation_Click(object sender, EventArgs e)
        {
            

            float efficiency = ResourceMath.ProcessingEfficiency(Crackers, Cokers, Desalters, Altitude);
            float processingTime = ResourceMath.ProcessingTime(Crackers, Cokers, Desalters);
            float time = 0;
            float cycleTime = 0;
            float gas = 0;
            float fuel = 0;

            /*var clmCycle = new ListViewItem();
            var clmFuel = new ListViewItem();
            var clmTime = new ListViewItem();
            var clmGas = new ListViewItem();*/

            List<ListViewItem> items = new List<ListViewItem>();

            for (int cycle = 1; cycle <= 15; ++cycle)
            {
                fuel += efficiency;

                items.Add(new ListViewItem(new[] { cycle.ToString(), fuel.ToString(), time.ToString(), gas.ToString() }));
                /*clmCycle.SubItems.Add(cycle.ToString());
                clmFuel.SubItems.Add(fuel.ToString());
                clmTime.SubItems.Add(time.ToString());
                clmGas.SubItems.Add(gas.ToString());*/
                //lstView.Items.Add(String.Format(ListBoxFormat, cycle.ToString("##.00"), fuel.ToString("##.00"), time.ToString("##.00"), gas.ToString("##.00")));

                while (true)
                {
                    //lstView.Items.Add("");
                    gas += ResourceMath.DangerousGasGeneration(gas, efficiency, Flares);
                    time += 2;
                    cycleTime += 2;

                    if (cycleTime > processingTime)
                    {
                        cycleTime -= processingTime;
                        break;
                    }

                    //lstView.Items.Add(String.Format(ListBoxFormat, " ", " ", time.ToString("##.00"), gas.ToString("##.00")));
                    items.Add(new ListViewItem(new[] { "", "", time.ToString(), gas.ToString()}));
                    /*clmCycle.SubItems.Add("");
                    clmFuel.SubItems.Add("");
                    clmTime.SubItems.Add(time.ToString());
                    clmGas.SubItems.Add(gas.ToString());*/
                }
            }

            /*clmCycle.SubItems.Add("16");
            clmFuel.SubItems.Add(fuel.ToString());
            clmTime.SubItems.Add(time.ToString());
            clmGas.SubItems.Add(gas.ToString());*/
            items.Add(new ListViewItem(new[] { "16", fuel.ToString(), time.ToString(), gas.ToString() }));

            lstSimulation.Items.AddRange(items.ToArray());
            //lstSimulation.Items.AddRange(new ListViewItem[] { clmCycle, clmFuel, clmTime, clmGas });

            //lstView.Items.Add(String.Format(ListBoxFormat, "16", fuel.ToString("##.00"), time.ToString("##.00"), gas.ToString("##.00")));
        }
    }
}
