/* Enemy.cs
 * Class for the basic structure of an enemy object, all enemies inherit from this class
 * This class has helpful functions all enemies need, including the overridable update and draw
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
    class Enemy:Tangible
    {
        protected Pickup item;
        protected bool showHP;
        protected Tangible target;
        protected Vector2 attackDirection;
        protected float attackAngle;

        public Enemy(Vector2 pos, Tangible t, Texture2D tex, float rot, int h)
            : base(pos, tex, rot, h)
        {
            showHP = true;
            target = t;
            attackDirection = Vector2.Zero;
        }
        public Pickup Item
        {
            get { return item; }
        }
        public override void OnDeath()
        {
            if (item!=null)
            {
                item.Position = this.position;
                Globals.CurrentRoom.Pickups.Add(item);
            }
            base.OnDeath();
        }
        protected Vector2 homeDirection(float speed)
        {
            if (target.Position.X >= position.X)
                attackAngle = (float)Math.Atan((target.Position.Y - position.Y) / (target.Position.X - position.X));
            else
                attackAngle = (float)Math.Atan((target.Position.Y - position.Y) / (target.Position.X - position.X)) + MathHelper.Pi;
            return new Vector2((float)Math.Cos(attackAngle) * speed, (float)Math.Sin(attackAngle) * speed);
        }
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            base.Draw(sb);
            if (alive && showHP && Globals.ShowHPBars)
            {
                int thickness = 1;
                Color c = Color.White;
                Vector2 dimension = new Vector2(texture.Width * 0.75f, 6);
                int x = (int)position.X - (int)dimension.X / 2;
                int y = (int)position.Y - (texture.Height / 2) - (int)dimension.Y;
                int w = (int)(dimension.X * 1);
                int h = (int)dimension.Y;
                sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y, w, thickness), c);
                sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y, thickness, h), c);
                sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x + w - thickness, y, thickness, h), c);
                sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y + h - thickness, w, thickness), c);
                Rectangle hp = new Rectangle(x + thickness, y + thickness, (int)(dimension.X * ((float)health / (float)maxHealth)) - 2 * thickness, (int)dimension.Y - 2 * thickness);
                sb.Draw(Globals.DUMMYTEXTURE, hp, Color.Red);
            }
        }
    }
}
