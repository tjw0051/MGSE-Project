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
using MGSE_Project.Utilities;

namespace MGSE_Project
{
    /// <summary>
    /// The main level for the game.
    /// </summary>
    class Level_0 : GameScreen
    {
        bool runOnce = false;
        string playerName = "NoName"; // Refactor: remove and get name from connection.instance.loadedPlayer.name
        List<PlayerObject> players;
        PlayerObject thisPlayer, thisPlayerPreviousState;
        List<IGameObject> worldObjects;
        SpriteBatch spriteBatch;
        ContentManager content;

        Texture2D playerTexture;

        int noOfPickups = 20;

        public Level_0()
        {
            Connection.Instance.removePlayerEvent += new Connection.RemovePlayerEvent(PlayerRemovedEvent);
            Connection.Instance.removePickupEvent += new Connection.RemovePickupEvent(RemovePickupEvent);
        }
        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        /// <summary>
        /// Load initial game data and assets once.
        /// </summary>
        public override void LoadContent()
        {
            //Load XNA Components.
            spriteBatch = ScreenManager.SpriteBatch;
            content = new ContentManager(ScreenManager.Game.Services, "Content");
            Random random = new Random();

            //Load Textures
            playerTexture = content.Load<Texture2D>("Textures/player");
            Texture2D pickupTexture = content.Load<Texture2D>("Textures/pickup");
            Texture2D boundaryTexture = content.Load<Texture2D>("Textures/boundary");

            //Create This Player
            players = new List<PlayerObject>();
            thisPlayer = 
                new PlayerObject(Connection.Instance.loadedPlayer.name, //"PLAYER " + random.Next(0, 100), 
                //"PLAYER " + random.Next(0, 100),
                new ClientInput(),
                ScreenManager.GraphicsDevice.Viewport,
                new Vector2(Connection.Instance.loadedPlayer.posX,
                    Connection.Instance.loadedPlayer.posY), 
                new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)),
                playerTexture,
                random.Next(48, 52));

            //Create Pickups
            //Check for existing players to load pickups from:
            worldObjects = new List<IGameObject>();

            while (Connection.Instance.PickupList == null) { }
            foreach(Vector2 pickupPos in Connection.Instance.PickupList)
            {
                Console.Write("x: " + pickupPos.X + " y: " + pickupPos.Y + " , ");
                worldObjects.Add(new WorldObject(
                    new Rectangle((int)pickupPos.X, (int)pickupPos.Y,
                        20, 20), pickupTexture));
            }
            /*
            for(int i = 0; i < noOfPickups; i++)
            {
                worldObjects.Add(new WorldObject(
                    new Rectangle(random.Next(0, ScreenManager.GraphicsDevice.Viewport.Width), 
                        random.Next(0, ScreenManager.GraphicsDevice.Viewport.Height),
                        20, 20), pickupTexture));
            }
            */

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
        }

        public override void UnloadContent() { }

        /// <summary>
        /// Updates player data based on data recieved from the server.
        /// </summary>
        /// <param name="newPlayers">List of newly recieved player data from server</param>
        private void updatePlayers(List<PlayerState> newPlayers)
        {
            bool exists;
            foreach (PlayerState player in newPlayers)
            {
                exists = false;
                foreach (PlayerObject currentPlayer in players)
                {
                    if(currentPlayer.Name == player.name)
                    {
                        currentPlayer.UpdateState(player);
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    players.Add(
                                new PlayerObject(player.name,
                                new ServerInput(),
                                ScreenManager.GraphicsDevice.Viewport,
                                new Vector2(player.posX, player.posY),
                                new Color(200, 0, 20),
                                playerTexture,
                                player.size));
                }
            }
            if(thisPlayer.Size > 1)
                Connection.Instance.SendMessage(thisPlayer.GetState());
        }

        public void PlayerRemovedEvent(string name)
        {
            Console.WriteLine("PlayerRemovedEvent");
            for(int i = 0; i < players.Count; i++)
            {
                if (players[i].Name == name)
                    players.RemoveAt(i);
            }

        }

        public void RemovePickupEvent(Vector2 pos)
        {
            for (int i = 0; i < worldObjects.Count; i++)
            {
                if (worldObjects[i].Rect.X == pos.X
                    && worldObjects[i].Rect.Y == pos.Y)
                    worldObjects.RemoveAt(i);
            }
        }

        /// <summary>
        /// Calls functions that require updating every cycle of the game loop.
        /// </summary>
        /// <param name="gameTime"> See: XNA Documentation</param>
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);

            thisPlayer.update(gameTime);
            updatePlayers(Connection.Instance.PlayerList);

            //Update World Objects
            for(int i = 0; i < worldObjects.Count(); i++)
            {
                worldObjects[i].update(gameTime);
                if (CollisionCheck.IGameObjectCollisionCheck(worldObjects[i], thisPlayer))
                {
                    thisPlayer.Grow();
                    Connection.Instance.SendMessage(
                        new RemovePickupMessage()
                        {
                            pos = new Vector2(worldObjects[i].Rect.X,
                                worldObjects[i].Rect.Y)
                        });
                    worldObjects.RemoveAt(i);
                }
            }

            //CollisionCheck(gameTime);
            foreach (PlayerObject player in players)
            {
                player.update(gameTime);
                if (CollisionCheck.IGameObjectCollisionCheck(player, thisPlayer))
                    thisPlayer.Colliding(player.Size);
            }
            
        }
        
        
        /// <summary>
        /// Draw the game on the canvas
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (IGameObject worldObjects in worldObjects)
                worldObjects.draw(gameTime, spriteBatch);
            foreach (PlayerObject player in players)
                player.draw(gameTime, spriteBatch);
            thisPlayer.draw(gameTime, spriteBatch);

            spriteBatch.End();
        }
        /// <summary>
        /// Called when transitioning from another game screen.
        /// </summary>
        /// <param name="message">
        /// Transition message from previous game screen. 
        /// </param>
        public override void Transition(string message)
        {
            playerName = message;
        }
    }
}
