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
        public List<PlayerIn> PlayerList { get; private set; }

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
            PlayerList = new List<PlayerIn>();

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
            SendUpdate(playerObject);
            readThread = new Thread(new ThreadStart(ConnectionThread));
            readThread.Start();
            readThread.IsBackground = true;
            return 0;
        }

        public void SendUpdate(PlayerObject playerObject)
        {
            if (playerObject != null && tcpClient.Connected)
            {
                PlayerIn data = new PlayerIn()
                {
                    name = playerObject.Name,
                    size = playerObject.Size,
                    posX = (int)playerObject.Pos.X,
                    posY = (int)playerObject.Pos.Y,
                };
                try
                {
                    //serializer.WriteObject(stream, data);
                    byte[] message = CreateMessage(jsSerializer.Serialize(data));
                    stream.Write(message, 0, message.Length);
                    Console.WriteLine("Message Sent");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Init Exception: " + e.Message);
                }
            }
        }
        
        private byte[] CreateMessage(string message)
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

                            byte[] jsonData = new byte[dataSize];
                            stream.Read(jsonData, 0, dataSize);
                            string jsonString = Encoding.UTF8.GetString(jsonData);
                            //Console.WriteLine("Data: " + Encoding.UTF8.GetString(jsonData));
                            PlayerIn newPlayer = jsSerializer.Deserialize<PlayerIn>(jsonString);
                            //Console.WriteLine("Player Name: " + newPlayer.name);
                            UpdatePlayerList(newPlayer);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Init Exception: " + e.Message);
                        }
                    }
                }
            }
        }

        private void UpdatePlayerList(PlayerIn newPlayer)
        {
            foreach(PlayerIn player in PlayerList)
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

    public class PlayerIn
    {
        public string name;
        public int size;
        public int posX;
        public int posY;
        public int velX;
        public int velY;
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
