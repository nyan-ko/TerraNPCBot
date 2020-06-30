using System;
using System.IO;
using Microsoft.Xna.Framework;
using TerraNPCBot.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraBotLib;
using TerraBotLib.Events;
using TShockAPI;
using System.Collections;

namespace TerraNPCBot {
    /// <summary>
    /// Based off of PlayerInfo(4), InventorySlot(5), PlayerHP(16), and PlayerMana(42) packets.
    /// </summary>
    public class Player {
        public int Index { get; set; }
        public byte SkinVariant { get; set; }
        public byte HairType { get; set; }
        public string Name { get; set; }
        public byte HairDye { get; set; }
        public byte HideVisuals { get; set; }
        public byte HideVisuals2 { get; set; }
        public byte HideMisc { get; set; }
        public Color HairColor { get; set; }
        public Color SkinColor { get; set; }
        public Color EyeColor { get; set; }
        public Color ShirtColor { get; set; }
        public Color UndershirtColor { get; set; }
        public Color PantsColor { get; set; }
        public Color ShoeColor { get; set; }
        public byte Difficulty { get; set; }
        public byte TorchFlags { get; set; }

        public ushort MaxHP { get; set; }
        public ushort CurHP { get; set; }

        public ushort MaxMana { get; set; }
        public ushort CurMana { get; set; }

        public NetItem[] InventorySlots { get; set; }
        public NetItem[] ArmorSlots { get; set; }
        public NetItem[] DyeSlots { get; set; }
        public NetItem[] MiscEquipSlots { get; set; }
        public NetItem[] MiscDyeSlots { get; set; }

        private Bot internalBot;

        public void SetSlotItem(NetItem[] slots, NetItem item, int index) {
            if (!internalBot.EventHooks.InventoryChange.Invoke(this, new InventoryChangedEventArgs() { NewItem = item, OldItem = slots[index], Slot = index }))
                return;

            slots[index] = item;
        }
        /// <summary>
        /// Initializes the default bot player with a given name.
        /// </summary>
        /// <param name="name"></param>
        public Player(string name, Bot _internalBot) {
            Index = -1;
            SkinVariant = 0;
            HairType = 0;
            Name = name;
            HairDye = 0;
            HideVisuals = 0;
            HideVisuals2 = 0;
            HideMisc = 0;
            HairColor = new Color(255, 255, 255);
            SkinColor = new Color(255, 255, 255);
            EyeColor = new Color(255, 255, 255);
            ShirtColor = new Color(255, 255, 255);
            UndershirtColor = new Color(255, 255, 255);
            PantsColor = new Color(255, 255, 255);
            ShoeColor = new Color(255, 255, 255);
            Difficulty = 0;
            TorchFlags = 2;

            MaxHP = 420;
            CurHP = 420;

            MaxMana = 69;
            CurMana = 69;

            InventorySlots = new NetItem[NetItem.InventorySlots];
            ArmorSlots = new NetItem[NetItem.ArmorSlots];
            DyeSlots = new NetItem[NetItem.DyeSlots];
            MiscEquipSlots = new NetItem[NetItem.MiscEquipSlots];
            MiscDyeSlots = new NetItem[NetItem.MiscDyeSlots];

            internalBot = _internalBot;
        }
    }
}
