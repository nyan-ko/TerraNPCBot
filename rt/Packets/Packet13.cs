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

    public class Packet13Parser : ParsedPacketBase {
        public byte id;
        public byte control;
        public byte pulley;
        public byte selected;
        public float posX;
        public float posY;
        public float vecX;
        public float vecY;

        public Packet13Parser(BinaryReader r) : base(0x11) {
            using (r) {
                id = r.ReadByte();
                control = r.ReadByte();
                pulley = r.ReadByte();
                selected = r.ReadByte();
                posX = r.ReadSingle();
                posY = r.ReadSingle();
                try {
                    vecX = r.ReadSingle();
                    vecY = r.ReadSingle();
                }
                catch (EndOfStreamException ex) {
                    return;
                }
            }
        }
    }
}
