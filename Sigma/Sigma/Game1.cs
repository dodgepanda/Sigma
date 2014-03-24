/*  Game1.cs
 *  This is where most logic for the game is along with functions for drawing the UI
 *  Controls: WASD to move character, arrow keys to shoot in directions.
 *            Left Shift to drop bombs, H to toggle health bars, ~ to enter debug state
 * 
 *  Project Sigma
 *  Michael Ou
 *  Wei Wei Huang
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Sigma
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Viewport vp;
        Dungeon dungeon;
        Texture2D uiBackground, minimapRoomSprite, minimapNDoor, minimapSDoor, minimapWDoor, minimapEDoor, 
            playerTexture, verticalWall, horizontalWall, minimapPlayer, verticalSolid, horizontalSolid, minimapBoss,
            dummyTexture, uiKey, uiBomb, uiMoney, uiDamage, uiMSpeed, uiASpeed, circle100;
        Texture2D ground, wallTexture, playerParTexture;
        SpriteFont uiFont;
        KeyboardState OldKeyState;
        Rectangle playableArea;
        Wall[] walls;
        Color uiColor = Color.White;
        Player player;    
        float flash = 0;
        bool tangibleWasDestroyed = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 600;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            vp = graphics.GraphicsDevice.Viewport;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.CONTENTMANAGER = this.Content;
            uiBackground = Content.Load<Texture2D>(@"Sprites\uibg");
            playableArea = new Rectangle(0,0,vp.Width, vp.Height - uiBackground.Height);
            minimapRoomSprite = Content.Load<Texture2D>(@"Sprites\room");
            minimapNDoor = Content.Load<Texture2D>(@"Sprites\ndoor");
            minimapSDoor = Content.Load<Texture2D>(@"Sprites\sdoor");
            minimapWDoor = Content.Load<Texture2D>(@"Sprites\wdoor");
            minimapEDoor = Content.Load<Texture2D>(@"Sprites\edoor");
            minimapBoss = Content.Load<Texture2D>(@"Sprites\uiskull");
            verticalWall = Content.Load<Texture2D>(@"Sprites\verticalWall2");
            horizontalWall = Content.Load<Texture2D>(@"Sprites\horizontalWall2");
            verticalSolid = Content.Load<Texture2D>(@"Sprites\verticalSolid2");
            horizontalSolid = Content.Load<Texture2D>(@"Sprites\horizontalSolid2");
            uiKey = Content.Load<Texture2D>(@"Sprites\key");
            uiBomb = Content.Load<Texture2D>(@"Sprites\bomb");
            uiMoney = Content.Load<Texture2D>(@"Sprites\money");
            uiDamage = Content.Load<Texture2D>(@"Sprites\damage");
            uiMSpeed = Content.Load<Texture2D>(@"Sprites\boots");
            uiASpeed = Content.Load<Texture2D>(@"Sprites\arrow");
            dummyTexture = Content.Load<Texture2D>(@"Sprites\dummy");
            Globals.DUMMYTEXTURE = dummyTexture;
            uiFont = Content.Load<SpriteFont>(@"Fonts\UIFont");

            playerTexture = Content.Load<Texture2D>(@"Sprites\player");
            playerParTexture = Content.Load<Texture2D>(@"Sprites\particle");
            minimapPlayer = Content.Load<Texture2D>(@"Sprites\minimapplayer");
            player = new Player(new Vector2(playableArea.Width / 2 , playableArea.Height / 2 ), playerTexture, 0);
            // TODO: use this.Content to load your game content here
            ground = Content.Load<Texture2D>(@"Sprites\Floor3");
            wallTexture = Content.Load<Texture2D>(@"Sprites\WallTexture");
            circle100 = Content.Load<Texture2D>(@"Sprites\100x100circle");
            dungeon = new Dungeon(6, 5, playableArea, player);
            Globals.Dungeon = dungeon;
            dungeon.TestLevel();
            walls = new Wall[4];
            createWalls();
            //trail = Content.Load<Effect>(@"Effects\Trail");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState NewKeyState = Keyboard.GetState();
            // Get input
            KeyboardState kInput = Keyboard.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            Vector2 direction = Vector2.Zero;

            // Handle keyboard inputs
            if (kInput.IsKeyDown(Keys.Escape)) 
                this.Exit();
            if (player.isAlive)
            {
                direction = movementInput(kInput);
                updatePlayerPosition(ref direction);
                if (player.Shoot == true)
                    attackInput(kInput, direction);
                if (NewKeyState.IsKeyDown(Keys.LeftShift)) { player.DropBomb(gameTime); }
            }
            if (NewKeyState.IsKeyDown(Keys.H) && !OldKeyState.IsKeyDown(Keys.H)) { Globals.ShowHPBars = !Globals.ShowHPBars; }
            if (NewKeyState.IsKeyDown(Keys.OemTilde) && !OldKeyState.IsKeyDown(Keys.OemTilde)) { Globals.Debug = !Globals.Debug; }
            

            //update each particle stream
            foreach (Projectile p in player.Weapon)
                p.Update();

            //Apply any collision any enemy tangibles made
            foreach (Tangible t in dungeon.CurrentRoom.Tangibles)
            {
                applyEnemyCollision(t);
                t.Update(gameTime);
            }
            //Player pickup
            for (int i = 0; i < dungeon.CurrentRoom.Pickups.Count; i++)
            {
                if (dungeon.CurrentRoom.Pickups[i].CollidesWith(player))
                {
                    if (player.PickUp(dungeon.CurrentRoom.Pickups[i]))
                    {
                        dungeon.CurrentRoom.Pickups.Remove(dungeon.CurrentRoom.Pickups[i]);
                        i--;
                    }
                }
            }
            //Apply any collisions the player's attack makes
            applyPlayerCollision();

            //Logic for when player is hit to make him invulnerable for 2 seconds
            if (player.isEthereal == true)
            {
                //player.Update(gameTime);
                player.isEthereal = false;
            }
            if (player.isInvulnerable == true)
            {
                flash += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (flash >= Globals.INVULNERABLE_TIME)
                {
                    player.isInvulnerable = false;
                    flash -= Globals.INVULNERABLE_TIME;
                    //player.Update(gameTime);
                }
            }
            player.Update(gameTime);
            //If a tangible was destroyed, remove it from the tangibles list
            if(tangibleWasDestroyed == true)
                destroyTangibles();
            //Check if player moved to another room
            updateRoomState();

            //player shot cooldown
            if (player.Shoot == false && player.isAlive)
            {
                player.Cooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (player.Cooldown >= player.AttackSpeed)
                {
                    player.Cooldown -= player.AttackSpeed;
                    player.Shoot = true;
                }
            }

            if (dungeon.CurrentRoom.isAllDead())
                SwitchDoors(false);
            OldKeyState = NewKeyState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            //draw terrain
            spriteBatch.Draw(ground, Vector2.Zero, Color.MintCream);
            walls[0].Draw(spriteBatch);
            walls[1].Draw(spriteBatch);
            walls[2].Draw(spriteBatch);
            walls[3].Draw(spriteBatch);
            //draw tangibles
            foreach (Tangible t in dungeon.CurrentRoom.Chests)
                t.Draw(spriteBatch);
            foreach (Tangible t in dungeon.CurrentRoom.Tangibles)
                t.Draw(spriteBatch);
            foreach (Tangible t in dungeon.CurrentRoom.Pickups)
                t.Draw(spriteBatch);
            foreach (Bomb b in player.Bombs)
                b.Draw(spriteBatch);
            foreach (Explosion e in player.Explosions)
                e.Draw(spriteBatch);
            //draw the particle streams
            spriteBatch.End();
            //Ended spritebatch to begin it with different rendering modes
            foreach (Projectile p in player.Weapon)
                p.Draw(spriteBatch);
            //Start the spritebatch again
            spriteBatch.Begin();
            if(player.isAlive == true)
                player.Draw(spriteBatch);
            drawUI(new Vector2(0, vp.Height - 100));
            spriteBatch.End();
            base.Draw(gameTime);
        }

        //Checks keyboard input to decide which direction to shoot
        private void attackInput(KeyboardState kInput, Vector2 dir)
        {
            if (kInput.IsKeyDown(Keys.Up))
            {
                player.Attack(new Vector2(dir.X, -1));
                player.Weapon[player.Weapon.Count - 1].LoadParticles(playerParTexture);
                player.Shoot = false;
            }
            else if (kInput.IsKeyDown(Keys.Down))
            {
                player.Attack(new Vector2(dir.X, 1));
                player.Weapon[player.Weapon.Count - 1].LoadParticles(playerParTexture);
                player.Shoot = false;
            }
            else if (kInput.IsKeyDown(Keys.Left))
            {
                player.Attack(new Vector2(-1, dir.Y));
                player.Weapon[player.Weapon.Count - 1].LoadParticles(playerParTexture);
                player.Shoot = false;
            }
            else if (kInput.IsKeyDown(Keys.Right)) 
            {
                player.Attack(new Vector2(1, dir.Y));
                player.Weapon[player.Weapon.Count - 1].LoadParticles(playerParTexture);
                player.Shoot = false;
            }
        }
        
        //Keyboard movement for player movement
        private Vector2 movementInput(KeyboardState kInput)
        {
            Vector2 dir = Vector2.Zero;
            if (kInput.IsKeyDown(Keys.A))
            {
                dir -= new Vector2(player.MovementSpeed, 0);
            }
            if (kInput.IsKeyDown(Keys.D))
            {
                dir += new Vector2(player.MovementSpeed, 0);
            }
            if (kInput.IsKeyDown(Keys.W))
            {
                dir -= new Vector2(0, player.MovementSpeed);
            }
            if (kInput.IsKeyDown(Keys.S))
            {
                dir += new Vector2(0, player.MovementSpeed);
            }
            return dir;
        }

        //Fixes player collision with walls
        private void updatePlayerPosition(ref Vector2 dir)
        {
            Vector2 playerInitial = player.Position;
            player.Position += dir;
            bool collide = false;
            List<int> wallIndex = new List<int>();

            //Find all colliding walls
            for (int i = 0; i < walls.Count(); ++i)
            {
                if (player.CollidesWith(walls[i]) || (player.RectangleCollidesWith(walls[i].SoftSpot) && walls[i].LockedOrClosed()))
                {
                    wallIndex.Add(i);
                    collide = true;
                }
            }

            //If a collision was detected...
            //Find the direction you can move in, and move in that direction only
            if (collide)
            {
                string unmoveableDir = "";
                Player tempPlayer = player;

                for (int j = 0; j < wallIndex.Count; j++)
                {
                    //Check if x direction is causing collision
                    tempPlayer.Position = playerInitial + new Vector2(dir.X, 0);
                    if (tempPlayer.CollidesWith(walls[wallIndex[j]]) || (player.RectangleCollidesWith(walls[wallIndex[j]].SoftSpot) && walls[wallIndex[j]].LockedOrClosed()))
                        unmoveableDir += "X";

                    //Check if y direction is causing collision
                    tempPlayer.Position = playerInitial + new Vector2(0, dir.Y);
                    if (tempPlayer.CollidesWith(walls[wallIndex[j]]) || (player.RectangleCollidesWith(walls[wallIndex[j]].SoftSpot) && walls[wallIndex[j]].LockedOrClosed()))
                        unmoveableDir += "Y";
                }
                if (unmoveableDir == "X") //if cannot move in x set x direction to 0
                    dir.X = 0;
                if (unmoveableDir == "Y") //if cannot move in y set y direction to 0
                    dir.Y = 0;
                if(unmoveableDir == "XY" || unmoveableDir == "YX") //if cannot move in both direction set direction vector to 0
                    dir = Vector2.Zero;
                player.Position = playerInitial + dir;
            }
        }

        private void WallToRegular(int wall)
        {
            switch (wall)
            {
                case 0: dungeon.CurrentRoom.West.Type = RoomType.REGULAR;//w
                    break;
                case 1: dungeon.CurrentRoom.North.Type = RoomType.REGULAR; //n
                    break;
                case 2: dungeon.CurrentRoom.East.Type = RoomType.REGULAR; //e
                    break;
                case 3: dungeon.CurrentRoom.South.Type = RoomType.REGULAR; //s
                    break;
                default:
                    break;
            }
        }

        private void applyPlayerCollision()
        {
            for (int i = 0; i < dungeon.CurrentRoom.Chests.Count; i++)
            {
                if(player.CollidesWith(dungeon.CurrentRoom.Chests[i]))
                {
                    if (dungeon.CurrentRoom.Chests[i].IsLocked())
                    {
                        if (player.Keys > 0)
                        {
                            dungeon.CurrentRoom.Chests[i].OnDeath();
                            dungeon.CurrentRoom.Chests.RemoveAt(i);
                            i--;
                            player.Keys--;
                        }
                    }
                    else
                    {
                        dungeon.CurrentRoom.Chests[i].OnDeath();
                        dungeon.CurrentRoom.Chests.RemoveAt(i);
                        i--;
                    }
                }
            }
            for (int i = 0; i < walls.Length; i++)
            {
                if (player.isAlive && player.RectangleCollidesWith(Rectangle.Union(walls[i].SoftSpot, new Rectangle(walls[i].SoftSpot.X, walls[i].SoftSpot.Y, walls[i].SoftSpot.Width + (int)player.MovementSpeed, walls[i].SoftSpot.Height + (int)player.MovementSpeed))))
                {
                    if (!walls[i].Closed && walls[i].Locked)
                    {
                        if (player.Keys > 0)
                        {
                            player.Keys--;
                            walls[i].Locked = false;
                            switch (i)
                            {
                                case 0: dungeon.CurrentRoom.West.Type = RoomType.REGULAR;//w
                                    break;
                                case 1: dungeon.CurrentRoom.North.Type = RoomType.REGULAR; //n
                                    break;
                                case 2: dungeon.CurrentRoom.East.Type = RoomType.REGULAR; //e
                                    break;
                                case 3: dungeon.CurrentRoom.South.Type = RoomType.REGULAR; //s
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                foreach (Explosion e in player.Explosions)
                {
                    //if (player.Explosions[j].RectangleCollidesWith(new Rectangle(walls[i].SoftSpot.X, walls[i].SoftSpot.Y, walls[i].SoftSpot.Width + 10, walls[i].SoftSpot.Height+10)))
                    if (e.AOE.Intersects(walls[i].SoftSpot))                    
                    {
                        if (walls[i].Type == RoomType.HIDDEN)
                        {
                            switch (i)
                            {
                                case 0: dungeon.CurrentRoom.West.Type = RoomType.REGULAR;//w
                                    break;
                                case 1: dungeon.CurrentRoom.North.Type = RoomType.REGULAR; //n
                                    break;
                                case 2: dungeon.CurrentRoom.East.Type = RoomType.REGULAR; //e
                                    break;
                                case 3: dungeon.CurrentRoom.South.Type = RoomType.REGULAR; //s
                                    break;
                                default:
                                    break;
                            }
                            walls[i].ExplodeWall();
                        }
                    }
                    if(player.RectangleCollidesWith(e.AOE) && player.isAlive && player.isInvulnerable == false && player.isEthereal == false)
                    {
                        player.Health -= Explosion.DAMAGE;                        
                        if (player.Health <= 0)
                        {
                            player.isAlive = false;
                            player.isInvulnerable = false;
                            player.isEthereal = false;
                            player.Shoot = false;
                        }
                        else
                        {
                            player.isInvulnerable = true;
                            player.isEthereal = true;
                        }
                    }
                    foreach (Tangible t in dungeon.CurrentRoom.Tangibles)
                    {
                        if (t.isAlive && t.isInvulnerable == false && t.RectangleCollidesWith(e.AOE))
                        {
                            if (dungeon.CurrentRoom.Type == RoomType.BOSS)
                            {
                                if (e.canHurtBoss == true)
                                {
                                    t.Health -= Explosion.DAMAGE;
                                    if (t.Health <= 0)
                                    {
                                        t.isAlive = false;
                                        if (t.GetType().BaseType == typeof(Enemy))
                                        {
                                            t.OnDeath();
                                        }
                                        tangibleWasDestroyed = true;
                                    }
                                    e.canHurtBoss = false;
                                }
                            }
                            else
                            {
                                t.Health -= Explosion.DAMAGE;
                                if (t.Health <= 0)
                                {
                                    t.isAlive = false;
                                    if (t.GetType().BaseType == typeof(Enemy))
                                    {
                                        t.OnDeath();
                                    }
                                    tangibleWasDestroyed = true;
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < player.Weapon.Count; ++i)
            {
                for(int j = 0; j < player.Weapon[i].attackStream.Count; j++)
                {
                    if (player.Weapon[i].attackStream[j].remove == true
                        || player.Weapon[i].attackStream[j].targetParticle.CollidesWith(walls[0]) //player.weapon[i].targetParticle.collideswith..
                        || player.Weapon[i].attackStream[j].targetParticle.CollidesWith(walls[1])
                        || player.Weapon[i].attackStream[j].targetParticle.CollidesWith(walls[2])
                        || player.Weapon[i].attackStream[j].targetParticle.CollidesWith(walls[3]))
                    {
                        player.Weapon[i].attackStream.Remove(player.Weapon[i].attackStream[j]);
                        j--;
                        break;
                    }
                    else
                    {
                        foreach (Tangible t in dungeon.CurrentRoom.Tangibles)
                        {
                            if (t.isAlive && t.isEthereal == false && player.Weapon[i].attackStream[j].targetParticle.CollidesWith(t))
                            {
                                if (t.isInvulnerable == false)
                                {
                                    t.Health -= player.Weapon[i].attackStream[j].damage;
                                    if (t.Health <= 0)
                                    {
                                        t.isAlive = false;
                                        if (t.GetType().BaseType == typeof(Enemy))
                                        {
                                            t.OnDeath();
                                        }
                                        tangibleWasDestroyed = true;
                                    }
                                }
                                player.Weapon[i].attackStream.Remove(player.Weapon[i].attackStream[j]);
                                j--;
                                break;
                            }
                        }
                    }
                }
                if (player.Weapon[i].attackStream.Count == 0)
                {
                    player.Weapon.Remove(player.Weapon[i]);
                    i--;
                }
            }
        }

        private void applyEnemyCollision(Tangible t)
        {
            if (t.isAlive && t.isEthereal == false && player.isAlive && player.isInvulnerable == false && t.CollidesWith(player))
            {
                player.Health -= t.MeleeDamage;
                player.isInvulnerable = true;
                player.isEthereal = true;
            }
            else
            {
                for (int p = 0; p < t.particles.Count; ++p)
                {
                    if (t.particles[p].CollidesWith(walls[0]) || t.particles[p].CollidesWith(walls[1])
                        || t.particles[p].CollidesWith(walls[2]) || t.particles[p].CollidesWith(walls[3]))
                    {
                        t.particles.RemoveAt(p);
                        p--;
                    }
                    else if (player.isAlive && player.isInvulnerable == false && t.particles[p].CollidesWith(player))
                    {
                        player.Health -= t.particles[p].power;
                        player.isInvulnerable = true;
                        player.isEthereal = true;
                        t.particles.RemoveAt(p);
                        p--;
                    }
                }
            }
            if (player.Health <= 0)
            {
                //player.isAlive = false;
                dungeon.CurrentRoom = dungeon.StartRoom;
                clearRoom();
                destroyTangibles();
                player.Position = new Vector2(playableArea.Width / 2, playableArea.Height / 2);
                createWalls();
                player.isInvulnerable = false;
                player.isEthereal = false;
                player.Shoot = false;
                player.Money = 0;
                player.Health = player.MaxHealth / 2;
            }
        }

        private void destroyTangibles()
        {
            for(int t = 0; t < dungeon.CurrentRoom.Tangibles.Count; ++t)
            {
                if (dungeon.CurrentRoom.Tangibles[t].isAlive == false)
                {
                    if (dungeon.CurrentRoom.Tangibles[t].particles.Count == 0)
                    {
                        dungeon.CurrentRoom.Tangibles.RemoveAt(t);
                        t--;
                    }
                }
            }
            tangibleWasDestroyed = false;
        }

        private void updateRoomState()
        {
            if (player.Position.X < playableArea.X) //head w
            {
                clearRoom();
                destroyTangibles();
                player.Position = new Vector2(playableArea.Width - (player.Texture.Width+10), player.Position.Y);
                dungeon.CurrentRoom = dungeon.CurrentRoom.West;
                createWalls();
            }
            if (player.Position.X > playableArea.Width) //head e
            {
                clearRoom();
                destroyTangibles();
                player.Position = new Vector2(playableArea.X + (player.Texture.Width+10), player.Position.Y);
                dungeon.CurrentRoom = dungeon.CurrentRoom.East;
                createWalls();
            }
            if (player.Position.Y < playableArea.Y) //head n
            {
                clearRoom();
                destroyTangibles();
                player.Position = new Vector2(player.Position.X, playableArea.Height-(10+player.Texture.Height));
                dungeon.CurrentRoom = dungeon.CurrentRoom.North;
                createWalls();
            }
            if (player.Position.Y > playableArea.Height) //head s
            {
                clearRoom();
                destroyTangibles();
                player.Position = new Vector2(player.Position.X, playableArea.Y + (10+player.Texture.Height));
                dungeon.CurrentRoom = dungeon.CurrentRoom.South;
                createWalls();
            }
        }

        private void clearRoom()
        {
            player.Weapon.Clear();
            player.Bombs.Clear();
            player.Explosions.Clear();
            foreach (Tangible t in dungeon.CurrentRoom.Tangibles)
                t.particles.Clear();
        }

        private void createWalls()
        {
            Texture2D westWall, northWall, eastWall, southWall;
            westWall = verticalWall;
            northWall = horizontalWall;
            eastWall = verticalWall;
            southWall = horizontalWall;
            RoomType wType, nType, eType, sType;
            wType = RoomType.REGULAR;
            nType = RoomType.REGULAR;
            eType = RoomType.REGULAR;
            sType = RoomType.REGULAR;

            if (dungeon.CurrentRoom.West == null || dungeon.CurrentRoom.West.Type == RoomType.HIDDEN)
                westWall = verticalSolid;
            if (dungeon.CurrentRoom.North == null || dungeon.CurrentRoom.North.Type == RoomType.HIDDEN)
                northWall = horizontalSolid;
            if (dungeon.CurrentRoom.East == null || dungeon.CurrentRoom.East.Type == RoomType.HIDDEN)
                eastWall = verticalSolid;
            if (dungeon.CurrentRoom.South == null || dungeon.CurrentRoom.South.Type == RoomType.HIDDEN)
                southWall = horizontalSolid;
            if (dungeon.CurrentRoom.West != null)
                wType = dungeon.CurrentRoom.West.Type;
            if (dungeon.CurrentRoom.North != null)
                nType = dungeon.CurrentRoom.North.Type;
            if (dungeon.CurrentRoom.East != null)
                eType = dungeon.CurrentRoom.East.Type;
            if (dungeon.CurrentRoom.South != null)
                sType = dungeon.CurrentRoom.South.Type;
            walls[0] = new Wall(new Vector2(playableArea.X + westWall.Width / 2, playableArea.Y + westWall.Height / 2), westWall, wType);   
            walls[1] = new Wall(new Vector2(playableArea.X + northWall.Width / 2, playableArea.Y + northWall.Height / 2), northWall, nType);
            walls[2] = new Wall(new Vector2(playableArea.Width - eastWall.Width + eastWall.Width / 2, playableArea.Y + eastWall.Height / 2), eastWall, eType);
            walls[3] = new Wall(new Vector2(playableArea.X + southWall.Width / 2, playableArea.Height - southWall.Height + southWall.Height / 2), southWall, sType);

            SwitchDoors(true);
        }

        private void SwitchDoors(bool closed)
        {
            foreach (Wall w in walls)
            {
                w.Closed = closed;
            }
        }
        #region UIStuff
        private void drawMinimapElement(Vector2 position, Texture2D tex, int x, int y, Color c)
        {
            spriteBatch.Draw(tex, new Vector2(x * tex.Width, -y * tex.Height) + new Vector2(position.X, position.Y + (dungeon.GetHeight() - 1) * tex.Height), c);
        }
        private void drawMinimap(Vector2 position)
        {
            for (int i = 0; i < dungeon.GetWidth(); i++)
            {
                for (int j = 0; j < dungeon.GetHeight(); j++)
                {
                    if (dungeon.Rooms[i, j] != null && dungeon.Rooms[i,j].IsExplored())
                    {
                        drawMinimapElement(position, minimapRoomSprite, i, j, uiColor);
                        if (dungeon.Rooms[i, j].North == null || dungeon.Rooms[i,j].North.Type == RoomType.HIDDEN)
                            drawMinimapElement(position, minimapNDoor, i, j, uiColor);
                        if (dungeon.Rooms[i, j].South == null || dungeon.Rooms[i, j].South.Type == RoomType.HIDDEN)
                            drawMinimapElement(position, minimapSDoor, i, j, uiColor);
                        if (dungeon.Rooms[i, j].West == null || dungeon.Rooms[i, j].West.Type == RoomType.HIDDEN)
                            drawMinimapElement(position, minimapWDoor, i, j, uiColor);
                        if (dungeon.Rooms[i, j].East == null || dungeon.Rooms[i, j].East.Type == RoomType.HIDDEN)
                            drawMinimapElement(position, minimapEDoor, i, j, uiColor);

                        if (dungeon.Rooms[i, j].Type == RoomType.BOSS)
                            drawMinimapElement(position, minimapBoss, i, j, uiColor);

                        if (dungeon.Rooms[i, j].Type == RoomType.SHOP)
                            drawMinimapElement(position, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\uishop"), i, j, uiColor);

                        if (dungeon.Rooms[i, j].Chests.Count>0)
                            drawMinimapElement(position, Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites\uichest"), i, j, uiColor);

                        if (dungeon.Rooms[i, j] == dungeon.CurrentRoom)
                            drawMinimapElement(position, minimapPlayer, i, j, uiColor);
                    }
                    else
                    {
                        //spriteBatch.Draw(minimapRoomSprite, new Vector2(i * minimapRoomSprite.Width, -j * minimapRoomSprite.Height) + new Vector2(position.X, position.Y + (dungeon.GetHeight()-1) * minimapRoomSprite.Height), Color.Teal);
                    }
                }
            }
        }
        private void drawBossHP(Vector2 position)
        {
            Rectangle bossHP = new Rectangle((int)position.X , (int)position.Y , (int)(200 * ((float)Math.Abs(dungeon.CurrentRoom.Boss.Health) / (float)dungeon.CurrentRoom.Boss.MaxHealth)), 20);
            spriteBatch.Draw(dummyTexture, bossHP, Color.Red);
            drawBox(position, new Vector2(200, 20), 2, uiColor);
        }
        private void drawPlayerHP(Vector2 position)
        {
            Rectangle playerHP = new Rectangle((int)position.X, (int)position.Y, (int)(200 * ((float)Math.Abs(player.Health) / (float)player.MaxHealth)), 20);
            spriteBatch.Draw(dummyTexture, playerHP, Color.Green);
            drawBox(position, new Vector2(200, 20), 2, uiColor);
        }
        private void drawInventoryInfo(Vector2 position)
        {
            spriteBatch.Draw(uiKey, position , uiColor);
            spriteBatch.Draw(uiBomb, position + new Vector2(65, 0), uiColor);
            spriteBatch.Draw(uiMoney, position + new Vector2(130, 0), uiColor);
            spriteBatch.DrawString(uiFont, ":" + player.Keys, position + new Vector2(30, 15), uiColor);
            spriteBatch.DrawString(uiFont, ":" + player.NumBombs, position + new Vector2(95, 15), uiColor);
            spriteBatch.DrawString(uiFont, ":" + player.Money, position + new Vector2(160, 15), uiColor);
        }
        private void drawStats(Vector2 position)
        {
            spriteBatch.Draw(uiDamage, position, uiColor);
            spriteBatch.DrawString(uiFont, ":" + player.MeleeDamage, position + new Vector2(35, 15), uiColor);
            spriteBatch.Draw(uiMSpeed, position + new Vector2(0, uiDamage.Height + 2), uiColor);
            spriteBatch.DrawString(uiFont, ":" + player.MovementSpeed, position + new Vector2(35, uiDamage.Height + 15), uiColor);
            spriteBatch.Draw(uiASpeed, position + new Vector2(0, uiMSpeed.Height + uiDamage.Height + 2), uiColor);
            spriteBatch.DrawString(uiFont, ":" + Math.Round(1/player.AttackSpeed,2), position + new Vector2(35, uiMSpeed.Height + uiDamage.Height + 15), uiColor);
        }
        private void drawUI(Vector2 position)
        {
            spriteBatch.Draw(uiBackground, position, Color.Black);
            drawMinimap(position);
            drawBox(position + new Vector2(140, 0), new Vector2(100, 100), 1, uiColor);
            drawStats(position + new Vector2(155, 3));
            drawBox(position + new Vector2(260, 0), new Vector2(100, 100), 1, uiColor);
            switch (player.GetProjectileCount())
            {
                case 1: spriteBatch.Draw(Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites/upgrade1"), position + new Vector2(260 + 25, 25), uiColor);
                    break;
                case 2: spriteBatch.Draw(Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites/upgrade2"), position + new Vector2(260 + 25, 25), uiColor);
                    break;
                case 3: spriteBatch.Draw(Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites/upgrade3"), position + new Vector2(260 + 25, 25), uiColor);
                    break;
                default: spriteBatch.Draw(Globals.CONTENTMANAGER.Load<Texture2D>(@"Sprites/upgrade3"), position + new Vector2(260+25, 25), uiColor);
                    break;
            }
            if (dungeon.CurrentRoom.Type == RoomType.BOSS)
                drawBossHP(position + new Vector2(380, 10));
            drawPlayerHP(position + new Vector2(380, 40));
            drawInventoryInfo(position + new Vector2(380, 65));

            if (dungeon.CurrentRoom == dungeon.StartRoom)
            {
                spriteBatch.DrawString(uiFont, "W A S D to move", new Vector2(50, 50), uiColor);
                spriteBatch.DrawString(uiFont, "Arrow keys to shoot", new Vector2(50, 100), uiColor);
                spriteBatch.DrawString(uiFont, "Shift to drop bomb", new Vector2(50, 150), uiColor);
            }
        }
        private void drawBox(Vector2 position, Vector2 dimension, int thickness, Color c)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            int w = (int)dimension.X;
            int h = (int)dimension.Y;
            spriteBatch.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y, w, thickness), c);
            spriteBatch.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y, thickness, h), c);
            spriteBatch.Draw(Globals.DUMMYTEXTURE, new Rectangle(x + w - thickness, y, thickness, h), c);
            spriteBatch.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y + h - thickness, w, thickness), c);
        }
        #endregion
    }
}
