/*  AttackBall.cs
 *  Particle stream class that consists of an array of particle objects for a stream like effect
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
    public class AttackStream
    {
        public Particle[] particles;
        public Particle targetParticle;
        Vector2 position, direction;
        Color particleColor;
        public int damage;

        int nextParticle = 0;
        Vector2 targetPos;
        Vector2 lastPos;
        public bool remove = false;

        public AttackStream(Vector2 pos, Vector2 dir, int damage = 1)
        {
            this.position = pos;
            this.direction = dir;
            particleColor = Color.White;
            this.damage = damage;
        }
        public void LoadParticles(Texture2D particleTexture)
        {
            targetPos = position;
            lastPos = targetPos;
            particles = new Particle[Globals.NUM_PARTICLES];
            for (int p = 0; p < particles.Length; p++)
            {
                particles[p] = new Particle(position, particleTexture, 0, 
                new Color(255, 255, 255, 45), Vector2.Zero, damage);
            }
            targetParticle = new Particle(position, particleTexture, 0,
                new Color(255, 255, 255, 45), Vector2.Zero, damage);
        }
        public void Update()
        {
            float tempScale = 0;
            for (int p = 0; p < particles.Length; p++)
            {
                if (p == nextParticle && lastPos != position)
                {
                    particles[p].Position = lastPos;
                    particles[p].pColor = particleColor;
                }
                if (particles[p].Position != position) // Particle is in use
                {
                    if (damage == 1)
                        particles[p].Scale = 1 - (Vector2.Distance(particles[p].Position, targetPos) / 65);
                    else
                        particles[p].Scale = (1 - (Vector2.Distance(particles[p].Position, targetPos) / 65)) * damage / 1.8f;
                    tempScale = particles[p].Scale;
                    particles[p].pColor = new Color(particles[p].pColor.R, particles[p].pColor.G, particles[p].pColor.B, (particles[p].Scale * 255));
                }
            }
            nextParticle++;

            if (nextParticle >= particles.Length)
                nextParticle = 0;

            lastPos = targetPos;

            if(direction.X == 1 || direction.X == -1)
                targetPos.X += 5.0f * direction.X;
            else
                targetPos.X += (float)Math.Cos(targetPos.Y / 4 % MathHelper.TwoPi) + direction.X/1.8f;

            if (direction.Y == 1 || direction.Y == -1)
                targetPos.Y += 5.0f * direction.Y;
            else
                targetPos.Y +=  (float)Math.Sin(targetPos.X / 4 % MathHelper.TwoPi) + direction.Y/1.8f;

            targetParticle.Scale = tempScale;
            targetParticle.pColor = new Color(targetParticle.pColor.R, targetParticle.pColor.G, targetParticle.pColor.B, (targetParticle.Scale * 255));
            targetParticle.Position = targetPos;

            if (targetPos.Y < 0 || targetPos.Y > 500 || targetPos.X < 0 || targetPos.X > 600)
                remove = true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null);
            for (int p = 0; p < particles.Length; p++)
                particles[p].Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
