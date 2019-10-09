using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressingMatters
{
    [Flags]
    public enum Tags : ushort
    {
        None,
        Sexy,
        Tits,
        Lips = 4,
        Legs = 8,
        Ass = 16,
        Eyes = 32,
        Hair = 64,
        Cute = 128,
        HighQuality = 256,
        WhoreLook = 512,
        Young = 1024,
        Paused = 2048
    }

    internal struct Matter
    {
        public ushort ID { get; set; }
        public byte Subject { get; set; }
        public Tags Tags { get; set; }
        public byte VideoLength { get; set; }
        // In quarters
        public byte DateAdded { get; set; }
        public ushort NumViews { get; set; }

        public void SerializeWith(FileStream file)
        {
            byte[] bytes = BitConverter.GetBytes(ID);
            file.Write(bytes, 0, 4);
            file.WriteByte(Subject);
            bytes = BitConverter.GetBytes(Tags);
            file.Write(bytes, 0, 4);
            file.WriteByte(VideoLength);
            file.WriteByte(DateAdded);
            bytes = BitConverter.GetBytes(NumViews);
            file.Write(bytes, 0, 4);
        }

        public static bool TryLoadFrom(FileStream file, out Matter matter)
        {
            if (file.CanRead)
            {
                matter = new Matter();
                byte[] bytes = new byte[2];
                try
                {
                    file.Read(bytes, 0, 2);
                    matter.ID = BitConverter.ToUInt16(bytes, 0);
                    matter.Subject = file.ReadByte();
                    file.Read(bytes, 0, 2);
                    matter.Tags = (Tags)BitConverter.ToUInt16(bytes, 0);
                    matter.VideoLength = file.ReadByte();
                    matter.DateAdded = file.ReadByte();
                    file.Read(bytes, 0, 2);
                    matter.NumViews = BitConverter.ToUInt16(bytes, 0);
                    return true;
                }
                catch
                {

                }
            }
            return false;
        }
    }
}
