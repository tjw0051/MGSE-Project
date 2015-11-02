﻿using System;
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

        public ScreenManager(Game game) : base(game)
        {

        }
        public override void Initialize()
        {
            base.Initialize();

            //isInitialized = true;
        }
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //spriteBatch = new SpriteBatch(GraphicsDevice);
            //font = content.Load<SpriteFont>("Fonts/classic1065");
            //blankTexture = content.Load<Texture2D>("Graphics/Menus/blank");

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
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
            /*
            // Read the keyboard and gamepad.
            //input.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            // screensToUpdate.Clear();

            //foreach (GameScreen screen in screens)
            //    screensToUpdate.Add(screen);

            //bool otherScreenHasFocus = !Game.IsActive;
            //bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
            */
            
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
            //screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            //screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            //if (isInitialized)
            ///{
           //     screen.LoadContent();
            //}

            screens.Add(screen);

            // update the TouchPanel to respond to gestures this screen is interested in
            //TouchPanel.EnabledGestures = screen.EnabledGestures;
        }
        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            //if (isInitialized)
            //{
            //    screen.UnloadContent();
            //}

            screens.Remove(screen);
            //screensToUpdate.Remove(screen);

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            //if (screens.Count > 0)
            //{
            //    TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
            //}
        }


        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }
    }
}
