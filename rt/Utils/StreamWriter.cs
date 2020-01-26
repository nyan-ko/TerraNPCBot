using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace rt.Utils {
    public static class StreamWriter {

        public static bool IsStreamCurrentVersion(byte version) {
            return version == Program.Program.PluginStreamVersion;
        }

        #region Writing

        #region BTSPlayer
        public static void BTSPlayerToStream(BTSPlayer player) {
            if (player.SPlayer?.User?.ID == null)
                return;
            using (BinaryWriter writer =
                new BinaryWriter(
                    new BufferedStream(
                            new GZipStream(
                                    File.Open(PluginUtils.AllocateSavePath(player.SPlayer.User.ID), FileMode.Create), CompressionMode.Compress), Program.Program.BufferSize))) {
                writer.Write(Program.Program.PluginStreamVersion);
                writer.Write(player._autosave);
                writer.Write(player._botLimit);
                writer.Write(player._ownedBots.Count);
                foreach (var b in player._ownedBots) {
                    BotToStream(writer, b);
                }
            }
        }
        #endregion

        #region Bot
        public static void BotToStream(System.IO.BinaryWriter writer, Bot bot) {
            PlrInfoToStream(writer, bot._player);
            PlrInvToStream(writer, bot._player);

            if (bot._recordedPackets != null) {
                writer.Write(bot._recordedPackets.Count);
                foreach (var r in bot._recordedPackets) {
                    RPToStream(writer, r);
                }
            }
            else {
                writer.Write(0);
            }
            if (bot._manager._listenReact != null) {
                writer.Write(bot._manager._listenReact.Count);
                foreach (var e in bot._manager._listenReact) {
                    FuncToStream(writer, e);
                }
            }
            else {
                writer.Write(0);
            }
        }
        #endregion

        #region Player
        public static void PlrInfoToStream(System.IO.BinaryWriter writer, Player plr) {
            writer.Write(plr.SkinVariant);
            writer.Write(plr.HairType);
            writer.Write(plr.Name);
            writer.Write(plr.HairDye);
            writer.Write(plr.HVisuals1);
            writer.Write(plr.HVisuals2);
            writer.Write(plr.HMisc);
            writer.WriteColor(plr.HairColor);
            writer.WriteColor(plr.SkinColor);
            writer.WriteColor(plr.EyeColor);
            writer.WriteColor(plr.ShirtColor);
            writer.WriteColor(plr.UnderShirtColor);
            writer.WriteColor(plr.PantsColor);
            writer.WriteColor(plr.ShoeColor);
            writer.Write(plr.Difficulty);
            //28 bytes + name

            writer.Write(plr.CurHP);
            writer.Write(plr.MaxHP);
            //4 bytes

            writer.Write(plr.CurMana);
            writer.Write(plr.MaxMana);
            //4 bytes
        }

        public static void PlrInvToStream(System.IO.BinaryWriter writer, Player plr) {
            foreach (var current in plr.InventorySlots) {
                writer.ItemToStream(current);
            }
            foreach (var current in plr.ArmorSlots) {
                writer.ItemToStream(current);
            }
            foreach (var current in plr.DyeSlots) {
                writer.ItemToStream(current);
            }
            foreach (var current in plr.MiscEquipSlots) {
                writer.ItemToStream(current);
            }
            foreach (var current in plr.MiscDyeSlots) {
                writer.ItemToStream(current);
            }
        }

        public static void ItemToStream(this BinaryWriter writer, Terraria.Item item) {
            writer.Write((short)item.stack);
            writer.Write(item.prefix);
            writer.Write((short)item.netID);
        }
        #endregion

        #region Recorded Packets
        public static void RPToStream(BinaryWriter writer, RecordedPacket rp) {
            writer.Write(rp.stream.Buffer.Length);
            writer.Write(rp.stream.Buffer);
            writer.Write(rp.stream.Type);
            writer.Write(rp.timeBeforeNextPacket);
        }
        #endregion

        #region Event Listeners
        public static void FuncToStream(BinaryWriter writer, KeyValuePair<PacketTypes, ParallelTask> funk) {
            writer.Write((byte)funk.Key);
            writer.Write(funk.Value.Tasks.Count);
            foreach (var f in funk.Value.Tasks) {
                writer.Write((int)Enum.Parse(typeof(Functions), f.Method.Name));
            }
        }
        #endregion

        #endregion

        #region Reading
        public static BTSPlayer BTSPlayerFromStream(string path, int index) {
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

        public static Bot BotFromStream(BinaryReader reader, int index) {
            Bot b = new Bot("127.0.0.1", index) {
                _player = PlayerFromStream(reader)
            };
            var count = reader.ReadInt32();
            for (int i = 0; i < count; ++i) {
                b._recordedPackets.Add(RPFromStream(reader));
            }
            count = reader.ReadInt32();
            for (int i = 0; i < count; ++i) {
                var packetfuncpair = FuncFromStream(reader);
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

            for (int i = 0; i < plr.InventorySlots.Length; ++i) {
                plr.InventorySlots[i] = ItemFromStream(reader);
            }
            for (int i = 0; i < plr.ArmorSlots.Length; ++i) {
                plr.ArmorSlots[i] = ItemFromStream(reader);
            }
            for (int i = 0; i < plr.DyeSlots.Length; ++i) {
                plr.DyeSlots[i] = ItemFromStream(reader);
            }
            for (int i = 0; i < plr.MiscEquipSlots.Length; ++i) {
                plr.MiscEquipSlots[i] = ItemFromStream(reader);
            }
            for (int i = 0; i < plr.MiscDyeSlots.Length; ++i) {
                plr.MiscDyeSlots[i] = ItemFromStream(reader);
            }
            return plr;
        }

        public static Terraria.Item ItemFromStream(BinaryReader reader) {
            return new Terraria.Item() { stack = reader.ReadInt16(), prefix = reader.ReadByte(), netID = reader.ReadInt16() };
        }

        public static RecordedPacket RPFromStream(BinaryReader reader) {
            var count = reader.ReadInt32();
            return new RecordedPacket(new StreamInfo(reader.ReadBytes(count), reader.ReadByte()),
                reader.ReadUInt32());
        }

        public static PacketFuncPair FuncFromStream(BinaryReader reader) {
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

        #endregion
    }
}
