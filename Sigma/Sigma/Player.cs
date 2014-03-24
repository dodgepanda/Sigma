/*  Player.cs
 *  Player class to a player and player information such at hp, location, direction, items, etc.
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
    class Player : Tangible
    {
        private int keys, numBombs, money, projectileCount;
        private float movementSpeed, cooldown, attackSpeed, bombCooldown, lastBomb;
        private bool shoot;
        private ProjectileType weaponType;
        private List<Projectile> weapon = new List<Projectile>();
        private List<Bomb> bombs = new List<Bomb>();
        private List<Explosion> explosions = new List<Explosion>();
        public Player(Vector2 Position, Texture2D Texture, float Rotation = 0, int h = 10)
            : base(Position, Texture, Rotation, h)
        {
            keys = 1;
            numBombs = 1;
            money = 10;
            movementSpeed = 2.5f;
            meleeDamage = 1;
            cooldown = 0;
            shoot = true;
            attackSpeed = 0.5f;
            bombCooldown = 1f;
            lastBomb = 0;
            projectileCount = 1;
            weaponType = ProjectileType.Default; 
        }
        public void Attack(Vector2 dir)
        {
            float xDir = MathHelper.Clamp(dir.X, -1, 1);
            float yDir = MathHelper.Clamp(dir.Y, -1, 1);
            weapon.Add(new Projectile(new Vector2(position.X + (xDir) * (texture.Width / 2), position.Y + (yDir) * (texture.Height / 2)),
                       dir, meleeDamage, weaponType));
            Globals.CONTENTMANAGER.Load<SoundEffect>(@"Sounds/laser3").Play(.5f,0f,0f);
        }
        public int GetProjectileCount() { return projectileCount; }
        public float BombCooldown
        {
            get { return bombCooldown; }
        }
        public List<Projectile> Weapon
        {
            get { return weapon; }
            set { weapon = value; }
        }
        public List<Bomb> Bombs
        {
            get { return bombs; }
            set { bombs = value; }
        }
        public List<Explosion> Explosions
        {
            get { return explosions; }
            set { explosions = value; }
        }
        public bool PickUp(Pickup item)
        {
            if (item.IsShopItem())
            {
                if (money < item.GetCost())
                    return false;
            }
            switch (item.Type)
            {
                case PickupType.Health: health += 1;
                    health = (int)MathHelper.Clamp(health, 0, maxHealth);
                    break;
                case PickupType.Bomb: numBombs += 1;
                    break;
                case PickupType.Key: keys += 1;
                    break;
                case PickupType.Money: money += 1;
                    break;
                case PickupType.Damage: 
                    if(meleeDamage < 10)
                        meleeDamage += 1;
                    break;
                case PickupType.Movementspeed: movementSpeed += 0.5f;
                    break;
                case PickupType.HealthUp: maxHealth += 1;
                    break;
                case PickupType.Map: Globals.Dungeon.ShowMap();
                    break;
                case PickupType.UpgradeShot: projectileCount++;
                    if (projectileCount == 2)
                        weaponType = ProjectileType.Double;
                    if (projectileCount == 3)
                        weaponType = ProjectileType.Triple;
                    if (projectileCount > 3)
                        if (meleeDamage < 10)
                            meleeDamage++;
                    break;
                case PickupType.Attackspeed: AttackSpeed -= .05f;
                    break;
                default: break;
            }
            if (item.IsShopItem())
                money -= item.GetCost();
            return true;
        }
        public void DropBomb(GameTime gameTime)
        {
            if (numBombs > 0 &&( lastBomb == 0 || ((lastBomb + bombCooldown) <= (float)gameTime.TotalGameTime.TotalSeconds)))
            {
                bombs.Add(new Bomb(position));
                lastBomb = (float)gameTime.TotalGameTime.TotalSeconds;
                numBombs--;
            }
        }
        public float MovementSpeed
        {
            get { return movementSpeed; }
        }
        public float Cooldown
        {
            get { return cooldown; }
            set
            {
                cooldown = value;
            }
        }
        public bool Shoot
        {
            get { return shoot; }
            set { shoot = value; }
        }
        public float AttackSpeed
        {
            get { return attackSpeed; }
            set
            {
                attackSpeed = value;
                if (attackSpeed < 0.1f)
                    attackSpeed = 0.1f;
            }
        }
        public int Keys
        {
            get { return keys; }
            set
            {
                keys = value;
                if (keys < 0)
                    keys = 0;
            }
        }
        public int NumBombs
        {
            get { return numBombs; }
            set
            {
                numBombs = value;
                if (numBombs < 0)
                    numBombs = 0;
            }
        }
        public int Money
        {
            get { return money; }
            set
            {
                money = value;
                if (money < 0)
                    money = 0;
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (isInvulnerable == true)
            {
                pColor = new Color(255, 255, 255) * (80f / 255f);
            }
            else
            {
                pColor = Color.White;
            }
            for (int i = 0; i < bombs.Count; i++)
            {
                bombs[i].Update(gameTime);
                if (bombs[i].Exploded == true)
                {
                    explosions.Add(new Explosion(bombs[i].Position, false));
                    bombs.Remove(bombs[i]);
                    i--;
                }
            }
            for (int j = 0; j < explosions.Count; j++)
            {
                explosions[j].Update(gameTime);
                if (explosions[j].Finished == true)
                {
                    explosions.Remove(explosions[j]);
                    j--;
                }
            }
            base.Update(gameTime);
        }
    }
}
