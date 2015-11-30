using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSE_Project
{

    public class PlayerState
    {
        public string name;
        public int size;
        public int posX;
        public int posY;
        public int velX;
        public int velY;
    }

    public class NewPlayerMessage
    {
        public string clientName;
    }
    public class JsonMessage
    {
        public string type;
        public string json;
    }
}
