using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.Server;
using TShockAPI;

namespace TerraNPCBot.Utils {
    public static class ExtensionUtils {

        /// <summary>
        /// Writes an object casted to its underlying type.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="o"></param>

        // Absolutely disgusting, you have been warned
        public static void WriteObject(this BinaryWriter writer, object o) {
            switch (Type.GetTypeCode(o.GetType())) {
                case TypeCode.Boolean:
                    writer.Write((bool)o);
                    break;

                case TypeCode.Byte:
                    writer.Write((byte)o);
                    break;
                case TypeCode.SByte:
                    writer.Write((sbyte)o);
                    break;

                case TypeCode.Int16:
                    writer.Write((short)o);
                    break;
                case TypeCode.UInt16:
                    writer.Write((ushort)o);
                    break;

                case TypeCode.Int32:
                    writer.Write((int)o);
                    break;
                case TypeCode.UInt32:
                    writer.Write((uint)o);
                    break;

                case TypeCode.Int64:
                    writer.Write((long)o);
                    break;
                case TypeCode.UInt64:
                    writer.Write((ulong)o);
                    break;

                case TypeCode.Single:
                    writer.Write((float)o);
                    break;  // Doubles are not used in terraria packets
                case TypeCode.String:
                    writer.Write((string)o);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        public static void WriteColor(this BinaryWriter writer, Color color) {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }

        public static Color ReadColor(this BinaryReader reader) {
            return new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }

        public static void MultiMsg(this TSPlayer player, Color color, params string[] msgs) {
            foreach (var x in msgs) {
                player?.SendMessage(x, color);
            }
            player?.SendMessage("Use ↑/↓ to scroll through the message.", Microsoft.Xna.Framework.Color.Yellow);
        }

        public static void MultiMsg(this TSPlayer player, string message, Color color) {
            MultiMsg(player, color, message.Split('\\'));
        }
    }
}
