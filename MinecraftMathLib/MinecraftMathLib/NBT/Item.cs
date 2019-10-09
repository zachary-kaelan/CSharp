using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fNbt;

namespace MinecraftMathLib.NBT
{
    public class Item
    {
        private NbtString _id = new NbtString("id");
        public string id { get => _id.Value; set => _id.Value = value; }

        private NbtByte _count = new NbtByte("Count");
        public byte Count { get => _count.Value; set => _count.Value = value; }

        private NbtShort _damage = new NbtShort("Damage");
        public short Damage { get => _damage.Value; set => _damage.Value = value; }


        public NbtCompound ToCompound(string tagName) =>
            new NbtCompound(
                tagName,
                new NbtTag[]
                {
                    _id,
                    _count,
                    _damage
                }
            );
    }
}
