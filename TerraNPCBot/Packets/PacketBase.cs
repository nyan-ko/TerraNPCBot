using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria.Net.Sockets;
using TShockAPI;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using TerrariaApi.Server;
using Terraria;

namespace TerraNPCBot {
    public class PacketBase {

        public byte packetType;
        public List<int> targets = new List<int>();

        private byte[] data;
        protected BinaryWriter Amanuensis;

        protected PacketBase(byte _packetType) {
            Amanuensis = new BinaryWriter(new MemoryStream());
            packetType = _packetType;
        }

        /// <summary>
        /// Creates the actual packet from the data stream with a header.
        /// </summary>
        /// <param name="stream"></param>
        protected void Packetize() { // I will NOT switch to American English
            using (MemoryStream stream = new MemoryStream()) {
                short packetLength = (short)(Amanuensis.BaseStream.Position + 3);
                Amanuensis.Write(packetLength);
                Amanuensis.Write(packetType);
                Amanuensis.BaseStream.Position = 0;
                Amanuensis.BaseStream.CopyTo(stream);
                data = new byte[packetLength];
                byte[] temp = stream.ToArray();

                Buffer.BlockCopy(temp, 0, data, 3, temp.Length - 3); // Copies actual packet data
                Buffer.BlockCopy(temp, packetLength - 3, data, 0, 3); // Copies packet header (length, type)
            }
        }

        public void Send() {
            // Force sent player active packet
            // Must be initialized with a target in the 'targets' list
            if (packetType == 254) { 
                try {
                    RemoteClient target = Netplay.Clients[targets[0]];
                    target.Socket.AsyncSend(data, 0, data.Length, target.ServerWriteCallBack);
                    return;
                }
                catch (IndexOutOfRangeException) { }
            }

            // Specific targets
            if (targets.Count != 0) {
                foreach (int i in targets) {
                    if (Program.Program.Players[i].IgnoreBots)
                        return;
                    try {
                        Netplay.Clients[i].Socket.AsyncSend(data, 0, data.Length, Netplay.Clients[i].ServerWriteCallBack);
                    }
                    catch { }
                }
                return;
            }

            // Entire server
            for (int i = 0; i < 256; ++i) {
                if (Netplay.Clients[i].IsConnected() && !Program.Program.Players[i].IgnoreBots) {
                    try {
                        Netplay.Clients[i].Socket.AsyncSend(data, 0, data.Length, Netplay.Clients[i].ServerWriteCallBack);
                    }
                    catch { }
                }
            }
        }

        //public static ParsedPacketBase Parse(BinaryReader reader, Player plr, Bot bot) {
        //    ParsedPacketBase packet = null;

        //    short length;
        //    byte type;

        //    using (reader) {
        //        length = reader.ReadInt16();
        //        type = reader.ReadByte();

        //        // Flag102
        //        switch (type) {
        //            case 13: // player update
        //                packet = new Packets.Packet13Parser(reader.BaseStream);
        //                break;
        //        }
        //    }

        //    return packet;
        //}

