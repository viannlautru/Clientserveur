using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

public class Client
{
    public static CommunicationBetween.Personne personne1 = new(20, "Vianney");
    public static CommunicationBetween.Personne personne2 = new(24, "Fakri");
    public static CommunicationBetween.Personne personne3 = new(21, "Mathias");

    public static List<CommunicationBetween.Personne> personnes = new();
    


    public static void StartClient()
    {
        personnes.Add(personne1);
        personnes.Add(personne2);
        personnes.Add(personne3);
        
        // Data buffer for incoming data.  
        byte[] bytes = new byte[1024];

        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP  socket.  
            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                sender.Connect(remoteEP);
                // Exercice 2
                //CommunicationBetween.Voiture car = new("Volkswagen", "Golf 7R");
                //string json = JsonConvert.SerializeObject(car);

                // Exercice 3
                string json = JsonConvert.SerializeObject(personnes);

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes(json);

                // Send the data through the socket.  
                int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = sender.Receive(bytes);
                Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

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

    public static int Main(String[] args)
    {
        StartClient();
        return 0;
    }

}