using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace WargamingAPI.Models
{
    public class PlayerSimplified
    {
        public int AccountID { get; private set; }
        public long CreatedAt { get; private set; }
        public long UpdatedAt { get; private set; }
        public long LastBattleTime { get; private set; }

        [JilDirective("spts")]
        public int Spotted { get; private set; }
        [JilDirective("hits")]
        public int Hits { get; private set; }
        [JilDirective("frgs")]
        public int Frags { get; private set; }
        [JilDirective("cpts")]
        public int CapturePoints { get; private set; }
        [JilDirective("ddlt")]
        public int DamageDealt { get; private set; }
        [JilDirective("dtkn")]
        public int DamageReceived { get; private set; }
        [JilDirective("shts")]
        public int ShotsFired { get; private set; }
        [JilDirective("t8ps")]
        public int FragsTier8Plus { get; private set; }
        [JilDirective("xp")]
        public int Experience { get; private set; }
        [JilDirective("dpts")]
        public int DefensePoints { get; private set; }
        [JilDirective("btls")]
        public int Battles { get; private set; }
        [JilDirective("wins")]
        public int Wins { get; private set; }
        [JilDirective("srvd")]
        public int Survived { get; private set; }
    }
}
