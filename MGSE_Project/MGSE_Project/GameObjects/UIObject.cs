﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MGSE_Project.GameObjects
{
    class UIObject : IGameObject
    {
        string name = "";
        public string Name
        {
            get
            {
                return name;
            }
        }
        
        public Rectangle Rect { get; set; }

        public void loadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public void update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
