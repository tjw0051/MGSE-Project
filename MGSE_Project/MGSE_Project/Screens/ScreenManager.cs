using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MGSE_Project
{
    public class ScreenManager : DrawableGameComponent
    {
        List<GameScreen> screens = new List<GameScreen>();

        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public ScreenManager(Game game) : base(game) { }

        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }
        protected override void UnloadContent()
        {
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }
        public override void Update(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if(screen.ScreenState != ScreenState.Hidden)
                {
                    screen.Update(gameTime);
                }
            }
            
        }
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ScreenManager = this;
            screens.Add(screen);
        }
        public void RemoveScreen(GameScreen screen)
        {
            screens.Remove(screen);
        }
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }
    }
}
