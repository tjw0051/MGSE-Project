using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSE_Project
{
    interface IMessage { };
    public class PlayerState : IMessage
    {
        public string name;
        public int size;
        public int posX;
        public int posY;
        public int velX;
        public int velY;
    }

    public class NewPlayerMessage : IMessage
    {
        public string clientName;
    }

    public class JsonMessage : IMessage
    {
        public string type;
        public string json;
    }

    public class ServerMessage : IMessage
    {
        public string type;
        public string command;
        public string message;
    }

    public class PlayerList : IMessage
    {
        public string type = "PlayerList";
        public string[] players;
    }

    public static class MessageBuilder
    {
        public static ServerMessage ServerMessageBuilder(string command, string message)
        {
            return new ServerMessage
            {
                type = "ServerMessage",
                command = command,
                message = message
            };

        }
        //public static IMessage MessageHandler()
    }
}
