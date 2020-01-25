using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace rt.Utils {
    public class StreamWriter {
        public static void ConvertToStream(BTSPlayer player) {
            using (BinaryWriter writer =
                new BinaryWriter(
                    new BufferedStream(
                            new GZipStream(
                                    File.Open(PluginUtils.GetSavePath(player.SPlayer.User.ID), FileMode.Create), CompressionMode.Compress), Program.Program.BufferSize))) {
                writer.Write(Program.Program.PluginStreamVersion);
                writer.Write(player._botLimit);
                writer.Write(player._ownedBots.Count);
                foreach (var b in player._ownedBots) {
                    b.WriteToStream(writer);
                }
            }
        }

        public static BTSPlayer ConvertFromStream(string path, int index) {
            BTSPlayer plr = null;
            try {
                using (BinaryReader reader = new BinaryReader(
                        new BufferedStream(
                                new GZipStream(File.Open(path, FileMode.Open), CompressionMode.Decompress)))) {
                    if (!IsStreamCurrentVersion(reader.ReadByte()))
                        return null;
                    plr._botLimit = reader.ReadUInt32();
                    var count = reader.ReadInt32();
                    for (int i = 0; i < count; ++i) {
                        plr._ownedBots.Add(BotFromStream(reader, index));
                    }
                }
            }
            catch (FileNotFoundException) {
                return null;  //Flag102
            }
            catch (EndOfStreamException) {
                return null;  //Flag102
            }
            return null;
        }

        public static bool IsStreamCurrentVersion(byte version) {
            return version == Program.Program.PluginStreamVersion;
        }

        public static Bot BotFromStream(BinaryReader reader, int index) {
            Bot b = new Bot("127.0.0.1", index);
            b._player = PlayerFromStream(reader);
            var count = reader.ReadInt32();
            for (int i = 0; i < count; ++i) {
                b._recordedPackets.Add(RPacketFromStream(reader));
            }
            count = reader.ReadInt32();
            for (int i = 0; i < count; ++i) {
                var packetfuncpair = FunctionsFromStream(reader);
                b._manager._listenReact.Add(packetfuncpair.packet, packetfuncpair.function);
            }
            return b;
        }

        public static Player PlayerFromStream(BinaryReader reader) {
            Player plr = new Player("");
            plr.SkinVariant = reader.ReadByte();
            plr.HairType = reader.ReadByte();
            plr.Name = reader.ReadString();
            plr.HairDye = reader.ReadByte();
            plr.HVisuals1 = reader.ReadByte();
            plr.HVisuals2 = reader.ReadByte();
            plr.HMisc = reader.ReadByte();
            plr.HairColor = reader.ReadColor();
            plr.SkinColor = reader.ReadColor();
            plr.EyeColor = reader.ReadColor();
            plr.ShirtColor = reader.ReadColor();
            plr.UnderShirtColor = reader.ReadColor();
            plr.PantsColor = reader.ReadColor();
            plr.ShoeColor = reader.ReadColor();
            plr.Difficulty = reader.ReadByte();

            plr.CurHP = reader.ReadUInt16();
            plr.MaxHP = reader.ReadUInt16();
            plr.CurMana = reader.ReadUInt16();
            plr.MaxMana = reader.ReadUInt16();

            //Flag102 Inventory
            return plr;
        }

        public static RecordedPacket RPacketFromStream(BinaryReader reader) {
            var count = reader.ReadInt32();
            return new RecordedPacket(new StreamInfo(reader.ReadBytes(count), reader.ReadByte()),
                reader.ReadUInt32());
        }

        public static PacketFuncPair FunctionsFromStream(BinaryReader reader) {
            List<Func<EventPacketInfo, Task>> funcs = new List<Func<EventPacketInfo, Task>>();

            var packet = (PacketTypes)reader.ReadByte();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; ++i) {
                var funcname = (string)Enum.Parse(typeof(PacketTypes), reader.ReadInt32().ToString());
                MethodInfo func = typeof(Bot).GetMethod(funcname);
                var f = (Func<EventPacketInfo, Task>)func.CreateDelegate(typeof(Func<EventPacketInfo, Task>), func);
                funcs.Add(f);
            }
            return new PacketFuncPair(packet, new ParallelTask(funcs.ToArray()));
        }
    }
}
