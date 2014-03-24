/*  Room.cs
 *  Room class to keep track of adjacent rooms along with all the tangibles for that room
 *  Each room also has a type which is kept tracked to be used on the minimap
 *  
 *  Project Sigma
 *  Michael Ou
 *  Wei Wei Huang
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sigma
{
    public enum RoomType
    {
        REGULAR,
        BOSS,
        SHOP,
        LOCKED,
        HIDDEN
    }
    class Room
    {
        private Room north, east, south, west;
        private RoomType type;
        private bool open, explored;
        private Color color;
        private Rectangle playableArea;
        private List<Tangible> tangibles = new List<Tangible>();
        private List<Pickup> pickups = new List<Pickup>();
        private List<TreasureChest> chests = new List<TreasureChest>();
        private Tangible boss = null;
        public Room(RoomType roomType, Rectangle playable)
        {
            playableArea = playable;
            north = null;
            east = null;
            south = null;
            west = null;
            open = false;
            type = roomType;
            color = Color.White;
            explored = false;
        }
        public void AddItemToShop(Pickup item)
        {
        }
        public void Explore()
        {
            explored = true;
        }
        public bool IsExplored()
        {
            return explored;
        }
        public Tangible Boss
        {
            get { return boss; }
            set 
            {
                tangibles.Add(value);
                boss = value; 
            }
        }
        public List<TreasureChest> Chests
        {
            get { return chests; }
            set { chests = value; }
        }
        public List<Tangible> Tangibles
        {
            get { return tangibles; }
            set { tangibles = value; }
        }
        public List<Pickup> Pickups
        {
            get { return pickups; }
            set { pickups = value; }
        }
        public bool isAllDead()
        {
            bool cleared = true;
            foreach(Tangible t in tangibles)
            {
                if (t.isAlive)
                    cleared = false;
            }
            return cleared;
        }
        //public void Draw(SpriteBatch sb, Rectangle playableArea, Texture2D verticalWall, Texture2D horizontalWall)
        //{
        //    sb.Draw(verticalWall, new Vector2(playableArea.X, playableArea.Y), color);
        //    sb.Draw(horizontalWall, new Vector2(playableArea.X, playableArea.Y), color);
        //    sb.Draw(verticalWall, new Vector2(playableArea.Width - verticalWall.Width, playableArea.Y), color);
        //    sb.Draw(horizontalWall, new Vector2(playableArea.X, playableArea.Height - horizontalWall.Height), color);
        //}
        public RoomType Type
        {
            get {return type;}
            set { type = value; }
        }
        public bool Open
        {
            get { return open; }
            set { open = value; }
        }

        public Room North
        {
            get { return north; }
            set 
            { 
                north = value;
                value.south = this;
            }
        }
        public Room South
        {
            get { return south; }
            set 
            {
                south = value;
                value.north = this;
            }
        }
        public Room West
        {
            get { return west; }
            set 
            {
                west = value;
                value.east = this;
            }
        }
        public Room East
        {
            get { return east; }
            set 
            {
                east = value;
                value.west = this;
            }
        }

    }
}
