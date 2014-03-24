/*  Explosion.cs
 *  Class for an explosion event.
 *  When a bomb explodes, an explosion is created and handled respectively
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
using Microsoft.Xna.Framework.Audio;

namespace Sigma
{
    class Explosion: Tangible
    {
        const int numFrames = 4;
        public static int DAMAGE = 5;
        float explodingTime = .45f;
        int animationFrame;
        bool finished;
        public bool canHurtBoss;
        const int explosionFrameWidth = 40;
        public Rectangle AOE;

        public Explosion(Vector2 pos, bool fin)
            : base(pos, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\explosion"))
        {
            finished = fin;
            origin = new Vector2(20, 20);
            debugThis = false;
            canHurtBoss = true;
            Globals.CONTENTMANAGER.Load<SoundEffect>(@"Sounds/explosion").Play();
        }
        public bool Finished
        {
            get { return finished; }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (elapseTime >= explodingTime)
            {
                finished = true;
            }
            else
            {
                animationFrame = (int)(elapseTime / (explodingTime / numFrames));
                scale = 1.3f*animationFrame;
                sourceRect = new Rectangle(explosionFrameWidth * animationFrame, 0, 40, 40);
            }
            AOE = Rectangle();
        }
        public override void Draw(SpriteBatch sb)
        {
            if (alive == true)
            {                
                if (Globals.Debug)
                {
                    int thickness = 1;
                    Color c = Color.White;
                    Vector2 dimension = new Vector2(AOE.Width,AOE.Height);

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
            base.Draw(sb);
        }
        public override Rectangle Rectangle()
        {
            return CalculateBoundingRectangle(
                        new Rectangle(0, 0, texture.Width/4, texture.Height),
                        Transform());
        }
    }
}
