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
        List<PlayerObject> players;
        List<IGameObject> worldObjects;
        SpriteBatch spriteBatch;
        ContentManager content;
        Connection connection;

        Texture2D playerTexture;

        int noOfPickups = 20;

        public Level_0()
        {
            ScreenState = ScreenState.Active;

            //Handle error code here (return to main menu)

            //connection.SendInit(); //Send player data
            //Call method to recieve other player data and add to worldObjects to be drawn. Call in update method
        }
        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        public override void LoadContent()
        {
            spriteBatch = ScreenManager.SpriteBatch;
            content = new ContentManager(ScreenManager.Game.Services, "Content");
            Random random = new Random();

            playerTexture = content.Load<Texture2D>("Textures/player");
            Texture2D pickupTexture = content.Load<Texture2D>("Textures/pickup");
            Texture2D boundaryTexture = content.Load<Texture2D>("Textures/boundary");

            players = new List<PlayerObject>();
            players.Add(
                new PlayerObject("PLAYER " + random.Next(0, 100), 
                new ClientInput(),
                new Vector2(random.Next(0, 500),random.Next(0, 500)), 
                new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)),
                playerTexture,
                random.Next(48, 52)));

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
            connection = new Connection();
            connection.Connect(IPAddress.Parse("127.0.0.1"), 8888);
            connection.Initialize(players.ElementAt(0) as PlayerObject); //send initial player data
            updatePlayers(connection.PlayerList);

            foreach (PlayerObject player in players)
                player.loadContent(content);

            


            //LoadTextures
            //LoadSounds
            //Load Game Objects?

            //Load UI
        }

        public override void UnloadContent()
        {

        }

        private void updatePlayers(List<PlayerIn> newPlayers)
        {
            bool exists;
            foreach (PlayerIn player in newPlayers)
            {
                exists = false;
                foreach (PlayerObject currentPlayer in players)
                {
                    if(currentPlayer.Name == player.name)
                    {
                        //gameObjects.Remove(gameObject);
                        //ToDo: Inconsistent - Properties and set methods
                        currentPlayer.updatePosition(player.posX, player.posY);
                        currentPlayer.Size = player.size;
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    players.Add(
                                new PlayerObject(player.name,
                                new ServerInput(),
                                new Vector2(player.posX, player.posY),
                                new Color(200, 0, 20),
                                playerTexture,
                                player.size));
                }
            }
            connection.SendUpdate(players.ElementAt(0));
        }
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
            if (!runOnce)
            {
                Console.WriteLine("GameScreen");
                runOnce = true;
            }

            updatePlayers(connection.PlayerList);
            foreach (IGameObject worldObjects in worldObjects)
                worldObjects.update(gameTime);

            //CollisionCheck(gameTime);
            foreach (PlayerObject player in players)
            {
                player.update(gameTime);
            }
        }
        public void CollisionCheck(GameTime gameTime)
        {
            foreach (PlayerObject player in players)
            {
                foreach (PlayerObject player2 in players)
                {
                    if (Vector2.Distance(player.Center, player2.Center) <= player.Size / 2 + player2.Size / 2)
                    {
                        if (player.Size <= player2.Size)
                        {
                            player.Shrink();
                            player2.Grow();
                        }
                        else
                        {
                            player.Grow();
                            player2.Shrink();
                        }
                    }
                }
                
            }
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (IGameObject worldObjects in worldObjects)
                worldObjects.draw(gameTime, spriteBatch);
            foreach (PlayerObject player in players)
                player.draw(gameTime, spriteBatch);

            spriteBatch.End();
        }
    }
}
