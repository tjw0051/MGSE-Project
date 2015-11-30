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
    class PlayerObject : IGameObject
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
                return new Vector2(Pos.X + size/2, Pos.Y + size/2);
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
                return pos;
            }
        }
        
        Vector2 velocity;
        
        Color color;
        Texture2D texture;
        Rectangle rect;
        SpriteFont font;

        IInputDevice inputDevice;

        public PlayerObject() { }

        public PlayerObject(string name, IInputDevice inputDevice,
            Vector2 startPosition, Color color, Texture2D texture, int size)
        {
            this.name = name;
            this.inputDevice = inputDevice;
            pos = startPosition;
            this.color = color;
            this.texture = texture;
            this.size = size;
            rect = new Rectangle((int)Pos.X, (int)Pos.Y, size, size);
        }

        public void loadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("SpriteFont1");
        }

        public void updatePosition(int x, int y)
        {
            pos.X = x;
            pos.Y = y;
        }
        public void Shrink()
        {
            size--;
        }
        public void Grow()
        {
            size++;
        }
        public void update(GameTime gameTime)
        {
            inputDevice.update();
            velocity = inputDevice.Axis * 0.3f;
            
            pos.X += velocity.X * gameTime.ElapsedGameTime.Milliseconds;
            pos.Y += velocity.Y * gameTime.ElapsedGameTime.Milliseconds;
            
            rect.X = (int)pos.X;
            rect.Y = (int)pos.Y;
            rect.Width = size;
            rect.Height = size;
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, color);
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
