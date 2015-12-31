using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;

using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using System.Web.Script;
using System.Web.Script.Serialization;

namespace MGSE_Project
{
    /// <summary>
    /// Singleton object used to communicate with server via JSON.
    /// </summary>
    class Connection
    {
        private static Connection instance = null;

        private TcpClient tcpClient;
        private NetworkStream stream;
        JavaScriptSerializer jsSerializer;

        Thread readThread;
        private bool listenToServer;
        
        public PlayerState loadedPlayer { get; set; }
        public string ServerName { get; private set; }
        public List<PlayerState> PlayerList { get; private set; }
        public Vector2[] PickupList { get; set; }
        public string[] playerNames;

        //Singleton used for Connection, as client can only operate
        //1 connection at a time. Connection needs to be accessible
        //across game screens and methods.
        public static Connection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Connection();
                }
                return instance;
            }
        }

        public Connection()
        {
            playerNames = new string[] { "Empty" };
        }

        /*
        public Connection()
        {
            tcpClient = new TcpClient();
            jsSerializer = new JavaScriptSerializer();
            PlayerList = new List<PlayerIn>();
        }
        
        public int Connect(IPAddress ipAddress, int port)
        {
            int error = 0;
            try
            {
                tcpClient.Connect(ipAddress, port);
                stream = tcpClient.GetStream();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to connect to " +
                    ipAddress.ToString() + " at port " + port);
                Console.WriteLine("Attempting to reconnect...");
                error = 1;
            }


            return error;
        }
        */

        /// <summary>
        /// Attempts to Connect to the Server.
        /// </summary>
        /// <param name="ipAddress">IP Address of server</param>
        /// <param name="port">Port of Server</param>
        /// <returns>Returns error code.</returns>
        public int ConnectToServer(string ipAddress, int port)
        {
            int error = 0;
            tcpClient = new TcpClient();
            jsSerializer = new JavaScriptSerializer();
            PlayerList = new List<PlayerState>();

            try
            {
                tcpClient.Connect(IPAddress.Parse(ipAddress), port);
                stream = tcpClient.GetStream();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to connect to " +
                    ipAddress + " at port " + port);
                error = 1;
            }
            return error;
        }

        /// <summary>
        /// Initialize communication thread with server to listen for incoming
        /// messages.
        /// </summary>
        /// <param name="playerObject"></param>
        /// <returns></returns>
        public async void Initialize()
        {
            Console.WriteLine("Sending Init");
            listenToServer = true;
            readThread = new Thread(new ThreadStart(ConnectionThread));
            readThread.Start();
            readThread.IsBackground = true;
        }

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        /// <param name="message"> Message type to send. <see cref="IMessage"/>
        ///  </param>
        public void SendMessage(IMessage message)
        {
            try {
                string jsonMessage = jsSerializer.Serialize(message);
                //Console.WriteLine("Sending IMessage = " + jsonMessage);
                byte[] byteMessage = StringToBytes(jsonMessage);
                stream.Write(byteMessage, 0, byteMessage.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Sending Message. E: " + e.Message);
            }
        }
        /// <summary>
        /// Convert string to a byte array, prepended with array size in 
        /// 4 bytes.
        /// </summary>
        /// <param name="message"> Message to convert to bytes. </param>
        /// <returns></returns>
        private byte[] StringToBytes(string message)
        {
            if (message == null)
                throw new System.ArgumentException("CreateMessage() must have an input message");

            byte[] jsonBytes = Encoding.UTF8.GetBytes(message);
            byte[] sizeBytes = BitConverter.GetBytes(jsonBytes.Length);
            byte[] messageBytes = new byte[jsonBytes.Length + 4];
            Console.WriteLine("size: " + jsonBytes.Length);
            //Message:
            //Bytes     Data
            //-----------------------------
            //0-3       Message Length
            //4+        Message
            System.Buffer.BlockCopy(sizeBytes, 0, messageBytes, 0, sizeBytes.Length);
            System.Buffer.BlockCopy(jsonBytes, 0, messageBytes, 4, jsonBytes.Length);
            return messageBytes;
        }

        /// <summary>
        /// Thread to listen to incoming messages from the server.
        /// </summary>
        protected void ConnectionThread()
        {
            Console.WriteLine("Thread started");
            while(listenToServer)
            {
                if (stream.DataAvailable) //TODO: Delete this IF
                {
                    //Console.WriteLine("Data...");
                    while (stream.DataAvailable)
                    {
                        try
                        {
                            byte[] data = new byte[4];
                            stream.Read(data, 0, 4);
                            int dataSize = BitConverter.ToInt32(data, 0);
                            //Console.WriteLine("Data Size: " + dataSize);
                            byte[] jsonData = new byte[dataSize];
                            stream.Read(jsonData, 0, dataSize);
                            string jsonMessageString = Encoding.UTF8.GetString(jsonData);
                            //Console.WriteLine("JSON Message: " + jsonMessageString);
                            JsonMessage jsonMessageType = jsSerializer.Deserialize<JsonMessage>(jsonMessageString);
                            if (jsonMessageType.type == "PlayerState")//"MGSE_Project.PlayerState")
                            {
                                //Console.WriteLine("Playerstate received");
                                PlayerState playerState = jsSerializer.Deserialize<PlayerState>(jsonMessageString);
                                if (playerState.name == loadedPlayer.name)
                                    loadedPlayer = playerState;
                                //loadPlayerEvent(playerState);
                                else
                                    UpdatePlayerList(playerState);
                            }
                            if(jsonMessageType.type == "PlayerList")
                            {
                                //Console.WriteLine("PlayerList Recieved");
                                PlayerList players = jsSerializer.Deserialize<PlayerList>(jsonMessageString);

                                playerNames = players.players;
                            }
                            if (jsonMessageType.type == "PickList")
                            {
                                //Console.WriteLine("PlayerList Recieved");
                                PickupListMessage pickupList = jsSerializer.Deserialize<PickupListMessage>(jsonMessageString);

                                PickupList = new Vector2[pickupList.pickupXPos.Length];
                                for(int i = 0; i < PickupList.Length; i++)
                                {
                                    PickupList[i] = new Vector2(Int32.Parse(pickupList.pickupXPos[i]), Int32.Parse(pickupList.pickupYPos[i]));
                                }

                            }
                            if (jsonMessageType.type == "ServerName")
                            {
                                //Console.WriteLine("Servername Recieved");
                                ServerNameMessage serverNameMessage =
                                    jsSerializer.Deserialize<ServerNameMessage>(jsonMessageString);

                                ServerName = serverNameMessage.name;
                            }
                            if(jsonMessageType.type == "RemovePlayer")
                            {
                                Console.WriteLine("Removing... ");
                                RemovePlayerMessage removePlayerMessage =
                                    jsSerializer.Deserialize<RemovePlayerMessage>(jsonMessageString);
                                Console.WriteLine("Removing player: " + removePlayerMessage.name);
                                for(int i = 0; i < PlayerList.Count; i++)
                                {
                                    if(PlayerList[i].name == removePlayerMessage.name)
                                    {
                                        removePlayerEvent(removePlayerMessage.name);
                                        PlayerList.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            if(jsonMessageType.type == "SessionChange")
                            {
                                SessionChangeMessage sessionChangeMessage = 
                                    jsSerializer.Deserialize<SessionChangeMessage>(jsonMessageString);
                                sessionChangeEvent(sessionChangeMessage.message);
                            }
                            else
                            {
                                //Console.WriteLine("Type is not PlayerState");
                                stream.Flush();
                            }
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine("Error reading stream: " + e.Message);
                        }
                    }
                }
                if(tcpClient.Connected == false)
                {
                    disconnectEvent("Lost connection to Server");
                    listenToServer = false;
                }
            }
        }

        /// <summary>
        /// Triggered when the session has been changed (start game, end game)
        /// </summary>
        public event SessionChangeEvent sessionChangeEvent;
        public delegate void SessionChangeEvent(String message);
        /// <summary>
        /// Triggered when the server find the existing user on the database 
        /// and sends the state to the client.
        /// </summary>
        public event LoadPlayerEvent loadPlayerEvent;
        public delegate void LoadPlayerEvent(PlayerState playerState);

        /// <summary>
        /// Triggered when the client loses connection to the server.
        /// </summary>
        public event DisconnectEvent disconnectEvent;
        public delegate void DisconnectEvent(string errormessage);

        /// <summary>
        /// Triggered when a player has left the game, informing the client
        /// to remove the player from their playerlist.
        /// </summary>
        public event RemovePlayerEvent removePlayerEvent;
        public delegate void RemovePlayerEvent(string name);

        /// <summary>
        /// Update the list of networked players with updated states.
        /// </summary>
        /// <param name="newPlayer"></param>
        private void UpdatePlayerList(PlayerState newPlayer)
        {
            //Console.WriteLine("Updating playerlist for: " + newPlayer.name);
            foreach(PlayerState player in PlayerList)
            {
                if(player.name == newPlayer.name)
                {
                    player.posX = newPlayer.posX;
                    player.posY = newPlayer.posY;
                    player.size = newPlayer.size;
                    player.velX = newPlayer.velX;
                    player.velY = newPlayer.velY;
                    return;
                }
            }
            PlayerList.Add(newPlayer);
        }

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                listenToServer = false;
                tcpClient.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error disconnecting from server.");
            }
        }
    }
}

/*

    TODO:
    - Run Connection on seperate thread.
    - Recieve:
        - Create Player
        - Kill Player (recognises connection loss)
        - Update Player
    - Send
        - Connect
        - Disconnect
        - Update
            - Position
            - Velocity?
            - Size
*/
/* Talk to server:
                       -Send init
                       -send GameObject data
                       -send 'Ready' (appear to clients)
                       -move to running phase
                   */
