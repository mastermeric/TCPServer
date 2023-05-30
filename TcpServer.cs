using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class TcpServer
    {
        private TcpListener _server;

        public TcpServer(int port)
        {
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();


            LoopClients();
        }

        private async void LoopClients()
        {
            while (true)
            {
Dene:
                try
                {                
                // wait for client connection
                TcpClient newClient = _server.AcceptTcpClient();

                // client found.. create a thread to handle communication
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("ERROR at LoopClients > " + ex.Message);
                    goto Dene;
                }
            }
        }

        private void HandleClient(object obj)
        {
            String _clientIP = "";
            try
            {
                // retrieve client from parameter passed to thread
            TcpClient client = (TcpClient)obj;

            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            // you could use the NetworkStream to read and write,
            // but there is no forcing flush, even when requested

            //Boolean bClientConnected = true;
            String sData = null;

            _clientIP = IPAddress.Parse(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()).ToString();

            if(client.Connected) {
                Console.WriteLine(_clientIP + " is Connected !");
            } else {
                Console.WriteLine(_clientIP + " is DISCONNECTED !");
            }

            while (true)
            {
                //metot1 : reads from stream
                // sData = sReader.ReadLine();
                // if(sData != null)
                // {
                //     Console.WriteLine(_clientIP +" > " + sData);
                // } else {
                //     break;
                // }

                Byte[] data = new Byte[256];
                NetworkStream stream = client.GetStream();
                Int32 bytes = stream.Read(data, 0, data.Length);
                sData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                if(sData != null)
                {
                    Console.WriteLine(_clientIP +" > " + sData);
                } else {
                    break;
                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(_clientIP + " DISCONNECTED ! ");
            }
        }
    }

}