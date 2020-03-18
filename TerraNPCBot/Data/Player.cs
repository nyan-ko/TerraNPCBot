using System;
using System.IO;
using Microsoft.Xna.Framework;
using TerraNPCBot.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace TerraNPCBot {
    /// <summary>
    /// Based off of PlayerInfo(4), InventorySlot(5), PlayerHP(16), and PlayerMana(42) packets.
    /// </summary>
    public class Player {
        public int PlayerID;
        public byte SkinVariant;
        public byte HairType;
        public string Name;
        public byte HairDye;
        public byte HVisuals1;
        public byte HVisuals2;
        public byte HMisc;
        public Color HairColor;
        public Color SkinColor;
        public Color EyeColor;
        public Color ShirtColor;
        public Color UnderShirtColor;
        public Color PantsColor;
        public Color ShoeColor;
        public byte Difficulty;

        public ushort MaxHP;
        public ushort CurHP;

        public ushort MaxMana;
        public ushort CurMana;

        public ItemData[] InventorySlots = new ItemData[NetItem.InventorySlots];
        public ItemData[] ArmorSlots = new ItemData[NetItem.ArmorSlots];
        public ItemData[] DyeSlots = new ItemData[NetItem.DyeSlots];
        public ItemData[] MiscEquipSlots = new ItemData[NetItem.MiscEquipSlots];
        public ItemData[] MiscDyeSlots = new ItemData[NetItem.MiscDyeSlots];

        /// <summary>
        /// Initializes the default bot player with a given name.
        /// </summary>
        /// <param name="name"></param>
        public Player(string name) {
            PlayerID = -1;
            SkinVariant = 0;
            HairType = 0;
            Name = name;
            HairDye = 0;
            HVisuals1 = 0;
            HVisuals2 = 0;
            HMisc = 0;
            HairColor = new Color(255, 255, 255);
            SkinColor = new Color(255, 255, 255);
            EyeColor = new Color(255, 255, 255);
            ShirtColor = new Color(255, 255, 255);
            UnderShirtColor = new Color(255, 255, 255);
            PantsColor = new Color(255, 255, 255);
            ShoeColor = new Color(255, 255, 255);
            Difficulty = 0;

            MaxHP = 420;
            CurHP = 420;

            MaxMana = 200;
            CurMana = 200;

            for (int i = 0; i < NetItem.InventorySlots; ++i) {
                InventorySlots[i] = new ItemData() { netID = 0, stack = 0, prefix = 0 }; // Do I need to do this
            }
            for (int i = 0; i < NetItem.ArmorSlots; ++i) {
                ArmorSlots[i] = new ItemData() { netID = 0, stack = 0, prefix = 0 };
            }
            for (int i = 0; i < NetItem.DyeSlots; ++i) {
                DyeSlots[i] = new ItemData() { netID = 0, stack = 0, prefix = 0 };
            }
            for (int i = 0; i < NetItem.MiscEquipSlots; ++i) {
                MiscEquipSlots[i] = new ItemData() { netID = 0, stack = 0, prefix = 0 };
            }
            for (int i = 0; i < NetItem.MiscDyeSlots; ++i) {
                MiscDyeSlots[i] = new ItemData() { netID = 0, stack = 0, prefix = 0 };
            }
        }
    }

    /// <summary>
    /// Stripped-down form of Terraria.Item to store item data.
    /// </summary>
    public class ItemData {
        public short netID;
        public short stack;
        public byte prefix;

        static internal ItemData FromTerrariaItem(Item i) {
            return new ItemData() { netID = (short)i.netID, stack = (short)i.stack, prefix = i.prefix };
        }
    }
}
