using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MGSE_Project
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    public abstract class GameScreen
    {
        protected ContentManager contentManager;
        
        ScreenState screenState = ScreenState.Hidden;

        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        ScreenManager screenManager;
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        public virtual void Dispose(bool disposing) { }
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Transition(string message) { }
    }
}
