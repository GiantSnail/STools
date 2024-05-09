using lightarc;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        StartServer();
    }

    static void StartServer()
    {
        // Set the IP address and port for the server to listen on
        var ipAddress = System.Net.IPAddress.Parse("192.168.31.131");
        int port = 8888;

        // Create a TCP/IP socket
        var listener = new TcpListener(ipAddress, port);

        try
        {
            // Start listening for client connections
            listener.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                // Accept incoming client connection
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                // Handle client communication in a separate thread
                var socketClient = new SocketClient(client);
                Thread clientThread = new Thread(socketClient.Start);
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // Stop listening for new clients
            listener.Stop();
        }
    }
}
