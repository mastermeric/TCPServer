using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPServer;
class Program
{

    static async Task Main(string[] args)
    {
        try
        {

            if (args is null)
            {
                Console.WriteLine("ERROR at Args!");
            }

            Console.WriteLine("SERVER Started.. OK..");

            //metot1
            TcpServer server = new TcpServer(6666);
            //Console.ReadLine();
            ManualResetEvent _quitEvent = new ManualResetEvent(false);
            _quitEvent.WaitOne();

            // _ = Task.Factory.StartNew(() =>
            // {
            //     TcpServer server = new TcpServer(6666);
            //     Console.ReadLine();
            // }, TaskCreationOptions.LongRunning);

            // while(true)
            // {
            //     TcpServer server = new TcpServer(6666);
            //     Console.ReadLine();
            // }
            // ThreadStart ts = new ThreadStart(StartOp);
            // Thread th = new Thread(ts);
            // th.Start();


            //metot2
            // await new Program().StartListener();
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR at Args!");
            Console.WriteLine(ex.Message);
        }
    }



    public static byte[] ConvertHexStringToByteArray(string hexString)
    {
        if (hexString.Length % 2 != 0)
        {
            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
        }

        byte[] data = new byte[hexString.Length / 2];
        for (int index = 0; index < data.Length; index++)
        {
            string byteValue = hexString.Substring(index * 2, 2);
            data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return data;
    }
}
