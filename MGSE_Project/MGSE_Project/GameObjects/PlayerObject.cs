using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MGSE_Project
{
    class PlayerObject : IGameObject
    {
        string name = "";
        Vector2 pos;
        Vector2 velocity;
        Color color;
        Texture2D texture;
        Rectangle rect;
        SpriteFont font;

        IInputDevice inputDevice;

        public PlayerObject() { }

        public PlayerObject(string name, IInputDevice inputDevice,
            Vector2 startPosition, Color color, Texture2D texture)
        {
            this.name = name;
            this.inputDevice = inputDevice;
            pos = startPosition;
            this.color = color;
            this.texture = texture;
        }

        public void loadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("SpriteFont1");
        }

        public void update(GameTime gameTime)
        {
            inputDevice.update();
            velocity = inputDevice.Axis * 0.3f;
            
            pos.X += velocity.X * gameTime.ElapsedGameTime.Milliseconds;
            pos.Y += velocity.Y * gameTime.ElapsedGameTime.Milliseconds;

            //Todo: Don't keep creating new rectangles
            rect = new Rectangle((int)pos.X + 50, (int)pos.Y + 50, 150, 150);
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, color);
            Vector2 fontOrigin = font.MeasureString(name) / 2;
            spriteBatch.DrawString(font, name,
                new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2),
                Color.Black, 0, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);
        }
    }
}
