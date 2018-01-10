using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slack
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public ConcurrentQueue<object> _queue = new ConcurrentQueue<object>();

        private void lstHist_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
