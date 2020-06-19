using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.Server;
using Terraria;
using TShockAPI;

namespace TerraNPCBot.Utils {
    public static class ExtensionUtils {

        #region Unused, thank god
        //       public enum TypeSerialization {
        //           Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, String, Vector2, Color
        //       }

        //       private static Dictionary<Type, TypeSerialization> serializationByType = new Dictionary<Type, TypeSerialization> {
        //           { typeof(bool), TypeSerialization.Boolean },
        //           { typeof(byte), TypeSerialization.Byte },
        //           { typeof(sbyte), TypeSerialization.SByte },
        //           { typeof(short), TypeSerialization.Int16 },
        //           { typeof(ushort), TypeSerialization.UInt16 },
        //           { typeof(int), TypeSerialization.Int32 },
        //           { typeof(uint), TypeSerialization.UInt32 },
        //           { typeof(long), TypeSerialization.Int64 },
        //           { typeof(ulong), TypeSerialization.UInt64 },
        //           { typeof(string), TypeSerialization.String },
        //           { typeof(Vector2), TypeSerialization.Vector2 },
        //           { typeof(Color), TypeSerialization.Color }
        //       };

        //       /// <summary>
        //       /// Writes an object casted to its underlying type.
        //       /// </summary>
        //       /// <param name="writer"></param>
        //       /// <param name="o"></param>
        //       // Absolutely disgusting
        //       public static void WriteObject(this BinaryWriter writer, object o) {
        //           if (serializationByType.TryGetValue(o.GetType(), out TypeSerialization type)) {
        //               switch (type) {
        //                   case TypeSerialization.Boolean:
        //                       writer.Write((bool)o);
        //                       break;

        //                   case TypeSerialization.Byte:
        //                       writer.Write((byte)o);
        //                       break;
        //                   case TypeSerialization.SByte:
        //                       writer.Write((sbyte)o);
        //                       break;

        //                   case TypeSerialization.Int16:
        //                       writer.Write((short)o);
        //                       break;
        //                   case TypeSerialization.UInt16:
        //                       writer.Write((ushort)o);
        //                       break;

        //                   case TypeSerialization.Int32:
        //                       writer.Write((int)o);
        //                       break;
        //                   case TypeSerialization.UInt32:
        //                       writer.Write((uint)o);
        //                       break;

        //                   case TypeSerialization.Int64:
        //                       writer.Write((long)o);
        //                       break;
        //                   case TypeSerialization.UInt64:
        //                       writer.Write((ulong)o);
        //                       break;

        //                   case TypeSerialization.Single:
        //                       writer.Write((float)o);
        //                       break;  // Doubles are not used in terraria packets
        //                   case TypeSerialization.String:
        //                       writer.Write((string)o);
        //                       break;

        //                   case TypeSerialization.Vector2:
        //                       writer.WriteVector2((Vector2)o);
        //                       break;
        //                   case TypeSerialization.Color:
        //                       writer.WriteColor((Color)o);
        //                       break;

        //                   default:
        //                       throw new InvalidOperationException();
        //}
        //           }
        //       }

        #endregion

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
