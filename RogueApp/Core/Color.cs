﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;

namespace RogueApp.Core
{
    public class Colors
    {
        
        //assign color values
        public static RLColor FloorBackground = RLColor.Black;
        public static RLColor Floor = Swatch.AlternateDarkest;
        public static RLColor FloorBackgroundFov = Swatch.DbDark;
        public static RLColor FloorFov = Swatch.Alternate;

        public static RLColor WallBackground = Swatch.SecondaryDarkest;
        public static RLColor Wall = Swatch.Secondary;
        public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;
        public static RLColor WallFov = Swatch.SecondaryLighter;

        public static RLColor TextHeading = Swatch.DbLight;
        public static RLColor Text = Swatch.DbLight;
        public static RLColor Gold = Swatch.DbSun;
        public static RLColor KoboldColor = Swatch.DbBrightWood;
        public static RLColor Player = Swatch.DbLight;
    }
}
