/*  Pickup.cs
 *  Pickup class for objects that can picked up by the player as items or upgrades
 *  There are a variety of pickups defined in this class
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
    public enum PickupType
    {
        Health,
        HealthUp,
        Money,
        Key,
        Bomb,
        Movementspeed,
        Damage,
        Attackspeed,
        UpgradeShot,
        Map
    }
    class Pickup : Tangible
    {
        protected PickupType type;
        protected int cost;
        protected bool shopItem;
        public Pickup(Vector2 pos, PickupType type = PickupType.Health, bool shop = false):base(pos, Globals.DUMMYTEXTURE)
        {
            shopItem = shop;
            this.type = type;
            switch (type)
            {
                case (PickupType.Health): Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\heart");
                    cost = 1;
                    break;
                case (PickupType.Bomb): Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\bomb");
                    cost = 2;
                    break;
                case (PickupType.Key): Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\key");
                    cost = 3;
                    break;
                case (PickupType.Money): Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\money");
                    break;
                case (PickupType.Damage): Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\damage");
                    cost = 5;
                    break;
                case (PickupType.Movementspeed): Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\boots");
                    cost = 5;
                    break;
                case (PickupType.Attackspeed): Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\arrow");
                    cost = 5;
                    break;
                case (PickupType.UpgradeShot): Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\multishot");
                    cost = 15;
                    break;
                case (PickupType.HealthUp): Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\maxheart");
                    cost = 10;
                    break;
                case (PickupType.Map): Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\map");
                    cost = 10;
                    break;
                default:    Texture = Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\heart");
                    cost = 1;
                    break;
            }
        }
        public bool IsShopItem() { return shopItem; }
        public int GetCost() { return cost; }
        public override void Draw(SpriteBatch sb)
        {
            if (shopItem)
            {
                sb.DrawString(Globals.CONTENTMANAGER.Load<SpriteFont>(@"Fonts\UIFont"), "$"+cost,position+new Vector2(-5,texture.Height/2),Color.White);
            }
            base.Draw(sb);
        }
        public PickupType Type
        {
            get { return type; }
        }

    }
}
