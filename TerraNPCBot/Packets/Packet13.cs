using System;
using System.IO;
using Microsoft.Xna.Framework;
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
                Packetize();
            }
        }

        /// <summary>
        /// Update player (13) sans vector update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="control"></param>
        /// <param name="pulley"></param>
        /// <param name="selected"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        public Packet13(byte id, byte control, byte pulley, byte selected,
            float posX, float posY) : base(0xD) {
            using (Amanuensis = new BinaryWriter(new MemoryStream())) {
                Amanuensis.Write(id);
                Amanuensis.Write(control);
                Amanuensis.Write(pulley);
                Amanuensis.Write(selected);
                Amanuensis.Write(posX);
                Amanuensis.Write(posY);
                Packetize();
            }
        }

        public Packet13(byte id, byte control, byte pulley, byte selected, Vector2 pos) : base(13) => new Packet13(id, control, pulley, selected, pos.X, pos.Y);

        public Packet13(byte id, byte control, byte pulley, byte selected, Vector2 pos, Vector2 vel) : base(13) => new Packet13(id, control, pulley, selected, pos.X, pos.Y, vel.X, vel.Y);

    }

    public class Packet13Parser : ParsedPacketBase {
        public Packet13Parser(Stream r) : base(0xD) {
            using (data = new MemoryStream()) {
                r.CopyTo(data);
            }
        }
    }
}
