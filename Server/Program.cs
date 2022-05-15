using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server;

public class Server
{
    // Incoming data from the client.  
    public static string data = null;
    public static Dictionary<int, CommunicationBetween.Personne> personnes = new Dictionary<int, CommunicationBetween.Personne>();
    public static SqlConnection conn;
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
        
        byte[] bytes = new Byte[1024];
        Console.WriteLine("Server start");
        while (true)
        {
            Socket client = listener.Accept();
            Console.WriteLine("Client connected");

            var thread = new Thread(() =>
            {
                SendOK(client);
                byte[] buffer = new byte[1024];
                int length = client.Receive(buffer);
                string resp = Encoding.ASCII.GetString(buffer, 0, length);
                Console.WriteLine(resp);
                if (verifBDD(resp))
                {
                    SqlConnection conn = BDDServeur.GetDBConnection();
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    var jsonString = JsonSerializer.Deserialize<CommunicationBetween.Personne>(resp);
                    string select1 = "Select * From Users where Nom='" + jsonString.name +"'";
                    cmd.Connection = conn;
                    cmd.CommandText = select1;
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    SendInfo(client, "Ton nom : " + reader.GetString(1)+ " Ton Age : " + reader.GetInt32(2) + " et pour finire ton IP : " + reader.GetString(3));
                    reader.Close();
                    conn.Close();
                }
                else { 
                    Console.WriteLine("Création..");
                    insertPersonne(resp);
                }
                
            });
            thread.Start();
        }

        // Show the data on the console.  
        
    }
    public static bool verfi(string o)
    {
        bool correct = false;
        var jsonString = JsonSerializer.Deserialize<CommunicationBetween.Personne>(o);
        Dictionary<int, CommunicationBetween.Personne> ledico = Deserializer();
        foreach (var perso in ledico)
        {
           if(jsonString.ip == perso.Value.ip)
            {
                correct= true;
            }
        }
        if (correct) return true;
        else return false;
    }
    public static Dictionary<int, CommunicationBetween.Personne> Deserializer()
    {
        string fileName = @"doc.json";
        fileName = Path.GetFullPath(fileName).Replace(@"\bin\Debug\net6.0", "");
        string jsonString = File.ReadAllText(fileName);
        jsonString = jsonString.Replace("\\", "\"");
        if (jsonString != "") { 
            personnes = JsonSerializer.Deserialize<Dictionary<int,CommunicationBetween.Personne>>(jsonString)!;
        }
        return personnes;
    }
    public static bool insertPersonne(string o)
    {
        SqlConnection conn = BDDServeur.GetDBConnection();
        conn.Open();
        var jsonString = JsonSerializer.Deserialize<CommunicationBetween.Personne>(o);
        string sql = "Insert into Users (Nom, Age,IPadresse) values ('" + jsonString.name + "',"+ jsonString.age + ",'"+ jsonString.ip +"') ";
        try
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return true;
        }catch(Exception e)
        {
            conn.Close();
            return false;
        }
        conn.Close();
    }
    public static bool verifBDD(string o)
    {
        bool exist = false;
        var jsonString = JsonSerializer.Deserialize<CommunicationBetween.Personne>(o);
        SqlConnection conn = BDDServeur.GetDBConnection();
        conn.Open();
        SqlCommand cmd = new SqlCommand();

        string select1 = "Select count(*) From Users"; 
        cmd.Connection = conn;
        cmd.CommandText = select1;
        int count = (int)cmd.ExecuteScalar();
        if (count != 0)
        {
            string select2 = "Select * from Users";
            // Créez un objet Command.
            // Combinez l'objet Command avec Connection.
            cmd.CommandText = select2;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                    while (reader.Read())
                    {
                        string Nom = reader.GetString(1);
                        if (Nom == jsonString.name)
                        {
                            exist = true;
                        break;
                        }
                        else exist = false;
                    }
            }
        }
        if (exist)
        {
            return true;
        }
        else
            return false;
        conn.Close();
    }
  
    public static string Serialize(string o)
    {
        string fileName = @"doc.json";
        fileName = Path.GetFullPath(fileName).Replace(@"\bin\Debug\net6.0", "");
        Random aleatoire = new Random();
        int idalea = aleatoire.Next(1, 1300000);
        CommunicationBetween.Personne ok = JsonSerializer.Deserialize<CommunicationBetween.Personne>(o);
        personnes.Add(idalea, ok);
        string jsonString = JsonSerializer.Serialize(personnes);
        File.AppendAllText(fileName, jsonString);
        return File.ReadAllText(fileName);
    }
    public static void SendOK(Socket client)
    {
        byte[] msg = Encoding.ASCII.GetBytes("OK");
        client.Send(msg);
    } 
    public static void SendInfo(Socket client,string texte)
    {
        byte[] msg = Encoding.ASCII.GetBytes(texte);
        client.Send(msg);
    }

    public static int Main(String[] args)
    {
        Start();
        return 0;
    }
}
