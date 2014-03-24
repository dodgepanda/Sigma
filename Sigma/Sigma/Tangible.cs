/*  Tangible.cs
 *  Tangible class is the basis for all objects in the game that have a position, texture, collision, etc
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
    public abstract class Tangible
    {
        protected Vector2 position;
        public Vector2 origin;
        protected float rotation, elapseTime, radius;
        protected Texture2D texture;
        protected Color color;
        protected Color[] textureData;
        protected int health, maxHealth, meleeDamage;
        protected bool alive, invulnerable, ethereal, debugThis;
        protected Rectangle sourceRect;
        protected float scale = 1.0f;
        protected SpriteEffects effect = SpriteEffects.None;
        public List<Particle> particles = new List<Particle>();

        public Tangible(Vector2 pos, Texture2D tex, float rot = 0, int h = 0) 
        {
            position = pos;
            rotation = rot;
            texture = tex;
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            color = Color.White;
            health = h;
            maxHealth = health;
            alive = true;
            invulnerable = false;
            ethereal = false;
            debugThis = true;
            radius = MathHelper.Max(texture.Width, texture.Height) / 2;
        }

        //Accessors
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;

                textureData = new Color[texture.Width * texture.Height];
                texture.GetData(textureData);
                origin = new Vector2(texture.Width / 2, texture.Height / 2);
            }
        }
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public int MeleeDamage
        {
            get { return meleeDamage; }
        }
        public int MaxHealth
        {
            get { return maxHealth; }
        }
        public int Health
        {
            get { return health; }
            set { 
                health = value;
                health = (int)MathHelper.Clamp(health, 0, maxHealth);
            }
        }
        public bool isAlive
        {
            get { return alive; }
            set { alive = value; }
        }
        public bool isInvulnerable
        {
            get { return invulnerable; }
            set { invulnerable = value; }
        }
        public bool isEthereal
        {
            get { return ethereal; }
            set { ethereal = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Color pColor
        {
            get { return color; }
            set { color = value; }
        }

        public virtual void OnDeath()
        {
        }

        //To be called from Game.Update() 
        public virtual void Update(GameTime gameTime)
        {
            elapseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        //To be called from Game.Draw() 
        public virtual void Draw(SpriteBatch sb)
        {
            if (alive == true)
            {
                if (sourceRect.IsEmpty)
                    sb.Draw(texture, position, null, color,
                        rotation, origin, scale, effect, 0.0f);
                else
                    sb.Draw(texture, position, sourceRect, color,
                        rotation, origin, scale, effect, 0.0f);
                if (Globals.Debug && debugThis)
                {
                    int thickness = 1;
                    Color c = Color.White;
                    Vector2 dimension = new Vector2(texture.Width, texture.Height);

                    int x = (int)position.X - (int)dimension.X / 2;
                    int y = (int)position.Y - ((int)dimension.Y / 2);
                    int w = (int)(dimension.X * 1);
                    int h = (int)dimension.Y;
                    sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y, w, thickness), c);
                    sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y, thickness, h), c);
                    sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x + w - thickness, y, thickness, h), c);
                    sb.Draw(Globals.DUMMYTEXTURE, new Rectangle(x, y + h - thickness, w, thickness), c);
                }
            }
        }

        //Returns transform Matrix
        public Matrix Transform()
        {
            return Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                Matrix.CreateScale(scale) * 
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(position, 0.0f));
        }

        public virtual Rectangle Rectangle()
        {
            return CalculateBoundingRectangle(
                        new Rectangle(0, 0, texture.Width, texture.Height),
                        Transform());
        }

        public bool CircleCollidesWith(Tangible t)
        {
            double distance = Math.Sqrt(Math.Pow(position.X - t.position.X, 2) + Math.Pow(position.Y - t.position.Y, 2));

            return radius + t.radius > distance;
        }

        public bool RectangleCollidesWith(Tangible t)
        {
            if (Rectangle().Intersects(t.Rectangle()))
            {
                return true;
            }
            return false;
        }
        public bool RectangleCollidesWith(Rectangle r)
        {
            if (Rectangle().Intersects(r))
            {
                return true;
            }
            return false;
        }

        public bool CollidesWith(Tangible t)
        {

            // The per-pixel check is expensive, so check the bounding rectangles
            // first to prevent testing pixels when collisions are impossible.
            if (RectangleCollidesWith(t))
            {
                // Check collision with person
                if (IntersectPixels(Transform(), texture.Width,
                                    texture.Height, textureData,
                                    t.Transform(), t.texture.Width,
                                    t.texture.Height, t.textureData))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A >= 200 && colorB.A >= 200)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                           Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }    
}
