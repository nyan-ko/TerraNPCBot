using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Update player (13)
    /// </summary>
    public class Packet13 : PacketBase {
        public Packet13(byte id, byte control, byte pulley, byte selected,
            float posX, float posY, float vecX, float vecY) : base(0xD, new List<byte>()) {
            using (Amanuensis = new BinaryWriter(new MemoryStream())) {
                Amanuensis.Write(id);
                Amanuensis.Write(control);
                Amanuensis.Write(pulley);
                Amanuensis.Write(selected);
                Amanuensis.Write(posX);
                Amanuensis.Write(posY);
                Amanuensis.Write(vecX);
                Amanuensis.Write(vecY);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
