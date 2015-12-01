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
    class Connection
    {
        private static Connection instance = null;

        private TcpClient tcpClient;
        private NetworkStream stream;
        JavaScriptSerializer jsSerializer;

        Thread readThread;

        //private List<PlayerIn> players;
        public List<PlayerState> PlayerList { get; private set; }

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

        public Connection() { }

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

        public int Initialize(PlayerObject playerObject)
        {
            Console.WriteLine("Sending Init");
            //SendUpdate(playerObject);
            readThread = new Thread(new ThreadStart(ConnectionThread));
            readThread.Start();
            readThread.IsBackground = true;
            return 0;
        }

        public void SendMessage(string messageType, object message)
        {
            Console.WriteLine("Sending message type: " + messageType);
            if(tcpClient.Connected)
            {
                JsonMessage json = new JsonMessage()
                {
                    type = messageType,
                    json = jsSerializer.Serialize(message)
                };
                string jsonMessage = jsSerializer.Serialize(json);

                //string jsonMessage = jsSerializer.Serialize(message);
                byte[] byteMessage = StringToBytes(jsonMessage);
                try {
                    stream.Write(byteMessage, 0, byteMessage.Length);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error writting to stream.");
                }
            }
        }
        public void SendServerMessage(string message)
        {
            Console.WriteLine("Sending Server Message");
            if (tcpClient.Connected)
            {
                JsonMessage json = new JsonMessage()
                {
                    type = "ServerMessage",
                    json = message
                };
                string jsonMessage = jsSerializer.Serialize(json);
                Console.WriteLine("ServerMessage = " + jsonMessage);
                //string jsonMessage = jsSerializer.Serialize(message);
                byte[] byteMessage = StringToBytes(jsonMessage);
                try
                {
                    stream.Write(byteMessage, 0, byteMessage.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error writting to stream.");
                }
            }
        }
        public void SendMessage(PlayerObject playerObject)
        {
            PlayerState data = new PlayerState()
            {
                name = playerObject.Name,
                size = playerObject.Size,
                posX = (int)playerObject.Pos.X,
                posY = (int)playerObject.Pos.Y,
            };
            SendMessage(data.ToString(), data);
        }

        public void SendUpdate(PlayerObject playerObject)
        {
            if (playerObject != null && tcpClient.Connected)
            {
                PlayerState data = new PlayerState()
                {
                    name = playerObject.Name,
                    size = playerObject.Size,
                    posX = (int)playerObject.Pos.X,
                    posY = (int)playerObject.Pos.Y,
                };
                try
                {
                    string jsonMessage = jsSerializer.Serialize(data);
                    Console.WriteLine("JSON = " + jsonMessage);
                    byte[] byteMessage = StringToBytes(jsonMessage);


                    stream.Write(byteMessage, 0, byteMessage.Length);
                    Console.WriteLine("Message Sent");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Init Exception: " + e.Message);
                }
            }
        }
        

        private byte[] StringToBytes(string message)
        {
            if (message == null)
                throw new System.ArgumentException("CreateMessage() must have an input message");

            byte[] jsonBytes = Encoding.UTF8.GetBytes(message);
            byte[] sizeBytes = BitConverter.GetBytes(jsonBytes.Length);
            byte[] messageBytes = new byte[jsonBytes.Length + 4];
            //Message:
            //Bytes     Data
            //-----------------------------
            //0-3       Message Length
            //4+        Message
            System.Buffer.BlockCopy(sizeBytes, 0, messageBytes, 0, sizeBytes.Length);
            System.Buffer.BlockCopy(jsonBytes, 0, messageBytes, 4, jsonBytes.Length);
            return messageBytes;
        }

        protected void ConnectionThread()
        {
            Console.WriteLine("Thread started");
            while(true)
            {
                if (stream.DataAvailable)
                {
                    Console.WriteLine("Stream not empty");
                    //byte[] data = new byte[4096];
                    
                    while(stream.DataAvailable)
                    {
                        try
                        {
                            byte[] data = new byte[4];
                            stream.Read(data, 0, 4);
                            int dataSize = BitConverter.ToInt32(data, 0);
                            Console.WriteLine("Data Size: " + dataSize);
                            byte[] jsonData = new byte[dataSize];
                            stream.Read(jsonData, 0, dataSize);
                            string jsonMessageString = Encoding.UTF8.GetString(jsonData);
                            Console.WriteLine("JSON Message: " + jsonMessageString);
                            JsonMessage jsonMessageType = jsSerializer.Deserialize<JsonMessage>(jsonMessageString);
                            if (jsonMessageType.type == "MGSE_Project.PlayerState")
                            {
                                PlayerState playerState = jsSerializer.Deserialize<PlayerState>(jsonMessageType.json);
                                Console.WriteLine("object is playerstate");
                                UpdatePlayerList(playerState);
                            }
                            else
                            {
                                Console.WriteLine("Type is not PlayerState");
                                stream.Flush();
                            }


                            /*
                            object deserializedObject = jsSerializer.DeserializeObject(jsonString);
                            if (deserializedObject as PlayerState != null)
                            {
                                Console.WriteLine("object is playerstate");
                                UpdatePlayerList((PlayerState)deserializedObject);
                            }
                            else if (deserializedObject as NewPlayerMessage != null)
                            {
                                Console.WriteLine("object is playerstate");
                            }
                            else
                                Console.WriteLine("Object cant be cast \n JSON = " + jsonString);
                                */
                                /*
                            Console.WriteLine("Data: " + Encoding.UTF8.GetString(jsonData));
                            PlayerState newPlayer = jsSerializer.Deserialize<PlayerState>(jsonMessageString);
                            Console.WriteLine("Player Name: " + newPlayer.name);
                            UpdatePlayerList(newPlayer);
                            */
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Init Exception: " + e.Message);
                        }
                    }
                }
            }
        }

        private void UpdatePlayerList(PlayerState newPlayer)
        {
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

        public int Disconnect()
        {
            try
            {
                //Send disconnect message to server
                tcpClient.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error disconnecting from server.");
                return 1;
            }
            return 0;
        }
        //Recieve input somewhere
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
