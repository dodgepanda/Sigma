/*  Boss1.cs
 *  First mini boss
 *  Class with boss draw function, update function, and the boss's attack pattern
 *  Attack Pattern - Spin projectile attack, and teleport. 
 *  Drops an attack upgrade
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
    class Boss1 : Enemy
    {
        const int TELEPORT_TIME = 5;
        Random r = new Random();
        float movetime = 0, rotTime = 0, teleTime = 0;
        bool canAttack = false;        
        Texture2D particleTexture;

        public Boss1(Vector2 Position, float Rotation = 0, int h = 0)
            : base(Position, null, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\boss1"), Rotation, h)
        {
            particleTexture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\particle");
            meleeDamage = 2;
            showHP = false;
            PickupType p = PickupType.UpgradeShot;
            item = new Pickup(position, p);
        }

        public override void Update(GameTime gameTime)
        {
            if (alive)
            {
                if (ethereal == false)
                {
                    movetime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (movetime >= TELEPORT_TIME)
                    {
                        teleport();
                        movetime -= TELEPORT_TIME;
                    }
                    else
                    {
                        rotTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (rotTime >= TELEPORT_TIME / 3.0f)
                        {
                            canAttack = true;
                            rotTime -= TELEPORT_TIME / 3.0f;
                        }
                        if (canAttack == true)
                        {
                            spinAttack();                            
                        }
                    }
                }
                else
                {
                    teleTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (teleTime >= Globals.INVULNERABLE_TIME)
                    {
                        ethereal = false;
                        pColor = Color.White;
                        teleTime -= Globals.INVULNERABLE_TIME;
                    }
                }
            }
            foreach (Particle p in particles)
            {
                p.Update(gameTime);
            }
            base.Update(gameTime);
        }
        private void teleport()
        {
            Position = new Vector2(r.Next((int)origin.X + 30, 570 - (int)origin.X), r.Next((int)origin.Y + 30, 470 - (int)origin.Y));
            pColor = new Color(255, 255, 255)*(80f/255f);
            ethereal = true;
            rotation = 0;
            canAttack = false;
            rotTime = 0;
        }
        private void spinAttack()
        {
            rotation = rotTime / 0.5f * MathHelper.TwoPi;
            if (rotation >= MathHelper.TwoPi)
            {
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 8)
                {
                    particles.Add(new Particle(new Vector2(position.X + (float)Math.Cos(i) * origin.X, position.Y + (float)Math.Sin(i) * origin.Y),
                        particleTexture, rotation, Color.NavajoWhite, new Vector2((float)Math.Cos(i), (float)Math.Sin(i)),1,ParticleType.Spiral,1,1.2f));
                }
                canAttack = false;
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null);
            foreach(Particle p in particles){
                p.Draw(sb);
            }
            sb.End();
            sb.Begin();
            base.Draw(sb);
        }
    }
}
