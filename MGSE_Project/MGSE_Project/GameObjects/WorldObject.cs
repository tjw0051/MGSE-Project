﻿using System;
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
        Rectangle rectangle;

        public WorldObject(Rectangle rectangle, Texture2D texture)
        {
            this.rectangle = rectangle;
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
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }
}
