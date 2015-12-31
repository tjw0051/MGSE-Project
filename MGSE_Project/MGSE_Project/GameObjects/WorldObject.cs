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
        public Rectangle Rect { get; set; }

        string name = "";
        public string Name
        {
            get
            {
                return name;
            }
        }
        public Vector2 Pos
        {
            get
            {
                return new Vector2(Rect.X, Rect.Y);
            }
        }

        public WorldObject(Rectangle rectangle, Texture2D texture)
        {
            this.Rect = rectangle;
            this.texture = texture;
        }
        //TODO: Cleanup - texture can be assigned in initializer
        public void loadContent(ContentManager content)
        {
        }

        public void update(GameTime gameTime)
        {
            //Check for collision with player
            //Make them float around slowly
            
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, Color.White);
        }
    }
}
