using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSE_Project
{
    interface IMessage { };

    //TODO: Store colour
    public class PlayerState : IMessage
    {
        public string type = "PlayerState";
        public string name;
        public int size;
        public int posX;
        public int posY;
        public int velX;
        public int velY;
    }

    public class PickupListMessage : IMessage
    {
        public string type = "PickupList";
        public Vector2[] pos;
        //public string[] pickupXPos;
        //public string[] pickupYPos;
    }
    public class RemovePickupMessage : IMessage
    {
        public string type = "RemovePickup";
        public Vector2 pos;
    }
    public class ServerNameMessage : IMessage
    {
        public string type = "ServerName";
        public string name;
    }
    public class RemovePlayerMessage : IMessage
    {
        public string type = "RemovePlayer";
        public string name;
    }
    public class SessionChangeMessage : IMessage
    {
        public string type = "SessionChange";
        public string message;
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
