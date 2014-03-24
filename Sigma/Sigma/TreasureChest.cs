/*  TreasureChest.cs
 *  Class that defines a treasure chest object
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
    class TreasureChest:Tangible
    {
        bool locked;
        Pickup item;
        public TreasureChest(Vector2 Position, bool Locked = false)
            : base(Position, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\chest"))
        {
            locked = Locked;
            color = Color.Silver;
            if (locked)
                color = Color.Gold;
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
        public bool IsLocked() { return locked; }
        public override void OnDeath()
        {
            if (item != null)
            {
                item.Position = this.position;
                Globals.CurrentRoom.Pickups.Add(item);
            }
            base.OnDeath();
        }
    }
}
