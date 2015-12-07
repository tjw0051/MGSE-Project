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
    class MatchmakingScreen : GameScreen
    {
        string message = "Enter your name below:";
        SpriteBatch spriteBatch;
        ContentManager content;

        MouseState mouseState, lastMouseState;

        enum MenuState
        {
            IDLE, INPUT, LOADING, ERROR
        };

        MenuState menuState;
        Button joinButton, exitButton;
        TextBox textMessage;
        List<TextBox> playerNames;
        List<PlayerState> playerList;

        public MatchmakingScreen()
        {
            ScreenState = ScreenState.Active;

            Connection.Instance.disconnectEvent += new Connection.DisconnectEvent(DisconnectEvent);

            //Handle error code here (return to main menu)

            //connection.SendInit(); //Send player data
            //Call method to recieve other player data and add to worldObjects to be drawn. Call in update method
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
            textMessage = new TextBox("Server: NAME",
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
            //Update Player List
            /*
            playerList = Connection.Instance.PlayerList;
            for (int i = 0; i < playerList.Count(); i++)             //TODO: Get number of players from server;
            {
                //Console.WriteLine("Player: " + playerList.ElementAt(i).name + " at: " + i);
                if(i < playerNames.Count)
                    playerNames.ElementAt(i).text = playerList.ElementAt(i).name;
            }
            */
            if (Connection.Instance.playerList.Length != 0)
            {
                for (int i = 0; i < Connection.Instance.playerList.Length; i++)
                {
                    if (i < playerNames.Count)
                        playerNames.ElementAt(i).text = Connection.Instance.playerList[i];
                }
            }
            Connection.Instance.SendMessage(MessageBuilder.ServerMessageBuilder("PlayerList", ""));

            foreach (TextBox playerName in playerNames)
            {
                playerName.update(gameTime, mousePos);
            }
            joinButton.update(gameTime, mousePos);
            exitButton.update(gameTime, mousePos);

            switch (menuState)
            {
                case MenuState.IDLE:
                    break;
                case MenuState.INPUT:
                    break;
                case MenuState.LOADING:
                    break;
                case MenuState.ERROR:
                    break;
            }

            lastMouseState = Mouse.GetState();
        }

        public void DisconnectEvent(string message)
        {
            Console.WriteLine(message);
            Connection.Instance.Disconnect();
            ScreenManager.Transition(typeof(LoginScreen), message);
        }

        protected void Join()
        {
            Console.WriteLine("Join clicked");
            ScreenManager.Transition(typeof(Level_0), "todo");

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

            switch (menuState)
            {
                case MenuState.INPUT:
                    break;
                case MenuState.LOADING:
                    break;
                case MenuState.ERROR:
                    break;
            }
        }
    }
}
