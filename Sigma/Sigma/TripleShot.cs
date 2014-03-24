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
    public class TripleShot
    {
        public List<AttackStream> attackStream = new List<AttackStream>();
        public int damage;

        public TripleShot(Vector2 pos, Vector2 dir, int damage = 1)
        {
            if (dir.X == 1 || dir.X == -1)
            {
                attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y + 4.5f), damage));
                attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y), damage));
                attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y - 4.5f), damage));
            }
            else
            {
                attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X + 4.5f, dir.Y), damage));
                attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y), damage));
                attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X - 4.5f, dir.Y), damage));
            }
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
