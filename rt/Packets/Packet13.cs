using System;
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
            AddData(id.ToString());
            AddData(control.ToString());
            AddData(pulley.ToString());
            AddData(selected.ToString());
            AddStructuredData<float>(posX);
            AddStructuredData<float>(posY);
            AddStructuredData<float>(vecX);
            AddStructuredData<float>(vecY);
        }
    }
}
