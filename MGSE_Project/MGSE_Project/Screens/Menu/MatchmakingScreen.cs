using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MGSE_Project
{
    /// <summary>
    /// Lobby screen to show other players connecting to the game.
    /// </summary>
    class MatchmakingScreen : GameScreen
    {
        const int pickupCount = 20;
        string transitionMessage;
        SpriteBatch spriteBatch;
        ContentManager content;

        MouseState mouseState, lastMouseState;
        
        Button joinButton, exitButton;
        TextBox textMessage;
        List<TextBox> playerNames;
        List<PlayerState> playerList;

        public MatchmakingScreen()
        {
            Connection.Instance.disconnectEvent += new Connection.DisconnectEvent(DisconnectEvent);
            Connection.Instance.sessionChangeEvent += new Connection.SessionChangeEvent(GameStartEvent);
        }
        /// <summary>
        /// Load initial game data and assets once.
        /// </summary>
        public override void LoadContent()
        {
            //TODO: Offload to initialize
            ScreenManager.Game.IsMouseVisible = true;
            spriteBatch = ScreenManager.SpriteBatch;
            content = new ContentManager(ScreenManager.Game.Services, "Content");
            //Load banner Image

            //Server Name
            int screenWidth = this.ScreenManager.GraphicsDevice.Viewport.Width;
            textMessage = new TextBox("Server: " + Connection.Instance.ServerName,
                new Vector2( screenWidth/ 2 , 100),
                Color.Black);
            textMessage.LoadContent(spriteBatch, ScreenManager.GraphicsDevice,
                content);
            //Player Names
            playerNames = new List<TextBox>();
            playerList = new List<PlayerState>();
            
            for(int i = 0; i < 10; i++)             //TODO: Get number of players from server;
            {
                playerNames.Add(new TextBox("",
                    new Vector2(screenWidth / 2, 150 + (50* i)),
                    Color.Gray));
                playerNames.ElementAt(i).LoadContent(spriteBatch, 
                    ScreenManager.GraphicsDevice, content);
            }
            //Load Login Button
            joinButton = new Button("Join", new Vector2(390, 650), Color.Aqua);
            joinButton.LoadContent(spriteBatch, ScreenManager.GraphicsDevice,
                content);
            exitButton = new Button("Exit", new Vector2(510, 650), Color.PaleVioletRed);
            exitButton.LoadContent(spriteBatch, ScreenManager.GraphicsDevice,
                content);
            //Load Exit Button
        }
        
        public override void UnloadContent()
        {
            content.Unload();
        }

        /// <summary>
        /// Updates player data based on data recieved from the server.
        /// </summary>
        /// <param name="newPlayers">List of newly recieved player data from server</param>

        /// <summary>
        /// Calls functions that require updating every cycle of the game loop.
        /// </summary>
        /// <param name="gameTime"> See: XNA Documentation</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Process Mouse Interaction
            mouseState = Mouse.GetState();
            Rectangle mousePos = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            if (mouseState.LeftButton == ButtonState.Released
                && lastMouseState.LeftButton == ButtonState.Pressed)
            {
                if (mousePos.Intersects(joinButton.rect))
                    Join();
                else if (mousePos.Intersects(exitButton.rect))
                    exit();
            }
            // Update list of players
            if (Connection.Instance.playerNames.Length != 0)
            {
                for (int i = 0; i < Connection.Instance.playerNames.Length; i++)
                {
                    if (i < playerNames.Count)
                        playerNames.ElementAt(i).text = Connection.Instance.playerNames[i];
                }
            }
            Connection.Instance.SendMessage(MessageBuilder.ServerMessageBuilder("PlayerList", ""));

            foreach (TextBox playerName in playerNames)
            {
                playerName.update(gameTime, mousePos);
            }
            joinButton.update(gameTime, mousePos);
            exitButton.update(gameTime, mousePos);
            
            lastMouseState = Mouse.GetState();
        }

        public void DisconnectEvent(string message)
        {
            Console.WriteLine(message);
            Connection.Instance.Disconnect();
            ScreenManager.Transition(typeof(LoginScreen), message);
        }

        public void GameStartEvent(string message)
        {
            if (message == "Start")
                ScreenManager.Transition(typeof(Level_0), transitionMessage);
        }

        protected void Join()
        {
            Console.WriteLine("Join clicked");
            //Only start game if there are more than 1 players
            if (Connection.Instance.playerNames.Length > 1)
            {
                Vector2[] pickupList = CreatePickups(pickupCount);
                Connection.Instance.PickupList = pickupList;
                PickupListMessage pickupListMessage = new PickupListMessage() { type = "PickList" };
                pickupListMessage.pickupXPos = new string[pickupCount];
                pickupListMessage.pickupYPos = new string[pickupCount];
                for(int i = 0; i < pickupCount; i++)
                {
                    pickupListMessage.pickupXPos[i] = pickupList[i].X.ToString();
                    pickupListMessage.pickupYPos[i] = pickupList[i].Y.ToString();
                }
                Connection.Instance.SendMessage(pickupListMessage);
                /*
                Connection.Instance.SendMessage(
                    new PickupListMessage()
                    {

                    });
                    */
                Connection.Instance.SendMessage(
                    new SessionChangeMessage()
                    {
                        message = "Start"
                    });
                ScreenManager.Transition(typeof(Level_0), transitionMessage);
            }

        }
        protected Vector2[] CreatePickups(int amount)
        {
            Vector2[] pickups = new Vector2[amount];
            Random rand = new Random();
            for(int i = 0; i < amount; i++)
            {
                int x = rand.Next(0, ScreenManager.GraphicsDevice.Viewport.Width - 21);
                int y = rand.Next(0, ScreenManager.GraphicsDevice.Viewport.Height - 21);

                pickups[i] = new Vector2(x, y);
            }
            return pickups;
        }

        protected void exit() { Console.WriteLine("Exit clicked"); ScreenManager.Game.Exit(); }
        /// <summary>
        /// Draw the game on the canvas
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            joinButton.draw();
            exitButton.draw();
            textMessage.draw();
            foreach (TextBox playerName in playerNames)
            {
                playerName.draw();
            }
            spriteBatch.End();
            
        }
        public override void Transition(string message)
        {
            this.transitionMessage = message;
        }
    }
}
