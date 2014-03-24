/*  Boss3.cs
 *  Third boss
 *  Class with boss draw function, update function, and the boss's attack pattern
 *  Attack Pattern1 - Wave attack and rapid fire at player location
 *  Attack Pattern2 - Wave attack, Charge, and rapid fire at player location 
 *  Attack Pattern3 - Scatter shot, then gather into an energy bomb
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
    class Boss3 : Enemy
    {
        int NUM_PROJECTILES = 150, NUM_PARTICLES = 25;
        const float PROJECTILE_SPEED = 2.2f, PROJECTILE_SPEED2 = 3.0f, TELEPORT_TIME = 4.0f;
        float ATTACK_COOLDOWN = 4.0f, ATTACK1_COOLDOWN = 2.0f, ATTACK2_COOLDOWN = 3.0f;
        const float REGAIN_FORM_TIME = 1.0f, ETHEREAL_TIME = 1.5f, CHARGE_TIME = 1.8f, ATTACKSWITCHCD = 2f;
        int numProjectilesShot, numProjectilesShot2;
        float etherealTimer, ChargeCooldown, rotTimer, teleTimer = 2, bombTimer, acceleration = 1, rapidfireCooldown = 0.002f;
        bool isSolid = true, gathered = false, canTeleport = true;
        bool[] canAttack = new bool[4] { false, false, false, false };
        float[] cooldownTimer = new float[4] {1,0,0,2};
        Vector2 diveDirection, initialPosition;
        Texture2D particleTexture;
        Random r = new Random();

        public Boss3(Vector2 Position, Tangible t, float Rotation = 0, int h = 0)
            : base(Position, t, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\boss3"), Rotation, h)
        {
            particleTexture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\superparticle2");
            meleeDamage = 2;
            showHP = false;
            etherealTimer = 0;
            rotTimer = 0;
            ChargeCooldown = 0;
            attackDirection = Vector2.Zero;
            numProjectilesShot = 0;
            numProjectilesShot2 = 0;
            bombTimer = 0;
            initialPosition = Position;
        }

        public override void Update(GameTime gameTime)
        {
            if (isAlive)
            {
                if (health >= 170)
                {
                    attackPattern1(gameTime);
                }
                else if (health >= 90)
                {
                    attackPattern2(gameTime);
                }
                else
                {
                    if (canTeleport)
                    {
                        //Teleport
                        teleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (teleTimer >= TELEPORT_TIME)
                        {
                            teleport();
                            teleTimer -= TELEPORT_TIME;
                            canTeleport = false;
                        }
                    }
                    else
                    {
                        attackPattern3(gameTime);
                    }
                }
                for (int p = 0; p < particles.Count; ++p)
                {
                    particles[p].Update(gameTime);
                    if (particles[p].returned && particles[p].type == ParticleType.Scatter)
                    {
                        if (particles[p].Position.X <= position.X + 50
                            && particles[p].Position.X >= position.X - 50
                            && particles[p].Position.Y <= position.Y + 50
                            && particles[p].Position.Y >= position.Y - 50)
                        {
                            particles.Remove(particles[p]);
                            p--;
                        }
                    }
                }   
            }
            else
            {
                for (int p = 0; p < particles.Count; ++p)
                {
                    particles.Remove(particles[p]);
                    p--;
                }   
            }
                         
            base.Update(gameTime);
        }
        private void attackPattern(GameTime gameTime)
        {
            if (canAttack[0])
                spinAttack();
            else
            {
                cooldownTimer[0] += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (cooldownTimer[0] >= ATTACK_COOLDOWN)
                {
                    cooldownTimer[0] -= ATTACK_COOLDOWN;
                    canAttack[0] = true;
                }
            }
        }
        private void attackPattern1(GameTime gameTime)
        {
            attackPattern(gameTime);
            if (canAttack[1])
            {
                if (numProjectilesShot == NUM_PROJECTILES)
                {
                    cooldownTimer[1] = 0;
                    canAttack[1] = false;
                }
                RapidFire(gameTime);
            }
            else
            {
                cooldownTimer[1] += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (cooldownTimer[1] >= ATTACK_COOLDOWN)
                    canAttack[1] = true;
            }
        }
        private void attackPattern2(GameTime gameTime)
        {
            attackPattern(gameTime);
            if (canAttack[2] && isSolid)
                Charge();
            else
            {
                if (isSolid == false)
                {
                    if (etherealTimer >= REGAIN_FORM_TIME)
                    {
                        if (numProjectilesShot == NUM_PROJECTILES)
                        {
                            isSolid = true;
                            etherealTimer = 0;
                            numProjectilesShot = 0;
                            canAttack[1] = false;
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
                    cooldownTimer[2] += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (cooldownTimer[2] >= ATTACK2_COOLDOWN)
                    {
                        canAttack[2] = true;
                        cooldownTimer[2] -= ATTACK2_COOLDOWN;
                        diveDirection = homeDirection(1.5f) * 4;
                        color = Color.IndianRed * (80f / 255f);
                        isEthereal = true;
                    }
                }
            }
        }
        private void attackPattern3(GameTime gameTime)
        {
            if (canAttack[3])
            {
                if(bombTimer >= CHARGE_TIME)
                    cooldownTimer[3] -= ATTACK1_COOLDOWN;
                if (gathered == true)
                    EnergyBomb(gameTime);
                else
                    ScatterShot(gameTime);
            }
            else
            {
                cooldownTimer[3] += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (cooldownTimer[3] >= ATTACK1_COOLDOWN)
                    canAttack[3] = true;
            }            
        }        
        private void RapidFire(GameTime gameTime, float rand = 0.5f)
        {
            attackDirection = homeDirection(PROJECTILE_SPEED);
            Vector2 randDir;
            rotation = attackAngle - (float)Math.PI / 2;

            if (numProjectilesShot < NUM_PROJECTILES)
            {
                if (ChargeCooldown >= rapidfireCooldown)
                {                    
                    randDir = getRandomDirection(rand);
                    Vector2 tempAttackDirection = new Vector2(attackDirection.X + randDir.X, attackDirection.Y + randDir.Y);
                    rotation = attackAngle - (float)Math.PI / 2;
                    particles.Add(new Particle(new Vector2(position.X, position.Y),
                        particleTexture, rotation, Color.DarkOrange, tempAttackDirection, 1,ParticleType.Regular,acceleration,0.5f));
                    ChargeCooldown -= 0.2f;
                    numProjectilesShot++;
                    
                }
                else
                    ChargeCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
                numProjectilesShot = 0;
        }
        private void Charge()
        {
            rotation = attackAngle - (float)Math.PI/2;
            Vector2 targetPos = position + new Vector2(diveDirection.X, diveDirection.Y);
            if (inBounds(targetPos) == false)
            {
                canAttack[2] = false;
                isSolid = false;
            }
            else
                position = targetPos;
        }
        private void EnergyBomb(GameTime gameTime)
        {            
            bombTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            color = Color.OrangeRed * ((175 + 80 * bombTimer) / 255);
            if (bombTimer >= CHARGE_TIME)
            {
                invulnerable = false;
                attackDirection = homeDirection(PROJECTILE_SPEED2+2.2f);
                for (int p = 0; p < NUM_PARTICLES; p++)
                {
                    Particle temp = new Particle(position, particleTexture, 0,
                        new Color(255, 255, 255, 45), attackDirection, 5, ParticleType.Super);
                    temp.Scale = 3.5f;
                    Color tempC = Color.OrangeRed;
                    tempC.A = 45;
                    temp.pColor = tempC;
                    particles.Add(temp);
                }
                bombTimer -= CHARGE_TIME;
                color = Color.White;
                gathered = false;
                canAttack[3] = false;            
            }
        }
        private void ScatterShot(GameTime gameTime, float rand = 0.5f)
        {
            Vector2 randDir;
            NUM_PROJECTILES = 51;
            if (numProjectilesShot2 < NUM_PROJECTILES)
            {
                rotTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                attackAngle = rotTimer * MathHelper.TwoPi;
                rotation = attackAngle;
                if (ChargeCooldown >= 0.1)
                {                    
                    attackDirection = new Vector2((float)Math.Cos(attackAngle) * PROJECTILE_SPEED, (float)Math.Sin(attackAngle) * PROJECTILE_SPEED);          
                    randDir = getRandomDirection(rand);
                    Vector2 tempAttackDirection = new Vector2(attackDirection.X + randDir.X, attackDirection.Y + randDir.Y);
                    float randSlow = MathHelper.Lerp(0.992f, 0.995f, (float)Globals.Rand.NextDouble());
                    particles.Add(new Particle(new Vector2(position.X, position.Y),
                            particleTexture, rotation, Color.White, tempAttackDirection, 1, ParticleType.Scatter, randSlow));
                    ChargeCooldown -= 0.1f;
                    numProjectilesShot2++;
                }
                else
                    ChargeCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                gatherShots();
                rotTimer = 0;
                numProjectilesShot2 = 0;
                canAttack[3] = false;
            }
        }
        private void gatherShots()
        {
            foreach (Particle p in particles)
            {
                if (p.type == ParticleType.Scatter)
                {
                    float dir;
                    if (position.X >= p.Position.X)
                        dir = (float)Math.Atan((position.Y - p.Position.Y) / (position.X - p.Position.X));
                    else
                        dir = (float)Math.Atan((position.Y - p.Position.Y) / (position.X - p.Position.X)) + MathHelper.Pi;
                    p.Direction = new Vector2((float)Math.Cos(dir) * 4.0f, (float)Math.Sin(dir) * 4.0f);
                    p.returned = true;
                }
            }
            invulnerable = true;
            gathered = true;
        }
        private void spinAttack()
        {
            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 8)
            {
                particles.Add(new Particle(new Vector2(position.X + (float)Math.Cos(i) * origin.X, position.Y + (float)Math.Sin(i) * origin.Y),
                    particleTexture, rotation, Color.MediumVioletRed, new Vector2((float)Math.Cos(i), (float)Math.Sin(i)), 1, ParticleType.Regular, 1, 0.4f));
            }
            canAttack[0] = false;
        }
        private void teleport()
        {
            //Position = new Vector2(r.Next((int)origin.X + 250, 350 - (int)origin.X), r.Next((int)origin.Y + 200, 300 - (int)origin.Y));
            position = initialPosition;
            rotation = 0;
            canAttack[3] = false;
        }
        private void hostileAttack(GameTime gameTime)
        {
            NUM_PROJECTILES = 30;
            acceleration = 1.0075f;
            NUM_PROJECTILES = 20;
            RapidFire(gameTime, 1.3f);
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
                p.Draw(sb);
            }            
            sb.End();
            sb.Begin();
            base.Draw(sb);
        }
    }
}
