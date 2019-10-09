using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinderPlus
{
    [Flags]
    public enum ProgressType
    {
        None = 0,
        Text = 1,
        Increment = 2,
        LoadingInitialize = 4
    }

    class LoadingProgress
    {
        public ProgressType type;
        public string txt;
        public bool increment;
        public int loadingMax;

        public LoadingProgress()
        {
            loadingMax = -1;
            type = ProgressType.LoadingInitialize;
        }

        public LoadingProgress(string txt)
        {
            this.txt = txt;
            type = ProgressType.Text;
        }

        public LoadingProgress(bool increment, int max)
        {
            this.increment = increment;
            this.loadingMax = max;
            type = ProgressType.LoadingInitialize | ProgressType.Increment;
        }

        public LoadingProgress(bool increment, string txt)
        {
            this.txt = txt;
            this.increment = increment;
            type = ProgressType.Text | ProgressType.Increment;
        }

        public LoadingProgress(bool increment, int max, string txt)
        {
            this.txt = txt;
            this.increment = increment;
            this.loadingMax = max;
            type = ProgressType.Text | ProgressType.Increment | ProgressType.LoadingInitialize;
        }

        public LoadingProgress(int max)
        {
            this.loadingMax = max;
            type = ProgressType.LoadingInitialize;
        }

        public LoadingProgress(int max, string txt)
        {
            this.txt = txt;
            this.loadingMax = max;
            type = ProgressType.Text | ProgressType.LoadingInitialize;
        }
    }
}
