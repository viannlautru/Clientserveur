using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.CodeDom.Compiler;
using System.Text.Json;
using Newtonsoft.Json;
// Client app is the one sending messages to a Server/listener.
// Both listener and client can send messages back and forth once a
// communication is established.
public class SocketClient
{
    public static int Main(String[] args)
    {
        Start();
        return 0;
    }



    public static void StartClient()
    {
        byte[] bytes = new byte[1024];



        try
        {
            // Connect to a Remote server
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            byte[] ip = new byte[4] { 10, 52, 203, 211 };
            IPAddress ipAddress = new(ip);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 9999);



            // Create a TCP/IP socket.
            Socket sender = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);



            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                // Connect to Remote EndPoint
                sender.Bind(remoteEP);



                sender.Connect(remoteEP);



                Console.WriteLine("Socket connected to {0}",
                sender.RemoteEndPoint.ToString());



                // Encode the data string into a byte array.

                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");



                // Send the data through the socket.
                int bytesSent = sender.Send(msg);



                // Receive the response from the remote device.
                int bytesRec = sender.Receive(bytes);
                Console.WriteLine("Echoed test = {0}",
                Encoding.ASCII.GetString(bytes, 0, bytesRec));



                // Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();



            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }



        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void Start()
    {
        int age;
        string nom;
        IPAddress ip = new(new byte[] { 127,0, 0, 1 });
        IPEndPoint endPoint = new(ip, 1234);
        Socket socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(endPoint);
        byte[] buffer = new byte[1024];
        int length = socket.Receive(buffer);
        string resp = Encoding.ASCII.GetString(buffer, 0, length);
        Console.WriteLine(resp);


        Console.WriteLine("age");
        age = int.Parse(Console.ReadLine());
        Console.WriteLine("nom");
        nom = Console.ReadLine();
        string host = Dns.GetHostName();
        string ipersonne = Dns.GetHostByName(host).AddressList[1].ToString();
        CommunicationBetween.Personne ok = new CommunicationBetween.Personne(age,nom,ipersonne);
        byte[] msg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(ok));
        int byteSent = socket.Send(msg);
        while (true)
        {
            var thread = new Thread(() =>
            {
                byte[] reponse = new byte[1024];
                int longue = socket.Receive(reponse);
                string rep = Encoding.ASCII.GetString(reponse, 0, longue);
                Console.WriteLine(rep);
            });
            thread.Start();
        }
        }
}