/*  HoverText.cs
 *  Hover text class for drawing hovering text on screen for limited amount of time; scrolling combat text, etc
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
    class HoverText
    {
        private SpriteFont font;
        private Vector2 position;
        private Color color;
        private string text;
        private float elapsedTime, hoverTime;

        public HoverText(string Text, Vector2 Position, Color C, float HoverTime = 2f, SpriteFont Font = null)
        {
            if (Font == null)
                font = Globals.CONTENTMANAGER.Load<SpriteFont>(@"Fonts\UIFont");
            else
                font = Font;
            text = Text;
            position = Position;
            color = C;
            hoverTime = HoverTime;
        }

        public bool Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime < hoverTime)
            {
                color.A = (byte)((color.A) * Math.Abs(1 - elapsedTime / hoverTime));
            }
            else
            {
                return false;
            }
            return true;
        }
        public void Draw(SpriteBatch sb)
        {
            sb.DrawString(font, text, position, color);
        }
    }
}
