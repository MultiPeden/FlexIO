using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;

public class UDPScript : MonoBehaviour {

    private Text instruction;

    private String message;

    Thread receiveThread;
    UdpClient client;

    // -------------------------------------------------------------------------
    public void Start()
    {
        message = "";
        instruction = GetComponent<Text>();
        instruction.text = "started";
        ReceiveData();
    }

    // -------------------------------------------------------------------------
    private void ReceiveData()
    {
        int somePort = 11000;


            client = new UdpClient(somePort);
                try
                {
                    client.BeginReceive(new AsyncCallback(recv), null);
                }
                catch (Exception e)
                {
              
                    this.message = "Error: " + e.ToString();
                }
        }



    //CallBack
    private void recv(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);

        //Process codes
;
        this.message = Encoding.UTF8.GetString(received);
        client.BeginReceive(new AsyncCallback(recv), null);
    }

    // Update is called once per frame
    void Update()
    {

         // instruction.text = "ole " + UnityEngine.Random.Range(0, 10);
        instruction.text = " " + message;

    }

    void OnApplicationQuit()
    {
        if (client != null)
            client.Close();
    }

}
