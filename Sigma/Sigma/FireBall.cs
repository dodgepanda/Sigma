/*  FireBall.cs
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
    public enum ProjectileType
    {
        Default,
        Double,
        Triple,
        Sine,
        Homing
    }
    public class Projectile 
    {
        public List<AttackStream> attackStream = new List<AttackStream>();
        public int damage;

        public Projectile(Vector2 pos, Vector2 dir, int damage = 1)
        {
            attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y), damage));
        }
        public void LoadParticles(Texture2D particleTexture)
        {
            foreach (AttackStream f in attackStream)
                f.LoadParticles(particleTexture);
        }
        public void Update()
        {
            foreach (AttackStream f in attackStream)
                f.Update();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (AttackStream f in attackStream)
                f.Draw(spriteBatch);
        }
    }
}
