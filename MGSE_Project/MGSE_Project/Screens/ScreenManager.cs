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
        public GameScreen currentScreen;

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
        protected override void Dispose(bool disposing)
        {
            /*
            foreach (GameScreen screen in screens)
                screen.Dispose(disposing);
                */
            base.Dispose(disposing);
        }
        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            if (currentScreen == null)
                currentScreen = screens.ElementAt(0);
            currentScreen.LoadContent();
        }
        protected override void UnloadContent()
        {
            currentScreen.UnloadContent();

        }
        public override void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            currentScreen.Draw(gameTime);
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
        /// <summary>
        /// Transition to the next screen.
        /// </summary>
        /// <param name="type">Next screen to transition to.</param>
        /// <param name="message">Pass a message on to the next screen </param>
        public void Transition(Type type, string message)
        {
            currentScreen.UnloadContent();
            for (int i = 0; i < screens.Count(); i ++)
            {
                if (screens[i].GetType() == type)
                    currentScreen = screens[i];
            }
            currentScreen.Transition(message);
            currentScreen.LoadContent();
            

            //currentScreen.Update()
        }
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }
    }
}
