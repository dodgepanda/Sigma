/*  Dungeon.cs
 *  Dungeon class that holds rooms that the player explores and keeps informations on current room the player is in.
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
    class Dungeon
    {
        private Room[,] rooms;
        private Room startRoom, currentRoom;
        private int width, height;
        private Rectangle playableArea;
        private Tangible target;
        public Dungeon(int dungeonWidth, int dungeonHeight, Rectangle playable, Player t)
        {
            playableArea = playable;
            width = dungeonWidth;
            height = dungeonHeight;
            rooms = new Room[width, height];
            target = t;
        }
        public void TestLevel()
        {
            rooms[2, 2] = new Room(RoomType.REGULAR, playableArea);
            rooms[2, 1] = new Room(RoomType.REGULAR, playableArea);
            rooms[1, 1] = new Room(RoomType.REGULAR, playableArea);
            rooms[2, 0] = new Room(RoomType.BOSS, playableArea);
            rooms[3, 1] = new Room(RoomType.SHOP, playableArea);
            rooms[2, 3] = new Room(RoomType.REGULAR, playableArea);
            rooms[2, 4] = new Room(RoomType.REGULAR, playableArea);
            rooms[1, 4] = new Room(RoomType.LOCKED, playableArea);
            rooms[3, 4] = new Room(RoomType.BOSS, playableArea);
            rooms[4, 4] = new Room(RoomType.REGULAR, playableArea);
            rooms[5, 4] = new Room(RoomType.REGULAR, playableArea);
            rooms[4, 3] = new Room(RoomType.REGULAR, playableArea);
            rooms[4, 2] = new Room(RoomType.BOSS, playableArea);
            rooms[0, 1] = new Room(RoomType.HIDDEN, playableArea);
            SetupRooms();
            startRoom = rooms[2, 1];
            CurrentRoom = startRoom;
            rooms[0,1].Chests.Add(new TreasureChest(new Vector2(100,100), true));
            rooms[0,1].Chests.Add(new TreasureChest(new Vector2(100,300), false));
            rooms[2, 0].Boss = (new Boss1(new Vector2(playableArea.Width / 2, playableArea.Height / 2), 0, 50));
            rooms[2, 2].Tangibles.Add(new Enemy1(new Vector2(100, 100), target, 0, 3));
            rooms[2, 2].Tangibles.Add(new Enemy1(new Vector2(400, 200), target, 0, 3));
            rooms[2, 2].Tangibles.Add(new Enemy1(new Vector2(200, 400), target, 0, 3));
            rooms[2, 3].Tangibles.Add(new Enemy1(new Vector2(150, 400), target, 0, 3));
            rooms[2, 3].Tangibles.Add(new Enemy1(new Vector2(475, 400), target, 0, 3));
            rooms[2, 3].Tangibles.Add(new Enemy2(new Vector2(200, 150), target, 0, 3));
            rooms[2, 3].Tangibles.Add(new Enemy2(new Vector2(400, 150), target, 0, 3));
            rooms[1, 1].Tangibles.Add(new Enemy2(new Vector2(300, 80), target, 0, 2));
            rooms[1, 1].Tangibles.Add(new Enemy2(new Vector2(300, 420), target, 0, 2));
            rooms[1, 1].Tangibles.Add(new Enemy2(new Vector2(80, 250), target, 0, 2));
            rooms[1, 4].Pickups.Add(new Pickup(new Vector2(100, 400), PickupType.Damage));
            rooms[1, 4].Pickups.Add(new Pickup(new Vector2(100, 100), PickupType.Attackspeed));
            rooms[1, 4].Pickups.Add(new Pickup(new Vector2(500, 100), PickupType.Movementspeed));
            rooms[1, 4].Pickups.Add(new Pickup(new Vector2(500, 400), PickupType.Money));
            rooms[1, 4].Chests.Add(new TreasureChest(new Vector2(300, 250), true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(125, 50), PickupType.Health, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(125, 175), PickupType.Bomb, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(125, 300), PickupType.Key, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(125, 425), PickupType.Money));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(250, 50), PickupType.Attackspeed, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(250, 175), PickupType.Movementspeed, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(250, 300), PickupType.HealthUp, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(250, 425), PickupType.Damage, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(375, 50), PickupType.Health, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(375, 175), PickupType.Bomb, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(375, 300), PickupType.Key, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(375, 425), PickupType.Map, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(500, 50), PickupType.Attackspeed, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(500, 175), PickupType.Movementspeed, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(500, 300), PickupType.HealthUp, true));
            rooms[3, 1].Pickups.Add(new Pickup(new Vector2(500, 425), PickupType.Damage, true));
            ArrangeShopItems(rooms[3, 1]);
            rooms[4, 4].Tangibles.Add(new Enemy1(new Vector2(250, 250), target, 0, 3));
            rooms[4, 4].Tangibles.Add(new Enemy1_2(new Vector2(350, 150), target, new Vector2(-0.5f, -0.5f),0, 1));
            rooms[4, 4].Tangibles.Add(new Enemy1_2(new Vector2(150, 150), target, new Vector2(0.5f, -0.5f), 0, 1));
            rooms[3, 4].Boss = (new Boss2(new Vector2(playableArea.Width / 2, playableArea.Height / 2), target, 0, 80));
            rooms[4, 2].Boss = (new Boss3(new Vector2(playableArea.Width / 2, playableArea.Height / 2), target, 0, 250)); 
            //for(int i = 0; i < 10; ++i)
            //    rooms[5, 4].Pickups.Add(new Pickup(new Vector2(100 + i*30, 100 + i*30),PickupType.Damage, false));
            //AMBUSH!!
            for(float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi/20)
            {
                Vector2 dir = new Vector2((float)Math.Cos(i) * 0.5f, (float)Math.Sin(i) * 0.5f);
                rooms[4, 3].Tangibles.Add(new Enemy1_2(new Vector2(250, 300),target, dir , 0, 1));
            }
        }
        public void ShowMap()
        {
            foreach (Room r in rooms)
            {
                if(r!=null)
                    r.Explore();
            }
        }
        public void ArrangeShopItems(Room r)
        {
        }
        public Room StartRoom
        {
            get { return startRoom; }
        }
        public Room CurrentRoom
        {
            get { return currentRoom; }
            set { currentRoom = value;
            Globals.CurrentRoom = currentRoom;
            currentRoom.Explore();
            }
        }
        public int GetWidth() { return width; }
        public int GetHeight() { return height; }
        public Room[,] Rooms
        {
            get { return rooms; }
        }
        private void SetupRooms()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bool nroom = false, sroom = false, wroom = false, eroom = false;
                    if (j < height-1) nroom = rooms[i, j+1] != null;
                    if (j > 0) sroom = rooms[i, j-1] != null;
                    if (i < width-1) eroom = rooms[i+1, j] != null;
                    if (i > 0) wroom = rooms[i-1, j] != null;
                    if(rooms[i,j]!=null)
                        LinkRooms(i, j, nroom, sroom, wroom, eroom);
                }
            }
        }
        private void LinkRooms(int x, int y, bool north = false, bool south = false, bool west = false, bool east = false)
        {
            if (rooms[x, y] == null)
                return;
            if (north && y < height )
                rooms[x, y].North = rooms[x, y + 1];
            if (south && y > 0)
                rooms[x, y].South = rooms[x, y - 1];
            if (west && x < width)
                rooms[x, y].West = rooms[x - 1, y];
            if (east && x > 0)
                rooms[x, y].East = rooms[x + 1, y];
        }

    }
}
