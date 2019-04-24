using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDTesting
{
    internal class GameInfo
    {
        private string _Name;

        public uint Id;
        public string Type;
        public int ImageIndex;

        public string Name
        {
            get { return this._Name; }
            set { this._Name = value ?? "App " + this.Id.ToString(CultureInfo.InvariantCulture); }
        }

        public string Logo;

        public GameInfo(uint id, string type)
        {
            this.Id = id;
            this.Type = type;
            this.Name = null;
            this.ImageIndex = 0;
            this.Logo = null;
        }
    }
}
