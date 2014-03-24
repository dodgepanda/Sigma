/*  Globals.cs
 *  Global class for keeping track of globals
 *  
 *  Project Sigma
 *  Michael Ou
 *  Wei Wei Huang
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sigma
{
    internal class Globals
    {
        public const int NUM_PARTICLES = 12;
        public const int MAX_SHOTS = 6;
        public const int INVULNERABLE_TIME = 2;
        public static Microsoft.Xna.Framework.Content.ContentManager CONTENTMANAGER;
        public static Microsoft.Xna.Framework.Graphics.Texture2D DUMMYTEXTURE;
        public static Random Rand = new Random();
        public static Room CurrentRoom;
        public static Dungeon Dungeon;
        public static bool ShowHPBars = false;
        public static bool Debug = false;
    }
}
