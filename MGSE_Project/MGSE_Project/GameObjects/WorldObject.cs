using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MGSE_Project
{
    class WorldObject : IGameObject
    {
        Texture2D texture;
        Vector2 pos;
        Rectangle rect;

        public WorldObject(Vector2 pos)
        {
            this.pos = pos;
        }
        public void loadContent(ContentManager content, Texture2D texture)
        {
            this.texture = texture;
        }

        public void update(GameTime gameTime)
        {
            //Check for collision with player
            rect = new Rectangle((int)pos.X + 50, (int)pos.Y + 50, 25, 25);
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, Color.White);
        }
    }
}
