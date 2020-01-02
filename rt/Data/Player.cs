using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace rt {
    /// <summary>
    /// Based off of playerinfo packet. Used for bot.
    /// </summary>
    public class Player {

        public byte PlayerID;
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

        public bool Initialized;
        public bool LoggedIn;

        public Player(string name) {
            PlayerID = 0;
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

            Initialized = false;
            LoggedIn = false;
        }
    }
}
