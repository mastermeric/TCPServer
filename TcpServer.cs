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
        private Boolean _isRunning;

        public TcpServer(int port)
        {
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();

            _isRunning = true;

            LoopClients();
        }

        private void LoopClients()
        {
            while (_isRunning)
            {
                // wait for client connection
                TcpClient newClient = _server.AcceptTcpClient();

                // client found.
                // create a thread to handle communication
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }

        private void HandleClient(object obj)
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

            String clientIP = IPAddress.Parse(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()).ToString();

            if(client.Connected) {
                Console.WriteLine(clientIP + " is Connected !");
            } else {
                Console.WriteLine(clientIP + " is DISCONNECTED !");
            }

            while (true)
            {
                //metot1 : reads from stream
                sData = sReader.ReadLine();
                if(sData != null)
                {
                    Console.WriteLine("Client > " + sData);
                }


                //metot2 :
                /*
                var Stream = client.GetStream();
                byte[] _bufferedData = new byte[client.ReceiveBufferSize]; // Initialize a new empty byte array with the data length.
                StringBuilder _stringData = new();

                while (Stream.DataAvailable) // Start converting bytes to string
                {
                    int _byteData = Stream.Read(_bufferedData, 0, _bufferedData.Length);
                    _stringData.AppendFormat("{0}", Encoding.ASCII.GetString(_bufferedData, 0, _byteData));
                }  // Until stream data is available

                if (_stringData != null) // Stream data is ready and converted to string Do some stuffs
                {
                    Console.WriteLine(_stringData);
                }
                */


                // to write something back.
                // sWriter.WriteLine("Meaningfull things here");
                // sWriter.Flush();
            }
        }
    }

}