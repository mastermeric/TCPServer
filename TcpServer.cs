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
                try
                {
                // wait for client connection
                TcpClient newClient = await _server.AcceptTcpClientAsync();

                string _clientIP = IPAddress.Parse(((IPEndPoint)newClient.Client.RemoteEndPoint).Address.ToString()).ToString();

                Console.WriteLine(newClient.Client.Handle.ToString() +  " > " + _clientIP + " Connected !");

                // client found.. create a thread to handle communication
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("ERROR at LoopClients > " + ex.Message);
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

                //metot2 : read string from bytes
                // Byte[] data = new Byte[256];
                // NetworkStream stream = client.GetStream();
                // Int32 bytes = stream.Read(data, 0, data.Length);
                // sData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                // if(bytes > 0)
                // {
                //     Console.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " > "+ sData);
                // } else {
                //     Console.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " Is Disconnected !");
                //     break;
                // }

                //Yontem3 : Read Byte arrays..
                Byte[] data = new Byte[256];
                NetworkStream stream = client.GetStream();
                Int32 bytes = stream.Read(data, 0, data.Length);

                Byte[] dataReal = new Byte[bytes];
                Array.Copy(data,0,dataReal,0,bytes);

                string strOkunurVeri = System.Text.Encoding.ASCII.GetString(dataReal);

                sData = BitConverter.ToString(dataReal).Replace("-"," ");

                if(bytes > 0)
                {
                    Console.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " > "+ sData);
                    Console.WriteLine("OkunurVeri > " + strOkunurVeri);
                } else {
                    Console.WriteLine(client.Client.Handle.ToString() +  " > " + _clientIP + " Is Disconnected !");
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