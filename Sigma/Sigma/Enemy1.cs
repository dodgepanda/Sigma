/*  Enemy1.cs
 *  Inherits from enemy class
 *  This enemy lerps around for a short time, then goes hostile and homes onto the player. 
 *  Has a chance to drop coin
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
    class Enemy1 : Enemy
    {
        const float ENEMY1_MOVESPEED = 1.5f;
        const float ATTACKSPEED = 3.1f;
        const float ATTACK_DURATION = 2.5f;
        const float ATTACK_COOLDOWN = 3;
        float dir = 0, turnTime = 0, attackTime, cooldownTimer = 0;
        bool canAttack = false;

        public Enemy1(Vector2 Position, Tangible t, float Rotation = 0, int h = 1)
            : base(Position, t, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\enemy1"), Rotation, h)
        {
            meleeDamage = 1;
            cooldownTimer = Globals.Rand.Next(0, 3);
            if (Globals.Rand.Next(0, 100) < 75)
            {
                item = new Pickup(position, PickupType.Money);
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (canAttack == true)
            {
                color = Color.Red;
                attackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                attackDirection = homeDirection(ATTACKSPEED);
                Attack();
            }
            else
            {
                color = Color.White;
                cooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (cooldownTimer >= ATTACK_COOLDOWN)
                {
                    cooldownTimer -= ATTACK_COOLDOWN;
                    canAttack = true;
                }
                updateMovement(gameTime);                
            }
            if (attackDirection.X < 0)
                effect = SpriteEffects.FlipHorizontally;
            else
                effect = SpriteEffects.None;
            base.Update(gameTime);
        }
        private bool inBounds(Vector2 v)
        {
            if (v.X <= 25 + origin.X || v.X >= 575 - origin.X || v.Y <= 25 + origin.Y || v.Y >= 475 - origin.Y)
                return false;
            return true;
        }
        private void updateMovement(GameTime gameTime)
        {
            if (inBounds(position + attackDirection) && attackDirection != Vector2.Zero && turnTime < 3)
            {
                position += attackDirection;
                turnTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                randomizeMoveDirection();                
                position += 2*attackDirection;
            }
        }
        public override void OnDeath()
        {
            Globals.CONTENTMANAGER.Load<SoundEffect>(@"Sounds/pop").Play();
            base.OnDeath();
        }
        private void randomizeMoveDirection()
        {
            turnTime = 0;
            Random r = new Random();
            dir = (float)r.NextDouble() * MathHelper.TwoPi;
            Vector2 targetDir = new Vector2(position.X + (float)Math.Cos(dir) * ENEMY1_MOVESPEED,
                position.Y + (float)Math.Sin(dir) * ENEMY1_MOVESPEED);
            while (!inBounds(targetDir))
            {
                dir = (float)r.NextDouble() * MathHelper.TwoPi;
                targetDir = new Vector2(position.X + (float)Math.Cos(dir) * ENEMY1_MOVESPEED,
                    position.Y + (float)Math.Sin(dir) * ENEMY1_MOVESPEED);
            }
            attackDirection = new Vector2((float)Math.Cos(dir) * ENEMY1_MOVESPEED,
                (float)Math.Sin(dir) * ENEMY1_MOVESPEED);
        }
        private void Attack()
        {
            position += attackDirection;
            if (attackTime >= ATTACK_DURATION)
            {
                attackTime = 0;
                canAttack = false;
            }
            else
            {
                if (this.CollidesWith(target))
                {
                    attackTime = 0;
                    canAttack = false;
                }
            }
        }
        
    }
}
