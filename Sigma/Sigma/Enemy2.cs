/*  Enemy2.cs
 *  Inherits from enemy class
 *  This enemy lerps around and shoots at the player, has a chance to drop an item.
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
    class Enemy2 : Enemy
    {
        const float ENEMY2_MOVESPEED = 1.5f;
        const float PROJECTILE_SPEED = 1.5f;
        const int ATTACK_COOLDOWN = 3;
        float dir = 0, attackTime = 0, turnTime = 0;
        Texture2D particleTexture;
        bool canAttack = true;
        Vector2 moveDirection = Vector2.Zero;

        public Enemy2(Vector2 Position, Tangible t, float Rotation = 0, int h = 1)
            : base(Position, t, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\enemy2"), Rotation, h)
        {
            particleTexture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\particle3");
            meleeDamage = 1;
            if (Globals.Rand.Next(0, 100) < 25)
            {
                PickupType p;
                switch (Globals.Rand.Next(0, 4))
                {
                    case 0: p = PickupType.Health;
                        break;
                    case 1: p = PickupType.Money;
                        break;
                    case 2: p = PickupType.Key;
                        break;
                    case 3: p = PickupType.Bomb;
                        break;
                    default: p = PickupType.Health;
                        break;
                }
                item = new Pickup(position, p);
            }
        }
        public override void OnDeath()
        {
            Globals.CONTENTMANAGER.Load<SoundEffect>(@"Sounds/pop").Play();
            base.OnDeath();
        }
        public override void Update(GameTime gameTime)
        {
            if (alive)
            {
                updateMovement(gameTime);
                if (canAttack == true)
                    attack();
                else
                {
                    attackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (attackTime >= ATTACK_COOLDOWN)
                    {
                        attackTime -= ATTACK_COOLDOWN;
                        canAttack = true;
                    }
                }
            }
            foreach (Particle p in particles)
                p.Update(gameTime);

            base.Update(gameTime);
        }
        private bool inBounds(Vector2 v)
        {
            if (v.X < 30 + origin.X || v.X > 570 - origin.X || v.Y < 30 + origin.Y || v.Y > 470 - origin.Y)
                return false;
            return true;
        }
        private void attack()
        {
            Vector2 temp = homeDirection(PROJECTILE_SPEED);
            particles.Add(new Particle(new Vector2(position.X, position.Y), particleTexture, 0, Color.White, temp, 1));
            canAttack = false;
        }
        private void updateMovement(GameTime gameTime)
        {
            if (inBounds(position + moveDirection) && moveDirection != Vector2.Zero && turnTime < 2.5)
            {
                position += moveDirection;
                turnTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                turnTime = 0;
                Random r = new Random();
                dir = (float)r.NextDouble() * MathHelper.TwoPi;
                Vector2 targetDir = new Vector2(position.X + (float)Math.Cos(dir) * ENEMY2_MOVESPEED,
                    position.Y + (float)Math.Sin(dir) * ENEMY2_MOVESPEED);
                while (!inBounds(targetDir))
                {
                    dir = (float)r.NextDouble() * MathHelper.TwoPi;
                    targetDir = new Vector2(position.X + (float)Math.Cos(dir) * ENEMY2_MOVESPEED,
                        position.Y + (float)Math.Sin(dir) * ENEMY2_MOVESPEED);
                }
                moveDirection = new Vector2((float)Math.Cos(dir) * ENEMY2_MOVESPEED,
                    (float)Math.Sin(dir) * ENEMY2_MOVESPEED);
                position += moveDirection;
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null);
            foreach (Particle p in particles)
            {
                p.Draw(sb);
            }
            sb.End();
            sb.Begin();
            base.Draw(sb);
        }
    }
}

