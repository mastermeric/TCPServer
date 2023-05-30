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

            //metot2
            // await new Program().StartListener();
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR at Args!");
            Console.WriteLine(ex.Message);
        }
    }
}
