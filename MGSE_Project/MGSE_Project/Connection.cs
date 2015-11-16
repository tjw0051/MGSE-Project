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
        int port;
        IPAddress ipAddress;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;
        DataContractJsonSerializer serializer;
        JavaScriptSerializer jsSerializer;

        Thread readThread;

        public Connection()
        {
            tcpClient = new TcpClient();
            serializer = new DataContractJsonSerializer(typeof(PlayerData));
            jsSerializer = new JavaScriptSerializer();
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
        
        public int sendInit(PlayerObject gameObject)
        {
            Console.WriteLine("Sending Init");
            if (gameObject != null)
            {
                PlayerIn data = new PlayerIn()
                {
                    name = gameObject.Name,
                    //size = gameObject.Score,
                    //posX = (int)gameObject.Pos.X,
                    //posY = (int)gameObject.Pos.Y,
                };
                try
                {
                    //serializer.WriteObject(stream, data);
                    string json = jsSerializer.Serialize(data);
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                    byte[] size = BitConverter.GetBytes(jsonBytes.Length);
                    byte[] message = new byte[jsonBytes.Length + 4];
                    Console.WriteLine("messageL: " + message.Length + " jsonL: " + jsonBytes.Length + " sizeL: " + size.Length);
                    //Message:
                    //Bytes     Data
                    //-----------------------------
                    //0-3       Message Length
                    //4+        Message
                    Console.WriteLine("Test1");
                    System.Buffer.BlockCopy(size, 0, message, 0, size.Length);
                    Console.WriteLine("Test2");
                    System.Buffer.BlockCopy(jsonBytes, 0, message, 4, jsonBytes.Length);
                    Console.WriteLine("Test3");
                    Console.WriteLine("Json: " + json + "\n jsonSize: " + jsonBytes.Length + "\n MessageLength: " + message.Length);
                    stream.Write(message, 0, message.Length);
                    Console.WriteLine("Message Sent");
                }
                catch(Exception e)
                {
                    return 1;
                    Console.WriteLine("Init Exception: " + e.Message);
                }
            }
            readThread = new Thread(new ThreadStart(ConnectionThread));
            readThread.Start();

            return 0;
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
                            string jsonString = Encoding.UTF8.GetString(jsonData);
                            Console.WriteLine("Data: " + Encoding.UTF8.GetString(jsonData));
                            PlayerIn newPlayer = jsSerializer.Deserialize<PlayerIn>(jsonString);
                            Console.WriteLine("Player Name: " + newPlayer.name);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Init Exception: " + e.Message);
                        }
                    }
                    
                    //stream.Read(data, 0, 4096);
                    //string read = Encoding.UTF8.GetString(data);
                    
                    //Console.WriteLine(read);
                    Console.WriteLine("...");
                    /*
                    try
                    {
                        byte[] data = new byte[4096];
                        int bytesRead = 0;
                        bytesRead = stream.Read(data, 0, 4096);
                        Console.WriteLine(Encoding.ASCII.GetString(data));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error reading from stream");
                    }
                    */
                    /*
                    try
                    {
                        //PlayerData newPlayer = (PlayerData)serializer.ReadObject(stream);
                        
                        //PlayerIn newPlayer = jsSerializer.Deserialize<PlayerIn>(read);//new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<PlayerIn>(read);
                        Console.WriteLine("New player recieved.");
                        //Console.WriteLine("Player Name: " + newPlayer.name);
                    }
                    catch (Exception e) //SerializationException
                    {
                        //Console.WriteLine("error thrown while deserializing object. Error: " + e.Message);

                    } */
                }
            }
            
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
        private byte[] read()
        {
            try
            {
                byte[] data = new byte[4096];
                int bytesRead = 0;
                bytesRead = stream.Read(data, 0, 4096);

                return data;
            }
            catch(Exception e)
            {
                Console.WriteLine("Error reading from stream");
                return null;
            }
        }
        public int SendMovement(Vector2 pos)
        {
            //Send position, vel, player size to server
            return 0;
        }
        private MemoryStream JsonToStream(object obj)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            serializer.WriteObject(stream, obj);

            return stream;
        }
        //Recieve input somewhere
    }

    public class PlayerIn
    {
        public string name;
        //public int size;
        //public int posX;
        //public int posY;
    }

}


[DataContract]
internal class PlayerData
{
    [DataMember]
    internal string name;
    /*
    [DataMember]
    internal int size;

    [DataMember]
    internal int posX;

    [DataMember]
    internal int posY;
    */
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
