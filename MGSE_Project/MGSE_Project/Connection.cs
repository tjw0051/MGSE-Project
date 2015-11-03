using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

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

        public Connection()
        {
            tcpClient = new TcpClient();
        }
        public int Connect(IPAddress ipAddress, int port)
        {
            int error = 1;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    tcpClient.Connect(ipAddress, port);
                    /* Talk to server:
                        -Send init
                        -send GameObject data
                        -send 'Ready' (appear to clients)
                        -move to running phase
                    */
                    error = 0;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to connect to " + ipAddress.ToString() + " at port " + port);
                    Console.WriteLine("Attempting to reconnect...");
                    error = 1;
                }
            }
            return error;
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
                Console.WriteLine("Failed to disconnect from Server");
                return 1;
            }
            return 0;
        }
        public int SendInit(IGameObject gameObject)
        {
            //Send initial data to server
            return 0;
        }
        public int SendMovement()
        {
            //Send position, vel, player size to server
            return 0;
        }
        //Recieve input somewhere
    }
}
