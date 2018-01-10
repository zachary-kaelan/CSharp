using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;

using Exocortex.DSP;

namespace BeatMatcher
{
    class AudioClient
    {
        const int FLOAT_SIZE = 4;
        const int WINDOW_SIZE = 1024;
        const int WINDOW_SHIFT_SIZE = 256;

        MusicProperties musicProps { get; set; }
        long size { get; set; }

        Stream audioStream { get; set; }
        byte[] window = new byte[WINDOW_SIZE];
        byte[] buffer = new byte[FLOAT_SIZE];
        Complex[] complex { get; set; }

        double[] real { get; set; }
        double[] imag { get; set; }
        
        public AudioClient(string path)
        {
            size = new FileInfo(path).Length;
            var file = StorageFile.GetFileFromPathAsync(path).GetResults();
            musicProps = file.Properties.GetMusicPropertiesAsync().GetResults();

            audioStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            audioStream.Read(window, 0, WINDOW_SIZE);
            
        }

        public void GetPower()
        {
            int count = WINDOW_SIZE / FLOAT_SIZE;
            complex = new Complex[count];
            real = new double[count];
            imag = new double[count];

            for (int i = 0; i < WINDOW_SIZE; i += 4)
            {
                //audioStream.Read(buffer, 0, FLOAT_SIZE);
                complex[i] = (Complex)BitConverter.ToSingle(window, i);
            }

            Fourier.FFT(complex, count, FourierDirection.Forward);
            real = complex.Select(c => Math.Pow(c.Re, 2)).ToArray();
            imag = complex.Select(c => Math.Pow(c.Im, 2)).ToArray();

            audioStream.Position -= (WINDOW_SIZE - WINDOW_SHIFT_SIZE);
            audioStream.Read(window, 0, WINDOW_SIZE);
        }

        public double[] GetBassRange()
        {
            List<double> bassFrequencies = new List<double>();
            while (audioStream.Position < audioStream.Length)
            {
                double bass = 0;
                this.GetPower();
                for (int i = 8; i < 96; i++)
                {
                    bass += real[i];
                }
                bassFrequencies.Add(bass);
            }

            return bassFrequencies.ToArray();
        }

        public int GetBPM(double[] frequencies)
        {
            TimeSpan dur = musicProps.Duration;
            double bitrate = Convert.ToDouble(musicProps.Bitrate);
            double bytesPerSec = (bitrate / 8.0) * (bitrate < 1000.0 ? 1000.0 : 1.0);
            double durPerWindow = Convert.ToDouble(WINDOW_SIZE) / bytesPerSec;

            int count = frequencies.Length;
            double avg = frequencies.Average();
            double std = frequencies.Average(x => Math.Pow(avg - x, 2));
            bool[] output = frequencies.Select(
                f => ((f - avg) / std) > 1.5
            ).ToArray();

            List<double> offBeatDurations = new List<double>();
            List<double> onBeatDurations = new List<double>();

            while(output.Length > 0)
            {
                try
                {
                    output = output.SkipWhile(o => o).ToArray();
                    double skipped = Convert.ToDouble(count - output.Length);
                    onBeatDurations.Add(skipped * durPerWindow);
                    count -= Convert.ToInt32(skipped);

                    output = output.SkipWhile(o => !o).ToArray();
                    skipped = Convert.ToDouble(count - output.Length);
                    offBeatDurations.Add(skipped * durPerWindow);
                    count -= Convert.ToInt32(skipped);
                }
                catch
                {
                    break;
                }
            }

            return Convert.ToInt32(offBeatDurations.Count / dur.TotalMinutes);
        }
    }
}
