using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

class Sender
{
    static void Main()
    {
        try
        {
            // Connect to the server
            TcpClient client = new TcpClient("127.0.0.1", 8888);
            NetworkStream stream = client.GetStream();

            // Generate RSA key pair
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Get the public and private key
                string publicKey = rsa.ToXmlString(false);
                string privateKey = rsa.ToXmlString(true);

                // Loop to send multiple messages
                while (true)
                {
                    Console.Write("Enter the message (or 'exit' to quit): ");
                    string input = Console.ReadLine();

                    if (input.ToLower() == "exit")
                        break;

                    string message = input;

                    Console.WriteLine("Message entered: " + message);

                    // Generate digital signature
                    string signature = GenerateDigitalSignature(message, privateKey);

                    // Prepare data to send
                    string dataToSend = publicKey + "|" + message + "|" + signature;

                    // Convert data to bytes and send to the server
                    byte[] dataBytes = Encoding.ASCII.GetBytes(dataToSend);
                    stream.Write(dataBytes, 0, dataBytes.Length);

                    Console.WriteLine("Data sent to server: " + dataToSend);
                }

                // Close the connection
                stream.Close();
                client.Close();

                Console.WriteLine("Connection closed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static string GenerateDigitalSignature(string message, string privateKey)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            // Import the private key
            rsa.FromXmlString(privateKey);

            // Compute the hash of the message
            byte[] hash = new SHA256Managed().ComputeHash(messageBytes);

            // Generate the digital signature
            byte[] signatureBytes = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));

            return Convert.ToBase64String(signatureBytes);
        }
    }
}
