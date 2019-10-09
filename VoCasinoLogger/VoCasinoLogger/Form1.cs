using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VoCoLib.Economy;

namespace VoCasinoLogger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private byte Current_Slot = 0;
        private Slot Slot1 = Slot.Grey_Question;
        private Slot Slot2 = Slot.Grey_Question;
        private Slot Slot3 = Slot.Grey_Question;

        private void pbxX_Click(object sender, EventArgs e)
        {

        }

        private void pbxCherries_Click(object sender, EventArgs e)
        {

        }

        private void pbxGem_Click(object sender, EventArgs e)
        {

        }

        private void pbxDonator_Click(object sender, EventArgs e)
        {

        }

        private void pbxDonatorPlus_Click(object sender, EventArgs e)
        {

        }

        private void pbxVCN_Click(object sender, EventArgs e)
        {

        }

        private void pbxVoCrate_Click(object sender, EventArgs e)
        {

        }

        private void RemoveSlot1()
        {
            if (Slot1 != Slot.Grey_Question)
            {
                if (Slot2 != Slot.Grey_Question)
                {
                    Slot1 = Slot2;
                    RemoveSlot2();
                }
                else
                    Slot1 = Slot.Grey_Question;
            }
        }

        private void RemoveSlot2()
        {
            if (Slot3 != Slot.Grey_Question)
            {
                Slot2 = Slot3;
                RemoveSlot3();
            }
            else
                Slot2 = Slot.Grey_Question;
        }

        private void RemoveSlot3()
        {
            Slot3 = Slot.Grey_Question;
        }

        private void pbxSlot1_Click(object sender, EventArgs e) => RemoveSlot1();
        private void pbxSlot2_Click(object sender, EventArgs e) => RemoveSlot2();
        private void pbxSlot3_Click(object sender, EventArgs e) => RemoveSlot3();
    }
}
