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
using System.Collections;

namespace TerraNPCBot {
    /// <summary>
    /// Based off of PlayerInfo(4), InventorySlot(5), PlayerHP(16), and PlayerMana(42) packets.
    /// </summary>
    public class Player {
        public int Index { get; set; }
        public byte SkinVariant { get; set; }
        public byte HairType { get; set; }
        public string Name { get; set; } = "Michael Jackson";
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

        public SlotCollection InventorySlots { get; set; }
        public SlotCollection ArmorSlots { get; set; }
        public SlotCollection DyeSlots { get; set; }
        public SlotCollection MiscEquipSlots { get; set; }
        public SlotCollection MiscDyeSlots { get; set; }

        /// <summary>
        /// Initializes the default bot player with a given name.
        /// </summary>
        /// <param name="name"></param>
        public Player(string name, Bot internalBot) {
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

            InventorySlots = new SlotCollection(NetItem.InventorySlots, internalBot);
            ArmorSlots = new SlotCollection(NetItem.ArmorSlots, internalBot);
            DyeSlots = new SlotCollection(NetItem.DyeSlots, internalBot);
            MiscEquipSlots = new SlotCollection(NetItem.MiscEquipSlots, internalBot);
            MiscDyeSlots = new SlotCollection(NetItem.MiscDyeSlots, internalBot);
        }
    }

    public class SlotCollection : IEnumerable<NetItem> {
        private NetItem[] contents;
        private Bot internalBot;

        public SlotCollection(int count, Bot _internalBot) {
            contents = new NetItem[count];
            internalBot = _internalBot;
        }
        
        public NetItem this[int i] {
            get => contents[i];
            set {
                if (internalBot.EventHooks.InventoryChange.Invoke(internalBot, new TerraBotLib.Events.InventoryChangedEventArgs() { Slot = i, OldItem = contents[i], NewItem = value }))
                    return;
                contents[i] = value;
            }
        }

        public IEnumerator<NetItem> GetEnumerator() {
            return new SlotCollectionEnum(contents);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Enumerator to allow foreach to be used with the SlotCollection class
        /// </summary>
        public class SlotCollectionEnum : IEnumerator<NetItem> {
            private int position = -1;
            public NetItem[] contents;
            public SlotCollectionEnum(NetItem[] list) {
                contents = list;
            }

            // just default IEnumerator stuff
            public NetItem Current {
                get {
                    try {
                        return contents[position];
                    }
                    catch (IndexOutOfRangeException) {  // if position is out of bounds
                        throw new InvalidOperationException();  
                    }
                }
            }

            object IEnumerator.Current => Current;

            public bool MoveNext() {
                position++;
                return position < contents.Length;
            }

            public void Reset() {
                position = -1;
            }

            public void Dispose() { }  // unnecessary
        }
    }
}
