using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rt.Utils;

namespace rt.Packets {
    /// <summary>
    /// Player info (4)
    /// </summary>
    public class Packet4 : PacketBase {
        public Packet4 (Player plr) : base(0x4, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(plr.PlayerID);
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
