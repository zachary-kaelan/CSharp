using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp.StaticDataEndpoint;

namespace RiotLib
{
    internal static class Extensions
    {
        public static void AddPath(this ImageStatic img, string dirPath)
        {
            img.Sprite = img.Full;
            img.Full = dirPath + @"\" + img.Full;
        }
    }
}