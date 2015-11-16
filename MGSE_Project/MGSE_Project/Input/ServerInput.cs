using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MGSE_Project
{
    class ServerInput : IInputDevice
    {
        private Vector2 axis;
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

        public float X
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public float Y
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        public ServerInput()
        {
            axis = new Vector2(0, 0);
        }

        public void update()
        {
        }
    }
}
