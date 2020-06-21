using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace TerraNPCBot {
    public class RecordedPacket {
        public StreamInfo stream;
        public uint timeBeforeNextPacket;

        public RecordedPacket(StreamInfo st, uint time) {
            stream = st;
            timeBeforeNextPacket = time;
        }        

        public override string ToString() {
            string s = "";

            s += "Packet: " + (PacketTypes)stream.Type;
            s += $", {timeBeforeNextPacket} milliseconds until next packet.";

            return s;
        }

                public static PacketBase WriteFromRecorded(StreamInfo r, Bot b) {
            PacketBase packet = null;

            using (var reader = new BinaryReader(new MemoryStream(r.Buffer))) {
                try {
                    switch (r.Type) {
                        case 5: {
                                reader.BaseStream.Position += 1;
                                var slot = reader.ReadInt16();
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
                                var timeRemain = reader.ReadInt32();
                                var context = reader.ReadByte();

                                packet = new Packets.Packet12(b.ID, spawnx, spawny, timeRemain, context);
                            }
                            break;  // player spawn
                        case 13: {
                                reader.BaseStream.Position += 1;
                                var control = reader.ReadByte();
                                var pulley = reader.ReadByte();
                                var misc = reader.ReadByte();
                                var sleepingInfo = reader.ReadByte();
                                var selectedItem = reader.ReadByte();
                                var posX = reader.ReadSingle();
                                var posY = reader.ReadSingle();
                                float vecX = 0.0F;
                                float vecY = 0.0F;
                                float oPosX = 0.0F;
                                float oPosY = 0.0F;
                                float hPosX = 0.0F;
                                float hPosY = 0.0F;
                                if ((pulley & 4) == 4) {
                                    vecX = reader.ReadSingle();
                                    vecY = reader.ReadSingle();
                                }  // update velocity
                                if ((misc & 64) == 64) {
                                    oPosX = reader.ReadSingle();
                                    oPosY = reader.ReadSingle();
                                    hPosX = reader.ReadSingle();
                                    hPosY = reader.ReadSingle();
                                }  // used potion of return

                                packet = new Packets.Packet13(b.ID, control, pulley, misc, sleepingInfo, selectedItem, posX, posY,
                                    vecX, vecY, oPosX, oPosY, hPosX, hPosY);
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
                                ushort[] buffs = new ushort[22];
                                for (int i = 0; i < buffs.Length; i++) {
                                    buffs[i] = reader.ReadUInt16();
                                }

                                packet = new Packets.Packet50(b.ID, buffs);
                            }
                            break;  // player buff
                        case 55: {
                                reader.BaseStream.Position += 1;
                                var buff = reader.ReadUInt16();
                                var time = reader.ReadInt32();

                                packet = new Packets.Packet55(b.ID, buff, time);
                            }
                            break;  // add player buff
                        case 65: {
                                var flag = reader.ReadByte();
                                var target = reader.ReadInt16();
                                var x = reader.ReadSingle();
                                var y = reader.ReadSingle();
                                var style = reader.ReadByte();
                                int extra = 0;
                                if ((flag & 8) == 8) {
                                    extra = reader.ReadInt32();
                                }

                                packet = new Packets.Packet65(flag, target, x, y, style, extra);
                            }
                            break;  // player npc teleport
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

    public class StreamInfo {
        public byte[] Buffer;
        public byte Type;

        public StreamInfo(byte[] stream, byte type) {
            Buffer = stream;
            Type = type;
        }
    }
}
