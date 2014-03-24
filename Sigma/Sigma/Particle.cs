/*  Particle.cs
 *  Class used to define particles used for particles systems.
 *  This class includes different types of particles which behave differently
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
    public enum ParticleType
    {
        Regular,
        Scatter,
        Super,
        Sine,
        Spiral
    }
    public class Particle : Tangible
    {
        public int power;
        private int width;
        private int height;
        private Vector2 direction;
        private float acceleration;
        public bool returned = false;
        public ParticleType type;
        private float angle;

        public Particle(Vector2 pos, Texture2D tex = null, float rot = 0, Color c = new Color(), Vector2 dir = new Vector2(), int p = 0, ParticleType t = ParticleType.Regular, float accel = 1, float s = 1)
            :base(pos, tex, rot)
        {
            color = c;
            width = tex.Width;
            height = tex.Height;
            direction = dir;
            power = p;
            type = t;
            acceleration = accel;
            scale = s;
            angle = (float)Math.Atan2(direction.Y, direction.X);
        }

        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (type == ParticleType.Super)
            {
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null);
                sb.Draw(texture, new Rectangle((int)position.X, (int)position.Y, (int)(width * scale), (int)(height * scale)),
                    new Rectangle(0, 0, texture.Width, texture.Height), color, rotation, origin, SpriteEffects.None, 0.0f);
                sb.End();
                sb.Begin();
            }
            else
                sb.Draw(texture, new Rectangle((int)position.X, (int)position.Y, (int)(width * scale), (int)(height * scale)),
                    new Rectangle(0, 0, texture.Width, texture.Height), color, rotation, origin, SpriteEffects.None, 0.0f);
            if (Globals.Debug)
            {
                int thickness = 1;
                Color c = Color.White;
                Vector2 dimension = new Vector2(Rectangle().Width, Rectangle().Height);

                int x = (int)position.X - (int)dimension.X / 2;
                int y = (int)position.Y - ((int)dimension.Y / 2);
                int w = (int)(dimension.X * 1);
                int h = (int)dimension.Y;
                sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y, w, thickness), c);
                sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y, thickness, h), c);
                sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x + w - thickness, y, thickness, h), c);
                sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y + h - thickness, w, thickness), c);
            }
        }
        public override void Update(GameTime gameTime)
        {
            if(acceleration != 1)
                direction = direction * acceleration;
            float pSpeed, pAmplitude, pPeriod;
            pSpeed = 2f;
            pAmplitude = 0f;
            pPeriod = 12;
            switch(type){
                case (ParticleType.Spiral): direction = new Vector2((float)(Math.Cos(elapseTime + angle)),
                     (float)(Math.Sin(elapseTime + angle)));
                    direction *= elapseTime;
                    break;
                case (ParticleType.Scatter) :
                    break;
                case (ParticleType.Super) :
                    break;
                default:    direction = new Vector2((float)(Math.Cos(angle) * pSpeed + pAmplitude * Math.Cos(angle - MathHelper.PiOver2) * Math.Cos(elapseTime * pPeriod)),
                            (float)(Math.Sin(angle) * pSpeed + pAmplitude * Math.Sin(angle - MathHelper.PiOver2) * Math.Cos(elapseTime * pPeriod)));
                    break;
            }
            
            Position += direction;
            base.Update(gameTime);
        }
    }
}
