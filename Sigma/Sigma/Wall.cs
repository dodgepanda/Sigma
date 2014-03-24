/*  Wall.cs
 *  Class for the walls of a room
 *  Softspots are defined for where a door might be
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
    class Wall : Tangible
    {
        private RoomType type;
        private Rectangle softSpot;
        private bool locked, closed;
        public Wall(Vector2 Position, Texture2D Texture, RoomType Type = RoomType.REGULAR, float Rotation = 0)
            : base(Position, Texture, Rotation)
        {
            closed = false;
            locked = false;
            if (Type == RoomType.LOCKED)
                locked = true;
            type = Type;
            Vector2 softSpotSize;
            if (texture.Width > texture.Height)
                softSpotSize = new Vector2(60, 30);
            else
                softSpotSize = new Vector2(30, 60);
            if(softSpotSize.X > softSpotSize.Y)
                softSpot = new Rectangle((int)(position.X - softSpotSize.X / 2), (int)(position.Y - softSpotSize.Y / 2), (int)softSpotSize.X, (int)softSpotSize.Y);
            else
                softSpot = new Rectangle((int)(position.X - softSpotSize.X / 2), (int)(position.Y - softSpotSize.Y / 2), (int)softSpotSize.X, (int)softSpotSize.Y);
        }
        public RoomType Type
        {
            get { return type; }
            set { type = value; }
        }
        public void ExplodeWall()
        {
            if (texture.Width > texture.Height)
                Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\horizontalWall2");
            else
                Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\verticalWall2");
        }
        public bool LockedOrClosed() { return locked || closed; }
        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }
        public bool Closed
        {
            get { return closed; }
            set { closed = value; }
        }
        public Rectangle SoftSpot
        {
            get { return softSpot; }
        }
        public override void Draw(SpriteBatch sb)
        {
            if (locked)
                sb.Draw(Globals.DUMMYTEXTURE, softSpot, Color.Gold);
            if (closed)
                sb.Draw(Globals.DUMMYTEXTURE, softSpot, Color.Gray);
            base.Draw(sb);
        }
    }
}
