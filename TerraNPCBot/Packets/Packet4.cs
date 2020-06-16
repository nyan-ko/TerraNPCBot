using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerraNPCBot.Utils;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Player info (4)
    /// </summary>
    public class Packet4 : PacketBase {
        /// <summary>
        /// Player info (4) //TODO
        /// </summary>
        public Packet4 (Player plr) : base(0x4) {
            using (Amanuensis) {
                Amanuensis.Write((byte)plr.Index);
                Amanuensis.Write(plr.SkinVariant);
                Amanuensis.Write(plr.HairType);

                Amanuensis.Write(plr.Name);

                Amanuensis.Write(plr.HairDye);
                Amanuensis.Write(plr.HideVisuals);
                Amanuensis.Write(plr.HideVisuals2);
                Amanuensis.Write(plr.HideMisc);

                Amanuensis.WriteColor(plr.HairColor);

                Amanuensis.WriteColor(plr.SkinColor);

                Amanuensis.WriteColor(plr.EyeColor);

                Amanuensis.WriteColor(plr.ShirtColor);

                Amanuensis.WriteColor(plr.UndershirtColor);

                Amanuensis.WriteColor(plr.PantsColor);

                Amanuensis.WriteColor(plr.ShoeColor);

                Amanuensis.Write(plr.Difficulty);

                Amanuensis.Write(plr.TorchFlags);
                Packetize();
            }
        }
    }
}