        public static PacketBase WriteFromRecorded(StreamInfo r, Bot b) {
            PacketBase packet = null;

            using (var reader = new BinaryReader(new MemoryStream(r.Buffer))) {
                try {
                    switch (r.Type) {
                        case 5: {
                                reader.BaseStream.Position += 1;
                                var slot = reader.ReadByte();
                                var stack = reader.ReadInt16();
                                var prefix = reader.ReadByte();
                                var id = reader.ReadInt16();

                                packet = new Packets.Packet5(b.ID, slot, stack, prefix, id);
                            }
                            break;  // inv slot
                        case 12: {
                                reader.BaseStream.Position += 1;
                                var spawnx = reader.ReadInt16();
                                var spawny = reader.ReadInt16();

                                packet = new Packets.Packet12(b.ID, spawnx, spawny);
                            }
                            break;  // player spawn
                        case 13: {
                                reader.BaseStream.Position += 1;
                                var num1 = reader.ReadByte();
                                var num2 = reader.ReadByte();
                                var num3 = reader.ReadByte();
                                var num4 = reader.ReadSingle();
                                var num5 = reader.ReadSingle();
                                float num6 = 0.0F;
                                float num7 = 0.0F;
                                if ((num2 & 4) == 4) {
                                    num6 = reader.ReadSingle();
                                    num7 = reader.ReadSingle();
                                }  // update velocity

                                packet = new Packets.Packet13(b.ID, num1, num2, num3, num4, num5, num6, num7);
                            }
                            break;  // player update
                        case 16: {
                                reader.BaseStream.Position += 1;
                                var hp = reader.ReadInt16();
                                var max = reader.ReadInt16();
                                packet = new Packets.Packet16(b.ID, hp, max);
                            }
                            break;  // player hp
                        case 17: {
                                var action = reader.ReadByte();
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var var1 = reader.ReadInt16();
                                var var2 = reader.ReadByte();

                                packet = new Packets.Packet17(action, x, y, var1, var2);
                            }
                            break;  // modify tile
                        case 19: {
                                var action = reader.ReadByte();
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var dir = reader.ReadByte();

                                packet = new Packets.Packet19(action, x, y, dir);
                            }
                            break;  // door toggle
                        case 30: {
                                reader.BaseStream.Position += 1;
                                var pvp = reader.ReadBoolean();

                                packet = new Packets.Packet30(b.ID, pvp);
                            }
                            break;  // toggle pvp
                        case 31: {
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();

                                packet = new Packets.Packet31(x, y);
                            }
                            break;  // open chest Flag103
                        case 32: {
                                var id = reader.ReadInt16();
                                var slot = reader.ReadByte();
                                var stack = reader.ReadInt16();
                                var prefix = reader.ReadByte();
                                var nid = reader.ReadInt16();

                                packet = new Packets.Packet32(id, slot, stack, prefix, nid);
                            }
                            break;  // update chest item
                        case 41: {
                                reader.BaseStream.Position += 1;
                                var rot = reader.ReadSingle();
                                var ani = reader.ReadInt16();

                                packet = new Packets.Packet41(b.ID, rot, ani);
                            }
                            break;  // item ani
                        case 42: {
                                reader.BaseStream.Position += 1;
                                var mana = reader.ReadInt16();
                                var max = reader.ReadInt16();

                                packet = new Packets.Packet42(b.ID, mana, max);
                            }
                            break;  // player mana
                        case 45: {
                                reader.BaseStream.Position += 1;
                                var team = reader.ReadByte();

                                packet = new Packets.Packet45(b.ID, team);
                            }
                            break;  // player team
                        case 50: {
                                reader.BaseStream.Position += 1;
                                var buffs = reader.ReadBytes(22);

                                packet = new Packets.Packet50(b.ID, buffs);
                            }
                            break;  // player buff
                        case 55: {
                                reader.BaseStream.Position += 1;
                                var buff = reader.ReadByte();
                                var time = reader.ReadInt32();

                                packet = new Packets.Packet55(b.ID, buff, time);
                            }
                            break;  // add player buff
                        case 61: {
                                reader.BaseStream.Position += 2;
                                var type = reader.ReadInt16();

                                packet = new Packets.Packet61(b.ID, type);
                            }
                            break;  // spawn boss invasion
                        case 65: {
                                var flag = reader.ReadByte();
                                var target = reader.ReadInt16();
                                var x = reader.ReadSingle();
                                var y = reader.ReadSingle();

                                packet = new Packets.Packet65(flag, target, x, y);
                            }
                            break;  // player npc teleport
                        case 71: {
                                var x = reader.ReadInt32();
                                var y = reader.ReadInt32();
                                var type = reader.ReadInt16();
                                var style = reader.ReadByte();

                                packet = new Packets.Packet71(x, y, type, style);
                            }
                            break;  // release npc
                        case 87: {
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var type = reader.ReadByte();

                                packet = new Packets.Packet87(x, y, type);
                            }
                            break;  // place tile entity
                        case 89: {
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var id = reader.ReadInt16();
                                var prefix = reader.ReadByte();
                                var stack = reader.ReadInt16();

                                packet = new Packets.Packet89(x, y, id, prefix, stack);
                            }
                            break;  // place item frame
                        case 95: {
                                var index = reader.ReadUInt16();

                                packet = new Packets.Packet95(index);
                            }
                            break;  // kill portal
                        case 96: {
                                reader.BaseStream.Position += 1;
                                var portalcolorindex = reader.ReadInt16();
                                var posx = reader.ReadSingle();
                                var posy = reader.ReadSingle();
                                var x = reader.ReadSingle();
                                var y = reader.ReadSingle();

                                packet = new Packets.Packet96(b.ID, portalcolorindex, posx, posy, x, y);
                            }
                            break;  // player teleport portal
                        case 105: {
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var on = reader.ReadBoolean();

                                packet = new Packets.Packet105(x, y, on);
                            }
                            break;  // gem lock toggle
                        case 109: {
                                var sx = reader.ReadInt16();
                                var sy = reader.ReadInt16();
                                var ex = reader.ReadInt16();
                                var ey = reader.ReadInt16();
                                var mode = reader.ReadByte();

                                packet = new Packets.Packet109(sx, sy, ex, ey, mode);
                            }
                            break;  // mass wire operation
                        case 111: {
                                packet = new Packets.Packet111();
                            }
                            break;  // toggle birthday
                        case 113: {
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();

                                packet = new Packets.Packet113(x, y);
                            }
                            break;  // crystal invasion start
                        case 117: {
                                reader.BaseStream.Position += 1;
                                var reason = Terraria.DataStructures.PlayerDeathReason.FromReader(reader);
                                var dmg = reader.ReadInt16();
                                var dir = reader.ReadByte();
                                var flags = reader.ReadByte();
                                var cc = reader.ReadSByte();

                                packet = new Packets.Packet117(b.ID, reason, dmg, dir, flags, cc);
                            }
                            break;  // player hurt
                        case 118: {
                                reader.BaseStream.Position += 1;
                                var reason = Terraria.DataStructures.PlayerDeathReason.FromReader(reader);
                                var dmg = reader.ReadInt16();
                                var dir = reader.ReadByte();
                                var flags = reader.ReadByte();

                                packet = new Packets.Packet118(b.ID, reason, dmg, dir, flags);
                            }
                            break;  // player death
                    }
                }
                catch (Exception ex) {
                    TShockAPI.TShock.Log.Write($"Exception thrown with writing packet from parsed data: {ex.ToString()}, Bot: {b.Name}", System.Diagnostics.TraceLevel.Error);
                    return packet;
                }
            }
            return packet;
        }
    }

    public class ParsedPacketBase {
        public MemoryStream data;
        public uint packetType;

        public ParsedPacketBase(uint packet) {
            packetType = packet;
        }        
    }
}
