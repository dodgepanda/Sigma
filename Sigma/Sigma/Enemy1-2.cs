/*  Enemy1-2.cs
 *  An alternate version of enemy 1
 *  Has a HIGH chance to drop coin
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
    class Enemy1_2 : Enemy
    {
        const float ENEMY1_MOVESPEED = 1.5f;
        const float ATTACKSPEED = 2.8f;
        const float ATTACK_DURATION = 2.5f;
        const float ATTACK_COOLDOWN = 3;
        float dir = 0, attackTime, cooldownTimer = 0;
        bool canAttack = false;

        public Enemy1_2(Vector2 Position, Tangible t, Vector2 d, float Rotation = 0, int h = 1)
            : base(Position, t, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\enemy1-2"), Rotation, h)
        {
            meleeDamage = 1;
            cooldownTimer = (float)Globals.Rand.NextDouble() * 3.0f;
            attackDirection = Vector2.Zero;
            if (Globals.Rand.Next(0, 100) < 50)
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
                updateAttackDirection();
                rotation = attackAngle;
                if (attackDirection.X < 0)
                    effect = SpriteEffects.FlipHorizontally;
                else
                    effect = SpriteEffects.None;
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
                rotation = attackAngle;
                if (attackDirection.X < 0)
                    effect = SpriteEffects.FlipHorizontally;
                else
                    effect = SpriteEffects.None;
            }            
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
            if (inBounds(position + attackDirection) && attackDirection != Vector2.Zero)
            {
                position += attackDirection;
            }
            else
            {
                circleMoveDirection();
                position += attackDirection;
            }
        }
        private void circleMoveDirection()
        {
            dir = (float)Globals.Rand.NextDouble() * MathHelper.TwoPi;
            Vector2 targetDir = new Vector2(position.X + (float)Math.Cos(dir) * ENEMY1_MOVESPEED,
                position.Y + (float)Math.Sin(dir) * ENEMY1_MOVESPEED);
            while (!inBounds(targetDir))
            {
                dir = (float)Globals.Rand.NextDouble() * MathHelper.TwoPi;
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
        private void updateAttackDirection()
        {
            float dirAngle = 0;
            if (target.Position.X > position.X)
                dirAngle = (float)Math.Atan((target.Position.Y - position.Y) / (target.Position.X - position.X));
            else
                dirAngle = (float)Math.Atan((target.Position.Y - position.Y) / (target.Position.X - position.X)) + MathHelper.Pi;
            attackDirection = new Vector2((float)Math.Cos(dirAngle) * ATTACKSPEED, (float)Math.Sin(dirAngle) * ATTACKSPEED);
        }
        public override void OnDeath()
        {

            Globals.CONTENTMANAGER.Load<SoundEffect>(@"Sounds/pop").Play();
            base.OnDeath();
        }
    }
}
