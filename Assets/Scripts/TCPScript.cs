using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;


public class TCPScript : MonoBehaviour
{

    byte[] bytes;
    IPAddress ipAddress;
    IPEndPoint remoteEP;
        Socket sender;


    public void Start()
    {
        // Data buffer for incoming data.  
       bytes = new byte[1024];
        // Establish the remote endpoint for the socket.  
         ipAddress = IPAddress.Parse(GetLocalIPAddress());
         remoteEP = new IPEndPoint(ipAddress, 11000);


    }
    public void ResetMesh()
    {
        // Encode the data string into a byte array and send
        Send(Encoding.ASCII.GetBytes("resetMesh<EOF>"));
    }

    private void Send(byte[] msg) { 
        // Connect to a remote device.  
        try
        {


                // Create a TCP/IP  socket.  
                sender = new Socket(ipAddress.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);
            

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                sender.Connect(remoteEP);
                Debug.Log(String.Format("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString()));



                
                // Send the data through the socket.  
                int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = sender.Receive(bytes);
                Debug.Log(String.Format("IRTracker: {0}",
                    Encoding.ASCII.GetString(bytes, 0, bytesRec)));

                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (ArgumentNullException ane)
            {
                Debug.Log(String.Format("ArgumentNullException : {0}", ane.ToString()));
            }
            catch (SocketException se)
            {
                Debug.Log(String.Format("SocketException : {0}", se.ToString()));
            }
            catch (Exception e)
            {
                Debug.Log(String.Format("Unexpected exception : {0}", e.ToString()));
            }

        }
        catch (Exception e)
        {
            Debug.Log(String.Format(e.ToString()));
        }
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}