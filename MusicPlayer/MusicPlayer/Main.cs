using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using CSCore.Streams;

namespace MusicPlayer
{
    public partial class Main : Form
    {
        private readonly MusicPlayer _musicPlayer = new MusicPlayer();
        private bool _stopSliderUpdate;
        private readonly ObservableCollection<MMDevice> _devices = new ObservableCollection<MMDevice>();

        public Main()
        {
            InitializeComponent();
            components = new Container();
            components.Add(_musicPlayer);
            _musicPlayer.PlaybackStopped += (s, args) =>
            {
                //WasapiOut uses SynchronizationContext.Post to raise the event
                //There might be already a new WasapiOut-instance in the background when the async Post method brings the PlaybackStopped-Event to us.
                if (_musicPlayer.PlaybackState != PlaybackState.Stopped)
                    btnPlay.Enabled = btnStop.Enabled = btnPause.Enabled = false;
            };
            
        }

        public static Dictionary<string, Thread> threads = new Dictionary<string, Thread>();
        public static Dictionary<string, Form> forms = new Dictionary<string, Form>();

        private void mnuEqualizer_Click(object sender, EventArgs e)
        {
            if (!forms.TryGetValue("Equalizer", out Form frmEqualizer))
            {
                forms.Add("Equalizer", new Equalizer());
                forms["Equalizer"].Show();
            }
            else
                forms["Equalizer"].BringToFront();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            
        }

        private void wmpMain_Enter(object sender, EventArgs e)
        {

        }
    }
}
