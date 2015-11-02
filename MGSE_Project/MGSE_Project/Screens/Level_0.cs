using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace MGSE_Project
{
    class Level_0 : GameScreen
    {
        bool runOnce = false;

        List<IGameObject> gameObjects;
        List<IGameObject> worldObjects;
        SpriteBatch spriteBatch;
        ContentManager content;

        int noOfPickups = 20;

        public Level_0()
        {
            ScreenState = ScreenState.Active;
        }
        public override void LoadContent()
        {
            spriteBatch = ScreenManager.SpriteBatch;
            content = new ContentManager(ScreenManager.Game.Services, "Content");

            Texture2D playerTexture = content.Load<Texture2D>("player");
            Texture2D pickupTexture = content.Load<Texture2D>("pickup");

            gameObjects = new List<IGameObject>();
            gameObjects.Add(new PlayerObject("Player 1", new ClientInput(), new Vector2(0,0), Color.BlueViolet));

            worldObjects = new List<IGameObject>();
            Random random = new Random();
            for(int i = 0; i < noOfPickups; i++)
            {
                worldObjects.Add(new WorldObject(
                    new Vector2(random.Next(0, ScreenManager.GraphicsDevice.Viewport.Width),
                                random.Next(0, ScreenManager.GraphicsDevice.Viewport.Height))));
            }

            

            foreach (IGameObject worldObjects in worldObjects)
                worldObjects.loadContent(content, pickupTexture);
            foreach (IGameObject gameObject in gameObjects)
                gameObject.loadContent(content, playerTexture);

            //LoadTextures
            //LoadSounds
            //Load Game Objects?

            //Load UI
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
            if (!runOnce)
            {
                Console.WriteLine("GameScreen");
                runOnce = true;
            }

            foreach (IGameObject worldObjects in worldObjects)
                worldObjects.update(gameTime);
            foreach (IGameObject gameobject in gameObjects)
                gameobject.update(gameTime);
            
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (IGameObject worldObjects in worldObjects)
                worldObjects.draw(gameTime, spriteBatch);
            foreach (IGameObject gameobject in gameObjects)
                gameobject.draw(gameTime, spriteBatch);

            spriteBatch.End();

        }
    }
}
