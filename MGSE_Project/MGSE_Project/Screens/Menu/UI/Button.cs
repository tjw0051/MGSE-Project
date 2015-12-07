using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSE_Project
{
    class Button
    {
        string text;
        Vector2 pos;
        Texture2D tex;
        public Rectangle rect
        {
            get; private set;
        }
        Color color, highlightColor, currentColor;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Vector2 fontCenter;

        public Button(string text, Vector2 pos, Color color)
        {
            this.text = text;
            this.pos = pos;
            this.color = color;
            this.highlightColor = new Color(color.R - 30, color.G - 30, color.B - 30);
        }
        public void LoadContent(SpriteBatch spriteBatch,
            GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.spriteBatch = spriteBatch;
            this.tex = new Texture2D(graphicsDevice, 1, 1);
            tex.SetData(new Color[] { Color.White });
            rect = new Rectangle((int)pos.X, (int)pos.Y, 100, 50);
            font = content.Load<SpriteFont>("SpriteFont1");
            fontCenter = font.MeasureString(text) / 2;
        }

        public void update(GameTime gametime, Rectangle mousePos)
        {
            if (mousePos.Intersects(rect))
            {
                currentColor = highlightColor;
            }
            else
                currentColor = color;
        }
        public void draw()
        {
            spriteBatch.Draw(tex, rect, currentColor);
            spriteBatch.DrawString(font, text,
                new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2),
                Color.Black, 0, fontCenter, 1.0f, SpriteEffects.None, 0.5f);
        }
    }
}
