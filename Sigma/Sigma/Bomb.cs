/*  Bomb.cs
 *  Class for a bomb object, for when users drop bombs for hidden rooms or to damage enemies
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
    class Bomb:Tangible
    {
        bool exploded = false;
        float explodeTime;

        public Bomb(Vector2 pos, float ExplodeTime = 2.5f)
            : base(pos, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\bomb"))
        {
            explodeTime = ExplodeTime;
        }
        public bool Exploded
        {
            get { return exploded; }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (elapseTime >= explodeTime)
            {
                exploded = true;
            }
            else
            {
                if (((int)(2 * Math.Abs(Math.Sin(Math.Pow(elapseTime, 3))))) == 0)
                {
                    color = Color.Red;
                }
                else
                    color = Color.White;
            }    
        }
    }
}
