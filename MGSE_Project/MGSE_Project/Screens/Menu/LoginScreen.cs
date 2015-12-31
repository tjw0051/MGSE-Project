using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Web.Script.Serialization;
using System.Threading.Tasks;

namespace MGSE_Project
{
    /// <summary>
    /// Login screen for players to input their name and go to the matchmaking screen.
    /// </summary>
    class LoginScreen: GameScreen
    {
        string message = "Enter your name below:";
        SpriteBatch spriteBatch;
        ContentManager content;

        MouseState mouseState, lastMouseState;
        
        Button loginButton, exitButton;
        InputBox inputBox;
        TextBox textMessage;

        public LoginScreen() { }
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

            //Load Instruction text (also error text)
            textMessage = new TextBox(message,
                new Vector2(this.ScreenManager.GraphicsDevice.Viewport.Width/2, 290), Color.Black);
            textMessage.LoadContent(spriteBatch, ScreenManager.GraphicsDevice,
                content);
            //Load Input box
            inputBox = new InputBox(new Vector2(390, 390), Color.Orange, 10);
            inputBox.LoadContent(spriteBatch, ScreenManager.GraphicsDevice, content);
            //Load Login Button
            loginButton = new Button("Login", new Vector2(390, 500), Color.Aqua);
            loginButton.LoadContent(spriteBatch, ScreenManager.GraphicsDevice,
                content);
            exitButton = new Button("Exit", new Vector2(510, 500), Color.PaleVioletRed);
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
                if (mousePos.Intersects(loginButton.rect))
                    login();
                else if (mousePos.Intersects(exitButton.rect))
                    exit();
            }
            inputBox.update(gameTime, mousePos);
            loginButton.update(gameTime, mousePos);
            exitButton.update(gameTime, mousePos);
            
            lastMouseState = Mouse.GetState();
        }
        protected void login()
        {
            Console.WriteLine("Login clicked");
            //Loging in player
            Connection connection = Connection.Instance;
            if (connection.ConnectToServer("127.0.0.1", 8888) == 0)
            {
                if (inputBox.text != "")
                {
                    textMessage.text = "Logging in...";
                    PlayerObject player = new PlayerObject(
                        inputBox.text,
                        null,
                        ScreenManager.GraphicsDevice.Viewport,
                        new Vector2(0, 0),
                        Color.Blue,
                        null,
                        50);
                    connection.loadedPlayer = new PlayerState() { name = inputBox.text };
                    //player.Name = inputBox.text;
                    Task task = new Task(connection.Initialize);
                    task.Start();
                    task.Wait();
                    ServerMessage joinMessage = MessageBuilder.ServerMessageBuilder("Join", player.Name);
                    connection.SendMessage(joinMessage);
                    connection.SendMessage(MessageBuilder.ServerMessageBuilder("PlayerList", ""));
                    /*
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    string testMessage = ser.Serialize(new PlayerList
                    {
                        type = "PlayerList",
                        players = new string[] { "Elmo", "Tom" }
                        });
                    Console.WriteLine(testMessage);
                    */

                    //connection.SendServerMessage("playerlist");
                    
                    //connection.SendMessage(player);
                    ScreenManager.Transition(typeof(MatchmakingScreen), inputBox.text);
                }
                else
                    textMessage.text = "Error: Name not valid.";

            }
            else
                textMessage.text = "Error: Unable to connect to server.";
            

            /*
            if (inputBox.text != "")
            {
                textMessage.text = "Logging in...";
                ScreenManager.Transition(typeof(MatchmakingScreen), inputBox.text);
                //Go to Matchmaiking
            }

            else
                textMessage.text = "Error: Name not valid";
            */
               
        }
        protected void exit() { Console.WriteLine("Exit clicked"); ScreenManager.Game.Exit(); }
        /// <summary>
        /// Draw the game on the canvas
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (spriteBatch == null)
                return;
            spriteBatch.Begin();
            loginButton.draw();
            exitButton.draw();
            inputBox.draw();
            textMessage.draw();
            spriteBatch.End();
        }
        public override void Transition(string message)
        {
            this.message = message;
        }
    }
}
