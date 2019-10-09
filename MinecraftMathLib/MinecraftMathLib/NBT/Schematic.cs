using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fNbt;

namespace MinecraftMathLib.NBT
{
    public class Schematic
    {
        private NbtShort _width = new NbtShort("Width");
        public short Width { get => _width.Value; set => _width.Value = value; }

        private NbtShort _height = new NbtShort("Height");
        public short Height { get => _height.Value; set => _height.Value = value; }

        private NbtShort _length = new NbtShort("Length");
        public short Length { get => _length.Value; set => _length.Value = value; }

        private NbtString _materials = new NbtString("Materials");
        public string Materials { get => _materials.Value; set => _materials.Value = value; }

        private NbtByteArray _blocks = new NbtByteArray("Blocks");
        public byte[] Blocks { get => _blocks.Value; set => _blocks.Value = value; }

        private NbtByteArray _addblocks = new NbtByteArray("AddBlocks");
        public byte[] AddBlocks { get => _addblocks.Value; set => _addblocks.Value = value; }

        private NbtByteArray _data = new NbtByteArray("Data");
        public byte[] Data { get => _data.Value; set => _data.Value = value; }

        private NbtList _tileentities = new NbtList("TileEntities", NbtTagType.Compound);
        public List<NbtCompound> TileEntities { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private NbtList _entities = new NbtList("Entities", NbtTagType.Compound);
        public NbtList Entities { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Item Icon { get; set; }

        public Dictionary<string, short> SchematicaMapping { get; set; }

        //private NbtCompound _extendedmetadata = new NbtCompound("ExtendedMetadata");
        //public compound ExtendedMetadata { get => _extendedmetadata.Value; set => _extendedmetadata.Value = value; }



    }
}
