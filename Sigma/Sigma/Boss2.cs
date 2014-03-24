/*  Boss2.cs
 *  Second mini boss
 *  Class with boss draw function, update function, and the boss's attack pattern
 *  Attack Pattern - Rapid fire at player location and charge attack
 *  Drop an attack upgrade
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
    class Boss2 : Enemy
    {
        int NUM_PROJECTILES = 150;
        const float PROJECTILE_SPEED = 2.2f;
        float ATTACK_COOLDOWN = 3.0f;
        const float REGAIN_FORM_TIME = 1.0f;
        Random r = new Random();
        int numProjectilesShot;
        float cooldownTimer, etherealTimer, rapidfireTimer, acceleration = 1, rapidfireCooldown = 0.002f;
        bool canAttack, isSolid = true, switchedAttacks = true;
        Vector2 diveDirection;
        Texture2D particleTexture;

        public Boss2(Vector2 Position, Tangible t, float Rotation = 0, int h = 0)
            : base(Position, t, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\boss2"), Rotation, h)
        {
            particleTexture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\particle4");
            meleeDamage = 2;
            showHP = false;
            PickupType p = PickupType.UpgradeShot;
            item = new Pickup(position, p);
            cooldownTimer = 2;
            etherealTimer = 0;
            rapidfireTimer = 0;
            canAttack = false;
            attackDirection = Vector2.Zero;
            numProjectilesShot = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (isAlive)
            {
                if (health >= 55)
                    attackPattern1(gameTime);
                else
                {
                    if (switchedAttacks == true)
                    {
                        canAttack = false;
                        switchedAttacks = false;
                        ATTACK_COOLDOWN = 2.0f;
                    }
                    attackPattern2(gameTime);
                } 
            }
            foreach (Particle p in particles)
            {
                p.Update(gameTime);
            }
            base.Update(gameTime);
        }
        private void attackPattern1(GameTime gameTime)
        {
            if (canAttack)
            {
                if (numProjectilesShot == NUM_PROJECTILES)
                {
                    cooldownTimer -= ATTACK_COOLDOWN;
                }
                Attack(gameTime);

            }
            else
            {
                cooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (cooldownTimer >= ATTACK_COOLDOWN)
                    canAttack = true;
            }
        }
        private void attackPattern2(GameTime gameTime)
        {
            if (canAttack && isSolid)
            {
                Attack2();
            }
            else
            {
                if (isSolid == false)
                {
                    if (etherealTimer >= REGAIN_FORM_TIME)
                    {
                        if (numProjectilesShot == NUM_PROJECTILES)
                        {
                            isSolid = true;
                            etherealTimer -= REGAIN_FORM_TIME;
                        }
                        color = Color.IndianRed;
                        hostileAttack(gameTime);
                    }
                    else
                    {
                        etherealTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        color = Color.IndianRed * ((80f + (175f * etherealTimer)) / 255f);
                    }
                }
                else
                {
                    cooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (cooldownTimer >= ATTACK_COOLDOWN)
                    {
                        canAttack = true;
                        cooldownTimer -= ATTACK_COOLDOWN;
                        diveDirection = homeDirection(1.5f) * 4;
                        color = Color.IndianRed * (80f / 255f);
                        isEthereal = true;
                    }
                }
            }
        }
        private void Attack(GameTime gameTime, float rand = 1)
        {
            attackDirection = homeDirection(PROJECTILE_SPEED);
            Vector2 randDir;

            if (numProjectilesShot < NUM_PROJECTILES)
            {
                rotation = attackAngle - (float)Math.PI / 2;
                if (rapidfireTimer >= rapidfireCooldown)
                {                    
                    randDir = getRandomDirection(rand);
                    Vector2 tempAttackDirection = new Vector2(attackDirection.X + randDir.X, attackDirection.Y + randDir.Y);
                    rotation = attackAngle - (float)Math.PI/2;
                    particles.Add(new Particle(new Vector2(position.X, position.Y),
                        particleTexture, rotation, Color.RosyBrown, tempAttackDirection, 1, ParticleType.Regular, acceleration));
                    rapidfireTimer -= 0.2f;
                    numProjectilesShot++;
                    
                }
                else
                    rapidfireTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                numProjectilesShot = 0;
                canAttack = false;
            }
        }
        private void Attack2()
        {
            rotation = attackAngle - (float)Math.PI/2;
            Vector2 targetPos = position + new Vector2(diveDirection.X, diveDirection.Y);
            if (inBounds(targetPos) == false)
            {
                canAttack = false;
                isSolid = false;
            }
            else
                position = targetPos;
        }
        private void hostileAttack(GameTime gameTime)
        {
            NUM_PROJECTILES = 30;
            Attack(gameTime, 1.3f);
            acceleration = 1.0075f;
            isEthereal = false;
            rapidfireCooldown = 0.01f;
        }
        private bool inBounds(Vector2 v)
        {
            if (v.X <= 25 + origin.X || v.X >= 575 - origin.X || v.Y <= 25 + origin.Y || v.Y >= 475 - origin.Y)
                return false;
            return true;
        }
        private Vector2 getRandomDirection(float Max)
        {
            Vector2 Dir = Vector2.Zero;
            float sign = 0;

            sign = Globals.Rand.Next(0, 1);
            if (sign == 0)
                Dir.X = -1 * (float)Globals.Rand.NextDouble() * Max;
            else
                Dir.X = (float)Globals.Rand.NextDouble() * Max;
            sign = Globals.Rand.Next(0, 1);
            if (sign == 0)
                Dir.Y = -1 * (float)Globals.Rand.NextDouble() * Max;
            else
                Dir.Y = (float)Globals.Rand.NextDouble() * Max;
            return Dir;
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null);
            foreach (Particle p in particles)
            {
                if (NUM_PROJECTILES == 25)
                {
                    p.pColor = Color.IndianRed;
                    p.Scale = 1.5f;
                }
                p.Draw(sb);
            }
            sb.End();
            sb.Begin();
            base.Draw(sb);
        }
    }
}
