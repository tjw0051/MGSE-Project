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
    class TextBox
    {
        public string text { get; set; }
        Vector2 pos;
        Texture2D tex;
        public Color color { get; set; }
        SpriteBatch spriteBatch;
        SpriteFont font;
        Vector2 fontCenter;

        public TextBox(string text, Vector2 pos, Color color)
        {
            this.text = text;
            this.pos = pos;
            this.color = color;
        }
        public void LoadContent(SpriteBatch spriteBatch,
            GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.spriteBatch = spriteBatch;
            this.tex = new Texture2D(graphicsDevice, 1, 1);
            tex.SetData(new Color[] { Color.White });
            font = content.Load<SpriteFont>("SpriteFont1");
            
        }

        public void update(GameTime gametime, Rectangle mousePos)
        {
            fontCenter = font.MeasureString(text) / 2;
        }
        public void draw()
        {
            spriteBatch.DrawString(font, text,
                new Vector2(pos.X - (font.MeasureString(text).X/2), pos.Y),
                Color.Black, 0, fontCenter, 1.0f, SpriteEffects.None, 0.5f);
        }
    }
}
