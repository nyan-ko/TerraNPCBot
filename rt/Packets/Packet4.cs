using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                AddData(Amanuensis.BaseStream);

                EncodeString(plr.Name);

                Amanuensis.Write(plr.HairDye);
                Amanuensis.Write(plr.HVisuals1);
                Amanuensis.Write(plr.HVisuals2);
                Amanuensis.Write(plr.HMisc);

                Amanuensis.Write(plr.HairColor.R);
                Amanuensis.Write(plr.HairColor.G);
                Amanuensis.Write(plr.HairColor.B);

                Amanuensis.Write(plr.SkinColor.R);
                Amanuensis.Write(plr.SkinColor.G);
                Amanuensis.Write(plr.SkinColor.B);

                Amanuensis.Write(plr.EyeColor.R);
                Amanuensis.Write(plr.EyeColor.G);
                Amanuensis.Write(plr.EyeColor.B);

                Amanuensis.Write(plr.ShirtColor.R);
                Amanuensis.Write(plr.ShirtColor.G);
                Amanuensis.Write(plr.ShirtColor.B);

                Amanuensis.Write(plr.UnderShirtColor.R);
                Amanuensis.Write(plr.UnderShirtColor.G);
                Amanuensis.Write(plr.UnderShirtColor.B);

                Amanuensis.Write(plr.PantsColor.R);
                Amanuensis.Write(plr.PantsColor.G);
                Amanuensis.Write(plr.PantsColor.B);

                Amanuensis.Write(plr.ShoeColor.R);
                Amanuensis.Write(plr.ShoeColor.G);
                Amanuensis.Write(plr.ShoeColor.B);

                Amanuensis.Write(plr.Difficulty);
            }
            AddData(Amanuensis.BaseStream);
        }
    }
}
