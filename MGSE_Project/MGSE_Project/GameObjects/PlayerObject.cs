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

        IInputDevice inputDevice;

        KeyboardState kbState;

        public PlayerObject() { }

        public PlayerObject(string name, IInputDevice inputDevice, Vector2 startPosition, Color color)
        {
            this.name = name;
            this.inputDevice = inputDevice;
            pos = startPosition;
            this.color = color;
        }

        public void loadContent(ContentManager content, Texture2D texture)
        {
            this.texture = texture;
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
        }
    }
}
