using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Utils;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Player info (4)
    /// </summary>
    public class Packet4 : PacketBase {
        /// <summary>
        /// Player info (4)
        /// </summary>
        public Packet4 (Player plr) : base(0x4) {
            using (Amanuensis) {
                Amanuensis.Write((byte)plr.PlayerID);
                Amanuensis.Write(plr.SkinVariant);
                Amanuensis.Write(plr.HairType);

                Amanuensis.Write(plr.Name);

                Amanuensis.Write(plr.HairDye);
                Amanuensis.Write(plr.HVisuals1);
                Amanuensis.Write(plr.HVisuals2);
                Amanuensis.Write(plr.HMisc);

                Amanuensis.WriteColor(plr.HairColor);

                Amanuensis.WriteColor(plr.SkinColor);

                Amanuensis.WriteColor(plr.EyeColor);

                Amanuensis.WriteColor(plr.ShirtColor);

                Amanuensis.WriteColor(plr.UnderShirtColor);

                Amanuensis.WriteColor(plr.PantsColor);

                Amanuensis.WriteColor(plr.ShoeColor);

                Amanuensis.Write(plr.Difficulty);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
