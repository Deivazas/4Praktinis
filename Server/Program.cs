using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

class Server
{
    static void Main()
    {
        try
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            server.Start();

            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                byte[] dataBytes = new byte[4096];
                int bytesRead = stream.Read(dataBytes, 0, dataBytes.Length);
                string receivedData = Encoding.ASCII.GetString(dataBytes, 0, bytesRead);

                string[] parts = receivedData.Split('|');
                string publicKey = parts[0];
                string message = parts[1];
                string signature = parts[2];

                stream.Close();
                client.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}

