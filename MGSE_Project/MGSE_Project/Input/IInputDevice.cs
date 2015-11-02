using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MGSE_Project
{
    interface IInputDevice
    {
        Vector2 Axis
        {
            get;
            set;
        }

        float X
        {
            get;
            set;
        }

        float Y
        {
            get;
            set;
        }
        void update();
    }
}
