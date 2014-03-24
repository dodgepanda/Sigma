/*  Projectile.cs
 *  A Projectile is a particle stream used for different attacks by player and bosses
 *  There are a variety of projectiles such as default, double, triple, Sine, and Homing 
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

        public Projectile(Vector2 pos, Vector2 dir, int damage = 1, ProjectileType type = ProjectileType.Default)
        {
            int shotDamage = damage;
            switch(type){
                case (ProjectileType.Triple):
                    shotDamage = (int)MathHelper.Clamp(((float)shotDamage * .75f), 1, shotDamage);
                    if (dir.X == 1 || dir.X == -1)
                    {
                        attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y + 3.4f), shotDamage));
                        attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y), shotDamage));
                        attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y - 3.4f), shotDamage));
                    }
                    else
                    {
                        attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X + 3.4f, dir.Y), shotDamage));
                        attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y), shotDamage));
                        attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X - 3.4f, dir.Y), shotDamage));
                    }
                    break;
                case(ProjectileType.Double):
                    shotDamage = (int)MathHelper.Clamp(((float)shotDamage * .9f), 1, shotDamage);
                    if (dir.X == 1 || dir.X == -1)
                    {
                        attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y + 1.5f), shotDamage));
                        //attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y), damage));
                        attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y - 1.5f), shotDamage));
                    }
                    else
                    {
                        attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X + 1.5f, dir.Y), shotDamage));
                        //attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y), damage));
                        attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X - 1.5f, dir.Y), shotDamage));
                    }
                    break;
                default: attackStream.Add(new AttackStream(new Vector2(pos.X, pos.Y), new Vector2(dir.X, dir.Y), shotDamage));
                    break;
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
