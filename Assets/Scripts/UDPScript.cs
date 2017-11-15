using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.Diagnostics;

public class UDPScript : MonoBehaviour
{

    private Text instruction;

    public String message;
    private bool msgUpdated;

    Thread receiveThread;
    UdpClient client;
    private IRPoint[] irs;
    int portNumber = 11000;

    public bool autoStartKinect;
    Process kinectProcess;

    // -------------------------------------------------------------------------
    public void Start()
    {
        if (autoStartKinect)
        {
            kinectProcess = new Process();
            kinectProcess.StartInfo.FileName = "C:/Users/MultiPeden/Documents/GitHub/infraredKinectData/bin/AnyCPU/Debug/InfraredKinectData-WPF.exe";
          //  kinectProcess.StartInfo.Arguments = "-s";
            kinectProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            kinectProcess.Start();
        }




        message = "";
  //      instruction = GetComponent<Text>();
//        instruction.text = "started";
        ReceiveData();
        msgUpdated = false;

    }

    // -------------------------------------------------------------------------
    private void ReceiveData()
    {
       


        client = new UdpClient(portNumber);
        try
        {
            client.BeginReceive(new AsyncCallback(Recv), null);

        }
        catch (Exception e)
        {

            this.message = "Error: " + e.ToString();
        }
    }



    //CallBack
    private void Recv(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
        byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);

        //Process codes
        
        this.message = Encoding.UTF8.GetString(received);
        client.BeginReceive(new AsyncCallback(Recv), null);

        this.irs = JsonHelper.FromJson<IRPoint>(message);
        msgUpdated = true;

    }

    // Update is called once per frame
    void Update1()
    {

        if (msgUpdated)
        {
            msgUpdated = false;
          //  Debug.Log(message);



          //  IRPoint[] irs = JsonHelper.FromJson<IRPoint>(message);



            if (irs != null)
                instruction.text = "first coords x = : " + irs[0].x;
            //Debug.Log(" first coords x = : " + irs[0].x );

        }




    }

    void OnApplicationQuit()
    {
        if (client != null)
            client.Close();
        if (kinectProcess != null)
            kinectProcess.Kill();
    }


    public String getMessage()
    {
        return this.message;
    }

    public IRPoint[] GetIRs()
    {


        return this.irs;
    }


  

}