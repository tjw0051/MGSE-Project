using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MGSE_Project
{
    class InputManager
    {
        KeyboardState keyboardState;
        MouseState mouseState;
        GamePadState gamePadState;


        public InputManager()
        {

        }
        public bool isKeyDown(Keys key)
        {
            keyboardState = Keyboard.GetState();
            if(keyboardState.IsKeyDown(key))
                return true;
            return false;
        }
        public Vector2 mousePos()
        {
            updateMouseState();
            return new Vector2(mouseState.X, mouseState.Y);
        }
        public bool getMouse1Click()
        {
            updateMouseState();
            return false;
        }
        private void updateMouseState()
        {
            mouseState = Mouse.GetState();
        }
        private void updateKeyboardState()
        {

        }
    }
}
