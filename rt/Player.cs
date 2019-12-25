using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt {
    public class Player {

        /// <summary>
        /// Based off of playerinfo packet.
        /// </summary>
        int PlayerID;
        byte SkinVariant;
        byte HairType;
        string Name;
        byte HairDye;
        byte HVisuals1;
        byte HVisuals2;
        byte HMisc;
        Color HairColor;
        Color SkinColor;
        Color EyeColor;
        Color ShirtColor;
        Color UnderShirtColor;
        Color PantsColor;
        Color ShoeColor;
        byte Difficulty;

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
        }
    }
}
