using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

public class Server
{
    // Incoming data from the client.  
    public static string data = null;

    public static void StartListening()
    {

        
        // Data buffer for incoming data.  
        byte[] bytes = new Byte[1024];

        // Establish the local endpoint for the socket.  
        // Dns.GetHostName returns the name of the
        // host running the application.  
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[1];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and
        // listen for incoming connections.  
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Start listening for connections.  
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();
                data = null;

                // An incoming connection needs to be processed.  
                while (true)
                {
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.Length > 1)
                    {
                        break;
                    }
                }

                // Show the data on the console.  
                Console.WriteLine("Text received : {0}", data);

                // Echo the data back to the client.  
                byte[] msg = Encoding.ASCII.GetBytes(data);

                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    private static IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
    private static Socket listener;
    public static void Start()
    {
        IPEndPoint endPoint = new IPEndPoint(ip, 1234);
        listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(endPoint);
        listener.Listen(6);

        Console.WriteLine("Server start");
        while (true)
        {
            Socket client = listener.Accept();
            Console.WriteLine("Client connected");
            
            var thread = new Thread(() =>
            {
                SendOK(client);

            });
            thread.Start();
        }
    }
    
    public static void SendOK(Socket client)
    {
        byte[] msg = Encoding.ASCII.GetBytes("OK");
        client.Send(msg);
    }

    public static int Main(String[] args)
    {
        Start();
        return 0;
    }
}
