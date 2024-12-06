using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port = 5053; // Port to listen for incoming UDP messages
    public bool startReceiving = true; // Flag to start or stop receiving
    public bool printToConsole = false; // Option to print received data to console
    public string data; // Variable to store received data

    public void Start()
    {
        // Initialize and start the receiving thread
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // Receive thread
    private void ReceiveData()
    {
        client = new UdpClient(port); // Create a new UdpClient listening on the specified port

        while (startReceiving)
        {
            try
            {
                // Create an endpoint to receive data from any IP address
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                // Receive the UDP message as a byte array
                byte[] dataByte = client.Receive(ref anyIP);
                // Convert the byte array to a UTF-8 string
                data = Encoding.UTF8.GetString(dataByte);

                // Optionally print the data to the console
                if (printToConsole)
                {
                    print(data);
                }
            }
            catch (Exception err)
            {
                // Log any exceptions
                print(err.ToString());
            }
        }
    }

    private void OnApplicationQuit()
    {
        // Cleanup when the application quits
        if (client != null)
        {
            client.Close();
        }
        if (receiveThread != null)
        {
            receiveThread.Abort();
        }
    }
}
