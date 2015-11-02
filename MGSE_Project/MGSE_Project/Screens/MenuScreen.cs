using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MGSE_Project
{
    class MenuScreen : GameScreen
    {
        bool runOnce = false;

        public MenuScreen()
        {
            ScreenState = ScreenState.Hidden;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!runOnce)
            {
                Console.WriteLine("MenuScreen");
                runOnce = true;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

        }
    }
}
