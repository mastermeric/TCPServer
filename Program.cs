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


    //======= Yontem 2 ================================================================
    object _lock = new Object(); // sync lock
    List<Task> _connections = new List<Task>(); // pending connections

    private Task StartListener()
    {
        return Task.Run(async () =>
        {
            var tcpListener = TcpListener.Create(8000);
            tcpListener.Start();
            while (true)
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine("[Server] Client has connected");
                var task = StartHandleConnectionAsync(tcpClient);
                if (task.IsFaulted)
                    await task;
            }
        });
    }
    // Register and handle the connection
    private async Task StartHandleConnectionAsync(TcpClient tcpClient)
    {
        // start the new connection task
        var connectionTask = HandleConnectionAsync(tcpClient);

        // add it to the list of pending task
        lock (_lock)
        _connections.Add(connectionTask);

        // catch all errors of HandleConnectionAsync
        try
        {
            await connectionTask;
            // we may be on another thread after "await"
        }
        catch (Exception ex)
        {
            // log the error
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            // remove pending task
            lock (_lock)
                _connections.Remove(connectionTask);
        }
    }
    // Handle new connection
    private static Task HandleConnectionAsync(TcpClient tcpClient)
    {
        return Task.Run(async () =>
        {
            using (var networkStream = tcpClient.GetStream())
            {
                var buffer = new byte[4096];
                Console.WriteLine("[Server] Reading from client");
                var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                Console.WriteLine("[Server] Client wrote {0}", request);
                var serverResponseBytes = Encoding.UTF8.GetBytes("Hello from server");
                await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                Console.WriteLine("[Server] Response has been written");
            }
        });
    }
    //==================================================================================================
}
