using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lightarc
{
    public class ClientManager
    {
        private readonly Dictionary<int, SocketClient> realmedClients = new Dictionary<int, SocketClient>();
        private readonly Dictionary<int, SocketClient> unrealmClients = new Dictionary<int, SocketClient>();

        public void AddClient(TcpClient tcpClient)
        {
            var client = new SocketClient(tcpClient, this);

            // Add the client to the unrealm clients dictionary initially
            unrealmClients[tcpClient.GetHashCode()] = client;

            // Start handling the client in a separate thread
            Thread clientThread = new Thread(client.Start);
            clientThread.Start();
        }

        public void MoveToRealmed(SocketClient client)
        {
            unrealmClients.Remove(client.GetHashCode());
            realmedClients[client.GetHashCode()] = client;
        }

        public void RemoveClient(SocketClient client)
        {
            unrealmClients.Remove(client.GetHashCode());
            realmedClients.Remove(client.GetHashCode());
        }

        public void BroadcastMessage(byte[] message)
        {
            foreach (var client in realmedClients.Values)
            {
                client.EnqueueMessage(message);
            }
        }
    }
}
