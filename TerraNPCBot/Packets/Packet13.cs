using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Update player (13)
    /// </summary>
    public class Packet13 : PacketBase {
        /// <summary>
        /// Update player (13)
        /// </summary>
        public Packet13(byte id, byte control, byte pulley, byte selected,
            float posX, float posY, float vecX, float vecY) : base(0xD) {
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

        public Packet13(byte id, byte control, byte pulley, byte selected,
            float posX, float posY) : base(0xD) {
            using (Amanuensis = new BinaryWriter(new MemoryStream())) {
                Amanuensis.Write(id);
                Amanuensis.Write(control);
                Amanuensis.Write(pulley);
                Amanuensis.Write(selected);
                Amanuensis.Write(posX);
                Amanuensis.Write(posY);
                AddData(Amanuensis.BaseStream);
            }
        }
    }

    public class Packet13Parser : ParsedPacketBase {
        public Packet13Parser(Stream r) : base(0xD) {
            using (_data = new MemoryStream()) {
                r.CopyTo(_data);
            }
        }
    }
}
