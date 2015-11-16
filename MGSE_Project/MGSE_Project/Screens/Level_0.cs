using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Net;


namespace MGSE_Project
{
    class Level_0 : GameScreen
    {
        bool runOnce = false;

        List<IGameObject> gameObjects;
        List<IGameObject> worldObjects;
        SpriteBatch spriteBatch;
        ContentManager content;
        Connection connection;

        int noOfPickups = 20;

        public Level_0()
        {
            ScreenState = ScreenState.Active;

            
            //Handle error code here (return to main menu)

            //connection.SendInit(); //Send player data
            //Call method to recieve other player data and add to worldObjects to be drawn. Call in update method
        }
        public override void LoadContent()
        {
            spriteBatch = ScreenManager.SpriteBatch;
            content = new ContentManager(ScreenManager.Game.Services, "Content");
            Random random = new Random();

            Texture2D playerTexture = content.Load<Texture2D>("Textures/player");
            Texture2D pickupTexture = content.Load<Texture2D>("Textures/pickup");
            Texture2D boundaryTexture = content.Load<Texture2D>("Textures/boundary");

            gameObjects = new List<IGameObject>();
            gameObjects.Add(
                new PlayerObject("PLAYER1", 
                new ClientInput(),
                new Vector2(0,0), 
                new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)),
                playerTexture));

            worldObjects = new List<IGameObject>();
            
            for(int i = 0; i < noOfPickups; i++)
            {
                worldObjects.Add(new WorldObject(
                    new Rectangle(random.Next(0, ScreenManager.GraphicsDevice.Viewport.Width), 
                        random.Next(0, ScreenManager.GraphicsDevice.Viewport.Height),
                        25, 25), pickupTexture));
            }


            //Draw World Boundaries
            worldObjects.Add(
                new WorldObject(
                    new Rectangle(0, 0, 5, ScreenManager.GraphicsDevice.Viewport.Height), 
                    boundaryTexture));
            worldObjects.Add(
                new WorldObject(
                    new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width - 5, 0, 5,
                        ScreenManager.GraphicsDevice.Viewport.Height),
                    boundaryTexture));
            worldObjects.Add(
                new WorldObject(
                    new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width,
                        5),
                    boundaryTexture));
            worldObjects.Add(
                new WorldObject(
                    new Rectangle(0, ScreenManager.GraphicsDevice.Viewport.Height - 5,
                    ScreenManager.GraphicsDevice.Viewport.Width, 5),
                    boundaryTexture));


            //TODO: World objects are explicitly named but gameobjects / Player are not
            /*
            foreach (IGameObject worldObjects in worldObjects)
                worldObjects.loadContent(content);
            */
            foreach (IGameObject gameObject in gameObjects)
                gameObject.loadContent(content);

            connection = new Connection();
            connection.Connect(IPAddress.Parse("127.0.0.1"), 8888);
            connection.sendInit(gameObjects.ElementAt(0) as PlayerObject); //send initial player data


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
