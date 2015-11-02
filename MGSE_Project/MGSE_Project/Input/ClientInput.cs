using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MGSE_Project
{
    class ClientInput : IInputDevice
    {
        KeyboardState keyboardState;

        Vector2 axis;
        public Vector2 Axis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = value;
            }
        }

        private float x;
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        private float y;
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public ClientInput()
        {
            axis = new Vector2(0, 0);
        }

        public void update()
        {
            //TODO: Xbox controller input

            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
                axis.X = -1;
            else if (keyboardState.IsKeyDown(Keys.Right))
                axis.X = 1;
            else if (keyboardState.IsKeyDown(Keys.Up))
                axis.Y = -1;
            else if (keyboardState.IsKeyDown(Keys.Down))
                axis.Y = 1;
            else
            {
                axis.X = 0;
                axis.Y = 0;
            }
        }
    }
}
