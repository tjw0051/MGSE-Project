﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MGSE_Project
{
    public interface IGameObject
    {
        /*
        Components:
        Input - User and network
        Collidable/rigid/etc

        */
        string Name
        {
            get;
        }
        Rectangle Rect { get; set; }
        void loadContent(ContentManager content);
        void update(GameTime gameTime);
        void draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
