using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace lightarc
{
    public class SocketClient
    {
        private TcpClient client;
        private readonly ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

        public SocketClient(TcpClient client, ClientManager clientManager)
        {
            this.client = client;
            heart_pack = Encoding.ASCII.GetBytes("tike");
        }

        public void Start()
        {
            try
            {
                NetworkStream stream = client.GetStream();

                // Start separate threads for sending and receiving messages
                Thread sendThread = new Thread(() => SendMessages(stream));
                Thread receiveThread = new Thread(() => ReceiveMessages(stream));

                sendThread.Start();
                receiveThread.Start();

                // Wait for both threads to finish
                sendThread.Join();
                receiveThread.Join();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                // Close the client connection
                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }

        public void EnqueueMessage(string message)
        {
            messageQueue.Enqueue(message);
        }

        public void EnqueueMessage(byte[] message)
        {
            //messageQueue.Enqueue(Encoding.ASCII.GetString(message));
        }

        private byte[] heart_pack;
        private void SendMessages(NetworkStream stream)
        {
            try
            {
                while (true)
                {
                    if (messageQueue.TryDequeue(out string message))
                    {
                        Console.WriteLine("Send:" + message);
                        byte[] data = Encoding.ASCII.GetBytes(message);
                        // Send message to the server
                        stream.Write(data, 0, data.Length);
                    }
                    else
                    {
                        //stream.Write(heart_pack, 0, heart_pack.Length);
                        // Sleep briefly to avoid tight loop
                        Thread.Sleep(500);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private void ReceiveMessages(NetworkStream stream)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ProcessReceivedData(buffer, bytesRead);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
            }
        }

        private ByteArray receivedData = new ByteArray();
        private void ProcessReceivedData(byte[] buffer, int bytesRead)
        {
            // Append received data to the client's buffer
            receivedData.Append(buffer, bytesRead);

            // Process complete messages
            while (receivedData.Position + sizeof(uint) * 2 <= receivedData.LeftCount)
            {
                // Check if the buffer has enough data for message length and message ID
                uint messageLength = receivedData.PeekUint();
                //uint messageID = receivedData.ReadUint();

                if (messageLength+4 <= receivedData.LeftCount)
                {
                    receivedData.Seek(4);
                    uint messageID = receivedData.ReadUint();
                    // Extract complete message content
                    byte[] messageContent = receivedData.ReadBytes((int)messageLength);

                    // Process the message here
                    Console.WriteLine($"Received complete message (ID: {messageID}, Length: {messageLength}): {BitConverter.ToString(messageContent)}");
                }
                else
                {
                    // If there's not enough data for a complete message, break the loop
                    break;
                }
            }
        }

    }
}
