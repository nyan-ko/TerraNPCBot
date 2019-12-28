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
            AddData(plr.PlayerID.ToString());
            AddData(plr.SkinVariant.ToString());
            AddData(plr.HairType.ToString());
            AddData(plr.Name, true);
            AddData(plr.HairDye.ToString());
            AddData(plr.HVisuals1.ToString());
            AddData(plr.HVisuals2.ToString());
            AddData(plr.HMisc.ToString());

            AddData(plr.HairColor.R.ToString());
            AddData(plr.HairColor.G.ToString());
            AddData(plr.HairColor.B.ToString());

            AddData(plr.SkinColor.R.ToString());
            AddData(plr.SkinColor.G.ToString());
            AddData(plr.SkinColor.B.ToString());

            AddData(plr.EyeColor.R.ToString());
            AddData(plr.EyeColor.G.ToString());
            AddData(plr.EyeColor.B.ToString());

            AddData(plr.ShirtColor.R.ToString());
            AddData(plr.ShirtColor.G.ToString());
            AddData(plr.ShirtColor.B.ToString());

            AddData(plr.UnderShirtColor.R.ToString());
            AddData(plr.UnderShirtColor.G.ToString());
            AddData(plr.UnderShirtColor.B.ToString());

            AddData(plr.PantsColor.R.ToString());
            AddData(plr.PantsColor.G.ToString());
            AddData(plr.PantsColor.B.ToString());

            AddData(plr.ShoeColor.R.ToString());
            AddData(plr.ShoeColor.G.ToString());
            AddData(plr.ShoeColor.B.ToString());

            AddData(plr.Difficulty.ToString());
        }
    }
}
