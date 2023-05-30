using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public static class Program33
    {
      public static void Main33(string[] args)
     {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 6666);
        tcpListener.Start();
        while (true)
        {
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            ThreadPool.QueueUserWorkItem(ThreadProc, tcpClient);
        }
     }

    private static void ThreadProc(object obj)
    {
        var tcpClient = (TcpClient)obj;    // Do your work here
        Task.Run(() => HandleClient(tcpClient));
    }

    private static void HandleClient(TcpClient tcpClient)
    {
        byte[] data = new byte[1024];
        NetworkStream ns = tcpClient.GetStream();
        int recv = ns.Read(data, 0, data.Length);

        //var byteCount = ns.ReadAsync(buffer, 0, buffer.Length);
        String  strData = Encoding.UTF8.GetString(data, 0, recv);
        Console.WriteLine(strData);
    }
    }
}