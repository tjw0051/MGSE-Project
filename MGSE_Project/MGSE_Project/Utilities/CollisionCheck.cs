using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSE_Project.Utilities
{
    public static class CollisionCheck
    {
        public static bool IGameObjectCollisionCheck(IGameObject object1, IGameObject object2)
        {
            if (object1.Rect.Intersects(object2.Rect))
                return true;
            else
                return false;
        }
    }
}
