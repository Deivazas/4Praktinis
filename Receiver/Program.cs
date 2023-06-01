using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

class Receiver
{
    static void Main()
    {
        try
        {
            // Start listening on the server
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            server.Start();

            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                // Accept client connection
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                // Create a separate thread for receiving and processing data
                Thread receiveThread = new Thread(() =>
                {
                    try
                    {
                        // Receive data from the client
                        byte[] dataBytes = new byte[4096];
                        int bytesRead = stream.Read(dataBytes, 0, dataBytes.Length);
                        string receivedData = Encoding.ASCII.GetString(dataBytes, 0, bytesRead);

                        Console.WriteLine("Data received from client: " + receivedData);

                        // Split the received data into public key, message, and signature
                        string[] parts = receivedData.Split('|');
                        string publicKey = parts[0];
                        string message = parts[1];
                        string signature = parts[2];

                        // Verify the digital signature

                        Console.WriteLine("Received Data:");
                        Console.WriteLine("Public Key: " + publicKey);
                        Console.WriteLine("Message: " + message);
                        Console.WriteLine("Signature: " + signature);
                    }
                    finally
                    {
                        // Close the connection
                        stream.Close();
                        client.Close();
                    }
                });

                // Start the receive thread
                receiveThread.Start();

                // Allow the receive thread to start before continuing
                Thread.Sleep(100);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static bool VerifyDigitalSignature(string message, string signature, string publicKey)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        byte[] signatureBytes = Convert.FromBase64String(signature);

        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            // Import the public key
            rsa.FromXmlString(publicKey);

            // Compute the hash of the message
            byte[] hash = new SHA256Managed().ComputeHash(messageBytes);

            // Verify the digital signature
            bool isSignatureValid = rsa.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA256"), signatureBytes);

            return isSignatureValid;
        }
    }
}
