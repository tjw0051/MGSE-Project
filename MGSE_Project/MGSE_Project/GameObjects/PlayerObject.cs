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
    /// Defines the players in the game.
    /// </summary>
    public class PlayerObject : IGameObject
    {
        string name = "";
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                Name = value;
            }
        }
        int score;
        public int Score
        {
            get
            {
                return score;
            }
        }
        Vector2 center;
        public Vector2 Center
        {
            get
            {
                return new Vector2(Rect.X + size/2, Rect.Y + size/2);
            }
        }
        int size;
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }
        Vector2 pos;
        public Vector2 Pos
        {
            get
            {
                pos.X = Rect.X;
                pos.Y = Rect.Y;
                return pos;
            }
        }
        public Rectangle Rect
        {
            get
            {
                return rect;
            }
            set
            {
                rect = value;
            }
        }
        
        Vector2 velocity;
        
        Color color;
        Texture2D texture;
        Rectangle rect;
        SpriteFont font;
        Viewport viewport;

        PlayerState currentState;

        IInputDevice inputDevice;

        public PlayerObject() { }

        public PlayerObject(string name, IInputDevice inputDevice, Viewport viewport,
            Vector2 startPosition, Color color, Texture2D texture, int size)
        {
            this.viewport = viewport;
            this.name = name;
            this.inputDevice = inputDevice;
            //pos = startPosition;
            this.color = color;
            this.texture = texture;
            this.size = size;
            velocity = new Vector2(0, 0);
            rect = new Rectangle((int)startPosition.X, (int)startPosition.Y,
                size, size);
            currentState = new PlayerState()
            {
                name = this.name,
                size = this.size,
                posX = (int)this.Rect.X,
                posY = (int)this.Rect.Y,
                velX = (int)this.velocity.X,
                velY = (int)this.velocity.Y
            };
        }

        public void loadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("SpriteFont1");
        }

        public void updatePosition(int x, int y)
        {
            rect.X = x;
            rect.Y = y;
        }

        public PlayerState GetState()
        {
            currentState.size = this.size;
            currentState.posX = (int)this.Rect.X;
            currentState.posY = (int)this.Rect.Y;
            currentState.velX = (int)this.velocity.X;
            currentState.velY = (int)this.velocity.Y;
            return currentState;
            /*
            return new PlayerState
            {
                type = "PlayerState",
                name = this.name,
                size = this.size,
                posX = (int)this.pos.X,
                posY = (int)this.pos.Y,
                velX = (int)this.velocity.X,
                velY = (int)this.velocity.Y
            }; */
        }
        public void UpdateState(PlayerState state)
        {
            this.size = state.size;
            this.rect.X = state.posX;
            this.rect.Y = state.posY;
            this.velocity.X = state.velX;
            this.velocity.Y = state.velY;
        }
        public void Shrink()
        {
            size--;
        }
        public void Grow()
        {
            size++;
        }
        /// <summary>
        /// Triggered when colliding with another player.
        /// </summary>
        /// <param name="size">Size of opponent.</param>
        public void Colliding(int size)
        {
            if (Size > size)
                Grow();
            if (Size < size)
                Shrink();
        }
        /// <summary>
        /// Check for collising with world boundaries
        /// </summary>
        public void WorldCollisionCheck()
        {
            if (Rect.Left < 0)
                rect.X = 0;
            if (Rect.Right > viewport.Width)
                rect.X = viewport.Width - Rect.Width;
            if (Rect.Top < 0)
                rect.Y = 0;
            if (Rect.Bottom > viewport.Height)
                rect.Y = viewport.Height - Rect.Height;
        }

        public void update(GameTime gameTime)
        {
            inputDevice.update();
            velocity = inputDevice.Axis * 0.3f;
            
            rect.X += (int) (velocity.X * gameTime.ElapsedGameTime.Milliseconds);
            rect.Y += (int) (velocity.Y * gameTime.ElapsedGameTime.Milliseconds);
            
            rect.Width = size;
            rect.Height = size;

            if (size <= 1)
                Connection.Instance.SendMessage(
                    new RemovePlayerMessage()
                    {
                        name = Name
                    });
                //Death Condition - Send RemovePlayer message to server.

            WorldCollisionCheck();
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, color);
            //Console.WriteLine("Drawing " + Name + " at " + rect.X + " , " 
            //    + rect.Y + " , " + rect.Width + " , " + rect.Height);

            //Vector2 fontCenter = font.MeasureString(name) / 2;
            /*
            spriteBatch.DrawString(font, name,
                new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2),
                Color.Black, 0, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            */
        }
    }
}
